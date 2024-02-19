﻿using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Generator;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using static System.StringComparer;
using CodeCompiler = ToSic.Sxc.Code.Internal.HotBuild.CodeCompiler;

namespace ToSic.Sxc.Dnn.Razor.Internal
{
    /// <summary>
    /// This class is responsible for managing the compilation of Razor templates using Roslyn.
    /// </summary>
    public class RoslynBuildManager : ServiceBase, IRoslynBuildManager
    {
        private static readonly ConcurrentDictionary<string, object> CompileAssemblyLocks = new(InvariantCultureIgnoreCase);

        private const string DefaultNamespace = "RazorHost";

        // TODO: THIS IS PROBABLY Wrong, but not important for now
        // It's wrong, because the web.config gives the default to be a very old 2sxc base class
        private const string FallbackBaseClass = "System.Web.WebPages.WebPageBase";

        private readonly AssemblyCacheManager _assemblyCacheManager;
        private readonly LazySvc<AppCodeLoader> _appCodeLoader;
        private readonly LazySvc<DependenciesLoader> _dependenciesLoader;
        private readonly AssemblyResolver _assemblyResolver;
        private readonly IReferencedAssembliesProvider _referencedAssembliesProvider;

        public RoslynBuildManager(AssemblyCacheManager assemblyCacheManager, LazySvc<AppCodeLoader> appCodeLoader, LazySvc<DependenciesLoader> dependenciesLoader, AssemblyResolver assemblyResolver, IReferencedAssembliesProvider referencedAssembliesProvider) : base("Dnn.RoslynBuildManager")
        {
            ConnectServices(
                _assemblyCacheManager = assemblyCacheManager,
                _appCodeLoader = appCodeLoader,
                _dependenciesLoader = dependenciesLoader,
                _assemblyResolver = assemblyResolver,
                _referencedAssembliesProvider = referencedAssembliesProvider
            );
        }

        /// <summary>
        /// Manage template compilations, cache the assembly and returns the generated type.
        /// </summary>
        /// <param name="codeFileInfo"></param>
        /// <param name="spec"></param>
        /// <returns>The generated type for razor cshtml.</returns>
        public Type GetCompiledType(CodeFileInfo codeFileInfo, HotBuildSpec spec)
            => GetCompiledAssembly(codeFileInfo, null, spec)?.MainType;

        public AssemblyResult GetCompiledAssembly(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};");

            var lockObject = CompileAssemblyLocks.GetOrAdd(codeFileInfo.FullPath, new object());

            var result = new TryLockTryDo(lockObject).Call(
                condition: () => AssemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath)?.MainType == null,
                generator: () => OnCacheMiss(codeFileInfo, className, spec),
                cacheOrDefault: AssemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath));

            return l.ReturnAsOk(result);

        }

        private AssemblyResult OnCacheMiss(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
        {
            var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};", timer: true);

            // Initialize the list of referenced assemblies with the default ones
            var referencedAssemblies = _referencedAssembliesProvider.Locations(codeFileInfo.RelativePath, spec);

            // Roslyn compiler need reference to location of dll, when dll is not in bin folder
            // get assembly - try to get from cache, otherwise compile
            //var codeAssembly = AppCodeLoader.TryGetAssemblyOfAppCodeFromCache(spec, Log)?.Assembly
            //                   ?? _appCodeLoader.Value.GetAppCodeAssemblyOrThrow(spec);
            var (codeAssembly, specOut) = _appCodeLoader.Value.TryGetOrFallback(spec);
            _assemblyResolver.AddAssembly(codeAssembly);

            var appCode = AssemblyCacheManager.TryGetAppCode(specOut);

            var appCodeAssembly = appCode.Result?.Assembly;
            if (appCodeAssembly != null)
            {
                var assemblyLocation = appCodeAssembly.Location;
                referencedAssemblies.Add(assemblyLocation);
                l.A($"Added reference to AppCode assembly: {assemblyLocation}");
            }

            // Compile the template
            var pathLowerCase = codeFileInfo.RelativePath.ToLowerInvariant();
            var isCshtml = pathLowerCase.EndsWith(CodeCompiler.CsHtmlFileExtension);
            if (isCshtml) className = GetSafeClassName(codeFileInfo.FullPath);
            l.A($"Compiling template. Class: {className}");

            var (generatedAssembly, errors) = isCshtml
                ? CompileTemplate(codeFileInfo.SourceCode, referencedAssemblies, className, DefaultNamespace, codeFileInfo.FullPath)
                : CompileCSharpCode(codeFileInfo.SourceCode, referencedAssemblies);
            if (generatedAssembly == null)
                throw l.Ex(new Exception(
                    $"Found {errors.Count} errors compiling Razor '{codeFileInfo.FullPath}' (length: {codeFileInfo.SourceCode.Length}, lines: {codeFileInfo.SourceCode.Split('\n').Length}): {ErrorMessagesFromCompileErrors(errors)}"));

            // Add the compiled assembly to the cache

            // Changed again: better to only monitor the current file
            // otherwise all caches keep getting flushed when any file changes
            // TODO: must also watch for global shared code changes

            var fileChangeMon = new HostFileChangeMonitor(new[] { codeFileInfo.FullPath });
            var sharedFolderChangeMon = appCode.Result == null ? null : new FolderChangeMonitor(appCode.Result.WatcherFolders);
            var changeMonitors = appCode.Result == null
                ? new ChangeMonitor[] { fileChangeMon }
                : [fileChangeMon, sharedFolderChangeMon];

            // directly attach a type to the cache
            var mainType = FindMainType(generatedAssembly, className, isCshtml);
            l.A($"Main type: {mainType}");

            var assemblyResult = new AssemblyResult(generatedAssembly, safeClassName: className, mainType: mainType);

            _assemblyCacheManager.Add(
                cacheKey: AssemblyCacheManager.KeyTemplate(codeFileInfo.FullPath),
                data: assemblyResult,
                duration: 3600,
                changeMonitor: changeMonitors,
                //appPaths: appPaths,
                updateCallback: null);

            return l.ReturnAsOk(assemblyResult);
        }

        private Type FindMainType(Assembly generatedAssembly, string className, bool isCshtml)
        {
            var l = Log.Fn<Type>($"{nameof(className)}: '{className}'; {nameof(isCshtml)}: {isCshtml}", timer: true);
            if (generatedAssembly == null) return l.ReturnAsError(null, "generatedAssembly is null, so type is null");

            var mainType = generatedAssembly.GetType(isCshtml ? $"{DefaultNamespace}.{className}" : className, false, true);
            if (mainType != null) return l.ReturnAsOk(mainType);

            l.A("can't find MainType in standard way, fallback #1 - search by classname, ignoring namespace");
            foreach (var mainTypeFallback1 in generatedAssembly.GetTypes())
                if (mainTypeFallback1.Name.Equals(className, StringComparison.OrdinalIgnoreCase))
                    return l.ReturnAsOk(mainTypeFallback1);

            l.A("can't find mainTypeFallback1, fallback #2 - just return first type (in most cases we have one only)");
            var mainTypeFallback2 = generatedAssembly.GetTypes().FirstOrDefault();
            return l.ReturnAsOk(mainTypeFallback2);
        }


        /// <summary>
        /// extract appRelativePath from relativePath
        /// </summary>
        /// <param name="relativePath">string "/Portals/site-id-or-name/2sxc/app-folder-name/etc..."</param>
        /// <returns>string "\\Portals\\site-id-or-name\\2sxc\\app-folder-name" or null</returns>
        private static string GetAppRelativePath(string relativePath)
        {
            // TODO: stv, this has to more generic because it is very 2sxc on DNN specific, only for default case of templates under /Portals/xxxx/ folder

            // validations
            var message = $"relativePath:'{relativePath}' is not in format '/Portals/site-id-or-name/2sxc/app-folder-name/etc...'";
            if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith("/Portals/"))
                throw new(message);

            var startPos = relativePath.IndexOf("/2sxc/", 10); // start from 10 to skip '/' before 'site-id-or-name'
            if (startPos < 0) throw new(message);

            // find position of 5th slash in relativePath 
            var pos = startPos + 6; // skipping first 4 slashes
            pos = relativePath.IndexOf('/', pos + 1);
            if (pos < 0) throw new(message);

            return relativePath.Substring(0, pos).Backslash();
        }

        private string GetSafeClassName(string templateFullPath)
        {
            if (!string.IsNullOrWhiteSpace(templateFullPath))
                return "RazorView" + GetSafeString(Path.GetFileNameWithoutExtension(templateFullPath));

            // Fallback class name with a unique identifier
            return "RazorView" + Guid.NewGuid().ToString("N");
        }

        private static string GetSafeString(string input)
        {
            var safeChars = input.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
            var safeString = new string(safeChars);

            // Ensure the class name starts with a letter or underscore
            if (!char.IsLetter(safeString.FirstOrDefault()) && safeString.FirstOrDefault() != '_')
                safeString = "_" + safeString;

            return safeString;
        }


        /// <summary>
        /// Compiles the template into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileTemplate(string template, List<string> referencedAssemblies, string className, string defaultNamespace, string sourceFileName)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"Template content length: {template.Length}");

            // Find the base class for the template
            var baseClass = FindBaseClass(template);
            l.A($"Base class: {baseClass}");

            // Create the Razor template engine host
            var engine = CreateHost(className, baseClass, defaultNamespace);

            // Generate C# code from Razor template
            var lTimer = Log.Fn("Generate Code", timer: true);
            using var reader = new StringReader(template);
            var razorResults = engine.GenerateCode(reader, className, defaultNamespace, sourceFileName);
            lTimer.Done();

            lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = new CompilerParameters([.. referencedAssemblies])
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,

                CompilerOptions = $"{DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };
            lTimer.Done();

            // Compile the template into an assembly
            lTimer = Log.Fn("Compile", timer: true);
            var codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

            var compilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            lTimer.Done();

            if (compilerResults.Errors.Count <= 0)
                return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

            // Handle compilation errors
            var errorList = compilerResults.Errors.Cast<CompilerError>().ToList();

            return l.ReturnAsError((null, errorList), "error");
        }



        /// <summary>
        /// Compiles the C# code into an assembly.
        /// </summary>
        /// <returns>The compiled assembly.</returns>
        private (Assembly Assembly, List<CompilerError> Errors) CompileCSharpCode(string csharpCode, List<string> referencedAssemblies)
        {
            var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"C# code content length: {csharpCode.Length}");

            var lTimer = Log.Fn("Compiler Params", timer: true);
            var compilerParameters = new CompilerParameters(referencedAssemblies.ToArray())
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = false,
                CompilerOptions = $"{DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };
            lTimer.Done();

            // Compile the C# code into an assembly
            lTimer = Log.Fn("Compile", timer: true);
            var codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider(); // TODO: @stvtest with latest nuget package for @inherits ; issue

            var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, csharpCode);
            lTimer.Done();

            if (compilerResults.Errors.Count <= 0)
                return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

            // Handle compilation errors
            var errorList = compilerResults.Errors.Cast<CompilerError>().ToList();

            return l.ReturnAsError((null, errorList), "error");
        }


        private string ErrorMessagesFromCompileErrors(List<CompilerError> errors)
        {
            var l = Log.Fn<string>(timer: true);
            var compileErrors = new StringBuilder();
            foreach (var compileError in errors)
                compileErrors.AppendLine($"Line: {compileError.Line}, Column: {compileError.Column}, Error: {compileError.ErrorText}");
            return l.ReturnAsOk(compileErrors.ToString());
        }


        /// <summary>
        /// Finds the type of the template based on the template content.
        /// </summary>
        /// <param name="template">The template content.</param>
        /// <returns>The type of the template.</returns>
        private static string FindBaseClass(string template)
        {
            try
            {
                // TODO: stv, use existing 2sxc regex for extraction (from CodeType)

                if (!template.Contains("@inherits ")) return FallbackBaseClass;

                // extract the type name from the template
                var at = template.IndexOf("@inherits ", StringComparison.Ordinal);
                var at2 = template.IndexOf("\n", at, StringComparison.Ordinal);
                if (at2 == -1) at2 = template.Length;
                var line = template.Substring(at, at2 - at);
                line = line.Trim();
                if (line.EndsWith(";")) line = line.Substring(0, line.Length - 1);
                var typeName = line.Replace("@inherits ", "").Trim();

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Type.GetType(typeName, true); // test for known type

                return typeName;
            }
            catch (Exception)
            {
                return FallbackBaseClass;
            }
        }

        /// <summary>
        /// Creates a new instance of the RazorTemplateEngine class with the specified configuration.
        ///
        /// Basically imitating https://github.com/Antaris/RazorEngine/blob/master/src/source/RazorEngine.Core/Compilation/CompilerServiceBase.cs#L203-L229
        /// </summary>
        /// <returns>The initialized RazorTemplateEngine instance.</returns>
        private RazorTemplateEngine CreateHost(string className, string baseClass, string defaultNamespace)
        {
            var l = Log.Fn<RazorTemplateEngine>($"{nameof(className)}: '{className}'; {nameof(baseClass)}: '{baseClass}'", timer: true);

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = baseClass,
                DefaultClassName = className,
                DefaultNamespace = defaultNamespace
            };

            var context = new GeneratedClassContext(
                "Execute",
                "Write",
                "WriteLiteral",
                "WriteTo",
                "WriteLiteralTo",
                typeof(HelperResult).FullName,
                "DefineSection")
            {
                ResolveUrlMethodName = "ResolveUrl"
            };

            host.GeneratedClassContext = context;

            // add implicit usings
            foreach (var ns in ImplicitUsings.ForRazor) host.NamespaceImports.Add(ns);

            var engine = new RazorTemplateEngine(host);
            return l.ReturnAsOk(engine);
        }
    }
}
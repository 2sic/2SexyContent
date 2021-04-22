#if NETSTANDARD
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Code.Builder
{

    // Code is based on DynamicRun by Laurent Kemp�
    // https://github.com/laurentkempe/DynamicRun
    // https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
    public class Compiler : HasLog
    {
        public Compiler() : base("Sys.DynamicRun.Cmpl")
        {

        }
        public Assembly Compile(string filePath, string dllName)
        {
            Log.Add($"Starting compilation of: '{filePath}'");

            var sourceCode = File.ReadAllText(filePath);

            return CompileSourceCode(filePath, sourceCode, dllName);
        }

        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]

        public Assembly CompileSourceCode(string path, string sourceCode, string dllName)
        {
            var wrapLog = Log.Call($"Source code compilation: {dllName}.");
            var encoding = Encoding.UTF8;
            var pdbName = $"{dllName}.pdb";
            using (var peStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var options = new EmitOptions(
                    debugInformationFormat: DebugInformationFormat.PortablePdb,
                    pdbFilePath: pdbName);

                var buffer = encoding.GetBytes(sourceCode);
                var sourceText = SourceText.From(buffer, buffer.Length, encoding, canBeEmbedded: true);

                var embeddedTexts = new List<EmbeddedText>
                {
                    EmbeddedText.FromSource(path, sourceText),
                };

                var result = GenerateCode(path, sourceText, dllName).Emit(peStream,
                    pdbStream,
                    embeddedTexts: embeddedTexts,
                    options: options);

                if (!result.Success)
                {
                    wrapLog("Compilation done with error.");

                    var errors = new List<string>();

                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Log.Add("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }

                    throw new Exception(String.Join("\n", errors));
                }

                wrapLog("Compilation done without any error.");

                peStream.Seek(0, SeekOrigin.Begin);
                pdbStream?.Seek(0, SeekOrigin.Begin);

                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
                var assembly = assemblyLoadContext.LoadFromStream(peStream, pdbStream);

                return assembly;
            }
        }

        public static CSharpCompilation GenerateCode(string path, SourceText sourceCode, string dllName)
        {
            var options = CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.CSharp9)
                .WithPreprocessorSymbols("OQTANE");

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options, path);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)
            };

            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            // Add references to all dll's in bin folder.
            var dllLocation = AppContext.BaseDirectory;
            var dllPath = Path.GetDirectoryName(dllLocation);
            foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll"))
                references.Add(MetadataReference.CreateFromFile(dllFile));

            var peName = $"{dllName}.dll";

            return CSharpCompilation.Create(peName,
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
#endif

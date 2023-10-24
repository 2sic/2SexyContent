﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Web.Compilation;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Web;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class CodeCompilerNetFull : CodeCompiler
    {
        public IHostingEnvironmentWrapper HostingEnvironment { get; }
        public IReferencedAssembliesProvider ReferencedAssembliesProvider { get; }

        public CodeCompilerNetFull(IServiceProvider serviceProvider, IHostingEnvironmentWrapper hostingEnvironment, IReferencedAssembliesProvider referencedAssembliesProvider) : base(serviceProvider)
        {
            ConnectServices(
                HostingEnvironment = hostingEnvironment,
                ReferencedAssembliesProvider = referencedAssembliesProvider
            );
        }

        public override AssemblyResult GetAssembly(string relativePath, string className)
        {
            var fullPath = NormalizeFullPath(HostingEnvironment.MapPath(relativePath));

            // 1. Handle Compile standalone file
            if (File.Exists(fullPath))
            {
                var assembly = BuildManager.GetCompiledAssembly(relativePath);
                return new AssemblyResult(assembly: assembly);
            }

            // 2. Handle Compile all in folder
            if (Directory.Exists(fullPath))
            {
                var cache = MemoryCache.Default;
                if (cache[fullPath.ToLowerInvariant()] is AssemblyResult assemblyResultCacheItem)
                    return assemblyResultCacheItem;

                // Get all C# files in the folder
                var sourceFiles = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);

                // Validate are there any C# files
                if (sourceFiles.Length == 0)
                    return new AssemblyResult(errorMessages: $"Error: given path '{relativePath}' doesn't contain any .cs files");

                var results = GetCompiledAssemblyFromFolder(sourceFiles);

                // Compile ok
                if (!results.Errors.HasErrors)
                {
                    // TODO: stv# missing assemblyBinary in cache
                    var cacheItem = new AssemblyResult(results.CompiledAssembly);
                    cache.Set(fullPath.ToLowerInvariant(), cacheItem, GetCacheItemPolicy(fullPath));
                    return cacheItem;
                }

                // Compile error case
                var errors = "";
                foreach (CompilerError error in results.Errors)
                    errors += $"Error ({error.ErrorNumber}): {error.ErrorText}\n";

                return new AssemblyResult(errorMessages: errors);
            }

            // 3. Path do not exists
            return new AssemblyResult(errorMessages: $"Error: given path '{relativePath}' doesn't exist");
        }

        private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = true,
                CompilerOptions = "/define:OQTANE;NETCOREAPP;NET5_0 /optimize-"
            };

            // Add all referenced assemblies
            parameters.ReferencedAssemblies.AddRange(ReferencedAssembliesProvider.Locations());

            return provider.CompileAssemblyFromFile(parameters, sourceFiles);
        }

        private CacheItemPolicy GetCacheItemPolicy(string filePath)
        {
            var filePaths = new List<string> { filePath };

            // expire cache item if not used in 30 min
            var cacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };

            // expire cache item on any file change
            cacheItemPolicy.ChangeMonitors.Add(new FolderChangeMonitor(filePaths));
            return cacheItemPolicy;
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
        {
            var compiledType = BuildManager.GetCompiledType(relativePath);
            var errMsg = compiledType == null
                ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
            return (compiledType, errMsg);
        }
    }
}
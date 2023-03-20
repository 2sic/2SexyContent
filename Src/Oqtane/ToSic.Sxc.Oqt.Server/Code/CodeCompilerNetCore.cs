﻿using System;
using System.IO;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Oqt.Server.Code
{
    [PrivateApi]
    public class CodeCompilerNetCore: CodeCompiler
    {
        private readonly LazySvc<IServerPaths> _serverPaths;

        public CodeCompilerNetCore(LazySvc<IServerPaths> serverPaths)
        {
            ConnectServices(
                _serverPaths = serverPaths
            );
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string virtualPath) 
            => throw new("Runtime Compile of .cshtml is Not Implemented in .net standard / core");
        protected override (Assembly Assembly, string ErrorMessages) GetAssembly(string virtualPath, string className)
        {
            var fullPath = _serverPaths.Value.FullContentPath(virtualPath.Backslash());
            fullPath = NormalizeFullFilePath(fullPath);
            try
            {
                return (new Compiler().Compile(fullPath, className), null);
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                var errorMessage =
                    $"Error: Can't compile '{className}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. " +
                    ex.Message;
                return (null, errorMessage);
            }
        }

        /**
         * Normalize full file path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
         */
        public static string NormalizeFullFilePath(string fullPath)
        {
            return new FileInfo(fullPath).FullName;
        }
    }
}

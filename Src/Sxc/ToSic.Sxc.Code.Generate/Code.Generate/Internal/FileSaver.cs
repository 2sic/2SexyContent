﻿using System;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static ToSic.Sxc.Internal.SxcLogging;

namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// Service to take generated files and save them to the file system
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FileSaver(CSharpDataModelsGenerator generator, ISite site, IAppStates appStates, IAppPathsMicroSvc appPaths)
    : ServiceBase(SxcLogName + ".GenFSv")
{
    public void GenerateAndSaveFiles(IFileGeneratorSpecs specs)
    {
        var l = Log.Fn();

        var bundle = generator.Generate(specs).First();
        var physicalPath = GetAppCodeDataPhysicalPath(bundle.Path, specs);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'");

        var classFiles = bundle.Files;
        foreach (var classSb in classFiles)
        {
            l.A($"Writing {classSb.FileName}; Path: {classSb.Path}; Content: {classSb.Body.Length}");

            var addPath = classSb.Path ?? "";
            if (addPath.StartsWith("/") || addPath.StartsWith("\\") || addPath.EndsWith("/") || addPath.EndsWith("\\") || addPath.Contains(".."))
                throw new($"Invalid path '{addPath}' in class '{classSb.FileName}' - contains invalid path like '..' or starts/ends with a slash.");

            var basePath = Path.Combine(physicalPath, classSb.Path);

            // ensure the folder for the file exists - it could be different for each file
            Directory.CreateDirectory(basePath);

            var fullPath = Path.Combine(basePath, classSb.FileName);
            File.WriteAllText(fullPath, classSb.Body);
        }

        l.Done();
    }

    private string GetAppFullPath(int appId)
    {
        var appState = appStates.ToReader(appStates.GetCacheState(appId));
        return appPaths.Init(site, appState).PhysicalPath;
    }

    private string GetAppCodeDataPhysicalPath(string mask, IFileGeneratorSpecs specs)
    {
        // Do basic mask tests
        if (mask.IsEmpty()) throw new("Mask must not be empty");
        if (mask.ContainsPathTraversal()) throw new($"Mask {PathFixer.PathTraversalMayNotContainMessage}");
        if (!mask.StartsWith(GenerateConstants.PathPlaceholderAppRoot)) throw new($"Mask must start with '{GenerateConstants.PathPlaceholderAppRoot}'");


        // Get the full path to the app
        var path = mask.Replace(GenerateConstants.PathPlaceholderAppRoot, GetAppFullPath(specs.AppId).TrimLastSlash());

        // Optionally add / replace the edition
        if (path.IndexOf(GenerateConstants.PathPlaceholderEdition, StringComparison.OrdinalIgnoreCase) > -1)
        {
            // sanitize path because 'edition' is user provided
            if (specs.Edition.ContainsPathTraversal())
                throw new($"Invalid edition '{specs.Edition}' - {PathFixer.PathTraversalMayNotContainMessage}");

            path = path.Replace(GenerateConstants.PathPlaceholderEdition, specs.Edition).TrimLastSlash();
        }

        path = path.FlattenSlashes().Backslash();

        var appWithEditionNormalized = new DirectoryInfo(path).FullName;

        if (!Directory.Exists(appWithEditionNormalized))
            throw new DirectoryNotFoundException(appWithEditionNormalized);

        return appWithEditionNormalized;
    }

    // TODO: @STV - this should be moved to an AppJsonService in Eav.Apps
    internal string GetPathToDotAppJson(FileGeneratorSpecs specs)
    {
        return Path.Combine(GetAppFullPath(specs.AppId), Constants.AppDataProtectedFolder, Constants.AppJson);
    }


    //public string Dump()
    //{
    //    var sb = new StringBuilder();

    //    var classFiles = Generate().First().Files;
    //    foreach (var classSb in classFiles)
    //    {
    //        sb.AppendLine($"// ----------------------- file: {classSb.FileName} ----------------------- ");
    //        sb.AppendLine(classSb.Body);
    //        sb.AppendLine();
    //        sb.AppendLine();
    //    }

    //    return sb.ToString();
    //}

}
﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToSic.Sxc.WebApi.Assets
{
    internal partial class AppAssetsBackend
    {
        public List<string> List(int appId, bool global = false, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
        {
            Log.Add($"list a#{appId}, global:{global}, path:{path}, mask:{mask}, withSub:{withSubfolders}, withFld:{returnFolders}");
            // set global access security if ok...
            var allowFullAccess = _user.IsSuperUser;

            // make sure the folder-param is not null if it's missing
            if (string.IsNullOrEmpty(path)) path = "";
            var appPath = ResolveAppPath(appId, global, allowFullAccess);
            var fullPath = Path.Combine(appPath, path);

            // make sure the resulting path is still inside 2sxc
            if (!allowFullAccess && !fullPath.Contains("2sxc"))
                throw new DirectoryNotFoundException("the folder is not inside 2sxc-scope any more and the current user doesn't have the permissions - must cancel");

            // if the directory doesn't exist, return empty list
            if (!Directory.Exists(fullPath))
                return new List<string>();

            var opt = withSubfolders
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            // try to collect all files, ignoring long paths errors and similar etc.
            var files = new List<FileInfo>();           // List that will hold the files and sub-files in path
            var folders = new List<DirectoryInfo>();    // List that hold directories that cannot be accessed
            var di = new DirectoryInfo(fullPath);
            FullDirList(di, mask, folders, files, opt);

            // return folders or files (depending on setting) with/without subfolders
            return (returnFolders
                    ? folders.Select(f => f.FullName)
                    : files.Select(f => f.FullName)
                )
                .Select(p => EnsurePathMayBeAccessed(p, appPath, allowFullAccess))  // do another security check
                .Select(x => x.Replace(appPath + "\\", ""))           // truncate / remove internal server root path
                .Select(x =>
                    x.Replace("\\", "/")) // tip the slashes to web-convention (old template entries used "\")
                .ToList();
        }
    }
}

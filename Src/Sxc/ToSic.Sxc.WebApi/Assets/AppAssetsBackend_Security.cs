﻿using System;
using System.IO;
using JetBrains.Annotations;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend
    {
        private static string SanitizePathAndContent(string path, FileContentsDto content)
        {
            var name = Path.GetFileName(path);
            var folder = Path.GetDirectoryName(path);
            var ext = Path.GetExtension(path);

            // not sure what this is for, since I believe code should only get here if there was an ext and it's cshtml
            // probably just to prevent some very unexpected create
            if (name == null) name = "missing-name.txt";

            switch (ext?.ToLowerInvariant())
            {
                // .cs files - usually API controllers
                case AssetEditor.CsExtension:
                    if ((folder?.ToLowerInvariant().IndexOf(AssetEditor.CsApiFolder, StringComparison.Ordinal) ?? -1) > -1)
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content =
                            AssetTemplates.DefaultWebApiBody.Replace(AssetTemplates.CsApiTemplateControllerName, nameWithoutExt);
                    }
                    break;

                // .cshtml files (razor) or .code.cshtml (razor code-behind)
                case AssetEditor.CshtmlExtension:
                    {
                        // ensure all .cshtml start with "_"
                        if (!name.StartsWith(AssetEditor.CshtmlPrefix))
                        {
                            name = AssetEditor.CshtmlPrefix + name;
                            path = (string.IsNullOrWhiteSpace(folder) ? "" : folder + "\\") + name;
                        }

                        // first check the code-extension, because it's longer but also would contain the non-code extension
                        if (name.EndsWith(AssetEditor.CodeCshtmlExtension))
                            content.Content = AssetTemplates.DefaultCodeCshtmlBody;
                        else if (name.EndsWith(AssetEditor.CshtmlExtension))
                            content.Content = AssetTemplates.DefaultCshtmlBody;
                        break;
                    }

                // .html files (Tokens)
                case AssetEditor.TokenHtmlExtension:
                    content.Content = AssetTemplates.DefaultTokenHtmlBody;
                    break;
            }

            return path;
        }

        [AssertionMethod]
        private string EnsurePathMayBeAccessed(string p, string appPath, bool allowFullAccess)
        {
            if (appPath == null) throw new ArgumentNullException(nameof(appPath));
            // security check, to ensure no results leak from outside the app

            if (!allowFullAccess && !p.StartsWith(appPath))
                throw new DirectoryNotFoundException("Result was not inside the app any more - must cancel");
            return p;
        }
    }
}

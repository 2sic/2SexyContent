﻿using System;
using System.IO;
using JetBrains.Annotations;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.WebApi.Assets
{
    internal partial class AppAssetsBackend
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
                    if ((folder?.ToLower().IndexOf(AssetEditor.CsApiFolder, StringComparison.Ordinal) ?? -1) > -1)
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content =
                            AssetEditor.DefaultCsBody.Replace(AssetEditor.CsApiTemplateControllerName, nameWithoutExt);
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
                            content.Content = AssetEditor.DefaultCodeCshtmlBody;
                        else if (name.EndsWith(AssetEditor.CshtmlExtension))
                            content.Content = AssetEditor.DefaultCshtmlBody;
                        break;
                    }

                // .html files (Tokens)
                case AssetEditor.TokenHtmlExtension:
                    content.Content = AssetEditor.DefaultTokenHtmlBody;
                    break;
            }

            return path;
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
            var isAdmin = _user.IsAdmin; // UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            var app = _app;
            if (appId != 0 && appId != app.AppId)
                app = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);
            var assetEditor = templateId != 0 && path == null
                ? new AssetEditor(app, templateId, _user.IsSuperUser, isAdmin, Log)
                : new AssetEditor(app, path, _user.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor);
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

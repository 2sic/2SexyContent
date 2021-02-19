﻿using System;
using System.IO;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Engines
{
    public enum PathTypes
    {
        PhysFull,
        PhysRelative,
        Link
    }

    public class TemplateHelpers: HasLog
    {
        public const string RazorC = "C# Razor";
        public const string TokenReplace = "Token";

        #region Constructor / DI


        private IServerPaths ServerPaths { get; }
        public IApp App;
        public TemplateHelpers(IServerPaths serverPaths, ILinkPaths linkPaths, IGlobalConfiguration globalConfiguration): base("Viw.Help")
        {
            ServerPaths = serverPaths;
            _linkPaths = linkPaths;
            _globalConfiguration = globalConfiguration;
        }

        public TemplateHelpers Init(IApp app, ILog parentLog)
        {
            App = app;
            Log.LinkTo(parentLog);
            return this;
        }

        #endregion

        private readonly ILinkPaths _linkPaths;
        private readonly IGlobalConfiguration _globalConfiguration;

        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        public void EnsureTemplateFolderExists(bool isShared)
        {
            var wrapLog = Log.Call($"{isShared}");
            var portalPath = isShared
                ? Path.Combine(ServerPaths.FullAppPath(Settings.PortalHostDirectory) ?? "", Settings.AppsRootFolder)
                : App.Site.AppsRootPhysicalFull ?? "";
            var sexyFolderPath = portalPath;

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder, but only if is there, and for Oqtane is not)
            // Note that DNN needs it because many razor file don't use @inherits and the web.config contains the default class
            // but in Oqtane we'll require that to work
            var webConfigTempletFilePath = Path.Combine(_globalConfiguration.GlobalFolder, Settings.WebConfigTemplateFile);
            if (File.Exists(webConfigTempletFilePath) && !sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
            {
                File.Copy(webConfigTempletFilePath,
                    Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            }

            // Create a Content folder (or App Folder)
            if (string.IsNullOrEmpty(App.Folder))
            {
                wrapLog("Folder name not given, won't create");
                return;
            }

            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
            contentFolder.Create();
            wrapLog("ok");
        }

        public string AppPathRoot(bool global) => AppPathRoot(global, PathTypes.PhysFull);

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public string AppPathRoot(bool useSharedFileSystem, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"{useSharedFileSystem}, {pathType}");
            string basePath;
            //var useSharedFileSystem = locationId == Settings.TemplateLocations.HostFileSystem;
            switch (pathType)
            {
                case PathTypes.Link:
                    basePath = useSharedFileSystem
                        ? _linkPaths.ToAbsolute(Settings.PortalHostDirectory, Settings.AppsRootFolder)
                        : App.Site.AppsRootLink;
                    break;
                case PathTypes.PhysRelative:
                    basePath = useSharedFileSystem
                        ? _linkPaths.ToAbsolute(Settings.PortalHostDirectory, Settings.AppsRootFolder)
                        : App.Site.AppsRootPhysical;
                    break;
                case PathTypes.PhysFull:
                    basePath = useSharedFileSystem
                        ? ServerPaths.FullAppPath(Path.Combine(Settings.PortalHostDirectory, Settings.AppsRootFolder))
                        : App.Site.AppsRootPhysicalFull;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            var finalPath = Path.Combine(basePath, App.Folder);
            return wrapLog(finalPath, finalPath);
        }

        public string IconPathOrNull(IView view, PathTypes type)
        {
            // 1. Check if the file actually exists
            //var iconFile = ViewPath(view);
            var iconFile = IconPath(view, PathTypes.PhysFull);
            var exists = File.Exists(iconFile);

            // 2. Return as needed
            return exists ? IconPath(view, type) : null;
        }

        private string IconPath(IView view, PathTypes type)
        {
            var viewPath1 = ViewPath(view, type);
            return viewPath1.Substring(0, viewPath1.LastIndexOf(".", StringComparison.Ordinal)) + ".png";
        }

        public string ViewPath(IView view, PathTypes type) => AppPathRoot(view.IsShared, type) + "/" + view.Path;
    }
}
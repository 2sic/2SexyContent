﻿using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Concurrent;
using System.IO;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Dnn.Install
{
    /// <summary>
    /// Helper class to ensure that the an app is ready.
    /// It will have to do various file accesses - so once it knows a module is ready, it will cache the result.
    /// </summary>
    public class DnnReadyCheckTurbo : ServiceBase
    {
        private readonly ILazySvc<AppFolderInitializer> _appFolderInitializerLazy;

        /// <summary>
        /// Fast static check to see if the check had previously completed. 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="log"></param>
        public static bool QuickCheckSiteAndAppFoldersAreReady(PortalModuleBase module, ILog log)
        {
            var wrapLog = log.Fn<bool>(message: $"Turbo Ready Check: module {module.ModuleId} on page {module.TabId}");
            if (CachedModuleResults.TryGetValue(module.ModuleId, out var exists) && exists)
                return wrapLog.ReturnTrue("quick-check: ready"); // all ok, skip

            return wrapLog.ReturnFalse("deep-check: not ready, must do extensive check");
        }

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="appFolderInitializerLazy"></param>
        public DnnReadyCheckTurbo(ILazySvc<AppFolderInitializer> appFolderInitializerLazy) : base("Dnn.PreChk")
        {
            _appFolderInitializerLazy = appFolderInitializerLazy;
        }

        /// <summary>
        /// Verify that the portal is ready, otherwise show a good error
        /// </summary>
        public bool EnsureSiteAndAppFoldersAreReady(PortalModuleBase module, IBlock block)
        {
            var timerWrap = Log.Fn<bool>(message: $"module {module.ModuleId} on page {module.TabId}", timer: true);
            if (CachedModuleResults.TryGetValue(module.ModuleId, out var exists) && exists)
                return timerWrap.ReturnTrue("Previous check completed, will skip");

            // throw better error if SxcInstance isn't available
            // not sure if this doesn't have side-effects...
            if (block?.BlockBuilder == null)
                throw new Exception("Error - can't find 2sxc instance configuration. " +
                                    "Probably trying to show an app or content that has been deleted or not yet installed. " +
                                    "You may also have EnterpriseCMS features enabled but are missing the license activation (but this is super rare). ");

            // check things if it's a module of this portal (ensure everything is ok, etc.)
            var isSharedModule = module.ModuleConfiguration.PortalID != module.ModuleConfiguration.OwnerPortalID;
            if (isSharedModule) return timerWrap.ReturnFalse("skip, shared");

            if (block.App != null)
            {
                Log.A("Will check if site is ready and template folder exists");
                EnsureSiteIsConfiguredAndTemplateFolderExists(module, block);

                // If no exception was raised inside, everything is fine - must cache
                CachedModuleResults.AddOrUpdate(module.ModuleId, true, (id, value) => true);
            }
            else
                Log.A("skip, content-block not ready");

            return timerWrap.ReturnTrue("ok");
        }

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        private bool EnsureSiteIsConfiguredAndTemplateFolderExists(PortalModuleBase module, IBlock block)
        {
            var wrapLog = Log.Fn<bool>($"AppId: {block.AppId}");

            var sexyFolder = new DirectoryInfo(block.Context.Site.AppsRootPhysicalFull);
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, Eav.Constants.ContentAppFolder));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            if (!(sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
            {
                // configure it
                var tm = _appFolderInitializerLazy.Value;
                tm.EnsureTemplateFolderExists(block.Context.AppState, false);
            }

            return wrapLog.ReturnTrue($"Completed init for module {module.ModuleId} showing {block.AppId}");
        }

        internal static ConcurrentDictionary<int, bool> CachedModuleResults = new ConcurrentDictionary<int, bool>();
    }
}
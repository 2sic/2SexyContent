﻿using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;
using ILog = ToSic.Lib.Logging.ILog;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is a factory to create CmsBlocks, Apps etc. and related objects from DNN.
    /// </summary>
    [PublicApi]
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static class Factory
    {
        /// <summary>
        /// Workaround - static build should actually be completely deprecated, but as it's not possible yet,
        /// we'll provide this for now
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
#pragma warning disable CS0618
        private static T StaticBuild<T>() => DnnStaticDi.StaticBuild<T>();
#pragma warning restore CS0618

        private static ILog NewLog()
        {
            var log = new Log("Dnn.Factor");
            StaticBuild<ILogStore>().Add("obsolete-dnn-factory", log);
            return log;
        }

        /// <summary>
        /// Get a Root CMS Block if you know the TabId and the ModId
        /// </summary>
        /// <param name="pageId">The DNN tab id (page id)</param>
        /// <param name="modId">The DNN Module id</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IBlockBuilder CmsBlock(int pageId, int modId) => CmsBlock(pageId, modId, NewLog());

        /// <summary>
        /// Get a Root CMS Block if you know the TabId and the ModId
        /// </summary>
        /// <param name="pageId">The DNN tab id (page id)</param>
        /// <param name="modId">The DNN Module id</param>
        /// <param name="parentLog">The parent log, optional</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IBlockBuilder CmsBlock(int pageId, int modId, ILog parentLog)
        {
            Compatibility.Obsolete.Warning13To15($"ToSic.Sxc.Dnn.Factory.{nameof(CmsBlock)}", "", "https://r.2sxc.org/brc-13-dnn-factory");
            var wrapLog = parentLog.Fn<IBlockBuilder>($"{pageId}, {modId}");
            var builder = StaticBuild<IModuleAndBlockBuilder>().LinkLog(parentLog).GetBlock(pageId, modId).BlockBuilder;
            return wrapLog.ReturnAsOk(builder);
        }

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object
        /// </summary>
        /// <param name="moduleInfo">A DNN ModuleInfo object</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IBlockBuilder CmsBlock(ModuleInfo moduleInfo)
            => CmsBlock(((DnnModule)StaticBuild<IModule>()).Init(moduleInfo, NewLog()));

        /// <summary>
        /// Get a Root CMS Block if you have the ModuleInfo object.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized CMS Block, ready to use/render</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IBlockBuilder CmsBlock(IModule module, ILog parentLog = null)
        {
            Compatibility.Obsolete.Warning13To15($"ToSic.Sxc.Dnn.Factory.{nameof(CmsBlock)}", "", "https://r.2sxc.org/brc-13-dnn-factory");
            parentLog = parentLog ?? NewLog();
            var dnnModule = ((Module<ModuleInfo>)module)?.GetContents();
            return StaticBuild<IModuleAndBlockBuilder>().LinkLog(parentLog).GetBlock(dnnModule, null).BlockBuilder;
        }

        /// <summary>
        /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
        /// </summary>
        /// <param name="blockBuilder">CMS Block for which the helper is targeted. </param>
        /// <returns>A Code Helper based on <see cref="IDnnDynamicCode"/></returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IDnnDynamicCode DynamicCode(IBlockBuilder blockBuilder)
        {
            Compatibility.Obsolete.Warning13To15($"ToSic.Sxc.Dnn.Factory.{nameof(DynamicCode)}", "", "https://r.2sxc.org/brc-13-dnn-factory");
            return StaticBuild<DnnDynamicCodeRoot>().InitDynCodeRoot(blockBuilder.Block, NewLog(), Constants.CompatibilityLevel10) as
                DnnDynamicCodeRoot;
        }

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="unusedButKeepForApiStability">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
            => App(AppConstants.AutoLookupZone, appId, null, showDrafts, parentLog ?? NewLog());

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="zoneId">The zone the app is in.</param>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="unusedButKeepForApiStability">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
            => App(zoneId, appId, null, showDrafts, parentLog ?? NewLog());

        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="ownerPortalSettings">The owner portal - this is important when retrieving Apps from another portal.</param>
        /// <param name="unusedButKeepForApiStability">
        /// Tells the App that you'll be using page-publishing.
        /// So changes will me auto-drafted for a future release as the whole page together.
        /// </param>
        /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <param name="parentLog">optional logger to attach to</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
        public static IApp App(int appId,
            PortalSettings ownerPortalSettings,
            bool unusedButKeepForApiStability = false,
            bool showDrafts = false,
            ILog parentLog = null)
            => App(AppConstants.AutoLookupZone, appId,
                ((DnnSite)StaticBuild<ISite>()).Swap(ownerPortalSettings, parentLog), showDrafts, parentLog);

        [InternalApi_DoNotUse_MayChangeWithoutNotice]
        private static IApp App(
            int zoneId,
            int appId,
            ISite site,
            bool showDrafts,
            ILog parentLog)
        {
            Compatibility.Obsolete.Warning13To15($"ToSic.Sxc.Dnn.Factory.{nameof(App)}", "", "https://r.2sxc.org/brc-13-dnn-factory");
            var log = new Log("Dnn.Factry", parentLog ?? NewLog());
            log.A($"Create App(z:{zoneId}, a:{appId}, tenantObj:{site != null}, showDrafts: {showDrafts}, parentLog: {parentLog != null})");
            var app = StaticBuild<App>().LinkLog(log);
            if (site != null) app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(zoneId, appId), StaticBuild<AppConfigDelegate>().LinkLog(log).Build(showDrafts));
            return appStuff;
        }

    }
}
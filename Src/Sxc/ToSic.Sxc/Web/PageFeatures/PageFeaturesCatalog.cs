﻿using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.PageFeatures
{
    /// <summary>
    /// Important: This is a singleton!
    /// </summary>
    public class PageFeaturesCatalog: GlobalCatalogBase<IPageFeature>
    {
        /// <summary>
        /// Constructor - ATM we'll just add our known services here.
        ///
        /// </summary>
        /// <remarks>
        /// Important: if you want to add more services in a DI Startup, it must happen at Configure.
        /// If you do it earlier, the singleton retrieved then will not be the one at runtime.
        /// </remarks>
        public PageFeaturesCatalog(LogHistory logHistory): base(logHistory, Constants.SxcLogName + ".PftCat", new CodeRef())
        {
            Register(
                BuiltInFeatures.JQuery,
                BuiltInFeatures.ContextPage,
                BuiltInFeatures.ContextModule,
                BuiltInFeatures.JsCore,
                BuiltInFeatures.JsCms,
                BuiltInFeatures.Toolbars,
                BuiltInFeatures.ToolbarsAuto,
                BuiltInFeatures.TurnOn
            );
        }
    }
}

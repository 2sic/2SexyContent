﻿using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.PageService
{
    /// <summary>
    /// This controller should collect what all the <see cref="ToSic.Sxc.Services.IPageService"/> objects do, for use on the final page
    /// It must be scoped, so that it's the same object across the entire page-lifecycle.
    /// </summary>
    public partial class PageServiceShared: IChangeQueue // , INeedsDynamicCodeRoot
    {

        public PageServiceShared(IPageFeatures pageFeatures, IFeaturesService featuresService, ModuleLevelCsp csp)
        {
            FeaturesService = featuresService;
            PageFeatures = pageFeatures;
            Csp = csp;
        }

        internal readonly IFeaturesService FeaturesService;
        public IPageFeatures PageFeatures { get; }
        public ModuleLevelCsp Csp { get; }


        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [PrivateApi("not final yet, will probably change")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;

        [PrivateApi("not final yet")]
        protected PageChangeModes GetMode(PageChangeModes modeForAuto)
        {
            switch (ChangeMode)
            {
                case PageChangeModes.Default:
                case PageChangeModes.Auto:
                    return modeForAuto;
                case PageChangeModes.Replace:
                case PageChangeModes.Append:
                case PageChangeModes.Prepend:
                    return ChangeMode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ChangeMode), ChangeMode, null);
            }
        }

    }
}

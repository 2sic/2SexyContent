﻿using System.IO;
using System.Web.Hosting;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// This is a DNN implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public sealed class DnnSite: Site<PortalSettings>, ISiteLight
    {
        #region Constructors and DI

        /// <summary>
        /// DI Constructor, will get the current portal settings
        /// #TodoDI not ideal yet, as PortalSettings.current is still retrieved from global
        /// </summary>
        public DnnSite() => Swap(null);


        public DnnSite Swap(PortalSettings settings)
        {
            UnwrappedContents = KeepBestPortalSettings(settings);
            return this;
        }

        public DnnSite TrySwap(ModuleInfo module)
        {
            if (module == null || module.OwnerPortalID < 0) return this;
            var modulePortalSettings = new PortalSettings(module.OwnerPortalID);
            return Swap(modulePortalSettings);
        }

        /// <summary>
        /// Very special helper to work around a DNN issue
        /// Reason is that PortalSettings.Current is always "perfect" and also contains root URLs and current Page
        /// Other PortalSettings may not contain this (partially populated objects)
        /// In case we're requesting a DnnTenant with incomplete PortalSettings
        /// we want to correct this here
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static PortalSettings KeepBestPortalSettings(PortalSettings settings)
        {
            // in case we don't have an HTTP Context with current portal settings, don't try anything
            if (PortalSettings.Current == null) return settings;

            // If we don't have settings, or they point to the same portal, then use that
            if (settings == null || settings == PortalSettings.Current || settings.PortalId == PortalSettings.Current.PortalId)
                return PortalSettings.Current;

            // fallback: use supplied settings
            return settings;
        }

        /// <inheritdoc />
        public override ISite Init(int siteId)
        {
            var newSettings = new PortalSettings(siteId);
            // only replace it if it's different - because the initial normal Portalsettings has more loaded values
            //if (newSettings.PortalId != (UnwrappedContents?.PortalId ?? -1))
            //    UnwrappedContents = newSettings;
            return Swap(newSettings);
        }

        #endregion


        /// <inheritdoc />
        public override string DefaultLanguage => _defaultLanguage ?? (_defaultLanguage = UnwrappedContents.DefaultLanguage.ToLowerInvariant());
        private string _defaultLanguage;

        /// <inheritdoc />
        public override int Id => UnwrappedContents?.PortalId ?? Eav.Constants.NullId;

        /// <inheritdoc />
        public override string Name => UnwrappedContents.PortalName;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Important special case: if the PortalSettings are not from the PortalSettings.Current, then the
        /// PortalAlias are null!!!
        /// I believe this should only matter in very special cases
        /// Like when showing a module from another portal - in which case we don't need that alias
        /// but the current one. Just keep this in mind in case anything ever breaks.
        /// </remarks>
        public override string Url => UnwrappedContents?.PortalAlias?.HTTPAlias
                                          ?? PortalSettings.Current?.PortalAlias?.HTTPAlias
                                          ?? "err-portal-alias-not-loaded";

        [PrivateApi]
        public override string AppsRootPhysical => AppsRootRelative;

        internal string AppsRootRelative => Path.Combine(UnwrappedContents.HomeDirectory, Settings.AppsRootFolder);

        [PrivateApi]
        public override string AppsRootPhysicalFull => HostingEnvironment.MapPath(AppsRootRelative);

        /// <inheritdoc />
        public override string ContentPath => UnwrappedContents.HomeDirectory;

        public override int ZoneId
        {
            get { 
                if(_zoneId != null) return _zoneId.Value;
                // check if id is negative; 0 is a valid tenant id
                if (Id < 0) return (_zoneId = Eav.Constants.NullId).Value;
                _zoneId = Eav.Factory.Resolve<DnnZoneMapper>().Init(null).GetZoneId(Id);
                return _zoneId.Value;
            }
        }

        private int? _zoneId;
    }
}
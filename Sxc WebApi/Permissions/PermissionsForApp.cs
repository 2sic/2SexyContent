﻿using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Errors;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class PermissionsForApp: HasLog, IContextPermissionCheck
    {
        /// <summary>
        /// The current app which will be used and can be re-used externally
        /// </summary>
        public App App { get; }

        public SxcInstance SxcInstance { get; }

        protected readonly PortalSettings PortalForSecurityCheck;

        public bool SamePortal { get; }

        public PermissionsForApp(SxcInstance sxcInstance, int appId, Log parentLog) :
            this(sxcInstance, SystemManager.ZoneIdOfApp(appId), appId, parentLog) { }

        protected PermissionsForApp(SxcInstance sxcInstance, int zoneId, int appId, Log parentLog) : base("Api.Perms", parentLog)
        {
            var wrapLog = Log.New("AppAndPermissions", $"..., appId: {appId}, ...");
            SxcInstance = sxcInstance;
            var tenant = new DnnTenant(PortalSettings.Current);
            var environment = Factory.Resolve<IEnvironmentFactory>().Environment(Log);
            var contextZoneId = environment.ZoneMapper.GetZoneId(tenant.Id);
            App = new App(tenant, zoneId, appId, false, Log);
            SamePortal = contextZoneId == zoneId;
            PortalForSecurityCheck = SamePortal ? PortalSettings.Current : null;
            wrapLog($"ready for z/a:{zoneId}/{appId} t/z:{tenant.Id}/{contextZoneId} same:{SamePortal}");
        }

        public bool ZoneChangedAndNotSuperUser(out HttpResponseException exp)
        {
            var wrapLog = Log.Call("ZoneChangedAndNotSuperUser()");
            var zoneSameOrSuperUser = SamePortal || PortalSettings.Current.UserInfo.IsSuperUser;
            exp = zoneSameOrSuperUser ? null: Http.PermissionDenied(
                $"accessing app {App.AppId} in zone {App.ZoneId} is not allowed for this user");

            wrapLog(zoneSameOrSuperUser ? $"sameportal:{SamePortal} - ok": "not ok, generate error");

            return !zoneSameOrSuperUser;
        }

        internal bool UserUnrestrictedAndFeatureEnabled(out HttpResponseException preparedException)
        {
            var wrapLog = Log.Call("UserUnrestrictedAndFeatureEnabled", "");
            // 1. check if user is restricted
            var userIsRestricted = !PermissionChecker.UserMay(GrantSets.WritePublished);

            // 2. check if feature is enabled
            var feats = new[] { FeatureIds.PublicForms };
            if (userIsRestricted && !Features.Enabled(feats))
            {
                preparedException = Http.PermissionDenied($"low-permission users may not access this - {Features.MsgMissingSome(feats)}");
                return false;
            }
            wrapLog("ok");
            preparedException = null;
            return true;
        }

        public IPermissionCheck PermissionChecker => _permissionCheck ?? (_permissionCheck = BuildPermissionChecker());
        private IPermissionCheck _permissionCheck;


        /// <summary>
        /// Creates a permission checker for an app
        /// Optionally you can provide a type-name, which will be 
        /// included in the permission check
        /// </summary>
        /// <returns></returns>
        private IPermissionCheck BuildPermissionChecker()
        {
            Log.Call("AppPermissionChecker");

            // user has edit permissions on this app, and it's the same app as the user is coming from
            return new DnnPermissionCheck(Log,
                instance: SxcInstance.EnvInstance,
                app: App,
                portal: PortalForSecurityCheck);
        }

        /// <summary>
        /// Ensure for this app only!
        /// </summary>
        /// <param name="grants"></param>
        /// <param name="preparedException"></param>
        /// <returns></returns>
        public bool Ensure(List<Grants> grants, out HttpResponseException preparedException)
        {
            if (!BuildPermissionChecker().UserMay(grants))
            {
                Log.Add("permissions not ok");
                preparedException = Http.PermissionDenied("required permissions for this type are not given");
                throw preparedException;
            }
            Log.Add("Ensure(...): ok");
            preparedException = null;
            return true;
        }
    }
}

﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Context
{
    public class ContextOfApp: ContextOfSite, IContextOfApp
    {
        #region Constructor / DI

        public ContextOfApp(IServiceProvider serviceProvider, ISite site, IUser user) : base(serviceProvider, site, user)
        {
            Log.Rename("Sxc.CtxApp");
        }

        #endregion

        public void ResetApp(IAppIdentity appIdentity)
        {
            if (AppIdentity == null || AppIdentity.AppId != appIdentity.AppId) 
                AppIdentity = appIdentity;
        }

        public void ResetApp(int appId) => ResetApp(State.Identity(null /*Site.IsMultiZone ? null as int? : Site.ZoneId*/, appId));

        protected virtual IAppIdentity AppIdentity
        {
            get => _appIdentity;
            set
            {
                _appIdentity = value;
                _appState = null;
                _userMayEdit = null;
            }
        }

        private IAppIdentity _appIdentity;

        public override bool UserMayEdit
        {
            get
            {
                if (_userMayEdit.HasValue) return _userMayEdit.Value;
                var wrapLog = Log.Call<bool>();
                if (AppState == null)
                    return wrapLog("no app, use fallback", base.UserMayEdit);
                _userMayEdit = ServiceProvider.Build<AppPermissionCheck>()
                    .ForAppInInstance(this, AppState, Log)
                    .UserMay(GrantSets.WriteSomething);
                return wrapLog($"{_userMayEdit.Value}", _userMayEdit.Value);
            }
        }
        private bool? _userMayEdit;

        public AppState AppState => _appState ?? (_appState = AppIdentity == null ? null : State.Get(AppIdentity));
        private AppState _appState;

    }
}

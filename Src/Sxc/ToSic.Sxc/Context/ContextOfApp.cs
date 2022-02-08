﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Languages;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    public class ContextOfApp: ContextOfSite, IContextOfApp
    {
        #region Constructor / DI

        public class ContextOfAppDependencies
        {
            public ContextOfAppDependencies(IAppStates appStates, Lazy<IFeaturesService> featsLazy, LazyInitLog<AppUserLanguageCheck> langCheckLazy)
            {
                AppStates = appStates;
                FeatsLazy = featsLazy;
                LangCheckLazy = langCheckLazy;
            }
            public IAppStates AppStates { get; }
            public Lazy<IFeaturesService> FeatsLazy { get; }
            public LazyInitLog<AppUserLanguageCheck> LangCheckLazy { get; }
        }

        public ContextOfApp(IServiceProvider serviceProvider, ISite site, IUser user, ContextOfAppDependencies dependencies)
            : base(serviceProvider, site, user)
        {
            Deps = dependencies;
            dependencies.LangCheckLazy.SetLog(Log);
            Log.Rename("Sxc.CtxApp");
        }
        protected readonly ContextOfAppDependencies Deps;

        #endregion

        public void ResetApp(IAppIdentity appIdentity)
        {
            if (AppIdentity == null || AppIdentity.AppId != appIdentity.AppId) 
                AppIdentity = appIdentity;
        }

        public void ResetApp(int appId) => ResetApp(Deps.AppStates.IdentityOfApp(appId));

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

                if (User.IsSuperUser)
                {
                    _userMayEdit = true;
                    return wrapLog("super", _userMayEdit.Value);
                }

                if (AppState == null)
                {
                    _userMayEdit = base.UserMayEdit;
                    return wrapLog("no app, use fallback", _userMayEdit.Value);
                }

                _userMayEdit = ServiceProvider.Build<AppPermissionCheck>()
                    .ForAppInInstance(this, AppState, Log)
                    .UserMay(GrantSets.WriteSomething);

                // Check if language permissions may alter edit
                if (_userMayEdit == true && Deps.FeatsLazy.Value.IsEnabled(FeaturesCatalog.PermissionsByLanguage.NameId))
                    _userMayEdit = Deps.LangCheckLazy.Ready.UserRestrictedByLanguagePermissions(AppState) ?? _userMayEdit;

                return wrapLog($"{_userMayEdit.Value}", _userMayEdit.Value);
            }
        }
        private bool? _userMayEdit;

        public AppState AppState => _appState ?? (_appState = AppIdentity == null ? null : Deps.AppStates.Get(AppIdentity));
        private AppState _appState;

    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Caching;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Web.LightSpeed
{
    public class LightSpeed : HasLog, IOutputCache
    {

        public LightSpeed(IFeaturesService features, Lazy<IAppStates> appStatesLazy, Lazy<AppPaths> appPathsLazy, LightSpeedStats lightSpeedStats) : base(Constants.SxcLogName + ".Lights")
        {
            LightSpeedStats = lightSpeedStats;
            _features = features;
            _appStatesLazy = appStatesLazy;
            _appPathsLazy = appPathsLazy;
        }
        public LightSpeedStats LightSpeedStats { get; }
        private readonly IFeaturesService _features;
        private readonly Lazy<IAppStates> _appStatesLazy;
        private readonly Lazy<AppPaths> _appPathsLazy;

        public IOutputCache Init(int moduleId, int pageId, IBlock block)
        {
            var wrapLog = Log.Call<IOutputCache>($"mod: {moduleId}");
            _moduleId = moduleId;
            _pageId = pageId;
            _block = block;
            return wrapLog($"{IsEnabled}", this);
        }
        private int _moduleId;
        private int _pageId;
        private IBlock _block;
        private AppState AppState => _block?.Context?.AppState;
        private IAppStates AppStates => _appStatesLazy.Value;

        public bool Save(IRenderResult data)
        {
            var wrapLog = Log.Call<bool>();
            if (!IsEnabled) return wrapLog("disabled", false);
            if (data == null) return wrapLog("null", false);
            if (data.IsError) return wrapLog("error", false);
            if (!data.CanCache) return wrapLog("can't cache", false);
            if (data == Existing?.Data) return wrapLog("not new", false);
            if (data.DependentApps?.Any() != true) return wrapLog("app not initialized", false);

            // get dependent appStates
            var dependentAppsStates = data.DependentApps.Select(da => AppStates.Get(da.AppId)).ToList();

            // when dependent apps have disabled caching, parent app should not cache also 
            if (!IsEnabledOnDependentApps(dependentAppsStates)) return wrapLog("disabled in dependent app", false);

            Log.A($"Found {data.DependentApps.Count} apps: " + string.Join(",", data.DependentApps.Select(da => da.AppId)));
            Fresh.Data = data;
            var duration = Duration;
            // only add if we really have a duration; -1 is disabled, 0 is not set...
            if (duration <= 0)
                return wrapLog($"not added as duration is {duration}", false);

            var appPathsToMonitor = _features.IsEnabled(LightSpeedOutputCacheAppFileChanges.NameId)
                ? _appPaths.Get(() =>AppPaths(dependentAppsStates))
                : null;
            var cacheKey = Ocm.Add(CacheKey, Fresh, duration, dependentAppsStates, appPathsToMonitor,
                (x) => LightSpeedStats.Remove(AppState.AppId, data.Size));
            Log.A($"LightSpeed Cache Key: {cacheKey}");
            if (cacheKey != "error") 
                LightSpeedStats.Add(AppState.AppId, data.Size);
            return wrapLog($"added for {duration}s", true);
        }

        /// <summary>
        /// find if caching is enabled on all dependent apps
        /// </summary>
        private bool IsEnabledOnDependentApps(List<AppState> appStates)
        {
            var cLog = Log.Call2<bool>();
            foreach (var appState in appStates)
            {
                var appConfig = LightSpeedDecorator.GetFromAppStatePiggyBack(appState, Log);
                if (appConfig.IsEnabled == false)
                    return cLog.Return(false, $"Can't cache; caching disabled on dependent app {appState.AppId}");
            }
            return cLog.Return(true, "ok");
        }

        /// <summary>
        /// Get physical paths for parent app and all dependent apps (portal and shared)
        /// </summary>
        /// <remarks>
        /// Note: The App Paths are only the apps in /2sxc (global and per portal)
        /// ADAM folders are not monitored
        /// </remarks>
        /// <returns>list of paths to monitor</returns>
        private IList<string> AppPaths(List<AppState> appStates)
        {
            if (!((_block as BlockFromModule)?.App is App app)) return null;
            if (appStates?.Any() != true) return null;

            var paths = new List<string>();
            foreach (var appState in appStates)
            {
                var appPaths = _appPathsLazy.Value.Init(app.Site, appState, Log);
                if (Directory.Exists(appPaths.PhysicalPath)) paths.Add(appPaths.PhysicalPath);
                if (Directory.Exists(appPaths.PhysicalPathShared)) paths.Add(appPaths.PhysicalPathShared);
            }

            return paths;
        }
        private readonly ValueGetOnce<IList<string>> _appPaths = new ValueGetOnce<IList<string>>();

        private int Duration => _duration.Get(() =>
        {
            var user = _block.Context.User;
            if (user.IsSuperUser) return AppConfig.DurationSystemAdmin;
            if (user.IsAdmin) return AppConfig.DurationEditor;
            if (!user.IsAnonymous) return AppConfig.DurationUser;
            return AppConfig.Duration;
        });
        private readonly ValueGetOnce<int> _duration = new ValueGetOnce<int>();

        private string Suffix => _suffix.Get(GetSuffix);
        private readonly ValueGetOnce<string> _suffix = new ValueGetOnce<string>();

        private string GetSuffix()
        {
            if (!AppConfig.ByUrlParam) return null;
            var urlParams = _block.Context.Page.Parameters.ToString();
            if (string.IsNullOrWhiteSpace(urlParams)) return null;
            if (!AppConfig.UrlParamCaseSensitive) urlParams = urlParams.ToLowerInvariant();
            return urlParams;
        }

        private string CacheKey => _key.Get(() => Log.Return(() => Ocm.Id(_moduleId, _pageId, UserIdOrAnon, ViewKey, Suffix)));
        private readonly ValueGetOnce<string> _key = new ValueGetOnce<string>();

        private int? UserIdOrAnon => _userId.Get(() => _block.Context.User.IsAnonymous ? (int?)null : _block.Context.User.Id);
        private readonly ValueGetOnce<int?> _userId = new ValueGetOnce<int?>();

        private string ViewKey => _viewKey.Get(() => _block.Configuration?.PreviewTemplateId.HasValue == true ? $"{_block.Configuration.AppId}:{_block.Configuration.View.Id}" : null);
        private readonly ValueGetOnce<string> _viewKey = new ValueGetOnce<string>();

        public OutputCacheItem Existing => _existing.Get(ExistingGenerator);
        private readonly ValueGetOnce<OutputCacheItem> _existing = new ValueGetOnce<OutputCacheItem>();

        private OutputCacheItem ExistingGenerator()
        {
            var wrapLog = Log.Call<OutputCacheItem>();
            if (AppState == null) return wrapLog("no app", null);

            var result = IsEnabled ? Ocm.Get(CacheKey) : null;
            if (result == null) return wrapLog("not in cache", null);

            // compare cache time-stamps
            var dependentApp = result.Data?.DependentApps?.FirstOrDefault();
            if (dependentApp == null) return wrapLog("no dep app", null);

            return wrapLog("found", result);
        }

        public OutputCacheItem Fresh => _fresh ?? (_fresh = new OutputCacheItem());
        private OutputCacheItem _fresh;


        public bool IsEnabled => _enabled.Get(IsEnabledGenerator);
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        private bool IsEnabledGenerator()
        {
            var wrapLog = Log.Call<bool>();
            var feat = _features.IsEnabled(LightSpeedOutputCache.NameId);
            if (!feat) return wrapLog("feature disabled", false);
            var ok = AppConfig.IsEnabled;
            return wrapLog($"app config: {ok}", ok);
        }

        public LightSpeedDecorator AppConfig => _lsd.Get(() => LightSpeedDecoratorGenerator(AppState));
        private readonly ValueGetOnce<LightSpeedDecorator> _lsd = new ValueGetOnce<LightSpeedDecorator>();

        private LightSpeedDecorator LightSpeedDecoratorGenerator(AppState appState)
        {
            var wrapLog = Log.Call<LightSpeedDecorator>();
            var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(appState, Log);
            return wrapLog($"{decoFromPiggyBack.Entity != null}", decoFromPiggyBack);
        }

        private OutputCacheManager Ocm => _ocm ?? (_ocm = new OutputCacheManager());
        private OutputCacheManager _ocm;
    }
}

﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.ImportExport;
using AppDto = ToSic.Sxc.WebApi.App.AppDto;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab)
    // we can't set this globally (only needed for imports)

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/app/[action]")]
    public class AppController : OqtStatefulControllerBase, IAppController
    {
        private readonly Lazy<AppsBackend> _appsBackendLazy;
        private readonly Lazy<CmsZones> _cmsZonesLazy;
        private readonly Lazy<ExportApp> _exportAppLazy;
        private readonly Lazy<ImportApp> _importAppLazy;
        private readonly Lazy<AppManager> _appManagerLazy;
        private readonly Lazy<AppCreator> _appBuilderLazy;
        protected override string HistoryLogName => "Api.App";

        public AppController(StatefulControllerDependencies dependencies,
            Lazy<AppsBackend> appsBackendLazy,
            Lazy<CmsZones> cmsZonesLazy,
            Lazy<ExportApp> exportAppLazy,
            Lazy<ImportApp> importAppLazy,
            Lazy<AppManager> appManagerLazy,
            Lazy<AppCreator> appBuilderLazy) : base(dependencies)
        {
            _appsBackendLazy = appsBackendLazy;
            _cmsZonesLazy = cmsZonesLazy;
            _exportAppLazy = exportAppLazy;
            _importAppLazy = importAppLazy;
            _appManagerLazy = appManagerLazy;
            _appBuilderLazy = appBuilderLazy;
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public List<AppDto> List(int zoneId)
            => _appsBackendLazy.Value.Init(Log).Apps();

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public void App(int zoneId, int appId, bool fullDelete = true)
            => _cmsZonesLazy.Value.Init(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId, fullDelete);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public void App(int zoneId, string name)
            => _appBuilderLazy.Value.Init(zoneId, Log).Create(name);


        /// <summary>
        /// Used to be GET ImportExport/GetAppInfo
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public AppExportInfoDto Statistics(int zoneId, int appId)
            => _exportAppLazy.Value.Init(GetContext().Site.Id, GetContext().User, Log)
                .GetAppInfo(appId, zoneId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            SystemManager.Purge(zoneId, appId, log: Log);
            return wrapLog("ok", true);
        }

        /// <summary>
        /// Used to be GET ImportExport/ExportApp
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <param name="includeContentGroups"></param>
        /// <param name="resetAppGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Export(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => _exportAppLazy.Value.Init(GetContext().Site.Id, GetContext().User, Log)
                .Export(appId, zoneId, includeContentGroups, resetAppGuid);

        /// <summary>
        /// Used to be GET ImportExport/ExportForVersionControl
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <param name="includeContentGroups"></param>
        /// <param name="resetAppGuid"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        public bool SaveData(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => _exportAppLazy.Value.Init(GetContext().Site.Id, GetContext().User, Log)
                .SaveDataForVersionControl(appId, zoneId, includeContentGroups, resetAppGuid);


        /// <summary>
        /// Used to be POST ImportExport/ImportApp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public ImportResultDto Import(int zoneId)
        {
            Log.Add("import app start");

            var request = HttpContext.Request.Form;

            PreventServerTimeout300();

            if (request.Files.Count <= 0) return new ImportResultDto(false, "no files uploaded");

            return _importAppLazy.Value.Init(GetContext().User, Log)
                .Import(zoneId, request["Name"], request.Files[0].OpenReadStream());
        }
    }
}
﻿using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [DnnLogExceptions]
    public class ViewController : SxcApiControllerBase<ViewControllerReal>, IViewController
    {
        public ViewController() : base(ViewControllerReal.LogSuffix) { }

        /// <inheritdoc />
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ViewDetailsDto> All(int appId) => Real.All(appId);

        /// <inheritdoc />
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public PolymorphismDto Polymorphism(int appId) => Real.Polymorphism(appId);

        /// <inheritdoc />
        [HttpGet, HttpDelete]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, int id) => Real.Delete(appId, id);

        /// <inheritdoc />
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int viewId) => Real.Json(appId, viewId);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId, int appId) => Real.ImportPrep(PreventServerTimeout300).Import(new HttpUploadedFile(Request, HttpContext.Current.Request), zoneId, appId);

        /// <inheritdoc />
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ViewDto> Usage(int appId, Guid guid) => Real.UsagePreparations((views, blocks) =>
        {
            // create array with all 2sxc modules in this portal
            var allMods = new Pages.Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
            Log.Add($"Found {allMods.Count} modules");

            return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
        }).Usage(appId, guid);
    }
}
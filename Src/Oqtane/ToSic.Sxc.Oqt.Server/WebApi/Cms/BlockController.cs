﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/cms/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/cms/block/[action]")]

    //[ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class BlockController : OqtStatefulControllerBase, ToSic.Sxc.WebApi.Cms.IBlockController<IActionResult>
    {
        private readonly Lazy<CmsRuntime> _lazyCmsRuntime;
        private readonly Lazy<ContentBlockBackend> _blockBackendLazy;
        private readonly Lazy<AppViewPickerBackend> _viewPickerBackendLazy;
        private readonly Lazy<CmsZones> _cmsZonesLazy;
        private readonly Lazy<AppViewPickerBackend> _appViewPickerBackendLazy;
        protected override string HistoryLogName => "Api.Block";
        public BlockController(StatefulControllerDependencies dependencies,
            Lazy<CmsRuntime> lazyCmsRuntime,
            Lazy<ContentBlockBackend> blockBackendLazy,
            Lazy<AppViewPickerBackend> viewPickerBackendLazy,
            Lazy<CmsZones> cmsZonesLazy,
            Lazy<AppViewPickerBackend> appViewPickerBackendLazy) : base(dependencies)
        {
            _lazyCmsRuntime = lazyCmsRuntime;
            _blockBackendLazy = blockBackendLazy;
            _viewPickerBackendLazy = viewPickerBackendLazy;
            _cmsZonesLazy = cmsZonesLazy;
            _appViewPickerBackendLazy = appViewPickerBackendLazy;
        }

        protected CmsRuntime CmsRuntime
        {
            get
            {
                var runtime = _cmsRuntime;
                if (runtime != null) return runtime;

                return _cmsRuntime = ContextApp == null ? null : _lazyCmsRuntime.Value.Init(ContextApp, true, Log);
            }
        }
        private CmsRuntime _cmsRuntime;

        private IApp ContextApp => _app ??= GetBlock().App;
        private IApp _app;

        #region Block

        private ContentBlockBackend Backend => _backend ??= _blockBackendLazy.Value.Init(Log);
        private ContentBlockBackend _backend;

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public string Block(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            var entityId = Backend.NewBlock(parentId, field, sortOrder, app, guid);

            // now return a rendered instance
            var newContentBlock = HttpContext.RequestServices.Build<BlockFromEntity>().Init(GetBlock(), entityId, Log);
            return newContentBlock.BlockBuilder.Render();

        }
        #endregion

        #region BlockItems
        /// <summary>
        /// used to be GET Module/AddItem
        /// </summary>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Item(int? index = null)
        {
            Backend.AddItem(index);
            return new NoContentResult();
        }

        #endregion


        #region App

        /// <summary>
        /// used to be GET Module/SetAppId
        /// </summary>
        /// <param name="appId"></param>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public void App(int? appId)
            => _viewPickerBackendLazy.Value.Init(Log)
                .SetAppId(appId);

        /// <summary>
        /// used to be GET Module/GetSelectableApps
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<AppUiInfo> Apps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = GetContext().Site;// new DnnTenant(PortalSettings);
            return _cmsZonesLazy.Value.Init(tenant.ZoneId, Log).AppsRt.GetSelectableApps(tenant, apps)
                .ToList();
        }

        #endregion

        #region Types

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => CmsRuntime?.Views.GetContentTypesWithStatus();

        #endregion

        #region Templates

        /// <summary>
        /// used to be GET Module/GetSelectableTemplates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<TemplateUiInfo> Templates() => CmsRuntime?.Views.GetCompatibleViews(ContextApp, GetBlock().Configuration);

        /// <summary>
        /// Used in InPage.js
        /// used to be GET Module/SaveTemplateId
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="forceCreateContentGroup"></param>
        /// <returns></returns>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //[Authorize(Roles = RoleNames.Registered)]
        [Authorize(Roles = RoleNames.Admin)]
        // TODO: 2DM please check permissions
        public Guid? Template(int templateId, bool forceCreateContentGroup)
            => _viewPickerBackendLazy.Value.Init(Log)
                .SaveTemplateId(templateId, forceCreateContentGroup);

        #endregion


        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Render(int templateId, string lang)
        {
            Log.Add($"render template:{templateId}, lang:{lang}");
            try
            {
                var rendered = _appViewPickerBackendLazy.Value.Init(Log).Render(templateId, lang);
                return new ContentResult
                {
                    Content = rendered,
                    ContentType = "text/plain"
                    // rendered, Encoding.UTF8, "text/plain"
                };
                //return new HttpResponseMessage(HttpStatusCode.OK)
                //{
                //    Content = new StringContent(rendered, Encoding.UTF8, "text/plain")
                //};
            }
            catch
            {
                //Exceptions.LogException(e);
                throw;
            }
        }

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Publish(string part, int index) => Backend.PublishPart(part, index);

    }
}
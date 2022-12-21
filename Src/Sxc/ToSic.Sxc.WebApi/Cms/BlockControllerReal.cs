﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.InPage;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.WebApi.Cms
{
    public class BlockControllerReal : ServiceBase, IBlockController
    {
        public const string LogSuffix = "Block";

        public BlockControllerReal(
            LazyInit<IContextOfSite> context,
            LazyInit<ContentBlockBackend> blockBackend,
            LazyInit<AppViewPickerBackend> viewsBackend,
            LazyInit<CmsZones> cmsZones
            ): base($"{LogNames.WebApi}.{LogSuffix}Rl")
        {
            ConnectServices(
                _context = context,
                _blockBackendLazy = blockBackend,
                _viewsBackendLazy = viewsBackend,
                _cmsZones = cmsZones
            );
        }

        private readonly LazyInit<IContextOfSite> _context;
        private readonly LazyInit<ContentBlockBackend> _blockBackendLazy;
        private readonly LazyInit<AppViewPickerBackend> _viewsBackendLazy;
        private readonly LazyInit<CmsZones> _cmsZones;


        #region Block

        private ContentBlockBackend Backend => _backend = _backend ?? _blockBackendLazy.Value;
        private ContentBlockBackend _backend;

        /// <inheritdoc />
        public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
            => Backend.NewBlockAndRender(parentId, field, index, app, guid).Html;
        #endregion

        #region BlockItems
        /// <summary>
        /// used to be GET Module/AddItem
        /// </summary>
        public void Item(int? index = null)
        {
            Backend.AddItem(index);
        }

        #endregion


        #region App

        /// <summary>
        /// used to be GET Module/SetAppId
        /// </summary>
        /// <param name="appId"></param>

        public void App(int? appId) => _viewsBackendLazy.Value.SetAppId(appId);

        /// <summary>
        /// used to be GET Module/GetSelectableApps
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        public IEnumerable<AppUiInfo> Apps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = _context.Value.Site;
            return _cmsZones.Value.Init(Log).SetId(tenant.ZoneId).AppsRt.GetSelectableApps(tenant, apps)
                .ToList();
        }

        #endregion

        #region Types

        /// <inheritdoc />
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => _viewsBackendLazy.Value.ContentTypes();

        #endregion

        #region Templates

        /// <summary>
        /// used to be GET Module/GetSelectableTemplates
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TemplateUiInfo> Templates() => _viewsBackendLazy.Value.Templates();

        /// <summary>
        /// Used in InPage.js
        /// used to be GET Module/SaveTemplateId
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="forceCreateContentGroup"></param>
        /// <returns></returns>
        public Guid? Template(int templateId, bool forceCreateContentGroup)
            => _viewsBackendLazy.Value
                .SaveTemplateId(templateId, forceCreateContentGroup);

        #endregion

        /// <inheritdoc />
        public AjaxRenderDto Render(int templateId, string lang)
        {
            Log.A($"render template:{templateId}, lang:{lang}");
            return Backend.RenderV2(templateId, lang, _moduleRoot);
        }
        public BlockControllerReal Set(string moduleRoot)
        {
            _moduleRoot = moduleRoot;
            return this;
        }
        private string _moduleRoot;


        /// <inheritdoc />
        public bool Publish(string part, int index) => Backend.PublishPart(part, index);
    }
}
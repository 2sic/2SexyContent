﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/cms/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]

    [ValidateAntiForgeryToken]
    public class HistoryController : OqtStatefulControllerBase, IHistoryController
    {
        private readonly IdentifierHelper _idHelper;
        private readonly Lazy<AppManager> _appManagerLazy;
        protected override string HistoryLogName => "Api.History";

        public HistoryController(StatefulControllerDependencies dependencies,
            IdentifierHelper idHelper,
            Lazy<AppManager> appManagerLazy) : base(dependencies)
        {
            _idHelper = idHelper;
            _appManagerLazy = appManagerLazy;
        }

        /// <summary>
        /// Used to be POST Entities/History
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item)
            => _appManagerLazy.Value.Init(appId, Log).Entities.VersionHistory(_idHelper.Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId);

        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item)
        {
            _appManagerLazy.Value.Init(appId, Log).Entities.VersionRestore(_idHelper.Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId, changeId);
            return true;
        }

    }
}
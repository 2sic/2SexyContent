﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class ContentGroupController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.ConGrp";

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public EntityInListDto Header(Guid guid) 
            => _build<ListsBackendBase>().Init(GetBlock(), Log)
                .HeaderItem(guid);

        private ListsBackendBase Backend => _build<ListsBackendBase>().Init(GetBlock(), Log);

        // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
            => Backend.Replace(GetContext(), guid, part, index, entityId, add);


        // TODO: WIP changing this from ContentGroup editing to any list editing
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic Replace(Guid guid, string part, int index)
            => Backend.GetReplacementOptions(guid, part, index);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<EntityInListDto> ItemList(Guid guid, string part)
            => Backend.ItemList(guid, part);


        // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool ItemList([FromUri] Guid guid, List<EntityInListDto> list, [FromUri] string part = null)
            => Backend.Reorder(GetContext(), guid, list, part);

    }
}
﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]
    [ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ContentGroupController : SxcStatefulControllerBase
    {
        private readonly Lazy<ListsBackendBase> _listBackendLazy;
        protected override string HistoryLogName => "Api.ConGrp";
        public ContentGroupController(StatefulControllerDependencies dependencies,
            Lazy<ListsBackendBase> listBackendLazy) : base(dependencies)
        {
            _listBackendLazy = listBackendLazy;
        }

        [HttpGet]
        //[Authorize(Policy = "EditModule")] // TODO: disabled
        public EntityInListDto Header(Guid guid)
            => _listBackendLazy.Value.Init(GetBlock(), Log)
                .HeaderItem(guid);

        private ListsBackendBase Backend => _listBackendLazy.Value.Init(GetBlock(), Log);

        // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
        [HttpPost]
        //[Authorize(Policy = "EditModule")]  // TODO: disabled
        public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
            => Backend.Replace(GetContext(), guid, part, index, entityId, add);


        // TODO: WIP changing this from ContentGroup editing to any list editing
        [HttpGet]
        //[Authorize(Policy = "EditModule")]  // TODO: disabled
        public dynamic Replace(Guid guid, string part, int index)
            => Backend.GetReplacementOptions(guid, part, index);

        [HttpGet]
        //[Authorize(Policy = "EditModule")]  // TODO: disabled
        public List<EntityInListDto> ItemList(Guid guid, string part)
            => Backend.ItemList(guid, part);


        // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
        [HttpPost]
        //[Authorize(Policy = "EditModule")]  // TODO: disabled
        public bool ItemList([FromQuery] Guid guid, List<EntityInListDto> list, [FromQuery] string part = null)
            => Backend.Reorder(GetContext(), guid, list, part);

    }
}
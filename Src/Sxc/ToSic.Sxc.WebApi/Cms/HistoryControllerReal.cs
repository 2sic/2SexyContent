﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;

namespace ToSic.Sxc.WebApi.Cms
{
    // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

    public class HistoryControllerReal : HasLog<HistoryControllerReal>, IHistoryController
    {
        public const string LogSuffix = "Api.CmsHistory";

        public HistoryControllerReal(LazyInitLog<IdentifierHelper> idHelper, Lazy<AppManager> appManagerLazy) : base("Api.CmsHistoryRl")
        {
            _idHelper = idHelper.SetLog(Log);
            _appManagerLazy = appManagerLazy;
        }
        private readonly LazyInitLog<IdentifierHelper> _idHelper;
        private readonly Lazy<AppManager> _appManagerLazy;


        public List<ItemHistory> Get(int appId, ItemIdentifier item)
            => _appManagerLazy.Value.Init(appId, Log).Entities.VersionHistory(_idHelper.Ready.ResolveItemIdOfGroup(appId, item, Log).EntityId);


        public bool Restore(int appId, int changeId, ItemIdentifier item)
        {
            _appManagerLazy.Value.Init(appId, Log).Entities.VersionRestore(_idHelper.Ready.ResolveItemIdOfGroup(appId, item, Log).EntityId, changeId);
            return true;
        }
    }
}

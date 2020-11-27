﻿using System;
using ToSic.Eav;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;


namespace ToSic.Sxc.Run
{
    internal class BasicModuleUpdater: HasLog, IPlatformModuleUpdater
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public BasicModuleUpdater() : base($"{LogNames.NotImplemented}.MapA2I") { }


        public IPlatformModuleUpdater Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }


        public void SetAppId(IModule instance, int? appId)
        {
            // do nothing
        }

        public void SetPreview(int instanceId, Guid previewTemplateGuid)
        {
            // do nothing
        }

        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid)
        {
            // do nothing
        }

        public void UpdateTitle(IBlock block, IEntity titleItem)
        {
            // do nothing
        }
    }
}

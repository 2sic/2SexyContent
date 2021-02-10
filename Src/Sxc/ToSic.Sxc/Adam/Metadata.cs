﻿using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Helpers to get the metadata for ADAM items
    /// </summary>
    public class AdamMetadataMaker
    {
        private readonly IServiceProvider _serviceProvider;

        public AdamMetadataMaker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Find the first metadata entity for this file/folder
        /// </summary>
        /// <param name="app">the app which manages the metadata</param>
        /// <param name="mdId"></param>
        /// <returns></returns>
        internal IEntity GetFirstMetadata(AppRuntime app, MetadataFor mdId)
            => app.Metadata
                .Get(mdId.TargetType, mdId.KeyString)
                .FirstOrDefault();

        /// <summary>
        /// Get the first metadata entity of an item - or return a fake one instead
        /// </summary>
        internal IDynamicEntity GetFirstOrFake(AdamManager manager, MetadataFor mdId)
        {
            var meta = GetFirstMetadata(manager.AppRuntime, mdId) 
                       ?? Build.FakeEntity(Eav.Constants.TransientAppId);
            var dynEnt = new DynamicEntity(meta,
                (manager.AppContext?.Site).SafeLanguagePriorityCodes(),
                manager.CompatibilityLevel,
                null, _serviceProvider);
            return dynEnt;
        }

    }
}

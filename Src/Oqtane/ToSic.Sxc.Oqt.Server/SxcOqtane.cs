﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Razor.Engine.DbgWip;

namespace ToSic.Sxc.Oqt.Server
{
    public class SxcOqtane: HasLog, ISxcOqtane
    {
        #region Constructor and DI

        public SxcOqtane(OqtAssetsAndHeaders assetsAndHeaders, RazorReferenceManager debugRefMan, OqtTempInstanceContext oqtTempInstanceContext,
            IServiceProvider serviceProvider, Lazy<SiteStateInitializer> siteStateInitializerLazy, IHttpContextAccessor httpContextAccessor
            ) : base($"{OqtConstants.OqtLogPrefix}.Buildr")
        {
            _assetsAndHeaders = assetsAndHeaders;
            _debugRefMan = debugRefMan;
            _oqtTempInstanceContext = oqtTempInstanceContext;
            _serviceProvider = serviceProvider;
            _siteStateInitializerLazy = siteStateInitializerLazy;
            _httpContextAccessor = httpContextAccessor;
            // add log to history!
            History.Add("oqt-view", Log);
        }

        public IOqtAssetsAndHeader AssetsAndHeaders => _assetsAndHeaders;
        private readonly OqtAssetsAndHeaders _assetsAndHeaders;
        private readonly RazorReferenceManager _debugRefMan;
        private readonly OqtTempInstanceContext _oqtTempInstanceContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<SiteStateInitializer> _siteStateInitializerLazy;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Prepare

        /// <summary>
        /// Prepare must always be the first thing to be called - to ensure that afterwards both headers and html are known.
        /// </summary>
        public void Prepare(Site site, Oqtane.Models.Page page, Module module)
        {
            //if (_renderDone) throw new Exception("already prepared this module");

            // set SiteState.Alias early as possible
            _siteStateInitializerLazy.Value.InitIfEmpty();

            // HACK: STV
            if (page != null) _httpContextAccessor?.HttpContext?.Items.TryAdd("PageForLookUp", page);
            // HACK: STV and wrong!!!!!
            if (module != null) _httpContextAccessor?.HttpContext?.Items.TryAdd("ModuleForLookUp", module);

            Site = site;
            Page = page;
            Module = module;

            Block = GetBlock();

            _assetsAndHeaders.Init(this);
            GeneratedHtml = (MarkupString) Block.BlockBuilder.Render();
            Resources = Block.BlockBuilder.Assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
            }).ToList();
            _renderDone = true;
        }

        internal Site Site;
        internal Oqtane.Models.Page Page;
        internal Module Module;
        internal IBlock Block;

        private bool _renderDone;
        public MarkupString GeneratedHtml { get; private set; }

        public List<SxcResource> Resources { get; private set; }

        #endregion

        public string Test() => _debugRefMan.CompilationReferences.Count.ToString();

        private IBlock GetBlock()
        {
            var context = _oqtTempInstanceContext.CreateContext(Page.PageId, Module, Log);
            var block = _serviceProvider.Build<BlockFromModule>().Init(context, Log);
            return block;
        }


    }
}

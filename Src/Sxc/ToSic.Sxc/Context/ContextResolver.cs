﻿using System;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    public class ContextResolver: HasLog<IContextResolver>, IContextResolver
    {
        #region Constructor / DI

        protected AppIdResolver AppIdResolver => _appIdResolver ?? (_appIdResolver = _appIdResolverLazy.Value.Init(Log));
        private AppIdResolver _appIdResolver;
        private readonly Lazy<AppIdResolver> _appIdResolverLazy;

        private IServiceProvider ServiceProvider { get; }

        public ContextResolver(IServiceProvider serviceProvider, Lazy<AppIdResolver> appIdResolverLazy) : base("Sxc.CtxRes")
        {
            _appIdResolverLazy = appIdResolverLazy;
            ServiceProvider = serviceProvider;
        }

        #endregion

        public IContextOfSite Site() => _site ?? (_site = ServiceProvider.Build<IContextOfSite>());
        private IContextOfSite _site;

        public IContextOfApp App(int appId)
        {
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        public IContextOfBlock BlockRequired() => BlockContext ?? throw new Exception("Block context required but not known. It was not attached.");

        public IContextOfBlock BlockOrNull() => BlockContext;

        public IContextOfApp BlockOrApp(int appId) => BlockContext ?? App(appId);

        private IContextOfBlock BlockContext => _blockContext ?? (_blockContext = _getBlockContext?.Invoke());
        private IContextOfBlock _blockContext;

        public void AttachBlockContext(Func<IContextOfBlock> getBlockContext) => _getBlockContext = getBlockContext;
        private Func<IContextOfBlock> _getBlockContext;

        public void AttachRealBlock(Func<IBlock> getBlock) => _getBlock = getBlock;
        private Func<IBlock> _getBlock;

        public IBlock RealBlockOrNull() => _getBlock?.Invoke();

        public IBlock RealBlockRequired() => _getBlock?.Invoke() ?? throw new Exception("Block required but missing. It was not attached");


        public IContextOfApp App(string nameOrPath) => App(AppIdResolver.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, true));

        public IContextOfApp AppOrBlock(string nameOrPath) => AppOrNull(nameOrPath) ?? BlockRequired();

        public IContextOfApp AppOrNull(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
            var id = AppIdResolver.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, false);
            return id <= Eav.Constants.AppIdEmpty ? null : App(id);
        }

        public IContextOfApp AppNameRouteBlock(string nameOrPath)
        {
            var ctx = AppOrNull(nameOrPath);
            if (ctx != null) return ctx;

            var identity = AppIdResolver.GetAppIdFromRoute();
            if (identity != null)
            {
                ctx = ServiceProvider.Build<IContextOfApp>();
                ctx.Init(Log);
                ctx.ResetApp(identity);
                return ctx;
            }

            ctx = BlockOrNull();
            return ctx ?? throw new Exception($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
        }


    }
}

﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWebApiBackendBase<T>: WebApiBackendBase<BlockWebApiBackendBase<T>> where T: class
    {
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        protected IContextOfApp ContextOfAppOrBlock;
        protected IBlock _block;
        protected CmsManager CmsManager;

        protected BlockWebApiBackendBase(IServiceProvider sp, Lazy<CmsManager> cmsManagerLazy, string logName) : base(sp, logName)
        {
            _cmsManagerLazy = cmsManagerLazy;
        }

        public T Init(IContextOfApp context, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            ContextOfAppOrBlock = context;
            _block = block;
            CmsManager = context.AppState == null ? null : _cmsManagerLazy.Value.Init(context.AppState, context.UserMayEdit, Log);

            return this as T;
        }

        protected void ThrowIfNotAllowedInApp(List<Grants> requiredGrants, IAppIdentity alternateApp = null)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(ContextOfAppOrBlock, alternateApp ?? ContextOfAppOrBlock.AppState, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
        }
    }
}

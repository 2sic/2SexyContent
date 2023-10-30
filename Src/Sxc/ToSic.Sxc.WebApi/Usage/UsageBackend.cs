﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.AppSys;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Apps.CmsSys;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Usage
{
    public class UsageBackend: ServiceBase
    {
        private readonly AppWorkSxc _appWorkSxc;
        private readonly AppBlocks _appBlocks;
        private readonly Generator<MultiPermissionsApp> _appPermissions;
        private readonly IContextResolver _ctxResolver;

        public UsageBackend(
            AppWorkSxc appWorkSxc,
            AppBlocks appBlocks,
            Generator<MultiPermissionsApp> appPermissions,
            IContextResolver ctxResolver
            ) : base("Bck.Usage")
        {
            ConnectServices(
                _appPermissions = appPermissions,
                _ctxResolver = ctxResolver,
                _appWorkSxc = appWorkSxc,
                _appBlocks = appBlocks
            );
        }

        public IEnumerable<ViewDto> ViewUsage(int appId, Guid guid, Func<List<IView>, List<BlockConfiguration>, IEnumerable<ViewDto>> finalBuilder)
        {
            var wrapLog = Log.Fn<IEnumerable<ViewDto>>($"{appId}, {guid}");
            var context = _ctxResolver.GetBlockOrSetApp(appId);

            // extra security to only allow zone change if host user
            var permCheck = _appPermissions.New().Init(context, context.AppState);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var appSysCtx = _appWorkSxc.AppWork.Context(appId);
            var appViews = _appWorkSxc.AppViews(appSysCtx);
            // treat view as a list - in case future code will want to analyze many views together
            var views = new List<IView> { appViews.Get(guid) };

            var blocks = _appBlocks.AllWithView(appSysCtx);

            Log.A($"Found {blocks.Count} content blocks");

            var result = finalBuilder(views, blocks);

            return wrapLog.ReturnAsOk(result);
        }
    }
}

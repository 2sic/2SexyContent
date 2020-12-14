﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityPickerBackend: WebApiBackendBase<EntityPickerBackend>
    {
        #region DI Constructor

        public EntityPickerBackend(EntityPickerApi entityPickerApi, IContextResolver ctxResolver, IServiceProvider serviceProvider) : base(serviceProvider, "BE.EntPck")
        {
            _entityPickerApi = entityPickerApi;
            _ctxResolver = ctxResolver;
        }
        private readonly EntityPickerApi _entityPickerApi;
        private readonly IContextResolver _ctxResolver;

        #endregion

        public IEnumerable<EntityForPickerDto> GetAvailableEntities(int appId, string[] items, string contentTypeName)
        {
            var context = _ctxResolver.App(appId);
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName)
                ? ServiceProvider.Build<MultiPermissionsApp>().Init(context, context.AppState, Log)
                : ServiceProvider.Build<MultiPermissionsTypes>().Init(context, context.AppState, contentTypeName, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // maybe in the future, ATM not relevant
            var withDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);

            return _entityPickerApi.Init(Log).GetAvailableEntities(appId, items, contentTypeName, withDrafts);
        }
    }
}

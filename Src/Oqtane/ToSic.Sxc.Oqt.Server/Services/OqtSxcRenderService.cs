using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared.Helpers;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Services;

public class OqtSxcRenderService(
    IHttpContextAccessor accessor,
    IOqtSxcViewBuilder oqtSxcViewBuilder,
    IAliasRepository aliases,
    ISiteRepository sites,
    IPageRepository pages,
    IModuleRepository modules,
    IModuleDefinitionRepository definitions,
    ISettingRepository settings,
    IUserPermissions userPermissions,
    ILogManager logger,
    SiteState siteState) : IOqtSxcRenderService/*, ITransientService*/
{
    public Task<OqtViewResultsDto> PrepareAsync(int aliasId, int pageId, int moduleId, string culture, bool preRender, string originalParameters)
    {
        return Task.FromResult(Prepare(aliasId, pageId, moduleId, culture, preRender, originalParameters));
    }

    public OqtViewResultsDto Prepare(int aliasId, int pageId, int moduleId, string culture, bool preRender, string originalParameters)
    {
        try
        {
            var alias = aliases.GetAlias(aliasId);
            if (alias == null)
                return Forbidden("Unauthorized Alias Get Attempt {AliasId}", aliasId);

            // HACKS: STV POC - indirectly share information
            accessor?.HttpContext?.Items.TryAdd("AliasFor2sxc", alias);

            // Store Alias in SiteState for background processing.
            if (siteState != null) siteState.Alias = alias;

            // Set User culture
            if (culture != CultureInfo.CurrentUICulture.Name) OqtCulture.SetCulture(culture);

            var site = sites.GetSite(alias.SiteId);
            if (site == null)
                return Forbidden("Unauthorized Site Get Attempt {SiteId}", alias.SiteId);

            var page = pages.GetPage(pageId);
            if (page == null || page.SiteId != alias.SiteId /*|| !userPermissions.IsAuthorized(accessor?.HttpContext?.User, EntityNames.Page, pageId, PermissionNames.View)*/) // HACK: @STV, fix this
                return Forbidden("Unauthorized Page Get Attempt {pageId}", pageId);

            var module = modules.GetModule(moduleId);
            if (module == null || module.SiteId != alias.SiteId /*|| !userPermissions.IsAuthorized(accessor?.HttpContext?.User, "View", module.Permissions)*/) // HACK: @STV, fix this
                return Forbidden("Unauthorized Module Get Attempt {ModuleId}", moduleId);

            var moduleDefinitions = definitions.GetModuleDefinitions(module.SiteId).ToList();
            module.ModuleDefinition = moduleDefinitions.Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);

            module.Settings = settings.GetSettings(EntityNames.Module, moduleId).ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);

            return oqtSxcViewBuilder.Prepare(alias, site, page, module, preRender);
        }
        catch (Exception ex)
        {
            return Error(ex);
        }
    }

    private OqtViewResultsDto Forbidden(string message, params object[] args)
    {
        logger.Log(LogLevel.Error, this, LogFunction.Security, message, args);
        //if (accessor?.HttpContext != null) 
        //    accessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return null;
    }

    private OqtViewResultsDto Error(Exception ex)
    {
        logger.Log(LogLevel.Error, this, LogFunction.Read, ex, $"exception in {nameof(Prepare)}");
        return new() { 
            ErrorMessage = ErrorHelper.ErrorMessage(ex, IsSuperUser)
        };
    }

    private bool IsSuperUser => 
        (accessor?.HttpContext?.User.IsInRole(RoleNames.Host) ?? false)
        || (accessor?.HttpContext?.User.IsInRole(RoleNames.Admin) ?? false);
}
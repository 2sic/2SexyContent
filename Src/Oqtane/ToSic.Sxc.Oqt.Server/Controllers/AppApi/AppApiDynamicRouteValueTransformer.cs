﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Oqtane.Models;
using Oqtane.Repository;
using System;
using System.IO;
using System.Threading.Tasks;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /// <summary>
    /// Enable dynamically manipulating of route value to select a 2sxc app api dynamic code controller action.
    /// </summary>
    public class AppApiDynamicRouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Lazy<OqtAppFolder> _oqtAppFolderLazy;

        public AppApiDynamicRouteValueTransformer(
            ITenantResolver tenantResolver,
            IWebHostEnvironment hostingEnvironment,
            Lazy<OqtAppFolder> oqtAppFolderLazy)
        {
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            _oqtAppFolderLazy = oqtAppFolderLazy;
            Log = new Log(HistoryLogName, null, "AppApiDynamicRouteValueTransformer");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }

        protected string HistoryLogGroup { get; } = "app-api";

        protected string HistoryLogName => "Route.Values";

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var wrapLog = Log.Call<RouteValueDictionary>();

            #region Ensure required alias
            Alias alias;
            if (values.ContainsKey("alias"))
            {
                //var aliasId = int.Parse((string) values["alias"]);
                alias = _tenantResolver.GetAlias();
            }
            else
            {
                var serviceProvider = httpContext.RequestServices;
                var siteStateInitializer = serviceProvider.Build<SiteStateInitializer>();
                //var aliasRepositoryLazy = serviceProvider.Build<Lazy<IAliasRepository>>();
                siteStateInitializer.InitIfEmpty(); //siteState, httpContext, aliasRepositoryLazy);
                alias = siteStateInitializer.SiteState.Alias ?? throw new ArgumentException($"Error: missing required 'alias' route value.", nameof(values));
            }
            var aliasPart = string.Format(OqtConstants.AppRootPublicBase, alias.SiteId);
            #endregion

            // Ensure required route values: alias, appFolder, controller, action.
            if (!values.ContainsKey("appFolder")) throw new ArgumentException($"Error: missing required 'appFolder' route value.", nameof(values));
            var appFolder = (string)values["appFolder"];
            if (appFolder == WebApiConstants.Auto) appFolder = _oqtAppFolderLazy.Value.GetAppFolder();


            if (!values.ContainsKey("controller")) throw new ArgumentException($"Error: missing required 'controller' route value.", nameof(values));
            var controller = (string)values["controller"];

            if (!values.ContainsKey("action")) throw new ArgumentException($"Error: missing required 'action' route value.", nameof(values));
            var action = (string)values["action"];

            Log.Add($"TransformAsync route required values are present, alias:{alias.AliasId}, app:{appFolder}, ctrl:{controller}, act:{action}.");

            var controllerTypeName = $"{controller}Controller";
            Log.Add($"Controller TypeName: {controllerTypeName}");
            values.Add("controllerTypeName", controllerTypeName);

            var edition = GetEdition(values);
            Log.Add($"Edition: {edition}");


            var controllerFolder = Path.Combine(aliasPart, appFolder, edition.Backslash(), "api");
            Log.Add($"Controller Folder: {controllerFolder}");

            var area = $"{alias.SiteId}/{OqtConstants.ApiAppLinkPart}/{appFolder}/{edition}api";
            Log.Add($"Area: {area}");
            values.Add("area", area);

            var controllerPath = Path.Combine(controllerFolder, controllerTypeName + ".cs");
            Log.Add($"Controller Path: {controllerPath}");

            var apiFile = Path.Combine(_hostingEnvironment.ContentRootPath, controllerPath);
            Log.Add($"Absolute Path: {apiFile}");
            values.Add("apiFile", apiFile);

            var dllName = $"DynCode_{controllerFolder.Replace(@"\", "_")}_{System.IO.Path.GetFileNameWithoutExtension(apiFile)}";
            Log.Add($"Dll Name: {dllName}");
            values.Add("dllName", dllName);

            // help with path resolution for compilers running inside the created controller
            httpContext.Request?.HttpContext.Items.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

            return wrapLog($"ok, TransformAsync route required values are prepared", values);
        }

        private static string GetEdition(RouteValueDictionary values)
        {
            // new for 2sxc 9.34 #1651
            var edition = "";
            if (values.ContainsKey("edition")) edition = values["edition"].ToString();
            if (!string.IsNullOrEmpty(edition)) edition += "/";
            return edition;
        }
    }
}

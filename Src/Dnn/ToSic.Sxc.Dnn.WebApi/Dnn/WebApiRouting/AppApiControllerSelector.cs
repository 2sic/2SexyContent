﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Sys;

namespace ToSic.Sxc.Dnn.WebApiRouting
{
    /// <inheritdoc />
    /// <summary>
    /// This controller will check if it's responsible (based on url)
    /// ...and if yes, compile / run the app-specific api controllers
    /// ...otherwise hand processing back to next api controller up-stream
    /// </summary>
    public class AppApiControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _config;
        public IHttpControllerSelector PreviousSelector { get; set; }

        public AppApiControllerSelector(HttpConfiguration configuration)
        {
            _config = configuration;
        }




        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping() => PreviousSelector.GetControllerMapping();

        private static readonly string[] AllowedRoutes = {"desktopmodules/2sxc/api/app-api/", "api/2sxc/app-api/"}; // old routes, dnn 7/8 & dnn 9 


        // new in 2sxc 9.34 #1651 - added "([^/]+\/)?" to allow an optional edition parameter
        private static readonly string[] RegExRoutes =
        {
            @"desktopmodules\/2sxc\/api\/app\/[^/]+\/([^/]+\/)?api",
            @"api\/2sxc\/app\/[^/]+\/([^/]+\/)?api"
        };

        private const string ApiErrPrefix = "2sxc Api Controller Finder Error: ";

        private const string ApiErrGeneral = "Error selecting / compiling an API controller. ";
        private const string ApiErrSuffix = "Check event-log, code and inner exception. ";


        /// <summary>
        /// Verify if this request is one which should be handled by this system
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if we want to handle it</returns>
        private bool HandleRequestWithThisController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            var simpleMatch = AllowedRoutes.Any(a => routeData.Route.RouteTemplate.ToLowerInvariant().Contains(a));
            if (simpleMatch)
                return true;

            var rexMatch = RegExRoutes.Any(
                a => new Regex(a, RegexOptions.None).IsMatch(routeData.Route.RouteTemplate.ToLowerInvariant()) );
            return rexMatch;

        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            // Log this lookup and add to history for insights
            var log = new Log("Sxc.Http", null, request?.RequestUri?.AbsoluteUri);
            AddToInsightsHistory(request?.RequestUri?.AbsoluteUri, log);

            var wrapLog = log.Call<HttpControllerDescriptor>();

            if (!HandleRequestWithThisController(request))
                return wrapLog("upstream", PreviousSelector.SelectController(request));

            var routeData = request.GetRouteData();
            var controllerTypeName = routeData.Values[Names.Controller] + "Controller";
            
            // Now Handle the 2sxc app-api queries

            // 1. Figure out the Path, or show error for that
            string appFolder = null;
            try
            {
                appFolder = Route.AppPathOrNull(routeData);

                // only check for app folder if we don't have a context
                if (appFolder == null)
                {
                    log.Add("no folder found in url, will auto-detect");
                    var block = Eav.Factory.StaticBuild<DnnGetBlock>().GetCmsBlock(request, log);
                    appFolder = block?.App?.Folder;
                }

                log.Add($"App Folder: {appFolder}");
            }
            catch (Exception getBlockException)
            {
                const string msg = ApiErrPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + ApiErrSuffix;
                throw ReportToLogAndThrow(request, HttpStatusCode.BadRequest, getBlockException, msg, wrapLog);
            }

            if (string.IsNullOrWhiteSpace(appFolder))
            {
                const string msg = ApiErrPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                                   "and tried app-detection using url-params/headers pageid/moduleid. " + ApiErrSuffix;
                throw ReportToLogAndThrow(request, HttpStatusCode.BadRequest, new Exception(), msg, wrapLog);
            }

            var controllerPath = "";
            try
            {
                // new for 2sxc 9.34 #1651
                var edition = "";
                if (routeData.Values.ContainsKey(Names.Edition))
                    edition = routeData.Values[Names.Edition].ToString();
                if (!string.IsNullOrEmpty(edition))
                    edition += "/";

                log.Add($"Edition: {edition}");

                var tenant = Eav.Factory.StaticBuild<DnnSite>();
                var controllerFolder = Path.Combine(tenant.AppsRootRelative, appFolder, edition + "api/");

                controllerFolder = controllerFolder.Replace("\\", @"/");
                log.Add($"Controller Folder: {controllerFolder}");

                controllerPath = Path.Combine(controllerFolder + controllerTypeName + ".cs");
                log.Add($"Controller Path: {controllerPath}");

                // note: this may look like something you could optimize/cache the result, but that's a bad idea
                // because when the file changes, the type-object will be different, so please don't optimize :)
                if (File.Exists(HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType(controllerTypeName, true, true);

                    // help with path resolution for compilers running inside the created controller
                    request?.Properties.Add(CodeCompiler.SharedCodeRootPathKeyInCache, controllerFolder);

                    var descriptor = new HttpControllerDescriptor(_config, type.Name, type);
                    return wrapLog("ok", descriptor);
                }

                log.Add("path not found, error will be thrown in a moment");
            }
            catch (Exception e)
            {
                var msg = ApiErrPrefix + ApiErrGeneral + ApiErrSuffix;
                throw ReportToLogAndThrow(request, HttpStatusCode.InternalServerError, e, msg, wrapLog);
            }

            var msgfinal = $"2sxc Api Controller Finder: Controller {controllerTypeName} not found in app. " +
                           $"We checked the virtual path '{controllerPath}'";
            throw ReportToLogAndThrow(request, HttpStatusCode.NotFound, new Exception(), msgfinal, wrapLog);
        }

        private static HttpResponseException ReportToLogAndThrow(HttpRequestMessage request, HttpStatusCode code, Exception e, string msg, Func<string, HttpControllerDescriptor, HttpControllerDescriptor> wrapLog)
        {
            var helpText = ErrorHelp.HelpText(e);
            var exception = new Exception(msg + helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
            wrapLog("error", null);
            return new HttpResponseException(request.CreateErrorResponse(code, exception.Message, e));
        }

        private static void AddToInsightsHistory(string url, ILog log)
        {
            var addToHistory = true;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!InsightsController.InsightsLoggingEnabled)
                if (url?.Contains(InsightsController.InsightsUrlFragment) ?? false)
                    addToHistory = false;
            if (addToHistory) History.Add("http-request", log);
        }
    }
}
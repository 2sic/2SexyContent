﻿using System;
using System.Configuration;
using System.Web.Hosting;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using ToSic.Eav.Caching;
using ToSic.Eav.Configuration;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Plumbing;
using ToSic.SexyContent.Dnn920;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace ToSic.Sxc.Dnn.StartUp
{
    /// <summary>
    /// This configures .net Core Dependency Injection
    /// The StartUp is defined as an IServiceRouteMapper.
    /// This way DNN will auto-run this code before anything else
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class StartupDnn : IServiceRouteMapper
    {
        /// <summary>
        /// This will be called by DNN when loading the assemblies.
        /// We just want to trigger the DI-Configure
        /// </summary>
        /// <param name="mapRouteManager"></param>
        public void RegisterRoutes(IMapRoute mapRouteManager) => Configure();


        private static bool _alreadyConfigured;

        /// <summary>
        /// Configure IoC for 2sxc. If it's already configured, do nothing.
        /// </summary>
        public void Configure()
        {
            // In some cases this may be called 2x - so we must avoid doing it again
            if (_alreadyConfigured) return;

            // Register Services if this has not happened yet
            // In Dnn9.4+ this was already done before
            // In older Dnn this didn't happen yet, so this is the latest it can happen
            DnnDi.RegisterServices(null);

            // Now activate the Service Provider, because some Dnn code still needs the static implementation
            DnnStaticDi.StaticDiReady();


            // Configure Newtonsoft Time zone handling
            // Moved here in v12.05 - previously it was in the Pre-Serialization converter
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Getting the service provider in Configure is tricky business, because
            // of .net core 2.1 bugs
            // ATM it appears that the service provider will get destroyed after startup, so we MUST get an additional one to use here
            var initialServiceProvider = DnnStaticDi.GetServiceProvider();
            var transientSp = initialServiceProvider;//.Build<IServiceProvider>();

            // now we should be able to instantiate registration of DB
            transientSp.Build<IDbConfiguration>().ConnectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var globalConfig = transientSp.Build<IGlobalConfiguration>();

            globalConfig.GlobalFolder = HostingEnvironment.MapPath(DnnConstants.SysFolderRootVirtual);
            globalConfig.GlobalSiteFolder = "~/Portals/_default/";

            // Load features from configuration
            var sysLoader = transientSp.Build<SystemLoader>();
            sysLoader.StartUp();

            // 2021-11-16 2dm - experimental, working on moving global/preset data into a normal AppState #PresetInAppState
            sysLoader.Log.Add("Try to load global app-state");
            var globalStateLoader = transientSp.Build<FileAppStateLoaderWIP>();
            var appState = globalStateLoader.AppState(Eav.Constants.PresetAppId);
            var appsMemCache = transientSp.Build<IAppsCache>();
            appsMemCache.Add(appState);
            // End experimental #PresetInAppState

            // also register this because of a long DNN issue which was fixed, but we don't know if we're running in another version
            SharpZipLibRedirect.RegisterSharpZipLibRedirect();

            // Help RazorBlade to have a proper best-practices ToJson
            // New v12.05
            Razor.Internals.StartUp.RegisterToJson(JsonConvert.SerializeObject);

            _alreadyConfigured = true;
        }
    }
}
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Block;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    // ReSharper disable once InconsistentNaming
    static partial class OqtaneDI
    {
        public static IServiceCollection AddSxcOqtane(this IServiceCollection services)
        {
            services.AddScoped<ILinkPaths, OqtLinkPaths>();
            services.TryAddTransient<IServerPaths, OqtServerPaths>();

            services.TryAddScoped<ISite, OqtSite>();
            services.TryAddScoped<IPage, OqtPage>();
            services.TryAddScoped<OqtSite>();
            services.TryAddScoped<IUser, OqtUser>();
            services.TryAddTransient<IModule, OqtModule>();
            services.TryAddTransient<OqtModule>();
            services.TryAddTransient<OqtTempInstanceContext>();

            services.TryAddTransient<IZoneCultureResolver, OqtSite>();
            services.TryAddTransient<IZoneMapper, OqtZoneMapper>();
            services.TryAddTransient<OqtZoneMapper>();
            services.TryAddTransient<AppPermissionCheck, OqtPermissionCheck>();
            services.TryAddTransient<ILinkHelper, OqtLinkHelper>();
            services.TryAddTransient<DynamicCodeRoot, OqtaneDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, OqtModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, OqtEnvironmentInstaller>();
            services.TryAddTransient<ILookUpEngineResolver, OqtGetLookupEngine>();
            services.TryAddTransient<IFingerprint, OqtFingerprintWip>();
            services.TryAddTransient<IUiContextBuilder, OqtUiContextBuilder>();
            services.TryAddTransient<OqtCulture>();
            services.TryAddTransient<SettingsHelper>();

            //// add page publishing
            services.TryAddTransient<IPagePublishing, OqtPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, OqtPagePublishingResolver>();

            //// Oqtane Specific stuff
            services.AddScoped<OqtAssetsAndHeaders>();
            services.TryAddTransient<OqtSxcViewBuilder>();
            services.TryAddTransient<IClientDependencyOptimizer, OqtClientDependencyOptimizer>();
            services.TryAddTransient<IValueConverter, OqtValueConverter>();

            services.AddSingleton<IPlatform, OqtPlatformContext>();

            // ADAM stuff
            services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
            services.TryAddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();
            //services.TryAddTransient(typeof(AdamItemDtoMaker<,>), typeof(OqtAdamItemDtoMaker<,>));

            //// Still pending...
            services.TryAddTransient<XmlExporter, OqtXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();
            //sc.TryAddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.TryAddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // 2020-10-22 2dm test
            services.TryAddTransient<ISxcOqtane, OqtSxcViewBuilder>();
            services.TryAddTransient<StatefulControllerDependencies>();

            // Site State Initializer for APIs etc. to ensure that the SiteState exists and is correctly preloaded
            services.TryAddTransient<SiteStateInitializer>();

            // Experimental - it seems that Blazor hides the url params in the request
            services.TryAddTransient<IHttp, HttpBlazor>();

            // Resolve appFolder when appName is "auto"
            services.TryAddTransient<OqtAppFolder>();
            services.TryAddTransient<AppAssetsDependencies>();

            // Asset Templates
            services.TryAddTransient<IAssetTemplates, OwtAssetTemplates>();

            // Lookup
            services.TryAddTransient<QueryStringLookUp>();
            services.TryAddTransient<SiteLookUp>();
            services.TryAddTransient<PageLookUp>();
            services.TryAddTransient<ModuleLookUp>();
            services.TryAddScoped<UserLookUp>();

            // Polymorphism Resolvers
            services.TryAddTransient<Sxc.Polymorphism.Koi>();
            services.TryAddTransient<Polymorphism.Permissions>();


            // new in v12 - .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetCore>();

            // new in v12 - integrate KOI - experimental!
            try
            {
                services.ActivateKoi2Di();
            }
            catch { /* ignore */ }


            return services;
        }



    }
}

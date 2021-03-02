﻿using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Oqtane.Infrastructure;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.RazorPages;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Razor.Engine;
using ToSic.Sxc.WebApi;
using Factory = ToSic.Eav.Factory;

namespace ToSic.Sxc.Oqt.Server
{
    class Startup : IServerStartup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; set; }


        public Startup()
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();

            var devMode = Configuration["DevMode"];
            if (devMode == "SPM") TestIds.Dev4Spm = true;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            // try to enable dynamic razor compiling - still WIP
            new StartUpRazorPages().ConfigureServices(services);

            // enable webapi - include all controllers in the Sxc.Mvc assembly
            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                // This is needed to preserve compatibility with previous api usage
                .AddNewtonsoftJson(options =>
                {
                    // this ensures that c# objects with Pascal-case keep that
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    Eav.ImportExport.Json.JsonSettings.Defaults(options.SerializerSettings);
                });
                //.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new RemoteControllerFeatureProvider(services)));

            // enable use of UrlHelper for AbsolutePath
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcOqtane()
                    .AddSxcRazor()
                    .AddAdamWebApi<int, int>()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });

            var sp = services.BuildServiceProvider();
            // STV
            // var connectionString = Configuration.GetConnectionString("DefaultConnection");
            // 2dm
            var connectionString = Configuration.GetConnectionString("SiteSqlServer");
            sp.Build<IDbConfiguration>().ConnectionString = connectionString;
            var hostingEnvironment = sp.Build<IHostEnvironment>();
            sp.Build<IGlobalConfiguration>().GlobalFolder = Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Modules\\ToSic.Sxc");

            // 2sxc Oqtane blob services for Imageflow.
            services.AddImageflowOqtaneBlobService();
            // WIP: stv
            //services.AddConditionalMiddlewareBeforeEndpoints();
            services.AddScoped<AppApiDynamicRouteValueTransformer>();
            services.AddSingleton<IActionDescriptorChangeProvider>(AppApiActionDescriptorChangeProvider.Instance);
            services.AddSingleton(AppApiActionDescriptorChangeProvider.Instance);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HostEnvironment = env;
            //_contentRootPath = env.ContentRootPath;

            // WIP: stv - this is hacky :-(
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDynamicControllerRoute<AppApiDynamicRouteValueTransformer>("{alias}/api/sxc/app/{appFolder}/{edition}/api/{controller}/{action}");
                endpoints.MapDynamicControllerRoute<AppApiDynamicRouteValueTransformer>("{alias}/api/sxc/app/{appFolder}/api/{controller}/{action}");
            });
        }

        // Workaround because of initialization issues with razor pages
        //private static string _contentRootPath;

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            //throw new NotImplementedException();
        }


    }
}

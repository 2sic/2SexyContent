﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.Engine.DbgWip;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Razor.Engine
{
    // ReSharper disable once InconsistentNaming
    public static class StartupRazor
    {
        public static IServiceCollection AddSxcRazor(this IServiceCollection services)
        {
            // .net Core parts
            services.TryAddTransient<IHttp, HttpNetCore>();

            // Razor Parts
            services.TryAddTransient<IRazorCompiler, RazorCompiler>();
            services.TryAddTransient<IRazorRenderer, RazorRenderer>();
            services.TryAddTransient<IEngineFinder, RazorEngineFinder>();

            // debugging
            services.TryAddTransient<RazorReferenceManager>();

            return services;
        }
    }
}

﻿using Imazen.Common.Storage;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Imageflow.Oqt.Server;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow;

public static class ImageflowExtensions
{
    public static IServiceCollection AddImageflowExtensions(this IServiceCollection services)
    {
        services.AddSingleton<IBlobProvider>((container) => new OqtaneBlobService(container));
        services.AddSingleton<ImageflowRewriteMiddleware>();
        services.AddSingleton<IPreregisterImageFlowMiddleware, PreregisterImageFlowMiddleware>(); // query string rewriting middleware

        return services;
    }
}
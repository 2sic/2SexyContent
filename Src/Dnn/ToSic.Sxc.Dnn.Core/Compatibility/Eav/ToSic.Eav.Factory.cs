﻿using Microsoft.Extensions.DependencyInjection;
using System;
using ToSic.Eav.Obsolete;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Dnn;
using static ToSic.Eav.Obsolete.CodeChangeInfo;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav
{
    /// <summary>
    /// The Eav DI Factory, used to construct various objects through [Dependency Injection](xref:NetCode.DependencyInjection.Index).
    ///
    /// If possible avoid using this, as it's a workaround for code which is outside of the normal Dependency Injection and therefor a bad pattern.
    /// </summary>
    [PublicApi("Careful - obsolete!")]
    [Obsolete("Deprecated, please use Dnn 9 DI instead https://go.2sxc.org/brc-13-eav-factory")]
	public class Factory
	{
        [Obsolete("Not used any more, but keep for API consistency in case something calls ActivateNetCoreDi")]
        [PrivateApi]
        public delegate void ServiceConfigurator(IServiceCollection service);

        [PrivateApi("Removed v13.02 - should not be in use, completely remove ca. July 2022")]
	    public static void ActivateNetCoreDi(ServiceConfigurator configure) =>
            DnnStaticDi.CodeChanges.Warn(V13Removed(nameof(ActivateNetCoreDi), "https://go.2sxc.org/brc-13-eav-factory-startup"));

        /// <summary>
        /// Dependency Injection resolver with a known type as a parameter.
        /// </summary>
        /// <typeparam name="T">The type / interface we need.</typeparam>
        [Obsolete("Please use standard Dnn 9.4+ Dnn DI instead https://go.2sxc.org/brc-13-eav-factory")]
        public static T Resolve<T>()
        {
            DnnStaticDi.CodeChanges.Warn(WarnObsolete.UsedAs(specificId: typeof(T).FullName));
            return DnnStaticDi.StaticBuild<T>();
        }

        private static readonly ICodeChangeInfo WarnObsolete = V13To17("ToSic.Eav.Factory.Resolve<T>", "https://go.2sxc.org/brc-13-eav-factory");
    }
}
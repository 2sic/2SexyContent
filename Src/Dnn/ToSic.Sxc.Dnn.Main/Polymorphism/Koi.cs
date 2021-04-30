﻿using Connect.Koi;
using ToSic.Eav.Logging;
using static System.StringComparison;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Polymorphism
{
    [PolymorphResolver("Koi")]

    public class Koi: IResolver
    {
        private readonly ICss _pageCss;
        public string Name => "Koi";

        public const string ModeCssFramework= "cssFramework";

        public Koi(ICss pageCss)
        {
            _pageCss = pageCss;
        }

        public string Edition(string parameters, ILog log)
        {
            var wrapLog = log.Call<string>(parameters);
            if (!string.Equals(parameters, ModeCssFramework, InvariantCultureIgnoreCase)) 
                return wrapLog("unknown param", null);
            // Note: this is still using the global object which we want to get rid of
            // But to use DI, we must refactor Polymorphism
            var cssFramework = _pageCss.Framework; // Connect.Koi.Koi.Css;
            return wrapLog(cssFramework, cssFramework);
        }
    }
}

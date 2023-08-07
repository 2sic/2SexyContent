﻿using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static System.StringComparison;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicJacketBase: ITyped
    {
        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? strict)
        {
            Protect(noParamOrder, nameof(fallback));
            var value = FindValueOrNull(name, InvariantCultureIgnoreCase, null);
            var strValue = WrapperFactory.ConvertForCode.ForCode(value, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        [PrivateApi]
        bool ITyped.ContainsKey(string name) => TypedHasImplementation(name);

        [PrivateApi]
        protected abstract bool TypedHasImplementation(string name);

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? strict)
        {
            Protect(noParamOrder, nameof(strict));
            return FindValueOrNull(name, InvariantCultureIgnoreCase, null);
        }

        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default) 
            => G4T(name, noParamOrder, fallback);

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? strict) 
            => G4T(name, noParamOrder, fallback);

        [PrivateApi]
        private TValue G4T<TValue>(string name,
            string noParamOrder = Protector,
            TValue fallback = default,
            [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var result = FindValueOrNull(name, InvariantCultureIgnoreCase, null);
            return result.ConvertOrFallback(fallback);
        }

        [PrivateApi]
        dynamic ITyped.Dyn => this;

        [PrivateApi] 
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? strict, bool scrubHtml)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Tags.Strip(value) : value;
#pragma warning restore CS0618
        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? strict)
        {
            var url =  G4T(name, noParamOrder: noParamOrder, fallback: fallback);
            return Tags.SafeUrl(url).ToString();
        }

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));

    }
}

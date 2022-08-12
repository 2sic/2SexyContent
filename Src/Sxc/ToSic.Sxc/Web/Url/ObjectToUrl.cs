﻿using System;
using System.Collections;
using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Url
{
    public class ObjectToUrl
    {
        public delegate NameObjectSet ValueHandler(NameObjectSet set);

        public ObjectToUrl() { }

        public ObjectToUrl(string prefix = null, ValueHandler customHandler = null): this()
        {
            _customHandler = customHandler;
            Prefix = prefix;
        }
        private readonly ValueHandler _customHandler;

        private string Prefix { get; }
        public string ArraySeparator { get; set; } = ",";
        public string DepthSeparator { get; set; } = ":";
        public string PairSeparator { get; set; } = UrlParts.ValuePairSeparator.ToString();

        public string KeyValueSeparator { get; set; } = "=";


        public string Serialize(object data) => Serialize(data, Prefix);


        public string SerializeIfNotString(object data, string prefix = null)
        {
            if (data == null) return null;
            if (data is string str) return str;
            return Serialize(data, prefix);
        }


        public string SerializeWithChild(object main, object child, string childPrefix)
        {
            var uiString = SerializeIfNotString(main);
            if (child == null) return uiString;
            childPrefix = childPrefix ?? ""; // null catch
            var prefillAddOn = "";
            if (child is string strPrefill)
            {
                var parts = strPrefill.Split(UrlParts.ValuePairSeparator)
                    .Where(p => p.HasValue())
                    .Select(p => p.StartsWith(childPrefix) ? p : childPrefix + p);
                prefillAddOn = string.Join(UrlParts.ValuePairSeparator.ToString(), parts);
            }
            else
                prefillAddOn = SerializeIfNotString(child, childPrefix);

            return UrlParts.ConnectParameters(uiString, prefillAddOn);
        }

        private UrlValuePair ValueSerialize(NameObjectSet set)
        {
            if (_customHandler != null)
            {
                set = _customHandler(set);
                if (!set.Keep) return null; // new UrlValuePair(set.FullName, null);
            }

            if (set.Value == null) return null; // new UrlValuePair(set.FullName, null);
            if (set.Value is string strValue) return new UrlValuePair(set.FullName, strValue);

            var valueType = set.Value.GetType();

            // Check array - not sure yet if we care
            if (set.Value is IEnumerable enumerable)
            {
                var isGeneric = valueType.IsGenericType;
                var valueElemType = isGeneric
                    ? valueType.GetGenericArguments()[0]
                    : valueType.GetElementType();

                if (valueElemType == null) throw new ArgumentNullException(
                    $"The field: '{set.FullName}', isGeneric: {isGeneric} with base type {valueType} to add to url seems to have a confusing setup");

                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                    return new UrlValuePair(set.FullName, string.Join(ArraySeparator, enumerable.Cast<object>()));

                return new UrlValuePair(set.FullName, "array-like-but-unclear-what");
            }

            return valueType.IsSimpleType()
                // Simple type - just serialize, except for bool, which should be lower-cased
                ? new UrlValuePair(set.FullName,
                    set.Value is bool ? set.Value.ToString().ToLowerInvariant() : set.Value.ToString())
                // Complex object, recursive serialize with current name as prefix
                : new UrlValuePair(null, Serialize(set.Value, set.FullName + DepthSeparator), true);
        }

        // https://ole.michelsen.dk/blog/serialize-object-into-a-query-string-with-reflection/
        // https://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
        public string Serialize(object objToConvert, string prefix)
        {
            if (objToConvert == null)
                throw new ArgumentNullException(nameof(objToConvert));

            // Get all properties on the object
            var properties = objToConvert.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Select(x => ValueSerialize(new NameObjectSet(x.Name, x.GetValue(objToConvert, null), prefix)))
                .Where(x => x?.Value != null)
                .ToList();

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join(PairSeparator, properties.Select(p => p.ToString()));

        }

    }
}

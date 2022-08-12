﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Url
{
    public class ObjectToUrl
    {
        public ObjectToUrl() { }

        public ObjectToUrl(string prefix = null, IEnumerable<UrlValueProcess> preProcessors = null): this()
        {
            Prefix = prefix;
            _preProcessors = preProcessors;
        }

        private readonly IEnumerable<UrlValueProcess> _preProcessors;

        private string Prefix { get; }
        public string ArraySeparator { get; set; } = ",";
        public string DepthSeparator { get; set; } = ":";
        public string PairSeparator { get; set; } = UrlParts.ValuePairSeparator.ToString();

        public string KeyValueSeparator { get; set; } = "=";


        public string Serialize(object data) => SerializeInternal(data, Prefix);


        public string SerializeIfNotString(object data, string prefix = null)
        {
            if (data == null) return null;
            if (data is string str) return str;
            return SerializeInternal(data, prefix);
        }


        public string SerializeWithChild(object main, object child, string childPrefix = null)
        {
            var asString = SerializeIfNotString(main);
            if (child == null) return asString;
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

            return UrlParts.ConnectParameters(asString, prefillAddOn);
        }

        private UrlValuePair ValueSerialize(NameObjectSet set)
        {
            if (_preProcessors?.Any() == true)
            {
                foreach (var pP in _preProcessors)
                {
                    set = pP.Process(set);
                    if (!set.Keep) return null;
                }
            }

            if (set.Value == null) return null;
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
                : new UrlValuePair(null, SerializeInternal(set.Value, set.FullName + DepthSeparator), true);
        }

        // https://ole.michelsen.dk/blog/serialize-object-into-a-query-string-with-reflection/
        // https://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
        private string SerializeInternal(object data, string prefix)
        {
            if (data == null) return null;
            if (data is string str) return str;
            //if (data == null)
            //    throw new ArgumentNullException(nameof(data));

            // Get all properties on the object
            var properties = data.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Select(x => ValueSerialize(new NameObjectSet(x.Name, x.GetValue(data, null), prefix)))
                .Where(x => x?.Value != null)
                .ToList();

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join(PairSeparator, properties.Select(p => p.ToString()));

        }

    }
}

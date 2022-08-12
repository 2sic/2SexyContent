﻿using System;
using System.Collections;
using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Url
{
    public class ObjectToUrl
    {
        public delegate (bool Keep, string name, object Value) ValueHandler(string name, object value);

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

        private UrlValuePair ValueSerialize(string name, object value)
        {
            if (_customHandler != null)
            {
                var (keep, newName, newValue) = _customHandler(name, value);
                if (!keep) return new UrlValuePair(name, null);
                name = newName;
                value = newValue;
            }

            if (value == null) return new UrlValuePair(name, null);
            if (value is string strValue) return new UrlValuePair(name, strValue);

            var valueType = value.GetType();

            // Check array - not sure yet if we care
            if (value is IEnumerable enumerable)
            {
                var isGeneric = valueType.IsGenericType;
                var valueElemType = isGeneric
                    ? valueType.GetGenericArguments()[0]
                    : valueType.GetElementType();

                if (valueElemType == null) throw new ArgumentNullException(
                    $"The field: '{name}', isGeneric: {isGeneric} with base type {value.GetType()} to add to url seems to have a confusing setup");

                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                    return new UrlValuePair(name, string.Join(ArraySeparator, enumerable.Cast<object>()));

                return new UrlValuePair(name, "array-like-but-unclear-what");
            }

            return valueType.IsSimpleType() 
                ? new UrlValuePair(name, value is bool ? value.ToString().ToLowerInvariant() : value.ToString()) 
                : new UrlValuePair(null, Serialize(value, name + DepthSeparator), true);
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
                .Select(x => ValueSerialize(prefix + x.Name, x.GetValue(objToConvert, null)))
                .Where(x => x.Value != null)
                .ToList();

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join(PairSeparator, properties.Select(p => p.ToString()));

        }

    }
}

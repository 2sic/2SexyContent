﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {
        private const string NameOfAsTyped = nameof(IDynamicCode16.AsTyped) + "(...)";
        private const string NameOfAsTypedList = nameof(IDynamicCode16.AsTypedList) + "(...)";

        public ITypedRead AsTypedPure(object original, bool required = false, string detailsMessage = default)
        {
            var l = Log.Fn<ITypedRead>();

            if (AsTypedPreflightReturnNull(original, NameOfAsTyped, required, detailsMessage))
                return l.ReturnNull();

            if (original is ITypedRead alreadyTyped)
                return l.Return(alreadyTyped, "already typed");

            var result = DynamicHelpers.WrapIfPossible(original, true, true, false);
            if (result is ITypedRead resTyped)
                return l.Return(resTyped, "converted to dyn-read");

            throw l.Done(new ArgumentException($"Can't wrap/convert the original '{original.GetType()}'"));
        }

        public IEnumerable<ITypedRead> AsTypedListPure(object list, bool? required = false)
        {
            var l = Log.Fn<IEnumerable<ITypedRead>>();

            if (AsTypedPreflightReturnNull(list, NameOfAsTypedList, required == true))
                return l.ReturnNull();

            if (list is IEnumerable<ITypedRead> alreadyTyped)
                return l.Return(alreadyTyped, "already typed");

            if (!(list is IEnumerable enumerable))
                throw new ArgumentException($"The object provided to {NameOfAsTypedList} is not enumerable/array so it can't be converted.", nameof(list));

            var itemsRequired = required != false;
            var result = enumerable
                .Cast<object>()
                .Select((o, i) => AsTypedPure(o, itemsRequired, $"index: {i}"))
                .ToList();

            return result;

            //var result = DynamicHelpers.WrapIfPossible(list, true, true, false);
            //if (result is IEnumerable<ITypedRead> resTyped)
            //    return l.Return(resTyped, "converted to dyn-read");

            //throw l.Done(new ArgumentException($"Can't wrap/convert the original '{list.GetType()}'"));
        }

        private bool AsTypedPreflightReturnNull(object original, string methodName, bool required, string detailsMessage = default)
        {
            var l = Log.Fn<bool>();
            switch (original)
            {
                case null:
                    return required 
                        ? throw l.Done(new ArgumentException($"Tried to convert using {methodName} but got null, with {nameof(required)} = {required}. {detailsMessage}"))
                        : l.ReturnTrue();
                case string _:
                    throw l.Done(new ArgumentException($"Tried to convert using {methodName} but got a string." +
                                                       $"If you want to convert a string JSON to an easier-to-use object, use Kit.Json.ToTyped(...) instead. {detailsMessage}"));
                case ICanBeEntity _:
                    throw l.Done(new ArgumentException(
                        $"Tried to convert using {methodName} but got an entity-like object." +
                        $"If you want to convert an Entity-Like object, use AsItem(...) instead. {detailsMessage}"));
                default:
                    // Check value types - note that it won't catch strings, but these were handled above
                    if (original.GetType().IsValueType)
                        throw l.Done(new ArgumentException(
                            $"Tried to convert using {methodName} but got value type: '{original.GetType()}'. This can't be converted. {detailsMessage}"));

                    return l.ReturnFalse();
            }
        }

    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data.Wrapper;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class CodeDataFactory
    {
        public const int MaxRecursions = 3;

        #region AsTyped Implementations


        public ITypedItem AsItem(object data, string noParamOrder, bool? required = default, ITypedItem fallback = default, bool? strict = default, bool? mock = default)
        {
            Protect(noParamOrder, $"{nameof(fallback)}, {nameof(strict)}, {nameof(mock)}");
            if (mock == true)
                return _codeDataWrapper.Value.TypedItemFromObject(data,
                    WrapperSettings.Typed(true, true, strict ?? true),
                    new LazyLike<CodeDataFactory>(this));

            return AsItemInternal(data, MaxRecursions, strict: strict ?? false) ?? fallback;
        }

        private ITypedItem AsItemInternal(object target, int recursions, bool strict)
        {
            var l = Log.Fn<ITypedItem>();
            if (recursions <= 0)
                throw l.Done(new Exception($"Conversion with {nameof(AsItem)} failed, max recursions reached"));

            ITypedItem ConvertOrNullAndLog(IEntity e, string typeName) => e == null
                ? l.ReturnNull($"empty {typeName}")
                : l.Return(new DynamicEntity(e, this, strict: strict), typeName);

            switch (target)
            {
                case null:
                    return l.ReturnNull("null");
                case string _:
                    throw l.Done(new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(ITypedItem)}"));
                case ITypedItem alreadyCmsItem:
                    return ConvertOrNullAndLog(alreadyCmsItem.Entity, nameof(ITypedItem));
                //case IDynamicEntity dynEnt:
                //    return l.Return(new TypedItem(dynEnt), nameof(IDynamicEntity));
                case IEntity entity:
                    return ConvertOrNullAndLog(entity, nameof(IEntity));
                case ICanBeEntity canBeEntity:
                    return ConvertOrNullAndLog(canBeEntity.Entity, nameof(ICanBeEntity));
                case IDataSource ds:
                    return ConvertOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataSource));
                case IDataStream ds:
                    return ConvertOrNullAndLog(ds.List.FirstOrDefault(), nameof(IDataStream));
                case IEnumerable<IEntity> entList:
                    return ConvertOrNullAndLog(entList.FirstOrDefault(), nameof(IEnumerable<IEntity>));
                case IEnumerable enumerable:
                    var enumFirst = enumerable.Cast<object>().FirstOrDefault();
                    if (enumFirst is null) return l.ReturnNull($"{nameof(IEnumerable)} with null object");
                    // retry conversion
                    return l.Return(AsItemInternal(enumFirst, recursions - 1, strict: strict));
                default:
                    throw l.Done(new ArgumentException($"Type '{target.GetType()}' cannot be converted to {nameof(ITypedItem)}. " +
                                                       $"If you are trying to create mock/fake/fallback data, try using \", mock: true\""));
            }

        }

        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder, bool? required = default, IEnumerable<ITypedItem> fallback = default, bool? strict = default)
        {
            Protect(noParamOrder);
            return AsItemList(list, required ?? true, fallback, MaxRecursions, strict: strict ?? false);
        }

        private IEnumerable<ITypedItem> AsItemList(object list, bool required, IEnumerable<ITypedItem> fallback, int recursions, bool strict)
        {
            var l = Log.Fn<IEnumerable<ITypedItem>>($"{nameof(list)}: '{list}'; {nameof(required)}: {required}; {nameof(recursions)}: {recursions}");

            // Inner call to complete scenarios where the data couldn't be created
            IEnumerable<ITypedItem> FallbackOrErrorAndLog(string fallbackMsg, string exMsg) => fallback != null
                ? l.Return(fallback, fallbackMsg + ", fallback")
                : required
                    ? throw l.Done(new ArgumentException($@"Conversion with {nameof(AsItems)} failed, {nameof(required)}=true. {exMsg}", nameof(list)))
                    : l.Return(new List<ITypedItem>(), "no fallback, not required, empty list");


            if (recursions <= 0)
                return FallbackOrErrorAndLog("max recursions", $"Max recursions {MaxRecursions} reached.");

            switch (list)
            {
                case null:
                    return FallbackOrErrorAndLog("null", "Got null.");
                // String check must come early because strings are enumerable
                case string str:
                    return FallbackOrErrorAndLog("string", "Got a string.");
                // List of ITypedItem
                case IEnumerable<ITypedItem> alreadyOk:
                    return l.Return(alreadyOk.Select(e => AsItemInternal(e, MaxRecursions, strict: strict)), nameof(IEnumerable<ITypedItem>));
                    //return l.Return(alreadyOk, "already matches type");
                //case IEnumerable<IDynamicEntity> dynIDynEnt:
                //    return l.Return(dynIDynEnt.Select(e => AsTyped(e, services, MaxRecursions, log)), "IEnum<DynEnt>");
                case IDataSource dsEntities:
                    return l.Return(AsItemList(dsEntities.List, required, fallback, recursions - 1, strict: strict), "DataSource - convert list");
                case IDataStream dataStream:
                    return l.Return(AsItemList(dataStream.List, required, fallback, recursions - 1, strict: strict), "DataStream - convert list");
                case IEnumerable<IEntity> iEntities:
                    return l.Return(iEntities.Select(e => AsItemInternal(e, MaxRecursions, strict: strict)), nameof(IEnumerable<IEntity>));
                case IEnumerable<dynamic> dynEntities:
                    return l.Return(dynEntities.Select(e => AsItemInternal(e as object, MaxRecursions, strict: strict)), nameof(IEnumerable<dynamic>));
                // Variations of single items - should be converted to list
                case IEntity _:
                case IDynamicEntity _:
                case ITypedItem _:
                case ICanBeEntity _:
                    var converted = AsItemInternal(list, MaxRecursions, strict: strict);
                    return converted != null
                        ? l.Return(new List<ITypedItem> { converted }, "single item to list")
                        : l.Return(new List<ITypedItem>(), "typed but converted to null; empty list");
                // Check for IEnumerable but make sure it's not a string
                // Should come fairly late, because some things like DynamicEntities can also be enumerated
                case IEnumerable asEnumerable when !(asEnumerable is string):
                    return l.Return(asEnumerable.Cast<object>().Select(e => AsItemInternal(e, MaxRecursions, strict: strict)), "IEnumerable");
                default:
                    return FallbackOrErrorAndLog($"can't convert '{list.GetType()}'", $"Type '{list.GetType()}' cannot be converted.");
            }
        }

        #endregion
    }
}

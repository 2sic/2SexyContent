﻿using System;
using Custom.Hybrid.Advanced;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic Razor files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [WorkInProgressApi("WIP 16.02 - not final")]
    public abstract class Razor16Strict: Razor14<dynamic, ServiceKit14> 
 // public abstract class Razor16Typed : Razor14<dynamic, ServiceKit14>
    {
        public new ITypedObject Settings => _DynCodeRoot.Settings;

        public new ITypedObject Resources => _DynCodeRoot.Resources;

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 strict. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 strict. Use Data.MyHeader instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object DynamicModel => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 strict. Use TypedModel instead.");

        //public ITypedItem MyItem => AsTyped(Data.MyContent);

        //public IEnumerable<ITypedItem> MyItems => AsTypedList(Data.MyContent);

        //public ITypedItem MyHeader => AsTyped(Data.MyHeader);


        public new IAppTyped App => (IAppTyped)base.App;

        public ITypedObject Merge(params object[] items)
        {
            var mergedDyn = ((DynamicCodeRoot)_DynCodeRoot).MergeTyped(items);
            return mergedDyn;
        }

        /// <summary>
        /// Convert a json ... TODO - different from AsTyped(IEntity...)
        /// </summary>
        /// <param name="json"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public ITypedObject AsTyped(string json, string fallback = default) => base.AsDynamic(json, fallback);


    }


}

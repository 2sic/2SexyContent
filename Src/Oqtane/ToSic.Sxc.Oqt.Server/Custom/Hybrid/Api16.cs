﻿using Custom.Hybrid.Advanced;
using System.Collections.Generic;
using System;
using ToSic.Eav;
using ToSic.Eav.CodeChanges;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Oqtane specific Api base class.
    ///
    /// It's identical to [](xref:Custom.Hybrid.Api14) but this may be enhanced in future. 
    /// </summary>
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Api16: Api14<dynamic, ServiceKit14>, IDynamicCode16
    {
        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        #endregion

        private CodeChangeService CcS => _ccs.Get(GetService<CodeChangeService>);
        private readonly GetOnce<CodeChangeService> _ccs = new GetOnce<CodeChangeService>();

        /// <inheritdoc />
        public new ITypedStack Settings => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Settings);

        /// <inheritdoc />
        public new ITypedStack Resources => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Resources);

        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)base.App;

        /// <inheritdoc />
        public ITypedStack SettingsStack => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public ITypedStack ResourcesStack => _DynCodeRoot.Resources;

        #endregion


        #region AsItem(s) / Merge

        /// <inheritdoc />
        public ITypedRead Merge(params object[] items)
            => _DynCodeRoot.AsC.MergeTyped(items);

        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        #endregion
    }
}
﻿using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Code;

[PrivateApi]
internal class DnnDynamicCodeRoot<TModel, TServiceKit>(DynamicCodeRoot.MyServices services)
    : DynamicCodeRoot<TModel, TServiceKit>(services, DnnConstants.LogName), Sxc.Code.IDynamicCode, IDnnDynamicCode,
        IHasDynamicCodeRoot
    where TModel : class
    where TServiceKit : ServiceKit
{
    /// <summary>
    /// Dnn context with module, page, portal etc.
    /// </summary>
    public IDnnContext Dnn => _dnn.Get(GetService<IDnnContext>);
    private readonly GetOnce<IDnnContext> _dnn = new();
}
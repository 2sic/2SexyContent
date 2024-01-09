﻿using Custom.Hybrid;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Razor.Internal;

[PrivateApi("not sure yet if this will stay in Hybrid or go to Web.Razor or something, so keep it private for now")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IRazor12: IRazor, IDynamicCode12
{
    [PrivateApi]
    dynamic DynamicModel { get; }

}
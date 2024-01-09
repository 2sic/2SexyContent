﻿using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Code;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class DynamicCodeServiceUnknown: DynamicCodeService
{
    public DynamicCodeServiceUnknown(MyServices services, WarnUseOfUnknown<DynamicCodeServiceUnknown> _) 
        : base(services)
    {
    }
}
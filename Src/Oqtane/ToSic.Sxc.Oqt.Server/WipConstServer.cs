﻿using ToSic.Eav.Context;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server
{
    class WipConstServer
    {
        public static IPage NullPage = new PageUnknown();
        public static IModule NullContainer = new ModuleUnknown();
    }
}

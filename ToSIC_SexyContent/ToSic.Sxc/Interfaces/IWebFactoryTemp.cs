﻿using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web;

namespace ToSic.SexyContent.Interfaces
{
    public interface IWebFactoryTemp
    {
        DynamicCodeHelper AppAndDataHelpers(ICmsBlock cms);
    }
}

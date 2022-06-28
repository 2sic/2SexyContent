﻿using Custom.Hybrid.Advanced;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// **_BETA_**
    /// 
    /// Base class for v14 Dynamic Razor files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v14.07")]
    public abstract class Razor14: Razor14<dynamic, ServiceKit14>
    {
    }


}

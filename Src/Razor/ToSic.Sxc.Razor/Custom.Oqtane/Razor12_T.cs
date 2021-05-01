﻿using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Oqtane
{
    /// <summary>
    /// The generic base class for Razor files in Oqtane which have a custom model.
    /// 
    /// As of 2sxc 12.0 it's identical to [](xref:Custom.Hybrid.Razor12), but in future it may have some more Oqtane specific features. 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    [PublicApi]
    public abstract class Razor12<TModel>: Hybrid.Razor12<TModel>
    {

    }
}

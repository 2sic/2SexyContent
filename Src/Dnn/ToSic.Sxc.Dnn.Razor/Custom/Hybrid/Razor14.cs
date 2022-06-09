﻿using Custom.Hybrid;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom
{
    public abstract class Razor14<TModel, TKit>: Razor12, IRazor14<TModel, TKit>
        where TModel : class
        where TKit : Kit
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TKit> root) ? default : root.Model;

        public TKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TKit>());
        private readonly ValueGetOnce<TKit> _kit = new ValueGetOnce<TKit>();
    }
}

﻿using ToSic.Sxc.Data;

namespace ToSic.Custom
{
    public partial class Razor12<TModel>
    {
        public dynamic DynamicModel => _dynamicModel ??= new DynamicReadObject(Model);
        private dynamic _dynamicModel;
    }
}

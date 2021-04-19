﻿using ToSic.Eav.Documentation;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.Custom
{
    public partial class Razor12
    {
        /// <summary>
        /// Dynamic object containing parameters. So in Dnn it contains the PageData, in Oqtane it contains the Model
        /// </summary>
        /// <remarks>
        /// New in v12
        /// </remarks>
        [PublicApi]
        public dynamic DynamicModel =>
            _dynamicModel ?? (_dynamicModel = new DynamicReadDictionary<object, dynamic>(PageData));

        private dynamic _dynamicModel;
    }
}

﻿using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is the same as IDynamicCode, but the root object. 
    /// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really neede.
    /// </summary>
    [PrivateApi]
    public interface IDynamicCodeRoot : IDynamicCode
    {
        [PrivateApi]
        ILookUpEngine ConfigurationProvider { get; }
        
        [PrivateApi]
        DataSourceFactory DataSourceFactory { get; }

        [PrivateApi]
        void LateAttachApp(IApp app);
    }
}

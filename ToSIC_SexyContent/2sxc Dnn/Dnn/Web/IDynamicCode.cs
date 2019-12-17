﻿using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This interface extends the IAppAndDataHelpers with the DNN Context.
    /// It's important, because if 2sxc also runs on other CMS platforms, then the Dnn Context won't be available, so it's in a separate interface.
    /// </summary>
    [PublicApi]
    public interface IDynamicCode : Sxc.Web.IDynamicCode
    {
        /// <summary>
        /// The DNN context.  
        /// </summary>
        /// <returns>
        /// The DNN context.
        /// </returns>
        IDnnContext Dnn { get; }
    }
}

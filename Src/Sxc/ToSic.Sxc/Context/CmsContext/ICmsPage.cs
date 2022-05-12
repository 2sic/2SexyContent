﻿using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Information about the page which is the context for the currently running code.
    /// </summary>
    /// <remarks>
    /// Note that the module context is the module for which the code is currently running.
    /// In some scenarios (like Web-API scenarios) the code is running _for_ this page but _not on_ this page,
    /// as it would then be running on a WebApi.
    /// </remarks>
    [PublicApi]
    public interface ICmsPage: IHasMetadata
    {
        /// <summary>
        /// The Id of the page.
        /// 
        /// 🪒 Use in Razor: `CmsContext.Page.Type`
        /// </summary>
        /// <remarks>
        /// Corresponds to the Dnn `TabId` or the Oqtane `Page.PageId`
        /// </remarks>
        int Id { get; }
        
        /// <summary>
        /// The page parameters, cross-platform.
        /// Use this for easy access to url parameters like ?id=xyz
        /// with `CmsContext.Page.Parameters["id"]` as a replacement for `Request.QueryString["id"]`
        /// 
        /// 🪒 Use in Razor: `CmsContext.Page.Parameters["id"]`
        /// </summary>
        IParameters Parameters { get; }

        // unsure if used
        /// <summary>
        /// The resource specific url, like the one to this page or portal
        /// </summary>
        [PrivateApi("Not yet official property, must decide if we'll put in on the ICmsPage or maybe on an ICmsUrl or something")]
        string Url { get; }
        
        [PrivateApi("WIP")]
        IDynamicMetadata Metadata { get; }
    }
}

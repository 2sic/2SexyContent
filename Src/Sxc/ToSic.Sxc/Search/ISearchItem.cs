﻿using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Search
{
    /// <summary>
    /// Defines an item in the search system - which is prepared by Sxc, and can be customized as needed
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface ISearchItem 
        : ToSic.SexyContent.Search.ISearchInfo // backward compatibility
    {
        new string UniqueKey { get; set; }

        /// <summary>
        /// Title in search results
        /// </summary>
        new string Title { get; set; }

        /// <summary>
        /// Description in search results
        /// </summary>
        new string Description { get; set; }

        /// <summary>
        /// Contents of this item - will be indexed
        /// </summary>
        new string Body { get; set; }

        /// <summary>
        /// Url to go to, when looking at the details of this search result
        /// </summary>
        new string Url { get; set; }

        /// <summary>
        /// Timestamp in GMT / UTC
        /// </summary>
        new DateTime ModifiedTimeUtc { get; set; }

        /// <summary>
        /// Determines if this item should appear in search or be ignored
        /// </summary>
        new bool IsActive { get; set; }

        /// <summary>
        /// Query String params to access this item
        /// </summary>
        new string QueryString { get; set; }

        /// <summary>
        /// Culture code, for language sensitive searches
        /// </summary>
        new string CultureCode { get; set; }

        /// <summary>
        /// The underlying data in the search
        /// </summary>
        new IEntity Entity { get; set; }

    }
}

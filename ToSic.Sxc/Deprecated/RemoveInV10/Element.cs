﻿using System;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent
{
    [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
    public class Element
    {
        /// <summary>
        /// The DynamicContent object, as dynamic
        /// </summary>
        public dynamic Content { get; set; }

        /// <summary>
        /// The Presentation object, as dynamic
        /// </summary>
        public dynamic Presentation { get; set; }
        
        /// <summary>
        /// The EntityID of the ContentGroupItem
        /// </summary>
        public int? EntityId { get; set; }

        /// <summary>
        /// The SortOrder of the ContentGroupItem
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// The ContentGroupID of the ContentGroupItem
        /// </summary>
        public Guid GroupId { get; set; }

    }
}
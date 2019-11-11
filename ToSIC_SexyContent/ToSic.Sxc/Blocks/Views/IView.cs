﻿using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Defines a view configuration which is loaded from an <see cref="EntityBasedType"/>.
    /// </summary>
    [PublicApi]
    public interface IView: IEntityBasedType
    {
        /// <summary>
        /// The name, localized in the current UI language.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Path to teh template
        /// </summary>
        string Path { get; }

        [PrivateApi] string ContentType { get; }
        [PrivateApi] IEntity ContentItem { get; }
        [PrivateApi] string PresentationType { get; }
        [PrivateApi] IEntity PresentationItem { get; }
        [PrivateApi] string HeaderType { get; }
        [PrivateApi] IEntity HeaderItem { get; }
        [PrivateApi] string HeaderPresentationType { get; }
        [PrivateApi] IEntity HeaderPresentationItem { get; }

        /// <summary>
        /// The underlying type name of the template, ATM they are unfortunately hard-coded as "C# Razor" and "Token"
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Determine if we should hide this view/template from the pick-UI.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Location of the template - in the current tenant/portal or global/shared location.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Determines if the view should behave as a list or not. Views that are lists also
        /// have Header configuration and treat content in a special way. 
        /// </summary>
        bool UseForList { get; }

        [PrivateApi] bool PublishData { get; }
        [PrivateApi] string StreamsToPublish { get; }

        /// <summary>
        /// The query which provides data to this view. 
        /// </summary>
        IEntity Query { get; }

        /// <summary>
        /// An identifier which could occur in the url, causing the view to automatically switch to this one. 
        /// </summary>
        string UrlIdentifier { get; }

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        [PrivateApi]
        bool IsRazor { get; }

    }
}
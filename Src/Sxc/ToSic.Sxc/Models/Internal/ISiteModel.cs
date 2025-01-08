﻿namespace ToSic.Sxc.Models.Internal;

public interface ISiteModel
{
    /// <summary>
    /// The site ID.
    ///
    /// * In Dnn it's from `PortalInfo.PortalID`
    /// * In Oqtane it's `Site.SiteId`
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The site GUID.
    ///
    /// * In Dnn it's from `PortalInfo.GUID`
    /// * In Oqtane it's `Guid.Empty` as Oqtane doesn't have site GUIDs
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// The site name.
    ///
    /// * In Dnn it's from `PageInfo.PortalName`
    /// * in Oqtane it's from `Site.Name`
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The public url to this site (without any trailing slashes)
    ///
    /// * In Dnn it's from `PortalAliasInfo.FullUrl` (last slash removed)
    /// * in Oqtane it's a combination of protocol, site-alias and path
    /// </summary>
    string Url { get; }

    /// <summary>
    /// The site languages, comma separated.
    /// Can be empty ever if a <see cref="DefaultLanguage"/> is set, if the site itself is not multi-language.
    /// </summary>
    string Languages { get; }

    /// <summary>
    /// The site Culture Code.
    ///
    /// * In Dnn it's from `PortalInfo.CultureCode`
    /// * in Oqtane it's from `Site.CultureCode`
    /// </summary>
    string DefaultLanguage { get; }

    /// <summary>
    /// The site creation date/time.
    ///
    /// * In Dnn it's from `PortalInfo.CreatedOnDate`
    /// * in Oqtane it's from `Site.CreatedOn`
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// The site modification date/time.
    ///
    /// * In Dnn it's from `PortalInfo.LastModifiedOnDate`
    /// * in Oqtane it's from `Site.ModifiedOn`
    /// </summary>
    DateTime Modified { get; }

    /// <summary>
    /// The Zone ID, which is the ID of the 2sxc/EAV zone which applies to this site.
    /// It's usually different from the site ID, and in rare cases can be shared among multiple sites.
    /// </summary>
    int ZoneId { get; }

    /// <summary>
    /// The Content App.
    /// </summary>
    int ContentAppId { get; }

    /// <summary>
    /// The (technical) primary App, which contains things such as site metadata.
    /// </summary>
    int PrimaryAppId { get; }
}
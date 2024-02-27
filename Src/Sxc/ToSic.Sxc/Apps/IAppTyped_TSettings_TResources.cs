﻿using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps;

public interface IAppTyped<out TSettings, out TResources> : IAppIdentity
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    #region Properties basically from Eav.Apps.IApp

    /// <inheritdoc cref="Eav.Apps.IApp.Name"/>
    string Name { get; }

    /// <inheritdoc cref="Eav.Apps.IApp.Data"/>
    IAppDataTyped Data { get; }

    #endregion

    #region From IAppTyped
    /// <inheritdoc cref="IAppTyped.GetQuery"/>
    IDataSource GetQuery(
        string name = default,
        NoParamOrder noParamOrder = default,
        IDataSourceLinkable attach = default,
        object parameters = default
    );


    /// <inheritdoc cref="IApp.Configuration"/>
    IAppConfiguration Configuration { get; }

    #endregion

    #region Paths / Urls

    IFolder Folder { get; }

    /// <inheritdoc cref="IAppTyped.FolderAdvanced"/>
    IFolder FolderAdvanced(NoParamOrder noParamOrder = default, string location = default);

    IFile Thumbnail { get; }

    #endregion

    /// <summary>
    /// All the app settings which are custom for each app.
    /// These are typed - typically to AppCode.Data.AppSettings
    /// </summary>
    new TSettings Settings { get; }

    /// <summary>
    /// All the app resources (usually used for multi-language labels etc.).
    /// /// These are typed - typically to AppCode.Data.AppResources
    /// </summary>
    new TResources Resources { get; }


}
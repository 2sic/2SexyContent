﻿using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Models.Internal;

/// <summary>
/// Internal class to hold all the information about the App files,
/// until it's converted to an IEntity in the <see cref="AppAssets"/> DataSource.
///
/// Important: this is an internal object.
/// We're just including it in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[ContentTypeSpecs(
    Guid = "3cf0822f-d276-469a-bbd1-cc84fd6ff748",
    Description = "File in an App",
    Name = TypeName
)]
public record FileRaw: FileFolderBase, IFileModel
{
    internal const string TypeName = "File";

    internal static DataFactoryOptions Options = new()
    {
        TypeName = TypeName,
        TitleField = nameof(Path)
    };

    /// <inheritdoc cref="IFileModel.Name"/>
    [ContentTypeAttributeSpecs(Description = "The file name without extension, like my-image")]
    public override string Name { get; init; }

    /// <inheritdoc cref="IFileModel.Extension"/>
    public string Extension { get; init; }

    /// <inheritdoc cref="IFileModel.Size"/>
    public int Size { get; init; }

    /// <summary>
    /// Data but without ID, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public override IDictionary<string, object> Attributes(RawConvertOptions options)
        => new Dictionary<string, object>(base.Attributes(options))
        {
            { nameof(Extension), Extension },
            { nameof(Size), Size },
        };

    [PrivateApi]
    public override IEnumerable<object> RelationshipKeys(RawConvertOptions options)
        => new List<object>
        {
            // For relationships looking for files in this folder
            $"FileIn:{ParentFolderInternal}"
        };

}
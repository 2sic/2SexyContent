﻿using System;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data.Decorators;

/// <summary>
/// Important: for now it should be abstract, because we're not sure in which cases
/// something will check if the derived decorator is there, and then get wrong conclusions
/// So for now, only the EntityInBlockDecorator should be used anywhere explicitly
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class EntityInListDecorator : IDecorator<IEntity>
{
    protected EntityInListDecorator(string field, int index = 0, IEntity parent = default)
    {
        Parent = parent;
        ParentGuid = parent?.EntityGuid; // parentGuid;
        Field = field;
        SortOrder = index;
    }

    protected const int DefIndex = 0;

    /// <summary>
    /// The position in the list
    /// </summary>
    /// <remarks>
    /// This has been in use since ca. 2sxc 2.0
    /// </remarks>
    public int SortOrder { get; }

    /// <summary>
    /// The field which has the list containing this item.
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 11.01
    /// </remarks>
    public string Field { get; }

    /// <summary>
    /// The parent item which has the list containing this item.
    /// </summary>
    /// <remarks>
    /// Important: as of now this is NOT the content-block guid, and shouldn't be because there is code checking for this to be empty
    /// on content-blocks.
    ///
    /// Added in 2sxc 11.01
    /// </remarks>
    public Guid? ParentGuid { get; private set; }

    public IEntity Parent { get; private set; }
}
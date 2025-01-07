﻿// ReSharper disable once CheckNamespace

using ToSic.Sxc.Data;

namespace Custom.Data;

partial class DataModelOfItem : IMultiWrapper<IEntity>, IEquatable<ITypedItem>
{
    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    public override bool Equals(object b)
        => MultiWrapperEquality.EqualsObj(this, b);

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int GetHashCode()
        => MultiWrapperEquality.GetWrappedHashCode(this);

    IEntity IMultiWrapper<IEntity>.RootContentsForEqualityCheck
        => (_item as IMultiWrapper<IEntity>)?.RootContentsForEqualityCheck;

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    /// <param name="item1">first item to compare</param>
    /// <param name="item2">second item to compare</param>
    /// <returns>true, if both wrappers are the same type and wrap the same entity</returns>
    public static bool operator ==(DataModelOfItem item1, DataModelOfItem item2)
        => MultiWrapperEquality.IsEqual(item1, item2);

    /// <summary>
    /// Ensure that the equality check is done correctly.
    /// If two objects wrap the same item, they will be considered equal.
    /// </summary>
    /// <param name="item1">first item to compare</param>
    /// <param name="item2">second item to compare</param>
    /// <returns>false, if both wrappers are the same type and wrap the same entity</returns>
    public static bool operator !=(DataModelOfItem item1, DataModelOfItem item2)
        => !MultiWrapperEquality.IsEqual(item1, item2);
}
﻿using Custom.Data;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Models;


/// <summary>
/// Base class for **plain** custom data models and can be used in Razor Components.
/// It wraps a <see cref="ITypedItem"/> and provides a simple way to access the data.
/// 
/// This is much lighter than the <see cref="CustomItem"/> which also wraps data, as it doesn't have any predefined properties and doesn't have the <see cref="ITypedItem"/> APIs.
/// </summary>
/// <example>
///
/// Usage ca. like this:
///
/// 1. A custom data model in `AppCode.Data` which inherits from this class (usually generated by 2sxc Copilot)
/// 2. Razor code which uses it to convert typed items into this custom data model
/// 
/// Example trivial custom **plain** data model:
/// ```c#
/// namespace AppCode.Data
/// {
///   class MyPerson : CustomModelOfItem
///   {
///     public string Name => _item.String("Name");
///   }
/// }
/// ```
///
/// Example usage in Razor:
///
/// ```razor#
/// @inherits Custom.Hybrid.RazorTyped
/// @using AppCode.Data
/// @{
///   var person = As&lt;MyPerson&gt;(MyItem);
/// }
/// <span>@person.Name</span>
/// ```
/// </example>
/// <remarks>
/// - Released in v19.01 (BETA)
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still beta, name may change to CustomModelOfItem or something")]
public abstract partial class DataModelOfItem : IDataModelOf<ITypedItem>, IDataModelForType, ICanBeItem, ICanBeEntity //, IHasPropLookup
{
    #region Explicit Interfaces for internal use - Setup, etc.

    void IDataModelOf<ITypedItem>.Setup(ITypedItem baseItem)
        => _item = baseItem;

    /// <inheritdoc />
    string IDataModelForType.ForContentType
        => GetType().Name;

    /// <summary>
    /// The actual item which is being wrapped, in rare cases where you must access it from outside.
    ///
    /// It's only on the explicit interface, so it is not available from outside or inside, unless you cast to it.
    /// Goal is that inheriting classes don't access it to keep API surface small.
    /// </summary>
    ITypedItem ICanBeItem.Item => _item;

    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    ///
    /// It's an explicit interface implementation, so that the object itself doesn't broadcast this.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEntity ICanBeEntity.Entity => _item.Entity;

    IBlock ICanBeItem.TryGetBlockContext() => _item.TryGetBlockContext();

    //IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= ((IHasPropLookup)((ICanBeItem)this).Item).PropertyLookup;
    //private IPropertyLookup _propLookup;

    #endregion

    /// <summary>
    /// The underlying item - for inheriting classes to access.
    /// </summary>
    /// <remarks>
    /// * this property is protected, not public, as it should only be used internally.
    /// * this also prevents it from being serialized in JSON, which is good.
    /// * it uses an unusual name `_item` to avoid naming conflicts with properties generated in inheriting classes.
    /// </remarks>
#pragma warning disable IDE1006
    // ReSharper disable once InconsistentNaming
    protected internal ITypedItem _item { get; private set; }
#pragma warning restore IDE1006

    /// <summary>
    /// Override ToString to give more information about the current object
    /// </summary>
    public override string ToString() 
        => $"{nameof(DataModelOfItem)} Data Model {GetType().FullName} " + (_item == null ? "without backing data (null)" : $"for id:{_item.Id} ({_item})");


    #region As...

    /// <inheritdoc cref="DataModelHelpers.As{T}"/>
    protected T As<T>(object item)
        where T : class, IDataModel, new()
        => DataModelHelpers.As<T>(item);

    /// <inheritdoc cref="DataModelHelpers.AsList{T}"/>
    protected IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, IDataModel, new()
        => DataModelHelpers.AsList<T>(source, protector, nullIfNull);

    #endregion

}
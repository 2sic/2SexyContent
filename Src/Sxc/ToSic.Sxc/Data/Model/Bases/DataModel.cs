﻿using Custom.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Data.Model;


/// <summary>
/// Base class for **plain** custom data models and can be used in Razor Components.
/// It wraps a <see cref="IEntity"/> and provides a simple way to access the data.
/// </summary>
/// <example>
///
/// Usage ca. like this:
///
/// 1. A custom data model in `AppCode.Data` which inherits from this class (usually generated by 2sxc Copilot)
/// 2. Razor code which uses it to convert typed items into this custom data model
/// 
/// Example trivial custom **plain** data model:
/// 
/// ```c#
/// namespace AppCode.Data
/// {
///   class MyPerson : DataModel
///   {
///     public string Name => _entity.Get&lt;string&gt; ("Name");
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
/// This is much lighter than the <see cref="CustomItem"/> which also wraps data, as it doesn't have any predefined properties and doesn't have the <see cref="ITypedItem"/> APIs.
/// 
/// History
/// 
/// - Released in v19.01 (BETA)
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still beta, name may change to CustomModelOfItem or something")]
public abstract partial class DataModel: IDataModelOf<IEntity>, ICanBeEntity //, IHasPropLookup
{
    #region Explicit Interfaces for internal use - Setup, etc.

    void IDataModelOf<IEntity>.Setup(IEntity baseItem, IModelFactory modelFactory)
    {
        _entity = baseItem;
        _modelFactory = modelFactory;
    }
    private IModelFactory _modelFactory;

    ///// <summary>
    ///// The actual item which is being wrapped, in rare cases where you must access it from outside.
    /////
    ///// It's only on the explicit interface, so it is not available from outside or inside, unless you cast to it.
    ///// Goal is that inheriting classes don't access it to keep API surface small.
    ///// </summary>
    //ITypedItem ICanBeItem.Item => Item;

    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    ///
    /// It's an explicit interface implementation, so that the object itself doesn't broadcast this.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEntity ICanBeEntity.Entity => _entity;

    //IBlock ICanBeItem.TryGetBlockContext() => Item.TryGetBlockContext();

    //IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= ((IHasPropLookup)((ICanBeItem)this).Item).PropertyLookup;
    //private IPropertyLookup _propLookup;

    #endregion

    /// <summary>
    /// The underlying entity - for inheriting classes to access.
    /// </summary>
    /// <remarks>
    /// * this property is protected, not public, as it should only be used internally.
    /// * this also prevents it from being serialized in JSON, which is good.
    /// * it uses an unusual name `_entity` to avoid naming conflicts with properties generated in inheriting classes.
    /// </remarks>
#pragma warning disable IDE1006
    // ReSharper disable once InconsistentNaming
    protected internal IEntity _entity { get; private set; }
#pragma warning restore IDE1006

    /// <summary>
    /// Override ToString to give more information about the current object
    /// </summary>
    public override string ToString() 
        => $"{nameof(DataModel)} Data Model {GetType().FullName} " + (_entity == null ? "without backing data (null)" : $"for id:{_entity.EntityId} ({_entity})");


    #region As...

    /// <inheritdoc cref="DataModelHelpers.As{T}"/>
    protected T As<T>(object item)
        where T : class, IDataModel
        => DataModelHelpers.As<T>(_modelFactory, item);

    /// <inheritdoc cref="DataModelHelpers.AsList{T}"/>
    protected IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, IDataModel
        => DataModelHelpers.AsList<T>(_modelFactory, source, protector, nullIfNull);

    #endregion

}
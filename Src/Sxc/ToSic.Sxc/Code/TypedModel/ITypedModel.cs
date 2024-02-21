﻿using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code;

/// <summary>
/// Object in partial Razor files to access parameters handed in.
/// Example caller:
///
/// ```c#
/// @Html.Partial(someFile, new { blogPost, file = mainFile, title = "hello" }
/// ```
///
/// Example partial:
///
/// ```c#
/// var blogPost = MyModel.Item("BlogPost");
/// var file = MyModel.File("File");
/// var title = MyModel.String("Title");
/// ```
///
/// > [!TIP]
/// > The common data types such as `string` or <see cref="ITypedItem"/> have methods to quickly get them in the desired type.
/// > This allows things such as `var message = MyModel.String("Message");`
/// > For less common types you'll need to use <see cref="Get"/> and cast it as needed, like this:
/// > `string message = MyModel.Get("Message");`.
/// </summary>
/// <remarks>Introduced in v16.02</remarks>
[PublicApi]
public interface ITypedModel: IHasKeys
{
    #region Check if parameters were supplied

    /// <inheritdoc cref="IHasKeys.ContainsKey"/>
    new bool ContainsKey(string name);


    /// <inheritdoc cref="IHasKeys.Keys"/>
    new IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default);
        
    #endregion

    #region Get

    /// <summary>
    /// Will get the value and return as object, since the type isn't known. 
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns>Object if found, `null` if not found.</returns>
    object Get(string name, NoParamOrder noParamOrder = default, bool? required = default);

    /// <summary>
    /// Will get the value and return as type T as specified.
    /// </summary>
    /// <typeparam name="T">The returned type</typeparam>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The fallback value. If provided, the type is automatically determined.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is provided which is not the `default`.
    /// So eg. `Get&lt;string&gt;(..., fallback: false)` can't be detected, but `..., fallback: "hello"` can.
    /// </param>
    /// <returns>Object of type T if found, `null` if not found.</returns>
    T Get<T>(string name, NoParamOrder noParamOrder = default, T fallback = default, bool? required = default);

    #endregion

    #region Code (new 16.05)

    /// <summary>
    /// Get code forwarded to the current razor.
    /// Code was usually created in the caller using `GetCode(...)` and may need to be passed around.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">A fallback to use if not found - not commonly used here.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>The resulting object is `dynamic` which is necessary for making calls to methods etc.</returns>
    /// <remarks>New in 16.05</remarks>
    dynamic Code(string name, NoParamOrder noParamOrder = default, object fallback = default, bool? required = default);

    #endregion

    #region Simple Values: String / Bool / Guid / DateTime

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found.</returns>
    string String(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, false if not found.</returns>
    bool Bool(string name, NoParamOrder noParamOrder = default, bool? fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, empty-guid if not found.</returns>
    Guid Guid(string name, NoParamOrder noParamOrder = default, Guid? fallback = default, bool? required = default);


    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, default-date if not found.</returns>
    DateTime DateTime(string name, NoParamOrder noParamOrder = default, DateTime? fallback = default, bool? required = default);

    #endregion

    #region Numbers

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>int result if found, `0` if not found/convertible.</returns>
    /// <exception cref="ArgumentException">if the name is not found an no fallback provided and required not false</exception>
    int Int(string name, NoParamOrder noParamOrder = default, int? fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>int result if found, `0` if not found/convertible.</returns>
    /// <exception cref="ArgumentException">if the name is not found an no fallback provided and required not false</exception>
    float Float(string name, NoParamOrder noParamOrder = default, float? fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>int result if found, `0` if not found/convertible.</returns>
    /// <exception cref="ArgumentException">if the name is not found an no fallback provided and required not false</exception>
    double Double(string name, NoParamOrder noParamOrder = default, double? fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value and return in the desired type.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>int result if found, `0` if not found/convertible.</returns>
    /// <exception cref="ArgumentException">if the name is not found an no fallback provided and required not false</exception>
    decimal Decimal(string name, NoParamOrder noParamOrder = default, decimal? fallback = default, bool? required = default);

    #endregion

    #region ADAM: File/Files, Folder/Folders

    /// <summary>
    /// Will get the value if specified.
    /// If the value is a list of files, then this will only return the first one.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found.</returns>
    IFile File(string name, NoParamOrder noParamOrder = default, IFile fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value if specified.
    /// If the value is a single file, will return a list containing that file.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, empty-list if not found.</returns>
    IEnumerable<IFile> Files(string name, NoParamOrder noParamOrder = default, IEnumerable<IFile> fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value if specified.
    /// If the value is a list of folders, then this will only return the first one.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found.</returns>
    IFolder Folder(string name, NoParamOrder noParamOrder = default, IFolder fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value if specified.
    /// If the value is a single folder, will return a list containing that folder.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, empty-list if not found.</returns>
    IEnumerable<IFolder> Folders(string name, NoParamOrder noParamOrder = default, IEnumerable<IFolder> fallback = default, bool? required = default);

    #endregion

    //#region Stacks

    ///// <summary>
    ///// Get a stack which was passed to this
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="noParamOrder"></param>
    ///// <param name="fallback"></param>
    ///// <param name="required"></param>
    ///// <returns></returns>
    //ITypedStack Stack(string name, NoParamOrder noParamOrder = default, ITypedStack fallback = default, bool? required = default);

    //#endregion

    #region Item / Entity

    /// <summary>
    /// Will get the value if specified.
    /// If the value is a list of items, then this will only return the first one.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found.</returns>
    ITypedItem Item(string name, NoParamOrder noParamOrder = default, ITypedItem fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value if specified.
    /// If the value is a single item, will return a list containing that item.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, empty-list if not found.</returns>
    IEnumerable<ITypedItem> Items(string name, NoParamOrder noParamOrder = default, IEnumerable<ITypedItem> fallback = default, bool? required = default);

    /// <summary>
    /// Will get the value being a toolbar as specified.
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is set / not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found</returns>
    IToolbarBuilder Toolbar(string name, NoParamOrder noParamOrder = default, IToolbarBuilder fallback = default, bool? required = default);

    #endregion

    #region HtmlTags

    /// <summary>
    /// Will get the value being an `IHtmlTag` as specified (RazorBlade objects)
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found</returns>
    IHtmlTag HtmlTag(string name, NoParamOrder noParamOrder = default, IHtmlTag fallback = default,
        bool? required = default);

    /// <summary>
    /// Will get the value being an list (IEnumerable) of `IHtmlTag` as specified (RazorBlade objects)
    /// </summary>
    /// <param name="name">Property name on the passed in data object</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="required">
    /// Throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired).
    /// It is automatically `false` if a `fallback` is not `null`.
    /// </param>
    /// <returns>typed result if found, `null` if not found</returns>
    IEnumerable<IHtmlTag> HtmlTags(string name, NoParamOrder noParamOrder = default,
        IEnumerable<IHtmlTag> fallback = default, bool? required = default);

    #endregion

    
    #region As Conversions (new 17.02)

    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T">the target type</typeparam>
    /// <param name="source">the source object - an `IEntity` or `ITypedItem`</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="nullIfNull">if `true` will return null when `source` is `null` - otherwise a wrapper item with empty-contents</param>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    T As<T>(ICanBeEntity source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new();

    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T">the target type</typeparam>
    /// <param name="source">the source object - a List/Enumerable of `IEntity` or `ITypedItem`</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="nullIfNull">if `true` will return null when `source` is `null` - otherwise a wrapper item with empty-contents</param>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEnumerable<T> AsList<T>(IEnumerable<ICanBeEntity> source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new();

    #endregion
}
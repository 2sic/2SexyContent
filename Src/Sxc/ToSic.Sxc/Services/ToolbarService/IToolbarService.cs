﻿using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Special helper to generate edit toolbars in the front-end.
    /// It is used in combination with `@Edit.Toolbar(...)`.
    /// It's especially useful for complex rules like Metadata-buttons which are more complex to create. 
    /// </summary>
    /// <remarks>
    /// History
    /// * Added in 2sxc 13
    /// </remarks>

    [PublicApi("WIP 13")]
    public interface IToolbarService
    {
        /// <summary>
        /// Build a Toolbar configuration using the `default` template/buttons to use with `@Edit.Toolbar`
        /// It's a fluid API, so the returned object can be extended with further `Add(...)` or special helpers to quickly create complex configurations.
        /// For guidance what to give it, also check out the [toolbar docs](xref:JsCode.Toolbars.Simple).
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">Parameters for the UI, like color=red - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options</param>
        /// <returns></returns>
        IToolbarBuilder Default(string noParamOrder = Eav.Parameters.Protector, string ui = "");

        /// <summary>
        /// Build a Toolbar configuration using the `empty` toolbar to use with `@Edit.Toolbar`
        /// It's a fluid API, so the returned object can be extended with further `Add(...)` or special helpers to quickly create complex configurations.
        /// For guidance what to give it, also check out the [toolbar docs](xref:JsCode.Toolbars.Simple).
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">Parameters for the UI, like color=red - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options</param>
        /// <returns></returns>
        IToolbarBuilder Empty(string noParamOrder = Eav.Parameters.Protector, string ui = "");
    }
}

﻿using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Edit.EditService;

partial class EditService
{
    /// <inheritdoc />
    public bool Enabled { 
        get; 
        [PrivateApi("hide, only used for demos")]
        set;
    }

    #region Scripts and CSS includes

    /// <inheritdoc/>
    public string Enable(NoParamOrder noParamOrder = default, bool? js = null, bool? api = null,
        bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null
    ) => Log.Func<string>(() =>
    {
        //Eav.Parameters.Protect(noParamOrder,
        //    $"{nameof(js)},{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

        var ps = _CodeApiSvc.GetKit<ServiceKit14>()?.Page;
        if (ps == null)
            return (null, "page service not found");

        if (js == true || api == true || forms == true) ps.Activate(SxcPageFeatures.JsCore.NameId);

        // only update the values if true, otherwise leave untouched
        // Must activate the "public" one JsCms, not internal, so feature-tests will run
        if (api == true || forms == true) ps.Activate(SxcPageFeatures.JsCms.NameId);

        if (styles == true) ps.Activate(SxcPageFeatures.Toolbars.NameId);

        if (context == true) ps.Activate(SxcPageFeatures.ContextModule.NameId);

        if (autoToolbar == true) ps.Activate(SxcPageFeatures.ToolbarsAuto.NameId);

        return (null, "ok");
    });

    #endregion Scripts and CSS includes
}
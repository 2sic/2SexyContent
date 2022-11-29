﻿using ToSic.Eav.Documentation;
using BuiltInFeatures = ToSic.Sxc.Web.PageFeatures.BuiltInFeatures;

namespace ToSic.Sxc.Edit.EditService
{
    public partial class EditService
    {
        /// <inheritdoc />
        public bool Enabled { 
            get; 
            [PrivateApi("hide, only used for demos")]
            set;
        }

        #region Scripts and CSS includes

        /// <inheritdoc/>
        public string Enable(string noParamOrder = Eav.Parameters.Protector, bool? js = null, bool? api = null,
            bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(js)},{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

            var psf = Block?.Context?.PageServiceShared;

            if (js == true || api ==true || forms == true) psf?.Activate(BuiltInFeatures.JsCore.NameId);

            // only update the values if true, otherwise leave untouched
            // Must activate the "public" one JsCms, not internal, so feature-tests will run
            if (api == true || forms == true) psf?.Activate(BuiltInFeatures.JsCms.NameId);

            if (styles.HasValue) psf?.Activate(BuiltInFeatures.Toolbars.NameId);

            if (context.HasValue) psf?.Activate(BuiltInFeatures.ContextModule.NameId);

            if (autoToolbar.HasValue) psf?.Activate(BuiltInFeatures.ToolbarsAuto.NameId);

            return null;
        }

        #endregion Scripts and CSS includes
    }
}
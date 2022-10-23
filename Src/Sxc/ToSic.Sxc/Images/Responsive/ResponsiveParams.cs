﻿using ToSic.Eav;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// Helper class to handle all kinds of parameters passed to a responsive tag
    /// </summary>
    [PrivateApi]
    public class ResponsiveParams
    {
        /// <summary>
        /// The only reliable object which knows about the url - can never be null
        /// </summary>
        public IHasLink Link { get; }

        /// <summary>
        /// The field used for this responsive output - can be null!
        /// </summary>
        public IDynamicField Field { get; }
        public IResizeSettings Settings { get; }
        public string ImgAlt { get; }
        public string ImgClass { get; }

        internal ResponsiveParams(
            string method,
            object link,
            string noParamOrder = Parameters.Protector,
            IResizeSettings settings = null,
            string imgAlt = null,
            string imgClass = null
            )
        {
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, method,
                $"{nameof(link)}, {nameof(settings)}, factor, {nameof(imgAlt)}, {nameof(imgClass)}, recipe");

            Field = link as IDynamicField;
            Link = (IHasLink)Field ?? new HasLink(link as string);
            Settings = settings;
            ImgAlt = imgAlt;
            ImgClass = imgClass;
        }
    }
}

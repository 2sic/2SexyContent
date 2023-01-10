﻿using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Helper;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Images
{
    public partial class ImageService: ServiceForDynamicCode, IImageService
    {
        #region Constructor and Inits

        public ImageService(ImgResizeLinker imgLinker, IFeaturesService features) : base(Constants.SxcLogName + ".ImgSvc")
            => ConnectServices(Features = features,
                ImgLinker = imgLinker
            );
        internal ImgResizeLinker ImgLinker { get; }
        internal IFeaturesService Features { get; }

        internal IEditService EditOrNull => _DynCodeRoot?.Edit;

        internal IToolbarService ToolbarOrNull => _toolbarSvc.Get(() => _DynCodeRoot?.GetService<IToolbarService>());
        private readonly GetOnce<IToolbarService> _toolbarSvc = new GetOnce<IToolbarService>();

        #endregion

        #region Settings Handling

        /// <summary>
        /// Use the given settings or try to use the default content-settings if available
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private object GetBestSettings(object settings)
        {
            var wrapLog = Log.Fn<object>(Debug);
            if (settings == null || settings is bool boolSettings && boolSettings)
                return wrapLog.Return((object)GetCodeRootSettingsByName("Content"), "null/default");

            if (settings is string strName && !string.IsNullOrWhiteSpace(strName))
                return wrapLog.Return((object)GetCodeRootSettingsByName(strName), $"name: {strName}");

            return wrapLog.Return(settings, "unchanged");
        }

        private dynamic GetCodeRootSettingsByName(string strName)
        {
            var wrapLog = Log.Fn<object>(Debug, strName, message: $"code root: {_DynCodeRoot != null}");
            var result = (_DynCodeRoot?.Settings?.Images as ICanGetByName)?.Get(strName);
            return wrapLog.Return((object)result, $"found: {result != null}");
        }

        /// <summary>
        /// Convert to Multi-Resize Settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private AdvancedSettings ToAdv(object value) => AdvancedSettings.Parse(value);

        #endregion

        /// <inheritdoc />
        public IResponsiveImage Img(
            object link = null,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = default,
            string imgAlt = null,
            string imgAltFallback = default,
            string imgClass = null,
            object recipe = null)
            => new ResponsiveImage(this,
                new ResponsiveParams(nameof(Img), link, noParamOrder,
                    Settings(settings, factor: factor, width: width, recipe: recipe),
                    imgAlt: imgAlt, imgAltFallback: imgAltFallback, imgClass: imgClass),
                Log);


        /// <inheritdoc />
        public IResponsiveImage ImgOrPic(
            object link = null,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = default,
            string imgAlt = null,
            string imgAltFallback = default,
            string imgClass = null,
            object recipe = null)
        {
            var respParams = new ResponsiveParams(nameof(ImgOrPic), link, noParamOrder,
                Settings(settings, factor: factor, width: width, recipe: recipe),
                imgAlt: imgAlt, imgAltFallback: imgAltFallback, imgClass: imgClass);
            var path = respParams.Link.Url;
            var format = GetFormat(path);
            return format.ResizeFormats.Any()
                ? (IResponsiveImage)new ResponsivePicture(this, respParams, Log)
                : new ResponsiveImage(this, respParams, Log);
        }


        /// <inheritdoc />
        public IResponsivePicture Picture(
            object link = null,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = default,
            string imgAlt = null,
            string imgAltFallback = default,
            string imgClass = null,
            object recipe = null)
            => new ResponsivePicture(this,
                new ResponsiveParams(nameof(Picture), link, noParamOrder,
                    Settings(settings, factor: factor, width: width, recipe: recipe),
                    imgAlt: imgAlt, imgAltFallback: imgAltFallback, imgClass: imgClass),
                Log);

        /// <inheritdoc />
        public bool Debug
        {
            get => _debug;
            set
            {
                _debug = value;
                ImgLinker.Debug = value;
                Features.Debug = value;
            }
        }
        private bool _debug;
    }
}

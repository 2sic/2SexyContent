﻿using System;
using System.IO;
using Custom.Hybrid;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor
{
    internal class OqtRazorHelper<TModel>: RazorHelperBase, ISetDynamicModel
    {
        public OqtRazorHelper(OqtRazorBase<TModel> owner) : base("Oqt.RzrHlp")
        {
            _owner = owner;
        }
        private readonly OqtRazorBase<TModel> _owner;

        #region DynamicCode Attachment / Handling through ViewData

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            _DynCodeRootMain = codeRoot;
        }

        const string DynCode = "_dynCode";

        public IDynamicCodeRoot _DynCodeRootMain
        {
            get
            {
                // Child razor page will have _dynCode == null, so it is provided via ViewData from parent razor page.
                if (_dynCode == null && _owner.ViewData?[DynCode] is IDynamicCodeRoot cdRt)
                    _dynCode = cdRt;

                return _dynCode;
            }

            set => _dynCode = value;
        }
        private IDynamicCodeRoot _dynCode;

        public ViewDataDictionary<TModel> HandleViewDataInject(ViewDataDictionary<TModel> value)
        {
            // Store _dynCode in ViewData, for child razor page.
            if (_dynCode != null && value != null && value[DynCode] == null)
                value[DynCode] = _dynCode;
            return value;
        }

        // ReSharper disable once InconsistentNaming
        public override IDynamicCodeRoot _DynCodeRoot => base._DynCodeRoot ?? _DynCodeRootMain;

        #endregion

        #region Dynamic Model / MyModel

        public dynamic DynamicModel => _dynamicModel ??= _owner.GetService<CodeDataWrapper>()
            .FromObject(_overridePageData ?? _owner.Model, WrapperSettings.Dyn(false, false));
        private dynamic _dynamicModel;
        private object _overridePageData;

        public void SetDynamicModel(object data)
        {
            _overridePageData = data;
        }

        public TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper()
        {
            var myModelData = (_overridePageData ?? _owner.Model)?.ToDicInvariantInsensitive();
            return new(_DynCodeRoot, _DynCodeRoot.Data, myModelData, true, _owner.Path);
        }


        #endregion

        #region GetCode / Create Instance

        protected override string GetCodeNormalizePath(string virtualPath)
        {
            var directory = Path.GetDirectoryName(_owner.Path)
                            ?? throw new("Current directory seems to be null");
            return Path.Combine(directory, virtualPath);
        }

        /// <summary>
        /// Cshtml CreateInstance - just throw error, as not supported in Oqtane
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        protected override object GetCodeCshtml(string path) =>
            throw new NotSupportedException(
                "CreateInstance with .cshtml files is not supported in Oqtane. Use a .cs file instead.");


        protected override string GetCodeFullPathForExistsCheck(string path)
        {
            var l = Log.Fn<string>(path);
            var pathFinder = _DynCodeRoot.GetService<IServerPaths>();
            var fullPath = pathFinder.FullAppPath(path);
            return l.ReturnAndLog(fullPath);
        }

        #endregion
    }
}

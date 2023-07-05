﻿using System;
using System.IO;
using Custom.Hybrid;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor
{
    internal class OqtRazorHelper<TModel>: RazorHelperBase, ISetDynamicModel
    {
        public OqtRazorHelper(OqtRazorBase<TModel> owner, string logName) : base(logName)
        {
            _owner = owner;
        }
        private readonly OqtRazorBase<TModel> _owner;

        //public OqtRazorHelper<TModel> Init(IHasDynamicCodeRoot owner)
        //{
        //    return this;
        //}

        #region DynamicCode Attachment / Handling through ViewData

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            _DynCodeRootMain = codeRoot;;
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
        public new IDynamicCodeRoot _DynCodeRoot => base._DynCodeRoot ?? _DynCodeRootMain;

        #endregion

        #region Dynamic Model / MyModel

        public dynamic DynamicModel => _dynamicModel ??= new DynamicReadObject(_owner.Model, true, false);
        private dynamic _dynamicModel;
        private object _overridePageData;

        public void SetDynamicModel(object data)
        {
            _overridePageData = data;
            //UpdateModel(data);
            _dynamicModel = new DynamicReadObject(data, false, false);
        }

        public TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper()
        {
            var myModelData = _overridePageData?.ObjectToDictionaryInvariant()
                              ?? _owner.Model?.ObjectToDictionary();
            return new TypedCode16Helper(_DynCodeRoot, _DynCodeRoot.Data, myModelData, true, _owner.Path);
        }


        #endregion

        #region Create Instance

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string razorPath,
            string noParamOrder = ToSic.Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var directory = System.IO.Path.GetDirectoryName(razorPath)
                            ?? throw new("Current directory seems to be null");
            var path = System.IO.Path.Combine(directory, virtualPath);
            VerifyFileExists(path);

            return path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                : throw new NotSupportedException("CreateInstance with .cshtml files is not supported in Oqtane. Use a .cs file instead. ");
        }


        private void VerifyFileExists(string path)
        {
            var pathFinder = _DynCodeRoot.GetService<IServerPaths>();
            var finalPath = pathFinder.FullAppPath(path);
            if (!File.Exists(finalPath))
                throw new FileNotFoundException("The shared file does not exist.", path);
        }


        #endregion
    }
}
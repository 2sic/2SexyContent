﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12<TModel>
    {
        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;

        #endregion


        #region Link, Edit, App, Data

        /// <inheritdoc />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc />
        public IEditService Edit => _DynCodeRoot.Edit;

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot.Data;

        #endregion

        #region AsDynamic in many variations

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot.AsDynamic(json, fallback);

        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);

        /// <inheritdoc/>
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsDynamic(entities);


        #endregion

        #region AsEntity
        /// <inheritdoc/>
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsEntity(dynamicEntity);
        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot.AsList(list);

        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);


        #endregion

        #region CreateDataSource new v15

        public T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
            => throw new Exception(DynamicCodeConstants.ErrorCreateDataSourceRequiresV14);

        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default)
            => throw new Exception(DynamicCodeConstants.ErrorCreateDataSourceRequiresV14);

        #endregion

        #region Convert-Service
        [PrivateApi] public IConvertService Convert => _DynCodeRoot.Convert;

        #endregion

        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);

        #endregion

        #region CmsContext 

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion

    }

}

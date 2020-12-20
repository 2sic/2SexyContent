﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {

        #region AsDynamic Implementations

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynamicJacket.AsDynamicJacket(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity)
            => new DynamicEntity(entity, CmsContext.SafeLanguagePriorityCodes(), CompatibilityLevel, Block);

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => dynamicEntity;


        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => ((IDynamicEntity)dynamicEntity).Entity;

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list)
        {
            switch (list)
            {
                case null:
                    return new List<dynamic>();
                case IDataSource dsEntities:
                    return AsList(dsEntities[Eav.Constants.DefaultStreamName]);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(e => AsDynamic(e));
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities;
                default:
                    return null;
            }
        }

        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => AsAdam(AsEntity(entity), fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
        {
            if (_adamManager == null)
                _adamManager = GetService<AdamManager>()
                    .Init(Block.Context, CompatibilityLevel, Log);
            return _adamManager.FolderOfField(entity.EntityGuid, fieldName);
        }
        private AdamManager _adamManager;

        #endregion
    }
}

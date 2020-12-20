﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a List of <see cref="IDynamicEntity"/>, which also behaves as an IDynamicEntity itself. <br/>
    /// So if it has any elements you can enumerate it (foreach). <br/>
    /// You can also do things like `.EntityId` or `.SomeProperty` just like a DynamicEntity.
    /// </summary>
    /// <remarks>Added in 2sxc 10.27</remarks>
    [PublicApi_Stable_ForUseInYourCode]
    public partial class DynamicEntityWithList: DynamicEntity, IReadOnlyList<IDynamicEntity>
    {
        [PrivateApi]
        protected List<IDynamicEntity> DynEntities;

        // ReSharper disable once NotAccessedField.Local
        private string _debugFieldName;

        [PrivateApi]
        internal DynamicEntityWithList(IEntity parent, string field, IEnumerable<IEntity> entities, string[] dimensions, int compatibility, IBlock block) 
            : base(null, dimensions, compatibility, block)
        {
            _debugFieldName = field; // remember name in case we do debugging and need to know what list was accessed
            var index = 0;
            DynEntities = entities
                .Select(e =>
                {
                    // we create an Entity with some metadata-decoration, so that toolbars know it's part of a list
                    var blockEntity = new EntityInBlock(e, parent.EntityGuid, field, index++);
                    return SubDynEntity(blockEntity);
                })
                .ToList();
            SetEntity(DynEntities.FirstOrDefault()?.Entity
                     // check empty list - create a dummy Entity so toolbars will know what to do
                     ?? EntityInBlock.PlaceHolder(parent, field));
        }

        public IEnumerator<IDynamicEntity> GetEnumerator() => DynEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => DynEntities.Count;

        public IDynamicEntity this[int index]
        {
            get => DynEntities[index];
            // setter is for IList<IDynamicEntity>
            set => throw new NotImplementedException();
        }
    }
}

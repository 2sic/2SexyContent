﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Data
{
    // backward compatible enumeration interface for people using IList<dynamic>
    public partial class DynamicEntityWithList : IList<object>
    {
        #region Implemented features as read-only List
        IEnumerator<object> IEnumerable<object>.GetEnumerator() => DynEntities.GetEnumerator();
        public bool Contains(object item) => DynEntities.Contains(item);

        public int IndexOf(object item) => DynEntities.IndexOf(item as IDynamicEntity);
        #endregion

        #region Not implemented IList interfaces

        public void Add(object item) => throw new NotImplementedException();

        public void CopyTo(object[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(object item) => throw new NotImplementedException();


        public void Insert(int index, object item) => throw new NotImplementedException();

        #endregion

        object IList<object>.this[int index]
        {
            get => (this as IList<IDynamicEntity>)[index];
            set => throw new NotImplementedException();
        }
    }
}

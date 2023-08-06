﻿using System;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public int EntityId => Entity?.EntityId ?? 0;


        /// <inheritdoc />
        public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;


        /// <inheritdoc />
        public IField Field(string name) => (this as ITypedItem).Field(name, strict: null);

        IField ITypedItem.Field(string name, string noParamOrder, bool? strict) =>
            IsErrStrict(name, strict, StrictGet)
                ? throw ErrStrict(name)
                : new Field(this, name, _Services);

        /// <inheritdoc />
        public string EntityType => Entity?.Type?.Name;

    }
}

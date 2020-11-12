﻿using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public double AnyBooleanProperty { get; }

        /// <inheritdoc />
        public DateTime AnyDateTimeProperty { get; }

        /// <inheritdoc />
        public IEnumerable<DynamicEntity> AnyChildrenProperty { get; }

        /// <inheritdoc />
        public IEnumerable<DynamicEntity> AnyJsonProperty { get; }

        /// <inheritdoc />
        public string AnyLinkOrFileProperty { get; }

        /// <inheritdoc />
        public double AnyNumberProperty { get; }

        /// <inheritdoc />
        public string AnyStringProperty { get; }
    }
}

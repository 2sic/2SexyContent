﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data;

[PrivateApi("Hide implementation")]
internal partial class Metadata: DynamicEntity, IMetadata, IHasPropLookup
{
    internal Metadata(IMetadataOf metadata, CodeDataFactory cdf)
        : base(metadata, null, "Metadata(virtual-field)", Eav.Constants.TransientAppId, propsRequired: false, cdf)
    {
        _metadata = metadata;
    }
    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new PropLookupMetadata(this, () => Debug);
    private PropLookupMetadata _propLookup;

    [PrivateApi]
    private CodeItemHelper ItemHelper => _itemHelper ??= new CodeItemHelper(GetHelper, this);
    private CodeItemHelper _itemHelper;

    [PrivateApi("Hide this")]
    private readonly IMetadataOf _metadata;


    IMetadataOf IHasMetadata.Metadata => _metadata;



    public override bool HasType(string type) => _metadata.HasType(type);

    public override IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

    #region Properties from the interfaces which are not really supported

    public override bool IsDemoItem => false;

    public new ITypedItem Presentation => throw new NotSupportedException();

    #endregion

    #region Equals

    public override bool Equals(object b)
    {
        if (b is null) return false;
        if (ReferenceEquals(this, b)) return true;
        if (b.GetType() != GetType()) return false;

        // TODO: ATM not clear how to best do this
        // probably need to check what's inside the PreWrap...
        //return EqualsWrapper(this, (IWrapper<IEntity>)b);
        return false;
    }

    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    #endregion

}
﻿using System.Collections.Generic;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Code
{
    public class TypedCode16Helper
    {
        private readonly IDynamicCodeRoot _codeRoot;
        private readonly IDictionary<string, object> _myModelData;
        private readonly bool _isRazor;
        private readonly string _codeFileName;
        internal ContextData Data { get; }
        public TypedCode16Helper(IDynamicCodeRoot codeRoot, IContextData data, IDictionary<string, object> myModelData, bool isRazor, string codeFileName)
        {
            _codeRoot = codeRoot;
            _myModelData = myModelData;
            _isRazor = isRazor;
            _codeFileName = codeFileName;
            Data = data as ContextData;
        }

        public ITypedItem MyItem => _myItem.Get(() => _codeRoot.AsC.AsItem(Data.MyContent));
        private readonly GetOnce<ITypedItem> _myItem = new GetOnce<ITypedItem>();

        public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => _codeRoot.AsC.AsItems(Data.MyContent));
        private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new GetOnce<IEnumerable<ITypedItem>>();

        public ITypedItem MyHeader => _myHeader.Get(() => _codeRoot.AsC.AsItem(Data.MyHeader));
        private readonly GetOnce<ITypedItem> _myHeader = new GetOnce<ITypedItem>();

        public ITypedModel MyModel => _myModel.Get(() => new TypedModel(_myModelData, _codeRoot, _isRazor, _codeFileName));
        private readonly GetOnce<ITypedModel> _myModel = new GetOnce<ITypedModel>();

    }
}
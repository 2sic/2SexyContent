﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Compatibility
{
    [Obsolete("this is just a workaround so that old code still works - especially Mobius forms which used this in V3")]
    public class CacheWithGetContentType
    {
        private readonly AppState _app;

        internal CacheWithGetContentType(AppState app)
        {
            _app = app;
        }

        public IContentType GetContentType(string typeName)
            => _app.GetContentType(typeName);
    }
}

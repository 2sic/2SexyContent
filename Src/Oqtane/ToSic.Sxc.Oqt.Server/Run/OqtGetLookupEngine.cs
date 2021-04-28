﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Shared;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine: HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {
        private readonly Lazy<LookUpInQueryString> _lookUpInQueryString;

        public OqtGetLookupEngine(Lazy<LookUpInQueryString> lookUpInQueryString) : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
            _lookUpInQueryString = lookUpInQueryString;
        }

        public ILookUpEngine GetLookUpEngine(int instanceId/*, ILog parentLog*/)
        {
            var providers = new LookUpEngine(Log);

            var dummy = new Dictionary<string, string>();
            dummy.Add("Ivo", "Ivić");
            dummy.Add("Pero","Perić");
            providers.Add(new LookUpInDictionary("dummy", dummy));

            providers.Add(_lookUpInQueryString.Value);
            providers.Add(new DateTimeLookUps());
            providers.Add(new TicksLookUps());

            return providers;
        }


    }


    public class LookUpInQueryString : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IQueryCollection _source;

        public LookUpInQueryString(IHttpContextAccessor httpContextAccessor)
        {
            Name = "QueryString";
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Get(string key, string format)
        {
            _source ??= _httpContextAccessor?.HttpContext?.Request.Query;
            if (_source == null) return string.Empty;
            return _source.TryGetValue(key, out var result) ? result.ToString() : string.Empty;
        }
    }

    public class DateTimeLookUps : LookUpBase
    {
        public DateTimeLookUps()
        {
            Name = "DateTime";
        }

        public override string Get(string key, string format)
        {
            return key.ToLowerInvariant() switch
            {
                "now" => DateTime.Now.ToString(format),
                _ => string.Empty
            };
        }
    }

    public class TicksLookUps : LookUpBase
    {
        public TicksLookUps()
        {
            Name = "Ticks";
        }

        public override string Get(string key, string format)
        {
            return key.ToLowerInvariant() switch
            {
                "now" => DateTime.Now.Ticks.ToString(format),
                "today" => DateTime.Today.Ticks.ToString(format),
                "ticksperday" => TimeSpan.TicksPerDay.ToString(format),
                _ => string.Empty
            };
        }
    }
}

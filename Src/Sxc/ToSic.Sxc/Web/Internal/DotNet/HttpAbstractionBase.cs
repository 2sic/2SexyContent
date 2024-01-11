﻿#if NETFRAMEWORK
using System.Web;
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Http;
#endif
using System.Collections.Specialized;


namespace ToSic.Sxc.Web.Internal.DotNet;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class HttpAbstractionBase: IHttp
{
    /// <inheritdoc />
    public HttpContext Current { get; protected set; }

    #region Request and properties thereof

    /// <inheritdoc />
    public HttpRequest Request => Current?.Request;

    /// <inheritdoc />
    public abstract NameValueCollection QueryStringParams { get; }

    /// <inheritdoc />
    public List<KeyValuePair<string, string>> QueryStringKeyValuePairs()
    {
        if (_queryStringKeyValuePairs != null) return _queryStringKeyValuePairs;
        var qs = QueryStringParams;
        _queryStringKeyValuePairs = qs?.AllKeys
                                        .Select(key => new KeyValuePair<string, string>(key, qs[key]))
                                        .ToList()
                                    ?? new List<KeyValuePair<string, string>>();
        return _queryStringKeyValuePairs;
    }
    private List<KeyValuePair<string, string>> _queryStringKeyValuePairs;

    #endregion Request

    //public abstract IDictionary<object, object> Items { get; }

}
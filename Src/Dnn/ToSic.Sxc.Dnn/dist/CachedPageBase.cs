﻿using System;
using System.IO;
using System.Web.Caching;

namespace ToSic.Sxc.Dnn.dist
{
    public class CachedPageBase : System.Web.UI.Page
    {
        protected string PageOutputCached(string virtualPath)
        {
            var key = CacheKey(virtualPath);
            if (Cache.Get(key) is string html) return html;
            var path = GetPath(virtualPath);
            html = File.ReadAllText(path);
            Cache.Insert(key, html, new CacheDependency(path));
            return html;
        }

        private static string CacheKey(string virtualPath) => $"2sxc-edit-ui-page-{virtualPath}";
        
        internal string GetPath(string virtualPath)
        {
            var path = Server.MapPath(virtualPath);
            if (!File.Exists(path)) throw new Exception("File not found: " + path);
            return path;
        }
    }
}
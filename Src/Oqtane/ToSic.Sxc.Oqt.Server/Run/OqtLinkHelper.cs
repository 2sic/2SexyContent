﻿using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using Custom.Hybrid;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Web;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// The Oqtane implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class OqtLinkHelper : IOqtLinkHelper, IHasLog
    {
        public Razor12 RazorPage { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IAliasRepository> _aliasRepositoryLazy;
        private readonly IPageRepository _pageRepository;
        private readonly SiteState _siteState;

        public OqtLinkHelper(
            IHttpContextAccessor httpContextAccessor,
            Lazy<IAliasRepository> aliasRepositoryLazy,
            IPageRepository pageRepository,
            SiteState siteState
        )
        {
            Log = new Log("OqtLinkHelper");
            // TODO: logging

            _httpContextAccessor = httpContextAccessor;
            _aliasRepositoryLazy = aliasRepositoryLazy;
            _pageRepository = pageRepository;
            _siteState = siteState;
        }

        public ILinkHelper Init(Razor12 razorPage)
        {
            RazorPage = razorPage;
            return this;
        }

        public ILog Log { get; }

        /// <inheritdoc />
        public string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null)
        {
            // prevent incorrect use without named parameters
            if (requiresNamedParameters != null)
                throw new Exception("The Link.To can only be used with named parameters. try Link.To( parameters: \"tag=daniel&sort=up\") instead.");

            var alias = _siteState.Alias;

            var currentPageId = RazorPage._DynCodeRoot?.CmsContext?.Page?.Id;

            var pid = pageId ?? currentPageId;
            if (pid == null)
                throw new Exception($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

            var page = _pageRepository.GetPage(pid.Value);

            return Oqtane.Shared.Utilities.NavigateUrl(alias.Path, page.Path, parameters);
        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));
        }

        public string Api(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, string path = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Api", $"{nameof(path)}");

            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.ForwardSlash();
            path = path.TrimPrefixSlash();

            //if (path.PrefixSlash().ToLowerInvariant().Contains("/app/"))
            //    throw new ArgumentException("Error, path shouldn't have \"app\" part in it. It is expected to be relative to application root.");

            //if (!path.PrefixSlash().ToLowerInvariant().Contains("/api/"))
            //    throw new ArgumentException("Error, path should have \"api\" part in it.");

            // TODO: build url with 'app'/'applicationName'

            var siteRoot = OqtAssetsAndHeaders.GetSiteRoot(_siteState).TrimLastSlash();
            return $"{siteRoot}/{path}";
        }
    }
}

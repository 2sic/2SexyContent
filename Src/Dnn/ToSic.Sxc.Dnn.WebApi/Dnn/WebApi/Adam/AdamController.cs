﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Dnn.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]    // use view, all methods must re-check permissions
    [ValidateAntiForgeryToken]
    public class AdamController : SxcApiControllerBase<AdamControllerReal<int>>, IAdamController<int>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://go.2sxc.org/proxy-controllers

        public AdamController() : base("Adam") { }

        [HttpPost]
        [HttpPut]
        public object Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false) 
            => SysHlp.Real.Upload(new HttpUploadedFile(Request, HttpContext.Current.Request), appId, contentType, guid, field, subFolder, usePortalRoot);

        // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(

        [HttpGet]
        public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
            => SysHlp.Real.Items(appId, contentType, guid, field, subfolder, usePortalRoot);


        [HttpPost]
        public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => SysHlp.Real.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);


        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
            => SysHlp.Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);


        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
            => SysHlp.Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}
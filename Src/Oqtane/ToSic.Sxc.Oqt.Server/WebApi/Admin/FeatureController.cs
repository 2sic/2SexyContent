﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.Configuration;
using ToSic.Eav.WebApi.Admin.Features;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    [ValidateAntiForgeryToken]

    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    public class FeatureController : OqtStatefulControllerBase<FeatureControllerReal>, IFeatureController
    {
        public FeatureController(): base(FeatureControllerReal.LogSuffix)
        { }


        /// <summary>
        /// POST updated features JSON configuration.
        /// </summary>
        /// <remarks>
        /// Added in 2sxc 13
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = RoleNames.Host)]
        public bool SaveNew([FromBody] List<FeatureManagementChange> changes) => Real.SaveNew(changes);

    }
}
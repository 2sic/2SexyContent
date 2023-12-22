﻿using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Sys.Licenses;
using RealController = ToSic.Eav.WebApi.Sys.Licenses.LicenseControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LicenseController : DnnApiControllerWithFixes, ILicenseController
{
    public LicenseController() : base("License") { }

    private RealController Real => SysHlp.GetService<RealController>();

    /// <summary>
    /// Make sure that these requests don't land in the normal api-log.
    /// Otherwise each log-access would re-number what item we're looking at
    /// </summary>
    protected override string HistoryLogGroup => "web-api.license";

    /// <inheritdoc />
    [HttpGet]
    public IEnumerable<LicenseDto> Summary() => Real.Summary();

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    public LicenseFileResultDto Upload()
    {
        SysHlp.PreventServerTimeout300();
        return Real.Upload(new HttpUploadedFile(Request, HttpContext.Current.Request));
    }

    /// <inheritdoc />
    [HttpGet]
    public LicenseFileResultDto Retrieve()
    {
        SysHlp.PreventServerTimeout300();
        return Real.Retrieve();
    }
}
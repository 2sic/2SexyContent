﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/sys/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/sys/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/sys/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/sys/install/[action]")]
    public class InstallController: OqtStatefulControllerBase
    {
        private readonly Lazy<IEnvironmentInstaller> _envInstallerLazy;
        private readonly Lazy<ImportFromRemote> _impFromRemoteLazy;
        protected override string HistoryLogName => "Api.Install";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.install";


        #region System Installation

        public InstallController(
            StatefulControllerDependencies dependencies,
            Lazy<IEnvironmentInstaller> envInstallerLazy,
            Lazy<ImportFromRemote> impFromRemoteLazy) : base(dependencies)
        {
            _envInstallerLazy = envInstallerLazy;
            _impFromRemoteLazy = impFromRemoteLazy;
        }

        /// <summary>
        /// Finish system installation which had somehow been interrupted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        [Authorize(Roles = RoleNames.Host)]
        public bool Resume() => _envInstallerLazy.Value.ResumeAbortedUpgrade();

        #endregion


        #region App / Content Package Installation

        /// <summary>
        /// Before this was GET Module/RemoteInstallDialogUrl
        /// </summary>
        /// <param name="isContentApp"></param>
        /// <returns></returns>
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult RemoteWizardUrl(bool isContentApp)
        {
            var result = _envInstallerLazy.Value.Init(Log)
                .GetAutoInstallPackagesUiUrl(
                    GetContext().Site,
                    GetContext().Module,
                    isContentApp);
            return Json(result);
        }

        /// <summary>
        /// Before this was GET Installer/InstallPackage
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <returns></returns>
        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        //[ValidateAntiForgeryToken] // now activate this, as it's post now, previously not, because this is a GET and can't include the RVT
        public IActionResult RemotePackage(string packageUrl)
        {
            PreventServerTimeout300();

            var oqtaneUser = GetContext().User;
            var container = GetContext().Module;
            bool isApp = !container.IsPrimary;

            Log.Add("install package:" + packageUrl);

            var block = container.BlockIdentifier;
            var result = _impFromRemoteLazy.Value.Init(oqtaneUser, Log)
                .InstallPackage(block.ZoneId, block.AppId, isApp, packageUrl);

            Log.Add("install completed with success:" + result.Item1);

            return (result.Item1 ? Ok(new { result.Item1, result.Item2 }) : Problem());
        }

        #endregion
    }
}

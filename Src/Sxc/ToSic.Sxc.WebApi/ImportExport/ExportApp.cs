﻿using System;
using System.Linq;
#if NETSTANDARD
using Microsoft.AspNetCore.Mvc;
#else
using System.IO;
using System.Net.Http;
using ToSic.Eav.WebApi.ImportExport;
#endif
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Shared;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;
using ISite = ToSic.Eav.Context.ISite;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ExportApp: ServiceWithLog
    {
        #region Constructor / DI

        public ExportApp(
            IZoneMapper zoneMapper, 
            ZipExport zipExport, 
            CmsRuntime cmsRuntime, 
            ISite site, 
            IUser user, 
            GeneratorLog<ImpExpHelpers> impExpHelpers, 
            IFeaturesInternal features
            ) : base("Bck.Export") =>
            ConnectServices(
                _zoneMapper = zoneMapper,
                _zipExport = zipExport,
                _cmsRuntime = cmsRuntime,
                _site = site,
                _user = user,
                _features = features,
                _impExpHelpers = impExpHelpers
            );

        private readonly IZoneMapper _zoneMapper;
        private readonly ZipExport _zipExport;
        private readonly CmsRuntime _cmsRuntime;
        private readonly ISite _site;
        private readonly IUser _user;
        private readonly IFeaturesInternal _features;
        private readonly GeneratorLog<ImpExpHelpers> _impExpHelpers;


        #endregion

        public AppExportInfoDto GetAppInfo(int zoneId, int appId)
        {
            Log.A($"get app info for app:{appId} and zone:{zoneId}");
            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared, Log);
            var cultCount = _zoneMapper.CulturesWithState(_site).Count(c => c.IsEnabled);

            var cms = _cmsRuntime.Init(Log).InitQ(currentApp, true);

            return new AppExportInfoDto
            {
                Name = currentApp.Name,
                Guid = currentApp.NameId,
                Version = currentApp.VersionSafe(),
                EntitiesCount = cms.Entities.All.Count(e => !e.HasAncestor()),
                LanguagesCount = cultCount,
                TemplatesCount = cms.Views.GetAll().Count(),
                HasRazorTemplates = cms.Views.GetRazor().Any(),
                HasTokenTemplates = cms.Views.GetToken().Any(),
                FilesCount = zipExport.FileManager.AllFiles.Count() // PortalFilesCount
                    + (currentApp.AppState.HasCustomParentApp() ? 0 : zipExport.FileManagerGlobal.AllFiles.Count()), // GlobalFilesCount
                TransferableFilesCount = zipExport.FileManager.AllTransferableFiles.Count() // TransferablePortalFilesCount
                    + (currentApp.AppState.HasCustomParentApp() ? 0 : zipExport.FileManagerGlobal.AllTransferableFiles.Count()), // TransferableGlobalFilesCount
            };
        }

        internal bool SaveDataForVersionControl(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool withSiteFiles)
        {
            Log.A($"export for version control z#{zoneId}, a#{appId}, include:{includeContentGroups}, reset:{resetAppGuid}");
            SecurityHelpers.ThrowIfNotAdmin(_user.IsSiteAdmin); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            // Ensure feature available...
            SyncWithSiteFilesVerifyFeaturesOrThrow(_features, withSiteFiles);

            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared, Log);
            zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid, withSiteFiles);

            return true;
        }

        internal static void SyncWithSiteFilesVerifyFeaturesOrThrow(IFeaturesInternal features, bool withSiteFiles)
        {
            if (!withSiteFiles) return;
            features.ThrowIfNotEnabled("Requires features enabled to sync with site files ",
                BuiltInFeatures.AppSyncWithSiteFiles.Guid);
        }

#if NETSTANDARD
        public IActionResult Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
#else
        public HttpResponseMessage Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
#endif

        {
            Log.A($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
            SecurityHelpers.ThrowIfNotAdmin(_user.IsSiteAdmin); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared, Log);
            var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

            var fileName =
                $"2sxcApp_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}{addOnWhenContainingContent}.zip";
            Log.A($"file name:{fileName}");

            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {

                var fileBytes = fileStream.ToArray();
                Log.A("will stream so many bytes:" + fileBytes.Length);
                var mimeType = MimeHelper.FallbackType;
#if NETSTANDARD
                return new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName };
#else
                return HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, mimeType, new MemoryStream(fileBytes));
#endif
            }
        }
    }
}

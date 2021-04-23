﻿using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Data;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport;
using ToSic.Eav.ImportExport.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Persistence.Xml;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Oqt.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Oqt.Server.Run
{

    public class OqtXmlExporter : XmlExporter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Lazy<ISiteRepository> _siteRepositoryLazy;
        private readonly Lazy<IFileRepository> _fileRepositoryLazy;
        private readonly Lazy<IFolderRepository> _folderRepositoryLazy;
        private readonly Lazy<IServerPaths> _oqtServerPathsLazy;
        private readonly Lazy<ITenantResolver> _oqtTenantResolverLazy;
        private readonly Lazy<IValueConverter> _oqtValueConverterLazy;
        private readonly IContextResolver _ctxResolver;

        #region Constructor / DI

        public OqtXmlExporter(
            AdamManager<int, int> adamManager,
            IContextResolver ctxResolver,
            XmlSerializer xmlSerializer,
            IServiceProvider serviceProvider,
            IWebHostEnvironment hostingEnvironment,
            Lazy<IFileRepository> fileRepositoryLazy,
            Lazy<IFolderRepository> folderRepositoryLazy,
            Lazy<IServerPaths> oqtServerPathsLazy,
            Lazy<ISiteRepository> siteRepositoryLazy,
            Lazy<ITenantResolver> oqtTenantResolverLazy,
            Lazy<IValueConverter> oqtValueConverterLazy) : base(xmlSerializer, OqtConstants.LogName)
        {
            _serviceProvider = serviceProvider;
            _hostingEnvironment = hostingEnvironment;
            _siteRepositoryLazy = siteRepositoryLazy;
            _fileRepositoryLazy = fileRepositoryLazy;
            _folderRepositoryLazy = folderRepositoryLazy;
            _oqtServerPathsLazy = oqtServerPathsLazy;
            _oqtTenantResolverLazy = oqtTenantResolverLazy;
            _oqtValueConverterLazy = oqtValueConverterLazy;
            _ctxResolver = ctxResolver.Init(Log);
            AdamManager = adamManager;
        }

        //private readonly IFileManager _dnnFiles = FileManager.Instance;
        internal AdamManager<int, int> AdamManager { get; }

        private string _appFolder;

        public override XmlExporter Init(int zoneId, int appId, AppRuntime appRuntime, bool appExport, string[] attrSetIds, string[] entityIds, ILog parentLog)
        {
            var context = _ctxResolver.App(appId);
            var contextOfSite = _ctxResolver.Site();
            var oqtSite = (OqtSite) contextOfSite.Site;
            var app = AdamManager.AppRuntime.ServiceProvider.Build<App>().InitNoData(new AppIdentity(zoneId, appId), Log);

            // needed for TenantFileItem path resolving
            _appFolder = app.Folder;

            AdamManager.Init(context, 10, Log);
            Constructor(zoneId, appRuntime, app.AppGuid, appExport, attrSetIds, entityIds, parentLog);

            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument(oqtSite.DefaultCultureCode, Settings.ModuleVersion);

            return this;
        }

        #endregion

        public override void AddFilesToExportQueue()
        {
            // Add Adam Files To Export Queue
            var adamIds = AdamManager.Export.AppFiles;
            adamIds.ForEach(AddFileAndFolderToQueue);

            // also add folders in adam - because empty folders may also have metadata assigned
            var adamFolders = AdamManager.Export.AppFolders;
            adamFolders.ForEach(ReferencedFolderIds.Add);
        }

        protected override void AddFileAndFolderToQueue(int fileNum)
        {
            try
            {
                ReferencedFileIds.Add(fileNum);

                // also try to remember the folder
                try
                {
                    var file = _fileRepositoryLazy.Value.GetFile(fileNum);
                    ReferencedFolderIds.Add(file.FolderId);
                }
                catch
                {
                    // don't do anything, because if the file doesn't exist, its FOLDER should also not land in the queue
                }
            }
            catch
            {
                // don't do anything, because if the file doesn't exist, it should also not land in the queue
            }
        }

        protected override string ResolveFolderId(int folderId)
        {
            var folderController = _folderRepositoryLazy.Value;
            var folder = folderController.GetFolder(folderId);
            return folder?.Path;
        }

        protected override TenantFileItem ResolveFile(int fileId)
        {
            var serverPaths = _oqtServerPathsLazy.Value;
            var fileController = _fileRepositoryLazy.Value;
            var file = fileController.GetFile(fileId);
            var filePath = Path.Combine(file?.Folder.Path.Backslash(), file.Name);

            var alias = _oqtTenantResolverLazy.Value.GetAlias();
            var absolutePath = ContentFileHelper.GetFilePath(_hostingEnvironment.ContentRootPath, alias, "default", _appFolder, filePath);

            return new TenantFileItem
            {
                Id = fileId,
                RelativePath = absolutePath,
                Path = serverPaths.FullContentPath(absolutePath)
            };
        }

    }


}

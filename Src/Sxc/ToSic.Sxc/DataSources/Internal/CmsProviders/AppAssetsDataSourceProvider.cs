﻿using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Helpers;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Cms.Assets.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the AppFiles DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppAssetsDataSourceProvider(AppAssetsDataSourceProvider.MyServices services)
    : ServiceBase<AppAssetsDataSourceProvider.MyServices>(services, $"{SxcLogName}.AppFls")
{
    public class MyServices(IAppReaderFactory appReaders, IAppPathsMicroSvc appPathMicroSvc, Generator<FileManager> fileManagerGenerator)
        : MyServicesBase(connect: [appReaders, appPathMicroSvc, fileManagerGenerator])
    {
        /// <summary>
        /// Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source
        /// </summary>
        internal Generator<FileManager> FileManagerGenerator { get; } = fileManagerGenerator;

        internal IAppPathsMicroSvc AppPathMicroSvc { get; } = appPathMicroSvc;

        internal IAppReaderFactory AppReaders { get; } = appReaders;
    }

    public AppAssetsDataSourceProvider Configure(
        AppAssetsGetSpecs specs,
        NoParamOrder noParamOrder = default
        //int zoneId = default,
        //int appId = default
        //string root = default,
        //string filter = default
    )
    {
        var root = specs.RootFolder;
        var filter = specs.FileFilter;
        var l = Log.Fn<AppAssetsDataSourceProvider>($"a:{specs.AppId}; z:{specs.ZoneId}, root:{root}, filter:{filter}");
        _root = root.TrimPrefixSlash().Backslash();
        _filter = filter;

        var appState = Services.AppReaders.Get(new AppIdentity(specs.ZoneId, specs.AppId));
        _appPaths = Services.AppPathMicroSvc.Get(appState);
        
        _fileManager = Services.FileManagerGenerator.New().SetFolder(specs.AppId, _appPaths.PhysicalPath, _root);
        return l.Return(this);
    }

    private string _root;
    private string _filter;
    private FileManager _fileManager;
    private IAppPaths _appPaths;

    /// <summary>
    /// FYI: The filters are not actually implemented yet.
    /// So the core data source doesn't have settings to configure this
    /// </summary>
    /// <returns></returns>
    public (List<FolderModelRaw> Folders, List<FileModelRaw> Files) GetAll()
        => Log.Func(l => (Folders, Files));

    public List<FileModelRaw> Files => _files.GetM(Log, l =>
    {
        var pathsFromRoot = PreparePaths(_appPaths, "");

        var files = _fileManager.GetAllTransferableFiles(_filter)
            .Select(p => new FileInfo(p))
            .Select(f =>
            {
                var fullNameFromAppRoot = FullNameWithoutAppFolder(f.FullName, pathsFromRoot);
                var name = Path.GetFileNameWithoutExtension(f.FullName);
                var path = fullNameFromAppRoot.TrimPrefixSlash();
                return new FileModelRaw
                {
                    Name = name,
                    Extension = f.Extension.TrimStart('.'), // Extension is without the dot
                    FullName = $"{name}{f.Extension}",
                    ParentFolderInternal = path.BeforeLast("/").SuffixSlash(),
                    Path = path,
                    // TODO convert characters for safe HTML
                    Url = $"{_appPaths.Path}{fullNameFromAppRoot}",

                    Size = (int)f.Length,
                    Created = f.CreationTime,
                    Modified = f.LastWriteTime
                };
            })
            .ToList();

        return (files, $"files:{files.Count}");
    });
    private readonly GetOnce<List<FileModelRaw>> _files = new();

    public List<FolderModelRaw> Folders => _folders.GetM(Log, l =>
    {
        var pathsFromRoot = PreparePaths(_appPaths, "");

        var folders = _fileManager.GetAllTransferableFolders(/*filter*/)
            .Select(p => new DirectoryInfo(p))
            .Select(d => ToFolderData(d, pathsFromRoot))
            .ToList();

        // if the root is just "/" then we need to add the root folder, otherwise not
        var root = new DirectoryInfo(
            $"{_appPaths.PhysicalPath}/{_root}"
                .FlattenMultipleForwardSlashes()
                .TrimLastSlash()
        );
        folders.Insert(0, ToFolderData(root, pathsFromRoot) with
        {
            Name = "",                  // Make name blank, since it's the root folder
            ParentFolderInternal = "",  // reset the ParentFolder, otherwise the root thinks it's a subfolder of itself
        });
        
        return (folders, $"found:{folders.Count}");
    });

    private FolderModelRaw ToFolderData(DirectoryInfo d, PreparedPaths pathsFromRoot)
    {
        var fullNameFromAppRoot = FullNameWithoutAppFolder(d.FullName, pathsFromRoot);

        var name = Path.GetFileName(d.FullName);

        return new()
        {
            Name = name,
            FullName = name,
            ParentFolderInternal = fullNameFromAppRoot.TrimPrefixSlash().BeforeLast("/").SuffixSlash(),
            Path = fullNameFromAppRoot.TrimPrefixSlash().SuffixSlash(),
            Created = d.CreationTime,
            Modified = d.LastWriteTime,
            Url = $"{_appPaths.Path}{fullNameFromAppRoot}",
        };
    }

    private readonly GetOnce<List<FolderModelRaw>> _folders = new();


    /// <summary>
    /// </summary>
    /// <returns></returns>
    private static string FullNameWithoutAppFolder(string path, PreparedPaths paths)
    {
        if (path == null)
            return string.Empty;

        var name = path.Replace(paths.AppSitePath, string.Empty);
        if (string.IsNullOrEmpty(name))
            return string.Empty;
        if (paths.HasShared) 
            name = name.Replace(paths.AppSharedPath, string.Empty);
        return name.ForwardSlash();
    }

    private static PreparedPaths PreparePaths(IAppPaths appPaths, string root)
    {
        var hasShared = appPaths.PhysicalPathShared != null;
        var appSharedPath = hasShared
            ? Path.Combine(appPaths.PhysicalPathShared, root)
            : "";
        return new(Path.Combine(appPaths.PhysicalPath, root), hasShared, appSharedPath);
    }

    private record PreparedPaths(string AppSitePath, bool HasShared, string AppSharedPath);
}
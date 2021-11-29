﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend: HasLog<AppAssetsBackend>
    {

        #region Constructor / DI

        public AppAssetsBackend(TemplateHelpers templateHelpers,
            IUser user, 
            Lazy<AssetEditor> assetEditorLazy,
            IServiceProvider serviceProvider,
            IAppStates appStates) : base("Bck.Assets")
        {

            _templateHelpers = templateHelpers;
            _assetEditorLazy = assetEditorLazy;
            _assetTemplates = new AssetTemplates().Init(Log);
            _serviceProvider = serviceProvider;
            _appStates = appStates;
            _user = user;
        }
        private readonly TemplateHelpers _templateHelpers;
        private readonly Lazy<AssetEditor> _assetEditorLazy;
        private readonly AssetTemplates _assetTemplates;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppStates _appStates;
        private readonly IUser _user;

        #endregion


        public AssetEditInfo Get(int appId, int templateId, string path, bool global)
        {
            var wrapLog = Log.Call<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor.EditInfoWithSource);
        }


        public bool Save(int appId, AssetEditInfo template, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<bool>($"templ:{templateId}, global:{global}, path:{path}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.Source = template.Code;
            return wrapLog(null, true);
        }

        [Obsolete("This Method is Deprecated", false)]
        public bool Create(int appId, string path, FileContentsDto content, string purpose, bool global = false)
        {
            Log.Add($"create a#{appId}, path:{path}, global:{global}, purpose:{purpose}, cont-length:{content.Content?.Length}");
            path = path.Replace("/", "\\");

            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (content.Content == null)
                content.Content = "";

            path = SanitizePathAndContent(path, content, purpose);

            var assetEditor = _assetEditorLazy.Value.Init(thisApp, path, global, 0, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(path);
            return assetEditor.Create(content.Content);
        }

        public bool Create(AssetFromTemplateDto assetFromTemplateDto)
        {
            Log.Add($"create a#{assetFromTemplateDto.AppId}, path:{assetFromTemplateDto.Path}, global:{assetFromTemplateDto.Global}, key:{assetFromTemplateDto.TemplateKey}");
            assetFromTemplateDto.Path = assetFromTemplateDto.Path.Replace("/", "\\");

            var thisApp = _serviceProvider.Build<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, assetFromTemplateDto.AppId), Log);

            // get and prepare template content
            var content = GetTemplateContent(assetFromTemplateDto);

            // ensure all .cshtml start with "_"
            EnsureCshtmlStartWithUnderscore(assetFromTemplateDto);

            var assetEditor = _assetEditorLazy.Value.Init(thisApp, assetFromTemplateDto.Path, assetFromTemplateDto.Global, 0, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(assetFromTemplateDto.Path);
            return assetEditor.Create(content);
        }

        public TemplatesDto GetTemplates(string purpose)
        {
            var templateInfos = _assetTemplates.GetTemplates();

            // TBD: future purpose implementation

            return new TemplatesDto
            {
                Default = AssetTemplates.RazorHybrid.Key,
                Templates = templateInfos
            };
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
            var app = _serviceProvider.Build<Apps.App>().InitNoData(_appStates.Identity(null, appId), Log);
            var assetEditor = _serviceProvider.Build<AssetEditor>();

            // TODO: simplify once we release v13 #cleanUp EOY 2021
            if (path == null)
                assetEditor.Init(app, templateId, Log);
            else
                assetEditor.Init(app, path, global, templateId, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor);
        }

        // TODO STV
        // - check if file can be created or already exists - if not, set Error-property
        // - run the code that would generate the file, so the UI can show a real preview
        public TemplatePreviewDto GetPreview(int appId, string path, string name, string templateKey, bool b)
        {
            return new TemplatePreviewDto
            {
                Preview = _assetTemplates.GetTemplate(templateKey)
            };
        }
    }
}

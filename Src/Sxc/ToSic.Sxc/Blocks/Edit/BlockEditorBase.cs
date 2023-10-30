﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppSys;
using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Apps.CmsSys;

namespace ToSic.Sxc.Blocks.Edit
{
    // todo: create interface
    // todo: move some parts out into a BlockManagement
    public abstract partial class BlockEditorBase : ServiceBase<BlockEditorBase.MyServices>
    {
        #region DI and Construction

        public class MyServices : MyServicesBase
        {
            public LazySvc<AppWork> AppSys { get; }
            public LazySvc<AppBlocks> AppBlocks { get; }
            public LazySvc<CmsManager> CmsManager { get; }
            public LazySvc<AppManager> AppManager { get; }
            public Generator<BlockEditorForModule> BlkEdtForMod { get; }
            public Generator<BlockEditorForEntity> BlkEdtForEnt { get; }

            public MyServices(
                LazySvc<AppWork> appSys,
                LazySvc<AppBlocks> appBlocks,
                LazySvc<CmsManager> cmsManager,
                LazySvc<AppManager> appManager,
                Generator<BlockEditorForModule> blkEdtForMod,
                Generator<BlockEditorForEntity> blkEdtForEnt)
            {
                ConnectServices(
                    CmsManager = cmsManager,
                    AppManager = appManager,
                    BlkEdtForMod = blkEdtForMod,
                    BlkEdtForEnt = blkEdtForEnt,
                    AppSys = appSys,
                    AppBlocks = appBlocks
                );
            }
        }

        internal BlockEditorBase(MyServices services) : base(services, "CG.RefMan")
        {
            Services.CmsManager.SetInit(r => r.Init(Block?.App));
            Services.AppManager.SetInit(r => r.Init(Block?.App));
        }

        internal void Init(IBlock block) => Block = block;

        #endregion

        private CmsManager CmsManager => Services.CmsManager.Value;
        private AppManager AppManager => Services.AppManager.Value;

        protected IBlock Block;

        private BlockConfiguration _cGroup;
        
        #region methods which are fairly stable / the same across content-block implementations

        protected BlockConfiguration BlockConfiguration => _cGroup ?? (_cGroup = Block.Configuration);
        
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            Guid? result;
            Log.A($"save template#{templateId}, CG-exists:{BlockConfiguration.Exists} forceCreateCG:{forceCreateContentGroup}");

            // if it exists or has a force-create, then write to the Content-Group, otherwise it's just a preview
            if (BlockConfiguration.Exists || forceCreateContentGroup)
            {
                var existedBeforeSettingTemplate = BlockConfiguration.Exists;
                var contentGroupGuid = CmsManager.Blocks.UpdateOrCreateContentGroup(BlockConfiguration, templateId);

                if (!existedBeforeSettingTemplate) EnsureLinkToContentGroup(contentGroupGuid);

                result = contentGroupGuid;
            }
            else
            {
                // only set preview / content-group-reference - but must use the guid
                var dataSource = Block.App.Data;
                var templateGuid = dataSource.List.One(templateId).EntityGuid;
                SavePreviewTemplateId(templateGuid);
                result = null; // send null back
            }

            return result;
        }

        public bool Publish(string part, int index)
        {
            Log.A($"publish part{part}, order:{index}");
            var contentGroup = BlockConfiguration;
            var contEntity = contentGroup[part][index];
            var presKey = part.ToLowerInvariant() == ViewParts.ContentLower 
                ? ViewParts.PresentationLower 
                : ViewParts.ListPresentationLower;
            var presEntity = contentGroup[presKey][index];

            var hasPresentation = presEntity != null;

            // make sure we really have the draft item an not the live one
            var contDraft = contEntity.IsPublished ? AppManager.AppState.GetDraft(contEntity) : contEntity;
            AppManager.Entities.Publish(contDraft.RepositoryId);
            
            if (hasPresentation)
            {
                var presDraft = presEntity.IsPublished ? AppManager.AppState.GetDraft(presEntity) : presEntity;
                AppManager.Entities.Publish(presDraft.RepositoryId);
            }

            return true;
        }

        private AppManager BlockAppManager => Services.AppManager.Value;

        #endregion

    }
}
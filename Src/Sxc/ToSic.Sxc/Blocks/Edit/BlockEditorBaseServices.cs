﻿using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Blocks.Edit
{
    public class BlockEditorBaseServices: MyServicesBase
    {
        public LazySvc<CmsRuntime> CmsRuntime { get; }
        public LazySvc<CmsManager> CmsManager { get; }
        public LazySvc<AppManager> AppManager { get; }
        public Generator<BlockEditorForModule> BlkEdtForMod { get; }
        public Generator<BlockEditorForEntity> BlkEdtForEnt { get; }

        public BlockEditorBaseServices(LazySvc<CmsRuntime> cmsRuntime, 
            LazySvc<CmsManager> cmsManager, 
            LazySvc<AppManager> appManager,
            Generator<BlockEditorForModule> blkEdtForMod,
            Generator<BlockEditorForEntity> blkEdtForEnt)
        {
            ConnectServices(
                CmsRuntime = cmsRuntime,
                CmsManager = cmsManager,
                AppManager = appManager,
                BlkEdtForMod = blkEdtForMod,
                BlkEdtForEnt = blkEdtForEnt
            );
        }
    }
}
﻿using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run.Context;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// A unit / block of output in a CMS. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public interface IBlock: IAppIdentity, IHasLog
    {
        /// <summary>
        /// The module ID or the parent-content-block id, probably not ideal here, but not sure
        /// </summary>
        [PrivateApi]
        int ParentId { get; }

        [PrivateApi]
        bool DataIsMissing { get; }

        [PrivateApi]
        int ContentBlockId { get; }

        #region Values related to the current unit of content / the view
        
        /// <summary>
        /// The context we're running in, with tenant, container etc.
        /// </summary>
        IContextOfBlock Context { get; }

        /// <summary>
        /// The view which will be used to render this block
        /// </summary>
        IView View { get; set; }

        [PrivateApi("unsure if this should be public, or only needed to initialize it?")]
        BlockConfiguration Configuration { get; }

        /// <summary>
        /// The app this block is running in
        /// </summary>
        IApp App { get; }

        /// <summary>
        /// The <see cref="IBlockDataSource"/> which delivers data for this block (will be used by the <see cref="IEngine"/> together with the View)
        /// </summary>
        IBlockDataSource Data { get; }

        [PrivateApi("might rename this some time")]
        bool IsContentApp { get; }
        #endregion

        [PrivateApi("naming not final")]
        IBlockBuilder BlockBuilder { get; }

        [PrivateApi("naming not final")]
        bool ContentGroupExists { get; }

        [PrivateApi]
        bool EditAllowed { get; }
    }
}

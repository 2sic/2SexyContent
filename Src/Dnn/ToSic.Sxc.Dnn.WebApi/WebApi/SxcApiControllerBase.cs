﻿using System.Web.Http.Controllers;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcBlock context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [DnnLogExceptions]
    public class SxcApiControllerBase: DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.CntBas";

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            SharedContextResolver = ServiceProvider.Build<IContextResolver>();
            SharedContextResolver.AttachRealBlock(() => BlockOfRequest);
            SharedContextResolver.AttachBlockContext(() => BlockOfRequest?.Context);
        }

        protected IContextResolver SharedContextResolver;

        private IBlock BlockOfRequest => _blockOfRequest ??
                                         (_blockOfRequest = ServiceProvider.Build<DnnGetBlock>().GetCmsBlock(Request, Log));
        private IBlock _blockOfRequest;

        [PrivateApi] protected IBlock GetBlock() => BlockOfRequest;

        ///// <summary>
        ///// Temporary call to replace GetBlock, so we can gradually filter out the bad uses of Block
        ///// </summary>
        ///// <returns></returns>
        //[PrivateApi] protected IBlock BlockReallyUsedAsBlock() => BlockOfRequest;

        #region App-Helpers for anonyous access APIs


        //protected IContextOfBlock GetContext()
        //{
        //    if (_context != null) return _context;
        //    if (BlockOfRequest?.Context != null) return _context = BlockOfRequest.Context;
        //    // in case the initial request didn't yet find a block builder, we need to create it now
        //    _context = ServiceProvider.Build<IContextOfBlock>();
        //    _context.Init(Log);
        //    _context.InitPageOnly();
        //    return _context;
        //}

        //private IContextOfBlock _context;

        #endregion
    }
}

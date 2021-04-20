﻿using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Edit.InPageEditingSystem;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Base class for any dynamic code root objects. <br/>
    /// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
    /// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
    /// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class DynamicCodeRoot : HasLog, IDynamicCodeRoot, IDynamicCode
    {
        #region Constructor

        /// <summary>
        /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
        /// </summary>
        public class Dependencies
        {
            public IServiceProvider ServiceProvider { get; }
            public ICmsContext CmsContext { get; }
            public ILinkHelper LinkHelper { get; }

            public Dependencies(IServiceProvider serviceProvider, ICmsContext cmsContext, ILinkHelper linkHelper)
            {
                ServiceProvider = serviceProvider;
                CmsContext = cmsContext;
                LinkHelper = linkHelper;
            }
        }

        protected DynamicCodeRoot(Dependencies dependencies, string logPrefix) : base(logPrefix + ".DynCdR")
        {
            _serviceProvider = dependencies.ServiceProvider;
            CmsContext = dependencies.CmsContext;
            Link = dependencies.LinkHelper;
        }

        private readonly IServiceProvider _serviceProvider;

        [PrivateApi] public ICmsContext CmsContext { get; }

        #endregion


        /// <inheritdoc />
        public TService GetService<TService>() => _serviceProvider.Build<TService>();

        [PrivateApi]
        public DynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility = 10)
        {
            Log.LinkTo(parentLog ?? block?.Log);
            if (block == null)
                return this;

            CompatibilityLevel = compatibility;
            ((CmsContext) CmsContext).Update(block.Context);
            Block = block;
            App = block.App;
            Data = block.Data;
            Edit = new InPageEditingHelper(block, Log);

            return this;
        }

        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IBlockDataSource Data { get; private set; }

        /// <inheritdoc />
        public ILinkHelper Link { get; protected set; }



        #region Edit

        /// <inheritdoc />
        public IInPageEditingSystem Edit { get; private set; }

        #endregion

        #region Accessor to Root

        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot => this;

        #endregion

    }
}
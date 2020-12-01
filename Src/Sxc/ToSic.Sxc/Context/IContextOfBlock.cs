﻿using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Context
{
    public interface IContextOfBlock: IContextOfSite, IContextOfApp
    {
        /// <summary>
        /// The page it's running on + parameters for queries, url etc.
        /// </summary>
        IPage Page { get; }

        /// <summary>
        /// The container for our block, basically the module
        /// </summary>
        IModule Module { get; }

        /// <summary>
        /// Publishing information about the current context
        /// </summary>
        BlockPublishingState Publishing { get; }
    }
}

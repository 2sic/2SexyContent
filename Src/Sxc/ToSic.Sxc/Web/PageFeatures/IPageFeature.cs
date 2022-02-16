﻿using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi("Internal / not final - neither name, namespace or anything")]
    public interface IPageFeature
    {
        /// <summary>
        /// Primary identifier to activate the feature
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Name of this feature. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Nice description of the feature.
        /// </summary>
        string Description { get; }

        string Html { get; }

        /// <summary>
        /// List of other features required to run this feature.
        /// </summary>
        IEnumerable<string> Requires { get; }

        bool AlreadyProcessed { get; set; }
    }
}
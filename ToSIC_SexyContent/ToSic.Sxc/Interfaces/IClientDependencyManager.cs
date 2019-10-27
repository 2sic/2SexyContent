﻿namespace ToSic.Sxc.Interfaces
{
    /// <summary>
    /// System to automatically pick up JS/CSS files which should be bundled
    /// </summary>
    internal interface IClientDependencyManager
    {
        /// <summary>
        /// Scan the html for possible JS/CSS files which should be bundled and extract these. 
        /// </summary>
        /// <param name="renderedTemplate">html to be rendered</param>
        /// <returns>Original html without the js/css tags which were bundled (so they get removed here)</returns>
        string Process(string renderedTemplate);
    }
}

﻿using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class DnnLinkHelper: ILinkHelper
    {
        private readonly IDnnContext _dnn;

        public DnnLinkHelper(IDnnContext dnn)
        {
            _dnn = dnn;
        }

        /// <inheritdoc />
        public string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null)
        {
            // prevent incorrect use without named parameters
            if(requiresNamedParameters != null)
                throw new Exception("The Link.To can only be used with named parameters. try Link.To( parameters: \"tag=daniel&sort=up\") instead.");

            var targetPage = pageId ?? _dnn.Tab.TabID;

            var parametersToUse = parameters;
            return parametersToUse == null
                ? _dnn.Tab.FullUrl
                : DotNetNuke.Common.Globals.NavigateURL(targetPage, "", parametersToUse);

        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));

        }
    }
}
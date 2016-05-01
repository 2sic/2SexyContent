﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SxcInstance GetSxcOfModuleContext(this HttpRequestMessage request)
        {
            string cbidHeader = "ContentBlockId";
            var moduleInfo = request.FindModuleInfo();

            // get url parameters and provide override values to ensure all configuration is 
            // preserved in AJAX calls
            NameValueCollection urlParams = new NameValueCollection(); 
            var requestParams = request.GetQueryNameValuePairs();
            // first, check the special overrides
            //var origparams = requestParams.Select(np => np.Key == "urlparameters").ToList();
            //if (origparams.Any())
            //{
            //    var paramSet = origparams.First();
            //}

            // then add remaining params
            foreach (KeyValuePair<string, string> keyValuePair in requestParams.Where(keyValuePair => keyValuePair.Key.IndexOf("orig", StringComparison.Ordinal) == 0))
                urlParams.Add(keyValuePair.Key.Substring(4), keyValuePair.Value);

            IContentBlock contentBlock = new ModuleContentBlock(moduleInfo);

            // check if we need an inner block
            if (request.Headers.Contains(cbidHeader)) { 
                var cbidh = request.Headers.GetValues(cbidHeader).FirstOrDefault();
                int cbid;
                int.TryParse(cbidh, out cbid);
                if (cbid < 0)   // negative id, so it's an inner block
                    contentBlock = new EntityContentBlock(contentBlock, cbid);
            }

            return contentBlock.SxcInstance;
        }

    }
}

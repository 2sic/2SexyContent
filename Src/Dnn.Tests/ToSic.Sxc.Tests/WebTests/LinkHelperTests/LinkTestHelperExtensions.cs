﻿using System;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    public static class LinkTestHelperExtensions
    {
        /// <summary>
        /// Special helper to avoid accessing the real To so many times
        /// </summary>
        public static string TestTo(this ILinkHelper link,
            string noParamOrder = Eav.Parameters.Protector,
            int? pageId = null,
            object parameters = null,
            string api = null,
            string type = null, // WIP, probably "full", "root", "https", "//", "http" etc.
            string part = null
        )
        {
            if (part != null) throw new Exception("part shouldn't be used ATM,not implemented");
            return link.To(noParamOrder: noParamOrder, pageId: pageId, parameters: parameters, api: api, type: type
                /*part: part*/);
        }


        public static string TestImage(this ILinkHelper link,
            string url = null,
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string part = null)
            => link.Image(url, settings, factor, noParamOrder, width, height, quality, resizeMode, scaleMode, format,
                aspectRatio, part);
    }
}

﻿using System.Web;

namespace ToSic.SexyContent.Html
{
    public class Build
    {
        /// <summary>
        /// Generate an HTML attribute 
        /// - but only if in edit mode
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// has trailing space to ensure attributes never "stick" together
        /// we also don't want to use {HttpUtility.HtmlAttributeEncode(value)...
        /// ...because it makes the html hard to work with when debugging
        /// so we just manually replace all apos to make sure it doesn't create invalid html
        /// </remarks>
        public static HtmlString Attribute(string name, string value)
            => new HtmlString($" {name}='{value.Replace("'", "&apos;")}'");
    }
}

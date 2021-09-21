﻿using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks.Renderers;
using ToSic.Sxc.Data;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif


namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
    /// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
    /// See also [](xref:Basics.Cms.InnerContent.Index)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class Render
    {
        /// <summary>
        /// Render one content block
        /// This is accessed through DynamicEntity.Render()
        /// At the moment it MUST stay internal, as it's not clear what API we want to surface
        /// </summary>
        /// <param name="dynParent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder"></param>
        /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
        /// <param name="field">Optional: </param>
        /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
        /// <returns></returns>
        public static IHtmlString One(DynamicEntity dynParent,
            string noParamOrder = Eav.Parameters.Protector,
            IDynamicEntity item = null, 
            string field = null,
            Guid? newGuid = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(One), $"{nameof(item)},{nameof(field)},{nameof(newGuid)}");
            if (item == null)
                item = dynParent;
            
            return new HtmlString(field == null
                ? Simple.Render(dynParent._Dependencies.BlockOrNull, item.Entity) // with edit-context
                : Simple.RenderWithEditContext(dynParent, item, field, newGuid) + "<b>data-list-context</b>"); // data-list-context (no edit-context)
        }

        /// <summary>
        /// Render content-blocks into a larger html-block containing placeholders
        /// </summary>
        /// <param name="context">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder"></param>
        /// <param name="field">Required: Field containing the content-blocks. </param>
        /// <param name="max">BETA / WIP</param>
        /// <param name="merge">Optional: html-text containing special placeholders.</param>
        /// <param name="apps">BETA / WIP</param>
        /// <returns></returns>
        public static IHtmlString All(DynamicEntity context,
            string noParamOrder = Eav.Parameters.Protector,
            string field = null, 
            string apps = null,
            int max = 100,
            string merge = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(All), $"{nameof(field)},{nameof(merge)}");
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return merge == null
                ? new HtmlString(Simple.RenderListWithContext(context, field))
                : new HtmlString(InTextContentBlocks.Render(context, field, merge));
        }
    }
}
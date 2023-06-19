﻿using System;
using ToSic.Eav.CodeChanges;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Markup;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn;
using static ToSic.Eav.CodeChanges.CodeChangeInfo;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
    /// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
    /// See also [](xref:Basics.Cms.InnerContent.Index)
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    [Obsolete("Deprecated in v12 - please use IRenderService instead - will not work in v12 Base classes like Razor12")]
    public class Render
    {
        /// <summary>
        /// Render one content block
        /// This is accessed through DynamicEntity.Render()
        /// At the moment it MUST stay internal, as it's not clear what API we want to surface
        /// </summary>
        /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
        /// <param name="field">Optional: </param>
        /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
        /// <returns></returns>
        /// <remarks>
        /// * Changed result object to `IRawHtmlString` in v16.02 from `IHybridHtmlString`
        /// </remarks>
        public static IRawHtmlString One(
            DynamicEntity parent,
            string noParamOrder = Eav.Parameters.Protector,
            ICanBeEntity item = null,
            string field = null,
            Guid? newGuid = null)
            => RenderServiceWithWarning(parent).One(parent, noParamOrder, item, data: null, field: field, newGuid: newGuid);

        /// <summary>
        /// Render content-blocks into a larger html-block containing placeholders
        /// </summary>
        /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="field">Required: Field containing the content-blocks. </param>
        /// <param name="max">BETA / WIP</param>
        /// <param name="merge">Optional: html-text containing special placeholders.</param>
        /// <param name="apps">BETA / WIP</param>
        /// <returns></returns>
        /// <remarks>
        /// * Changed result object to `IRawHtmlString` in v16.02 from `IHybridHtmlString`
        /// </remarks>
        public static IRawHtmlString All(
            DynamicEntity parent,
            string noParamOrder = Eav.Parameters.Protector,
            string field = null,
            string apps = null,
            int max = 100,
            string merge = null) 
            => RenderServiceWithWarning(parent).All(parent, noParamOrder, field, apps, max, merge);

        private static Services.IRenderService RenderServiceWithWarning(DynamicEntity parent)
        {
            var services = parent._Services;
            // First do version checks -should not be allowed if compatibility is too low
            if (services.CompatibilityLevel > Constants.MaxLevelForStaticRender)
                throw new Exception(
                    "The static ToSic.Sxc.Blocks.Render can only be used in old Razor components. For v12+ use the ToSic.Sxc.Services.IRenderService instead");


            var block = services.BlockOrNull;
            DnnStaticDi.CodeChanges.WarnSxc(WarnObsolete.UsedAs(appId: parent.Entity.AppId, specificId: $"View:{block?.View?.Id}"), block: block);

            return services.RenderService;
        }

        private static readonly ICodeChangeInfo WarnObsolete = V13To17("Deprecated Static RenderService", "https://go.2sxc.org/brc-13-static-render");

    }
}
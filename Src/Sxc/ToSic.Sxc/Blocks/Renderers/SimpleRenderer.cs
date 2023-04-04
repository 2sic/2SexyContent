﻿using System;
using System.Collections.Generic;
using System.Text;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Blocks.Renderers
{
    [PrivateApi]
    public class SimpleRenderer: ServiceBase
    {
        private readonly Generator<BlockFromEntity> _blkFrmEntGen;
        private static string EmptyMessage = "<!-- auto-render of item {0} -->";

        public SimpleRenderer(Generator<BlockFromEntity> blkFrmEntGen): base(Constants.SxcLogName + "RndSmp")
        {
            ConnectServices(
                _blkFrmEntGen = blkFrmEntGen
            );
        }

        public string Render(IBlock parentBlock, IEntity entity, object data = default)
        {
            var l = Log.Fn<string>();

            // if not the expected content-type, just output a hidden html placeholder
            if (entity.Type.Name != AppConstants.ContentGroupRefTypeName)
            {
                l.A("empty, will return hidden html placeholder");
                return string.Format(EmptyMessage, entity.EntityId);
            }

            // render it
            l.A("found, will render");
            var cb = _blkFrmEntGen.New().Init(parentBlock, entity);
            var result = cb.BlockBuilder.Run(false, data);

            // Special: during Run() various things are picked up like header changes, activations etc.
            // Depending on the code flow, it could have picked up changes of other templates (not this one)
            // because these were scoped, 
            // must attach additional info to the parent block, so it doesn't loose header changes and similar

            // Return resulting string
            return l.Return(result.Html);
        }

        private const string WrapperTemplate = "<div class='{0}' {1}>{2}</div>";
        private const string WrapperMultiItems = "sc-content-block-list"; // tells quickE that it's an editable area
        private const string WrapperSingleItem = WrapperMultiItems + " show-placeholder single-item"; // enables a placeholder when empty, and limits one entry

        public string RenderWithEditContext(DynamicEntity parent, IDynamicEntity subItem, string cbFieldName, Guid? newGuid, IEditService edit, object data = default)
        {
            var l = Log.Fn<string>();
            var attribs = edit.ContextAttributes(parent, field: cbFieldName, newGuid: newGuid);
            var inner = subItem == null ? "": Render(parent._Services.BlockOrNull, subItem.Entity, data: data);
            var cbClasses = edit.Enabled ? WrapperSingleItem : "";
            return l.Return(string.Format(WrapperTemplate, new object[] { cbClasses, attribs, inner}));
        }

        public string RenderListWithContext(DynamicEntity parent, string fieldName, string apps, int max, IEditService edit)
        {
            var l = Log.Fn<string>();
            var innerBuilder = new StringBuilder();
            var found = parent.TryGetMember(fieldName, out var objFound);
            if (found && objFound is IList<DynamicEntity> items)
                foreach (var cb in items)
                    innerBuilder.Append(Render(cb._Services.BlockOrNull, cb.Entity));

            var result = string.Format(WrapperTemplate, new object[]
            {
                edit.Enabled ? WrapperMultiItems : "",
                edit.ContextAttributes(parent, field: fieldName, apps: apps, max: max),
                innerBuilder
            });
            return l.Return(result);
        }
    }
}
﻿using System;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Services
{
    public class TurnOnService: HasLog, ITurnOnService
    {
        private const string TagName = "turnOn";
        private const string AttributeName = "turn-on";

        public TurnOnService(Lazy<IHtmlTagService> htmlTagService) : base(Constants.SxcLogName + ".TrnOnS")
        {
            _htmlTagService = htmlTagService;
        }

        private readonly Lazy<IHtmlTagService> _htmlTagService;

        // TODO:
        // - TEST
        // - CREATE LOG INTERCEPT - probably only exist on the FN method right now?

        public Attribute Attribute(
            object runOrSpecs,
            string noParamOrder = Eav.Parameters.Protector,
            object require = default,
            object data = default
        )
        {
            var l = Log.Fn<Attribute>();
            var specs = PickOrBuildSpecs(runOrSpecs, require, data);
            var attr = _htmlTagService.Value.Attr(AttributeName, specs);
            return l.Return(attr);
        }

        public IHtmlTag Run(
            object runOrSpecs,
            string noParamOrder = Eav.Parameters.Protector,
            object require = default,
            object data = default
        )
        {
            var l = Log.Fn<IHtmlTag>();
            var specs = PickOrBuildSpecs(runOrSpecs, require, data);
            var tag = _htmlTagService.Value.Custom(TagName).Attr(AttributeName, specs);
            return l.Return(tag);
        }

        private static object PickOrBuildSpecs(object runOrSpecs, object require, object data) =>
            runOrSpecs is string run
                ? new
                {
                    run,
                    require,
                    data
                }
                : runOrSpecs;
    }
}
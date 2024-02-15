﻿namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyString: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.String;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;
        return
        [
            GenPropSnip(tabs: tabs, returnType: "string", name: name, method: "String",
                parameters: "fallback: \"\"",
                summary:
                [
                    $"{name} as string. For advanced manipulation like scrubHtml, use .String(\"{name}\", ...)"
                ]),
        ];
    }
}
﻿namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyEntity: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Entity;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "IEnumerable<ITypedItem>", name, "Children", summary:
            [
                $"{name} as list of ITypedItem.",
            ]),
        ];
    }
}
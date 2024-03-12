﻿namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyDateTime(CodeGenHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.DateTime;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "DateTime", name, $"{Specs.ItemAccessor}.DateTime", usings: UsingDateTime, summary:
            [
                $"{name} as DateTime.",
            ]),
        ];
    }

    private List<string> UsingDateTime { get; } = ["System"];
}
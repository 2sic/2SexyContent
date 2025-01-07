using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public record ModuleServiceData
{
    public List<IHtmlTag> MoreTags { get; init; } = [];
    public HashSet<string> ExistingKeys { get; init; } = [];
}

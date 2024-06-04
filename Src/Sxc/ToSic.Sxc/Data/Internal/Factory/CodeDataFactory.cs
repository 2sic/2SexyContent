﻿using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Integration;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Data.Internal;

// todo: make internal once we have an interface
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class CodeDataFactory(
    LazySvc<CodeDataServices> codeDataServices,
    LazySvc<AdamManager> adamManager,
    LazySvc<IContextOfApp> contextOfAppLazy,
    LazySvc<DataBuilder> dataBuilderLazy,
    LazySvc<CodeDataWrapper> codeDataWrapper,
    Generator<CodeJsonWrapper> wrapJsonGenerator,
    LazySvc<CodeInfoService> codeInfoSvc,
    LazySvc<IZoneMapper> zoneMapper)
    : ServiceForDynamicCode("Sxc.AsConv",
        connect: [codeDataServices, adamManager, contextOfAppLazy, dataBuilderLazy, codeDataWrapper, wrapJsonGenerator, codeInfoSvc, zoneMapper])
{
    internal CodeInfoService CodeInfo => codeInfoSvc.Value;

    public void SetCompatibilityLevel(int compatibilityLevel) => _priorityCompatibilityLevel = compatibilityLevel;

    public void SetFallbacks(ISite site, int? compatibility = default, AdamManager adamManagerPrepared = default)
    {
        _siteOrNull = site;
        _compatibilityLevel = compatibility ?? _compatibilityLevel;
        _adamManager.Reset(adamManagerPrepared);
    }
    private ISite _siteOrNull;

    private ISite SiteFromContextOrFallback => _siteFromContextOrFallback 
        ??= (_CodeApiSvc?.CmsContext as CmsContext)?.CtxSite.Site
            ?? _siteOrNull
            ?? throw new("Tried getting site from context or fallback, neither returned anything useful. ");

    private ISite _siteFromContextOrFallback;
    public int CompatibilityLevel => _priorityCompatibilityLevel ?? _compatibilityLevel;
    private int? _priorityCompatibilityLevel;
    private int _compatibilityLevel = CompatibilityLevels.CompatibilityLevel10;


    #region CodeDataServices

    public CodeDataServices Services => _services.Get(() => 
    {
        var cds = codeDataServices.Value;
        // if the render service is ever needed, it should be connected to the root
        cds.RenderServiceGenerator.SetInit(nowRs => (nowRs as INeedsCodeApiService)?.ConnectToRoot(_CodeApiSvc));
        return cds;
    });
    private readonly GetOnce<CodeDataServices> _services = new();

    // If we don't have a DynCodeRoot, try to generate the language codes and compatibility
    // There are cases where these were supplied using SetFallbacks, but in some cases none of this is known
    internal string[] Dimensions => _dimensions.Get(() => SiteFromContextOrFallback.SafeLanguagePriorityCodes()
        //_CodeApiSvc?.CmsContext.SafeLanguagePriorityCodes()
        //?? _siteOrNull.SafeLanguagePriorityCodes()
    );
    private readonly GetOnce<string[]> _dimensions = new();

    internal IBlock BlockOrNull => ((ICodeApiServiceInternal)_CodeApiSvc)?._Block;

    #endregion

    public object Json2Jacket(string json, string fallback = default)
        => wrapJsonGenerator.New().Setup(WrapperSettings.Dyn(true, true))
            .Json2Jacket(json, fallback: fallback);

    /// <summary>
    /// WIP, we need this in the GetAndConvertHelper, and want to make sure it's not executed on every entity used,
    /// so for now we're doing this once only here.
    /// </summary>
    public List<string> SiteCultures => _siteCultures ??= zoneMapper.Value
                                                              .CulturesEnabledWithState(SiteFromContextOrFallback)
                                                              ?.Select(c => c.Code)
                                                              .ToList()
                                                          ?? [];
    private List<string> _siteCultures;
}
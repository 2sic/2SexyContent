﻿using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

/// <summary>
/// Note: normally dependencies are Constructor injected.
/// This doesn't work in DNN.
/// But for consistency, we're building a comparable structure here.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ApiControllerMyServices : MyServicesBase
{
    public LazySvc<AppConfigDelegate> AppConfigDelegateLazy { get; }
    public LazySvc<Apps.App> AppOverrideLazy { get; }
    public CodeRootFactory CodeRootFactory { get; }
    public DnnAppFolderUtilities AppFolderUtilities { get; }

    public ApiControllerMyServices(
        CodeRootFactory codeRootFactory,
        DnnAppFolderUtilities appFolderUtilities,
        LazySvc<Apps.App> appOverrideLazy,
        LazySvc<AppConfigDelegate> appConfigDelegateLazy)
    {
        ConnectServices(
            CodeRootFactory = codeRootFactory,
            AppFolderUtilities = appFolderUtilities,
            AppOverrideLazy = appOverrideLazy,
            AppConfigDelegateLazy = appConfigDelegateLazy
        );
    }
}
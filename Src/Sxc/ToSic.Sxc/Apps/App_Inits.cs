﻿using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Apps;

public partial class App
{
    [PrivateApi]
    public App PreInit(ISite site)
    {
        var l = Log.Fn<App>();
        Site = site;
        return l.Return(this);
    }

    /// <summary>
    /// Main constructor which auto-configures the app-data
    /// </summary>
    [PrivateApi]
    public new App Init(IAppIdentityPure appIdentity, AppDataConfigSpecs dataSpecs)
    {
        var l = Log.Fn<App>();
        base.Init(appIdentity, dataSpecs);
        return dataSpecs == null 
            ? l.Return(this, "App only initialized for light use - .Data shouldn't be used") 
            : l.ReturnAsOk(this);
    }
}
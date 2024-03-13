﻿using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFolder(ISxcContextResolver ctxResolver) : ServiceBase("AppFolder")
{
    /// <summary>
    /// This is necessary for special calls where the _ctxResolve may not yet be complete...
    /// Important: not sure if this is actually needed, I believe the ctxResolver is always initialized on all web-api requests...?
    /// </summary>
    /// <param name="getBlock"></param>
    /// <returns></returns>
    public AppFolder Init(IBlock getBlock)
    {
        ctxResolver.AttachBlock(getBlock);
        return this;
    }

    public string GetAppFolder()
    {
        var ctx = ctxResolver.AppNameRouteBlock("");
        return ctx.AppState.Folder;
    }
}
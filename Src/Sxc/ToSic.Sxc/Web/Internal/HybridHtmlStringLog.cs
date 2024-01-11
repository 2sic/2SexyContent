﻿using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.Internal;

/// <summary>
/// IMPORTANT: Changed to internal for v16.08. #InternalMaybeSideEffectDynamicRazor
/// This is how it should be done, but it could have a side-effect in dynamic razor in edge cases where interface-type is "forgotton" by Razor.
/// Keep unless we run into trouble.
/// Remove this comment 2024 end of Q1 if all works, otherwise re-document why it must be public
///
/// 2023-11-14 - seems to cause trouble, going public for now
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class HybridHtmlStringLog: RawHtmlString, IHasLog
{
    protected HybridHtmlStringLog(string logName)
    {
        Log = new Log(logName);
    }
    protected HybridHtmlStringLog(ILog parentLog, string logName)
    {
        Log = new Log(logName, parentLog);
    }


    public ILog Log { get; }

}
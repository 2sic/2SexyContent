﻿using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Engines;

/// <summary>
/// The sub-system in charge of taking
/// - a configuration for an instance (aka Module)
/// - a template
/// and using all that to produce an html-string for the browser. 
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IEngine: IHasLog
{
    void Init(IBlock block);

    /// <summary>
    /// Renders a template, returning a string with the rendered template.
    /// </summary>
    /// <returns>The string - usually HTML - which the engine created. </returns>
    RenderEngineResult Render(object data);
}
﻿using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Threading.Tasks;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Razor
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IRazorRenderer
    {
        Task<string> RenderToStringAsync<TModel>(string templatePath, TModel model, Action<RazorView> configure = null, IApp app = null);
    }
}
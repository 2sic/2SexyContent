﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Client;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App
{
    public class ModuleProBase: ModuleBase
    {
        #region Injected Services

        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }

        #endregion

        #region Shared Variables

        //public static bool Debug;

        public bool Debug // persist state across circuits (blazor server only)
        {
            get => (HttpContextAccessor?.HttpContext?.Items[DebugKey] as bool?) ?? false;
            set
            {
                if (HttpContextAccessor?.HttpContext != null)
                    HttpContextAccessor.HttpContext.Items[DebugKey] = value;
            }
        }
        private const string DebugKey = "Debug";

        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(PageState.User, RoleNames.Host);
        private bool? _isSuperUser;

        public SxcInterop SxcInterop;
        public bool IsSafeToRunJs;
        public readonly ConcurrentQueue<object[]> LogMessageQueue = new();

        #endregion

        //protected override async Task OnInitializedAsync()
        //{
        //    await base.OnInitializedAsync();
        //}
        public bool IsPreRendering() => PageState.Site.RenderMode == "ServerPrerendered"; // The render mode for the site.

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (NavigationManager.TryGetQueryString<bool>("debug", out var debugInQueryString))
                Debug = debugInQueryString;
            
            Log($"2sxc Blazor Logging Enabled");  // will only show if it's enabled
        }
        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                SxcInterop = new SxcInterop(JSRuntime);
                // now we are safe to have SxcInterop and run js
                IsSafeToRunJs = true;
            } 
        }

        #region Log Helpers

        /// <summary>
        /// console.log
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void Log(params object[] message)
        {
            // If the url has a debug=true and we are the super-user
            if (message == null || !message.Any() || !Debug || !IsSuperUser) return;

            _logPrefix ??= $"2sxc:Page({PageState?.Page?.PageId}):Mod({ModuleState?.ModuleId}):";
            try
            {
                // log on web server
                foreach (var item in message)
                    Console.WriteLine($"{_logPrefix} {item}");

                // log to browser console
                if (IsSafeToRunJs)
                {
                    // first log messages from queue
                    var timeOut = 0;
                    while (!LogMessageQueue.IsEmpty && timeOut < 100)
                    {
                        if (LogMessageQueue.TryDequeue(out var messageToLog))
                        {
                            ConsoleLog(new List<object> { $"dequeue({LogMessageQueue.Count}):" }.Concat(messageToLog).ToArray());
                            timeOut = 0;
                        }
                        else
                            timeOut++;
                    };
                    
                    // than log current message
                    ConsoleLog(message);
                }
                else
                {
                    // browser is not ready, so store messages in queue
                    LogMessageQueue.Enqueue(message.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{_logPrefix}:{ex.Message}");
                if (IsSafeToRunJs)
                    JSRuntime.InvokeVoidAsync(ConsoleLogJs, "Error:", _logPrefix, ex.Message);
                else
                    LogMessageQueue.Enqueue(new List<object> { "Error:", _logPrefix, ex.Message }.ToArray());
            }
        }

        private void ConsoleLog(object[] message)
        {
            var data = new List<object> { _logPrefix }.Concat(message);
            JSRuntime.InvokeVoidAsync(ConsoleLogJs, data.ToArray());
        }
        private string _logPrefix;
        private const string ConsoleLogJs = "console.log";

        #endregion
    }
}
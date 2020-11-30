﻿using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared.Dev;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatelessControllerBase : Controller, IHasLog
    {
        protected OqtStatelessControllerBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            // todo: redesign so it works - in .net core the HttpContext isn't ready in the constructor
            Log = new Log(HistoryLogName, null, $"Path: {HttpContext?.Request.GetDisplayUrl()}");
            //TimerWrapLog = Log.Call(message: "timer", useTimer: true);
            // ReSharper disable once VirtualMemberCallInConstructor
            History.Add(HistoryLogGroup, Log);
            // register for dispose / stopping the timer at the end
            _logWrapper = new LogWrapper(Log);
            // todo: get this to work
            // ControllerContext.HttpContext.Response.RegisterForDispose(_logWrapper);
        }

        /// <inheritdoc />
        public ILog Log { get; }

        private readonly LogWrapper _logWrapper;

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case.
        /// </summary>
        protected virtual string HistoryLogGroup { get; } = "web-api";

        /// <summary>
        /// The name of the logger in insights.
        /// The inheriting class should provide the real name to be used.
        /// </summary>
        protected abstract string HistoryLogName { get; }

        //protected void Dispose(bool disposing)
        //{
        //    TimerWrapLog(null);
        //    base.Dispose(disposing);
        //}

        #region Extend Time so Web Server doesn't time out

        protected void PreventServerTimeout300() => WipConstants.DontDoAnythingImplementLater();

        #endregion
    }

    internal class LogWrapper : IDisposable
    {
        private readonly Action<string> _timerWrapLog;

        internal LogWrapper(ILog log) => _timerWrapLog = log.Call(message: "timer", useTimer: true);
        public void Dispose() => _timerWrapLog(null);
    }
}

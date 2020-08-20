﻿using ToSic.Eav.Apps;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public partial class InsightsController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.Debug";

        /// <summary>
        /// Enable/disable logging of access to insights
        /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
        /// </summary>
        internal const bool InsightsLoggingEnabled = false;

        internal const string InsightsUrlFragment = "/sys/insights/";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.insights";


        //private AppRuntime AppRt(int? appId) => new AppRuntime(appId.Value, true, Log);

        //private AppState AppState(int? appId) => State.Get(appId.Value);

        protected ToSic.Sxc.Web.WebApi.System.Ins Insights =>
            _insights ?? (_insights = new ToSic.Sxc.Web.WebApi.System.Ins(Log, ThrowIfNotSuperuser, (string msg) => Http.BadRequest(msg)));
        private ToSic.Sxc.Web.WebApi.System.Ins _insights;
    }
}
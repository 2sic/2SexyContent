﻿using Newtonsoft.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarContext: IAppIdentity
    {
        internal const string CtxZone = "context:zoneId";
        internal const string CtxApp = "context:appId";
        internal const int NotInitialized = -7007;

        public ToolbarContext(IAppIdentity parent): this(parent.ZoneId, parent.AppId) { }

        public ToolbarContext(int zoneId, int appId)
        {
            ZoneId = zoneId;
            AppId = appId;
        }

        public ToolbarContext(string custom) => Custom = custom;


        [JsonProperty("zoneId")] public int ZoneId { get; } = NotInitialized;
        [JsonProperty("appId")] public int AppId { get; } = NotInitialized;

        [JsonProperty("custom", NullValueHandling = NullValueHandling.Ignore)] public string Custom { get; } = null;
    }

    public static class ToolbarContextExtensions
    {
        public static string ToRuleString(this ToolbarContext tlbCtx) => tlbCtx == null 
            ? null 
            : tlbCtx.Custom.HasValue() 
                ? tlbCtx.Custom 
                : UrlParts.ConnectParameters($"{ToolbarContext.CtxZone}={tlbCtx.ZoneId}", $"{ToolbarContext.CtxApp}={tlbCtx.AppId}");
    }
}
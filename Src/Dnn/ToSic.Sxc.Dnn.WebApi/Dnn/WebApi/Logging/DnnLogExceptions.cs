﻿using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using ToSic.Eav.Logging;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.WebApi.Logging
{
    public class DnnLogExceptions : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);

            // try to access log created so far
            try
            {
                if (context.Request?.Properties.ContainsKey(DnnConstants.EavLogKey) ?? false)
                {
                    // must to ContainsKey checks, otherwise we get too many errors which is a problem while debugging
                    var log = context.Request.Properties.ContainsKey(DnnConstants.EavLogKey)
                        ? context.Request.Properties[DnnConstants.EavLogKey] as ILog
                        : null;
                    var dnnContext = context.Request.Properties.ContainsKey(DnnConstants.DnnContextKey) 
                        ? context.Request.Properties[DnnConstants.DnnContextKey] as DnnContextOld
                        : null;
                    DnnLogging.LogToDnn("2sxc-Api", "Auto-Log Exception", log, dnnContext, force: true);
                }
                else
                    DnnLogging.LogToDnn("2sxc-Api",
                        "exception, but no additional internal log to add, EavLog doesn't exist", force: true);
            }
            catch
            {
                DnnLogging.TryToReportLoggingFailure("SxcWebApiExceptionHandling");
            }

            // special manual exception maker, because otherwise IIS at runtime removes all messages
            // without this, debug-infos like what entity is causing the problem will not be shown to the client
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                                               "Bad Request",
                                                               context.Exception);
            var httpError = (HttpError)((ObjectContent<HttpError>)context.Response.Content).Value;
            if (!httpError.ContainsKey("ExceptionType"))
                httpError.Add("ExceptionType", exception.GetType().FullName);
            if (!httpError.ContainsKey("ExceptionMessage"))
                httpError.Add("ExceptionMessage", exception.Message);
        }


    }
}
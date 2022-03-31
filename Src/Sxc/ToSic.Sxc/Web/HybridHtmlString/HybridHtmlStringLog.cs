﻿using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Web
{
    public abstract class HybridHtmlStringLog: HybridHtmlString, IHasLog
    {
        protected HybridHtmlStringLog(string logName) : base()
        {
            Log = new Log(logName);
        }


        public ILog Log { get; }

    }
}
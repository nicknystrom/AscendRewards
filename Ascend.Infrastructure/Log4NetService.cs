using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;
using Ascend.Core.Services;
using Ascend.Core;

namespace Ascend.Infrastructure
{
    public class Log4NetService : Ascend.Core.Services.ILog
    {
        public Tenant Tenant { get; set; }

        protected log4net.ILog _log;

        protected log4net.ILog Log
        {
            get { return _log ?? (_log = LogManager.GetLogger(null == Tenant ? "Global" : Tenant.Document.Id)); }
        }

        static Log4NetService()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Debug(object message) { Log.Debug(message); }
        public void Debug(object message, Exception exception) { Log.Debug(message, exception); }
        public void DebugFormat(string format, params object[] args) { Log.DebugFormat(format, args); }

        public void Error(object message) { Log.Error(message); }
        public void Error(object message, Exception exception) { Log.Error(message, exception); }
        public void ErrorFormat(string format, params object[] args) { Log.ErrorFormat(format, args); }

        public void Fatal(object message) { Log.Fatal(message); }
        public void Fatal(object message, Exception exception) { Log.Fatal(message, exception); }
        public void FatalFormat(string format, params object[] args) { Log.FatalFormat(format, args); }

        public void Info(object message) { Log.Info(message); }
        public void Info(object message, Exception exception) { Log.Info(message, exception); }
        public void InfoFormat(string format, params object[] args) { Log.InfoFormat(format, args); }

        public void Warn(object message) { Log.Warn(message); }
        public void Warn(object message, Exception exception) { Log.Warn(message, exception); }
        public void WarnFormat(string format, params object[] args) { Log.WarnFormat(format, args); }

    }
}
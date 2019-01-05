using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Logging.Base;

namespace Logging.Framework
{
    public abstract class LoggingProviderBase:ILoggingProviderBase
    {

        public logMessageTypes CurrentlogMessageType { get; set; }
        
        public static string CreateMessageLogEntry(Exception ex)
        {
            string msg;
            Exception inex = ex;
            msg = ex.Message;
            string OrignMethod="";
            StackTrace stackTrace = new StackTrace(ex);
            if (stackTrace.GetFrame(0) != null)
            {
                OrignMethod = "OrignMethod = " + stackTrace.GetFrame(0).GetMethod().Name;
                OrignMethod += "( ";
                foreach (var item in stackTrace.GetFrame(0).GetMethod().GetParameters())
                {
                    OrignMethod += item.ToString() + ", ";
                }
                OrignMethod = OrignMethod.Substring(0, OrignMethod.Length - 2);
                OrignMethod += ")";
            }
            while (inex.InnerException != null)
            {
                if (inex == null)
                    break;
                inex = ex.InnerException;
                msg = msg + "--Inner Exception--" + inex.Message;
            };
            msg = msg + "\n\r" + " - " + OrignMethod + "\n\r" + ".Stack Trace: -" + ex.StackTrace;
            return msg;
        }

        #region ILoggingProviderBase Members

        public abstract bool AddLogEntry(string logentry);
      
        public abstract bool AddExceptionEntry(Exception ex);
        
        public abstract bool AddExceptionEntry(Exception ex, string header);

        #endregion
    }
}

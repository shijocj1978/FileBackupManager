using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging.Base;

namespace Logging.Framework
{
    public class StaticLogBuilder : StaticLogger
    {
        public static string GetLogEntry(string logentry)
        {
            return new LogEntry(logentry).ErrorMessage;
        }

        public static string GetExceptionEntry(Exception ex)
        {
            string msg = CreateMessageLogEntry(ex);
            return new LogEntry(msg).ErrorMessage;
        }

        public static string GetExceptionEntry(Exception ex, string header)
        {
            string msg = CreateMessageLogEntry(ex);
            return new LogEntry(header + " - " + msg).ErrorMessage;
        }

        private new bool AddLogEntry(string logentry)
        {
            return true;
        }

        private new bool AddExceptionEntry(Exception ex)
        {
            return true;
        }

        private new bool AddExceptionEntry(Exception ex, string header)
        {
            return true;
        }
    }
}

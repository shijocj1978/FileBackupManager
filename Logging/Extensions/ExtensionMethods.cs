using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging.Base;
using Logging.Framework;
using Logging.OSInterfaces;
using System.Diagnostics;

namespace Logging
{

    public static class LoggingManagerExtensions
    {
        public static bool AddErrorLogEntry(this LoggingManager str, string logentry)
        {
            return Logger.StaticLogger.AddLogEntry(logentry,logMessageTypes.Errors);
        }

        public static bool AddErrorLogEntry(this LoggingManager str, Exception ex, string logentry)
        {
            return Logger.StaticLogger.AddExceptionEntry(ex, logentry,logMessageTypes.Errors);
        }

        public static bool AddErrorLogEntry(this LoggingManager str, Exception ex)
        {
            return Logger.StaticLogger.AddExceptionEntry(ex,logMessageTypes.Errors);
        }

        public static bool AddWarningEntry(this LoggingManager str, string logentry)
        {
            return Logger.StaticLogger.AddLogEntry(logentry,logMessageTypes.Warnings);
        }

        public static bool AddAuditLogEntry(this LoggingManager str, string logentry)
        {
            return Logger.StaticLogger.AddLogEntry(logentry,logMessageTypes.AuditLogs);
        }

        public static bool AddDebugMessagesEntry(this LoggingManager str, string logentry)
        {
            Debug.WriteLine(logentry);
            if (AppConfigSettings.IsDebugLoggingEnabled == false)
                return false;
            return Logger.StaticLogger.AddLogEntry(logentry,logMessageTypes.DebugMessages);
        }
    }
}
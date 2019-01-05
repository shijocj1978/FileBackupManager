using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Logging.Framework;

namespace Logging.OSInterfaces
{
    public class OSApplicationLogger
    {
        
        private static string source = "3TierClient";

        /// <summary>
        /// Writes to System Log in Application errors context.
        /// </summary>
        /// <param name="msg">Message as string</param>
        /// <param name="msgtype">Type of Message</param>
        /// <returns></returns>
        public static bool WriteToOSApplicationLog(string msg, EventLogEntryType msgtype)
        {
            try//not sure why but this windows function
            {
                if (EventLog.Exists(source) == false)
                    EventLog.CreateEventSource(source, "Application");
            }
            catch
            {
            }
            EventLog.WriteEntry(source, msg, EventLogEntryType.Error);
            return true;
        }

        /// <summary>
        /// Writes to System Log in Application errors context.
        /// </summary>
        /// <param name="msg">Message as string</param>
        /// <param name="msgtype">Type of Message</param>
        /// <returns></returns>
        public static bool WriteToOSApplicationLog(Exception ex, EventLogEntryType msgtype)
        {
            string str = LoggingProviderBase.CreateMessageLogEntry(ex);
            WriteToOSApplicationLog(str, msgtype);
            return true;
        }
    }
}

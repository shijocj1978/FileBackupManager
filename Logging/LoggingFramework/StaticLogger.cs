using System;
using System.Collections.Generic;
using System.Text;
using Logging.Base;
using System.Diagnostics;

namespace Logging.Framework
{
    public class StaticLogger : LoggingProviderBase, IDisposable
    {
        ExtendedList<LogEntry> Logs;

        public StaticLogger()
            : base()
        {
            Logs = new ExtendedList<LogEntry>(0);
        }

        public override bool AddLogEntry(string logentry)
        {
            Logs.Add(new LogEntry(logentry));
            return true;
        }

        public override bool AddExceptionEntry(Exception ex)
        {
            string msg = CreateMessageLogEntry(ex);
            Logs.Add(new LogEntry(msg));
            return true;
        }

        public override bool AddExceptionEntry(Exception ex, string header)
        {
            string msg = CreateMessageLogEntry(ex);
            Logs.Add(new LogEntry(header + " - " + msg));
            return true;
        }

        public bool SaveStaticLogs(LoggingProviderBase logger)
        {
            try
            {
                foreach (LogEntry var in Logs)
                {
                    try
                    {
                        logger.AddLogEntry(var.ErrorMessage);
                    }
                    catch
                    {
                        continue;
                    }
                }
                Logs.Clear();
                return true;
            }
            catch
            { 
                return false;
            }
        }

        public bool DumpCriticalLogs()
        {
            if (Logs.Count == 0)
                return true;
            //LoggingProviderBase logger = new FileSystemLogger(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location) + "\\" +  "-Critical.log");
            LoggingProviderBase logger = new FileSystemLogger(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + Process.GetCurrentProcess().ProcessName + "-Critical.log");
            foreach (LogEntry var in Logs)
            {
                logger.AddLogEntry(var.ErrorMessage);
            }
            Logs.Clear();
            return true;
            //to do: save all the data in object collection to a file if it is not saved to dd yet. 
            //means if the app exists un expectdly this will contain a list of exceptions.
        }

        #region IDisposable Members

        public void Dispose()
        {
            DumpCriticalLogs();
        }

        ~StaticLogger()
        {
            DumpCriticalLogs();
        }
        #endregion
    }
}

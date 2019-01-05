using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Logging.Base;
using Logging.Framework;

namespace Logging
{

    public class LoggingManager
    {
        private LoggingModes _CurrentMode;

        public LoggingModes CurrentMode
        {
            get { return _CurrentMode; }
            private set { _CurrentMode = value; }
        }

        static internal System.Collections.Generic.Dictionary<string, FileSystemLogger> FileLogger {get; set;}

        static internal System.Collections.Generic.Dictionary<string, StaticLogger> StaticLogger { get; set; }

        public IndexedPropertyBase LogFilePath { get; private set; }

        /// <summary>
        /// Creates a static instance of logging manager with runnumber as null
        /// </summary>
        public LoggingManager():base()
        {
            CurrentMode = LoggingModes.Static;
            StaticLogger = new Dictionary<string, StaticLogger>();
            foreach (var val in Enum.GetValues(typeof(logMessageTypes)))
            {
                StaticLogger st = new StaticLogger();
                st.CurrentlogMessageType = (logMessageTypes)val;
                StaticLogger.Add(val.ToString(), st);
            }
        }
        
        /*
         * The mode should decide the parameters and arguments to the logging module. 
         * it cannot pass specific parameters to each mode. 
         * So need to get everything from global at this point.
         */
        public LoggingManager(LoggingModes mode,string RootPath): base()
        {
            try
            {
                CurrentMode = mode;
                switch (mode)
                {
                    case LoggingModes.FileSystem:
                        FileLogger = new Dictionary<string, FileSystemLogger>();
                        foreach (var val in Enum.GetValues(typeof(logMessageTypes)))
                        {
                            FileSystemLogger tmp = new FileSystemLogger(RootPath + "\\" + val + ".log");
                            SaveStaticLogs(tmp);
                            tmp.CurrentlogMessageType = (logMessageTypes)val;
                            FileLogger.Add(val.ToString(),tmp);
                        }
                        LogFilePath = new IndexedPropertyBase(FileLogger);
                        break;
                    case LoggingModes.Static:
                        StaticLogger = new Dictionary<string,StaticLogger>(); // new StaticLogger();
                        foreach (var val in Enum.GetValues(typeof(logMessageTypes)))
                        {
                            StaticLogger st = new StaticLogger();
                            st.CurrentlogMessageType = (logMessageTypes)val;
                            StaticLogger.Add(val.ToString(), st);
                        }
                        break;
                    case LoggingModes.Database & LoggingModes.FileSystem:
                        FileLogger = new Dictionary<string, FileSystemLogger>();
                        foreach (var val in Enum.GetValues(typeof(logMessageTypes)))
                        {
                            FileSystemLogger tmp = new FileSystemLogger(RootPath + "\\" + val + ".log");
                            SaveStaticLogs(tmp);
                            tmp.CurrentlogMessageType = (logMessageTypes)val;
                            FileLogger.Add(val.ToString(), tmp);
                        }
                        LogFilePath = new IndexedPropertyBase(FileLogger);
                        break;
                    default:
                        break;
                }
            }
            catch 
            {
                
            }
        }

        internal bool AddLogEntry(string logentry, logMessageTypes MessageType)
        {
            switch (CurrentMode)
            {
                case LoggingModes.FileSystem:
                    FileLogger[MessageType.ToString()].AddLogEntry(logentry);
                    break;
                case LoggingModes.Static:
                    StaticLogger[MessageType.ToString()].AddLogEntry(logentry);
                    break;
                case LoggingModes.Database & LoggingModes.FileSystem:
                    FileLogger[MessageType.ToString()].AddLogEntry(logentry);
                    break;
                default:
                    break;
            }
            return true;
        }

        internal bool AddExceptionEntry(Exception ex, logMessageTypes MessageType)
        {
            switch (CurrentMode)
            {
                case LoggingModes.FileSystem:
                    FileLogger[MessageType.ToString()].AddExceptionEntry(ex);
                    break;
                case LoggingModes.Static:
                    StaticLogger[MessageType.ToString()].AddExceptionEntry(ex);
                    break;
                case LoggingModes.Database & LoggingModes.FileSystem:
                    FileLogger[MessageType.ToString()].AddExceptionEntry(ex);
                    break;
                default:
                    break;
            }
            return true;
        }

        internal bool AddExceptionEntry(Exception ex, string header, logMessageTypes MessageType)
        {
            switch (CurrentMode)
            {
                case LoggingModes.FileSystem:
                    FileLogger[MessageType.ToString()].AddExceptionEntry(ex, header);
                    break;
                case LoggingModes.Static:
                    StaticLogger[MessageType.ToString()].AddExceptionEntry(ex, header);
                    break;
                case LoggingModes.Database & LoggingModes.FileSystem:
                    FileLogger[MessageType.ToString()].AddExceptionEntry(ex, header);
                    break;
                default:
                    break;
            }
            return true;
        }

        public bool DumpCriticalLogs()
        {
            bool status = true;
            if (StaticLogger == null)
                return false;
            foreach (var item in StaticLogger)
            {
                if (item.Value.DumpCriticalLogs() == false)
                    status = false;
            }
            return status;
        }

        public bool SaveStaticLogs(LoggingProviderBase logger)
        {
            if (StaticLogger == null)
                return false;
            return StaticLogger[logMessageTypes.Errors.ToString()].SaveStaticLogs(logger);
        }

        public bool ChangeLoggerFilePath(logMessageTypes FileType, string LogFilePath_FileName)
        {
            FileLogger[FileType.ToString()].LogFilePath = LogFilePath_FileName;
            return true;
        }

        public bool ChangeLoggerFileName(logMessageTypes FileType, string LogFileName)
        {
            string path = Path.GetDirectoryName(FileLogger[FileType.ToString()].LogFilePath);
            return ChangeLoggerFilePath(FileType, path + "\\" + LogFileName);
        }

        public string GetLogEntry(string logentry)
        {
            return StaticLogBuilder.GetLogEntry(logentry);
        }

        public string GetExceptionEntry(Exception ex)
        {
            return StaticLogBuilder.GetExceptionEntry(ex);
        }

        public string GetExceptionEntry(Exception ex, string header)
        {
            return StaticLogBuilder.GetExceptionEntry(ex,header);
        }
    }

    public enum LoggingModes
	{
        FileSystem = 1,
        Database = 2,
        Static =4
        //,
        //Email =8,
        //SysLog =16
        //add by factors of 2.
	};
}

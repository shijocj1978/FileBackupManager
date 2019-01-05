using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Logging.OSInterfaces;
using Logging.Base;

namespace Logging.Framework
{

    public class FileSystemLogger : LoggingProviderBase
    {
        public FileSystemLogger(string LogFilePath):base()
        {
            InitFile(LogFilePath);
        }

        private void InitFile(string LogFilePath)
        {
            try
            {
                //incase an access issue is there in the folder. need to reset the path to prevent system from crashing.
                if (File.Exists(LogFilePath) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                    File.Create(LogFilePath);
                }
                else
                {
                    File.Create(LogFilePath + ".tmp").Close();
                    File.Delete(LogFilePath + ".tmp");
                }
            }
            catch
            {
                _LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + Process.GetCurrentProcess().ProcessName + "\\" + Path.GetFileName(LogFilePath);
            }
            _LogFilePath = LogFilePath;
        }

        //[ThreadStatic]
        private string _LogFilePath;

        /// <summary>
        /// Gets or Sets(internal only) the log file path so that consumer can overloads without specifying the path.
        /// </summary>
        public string LogFilePath
        {
            get { return _LogFilePath; }
            internal set { InitFile(value); }
        }

        /// <summary>
        /// Writes an error message to application log in the system.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool WriteApplicationLog(Exception ex, string header, string path)
        {
            string msg = CreateMessageLogEntry(ex);
            return AddFileLogEntry(header + " - " + msg,path);
        }

        

        private  bool AddFileLogEntry(string msg)
        {
            return AddFileLogEntry(msg, _LogFilePath);
        }

        private bool AddFileLogEntry(string msg, string path)
        {
            FileManager.AppendText(path , "\n\n" + DateTime.Now.ToString() + ": "+ msg);
            return true;
        }

        #region LoggingProviderBase Members

        /// <summary>
        /// Writes an error message to application log in the system.
        /// </summary>
        /// <param name="logentry"></param>
        /// <returns></returns>
        public override bool AddLogEntry(string logentry)
        {
            return AddFileLogEntry(logentry);
        }

        /// <summary>
        /// Writes an error message to application log in the system.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool AddExceptionEntry(Exception ex)
        {
            string msg = CreateMessageLogEntry(ex);
            return AddFileLogEntry(msg);
        }

        /// <summary>
        /// Writes an error message to application log in the system.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public override bool AddExceptionEntry(Exception ex, string header)
        {
            string msg = CreateMessageLogEntry(ex);
            return AddFileLogEntry(header + " - " + msg);
        }

        #endregion
    }
}
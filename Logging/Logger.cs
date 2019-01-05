using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging.Framework;
using Logging.Base;
using System.IO;

namespace Logging
{

    public class Logger 
    {
        public static LoggingManager StaticLogger {get;set;}

        static Logger()
        {
            StaticLogger = new LoggingManager();
        }

        public  static bool EnableFileModeLogging(string LogDirectoryRoot)
        {
            if (Path.IsPathRooted(LogDirectoryRoot) == false)
                LogDirectoryRoot = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + LogDirectoryRoot + "\\";
            StaticLogger = new LoggingManager(LoggingModes.FileSystem, LogDirectoryRoot);
            return true;
        }
    }
}

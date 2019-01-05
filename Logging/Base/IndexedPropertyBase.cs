using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging.Framework;

namespace Logging.Base
{
    public class IndexedPropertyBase
    {
        System.Collections.Generic.Dictionary<string, FileSystemLogger> filelogger;

        public IndexedPropertyBase(System.Collections.Generic.Dictionary<string, FileSystemLogger> FileLogger)
        {
            filelogger = FileLogger;
        }

        public string this[logMessageTypes MessageType] 
        {
            get
            {
                return filelogger[MessageType.ToString()].LogFilePath;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Base
{
    public class LogEntry
    {

        public LogEntry(string errormsg)
        {
            _ErrorMessage = errormsg;
        }

        private string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

    }
}

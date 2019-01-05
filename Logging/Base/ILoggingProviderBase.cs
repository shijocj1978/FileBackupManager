using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logging.Base
{
    public interface ILoggingProviderBase
    {
        bool AddLogEntry(string logentry);
        bool AddExceptionEntry(Exception ex);
        bool AddExceptionEntry(Exception ex, string header);
    }
}

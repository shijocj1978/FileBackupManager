using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Logging;
using Logging.Base;

namespace FileManagerLibrary
{
    public class Global
    {
        public static bool IsWindowsUIInstance { get; set; }

        public Global()
        { 

        }

        public static bool DisableMessageBoxOnce { get; set; } 

        public static bool MessageBoxShow(string message)
        {
            if (Global.IsWindowsUIInstance == true)
            {
                if (DisableMessageBoxOnce == true)
                {
                    DisableMessageBoxOnce = false;
                    return true;
                }
                MessageBox.Show(message);
                return true;
            }
            else
            {
                Logger.StaticLogger.AddErrorLogEntry(message);
                return true;
            }
        }

        public static bool MessageBoxShow(string message, string caption = "Critical Error", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error, logMessageTypes MessageType = logMessageTypes.Errors)
        {
            if (Global.IsWindowsUIInstance == true)
            {
                MessageBox.Show(message,caption,buttons,icon);
                return true;
            }
            else
            {
                switch (MessageType)
                {
                    case logMessageTypes.Errors:
                        Logger.StaticLogger.AddErrorLogEntry(message);
                        break;
                    case logMessageTypes.Warnings:
                        Logger.StaticLogger.AddWarningEntry(message);
                        break;
                    case logMessageTypes.AuditLogs:
                        Logger.StaticLogger.AddAuditLogEntry(message);
                        break;
                    case logMessageTypes.DebugMessages:
                        Logger.StaticLogger.AddDebugMessagesEntry(message);
                        break;
                    default:
                        break;
                }
                return true;
            }
        }
    }
}

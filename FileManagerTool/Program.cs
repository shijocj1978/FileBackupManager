using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FileManagerTool.Properties;
using Logging;
using FileManagerLibrary.Base.FileBase;

namespace FileManagerTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.ControlAppDomain)]
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Logging.Logger.EnableFileModeLogging(FileOperations.GetAppSettingsFolderRoot() + "\\" + Settings.Default.ApplicationLogFolder);
            FileManagerLibrary.Global.IsWindowsUIInstance = true;
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.Run(new DashBoard());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Logger.StaticLogger.AddErrorLogEntry(ex, "Critical: Application UnhandledException Occured.");
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Logger.StaticLogger.AddErrorLogEntry(ex, "Critical: Application UnhandledException Occured.");
        }
    }
}

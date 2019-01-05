using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using FileManagerLibrary;
using FileManagerService.Properties;
using Logging;
using FileManagerLibrary.Base.FileBase;


namespace FileManagerService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            //this need to be done here as installer is using reflection not Main to trigger the installation.
            InitializeComponent();
            Logging.Logger.EnableFileModeLogging(FileOperations.GetAppSettingsFolderRoot() + "\\" + Settings.Default.ApplicationLogLocation);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            serviceInstaller1.ServiceName = AppSettings.WindowsServiceName;
            serviceInstaller1.DisplayName = AppSettings.WindowsServiceDisplayName;
            serviceInstaller1.Description = AppSettings.WindowsServiceDescription;
            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.StaticLogger.AddErrorLogEntry((Exception)e.ExceptionObject, "Error Occured during Installation of windows service");
            Logger.StaticLogger.AddErrorLogEntry("Servcie details" + AppSettings.WindowsServiceName + ", " + AppSettings.WindowsServiceDisplayName + ", " + AppSettings.WindowsServiceDescription + ".");
        }
    }
}

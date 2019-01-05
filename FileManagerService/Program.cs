using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using FileManagerService.Properties;
using FileManagerLibrary.Base.FileBase;

namespace FileManagerService
{
    static class Program
    {
        /*
         * 
         * 
         * DONOT USE. REPLACED THIS WITH FileManagerService.cs.MAIN() TO HAVE BETTER CAPABILITES FOR DEBUGGING.
         * 
         * 
         * 
         * 
         */
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new FileManagerService() 
			};
            Logging.Logger.EnableFileModeLogging(FileOperations.GetAppSettingsFolderRoot() + "\\" + Settings.Default.ApplicationLogLocation);
            ServiceBase.Run(ServicesToRun);
        }
    }
}

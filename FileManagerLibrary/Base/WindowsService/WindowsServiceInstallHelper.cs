using FileManagerBase;
using Logging;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace FileManagerLibrary.Base.WindowsService
{
    public class WindowsServiceInstallHelper
    {
        public static bool CheckAndInstallWindowsService()
        {
            try
            {
                if ((new WindowsServiceHelper()).GetServiceStatus() == WindowsServiceInstallStatus.NotInstalled)
                {
                    if (MessageBox.Show("It appears you do not have the windows service installed. Proceed with Install?", "Windows ServiceInstall", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        MessageBox.Show("Skipping installation for now. You need to restart the application to install the service.");
                        return false;
                    }
                    ManagedInstallerClass.InstallHelper(new string[] { Path.GetDirectoryName(Assembly.GetAssembly(typeof(WindowsServiceInstallHelper)).Location) + @"\FileManagerService.exe" });
                    if (new WindowsServiceHelper().GetServiceStatus() == WindowsServiceInstallStatus.Stopped)
                    {
                        MessageBox.Show("Windows service installed sucessfully. Unistall the service through menu before unistalling the application from the system.");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("An error Occured during installation. Please refer to logs for more information.");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error during installing Windows service from Local Machine");
                MessageBox.Show("An error Occured during installation. Please refer to logs for more information.");
                return false;
            }
        }

        public static bool UnInstallWindowsService()
        {
            try
            {
                if ((new WindowsServiceHelper()).GetServiceStatus() == WindowsServiceInstallStatus.NotInstalled)
                {
                    MessageBox.Show("It appears you do not have the windows service installed.");
                }
                else 
                {
                    if (MessageBox.Show("Proceed with Uninstall of the windows service?", "Windows Service Uninstall", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return true;
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Path.GetDirectoryName(Assembly.GetAssembly(typeof(WindowsServiceInstallHelper)).Location) + @"\\FileManagerService.exe" });
                    if ((new WindowsServiceHelper()).GetServiceStatus() == WindowsServiceInstallStatus.NotInstalled)
                        MessageBox.Show("Windows service uninstalled sucessfully. You can now safley unistall the application from system. To install service again restart the application.");
                    else
                        MessageBox.Show("An error occured during uninstallation. Please refer to logs for more information.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error during installing Windows Service from Local Machine");
                MessageBox.Show("An error Occured during installation. Please refer to logs for more information.");
                return false;
            }
        }
    }
}

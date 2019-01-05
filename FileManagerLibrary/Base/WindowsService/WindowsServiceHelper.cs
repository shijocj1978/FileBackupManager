using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using FileManagerLibrary;
using FileManagerLibrary.Base.WindowsService;
using Logging;
using FileManagerBase;

namespace FileManagerLibrary.Base.WindowsService
{
    /*
     * Purpose: This class is for Managing the windows service from UI.
     * An instance of this class is created in dashboard to start/stop/
     * get status of windows service and also filesystem watcher service.
     */
    public class WindowsServiceHelper
    {
        public WindowsServiceInstallStatus GetServiceStatus()
        {
            try
            {
                if (ServiceController.GetServices().Where(s => s.ServiceName == AppSettings.WindowsServiceName).FirstOrDefault() != null)
                {
                    ServiceController myService = new ServiceController(AppSettings.WindowsServiceName);
                    if (myService.Status == ServiceControllerStatus.Stopped)
                        return WindowsServiceInstallStatus.Stopped;
                    else if (myService.Status == ServiceControllerStatus.Running)
                        return WindowsServiceInstallStatus.Running;
                    else
                        return WindowsServiceInstallStatus.StatusChangeInProgress;
                }
                else
                    return WindowsServiceInstallStatus.NotInstalled;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error during retriving Windows Service from Local Machine");
                return WindowsServiceInstallStatus.ExceptionOccured;
            }
        }

        public ServiceControllerStatus StartService()
        {
            ServiceController myService = new ServiceController(AppSettings.WindowsServiceName);
            ServiceControllerStatus svcStatus = myService.Status;
            if (svcStatus == ServiceControllerStatus.Stopped)
            {
                try
                {
                    ServiceStartupTypeHelper.ChangeStartMode(myService, ServiceStartMode.Automatic);
                    myService.Start();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot open FileManagerService service on computer"))
                    {
                        Global.MessageBoxShow("You do not have enough rights to start the service. Login as a different user");
                        return svcStatus;
                    }
                    else if((ex.Message !=null && ex.InnerException !=null) && ex.Message.Contains("Cannot start service FileManagerService on computer") && ex.InnerException.Message.Contains("The service did not start due to a logon failure"))
                    {
                        Global.MessageBoxShow("Login to service failed. Check the Service username and password is valid.");
                        return svcStatus;
                    }
                }
                Thread.Sleep(1000);
                int i = 0;
                while (i < 100 || svcStatus == ServiceControllerStatus.StartPending || svcStatus == ServiceControllerStatus.Stopped)
                {
                    i++;
                    Thread.Sleep(100);
                    myService.Refresh();
                    svcStatus = myService.Status;
                }
            }
            return svcStatus;
        }

        public ServiceControllerStatus StopService()
        {
            ServiceController myService = new ServiceController(AppSettings.WindowsServiceName);
            ServiceControllerStatus svcStatus = myService.Status;
            if (svcStatus != ServiceControllerStatus.Stopped)
            {
                try
                {
                    myService.Stop();
                    ServiceStartupTypeHelper.ChangeStartMode(myService, ServiceStartMode.Disabled);
                }
                catch (Exception ex)
                {
                    if(ex.Message.Contains("Cannot open FileManagerService service on computer"))
                        if(FileManagerLibrary.Global.IsWindowsUIInstance == true)
                        {
                            MessageBox.Show("You do not have enough rights to stop the service. Login as a different user");
                            return svcStatus;
                        }
                }
                Thread.Sleep(1000);
                int i = 0;
                while (i < 100 || svcStatus == ServiceControllerStatus.StopPending || svcStatus == ServiceControllerStatus.Running)
                {
                    i++;
                    Thread.Sleep(100);
                    myService.Refresh();
                    svcStatus = myService.Status;
                }
            }
            return svcStatus;
        }
    }
}

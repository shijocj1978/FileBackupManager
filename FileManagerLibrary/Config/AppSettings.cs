using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Logging;
using System.Xml;
using FileManagerLibrary.Config;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace FileManagerLibrary
{
    public class AppSettings
    {
        static FileManagerLibraryUserSettings Setting;

        static AppSettings()
        {
            RefreshConfig();
        }

        public static bool RefreshConfig()
        {
            try
            {
                /* Sometimes when calling custom config .Net framework/ myDllConfig.Sections 
                 * cannot find the application config by its own and in this case application 
                 * need to manualy specify this assembly. So using AppDomain.CurrentDomain.AssemblyResolve
                 * specify where is the assembly */
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                //Open the configuration file using the dll location
                Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(typeof(AppSettings)).Location);
                string str = myDllConfig.AppSettings.Settings["UserContext"].Value;
                FileManagerLibraryUserSettingsConfiguration serviceConfigSection = myDllConfig.Sections["FileManagerLibraryCustomSettings"] as FileManagerLibraryUserSettingsConfiguration;
                Setting = serviceConfigSection.SettingsList[ActiveUserName];
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during Refreshing-Loading the Config file");
                return false;
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(Assembly.GetAssembly(typeof(AppSettings)).Location);
        }

        public static string AppSettingsNetworkLocation
        {
            get { return Setting.SettingsDetails.AppSettingsNetworkLocation; }
        }

        public static bool OverWriteUnChangedFiles
        {
            get { return Setting.SettingsDetails.OverWriteUnChangedFiles; }
        }

        public static bool AllowPerUserSettings
        {
            get { return Setting.SettingsDetails.AllowPerUserSettings; }
        }

        public static int FileLockoutWaitTimeOutSeconds
        {
            get { return Setting.SettingsDetails.FileLockoutWaitTimeOutSeconds; }
        }

        public static string WindowsServiceDisplayName
        {
            get { return Setting.SettingsDetails.WindowsServiceDisplayName; }
        }

        public static string WindowsServiceName
        {
            get { return Setting.SettingsDetails.WindowsServiceName; }
        }

        public static string WindowsServiceDescription
        {
            get { return Setting.SettingsDetails.WindowsServiceDescription; }
        }

        public static string ActiveUserName
        {
            get 
            {
                //Open the configuration file using the dll location
                Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(new AppSettings().GetType().Assembly.Location);
                // Get the appSettings section
                AppSettingsSection AppSetting = (AppSettingsSection)myDllConfig.GetSection("appSettings");
                string user = AppSetting.Settings["UserContext"].Value.ToString().ToUpper();
                return user;
            }
        }

        private static bool? _IsSafeMode;
        public static bool IsSafeMode
        {
            get
            {
                if (_IsSafeMode == null)
                {
                    //Open the configuration file using the dll location
                    Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(new AppSettings().GetType().Assembly.Location);
                    // Get the appSettings section
                    AppSettingsSection AppSetting = (AppSettingsSection)myDllConfig.GetSection("appSettings");
                    _IsSafeMode = (AppSetting.Settings["UseSafeMode"].Value.ToString().ToUpper() == "TRUE") ? true : false;
                }
                return _IsSafeMode.Value;
            }
        }
    }
}

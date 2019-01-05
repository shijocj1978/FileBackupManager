using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Logging.OSInterfaces
{
    public class AppConfigSettings
    {

        static bool? _IsDebugLoggingEnabled;
        public static bool IsDebugLoggingEnabled
        {
            get 
            {
                if (_IsDebugLoggingEnabled == null)
                {
                    //Open the configuration file using the dll location
                    Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(new AppConfigSettings().GetType().Assembly.Location);
                    // Get the appSettings section
                    AppSettingsSection AppSetting = (AppSettingsSection)myDllConfig.GetSection("appSettings");
                    _IsDebugLoggingEnabled = bool.Parse(AppSetting.Settings["IsDebugLoggingEnabled"].Value.ToString());
                    return _IsDebugLoggingEnabled.Value;
                }
                else
                {
                    return _IsDebugLoggingEnabled.Value;
                }
            }
        }

        static int? _FileLockoutWaitTimeOutSeconds;
        public static int FileLockoutWaitTimeOutSeconds
        {
            get 
            {
                if (_FileLockoutWaitTimeOutSeconds == null)
                {
                    //Open the configuration file using the dll location
                    Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(new AppConfigSettings().GetType().Assembly.Location);
                    // Get the appSettings section
                    AppSettingsSection AppSetting = (AppSettingsSection)myDllConfig.GetSection("appSettings");

                    _FileLockoutWaitTimeOutSeconds = int.Parse(AppSetting.Settings["FileLockoutWaitTimeOutSeconds"].Value.ToString());
                    return _FileLockoutWaitTimeOutSeconds.Value;
                }
                else
                {
                    return _FileLockoutWaitTimeOutSeconds.Value;
                }

            }
        }
    }
}

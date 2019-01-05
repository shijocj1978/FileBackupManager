using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using Logging.Framework;

namespace Logging.OSInterfaces
{
    public class FileManager
    {
        private static readonly object locker = new object();
        public static void WriteText(string path, string content)
        {
            try
            {
                lock (locker)
                {
                    if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, content);
                }     
            }
            catch (System.IO.IOException)
            {
                object args = new string[] { path, content };
                ThreadPool.QueueUserWorkItem(WriteTextAsync, args);
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                OSApplicationLogger.WriteToOSApplicationLog(LoggingProviderBase.CreateMessageLogEntry(ex), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public static void AppendText(string path, string content)
        {
            try
            {
                lock (locker)
                {
                    if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    //this file method writes all test you pass     
                    File.AppendAllText(path, content);
                }
            }
            catch (System.IO.IOException)
            {
                object args = new string[] { path, content };
                ThreadPool.QueueUserWorkItem(AppendTextAsync, args);
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                OSApplicationLogger.WriteToOSApplicationLog(LoggingProviderBase.CreateMessageLogEntry(ex), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public static string ReadText(string path)
        {
            try
            {

                if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                    return "";
                //this file method reads whole content of the text file.     
                return File.ReadAllText(path);
            }
            catch { return ""; }
        }

        public static void Archive(string path, String destFileName, bool KeepCopy)
        {
            try
            {
                if (KeepCopy)
                {
                    File.Copy(path, destFileName, true);
                }
                else
                {
                    File.Move(path, destFileName);
                }
            }
            catch { }
        }

        private static void AppendTextAsync(object args)
        {
            string path = "", content = "";
            try
            {
                bool CompleteFlag = false;
                string[] strarr = (string[])args;
                path = strarr[0];
                content = strarr[1];
                int i = 0; //2 min time to write. or release this thread.
                while (CompleteFlag == false || i < (2 * AppConfigSettings.FileLockoutWaitTimeOutSeconds))
                {
                    try
                    {
                        Thread.Sleep(500);
                        File.AppendAllText(path, content);
                        CompleteFlag = true;
                        i++;
                        return;
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                OSApplicationLogger.WriteToOSApplicationLog(ex, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private static void WriteTextAsync(object args)
        {
            string path = "", content = "";
            try
            {
                bool CompleteFlag = false;
                string[] strarr = (string[])args;
                path = strarr[0];
                content = strarr[1];
                int i = 0; //2 min time to write. or release this thread.
                while (CompleteFlag == false || i < (2 * AppConfigSettings.FileLockoutWaitTimeOutSeconds))
                {
                    try
                    {
                        Thread.Sleep(500);
                        File.WriteAllText(path, content);
                        CompleteFlag = true;
                        i++;
                        return;
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                OSApplicationLogger.WriteToOSApplicationLog(ex, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}

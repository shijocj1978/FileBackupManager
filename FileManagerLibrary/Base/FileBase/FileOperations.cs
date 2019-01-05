using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Logging;
using Logging.Framework;
using FileManagerBase;
using System.Threading;
using FileManagerLibrary.Base.UIData;

namespace FileManagerLibrary.Base.FileBase
{
    public class FileOperations
    {
        /// <summary>
        /// Gets the Root backup destination folder. Using the job name and root folder 
        /// name specified by user since there is a chance that the same destination location 
        /// can be used for multiple backups and they can be duplicate names.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetRootBackupDestinationFolder(BackupJob item)
        {
            return GetRootBackupDestinationFolder(item.RootBackupFolder, item.RootSourceFolder, item.JobSetName, item.JobGUID,item.CurrentBackupJobMode);
        }


        /// <summary>
        /// Gets the Root backup destination folder. Using the job name and root folder 
        /// name specified by user since there is a chance that the same destination location 
        /// can be used for multiple backups and they can be duplicate names.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetRootBackupDestinationFolder(string RootBackupFolder, string RootFolder, string JobSetName, Guid JobGUID,BackupJobModeEnum BackupMode)
        {
            try
            {
                if (BackupMode == BackupJobModeEnum.Files)
                {
                    return RootBackupFolder;
                }
                else
                {
                    //pass second and third parameter "" will tell it is to tell called method this is file mode.
                    if (RootBackupFolder == null || JobSetName == null || RootBackupFolder.Length == 0 || JobSetName.Length == 0)
                        return RootBackupFolder;
                    else
                    {
                        if (RootFolder != JobSetName)
                        {
                            if (JobSetName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                            {
                                if (!RootBackupFolder.EndsWith(JobSetName))
                                    return RootBackupFolder + @"\" + JobSetName;
                                else // avoid adding sub areas again and again.
                                    return RootBackupFolder;
                            }
                            else
                            {
                                Global.MessageBoxShow("Warning: Create a filesystem compatible name for " + JobSetName + ".This name contain invalid characters. Using temp id now");
                                return RootBackupFolder + @"\" + JobGUID.GetHashCode();
                            }
                        }
                        else
                        {
                            Global.MessageBoxShow("Warning: Create a filesystem compatible name for Unnamed BackupSet Name. Using temp name for now. Once the BackupJob name is valid the backup location will be modified.");
                            if (!RootBackupFolder.EndsWith(JobGUID.GetHashCode().ToString()))
                                return RootBackupFolder + @"\" + JobGUID.GetHashCode();
                            else // avoid adding sub areas again and again.
                                return RootBackupFolder;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex);
                return "";
            }
        }

        public static bool IsSubfolder(string RootFolder, string ChildFolder)
        {
            try
            {
                if (ChildFolder.StartsWith(RootFolder))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "RootFolder: " + RootFolder + " , ChildFolder: " + ChildFolder);
                return false;
            }
        }

        public static string Getfolder()
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowNewFolderButton = true;
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return "";
                return dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex);
                return "";
            }
        }

        public static string Getfolder(string startupfolder)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowNewFolderButton = true;
                dlg.SelectedPath = startupfolder;
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return "";
                return dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + startupfolder);
                return "";
            }
        }

        public static string[] GetFiles(string startupfolder)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = "*.*";
                dlg.InitialDirectory = startupfolder;
                dlg.Multiselect = true;
                dlg.ReadOnlyChecked = true;
                dlg.ShowReadOnly = false;
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return null;
                return dlg.FileNames;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + startupfolder);
                return null;
            }
        }

        public static bool IsFileSizeExceeded(string filename, int Maxsize, FileSizeTypeEnum maxfileType)
        {
            if (File.Exists(filename) == false)
                return false;
            FileInfo fl = new FileInfo(filename);
            long bitsize = 0; //need to define as a variable due to VS.Net buffer limitations.
            long maxlgth = 0;
            if (maxfileType == FileSizeTypeEnum.KB)
               bitsize = 1024;
            else if (maxfileType == FileSizeTypeEnum.MB)
                bitsize = 1048576;
            else if (maxfileType == FileSizeTypeEnum.GB)
                bitsize = 1073741824;
            else if(maxfileType == FileSizeTypeEnum.TB)
                bitsize = 1099511627776;
            maxlgth = Maxsize * bitsize;
            if (fl.Length > maxlgth)
                return true;
            else
                return false;
        }

        public static bool IsCreatedOrUpdatedOrAccessedBeforeHours(FileSystemInfo flobject, int Hours, FileCheckModeEnum FileCheckMode)
        {
            try
            {
                //second user requirement. wanted to check by modified date instead of created.
                if (FileCheckMode == FileCheckModeEnum.ByModifiedDate)
                    return IsModifiedBeforeHours(flobject, Hours);
                else if (FileCheckMode == FileCheckModeEnum.ByCreatedDate)
                    return IsCreatedBeforeHours(flobject, Hours);
                else
                    return IsAccessedBeforeHours(flobject, Hours);
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + flobject.FullName);
                return false;
            }
        }

        public static bool IsAccessedBeforeHours(FileSystemInfo flobject, int Hours)
        {
            try
            {
                DateTime date1 = DateTime.Today;
                DateTime date2 = flobject.LastAccessTime.Date;
                TimeSpan ts;
                ts = date1.Subtract(date2.Date);
                if (ts.TotalHours >= (double)Hours)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + flobject.FullName);
                return false;
            }
        }

        public static bool IsCreatedBeforeHours(FileSystemInfo flobject, int Hours)
        {
            try
            {
                DateTime date1 = DateTime.Today;
                DateTime date2 = flobject.CreationTime.Date;
                TimeSpan ts;
                ts = date1.Subtract(date2.Date);
                if (ts.TotalHours >= (double)Hours)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + flobject.FullName);
                return false;
            }
        }

        public static bool IsModifiedBeforeHours(FileSystemInfo flobject, int Hours)
        {
            try
            {
                DateTime date1 = DateTime.Today;
                DateTime date2 = flobject.LastWriteTime.Date;
                //Noticed some files are having modified dates much ahead of time. may be coming from other OS..
                if (flobject.LastWriteTime.Date > DateTime.Today)
                {
                    Logger.StaticLogger.AddWarningEntry("Possible issue in File: " + flobject.FullName + ". The Modified date looks like much time ahead.");
                    return false;
                }
                TimeSpan ts;
                ts = date1.Subtract(date2.Date);
                if (ts.TotalHours >= (double)Hours)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + flobject.FullName);
                return false;
            }
        }

        public static void XCopy(string SourcePathRoot, string TargetPath)
        {
            string strNewTargetPath = string.Empty;
            try
            {
                DirectoryInfo di = new DirectoryInfo(SourcePathRoot);
                foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
                {
                    if ((fi.Attributes & FileAttributes.Directory)== FileAttributes.Directory)
                    {
                        strNewTargetPath = TargetPath + @"\" + fi.Name;
                        XCopy(fi.FullName, strNewTargetPath);
                    }
                    else
                    {
                        FileCopy(fi.FullName, TargetPath + @"\" + fi.Name, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex);
            }
        }

        public static void DeletFolderContents(string TargetPath)
        {
            try
            {
                if (Directory.Exists(TargetPath))
                    DirectoryDelete(TargetPath);
                Directory.CreateDirectory(TargetPath);
            }
            catch (System.IO.PathTooLongException ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "A long filename or path found which is not supported by OS. Destination: " + TargetPath);
                return;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + TargetPath);
            }
        }

        public static bool DirectoryDelete(string TargetPath)
        {
            try
            {
                if (!Directory.Exists(TargetPath))
                    return true;
                Directory.Delete(TargetPath, true);
                return true;
            }
            catch (System.IO.PathTooLongException ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "A long filename or path found which is not supported by OS. Destination: " + TargetPath);
                return false;
            }
            catch (System.UnauthorizedAccessException)
            {
                try
                {
                    string[] files = Directory.GetFiles(TargetPath, "*", SearchOption.AllDirectories);
                    foreach (string fl in files)
                    {
                        File.SetAttributes(fl, FileAttributes.Normal);
                    }
                    Directory.Delete(TargetPath, true);
                    return true;
                }
                catch (Exception ex1)
                {
                    Logger.StaticLogger.AddErrorLogEntry(ex1, "Destination: " + TargetPath);
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + TargetPath);
                return false;
            }
        }

        public static bool DirectoryDelete(string TargetPath,bool DeleteOnlyifEmpty)
        {
            try
            {
                if (DeleteOnlyifEmpty == false)
                    return DirectoryDelete(TargetPath);
                else
                {
                    if (Directory.Exists(TargetPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(TargetPath);
                        if (di.HasSubDirectories() == true)
                            return true; //return success as the folder do not need to be deleted.
                        bool sts = DirectoryDelete(TargetPath);
                        Logger.StaticLogger.AddAuditLogEntry("Deleted empty folder " + di.FullName + ".");
                        return sts;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + TargetPath);
                return false;
            }
        }

        public static FileCopyStatus FileCopy(string source, string dest, bool overwrite)
        {
            try
            {
                if (File.Exists(source) == false)
                {
                    Logger.StaticLogger.AddErrorLogEntry("Error. Trying to copy a file that doesn't exist. Check the file name and location and verify the access permission is available.");
                    return FileCopyStatus.Error;
                }
                string path = Path.GetDirectoryName(dest);
                if (!Directory.Exists(path))
                    FileOperations.CreateDirectory(path);
                if (IsFilesChanged(source, dest) == true)
                {
                    File.Copy(source, dest, overwrite);
                    return FileCopyStatus.Success;
                }
                else if (AppSettings.OverWriteUnChangedFiles == true)
                {
                    File.Copy(source, dest, true);
                    return FileCopyStatus.Success;
                }
                return FileCopyStatus.Duplicate;
            }
            catch (System.IO.PathTooLongException ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "A long filename or path found which is not supported by OS. Filename: " + source + ", Destination: " + dest);
                return FileCopyStatus.Error;
            }
            catch (System.IO.IOException)
            {
                ThreadPool.QueueUserWorkItem(FileCopyAsync, new object[] { source, dest, overwrite });
                return FileCopyStatus.Success;
            }
            catch (System.UnauthorizedAccessException)
            {
                try
                {
                    File.SetAttributes(dest, FileAttributes.Normal);
                    File.Delete(dest);
                    File.Copy(source, dest);
                    return FileCopyStatus.Success;
                }
                catch (System.IO.IOException)
                {
                    ThreadPool.QueueUserWorkItem(FileCopyAsync, new object[] { source, dest, overwrite });
                    return FileCopyStatus.Success;
                }
                catch (Exception ex1)
                {
                    Logger.StaticLogger.AddErrorLogEntry(ex1, "Filename: " + source + ", Destination: " + dest);
                    return FileCopyStatus.Error;
                }
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex,"Filename: " + source + ", Destination: " + dest);
                return FileCopyStatus.Error;
            }
        }

        private static void FileCopyAsync(object args)
        {
            try
            {
                bool CompleteFlag = false;
                object[] obj = (object[])args;
                string sourcefilename = (string)obj[0];
                string destfilename = (string)obj[1];
                bool overwrite = (bool)obj[2];
                int i = 0; //AppSettings.FileLockoutWaitTimeOutSeconds time to write. or release this thread.
                while (CompleteFlag == false || i < (2 * AppSettings.FileLockoutWaitTimeOutSeconds))
                {
                    try
                    {
                        Thread.Sleep(500);
                        File.Copy(sourcefilename, destfilename, overwrite);
                        CompleteFlag = true;
                        i++;
                        return;
                    }
                    catch
                    { }
                }
                //incase any other exception so tht no file can be written then log to application log of the system
                Logger.StaticLogger.AddErrorLogEntry("Error occured during async copy operation. I tried for " + AppSettings.FileLockoutWaitTimeOutSeconds + " Seconds and not more. It is still locked or no access provided. Details: Sourcefilename" + sourcefilename + ", Destination: " + destfilename + ", Overwrite: " + overwrite.ToString());
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during async copy operation.");
            }
        }

        public static FileCopyStatus FileMove(string source, string dest, bool overwrite)
        {
            try
            {
                if (File.Exists(source) == false)
                {
                    Logger.StaticLogger.AddErrorLogEntry("Error. Trying to copy a file that doesn't exist. Check the file name and location and verify the access permission is available." 
                        + "source = " + source + ", dest = " + dest + ", overwrite = " + overwrite);
                    return FileCopyStatus.Error;
                }
                string path = Path.GetDirectoryName(dest);
                if (!Directory.Exists(path))
                    FileOperations.CreateDirectory(path);
                if (IsFilesChanged(source, dest) == true)
                {
                    if (overwrite == true && File.Exists(dest) == true)
                        DeleteFile(dest, true);
                    File.Move(source, dest);
                    if (File.Exists(source) == true)
                    {
                        Logger.StaticLogger.AddAuditLogEntry("Error while moving file, " + source + ". Unable to delete the source. Copy Exists in, " + dest + ", overwrite mode = " + overwrite);
                        return FileCopyStatus.Error;
                    }
                    else
                        return FileCopyStatus.Success;
                }
                else if (AppSettings.OverWriteUnChangedFiles == true)
                {
                    if (overwrite == true)
                        if (File.Exists(dest) == true)
                            DeleteFile(dest, true);
                    File.Move(source, dest);
                    return FileCopyStatus.Success;
                }
                else //Somehow the file is there in source and destination and it is supposed to be moved. So delete the file in source.
                    DeleteFile(source);
                return FileCopyStatus.Duplicate;
            }
            catch (System.IO.PathTooLongException ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "A long filename or path found which is not supported by OS. Filename: " + source + ", Destination: " + dest + ", overwrite = " + overwrite);
                return FileCopyStatus.Error;
            }
            catch (System.IO.IOException)
            {
                ThreadPool.QueueUserWorkItem(FileMoveAsync, new object[] { source, dest });
                return FileCopyStatus.Success;
            }
            catch (System.UnauthorizedAccessException)
            {
                try
                {
                    File.SetAttributes(dest, FileAttributes.Normal);
                    File.Delete(dest);
                    File.Move(source, dest);
                    return FileCopyStatus.Success;
                }
                catch (System.IO.IOException)
                {
                    ThreadPool.QueueUserWorkItem(FileMoveAsync, new object[] {source,dest});
                    return FileCopyStatus.Success;
                }
                catch (Exception ex1)
                {
                    Logger.StaticLogger.AddErrorLogEntry(ex1, "Filename: " + source + ", Destination: " + dest + ", Overwrite: " + overwrite);
                    return FileCopyStatus.Error;
                }
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Filename: " + source + ", Destination: " + dest + ", Overwrite: " + overwrite);
                return FileCopyStatus.Error;
            }
        }

        private static void FileMoveAsync(object args)
        {
            try
            {
                bool CompleteFlag = false;
                object[] obj = (object[])args;
                string sourcefilename = (string)obj[0];
                string destfilename = (string)obj[1];
                int i = 0; //AppSettings.FileLockoutWaitTimeOutSeconds time to write. or release this thread.
                while (CompleteFlag == false || i < (2 * AppSettings.FileLockoutWaitTimeOutSeconds))
                {
                    try
                    {
                        Thread.Sleep(500);
                        File.Move(sourcefilename, destfilename);
                        CompleteFlag = true;
                        i++;
                        return;
                    }
                    catch
                    { }
                }
                //incase any other exception so tht no file can be written then log to application log of the system
                Logger.StaticLogger.AddErrorLogEntry("Error occured during async copy operation. I tried for " + AppSettings.FileLockoutWaitTimeOutSeconds + " Seconds and not more. It is still locked or no access provided. Details: Sourcefilename" + sourcefilename + ", Destination: " + destfilename);
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during async copy operation.");
            }
        }

        public static bool DeleteFile(string filename)
        {
            return DeleteFile(filename, false);
        }

        public static bool DeleteFile(string filename,bool waitforcompletion)
        {
            try
            {
                if (AppSettings.IsSafeMode == false)
                {
                    File.Delete(filename);
                    return true;
                }
                else
                {
                    Logger.StaticLogger.AddAuditLogEntry("File deletion skipped by user override. FileName: " + filename);
                    return false;
                }
            }
            catch (System.IO.PathTooLongException ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "A long filename or path found which is not supported by OS. Filename: " + filename);
                return false;
            }
            catch (System.IO.IOException)
            {
                if (waitforcompletion == false)
                    ThreadPool.QueueUserWorkItem(DeleteFileAsync, filename);
                else
                    DeleteFileAsync(filename);
                return true;
            }
            catch (System.UnauthorizedAccessException)
            {
                try
                {
                    File.SetAttributes(filename, FileAttributes.Normal);
                    File.Delete(filename);
                    return true;
                }
                catch (System.IO.IOException)
                {
                    if (waitforcompletion == false)
                        ThreadPool.QueueUserWorkItem(DeleteFileAsync, filename);
                    else
                        DeleteFileAsync(filename);
                    return true;
                }
                catch (Exception ex1)
                {
                    Logger.StaticLogger.AddErrorLogEntry(ex1, "Destination: " + filename);
                    return false;
                }
            }
            
            catch(Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Destination: " + filename);
                return false;
            }
        }

        private static void DeleteFileAsync(object args)
        {
            try
            {
                bool CompleteFlag = false;
                string filename = (string)args;
                int i = 0; //AppSettings.FileLockoutWaitTimeOutSeconds time to write. or release this thread.
                while (CompleteFlag == false || i < (2 * AppSettings.FileLockoutWaitTimeOutSeconds))
                {
                    try
                    {
                        Thread.Sleep(500);
                        File.Delete(filename);
                        CompleteFlag = true;
                        i++;
                        return;
                    }
                    catch
                    { }
                }
                //incase any other exception so tht no file can be written then log to application log of the system
                Logger.StaticLogger.AddErrorLogEntry("Error occured during async delete operation. I tried for " + AppSettings.FileLockoutWaitTimeOutSeconds + " Seconds and not more. It is still locked or no access provided. Details, Filename: " + filename);
            }
            catch (Exception ex)
            {
                //incase any other exception so tht no file can be written then log to application log of the system
                Logger.StaticLogger.AddErrorLogEntry(ex,"Error occured during async delete operation.");
            }
        }

        public static bool IsFilesChanged(string sourcefile, string destinationfile)
        {
            try
            {
                if (sourcefile == null || destinationfile == null || sourcefile == "" || destinationfile == "")
                    return true;
                FileInfo source = new FileInfo(sourcefile);
                FileInfo dest = new FileInfo(destinationfile);
                if (source.Exists != dest.Exists) //if both source and destination exists or if both donot exists the nreturn true as it cannot be comapred.
                    return true;
                if (source.Length != dest.Length)
                    return true;
                if (source.LastWriteTime.Millisecond != dest.LastWriteTime.Millisecond)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Filename: " + sourcefile + ", Destination: " + destinationfile);
                return false;
            }
        }

        public static bool CreateDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex);
                return false;
            }
        }

        public static string GetAppSettingsFolderRoot()
        {
            try
            {
                if (AppSettings.AppSettingsNetworkLocation != null && AppSettings.AppSettingsNetworkLocation.Length > 0)
                {
                    try
                    {
                        if (Directory.Exists(AppSettings.AppSettingsNetworkLocation.TrimEnd(new char [] {'\\'}) + "\\FileBackupManagerData\\") == false)
                            Directory.CreateDirectory(AppSettings.AppSettingsNetworkLocation.TrimEnd(new char [] {'\\'}) + "\\FileBackupManagerData\\");
                        //try the write access or any other issues. 
                        File.Create(AppSettings.AppSettingsNetworkLocation.TrimEnd(new char[] { '\\' }) + "\\FileBackupManagerData\\" + "Test.tmp").Close();
                        File.Delete(AppSettings.AppSettingsNetworkLocation.TrimEnd(new char[] { '\\' }) + "\\FileBackupManagerData\\" + "Test.tmp");
                        return AppSettings.AppSettingsNetworkLocation.TrimEnd(new char[] { '\\' }) + "\\FileBackupManagerData";
                    }
                    catch (Exception ex)
                    {
                            Global.MessageBoxShow("Critical error occured during GetAppSettings FolderRoot with user: " + AppSettings.ActiveUserName + ". Trying to continue with default path. Exception details.: " + Logger.StaticLogger.GetExceptionEntry(ex));
                    }//if error then go to next statement and then continue with defaul path. 
                }
                if (AppSettings.AllowPerUserSettings == false)
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\FileBackupManagerData\\" + AppSettings.ActiveUserName + "\\";
                else
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\FileBackupManagerData\\";
            }
            catch (Exception ex)
            {
                Global.MessageBoxShow("Critical Error Occured during GetAppSettings FolderRoot. " + 
                "Reset Appsettings location may be required. " + Environment.NewLine + "User: " +
                AppSettings.ActiveUserName + Environment.NewLine + "Error Message: " + 
                Logger.StaticLogger.GetExceptionEntry(ex) + Environment.NewLine + 
                "Check FileManagerLibrary.dll.Config file for details.");
                Application.Exit();
                return "";
            }
        }

        public static string GetAppLocationRoot()
        {
            return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        public const string FILETIMESTAMP_PATTERN = "dd-MMM-yyyy hh.mm.ss.ff tt";

        public static string GetSearchPattern(string filename)
        {
            FileInfo fl = new FileInfo(filename);
            string pattern = fl.Name.Substring(0, fl.Name.Length - (fl.Extension.Length)) +
            "(??????????????.??.??.?????)" + fl.Extension;
            return pattern;
        }

        public static string GetNextFileNameVersion(string filename, string destinationroot)
        {
            /*Date Format used = dd-MMM-yyyy hh.mm.ss.ff tt */
            FileInfo fl = new FileInfo(filename);
            string newfilename = destinationroot + "\\" + fl.Name + "\\" + fl.Name.Substring(0, fl.Name.Length -
                (fl.Extension.Length)) + "(" + DateTime.Now.ToString(FILETIMESTAMP_PATTERN) +
                ")" + fl.Extension;
            return newfilename;
        }

        public static bool DeleteOldestFileinFolder(string destinationroot, string filename, int maxbackups)
        {
            FileInfo fl = new FileInfo(filename);
            string pattern = GetSearchPattern(filename);
            DirectoryInfo info = new DirectoryInfo(destinationroot + "\\" + fl.Name);
            FileInfo[] files = info.GetFiles(pattern, SearchOption.TopDirectoryOnly);
            int filestodelete=0;
            if (files.Length == 0)
                return true;
            else
                filestodelete = files.Length - maxbackups;  //incase the maxbackups is changed or somehow the files number changed and gone up thatn decided limit then delete all the old files.
            for (int i = 0; i < filestodelete; i++)
            {
                string name = GetOldestFileInFolder(destinationroot + "\\" + fl.Name, pattern);
                if(name!=null)
                    FileOperations.DeleteFile(name);
            }
            return true;
        }

        public static string GetLatestFileInFolder(string destinationroot, string pattern)
        {
            return GetLatest_OldestFileInFolder(destinationroot, pattern, true);
        }

        public static string GetOldestFileInFolder(string destinationroot, string pattern)
        {
            return GetLatest_OldestFileInFolder(destinationroot, pattern, false);
        }

        private static string GetLatest_OldestFileInFolder(string destinationroot,string pattern, bool findlatestfile)
        {
            if (Directory.Exists(destinationroot) == false)
                return null;
            DirectoryInfo info = new DirectoryInfo(destinationroot);
            FileInfo[] files = info.GetFiles(pattern, SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return null;
            //find the oldest file in the group using the file name.
            FileInfo oldestfile = files[0];
            DateTime oldesttime;
            if (findlatestfile == true)
                oldesttime = DateTime.MinValue;
            else
                oldesttime = DateTime.MaxValue;
            foreach (FileInfo file in files)
            {
                //Extract timestamp and convert it to datetime.
                int start = file.Name.Length - (file.Extension.Length + 27); //27 = length of timestamp + ")"
                string timestamp = file.Name.Substring(start, 26);
                DateTime currenttime;
                bool rslt = DateTime.TryParseExact(timestamp, FILETIMESTAMP_PATTERN,
                                       System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out currenttime);
                if (rslt == false) // if date string is invalid then skip.
                {
                    Logger.StaticLogger.AddErrorLogEntry("Invalid date string found in versioned backup folder. Remove the files if possible. Try avoid modifying files in version folders. " + destinationroot + ", " + pattern + ".");
                    continue;
                }
                if (findlatestfile == true)
                {
                    if (currenttime > oldesttime)
                    {
                        oldesttime = currenttime;
                        oldestfile = file;
                    }
                }
                else
                {
                    if (currenttime < oldesttime)
                    {
                        oldesttime = currenttime;
                        oldestfile = file;
                    }
                }
            }
            return oldestfile.FullName;
        }

        public static Int64 GetFilesCountInDirectory(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            return dir.GetFiles().Length;
        }

        public static Int64 GetFilesCountInDirectory(string directory,string extension)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] files = dir.GetFiles("*" + extension);
            return files.Length;
        }
    }

    public enum FileCopyStatus
    {
        Success,
        Duplicate,
        Error
    }
}

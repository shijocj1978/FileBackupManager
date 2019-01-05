using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Logging;
using FileManagerBase;
using System.Threading;
using System.Windows.Forms;
using FileManagerLibrary.Base.FileBase;
using FileManagerLibrary.Base.UIData;

namespace FileManagerLibrary.Base
{
    public class FileSystemOperations
    {
        
        public event EventHandler<GenericEventArgsType<bool>> OperationCompleted;

        public void PerformFolderBackupAsyc(object backupjob)
        {
            bool rslt = PerformFolderBackup((BackupJob)backupjob);
            if(OperationCompleted!=null)
                OperationCompleted(this, new GenericEventArgsType<bool>(rslt));
        }

        public bool PerformFolderBackup(BackupJob backupjob)
        {
            JobFilesOperationModeEnum CurrentMode =backupjob.JobFileOperationMode;
            string SourcePathRoot =backupjob.RootSourceFolder;
            string TargetPath = FileOperations.GetRootBackupDestinationFolder(backupjob);
            List<string> ExcludedFolders=backupjob.FolderSettings.ExcludedLocations; 
            int KeepFileForDays=backupjob.BackupAfterDays; 
            int KeepFilesForHours=backupjob.BackupAfterHours; 
            bool DeleteEmptyFolders=backupjob.FolderSettings.IncludeEmptyFolders; 
            FileCheckModeEnum FileCheckMode=backupjob.FolderSettings.FileCheckMode; 
            bool IncludeEmptyFolders =backupjob.FolderSettings.IncludeEmptyFolders;
            try
            {
                int BufferHours = (KeepFileForDays * 24) + KeepFilesForHours;
                switch (CurrentMode)
                {
                    case JobFilesOperationModeEnum.Copy:
                        CopyFolders(SourcePathRoot, TargetPath, ExcludedFolders, BufferHours, FileCheckMode, IncludeEmptyFolders);
                        break;
                    case JobFilesOperationModeEnum.Delete:
                        DeleteFolders(SourcePathRoot, ExcludedFolders, BufferHours, DeleteEmptyFolders,FileCheckMode);
                        if (Directory.Exists(SourcePathRoot) == false)
                            FileOperations.CreateDirectory(SourcePathRoot);
                        break;
                    case JobFilesOperationModeEnum.Move:
                        MoveFolders(SourcePathRoot, TargetPath, ExcludedFolders, BufferHours, DeleteEmptyFolders, FileCheckMode);
                        if (Directory.Exists(SourcePathRoot) == false)
                            FileOperations.CreateDirectory(SourcePathRoot);
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex,"PerformFolderBackup(FileOperationModeEnum CurrentMode, string SourcePathRoot, string TargetPath, List<string> ExcludedFolders, int KeepFileForDays, int KeepFilesForHours, bool DeleteEmptyFolders)");
                return false;
            }
        }

        public void PerformFilesBackupAsyc(object backupjob)
        {
            bool rslt = PerformFilesBackup((BackupJob)backupjob);
            if (OperationCompleted != null)
                OperationCompleted(this, new GenericEventArgsType<bool>(rslt));
        }

        public bool PerformFilesBackup(BackupJob backupjob)
        {
            try
            {
                switch (backupjob.JobFileOperationMode)
                {
                    case JobFilesOperationModeEnum.Copy:
                        foreach (var item in backupjob.FileSettings.FileNames)
                        {
                            PerformSinlgeFileCopy(item, backupjob);
                        }
                        break;
                    case JobFilesOperationModeEnum.Move:
                        foreach (var item in backupjob.FileSettings.FileNames)
                        {
                            PerformSingleFileMove(item, backupjob);
                        }
                        break;
                    case JobFilesOperationModeEnum.Delete:
                        foreach (var item in backupjob.FileSettings.FileNames)
                        {
                            PerformSingleFileDelete(item, backupjob);
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "PerformFilesBackup(BackupJob backupjob)");
                return false;
            }
        }

        public bool PerformSingleFileBackup(BackupJob backupjob,string filename)
        {
            try
            {
                if (backupjob.FileSettings.FileNames.Contains(filename) == false)
                    return false;
                switch (backupjob.JobFileOperationMode)
                {
                    case JobFilesOperationModeEnum.Copy:
                        PerformSinlgeFileCopy(filename, backupjob);
                        break;
                    case JobFilesOperationModeEnum.Move:
                        PerformSingleFileMove(filename, backupjob);
                        break;
                    case JobFilesOperationModeEnum.Delete:
                        PerformSingleFileDelete(filename, backupjob);
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "PerformSingleFileBackup(BackupJob backupjob,string filename)");
                return false;
            }
        }

        public bool PerformSinlgeFileCopy(string filename, BackupJob backupjob)
        {
            try
            {
                int BufferHours = (backupjob.BackupAfterDays * 24) + backupjob.BackupAfterHours;
                if (File.Exists(filename) == false)
                    Logger.StaticLogger.AddErrorLogEntry("Error. Trying to copy a file that doesn't exist. Check the file name and location and verify the access permission is available.");
                else
                {
                    FileInfo fl = new FileInfo(filename);
                    string destinationfilename = backupjob.RootBackupFolder + "\\" + fl.Name;
                    if (backupjob.FileSettings.MaxVersions > 1)
                    {
                        if (File.Exists(destinationfilename)) //incase the user decides to keep multiple copies after last run there can be a file in the same name instead of folder. so need to delete that before continue.
                            if (FileOperations.DeleteFile(destinationfilename,true) == false)
                                return false; //if cannot delete the file then skip
                        if (FileOperations.IsFilesChanged(filename, FileOperations.GetLatestFileInFolder(destinationfilename, FileOperations.GetSearchPattern(filename))) == false)
                            return false; //if file in backup location not changed then skip
                        destinationfilename = FileOperations.GetNextFileNameVersion(filename, backupjob.RootBackupFolder);
                    }
                    else
                    {
                        if (Directory.Exists(destinationfilename)) //incase the user decides not to keep multiple copies after last run there can be a folder in the same name instead of file. so need to delete that before continue.
                            if (FileOperations.DirectoryDelete(destinationfilename) == false)
                                return false; //if cannot delete the file then skip
                    }
                    if (backupjob.FileSettings.FileSizeCheckEnabled == true)
                    {
                        if (FileOperations.IsFileSizeExceeded(filename, backupjob.FileSettings.MaxFileSize, backupjob.FileSettings.MaxFileSizeType) == true)
                            CopyFile(filename, destinationfilename, BufferHours, FileCheckModeEnum.ByModifiedDate);
                        else
                            return false;
                    }
                    else
                        CopyFile(filename, destinationfilename, BufferHours, FileCheckModeEnum.ByModifiedDate);
                    if (backupjob.FileSettings.MaxVersions > 1)
                        FileOperations.DeleteOldestFileinFolder(backupjob.RootBackupFolder, filename, backupjob.FileSettings.MaxVersions);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during copy of file: " + filename);
                return false;
            }
        }

        public bool PerformSingleFileDelete(string filename, BackupJob backupjob)
        {
            try
            {
                int BufferHours = (backupjob.BackupAfterDays * 24) + backupjob.BackupAfterHours;
                if (backupjob.FileSettings.FileSizeCheckEnabled == true)
                {
                    if (FileOperations.IsFileSizeExceeded(filename, backupjob.FileSettings.MaxFileSize, backupjob.FileSettings.MaxFileSizeType) == true)
                        DeleteFile(filename, BufferHours, FileCheckModeEnum.ByModifiedDate);
                    else
                        return false;
                }
                else
                    DeleteFile(filename, BufferHours, FileCheckModeEnum.ByModifiedDate);
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during delete of file: " + filename);
                return false;
            }
        }

        public bool PerformSingleFileMove(string filename, BackupJob backupjob)
        {
            try
            {
                int BufferHours = (backupjob.BackupAfterDays * 24) + backupjob.BackupAfterHours;
                if (File.Exists(filename) == false)
                    Logger.StaticLogger.AddErrorLogEntry("Error. Trying to move a file that doesn't exist. Check the file name and location and verify the access permission is available.");
                else
                {
                    FileInfo fl = new FileInfo(filename);
                    string destinationfilename = backupjob.RootBackupFolder + "\\" + fl.Name;
                    if (backupjob.FileSettings.MaxVersions > 1)
                    {
                        if (File.Exists(destinationfilename)) //incase the user decides to keep multiple copies after last run there can be a file in the same name instead of folder. so need to delete that before continue.
                            if (FileOperations.DeleteFile(destinationfilename,true) == false)
                                return false; //if cannot delete the file then skip
                        if (FileOperations.IsFilesChanged(filename, FileOperations.GetLatestFileInFolder(destinationfilename, FileOperations.GetSearchPattern(filename))) == false)
                            return false; //if file in backup location 
                        destinationfilename = FileOperations.GetNextFileNameVersion(filename, backupjob.RootBackupFolder);
                    }
                    else
                    {
                        if (Directory.Exists(destinationfilename)) //incase the user decides not to keep multiple copies after last run there can be a folder in the same name instead of file. so need to delete that before continue.
                            if (FileOperations.DirectoryDelete(destinationfilename) == false)
                                return false; //if cannot delete the file then skip
                    }
                    if (backupjob.FileSettings.FileSizeCheckEnabled == true)
                    {
                        if (FileOperations.IsFileSizeExceeded(filename, backupjob.FileSettings.MaxFileSize, backupjob.FileSettings.MaxFileSizeType) == true)
                            MoveFile(filename, destinationfilename, BufferHours, FileCheckModeEnum.ByModifiedDate);
                        else
                            return false;
                    }
                    else
                        MoveFile(filename, destinationfilename, BufferHours, FileCheckModeEnum.ByModifiedDate);
                    if (backupjob.FileSettings.MaxVersions > 1)
                        FileOperations.DeleteOldestFileinFolder(backupjob.RootBackupFolder, filename, backupjob.FileSettings.MaxVersions);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during move of file: " + filename);
                return false;
            }
        }

        private void CopyFile(string filename, string destination, int includefile_only_before_hours, FileCheckModeEnum FileCheckMode)
        {
            if (FileOperations.IsCreatedOrUpdatedOrAccessedBeforeHours(new FileInfo(filename), includefile_only_before_hours, FileCheckMode) == true)
            {
                if (FileOperations.FileCopy(filename, destination, true) == FileCopyStatus.Success)
                    Logger.StaticLogger.AddAuditLogEntry("Copied file " + filename + " to " + destination + ", includefile_only_before_hours = " + includefile_only_before_hours + ", FileCheckMode" + FileCheckMode.ToString());
            }
        }

        private void DeleteFile(string filename, int includefile_only_before_hours, FileCheckModeEnum FileCheckMode)
        {
            if (FileOperations.IsCreatedOrUpdatedOrAccessedBeforeHours(new FileInfo(filename), includefile_only_before_hours, FileCheckMode) == true)
            {
                if (FileOperations.DeleteFile(filename) == true)
                    Logger.StaticLogger.AddAuditLogEntry("Deleted file, " + filename + ", includefile_only_before_hours = " + includefile_only_before_hours + ", FileCheckMode" + FileCheckMode.ToString());
            }
        }

        private bool MoveFile(string filename, string destination, int includefile_only_before_hours, FileCheckModeEnum FileCheckMode)
        {
            if (FileOperations.IsCreatedOrUpdatedOrAccessedBeforeHours(new FileInfo(filename), includefile_only_before_hours, FileCheckMode))
            {
                FileCopyStatus sts = FileOperations.FileMove(filename, destination, true);
                if (sts != FileCopyStatus.Error)
                {
                    if(sts == FileCopyStatus.Success)
                        Logger.StaticLogger.AddAuditLogEntry("Moved file, " + filename + ", destination" + destination + ", includefile_only_before_hours = " + includefile_only_before_hours + ", FileCheckMode" + FileCheckMode.ToString());
                    else
                        Logger.StaticLogger.AddAuditLogEntry("Deleted source file, " + filename + ", as destination" + destination + "Exists is the same. options: includefile_only_before_hours = " + includefile_only_before_hours + ", FileCheckMode" + FileCheckMode.ToString());
                }
            }
            return true;
        }

        private void MoveFolders(string SourcePathRoot, string TargetPath, List<string> ExcludedFolders, int MoveFilesAfterHours, bool DeleteEmptyFolders, FileCheckModeEnum FileCheckMode)
        {
            string strNewTargetPath = string.Empty;
            try
            {
                DirectoryInfo di = new DirectoryInfo(SourcePathRoot);
                foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
                {
                    try // prevents the execution from breakout of loop if an exception occurs
                    {
                        if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (ExcludedFolders.Contains(fi.FullName) == false)
                            {
                                strNewTargetPath = TargetPath + @"\" + fi.Name;
                                this.MoveFolders(fi.FullName, strNewTargetPath, ExcludedFolders, MoveFilesAfterHours, DeleteEmptyFolders,FileCheckMode);
                            }
                        }
                        else
                        {
                            if (ExcludedFolders.Contains(fi.FullName) == false)
                            {
                                MoveFile(fi.FullName, TargetPath + @"\" + fi.Name, MoveFilesAfterHours, FileCheckMode);
                            }
                        }
                        if (DeleteEmptyFolders == true)
                            FileOperations.DirectoryDelete(di.FullName, true);
                    }
                    catch (Exception subex)
                    {
                        Logger.StaticLogger.AddErrorLogEntry(subex,"MoveFiles(string SourcePathRoot, string TargetPath, List<string> ExcludedFolders, int MoveFilesAfterHours, bool DeleteEmptyFolders)" +  " - Error on file, " + fi.FullName);
                    }
                }
                if (DeleteEmptyFolders == true)
                {
                    FileOperations.DirectoryDelete(di.FullName, true);
                }
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex,"MoveFiles(string SourcePathRoot, string TargetPath, List<string> ExcludedFolders, int MoveFilesAfterHours, bool DeleteEmptyFolders)");
            }
        }

        private void DeleteFolders(string SourcePathRoot, List<string> ExcludedFolders, int DeleteFilesAfterHours, bool DeleteEmptyFolders, FileCheckModeEnum FileCheckMode)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(SourcePathRoot);
                foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
                {
                    try// prevents the execution from breakout of loop if an exception occurs
                    {
                        if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (ExcludedFolders.Contains(fi.FullName) == false)
                            {
                                this.DeleteFolders(fi.FullName, ExcludedFolders, DeleteFilesAfterHours, DeleteEmptyFolders,FileCheckMode);
                            }
                        }
                        else
                        {
                            if (ExcludedFolders.Contains(fi.FullName) == false)
                            {
                                DeleteFile(fi.FullName, DeleteFilesAfterHours, FileCheckMode);
                            }
                        }
                        if (DeleteEmptyFolders == true)
                            FileOperations.DirectoryDelete(di.FullName, true);
                    }                    
                    catch (Exception subex)
                    {
                        Logger.StaticLogger.AddErrorLogEntry(subex,"DeleteFiles(string SourcePathRoot, List<string> ExcludedFolders, int DeleteFilesAfterHours,bool DeleteEmptyFolders)" + " - Error on file, " + fi.FullName);
                    }
                }
                if (DeleteEmptyFolders == true)
                    FileOperations.DirectoryDelete(di.FullName,true);
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex,"DeleteFiles(string SourcePathRoot, List<string> ExcludedFolders, int DeleteFilesAfterHours,bool DeleteEmptyFolders)");
            }
        }

        private void CopyFolders(string SourcePathRoot, string TargetPath, List<string> ExcludedFolders, int CopyFilesAfterHours, FileCheckModeEnum FileCheckMode, bool IncludeEmptyFolders)
        {
            string strNewTargetPath = string.Empty;
            try
            {
                DirectoryInfo di = new DirectoryInfo(SourcePathRoot);
                foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
                {
                    try // prevents the execution from breakout of loop if an exception occurs
                    {
                        if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (ExcludedFolders.Contains(fi.FullName) == false)
                            {
                                strNewTargetPath = TargetPath + @"\" + fi.Name;
                                this.CopyFolders(fi.FullName, strNewTargetPath, ExcludedFolders, CopyFilesAfterHours, FileCheckMode, IncludeEmptyFolders);
                                //creates and empty directory if specified in UI. This is required
                                if (!Directory.Exists(strNewTargetPath) && IncludeEmptyFolders == true)
                                    FileOperations.CreateDirectory(strNewTargetPath);
                            }
                        }
                        else
                        {
                            if (ExcludedFolders.Contains(fi.FullName) == false)
                            {
                                CopyFile(fi.FullName, TargetPath + @"\" + fi.Name, CopyFilesAfterHours, FileCheckMode);
                            }
                        }
                    }
                    catch (Exception subex)
                    {
                        Logger.StaticLogger.AddErrorLogEntry(subex,"Error on file copy, " + fi.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex,"CopyFiles(string SourcePathRoot, string TargetPath, List<string> ExcludedFolders, int CopyFilesAfterHours)");
            }
        }

        public void PerformCleanupDestinationAsyc(object backupjob)
        {
            bool rslt = PerformCleanupDestination((BackupJob)backupjob);
            if(OperationCompleted!=null)
                OperationCompleted(this, new GenericEventArgsType<bool>(rslt));
        }

        public bool PerformCleanupDestination(BackupJob backupjob)
        {
            return PerformCleanupDestination(backupjob.RootSourceFolder, FileOperations.GetRootBackupDestinationFolder(backupjob));
        }

        private bool PerformCleanupDestination(string RootSourceFolder, string RootBackupFolder)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(RootBackupFolder);
                foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
                {
                    try// prevents the execution from breakout of loop if an exception occurs
                    {
                        if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            this.PerformCleanupDestination(RootSourceFolder + fi.FullName.Substring(RootBackupFolder.Length), fi.FullName);
                        }
                        else
                        {
                            FileSystemInfo source = new FileInfo(RootSourceFolder + fi.FullName.Substring(RootBackupFolder.Length));
                            if (source.Exists == false)
                            {
                                if (FileOperations.DeleteFile(fi.FullName) == true)
                                    Logger.StaticLogger.AddAuditLogEntry("Deleted file, " + fi.FullName + ".");
                            }
                        }
                    }
                    catch (Exception subex)
                    {
                        Logger.StaticLogger.AddErrorLogEntry(subex, "PerformCleanupDestination(string RootSourceFolder, string RootBackupFolder)" + " - Error on file, " + fi.FullName);
                    }
                }
                //deletes the current folder if it is empty.
                FileOperations.DirectoryDelete(di.FullName, true);
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "PerformCleanupDestination(string RootSourceFolder, string RootBackupFolder)");
            }
            return true;
        }
    }
}

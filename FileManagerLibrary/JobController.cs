using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FileManagerBase;
using FileManagerLibrary.Base;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Logging;
using FileManagerLibrary.Base.FileBase;
using FileManagerLibrary.Base.Serialization;
using FileManagerLibrary.Base.UIData;

namespace FileManagerLibrary
{
    [Serializable]
    public class JobController : ISerializable
    {
        public BackupJobList FolderData { get; set; }
        public JobController()
        {
            FolderData = new BackupJobList();
        }

        public static JobController Init()
        {
            if (File.Exists(FileOperations.GetAppSettingsFolderRoot() + "\\" + "Appsettings.datastore") == false)
            {
                return new JobController();
            }
            Serializer<JobController> data = new Serializer<JobController>();
            JobController mgr = data.DeSerializeObject(FileOperations.GetAppSettingsFolderRoot() + "\\" + "Appsettings.datastore");
            return mgr;
        }

        public bool SaveData()
        {
            Serializer<JobController> data = new Serializer<JobController>();
            data.SerializeObject(FileOperations.GetAppSettingsFolderRoot() + "\\" + "Appsettings.datastore", this);
            return true;
        }

        #region ISerializable Members

        public JobController(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("Unexpected error occured during de-serialization");
            try
            {
                FolderData = (BackupJobList)info.GetValue("BackupJobList", typeof(BackupJobList));
            }
            catch
            {
                FolderData = new BackupJobList();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("Unexpected error occured during serialization");
            info.AddValue("BackupJobList", FolderData);
        }

        #endregion
       
        /// <summary>
        ///  Call this one only from a Windows Forms application
        /// </summary>
        /// <returns>bool</returns>
        public bool WinValidateAllDataForScheduler()
        {
            bool val;
            val = ValidateAllDataForScheduler();
            return val;
        }

        public bool ValidateAllDataForScheduler()
        {
            if (ValidateGlobalFolderData() == false)
                return false;
            foreach (var val in FolderData)
            {
                if (val.Value.Enabled == false)
                    continue;
                BackupJob item = val.Value;
                if (ValidateFolderData(item) == false)
                    return false;
            }
            return true;
        }

        public bool ValidateGlobalFolderData()
        {
            if (FolderData.Count == 0)
            {
                Global.MessageBoxShow("No Source folder Selected. Atleast one is required. Cannot proceed.");
                return false;
            }
            return true;
        }

        public bool ValidateFolderData(BackupJob item)
        {
            if (item.CurrentBackupJobMode == BackupJobModeEnum.Folders)
            {
                if (Directory.Exists(item.RootSourceFolder) == false)
                {
                    string msg = "Found Issues in '" + item.JobSetName + "'. Unable to find the RootFolder. Cannot proceed.";
                    Global.MessageBoxShow(msg);
                    return false;
                }
            }
            if (item.JobFileOperationMode == JobFilesOperationModeEnum.Copy || item.JobFileOperationMode == JobFilesOperationModeEnum.Move)
            {
                if (item.RootBackupFolder == "")
                {
                    string msg1 = "Found Issues in '" + item.JobSetName + "'.Destination folder required. Cannot proceed.";
                    Global.MessageBoxShow(msg1);
                    return false;
                }
            }
            return true;
        }
    }
}

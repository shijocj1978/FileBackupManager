using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FileManagerBase;

namespace FileManagerLibrary.Base.UIData
{
    [Serializable()]
    public class BackupJobList : Dictionary<Guid, BackupJob>,ISerializable
    {
        public string DataFileVersion { get; set; } // will be used for upgrade the data file versions.
        public BackupJobModeEnum DefaultAddnewJobType { get; set; } // stores value for default add new tab as per user saved last time.
        public string DefaultRootDestinationFolder { get; set; } //stores value for root destination folder as per user saved last time.
        public BackupJobList()
        {
            DefaultAddnewJobType = BackupJobModeEnum.Folders;
            DataFileVersion = "1";
        }

        public BackupJobList(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                DataFileVersion = (string)info.GetValue("DataFileVersion", typeof(string));
            }
            catch
            {
                DataFileVersion = "1";
            }
            int count = (int)info.GetValue("FoldersList_Count", typeof(int));
            for (int i = 0; i < count; i++)
            {
                BackupJob data = (BackupJob)info.GetValue("FoldersList(" + i.ToString() + ")", typeof(BackupJob));
                this.Add(data.JobGUID, data);
            }
            try
            {
                DefaultAddnewJobType = (BackupJobModeEnum)info.GetValue("DefaultAddnewJobType", typeof(BackupJobModeEnum));
            }
            catch
            {
                DefaultAddnewJobType = BackupJobModeEnum.Folders;
            }
            try
            {
                DefaultRootDestinationFolder = (string)info.GetValue("DefaultRootDestinationFolder", typeof(string));
            }
            catch
            {
                DefaultRootDestinationFolder = "";
            }
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FoldersList_Count", this.Count);
            info.AddValue("DataFileVersion", DataFileVersion);
            int i = 0;
            foreach (var item in this.Keys)
            {
                info.AddValue("FoldersList(" + i.ToString() + ")", this[item]);
                i++;
            }
            info.AddValue("DefaultAddnewJobType", DefaultAddnewJobType);
            info.AddValue("DefaultRootDestinationFolder", DefaultRootDestinationFolder);
        }
        #endregion

    }

    [Serializable]
    public class BackupJob : UIDataBase, System.Runtime.Serialization.ISerializable
    {
        public BackupJobModeEnum CurrentBackupJobMode { get; set; }
        public Guid JobGUID { get; private set; }
        public string RootSourceFolder { get; set; }
        public string JobSetName { get; set; }
        public bool Enabled { get; set; }
        public JobFilesOperationModeEnum JobFileOperationMode { get; set; }
        public ServiceIntervalEnum ServiceCheckMode { get; set; }
        public int BackupAfterHours { get; set; }
        public int BackupAfterDays { get; set; }
        public DateTime ServiceTriggerTimeInterval { get; set; }
        public int ServiceTriggerDayInterval { get; set; }
        public string RootBackupFolder { get; set; }

        public FileJobSettings FileSettings { get; set; }
        public FolderJobSettings FolderSettings { get; set; }

        public BackupJob()
        {
            JobGUID = Guid.NewGuid();
            JobSetName = "";
            //Get the data from appsettings and load all of them.
            ServiceTriggerTimeInterval = (DateTime.Now).AddHours(1); //setting default as 1 hours from now, which could be the best time.
            ServiceCheckMode = ServiceIntervalEnum.Daily;
            Enabled = true;
            JobFileOperationMode = JobFilesOperationModeEnum.Copy;
            RootSourceFolder = "";
            RootBackupFolder = "";
            CurrentBackupJobMode = BackupJobModeEnum.Folders;
            FileSettings = new FileJobSettings();
            FolderSettings = new FolderJobSettings();
        }

        public override string ToString()
        {
            return JobSetName;
        }

        private string tostringvar;
       
        public string GetString()
        {
            tostringvar = "";
            AddValue("FolderGUID", JobGUID);
            AddValue("RootFolder", RootSourceFolder);
            AddValue("FolderSetName", JobSetName);
            AddValue("Enabled", Enabled);
            AddValue("FileOperationMode", JobFileOperationMode);
            AddValue("ServiceCheckMode", ServiceCheckMode);
            AddValue("BackupAfterHours", BackupAfterHours);
            AddValue("BackupAfterDays", BackupAfterDays);
            AddValue("ServiceTriggerTimeInterval", ServiceTriggerTimeInterval);
            AddValue("ServiceTriggerDayInterval", ServiceTriggerDayInterval);
            AddValue("BackupJobMode", CurrentBackupJobMode);
            AddValue("FolderSettings", FolderSettings);
            AddValue("FileSettings", FileSettings);
            AddValue("RootBackupFolder", RootBackupFolder);
            return tostringvar;
        }

        #region ISerializable Members

        public string GetSerializableInfo()
        {
            return "";
        }

        protected BackupJob(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("Unexpected error occured during de-serialization");
            try
            { 
                JobGUID = (Guid)info.GetValue("FolderGUID", typeof(Guid)); 
            }
            catch
            {
                JobGUID = Guid.NewGuid(); 
            }
            try
            {
                RootSourceFolder = (string)info.GetValue("RootFolder", typeof(string));
            }
            catch
            {
                RootSourceFolder = ""; 
            }
            try
            {
                JobSetName = (string)info.GetValue("FolderSetName", typeof(string));
            }
            catch
            {
                JobSetName = ""; 
            }
            try
            {
                Enabled = (bool)info.GetValue("Enabled", typeof(bool));
            }
            catch
            {
                Enabled = true; 
            }
            try
            {
                JobFileOperationMode = (JobFilesOperationModeEnum)info.GetValue("FileOperationMode", typeof(JobFilesOperationModeEnum));
            }
            catch
            {
                JobFileOperationMode = JobFilesOperationModeEnum.Copy; 
            }
            try
            {
                ServiceCheckMode = (ServiceIntervalEnum)info.GetValue("ServiceCheckMode", typeof(ServiceIntervalEnum));
            }
            catch
            { 
                ServiceCheckMode = ServiceIntervalEnum.Daily; 
            }
            try
            {
                BackupAfterHours = (int)info.GetValue("BackupAfterHours", typeof(int));
            }
            catch
            {
                BackupAfterHours = 0; 
            }
            try
            {
                BackupAfterDays = (int)info.GetValue("BackupAfterDays", typeof(int));
            }
            catch
            {
                BackupAfterDays = 0;
            }
            try
            {
                ServiceTriggerTimeInterval = (DateTime)info.GetValue("ServiceTriggerTimeInterval", typeof(DateTime));
            }
            catch
            { 
                ServiceTriggerTimeInterval = (DateTime.Now).AddHours(1); 
            }
            try
            {
                ServiceTriggerDayInterval = (int)info.GetValue("ServiceTriggerDayInterval", typeof(int));
            }
            catch
            { 
                ServiceTriggerDayInterval = 0; 
            }
            try
            {
                CurrentBackupJobMode = (BackupJobModeEnum)info.GetValue("BackupJobMode", typeof(BackupJobModeEnum));
            }
            catch
            {
                CurrentBackupJobMode = BackupJobModeEnum.Folders; 
            }
            try
            {
                FileSettings = (FileJobSettings)info.GetValue("FileSettings", typeof(FileJobSettings));
            }
            catch
            { 
                FileSettings = new FileJobSettings();
            }
            try
            {
                FolderSettings = (FolderJobSettings)info.GetValue("FolderSettings", typeof(FolderJobSettings));
            }
            catch
            { 
                FolderSettings = new FolderJobSettings(); 
            }
            try
            {
                RootBackupFolder = (string)info.GetValue("RootBackupFolder", typeof(string));
            }
            catch
            {
                RootBackupFolder = "";
            }
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("Unexpected error occured during serialization");
            info.AddValue("FolderGUID", JobGUID);
            info.AddValue("RootFolder", RootSourceFolder);
            info.AddValue("FolderSetName", JobSetName);
            info.AddValue("Enabled", Enabled);
            info.AddValue("FileOperationMode", JobFileOperationMode);
            info.AddValue("ServiceCheckMode", ServiceCheckMode);
            info.AddValue("BackupAfterHours", BackupAfterHours);
            info.AddValue("BackupAfterDays", BackupAfterDays);
            info.AddValue("ServiceTriggerTimeInterval", ServiceTriggerTimeInterval);
            info.AddValue("ServiceTriggerDayInterval", ServiceTriggerDayInterval);
            info.AddValue("BackupJobMode", CurrentBackupJobMode);
            info.AddValue("FolderSettings", FolderSettings);
            info.AddValue("FileSettings", FileSettings);
            info.AddValue("RootBackupFolder", RootBackupFolder);
        }

        #endregion
    }
}

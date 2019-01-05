using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FileManagerBase;

namespace FileManagerLibrary.Base.UIData
{
    [Serializable]
    public class FolderJobSettings: UIDataBase
    {
        public FolderJobSettings()
        {
            IncludeSubfolders = true;
            ExcludedLocations = new List<string>();
            FileCheckMode = FileCheckModeEnum.ByModifiedDate;
            CleanupSchedule = 0;
            
            IncludeEmptyFolders = false;
        }

        public bool IncludeSubfolders { get; set; }
        public List<string> ExcludedLocations { get; set; }
        public FileCheckModeEnum FileCheckMode { get; set; }
        public int CleanupSchedule { get; set; }
        
        public bool IncludeEmptyFolders { get; set; }

        public override string ToString()
        {
            tostringvar = "";
            AddValue("IncludeSubfolders", IncludeSubfolders);
            AddValue("ExcludedLocations", ExcludedLocations);
            AddValue("CheckFilesByModifiedDate", FileCheckMode);
            AddValue("IncludeEmptyFolders", IncludeEmptyFolders);
            AddValue("CleanupSchedule", CleanupSchedule);
            return tostringvar;
        }

        public string GetSerializableInfo()
        {
            return "";
        }

        protected FolderJobSettings(SerializationInfo info, StreamingContext context)
        {
            try
            {
                IncludeSubfolders = (bool)info.GetValue("IncludeSubfolders", typeof(bool));
            }
            catch
            {
                IncludeSubfolders = true;
            }
            try
            {
                ExcludedLocations = (List<string>)info.GetValue("ExcludedLocations", typeof(List<string>));
            }
            catch
            {
                ExcludedLocations = new List<string>();
            }
            
            try
            {
                IncludeEmptyFolders = (bool)info.GetValue("IncludeEmptyFolders", typeof(bool));
            }
            catch
            {
                IncludeEmptyFolders = false;
            }
            try
            {
                FileCheckMode = (FileCheckModeEnum)info.GetValue("CheckFilesByModifiedDate", typeof(FileCheckModeEnum));
            }
            catch
            {
                FileCheckMode = FileCheckModeEnum.ByModifiedDate;
            }
            try
            {
                CleanupSchedule = (int)info.GetValue("CleanupSchedule", typeof(int));
            }
            catch
            {
                CleanupSchedule = 0;
            }

        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("IncludeSubfolders", IncludeSubfolders);
            info.AddValue("ExcludedLocations", ExcludedLocations);
            info.AddValue("CheckFilesByModifiedDate", FileCheckMode);
            info.AddValue("IncludeEmptyFolders", IncludeEmptyFolders);
            info.AddValue("CleanupSchedule", CleanupSchedule);
        }
    }
}

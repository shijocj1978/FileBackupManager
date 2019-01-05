using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using FileManagerBase;
using FileManagerLibrary.Base.FileBase;
using FileManagerLibrary.Base.Serialization;

namespace FileManagerLibrary.Base.UIData
{


    [Serializable()]
    public class LastRunStatusList : Dictionary<Guid, LastRunStatus>, ISerializable
    {
        private LastRunStatusList()
        { }

        public LastRunStatusList(SerializationInfo info, StreamingContext ctxt)
        {
            int count = (int)info.GetValue("LastRunStatusList_Count", typeof(int));
            for (int i = 0; i < count; i++)
            {
                LastRunStatus data = (LastRunStatus)info.GetValue("LastRunStatus(" + i.ToString() + ")", typeof(LastRunStatus));
                this.Add(data.FolderSetGuid, data);
            }
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LastRunStatusList_Count", this.Count);
            int i = 0;
            foreach (var item in this.Keys)
            {
                info.AddValue("LastRunStatus(" + i.ToString() + ")", this[item]);
                i++;
            }
        }
        #endregion

        public static LastRunStatusList LastRunStatusListInit()
        {
            if (File.Exists(FileOperations.GetAppSettingsFolderRoot() + "\\" + "LastRunStatus.datastore") == false)
            {
                return new LastRunStatusList();
            }
            Serializer<LastRunStatusList> data = new Serializer<LastRunStatusList>();
            LastRunStatusList mgr = data.DeSerializeObject(FileOperations.GetAppSettingsFolderRoot() + "\\" + "LastRunStatus.datastore");
            return mgr;
        }

        public bool SaveData()
        {
            Serializer<LastRunStatusList> data = new Serializer<LastRunStatusList>();
            data.SerializeObject(FileOperations.GetAppSettingsFolderRoot() + "\\" + "LastRunStatus.datastore", this);
            return true;
        }
    }

    [Serializable]
    public class LastRunStatus : System.Runtime.Serialization.ISerializable
    {

        public LastRunStatus(DateTime lastSucessfullRun, Guid folderSetGuid, int lastRunCount)
        {
            LastSucessfullRunTime = lastSucessfullRun;
            FolderSetGuid = folderSetGuid;
            LastRunCount = lastRunCount;
        }

        public LastRunStatus(SerializationInfo info, StreamingContext ctxt)
        {
            FolderSetGuid = (Guid)info.GetValue("FolderSetGuid", typeof(Guid));
            try
            {
                LastSucessfullRunTime = (DateTime)info.GetValue("LastSucessfullRun", typeof(DateTime));
            }
            catch
            {
                LastSucessfullRunTime = DateTime.Now;
            }
            try
            {
                LastRunCount = (int)info.GetValue("LastRunCount", typeof(int));
            }
            catch
            {
                LastRunCount = 0;
            }
            
        }

        public DateTime LastSucessfullRunTime { get; set; }

        public Guid FolderSetGuid { get; set; }

        /// <summary>
        /// Stores the values of last run count so that it can be used for cleanup destination if set to some iterations.
        /// </summary>
        public int LastRunCount { get; set; }


        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("Unexpected error occured during serialization");
            info.AddValue("FolderSetGuid", FolderSetGuid);
            info.AddValue("LastSucessfullRun", LastSucessfullRunTime);
            info.AddValue("LastRunCount", LastRunCount);

        }
    }
}

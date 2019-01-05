using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FileManagerBase;

namespace FileManagerLibrary.Base.UIData
{
    [Serializable]
    public class FileJobSettings: UIDataBase
    {

        public List<string> FileNames { get; set; }
        public bool FileSizeCheckEnabled { get; set; }
        public int MaxFileSize { get; set; }
        public FileSizeTypeEnum MaxFileSizeType { get; set; }
        public int WaitSecondsOnFileChange { get; set; }
        public int MaxVersions { get; set; }

        public FileJobSettings()
        {
            FileNames = new List<string>();
            MaxFileSizeType = FileSizeTypeEnum.MB;
        }

        
        public override string ToString()
        {
            tostringvar = "";
            AddValue("FileSizeCheckEnabled", FileSizeCheckEnabled);
            AddValue("MaxFileSize", MaxFileSize);
            AddValue("MaxFileSizeType", MaxFileSizeType);
            AddValue("WaitSecondsOnFileChange", WaitSecondsOnFileChange);
            AddValue("MaxVersions", MaxVersions);
            return tostringvar;
        }

        public string GetSerializableInfo()
        {
            return "";
        }

        protected FileJobSettings(SerializationInfo info, StreamingContext context)
        {
            try
            {
                FileNames = (List<string>)info.GetValue("FileNames", typeof(List<string>));
            }
            catch
            {
                FileNames = new List<string>();
            }
            try
            {
                FileSizeCheckEnabled = (bool)info.GetValue("FileNames", typeof(bool));
            }
            catch
            {
                FileSizeCheckEnabled = false;
            }
            try
            {
                MaxFileSize = (int)info.GetValue("MaxFileSize", typeof(int));
            }
            catch
            {
                MaxFileSize = 0;
            }
            try
            {
                MaxFileSizeType = (FileSizeTypeEnum)info.GetValue("MaxFileSizeType", typeof(FileSizeTypeEnum));
            }
            catch
            {
                MaxFileSizeType = FileSizeTypeEnum.MB;
            }
            try
            {
                WaitSecondsOnFileChange = (int)info.GetValue("FileNames", typeof(double));
            }
            catch
            {
                WaitSecondsOnFileChange = 0;
            }
            try
            {
                MaxVersions = (int)info.GetValue("MaxVersions", typeof(int));
            }
            catch
            {
                MaxVersions = 0;
            }
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("FileNames", FileNames);
            info.AddValue("FileSizeCheckEnabled", FileSizeCheckEnabled);
            info.AddValue("MaxFileSize", MaxFileSize);
            info.AddValue("MaxFileSizeType", MaxFileSizeType);
            info.AddValue("WaitSecondsOnFileChange", WaitSecondsOnFileChange);
            info.AddValue("MaxVersions", MaxVersions);
        }
    }
}

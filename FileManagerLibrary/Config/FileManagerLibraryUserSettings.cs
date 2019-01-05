using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace FileManagerLibrary.Config
{
    public class FileManagerLibraryUserSettingsConfiguration : ConfigurationSection
    {
        public FileManagerLibraryUserSettingsConfiguration()
        {}

        [ConfigurationProperty("UserSettings", DefaultValue = null, IsRequired = true)]
        [ConfigurationCollection(typeof(FileManagerLibraryUserSettingsCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public FileManagerLibraryUserSettingsCollection SettingsList 
        {
            get
            {
                return (FileManagerLibraryUserSettingsCollection)base["UserSettings"];
            }
        }
    }

    public class FileManagerLibraryUserSettingsCollection : ConfigurationElementCollection
    {
        public FileManagerLibraryUserSettingsCollection()
        { }

        public FileManagerLibraryUserSettingsCollection this[int index]
        {
            get { return (FileManagerLibraryUserSettingsCollection)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new FileManagerLibraryUserSettings this[string userName]
        {
            get
            {
                for (int i = 0; i < BaseGetAllKeys().Length; i++)
                {
                    if (((FileManagerLibraryUserSettings)BaseGet(i)).UserName.ToUpper() == userName.ToUpper())
                        return (FileManagerLibraryUserSettings)BaseGet(i);
                }
                return null;
            }
            set
            {
                BaseAdd(value);
            }
        }

        public void Add(FileManagerLibraryUserSettings setting)
        {
            BaseAdd(setting);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FileManagerLibraryUserSettings();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileManagerLibraryUserSettings)element).UserName;
        }

        public void Remove(FileManagerLibraryUserSettings settings)
        {
            BaseRemove(settings.UserName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string userName)
        {
            BaseRemove(userName);
        }
    }

    public class FileManagerLibraryUserSettings : ConfigurationElement
    {
        public FileManagerLibraryUserSettings()
        { }
        public FileManagerLibraryUserSettings(string _UserName, UserSettingsDetails _SettingsDetails)
        {
            UserName = _UserName;
            SettingsDetails = _SettingsDetails;
        }

        [ConfigurationProperty("UserName", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string UserName
        {
            get { return (string)this["UserName"]; }
            set { this["UserName"] = value; }
        }

        [ConfigurationProperty("Settings")]
        public UserSettingsDetails SettingsDetails
        {
            get { return (UserSettingsDetails)this["Settings"]; }
            set { this["Settings"] = value; }
        }
    }

    public class UserSettingsDetails : ConfigurationElement
    {
        [ConfigurationProperty("AllowPerUserSettings", DefaultValue = false, IsRequired = true, IsKey = false)]
        public bool AllowPerUserSettings
        {
            get { return (bool)this["AllowPerUserSettings"]; }
            set { this["AllowPerUserSettings"] = value; }
        }

        [ConfigurationProperty("OverWriteUnChangedFiles", DefaultValue = false, IsRequired = true, IsKey = false)]
        public bool OverWriteUnChangedFiles
        {
            get { return (bool)this["OverWriteUnChangedFiles"]; }
            set { this["OverWriteUnChangedFiles"] = value; }
        }

        [ConfigurationProperty("AppSettingsNetworkLocation", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string AppSettingsNetworkLocation
        {
            get { return (string)this["AppSettingsNetworkLocation"]; }
            set { this["AppSettingsNetworkLocation"] = value; }
        }

        //FileLockoutWaitTimeOutSeconds
        [ConfigurationProperty("FileLockoutWaitTimeOutSeconds", DefaultValue = 0, IsRequired = true, IsKey = false)]
        public int FileLockoutWaitTimeOutSeconds
        {
            get { return (int)this["FileLockoutWaitTimeOutSeconds"]; }
            set { this["FileLockoutWaitTimeOutSeconds"] = value; }
        }

        [ConfigurationProperty("WindowsServiceName", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string WindowsServiceName
        {
            get { return (string)this["WindowsServiceName"]; }
            set { this["WindowsServiceName"] = value; }
        }

        [ConfigurationProperty("WindowsServiceDisplayName", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string WindowsServiceDisplayName
        {
            get { return (string)this["WindowsServiceDisplayName"]; }
            set { this["WindowsServiceDisplayName"] = value; }
        }

        [ConfigurationProperty("WindowsServiceDescription", DefaultValue = "", IsRequired = true, IsKey = false)]
        public string WindowsServiceDescription
        {
            get { return (string)this["WindowsServiceDescription"]; }
            set { this["WindowsServiceDescription"] = value; }
        }

    }
}

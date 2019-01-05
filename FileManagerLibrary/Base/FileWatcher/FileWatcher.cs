using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FileManagerLibrary.Base.FileBase;
using System.Threading;
using Logging;

namespace FileManagerLibrary.Base.FileWatcher
{
    public class JobFileWatcher
    {
        /* HANDLING LOGIC
         * We are not considering sub folder for watching.
         * We will create seperate watchers for each folder.
         * Default buffer for each file watcher could be configured from App config. Which should be multiples of 1024.
         * If we have more than 1000 files in one folder then split based on number of extensions.
         * If we have more than 1000 files of same extension in one folder then increase buffer by 8KB for each 500 files.
         * 
         * FileChangeWatcherBuffer ="8192" - change this value if you are having too much of files 
         * in folders of 'When Files Changed option'. 
         * Also increase buffer when you have longer file names or paths.
         */

        /// <summary>
        /// UID for JobFileWatcher. Created when the object is created.
        /// </summary>
        public Guid JobFileWatcherGuid { get; private set; }

        /// <summary>
        /// GUID of the Job which the watcher is created for. will get this value through constructor
        /// </summary>
        public Guid JobGuid { get; set; }

        /// <summary>
        /// Job wait time which for the watcher. will get this value through constructor
        /// </summary>
        public int TriggerWaitTime { get; set; }

        /// <summary>
        /// Get Watchers based on Folders. Guid is connected to  FileMetadata.
        /// </summary>
        public Dictionary<Guid,FileSystemWatcher> SubWatchers {get; private set;}

        /// <summary>
        /// Gets information regarding the watcher which the file belongs to.
        /// First string is File Path and second string is folder path Guid.
        /// </summary>
        public Dictionary<string,FileMetadata> FilesMapping { get; private set; }

        public event EventHandler<GenericEventArgsType<FileChangedEventArgsData>> FileChanged;

        public JobFileWatcher(List<string> files,Guid JobGuid,int triggerwaittime)
        {
            SubWatchers = new Dictionary<Guid, FileSystemWatcher>();
            FilesMapping = new Dictionary<string, FileMetadata>();
            JobFileWatcherGuid = Guid.NewGuid();
            CreateWatchers(files);
            this.TriggerWaitTime = triggerwaittime;
            this.JobGuid = JobGuid;
        }

        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set 
            {
                foreach (var item in SubWatchers)
                {
                    item.Value.EnableRaisingEvents = value;
                }
                _Enabled = value;
            }
        }

        bool CreateWatchers(List<string> files)
        {
            foreach (var item in files)
            {
                FileMetadata st = RetriveFolderMetadata(item); //Creates a file metadata object to use everywhere from here on. 
                if (SubWatchers.ContainsKey(st.SubWatchersGuid) == false) //find if a watcher is created for current folder
                {
                    if (FileOperations.GetFilesCountInDirectory(st.FolderName) > 1000) //if folder got more than 1000 files then need more filter.
                    {
                        Int64 count = FileOperations.GetFilesCountInDirectory(st.FolderName, st.Extension);
                        if (count > 1000) //if count of one extension itself is more than 1000, need additioanl buffer based on count.
                        {
                            int multiple = (int)(count / 1000);
                            FileSystemWatcher newwatcher = CreateWatcher(st.FolderName, "*" + st.Extension);
                            newwatcher.InternalBufferSize = multiple * 8192;
                            FilesMapping.Add(item, st);
                            SubWatchers.Add(st.SubWatchersGuid, newwatcher);
                        }
                        else //create a watcher based on this filter extension.
                        {
                            FilesMapping.Add(item, st);
                            SubWatchers.Add(st.SubWatchersGuid, CreateWatcher(st.FolderName, "*" + st.Extension));
                        }
                    }
                    else
                    {
                        FilesMapping.Add(item, st);
                        st.Extension = ""; //this will be set in GetFoldersStruct when created by default. But no need of this if the files count is less thatn 1000.
                        SubWatchers.Add(st.SubWatchersGuid, CreateWatcher(st.FolderName, st.Extension));
                    }
                }
                else
                {
                    FilesMapping.Add(item, st);
                    //No action required as the watcher is already added and if the count of files is more than 1000 then to specific extension based watcher.
                    //the same case with FolderMapping also. The FolderMapping count must be the same as watcher count
                }
            }
            return true;
        }

        FileMetadata RetriveFolderMetadata(string filename)
        {
            string folder = GetContainingFolder(filename);
            string ext = Path.GetExtension(filename);
            var tmp = from t in FilesMapping
                      where
                          t.Value.FolderName == folder && t.Value.Extension == "" ||
                          t.Value.FolderName == folder && t.Value.Extension == ext
                      select t.Value;
            if (tmp.Count<FileMetadata>() != 0)
                return new FileMetadata(folder, ext, filename, JobFileWatcherGuid, tmp.First<FileMetadata>().SubWatchersGuid);
            else
                return new FileMetadata(folder, ext, filename, JobFileWatcherGuid, Guid.NewGuid()); //this newGuid will be used to add new Watcher object.
        }

        string GetContainingFolder(string filename)
        {
            return Path.GetDirectoryName(filename);
        }

        FileSystemWatcher CreateWatcher(string Directory,string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Directory);
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Changed);
            watcher.Filter = filter;
            return watcher;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
                    //ThreadPool.QueueUserWorkItem(ProcessChangeAsync,e.FullPath);
                    ProcessChangeAsync(e.FullPath);
            }
            catch(Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error occured during Filechanged.");
            }
        }

        void ProcessChangeAsync(object fullPath)
        {
            if (FilesMapping.ContainsKey((string)fullPath) == true)
            {
                FileMetadata strt = FilesMapping[(string)fullPath];
                FileChangedEventArgsData tmp = new FileChangedEventArgsData(strt, TriggerWaitTime, JobGuid);
                if(FileChanged!=null)
                    FileChanged(this, new GenericEventArgsType<FileChangedEventArgsData>(tmp));
            }
        }
    }

    public class FileMetadata
    {
        public string FolderName { get; set; }
        public string Extension { get; set; }
        public string FullFileName { get; set; }
        public Guid JobFileWatcherGuid { get; set; }
        public Guid FileMetadataGuid { get; set; }
        public Guid SubWatchersGuid { get; set; }
        public FileMetadata(string foldername, string extension, string fullfilename, Guid jobfilewatcherguid, Guid subwatchersguid)
        {
            this.FolderName = foldername;
            this.Extension = extension;
            this.FullFileName = fullfilename;
            this.JobFileWatcherGuid = jobfilewatcherguid;
            this.FileMetadataGuid = Guid.NewGuid();
            this.SubWatchersGuid = subwatchersguid;
        }
    }

    public class FileChangedEventArgsData : FileMetadata
    {
        public int TriggerWaitTime { get; set; }
        public Guid JobGuid { get; set; }
        public DateTime FileChangedTime { get; set; }

        public FileChangedEventArgsData(FileMetadata folder, int triggerwaittime, Guid jobguid): base(folder.FolderName,folder.Extension,folder.FullFileName,folder.JobFileWatcherGuid,folder.SubWatchersGuid)
        {
            TriggerWaitTime = triggerwaittime;
            FileChangedTime = DateTime.Now;
            JobGuid = jobguid;
        }
    }
}

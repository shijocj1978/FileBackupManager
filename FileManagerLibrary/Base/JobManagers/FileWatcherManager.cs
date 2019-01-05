using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FileManagerLibrary.Base.UIData;
using FileManagerLibrary.Base.FileWatcher;
using System.Threading;
using Logging;
using FileManagerLibrary.Base.FileBase;
using System.Diagnostics;

namespace FileManagerLibrary.Base.JobManagers
{
    public class FileWatcherManager
    {
        JobController mgr;
        Dictionary<Guid, JobFileWatcher> WatcherGroups { get; set; }

        /* This copy is done here to keep the performance for other apps good. 
         * If this is created inside watchers then it will result in multiple file copy 
         * at same time which may add more load to system */
        /// <summary>
        /// Datetime = time at which the job is supposed to start
        /// FileChangedEventArgsData contains all information regarding the job. 
        /// </summary>
        SortedList<DateTime,FileChangedEventArgsData> FilesQueue { get; set; }

        FileChangedEventArgsData CurrentJob { get; set; }

        bool IsJobAddInProgress { get; set; }

        System.Threading.Timer timer;

        public FileWatcherManager()
        {
            WatcherGroups = new Dictionary<Guid, JobFileWatcher>();
            FilesQueue = new SortedList<DateTime, FileChangedEventArgsData>();
            mgr = JobController.Init();
            LoadWatchers();
        }

        public bool RefreshAllWatchers()
        {
            mgr = JobController.Init();
            bool tmp = WatcherEnabled;
            WatcherEnabled = false;
            LoadWatchers();
            WatcherEnabled = tmp; //start monitoring if it was running before.
            return true;
        }

        public bool LoadWatchers()
        {
            WatcherGroups.Clear();
            foreach (var item in mgr.FolderData)
            {
                if (item.Value.Enabled == true && item.Value.CurrentBackupJobMode == FileManagerBase.BackupJobModeEnum.Files && item.Value.ServiceCheckMode == FileManagerBase.ServiceIntervalEnum.OnFileUpdated)
                {
                    AddFileWatcher(item.Value);
                }
            }
            return true;
        }

        /// <summary>
        /// Adds a new Filewacher object to watcher list. If the watcher is enabled then this instance will also be enabled.
        /// </summary>
        /// <param name="data">BackupJob</param>
        /// <returns></returns>
        public bool AddFileWatcher(BackupJob data)
        {
            JobFileWatcher watcher = new JobFileWatcher(data.FileSettings.FileNames, data.JobGUID,data.FileSettings.WaitSecondsOnFileChange);
            watcher.FileChanged += new EventHandler<GenericEventArgsType<FileChangedEventArgsData>>(watcher_FileChanged);
            WatcherGroups.Add(watcher.JobGuid,watcher);
            if (WatcherEnabled == true)
                watcher.Enabled = true;
            return true;
        }

        /// <summary>
        /// Triggers when any of the file in any of the watcher is modified.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_FileChanged(object sender, GenericEventArgsType<FileChangedEventArgsData> e)
        {
            IsJobAddInProgress = true; //start adding to the queue
            FileChangedEventArgsData currentdata = GetValueIfExisting(e.Data);
            /* removes a change if it occured before 500 milliseonds as this could be a duplicate, 
             * and this change will be included in the next copy as 500 milliseonds are added to wait
             * This is to fix a bug in Filesystem watcher which will fires events multiple times on single change.
             * Ref: http://weblogs.asp.net/ashben/archive/2003/10/14/31773.aspx 
             */
            if (IsFileChangedBefore500Milliseconds(currentdata, e.Data) == false)
            {
                IsJobAddInProgress = false;
                return;
            }
            /* removes the currrent from queue and put it to a upper location so that this 
             * file opertation will start only after the buffer time specified in UI.
             */
            if (currentdata != null)
            {
                FilesQueue.RemoveAt(FilesQueue.IndexOfValue(currentdata));
                FilesQueue.Add(GetTriggerTime(e.Data.TriggerWaitTime), e.Data);
            }
            else
                FilesQueue.Add(GetTriggerTime(e.Data.TriggerWaitTime), e.Data);
            /* Special Case
             * Once the adding of new job is completed, if the currentjob is having 
             * more wait time than the newly added job(new job gets into the topmost of the queue)
             * then reset timer and add the new job to queue.
             */
            if (FilesQueue.Count > 0 && FilesQueue[FilesQueue.Keys[0]].JobGuid == e.Data.JobGuid)
            {
                if (timer != null)
                    timer.Dispose(); //the existing timer will be having the timer on and will trigger if not collected by GC. So need to dispose that manualy.
                timer = null;
                CurrentJob = null;
            }
            if (timer == null)
            {
                IsJobAddInProgress = false;
                ScheduleNextJob();
                return;
            }
            IsJobAddInProgress = false;
        }

        private bool IsFileChangedBefore500Milliseconds(FileChangedEventArgsData datainqueue, FileChangedEventArgsData newdata)
        {
            if (datainqueue == null)
            {
                if (CurrentJob == null)
                    return true;
                /* If the file not found in jobqueue then, also check in CurrentJob 
                 * to find it is the same file.
                 * If yes then need to check it got changed before 500 millseonds
                 * so that the current change need to be skipped. If not then it is 
                 * considered as a seperate change and will be backuped again.
                 */
                if (CurrentJob.FullFileName == newdata.FullFileName) 
                {
                    if (CurrentJob.FileChangedTime.AddMilliseconds(500) > DateTime.Now) //if filechanged before FileChangedTime + 500mls 
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            else if (datainqueue.FileChangedTime.AddMilliseconds(500) > DateTime.Now) // Check file got changed before 500 millseonds so that the current change need to be skipped. If not then it is considered as a seperate change and will be backuped again. */
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the existing value if it is ther in the watcher list already.
        /// </summary>
        /// <param name="newval"></param>
        /// <returns></returns>
        private FileChangedEventArgsData GetValueIfExisting(FileChangedEventArgsData newval)
        {
            FileChangedEventArgsData current = null;
            if (FilesQueue.Keys.Count == 0)
                return current;
            var temp = (from t in FilesQueue where t.Value.FullFileName == newval.FullFileName select t.Value);
            if (temp.Count<FileChangedEventArgsData>() == 0)
                return current;
            else
                return temp.First<FileChangedEventArgsData>();
        }

        public bool RemoveFileWatcher(BackupJob data)
        {
            if (WatcherGroups.ContainsKey(data.JobGUID) == true)
            {
                WatcherGroups[data.JobGUID].Enabled = false;
                WatcherGroups.Remove(data.JobGUID);
            }
            return true;
        }

        private bool _enabled;
        public bool WatcherEnabled
        {
            get { return _enabled; }

            set 
            {
                foreach (var item in WatcherGroups)
                {
                    item.Value.Enabled = value;
                }
                _enabled = value; 
            }
        }

        private DateTime GetTriggerTime(int waitminutes)
        {
            try
            {
                DateTime triggertime = DateTime.Now;
                triggertime = triggertime.AddMinutes((double)waitminutes);
                while (FilesQueue.Keys.Contains(triggertime) == true) //avoid adding duplicates in list.
                    triggertime = triggertime.AddMilliseconds(1);
                return triggertime;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex,"Error on GetTriggerTime.");
                return DateTime.Now;
            }
        }

        private bool ScheduleNextJob()
        {
            if (FilesQueue.Count == 0)
                return true;
            // if a new job is getting added then there is a chance for conflict and so wait until it is completed. Setting timer to null so that after that the add itself will call ScheduleNextJob()
            if (IsJobAddInProgress == true) 
            {
                if(timer!=null)
                    timer.Dispose(); //the existing timer will be having the timer on and will trigger if not collected by GC. So need to dispose that manualy.
                timer = null;
                return true;
            }
            DateTime runtime = FilesQueue.Keys[0];
            CurrentJob = FilesQueue[runtime];
            TimeSpan diff = runtime.Subtract(DateTime.Now);
            if (diff.TotalMilliseconds <= 0)
                diff = new TimeSpan(); //reset the timspan to 0 because this item is supposed to run and so it is late. need to now ASAP.
            try
            {
                Logger.StaticLogger.AddDebugMessagesEntry("runtime " + runtime.ToString() + ", now" + DateTime.Now.ToString() + ", timespan" + diff);
                timer = new System.Threading.Timer(new System.Threading.TimerCallback(
                   (x) =>
                   {
                       FilesQueue.Remove(runtime);
                       PerformJob(CurrentJob);
                       //set the timer to start on next interval specifed interval in file queue.
                       if (FilesQueue.Count == 0)
                       {
                           LastRunStatusList sts = LastRunStatusList.LastRunStatusListInit();
                           if (sts.ContainsKey(CurrentJob.JobGuid))
                               sts[CurrentJob.JobGuid].LastSucessfullRunTime = DateTime.Now;
                           else
                           {
                               sts.Add(CurrentJob.JobGuid,new LastRunStatus(DateTime.Now,CurrentJob.JobGuid,0));
                           }
                           sts.SaveData();
                           timer = null;
                       }
                       else
                           ScheduleNextJob();
                   })
                   , null,diff,new TimeSpan(-1));
                return true;
            }
            catch (Exception ex)
            {
                Logger.StaticLogger.AddErrorLogEntry(ex, "Error in ScheduleNextJob, FileName: " + CurrentJob.FullFileName + ", Job Name: " + mgr.FolderData[CurrentJob.JobGuid].JobSetName);
                ScheduleNextJob();
                return false;
            }
        }

        private bool PerformJob(FileChangedEventArgsData data)
        {
            FileSystemOperations floper = new FileSystemOperations();
            return floper.PerformSingleFileBackup(mgr.FolderData[data.JobGuid], data.FullFileName);
        }
    }
}

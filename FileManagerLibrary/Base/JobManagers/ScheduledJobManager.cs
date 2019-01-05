using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileManagerLibrary;
using System.IO;
using FileManagerLibrary.Base;
using Logging;
using FileManagerBase;
using Logging.Framework;
using FileManagerLibrary.Base.UIData;
using System.Threading;
using FileManagerLibrary.Base.FileBase;
using FileManagerLibrary.Base.FileWatcher;

namespace FileManagerLibrary.Base.JobManagers
{
    public class ScheduledJobManager
    {
        JobController mgr;
        public LastRunStatusList LastRunStatus { get; private set; }

        System.Threading.Timer timer;
        int startup, interval;
        /*For making the time service trigger for a different first time change this value. 
         * But there is a chance that next trigger may fail and cause windows 
         * exception because the value will get -1 on next call*/
        int PRODUCTION_ROUNDUPINTERVAL = 60;
        bool production;
        bool firsttimetrigger;

        public ScheduledJobManager()
        {
            mgr = JobController.Init();
            LastRunStatus = LastRunStatusList.LastRunStatusListInit();
        }

        public void StartService()
        {
            production = true;
            firsttimetrigger = true;
            StartService((PRODUCTION_ROUNDUPINTERVAL - DateTime.Now.Minute), 60);
        }

        public void StartService(int startafterminutes, int intervalminutes)
        {
            mgr = JobController.Init();
            startup = startafterminutes;
            interval = intervalminutes;
            if(firsttimetrigger == true)
            {
                Logger.StaticLogger.AddAuditLogEntry("First Trigger after " + (PRODUCTION_ROUNDUPINTERVAL - DateTime.Now.Minute).ToString() + " Minutes, Next Triggers on Every " + intervalminutes.ToString() + " minutes.");
                firsttimetrigger = false;
            }
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(
                (x) => 
                {
                    try
                    {
                        StopService();
                        mgr = JobController.Init();
                        LastRunStatus = LastRunStatusList.LastRunStatusListInit();
                        Trigger();
                    }
                    catch (Exception ex)
                    {
                        Logger.StaticLogger.AddErrorLogEntry(ex);
                    }
                    finally
                    {
                        if (production == true)
                            StartService((PRODUCTION_ROUNDUPINTERVAL - DateTime.Now.Minute), 60);
                        else
                            StartService(startup, interval);
                    }
                })
                , null, (startafterminutes * 60) * 1000, (intervalminutes * 60) * 1000);
        }

        public void StopService()
        {
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        public event EventHandler<GenericEventArgsType<bool>> OperationCompleted;

        public void TriggerAsyc(object value)
        {
            Trigger();
            if (OperationCompleted != null)
                OperationCompleted(this,new GenericEventArgsType<bool>(true));
        }

        public bool Trigger()
        {
            try
            {
                if (mgr.ValidateGlobalFolderData() == false)
                    return false;
                foreach (var item in mgr.FolderData)
                {
                    if (LastRunStatus.ContainsKey(item.Value.JobGUID) == false)
                        LastRunStatus.Add(item.Value.JobGUID, new FileManagerLibrary.Base.UIData.LastRunStatus(DateTime.MinValue, item.Value.JobGUID,0));
                    BackupJob currentjob = item.Value;
                    if (currentjob.Enabled == false)
                        continue;
                    if (mgr.ValidateFolderData(currentjob) == false)
                        continue;
                    switch (currentjob.ServiceCheckMode)
                    {
                        case FileManagerBase.ServiceIntervalEnum.Monthly:
                            if (LastRunCompletedSucessfully(currentjob) == false)
                            {
                                InitiateBackupandSync(currentjob, LastRunStatus[currentjob.JobGUID],"MONTHLY BACKUP");
                            }
                            else if (DateTime.Now.Day == currentjob.ServiceTriggerDayInterval)
                            {
                                if (DateTime.Now.Hour == currentjob.ServiceTriggerTimeInterval.Hour)
                                {
                                    InitiateBackupandSync(currentjob, LastRunStatus[currentjob.JobGUID],"MONTHLY");
                                }
                            }
                            break;
                        case FileManagerBase.ServiceIntervalEnum.Weekly:
                            if (LastRunCompletedSucessfully(currentjob) == false)
                            {
                                InitiateBackupandSync(currentjob, LastRunStatus[currentjob.JobGUID],"CATCHUP WEEKLY");
                            }
                            else if (((int)DateTime.Now.DayOfWeek) + 1 == currentjob.ServiceTriggerDayInterval)
                            {
                                if (DateTime.Now.Hour == currentjob.ServiceTriggerTimeInterval.Hour)
                                {
                                    InitiateBackupandSync(currentjob, LastRunStatus[currentjob.JobGUID],"WEEKLY");
                                }
                            }
                            break;
                        case FileManagerBase.ServiceIntervalEnum.Daily:
                            if (LastRunCompletedSucessfully(currentjob) == false)
                            {
                                InitiateBackupandSync(currentjob, LastRunStatus[currentjob.JobGUID],"CATCHUP DAILY");
                            }
                            else if (DateTime.Now.Hour == currentjob.ServiceTriggerTimeInterval.Hour)
                            {
                                InitiateBackupandSync(currentjob, LastRunStatus[currentjob.JobGUID],"DAILY");
                            }
                            break;
                        case FileManagerBase.ServiceIntervalEnum.OnFileUpdated:
                            //Reminder place holder. This will be handled in FileMonitor Service.
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex, "Trigger()");
                return false;
            }
            finally
            {
                LastRunStatus.SaveData();
            }
        }

        private bool LastRunCompletedSucessfully(BackupJob currentjob)
        {
            DateTime Lastruntime = LastRunStatus[currentjob.JobGUID].LastSucessfullRunTime;
            if (currentjob.ServiceCheckMode == ServiceIntervalEnum.Daily)
            {
                if (DateTime.Now.Subtract(Lastruntime).Days > 1)
                    return false;
            }
            else if (currentjob.ServiceCheckMode == ServiceIntervalEnum.Monthly)
            {
                if (DateTime.Now.Subtract(Lastruntime).Days > 30)
                    return false;
            }
            else if (currentjob.ServiceCheckMode == ServiceIntervalEnum.Weekly)
            {
                if (DateTime.Now.Subtract(Lastruntime).Days > 7)
                    return false;
            }
            return true;
        }

        public bool IsJobLastRunMissed()
        {
            foreach (var item in mgr.FolderData)
            {
                if (item.Value.CurrentBackupJobMode == BackupJobModeEnum.Files && item.Value.ServiceCheckMode == ServiceIntervalEnum.OnFileUpdated)
                    continue;
                if (LastRunStatus.ContainsKey(item.Value.JobGUID) == false)
                {
                    LastRunStatus.Add(item.Value.JobGUID, new FileManagerLibrary.Base.UIData.LastRunStatus(DateTime.MinValue, item.Value.JobGUID,0));
                    return true;
                }
                else
                    if (LastRunCompletedSucessfully(item.Value) == false)
                        return true;
            }
            LastRunStatus.SaveData();
            return false;
        }

        void InitiateBackupandSync(BackupJob currentjob, LastRunStatus lastRunStatus, string logMessage)
        {
            Logger.StaticLogger.AddAuditLogEntry("PERFORMING " + logMessage + " BACKUP FOR " + currentjob.JobSetName);
            PerformBackup(currentjob);
            Logger.StaticLogger.AddAuditLogEntry("COMPLETED " + logMessage + " BACKUP FOR " + currentjob.JobSetName);
            lastRunStatus.LastSucessfullRunTime = DateTime.Now;
            /* prevents job to perform the cleanup operation for jobs in Move or Delete or in files mode.
             * allowing this will delete files from both source and destination, which is critical data loss issue.*/
            if (currentjob.CurrentBackupJobMode == BackupJobModeEnum.Folders && currentjob.JobFileOperationMode == JobFilesOperationModeEnum.Copy && currentjob.FolderSettings.CleanupSchedule > 0)
            {
                lastRunStatus.LastRunCount++;
                if (lastRunStatus.LastRunCount > currentjob.FolderSettings.CleanupSchedule)
                {
                    FileSystemOperations flobj = new FileSystemOperations();
                    flobj.OperationCompleted += (x, y) =>
                    {
                        if (y.Data == true)
                        {
                            lastRunStatus.LastRunCount = 0;
                            Logger.StaticLogger.AddAuditLogEntry("COMPLETED" + logMessage + " CLEANUP FOR " + currentjob.JobSetName);
                        }
                        else
                            Logger.StaticLogger.AddAuditLogEntry("COMPLETED (WITH ERROR) ON " + logMessage + " CLEANUP FOR " + currentjob.JobSetName);
                    };
                    Logger.StaticLogger.AddAuditLogEntry("PERFORMING " + logMessage + " CLEANUP FOR " + currentjob.JobSetName);
                    ThreadPool.QueueUserWorkItem(flobj.PerformCleanupDestinationAsyc, currentjob);
                }
            }
        }

        void PerformBackup(BackupJob backupjob)
        {
            try
            {
                string rootbackupdestinationfolder = FileOperations.GetRootBackupDestinationFolder(backupjob);
                Logger.StaticLogger.AddAuditLogEntry(
                    "VALUES:: Backup OperationMode: " + backupjob.CurrentBackupJobMode.ToString() + 
                    ", FileCopyMode: " + backupjob.JobFileOperationMode +
                    ", RootSourceFolder: " + backupjob.RootSourceFolder +
                    ", RootBackupDestinationFolder: " + rootbackupdestinationfolder +
                    ", ExcludedLocations: " + GetLocationList(backupjob.FolderSettings.ExcludedLocations) +
                    ", BackupAfterDays: " + backupjob.BackupAfterDays.ToString() +
                    ", BackupAfterHours: " + backupjob.BackupAfterHours.ToString() +
                    ", DeleteEmptyFolders: " + backupjob.FolderSettings.IncludeEmptyFolders.ToString());
                if (backupjob.CurrentBackupJobMode == BackupJobModeEnum.Folders)
                {
                    FileSystemOperations obj = new FileSystemOperations();
                    obj.PerformFolderBackup(backupjob);
                }
                else if (backupjob.CurrentBackupJobMode == BackupJobModeEnum.Files)
                {
                    FileSystemOperations obj = new FileSystemOperations();
                    obj.PerformFilesBackup(backupjob);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex,"PerformBackup(Folders currentfolder)");
                return;
            }
        }

        string GetLocationList(List<string> list)
        {
            try
            {
                string var = "";
                foreach (var item in list)
                {
                    var = var + item + ", ";
                }
                return var;
            }
            catch (Exception ex)
            {
                Logging.Logger.StaticLogger.AddErrorLogEntry(ex,"GetLocationList");
                return "";
            }
        }

        public void StopTimer()
        {
            StopService();
        }
    }
}

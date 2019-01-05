using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FileManagerBase
{
    public enum ServiceIntervalEnum
    {
        Monthly,
        Weekly,
        Daily,
        OnFileUpdated
    }

    public enum JobFilesOperationModeEnum
    {
        Move,
        Copy,
        Delete
    }

    public enum FileCheckModeEnum
    {
        ByModifiedDate,
        ByCreatedDate,
        ByAccessedDate
    }

    public enum BackupJobModeEnum
    {
        Folders,
        Files
    }

    public enum FileSizeTypeEnum
    {
        KB,
        MB,
        GB,
        TB
    }

    public enum WindowsServiceInstallStatus
    {
        NotInstalled ,
        Running,
        Stopped,
        StatusChangeInProgress,
        ExceptionOccured
    }
}

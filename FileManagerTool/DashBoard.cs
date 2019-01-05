using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileManagerBase;
using FileManagerLibrary.Base;
using Logging;
using System.Threading;
using FileManagerLibrary.Base.JobManagers;
using FileManagerLibrary.Base.FileBase;
using FileManagerLibrary.Base.UIData;
using FileManagerLibrary.Base.WindowsService;
using FileManagerLibrary;
using System.Runtime.InteropServices;
using System.Reflection;

namespace FileManagerTool
{
    public partial class DashBoard : Form
    {
        JobController mgr = JobController.Init();
        BackupJob CurrentJob = null;
        bool IsEditmode;
        bool IsUILoading;
        
        /*If a background process is in progress, it should be notified to whole application 
         * so that it can be used in other operations like prevent close the application etc.*/
        bool IsAsycWorkInProgress;

        public DashBoard()
        {
            /*this is for btnBckUpNow_Click -> btnBckUpNow.Enabled = true;*/
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.Text = Text + " - " + FileManagerLibrary.AppSettings.ActiveUserName;
            IntPtr MenuHandle = GetSystemMenu(this.Handle, false);
            InsertMenu(MenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty); // Add a menu seperator in title bar
            InsertMenu(MenuHandle, 6, MF_BYPOSITION, MYMENU1, "Open Settings Location"); // Add a menu location to Open Appsettings location
            if (WindowsServiceInstallHelper.CheckAndInstallWindowsService() == true)
            {
                InsertMenu(MenuHandle, 7, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty); // Add a menu seperator in title bar
                InsertMenu(MenuHandle, 8, MF_BYPOSITION, MYMENU3, "Unistall Windows Service"); // If windows service is installed add a menu to help user to unistall it if required.                
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            IsUILoading = true;
            BackupJob data = new BackupJob();
            data.RootBackupFolder = mgr.FolderData.DefaultRootDestinationFolder;
            BackupJobManagerView dlg = new BackupJobManagerView(data,false,mgr.FolderData.DefaultAddnewJobType);
            dlg.DataSaved += new BackupJobManagerView.MyEventHandler<GenericEventArgsType<BackupJob>>(dlg_DataSaved);
            IsUILoading = false;
            dlg.ShowDialog();
        }

        bool dlg_DataSaved(object sender, GenericEventArgsType<BackupJob> e)
        {
            bool DuplicateFolder = false;
            bool FolderBelongsToFolderJob = false;
            Global.DisableMessageBoxOnce = true;//ignore warning from msgbox on below.
            string tmp = FileOperations.GetRootBackupDestinationFolder(e.Data);
            foreach (var item in mgr.FolderData)
            {
                if (item.Value.CurrentBackupJobMode != BackupJobModeEnum.Files)
                {
                    if (item.Value.RootSourceFolder == e.Data.RootSourceFolder && item.Value.JobGUID != e.Data.JobGUID)
                    {
                        MessageBox.Show("Cannot Add. Duplicate source folder.");
                        return false;
                    }
                }
                if (item.Value.JobSetName == e.Data.JobSetName && item.Value.JobGUID != e.Data.JobGUID)
                {
                    MessageBox.Show("Cannot Add. Duplicate JobName.");
                    return false;
                }
                Global.DisableMessageBoxOnce = true;//ignore warning from msgbox on below.
                string tmp1 =FileOperations.GetRootBackupDestinationFolder(item.Value);
                if (tmp1 ==  tmp && item.Value.JobGUID != e.Data.JobGUID)
                {
                    DuplicateFolder = true;
                    if(item.Value.CurrentBackupJobMode == BackupJobModeEnum.Folders)
                        FolderBelongsToFolderJob = true;
                }
            }
            if (DuplicateFolder == true && FolderBelongsToFolderJob == false)
            {
                if (MessageBox.Show("Root destination folder is used by another file job. This may overwirte files and merge folders if same name exsits in source." + Environment.NewLine + Environment.NewLine + "Do you want to share destination between multiple jobs?", "Duplicate", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;
            }
            if (DuplicateFolder == true && FolderBelongsToFolderJob == true)
            {
                MessageBox.Show("Cannot Add. The destination folder is already used as the Root for another Folder job.");
                return false;
            }
            if (IsEditmode == false)
            {
                mgr.FolderData.Add(e.Data.JobGUID, e.Data);
                mgr.FolderData.DefaultAddnewJobType = e.Data.CurrentBackupJobMode;
            }
            if(e.Data.CurrentBackupJobMode == BackupJobModeEnum.Folders)
                mgr.FolderData.DefaultRootDestinationFolder = e.Data.RootBackupFolder;
            IsEditmode = false;
            ReloadUI();
            Logger.StaticLogger.AddDebugMessagesEntry("Start Saving " + e.Data.JobSetName + " data into file");
            mgr.SaveData();
            Logger.StaticLogger.AddDebugMessagesEntry("Completed Saving " + e.Data.JobSetName + " data into file");
            return true;
        }

        bool ReloadUI()
        {
            IsUILoading = true;
            lstFolderSets.Items.Clear();
            foreach (var item in mgr.FolderData)
            {
                lstFolderSets.Items.Add(item.Value);
                lstFolderSets.SetItemChecked(lstFolderSets.Items.Count - 1, item.Value.Enabled);
            }
            if (lstFolderSets.Items.Count > 0)
                lstFolderSets.SelectedIndex = 0; // this will inturn call  ReloadFolderSettings();
            else
            {
                txtRootFolder.Text = "";
                lstExcludedFolders.Items.Clear();
            }
            UpdateServicestatus();
            IsUILoading = false;
            return true;
        }

        private void lstFolderSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFolderSets.SelectedItems.Count != 1)
                return;
            CurrentJob = (BackupJob)lstFolderSets.SelectedItems[0];
            txtRootFolder.Text = CurrentJob.RootSourceFolder;
            lstExcludedFolders.Items.Clear();
            foreach (var item in CurrentJob.FolderSettings.ExcludedLocations)
            {
                lstExcludedFolders.Items.Add(item);
            }
            ReloadFolderSettings();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int sel = 0;
            IsEditmode = true;
            if (lstFolderSets.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select the job to modify.");
                return;
            }
            sel = lstFolderSets.SelectedIndex;
            BackupJob data = (BackupJob)lstFolderSets.SelectedItems[0];
            BackupJobManagerView dlg = new BackupJobManagerView(data,true,null);
            dlg.DataSaved+=new BackupJobManagerView.MyEventHandler<GenericEventArgsType<BackupJob>>(dlg_DataSaved);
            dlg.ShowDialog();
            lstFolderSets.SelectedIndex = sel;
        }

        private void DashBoard_Load(object sender, EventArgs e)
        {
            ReloadUI();
        }

        private void ReloadFolderSettings()
        {
            lblBackUpMode.Text =  CurrentJob.JobFileOperationMode.ToString();
            if (CurrentJob.JobFileOperationMode == JobFilesOperationModeEnum.Copy && CurrentJob.CurrentBackupJobMode == BackupJobModeEnum.Folders && CurrentJob.FolderSettings.CleanupSchedule > 0)
                btnSync.Enabled = true;
            else
                btnSync.Enabled = false;
            if (CurrentJob.JobFileOperationMode == JobFilesOperationModeEnum.Copy)
            {
                lblDeleteEmptyfolders.Text = "NA";
                lblDeleteEmptyfolders.ForeColor = Color.Black;
            }
            else
            {
                lblDeleteEmptyfolders.Text = CurrentJob.FolderSettings.IncludeEmptyFolders.ToString();
                if (CurrentJob.FolderSettings.IncludeEmptyFolders == true)
                    lblDeleteEmptyfolders.ForeColor = Color.Red;
            }
            lblIncludeSubFolders.Text = CurrentJob.FolderSettings.IncludeSubfolders.ToString();
            if (CurrentJob.CurrentBackupJobMode == BackupJobModeEnum.Files)
                txtbckLocation.Text = CurrentJob.RootBackupFolder;
            else
            {
                Global.DisableMessageBoxOnce = true; //disables the Messagebox for this time.
                txtbckLocation.Text = FileOperations.GetRootBackupDestinationFolder(CurrentJob);
            }
            if (CurrentJob.ServiceCheckMode == ServiceIntervalEnum.Daily)
            {
                lblServiceInterval.Text = "Daily on " + CurrentJob.ServiceTriggerTimeInterval.ToString("hh:tt") + ".";
            }
            else if (CurrentJob.ServiceCheckMode == ServiceIntervalEnum.Weekly)
            {
                lblServiceInterval.Text = "Weekly on " + ((DayOfWeek)CurrentJob.ServiceTriggerDayInterval - 1).ToString() + ", " +
                    CurrentJob.ServiceTriggerTimeInterval.ToString("hh:tt") + ".";
            }
            else if (CurrentJob.ServiceCheckMode == ServiceIntervalEnum.Monthly)
            {
                lblServiceInterval.Text = "Montly on " + CurrentJob.ServiceTriggerDayInterval.ToString() + ", " +
                    CurrentJob.ServiceTriggerTimeInterval.ToString("hh:tt") + ".";
            }
            lblchkFileBefore.Text = CurrentJob.BackupAfterDays.ToString() + " Days, " +
                CurrentJob.BackupAfterHours.ToString() + " Hours.";
            LastRunStatusList sts = LastRunStatusList.LastRunStatusListInit();
            if (sts.ContainsKey(CurrentJob.JobGUID) == true)
                lblLastUpdatedTime.Text = sts[CurrentJob.JobGUID].LastSucessfullRunTime.ToShortDateString() + ", " + sts[CurrentJob.JobGUID].LastSucessfullRunTime.ToShortTimeString();
            else
                lblLastUpdatedTime.Text = "NA";
            if (CurrentJob.CurrentBackupJobMode == BackupJobModeEnum.Files && CurrentJob.ServiceCheckMode == ServiceIntervalEnum.OnFileUpdated)
                btnBckUpNow.Enabled = false;
            else if(IsAsycWorkInProgress == true)
                btnBckUpNow.Enabled =false;
            else
                btnBckUpNow.Enabled = true;
        }

        private void btnbckLocation_Click(object sender, EventArgs e)
        {
            string path = FileOperations.Getfolder();
            if (path == "")
                return;
            txtbckLocation.Text = path;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstFolderSets.SelectedItems.Count == 0)
                return;
            BackupJob data = (BackupJob)lstFolderSets.SelectedItems[0];
            if (MessageBox.Show("Do you want to delete '" + data.JobSetName + "' from the list?", "Delete Job", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            mgr.FolderData.Remove(data.JobGUID);
            if (lstFolderSets.SelectedItems.Count == 1)
                lstFolderSets.Items.Remove(lstFolderSets.SelectedItems[0]);
            ReloadUI();
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            mgr.SaveData();
            /*this is required as the validation should 
             * not come after hours or days when the event is occured.*/
            if (mgr.WinValidateAllDataForScheduler() == false)
                return;
            ScheduledJobManager intrface = new ScheduledJobManager();
            btnStopService.Enabled = false;
            btnStartService.Enabled = false;
            if (intrface.IsJobLastRunMissed() == true)
            {
                if (MessageBox.Show("Some backup schedules have been missed. Do you want to catchup those?", "Catchup Schedules", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    intrface.OperationCompleted+=new EventHandler<GenericEventArgsType<bool>>
                        ((x, y) =>
                        {
                            new WindowsServiceHelper().StartService();
                            UpdateServicestatus();
                            IsAsycWorkInProgress = false;
                        });
                    ThreadPool.QueueUserWorkItem(intrface.TriggerAsyc);
                    IsAsycWorkInProgress = true;
                }
                else
                {
                    new WindowsServiceHelper().StartService();
                    UpdateServicestatus();
                }
            }
            else
            {
                new WindowsServiceHelper().StartService();
                UpdateServicestatus();
            }
        }

         private void textBox1_Click(object sender, EventArgs e)
        {
            UpdateServicestatus();
        }

        private void btnStopService_Click(object sender, EventArgs e)
        {
            mgr.SaveData();
            btnStopService.Enabled = false;
            btnStartService.Enabled = false;
            (new WindowsServiceHelper()).StopService();
            UpdateServicestatus();
        }

        void UpdateServicestatus()
        {
            WindowsServiceInstallStatus sts  = (new WindowsServiceHelper()).GetServiceStatus();
            if (sts == WindowsServiceInstallStatus.Running )
            {
                btnStartService.Enabled = false;
                btnStopService.Enabled = true;
                txtServiceStatus.Text = "Running";
            }
            else if (sts == WindowsServiceInstallStatus.Stopped )
            {
                btnStartService.Enabled = true;
                btnStopService.Enabled = false;
                txtServiceStatus.Text = "Stopped";
            }
            else if (sts == WindowsServiceInstallStatus.ExceptionOccured)
            {
                btnStartService.Enabled = false;
                btnStopService.Enabled = false;
                txtServiceStatus.Text = "Exception Occured";
            }
            else
            {
                btnStartService.Enabled = false;
                btnStopService.Enabled = false;
                txtServiceStatus.Text = "Service Not Installed";
            }
        }

        private void btnBckUpNow_Click(object sender, EventArgs e)
        {
            FileSystemOperations flobj = new FileSystemOperations();
            if (lstFolderSets.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the job to perform the backup");
                return;
            }
            BackupJob data = (BackupJob)lstFolderSets.SelectedItems[0];
            if (mgr.ValidateFolderData(data) == false)
                return;
            btnBckUpNow.Text = "Working";
            btnBckUpNow.Enabled = false;
            flobj.OperationCompleted += new EventHandler<GenericEventArgsType<bool>>((x, y) => 
            {
                btnBckUpNow.Text = "Sync Folder now";
                btnBckUpNow.Enabled = true;
                IsAsycWorkInProgress = false;
                Logger.StaticLogger.AddAuditLogEntry("Completed manual backup for " + data.JobSetName);
                LastRunStatusList LastRunSts = LastRunStatusList.LastRunStatusListInit();
                if (LastRunSts.ContainsKey(data.JobGUID) == false)
                    LastRunSts.Add(data.JobGUID, new FileManagerLibrary.Base.UIData.LastRunStatus(DateTime.MinValue, data.JobGUID, 0));
                else
                {
                    LastRunStatus sts = LastRunSts[data.JobGUID];
                    sts.LastSucessfullRunTime = DateTime.Now;
                    LastRunSts.SaveData();
                    ReloadFolderSettings();
                }
            });
            Logger.StaticLogger.AddAuditLogEntry("Start manual backup for " + data.JobSetName);
            if (data.CurrentBackupJobMode == BackupJobModeEnum.Folders)
                ThreadPool.QueueUserWorkItem(flobj.PerformFolderBackupAsyc, data);
            else
                ThreadPool.QueueUserWorkItem(flobj.PerformFilesBackupAsyc, data);
            IsAsycWorkInProgress = true;
        }

        private void lstFolderSets_ItemCheck(object sender, ItemCheckEventArgs e)
        {   
            if (IsUILoading == true)
                return;
            if (DoubleClicked == false)
            {
                e.NewValue = e.CurrentValue;
                return;
            }
            DoubleClicked = false;
            BackupJob data = (BackupJob)lstFolderSets.SelectedItems[0];
            if (e.NewValue == CheckState.Checked)
            {
                if (MessageBox.Show("Do you want to enable this job?", "Enable Job", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    data.Enabled = true;
                else
                    e.NewValue = e.CurrentValue;
            }
            else
            {
                if (MessageBox.Show("Do you want to disable this job?", "Disable Job", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    data.Enabled = false;
                else
                    e.NewValue = e.CurrentValue;

            }
            mgr.SaveData();
        }

        bool DoubleClicked; /* this value and logic is used to disable the check changed status in checked list box on single click. So now user can change checked status only on double click on list box items.*/
        private void lstFolderSets_DoubleClick(object sender, EventArgs e)
        {
            DoubleClicked = true;
            if (lstFolderSets.SelectedIndices.Count == 0)
                return;
            lstFolderSets.SetItemChecked(lstFolderSets.SelectedIndices[0], !lstFolderSets.GetItemChecked(lstFolderSets.SelectedIndices[0]));
        }

        private void DashBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
            mgr.SaveData();
        }

        private void DashBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsAsycWorkInProgress == true)
            {
                if(MessageBox.Show("Background work in progress. Do you want to wait until the process is completed?","Quit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    e.Cancel = true;
                return;
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
           
                FileSystemOperations flobj = new FileSystemOperations();
                if (lstFolderSets.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Select the folder to perform the Cleanup");
                    return;
                }
                if (MessageBox.Show("Any  files and folders in the backup location, if not present in source will be deleted. You want to Continue?", "Cleanup", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
                BackupJob data = (BackupJob)lstFolderSets.SelectedItems[0];
                /* prevents job to perform the cleanup operation for jobs in Move or Delete or in files mode.
                 * allowing this will delete files from both source and destination, which is critical data loss issue.*/
                if (data.CurrentBackupJobMode == BackupJobModeEnum.Folders && data.JobFileOperationMode == JobFilesOperationModeEnum.Copy)
                {  
                     if (mgr.ValidateFolderData(data) == false)
                        return;
                    btnSync.Text = "Working";
                    btnSync.Enabled = false;
                    flobj.OperationCompleted += new EventHandler<GenericEventArgsType<bool>>((x, y) =>
                    {
                        btnSync.Text = "Cleanup Destination";
                        btnSync.Enabled = true;
                        IsAsycWorkInProgress = false;
                        Logger.StaticLogger.AddAuditLogEntry("Completed manual cleanup for destination " + data.JobSetName);
                    });
                    Logger.StaticLogger.AddAuditLogEntry("Started manual cleanup for destination " + data.JobSetName);
                    ThreadPool.QueueUserWorkItem(flobj.PerformCleanupDestinationAsyc, data);
                    IsAsycWorkInProgress = true;
                }
                else
                {
                    MessageBox.Show("Cannot perform cleanup peration for destination folders for Files Jobs or Folder Jobs with Move or Delete operations.");
                }
        }

        #region Adding Custom Menu item to TitleBar

        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MYMENU1 = 1000;
        public const Int32 MUMENU2 = 1001;
        public const Int32 MYMENU3 = 1002;
        public const Int32 MF_SEPARATOR = 0x800;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);


        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_SYSCOMMAND)
            {
                switch (msg.WParam.ToInt32())
                {
                    case MYMENU1:
                        System.Diagnostics.Process.Start("explorer.exe", FileOperations.GetAppSettingsFolderRoot());
                        return;
                    case MYMENU3:
                        WindowsServiceInstallHelper.UnInstallWindowsService();
                        return;
                    case 61587:
                        break;
                    default:
                        break;
                }
            }
            base.WndProc(ref msg);
        }

        #endregion

        private void MouseDoubleClick_OpenFolder(object sender, MouseEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", ((TextBox)sender).Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Logging.Logger.StaticLogger.GetExceptionEntry(ex));
            }
        }
    }
}

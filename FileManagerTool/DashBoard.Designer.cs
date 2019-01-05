namespace FileManagerTool
{
    partial class DashBoard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashBoard));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServiceStatus = new System.Windows.Forms.TextBox();
            this.btnStopService = new System.Windows.Forms.Button();
            this.btnStartService = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnBckUpNow = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtRootFolder = new System.Windows.Forms.TextBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lstExcludedFolders = new System.Windows.Forms.ListBox();
            this.lstFolderSets = new System.Windows.Forms.CheckedListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblchkFileBefore = new System.Windows.Forms.Label();
            this.lblLastUpdatedTime = new System.Windows.Forms.Label();
            this.lblServiceInterval = new System.Windows.Forms.Label();
            this.lblDeleteEmptyfolders = new System.Windows.Forms.Label();
            this.lblIncludeSubFolders = new System.Windows.Forms.Label();
            this.lblBackUpMode = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblbcklocation = new System.Windows.Forms.Label();
            this.txtbckLocation = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtServiceStatus);
            this.groupBox1.Controls.Add(this.btnStopService);
            this.groupBox1.Controls.Add(this.btnStartService);
            this.groupBox1.Location = new System.Drawing.Point(611, 365);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(173, 145);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service Management";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Current Service Status";
            // 
            // txtServiceStatus
            // 
            this.txtServiceStatus.Location = new System.Drawing.Point(6, 37);
            this.txtServiceStatus.Name = "txtServiceStatus";
            this.txtServiceStatus.ReadOnly = true;
            this.txtServiceStatus.Size = new System.Drawing.Size(154, 20);
            this.txtServiceStatus.TabIndex = 1;
            this.txtServiceStatus.Click += new System.EventHandler(this.textBox1_Click);
            // 
            // btnStopService
            // 
            this.btnStopService.Location = new System.Drawing.Point(6, 99);
            this.btnStopService.Name = "btnStopService";
            this.btnStopService.Size = new System.Drawing.Size(154, 28);
            this.btnStopService.TabIndex = 0;
            this.btnStopService.Text = "Stop Service";
            this.btnStopService.UseVisualStyleBackColor = true;
            this.btnStopService.Click += new System.EventHandler(this.btnStopService_Click);
            // 
            // btnStartService
            // 
            this.btnStartService.Location = new System.Drawing.Point(6, 65);
            this.btnStartService.Name = "btnStartService";
            this.btnStartService.Size = new System.Drawing.Size(154, 28);
            this.btnStartService.TabIndex = 0;
            this.btnStartService.Text = "Start Service";
            this.btnStartService.UseVisualStyleBackColor = true;
            this.btnStartService.Click += new System.EventHandler(this.btnStartService_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.btnAdd);
            this.groupBox4.Controls.Add(this.btnSync);
            this.groupBox4.Controls.Add(this.btnBckUpNow);
            this.groupBox4.Controls.Add(this.btnDelete);
            this.groupBox4.Controls.Add(this.txtRootFolder);
            this.groupBox4.Controls.Add(this.btnEdit);
            this.groupBox4.Controls.Add(this.lstExcludedFolders);
            this.groupBox4.Controls.Add(this.lstFolderSets);
            this.groupBox4.Location = new System.Drawing.Point(14, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(770, 346);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "BackupSets Management";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(12, 244);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(102, 15);
            this.label10.TabIndex = 2;
            this.label10.Text = "Excluded Folders";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(12, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "Backup Sets";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 195);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Root Folder";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(682, 29);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 25);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(682, 151);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 35);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Cleanup Destination";
            this.toolTip1.SetToolTip(this.btnSync, resources.GetString("btnSync.ToolTip"));
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnBckUpNow
            // 
            this.btnBckUpNow.Location = new System.Drawing.Point(682, 114);
            this.btnBckUpNow.Name = "btnBckUpNow";
            this.btnBckUpNow.Size = new System.Drawing.Size(75, 35);
            this.btnBckUpNow.TabIndex = 2;
            this.btnBckUpNow.Text = "Sync Folder now";
            this.toolTip1.SetToolTip(this.btnBckUpNow, "Perform Backup/Cleanup Now for the selected folderset");
            this.btnBckUpNow.UseVisualStyleBackColor = true;
            this.btnBckUpNow.Click += new System.EventHandler(this.btnBckUpNow_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(682, 80);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtRootFolder
            // 
            this.txtRootFolder.Location = new System.Drawing.Point(12, 213);
            this.txtRootFolder.Name = "txtRootFolder";
            this.txtRootFolder.ReadOnly = true;
            this.txtRootFolder.Size = new System.Drawing.Size(745, 20);
            this.txtRootFolder.TabIndex = 1;
            this.txtRootFolder.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MouseDoubleClick_OpenFolder);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(682, 56);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 22);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lstExcludedFolders
            // 
            this.lstExcludedFolders.FormattingEnabled = true;
            this.lstExcludedFolders.Location = new System.Drawing.Point(13, 262);
            this.lstExcludedFolders.Name = "lstExcludedFolders";
            this.lstExcludedFolders.Size = new System.Drawing.Size(744, 69);
            this.lstExcludedFolders.TabIndex = 7;
            // 
            // lstFolderSets
            // 
            this.lstFolderSets.FormattingEnabled = true;
            this.lstFolderSets.Location = new System.Drawing.Point(12, 31);
            this.lstFolderSets.Name = "lstFolderSets";
            this.lstFolderSets.Size = new System.Drawing.Size(664, 154);
            this.lstFolderSets.TabIndex = 8;
            this.lstFolderSets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstFolderSets_ItemCheck);
            this.lstFolderSets.SelectedIndexChanged += new System.EventHandler(this.lstFolderSets_SelectedIndexChanged);
            this.lstFolderSets.DoubleClick += new System.EventHandler(this.lstFolderSets_DoubleClick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblchkFileBefore);
            this.groupBox5.Controls.Add(this.lblLastUpdatedTime);
            this.groupBox5.Controls.Add(this.lblServiceInterval);
            this.groupBox5.Controls.Add(this.lblDeleteEmptyfolders);
            this.groupBox5.Controls.Add(this.lblIncludeSubFolders);
            this.groupBox5.Controls.Add(this.lblBackUpMode);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.lblbcklocation);
            this.groupBox5.Controls.Add(this.txtbckLocation);
            this.groupBox5.Location = new System.Drawing.Point(14, 364);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(591, 146);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Job Status && Settings";
            // 
            // lblchkFileBefore
            // 
            this.lblchkFileBefore.BackColor = System.Drawing.SystemColors.Control;
            this.lblchkFileBefore.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblchkFileBefore.ForeColor = System.Drawing.Color.Black;
            this.lblchkFileBefore.Location = new System.Drawing.Point(338, 75);
            this.lblchkFileBefore.Name = "lblchkFileBefore";
            this.lblchkFileBefore.Size = new System.Drawing.Size(117, 19);
            this.lblchkFileBefore.TabIndex = 9;
            this.lblchkFileBefore.Text = "0 Days, 0 Hours";
            this.lblchkFileBefore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLastUpdatedTime
            // 
            this.lblLastUpdatedTime.BackColor = System.Drawing.SystemColors.Control;
            this.lblLastUpdatedTime.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastUpdatedTime.ForeColor = System.Drawing.Color.Black;
            this.lblLastUpdatedTime.Location = new System.Drawing.Point(338, 25);
            this.lblLastUpdatedTime.Name = "lblLastUpdatedTime";
            this.lblLastUpdatedTime.Size = new System.Drawing.Size(195, 19);
            this.lblLastUpdatedTime.TabIndex = 9;
            this.lblLastUpdatedTime.Text = "Monthly, every 28th, 11:PM";
            this.lblLastUpdatedTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblServiceInterval
            // 
            this.lblServiceInterval.BackColor = System.Drawing.SystemColors.Control;
            this.lblServiceInterval.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServiceInterval.ForeColor = System.Drawing.Color.Black;
            this.lblServiceInterval.Location = new System.Drawing.Point(338, 48);
            this.lblServiceInterval.Name = "lblServiceInterval";
            this.lblServiceInterval.Size = new System.Drawing.Size(195, 19);
            this.lblServiceInterval.TabIndex = 9;
            this.lblServiceInterval.Text = "Monthly, every 28th, 11:PM";
            this.lblServiceInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDeleteEmptyfolders
            // 
            this.lblDeleteEmptyfolders.BackColor = System.Drawing.SystemColors.Control;
            this.lblDeleteEmptyfolders.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeleteEmptyfolders.ForeColor = System.Drawing.Color.Black;
            this.lblDeleteEmptyfolders.Location = new System.Drawing.Point(117, 73);
            this.lblDeleteEmptyfolders.Name = "lblDeleteEmptyfolders";
            this.lblDeleteEmptyfolders.Size = new System.Drawing.Size(79, 19);
            this.lblDeleteEmptyfolders.TabIndex = 9;
            this.lblDeleteEmptyfolders.Text = "Copy";
            this.lblDeleteEmptyfolders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblIncludeSubFolders
            // 
            this.lblIncludeSubFolders.BackColor = System.Drawing.SystemColors.Control;
            this.lblIncludeSubFolders.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIncludeSubFolders.ForeColor = System.Drawing.Color.Black;
            this.lblIncludeSubFolders.Location = new System.Drawing.Point(117, 49);
            this.lblIncludeSubFolders.Name = "lblIncludeSubFolders";
            this.lblIncludeSubFolders.Size = new System.Drawing.Size(79, 19);
            this.lblIncludeSubFolders.TabIndex = 9;
            this.lblIncludeSubFolders.Text = "Copy";
            this.lblIncludeSubFolders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBackUpMode
            // 
            this.lblBackUpMode.BackColor = System.Drawing.SystemColors.Control;
            this.lblBackUpMode.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackUpMode.ForeColor = System.Drawing.Color.Black;
            this.lblBackUpMode.Location = new System.Drawing.Point(117, 25);
            this.lblBackUpMode.Name = "lblBackUpMode";
            this.lblBackUpMode.Size = new System.Drawing.Size(76, 19);
            this.lblBackUpMode.TabIndex = 9;
            this.lblBackUpMode.Text = "Copy";
            this.lblBackUpMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(224, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 26);
            this.label4.TabIndex = 2;
            this.label4.Text = "Last Updated on";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(7, 72);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 21);
            this.label13.TabIndex = 9;
            this.label13.Text = "Delete Empty Folders";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(224, 42);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 26);
            this.label11.TabIndex = 2;
            this.label11.Text = "Sync files ";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 21);
            this.label2.TabIndex = 9;
            this.label2.Text = "Include SubFolders";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(224, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 31);
            this.label6.TabIndex = 2;
            this.label6.Text = "Check for files created before ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 21);
            this.label3.TabIndex = 9;
            this.label3.Text = "Operation Mode";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblbcklocation
            // 
            this.lblbcklocation.AutoSize = true;
            this.lblbcklocation.Location = new System.Drawing.Point(7, 98);
            this.lblbcklocation.Name = "lblbcklocation";
            this.lblbcklocation.Size = new System.Drawing.Size(92, 13);
            this.lblbcklocation.TabIndex = 2;
            this.lblbcklocation.Text = "Destination Folder";
            // 
            // txtbckLocation
            // 
            this.txtbckLocation.Location = new System.Drawing.Point(9, 115);
            this.txtbckLocation.Name = "txtbckLocation";
            this.txtbckLocation.ReadOnly = true;
            this.txtbckLocation.Size = new System.Drawing.Size(569, 20);
            this.txtbckLocation.TabIndex = 1;
            this.txtbckLocation.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MouseDoubleClick_OpenFolder);
            // 
            // DashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 536);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DashBoard";
            this.Text = "File Backup Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashBoard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DashBoard_FormClosed);
            this.Load += new System.EventHandler(this.DashBoard_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStopService;
        private System.Windows.Forms.Button btnStartService;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServiceStatus;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtbckLocation;
        private System.Windows.Forms.TextBox txtRootFolder;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblbcklocation;
        private System.Windows.Forms.ListBox lstExcludedFolders;
        private System.Windows.Forms.Button btnBckUpNow;
        private System.Windows.Forms.CheckedListBox lstFolderSets;
        private System.Windows.Forms.Label lblBackUpMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblchkFileBefore;
        private System.Windows.Forms.Label lblServiceInterval;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblDeleteEmptyfolders;
        private System.Windows.Forms.Label lblIncludeSubFolders;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Label lblLastUpdatedTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}


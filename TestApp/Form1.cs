using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileManagerService;
using System.IO;
using FileManagerLibrary.Base.JobManagers;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ScheduledJobManager obj = new ScheduledJobManager();
            obj.Trigger();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalHours.ToString());
            textBox2.Text = (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalDays.ToString());
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalHours.ToString());
            textBox2.Text = (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalDays.ToString());

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalHours.ToString());
            textBox2.Text = (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalDays.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScheduledJobManager obj = new ScheduledJobManager();
            obj.StartService(1,1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DirectoryInfo source = new DirectoryInfo(@"F:\Pictures");
            DirectoryInfo dest = new DirectoryInfo(@"M:\Backups\Pictures");
            FileInfo[] sourcefiles = source.GetFiles("*.*", SearchOption.AllDirectories);
            FileInfo[] destfiles = dest.GetFiles("*.*", SearchOption.AllDirectories);
            IEnumerable<String> sourcelist = from file in sourcefiles select file.FullName.Substring(@"F:\Pictures".Length);
            IEnumerable<String> destlist = from file in destfiles select file.FullName.Substring(@"M:\Backups\Pictures".Length);
            IEnumerable<String> filetodelete = from str in destlist.Exclude<string>(sourcelist) select @"M:\Backups\Pictures" + str;
            listBox1.Items.AddRange(filetodelete.ToArray<string>());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(FileManagerLibrary.AppSettings.AppSettingsNetworkLocation);
        }
    }
}

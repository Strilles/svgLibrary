using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace svgLibrary
{
    public partial class ScanForm : Form
    {
        private FileScanner fScan = new FileScanner();

        delegate void UpdateLogDelegate(string message);

        static bool deleteFiles = false;
        static bool deleteFolders = false;
        static int items = 0;

        public ScanForm()
        {
            InitializeComponent();

            //Subscribe to OnProgressUpdate
            fScan.OnProgressUpdate += fScan_OnProgressUpdate;
            //Report progress from worker to set property
            backgroundWorker.WorkerReportsProgress = true;

            
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Scan and update GUI -- Update not working as inteded
            fScan.ScanDirectory();
            fScan.Update();
        }

        private void fScan_OnProgressUpdate(int v)
        {
            base.Invoke((Action)delegate
            {
                backgroundWorker.ReportProgress(GetPercent(v, items));
            });

        }

        private int GetPercent(int current, int total)
        {
            //Convert current progress to int usable by ProgressBar
            double d = current / total;
            double dd = d * 100;
            int percent = (int)Math.Floor(dd);
            return percent;
        }

        void UpdateLog (string message)
        {
            if (InvokeRequired)
            {
                //Add messages to log if required
                Invoke(new UpdateLogDelegate(UpdateLog), message);
                return;
            }
            logRichTextBox.AppendText(message + "\n");
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Update progress
            progressBar.Value = e.ProgressPercentage;

        }

        private void unusedFileDelete_CheckedChanged(object sender, EventArgs e)
        {
            //Does user want to delete unused files?
            deleteFiles = unusedFileDelete.Checked;
        }

        private void emptyFoldersDelete_CheckedChanged(object sender, EventArgs e)
        {
            //Does user want to delete empty folders?
            deleteFolders = emptyFoldersDelete.Checked;
        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            scanButton.Enabled = false;
            //Reset results from last scan
            fScan.ResetResults();
            resultsBox.Clear();
            //Init the progress bar
            items = fScan.InitProgressBar();
            progressBar.Value = 0;
            //Start Backgroundworker
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //After worker completed report results
            List<KeyValuePair<string, int>> results = fScan.ReportResults();
            string resultConcat = "";
            foreach (var i in results)
            {
                resultConcat = string.Concat(resultConcat, i.Key, i.Value);
            }

            resultsBox.AppendText($"\n{resultConcat}");

            //User wanted to delete files
            if (deleteFiles && fScan.GetFileDeletionQueue().Count >= 1)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to continue with unused file deletion?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    DeleteFiles();
                }
                else
                {
                    return;
                }
            }
            //User wanted to delete empty folders
            if (deleteFolders && fScan.GetFolderDeletionQueue().Count >= 1)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to continue with empty folder deletion?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    DeleteFolders();
                }
                else
                {
                    return;
                }
            }
            backgroundWorker.Dispose();
            scanButton.Enabled = true;
        }

        private void DeleteFiles()
        {
            foreach (KeyValuePair<string, string> file in fScan.GetFileDeletionQueue())
            {
                File.Delete(file.Value);
                UpdateLog($"{file.Key} Deleted!");
            }
        }

        private void DeleteFolders()
        {
            foreach (string folder in fScan.GetFolderDeletionQueue())
            {
                Directory.Delete(folder);
                UpdateLog($"{folder} Deleted!");
            }
        }
    }
}

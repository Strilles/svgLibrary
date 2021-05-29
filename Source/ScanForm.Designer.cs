
namespace svgLibrary
{
    partial class ScanForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanForm));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.scanButton = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.emptyFoldersDelete = new System.Windows.Forms.CheckBox();
            this.unusedFileDelete = new System.Windows.Forms.CheckBox();
            this.logRichTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.resultsBox = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 427);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(800, 23);
            this.progressBar.TabIndex = 0;
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(373, 398);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(75, 23);
            this.scanButton.TabIndex = 1;
            this.scanButton.Text = "Scan";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.emptyFoldersDelete);
            this.groupBox1.Controls.Add(this.unusedFileDelete);
            this.groupBox1.Location = new System.Drawing.Point(568, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 73);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // emptyFoldersDelete
            // 
            this.emptyFoldersDelete.AutoSize = true;
            this.emptyFoldersDelete.Location = new System.Drawing.Point(7, 44);
            this.emptyFoldersDelete.Name = "emptyFoldersDelete";
            this.emptyFoldersDelete.Size = new System.Drawing.Size(128, 17);
            this.emptyFoldersDelete.TabIndex = 1;
            this.emptyFoldersDelete.Text = "Delete empty folders?";
            this.emptyFoldersDelete.UseVisualStyleBackColor = true;
            this.emptyFoldersDelete.CheckedChanged += new System.EventHandler(this.emptyFoldersDelete_CheckedChanged);
            // 
            // unusedFileDelete
            // 
            this.unusedFileDelete.AutoSize = true;
            this.unusedFileDelete.Location = new System.Drawing.Point(7, 20);
            this.unusedFileDelete.Name = "unusedFileDelete";
            this.unusedFileDelete.Size = new System.Drawing.Size(122, 17);
            this.unusedFileDelete.TabIndex = 0;
            this.unusedFileDelete.Text = "Delete unused files?";
            this.unusedFileDelete.UseVisualStyleBackColor = true;
            this.unusedFileDelete.CheckedChanged += new System.EventHandler(this.unusedFileDelete_CheckedChanged);
            // 
            // logRichTextBox
            // 
            this.logRichTextBox.Location = new System.Drawing.Point(13, 13);
            this.logRichTextBox.Name = "logRichTextBox";
            this.logRichTextBox.Size = new System.Drawing.Size(549, 372);
            this.logRichTextBox.TabIndex = 3;
            this.logRichTextBox.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.resultsBox);
            this.groupBox2.Location = new System.Drawing.Point(568, 93);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 194);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Results";
            // 
            // resultsBox
            // 
            this.resultsBox.Location = new System.Drawing.Point(7, 20);
            this.resultsBox.Name = "resultsBox";
            this.resultsBox.ReadOnly = true;
            this.resultsBox.Size = new System.Drawing.Size(187, 168);
            this.resultsBox.TabIndex = 0;
            this.resultsBox.Text = "";
            // 
            // ScanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.logRichTextBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.progressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScanForm";
            this.Text = "Scan";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button scanButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox logRichTextBox;
        private System.Windows.Forms.CheckBox emptyFoldersDelete;
        private System.Windows.Forms.CheckBox unusedFileDelete;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox resultsBox;
    }
}
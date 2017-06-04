﻿namespace MonitorMe
{
    partial class MonitorMeForm
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.fileSelectionBtn = new System.Windows.Forms.Button();
            this.reportBtn = new System.Windows.Forms.Button();
            this.monthComboBox = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.errorLb = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.fileSelectionBtn);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.errorLb);
            this.splitContainer.Panel2.Controls.Add(this.reportBtn);
            this.splitContainer.Panel2.Controls.Add(this.monthComboBox);
            this.splitContainer.Size = new System.Drawing.Size(669, 383);
            this.splitContainer.SplitterDistance = 307;
            this.splitContainer.TabIndex = 0;
            // 
            // fileSelectionBtn
            // 
            this.fileSelectionBtn.Location = new System.Drawing.Point(64, 117);
            this.fileSelectionBtn.Name = "fileSelectionBtn";
            this.fileSelectionBtn.Size = new System.Drawing.Size(75, 23);
            this.fileSelectionBtn.TabIndex = 0;
            this.fileSelectionBtn.Text = "Select File";
            this.fileSelectionBtn.UseVisualStyleBackColor = true;
            this.fileSelectionBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // reportBtn
            // 
            this.reportBtn.Location = new System.Drawing.Point(82, 163);
            this.reportBtn.Name = "reportBtn";
            this.reportBtn.Size = new System.Drawing.Size(120, 23);
            this.reportBtn.TabIndex = 1;
            this.reportBtn.Text = "Generate Report";
            this.reportBtn.UseVisualStyleBackColor = true;
            this.reportBtn.Click += new System.EventHandler(this.reportBtn_Click);
            // 
            // monthComboBox
            // 
            this.monthComboBox.FormattingEnabled = true;
            this.monthComboBox.Items.AddRange(new object[] {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"});
            this.monthComboBox.Location = new System.Drawing.Point(82, 119);
            this.monthComboBox.Name = "monthComboBox";
            this.monthComboBox.Size = new System.Drawing.Size(121, 21);
            this.monthComboBox.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // errorLb
            // 
            this.errorLb.AutoSize = true;
            this.errorLb.Location = new System.Drawing.Point(82, 82);
            this.errorLb.Name = "errorLb";
            this.errorLb.Size = new System.Drawing.Size(0, 13);
            this.errorLb.TabIndex = 2;
            // 
            // MonitorMeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 383);
            this.Controls.Add(this.splitContainer);
            this.Name = "MonitorMeForm";
            this.Text = "Monitor Me";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button fileSelectionBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox monthComboBox;
        private System.Windows.Forms.Button reportBtn;
        private System.Windows.Forms.Label errorLb;
    }
}


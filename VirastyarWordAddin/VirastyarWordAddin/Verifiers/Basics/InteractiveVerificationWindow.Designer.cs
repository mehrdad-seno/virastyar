namespace VirastyarWordAddin.Verifiers.Basics
{
    partial class InteractiveVerificationWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InteractiveVerificationWindow));
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.panelMainContent = new System.Windows.Forms.Panel();
            this.btnChangeAll = new System.Windows.Forms.Button();
            this.btnAddToDictionary = new System.Windows.Forms.Button();
            this.btnIgnoreAll = new System.Windows.Forms.Button();
            this.panelDisplayContent = new System.Windows.Forms.Panel();
            this.splitContainerErrorArea = new System.Windows.Forms.SplitContainer();
            this.panelUp = new System.Windows.Forms.Panel();
            this.rtbErrorText = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.panelMainContent.SuspendLayout();
            this.panelDisplayContent.SuspendLayout();
            this.splitContainerErrorArea.Panel1.SuspendLayout();
            this.splitContainerErrorArea.SuspendLayout();
            this.panelUp.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripMain
            // 
            this.statusStripMain.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStripMain.Location = new System.Drawing.Point(0, 358);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStripMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStripMain.Size = new System.Drawing.Size(550, 22);
            this.statusStripMain.SizingGrip = false;
            this.statusStripMain.TabIndex = 0;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // panelMainContent
            // 
            this.panelMainContent.Controls.Add(this.btnChangeAll);
            this.panelMainContent.Controls.Add(this.btnAddToDictionary);
            this.panelMainContent.Controls.Add(this.btnIgnoreAll);
            this.panelMainContent.Controls.Add(this.panelDisplayContent);
            this.panelMainContent.Controls.Add(this.btnStop);
            this.panelMainContent.Controls.Add(this.btnIgnore);
            this.panelMainContent.Controls.Add(this.btnChange);
            this.panelMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContent.Location = new System.Drawing.Point(0, 0);
            this.panelMainContent.Name = "panelMainContent";
            this.panelMainContent.Size = new System.Drawing.Size(550, 358);
            this.panelMainContent.TabIndex = 0;
            // 
            // btnChangeAll
            // 
            this.btnChangeAll.Location = new System.Drawing.Point(8, 211);
            this.btnChangeAll.Name = "btnChangeAll";
            this.btnChangeAll.Size = new System.Drawing.Size(103, 25);
            this.btnChangeAll.TabIndex = 4;
            this.btnChangeAll.Text = "تغییر همه";
            this.btnChangeAll.UseVisualStyleBackColor = true;
            this.btnChangeAll.Click += new System.EventHandler(this.BtnChangeAllClick);
            // 
            // btnAddToDictionary
            // 
            this.btnAddToDictionary.Location = new System.Drawing.Point(8, 94);
            this.btnAddToDictionary.Name = "btnAddToDictionary";
            this.btnAddToDictionary.Size = new System.Drawing.Size(103, 25);
            this.btnAddToDictionary.TabIndex = 2;
            this.btnAddToDictionary.Text = "افزودن به واژه‌نامه";
            this.btnAddToDictionary.UseVisualStyleBackColor = true;
            this.btnAddToDictionary.Click += new System.EventHandler(this.BtnAddToDictionaryClick);
            // 
            // btnIgnoreAll
            // 
            this.btnIgnoreAll.Location = new System.Drawing.Point(8, 63);
            this.btnIgnoreAll.Name = "btnIgnoreAll";
            this.btnIgnoreAll.Size = new System.Drawing.Size(103, 25);
            this.btnIgnoreAll.TabIndex = 1;
            this.btnIgnoreAll.Text = "نادیده گرفتن همه";
            this.btnIgnoreAll.UseVisualStyleBackColor = true;
            this.btnIgnoreAll.Click += new System.EventHandler(this.BtnIgnoreAllClick);
            // 
            // panelDisplayContent
            // 
            this.panelDisplayContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisplayContent.Controls.Add(this.splitContainerErrorArea);
            this.panelDisplayContent.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelDisplayContent.Location = new System.Drawing.Point(117, 12);
            this.panelDisplayContent.Name = "panelDisplayContent";
            this.panelDisplayContent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panelDisplayContent.Size = new System.Drawing.Size(429, 333);
            this.panelDisplayContent.TabIndex = 3;
            // 
            // splitContainerErrorArea
            // 
            this.splitContainerErrorArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerErrorArea.Location = new System.Drawing.Point(0, 0);
            this.splitContainerErrorArea.Name = "splitContainerErrorArea";
            this.splitContainerErrorArea.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerErrorArea.Panel1
            // 
            this.splitContainerErrorArea.Panel1.Controls.Add(this.panelUp);
            this.splitContainerErrorArea.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitContainerErrorArea.Panel1MinSize = 100;
            // 
            // splitContainerErrorArea.Panel2
            // 
            this.splitContainerErrorArea.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitContainerErrorArea.Panel2MinSize = 100;
            this.splitContainerErrorArea.Size = new System.Drawing.Size(429, 333);
            this.splitContainerErrorArea.SplitterDistance = 136;
            this.splitContainerErrorArea.TabIndex = 2;
            // 
            // panelUp
            // 
            this.panelUp.Controls.Add(this.rtbErrorText);
            this.panelUp.Controls.Add(this.label1);
            this.panelUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUp.Location = new System.Drawing.Point(0, 0);
            this.panelUp.Name = "panelUp";
            this.panelUp.Size = new System.Drawing.Size(429, 136);
            this.panelUp.TabIndex = 1;
            // 
            // rtbErrorText
            // 
            this.rtbErrorText.BackColor = System.Drawing.Color.White;
            this.rtbErrorText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbErrorText.Location = new System.Drawing.Point(0, 19);
            this.rtbErrorText.Name = "rtbErrorText";
            this.rtbErrorText.ReadOnly = true;
            this.rtbErrorText.Size = new System.Drawing.Size(429, 117);
            this.rtbErrorText.TabIndex = 0;
            this.rtbErrorText.Text = "";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(429, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "خطاهای یافت شده:";
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(8, 320);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(103, 25);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "توقف";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.BtnStopClick);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(8, 32);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(103, 25);
            this.btnIgnore.TabIndex = 0;
            this.btnIgnore.Text = "نادیده گرفتن";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.BtnIgnoreClick);
            // 
            // btnChange
            // 
            this.btnChange.Location = new System.Drawing.Point(8, 180);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(103, 25);
            this.btnChange.TabIndex = 3;
            this.btnChange.Text = "تغییر";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.BtnChangeClick);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(0, 0);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(103, 25);
            this.btnResume.TabIndex = 6;
            this.btnResume.Text = "ادامه...";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.BtnResumeClick);
            // 
            // InteractiveVerificationWindow
            // 
            this.AcceptButton = this.btnChange;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(550, 380);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.panelMainContent);
            this.Controls.Add(this.statusStripMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InteractiveVerificationWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "پنجره تصحیح کننده";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.VerificationWindowHelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VerificationWindowFormClosing);
            this.Resize += new System.EventHandler(this.VerificationWindowResize);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.VerificationWindowHelpRequested);
            this.panelMainContent.ResumeLayout(false);
            this.panelDisplayContent.ResumeLayout(false);
            this.splitContainerErrorArea.Panel1.ResumeLayout(false);
            this.splitContainerErrorArea.ResumeLayout(false);
            this.panelUp.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.Panel panelMainContent;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Panel panelDisplayContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbErrorText;
        private System.Windows.Forms.SplitContainer splitContainerErrorArea;
        private System.Windows.Forms.Button btnIgnoreAll;
        private System.Windows.Forms.Button btnAddToDictionary;
        private System.Windows.Forms.Button btnChangeAll;
        private System.Windows.Forms.Panel panelUp;
        private System.Windows.Forms.Button btnResume;
    }
}
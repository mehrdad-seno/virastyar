namespace VirastyarWordAddin.Update
{
    partial class UpdateNotificationWindow
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
            this.txtChangeLog = new System.Windows.Forms.TextBox();
            this.lblUpdateNotice = new System.Windows.Forms.Label();
            this.lblGetUpdate = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLatestVersion = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlUpdateAvailable = new System.Windows.Forms.Panel();
            this.pnlChecking = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlNotAvialable = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlUpdateAvailable.SuspendLayout();
            this.pnlChecking.SuspendLayout();
            this.pnlNotAvialable.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtChangeLog
            // 
            this.txtChangeLog.AcceptsReturn = true;
            this.txtChangeLog.AcceptsTab = true;
            this.txtChangeLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChangeLog.BackColor = System.Drawing.Color.White;
            this.txtChangeLog.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChangeLog.Location = new System.Drawing.Point(10, 32);
            this.txtChangeLog.Multiline = true;
            this.txtChangeLog.Name = "txtChangeLog";
            this.txtChangeLog.ReadOnly = true;
            this.txtChangeLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtChangeLog.Size = new System.Drawing.Size(405, 133);
            this.txtChangeLog.TabIndex = 2;
            // 
            // lblUpdateNotice
            // 
            this.lblUpdateNotice.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblUpdateNotice.Location = new System.Drawing.Point(0, 0);
            this.lblUpdateNotice.Name = "lblUpdateNotice";
            this.lblUpdateNotice.Size = new System.Drawing.Size(425, 29);
            this.lblUpdateNotice.TabIndex = 3;
            this.lblUpdateNotice.Text = "    نسخه جدید ویراستیار از سایت قابل دریافت است. این نسخه شامل تغییرات زیر است:";
            this.lblUpdateNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGetUpdate
            // 
            this.lblGetUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGetUpdate.Location = new System.Drawing.Point(12, 216);
            this.lblGetUpdate.Name = "lblGetUpdate";
            this.lblGetUpdate.Size = new System.Drawing.Size(400, 29);
            this.lblGetUpdate.TabIndex = 8;
            this.lblGetUpdate.Text = "آیا مایلید برنامهٔ به‌روز رسان ویراستیار را اجرا کنید؟";
            this.lblGetUpdate.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnYes
            // 
            this.btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnYes.AutoSize = true;
            this.btnYes.Location = new System.Drawing.Point(10, 251);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(64, 23);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "بلی";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNo.AutoSize = true;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNo.Location = new System.Drawing.Point(80, 251);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(64, 23);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "خیر";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(323, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "نسخه نصب شده:";
            // 
            // lblInstalledVersion
            // 
            this.lblInstalledVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstalledVersion.AutoSize = true;
            this.lblInstalledVersion.Location = new System.Drawing.Point(279, 174);
            this.lblInstalledVersion.Name = "lblInstalledVersion";
            this.lblInstalledVersion.Size = new System.Drawing.Size(0, 13);
            this.lblInstalledVersion.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(348, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "نسخه جدید:";
            // 
            // lblLatestVersion
            // 
            this.lblLatestVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLatestVersion.AutoSize = true;
            this.lblLatestVersion.ForeColor = System.Drawing.Color.Red;
            this.lblLatestVersion.Location = new System.Drawing.Point(279, 198);
            this.lblLatestVersion.Name = "lblLatestVersion";
            this.lblLatestVersion.Size = new System.Drawing.Size(0, 13);
            this.lblLatestVersion.TabIndex = 7;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.pnlUpdateAvailable);
            this.flowLayoutPanel1.Controls.Add(this.pnlChecking);
            this.flowLayoutPanel1.Controls.Add(this.pnlNotAvialable);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(428, 372);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // pnlUpdateAvailable
            // 
            this.pnlUpdateAvailable.Controls.Add(this.lblGetUpdate);
            this.pnlUpdateAvailable.Controls.Add(this.lblLatestVersion);
            this.pnlUpdateAvailable.Controls.Add(this.lblUpdateNotice);
            this.pnlUpdateAvailable.Controls.Add(this.label3);
            this.pnlUpdateAvailable.Controls.Add(this.txtChangeLog);
            this.pnlUpdateAvailable.Controls.Add(this.lblInstalledVersion);
            this.pnlUpdateAvailable.Controls.Add(this.btnYes);
            this.pnlUpdateAvailable.Controls.Add(this.label1);
            this.pnlUpdateAvailable.Controls.Add(this.btnNo);
            this.pnlUpdateAvailable.Location = new System.Drawing.Point(0, 3);
            this.pnlUpdateAvailable.Name = "pnlUpdateAvailable";
            this.pnlUpdateAvailable.Size = new System.Drawing.Size(425, 282);
            this.pnlUpdateAvailable.TabIndex = 0;
            this.pnlUpdateAvailable.Visible = false;
            // 
            // pnlChecking
            // 
            this.pnlChecking.Controls.Add(this.progressBar);
            this.pnlChecking.Controls.Add(this.label2);
            this.pnlChecking.Location = new System.Drawing.Point(0, 291);
            this.pnlChecking.Name = "pnlChecking";
            this.pnlChecking.Size = new System.Drawing.Size(425, 35);
            this.pnlChecking.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(3, 5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(283, 23);
            this.progressBar.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(260, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "در حال بررسی ...";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlNotAvialable
            // 
            this.pnlNotAvialable.Controls.Add(this.label5);
            this.pnlNotAvialable.Controls.Add(this.btnClose);
            this.pnlNotAvialable.Location = new System.Drawing.Point(0, 332);
            this.pnlNotAvialable.Name = "pnlNotAvialable";
            this.pnlNotAvialable.Size = new System.Drawing.Size(425, 35);
            this.pnlNotAvialable.TabIndex = 2;
            this.pnlNotAvialable.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(133, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(280, 23);
            this.label5.TabIndex = 11;
            this.label5.Text = "نسخه جدیدی از ویراستیار یافت نشد";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.AutoSize = true;
            this.btnClose.Location = new System.Drawing.Point(10, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "خروج";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // UpdateNotificationWindow
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(428, 372);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateNotificationWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "به‌روز رسانی ویراستیار";
            this.Load += new System.EventHandler(this.UpdateNotificationWindow_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlUpdateAvailable.ResumeLayout(false);
            this.pnlUpdateAvailable.PerformLayout();
            this.pnlChecking.ResumeLayout(false);
            this.pnlNotAvialable.ResumeLayout(false);
            this.pnlNotAvialable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtChangeLog;
        private System.Windows.Forms.Label lblUpdateNotice;
        private System.Windows.Forms.Label lblGetUpdate;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInstalledVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLatestVersion;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel pnlUpdateAvailable;
        private System.Windows.Forms.Panel pnlChecking;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlNotAvialable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnClose;
    }
}
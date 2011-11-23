namespace VirastyarWordAddin.Configurations
{
    partial class AutomaticReportConfirmWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutomaticReportConfirmWindow));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.linkLabelViewGatheredInfo = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.rdoSendReportDecline = new System.Windows.Forms.RadioButton();
            this.rdoSendReportAccept = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelContent.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(340, 13);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "تأیید";
            this.toolTip.SetToolTip(this.btnOK, "پس از تأیید نیز می‌توانید انتخاب خود را از طریق پنجرهٔ تنظیمات، سربرگ «افزونه» تغ" +
                    "ییر دهید");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(259, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "لغو";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // linkLabelViewGatheredInfo
            // 
            this.linkLabelViewGatheredInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelViewGatheredInfo.AutoSize = true;
            this.linkLabelViewGatheredInfo.Location = new System.Drawing.Point(280, 169);
            this.linkLabelViewGatheredInfo.Name = "linkLabelViewGatheredInfo";
            this.linkLabelViewGatheredInfo.Size = new System.Drawing.Size(149, 14);
            this.linkLabelViewGatheredInfo.TabIndex = 6;
            this.linkLabelViewGatheredInfo.TabStop = true;
            this.linkLabelViewGatheredInfo.Text = "» مشاهده اطلاعاتِ ارسالی";
            this.linkLabelViewGatheredInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelViewGatheredInfo_LinkClicked);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(10, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(419, 97);
            this.label3.TabIndex = 5;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // rdoSendReportDecline
            // 
            this.rdoSendReportDecline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdoSendReportDecline.AutoSize = true;
            this.rdoSendReportDecline.Location = new System.Drawing.Point(322, 225);
            this.rdoSendReportDecline.Name = "rdoSendReportDecline";
            this.rdoSendReportDecline.Size = new System.Drawing.Size(91, 18);
            this.rdoSendReportDecline.TabIndex = 4;
            this.rdoSendReportDecline.Text = "موافق نیستم";
            this.rdoSendReportDecline.UseVisualStyleBackColor = true;
            // 
            // rdoSendReportAccept
            // 
            this.rdoSendReportAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rdoSendReportAccept.AutoSize = true;
            this.rdoSendReportAccept.Checked = true;
            this.rdoSendReportAccept.Location = new System.Drawing.Point(355, 201);
            this.rdoSendReportAccept.Name = "rdoSendReportAccept";
            this.rdoSendReportAccept.Size = new System.Drawing.Size(58, 18);
            this.rdoSendReportAccept.TabIndex = 3;
            this.rdoSendReportAccept.TabStop = true;
            this.rdoSendReportAccept.Text = "موافقم";
            this.rdoSendReportAccept.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(422, 43);
            this.label2.TabIndex = 2;
            this.label2.Text = "ویراستیار می‌تواند بصورت خودکار گزارشی از عملکرد و خطاهای رخ‌داده در برنامه را جم" +
                "ع‌آوری و ارسال کند.\r\nبا فعال‌سازی این قابلیت می‌توانید به بهبود ویراستیار کمک کن" +
                "ید.";
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 200;
            this.toolTip.IsBalloon = true;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.Controls.Add(this.linkLabelViewGatheredInfo);
            this.panelContent.Controls.Add(this.label2);
            this.panelContent.Controls.Add(this.label3);
            this.panelContent.Controls.Add(this.rdoSendReportAccept);
            this.panelContent.Controls.Add(this.rdoSendReportDecline);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(434, 264);
            this.panelContent.TabIndex = 9;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnOK);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 264);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(434, 48);
            this.panelBottom.TabIndex = 10;
            // 
            // AutomaticReportConfirmWindow
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(434, 312);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 350);
            this.Name = "AutomaticReportConfirmWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "کمک به بهبود ویراستیار";
            this.Load += new System.EventHandler(this.AutomaticReportConfirmWindow_Load);
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelViewGatheredInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdoSendReportDecline;
        private System.Windows.Forms.RadioButton rdoSendReportAccept;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelBottom;

    }
}
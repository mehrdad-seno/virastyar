// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

namespace VirastyarWordAddin
{
    partial class VerificationWindowBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerificationWindowBase));
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlSuggestionArea = new System.Windows.Forms.Panel();
            this.pnlItems = new System.Windows.Forms.Panel();
            this.rtbMainContent = new System.Windows.Forms.RichTextBox();
            this.lblContentCaption = new System.Windows.Forms.Label();
            this.panelVerificationMode = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnAddToDic = new System.Windows.Forms.Button();
            this.btnStopVerify = new System.Windows.Forms.Button();
            this.btnIgnoreAll = new System.Windows.Forms.Button();
            this.btnChangeAll = new System.Windows.Forms.Button();
            this.panelProgressMode = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnStopProgress = new System.Windows.Forms.Button();
            this.panelConfirmation = new System.Windows.Forms.Panel();
            this.btnConfirmNo = new System.Windows.Forms.Button();
            this.btnConfirmYes = new System.Windows.Forms.Button();
            this.lblConfirmation = new System.Windows.Forms.Label();
            this.pnlRight.SuspendLayout();
            this.pnlItems.SuspendLayout();
            this.panelVerificationMode.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.panelProgressMode.SuspendLayout();
            this.panelConfirmation.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRight
            // 
            this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRight.Controls.Add(this.pnlSuggestionArea);
            this.pnlRight.Controls.Add(this.pnlItems);
            this.pnlRight.Location = new System.Drawing.Point(122, 6);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(474, 388);
            this.pnlRight.TabIndex = 20;
            // 
            // pnlSuggestionArea
            // 
            this.pnlSuggestionArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSuggestionArea.Location = new System.Drawing.Point(0, 155);
            this.pnlSuggestionArea.Name = "pnlSuggestionArea";
            this.pnlSuggestionArea.Size = new System.Drawing.Size(474, 233);
            this.pnlSuggestionArea.TabIndex = 0;
            // 
            // pnlItems
            // 
            this.pnlItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlItems.Controls.Add(this.rtbMainContent);
            this.pnlItems.Controls.Add(this.lblContentCaption);
            this.pnlItems.Location = new System.Drawing.Point(0, 0);
            this.pnlItems.Name = "pnlItems";
            this.pnlItems.Size = new System.Drawing.Size(474, 155);
            this.pnlItems.TabIndex = 24;
            // 
            // rtbMainContent
            // 
            this.rtbMainContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbMainContent.BackColor = System.Drawing.SystemColors.Window;
            this.rtbMainContent.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbMainContent.Location = new System.Drawing.Point(0, 16);
            this.rtbMainContent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbMainContent.Name = "rtbMainContent";
            this.rtbMainContent.ReadOnly = true;
            this.rtbMainContent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.rtbMainContent.Size = new System.Drawing.Size(474, 139);
            this.rtbMainContent.TabIndex = 0;
            this.rtbMainContent.Text = "";
            // 
            // lblContentCaption
            // 
            this.lblContentCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContentCaption.Location = new System.Drawing.Point(0, 0);
            this.lblContentCaption.Name = "lblContentCaption";
            this.lblContentCaption.Size = new System.Drawing.Size(474, 16);
            this.lblContentCaption.TabIndex = 23;
            this.lblContentCaption.Text = "خطاهای یافت شده:";
            // 
            // panelVerificationMode
            // 
            this.panelVerificationMode.Controls.Add(this.pnlLeft);
            this.panelVerificationMode.Controls.Add(this.pnlRight);
            this.panelVerificationMode.Location = new System.Drawing.Point(0, 0);
            this.panelVerificationMode.Name = "panelVerificationMode";
            this.panelVerificationMode.Size = new System.Drawing.Size(599, 397);
            this.panelVerificationMode.TabIndex = 21;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlLeft.Controls.Add(this.btnIgnore);
            this.pnlLeft.Controls.Add(this.btnChange);
            this.pnlLeft.Controls.Add(this.btnAddToDic);
            this.pnlLeft.Controls.Add(this.btnStopVerify);
            this.pnlLeft.Controls.Add(this.btnIgnoreAll);
            this.pnlLeft.Controls.Add(this.btnChangeAll);
            this.pnlLeft.Location = new System.Drawing.Point(2, 22);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(114, 368);
            this.pnlLeft.TabIndex = 21;
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(8, 1);
            this.btnIgnore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(102, 28);
            this.btnIgnore.TabIndex = 6;
            this.btnIgnore.Text = "نادیده گرفتن";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnChange
            // 
            this.btnChange.Location = new System.Drawing.Point(8, 160);
            this.btnChange.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(102, 28);
            this.btnChange.TabIndex = 9;
            this.btnChange.Text = "تغییر";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // btnAddToDic
            // 
            this.btnAddToDic.Location = new System.Drawing.Point(8, 73);
            this.btnAddToDic.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddToDic.Name = "btnAddToDic";
            this.btnAddToDic.Size = new System.Drawing.Size(102, 28);
            this.btnAddToDic.TabIndex = 8;
            this.btnAddToDic.Text = "افزودن به واژه‌نامه";
            this.btnAddToDic.UseVisualStyleBackColor = true;
            this.btnAddToDic.Click += new System.EventHandler(this.btnAddToDic_Click);
            // 
            // btnStopVerify
            // 
            this.btnStopVerify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopVerify.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStopVerify.Location = new System.Drawing.Point(8, 343);
            this.btnStopVerify.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStopVerify.Name = "btnStopVerify";
            this.btnStopVerify.Size = new System.Drawing.Size(102, 25);
            this.btnStopVerify.TabIndex = 11;
            this.btnStopVerify.Text = "پایان";
            this.btnStopVerify.UseVisualStyleBackColor = true;
            this.btnStopVerify.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnIgnoreAll
            // 
            this.btnIgnoreAll.Location = new System.Drawing.Point(8, 37);
            this.btnIgnoreAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnIgnoreAll.Name = "btnIgnoreAll";
            this.btnIgnoreAll.Size = new System.Drawing.Size(102, 28);
            this.btnIgnoreAll.TabIndex = 7;
            this.btnIgnoreAll.Text = "نادیده گرفتن همه";
            this.btnIgnoreAll.UseVisualStyleBackColor = true;
            this.btnIgnoreAll.Click += new System.EventHandler(this.btnIgnoreAll_Click);
            // 
            // btnChangeAll
            // 
            this.btnChangeAll.Location = new System.Drawing.Point(8, 195);
            this.btnChangeAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChangeAll.Name = "btnChangeAll";
            this.btnChangeAll.Size = new System.Drawing.Size(102, 28);
            this.btnChangeAll.TabIndex = 10;
            this.btnChangeAll.Text = "تغییر همه";
            this.btnChangeAll.UseVisualStyleBackColor = true;
            this.btnChangeAll.Click += new System.EventHandler(this.btnChangeAll_Click);
            // 
            // panelProgressMode
            // 
            this.panelProgressMode.Controls.Add(this.progressBar);
            this.panelProgressMode.Controls.Add(this.btnStopProgress);
            this.panelProgressMode.Location = new System.Drawing.Point(0, 398);
            this.panelProgressMode.MinimumSize = new System.Drawing.Size(577, 62);
            this.panelProgressMode.Name = "panelProgressMode";
            this.panelProgressMode.Size = new System.Drawing.Size(599, 62);
            this.panelProgressMode.TabIndex = 22;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(92, 21);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(501, 23);
            this.progressBar.TabIndex = 7;
            // 
            // btnStopProgress
            // 
            this.btnStopProgress.Location = new System.Drawing.Point(6, 21);
            this.btnStopProgress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStopProgress.Name = "btnStopProgress";
            this.btnStopProgress.Size = new System.Drawing.Size(80, 25);
            this.btnStopProgress.TabIndex = 6;
            this.btnStopProgress.Text = "پایان";
            this.btnStopProgress.UseVisualStyleBackColor = true;
            this.btnStopProgress.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // panelConfirmation
            // 
            this.panelConfirmation.Controls.Add(this.btnConfirmNo);
            this.panelConfirmation.Controls.Add(this.btnConfirmYes);
            this.panelConfirmation.Controls.Add(this.lblConfirmation);
            this.panelConfirmation.Location = new System.Drawing.Point(0, 460);
            this.panelConfirmation.MinimumSize = new System.Drawing.Size(577, 79);
            this.panelConfirmation.Name = "panelConfirmation";
            this.panelConfirmation.Size = new System.Drawing.Size(599, 79);
            this.panelConfirmation.TabIndex = 23;
            // 
            // btnConfirmNo
            // 
            this.btnConfirmNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmNo.Location = new System.Drawing.Point(379, 41);
            this.btnConfirmNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfirmNo.Name = "btnConfirmNo";
            this.btnConfirmNo.Size = new System.Drawing.Size(102, 25);
            this.btnConfirmNo.TabIndex = 8;
            this.btnConfirmNo.Text = "خیر";
            this.btnConfirmNo.UseVisualStyleBackColor = true;
            this.btnConfirmNo.Click += new System.EventHandler(this.btnConfirmNo_Click);
            // 
            // btnConfirmYes
            // 
            this.btnConfirmYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmYes.Location = new System.Drawing.Point(487, 41);
            this.btnConfirmYes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfirmYes.Name = "btnConfirmYes";
            this.btnConfirmYes.Size = new System.Drawing.Size(102, 25);
            this.btnConfirmYes.TabIndex = 7;
            this.btnConfirmYes.Text = "بله";
            this.btnConfirmYes.UseVisualStyleBackColor = true;
            this.btnConfirmYes.Click += new System.EventHandler(this.btnConfirmYes_Click);
            // 
            // lblConfirmation
            // 
            this.lblConfirmation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConfirmation.AutoSize = true;
            this.lblConfirmation.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblConfirmation.Location = new System.Drawing.Point(206, 11);
            this.lblConfirmation.MinimumSize = new System.Drawing.Size(386, 16);
            this.lblConfirmation.Name = "lblConfirmation";
            this.lblConfirmation.Size = new System.Drawing.Size(386, 16);
            this.lblConfirmation.TabIndex = 0;
            this.lblConfirmation.Text = "برنامه به انتهای متن رسید. آیا عملیات از ابتدای متن مجددا آغاز شود؟";
            // 
            // VerificationWindowBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnStopVerify;
            this.ClientSize = new System.Drawing.Size(599, 539);
            this.ControlBox = false;
            this.Controls.Add(this.panelVerificationMode);
            this.Controls.Add(this.panelProgressMode);
            this.Controls.Add(this.panelConfirmation);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 100);
            this.Name = "VerificationWindowBase";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "عنوان";
            this.Load += new System.EventHandler(this.VerificationWindowBase_Load);
            this.Shown += new System.EventHandler(this.VerificationWindowBase_Shown);
            this.VisibleChanged += new System.EventHandler(this.VerificationWindowBase_VisibleChanged);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VerificationWindowBase_KeyUp);
            this.pnlRight.ResumeLayout(false);
            this.pnlItems.ResumeLayout(false);
            this.panelVerificationMode.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.panelProgressMode.ResumeLayout(false);
            this.panelConfirmation.ResumeLayout(false);
            this.panelConfirmation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected internal System.Windows.Forms.Panel pnlSuggestionArea;
        protected internal System.Windows.Forms.Label lblContentCaption;
        protected internal System.Windows.Forms.RichTextBox rtbMainContent;
        protected System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblConfirmation;
        protected internal System.Windows.Forms.Panel pnlItems;
        protected internal System.Windows.Forms.Panel panelVerificationMode;
        protected internal System.Windows.Forms.Panel panelProgressMode;
        protected internal System.Windows.Forms.Panel panelConfirmation;
        protected System.Windows.Forms.Panel pnlLeft;
        protected internal System.Windows.Forms.Button btnIgnore;
        protected internal System.Windows.Forms.Button btnChange;
        protected internal System.Windows.Forms.Button btnAddToDic;
        protected internal System.Windows.Forms.Button btnStopVerify;
        protected internal System.Windows.Forms.Button btnIgnoreAll;
        protected internal System.Windows.Forms.Button btnChangeAll;
        public System.Windows.Forms.Button btnStopProgress;
        private System.Windows.Forms.Button btnConfirmNo;
        private System.Windows.Forms.Button btnConfirmYes;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
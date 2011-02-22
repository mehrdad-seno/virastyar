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
    partial class TipOfTheDayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TipOfTheDayForm));
            this.rtbTip = new System.Windows.Forms.RichTextBox();
            this.linkLabelOuterLink = new System.Windows.Forms.LinkLabel();
            this.panelRightImage = new System.Windows.Forms.Panel();
            this.pictureBoxTip = new System.Windows.Forms.PictureBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.cbShowOnStartup = new System.Windows.Forms.CheckBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelRightImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTip)).BeginInit();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbTip
            // 
            this.rtbTip.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbTip.BackColor = System.Drawing.Color.White;
            this.rtbTip.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbTip.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbTip.Location = new System.Drawing.Point(4, 55);
            this.rtbTip.Name = "rtbTip";
            this.rtbTip.ReadOnly = true;
            this.rtbTip.Size = new System.Drawing.Size(386, 155);
            this.rtbTip.TabIndex = 1;
            this.rtbTip.Text = "";
            this.rtbTip.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbTip_LinkClicked);
            // 
            // linkLabelOuterLink
            // 
            this.linkLabelOuterLink.AutoSize = true;
            this.linkLabelOuterLink.LinkArea = new System.Windows.Forms.LinkArea(19, 6);
            this.linkLabelOuterLink.Location = new System.Drawing.Point(4, 15);
            this.linkLabelOuterLink.Name = "linkLabelOuterLink";
            this.linkLabelOuterLink.Size = new System.Drawing.Size(184, 19);
            this.linkLabelOuterLink.TabIndex = 6;
            this.linkLabelOuterLink.TabStop = true;
            this.linkLabelOuterLink.Text = "برای اطلاعات بیشتر این‌جا را ببینید.";
            this.linkLabelOuterLink.UseCompatibleTextRendering = true;
            this.linkLabelOuterLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOuterLink_LinkClicked);
            // 
            // panelRightImage
            // 
            this.panelRightImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelRightImage.Controls.Add(this.pictureBoxTip);
            this.panelRightImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRightImage.Location = new System.Drawing.Point(396, 0);
            this.panelRightImage.Name = "panelRightImage";
            this.panelRightImage.Size = new System.Drawing.Size(98, 213);
            this.panelRightImage.TabIndex = 7;
            // 
            // pictureBoxTip
            // 
            this.pictureBoxTip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTip.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxTip.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTip.Image")));
            this.pictureBoxTip.Location = new System.Drawing.Point(-19, 0);
            this.pictureBoxTip.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxTip.Name = "pictureBoxTip";
            this.pictureBoxTip.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxTip.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxTip.TabIndex = 0;
            this.pictureBoxTip.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panelBottom.Controls.Add(this.cbShowOnStartup);
            this.panelBottom.Controls.Add(this.btnNext);
            this.panelBottom.Controls.Add(this.btnPrevious);
            this.panelBottom.Controls.Add(this.btnStop);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 213);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(494, 46);
            this.panelBottom.TabIndex = 8;
            // 
            // cbShowOnStartup
            // 
            this.cbShowOnStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbShowOnStartup.AutoSize = true;
            this.cbShowOnStartup.Location = new System.Drawing.Point(358, 14);
            this.cbShowOnStartup.Name = "cbShowOnStartup";
            this.cbShowOnStartup.Size = new System.Drawing.Size(130, 18);
            this.cbShowOnStartup.TabIndex = 9;
            this.cbShowOnStartup.Text = "نمایش در ابتدای اجرا";
            this.cbShowOnStartup.UseVisualStyleBackColor = true;
            this.cbShowOnStartup.CheckedChanged += new System.EventHandler(this.cbShowOnStartup_CheckedChanged);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.Location = new System.Drawing.Point(113, 12);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 8;
            this.btnNext.Text = "بعدی";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrevious.Location = new System.Drawing.Point(194, 12);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 7;
            this.btnPrevious.Text = "قبلی";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(12, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "پایان";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(246, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 39);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // TipOfTheDayForm
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(494, 259);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panelRightImage);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.linkLabelOuterLink);
            this.Controls.Add(this.rtbTip);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(510, 250);
            this.Name = "TipOfTheDayForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ویراستیار";
            this.panelRightImage.ResumeLayout(false);
            this.panelRightImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTip)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbTip;
        private System.Windows.Forms.LinkLabel linkLabelOuterLink;
        private System.Windows.Forms.Panel panelRightImage;
        private System.Windows.Forms.PictureBox pictureBoxTip;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.CheckBox cbShowOnStartup;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
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
    partial class SpellCheckerVerificationWindow
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
            this.lstSuggestions = new System.Windows.Forms.ListBox();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.btnAddCustomToDic = new System.Windows.Forms.Button();
            this.txtCustomSuggestion = new System.Windows.Forms.TextBox();
            this.lblSuggestionCaption = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusSugs = new System.Windows.Forms.StatusStrip();
            this.statusSugsLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusSugsLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlSuggestionArea.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlItems.SuspendLayout();
            this.panelVerificationMode.SuspendLayout();
            this.panelProgressMode.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.statusSugs.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSuggestionArea
            // 
            this.pnlSuggestionArea.Controls.Add(this.lstSuggestions);
            this.pnlSuggestionArea.Controls.Add(this.pnlMiddle);
            // 
            // panelVerificationMode
            // 
            this.panelVerificationMode.Controls.Add(this.statusSugs);
            this.panelVerificationMode.Location = new System.Drawing.Point(0, 5);
            this.panelVerificationMode.Controls.SetChildIndex(this.pnlLeft, 0);
            this.panelVerificationMode.Controls.SetChildIndex(this.statusSugs, 0);
            this.panelVerificationMode.Controls.SetChildIndex(this.pnlRight, 0);
            // 
            // panelProgressMode
            // 
            this.panelProgressMode.Location = new System.Drawing.Point(0, 5);
            // 
            // btnStopProgress
            // 
            this.btnStopProgress.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSuggestions.FormattingEnabled = true;
            this.lstSuggestions.Location = new System.Drawing.Point(0, 29);
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.Size = new System.Drawing.Size(474, 199);
            this.lstSuggestions.TabIndex = 1;
            this.lstSuggestions.SelectedIndexChanged += new System.EventHandler(this.lstSuggestions_SelectedIndexChanged);
            this.lstSuggestions.DoubleClick += new System.EventHandler(this.lstSuggestions_DoubleClick);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.btnAddCustomToDic);
            this.pnlMiddle.Controls.Add(this.txtCustomSuggestion);
            this.pnlMiddle.Controls.Add(this.lblSuggestionCaption);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 0);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(474, 29);
            this.pnlMiddle.TabIndex = 3;
            // 
            // btnAddCustomToDic
            // 
            this.btnAddCustomToDic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAddCustomToDic.Location = new System.Drawing.Point(0, 4);
            this.btnAddCustomToDic.Name = "btnAddCustomToDic";
            this.btnAddCustomToDic.Size = new System.Drawing.Size(18, 22);
            this.btnAddCustomToDic.TabIndex = 25;
            this.btnAddCustomToDic.Text = "+";
            this.toolTip1.SetToolTip(this.btnAddCustomToDic, "افزودن عبارت درج شده به واژه‌نامه");
            this.btnAddCustomToDic.UseVisualStyleBackColor = true;
            this.btnAddCustomToDic.Click += new System.EventHandler(this.btnAddCustomToDic_Click);
            // 
            // txtCustomSuggestion
            // 
            this.txtCustomSuggestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.txtCustomSuggestion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustomSuggestion.Location = new System.Drawing.Point(24, 5);
            this.txtCustomSuggestion.Name = "txtCustomSuggestion";
            this.txtCustomSuggestion.Size = new System.Drawing.Size(114, 21);
            this.txtCustomSuggestion.TabIndex = 4;
            this.toolTip1.SetToolTip(this.txtCustomSuggestion, "جهت درج نیم‌فاصله از ترکیب کنترل + شیفت + ۲ استفاده کنید.");
            this.txtCustomSuggestion.TextChanged += new System.EventHandler(this.txtCustomSuggestion_TextChanged);
            this.txtCustomSuggestion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomSuggestion_KeyDown);
            // 
            // lblSuggestionCaption
            // 
            this.lblSuggestionCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSuggestionCaption.Location = new System.Drawing.Point(292, 5);
            this.lblSuggestionCaption.Name = "lblSuggestionCaption";
            this.lblSuggestionCaption.Size = new System.Drawing.Size(182, 20);
            this.lblSuggestionCaption.TabIndex = 3;
            this.lblSuggestionCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusSugs
            // 
            this.statusSugs.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSugs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusSugsLabel1,
            this.statusSugsLabel2});
            this.statusSugs.Location = new System.Drawing.Point(0, 375);
            this.statusSugs.Name = "statusSugs";
            this.statusSugs.Size = new System.Drawing.Size(599, 22);
            this.statusSugs.SizingGrip = false;
            this.statusSugs.TabIndex = 24;
            this.statusSugs.Text = "statusStrip1";
            this.statusSugs.Visible = false;
            // 
            // statusSugsLabel1
            // 
            this.statusSugsLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusSugsLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.statusSugsLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSugsLabel1.Name = "statusSugsLabel1";
            this.statusSugsLabel1.Size = new System.Drawing.Size(4, 17);
            // 
            // statusSugsLabel2
            // 
            this.statusSugsLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.statusSugsLabel2.Name = "statusSugsLabel2";
            this.statusSugsLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // SpellCheckerVerificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnStopProgress;
            this.ClientSize = new System.Drawing.Size(598, 295);
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "SpellCheckerVerificationWindow";
            this.pnlSuggestionArea.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlItems.ResumeLayout(false);
            this.panelVerificationMode.ResumeLayout(false);
            this.panelVerificationMode.PerformLayout();
            this.panelProgressMode.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlMiddle.ResumeLayout(false);
            this.pnlMiddle.PerformLayout();
            this.statusSugs.ResumeLayout(false);
            this.statusSugs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstSuggestions;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Label lblSuggestionCaption;
        private System.Windows.Forms.TextBox txtCustomSuggestion;
        private System.Windows.Forms.Button btnAddCustomToDic;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.StatusStrip statusSugs;
        private System.Windows.Forms.ToolStripStatusLabel statusSugsLabel1;
        private System.Windows.Forms.ToolStripStatusLabel statusSugsLabel2;
    }
}

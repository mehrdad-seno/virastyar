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
    partial class SimpleVerificationWindow
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
            this.lblSuggestionCaption = new System.Windows.Forms.Label();
            this.lstSuggestions = new System.Windows.Forms.ListBox();
            this.pnlSuggestionArea.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlItems.SuspendLayout();
            this.panelVerificationMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSuggestionArea
            // 
            this.pnlSuggestionArea.Controls.Add(this.lstSuggestions);
            this.pnlSuggestionArea.Controls.Add(this.lblSuggestionCaption);
            // 
            // panelProgressMode
            // 
            this.panelProgressMode.Location = new System.Drawing.Point(0, 5);
            // 
            // lblSuggestionCaption
            // 
            this.lblSuggestionCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSuggestionCaption.Location = new System.Drawing.Point(0, 0);
            this.lblSuggestionCaption.Name = "lblSuggestionCaption";
            this.lblSuggestionCaption.Size = new System.Drawing.Size(474, 30);
            this.lblSuggestionCaption.TabIndex = 2;
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSuggestions.FormattingEnabled = true;
            this.lstSuggestions.Location = new System.Drawing.Point(0, 30);
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.Size = new System.Drawing.Size(474, 199);
            this.lstSuggestions.TabIndex = 1;
            this.lstSuggestions.DoubleClick += new System.EventHandler(this.lstSuggestions_DoubleClick);
            // 
            // SpellCheckerVerificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(598, 295);
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "SpellCheckerVerificationWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pnlSuggestionArea.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlItems.ResumeLayout(false);
            this.panelVerificationMode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSuggestionCaption;
        private System.Windows.Forms.ListBox lstSuggestions;
    }
}

namespace VirastyarWordAddin
{
    partial class DateVerificationWindow
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("تبدیل به تاریخ شمسی", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("تبدیل به تاریخ میلادی", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("تبدیل به تاریخ قمری", System.Windows.Forms.HorizontalAlignment.Left);
            this.lstSuggestions = new System.Windows.Forms.ListView();
            this.columnHeaderExpression = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDateType = new System.Windows.Forms.ColumnHeader();
            this.pnlSubSuggestion = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboYearNumber = new System.Windows.Forms.ComboBox();
            this.cmboGuessedCalendarType = new System.Windows.Forms.ComboBox();
            this.pnlSuggestionArea.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlItems.SuspendLayout();
            this.panelVerificationMode.SuspendLayout();
            this.pnlSubSuggestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSuggestionArea
            // 
            this.pnlSuggestionArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlSuggestionArea.Controls.Add(this.lstSuggestions);
            this.pnlSuggestionArea.Controls.Add(this.pnlSubSuggestion);
            this.pnlSuggestionArea.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // pnlItems
            // 
            this.pnlItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlItems.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // panelProgressMode
            // 
            this.panelProgressMode.Location = new System.Drawing.Point(0, 5);
            // 
            // btnAddToDic
            // 
            this.btnAddToDic.Visible = false;
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderExpression,
            this.columnHeaderDateType});
            this.lstSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSuggestions.FullRowSelect = true;
            this.lstSuggestions.GridLines = true;
            listViewGroup1.Header = "تبدیل به تاریخ شمسی";
            listViewGroup1.Name = "listViewGroupJalali";
            listViewGroup2.Header = "تبدیل به تاریخ میلادی";
            listViewGroup2.Name = "listViewGroupGregorian";
            listViewGroup3.Header = "تبدیل به تاریخ قمری";
            listViewGroup3.Name = "listViewGroupGhamari";
            this.lstSuggestions.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.lstSuggestions.HideSelection = false;
            this.lstSuggestions.Location = new System.Drawing.Point(0, 34);
            this.lstSuggestions.MultiSelect = false;
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.RightToLeftLayout = true;
            this.lstSuggestions.ShowItemToolTips = true;
            this.lstSuggestions.Size = new System.Drawing.Size(474, 199);
            this.lstSuggestions.TabIndex = 17;
            this.lstSuggestions.UseCompatibleStateImageBehavior = false;
            this.lstSuggestions.View = System.Windows.Forms.View.Details;
            this.lstSuggestions.DoubleClick += new System.EventHandler(this.lstSuggestions_DoubleClick);
            // 
            // columnHeaderExpression
            // 
            this.columnHeaderExpression.Text = "عبارت";
            this.columnHeaderExpression.Width = 369;
            // 
            // columnHeaderDateType
            // 
            this.columnHeaderDateType.Text = "نوع تاریخ";
            this.columnHeaderDateType.Width = 158;
            // 
            // pnlSubSuggestion
            // 
            this.pnlSubSuggestion.Controls.Add(this.label3);
            this.pnlSubSuggestion.Controls.Add(this.label2);
            this.pnlSubSuggestion.Controls.Add(this.comboYearNumber);
            this.pnlSubSuggestion.Controls.Add(this.cmboGuessedCalendarType);
            this.pnlSubSuggestion.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSubSuggestion.Location = new System.Drawing.Point(0, 0);
            this.pnlSubSuggestion.Name = "pnlSubSuggestion";
            this.pnlSubSuggestion.Size = new System.Drawing.Size(474, 34);
            this.pnlSubSuggestion.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "سال:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(417, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "نوع تقویم:";
            // 
            // comboYearNumber
            // 
            this.comboYearNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboYearNumber.Enabled = false;
            this.comboYearNumber.FormattingEnabled = true;
            this.comboYearNumber.Location = new System.Drawing.Point(120, 6);
            this.comboYearNumber.Name = "comboYearNumber";
            this.comboYearNumber.Size = new System.Drawing.Size(107, 21);
            this.comboYearNumber.TabIndex = 21;
            this.comboYearNumber.SelectedIndexChanged += new System.EventHandler(this.comboYearNumber_SelectedIndexChanged);
            this.comboYearNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboYearNumber_KeyUp);
            // 
            // cmboGuessedCalendarType
            // 
            this.cmboGuessedCalendarType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboGuessedCalendarType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboGuessedCalendarType.Enabled = false;
            this.cmboGuessedCalendarType.FormattingEnabled = true;
            this.cmboGuessedCalendarType.Items.AddRange(new object[] {
            "تقویم شمسی",
            "تقویم میلادی",
            "تقویم قمری"});
            this.cmboGuessedCalendarType.Location = new System.Drawing.Point(290, 6);
            this.cmboGuessedCalendarType.Name = "cmboGuessedCalendarType";
            this.cmboGuessedCalendarType.Size = new System.Drawing.Size(121, 21);
            this.cmboGuessedCalendarType.TabIndex = 20;
            this.cmboGuessedCalendarType.SelectedIndexChanged += new System.EventHandler(this.cmboGuessedCalendarType_SelectedIndexChanged);
            // 
            // DateVerificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(675, 470);
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "DateVerificationWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pnlSuggestionArea.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlItems.ResumeLayout(false);
            this.panelVerificationMode.ResumeLayout(false);
            this.pnlSubSuggestion.ResumeLayout(false);
            this.pnlSubSuggestion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstSuggestions;
        private System.Windows.Forms.ColumnHeader columnHeaderExpression;
        private System.Windows.Forms.ColumnHeader columnHeaderDateType;
        private System.Windows.Forms.Panel pnlSubSuggestion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboYearNumber;
        private System.Windows.Forms.ComboBox cmboGuessedCalendarType;
    }
}

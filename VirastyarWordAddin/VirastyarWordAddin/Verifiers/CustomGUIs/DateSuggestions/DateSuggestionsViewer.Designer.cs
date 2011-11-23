namespace VirastyarWordAddin.Verifiers.CustomGUIs.DateSuggestions
{
    partial class DateSuggestionsViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("تبدیل به تاریخ شمسی", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("تبدیل به تاریخ میلادی", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("تبدیل به تاریخ قمری", System.Windows.Forms.HorizontalAlignment.Left);
            this.lstSuggestions = new System.Windows.Forms.ListView();
            this.columnHeaderExpression = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDateType = new System.Windows.Forms.ColumnHeader();
            this.pnlSubSuggestion = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboYearNumber = new System.Windows.Forms.ComboBox();
            this.cmboGuessedCalendarType = new System.Windows.Forms.ComboBox();
            this.pnlSubSuggestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderExpression,
            this.columnHeaderDateType});
            this.lstSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSuggestions.FullRowSelect = true;
            this.lstSuggestions.GridLines = true;
            listViewGroup10.Header = "تبدیل به تاریخ شمسی";
            listViewGroup10.Name = "listViewGroupJalali";
            listViewGroup11.Header = "تبدیل به تاریخ میلادی";
            listViewGroup11.Name = "listViewGroupGregorian";
            listViewGroup12.Header = "تبدیل به تاریخ قمری";
            listViewGroup12.Name = "listViewGroupGhamari";
            this.lstSuggestions.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup10,
            listViewGroup11,
            listViewGroup12});
            this.lstSuggestions.HideSelection = false;
            this.lstSuggestions.Location = new System.Drawing.Point(0, 37);
            this.lstSuggestions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstSuggestions.MultiSelect = false;
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.RightToLeftLayout = true;
            this.lstSuggestions.ShowItemToolTips = true;
            this.lstSuggestions.Size = new System.Drawing.Size(465, 178);
            this.lstSuggestions.TabIndex = 18;
            this.lstSuggestions.UseCompatibleStateImageBehavior = false;
            this.lstSuggestions.View = System.Windows.Forms.View.Details;
            this.lstSuggestions.Resize += new System.EventHandler(this.lstSuggestions_Resize);
            this.lstSuggestions.SelectedIndexChanged += new System.EventHandler(this.lstSuggestions_SelectedIndexChanged);
            this.lstSuggestions.DoubleClick += new System.EventHandler(this.lstSuggestions_DoubleClick);
            // 
            // columnHeaderExpression
            // 
            this.columnHeaderExpression.Text = "عبارت";
            this.columnHeaderExpression.Width = 300;
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
            this.pnlSubSuggestion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlSubSuggestion.Name = "pnlSubSuggestion";
            this.pnlSubSuggestion.Size = new System.Drawing.Size(465, 37);
            this.pnlSubSuggestion.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(184, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 23;
            this.label3.Text = "سال:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 14);
            this.label2.TabIndex = 22;
            this.label2.Text = "نوع تقویم:";
            // 
            // comboYearNumber
            // 
            this.comboYearNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboYearNumber.Enabled = false;
            this.comboYearNumber.FormattingEnabled = true;
            this.comboYearNumber.Location = new System.Drawing.Point(52, 6);
            this.comboYearNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboYearNumber.Name = "comboYearNumber";
            this.comboYearNumber.Size = new System.Drawing.Size(124, 22);
            this.comboYearNumber.TabIndex = 21;
            this.comboYearNumber.SelectedIndexChanged += new System.EventHandler(this.comboYearNumber_SelectedIndexChanged);
            this.comboYearNumber.Leave += new System.EventHandler(this.comboYearNumber_Leave);
            this.comboYearNumber.Enter += new System.EventHandler(this.comboYearNumber_Enter);
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
            this.cmboGuessedCalendarType.Location = new System.Drawing.Point(251, 6);
            this.cmboGuessedCalendarType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmboGuessedCalendarType.Name = "cmboGuessedCalendarType";
            this.cmboGuessedCalendarType.Size = new System.Drawing.Size(140, 22);
            this.cmboGuessedCalendarType.TabIndex = 20;
            this.cmboGuessedCalendarType.SelectedIndexChanged += new System.EventHandler(this.cmboGuessedCalendarType_SelectedIndexChanged);
            // 
            // DateSuggestionsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstSuggestions);
            this.Controls.Add(this.pnlSubSuggestion);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(465, 215);
            this.Name = "DateSuggestionsViewer";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(465, 215);
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

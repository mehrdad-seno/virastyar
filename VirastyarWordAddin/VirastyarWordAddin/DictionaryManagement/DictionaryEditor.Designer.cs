namespace VirastyarWordAddin
{
    partial class DictionaryEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryEditor));
            this.DicGridView = new System.Windows.Forms.DataGridView();
            this.Word = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsNoun = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsVerb = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IsAdjective = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Ending = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DicElementBinding = new System.Windows.Forms.BindingSource(this.components);
            this.OuterTable = new System.Windows.Forms.TableLayoutPanel();
            this.AddWordGroupBox = new System.Windows.Forms.GroupBox();
            this.HelpEndingLink = new System.Windows.Forms.LinkLabel();
            this.EndingComboBox = new System.Windows.Forms.ComboBox();
            this.AddWordButton = new System.Windows.Forms.Button();
            this.IsAdjectiveCheckBox = new System.Windows.Forms.CheckBox();
            this.IsVerbCheckBox = new System.Windows.Forms.CheckBox();
            this.IsNounCheckBox = new System.Windows.Forms.CheckBox();
            this.NewWordText = new System.Windows.Forms.TextBox();
            this.EndingLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ReloadButton = new System.Windows.Forms.Button();
            this.DeleteCurrentButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.DeleteAllButton = new System.Windows.Forms.Button();
            this.MessageToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DicGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DicElementBinding)).BeginInit();
            this.OuterTable.SuspendLayout();
            this.AddWordGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // DicGridView
            // 
            this.DicGridView.AllowUserToAddRows = false;
            this.DicGridView.AllowUserToOrderColumns = true;
            this.DicGridView.AutoGenerateColumns = false;
            this.DicGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DicGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Word,
            this.IsNoun,
            this.IsVerb,
            this.IsAdjective,
            this.Ending});
            this.DicGridView.DataSource = this.DicElementBinding;
            this.DicGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DicGridView.Location = new System.Drawing.Point(3, 53);
            this.DicGridView.Name = "DicGridView";
            this.DicGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DicGridView.Size = new System.Drawing.Size(365, 182);
            this.DicGridView.StandardTab = true;
            this.DicGridView.TabIndex = 4;
            this.DicGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DicGridView_CellEndEdit);
            // 
            // Word
            // 
            this.Word.DataPropertyName = "Word";
            this.Word.Frozen = true;
            this.Word.HeaderText = "کلمه";
            this.Word.Name = "Word";
            this.Word.ReadOnly = true;
            this.Word.Width = 120;
            // 
            // IsNoun
            // 
            this.IsNoun.DataPropertyName = "IsNoun";
            this.IsNoun.Frozen = true;
            this.IsNoun.HeaderText = "اسم";
            this.IsNoun.Name = "IsNoun";
            this.IsNoun.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IsNoun.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsNoun.Width = 40;
            // 
            // IsVerb
            // 
            this.IsVerb.DataPropertyName = "IsVerb";
            this.IsVerb.Frozen = true;
            this.IsVerb.HeaderText = "فعل";
            this.IsVerb.Name = "IsVerb";
            this.IsVerb.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IsVerb.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsVerb.Width = 40;
            // 
            // IsAdjective
            // 
            this.IsAdjective.DataPropertyName = "IsAdjective";
            this.IsAdjective.Frozen = true;
            this.IsAdjective.HeaderText = "صفت";
            this.IsAdjective.Name = "IsAdjective";
            this.IsAdjective.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IsAdjective.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsAdjective.Width = 40;
            // 
            // Ending
            // 
            this.Ending.DataPropertyName = "Ending";
            this.Ending.Frozen = true;
            this.Ending.HeaderText = "صامت - مصوت";
            this.Ending.Name = "Ending";
            this.Ending.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Ending.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Ending.Width = 80;
            // 
            // DicElementBinding
            // 
            this.DicElementBinding.DataSource = typeof(VirastyarWordAddin.DictionaryElement);
            // 
            // OuterTable
            // 
            this.OuterTable.ColumnCount = 1;
            this.OuterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OuterTable.Controls.Add(this.AddWordGroupBox, 0, 2);
            this.OuterTable.Controls.Add(this.groupBox2, 0, 0);
            this.OuterTable.Controls.Add(this.DicGridView, 0, 1);
            this.OuterTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterTable.Location = new System.Drawing.Point(0, 0);
            this.OuterTable.Name = "OuterTable";
            this.OuterTable.RowCount = 3;
            this.OuterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.OuterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OuterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.OuterTable.Size = new System.Drawing.Size(371, 338);
            this.OuterTable.TabIndex = 1;
            // 
            // AddWordGroupBox
            // 
            this.AddWordGroupBox.Controls.Add(this.HelpEndingLink);
            this.AddWordGroupBox.Controls.Add(this.EndingComboBox);
            this.AddWordGroupBox.Controls.Add(this.AddWordButton);
            this.AddWordGroupBox.Controls.Add(this.IsAdjectiveCheckBox);
            this.AddWordGroupBox.Controls.Add(this.IsVerbCheckBox);
            this.AddWordGroupBox.Controls.Add(this.IsNounCheckBox);
            this.AddWordGroupBox.Controls.Add(this.NewWordText);
            this.AddWordGroupBox.Controls.Add(this.EndingLabel);
            this.AddWordGroupBox.Controls.Add(this.label1);
            this.AddWordGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddWordGroupBox.Location = new System.Drawing.Point(3, 241);
            this.AddWordGroupBox.Name = "AddWordGroupBox";
            this.AddWordGroupBox.Size = new System.Drawing.Size(365, 94);
            this.AddWordGroupBox.TabIndex = 1;
            this.AddWordGroupBox.TabStop = false;
            this.AddWordGroupBox.Text = "اضافه کردن کلمه جدید - اگر نوع کلمه را نمی‌دانید گزینه‌ها را خالی بگذارید.";
            // 
            // HelpEndingLink
            // 
            this.HelpEndingLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpEndingLink.AutoSize = true;
            this.HelpEndingLink.Location = new System.Drawing.Point(154, 57);
            this.HelpEndingLink.Name = "HelpEndingLink";
            this.HelpEndingLink.Size = new System.Drawing.Size(12, 13);
            this.HelpEndingLink.TabIndex = 12;
            this.HelpEndingLink.TabStop = true;
            this.HelpEndingLink.Text = "؟";
            this.HelpEndingLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HelpEndingLink_LinkClicked);
            // 
            // EndingComboBox
            // 
            this.EndingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EndingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EndingComboBox.FormattingEnabled = true;
            this.EndingComboBox.Location = new System.Drawing.Point(172, 52);
            this.EndingComboBox.Name = "EndingComboBox";
            this.EndingComboBox.Size = new System.Drawing.Size(108, 21);
            this.EndingComboBox.TabIndex = 11;
            this.MessageToolTip.SetToolTip(this.EndingComboBox, resources.GetString("EndingComboBox.ToolTip"));
            // 
            // AddWordButton
            // 
            this.AddWordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddWordButton.Location = new System.Drawing.Point(10, 52);
            this.AddWordButton.Name = "AddWordButton";
            this.AddWordButton.Size = new System.Drawing.Size(75, 23);
            this.AddWordButton.TabIndex = 10;
            this.AddWordButton.Text = "اضافه کن";
            this.AddWordButton.UseVisualStyleBackColor = true;
            this.AddWordButton.Click += new System.EventHandler(this.AddWordButton_Click);
            // 
            // IsAdjectiveCheckBox
            // 
            this.IsAdjectiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IsAdjectiveCheckBox.AutoSize = true;
            this.IsAdjectiveCheckBox.Location = new System.Drawing.Point(10, 27);
            this.IsAdjectiveCheckBox.Name = "IsAdjectiveCheckBox";
            this.IsAdjectiveCheckBox.Size = new System.Drawing.Size(51, 17);
            this.IsAdjectiveCheckBox.TabIndex = 8;
            this.IsAdjectiveCheckBox.Text = "صفت";
            this.MessageToolTip.SetToolTip(this.IsAdjectiveCheckBox, "اگر کلمه از این نوع نیست یا مطمئن نیستید این گزینه را خالی بگذارید.\r\nبرای اطمینان" +
                    " بررسی کنید کلمه «تر» یا «ترین» می‌گیرد؟");
            this.IsAdjectiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // IsVerbCheckBox
            // 
            this.IsVerbCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IsVerbCheckBox.AutoSize = true;
            this.IsVerbCheckBox.Location = new System.Drawing.Point(67, 27);
            this.IsVerbCheckBox.Name = "IsVerbCheckBox";
            this.IsVerbCheckBox.Size = new System.Drawing.Size(45, 17);
            this.IsVerbCheckBox.TabIndex = 7;
            this.IsVerbCheckBox.Text = "فعل";
            this.MessageToolTip.SetToolTip(this.IsVerbCheckBox, "اگر کلمه از این نوع نیست یا مطمئن نیستید این گزینه را خالی بگذارید.");
            this.IsVerbCheckBox.UseVisualStyleBackColor = true;
            // 
            // IsNounCheckBox
            // 
            this.IsNounCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IsNounCheckBox.AutoSize = true;
            this.IsNounCheckBox.Location = new System.Drawing.Point(118, 27);
            this.IsNounCheckBox.Name = "IsNounCheckBox";
            this.IsNounCheckBox.Size = new System.Drawing.Size(48, 17);
            this.IsNounCheckBox.TabIndex = 6;
            this.IsNounCheckBox.Text = "اسم";
            this.MessageToolTip.SetToolTip(this.IsNounCheckBox, "اگر کلمه از این نوع نیست یا مطمئن نیستید این گزینه را خالی بگذارید.\r\nبرای اطمینان" +
                    " بررسی کنید کلمه با «ها» جمع بسته می‌شود؟");
            this.IsNounCheckBox.UseVisualStyleBackColor = true;
            // 
            // NewWordText
            // 
            this.NewWordText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NewWordText.Location = new System.Drawing.Point(172, 25);
            this.NewWordText.Name = "NewWordText";
            this.NewWordText.Size = new System.Drawing.Size(143, 21);
            this.NewWordText.TabIndex = 5;
            this.NewWordText.TextChanged += new System.EventHandler(this.NewWordText_TextChanged);
            // 
            // EndingLabel
            // 
            this.EndingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EndingLabel.AutoSize = true;
            this.EndingLabel.Location = new System.Drawing.Point(286, 57);
            this.EndingLabel.Name = "EndingLabel";
            this.EndingLabel.Size = new System.Drawing.Size(68, 13);
            this.EndingLabel.TabIndex = 0;
            this.EndingLabel.Text = "آوای انتهایی:";
            this.MessageToolTip.SetToolTip(this.EndingLabel, "برای کلمات مختوم به «و» و «ه» آوای انتهایی نقش مهمی در نوشتار خواهد داشت.");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(321, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "کلمه:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CloseButton);
            this.groupBox2.Controls.Add(this.ReloadButton);
            this.groupBox2.Controls.Add(this.DeleteCurrentButton);
            this.groupBox2.Controls.Add(this.SaveButton);
            this.groupBox2.Controls.Add(this.DeleteAllButton);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(365, 44);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "مدیریت";
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(176, 15);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(0, 0);
            this.CloseButton.TabIndex = 11;
            this.CloseButton.Text = "بستن";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ReloadButton
            // 
            this.ReloadButton.Location = new System.Drawing.Point(82, 15);
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(80, 23);
            this.ReloadButton.TabIndex = 2;
            this.ReloadButton.Text = "بارگذاری مجدد";
            this.MessageToolTip.SetToolTip(this.ReloadButton, "از تغییراتی که داده‌اید صرف‌نظر کرده، لغت‌نامه را از نو بارگذاری می‌کند.");
            this.ReloadButton.UseVisualStyleBackColor = true;
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // DeleteCurrentButton
            // 
            this.DeleteCurrentButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteCurrentButton.Location = new System.Drawing.Point(283, 15);
            this.DeleteCurrentButton.Name = "DeleteCurrentButton";
            this.DeleteCurrentButton.Size = new System.Drawing.Size(80, 23);
            this.DeleteCurrentButton.TabIndex = 0;
            this.DeleteCurrentButton.Text = "حذف کن";
            this.MessageToolTip.SetToolTip(this.DeleteCurrentButton, "سطرهای انتخاب شده از لیست زیر را از لغت‌نامه حذف می‌کند.");
            this.DeleteCurrentButton.UseVisualStyleBackColor = true;
            this.DeleteCurrentButton.Click += new System.EventHandler(this.DeleteCurrentButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(1, 15);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(80, 23);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "ذخیره کن";
            this.MessageToolTip.SetToolTip(this.SaveButton, "تغییرات انجام شده را در لغت‌نامه ذخیره می‌کند.");
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DeleteAllButton
            // 
            this.DeleteAllButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteAllButton.Location = new System.Drawing.Point(202, 15);
            this.DeleteAllButton.Name = "DeleteAllButton";
            this.DeleteAllButton.Size = new System.Drawing.Size(80, 23);
            this.DeleteAllButton.TabIndex = 1;
            this.DeleteAllButton.Text = "کلاً پاک کن";
            this.MessageToolTip.SetToolTip(this.DeleteAllButton, "همه‌ی کلمات را از لغت‌نامه حذف می‌کند.");
            this.DeleteAllButton.UseVisualStyleBackColor = true;
            this.DeleteAllButton.Click += new System.EventHandler(this.DeleteAllButton_Click);
            // 
            // DictionaryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(371, 338);
            this.Controls.Add(this.OuterTable);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DictionaryEditor";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ویرایش واژه‌نامه";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserDicManager_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.DicGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DicElementBinding)).EndInit();
            this.OuterTable.ResumeLayout(false);
            this.AddWordGroupBox.ResumeLayout(false);
            this.AddWordGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DicGridView;
        private System.Windows.Forms.TableLayoutPanel OuterTable;
        private System.Windows.Forms.GroupBox AddWordGroupBox;
        private System.Windows.Forms.Button AddWordButton;
        private System.Windows.Forms.CheckBox IsAdjectiveCheckBox;
        private System.Windows.Forms.CheckBox IsVerbCheckBox;
        private System.Windows.Forms.CheckBox IsNounCheckBox;
        private System.Windows.Forms.TextBox NewWordText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button DeleteCurrentButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button DeleteAllButton;
        private System.Windows.Forms.Button ReloadButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.ToolTip MessageToolTip;
        private System.Windows.Forms.ComboBox EndingComboBox;
        private System.Windows.Forms.BindingSource DicElementBinding;
        private System.Windows.Forms.Label EndingLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Word;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsNoun;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsVerb;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsAdjective;
        private System.Windows.Forms.DataGridViewComboBoxColumn Ending;
        private System.Windows.Forms.LinkLabel HelpEndingLink;




    }
}
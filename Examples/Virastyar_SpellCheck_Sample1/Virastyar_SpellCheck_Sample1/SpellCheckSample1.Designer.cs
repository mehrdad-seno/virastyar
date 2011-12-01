namespace VirastyarSpellCheckSample1
{
    partial class SpellCheckSample1
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
            this.rtbInput = new System.Windows.Forms.RichTextBox();
            this.btnSpellCheck = new System.Windows.Forms.Button();
            this.lstResult = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbInput
            // 
            this.rtbInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInput.HideSelection = false;
            this.rtbInput.Location = new System.Drawing.Point(0, 0);
            this.rtbInput.Name = "rtbInput";
            this.rtbInput.Size = new System.Drawing.Size(606, 501);
            this.rtbInput.TabIndex = 0;
            this.rtbInput.Text = "";
            // 
            // btnSpellCheck
            // 
            this.btnSpellCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSpellCheck.Location = new System.Drawing.Point(474, 9);
            this.btnSpellCheck.Name = "btnSpellCheck";
            this.btnSpellCheck.Size = new System.Drawing.Size(119, 23);
            this.btnSpellCheck.TabIndex = 0;
            this.btnSpellCheck.Text = "متن را غلط‌یابی کن";
            this.btnSpellCheck.UseVisualStyleBackColor = true;
            this.btnSpellCheck.Click += new System.EventHandler(this.btnSpellCheck_Click);
            // 
            // lstResult
            // 
            this.lstResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstResult.FormattingEnabled = true;
            this.lstResult.Location = new System.Drawing.Point(0, 341);
            this.lstResult.Name = "lstResult";
            this.lstResult.Size = new System.Drawing.Size(606, 160);
            this.lstResult.TabIndex = 1;
            this.lstResult.SelectedIndexChanged += new System.EventHandler(this.lstResult_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnSpellCheck);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 298);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(606, 43);
            this.panel1.TabIndex = 2;
            // 
            // SpellCheckSample1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 501);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lstResult);
            this.Controls.Add(this.rtbInput);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SpellCheckSample1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "غلط‌یابی با استفاده از ویراستیار";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInput;
        private System.Windows.Forms.Button btnSpellCheck;
        private System.Windows.Forms.ListBox lstResult;
        private System.Windows.Forms.Panel panel1;
    }
}


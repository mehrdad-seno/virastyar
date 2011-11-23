namespace VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox
{
    partial class TitledListBoxSuggestionViewer
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbSuggestions = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(306, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(5, 5, 0, 5);
            this.lblTitle.Size = new System.Drawing.Size(69, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "پیشنهادها:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblTitle);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(378, 24);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // lbSuggestions
            // 
            this.lbSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSuggestions.FormattingEnabled = true;
            this.lbSuggestions.IntegralHeight = false;
            this.lbSuggestions.ItemHeight = 14;
            this.lbSuggestions.Location = new System.Drawing.Point(0, 24);
            this.lbSuggestions.Name = "lbSuggestions";
            this.lbSuggestions.Size = new System.Drawing.Size(378, 299);
            this.lbSuggestions.TabIndex = 2;
            this.lbSuggestions.Resize += new System.EventHandler(this.lbSuggestions_Resize);
            this.lbSuggestions.SelectedIndexChanged += new System.EventHandler(this.lbSuggestions_SelectedIndexChanged);
            this.lbSuggestions.DoubleClick += new System.EventHandler(this.lbSuggestions_DoubleClick);
            // 
            // TitledListBoxSuggestionViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbSuggestions);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TitledListBoxSuggestionViewer";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(378, 323);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListBox lbSuggestions;
    }
}

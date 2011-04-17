namespace VirastyarWordAddin
{
    partial class WordCompletionForm
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
            this.lstItems = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstItems
            // 
            this.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstItems.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstItems.FormattingEnabled = true;
            this.lstItems.Items.AddRange(new object[] {
            "سلام",
            "چطوری؟",
            "خوبی؟",
            "خوشی؟",
            "سلامتی؟",
            "سرکیفی؟",
            "دماغت",
            "چاقه؟",
            "دیگه",
            "چه",
            "خبرا؟"});
            this.lstItems.Location = new System.Drawing.Point(0, 0);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(117, 104);
            this.lstItems.TabIndex = 0;
            this.lstItems.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstItems_MouseDoubleClick);
            // 
            // WordCompletionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(117, 109);
            this.ControlBox = false;
            this.Controls.Add(this.lstItems);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "WordCompletionForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.WordCompletionForm_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WordCompletionForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstItems;
    }
}
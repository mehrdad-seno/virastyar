namespace VirastyarWordAddin.Verifiers.CustomGUIs.SpellCheckerSuggestions
{
    partial class SpellCheckerSuggestionViewer
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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbSuggestions = new System.Windows.Forms.ListBox();
            this.txtCustomSuggestions = new System.Windows.Forms.TextBox();
            this.btnAddCustomToDic = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
            this.lblTitle.Size = new System.Drawing.Size(69, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "پیشنهادها:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblTitle);
            this.flowLayoutPanel1.Controls.Add(this.txtCustomSuggestions);
            this.flowLayoutPanel1.Controls.Add(this.btnAddCustomToDic);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(378, 28);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // lbSuggestions
            // 
            this.lbSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSuggestions.FormattingEnabled = true;
            this.lbSuggestions.IntegralHeight = false;
            this.lbSuggestions.ItemHeight = 14;
            this.lbSuggestions.Location = new System.Drawing.Point(0, 28);
            this.lbSuggestions.Name = "lbSuggestions";
            this.lbSuggestions.Size = new System.Drawing.Size(378, 295);
            this.lbSuggestions.TabIndex = 2;
            this.lbSuggestions.Resize += new System.EventHandler(this.lbSuggestions_Resize);
            this.lbSuggestions.SelectedIndexChanged += new System.EventHandler(this.lbSuggestions_SelectedIndexChanged);
            this.lbSuggestions.DoubleClick += new System.EventHandler(this.lbSuggestions_DoubleClick);
            // 
            // txtCustomSuggestions
            // 
            this.txtCustomSuggestions.Location = new System.Drawing.Point(163, 3);
            this.txtCustomSuggestions.Name = "txtCustomSuggestions";
            this.txtCustomSuggestions.Size = new System.Drawing.Size(137, 22);
            this.txtCustomSuggestions.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtCustomSuggestions, "جهت درج نیم‌فاصله از ترکیب کنترل + شیفت + ۲ استفاده کنید.");
            this.txtCustomSuggestions.TextChanged += new System.EventHandler(this.txtCustomSuggestions_TextChanged);
            this.txtCustomSuggestions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomSuggestions_KeyDown);
            // 
            // btnAddCustomToDic
            // 
            this.btnAddCustomToDic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAddCustomToDic.Location = new System.Drawing.Point(139, 3);
            this.btnAddCustomToDic.Name = "btnAddCustomToDic";
            this.btnAddCustomToDic.Size = new System.Drawing.Size(18, 22);
            this.btnAddCustomToDic.TabIndex = 26;
            this.btnAddCustomToDic.Text = "+";
            this.toolTip1.SetToolTip(this.btnAddCustomToDic, "افزودن عبارت درج شده به واژه‌نامه");
            this.btnAddCustomToDic.UseVisualStyleBackColor = true;
            this.btnAddCustomToDic.Click += new System.EventHandler(this.btnAddCustomToDic_Click);
            // 
            // SpellCheckerSuggestionViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbSuggestions);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SpellCheckerSuggestionViewer";
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
        private System.Windows.Forms.TextBox txtCustomSuggestions;
        private System.Windows.Forms.Button btnAddCustomToDic;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

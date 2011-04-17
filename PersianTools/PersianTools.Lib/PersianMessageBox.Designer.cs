namespace SCICT.Utility.GUI
{
    partial class PersianMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersianMessageBox));
            this.labelMessage = new System.Windows.Forms.Label();
            this.panelMessage = new System.Windows.Forms.Panel();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btn1 = new System.Windows.Forms.Button();
            this.panelMessageAndImage = new System.Windows.Forms.Panel();
            this.panelImage = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.imagesMBox = new System.Windows.Forms.ImageList(this.components);
            this.panelMessage.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelMessageAndImage.SuspendLayout();
            this.panelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.BackColor = System.Drawing.Color.White;
            this.labelMessage.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelMessage.Location = new System.Drawing.Point(11, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.labelMessage.Size = new System.Drawing.Size(52, 26);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "label1";
            // 
            // panelMessage
            // 
            this.panelMessage.BackColor = System.Drawing.Color.White;
            this.panelMessage.Controls.Add(this.labelMessage);
            this.panelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMessage.Location = new System.Drawing.Point(0, 0);
            this.panelMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Size = new System.Drawing.Size(63, 27);
            this.panelMessage.TabIndex = 1;
            // 
            // btn3
            // 
            this.btn3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn3.Location = new System.Drawing.Point(-146, 6);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(85, 23);
            this.btn3.TabIndex = 2;
            this.btn3.Text = "button3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // btn2
            // 
            this.btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn2.Location = new System.Drawing.Point(-55, 6);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(85, 23);
            this.btn2.TabIndex = 1;
            this.btn2.Text = "button2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btn1);
            this.panelButtons.Controls.Add(this.btn3);
            this.panelButtons.Controls.Add(this.btn2);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 27);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(127, 38);
            this.panelButtons.TabIndex = 4;
            // 
            // btn1
            // 
            this.btn1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn1.Location = new System.Drawing.Point(36, 6);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(85, 23);
            this.btn1.TabIndex = 0;
            this.btn1.Text = "button1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // panelMessageAndImage
            // 
            this.panelMessageAndImage.Controls.Add(this.panelMessage);
            this.panelMessageAndImage.Controls.Add(this.panelImage);
            this.panelMessageAndImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMessageAndImage.Location = new System.Drawing.Point(0, 0);
            this.panelMessageAndImage.Name = "panelMessageAndImage";
            this.panelMessageAndImage.Size = new System.Drawing.Size(127, 27);
            this.panelMessageAndImage.TabIndex = 5;
            // 
            // panelImage
            // 
            this.panelImage.BackColor = System.Drawing.Color.White;
            this.panelImage.Controls.Add(this.pictureBox1);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelImage.Location = new System.Drawing.Point(63, 0);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(64, 27);
            this.panelImage.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(6, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // imagesMBox
            // 
            this.imagesMBox.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagesMBox.ImageStream")));
            this.imagesMBox.TransparentColor = System.Drawing.Color.Transparent;
            this.imagesMBox.Images.SetKeyName(0, "error.png");
            this.imagesMBox.Images.SetKeyName(1, "help.png");
            this.imagesMBox.Images.SetKeyName(2, "info.png");
            this.imagesMBox.Images.SetKeyName(3, "Warning.png");
            // 
            // PersianMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(127, 65);
            this.ControlBox = false;
            this.Controls.Add(this.panelMessageAndImage);
            this.Controls.Add(this.panelButtons);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PersianMessageBox";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PersianMessageBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PersianMessageBox_FormClosing);
            this.panelMessage.ResumeLayout(false);
            this.panelMessage.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelMessageAndImage.ResumeLayout(false);
            this.panelImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Panel panelMessage;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Panel panelMessageAndImage;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.ImageList imagesMBox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
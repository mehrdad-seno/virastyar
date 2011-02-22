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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SCICT.Utility.GUI
{
    /// <summary>
    /// A Persian Message Box
    /// </summary>
    public partial class PersianMessageBox : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersianMessageBox"/> class.
        /// </summary>
        public PersianMessageBox()
        {
            InitializeComponent();
        }

        private const string CaptionAbort = "توقف";
        private const string CaptionRetry = "تلاش دوباره";
        private const string CaptionIgnore = "نادیده گرفتن";
        private const string CaptionOK = "تایید";
        private const string CaptionCancel = "لغو";
        private const string CaptionYes = "بله";
        private const string CaptionNo = "خیر";

        private Button btnCancelButton = null;
        private Button buttonPressed = null;
        private int minButtonWidth = 0;

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text)
        {
            return Show(text, "پیغام", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="icon">The icon to be shown.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, MessageBoxIcon icon)
        {
            return Show(text, "", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="buttons">The buttons of the message box.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="buttons">The buttons of the message box.</param>
        /// <param name="icon">The icon to be shown.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <param name="buttons">The buttons of the message box.</param>
        /// <param name="icon">The icon to be shown.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            DialogResult dr = DialogResult.None;
            using (PersianMessageBox frm = new PersianMessageBox())
            {
                frm.Text = caption;
                frm.labelMessage.Text = text;

                frm.SetIcon(icon);

                frm.SetButtons(buttons, defaultButton);
                frm.SetSize();
                frm.ShowDialog();
                dr = frm.GetDialogResult();
            }

            return dr;
        }

        private DialogResult GetDialogResult()
        {
            if (buttonPressed == null)
            {
                return DialogResult.None;
            }
            else
            {
                DialogResult dr = DialogResult.None;
                string caption = buttonPressed.Text;
                switch (caption)
                {
                    case CaptionAbort:
                        dr = DialogResult.Abort;
                        break;
                    case CaptionCancel:
                        dr = DialogResult.Cancel;
                        break;
                    case CaptionIgnore:
                        dr = DialogResult.Ignore;
                        break;
                    case CaptionNo:
                        dr = DialogResult.No;
                        break;
                    case CaptionOK:
                        dr = DialogResult.OK;
                        break;
                    case CaptionRetry:
                        dr = DialogResult.Retry;
                        break;
                    case CaptionYes:
                        dr = DialogResult.Yes;
                        break;
                    default:
                        break;
                }

                return dr;
            }

        }

        private void SetIcon(MessageBoxIcon icon)
        {
            int imageIndex = -1;
            switch (icon)
            {
                case MessageBoxIcon.Asterisk:
                    imageIndex = 2;
                    break;
                case MessageBoxIcon.Stop:
                    imageIndex = 0;
                    break;
                case MessageBoxIcon.Question:
                    imageIndex = 1;
                    break;
                case MessageBoxIcon.Exclamation:
                    imageIndex = 3;
                    break;
                case MessageBoxIcon.None:
                default:
                    break;
            }

            if (imageIndex >= 0)
            {
                HasImage = true;
                pictureBox1.Image = imagesMBox.Images[imageIndex];
                panelImage.Visible = true;
            }
            else
            {
                HasImage = false;
                panelImage.Visible = false;
            }
        }

        private void SetButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            int btnCount = 0;
            Button cancelButton = null;
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    btn1.Text = CaptionAbort;
                    btn2.Text = CaptionRetry;
                    btn3.Text = CaptionIgnore;
                    btnCount = 3;
                    break;
                case MessageBoxButtons.OK:
                    btn1.Text = CaptionOK;
                    cancelButton = btn1;
                    btnCount = 1;
                    break;
                case MessageBoxButtons.OKCancel:
                    btn1.Text = CaptionOK;
                    btn2.Text = CaptionCancel;
                    cancelButton = btn2;
                    btnCount = 2;
                    break;
                case MessageBoxButtons.RetryCancel:
                    btn1.Text = CaptionRetry;
                    btn2.Text = CaptionCancel;
                    cancelButton = btn2;
                    btnCount = 2;
                    break;
                case MessageBoxButtons.YesNo:
                    btn1.Text = CaptionYes;
                    btn2.Text = CaptionNo;
                    cancelButton = btn2;
                    btnCount = 2;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    btn1.Text = CaptionYes;
                    btn2.Text = CaptionNo;
                    btn3.Text = CaptionCancel;
                    cancelButton = btn3;
                    btnCount = 3;
                    break;
                default:
                    break;
            }

            btnCancelButton = cancelButton;
            if (cancelButton != null)
                this.CancelButton = cancelButton;

            switch (defaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                    this.AcceptButton = btn1;
                    break;
                case MessageBoxDefaultButton.Button2:
                    this.AcceptButton = btn2;
                    break;
                case MessageBoxDefaultButton.Button3:
                    this.AcceptButton = btn3;
                    break;
                default:
                    break;
            }

            int paddingButtons = panelButtons.Size.Width - (btn1.Size.Width + btn1.Location.X);
            int paddingPanel = this.Width - panelButtons.Width;

            switch (btnCount)
            {
                case 1:
                    btn2.Visible = false;
                    btn3.Visible = false;
                    minButtonWidth = btn1.Width + 2 * paddingButtons + paddingPanel;
                    break;
                case 2:
                    btn3.Visible = false;
                    minButtonWidth = 2 * btn1.Width + 3 * paddingButtons + paddingPanel;
                    break;
                default:
                    minButtonWidth = 3 * btn1.Width + 4 * paddingButtons + paddingPanel;
                    break;
            }
        }

        private bool HasImage { get; set; }

        private void SetSize()
        {
            int width = Math.Max(minButtonWidth, this.Width);
            width = Math.Max(width, labelMessage.Width + 2*(panelMessage.Margin.Left + panelMessage.Margin.Right) + (HasImage? panelImage.Width : 0));

            if (HasImage)
                this.Height += pictureBox1.Height + 10;
            int height = Math.Max(this.Height, labelMessage.Size.Height /* + (HasImage? panelImage.Height : 0)*/ + panelButtons.Height + 4*(panelMessage.Margin.Top + panelMessage.Margin.Bottom));

            width = Math.Min(width, Screen.PrimaryScreen.Bounds.Width);
            height = Math.Min(height, Screen.PrimaryScreen.Bounds.Height);
            this.Size = new Size(width, height);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            buttonPressed = btn1;
            this.Close();
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            buttonPressed = btn2;
            this.Close();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            buttonPressed = btn3;
            this.Close();
        }

        private void PersianMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (buttonPressed == null)
                e.Cancel = true;
        }
    }
}

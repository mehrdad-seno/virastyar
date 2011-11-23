using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

        private Button m_btnCancelButton = null;
        private Button m_buttonPressed = null;
        private int m_minButtonWidth = 0;

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text)
        {
            return Show(null, text, "پیام", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            return Show(owner, text, "پیام", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="icon">The icon to be shown.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, MessageBoxIcon icon)
        {
            return Show(text, " ", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Opens a persian message box.
        /// </summary>
        /// <param name="text">The message to be shown.</param>
        /// <param name="caption">The caption of the message box.</param>
        /// <returns>The button pressed</returns>
        public static DialogResult Show(string text, string caption)
        {
            return Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
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
            return Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
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
            return Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton shieldedButton, MessageBoxIcon icon)
        {
            return Show(null, text, caption, buttons, shieldedButton, icon, MessageBoxDefaultButton.Button1);
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
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, 
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return Show(null, text, caption, buttons, icon, defaultButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            DialogResult result = DialogResult.None;
            using (var form = new PersianMessageBox())
            {
                form.Text = (caption != "") ? caption : " ";
                form.labelMessage.Text = (text != "") ? text : " ";

                form.SetIcon(icon);

                form.SetButtons(buttons, defaultButton);

                form.SetSize();
                form.ShowDialog(owner);
                result = form.GetDialogResult();
            }

            return result;
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
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton shieldedButton, 
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return Show(null, text, caption, buttons, shieldedButton, icon, defaultButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton shieldedButton,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            DialogResult result = DialogResult.None;
            using (var form = new PersianMessageBox())
            {
                form.Text = (caption != "") ? caption : " ";
                form.labelMessage.Text = (text != "") ? text : " ";

                form.SetIcon(icon);

                form.SetButtons(buttons, defaultButton);
                form.SetShieldedButton(shieldedButton);
                form.SetSize();
                form.ShowDialog(owner);
                result = form.GetDialogResult();
            }

            return result;
        }

        private DialogResult GetDialogResult()
        {
            if (m_buttonPressed == null)
            {
                return DialogResult.None;
            }
            else
            {
                DialogResult dr = DialogResult.None;
                string caption = m_buttonPressed.Text;
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
                    btn1.DialogResult = DialogResult.Abort;
                    btn2.Text = CaptionRetry;
                    btn2.DialogResult = DialogResult.Retry;
                    btn3.Text = CaptionIgnore;
                    btn3.DialogResult = DialogResult.Ignore;
                    btnCount = 3;
                    break;
                case MessageBoxButtons.OK:
                    btn1.Text = CaptionOK;
                    btn1.DialogResult = DialogResult.OK;
                    cancelButton = btn1;
                    btnCount = 1;
                    break;
                case MessageBoxButtons.OKCancel:
                    btn1.Text = CaptionOK;
                    btn1.DialogResult = DialogResult.OK;
                    btn2.Text = CaptionCancel;
                    btn2.DialogResult = DialogResult.Cancel;
                    cancelButton = btn2;
                    btnCount = 2;
                    break;
                case MessageBoxButtons.RetryCancel:
                    btn1.Text = CaptionRetry;
                    btn1.DialogResult = DialogResult.Retry;
                    btn2.Text = CaptionCancel;
                    btn2.DialogResult = DialogResult.Cancel;
                    cancelButton = btn2;
                    btnCount = 2;
                    break;
                case MessageBoxButtons.YesNo:
                    btn1.Text = CaptionYes;
                    btn1.DialogResult = DialogResult.Yes;
                    btn2.Text = CaptionNo;
                    btn2.DialogResult = DialogResult.No;
                    cancelButton = btn2;
                    btnCount = 2;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    btn1.Text = CaptionYes;
                    btn1.DialogResult = DialogResult.Yes;
                    btn2.Text = CaptionNo;
                    btn2.DialogResult = DialogResult.No;
                    btn3.Text = CaptionCancel;
                    btn3.DialogResult = DialogResult.Cancel;
                    cancelButton = btn3;
                    btnCount = 3;
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
                    m_minButtonWidth = btn1.Width + 2 * paddingButtons + paddingPanel;
                    break;
                case 2:
                    btn3.Visible = false;
                    m_minButtonWidth = 2 * btn1.Width + 3 * paddingButtons + paddingPanel;
                    break;
                default:
                    m_minButtonWidth = 3 * btn1.Width + 4 * paddingButtons + paddingPanel;
                    break;
            }

            m_btnCancelButton = cancelButton;
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
        }

        private void SetShieldedButton(MessageBoxDefaultButton shieldedButton)
        {
            if (!AtLeastVista())
                return;

            Button shielded = null;
            switch (shieldedButton)
            {
                case MessageBoxDefaultButton.Button1:
                    shielded = btn1;
                    break;
                case MessageBoxDefaultButton.Button2:
                    shielded = btn2;
                    break;
                case MessageBoxDefaultButton.Button3:
                    shielded = btn3;
                    break;
            }

            SetButtonShield(shielded, true);
        }

        private bool HasImage { get; set; }

        private void SetSize()
        {
            int width = Math.Max(m_minButtonWidth, this.Width);
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
            m_buttonPressed = btn1;
            this.Close();
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            m_buttonPressed = btn2;
            this.Close();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            m_buttonPressed = btn3;
            this.Close();
        }

        private void PersianMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_buttonPressed == null)
                e.Cancel = true;
        }

        #region Helper Methods

        public static bool AtLeastVista()
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static void SetButtonShield(Button btn, bool showShield)
        {
            //Note: make sure the button FlatStyle = FlatStyle.System
            // BCM_SETSHIELD = 0x0000160C
            SendMessage(new HandleRef(btn, btn.Handle), 0x160C, IntPtr.Zero, showShield ? new IntPtr(1) : IntPtr.Zero);
        }

        #endregion
    }
}

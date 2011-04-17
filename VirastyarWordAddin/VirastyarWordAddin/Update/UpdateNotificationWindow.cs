using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using SCICT.Utility.GUI;
using VirastyarWordAddin.Log;
using System.Diagnostics;
using VirastyarWordAddin.Properties;

namespace VirastyarWordAddin.Update
{
    public partial class UpdateNotificationWindow : Form
    {
        private readonly string Message_RunUpdateProgram = "آیا مایلید برنامه‌ی به‌روز رسان ویراستیار را اجرا کنید؟";
        private readonly string Message_GoToUpdateSite = "آیا مایلید برای دریافت نسخه جدید به سایت ویراستیار هدایت شوید؟";

        private readonly EventHandler m_closeDialogesHandler;
        private readonly bool m_recheck;

        public UpdateNotificationWindow(EventHandler closeDialogesHandler, bool recheck)
        {
            InitializeComponent();
            m_closeDialogesHandler = closeDialogesHandler;
            m_recheck = recheck;
        }

        private void UpdateNotificationWindow_Load(object sender, EventArgs e)
        {
            SwitchToCheckingForUpdate();
        }

        private void RunUpdateProgram()
        {
            if (UpdateChecker.ExitBeforeClose)
            {
                if (PersianMessageBox.Show("پیش از آغاز به‌روز رسانی، برنامه ورد باید بسته شود. آیا موافقید؟",
                                           "بستن ورد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    bool showErrorMsg = false;
                    if (Globals.ThisAddIn.CheckDataDependencies(true))
                    {
                        UpdateChecker.RunUpdateProgramBeforeClose = true;
                        Close();
                        m_closeDialogesHandler(this, EventArgs.Empty);

                        object missing = Missing.Value;
                        Globals.ThisAddIn.Application.Quit(ref missing, ref missing, ref missing);

                        //if (UpdateChecker.RunUpdateProgram())
                        //{
                        //    m_closeDialogesHandler(this, EventArgs.Empty);
                        //}
                        //else
                        //{
                        //    LogHelper.Error("Unable to run update program");
                        //    showErrorMsg = true;
                        //}
                    }
                    else
                    {
                        LogHelper.Error(
                            "Unable to restore/overwrite data files before updating virastyar. Updating aborted");
                        showErrorMsg = true;
                    }

                    if (showErrorMsg)
                    {
                        PersianMessageBox.Show(
                            "آماده‌سازی برنامه‌ی به‌روز رسان با خطا مواجه شد. بهروز رسانی متوقف می‌شود", "خطا",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                UpdateChecker.RunUpdateProgram();
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SwitchToUpdateNotAvailableMode()
        {
            pnlChecking.Visible = false;
            pnlNotAvialable.Visible = true;
            pnlUpdateAvailable.Visible = false;

            this.Height = 75;
        }

        private void SwitchToUpdateAvailableMode()
        {
            lblLatestVersion.Text = UpdateChecker.LatestVersion;
            txtChangeLog.Text = UpdateChecker.ChangeLog;
            
            lblInstalledVersion.Text = ThisAddIn.InstalledVersion.ToString(3);
            lblGetUpdate.Text = UpdateChecker.ExitBeforeClose ? Message_RunUpdateProgram : Message_GoToUpdateSite;

            pnlChecking.Visible = false;
            pnlNotAvialable.Visible = false;
            pnlUpdateAvailable.Visible = true;

            btnYes.Focus();

            this.Height = 315;
        }

        private void SwitchToCheckingForUpdate()
        {
            pnlChecking.Visible = true;
            pnlNotAvialable.Visible = false;
            pnlUpdateAvailable.Visible = false;

            progressBar.MarqueeAnimationSpeed = 25;
            progressBar.Style = ProgressBarStyle.Marquee;

            this.Height = 75;

            if (!m_recheck && UpdateChecker.UpdateIsAvailable)
            {
                SwitchToUpdateAvailableMode();
            }
            else
            {
                UpdateChecker.UpdateAvailable += UpdateChecker_UpdateAvailable;
                UpdateChecker.UpdateNotAvailable += UpdateChecker_UpdateNotAvailable;
                UpdateChecker.CheckForUpdate(m_recheck);
            }
        }

        void UpdateChecker_UpdateNotAvailable(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new Action(SwitchToUpdateNotAvailableMode));
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException("", ex);
                }
            }
            else
            {
                SwitchToUpdateNotAvailableMode();
            }
        }

        void UpdateChecker_UpdateAvailable(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new Action(SwitchToUpdateAvailableMode));
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException("", ex);
                }
            }
            else
            {
                SwitchToUpdateAvailableMode();
            }
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            RunUpdateProgram();
        }
    }
}

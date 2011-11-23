using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.Verifiers.Basics;
using System.Threading;
using VirastyarWordAddin.Properties;

namespace VirastyarWordAddin.Configurations
{
    public partial class AutomaticReportConfirmWindow : Form
    {
        private static AutomaticReportConfirmWindow automaticReportWindow;

        public AutomaticReportConfirmWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings.Default.LogReport_AutomaticReport = rdoSendReportAccept.Checked;
            Settings.Default.Save();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AutomaticReportConfirmWindow_Load(object sender, EventArgs e)
        {
            rdoSendReportAccept.Checked = Settings.Default.LogReport_AutomaticReport;
            rdoSendReportDecline.Checked = !Settings.Default.LogReport_AutomaticReport;
        }

        private void linkLabelViewGatheredInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string logPath = LogReporter.GetLogPath();
            var pInfo = new ProcessStartInfo { FileName = logPath, Verb = "Open", UseShellExecute = true };

            try
            {
                Process.Start(pInfo);
            }
            catch
            {
                // Ignore
            }
        }
    }

    public class AutomaticReportConfirm
    {
        private static readonly NotifyIcon s_updateNotifyIcon = null;
        private static AutomaticReportConfirmWindow s_window = null;

        static AutomaticReportConfirm()
        {
            s_updateNotifyIcon = new NotifyIcon();
            s_updateNotifyIcon.Icon = Icon.FromHandle(Resources.IconVirastyar.GetHicon());
            s_updateNotifyIcon.Click += m_updateNotifyIcon_Click;
        }

        public static void ShowConfirmTray()
        {
            if (!Settings.Default.LogReport_ConfirmationDone)
            {
                s_updateNotifyIcon.Visible = true;
                s_updateNotifyIcon.BalloonTipText = "کمک به بهبود ویراستیار";
                s_updateNotifyIcon.ShowBalloonTip(1000);
            }
        }

        static void m_updateNotifyIcon_Click(object sender, EventArgs e)
        {
            if (s_window == null || s_window.Disposing || s_window.IsDisposed)
            {
                s_window = new AutomaticReportConfirmWindow();
                s_window.FormClosed += s_window_FormClosed;
            }
            if (!s_window.Visible)
            {
                s_window.Show(ThisAddIn.GetWin32Window());
            }
        }

        static void s_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (s_window.DialogResult == DialogResult.OK)
            {
                s_updateNotifyIcon.Visible = false;
                Settings.Default.LogReport_ConfirmationDone = true;
                Settings.Default.Save();
            }
            else
            {
                s_updateNotifyIcon.ShowBalloonTip(1000);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VirastyarWordAddin.Log;
using wyDay.Controls;

namespace VirastyarWordAddin.Update
{
    public class WyUpdateChecker : IUpdateChecker
    {
        #region Fields

        private readonly AutomaticUpdaterBackend m_automaticUpdater = new AutomaticUpdaterBackend();

        #endregion

        #region Ctors

        public WyUpdateChecker()
        {
            m_automaticUpdater.GUID = "8bd818f3-3173-40a1-8c83-aea5aaa83fed";
            m_automaticUpdater.UpdateType = UpdateType.DoNothing;

            m_automaticUpdater.BeforeChecking += automaticUpdater_BeforeChecking;
            m_automaticUpdater.BeforeDownloading += automaticUpdater_BeforeDownloading;
            m_automaticUpdater.BeforeExtracting += automaticUpdater_BeforeExtracting;
            m_automaticUpdater.Cancelled += automaticUpdater_Cancelled;
            m_automaticUpdater.CheckingFailed += automaticUpdater_CheckingFailed;
            m_automaticUpdater.ClosingAborted += automaticUpdater_ClosingAborted;
            m_automaticUpdater.DownloadingFailed += automaticUpdater_DownloadingFailed;
            m_automaticUpdater.ExtractingFailed += automaticUpdater_ExtractingFailed;
            m_automaticUpdater.ProgressChanged += automaticUpdater_ProgressChanged;
            m_automaticUpdater.ReadyToBeInstalled += automaticUpdater_ReadyToBeInstalled;
            m_automaticUpdater.UpdateAvailable += automaticUpdater_UpdateAvailable;
            m_automaticUpdater.UpdateFailed += automaticUpdater_UpdateFailed;
            m_automaticUpdater.UpdateStepMismatch += automaticUpdater_UpdateStepMismatch;
            m_automaticUpdater.UpdateSuccessful += automaticUpdater_UpdateSuccessful;
            m_automaticUpdater.UpToDate += automaticUpdater_UpToDate;

            m_automaticUpdater.wyUpdateLocation = Path.Combine(SettingsHelper.GetInstallationPath(), "wyUpdate.exe");

            m_automaticUpdater.Initialize();
        }

        #endregion

        #region Methods

        public bool CloseUpdaterBackend()
        {
            m_automaticUpdater.Cancel();
            try
            {
                int retries = 3;
                do
                {
                    var wyUpdates = Process.GetProcessesByName("wyUpdate");
                    if (wyUpdates.Length <= 0)
                        return true;

                    foreach (var wyUpdate in wyUpdates)
                    {
                        try
                        {
                            wyUpdate.Kill();
                            if (wyUpdate.WaitForExit(1500))
                                return true;

                            --retries;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.DebugException("", ex);
                        }
                    }
                } while (retries > 0);
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
            }
            return false;
        }

        #endregion

        #region Event Handlers

        void automaticUpdater_UpToDate(object sender, SuccessArgs e)
        {
            LogHelper.Trace("automaticUpdater_UpToDate");

            UpdateIsAvailable = false;

            if (UpdateNotAvailable != null)
                UpdateNotAvailable(this, EventArgs.Empty);
        }

        void automaticUpdater_UpdateSuccessful(object sender, SuccessArgs e)
        {
            LogHelper.Trace("automaticUpdater_UpdateSuccessful");
        }

        void automaticUpdater_UpdateStepMismatch(object sender, EventArgs e)
        {
            LogHelper.Trace("automaticUpdater_UpdateStepMismatch");
        }

        void automaticUpdater_UpdateFailed(object sender, FailArgs e)
        {
            LogHelper.Trace("automaticUpdater_UpdateFailed");
        }

        void automaticUpdater_UpdateAvailable(object sender, EventArgs e)
        {
            LogHelper.Trace("automaticUpdater_UpdateAvailable");

            UpdateIsAvailable = true;

            if (UpdateAvailable != null)
                UpdateAvailable(this, EventArgs.Empty);
        }

        void automaticUpdater_ReadyToBeInstalled(object sender, EventArgs e)
        {
            LogHelper.Trace("automaticUpdater_ReadyToBeInstalled");
        }

        void automaticUpdater_ProgressChanged(object sender, int progress)
        {
            LogHelper.Trace("automaticUpdater_ProgressChanged");
        }

        void automaticUpdater_ExtractingFailed(object sender, FailArgs e)
        {
            LogHelper.Trace("automaticUpdater_ExtractingFailed");
        }

        void automaticUpdater_DownloadingFailed(object sender, FailArgs e)
        {
            LogHelper.Trace("automaticUpdater_DownloadingFailed");
        }

        void automaticUpdater_ClosingAborted(object sender, EventArgs e)
        {
            LogHelper.Trace("automaticUpdater_ClosingAborted");
        }

        void automaticUpdater_CheckingFailed(object sender, FailArgs e)
        {
            LogHelper.Trace("automaticUpdater_CheckingFailed");

            UpdateIsAvailable = false;

            if (UpdateNotAvailable != null)
                UpdateNotAvailable(this, EventArgs.Empty);
        }

        void automaticUpdater_Cancelled(object sender, EventArgs e)
        {
            LogHelper.Trace("automaticUpdater_Cancelled");
        }

        void automaticUpdater_BeforeExtracting(object sender, BeforeArgs e)
        {
            LogHelper.Trace("automaticUpdater_BeforeExtracting");
        }

        void automaticUpdater_BeforeDownloading(object sender, BeforeArgs e)
        {
            LogHelper.Trace("automaticUpdater_BeforeDownloading");
        }

        void automaticUpdater_BeforeChecking(object sender, BeforeArgs e)
        {
            LogHelper.Trace("automaticUpdater_BeforeChecking");
        }

        #endregion

        #region IUpdateChecker Members

        public event EventHandler UpdateAvailable;
        public event EventHandler UpdateNotAvailable;

        public void CheckForUpdate()
        {
            m_automaticUpdater.ForceCheckForUpdate();
        }

        public void CheckForUpdate(bool recheck)
        {
            m_automaticUpdater.ForceCheckForUpdate(recheck);
        }

        public bool RunUpdateProgram()
        {
            if (!CloseUpdaterBackend())
                return false;

            LogHelper.Trace("UpdateBackend closed successfully");

            var processInfo = new ProcessStartInfo(m_automaticUpdater.wyUpdateLocation);
            processInfo.UseShellExecute = false;
            processInfo.WorkingDirectory = SettingsHelper.GetInstallationPath();
            processInfo.Arguments = string.Format("-basedir:\"{0}\"", SettingsHelper.GetInstallationPath());

            try
            {
                LogHelper.Trace("Before trying to run the wyUpdate process");
                var process = Process.Start(processInfo);
                LogHelper.Trace("After trying to run the wyUpdate process");
                Thread.Sleep(250);
                process.Refresh();

                if (!process.HasExited)
                    return true;
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Error runnig wyUpdate", ex);
            }
            return false;
        }

        public string LatestVersion
        {
            get { return m_automaticUpdater.Version; }
        }

        public string ChangeLog
        {
            get { return m_automaticUpdater.Changes.Replace("\n", Environment.NewLine); }
        }

        public bool UpdateIsAvailable { get; private set; }

        public bool ExitBeforeUpdate
        {
            get { return true; }
        }

        #endregion
    }
}

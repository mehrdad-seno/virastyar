using System;
using System.Timers;

namespace VirastyarWordAddin.Update
{
    /// <summary>
    /// Wrapper over two different update checkers
    /// </summary>
    public static class UpdateChecker
    {
        private static MsiUpdateChecker s_msiUpdateChecker = null;
        private static WyUpdateChecker s_wyUpdateChecker = null;

        private static readonly Timer s_updateTimer = new Timer(15000);

        static UpdateChecker()
        {
            s_updateTimer.AutoReset = false;
            s_updateTimer.Enabled = false;
            s_updateTimer.Elapsed += UpdateTimer_Elapsed;

            InitMsiUpdateChecker();
            InitWyUpdateChecker();
        }

        static void InitWyUpdateChecker()
        {
            if (s_wyUpdateChecker != null)
            {
                s_wyUpdateChecker.UpdateAvailable -= UpdateChecker_UpdateAvailable;
                s_wyUpdateChecker.UpdateNotAvailable -= UpdateChecker_UpdateNotAvailable;
            }

            s_wyUpdateChecker = new WyUpdateChecker();
            s_wyUpdateChecker.UpdateAvailable += UpdateChecker_UpdateAvailable;
            s_wyUpdateChecker.UpdateNotAvailable += UpdateChecker_UpdateNotAvailable;
        }

        static void InitMsiUpdateChecker()
        {
            if (s_msiUpdateChecker != null)
            {
                s_msiUpdateChecker.UpdateAvailable -= UpdateChecker_UpdateAvailable;
                s_msiUpdateChecker.UpdateNotAvailable -= UpdateChecker_UpdateNotAvailable;
            }

            s_msiUpdateChecker = new MsiUpdateChecker();
            s_msiUpdateChecker.UpdateAvailable += UpdateChecker_UpdateAvailable;
            s_msiUpdateChecker.UpdateNotAvailable += UpdateChecker_UpdateNotAvailable;
        }

        static void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!UpdateIsAvailable)
                if (UpdateNotAvailable != null)
                    UpdateNotAvailable(null, e);
        }

        static void UpdateChecker_UpdateAvailable(object sender, EventArgs e)
        {
            if (ProperUpdater == null)
                ProperUpdater = (IUpdateChecker)sender;
            else if (ProperUpdater != s_msiUpdateChecker)
                ProperUpdater = (IUpdateChecker)sender;

            if (s_updateTimer.Enabled)
                s_updateTimer.Stop();

            if (UpdateAvailable != null)
                UpdateAvailable(sender, e);
        }

        static void UpdateChecker_UpdateNotAvailable(object sender, EventArgs e)
        {
            if (UpdateIsAvailable)
                return;

            if (UpdateNotAvailable != null)
                UpdateNotAvailable(sender, e);
        }

        private static IUpdateChecker ProperUpdater { get; set; }

        public static event EventHandler UpdateAvailable;
        public static event EventHandler UpdateNotAvailable;

        public static bool UpdateIsAvailable
        {
            get
            {
                return ProperUpdater != null;
            }
        }

        public static string LatestVersion
        {
            get
            {
                return ProperUpdater != null ? ProperUpdater.LatestVersion : "";
            }
        }

        public static string ChangeLog
        {
            get
            {
                return ProperUpdater != null ? ProperUpdater.ChangeLog : "";
            }
        }

        public static void CheckForUpdate()
        {
            //InitWyUpdateChecker();
            ProperUpdater = null;

            if (s_updateTimer.Enabled)
                s_updateTimer.Stop();

            s_updateTimer.Start();
            s_msiUpdateChecker.CheckForUpdate();
            s_wyUpdateChecker.CheckForUpdate();
        }

        public static void CheckForUpdate(bool recheck)
        {
            //InitWyUpdateChecker();
            ProperUpdater = null;
            
            if (s_updateTimer.Enabled)
                s_updateTimer.Stop();
            s_updateTimer.Start();

            s_msiUpdateChecker.CheckForUpdate(recheck);
            s_wyUpdateChecker.CheckForUpdate(recheck);
        }

        public static bool RunUpdateProgram()
        {
            return ProperUpdater != null && ProperUpdater.RunUpdateProgram();
        }

        public static bool ExitBeforeClose
        {
            get
            {
                return ProperUpdater != null && ProperUpdater.ExitBeforeUpdate;
            }
        }

        public static bool RunUpdateProgramBeforeClose { get; set; }
    }

    #region IUpdateChecker

    public interface IUpdateChecker
    {
        event EventHandler UpdateAvailable;
        event EventHandler UpdateNotAvailable;

        string LatestVersion { get; }
        string ChangeLog { get; }
        bool UpdateIsAvailable { get; }

        bool ExitBeforeUpdate { get; }

        void CheckForUpdate();
        void CheckForUpdate(bool recheck);
        bool RunUpdateProgram();
    }

    #endregion
}

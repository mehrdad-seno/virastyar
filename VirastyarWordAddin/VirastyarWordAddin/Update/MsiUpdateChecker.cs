using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.Properties;

namespace VirastyarWordAddin.Update
{
    public class MsiUpdateChecker : IUpdateChecker
    {
        #region Fields

        #endregion

        #region Ctors

        #endregion

        #region IUpdateChecker Members

        public void CheckForUpdate(bool recheck, int startDelay)
        {
            var thread = new Thread(() =>
            {
                Thread.Sleep(startDelay);

                Version installedVersion = ThisAddIn.InstalledVersion;
                Version lastCheckedVersion = LastCheckedVersion;

                if (string.IsNullOrEmpty(LatestVersion))
                    LatestVersion = GetLatestVersion();
                else if (recheck)
                    LatestVersion = GetLatestVersion();

                Version latestVersion = ThisAddIn.CheckVersionNumbers(new Version(LatestVersion));

                if (recheck || latestVersion.CompareTo(lastCheckedVersion) > 0)
                {
                    if (latestVersion.CompareTo(installedVersion) > 0)
                    {
                        LastCheckedVersion = latestVersion;
                        ChangeLog = GetChangeLog();

                        if (UpdateAvailable != null)
                            UpdateAvailable(this, EventArgs.Empty);
                        return;
                    }
                }
            });

            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Could not start the update thread", ex);
            }
        }

        public void CheckForUpdate(int startDelay)
        {
            CheckForUpdate(false, startDelay);
        }

        public void CheckForUpdate()
        {
            CheckForUpdate(false, 0);
        }

        public void CheckForUpdate(bool recheck)
        {
            CheckForUpdate(recheck, 0);
        }

        public string LatestVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Uses the http-based method for acquiring the information of latest version
        /// Warning: The speed of this operation depends on the internet connection speed
        /// </summary>
        public static string GetLatestVersion()
        {
            string latestVersionUrl = Settings.Default.Updater_LatestVersionUrl;

            try
            {
                var request = WebRequest.Create(latestVersionUrl);

                #region Headers

                request.Headers.Add("OfficeVersion", Globals.ThisAddIn.Application.Version);
                request.Headers.Add("WindowsVersion", Environment.OSVersion.VersionString);
                request.Headers.Add("Guid", ThisAddIn.InstallationGuid);

                #endregion

                using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    string version = reader.ReadToEnd();
                    return ThisAddIn.CheckVersionNumbers(new Version(version)).ToString(3);
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Unable to get information of the lastest version from virastyar web address", ex);
            }

            return ThisAddIn.InstalledVersion.ToString(3);
        }

        public string ChangeLog
        {
            get;
            private set;
        }

        /// <summary>
        /// Uses the http-based method for acquiring the information of change-log
        /// </summary>
        public string GetChangeLog()
        {
            string changeLogUrl = Settings.Default.Updater_ChangeLogUrl;

            try
            {
                using (var reader = new StreamReader(WebRequest.Create(changeLogUrl)
                                                         .GetResponse().GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Unable to get the change-log", ex);
            }

            return "";
        }

        /// <summary>
        /// The value of last version, checked via the http-request
        /// </summary>
        public Version LastCheckedVersion
        {
            get
            {
                string lastCheckedVersion = Settings.Default.Updater_LastCheckedVersion;
                if (!string.IsNullOrEmpty(lastCheckedVersion))
                    return ThisAddIn.CheckVersionNumbers(new Version(lastCheckedVersion));

                return ThisAddIn.InstalledVersion;
            }
            set
            {
                Settings.Default.Updater_LastCheckedVersion = value.ToString();
                Settings.Default.Save();
            }
        }

        public bool UpdateIsAvailable
        {
            get 
            {
                if (string.IsNullOrEmpty(LatestVersion))
                    return false;

                return ThisAddIn.CheckVersionNumbers(new Version(LatestVersion)).CompareTo(ThisAddIn.InstalledVersion) > 0; 
            }
        }

        public event EventHandler UpdateAvailable;
        public event EventHandler UpdateNotAvailable;

        public bool ExitBeforeUpdate
        {
            get { return false; }
        }

        public bool RunUpdateProgram()
        {
            var thread = new Thread(() =>
            {
                var pInfo = new ProcessStartInfo
                                {
                                    FileName = Settings.Default.Updater_DownloadUrl,
                                    Verb = "Open",
                                    UseShellExecute = true
                                };

                try
                {
                    Process.Start(pInfo);
                }
                catch (Exception ex)
                {
                    LogHelper.TraceException(ex);
                }
            });

            try
            {
                thread.Start();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.DebugException(ex);
                return false;
            }
        }

        #endregion
    }
}

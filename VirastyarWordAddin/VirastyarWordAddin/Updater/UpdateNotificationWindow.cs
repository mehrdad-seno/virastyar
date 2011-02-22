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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VirastyarWordAddin.Properties;

namespace VirastyarWordAddin.Updater
{
    public partial class UpdateNotificationWindow : Form
    {
        public UpdateNotificationWindow(string installedVersion, string latestVersion, string changeLog)
        {
            InitializeComponent();

            lblInstalledVersion.Text = installedVersion;
            lblLatestVersion.Text = latestVersion;
            txtChangeLog.Text = changeLog;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo(Settings.Default.Updater_DownloadUrl);
                processStartInfo.Verb = "Open";
                processStartInfo.UseShellExecute = true;

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static void CheckForUpdates(bool ignoreLastCheck, int startDelay)
        {
            var thread = new Thread(() =>
            {
                Thread.Sleep(startDelay);

                Version installedVersion = InstalledVersion;
                Version latestVersion = LatestVersion;
                Version lastCheckedVersion = LastCheckedVersion;

                if (latestVersion.CompareTo(installedVersion) > 0 &&
                    latestVersion.CompareTo(lastCheckedVersion) > 0)
                {
                    LastCheckedVersion = latestVersion;
                    // New update is available !
                    var updaterForm = new UpdateNotificationWindow(
                        installedVersion.ToString(), latestVersion.ToString(), ChangeLog);
                    updaterForm.ShowDialog();
                }
            });

            thread.Start();
        }

        public static void CheckForUpdates(int startDelay)
        {
            CheckForUpdates(false, startDelay);
        }

        public static void CheckForUpdates()
        {
            CheckForUpdates(false, 0);
        }

        public static void CheckForUpdates(bool ignoreLastCheck)
        {
            CheckForUpdates(ignoreLastCheck, 0);
        }

        public static Version LastCheckedVersion
        {
            get
            {
                string lastCheckedVersion = Settings.Default.Updater_LastCheckedVersion;
                if (!string.IsNullOrEmpty(lastCheckedVersion))
                    return new Version(lastCheckedVersion);

                return InstalledVersion;
            }
            private set 
            {
                Settings.Default.Updater_LastCheckedVersion = value.ToString();
                Settings.Default.Save();
            }
        }

        public static Version InstalledVersion
        {
            get
            {
                string virastyarAppData = SettingsHelper.GetCommonDataPath();
                Version[] versions = Directory.GetFiles(virastyarAppData, "version-*")
                    .OrderByDescending(verStr => verStr)
                    .Select(verStr => 
                        new Version(Path.GetFileName(verStr).ToLower().Replace("version-", "")))
                    .ToArray();

                if (versions.Length > 0)
                    return versions[0];
                
                return new Version("1");
            }
        }

        public static Version LatestVersion
        {
            get
            {
                string latestVersionUrl = Settings.Default.Updater_LatestVersionUrl;

                try
                {
                    using (var reader = new StreamReader(WebRequest
                                                             .Create(latestVersionUrl)
                                                             .GetResponse()
                                                             .GetResponseStream()))
                    {
                        string version = reader.ReadToEnd();
                        return new Version(version);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return InstalledVersion;
            }
        }

        public static string ChangeLog
        {
            get 
            {
                string changeLogUrl = Settings.Default.Updater_ChangeLogUrl;

                try
                {
                    using (var reader = new StreamReader(WebRequest
                                                             .Create(changeLogUrl)
                                                             .GetResponse()
                                                             .GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return "";
            }
        }

        private void UpdateNotificationWindow_Load(object sender, EventArgs e)
        {
            btnYes.Focus();
        }
    }
}

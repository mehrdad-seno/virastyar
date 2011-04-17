using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;

namespace VirastyarWordAddin.Configurations
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void AboutWindow_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Globals.ThisAddIn.ShowHelp(HelpConstants.MainIntro);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenURL("http://www.gnu.org/licenses/gpl.html");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenURL("http://www.virastyar.ir/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenURL("http://www.scict.ir/");
        }

        private void OpenURL(string url)
        {
            var pInfo = new ProcessStartInfo();
            pInfo.Verb = "Open";
            pInfo.UseShellExecute = true;
            pInfo.FileName = url;

            try
            {
                Process.Start(pInfo);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        private void AboutWindow_Load(object sender, EventArgs e)
        {
            string version = ParsingUtils.ConvertNumber2Persian(ThisAddIn.InstalledVersion.ToString(3));
            lblVersion.Text = "نگارش " + version;
        }
    }
}

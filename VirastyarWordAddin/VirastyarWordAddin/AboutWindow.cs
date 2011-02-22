using System.ComponentModel;
using System.Windows.Forms;

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
    }
}

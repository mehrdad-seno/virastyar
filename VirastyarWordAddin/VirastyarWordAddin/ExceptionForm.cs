using System;
using System.Windows.Forms;
using VirastyarWordAddin.Log;
using NLog;
using SCICT.Utility.GUI;

namespace VirastyarWordAddin
{
    public partial class ExceptionForm : Form
    {
        const string ShowDetailsCaption = "نمایش جزئیات";
        const string LessDetailsCaption = "عدم نمایش جزئیات";
        public Exception ExceptionToShow { get; private set; }

        public ExceptionForm()
        {
            InitializeComponent();
            this.btnShowDetails.Text = ShowDetailsCaption;
        }

        public ExceptionForm(Exception ex) : this()
        {
            this.ExceptionToShow = ex;
            this.tbDetails.Text = ex.ToString();
        }

        public static void ShowExceptionForm(Exception ex)
        {
            using (var frm = new ExceptionForm(ex))
            {
                frm.ShowDialog();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            if (tbDetails.Visible == true)
            {
                btnShowDetails.Text = ShowDetailsCaption;
                tbDetails.Visible = false;
                this.Height = MinimumSize.Height;
            }
            else
            {
                btnShowDetails.Text = LessDetailsCaption;
                tbDetails.Visible = true;
                this.Height = MaximumSize.Height;
            }
        }

        private void btnSendError_Click(object sender, EventArgs e)
        {
            // TODO: Exception form must accept an instance of VirastyarLogEventInfo
            var logEventInfo = new VirastyarLogEventInfo(LogLevel.Error, "ExceptionForm", "");
            logEventInfo.Exception = ExceptionToShow;

            if (LogReporter.Send(new[] {logEventInfo}))
            {
                PersianMessageBox.Show("گزارش خطا با موفقیت ارسال شد", MessageBoxIcon.Information);
            }
            Close();
        }
    }
}

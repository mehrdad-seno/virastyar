using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.Windows;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public partial class BatchModeVerificationWindow : Form, IBatchModeVerificationWindow
    {
        private string m_helpTopicFileName = "";
        private IntPtr m_ptrOwner = IntPtr.Zero;
        private VerifierBase m_verifier;
        private Thread m_verifierThread;
        private bool m_cancelationPending;
        private readonly AutoResetEvent m_eventVerifierThreadStoped = new AutoResetEvent(false);
        private bool m_isFormClosed = false;

        public BatchModeVerificationWindow()
        {
            InitializeComponent();

            ShowProgressAtDocument = cbShowProgressInDoc.Checked;
        }

        #region Public Static Methods

        public static void ShowWindowModal(VerifierBase verifier)
        {
            using (var wnd = new BatchModeVerificationWindow())
            {
                wnd.SetVerifier(verifier);
                wnd.ShowDialog(WindowWrapper.GetWordActiveWindowWrapper());
            }
        }

        public static void ShowWindowModeless(VerifierBase verifier)
        {
            var wnd = new BatchModeVerificationWindow();
            wnd.SetVerifier(verifier);
            wnd.Show(WindowWrapper.GetWordActiveWindowWrapper());
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            if (owner != null)
                m_ptrOwner = owner.Handle;

            return base.ShowDialog(owner);
        }

        public new void Show(IWin32Window owner)
        {
            if (owner != null)
                m_ptrOwner = owner.Handle;

            base.Show(owner);
        }

        #endregion

        private void SetVerifier(VerifierBase verifier)
        {
            m_verifier = verifier;
            verifier.VerificationWindowBatchMode = this;
            this.Text = verifier.Title;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            m_cancelationPending = false;

            m_verifierThread = new Thread(BatchVerifierThreadCallBack);
            m_verifierThread.Start();
        }

        private void BatchVerifierThreadCallBack()
        {
            bool isRevisionsEnabled = false;
            try
            {
                // Check if revisions are visible, if so make them invisible and turn them back on in the end
                isRevisionsEnabled = DocumentUtils.IsRevisionsEnabled(m_verifier.Document);
                if (isRevisionsEnabled)
                    DocumentUtils.ChangeShowingRevisions(m_verifier.Document, false);

                int docPageCount = -1;
                foreach (RangeWrapper par in RangeWrapper.ReadParagraphs(m_verifier.Document))
                {
                    if(m_cancelationPending)
                        break;
                    
                    if(docPageCount <= 0)
                    {
                        docPageCount = par.NumberOfPagesInDocument;
                    }

                    int curPage = par.PageNumber;

                    if(curPage > 0 && docPageCount > 0)
                        ShowPageNumberProgress(curPage, docPageCount);
                    //System.Diagnostics.Debug.WriteLine("Page: " + curPage + " of " + docPageCount);

                    SendParagraphForVerification(par);

                    if (m_cancelationPending)
                        break;

                } // end of foreach par
            }
            finally
            {
                // Check if revisions are visible, if so make them invisible and turn them back on in the end
                if (isRevisionsEnabled)
                    DocumentUtils.ChangeShowingRevisions(m_verifier.Document, true);

                m_eventVerifierThreadStoped.Set();
            }

            if (!m_isFormClosed)
            {
                StopThreadsAndClose();
            }
        }

        private void SendParagraphForVerification(RangeWrapper par)
        {
            if (par == null || !par.IsRangeValid)
                return;


            if (!m_verifier.InitParagraph(par))
                return;


            while (m_verifier.HasVerification())
            {
                if (m_cancelationPending)
                    return;

                // info about content and location of error, or even the related range
                VerificationData verData = m_verifier.GetNextVerificationData();

                if(verData == null || !verData.IsValid)
                    continue;

                //// SendToUi this verData and wait for user interaction
                //// VerificationResult contains selected suggestion user action and viwercontroller argument
                //VerificationResult verRes = SendVerificationToUi(verData);

                string defSuggestion = verData.Suggestions.DefaultSuggestion;
                if(defSuggestion == null)
                    continue;
                
                if(ShowProgressAtDocument)
                    verData.RangeToHighlight.Select();

                m_verifier.ApplyBatchModeAction(verData.RangeToHighlight, defSuggestion);
            }
        }


        private void StopThreadsAndClose()
        {
            m_cancelationPending = true;

            var act = new Action(delegate
                 {
                     ForceThreadsToClose();
                     //CloseAllActiveThreads();
                     Close();
                     AfterFormCloseCleanUp();
                 }
            );

            if(InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }

        private void ForceThreadsToClose()
        {
            m_cancelationPending = true;
            Thread.Sleep(0); // this forces the switch between threads
        }

        private void AfterFormCloseCleanUp()
        {
            //VerificationController.UnregisterInteractiveVerifier(m_verifier.GetType(), m_verifier.Document);
            m_verifier.OnReleaseWindow();
            if (m_ptrOwner != IntPtr.Zero)
                User32.SetFocus(m_ptrOwner);
        }


        private void CloseAllActiveThreads()
        {
            m_cancelationPending = true;

            while (m_verifierThread.IsAlive)
            {
                if (m_eventVerifierThreadStoped.WaitOne(200))
                    break;

                Application.DoEvents();
            }
        }

        #region IBatchModeVerificationWindow Members

        public bool CancelationPending
        {
            get { return m_cancelationPending; }
        }

        public string CurrentStatus
        {
            get { return lblStatus.Text; }
            set
            {
                var act = new Action(delegate { lblStatus.Text = value; });

                if(InvokeRequired)
                    Invoke(act);
                else
                    act.Invoke();
            }
        }

        public void ShowPageNumberProgress(int pageNo, int pageCount)
        {
            string msg = String.Format("صفحهٔ {0} از {1}", 
                ParsingUtils.ConvertNumber2Persian(pageNo.ToString()), 
                ParsingUtils.ConvertNumber2Persian(pageCount.ToString()));
            var act = new Action(delegate 
                { 
                    lblProgress.Text = msg; 
                    if(!progressBar.Visible)
                        progressBar.Visible = true;
                    progressBar.Minimum = 0;
                    progressBar.Maximum = pageCount;
                    progressBar.Value = Math.Min(pageNo, pageCount);
                });

            if (InvokeRequired)
                Invoke(act);
            else
                act.Invoke();

        }

        public bool ShowProgressAtDocument { get; set; }

        /// <summary>
        /// provide an empty or null argument to disable help.
        /// </summary>
        /// <param name="helpTopicFileName">Name of the help topic file.
        /// Provide an empty or null argument to disable help.</param>
        public void SetHelp(string helpTopicFileName)
        {
            bool showHelp = !String.IsNullOrEmpty(helpTopicFileName);

            m_helpTopicFileName = helpTopicFileName;
            HelpButton = showHelp;
        }

        #endregion

        private void BtnStopClick(object sender, EventArgs e)
        {
            ForceThreadsToClose();
            //StopThreadsAndClose();
        }

        private void VerificationWindowHelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (!String.IsNullOrEmpty(m_helpTopicFileName))
                Globals.ThisAddIn.ShowHelp(m_helpTopicFileName);
        }

        private void VerificationWindowHelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (!String.IsNullOrEmpty(m_helpTopicFileName))
                Globals.ThisAddIn.ShowHelp(m_helpTopicFileName);
        }

        private void CbShowProgressInDocCheckedChanged(object sender, EventArgs e)
        {
            ShowProgressAtDocument = cbShowProgressInDoc.Checked;
        }

        private void BatchModeVerificationWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_isFormClosed = true;
            ForceThreadsToClose();
            AfterFormCloseCleanUp();
        }

    }
}

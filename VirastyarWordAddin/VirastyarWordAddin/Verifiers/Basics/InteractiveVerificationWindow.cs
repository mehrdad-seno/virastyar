using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SCICT.NLP.Persian.Constants;
using System.Threading;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.Utility.GUI;
using SCICT.Utility.Windows;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.WinFormsUtils;
using SCICT.Utility;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public partial class InteractiveVerificationWindow : Form, IInteractiveVerificationWindow
    {
        #region Private Fields

        private readonly StatusBarToProgressToggler m_statusToggler;
        private readonly Dictionary<string, ToolStripStatusLabel> m_dicStatusLables = new Dictionary<string, ToolStripStatusLabel>();
        private string m_helpTopicFileName = "";
        private IntPtr m_ptrOwner = IntPtr.Zero;
        private bool m_isContentDeactivated;

        // private variables for threaded control
        private VerifierBase m_verifier;

        private Thread m_verifierThread;
        private Thread m_uiWaitThread;

        private bool m_cancelationPending;

        // The request-available pair of events for user input data
        private readonly AutoResetEvent m_eventUserInputRequest = new AutoResetEvent(false);
        private readonly AutoResetEvent m_eventUserInputAvailable = new AutoResetEvent(false);

        // We create one event per each working thread, so that the form's close event can decide
        // for which thread it has to wait.
        // Event specifying that the corresponding thread has been stopped, 
        // so that the close method can safely exit the application.
        //private readonly AutoResetEvent m_eventVerifierThreadStoped = new AutoResetEvent(false);
        //private readonly AutoResetEvent m_eventUiWaitThreadStopped = new AutoResetEvent(false);
        private VerifierRequestTypes m_verifierRequestType = VerifierRequestTypes.Nothing;
        private string m_viewerControlArg = null;

        private bool m_isRevisionsEnabled = false;
        private bool m_isFormClosed = false;
        private bool m_isDocumentClosed = false;

        #endregion

        #region Properties
        public Color HighlightBackColor { get; set; }
        public Color HighlightForeColorError { get; set; }
        public Color HighlightForeColorWarning { get; set; }
        public Color HighlightForeColorInformation { get; set; }

        public bool CancelationPending
        {
            get { return m_cancelationPending; }
        }

        public UserSelectedActions LastUserAction { get; private set; }

        protected ISuggestionsViwer SuggestionViewerControl { get; private set; }

        #endregion

        #region Constructor

        public InteractiveVerificationWindow()
        {
            LastUserAction = UserSelectedActions.None;
            InitializeComponent();

            // set some ui related features
            btnResume.Visible = false;
            AdjustFormMinSize();

            // setting the default properties
            HighlightBackColor = Color.Yellow;
            HighlightForeColorError = Color.Red;
            HighlightForeColorWarning = Color.Green;
            HighlightForeColorInformation = Color.Blue;

            // instantiating the fields
            m_statusToggler = new StatusBarToProgressToggler(statusStripMain, panelMainContent);
        }

        #endregion

        #region Show and ShowDialog new methods

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

        public static void ShowWindowModal(VerifierBase verifier)
        {
            using (var wnd = new InteractiveVerificationWindow())
            {
                wnd.WireVerifierAndWindow(verifier);
                wnd.ShowDialog(WindowWrapper.GetWordActiveWindowWrapper());
            }
        }

        public static void ShowWindowModeless(VerifierBase verifier)
        {
            var wnd = new InteractiveVerificationWindow();
            wnd.WireVerifierAndWindow(verifier);
            wnd.Show(WindowWrapper.GetWordActiveWindowWrapper());
        }

        #endregion

        #region UI Adjustment Methods

        protected void WireVerifierAndWindow(VerifierBase verifier)
        {
            // wire verifier and window together
            m_verifier = verifier;
            verifier.VerificationWindowInteractive = this;

            //var verifWindowForm = m_verificationWindow as Form;
            this.Text = verifier.Title;
            SetSuggestionsViewerControl(verifier.SuggestionViewerType);

            if (!String.IsNullOrEmpty(verifier.HelpTopicFileName))
                SetHelp(verifier.HelpTopicFileName);

            foreach (UserSelectedActions act in Enum.GetValues(typeof(UserSelectedActions)))
            {
                if (act == UserSelectedActions.None || act == UserSelectedActions.Resume || act == UserSelectedActions.Stop)
                    continue;

                if (verifier.ActionsToDisable.Has(act))
                {
                    DisableAction(act);
                }
            }

            verifier.OnInitWindow();
        }

        protected void SetSuggestionsViewerControl(Type controlType)
        {
            var sugsViewerControl = Activator.CreateInstance(controlType) as Control;

            if (sugsViewerControl == null || !(sugsViewerControl is ISuggestionsViwer))
            {
                throw new ArgumentException("The specified type is not appropriate to be shown as the suggestions viewer contorl", 
                    "controlType");
            }

            SuggestionViewerControl = sugsViewerControl as ISuggestionsViwer;

            sugsViewerControl.Dock = DockStyle.Fill;
            splitContainerErrorArea.Panel2.Controls.Add(sugsViewerControl);
            SuggestionViewerControl.ParentVerificationWindow = this;
            AdjustFormMinSize();

            SuggestionViewerControl.MainControlTopChanged += SuggestionViewerControlMainControlTopChanged;
            SuggestionViewerControl.SuggestionSelected += SuggestionViewerControlSuggestionSelected;
            SuggestionViewerControl.ActionInvoked += SuggestionViewerControlActionInvoked;
        }

        private void SuggestionViewerControlMainControlTopChanged(object sender, EventArgs e)
        {
            AdjustButtonPositions();
        }

        private void SuggestionViewerControlSuggestionSelected(object sender, EventArgs e)
        {
            ChangeAction();
        }

        private void SuggestionViewerControlActionInvoked(object sender, UserSelectedActions selAction, string arg)
        {
            m_viewerControlArg = arg;
            switch (selAction)
            {
                case UserSelectedActions.AddToDictionary:
                    AddToDictionaryAction();
                    break;
                case UserSelectedActions.Change:
                    ChangeAction();
                    break;
                case UserSelectedActions.ChangeAll:
                    ChangeAllAction();
                    break;
                case UserSelectedActions.Ignore:
                    IgnoreAction();
                    break;
                case UserSelectedActions.IgnoreAll:
                    IgnoreAllAction();
                    break;
                case UserSelectedActions.Resume:
                    ResumeAction();
                    break;
                case UserSelectedActions.Stop:
                    StopAction();
                    break;
            }
        }

        private void AdjustButtonPositions()
        {
            if (SuggestionViewerControl != null)
            {
                var ptScreen = ((Control)SuggestionViewerControl).PointToScreen(new Point(0, SuggestionViewerControl.MainControlTop));
                int ptForm = panelMainContent.PointToClient(ptScreen).Y;

                int minY = btnAddToDictionary.Bottom + 6;
                int maxY = btnStop.Top - ((btnChange.Height + 6) * 2);
                int newY = ptForm;

                if (maxY < minY)
                {
                    // do nothing, or possibly call
                    //AdjustFormMinSize();
                    newY = minY;
                }
                else
                {
                    if (ptForm < minY)
                    {
                        newY = minY;
                    }
                    else if (ptForm > maxY)
                    {
                        newY = maxY;
                    }
                }

                btnChange.Top = newY;
                btnChangeAll.Top = newY + btnChange.Height + 6;
            }
        }

        /// <summary>
        /// provide an empty or null argument to disable help.
        /// </summary>
        /// <param name="helpTopicFileName">Name of the help topic file.
        /// Provide an empty or null argument to disable help.</param>
        protected void SetHelp(string helpTopicFileName)
        {
            bool showHelp = !String.IsNullOrEmpty(helpTopicFileName);

            m_helpTopicFileName = helpTopicFileName;
            HelpButton = showHelp;
        }

        /// <summary>
        /// Adjusts the min sizes of controls in the main window as well as the verification window itself.
        /// This method is called whenever the form is rebuilt or the suggestion viewer controller is specified.
        /// </summary>
        private void AdjustFormMinSize()
        {
            const int minPanelHeight = 100;
            splitContainerErrorArea.Panel1MinSize = minPanelHeight;
            splitContainerErrorArea.Panel2MinSize = minPanelHeight;

            int minWidth = 300;
            if (SuggestionViewerControl != null)
            {
                splitContainerErrorArea.Panel2MinSize = Math.Max(
                    minPanelHeight + SuggestionViewerControl.MainControlTop, SuggestionViewerControl.MinimumSize.Height);
                minWidth = Math.Max(minWidth, SuggestionViewerControl.MinimumSize.Width);
            }
            else
            {
                minWidth = Math.Max(minWidth, splitContainerErrorArea.Width);
            }

            int panelMinWidth = minWidth + 10;
            int panelMinHeight = splitContainerErrorArea.Panel1MinSize + splitContainerErrorArea.Panel2MinSize + 20;

            Size diff = Size - ClientSize;
            MinimumSize = diff + // panelDisplayContent.MinimumSize +
                new Size(panelDisplayContent.Location) +
                new Size(panelMinWidth, panelMinHeight + statusStripMain.Height);
        }

        private void InitUIBasedOnModality()
        {
            if (Modal)
            {
                // do nothing yet!
            }
            else
            {
                //Leave += VerificationWindowDeactivate;
                Globals.ThisAddIn.Application.WindowActivate += WordApplicationWindowActivate;
                Globals.ThisAddIn.Application.DocumentBeforeClose += ApplicationDocumentBeforeClose;
                
                Deactivate += VerificationWindowDeactivate;
                //this.Activated += new EventHandler(VerificationWindow_Activated);
            }
        }

        /// <summary>
        /// Fixes the location of the window based on the position of the highlighted range,
        /// so that the range will be perfectly visible.
        /// </summary>
        /// <param name="r">The range that is highlighted and wished to be visible</param>
        private void FixWindowLocation(RangeWrapper r)
        {
            try
            {
                int rangeLeft, rangeTop, rangeWidth, rangeHeight;
                Globals.ThisAddIn.Application.ActiveWindow.GetPoint(out rangeLeft, out rangeTop, out rangeWidth,
                                                                    out rangeHeight, r.Range);

                int x = rangeLeft;
                if (rangeWidth < 0)
                {
                    x += rangeWidth;
                    rangeWidth = Math.Abs(rangeWidth);
                }

                //int scrWidth = Screen.PrimaryScreen.WorkingArea.Width;
                int scrHeight = Screen.PrimaryScreen.WorkingArea.Height;

                // if it is masked
                if ((this.Left <= x && x <= this.Left + this.Width ||
                     this.Left <= x + rangeWidth && x + rangeWidth <= this.Left + this.Width) &&
                    (this.Top <= rangeTop && rangeTop <= this.Top + this.Height ||
                     this.Top <= rangeTop + rangeHeight && rangeTop + rangeHeight <= this.Top + this.Height))
                {
                    // if it is masked vertically
                    if (this.Top <= rangeTop && rangeTop <= this.Top + this.Height ||
                        this.Top <= rangeTop + rangeHeight && rangeTop + rangeHeight <= this.Top + this.Height)
                    {
                        // if there's enough space below it
                        if (scrHeight - rangeTop - rangeHeight > this.Height)
                        {
                            this.Top = rangeTop + rangeHeight;
                        }
                        // else if there's enough space above it
                        else if (rangeTop > this.Height)
                        {
                            this.Top = rangeTop - this.Height;
                        }
                        //// if there's not enough space neither below nor above it try to move it horizontally
                        //else
                        //{
                        //    //    // if it can be shown on both sides
                        //    //    if (x > this.Width && scrWidth - x > this.Width)
                        //    //    {
                        //    //    }
                        //    //    else // if it can be shown on either side or none
                        //    //    {
                        //    //    }
                        //}
                    }
                }
            }
            catch
            {
                // simply do nothing in case of exception
                return;
            }
        }

        #endregion

        #region Activation and Deactivation Stuff

        void VerificationWindowDeactivate(object sender, EventArgs e)
        {
            if (!m_isContentDeactivated)
            {
                DeactivateContent();
            }
        }

        private void ActivateContent()
        {
            if (!Modal && m_isContentDeactivated)
            {
                HideResumeButton();

                panelMainContent.Enabled = true;

                m_isContentDeactivated = false;
                if(!m_isDocumentClosed)
                    m_verifier.Document.Activate();
            }
        }

        private void DeactivateContent()
        {
            if (!this.Modal && !m_isContentDeactivated)
            {
                try
                {
                    // Mehrdad: Preventing from a useless try/catch which probably throws an exception
                    if (!InvokeRequired && (this.Disposing || this.IsDisposed))
                        return;

                    this.BeginInvoke(new Action(() =>
                    {
                         if (this.Disposing || this.IsDisposed)
                             return;

                         var ptrNewActivatedWindow = User32.GetActiveWindow();
                         // if the newly activated window is not its owner window, no need to deactivate content
                         if (!m_ptrOwner.Equals(ptrNewActivatedWindow))
                             return;

                         panelMainContent.Enabled = false;
                         btnResume.Location = new Point(btnIgnore.Location.X, btnIgnore.Location.Y);
                         btnResume.Size = new Size(btnIgnore.Size.Width, btnIgnore.Size.Height);
                         btnResume.Visible = true;
                         btnResume.BringToFront();

                         m_isContentDeactivated = true;
                    }));
                }
                catch (InvalidOperationException ex)
                {
                    LogHelper.DebugException("Exception in DeactivateContent", ex);
                }
            }
        }

        void ApplicationDocumentBeforeClose(Microsoft.Office.Interop.Word.Document Doc, ref bool Cancel)
        {
            if (!Doc.Equals(m_verifier.Document))
            {
                return;
            }

            m_cancelationPending = true;
            m_isDocumentClosed = true;

            // Check if revisions are visible, if so make them invisible and turn them back on in the end
            if (m_isRevisionsEnabled)
            {
                DocumentUtils.ChangeShowingRevisions(m_verifier.Document, true);
                m_isRevisionsEnabled = false;
            }

            VerificationController.UnregisterInteractiveVerifier(m_verifier.GetType(), m_verifier.Document);

            ForceThreadsToClose();
        }



        void WordApplicationWindowActivate(Microsoft.Office.Interop.Word.Document Doc, Microsoft.Office.Interop.Word.Window Wn)
        {
            if(m_isDocumentClosed)
                return;

            if (!Doc.Equals(m_verifier.Document))
            {
                return;
            }


            // if the word window gets activated and the verifier window is also active,
            // then make the verifier window deactive
            // this happens when switching between windows with alt+tab or other pop-up windows
            if (!m_isContentDeactivated)
            {
                DeactivateContent();
            }
        }

        #endregion

        #region Utils
        /// <summary>
        /// Gets the color of the verification.
        /// </summary>
        /// <param name="vt">The verification type.</param>
        /// <returns></returns>
        protected Color GetVerificationColor(VerificationTypes vt)
        {
            switch (vt)
            {
                case VerificationTypes.Error:
                    return this.HighlightForeColorError;
                case VerificationTypes.Warning:
                    return this.HighlightForeColorWarning;
                //case VerificationTypes.Information:
                default:
                    return this.HighlightForeColorInformation;
            }
        }

        #endregion

        #region Threaded Contorl

        protected override void OnLoad(EventArgs e)
        {
            InitUIBasedOnModality();

            m_cancelationPending = false;
            GoIdle();

            m_verifierThread = new Thread(VerifierThreadCallBack);
            m_uiWaitThread = new Thread(UiWaitThreadCallBack);

            m_uiWaitThread.Start();
            m_verifierThread.Start();

            base.OnLoad(e);
        }

        private void VerifierThreadCallBack()
        {
            LastUserAction = UserSelectedActions.None;
            IEnumerable<RangeWrapper> parEnumerable = RangeWrapper.ReadParagraphsStartingFromCursor(m_verifier.Document);
            try
            {
                // Check if revisions are visible, if so make them invisible and turn them back on in the end
                m_isRevisionsEnabled = DocumentUtils.IsRevisionsEnabled(m_verifier.Document);
                if (m_isRevisionsEnabled)
                    DocumentUtils.ChangeShowingRevisions(m_verifier.Document, false);

                while (!m_cancelationPending)
                {
                    foreach (RangeWrapper par in parEnumerable)
                    {
                        SendParagraphForVerification(par);

                        if (m_cancelationPending)
                            break;

                        if (m_cancelationPending || LastUserAction == UserSelectedActions.Resume)
                            break;

                    } // end of foreach par
                    
                    // if resume was pressed without been cancelled, then restart from the beginning
                    if (!m_cancelationPending && LastUserAction == UserSelectedActions.Resume)
                    {
                        parEnumerable = RangeWrapper.ReadParagraphsStartingFromCursor(m_verifier.Document);
                        LastUserAction = UserSelectedActions.None;
                    }
                    // otherwise it means that we actually reached the end of the text
                    else if (!m_cancelationPending)
                    {
                        m_verifierRequestType = VerifierRequestTypes.RequestEndOfLoop;
                        m_eventUserInputRequest.Set();
                        m_eventUserInputAvailable.WaitOne();

                        if (!m_cancelationPending)
                        {
                            // make it read from the beginning of the document
                            parEnumerable = RangeWrapper.ReadParagraphs(m_verifier.Document);
                            LastUserAction = UserSelectedActions.None;
                            HideResumeButton();
                            GoIdle();
                        }
                    }
                } // end of while
            }
            finally
            {
                // Check if revisions are visible, if so make them invisible and turn them back on in the end
                if (m_isRevisionsEnabled && !m_isDocumentClosed)
                    DocumentUtils.ChangeShowingRevisions(m_verifier.Document, true);
                //m_eventVerifierThreadStoped.Set();
            }
        }

        private void SendParagraphForVerification(RangeWrapper par)
        {
            if (par == null || !par.IsRangeValid)
                return;


            if (!m_verifier.InitParagraph(par))
                return;

            
            while(m_verifier.HasVerification())
            {
                if(m_cancelationPending || LastUserAction == UserSelectedActions.Resume)
                    return;
                
                // info about content and location of error, or even the related range
                VerificationData verData = m_verifier.GetNextVerificationData();

                if(verData == null || !verData.IsValid)
                    continue;

                // SendToUi this verData and wait for user interaction
                // VerificationResult contains selected suggestion user action and viwercontroller argument
                VerificationResult verRes = SendVerificationToUi(verData);

                if (m_cancelationPending || LastUserAction == UserSelectedActions.Resume)
                    return;

                // UI is unaware of the addToDic dialog or any other kind of dialogs it may have
                // UI should provide means for verifier to call its desired UI dialog shows or others
                ProceedTypes procType = m_verifier.GetProceedTypeForVerificationResult(verRes);


                while (procType == ProceedTypes.InvalidUserAction)
                {
                    verRes = WaitForMoreUserActions();

                    if (m_cancelationPending || LastUserAction == UserSelectedActions.Resume)
                        return;

                    procType = m_verifier.GetProceedTypeForVerificationResult(verRes);
                }

                if (procType == ProceedTypes.IdleProceed)
                    GoIdle();
            } 
        }

        private VerificationResult WaitForMoreUserActions()
        {
            m_verifierRequestType = VerifierRequestTypes.RequestUserAction;

            m_eventUserInputRequest.Set();
            m_eventUserInputAvailable.WaitOne();

            if(m_cancelationPending)
                return null;

            string tempSelSug = SelectedSuggestion;

            return new VerificationResult
            {
                SelectedSuggestion = tempSelSug,
                UserAction = LastUserAction,
                ViewerControlArg = m_viewerControlArg
            };
        }

        public VerificationResult SendVerificationToUi(VerificationData data)
        {
            var act = new Action(delegate
            {
                SetErrorContent(
                    data.ErrorContext,
                    data.ErrorIndex,
                    data.ErrorLength,
                    data.ErrorType);

                SetSuggestions(data.Suggestions);

                data.RangeToHighlight.Select();
                FixWindowLocation(data.RangeToHighlight);
            }
            );

            if (this.InvokeRequired)
                Invoke(act);
            else
                act.Invoke();

            return WaitForMoreUserActions();
            //m_eventUserInputRequest.Set();
            //m_eventUserInputAvailable.WaitOne();

            //string tempSelSug = SelectedSuggestion;

            //return new VerificationResult
            //{
            //    SelectedSuggestion = tempSelSug,
            //    UserAction = LastUserAction,
            //    ViewerControlArg = m_viewerControlArg
            //};
        }

        private void UiWaitThreadCallBack()
        {
            while (!m_cancelationPending)
            {
                m_eventUserInputRequest.WaitOne();
                if(m_cancelationPending)
                    break;

                switch (m_verifierRequestType)
                {
                    case VerifierRequestTypes.RequestUserAction:
                        AskForUserInteraction();
                        break;
                    case VerifierRequestTypes.RequestEndOfLoop:
                        AskForContinuation();
                        break;
                    default:
                        break;
                }
            }

            //m_eventUiWaitThreadStopped.Set();

            if (!m_isFormClosed)
            {
                StopThreadsAndClose();
            }
        }

        private void HideResumeButton()
        {
            var actDisableResume = new Action(() =>
            {
                btnResume.SendToBack();
                btnResume.Visible = false;
            });

            if (InvokeRequired)
                Invoke(actDisableResume);
            else
                actDisableResume.Invoke();
        }

        protected void ProceedVerifying(UserSelectedActions userAction)
        {
            LastUserAction = userAction;
            m_eventUserInputAvailable.Set();
        }

        private void StopThreadsAndClose()
        {
            var act = new Action(ClosingProcedure);

            if (InvokeRequired)
                Invoke(act);
            else
                ClosingProcedure();
        }

        private void ClosingProcedure()
        {
            ForceThreadsToClose();
            Close();
            AfterFormCloseCleanUp();
        }

        private void ForceThreadsToClose()
        {
            m_cancelationPending = true;
            m_eventUserInputRequest.Set();
            m_eventUserInputAvailable.Set();
            Thread.Sleep(0); // this forces the switch between threads
            Thread.Sleep(0); // this forces the switch between threads
        }

        private void AfterFormCloseCleanUp()
        {
            if (!m_isDocumentClosed)
            {
                VerificationController.UnregisterInteractiveVerifier(
                    m_verifier.GetType(), m_verifier.Document);
            }
            m_verifier.OnReleaseWindow();
            if (m_ptrOwner != IntPtr.Zero)
                User32.SetFocus(m_ptrOwner);
        }

        //private void CloseAllActiveThreads2()
        //{
        //    LogHelper.Debug("Closing Active Threads", "After-call-2");

        //    LastUserAction = UserSelectedActions.Stop;
        //    m_cancelationPending = true;

        //    m_eventUserInputAvailable.Set();
        //    for (int tri = 0; tri < 2 && m_verifierThread.IsAlive; tri++)
        //    {
        //        if (m_eventVerifierThreadStoped.WaitOne(100))
        //        {
        //            break;
        //        }

        //        Application.DoEvents();
        //    }

        //    m_verifierRequestType = VerifierRequestTypes.Nothing;
        //    m_eventUserInputRequest.Set();
        //    for (int tri = 0; tri < 2 && m_uiWaitThread.IsAlive; tri++)
        //    {
        //        if (m_eventUiWaitThreadStopped.WaitOne(100))
        //        {
        //            break;
        //        }

        //        Application.DoEvents();
        //    }
        //}


        //private void CloseAllActiveThreads()
        //{
        //    lock (m_closeActiveThreadsLock)
        //    {
        //        //if (Thread.CurrentThread.ManagedThreadId == m_uiWaitThread.ManagedThreadId
        //        //    || Thread.CurrentThread.ManagedThreadId == m_verifierThread.ManagedThreadId)
        //        //return;

        //        if(m_isCloseAllActiveThreadsRuned)
        //            return;

        //        m_isCloseAllActiveThreadsRuned = true;

        //        LogHelper.Debug("Closing Active Threads", "After call-1");
            
        //        LastUserAction = UserSelectedActions.Stop;
        //        m_cancelationPending = true;

        //        m_eventUserInputAvailable.Set();
        //        for (int tri = 0; tri < 2 && m_verifierThread.IsAlive; tri++)
        //        {
        //            if (m_eventVerifierThreadStoped.WaitOne(100))
        //            {
        //                break;
        //            }

        //            Application.DoEvents();
        //        }

        //        m_verifierRequestType = VerifierRequestTypes.Nothing;
        //        m_eventUserInputRequest.Set();
        //        for (int tri = 0; tri < 2 && m_uiWaitThread.IsAlive; tri++)
        //        {
        //            if (m_eventUiWaitThreadStopped.WaitOne(100))
        //            {
        //                break;
        //            }

        //            Application.DoEvents();
        //        }

        //        m_isCloseAllActiveThreadsRuned = true;

        //    }

        //}

        protected void GoIdle()
        {
            var act = new Action(delegate
             {
                 if (!m_statusToggler.IsInProgressBarMode)
                 {
                     Clear();
                     m_statusToggler.Toggle(true);
                 }
             }
            );

            if(InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }

        protected void GoResponsive()
        {
            if(m_isContentDeactivated)
                ActivateContent();

            if (m_statusToggler.IsInProgressBarMode)
                m_statusToggler.Toggle(false);

            SuggestionViewerControl.SetFocus();
        }

        protected void Clear()
        {
            rtbErrorText.Clear();
            SuggestionViewerControl.Clear();
        }

        /// <summary>
        /// Specifies the contents of the controls which show error context to the user.
        /// </summary>
        protected void SetErrorContent(string errorText, int errorIndex, int errorLength, VerificationTypes verificationType)
        {
            m_viewerControlArg = null;
            rtbErrorText.Text = "";
            rtbErrorText.SelectionColor = Color.Black;
            rtbErrorText.ForeColor = Color.Black;
            rtbErrorText.Text = errorText;

            rtbErrorText.Select(errorIndex, errorLength);
            rtbErrorText.SelectionColor = GetVerificationColor(verificationType);

            int highlightStart = errorIndex;
            int highlightLength = errorLength;

            if (errorLength < 3)
            {
                highlightStart -= 1;
                highlightStart = Math.Max(highlightStart, 0);
                highlightLength += 2;
                if (highlightStart + highlightLength > rtbErrorText.Text.Length)
                    highlightLength = rtbErrorText.Text.Length - highlightStart;
            }

            rtbErrorText.Select(highlightStart, highlightLength);
            rtbErrorText.SelectionBackColor = HighlightBackColor;

            // Replacing footnote and formula character codes with proper words in Persian
            rtbErrorText.ScrollToCaret();
            rtbErrorText.DeselectAll();
            rtbErrorText.Rtf = rtbErrorText.Rtf.
                Replace("\\'02", WordSpecialCharacters.FootnoteDelimiterReplacementRTF).
                Replace("\\'01", WordSpecialCharacters.FormulaDelimiterReplacementRTF);
        }

        protected void SetSuggestions(ISuggestions suggestions)
        {
            SuggestionViewerControl.SetSuggestions(suggestions);
        }

        protected string SelectedSuggestion
        {
            get
            {
                if(!InvokeRequired)
                    return SuggestionViewerControl.SelectedSuggestion;

                string tempSelSug = "";
                Invoke(new Action(delegate { tempSelSug = SuggestionViewerControl.SelectedSuggestion; }));
                return tempSelSug;
            }
        }

        /// <summary>
        /// Called when the verifier has reached the end of the document and asks if the user
        /// wants to restart from the beginning
        /// </summary>
        protected void AskForContinuation()
        {
            var act = new Action(delegate
             {
                 var result =
                     PersianMessageBox.Show(GetWin32Window(),                        
                         "برنامه به انتهای متن رسید، آیا عملیات مجدداً از ابتدای متن آغاز شود؟",
                         "ویراستیار", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                 if (result != DialogResult.Yes)
                     m_cancelationPending = true;

                 m_eventUserInputAvailable.Set();
             }
            );

            if(InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }

        /// <summary>
        /// Called when the verifier-specified data is available to be shown and the user is needed to
        /// choose an action (such as change, change-all, etc)
        /// </summary>
        protected void AskForUserInteraction()
        {
            var act = new Action(GoResponsive);
            if(InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }

        public void InvokeMethod(Action act)
        {
            if (InvokeRequired)
                Invoke(act);
            else
                act.Invoke();
        }

        public IWin32Window GetWin32Window()
        {
            return new WindowWrapper(this.Handle);
        }

        #endregion

        #region StatusBar Stuff
        public bool AddTextStatusLabel(string name, string value)
        {
            if (!m_statusToggler.IsInProgressBarMode)
            {
                if (!m_dicStatusLables.ContainsKey(name))
                {
                    if (statusStripMain.Items.Count > 0)
                    {
                        var stripLabelSep = new ToolStripStatusLabel(" ")
                        {
                            AutoSize = true,
                            Margin = new Padding(5, 0, 5, 0),
                            BorderSides = ToolStripStatusLabelBorderSides.Left,
                            BorderStyle = Border3DStyle.Etched
                        };
                        statusStripMain.Items.Add(stripLabelSep);
                    }

                    var stripLabel = new ToolStripStatusLabel(value)
                    {
                        AutoSize = true,
                        Margin = new Padding(10, 0, 10, 0)
                    };
                    statusStripMain.Items.Add(stripLabel);

                    m_dicStatusLables.Add(name, stripLabel);

                    return true;
                }
            }
            return false;
        }

        public bool SetStatusValue(string name, string value)
        {
            ToolStripStatusLabel statusLabel;
            if (m_dicStatusLables.TryGetValue(name, out statusLabel))
            {
                statusLabel.Text = value;
                return true;
            }
            return false;
        }

        #endregion

        #region Enable-Disable Action
        public void EnableAction(UserSelectedActions action)
        {
            ChangeActionEnablity(action, true);
        }

        public void DisableAction(UserSelectedActions action)
        {
            ChangeActionEnablity(action, false);
        }

        private void ChangeActionEnablity(UserSelectedActions action, bool makeEnable)
        {
            var act = new Action(() =>
                           {
                               switch (action)
                               {
                                   case UserSelectedActions.Change:
                                       btnChange.Enabled = makeEnable;
                                       break;
                                   case UserSelectedActions.ChangeAll:
                                       btnChangeAll.Enabled = makeEnable;
                                       break;
                                   case UserSelectedActions.Ignore:
                                       btnIgnore.Enabled = makeEnable;
                                       break;
                                   case UserSelectedActions.IgnoreAll:
                                       btnIgnoreAll.Enabled = makeEnable;
                                       break;
                                   case UserSelectedActions.Stop:
                                       btnStop.Enabled = makeEnable;
                                       break;
                                   case UserSelectedActions.AddToDictionary:
                                       btnAddToDictionary.Enabled = makeEnable;
                                       break;
                                       //case UserSelectedActions.Resume:
                                       //case UserSelectedActions.Other:
                                   default:
                                       throw new Exception(String.Format(
                                           "You cannot enable or disable \"{0}\" action", action));
                               }
                           });
            if (this.InvokeRequired)
            {
                this.Invoke(act);
            }
            else
            {
                act.Invoke();
            }
        }

        #endregion

        #region ...Action methods

        private void StopAction()
        {
            // this call causes the ui-wait-thread reach its end
            // then the thread will call close window stuff
            ForceThreadsToClose();
        }

        private void IgnoreAction()
        {
            ProceedVerifying(UserSelectedActions.Ignore);
        }

        private void IgnoreAllAction()
        {
            ProceedVerifying(UserSelectedActions.IgnoreAll);
        }

        private void AddToDictionaryAction()
        {
            ProceedVerifying(UserSelectedActions.AddToDictionary);
        }

        private void ChangeAction()
        {
            ProceedVerifying(UserSelectedActions.Change);
        }

        private void ChangeAllAction()
        {
            ProceedVerifying(UserSelectedActions.ChangeAll);
        }

        private void ResumeAction()
        {
            ProceedVerifying(UserSelectedActions.Resume);
        }


        #endregion

        #region Event Handlers

        private void BtnStopClick(object sender, EventArgs e)
        {
            StopAction();
        }

        private void BtnChangeClick(object sender, EventArgs e)
        {
            ChangeAction();
        }

        private void BtnIgnoreClick(object sender, EventArgs e)
        {
            IgnoreAction();
        }

        private void BtnIgnoreAllClick(object sender, EventArgs e)
        {
            IgnoreAllAction();
        }

        private void BtnAddToDictionaryClick(object sender, EventArgs e)
        {
            AddToDictionaryAction();
        }

        private void BtnChangeAllClick(object sender, EventArgs e)
        {
            ChangeAllAction();
        }

        private void BtnResumeClick(object sender, EventArgs e)
        {
            ActivateContent();
            ResumeAction();
        }

        private void VerificationWindowFormClosing(object sender, FormClosingEventArgs e)
        {
            m_isFormClosed = true;
            ForceThreadsToClose();
            AfterFormCloseCleanUp();
        }

        private void VerificationWindowResize(object sender, EventArgs e)
        {
            AdjustButtonPositions();
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

        #endregion

    }
}

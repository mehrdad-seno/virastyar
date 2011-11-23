using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.Microsoft.Windows;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.Utility.SpellChecker;
using SCICT.Utility.Windows;
using VirastyarWordAddin.Log;
using Point = System.Drawing.Point;

namespace VirastyarWordAddin
{
    public partial class WordCompletionForm : Form
    {
        private WindowListener windowListener;
        private string preText = "";
        private static SessionLogger sessionLogger = new SessionLogger();

        public WordCompletionForm(WindowListener windowListener)
        {
            InitializeComponent();

            this.lstItems.Font = new System.Drawing.Font(this.lstItems.Font.FontFamily, FontSize);
            this.Enabled = GlobalEnabled;

            if(windowListener == null)
            {
                throw new ArgumentNullException("windowListener");
            }

            this.windowListener = windowListener;

            windowListener.AddKeyboardIgnoreCode(33); // PgUp
            windowListener.AddKeyboardIgnoreCode(34); // PgDn
            windowListener.AddKeyboardIgnoreCode(38); // Up
            windowListener.AddKeyboardIgnoreCode(40); // Dn
            windowListener.AddKeyboardIgnoreCode(13); // Enter
            windowListener.AddKeyboardIgnoreCode(9);  // tab
            //windowListener.AddKeyboardIgnoreCode(32); // space

            this.windowListener.KeyDown += new WindowListener.WinMessageEventHandler(windowListener_KeyDown);
            this.windowListener.KeyPressed += new WindowListener.WinMessageEventHandler(windowListener_KeyPressed);
            this.windowListener.LostFocus += new WindowListener.WinMessageEventHandler(windowListener_LostFocus);
            this.windowListener.PositionChanging += new WindowListener.WinMessageEventHandler(windowListener_PositionChanging);
            this.windowListener.Paint += new WindowListener.WinMessageEventHandler(windowListener_Paint);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams ret = base.CreateParams;
                ret.ExStyle |= (int)WindowStyles.WS_EX_NOACTIVATE;
                return ret;
                //ret.Style |= (int)WindowStyles.WS_THICKFRAME | (int)WindowStyles.WS_CHILD;
                //ret.ExStyle |= (int)WindowStyles.WS_EX_NOACTIVATE | (int)WindowStyles.WS_EX_TOOLWINDOW;
                //ret.X = this.Location.X;
                //ret.Y = this.Location.Y;
                //return ret;
            }
        }


        public void ClearItems()
        {
            this.lstItems.Items.Clear();
        }

        public void SetItems(string preText, string[] items)
        {
            this.lstItems.Items.Clear();

            int len = Math.Min(items.Length, AutoCompletetionListMaxCount);

            for (int i = 0; i < len; ++i)
            {
                this.lstItems.Items.Add(items[i]);
            }

            if(this.lstItems.Items.Count > 0)
                this.lstItems.SelectedIndex = 0;

            this.preText = preText;
        }

        public void SetItems(string preText)
        {
            String refinedString = preText;
            if (preText.Length > 0 && StringUtil.IsHalfSpace(preText[preText.Length - 1]))
            {
                refinedString = StringUtil.RefineAndFilterPersianWord(preText.Substring(0, preText.Length - 1)) + PseudoSpace.ZWNJ;
            }
            else
            {
                refinedString = StringUtil.RefineAndFilterPersianWord(preText);
            }

            SetItems(preText, GetWordCompletionSuggestion(refinedString));
        }

        public void ShowAtPoint(System.Drawing.Point point)
        {
            if (this.Enabled)
            {
                this.Location = RefineLocationPoint(point);
                this.Show();
                //this.Visible = this.Enabled;
                windowListener.Focus();
            }
        }

        public void ShowAtCaret()
        {
            ShowAtPoint(GetCaretPosition());
        }

        private void windowListener_Paint(object sender, WindowListenerEventArgs e)
        {
            // if Word's window has been invalidated hide the auto-complete window
            if (this.Visible)
                this.Visible = false;
        }

        private void windowListener_PositionChanging(object sender, WindowListenerEventArgs e)
        {
            // if Word's window has been moved hide the auto-complete window
            if (this.Visible)
                this.Visible = false;
        }

        private void windowListener_LostFocus(object sender, WindowListenerEventArgs e)
        {
            // if the focus has been moved from word to some other application except the
            // autocomplete window then make the auto-complete window disappear
            if (windowListener.HasFocus() || this.Focused || this.lstItems.Focused)
                return;
            this.Visible = false;
        }

        private void windowListener_KeyDown(object sender, WindowListenerEventArgs e)
        {
            windowListener.CheckIgnoreCodes = this.Visible;
            if (!this.Enabled) return;
            int keyValue = e.wparam;

            if (this.Visible)
            {
                // PgUp, PgDn, Up, Down
                if ((keyValue == 33) || (keyValue == 34) || (keyValue == 38) || (keyValue == 40) || (keyValue == 13) || (keyValue == 9))
                {
                    //this.Focus();
                    this.SendKey((char)keyValue);
                }

                if (keyValue == 13 || ((keyValue == 9 /* || keyValue == 32 */) && !e.KeyStateMonitor.Alt && !e.KeyStateMonitor.Control && !e.KeyStateMonitor.Shift))
                {
                    SelectCurrentWord();
                }

                // characters that make the window invisible
                //  ->                  <-                  home                end                 alt                 esc
                if ((keyValue == 39) || (keyValue == 37) || (keyValue == 36) || (keyValue == 35) || (keyValue == 18) || (keyValue == 27))
                {
                    this.Visible = false;
                }

                if (keyValue == 8 && e.KeyStateMonitor.Control)
                {
                    this.Visible = false;
                }
            }
        }

        private void windowListener_KeyPressed(object sender, WindowListenerEventArgs e)
        {
            if (!this.Enabled) return;

            int keyValue = e.wparam;
            char chKey = Convert.ToChar(keyValue);
            if (this.Visible)
            {
                if (keyValue == 8)
                {
                    if (!e.KeyStateMonitor.Control)
                    {
                        if (preText.Length > 0)
                        {
                            preText = preText.Substring(0, preText.Length - 1);
                        }
                    }
                    else
                    {
                        this.Visible = false;
                    }
                }
                else if (StringUtil.IsInArabicWord(chKey))
                {
                    preText += chKey;
                }
                else
                {
                    this.Visible = false;
                }

                if (preText.Length > 0)
                {
                    this.SetItems(preText);
                    //ShowAtCaret();
                }
                else
                {
                    if (this.Visible == true)
                        this.Visible = false;
                }
            }
            else // if is not visible
            {
                if(!CompleteWithoutHotKey) return;

                if (StringUtil.IsInArabicWord(chKey))
                {
                    Range r = RangeUtils.GetWordBeforeCursor(Globals.ThisAddIn.Application.Selection);
                    if (RangeUtils.IsRangeEmpty(r))
                    {
                        TryRetrieveSelection();
                        r = RangeUtils.GetWordBeforeCursor(Globals.ThisAddIn.Application.Selection);
                    }

                    if (!RangeUtils.IsRangeEmpty(r))
                    {
                        string text = r.Text + chKey;
                        string refinedText = StringUtil.RefineAndFilterPersianWord(text);
                        if (refinedText.Length >= CompleteWithoutHotKeyMinLength)
                        {
                            SetItems(text); // make sure you are sending the not-refined version of the string
                            if (lstItems.Items.Count > 0)
                                ShowAtCaret();
                        }
                    }
                }
            }
        }

        private static void TryRetrieveSelection()
        {
            //s_distractionForm.ShowNoFoucus(s_windowListenerInstance.Handle);
            ////s_distractionForm.Focus();
            //s_distractionForm.Visible = false;
            //s_windowListenerInstance.Focus();

            LogHelper.Debug("KeyDown not vis : Range is Empty, Trying timer");
            SetTimerParams("", RangeRecognitionState.PopupInit);
            s_wordCompletionTimer.Enabled = true;
        }

        private void WordCompletionForm_KeyDown(object sender, KeyEventArgs e)
        {
            // 32: space
            // 27: escape
            // 13: enter
            //  9: tab

            if (e.KeyValue == 32 || e.KeyValue == 27 || e.KeyValue == 13 || e.KeyValue == 9)
                this.Visible = false;

            if (e.KeyValue == 9 || e.KeyValue == 13)
                SelectCurrentWord();
        }

        private void WordCompletionForm_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyValue == 33) || (e.KeyValue == 34) || (e.KeyValue == 38) || (e.KeyValue == 40))
            {
                windowListener.Focus();
            }
        }

        private void SelectCurrentWord()
        {
            if (this.lstItems.SelectedItem == null)
                return;

            ReplaceCurrentRangeForWordCompletion(this.lstItems.SelectedItem.ToString());
            this.Visible = false;
        }

        private void ReplaceCurrentRangeForWordCompletion(string replacement)
        {
            //sessionLogger.AddUsage(replacement);

            Range r = RangeUtils.GetWordBeforeCursor(Globals.ThisAddIn.Application.Selection);
            if (RangeUtils.IsRangeEmpty(r))
            {
                TryRetrieveSelection();
                r = RangeUtils.GetWordBeforeCursor(Globals.ThisAddIn.Application.Selection);
            }

            if (!RangeUtils.IsRangeEmpty(r))
            {
                ChangeRangeForWordCompletion(r, replacement);
            }
        }

        private void lstItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectCurrentWord();
        }

        private System.Drawing.Point RefineLocationPoint(System.Drawing.Point point)
        {
            int x = point.X - this.Size.Width + 10; // 10 estimates the window border
            
            if (x < 0) x = 0;

            int y = point.Y + EstimateCaretHeight();
            if (y + this.Height > Screen.PrimaryScreen.Bounds.Height) // if it does not fit underneath
            {
                if (point.Y - this.Height < 0) // and neither fits above 
                {
                    y = Screen.PrimaryScreen.Bounds.Height - this.Height; // it's better to put it underneath
                }
                else // otherwise put it above
                {
                    y = point.Y - this.Height;
                }
            }

            return new Point(x, y);
        }

        private int EstimateCaretHeight()
        {
            int left = 0, top = 0, width, height = 0;
            Range r = Globals.ThisAddIn.Application.Selection.Range;
            if (r != null) // try getting the height from the selection range
            {
                Globals.ThisAddIn.Application.ActiveWindow.GetPoint(out left, out top, out width, out height, r);
            }
            else // if the selection is null try to guess it from some other parameters.
            {
                int zoomPerc = Globals.ThisAddIn.Application.ActiveWindow.View.Zoom.Percentage;
                height = (int)(1.8 * Globals.ThisAddIn.Application.Selection.Range.Font.Size * ((float)zoomPerc / 100));
            }

            return height;
        }

        private void SendKey(char key)
        {
            User32.SendMessage(lstItems.Handle, (int)Msgs.WM_KEYDOWN, new IntPtr(Convert.ToInt32(key)), IntPtr.Zero);
        }

        #region Static stuff

        private static WordCompletionDistractionForm s_distractionForm = new WordCompletionDistractionForm();
        private static WordCompletionForm s_frmInstance = null;
        private static WindowListener s_windowListenerInstance = null;
        private static Range s_rangeToBeChanged = null;
        private static string s_strReplacement = "";
        private static int s_nTimerTickCount = 0;
        private static int s_nFontSize = 10;
        private static RangeRecognitionState s_rangeRecognitionState = RangeRecognitionState.Unknown;
        private static System.Windows.Forms.Timer s_wordCompletionTimer = new System.Windows.Forms.Timer();
        private static bool s_bGlobalEnabled = true;

        static WordCompletionForm()
        {
            s_wordCompletionTimer.Enabled = false;
            s_wordCompletionTimer.Interval = 100;
            s_wordCompletionTimer.Tick += new EventHandler(wordCompletionTimer_Tick);
            CompleteWithoutHotKeyMinLength = 2;
            AutoCompletetionListMaxCount = 700;
            AddSpaceAfterCompletion = true;
        }

        public static bool CompleteWithoutHotKey { get; set; }
        public static int  CompleteWithoutHotKeyMinLength { get; set; }
        public static int  AutoCompletetionListMaxCount { get; set; }
        public static bool AddSpaceAfterCompletion { get; set; }

        public static int FontSize
        {
            get
            {
                return s_nFontSize;
            }

            set
            {
                s_nFontSize = value;
                if (s_frmInstance != null)
                {
                    s_frmInstance.lstItems.Font = new System.Drawing.Font(s_frmInstance.lstItems.Font.FontFamily, FontSize);
                }
            }
        }

        public static bool GlobalEnabled
        {
            get
            {
                return s_bGlobalEnabled;
            }

            set
            {
                s_bGlobalEnabled = value;

                if (s_frmInstance != null)
                {
                    s_frmInstance.Enabled = value;
                }

            }
        }

        public static void WordCompletionEventHandler(object sender, EventArgs e)
        {
            // get the window handle which has the focus
            IntPtr focusedHandle = User32.GetFocus();

            // if it's needed to create a new instance of window-listener
            // i.e. if it's null or it is listening to another window
            if (s_windowListenerInstance == null || s_windowListenerInstance.Handle != focusedHandle)
            {
                // dispose the window-listener which is listening to another window
                if(s_windowListenerInstance != null)
                    s_windowListenerInstance.Dispose();

                s_windowListenerInstance = new WindowListener();
                s_windowListenerInstance.SetWindowHandle(focusedHandle);
            }

            if (s_frmInstance == null)
            {
                s_frmInstance = new WordCompletionForm(s_windowListenerInstance);
            }
            else if (s_frmInstance.windowListener.Handle != s_windowListenerInstance.Handle)
            {
                s_frmInstance.Dispose();
                s_frmInstance = new WordCompletionForm(s_windowListenerInstance);
            }

            Microsoft.Office.Interop.Word.Application wordApp = Globals.ThisAddIn.Application;

            try
            {
                string text;
                string selectedSug = null;
                Range r = RangeUtils.GetWordBeforeCursor(wordApp.Selection);
                if (!RangeUtils.IsRangeEmpty(r))
                {
                    string originalText = r.Text;
                    text = StringUtil.RefineAndFilterPersianWord(originalText);

                    string[] sugs = GetWordCompletionSuggestion(text);
                    if (sugs.Length <= 0)
                    {
                        selectedSug = null;
                    }
                    else if (sugs.Length == 1)
                    {
                        selectedSug = sugs[0];
                    }
                    else
                    {
                        s_frmInstance.SetItems(originalText, sugs);
                        s_frmInstance.ShowAtCaret();
                    }

                    if (selectedSug != null)
                    {
                        ChangeRangeForWordCompletion(r, selectedSug);
                    }
                }
                else // if range is empty
                {
                    LogHelper.Debug("Range is empty");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorException("Error in word completion event handler", ex);
            }
        }

        private static Point GetCaretPosition()
        {
            int left = 0, top = 0, width, height;
            Range r = Globals.ThisAddIn.Application.Selection.Range;
            if (r != null)
            {
                Globals.ThisAddIn.Application.ActiveWindow.GetPoint(out left, out top, out width, out height, r);
            }

            return new Point(left, top);

            //Point caretPos = new Point();
            //User32.GetCaretPos(ref caretPos);
            //IntPtr internalDocHandle = User32.GetFocus();
            //Point realCaretPos = MapClientPointToScreenPoint(internalDocHandle, caretPos);
            //LogHelper.DebugException("caret pos: " + realCaretPos.ToString());
            //return realCaretPos;
        }

        private static Point MapClientPointToScreenPoint(Point pt)
        {
            return MapClientPointToScreenPoint(Process.GetCurrentProcess().MainWindowHandle, pt);
        }

        private static Point MapClientPointToScreenPoint(IntPtr clientHandle, Point pt)
        {
            POINT nativePoint = new POINT(pt.X, pt.Y);
            User32.MapWindowPoints(new HandleRef(null, clientHandle), new HandleRef(null, IntPtr.Zero), ref nativePoint, 1);
            return new Point(nativePoint.X, nativePoint.Y);
        }

        private static void ChangeRangeForWordCompletion(Range r, string str)
        {
            if (r == null || str == null)
                return;

            r.Text = str + (AddSpaceAfterCompletion ? " " : "");
            Globals.ThisAddIn.Application.Selection.Start = r.End;
        }

        private static string[] GetWordCompletionSuggestion(string preStr)
        {
            const int suggestionCount = Int32.MaxValue;
            PersianSpellChecker engine;
            if (!Globals.ThisAddIn.GetSpellCheckerEngine(out engine))
            {
                return new string[0];
            }

            string[] completed = engine.CompleteWord(preStr);

            completed = sessionLogger.Sort(completed);

            if (completed.Length > suggestionCount)
            {
                string[] shorterList = new string[suggestionCount];
                for (int i = 0; i < suggestionCount; ++i)
                {
                    shorterList[i] = completed[i];
                }

                return shorterList;
            }
            else
            {
                return completed;
            }
        }

        public static void BeforeApplicationShutDown()
        {
            if (s_windowListenerInstance != null)
            {
                s_windowListenerInstance.Dispose();
            }
        }

        private static void SetTimerParams(string replacement, RangeRecognitionState state)
        {
            s_strReplacement = replacement;
            s_nTimerTickCount = 0;
            s_rangeRecognitionState = state;
        }

        private static void wordCompletionTimer_Tick(object sender, EventArgs e)
        {
            s_wordCompletionTimer.Enabled = false;
            s_nTimerTickCount++;
            if (s_nTimerTickCount > 3)
            {
                LogHelper.Debug("Failed to change the word content in 10 tries");
                return;
            }

            if (s_nTimerTickCount % 2 == 0)
            {
                s_distractionForm.Visible = true;
                //s_distractionForm.Focus();
                s_distractionForm.Visible = false;
                s_windowListenerInstance.Focus();
            }

            Range r = RangeUtils.GetWordBeforeCursor(Globals.ThisAddIn.Application.Selection);

            if (RangeUtils.IsRangeEmpty(r))
            {
                LogHelper.Debug(String.Format("wordCompletionTimer_Tick : Range is Empty at ({0})th try.", s_nTimerTickCount));
                s_wordCompletionTimer.Enabled = true;
            }
            else
            {
                s_rangeToBeChanged = r;
                RangeBeforeCuresorRecognized();
            }

        }

        private static void RangeBeforeCuresorRecognized()
        {
            if (s_rangeRecognitionState == RangeRecognitionState.RangeModification)
            {
                ChangeRangeForWordCompletion(s_rangeToBeChanged, s_strReplacement);
            }
            else if (s_rangeRecognitionState == RangeRecognitionState.PopupInit)
            {
                TryInitPopup();
            }
        }

        private static void TryInitPopup()
        {
            Microsoft.Office.Interop.Word.Application wordApp = Globals.ThisAddIn.Application;

            if (!RangeUtils.IsRangeEmpty(s_rangeToBeChanged))
            {
                string originalText = s_rangeToBeChanged.Text;
                string text = StringUtil.RefineAndFilterPersianWord(originalText);

                if (text.Length >= CompleteWithoutHotKeyMinLength)
                {
                    string[] sugs = GetWordCompletionSuggestion(text);
                    if (sugs.Length > 0)
                    {
                        s_frmInstance.SetItems(originalText, sugs);
                        s_frmInstance.ShowAtCaret();
                    }
                    else // if range is empty
                    {
                        LogHelper.Debug("Range is empty again");
                    }
                }
            }
        }

        private enum RangeRecognitionState
        {
            Unknown,
            RangeModification,
            PopupInit
        }

        #endregion

    }
}

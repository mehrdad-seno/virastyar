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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Persian.Constants;
using System.Diagnostics;

namespace VirastyarWordAddin
{
    public partial class VerificationWindowBase : Form
    {
        public VerifierBase Verifier { get; set; }

        #region Private Fields

        private VerificationWindowButtons buttonPressed = VerificationWindowButtons.Stop;

        protected Range bParagraph;
        protected Range bContent;

        private Point lastLocation = Point.Empty;

        private Size formDefaultSize = Size.Empty;
        private Size lastVerifSize = Size.Empty;
        private VerificationWindowButtons buttonsToDisable;

        private string helpTopicFileName = "";

        bool confirmButtonPressed = false;
        DialogResult confirmationResult = DialogResult.No;

        #endregion

        public VerificationWindowBase()
        {
            InitializeComponent();
            this.HelpButtonClicked += new CancelEventHandler(VerificationWindowBase_HelpButtonClicked);
            this.HelpRequested += new HelpEventHandler(VerificationWindowBase_HelpRequested);
            this.VerificationType = VerificationTypes.Error;

            formDefaultSize = this.Size;
            lastVerifSize = new Size(550, 350);
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public void SetHelp(string helpTopicFileName)
        {
            this.helpTopicFileName = helpTopicFileName;
            this.ControlBox = true;
            this.HelpButton = true;
        }

        /// <summary>
        /// Disables the buttions.
        /// </summary>
        /// <param name="buttons">The buttons.</param>
        public void DisableButtionsPermanently(VerificationWindowButtons buttons)
        {
            buttonsToDisable = buttons;
            ChangeButtonsStatus(buttons, false);
        }

        public void EnableButtons(VerificationWindowButtons buttons)
        {
            ChangeButtonsStatus(buttons, true);
        }

        private void ChangeButtonsStatus(VerificationWindowButtons buttons, bool enabled)
        {
            if ((VerificationWindowButtons.Change & buttons) == VerificationWindowButtons.Change)
                btnChange.Enabled = enabled;
            if ((VerificationWindowButtons.ChangeAll & buttons) == VerificationWindowButtons.ChangeAll)
                btnChangeAll.Enabled = enabled;
            if ((VerificationWindowButtons.AddToDictionary & buttons) == VerificationWindowButtons.AddToDictionary)
                btnAddToDic.Enabled = enabled;
            if ((VerificationWindowButtons.Ignore & buttons) == VerificationWindowButtons.Ignore)
                btnIgnore.Enabled = enabled;
            if ((VerificationWindowButtons.IgnoreAll & buttons) == VerificationWindowButtons.IgnoreAll)
                btnIgnoreAll.Enabled = enabled;
        }

        public bool CanEnableButton(VerificationWindowButtons button)
        {
            return ((button & buttonsToDisable) != button);
        }

        /// <summary>
        /// Gets the color of the verification.
        /// </summary>
        /// <param name="vt">The vt.</param>
        /// <returns></returns>
        public Color GetVerificationColor(VerificationTypes vt)
        {
            switch (vt)
            {
                case VerificationTypes.Error:
                    return Color.Red;
                case VerificationTypes.Warning:
                    return Color.Green;
                case VerificationTypes.Information:
                default:
                    return Color.Blue;
            }
        }

        /// <summary>
        /// Gets the button pressed.
        /// </summary>
        /// <value>The button pressed.</value>
        public VerificationWindowButtons ButtonPressed
        {
            get
            {
                return buttonPressed;
            }
        }

        /// <summary>
        /// Gets the selected suggestion.
        /// </summary>
        /// <value>The selected suggestion.</value>
        public virtual string SelectedSuggestion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the caption of the dialog
        /// </summary>
        /// <param name="caption">The caption.</param>
        public void SetCaption(string caption)
        {
            this.Text = " " + caption + " ";
        }

        /// <summary>
        /// Sets the content caption.
        /// </summary>
        /// <param name="caption">The caption.</param>
        public void SetContentCaption(string caption)
        {
            this.lblContentCaption.Text = caption;
        }

        #region Virtual Methods
        protected virtual void OnCancelButtonPressed()
        {
            buttonPressed = VerificationWindowButtons.Stop;
            Verifier.RequestCancellation();
            //this.Close();
        }

        protected virtual void OnIgnoreButtonPressed()
        {
            buttonPressed = VerificationWindowButtons.Ignore;
            //this.Close();
        }

        protected virtual void OnChangeButtonPressed()
        {
            buttonPressed = VerificationWindowButtons.Change;
            //this.Close();
        }

        protected virtual void OnChangeAllButtonPressed()
        {
            buttonPressed = VerificationWindowButtons.ChangeAll;
            //this.Close();
        }

        protected virtual void OnAddToDicButtonPressed()
        {
            buttonPressed = VerificationWindowButtons.AddToDictionary;
            //this.Close();
        }

        protected virtual void OnIgnoreAllButtonPressed()
        {
            buttonPressed = VerificationWindowButtons.IgnoreAll;
            //this.Close();
        }

        #endregion

        #region Event Handlers

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancelButtonPressed();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            OnIgnoreButtonPressed();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            OnChangeButtonPressed();
        }

        private void btnChangeAll_Click(object sender, EventArgs e)
        {
            OnChangeAllButtonPressed();
        }

        private void btnIgnoreAll_Click(object sender, EventArgs e)
        {
            OnIgnoreAllButtonPressed();
        }

        private void btnAddToDic_Click(object sender, EventArgs e)
        {
            OnAddToDicButtonPressed();
        }

        private void VerificationWindowBase_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                lastLocation = this.Location;
            }
            else
            {
                if (lastLocation != Point.Empty)
                    this.Location = lastLocation;
                buttonPressed = VerificationWindowButtons.Stop;
            }
        }

        #endregion

        public VerificationTypes VerificationType { get; set; }

        private void FixWindowLocation(Range r)
        {
            int rangeLeft, rangeTop, rangeWidth, rangeHeight;
            Globals.ThisAddIn.Application.ActiveWindow.GetPoint(out rangeLeft, out rangeTop, out rangeWidth, out rangeHeight, r);

            int x = rangeLeft;
            if (rangeWidth < 0)
            {
                x += rangeWidth;
                rangeWidth = Math.Abs(rangeWidth);
            }

            int scrWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int scrHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // if it is masked
            if ((this.Left <= x && x <= this.Left + this.Width || this.Left <= x + rangeWidth && x + rangeWidth <= this.Left + this.Width) &&
                (this.Top <= rangeTop && rangeTop <= this.Top + this.Height || this.Top <= rangeTop + rangeHeight && rangeTop + rangeHeight <= this.Top + this.Height))
            {
                // if it is masked vertically
                if (this.Top <= rangeTop && rangeTop <= this.Top + this.Height || this.Top <= rangeTop + rangeHeight && rangeTop + rangeHeight <= this.Top + this.Height)
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
                    // if there's not enough space neither below nor above it try to move it horizontally
                    else
                    {
                        // TODO
                        //    // if it can be shown on both sides
                        //    if (x > this.Width && scrWidth - x > this.Width)
                        //    {
                        //    }
                        //    else // if it can be shown on either side or none
                        //    {
                        //    }
                    }
                }
            }
        }

        private int m_highlightBorder = 0;
        protected bool SetContent(Range rParagraph, Range rContent)
        {
            if (rParagraph == null) return false;
            if (rContent == null) return false;
            if (rContent.Text == null) return false;

            object oStart = Missing.Value;
            Globals.ThisAddIn.Application.ActiveWindow.ScrollIntoView(rContent, ref oStart);
            FixWindowLocation(rContent);

            rtbMainContent.Text = "";
            rtbMainContent.SelectionColor = Color.Black;
            rtbMainContent.ForeColor = Color.Black;
            rtbMainContent.Text = rParagraph.Text;

            int beforeContentLength = 0;
            if (rContent.Start > rParagraph.Start)
            {
                Range rBeforeContent = rParagraph.Words[1];
                rBeforeContent.SetRange(rParagraph.Start, rContent.Start);
                if (rBeforeContent == null || rBeforeContent.Text == null)
                    beforeContentLength = 0;
                else
                    beforeContentLength = rBeforeContent.Text.Length;
            }

            int startIndex = beforeContentLength;
            int length = rContent.Text.Length;

            rContent.Select();

            rtbMainContent.Select(startIndex, length);
            rtbMainContent.SelectionColor = GetVerificationColor(VerificationType);
            if(length < 3 && m_highlightBorder <= 0)
                m_highlightBorder = 1;
            startIndex -= m_highlightBorder;
            startIndex = Math.Max(startIndex, 0);
            length += 2 * m_highlightBorder;
            if (startIndex + length > rtbMainContent.Text.Length)
                length = rtbMainContent.Text.Length - startIndex;
            rtbMainContent.Select(startIndex, length);
            rtbMainContent.SelectionBackColor = Color.Yellow;
            
            // Replacing footnote and formula character codes with proper words in Persian
            rtbMainContent.ScrollToCaret();
            rtbMainContent.DeselectAll();
            rtbMainContent.Rtf = rtbMainContent.Rtf.Replace("\\'02", WordSpecialCharacters.FootnoteDelimiterReplacementRTF).
                Replace("\\'01", WordSpecialCharacters.FormulaDelimiterReplacementRTF);

            return true;
        }

        private void VerificationWindowBase_Load(object sender, EventArgs e)
        {
            SwitchToProgressMode();
        }

        public VerificationWindowButtons ShowDialog(BaseVerificationWinArgs args, out string selectedSugs)
        {
            Globals.ThisAddIn.UsageLogger.SetContent(args.bContent.Text);

            selectedSugs = "";
            
            buttonPressed = VerificationWindowButtons.None;
            SwitchToVerificationMode();

            if (!PrepareAlert(args))
            {
                // TODO: Check return type
                return VerificationWindowButtons.Stop;
            }

            while (buttonPressed == VerificationWindowButtons.None)
            {
                ThisAddIn.ApplicationDoEvents();
                //Thread.Sleep(10);
                ThisAddIn.DebugWriteLine("ShowDialog: " + Environment.TickCount);
            }

            if (buttonPressed == VerificationWindowButtons.Stop)
            {
                throw new VerificationCanceledException();
            }

            selectedSugs = this.SelectedSuggestion;

            Globals.ThisAddIn.UsageLogger.SetAction(buttonPressed.ToString());
            Globals.ThisAddIn.UsageLogger.LogLastAction();

            SwitchToProgressMode();
            return buttonPressed;
        }

        /// <summary>
        /// Sample implementation
        /// </summary>
        protected virtual bool PrepareAlert(BaseVerificationWinArgs args)
        {
            return SetContent(args.bParagraph, args.bContent);
        }

        private void VerificationWindowBase_Shown(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                try
                {
                    Verifier.Verify();
                }
                catch (Exception ex)
                {
                    Globals.ThisAddIn.OnExceptionOccured(ex);
                }
                Close();
            }
        }

        protected void SwitchToVerificationMode()
        {
            this.panelConfirmation.Visible = false;
            this.panelProgressMode.Visible = false;
            this.panelVerificationMode.Visible = true;

            this.CancelButton = btnStopVerify;
            this.AcceptButton = null;


            FitPanelInForm(panelVerificationMode);
            AfterVerificationModeDesignChanges();
        }

        virtual protected void AfterVerificationModeDesignChanges()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(550, 350);
            if(lastVerifSize != Size.Empty)
                this.Size = lastVerifSize;
            this.panelVerificationMode.Dock = DockStyle.Fill;
        }

        protected void SwitchToConfirmationMode()
        {
            if (panelVerificationMode.Visible == true) // i.e. the prior mode was verification mode
            {
                this.lastVerifSize = this.Size;
            }

            this.panelVerificationMode.Visible = false;
            this.panelProgressMode.Visible = false;
            this.panelConfirmation.Visible = true;

            this.CancelButton = btnConfirmNo;
            this.AcceptButton = null;

            FitPanelInForm(panelConfirmation);
            AfterConfirmationModeDesignChanges();
        }

        virtual protected void AfterConfirmationModeDesignChanges()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.panelVerificationMode.Dock = DockStyle.None;
            this.MinimumSize = new Size(0, 0);
            this.Size = new Size(this.formDefaultSize.Width + 25, this.panelConfirmation.Size.Height + 30);
        }

        protected void SwitchToProgressMode()
        {
            if (panelVerificationMode.Visible == true) // i.e. the prior mode was verification mode
            {
                this.lastVerifSize = this.Size;
            }

            this.panelConfirmation.Visible = false;
            this.panelVerificationMode.Visible = false;
            this.panelProgressMode.Visible = true;

            this.CancelButton = btnStopProgress;
            this.AcceptButton = null;

            FitPanelInForm(panelProgressMode);
            AfterProgressModeDesignChanges();
        }

        virtual protected void AfterProgressModeDesignChanges()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.panelVerificationMode.Dock = DockStyle.None;
            this.MinimumSize = new Size(0, 0);
            this.Size = new Size(this.formDefaultSize.Width + 25, this.panelProgressMode.Size.Height + 30);
        }

        protected void FitPanelInForm(Panel panel)
        {
            panel.Top = 5;
            this.Size = new Size(panel.Size.Width + 25, panel.Size.Height + 30);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            OnCancelButtonPressed();
        }

        public void SetProgress(int progress)
        {
            if (progress < 0) progress = 0;
            if (progress > 100) progress = 100;
            this.progressBar.Value = progress;
        }

        internal DialogResult ConfirmContinue()
        {
            confirmButtonPressed = false;

            SwitchToConfirmationMode();

            while (!confirmButtonPressed)
            {
                ThisAddIn.ApplicationDoEvents();
                //Thread.Sleep(10);
                ThisAddIn.DebugWriteLine("ConfirmContinue: " + Environment.TickCount);
            }

            return confirmationResult;
        }

        private void btnConfirmYes_Click(object sender, EventArgs e)
        {
            confirmationResult = DialogResult.Yes;
            confirmButtonPressed = true;
        }

        private void btnConfirmNo_Click(object sender, EventArgs e)
        {
            confirmationResult = DialogResult.No;
            confirmButtonPressed = true;
        }

        void VerificationWindowBase_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Globals.ThisAddIn.ShowHelp(helpTopicFileName);
        }

        void VerificationWindowBase_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Globals.ThisAddIn.ShowHelp(helpTopicFileName);
        }

        private void VerificationWindowBase_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                if (CancelButton != null)
                    CancelButton.PerformClick();
            }
            else if (e.KeyData == Keys.Enter)
            {
                if (AcceptButton != null)
                    AcceptButton.PerformClick();
            }
        }
    }
}

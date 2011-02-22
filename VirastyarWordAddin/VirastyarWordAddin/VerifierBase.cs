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
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;

namespace VirastyarWordAddin
{
    /// <summary>
    /// Base class for all verifiers.
    /// </summary>
    public abstract class VerifierBase
    {
        protected VerifierBase()
        {
            InitVerifWin();
        }

        protected virtual void InitVerifWin()
        {
            m_verificationWindow = new VerificationWindowBase {Verifier = this};
        }

        #region Private Members
        
        private TextProcessType m_verifierType = TextProcessType.Interactive;
        protected VerificationWindowBase m_verificationWindow = null;

        #endregion

        #region Verify

        /// <summary>
        /// Note: Please set the desired TextProcessType before calling this method
        /// </summary>
        public void Verify()
        {
            CancellationPending = false;
            VerifyDocument();
        }

        private void VerifyDocument()
        {
            using (MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument)
            {
                if (d == null) return;

                Selection selection = Globals.ThisAddIn.Application.Selection;
                MSWordBlock curPar = null;

                bool isSelectionFound = VerifierType == TextProcessType.Batch;
                bool finished = false;
                int paragraphCount = 0;
                int maxParagraphs = d.CurrentMSDocument.Paragraphs.Count;

                #region Main processing loop
                while (!finished)
                {
                    paragraphCount = 0;
                    foreach (var b in ((MSWordBlock)d.GetContent()).RawParagraphs)
                    {
                        paragraphCount++;
                        m_verificationWindow.SetProgress((paragraphCount * 100) / maxParagraphs);
                        ThisAddIn.ApplicationDoEvents();

                        if (CancellationPending)
                            goto FINAL;
                        
                        try
                        {
                            if (!isSelectionFound)
                            {
                                if (Globals.ThisAddIn.IsSelectionStartedInBlock(selection, b))
                                {
                                    isSelectionFound = true;
                                    curPar = b;
                                    try
                                    {
                                        if (!ProcessParagraphForVerification(b))
                                        {
                                            finished = true;
                                            break;
                                        }
                                    }
                                    catch (VerificationCanceledException)
                                    {
                                        throw;
                                    }
                                    catch (Exception ex)
                                    {
                                        Globals.ThisAddIn.OnExceptionOccured(ex);
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (!ProcessParagraphForVerification(b))
                                    {
                                        finished = true;
                                        break;
                                    }
                                }
                                catch (VerificationCanceledException)
                                {
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    Globals.ThisAddIn.OnExceptionOccured(ex);
                                }
                            }
                        }
                        catch (VerificationCanceledException ex)
                        {
                            goto FINAL;
                        }
                    }

                    if (VerifierType == TextProcessType.Batch)
                    {
                        finished = true;
                    }

                    if (!finished)
                    {
                        var dr = m_verificationWindow.ConfirmContinue();
                        if (dr == DialogResult.No)
                        {
                            finished = true;
                        }
                        else
                        {
                            isSelectionFound = true; // as if the cursor was in the first paragraph
                        }
                    }
                }
                #endregion

            // Nothing. None of anybody's business. I like to use goto. Chie dadash?
            FINAL: ;
            }
        }

        #endregion

        #region VerificationWindow

        /*protected VerificationWindowButtons ShowVerificationWindow(SpellCheckerVerificationWindow verificationWinow, 
            Range bParagraph, Range bContent, string[] sugs, out string selectedSug)
        {
            Globals.ThisAddIn.usageLogger.SetContent(bContent.Text);

            selectedSug = "";
            VerificationWindowButtons rtValue = VerificationWindowButtons.Stop;
            try
            {
                if (verificationWinow != null)
                {
                    if (verificationWinow.SetContent(bParagraph, bContent, sugs))
                    {
                        //verificationWinow.ShowDialog();
                        SpellCheckVerificationWinArgs args = new SpellCheckVerificationWinArgs();
                        //args.
                        m_verificationWindow.Alert(args);

                        rtValue = verificationWinow.ButtonPressed;
                        selectedSug = verificationWinow.SelectedSuggestion;
                    }
                    else
                    {
                        rtValue = VerificationWindowButtons.Ignore;
                        selectedSug = "";
                    }
                }
            }
            catch (VerificationCanceledException ex)
            {
                rtValue = VerificationWindowButtons.Stop;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            Globals.ThisAddIn.usageLogger.SetAction(rtValue.ToString());
            Globals.ThisAddIn.usageLogger.LogLastAction();

            return rtValue;

        }*/

        /*protected VerificationWindowButtons ShowVerificationWindow(SpellCheckerVerificationWindow verificationWinow, Range bParagraph, Range word1, Range word2, string[] sugs, out string selectedSug)
        {
            Range bContent = word1;
            if (word2 != null)
            {
                object oMissing = System.Reflection.Missing.Value;
                if (word1 != null)
                {
                    Range r = word1.Next(ref oMissing, ref oMissing);
                    r.SetRange(word1.Start, word2.End);
                    bContent = r;
                }
                else
                {
                    bContent = word2;
                }
            }

            return ShowVerificationWindow(verificationWinow, bParagraph, bContent, sugs, out selectedSug);
        }*/ 

        #endregion

        public void RequestCancellation()
        {
            CancellationPending = true;
        }

        public bool CancellationPending { get; private set; }

        #region Virtual and Abstract methods

        protected abstract bool ProcessParagraphForVerification(MSWordBlock b);

        protected virtual void ResetStats()
        {
            ThisAddIn.DebugWriteLine("Not implemented in " + GetType());
        }

        public virtual void ShowStats()
        {
            ThisAddIn.DebugWriteLine("Not implemented in " + GetType());
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the current processing type.
        /// </summary>
        public TextProcessType VerifierType
        {
            get
            {
                return m_verifierType;
            }
            protected set
            {
                m_verifierType = value;
            }
        }

        #endregion

        public void RunVerify()
        {
            Debug.Assert(m_verificationWindow != null);
            ResetStats();
            if (m_verificationWindow == null)
            {
                throw new Exception("The reference to the Verification Window is null!");
            }
            m_verificationWindow.ShowDialog();
        }

        public void RunVerify(TextProcessType type)
        {
            this.VerifierType = type;
            WdViewType oldViewType = Globals.ThisAddIn.Application.ActiveWindow.View.Type;
            try
            {
                RunVerify();
            }
            finally
            {
                Globals.ThisAddIn.Application.ActiveWindow.View.Type = oldViewType;
            }
        }
    }
}

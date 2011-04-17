using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using VirastyarWordAddin.Log;

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

        public void RequestCancellation()
        {
            CancellationPending = true;
        }

        public bool CancellationPending { get; private set; }

        #region Virtual and Abstract methods

        protected abstract bool ProcessParagraphForVerification(MSWordBlock b);

        protected virtual void ResetStats()
        {
            LogHelper.Debug("Not implemented in " + GetType());
        }

        public virtual void ShowStats()
        {
            LogHelper.Debug("Not implemented in " + GetType());
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

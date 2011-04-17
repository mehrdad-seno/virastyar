using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.TextProofing.Punctuation;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.GUI;

namespace VirastyarWordAddin
{
    class PunctuationVerifier : VerifierBase
    {
        #region Private Fields

        private PunctuationCheckerEngine PunctuationCheckerEngine = null;
        private int m_stats = 0;

        #endregion

        #region Ctors

        public PunctuationVerifier()
            : base()
        {
            PunctuationCheckerEngine = new PunctuationCheckerEngine(SettingsHelper.GetFullPath(Constants.PatternsFileName, VirastyarFilePathTypes.AllUsersFiles));
        }

        protected override void InitVerifWin()
        {
            m_verificationWindow = new SimpleVerificationWindow();
            m_verificationWindow.SetCaption("تصحیح نشانه‌گذاری");
            m_verificationWindow.DisableButtionsPermanently(
                VerificationWindowButtons.AddToDictionary);
            m_verificationWindow.Verifier = this;
            m_verificationWindow.SetHelp(HelpConstants.Punctuations);
        }

        #endregion

        private int SearchArray(string[] ar, string key)
        {
            for (int i = 0; i < ar.Length; ++i)
                if (ar[i] == key)
                    return i;
            return -1;
        }

        protected override bool ProcessParagraphForVerification(MSWordBlock b)
        {
            Range range = b.Range;
            RangeUtils.TrimRange(range);
            string text = range.Text;
            
            if (String.IsNullOrEmpty(text))
                return true;

            PunctuationCheckerEngine.InitInputString(text);

            do
            {
                ThisAddIn.ApplicationDoEvents();

                PunctuationCheckerEngine.FindMistake();
                if (!PunctuationCheckerEngine.IsErrorFound())
                    break;

                //ThisAddIn.ApplicationDoEvents();

                string selectedSug;
                string[] sugs = PunctuationCheckerEngine.GetMultiSubstitutionString();
                    //new string[] { PunctuationCheckerEngine.getSubstitutionString() };


                int startIndex = PunctuationCheckerEngine.GetMistakeIndex();
                int length = PunctuationCheckerEngine.GetMistakeLength();
                int endIndex = length + startIndex - 1;
                //string tobeReplacedText = text.Substring(startIndex, endIndex - startIndex + 1);
                Range curPunc;

                if (startIndex >= text.Length)
                {
                    Range r = range.Words[1];
                    r.SetRange(range.Start, range.End);
                    RangeUtils.ShrinkRangeToLastWord(r);

                    string rText = r.Text;

                    int newSugLen;
                    for (int i = 0; i < sugs.Length; ++i)
                    {
                        newSugLen = rText.Length - sugs[i].Length + length;
                        if (newSugLen <= rText.Length)
                        {
                            sugs[i] = rText.Substring(0, newSugLen) + sugs[i];
                        }
                        else // TODO: Sorry for this, fix it later
                        {
                            PunctuationCheckerEngine.SkipMistake();
                            return true;
                        }
                    }
                    curPunc = r;
                }
                else
                {
                    //string textToMatch = text.Substring(startIndex, length);
                    curPunc = range.GetSubRange(startIndex, endIndex);
                }

                //Debug.Assert(tobeReplacedText == curPunc.Text);
                //if (tobeReplacedText != curPunc.Text) 
                //{
                //    // do not replace ignore the problem
                //    PunctuationCheckerEngine.skipMistake();
                //}

                if (VerifierType == TextProcessType.Interactive)
                {

                    VerificationWindowButtons buttonPressed = VerificationWindowButtons.Ignore;
                    ((SimpleVerificationWindow)m_verificationWindow).SetSuggestionCaption(PunctuationCheckerEngine.GetMistakeDescription());
                    ((SimpleVerificationWindow)m_verificationWindow).IsChangeAllPossible = PunctuationCheckerEngine.IsAllChangeable();

                    buttonPressed = m_verificationWindow.ShowDialog(new PunctuationVerificationWinArgs(range, curPunc, sugs), out selectedSug);
                    int selectedIndex = SearchArray(sugs, selectedSug);

                    switch (buttonPressed)
                    {
                        case VerificationWindowButtons.Change:
                            try
                            {
                                if (Globals.ThisAddIn.SetRangeContent(curPunc, selectedSug, true))
                                {
                                    range.SetRange(range.Start, Math.Max(curPunc.End, range.End));
                                    PunctuationCheckerEngine.CorrectMistake(selectedIndex);
                                }
                                else
                                {
                                    PunctuationCheckerEngine.SkipMistake();
                                }
                            }
                            catch (COMException)
                            {
                                // Ignore
                            }

                            bool check = PunctuationCheckerEngine.GetCorrectedString().Trim() == range.Text.Trim();
                            if (!check)
                            {
                                object n = 1;
                                Globals.ThisAddIn.Application.ActiveDocument.Undo(ref n);

                                //PunctuationCheckerEngine.SkipMistake();
                                //Debug.Assert(check == true);
                                //throw new Exception("Word Content and punctuation engine content do not match!");
                                return true;
                            }
                            break;
                        case VerificationWindowButtons.ChangeAll:
                            // The change code
                            try
                            {
                                if (Globals.ThisAddIn.SetRangeContent(curPunc, selectedSug, true))
                                {
                                    range.SetRange(range.Start, Math.Max(curPunc.End, range.End));
                                    PunctuationCheckerEngine.CorrectMistake(selectedIndex);
                                }
                                else
                                {
                                    PunctuationCheckerEngine.SkipMistake();
                                }
                            }
                            catch (COMException)
                            {
                                // Ignore
                            }

                            bool check2 = PunctuationCheckerEngine.GetCorrectedString().Trim() == range.Text.Trim();
                            if (!check2)
                            {
                                object n = 1;
                                Globals.ThisAddIn.Application.ActiveDocument.Undo(ref n);

                                //PunctuationCheckerEngine.SkipMistake();
                                Debug.Assert(check2 == true);
                                //throw new Exception("Word Content and punctuation engine content do not match!");
                                return true;
                            }

                            PunctuationCheckerEngine.SetGoldenRule();

                            // Now the new change-all stuff
                            PunctuationCheckerEngine.BookMarkSkipIndex();

                            PerformReplaceAll(range, selectedIndex);
                            // extra change-all stuff

                            Range renewPar = range.Paragraphs[1].Range;
                            renewPar.Trim();
                            range = renewPar;

                            PunctuationCheckerEngine.InitInputString(range.Text);
                            PunctuationCheckerEngine.RecallSkipIndex();
                            PunctuationCheckerEngine.UnsetGoldenRule();
                            break;
                        case VerificationWindowButtons.Ignore:
                            PunctuationCheckerEngine.SkipMistake();
                            break;
                        case VerificationWindowButtons.IgnoreAll:
                            PunctuationCheckerEngine.DisableLastRule();
                            break;
                        case VerificationWindowButtons.Stop:
                        default:
                            return false;
                    }
                }
                else if (VerifierType == TextProcessType.Batch)
                {
                    if (sugs.Length > 0)
                    {
                        selectedSug = sugs[0];
                        try
                        {
                            if (Globals.ThisAddIn.SetRangeContent(curPunc, selectedSug, false))
                            {
                                range.SetRange(range.Start, Math.Max(curPunc.End, range.End));
                                PunctuationCheckerEngine.CorrectMistake(0);
                                m_stats++;
                            }
                            else
                            {
                                PunctuationCheckerEngine.SkipMistake();
                            }
                        }
                        catch (COMException)
                        {
                            // Ignore
                        }

                        bool check = PunctuationCheckerEngine.GetCorrectedString().Trim() == range.Text.Trim();
                        if (!check)
                        {
                            object n = 1;
                            Globals.ThisAddIn.Application.ActiveDocument.Undo(ref n);

                            //PunctuationCheckerEngine.SkipMistake();
                            Debug.Assert(check == true);
                            //throw new Exception("Word Content and punctuation engine content do not match!");
                            return true;

                        }
                    }
                }
            } while (!CancellationPending);

            return true;
        }

        private void PerformReplaceAll(Range curPar, int selectedIndex)
        {
            using (MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument)
            {
                if (d == null) return;

                Range rPar = null;
                string strPar = null;
                string curSug = "";

                foreach (MSWordBlock p in ((MSWordBlock)d.GetContent()).RawParagraphs)
                {
                    rPar = p.Range;
                    rPar.Trim();
                    if (rPar == null || rPar.Text == null)
                        continue;

                    //if (RangeUtils.AreRangesEqual(rPar, curPar))
                    //    continue;

                    strPar = rPar.Text.Trim();

                    PunctuationCheckerEngine.InitInputString(strPar);

                    PunctuationCheckerEngine.FindMistake();
                    while (PunctuationCheckerEngine.IsErrorFound())
                    {
                        curSug = PunctuationCheckerEngine.GetMultiSubstitutionString()[selectedIndex];

                        int startIndex = PunctuationCheckerEngine.GetMistakeIndex();
                        int endIndex = PunctuationCheckerEngine.GetMistakeLength() + startIndex - 1;
                        Range curPunc;

                        if (startIndex >= strPar.Length)
                        {
                            Range r = rPar.Words[1];
                            r.SetRange(rPar.Start, rPar.End);
                            RangeUtils.ShrinkRangeToLastWord(r);

                            curSug = r.Text + curSug;
                            curPunc = r;
                        }
                        else
                        {
                            //curPunc = GetSubRange(b.Range, startIndex, endIndex + 1);
                            curPunc = RangeUtils.GetSubRange2(rPar, startIndex, endIndex);
                        }

                        try
                        {
                            if (Globals.ThisAddIn.SetRangeContent(curPunc, curSug, false))
                            {
                                rPar.SetRange(rPar.Start, Math.Max(curPunc.End, rPar.End));
                                PunctuationCheckerEngine.CorrectMistake(selectedIndex);
                            }
                            else
                            {
                                PunctuationCheckerEngine.SkipMistake();
                            }
                        }
                        catch (COMException)
                        {
                            // Ignore
                        }

                        bool check = PunctuationCheckerEngine.GetCorrectedString().Trim() == rPar.Text.Trim();
                        Debug.Assert(check == true);
                        if (check != true)
                        {
                            throw new Exception("Word content and punctuation engine content do not match!");
                        }

                        PunctuationCheckerEngine.FindMistake();
                    }
                }
            }
        }

        protected override void ResetStats()
        {
            m_stats = 0;
        }

        public override void ShowStats()
        {
            string message = string.Format("تعداد اصلاحات انجام شده: {0}", ParsingUtils.ConvertNumber2Persian(m_stats.ToString()));
            PersianMessageBox.Show(message, Constants.UIMessages.SuccessRefinementTitle);
        }
    }
}

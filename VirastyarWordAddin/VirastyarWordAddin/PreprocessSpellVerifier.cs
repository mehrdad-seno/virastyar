using System;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.GUI;
using SCICT.Utility.SpellChecker;
using VirastyarWordAddin.Log;

namespace VirastyarWordAddin
{
    /// <summary>
    /// 
    /// </summary>
    class PreprocessSpellVerifier : VerifierBase
    {
        #region Private Fields

        private int m_stats = 0;
        private PersianSpellChecker engine;
        private SessionLogger sessionLogger;
        // private SpellCheckerConfig curSettings;

        #endregion

        #region Ctors

        public PreprocessSpellVerifier(PersianSpellChecker engine) : base()
        {
            this.engine = engine;
            this.sessionLogger = new SessionLogger();
            //ChangeConfig(ss);
        }

        public PreprocessSpellVerifier(PersianSpellChecker engine, SessionLogger sessionLogger)
            : base()
        {
            this.engine = engine;
            this.sessionLogger = sessionLogger;
            //ChangeConfig(ss);
        }

        protected override void InitVerifWin()
        {
            base.InitVerifWin();
            m_verificationWindow.SetCaption("پیش‌پردازش املایی");
            this.m_verificationWindow.SetHelp(HelpConstants.SpellCheckerPreprocessing);
        }

        #endregion

        private int m_grams = 1;
        protected override bool ProcessParagraphForVerification(MSWordBlock b)
        {
            b.Range.SelectIfPossible();

            #region The n-gram code - Commented Out (DO NOT DELETE)

            //RoundQueue<Range> rq = new RoundQueue<Range>(m_grams);

            //Range[] items;
            //int curItemIndex;

            //bool curHLStatus = false;
            //bool prevHLStatus = false;

            //string realWord;
            //bool canProceed = false;
            //Range w, rPrev = null;
            //for (w = b.Range.GetFirstWord(); w != null; w = w.NextWord())
            //{
            //    if (CancellationPending)
            //        return false;

            //    ThisAddIn.ApplicationDoEvents();

            //    string tempStr = w.Text; // to use in the following conditio; in order to reduce COM calls

            //    if (tempStr != null && tempStr.StartsWith("\r"))
            //        break;

            //    w.Trim();

            //    bool bAreRangesEqual = false;
            //    Range lastItem = rq.GetLastItem();
            //    if (w != null && lastItem != null)
            //    {
            //        bAreRangesEqual = RangeUtils.AreRangesEqual(w, lastItem); // i.e., if the currently read range equals the most recent read range; it is a duplication
            //    }

            //    if (w == null || w.Text == null || bAreRangesEqual)
            //    {
            //        rq.BlockEntry();
            //        while (rq.ReadNextWordLists(out items, out curItemIndex))
            //        {
            //            canProceed = true;
            //            do
            //            {
            //                if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //                    return false;
            //            } while (!canProceed);
            //        }

            //        continue;
            //    }

            //    realWord = StringUtil.RefineAndFilterPersianWord(w.Text);
            //    curHLStatus = RangeUtils.IsRangeInsideHyperlink(w);

            //    if (curHLStatus != prevHLStatus)
            //    {
            //        rq.BlockEntry();
            //        while (rq.ReadNextWordLists(out items, out curItemIndex))
            //        {
            //            canProceed = true;
            //            do
            //            {
            //                if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //                    return false;
            //            } while (!canProceed);
            //        }
            //    }

            //    if (realWord.Length > 0 && StringUtil.IsInArabicWord(realWord[0]))
            //    {
            //        rq.AddItem(w);
            //        if (rq.ReadNextWordLists(out items, out curItemIndex))
            //        {
            //            canProceed = true;
            //            do
            //            {
            //                if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //                    return false;
            //            } while (!canProceed);
            //        }
            //    }
            //    else
            //    {
            //        rq.BlockEntry();
            //        while (rq.ReadNextWordLists(out items, out curItemIndex))
            //        {
            //            canProceed = true;
            //            do
            //            {
            //                if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //                    return false;
            //            } while (!canProceed);
            //        }
            //    }

            //    //------------ old
            //    //if (bAreRangesEqual || w == null || w.Text == null || !(realWord.Length > 0 && StringUtil.IsInArabicWord(realWord[0])))
            //    //{
            //    //    rq.BlockEntry();
            //    //    while (rq.ReadNextWordLists(out items, out curItemIndex))
            //    //    {
            //    //        canProceed = true;
            //    //        do
            //    //        {
            //    //            if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //    //                return false;
            //    //        } while (!canProceed);
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    rq.AddItem(w);
            //    //    if (rq.ReadNextWordLists(out items, out curItemIndex))
            //    //    {
            //    //        canProceed = true;
            //    //        do
            //    //        {
            //    //            if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //    //                return false;
            //    //        } while (!canProceed);
            //    //    }
            //    //}

            //    prevHLStatus = curHLStatus;
            //} // end of for Range w ...

            //// make sure the queue is empty
            //rq.BlockEntry();
            //while (rq.ReadNextWordLists(out items, out curItemIndex))
            //{
            //    canProceed = true;
            //    do
            //    {
            //        if (!ShowWordsLists(items, curItemIndex, b, out canProceed))
            //            return false;
            //    } while (!canProceed);
            //}

            #endregion

            #region Working Code

            bool canProceed = false;

            bool isFirst = true; // shall I read until queue is full enough as if I am reading the beginning of a paragraph
            Range curword = null, prevword = null, nextword = null;
            string realWord;
            bool curHLStatus = false;
            bool prevHLStatus = false;
            Range w = null;
            for(w = b.Range.GetFirstWord(); w != null; w = w.NextWord())
            {
                if (CancellationPending)
                    return false;

                ThisAddIn.ApplicationDoEvents();

                string tempStr = w.Text; // to use in the following conditio; in order to reduce COM calls

                if (tempStr != null && tempStr.StartsWith("\r"))
                    break;

                w.Trim();

                bool bAreRangesEqual = false;
                if (w != null && nextword != null)
                {
                    bAreRangesEqual = RangeUtils.AreRangesEqual(w, nextword); // i.e., if the currently read range equals the most recent read range; it is a duplication
                }

                if (w == null || w.Text == null || bAreRangesEqual)
                {
                    RangeUtils.SetOrInitializeRange(ref prevword, curword);
                    RangeUtils.SetOrInitializeRange(ref curword, nextword);
                    nextword = null;
                    isFirst = true;

                    continue;
                }

                realWord = StringUtil.RefineAndFilterPersianWord(w.Text);
                curHLStatus = RangeUtils.IsRangeInsideHyperlink(w);

                if (prevHLStatus != curHLStatus)
                {
                    #region I saw a paranthesis
                    RangeUtils.SetOrInitializeRange(ref prevword, curword);
                    RangeUtils.SetOrInitializeRange(ref curword, nextword);
                    nextword = null;

                    if (curword != null && curword.Text != null)
                    {
                        canProceed = true;
                        do
                        {
                            if (!ProcessForSpellChecker(ref prevword, ref curword, ref nextword, b, out canProceed))
                                return false;
                        } while (!canProceed);
                    }

                    #endregion

                    # region Now I saw o word
                    if (realWord.Length > 0 && StringUtil.IsInArabicWord(realWord[0]))
                    {
                        RangeUtils.SetOrInitializeRange(ref nextword, w);
                        curword = prevword = null;
                        isFirst = false;
                    }
                    else
                    {
                        isFirst = true;
                        RangeUtils.SetOrInitializeRange(ref prevword, curword);
                        RangeUtils.SetOrInitializeRange(ref curword, nextword);
                        nextword = null;
                    }
                    #endregion
                }
                else
                {
                    if (realWord.Length > 0 && StringUtil.IsInArabicWord(realWord[0]))
                    {
                        if (isFirst)
                        {
                            RangeUtils.SetOrInitializeRange(ref nextword, w);
                            curword = prevword = null;
                            isFirst = false;
                        }
                        else
                        {
                            RangeUtils.SetOrInitializeRange(ref prevword, curword);
                            RangeUtils.SetOrInitializeRange(ref curword, nextword);
                            RangeUtils.SetOrInitializeRange(ref nextword, w);

                            if (curword != null && curword.Text != null 
                                // && (isPrevWordInHL == isCurrentWordInHL && isCurrentWordInHL == isNextWordInHL)
                                )
                            {
                                canProceed = true;
                                do
                                {
                                    if (!ProcessForSpellChecker(ref prevword, ref curword, ref nextword, b, out canProceed))
                                        return false;
                                } while (!canProceed);
                            }
                        }
                    }
                    else
                    {
                        isFirst = true;
                        RangeUtils.SetOrInitializeRange(ref prevword, curword);
                        RangeUtils.SetOrInitializeRange(ref curword, nextword);
                        nextword = null;

                        if (curword != null && curword.Text != null 
                            // && (isPrevWordInHL == isCurrentWordInHL && isCurrentWordInHL == isNextWordInHL)
                            )
                        {
                            canProceed = true;
                            do
                            {
                                if (!ProcessForSpellChecker(ref prevword, ref curword, ref nextword, b, out canProceed))
                                    return false;
                            } while (!canProceed);
                        }
                    }
                }

                prevHLStatus = curHLStatus;

                // TODO: make sure if it is okay
                if (nextword != null)
                {
                    w.SetRange(nextword);
                }
            }

            prevword = curword;
            curword = nextword;
            nextword = null;

            if (curword != null && curword.Text != null 
                // && (isPrevWordInHL == isCurrentWordInHL && isCurrentWordInHL == isNextWordInHL) //
                )
            {
                canProceed = true;
                do
                {
                    if (!ProcessForSpellChecker(ref prevword, ref curword, ref nextword, b, out canProceed))
                        return false;
                } while (!canProceed);
            }
            
            #endregion

            return true;
        }

        private bool ShowWordsLists(Range[] wRanges, int curIndex, MSWordBlock wholeParagraph, out bool canProceed)
        {
            if (m_grams == 1)
            {
                Range r0 = null, r1 = null, r2 = null;

                r1 = wRanges[curIndex];

                if (curIndex == 0)
                {
                    r0 = null;
                    if(wRanges.Length > 1)
                        r1 = wRanges[1];
                }
                else if (curIndex == 1)
                {
                    r0 = wRanges[0];
                    if (wRanges.Length > 2)
                        r2 = wRanges[2];
                }
                else if (curIndex == 2)
                {
                    r0 = wRanges[1];
                    r2 = null;
                }

                return ProcessForSpellChecker(ref r0, ref r1, ref r2, wholeParagraph, out canProceed);

            }
            else
            {
                // TOOD: call ProcessForSpellChecker
                canProceed = true;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < wRanges.Length; ++i)
                {
                    if (i == curIndex)
                    {
                        sb.AppendFormat(" *{0}* ", wRanges[i].Text);
                        wRanges[i].Select();
                    }
                    else
                    {
                        sb.AppendFormat("  {0}  ", wRanges[i].Text);
                    }
                }

                return DialogResult.OK == MessageBox.Show(sb.ToString(), "", MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
        }

        private bool ProcessForSpellChecker(ref Range prevword, ref Range curword, ref Range nextword, MSWordBlock wholeParagraph, out bool canProceed)
        {
            canProceed = true;

            //MessageBox.Show(String.Format("{0} ( {1} ) {2}", prevword == null || prevword.Text == null ? "ه" : prevword.Text, curword == null || curword.Text == null ? "ه" : curword.Text, nextword == null || nextword.Text == null ? "ه" : nextword.Text));
            //ThisAddIn.ApplicationDoEvents();
            string strCurWord, strPrevWord, strNextWord;
            if (curword == null || curword.Text == null) return true;
            strCurWord = StringUtil.RefineAndFilterPersianWord(curword.Text);

            if (Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.IsExistInIgnoreList(strCurWord))
                return true;

            if (prevword == null || prevword.Text == null)
            {
                strPrevWord = "";
            }
            else
            {
                strPrevWord = StringUtil.RefineAndFilterPersianWord(prevword.Text);
            }
            if (nextword == null || nextword.Text == null)
            {
                strNextWord = "";
            }
            else
            {
                strNextWord = StringUtil.RefineAndFilterPersianWord(nextword.Text);
            }

            SpaceCorrectionState scs = SpaceCorrectionState.None;
            SuggestionType st = SuggestionType.Red;
            string[] sugs = new string[0];

            try
            {
                if (engine.OnePassCorrection(strCurWord, strPrevWord, strNextWord, engine.SuggestionCount, out sugs, out st, out scs))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("OnePassCorrection", ex);
                return true;
            }

            // if there's some suggestions
            if (sugs.Length > 0)
            {
                ++m_stats;
                string selectedSug = sugs[0];
                if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationLeftSerrially)
                {
                    Globals.ThisAddIn.ReplaceTwoRangesWithOne(curword, nextword, selectedSug);
                }
                else if (scs == SpaceCorrectionState.SpaceInsertationRight || scs == SpaceCorrectionState.SpaceInsertationRightSerrially)
                {
                    Globals.ThisAddIn.ReplaceTwoRangesWithOne(prevword, curword, selectedSug);
                }
                else if (scs == SpaceCorrectionState.SpaceDeletation || scs == SpaceCorrectionState.SpaceDeletationSerrially)
                {
                    if (curword.Text != selectedSug)
                    {
                        Globals.ThisAddIn.SetRangeContent(curword, selectedSug, true);

                        prevword = curword.Words[1];

                        curword.SetRangeAndTrim(prevword.NextWord());

                        nextword = curword.NextWord();
                        nextword.Trim();

                        canProceed = false;
                    }
                }
                else
                {
                    if (curword.Text != selectedSug)
                    {
                        Globals.ThisAddIn.SetRangeContent(curword, selectedSug, true);
                    }
                }
            }

            return true;
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

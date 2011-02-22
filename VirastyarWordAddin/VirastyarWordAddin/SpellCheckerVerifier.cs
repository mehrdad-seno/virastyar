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
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.Utility.GUI;
using SCICT.Utility.SpellChecker;
using System.Collections.Generic;

namespace VirastyarWordAddin
{
    class SpellCheckerVerifier : VerifierBase
    {
        #region Private Fields

        private PersianSpellChecker engine;
        private SessionLogger sessionLogger;
        // private SpellCheckerConfig curSettings;

        #endregion

        #region Ctors

        public SpellCheckerVerifier(PersianSpellChecker engine) : base()
        {
            this.engine = engine;
            this.sessionLogger = new SessionLogger();
            //ChangeConfig(ss);
        }

        public SpellCheckerVerifier(PersianSpellChecker engine, SessionLogger sessionLogger)
            : base()
        {
            this.engine = engine;
            this.sessionLogger = sessionLogger;
            //ChangeConfig(ss);
        }

        protected override void InitVerifWin()
        {
            m_verificationWindow = new SpellCheckerVerificationWindow();
            m_verificationWindow.SetCaption("اصلاح خطاهای املایی");
            ((SpellCheckerVerificationWindow)m_verificationWindow).SetSuggestionCaption(Constants.UIMessages.Suggestions);
            m_verificationWindow.Verifier = this;
            this.m_verificationWindow.SetHelp(HelpConstants.SpellChecker);
        }

        #endregion

        private int m_grams = 1;
        /// <summary>
        /// Processes the paragraph for verification.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected override bool ProcessParagraphForVerification(MSWordBlock b)
        {
            #region The n-gram code

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

            #region Commented Out (DO NOT DELETE)
            
            if (b.Range == null || b.Range.Text == null || String.IsNullOrEmpty(StringUtil.TrimWithControlChars(b.Range.Text)))
            {
                return true;
            }


            bool canProceed = false;

            bool isFirst = true; // shall I read until queue is full enough as if I am reading the beginning of a paragraph
            Range curword = null, prevword = null, nextword = null;
            string realWord = null;
            bool curHLStatus = false;
            bool prevHLStatus = false;
            Range w = null;
            for(w = b.Range.GetFirstWord(); w != null; w = w.NextWord())
            {
                // process the event queue
                ThisAddIn.ApplicationDoEvents();

                // if user has pressed cancel button then exit
                if (CancellationPending)
                    return false;

                var tempStr = w.Text; // to use in the following condition; in order to reduce COM calls

                // \r means end of a paragraph
                if (tempStr != null && tempStr.StartsWith("\r"))
                    break;

                // trim the range if it is not empty or null
                // triming a white space range will cause the call to Range.NextWord() 
                // to skip to the second next word instead of first.
                if (!(String.IsNullOrEmpty(tempStr) || StringUtil.IsWhiteSpace(tempStr)))
                {
                    w.Trim();
                    tempStr = w.Text; // extract the trimmed range content
                }

                // we can skip words which are not proper only if occurring at the beginning of a sequence
                if (isFirst && IsNotWordProperForSpellChecker(tempStr))
                    continue;

                bool areRangesEqual = false;
                if (w != null && nextword != null) // only upon this conditin check if ranges are equal
                {
                    areRangesEqual = RangeUtils.AreRangesEqual(w, nextword); // i.e., if the currently read range equals the most recent read range; it is a duplication
                }

                // now extract refined range content
                realWord = StringUtil.RefineAndFilterPersianWord(tempStr ?? "");
                
                // if you have read an empty or duplicate ranges or not a proper word (e.g. punctuation marks) proceed the chain of ranges one step
                if (w == null || w.Text == null || areRangesEqual || IsNotWordProperForSpellChecker(realWord))
                {
                    ProceedChainOfRanges(ref curword, ref prevword, ref nextword);

                    if (!SendCurWordForSpellChecking(b, ref canProceed, ref curword, ref prevword, ref nextword))
                        return false;

                    isFirst = true;

                    continue;
                }


                // check curWordFurther; may be spell checker have corrupted curWord, 
                // so that it is not referring to a Persian word
                if (curword != null)
                {
                    string curWordText = curword.Text;
                    if (IsNotWordProperForSpellChecker(curWordText))
                    {
                        // next word will refer to w, and others will become null
                        InitChainOfRanges(ref curword, ref prevword, ref nextword, w);
                        isFirst = false;
                        continue;
                    }
                }


                curHLStatus = RangeUtils.IsRangeInsideHyperlink(w);
                if (prevHLStatus != curHLStatus)
                {
                    // as if I saw a paranthesis
                    ProceedChainOfRanges(ref curword, ref prevword, ref nextword);

                    if (!SendCurWordForSpellChecking(b, ref canProceed, ref curword, ref prevword, ref nextword))
                        return false;

                    // as if Now I saw a word
                    if (realWord.Length > 0 && StringUtil.IsArabicWord(realWord))
                    {
                        InitChainOfRanges(ref curword, ref prevword, ref nextword, w);
                        isFirst = false;
                    }
                    else
                    {
                        isFirst = true;
                        ProceedChainOfRanges(ref curword, ref prevword, ref nextword);
                    }
                }
                else
                {
                    // if the word is proper for spell checking
                    if (!IsNotWordProperForSpellChecker(realWord))
                    {
                        if (isFirst)
                        {
                            InitChainOfRanges(ref curword, ref prevword, ref nextword, w);
                            isFirst = false;
                        }
                        else
                        {
                            ProceedChainOfRangesSettingNext(ref curword, ref prevword, ref nextword, w);

                            if (!SendCurWordForSpellChecking(b, ref canProceed, ref curword, ref prevword, ref nextword))
                                return false;

                        }
                    }
                    else // if the word is not proper for spell checking
                    {
                        isFirst = true;
                        ProceedChainOfRanges(ref curword, ref prevword, ref nextword);

                        if(!SendCurWordForSpellChecking(b, ref canProceed, ref curword, ref prevword, ref nextword))
                            return false;

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

            if (!SendCurWordForSpellChecking(b, ref canProceed, ref curword, ref prevword, ref nextword))
                return false;

            
            #endregion

            return true;
        }

        private bool SendCurWordForSpellChecking(MSWordBlock b, ref bool canProceed, ref Range curword, ref Range prevword, ref Range nextword)
        {
            if (curword != null && curword.Text != null)
            {
                canProceed = true;
                do
                {
                    if (!ProcessForSpellChecker(ref prevword, ref curword, ref nextword, b, out canProceed))
                        return false;
                } while (!canProceed);
            }

            return true;
        }

        private static void ProceedChainOfRangesSettingNext(ref Range curword, ref Range prevword, ref Range nextword, Range w)
        {
            RangeUtils.SetOrInitializeRange(ref prevword, curword);
            RangeUtils.SetOrInitializeRange(ref curword, nextword);
            RangeUtils.SetOrInitializeRange(ref nextword, w);
        }

        private static void InitChainOfRanges(ref Range curword, ref Range prevword, ref Range nextword, Range w)
        {
            RangeUtils.SetOrInitializeRange(ref nextword, w);
            curword = prevword = null;
        }

        private static void ProceedChainOfRanges(ref Range curword, ref Range prevword, ref Range nextword)
        {
            RangeUtils.SetOrInitializeRange(ref prevword, curword);
            RangeUtils.SetOrInitializeRange(ref curword, nextword);
            nextword = null;
        }



        /// <summary>
        /// Determines whether the specified word is proper to be passed to spell checker.
        /// Words which are not proper are null or empty words, white space strings, string full of halfspace and finally
        /// words which wholly do not belong to the Persian or Arabic language.
        /// </summary>
        /// <param name="tempStr">The word to check.</param>
        /// <returns>
        /// <c>true</c> if the specified word is proper to be passed to spell checker; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsNotWordProperForSpellChecker(string tempStr)
        {
            return (String.IsNullOrEmpty(tempStr) || StringUtil.IsWhiteSpace(tempStr) ||
                    StringUtil.IsHalfSpace(tempStr) || !StringUtil.IsArabicWord(tempStr.Trim()));
        }

        private bool ShowWordsLists(Range[] wRanges, int curIndex, MSWordBlock wholeParagraph, out bool canProceed)
        {
            if (m_grams == 1)
            {
                Range r0 = null, r1 = null, r2 = null;

                r1 = wRanges[curIndex];

                switch (curIndex)
                {
                    case 0:
                        r0 = null;
                        if(wRanges.Length > 1)
                            r1 = wRanges[1];
                        break;
                    case 1:
                        r0 = wRanges[0];
                        if (wRanges.Length > 2)
                            r2 = wRanges[2];
                        break;
                    case 2:
                        r0 = wRanges[1];
                        r2 = null;
                        break;
                }

                return ProcessForSpellChecker(ref r0, ref r1, ref r2, wholeParagraph, out canProceed);

            }
            else
            {
                // TOOD: call ProcessForSpellChecker
                canProceed = true;
                var sb = new StringBuilder();

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

        public Dictionary<string, string> CurrentRankingDetail
        {
            get
            {
                return engine.RankingDetail;
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

            if (String.IsNullOrEmpty(strCurWord) || StringUtil.IsWhiteSpace(strCurWord) || StringUtil.IsHalfSpace(strCurWord) || !StringUtil.IsArabicWord(strCurWord.Trim()))
                return true;

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
                if (engine.CheckSpelling(strCurWord, strPrevWord, strNextWord, engine.SuggestionCount, out sugs, out st, out scs))
                {
                    return true;
                }

                //Added by Omid
                sugs = this.sessionLogger.Sort(sugs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return true;
            }

            switch (st)
            {
                case SuggestionType.Green:
                    m_verificationWindow.VerificationType = VerificationTypes.Warning;
                    break;
                case SuggestionType.Red:
                default:
                    m_verificationWindow.VerificationType = VerificationTypes.Error;
                    break;
            }

            string selectedSug;
            VerificationWindowButtons buttonPressed = VerificationWindowButtons.Ignore;
            if (scs == SpaceCorrectionState.SpaceInsertationLeft ||
                scs == SpaceCorrectionState.SpaceInsertationLeftSerrially)
            {
                if (Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.IsExistInIgnoreList(strCurWord + ' ' + strNextWord))
                    return true;

                //buttonPressed = ShowVerificationWindow((SpellCheckerVerificationWindow)m_verificationWindow, wholeParagraph.Range, curword, nextword, sugs, out selectedSug);
                buttonPressed = m_verificationWindow.ShowDialog(new SpellCheckVerificationWinArgs(wholeParagraph.Range, curword, nextword, sugs), out selectedSug);
            }
            else if (scs == SpaceCorrectionState.SpaceInsertationRight ||
                     scs == SpaceCorrectionState.SpaceInsertationRightSerrially)
            {
                if (Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.IsExistInIgnoreList(strPrevWord + ' ' + strCurWord))
                    return true;

                //buttonPressed = ShowVerificationWindow((SpellCheckerVerificationWindow)m_verificationWindow, wholeParagraph.Range, prevword, curword, sugs, out selectedSug);
                buttonPressed = m_verificationWindow.ShowDialog(new SpellCheckVerificationWinArgs(wholeParagraph.Range, prevword, curword, sugs), out selectedSug);
            }
            else
            {
                //buttonPressed = ShowVerificationWindow((SpellCheckerVerificationWindow)m_verificationWindow, wholeParagraph.Range, curword, sugs, out selectedSug);
                buttonPressed = m_verificationWindow.ShowDialog(new SpellCheckVerificationWinArgs(wholeParagraph.Range, curword, sugs), out selectedSug);
            }

            if (((SpellCheckerVerificationWindow)m_verificationWindow).IsCustomAddToDicPressed)
                canProceed = false;

            #region ButtonPressed

            switch (buttonPressed)
            {
                case VerificationWindowButtons.Change:
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
                    //Added by Omid
                    this.sessionLogger.AddUsage(selectedSug);
                    break;
                case VerificationWindowButtons.ChangeAll:
                    if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationLeftSerrially)
                    {
                        Globals.ThisAddIn.ReplaceTwoRangesWithOne(curword, nextword, selectedSug);
                        Globals.ThisAddIn.ReplaceAllTwoWordsWithOne(strCurWord, strNextWord, selectedSug);
                    }
                    else if (scs == SpaceCorrectionState.SpaceInsertationRight || scs == SpaceCorrectionState.SpaceInsertationRightSerrially)
                    {
                        Globals.ThisAddIn.ReplaceTwoRangesWithOne(prevword, curword, selectedSug);
                        Globals.ThisAddIn.ReplaceAllTwoWordsWithOne(strPrevWord, strCurWord, selectedSug);
                    }
                    else if (scs == SpaceCorrectionState.SpaceDeletation || scs == SpaceCorrectionState.SpaceDeletationSerrially)
                    {
                        if (curword.Text != selectedSug)
                        {
                            Globals.ThisAddIn.SetRangeContent(curword, selectedSug, true);
                            prevword = curword.Words[1];

                            curword.SetRangeAndTrim(prevword.NextWord());

                            if (nextword != null)
                                nextword.SetRangeAndTrim(curword.NextWord());

                            canProceed = false;
                        }
                        Globals.ThisAddIn.ReplaceAll(strCurWord, selectedSug);
                    }
                    else
                    {
                        Globals.ThisAddIn.ReplaceAll(strCurWord, selectedSug);
                    }

                    //Added by Omid
                    this.sessionLogger.AddUsage(selectedSug);
                    break;
                case VerificationWindowButtons.Ignore:
                    break;
                case VerificationWindowButtons.IgnoreAll:
                    //engine.AddToIgnoreList(strCurWord);
                    string wordToIgnore = strCurWord;
                    if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationLeftSerrially)
                    {
                        wordToIgnore = strCurWord + ' ' + strNextWord;
                    }
                    else if (scs == SpaceCorrectionState.SpaceInsertationRight || scs == SpaceCorrectionState.SpaceInsertationRightSerrially)
                    {
                        wordToIgnore = strPrevWord + ' ' + strCurWord;
                    }
                    Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.AddToIgnoreList(wordToIgnore);
                    break;
                case VerificationWindowButtons.AddToDictionary:
                    canProceed = false;
                    this.AddToDictionary(strCurWord);
                    break;
                case VerificationWindowButtons.Stop:
                default:
                    return false;
            }

            #endregion

            return true;

        }

        #region Previous AddToDictionary method kept as Backup, just in case
        //public bool AddToDictionary(string wordToAdd)
        //{
        //    string[] dicSuggestions = engine.GetSimpleFormOfWord(wordToAdd);
        //    string selectedDicSuggestion = null;
        //    bool acceptAffix = true;

        //    if (dicSuggestions.Length <= 0)
        //    {
        //        PersianMessageBox.Show("افزودن این واژه به واژه‌نامه ممکن نمی‌باشد!");
        //    }
        //    else if (dicSuggestions.Length == 1 && dicSuggestions[0] == wordToAdd)
        //    {
        //        selectedDicSuggestion = dicSuggestions[0];
        //        DialogResult dr = PersianMessageBox.Show(String.Format("آیا عبارت «{0}» می‌تواند پسوند بپذیرد؟", selectedDicSuggestion), "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        //        if (dr == DialogResult.Cancel)
        //            selectedDicSuggestion = null;
        //        else
        //            acceptAffix = (dr == DialogResult.Yes);
        //    }
        //    else
        //    {
        //        selectedDicSuggestion = ListBoxWithOptionForm.ShowListBoxForm(dicSuggestions,
        //            "کدام کلمه به واژه‌نامه افزوده شود؟ لطفاً ساده‌ترین شکل معنادار کلمه را انتخاب کنید.",
        //            "افزودن به واژه‌نامه",
        //            "عبارت انتخابی می‌تواند پسوند بپذیرد",
        //            out acceptAffix);

        //        //selectedDicSuggestion = ListBoxForm.ShowListBoxForm(dicSuggestions,
        //        //    "کدام کلمه به واژه‌نامه افزوده شود؟ لطفاً ساده‌ترین شکل معنادار کلمه را انتخاب کنید.",
        //        //    "افزودن به واژه‌نامه");
        //    }

        //    if (selectedDicSuggestion != null)
        //    {
        //        bool isBaseDic, isRealWord, dontAddCondition;
        //        isRealWord = engine.IsRealWord(selectedDicSuggestion);
        //        dontAddCondition = isRealWord/* && (isBaseDic == acceptAffix)*/;

        //        if (dontAddCondition)
        //        {
        //            PersianMessageBox.Show("عبارت درج شده، در واژه‌نامه موجود می‌باشد.");
        //        }
        //        else
        //        {
        //            //if (engine.AddToDictionary(selectedDicSuggestion, 1, SCICT.NLP.Persian.Constants.PersianPOSTag.UserPOS))
        //            if (engine.AddToDictionary(selectedDicSuggestion, 1, SCICT.NLP.Persian.Constants.PersianPOSTag.UserPOS))
        //            {
        //                PersianMessageBox.Show("کلمه با موفقیت به واژه‌نامه افزوده شد");
        //                return true;
        //            }
        //            else
        //            {
        //                PersianMessageBox.Show("این شکل از کلمه در واژه‌نامه موجود می‌باشد!");
        //            }
        //        }
        //    }

        //    return false;
        //}
        #endregion

        public bool AddToDictionary(string wordToAdd)
        {
            string[] dicSuggestions = engine.GetSimpleFormOfWord(wordToAdd);
            string selectedDicSuggestion = null;
            //bool acceptAffix = true;

            if (dicSuggestions.Length <= 0)
            {
                PersianMessageBox.Show("افزودن این واژه به واژه‌نامه ممکن نیست!");
            }
            else if (dicSuggestions.Length == 1 && dicSuggestions[0] == wordToAdd)
            {
                selectedDicSuggestion = dicSuggestions[0];
            }
            else
            {
                selectedDicSuggestion = ListBoxForm.ShowListBoxForm(dicSuggestions,
                    "کدام کلمه به واژه‌نامه افزوده شود؟ لطفاً ساده‌ترین شکل معنادار کلمه را انتخاب کنید.",
                    "افزودن به واژه‌نامه");
            }

            if (selectedDicSuggestion != null)
            {
                if (engine.AddToDictionary(selectedDicSuggestion, wordToAdd, Globals.ThisAddIn.SpellCheckerWrapper.UserDictionary))
                {
                    //PersianMessageBox.Show("کلمه با موفقیت به واژه‌نامه افزوده شد");
                    return true;
                }
                else
                {
                    PersianMessageBox.Show("این شکل از کلمه در واژه‌نامه وجود دارد!");
                }
            }

            return false;
        }


    }
}

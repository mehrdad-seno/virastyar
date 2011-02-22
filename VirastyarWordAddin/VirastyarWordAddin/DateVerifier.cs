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
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;

namespace VirastyarWordAddin
{
    class DateVerifier : VerifierBase
    {
        #region Private Fields

        private readonly PersianDateParser m_perianDataParser = new PersianDateParser();
        private readonly EnglishDateParser m_englishDateParser = new EnglishDateParser();
        private readonly NumericDateParser m_numericDateParser = new NumericDateParser();

        private readonly List<IPatternInfo> m_lstPatternInfo = new List<IPatternInfo>();

        #endregion

        #region Ctors

        public DateVerifier() : base()
        {
        }

        protected override void InitVerifWin()
        {
            m_verificationWindow = new DateVerificationWindow();
            m_verificationWindow.DisableButtionsPermanently(VerificationWindowButtons.AddToDictionary | VerificationWindowButtons.IgnoreAll | VerificationWindowButtons.ChangeAll);
            m_verificationWindow.SetContentCaption("موارد یافت شده:");
            m_verificationWindow.SetCaption("مبدل تاریخ");
            m_verificationWindow.Verifier = this;
            this.m_verificationWindow.SetHelp(HelpConstants.DateConvertor);
        }

        #endregion

        protected override bool ProcessParagraphForVerification(MSWordBlock paragraph)
        {
            int index = 0;
            int endIndex = 0;
            Range remainedParagraph = paragraph.Range;
            
            while (!this.CancellationPending)
            {
                if (remainedParagraph.Text == null)
                {
                    //if (remainedParagraph.Start != remainedParagraph.End)
                    //    throw new Exception("Development Exception");
                    return true;
                }

                RangeUtils.TrimRange(remainedParagraph);

                if (ProcessSubParagraph(remainedParagraph, ref endIndex))
                {
                    if (index == endIndex) // i.e. nothing was found but process is not cancelled
                        return true;
                    else
                    {
                        index = endIndex;
                        remainedParagraph = RangeUtils.GetSubRange(remainedParagraph, index - remainedParagraph.Start + 1);

                        if (remainedParagraph == null || remainedParagraph.Text == null)
                            return true;
                    }
                }
                else
                    return false;
            }

            return false;
        }

        private IPatternInfo PeakFirstVerification()
        {
            IPatternInfo minPi = null;

            int minIndex = Int32.MaxValue;
            foreach (IPatternInfo pi in m_lstPatternInfo)
            {
                if (pi.Index < minIndex)
                {
                    minIndex = pi.Index;
                    minPi = pi;
                }
            }

            return minPi;
        }

        private bool ProcessSubParagraph(Range originalParagraph, ref int endIndex)
        {
            string actualContent = originalParagraph.Text;
            if (actualContent == null)
                return true;

            // refined content without erab and mid-word-spaces
            string content = StringUtil.RefineAndFilterPersianWord(actualContent);
            Range rContent = RangeUtils.GetSubRange(originalParagraph, 0);

            #region find pattern infos and store it in minPi variable
            m_lstPatternInfo.Clear();

            foreach (IPatternInfo pi in m_perianDataParser.FindAndParse(content))
            {
                if(((PersianDatePatternInfo) pi).YearNumber >= 0)
                    m_lstPatternInfo.Add(pi);
            }

            foreach (IPatternInfo pi in m_englishDateParser.FindAndParse(content))
            {
                if (((EnglishDatePatternInfo)pi).YearNumber >= 0)
                    m_lstPatternInfo.Add(pi);
            }

            foreach (IPatternInfo pi in m_numericDateParser.FindAndParse(content))
            {
                m_lstPatternInfo.Add(pi);
            }

            if (m_lstPatternInfo.Count <= 0)
                return true;

            IPatternInfo minPi = PeakFirstVerification();
            #endregion

            #region HELL

            int actualStartIndex = 0;// rStartWord.Start;
            int actualEndIndex = 0;//rEndWord.End;

            bool result = RangeUtils.MatchStringWithRange(rContent, content, minPi.Index, minPi.Length,
                out actualStartIndex, out actualEndIndex);
            Debug.Assert(result == true);
            if (result != true)
            {
                throw new Exception("Unexpected Condition Met in Date Verifier!");
            }

            #endregion

            if (actualEndIndex >= 0 && actualStartIndex >= 0)
            {
                Range foundRange = RangeUtils.GetSubRange(rContent, actualStartIndex - rContent.Start, actualEndIndex - rContent.Start + 1);
                RangeUtils.TrimRange(foundRange);

                Debug.Assert(foundRange != null && foundRange.Text != null);
                if (!(foundRange != null && foundRange.Text != null))
                {
                    throw new Exception("Unexpected Condition Met in Date Verifier!");
                }

                if (foundRange != null)
                {
                    foundRange.Select();
                    int length;
                    if (!ProcessForVerificationWindow(originalParagraph, foundRange, minPi, out length))
                    {
                        length = 0;
                        return false;
                    }
                    else
                    {
                        if (length == 0)
                            //endIndex = actualEndIndex;
                            endIndex = foundRange.End;
                        else
                            endIndex = foundRange.Start + length;
                        //endIndex = foundRange.End - foundRange.Start;
                    }
                }
                else
                    throw new Exception("Development exception");
            }

            return true;
        }


        private bool ProcessForVerificationWindow(Range wholeParagraph, Range content, IPatternInfo patternInfo, out int newLength)
        {
            newLength = 0;

            Debug.Assert(!(wholeParagraph == null || wholeParagraph.Text == null ||
                content == null || content.Text == null || patternInfo == null));

            if (wholeParagraph == null || wholeParagraph.Text == null || content == null || content.Text == null || patternInfo == null)
                return true; // do not cancel remainder of the process

            string selectedSug;
            //VerificationWindowButtons buttonPressed = ShowWordVerificationWindow(wholeParagraph, content, patternInfo, out selectedSug);
            VerificationWindowButtons buttonPressed = m_verificationWindow.ShowDialog(new DateVerificationWinArgs(wholeParagraph, content, patternInfo), out selectedSug);

            switch (buttonPressed)
            {
                case VerificationWindowButtons.Change:
                    if (content.Text != selectedSug)
                    {
                        Globals.ThisAddIn.SetRangeContent(content, selectedSug, true);
                        newLength = selectedSug.Length;
                    }
                    break;
                case VerificationWindowButtons.ChangeAll:
                    //Globals.ThisAddIn.ReplaceAllCaseInsensitive(curword.Content, selectedSug);
                    break;
                case VerificationWindowButtons.Ignore:
                    break;
                case VerificationWindowButtons.IgnoreAll:
                    break;
                case VerificationWindowButtons.AddToDictionary:
                    break;
                case VerificationWindowButtons.Stop:
                default:
                    return false;
            }

            return true;
        }

        //private DateVerificationWindow vWnd;

       /* private VerificationWindowButtons ShowWordVerificationWindow(Range bParagraph, Range bContent, IPatternInfo patternInfo, out string selectedSug)
        {
            Globals.ThisAddIn.usageLogger.SetContent(bContent.Text);

            selectedSug = "";
            VerificationWindowButtons rtValue = VerificationWindowButtons.Stop;
            try
            {
                if (m_verificationWindow != null)
                {
                    if (((DateVerificationWindow)m_verificationWindow).SetContent(bParagraph, bContent, patternInfo))
                    {
                        m_verificationWindow.Alert(new DateVerificationWinArgs(bParagraph, bContent, patternInfo), out selectedSug);

                        rtValue = m_verificationWindow.ButtonPressed;
                        selectedSug = m_verificationWindow.SelectedSuggestion;
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

            Globals.ThisAddIn.usageLogger.SetSelectedSuggestions(selectedSug, -1);
            Globals.ThisAddIn.usageLogger.SetAction(rtValue.ToString());
            Globals.ThisAddIn.usageLogger.LogLastAction();

            return rtValue;
        }*/
    }
}

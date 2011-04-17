using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;

namespace VirastyarWordAddin
{
    class NumberVerifier : VerifierBase
    {
        private readonly PersianRealNumberParser m_perianRealNumberParser = new PersianRealNumberParser();
        private readonly DigitizedNumberParser m_digitizedNumberParser = new DigitizedNumberParser();

        private readonly List<IPatternInfo> m_lstPatternInfo = new List<IPatternInfo>();

        public NumberVerifier() : base()
        {
        }

        protected override void InitVerifWin()
        {
            m_verificationWindow = new SimpleVerificationWindow();

            m_verificationWindow.DisableButtionsPermanently(VerificationWindowButtons.AddToDictionary | VerificationWindowButtons.IgnoreAll | VerificationWindowButtons.ChangeAll);
            m_verificationWindow.SetCaption("تبدیل اعداد");
            m_verificationWindow.SetContentCaption("موارد یافت شده:");
            ((SimpleVerificationWindow)m_verificationWindow).SetSuggestionCaption(Constants.UIMessages.Suggestions);

            m_verificationWindow.Verifier = this;
            m_verificationWindow.SetHelp(HelpConstants.NumberConvertor);
        }

        protected override bool ProcessParagraphForVerification(MSWordBlock b)
        {
            int index = 0;
            int endIndex = 0;
            Range remainedParagraph = b.Range;
            while (!this.CancellationPending)
            {
                if (remainedParagraph.Text == null)
                {
                    if (remainedParagraph.Start != remainedParagraph.End)
                        throw new Exception("Development Exception");
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
                        // if the text is shorter or equal than the length that is going to be cut from the paragraph;
                        // then we expect that no text should remain in the remainder paragraph; but in action it does, 
                        // so hereby we prevent it.
                        if (remainedParagraph == null || remainedParagraph.Text == null || remainedParagraph.Text.Length <= index - remainedParagraph.Start)
                            return true;

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

        protected override void ResetStats()
        {
            
        }

        public override void ShowStats()
        {
            throw new NotImplementedException();
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
                else if (pi.Index == minIndex)
                {
                    if (pi.Length > minPi.Length)
                    {
                        minIndex = pi.Index;
                        minPi = pi;
                    }
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

            bool isFirst = true;

            IPatternInfo firstPi = null;
            foreach (IPatternInfo pi in m_perianRealNumberParser.FindAndParse(content))
            {
                if (isFirst)
                {
                    firstPi = pi;
                    m_lstPatternInfo.Add(pi);
                    isFirst = false;
                }
                else
                {
                    if (pi.Index == firstPi.Index && pi.Length == firstPi.Length)
                        m_lstPatternInfo.Add(pi);
                    else
                        break;
                }

            }

            isFirst = true;
            foreach (IPatternInfo pi in m_digitizedNumberParser.FindAndParse(content))
            {
                if (m_lstPatternInfo.Count > 0)
                {
                    if ((pi.Index < firstPi.Index) || (pi.Index == firstPi.Index && pi.Length > firstPi.Length))
                    {
                        m_lstPatternInfo.Clear();
                        m_lstPatternInfo.Add(pi);
                    }
                }
                else
                {
                    m_lstPatternInfo.Add(pi);
                }

                break;
            }

            if (m_lstPatternInfo.Count <= 0)
                return true;

            IPatternInfo minPi = m_lstPatternInfo[0];
            #endregion

            #region HELL

            int actualStartIndex = 0;// rStartWord.Start;
            int actualEndIndex = 0;//rEndWord.End;

            RangeUtils.MatchStringWithRange(rContent, content, minPi.Index, minPi.Length,
                out actualStartIndex, out actualEndIndex);

            #endregion

            if (actualEndIndex >= 0 && actualStartIndex >= 0)
            {
                Range foundRange = RangeUtils.GetSubRange(rContent, actualStartIndex - rContent.Start, actualEndIndex - rContent.Start + 1);
                RangeUtils.TrimRange(foundRange);

                Debug.Assert(foundRange != null && foundRange.Text != null);
                if (!(foundRange != null && foundRange.Text != null))
                {
                    throw new Exception("Unexpected condition occured in number verifier!");
                }

                if (foundRange != null && foundRange.Text != null)
                {
                    foundRange.Select();
                    int length;
                    if (!ProcessForVerificationWindow(originalParagraph, foundRange, m_lstPatternInfo, out length))
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


        private bool ProcessForVerificationWindow(Range wholeParagraph, Range content, IEnumerable< IPatternInfo> patternInfos, out int newLength)
        {
            newLength = 0;

            Debug.Assert(!(wholeParagraph == null || wholeParagraph.Text == null ||
                content == null || content.Text == null || patternInfos == null));


            if (wholeParagraph == null || wholeParagraph.Text == null || content == null || content.Text == null || patternInfos == null)
                return true; // do not cancel remainder of the process

            List<string> listSugs = new List<string>();
            foreach (IPatternInfo pi in patternInfos)
            {
                listSugs.AddRange(CreateSuggestions(pi));
            }
            string[] sugs = listSugs.ToArray();

            string selectedSug;
            //VerificationWindowButtons buttonPressed = ShowVerificationWindow((SpellCheckerVerificationWindow)m_verificationWindow, wholeParagraph, content, sugs, out selectedSug);
            NumberVerificationWinArgs args = new NumberVerificationWinArgs(wholeParagraph, content, sugs);
            VerificationWindowButtons buttonPressed = m_verificationWindow.ShowDialog(args, out selectedSug);

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

        #region Suggestionns

        private string[] CreateSuggestions(IPatternInfo minPi)
        {
            switch (minPi.PatternInfoType)
            {
                case PatternInfoTypes.PersianNumber:
                    return CreatePersianNumberSuggestions(minPi as GeneralNumberInfo);
                    //return CreatePersianNumberSuggestions(minPi as PersianNumberPatternInfo);
                case PatternInfoTypes.DigitizedNumber:
                    return CreateDigitizedNumberSuggestions(minPi as DigitizedNumberPatternInfo);
                default:
                    return new string[0];
            }
        }

        private string[] CreatePersianNumberSuggestions(GeneralNumberInfo pi)
        {
            if (pi == null)
            {
                return new string[0];
            }

            List<string> lstSug = new List<string>();

            if (pi.IsFraction)
            {
                string fracStr = pi.FractionString;
                lstSug.Add(ParsingUtils.ConvertNumber2Persian(fracStr));
                lstSug.Add(fracStr);
            }

            bool addCurrency = false;
            double value = pi.GetValue();
            if (pi.IsFraction) value = Math.Round(value, 3);
            string f20normalized = MathUtils.NormalizeForF20Format(value.ToString("F20"));
            string f20currency = MathUtils.InsertThousandSeperator(f20normalized);
            addCurrency = (f20normalized != f20currency);
            lstSug.Add(ParsingUtils.ConvertNumber2Persian(f20normalized));
            if(addCurrency)
                lstSug.Add(ParsingUtils.ConvertNumber2Persian(f20currency));
            lstSug.Add(f20normalized);
            if (addCurrency)
                lstSug.Add(f20currency);

            string strWritten;
            if (NumberToPersianString.TryConvertNumberToPersianString(value, out strWritten))
                lstSug.Add(strWritten);

            return lstSug.ToArray();
        }

        private string[] CreatePersianNumberSuggestions(PersianNumberPatternInfo pi)
        {
            if (pi == null)
            {
                return new string[0];
            }

            List<string> lstSug = new List<string>();
            lstSug.Add(ParsingUtils.ConvertNumber2Persian(pi.Number.ToString()));
            lstSug.Add(pi.Number.ToString());
            lstSug.Add(NumberToPersianString.ToString(pi.Number));

            return lstSug.ToArray();
        }

        private string[] CreateDigitizedNumberSuggestions(DigitizedNumberPatternInfo pi)
        {
            if (pi == null)
            {
                return new string[0];
            }

            List<string> lstSug = new List<string>();

            string f20normalized = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
            string f20currency = MathUtils.InsertThousandSeperator(f20normalized);
            bool addCurrency = (f20normalized != f20currency);


            lstSug.Add(ParsingUtils.ConvertNumber2Persian(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20"))));
            if (addCurrency)
                lstSug.Add(ParsingUtils.ConvertNumber2Persian(f20currency));
            lstSug.Add(f20normalized);
            if (addCurrency)
                lstSug.Add(f20currency);

            string strNumber;
            if (NumberToPersianString.TryConvertNumberToPersianString(pi.Number, out strNumber))
            {
                lstSug.Add(strNumber);
            }

            return lstSug.ToArray();
        }


        #endregion

    }
}

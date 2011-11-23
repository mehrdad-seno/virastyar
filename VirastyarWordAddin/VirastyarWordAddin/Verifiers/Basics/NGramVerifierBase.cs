using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using SCICT.Utility.GUI;
using System.Diagnostics;
using SCICT.Utility;
using SCICT.NLP;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public abstract class NGramVerifierBase : VerifierBase
    {
        protected int m_numWordsBefore = 0;
        protected int m_numWordsAfter = 0;
        protected WordTokenizer m_wordTokenizer;
        private readonly RoundQueue<TokenInfo> m_roundq;

        private RangeWrapper m_curPar;
        protected string m_rawContent = "";
        protected string m_contentToVerify = "";

        private IEnumerator<StringVerificationData> m_etorNGramVerifs = null;
        protected VerificationData m_curVerData = null;

        private int m_resetIndex = 0;
        private bool m_resetNeeded = false;
        private int m_numIgnores = 0;

        private int m_rangeRepairOffset = 0;
        
        protected NGramVerifierBase(int numWordsBefore, int numWordsAfter)
        {
            m_numWordsAfter = numWordsAfter;
            m_numWordsBefore = numWordsBefore;
            m_roundq = new RoundQueue<TokenInfo>(m_numWordsBefore, m_numWordsAfter);
        }

        public override bool InitParagraph(RangeWrapper par)
        {
            // TODO: make sure that creating a copy is needed
            m_curPar = par.GetCopy();
            if (m_wordTokenizer == null)
                m_wordTokenizer = new WordTokenizer(TokenizerOptions);

            if(!RefreshContents())
                return false;

            m_etorNGramVerifs = null;
            m_roundq.Clear();
            m_rangeRepairOffset = 0;
            return true;
        }

        private bool RefreshContents()
        {
            m_curPar.Invalidate();
            m_rawContent = m_curPar.Text;
            if (String.IsNullOrEmpty(m_rawContent))
                return false;

            if (NeedRefinedStrings)
            {
                m_contentToVerify = StringUtil.RefineAndFilterPersianWord(m_rawContent);
                if (String.IsNullOrEmpty(m_rawContent))
                    return false;
            }
            else
            {
                m_contentToVerify = m_rawContent;
            }

            return true;
        }

        public override bool HasVerification()
        {
            if (m_etorNGramVerifs == null)
                m_etorNGramVerifs = ReadNGrams().GetEnumerator();

            while(m_etorNGramVerifs.MoveNext())
            {
                var strVerifData = m_etorNGramVerifs.Current;
                if(m_curVerData == null)
                    m_curVerData = new VerificationData();

                m_curVerData.SetContent(strVerifData, m_curPar, m_rawContent, NeedRefinedStrings, ref m_rangeRepairOffset);
                if(m_curVerData.IsValid)
                    return true;
            }

            return false;
        }

        protected RangeWrapper GetCorresponidngRange(int ind, int end)
        {
            RangeWrapper foundRange;

            if (NeedRefinedStrings)
            {
                int newInd = StringUtil.IndexInNotFilterAndRefinedString(m_rawContent, ind);
                int newEnd = StringUtil.IndexInNotFilterAndRefinedString(m_rawContent, end);

                foundRange = m_curPar.GetRangeWithCharIndex(newInd, newEnd);
            }
            else
            {
                foundRange = m_curPar.GetRangeWithCharIndex(ind, end);
            }

            return foundRange;
        }

        [Conditional("DEBUG")]
        private void CheckWordToRangeCorrespondence(TokenInfo wordInfo)
        {
            var range = GetCorresponidngRange(wordInfo.Index, wordInfo.EndIndex);
            string rangeText = range.Text;
            string rangeTextToShow = rangeText;

            if(NeedRefinedStrings)
            {
                rangeTextToShow = StringUtil.RefineAndFilterPersianWord(rangeText);
                int i = rangeText.Length - 1;
                for (; i >= 0; i--)
                {
                    if (!StringUtil.IsHalfSpace(rangeText[i]))
                        break;
                }

                if (i < rangeText.Length - 1)
                {
                    rangeTextToShow += rangeText.Substring(i + 1);
                }
            }

            if (rangeTextToShow != wordInfo.Value)
            {
                range.Select();
            }

            Debug.Assert(rangeTextToShow == wordInfo.Value, String.Format(
                "The word tokenizer's found word boundries do not match the corresponding range content." + Environment.NewLine + 
                "Tokenizer: *{0}*" + Environment.NewLine + "({1} - {2})" + Environment.NewLine +
                "Range: *{3}*" + Environment.NewLine + "({4} - {5})", 
                StringUtil.MakeStringVisible(wordInfo.Value), wordInfo.Index, wordInfo.EndIndex,
                StringUtil.MakeStringVisible(rangeText), range.Start, range.End));
        }

        public sealed override VerificationData GetNextVerificationData()
        {
            return m_curVerData;
        }

        private IEnumerable<StringVerificationData> ReadNGrams()
        {
            m_resetIndex = 0;
            bool parEnded = false;
            while (!parEnded)
            {
                m_resetNeeded = false;
                if (CancelationPending)
                    yield break;

                TokenInfo[] readItems;
                int mainItemIndex;
                foreach (var wordInfo in m_wordTokenizer.Tokenize(m_contentToVerify, m_resetIndex))
                {
                    if (CancelationPending)
                        yield break;

                    if (!IsProperWord(wordInfo.Value))
                    {
                        m_roundq.BlockEntry();
                        while (m_roundq.ReadNextWordLists(out readItems, out mainItemIndex))
                        {
                            if(m_numIgnores > 0)
                            {
                                m_numIgnores--;
                                continue;
                            }

                            var verData = CheckNGramWordsList(readItems, mainItemIndex);

                            if(verData != null && verData.IsValid)
                            {
                                yield return verData;
                            }

                            if (m_resetNeeded)
                                goto RESETJUMP;
                        }

                        continue;
                    }

                    m_roundq.AddItem(wordInfo);
                    if (m_roundq.ReadNextWordLists(out readItems, out mainItemIndex))
                    {
                        if (m_numIgnores > 0)
                        {
                            m_numIgnores--;
                        }
                        else
                        {

                            var verData = CheckNGramWordsList(readItems, mainItemIndex);

                            if (verData != null && verData.IsValid)
                            {
                                yield return verData;
                            }

                            if (m_resetNeeded)
                                goto RESETJUMP;
                        }
                    }
                }

                // in the end make sure the queue is empty
                m_roundq.BlockEntry();
                while (m_roundq.ReadNextWordLists(out readItems, out mainItemIndex))
                {
                    if (m_numIgnores > 0)
                    {
                        m_numIgnores--;
                        continue;
                    }

                    var verData = CheckNGramWordsList(readItems, mainItemIndex);

                    if (verData != null && verData.IsValid)
                    {
                        yield return verData;
                    }

                    if (m_resetNeeded)
                        goto RESETJUMP;
                }

                parEnded = true;

            RESETJUMP: ;
                if(m_resetNeeded)
                {
                    RefreshContents();
                    m_roundq.Clear();
                }
            }
        }

        protected void RefreshForChangeCalled(RangeWrapper rangeToChange, string replaceText)
        {
            RefreshContents();

            m_resetNeeded = true;
            m_resetIndex = m_roundq[0].Index;
            m_numIgnores = Math.Max(0, m_roundq.CurMainItemIndex - 1);
        }

        protected void RefreshForChangeAllCalled(int priorRangeLength, RangeWrapper rangeWrapper, string selectedSug)
        {
            RefreshContents();

            m_resetNeeded = true;
            m_resetIndex = priorRangeLength;

            if(priorRangeLength == 0)
                return;

            // the length is given in raw text length
            if(this.NeedRefinedStrings)
            {
                string preStr = m_rawContent.Substring(0, priorRangeLength - 1);
                string strRefined = StringUtil.RefineAndFilterPersianWord(preStr);
                m_resetIndex = strRefined.Length; // -1 + 1
            }
        }

        protected void RefreshForAddToDictionaryCalled()
        {
            m_resetNeeded = true;
            m_resetIndex = m_roundq[0].Index;
            m_numIgnores = Math.Max(0, m_roundq.CurMainItemIndex);
        }

        protected bool ShowWordsList(TokenInfo[] readItems, int mainItemIndex)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < readItems.Length; i++ )
            {
                if (mainItemIndex == i)
                {
                    sb.AppendFormat(" ({0}) ", readItems[i].Value);
                    var wRange = m_curPar.GetRange(readItems[i].Index, readItems[i].EndIndex);
                    wRange.Select();
                }
                else
                    sb.AppendFormat(" {0} ", readItems[i].Value);
            }

            Debug.WriteLine(sb.ToString());
            return DialogResult.Yes == PersianMessageBox.Show(ThisAddIn.GetWin32Window(), sb.ToString(), "df", MessageBoxButtons.YesNo);
        }

        protected abstract StringVerificationData CheckNGramWordsList(TokenInfo[] readItems, int mainItemIndex);
        protected abstract WordTokenizerOptions TokenizerOptions { get; }
        protected abstract bool IsProperWord(string word);
    }
}

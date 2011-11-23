using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    public class RangeWrapper
    {
        #region static methods

        //public readonly static char InvisibleChar = '\uE83D';

        /// <summary>
        /// Reads the paragraphs of the document starting from cursor.
        /// </summary>
        /// <param name="document">The document to read.</param>
        /// <returns></returns>
        public static IEnumerable<RangeWrapper> ReadParagraphsStartingFromCursor(Document document)
        {
            foreach (Range r in DocumentUtils.ReadParagraphsStartingFromCursor(document))
            {
                yield return new RangeWrapper(r);
            }
        }

        /// <summary>
        /// Reads the paragraphs of the document starting from the beginning.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public static IEnumerable<RangeWrapper> ReadParagraphs(Document document)
        {
            foreach (Range r in DocumentUtils.ReadParagraphs(document))
            {
                yield return new RangeWrapper(r);
            }
        }

        #endregion

        private readonly Range m_range = null;

        private int? m_start = null;
        private int? m_end = null;

        private bool m_isTextRead = false;
        private string m_text = null;
        private bool m_isInvalid = false;

        public RangeWrapper(Range range)
        {
            m_range = range;
        }

        public RangeWrapper(RangeWrapper other)
            : this(RangeUtils.GetCopy(other.Range))
        {
            
        }

        public bool HasRange
        {
            get
            {
                return m_range != null;
            }
        }

        public Range Range
        {
            get
            {
                return m_range;
            }
        }

        public void Invalidate()
        {
            m_start = null;
            m_end = null;
            m_isTextRead = false;
        }

        public int Start
        {
            get
            {
                if (!m_start.HasValue)
                {
                    m_start = m_range.Start;
                }

                return m_start.Value;
            }

            set
            {
                m_range.Start = value;
                Invalidate();
            }
        }

        public int End
        {
            get
            {
                if (!m_end.HasValue)
                {
                    m_end = m_range.End;
                }

                return m_end.Value;
            }

            set
            {
                m_range.End = value;
                Invalidate();
            }
        }

        public string Text
        {
            get
            {
                if (!m_isTextRead)
                {
                    ReadText();
                }
                return m_text;
            }
        }

        public override string ToString()
        {
            return Text;
        }

        public bool TryChangeText(string str)
        {
            try
            {
                int spCharInd;
                string rText = Text;
                int stIndex = 0;
                if (StringUtil.StringContainsAny(rText, stIndex, WordSpecialCharacters.SpecialCharsArray, out spCharInd))
                {
                    do
                    {
                        char spChar = rText[spCharInd];
                        stIndex = spCharInd + 1;
                        if (spCharInd > str.Length)
                        {
                            str += spChar;
                        }
                        else
                        {
                            str = str.Insert(spCharInd, spChar.ToString());
                        }

                    } while (StringUtil.StringContainsAny(rText, stIndex, WordSpecialCharacters.SpecialCharsArray, out spCharInd));

                    if (str == rText)
                    {
                        // no need to replace do nothing and 
                        return false;
                    }

                    return TryChangeTextCharSensitive(str);
                }
                else
                {
                    m_range.Text = str;
                    Invalidate();
                    return true;
                }

            }
            catch (Exception)
            {
                Invalidate();
                return false;
            }
        }

        public bool TryChangeTextCharSensitive(string content)
        {
            try
            {
                bool succeeded = RangeUtils.SetRangeContentCharSensitive(m_range, content);
                Invalidate();
                return succeeded;
            }
            catch (Exception)
            {
                Invalidate();
                return false;
            }
        }

        /// <summary>
        /// Replaces all instances of the given string to search with the
        /// new value provided, ignoring the character case. The original 
        /// string values are used, and they are not standardized. The search and 
        /// replacement does not respect word boundaries.
        /// </summary>
        /// <param name="oldValue">the string to search</param>
        /// <param name="newValue">the string to replace</param>
        public void ReplaceAll(string oldValue, string newValue)
        {
            string text = this.Text;
            if(text == null) return;

            int[] inds, ends;
            StringUtil.FindAll(text, oldValue, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = this.GetRangeWithCharIndex(inds[i], ends[i]);
                if(matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Replaces all instances of the given string to search with the
        /// new value provided, ignoring the character case. The original 
        /// string values are used, and they are not standardized. The search and 
        /// replacement does not respect word boundaries.
        /// </summary>
        /// <param name="oldValue">the string to search</param>
        /// <param name="newValue">the string to replace</param>
        public void ReplaceAllCaseInsensitive(string oldValue, string newValue)
        {
            string text = this.Text;
            if (text == null) return;

            int[] inds, ends;
            StringUtil.FindAllCaseInsensitive(text, oldValue, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = this.GetRangeWithCharIndex(inds[i], ends[i]);
                if (matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Replaces all instances of the given string to search with the
        /// new value provided, ignoring the character case. The original 
        /// string values are used, and they are not standardized. The search and 
        /// replacement respect word boundaries.
        /// </summary>
        /// <param name="oldValue">the string to search</param>
        /// <param name="newValue">the string to replace</param>
        public void ReplaceAllWordsCaseInsensitive(string oldValue, string newValue)
        {
            string text = this.Text;
            if (text == null) return;

            int[] inds, ends;
            StringUtil.FindAllWordsCaseInsensitive(text, oldValue, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = this.GetRangeWithCharIndex(inds[i], ends[i]);
                if (matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }


        /// <summary>
        /// Replaces all instances of the given string to search with the
        /// new value provided. The original string values are used, and they
        /// are not standardized. The search and replacement does not respect
        /// word boundaries.
        /// </summary>
        /// <param name="oldValue">the string to search</param>
        /// <param name="newValue">the string to replace</param>
        public void ReplaceAllStandardized(string oldValue, string newValue)
        {
            string text = this.Text;
            if (text == null) return;

            int[] inds, ends;
            StringUtil.FindAllStandardized(text, oldValue, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = this.GetRangeWithCharIndex(inds[i], ends[i]);
                if (matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Replaces all instances of the given string to search with the
        /// new value provided. The original string values are used, and they
        /// are not standardized. The search and replacement respect
        /// word boundaries.
        /// </summary>
        /// <param name="oldValue">the string to search</param>
        /// <param name="newValue">the string to replace</param>
        public void ReplaceAllWordsStandardized(string oldValue, string newValue)
        {
            string text = this.Text;
            if (text == null) return;

            int[] inds, ends;
            StringUtil.FindAllWordsStandardized(text, oldValue, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = this.GetRangeWithCharIndex(inds[i], ends[i]);
                if (matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }


        /// <summary>
        /// Replaces the pattern specified by the given regular expression 
        /// with the new value provided. The standardized version of string 
        /// contents are used for search and replacement and the values are 
        /// not standardized.
        /// </summary>
        /// <param name="regexp">the pattern to search</param>
        /// <param name="newValue">the value to be replaced (NOT a regexp replace pattern such as $1 or $2)</param>
        public void ReplaceAllRegexpStandardized(string regexp, string newValue)
        {
            string text = this.Text;
            if (text == null) return;

            int[] inds, ends;
            StringUtil.FindAllRegexpStandardized(text, regexp, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = this.GetRangeWithCharIndex(inds[i], ends[i]);
                if (matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }

        
        /// <summary>
        /// Replaces the pattern specified by the given regular expression 
        /// with the new value provided. The original string contents are used for
        /// search and replacement and the values are not standardized.
        /// </summary>
        /// <param name="regexp">The pattern to search</param>
        /// <param name="newValue">the value to be replaced (NOT a regexp replace pattern such as $1 or $2)</param>
        public void ReplaceAllRegexp(string regexp, string newValue)
        {
            string text = this.Text;
            if (text == null) return;

            int[] inds, ends;
            StringUtil.FindAllRegexp(text, regexp, out inds, out ends);

            var matchedRanges = new List<RangeWrapper>();
            for (int i = 0; i < inds.Length; i++)
            {
                var matchRange = GetRangeWithCharIndex(inds[i], ends[i]);
                if (matchRange != null && matchRange.IsRangeValid)
                    matchedRanges.Add(matchRange);
            }

            if (matchedRanges.Count > 0)
            {
                foreach (var matchedRange in matchedRanges)
                {
                    matchedRange.TryChangeText(newValue);
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Replaces the content of two consequtive words with thw new value provided.
        /// The values are first standardized.
        /// The search respects word boundaries.
        /// </summary>
        /// <param name="word1">the first word</param>
        /// <param name="word2">the second word</param>
        /// <param name="newValue">the new value to be replaced</param>
        public void ReplaceAllTwoWordsCombination(string word1, string word2, string newValue)
        {
            string regexp = @"\b" + word1 + @"\s+" + word2 + @"\b";
            ReplaceAllRegexpStandardized(regexp, newValue);
        }


        /// <summary>
        /// Tries to make the range to fit its visible content. This method does NOT change 
        /// the underlying range; instead it returns a new instance of <c>RangeWrapper</c>,
        /// or <c>null</c> if the range could not be trimmed.
        /// </summary>
        /// <returns></returns>
        public RangeWrapper TrimRange()
        {
            if(m_range == null)
            {
                return null;
            }

            Range trimmedRange;
            var result = RangeUtils.TryTrimRange(m_range, out trimmedRange);
            if (result == TrimRangeResult.InvalidRange)
                return null;
            else if (result == TrimRangeResult.Failure)
                return this;
            else
                return new RangeWrapper(trimmedRange);
        }

        public bool IsRangeValid
        {
            get
            {
                if(m_range == null)
                    return false;
                if (!m_isTextRead)
                    ReadText();
                return !m_isInvalid;
            }
        }

        private void ReadText()
        {
            m_text = (m_range == null) ? null : m_range.Text;

            if(m_text == null || String.IsNullOrEmpty(RangeUtils.TrimRangeText(m_text)))
                m_isInvalid = true;
            m_isTextRead = true;
        }

        public int RangeLength
        {
            get
            {
                return this.End - this.Start;
            }
        }

        public RangeWrapper FirstParagraph
        {
            get
            {
                return new RangeWrapper(m_range.Paragraphs[1].Range);
            }
        }

        public RangeWrapper FirstSentence
        {
            get
            {
                return new RangeWrapper(m_range.Sentences[1]);
            }
        }

        public RangeWrapper FirstWord
        {
            get
            {
                return new RangeWrapper(m_range.Words[1]);
            }
        }

        public RangeWrapper FirstCharacter
        {
            get
            {
                return new RangeWrapper(m_range.Characters[1]);
            }
        }

        public int PageNumber
        {
            get
            {
                try
                {
                    return (int)m_range.get_Information(WdInformation.wdActiveEndPageNumber);
                }
                catch (COMException)
                {
                    return -1;
                }
            }
        }

        public int NumberOfPagesInDocument
        {
            get
            {
                try
                {
                    return (int)m_range.get_Information(WdInformation.wdNumberOfPagesInDocument);
                }
                catch (COMException)
                {
                    return -1;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if(Object.ReferenceEquals(this, obj))
                return true;

            var other = obj as RangeWrapper;
            if (other == null)
                return false;

            return (this.Start == other.Start && this.End == other.End && this.Range.StoryType == other.Range.StoryType);
        }

        /// <summary>
        /// levels: 
        /// 0: para
        /// 1: sent
        /// 2: wrod
        /// 3: char
        /// 4+: return
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public RangeWrapper GetFirst(int level)
        {
            switch (level)
            {
                case 0:
                    return FirstParagraph;
                case 1:
                    return FirstSentence;
                case 2:
                    return FirstWord;
                case 3:
                    return FirstCharacter;
                default:
                    throw new ArgumentException("level cannot exceed 3");
            }
        }

        public RangeWrapper GetNext(WdUnits units, int count)
        {
            object moveCount = count;
            object unitsOfMovement = units;

            Range nextRange = m_range.Next(ref unitsOfMovement, ref moveCount);
            
            if(nextRange == null)
                return null;

            return new RangeWrapper(nextRange);
        }

        /// <summary>
        /// levels: 
        /// 0: para
        /// 1: sent
        /// 2: wrod
        /// 3: char
        /// 4+: return
        /// </summary>
        /// <param name="level"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RangeWrapper GetNext(int level, int count)
        {
            WdUnits unitsOfMovement = WdUnits.wdParagraph;
            switch (level)
            {
                case 0:
                    unitsOfMovement = WdUnits.wdParagraph;
                    break;
                case 1:
                    unitsOfMovement = WdUnits.wdSentence;
                    break;
                case 2:
                    unitsOfMovement = WdUnits.wdWord;
                    break;
                case 3:
                    unitsOfMovement = WdUnits.wdCharacter;
                    break;
                default:
                    throw new ArgumentException("level cannot exceed 3");
            }

            return GetNext(unitsOfMovement, count);
        }

        public RangeWrapper GetCopy()
        {
            return GetRange(this.Start, this.End);
        }

        public RangeWrapper GetRange(int start, int end)
        {
            try
            {
                Range r2 = m_range.Words[1];
                r2.SetRange(start, end);
                return new RangeWrapper(r2);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public RangeWrapper GetRangeWithOffset(int startOffset, int length)
        {
            int baseIndex = this.Start;
            return GetRange(baseIndex + startOffset, baseIndex + startOffset + length);
        }

        public RangeWrapper GetRangeWithOffset(int startOffset)
        {
            int baseIndex = this.Start;
            return GetRange(baseIndex + startOffset, this.End);
        }

        /// <summary>
        /// Gets the index of the range with 0-based char index.
        /// The indexes are inclusive
        /// </summary>
        /// <param name="startChar">0-based character index.</param>
        /// <returns></returns>
        public RangeWrapper GetRangeWithCharIndex(int startChar)
        {
            try
            {
                // the indexes in Word COM are 1-based
                int start = m_range.Characters[startChar + 1].Start;
                return GetRange(start, m_range.End);
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the index of the range with 0-based char index.
        /// The indexes are inclusive
        /// </summary>
        /// <param name="startChar">0-based start char index.</param>
        /// <param name="endChar">0-based end char index.</param>
        /// <returns></returns>
        public RangeWrapper GetRangeWithCharIndex(int startChar, int endChar)
        {
            try
            {
                int start = m_range.Characters[startChar + 1].Start;
                int end = m_range.Characters[endChar + 1].End;
                return GetRange(start, end);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IntervalOverlapKinds DetectOverlapKind(RangeWrapper second)
        {
            return IntervalOverlap.Detect(this.Start, this.End, second.Start, second.End);
        }

        public void Select()
        {
            try
            {
                m_range.Select();
            }
            catch
            { 
                // some ranges cannot be selected; e.g., those in the comments
            }
        }
    }
}

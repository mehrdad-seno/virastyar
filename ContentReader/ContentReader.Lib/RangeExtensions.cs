using Microsoft.Office.Interop.Word;
using SCICT.NLP.Utility;
using System;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// 
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Makes a range to become the same as anothr range. The other range's 
        /// storytype must be the same as the source range. otherwise the behavior of this
        /// function is unknown.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="otherRange"></param>
        public static void SetRange(this Range range, Range otherRange)
        {
            if (otherRange == null) return;
            range.SetRange(otherRange.Start, otherRange.End);
        }

        /// <summary>
        /// Sets the range and trims it.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="otherRange">The other range.</param>
        public static void SetRangeAndTrim(this Range range, Range otherRange)
        {
            range.SetRange(otherRange);
            range.Trim();
        }

        /// <summary>
        /// Gets the next word after the range. Preferably call this method if the 
        /// StoryType of the range is also Word.
        /// </summary>
        /// <param name="range">The range.</param>
        public static Range NextWord(this Range range)
        {
            object objUnit = WdUnits.wdWord;
            object objCount = 1;
            Range nextWord = range.Next(ref objUnit, ref objCount);
            return nextWord;

            //if(nextWord == null)
            //    return nextWord;

            //string wordText = nextWord.Text;

            //while (String.IsNullOrEmpty(wordText) || StringUtil.IsWhiteSpace(wordText) ||
            //    StringUtil.IsHalfSpace(wordText) || !StringUtil.IsArabicWord(wordText))
            //{
            //    nextWord = nextWord.Next(ref objUnit, ref objCount);
            //    if (nextWord == null)
            //        break;
            //    wordText = nextWord.Text;
            //}

            //if (nextWord != null)
            //    nextWord.Trim();

            //return nextWord;
        }

        /// <summary>
        /// Gets the previous word before the range. Preferably call this method if the 
        /// StoryType of the range is also Word.
        /// </summary>
        /// <param name="range">The range.</param>
        public static Range PreviousWord(this Range range)
        {
            object objUnit = WdUnits.wdWord;
            object objCount = 1;

            Range prevWord = range.Previous(ref objUnit, ref objCount);
            return prevWord;

            //if (prevWord == null)
            //    return prevWord;

            //string wordText = prevWord.Text;

            //while (String.IsNullOrEmpty(wordText) || StringUtil.IsWhiteSpace(wordText) ||
            //    StringUtil.IsHalfSpace(wordText) || !StringUtil.IsArabicWord(wordText))
            //{
            //    prevWord = prevWord.Previous(ref objUnit, ref objCount);
            //    if (prevWord == null)
            //        break;
            //    wordText = prevWord.Text;
            //}

            //return prevWord;
        }

        /// <summary>
        /// Gets the first word of the range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public static Range GetFirstWord(this Range range)
        {
            return range.Words[1];
        }

        /// <summary>
        /// Gets the range of the valid char at the given index. 
        /// The index specified here corresponds to the string-index, except that, this index is 1-based, 
        /// but a typical string index is 0-based.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="index">The 1-based index.</param>
        /// <returns></returns>
        public static Range GetValidCharAt(this Range range, int index)
        {
            int charCount = range.Characters.Count;
            int charsRead = 0;
            for (int i = 1; i <= charCount; ++i)
            {
                if (range.Characters[i] != null && range.Characters[i].Text != null)
                {
                    charsRead++;
                    if (charsRead == index)
                        return range.Characters[i];
                }
            }

            //return range.Characters[charCount];
            return null;
        }

        /// <summary>
        /// Gets the sub range - the safe way.
        /// This method character ranges instead of substrings, and that's why it's safer than
        /// GetSubRange.
        /// </summary>
        /// <param name="range">The range</param>
        /// <param name="startIndex">The 0-based start index (not the range-index).</param>
        /// <param name="endIndex">The 0-based end index (not the range-index).</param>
        /// <returns></returns>
        public static Range GetSubRange(this Range range, int startIndex, int endIndex)
        {
            return RangeUtils.GetSubRange2(range, startIndex, endIndex);
        }

        /// <summary>
        /// Gets the sub range - the safe way.
        /// This method character ranges instead of substrings, and that's why it's safer than
        /// GetSubRange.
        /// </summary>
        /// <param name="range">The range</param>
        /// <param name="startIndex">The 0-based start index (not the range-index).</param>
        /// <returns></returns>
        public static Range GetSubRange(this Range range, int startIndex)
        {
            return RangeUtils.GetSubRange2(range, startIndex);
        }

        /// <summary>
        /// Gets a copy of the range, so that modifying parameters of either of them will not alter the other.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public static Range GetCopy(this Range range)
        {
            return RangeUtils.GetCopy(range);
        }

        /// <summary>
        /// Fixes the limits of the specified range, so that it fits its content.
        /// </summary>
        /// <param name="range">The range.</param>
        public static void FixLimits(this Range range)
        {
            RangeUtils.FixRangeLimits(range);
        }

        /// <summary>
        /// Trims the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        public static void Trim(this Range range)
        {
            RangeUtils.TrimRange(range);
        }

        /// <summary>
        /// Selects Range if possible.
        /// </summary>
        /// <param name="range">The range.</param>
        public static void SelectIfPossible(this Range range)
        {
            try
            {
                range.Select();
            }
            catch
            {
                // Nothing
            }
        }


    }
}

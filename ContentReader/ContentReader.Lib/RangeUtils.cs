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
using System.Text;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Persian;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// This class provides various helper methods to work with the Range class.
    /// </summary>
    public static class RangeUtils
    {
        /// <summary>
        /// Matches the string with range.
        /// </summary>
        /// <param name="r">The range</param>
        /// <param name="str">The String.</param>
        /// <param name="subStrIndex">Index of the sub String.</param>
        /// <param name="subStrLength">Length of the sub String.</param>
        /// <param name="rangeStartIndex">Start index of the range.</param>
        /// <param name="rangeEndIndx">The range end indx.</param>
        /// <returns></returns>
        public static bool MatchStringWithRange(Range r, string str, int subStrIndex, int subStrLength, 
            out int rangeStartIndex, out int rangeEndIndx)
        {
            rangeEndIndx = rangeStartIndex = 0;

            string subString = str.Substring(subStrIndex, subStrLength);

            rangeStartIndex = GetStrIndexInRange(r, str, subStrIndex);
            rangeEndIndx = GetStrIndexInRange(r, str, subStrIndex + subStrLength - 1);

            if (rangeStartIndex < 0 || rangeEndIndx < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the range-index equivalant to strStartIndex in str In case that it is going to be 
        /// normalized with respect to its spaces and half-spaces.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="str"></param>
        /// <param name="strStartIndex"></param>
        /// <returns></returns>
        public static int GetStrIndexInRange(Range r, string str, int strStartIndex)
        {
            // NOTE: Parts of this function MUST use exactly the same algorithm as in
            //  Utility.StringUtils.NormalizeSpacesAndHalfSpacesInWord
            // Any change here must be reflected there, and 
            //  any change there must be reflected here.

            int charCount = r.Characters.Count;
            if (charCount <= 0) return r.Start;

            bool isContentMet = false;
            StringBuilder sbRefinedRangeContent = new StringBuilder();

            Range rChar0, rChar1;
            char ch0, ch1;
            char refinedChar0;
            string strRefinedChar0;
            rChar0 = r.Characters[1];
            ch0 = rChar0.Text[0];

            int curStrIndex = 0;

            for (int i = 2; i <= charCount; ch0 = ch1, rChar0 = rChar1, i++)
            {
                rChar1 = r.Characters[i];
                ch1 = rChar1.Text[0];

                strRefinedChar0 = StringUtil.RefineAndFilterPersianChar(ch0);
                if (strRefinedChar0 == null || strRefinedChar0.Length <= 0) // i.e. it was either e'rab or midspace or something filtered away
                {
                    continue;
                }
                else
                {
                    refinedChar0 = strRefinedChar0[0];
                    bool shouldBeFiltered = true;

                    if (isContentMet)
                    {
                        if (StringUtil.IsHalfSpace(refinedChar0))
                        {
                            string rangeContentSoFar = sbRefinedRangeContent.ToString();
                            if (!((rangeContentSoFar.Length > 0) && (Array.FindIndex(PersianAlphabets.NonStickerChars, charInArray => charInArray == rangeContentSoFar[sbRefinedRangeContent.Length - 1]) >= 0)))
                            {
                                if (!(StringUtil.IsWhiteSpace(ch1) || StringUtil.IsHalfSpace(ch1)))
                                    shouldBeFiltered = false;
                            }
                        }
                        else
                        {
                            shouldBeFiltered = false;
                        }
                    }
                    else
                    {
                        if (!(StringUtil.IsWhiteSpace(refinedChar0) || StringUtil.IsHalfSpace(refinedChar0)))
                        {
                            isContentMet = true;
                            shouldBeFiltered = false;
                        }
                    }

                    if (!shouldBeFiltered)
                    {
                        Debug.Assert(refinedChar0 == str[curStrIndex]);
                        sbRefinedRangeContent.Append(refinedChar0);
                        if (curStrIndex >= strStartIndex)
                            return rChar0.Start;
                        ++curStrIndex;
                    }
                }
            }

            #region char 0 is now left unprocessed right after the loop.
            strRefinedChar0 = StringUtil.RefineAndFilterPersianChar(ch0);
            if (!(strRefinedChar0 == null || strRefinedChar0.Length <= 0)) // i.e. it's either e'rab or midspace or something
            {
                refinedChar0 = strRefinedChar0[0];
                if (!(StringUtil.IsWhiteSpace(refinedChar0) || StringUtil.IsHalfSpace(refinedChar0)))
                {
                    Debug.Assert(refinedChar0 == str[curStrIndex]);
                    sbRefinedRangeContent.Append(refinedChar0);
                    if (curStrIndex >= strStartIndex)
                        return rChar0.Start;
                    ++curStrIndex;
                }
            }
            #endregion

            return -1;
        }

        /// <summary>
        /// Returns the range-index for the real end of the range, 
        /// skipping whitespace, control characters, and hidden text at the end of the range.
        /// </summary>
        public static int GetTrimmedContentEndIndexInRange(Range r)
        {
            string content = r.Text.Trim();
            if (content.Length <= 0) return r.End;

            char contentLastChar = content[content.Length - 1];

            Range rCurChar;
            char curChar;
            for (int i = r.Characters.Count; i >= 1; i--)
            {
                rCurChar = r.Characters[i];
                if (rCurChar == null || rCurChar.Text == null) continue;

                curChar = rCurChar.Text[0];
                if (Char.IsControl(curChar) || Char.IsWhiteSpace(curChar)) continue;

                if (curChar == contentLastChar)
                {
                    return rCurChar.Start + 1;
                }
            }

            return r.End;
        }

        /// <summary>
        /// Returns the range-index for the real beginning of the range, 
        /// skipping whitespace, control characters, and hidden text at the beginning of the range.
        /// </summary>
        public static int GetContentStartIndexInRange(Range r)
        {
            string content = r.Text.Trim();
            if (content.Length <= 0) return r.Start;

            char contentFirstChar = content[0];

            Range rCurChar;
            char curChar;

            int charCounts = r.Characters.Count;
            for (int i = 1; i <= charCounts; i++)
            {
                rCurChar = r.Characters[i];
                if (rCurChar == null || rCurChar.Text == null) continue;

                curChar = rCurChar.Text[0];
                if (Char.IsControl(curChar) || Char.IsWhiteSpace(curChar)) continue;

                if (curChar == contentFirstChar)
                {
                    return rCurChar.End - 1;
                }
            }

            return r.Start;
        }

        private static void FixRangeLimitsBase(Range r, bool trim)
        {
            if (RangeUtils.IsRangeEmpty(r))
                return;

            int oldStart = r.Start;
            int oldEnd = r.End;

            int newStart = oldStart;
            int newEnd = oldEnd;

            string trimmedText = "";
            if(trim)
                trimmedText = StringUtil.TrimWithControlChars(r.Text);
            else
                trimmedText = StringUtil.TrimOnlyControlChars(r.Text);

            if (trimmedText.Length <= 0)
            {
                r.SetRange(oldEnd, oldEnd); // <------------------------
                return;
            }

            string firstChar = trimmedText[0].ToString();
            string lastChar = trimmedText[trimmedText.Length - 1].ToString();

            int rangeLen = r.End - r.Start + 1;
            for (int i = 0; i < rangeLen; ++i)
            {
                r.SetRange(oldStart + i, oldStart + i + 1);
                if (r.Text == firstChar)
                {
                    r.SetRange(oldStart + i, newEnd);
                    if (r.Text.StartsWith(trimmedText))
                    {
                        newStart = oldStart + i;
                        break;
                    }
                }
            }

            for (int i = 0; i < rangeLen; ++i)
            {
                r.SetRange(oldEnd - i - 1, oldEnd - i);
                if (r.Text == lastChar)
                {
                    r.SetRange(newStart, oldEnd - i);
                    if (r.Text.EndsWith(trimmedText))
                    {
                        newEnd = oldEnd - i;
                        break;
                    }
                }
            }

            r.SetRange(newStart, newEnd);
        }

        /// <summary>
        /// Trims the range so that it fits its trimmed content.
        /// If the specified range is null or its Text property is null,
        /// it does not modify the range.
        /// Note: It modifies the range in parameter and does NOT create a copy
        /// </summary>
        /// <param name="r">The range to trim.</param>
        public static void TrimRange(Range r)
        {
            FixRangeLimitsBase(r, true);
        }

        /// <summary>
        /// Fixes the range limits, so that it fits its content.
        /// </summary>
        /// <param name="r">The range.</param>
        public static void FixRangeLimits(Range r)
        {
            FixRangeLimitsBase(r, false);
        }


        /// <summary>
        /// Trims the range so that it fits its trimmed content.
        /// If the specified range is null or its Text property is null, 
        /// it does not modify the range.
        /// Note: It modifies the range in parameter and does NOT create a copy
        /// </summary>
        [Obsolete("Use TrimRange instead", true)]
        public static void TrimRangeOld(Range r)
        {
            if (RangeUtils.IsRangeEmpty(r)) return;

            string oldContent = r.Text;
            int textLen = oldContent.Length;

            int trimBefore = 0, trimAfter = 0;

            #region First evaluate trimBefore & trimAfter values which Count # chars to be removed

            for (trimBefore = 0; trimBefore < textLen; ++trimBefore)
                if (!StringUtil.IsWhiteSpace(oldContent[trimBefore]))
                    break;

            if (trimBefore >= textLen) return; // i.e. all the content was whitespace

            for (trimAfter = 0; trimAfter < textLen; ++trimAfter)
                if (!StringUtil.IsWhiteSpace(oldContent[textLen - 1 - trimAfter]))
                    break;

            Debug.Assert(trimAfter < textLen);

            #endregion

            int oldStart = r.Start;
            int oldEnd = r.End;
            string contentTrimmed = oldContent.Substring(trimBefore, textLen - trimAfter - trimBefore);
            int rangeLen = oldEnd - oldStart;
            int trimmedLen = contentTrimmed.Length;

            if (oldContent == contentTrimmed && rangeLen == contentTrimmed.Length) return; // nothing is needed to be done

            for (int i = 0; i <= rangeLen - trimmedLen; ++i)
            {
                r.SetRange(oldStart + i, oldStart + i + trimmedLen);
                if (r.Text != null && r.Text == contentTrimmed) return;
            }

            r.SetRange(oldStart, oldEnd);

            //if (r == null || r.Text == null) return;
            //int startIndex = RangeUtils.GetContentStartIndexInRange(r);
            //int endIndex = RangeUtils.GetTrimmedContentEndIndexInRange(r);
            //r.SetRange(startIndex, endIndex);
        }

        /// <summary>
        /// Checks whether the specified range is in a hyperlink or not.
        /// </summary>
        /// <returns>True, if the range is in a hyperlink. Otherwise returns false.</returns>
        public static bool IsRangeInsideHyperlink(Range r)
        {
            Hyperlink hl;
            return IsRangeInsideHyperlink(r, out hl);
        }

        /// <summary>
        /// Checks whether the specified range is in a hyperlink or not. 
        /// If true, gives the hyperlink as an out parameter.
        /// </summary>
        /// <returns>True, if the range is in a hyperlink. Otherwise returns false.</returns>
        public static bool IsRangeInsideHyperlink(Range r, out Hyperlink hl)
        {
            hl = null;
            if (r.Hyperlinks.Count != 1)
                return false;

            object index = 1;
            hl = r.Hyperlinks.get_Item(ref index);
            Debug.Assert(hl != null);
            try
            {
                Range hlRange = hl.Range;
                return r.InRange(hlRange);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Seemingly it checks if the link inside the range is the same as the provided hyperlink!
        /// </summary>
        public static bool IsRangeInsideHyperlink(Range r, Hyperlink hyperlink)
        {
            Hyperlink hl = null;
            if (r.Hyperlinks.Count != 1)
                return false;

            object index = 1;
            hl = r.Hyperlinks.get_Item(ref index);
            Debug.Assert(hl != null);
            try
            {
                Range hlRange = hyperlink.Range;
                return r.InRange(hlRange);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the given range is inside a hyperlink and refine the range according to its hyperlink.
        /// Note: Use this method whenever you want to modify a range that is the first or last word of a hyperlink.
        /// Note: This method assumes that the given range is already trimmed.
        /// [Seemingly this method is not used anywhere]
        /// </summary>
        public static bool RefineAccordingToHL(Range r)
        {
            Hyperlink currentHl = null;
            int length = r.Text.Length;

            if (RangeUtils.IsRangeInsideHyperlink(r, out currentHl))
            {
                if (r.Start == currentHl.Range.Start) // first word of the hyperlink
                {
                    int stIndex = currentHl.Range.Start;
                    int enIndex = currentHl.Range.End;
                    int rlIndex = enIndex - (stIndex + currentHl.TextToDisplay.Length + 1);
                    RangeUtils.ShrinkRangeToSubRange(r, rlIndex, rlIndex + length);
                    return true;
                }
                else
                {
                    if (r.End == currentHl.Range.End) // last word of the hyperlink
                    {
                        RangeUtils.ShrinkRangeToSubRange(r, 0, length);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Modifies the given range to fit only the first word inside the range.
        /// [Caution] This function modifies the original range object.
        /// </summary>
        public static void ShrinkRangeToFirstWord(Range r)
        {
            if (r == null || IsRangeEmpty(r)) return;

            Range firstWord = r.GetFirstWord();
            firstWord.Trim();
            r.SetRange(firstWord);
            //string content = r.Text;
            //int len = content.Length;

            //bool isWordMet = false;
            //int i = 0;
            //for (i = 0; i < len; ++i)
            //{
            //    if (!StringUtil.IsWhiteSpace(content[i]))
            //    {
            //        isWordMet = true;
            //    }
            //    else
            //    {
            //        if (isWordMet) break;
            //    }
            //}

            //if (i >= len) // i.e. loop was not breaked
            //{
            //    return; // the range consists only of one word and is trimmed
            //}
            //else
            //{
            //    int start = r.Start;
            //    int end = r.Start + i - 1;
            //    if (start > end) start = end;
            //    r.SetRange(start, end);
            //}
        }

        /// <summary>
        /// Modifies the given range to fit only the last word inside the range.
        /// [Caution] This function modifies the original range object.
        /// </summary>
        public static void ShrinkRangeToLastWord(Range r)
        {
            if (IsRangeEmpty(r)) return;

            string content = r.Text;
            int len = content.Length;

            bool isWordMet = false;
            int i = 0;
            for (i = len-1; i >= 0; --i)
            {
                if (!StringUtil.IsWhiteSpace(content[i]))
                {
                    isWordMet = true;
                }
                else
                {
                    if (isWordMet) break;
                }
            }

            if (i < 0) // i.e. loop was not breaked
            {
                return; // the range consists only of one word and is trimmed
            }
            else
            {
                int start = r.End - (len - 1 - i);
                int end = r.End;
                if (start > end) start = end;
                r.SetRange(start, end);
            }
        }

        /// <summary>
        /// Modifies the given range boundaries to the specified boundaries.
        /// [Caution] This function modifies the original range object.
        /// </summary>
        /// <param name="r">The range to shrink.</param>
        /// <param name="startIndex">0-based index (not the range-index)</param>
        public static void ShrinkRangeToSubRange(Range r, int startIndex)
        {
            ShrinkRangeToSubRange(r, startIndex, r.End - r.Start);
        }

        /// <summary>
        /// Modifies the given range boundaries to the specified boundaries.
        /// [Caution] This function modifies the original range object.
        /// </summary>
        /// <param name="r">The range to shrink.</param>
        /// <param name="startIndex">0-based index (not the range-index)</param>
        /// <param name="endIndex">0-based index (not the range-index)</param>
        public static void ShrinkRangeToSubRange(Range r, int startIndex, int endIndex)
        {
            // convert 0-based indices to range-indices
            startIndex = r.Start + startIndex;
            endIndex = r.Start + endIndex;

            if (r.Start <= startIndex && startIndex <= r.End &&
                r.Start <= endIndex && endIndex <= r.End && 
                startIndex <= endIndex)
                r.SetRange(startIndex, endIndex);
        }

        /// <summary>
        /// Gets a copy of the range, so that modifying parameters of either of them will not alter the other.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public static Range GetCopy(Range range)
        {
            Range newRange = range.GetFirstWord();
            newRange.SetRange(range.Start, range.End);
            return newRange;
        }

        /// <summary>
        /// Returns a range from index indicated by startIndex (which is a 0-based index not a range-index), to
        /// endIndex inclusively.
        /// It returns a new Range as return value (i.e. does not modify the original range).
        /// Always check the return value for null.
        /// </summary>
        /// <param name="r">The range to return its sub-range.</param>
        /// <param name="startIndex">0-based index (not the range-index)</param>
        /// <param name="endIndex">0-based index (not the range-index)</param>
        /// <returns></returns>
        public static Range GetSubRange(Range r, int startIndex, int endIndex)
        {
            return GetSubRange2(r, startIndex, endIndex - 1);

            //if (r == null) return null;

            //// convert 0-based indices to range-indices
            //startIndex = r.Start + startIndex;
            //endIndex = r.Start + endIndex;

            //NormalizeLimits(ref startIndex, ref endIndex, r.Start, r.End);

            //Range newRange = null;
            //if (r.Words.Count > 0)
            //{
            //    newRange = r.Words[1];
            //    newRange.SetRange(startIndex, endIndex);
            //}

            //return newRange;
        }

        /// <summary>
        /// Returns a range from index indicated by startIndex (which is a 0-based index not a range-index).
        /// It returns a new Range as return value (i.e. does not modify the original range).
        /// Always check the return value for null.
        /// </summary>
        /// <param name="r">The range to return its subrange.</param>
        /// <param name="startIndex">0-based index (not the range-index)</param>
        /// <returns></returns>
        public static Range GetSubRange(Range r, int startIndex)
        {
            if (startIndex == 0)
                return GetCopy(r);
            return GetSubRange(r, startIndex, r.End - r.Start);
        }

        /// <summary>
        /// Determines whether the specified character is visible in range's text but not in range character ranges
        /// </summary>
        /// <param name="ch">The character.</param>
        private static bool IsCharInTextButNotInRange(char ch)
        {
            int n = (int)ch / 100;
            if (n  == 563 || n == 564)
                return true;
            
            //if (ch == 56388 || ch == 56390 || ch == 56372 || ch == 56389 || ch == 56417 || ch == 56391 || ch == 56407)
            //    return true;

            return false;
        }

        /// <summary>
        /// Gets the sub range - the safe way.
        /// This method character ranges instead of substrings, and that's why it's safer than
        /// GetSubRange.
        /// </summary>
        /// <param name="r">The range</param>
        /// <param name="startIndex">The 0-based start index (not the range-index).</param>
        /// <param name="endIndex">The 0-based end index (not the range-index).</param>
        /// <returns></returns>
        public static Range GetSubRange2(Range r, int startIndex, int endIndex)
        {
            if (r == null) return null;

            string text = r.Text;
            
            if (text == null)
                return r;

            int charCount = r.Characters.Count;
            NormalizeLimits(ref startIndex, ref endIndex, 0, text.Length - 1); // charCount);

            //Range startChar = r.Characters[startIndex + 1];
            //Range endChar = r.Characters[endIndex + 1];

            //Range startChar = r.GetValidCharAt(startIndex + 1);
            //Range endChar = r.GetValidCharAt(endIndex + 1);

            

            Range startChar = null;
            Range endChar = null;
            
            int charsRead = 0;
            Range rCharsi;
            for (int i = 1; i <= charCount; ++i)
            {
                rCharsi = r.Characters[i];
                if (IsCharInTextButNotInRange(text[i - 1]))
                {
                    if (endChar == null) // i.e. if endChar is not passed
                        --endIndex;
                    if (startChar == null) // i.e. if startChar is not passed
                        --startIndex;
                }

                if (rCharsi != null && rCharsi.Text != null)
                {
                    charsRead++;
                    if (charsRead == startIndex + 1)
                        startChar = rCharsi;
                    if (charsRead == endIndex + 1)
                    {
                        endChar = rCharsi;
                        break;
                    }
                }
            }

            if (startChar == null || endChar == null)
                return r.GetCopy();

            startChar.FixLimits();
            endChar.FixLimits();

            //RangeUtils.TrimRange(startChar);
            //RangeUtils.TrimRange(endChar);

            Range newRange = r.Words[1];
            newRange.SetRange(startChar.Start, endChar.End);
            return newRange;
        }

        /// <summary>
        /// Gets the sub range - the safe way.
        /// This method character ranges instead of substrings, and that's why it's safer than
        /// GetSubRange.
        /// </summary>
        /// <param name="r">The range</param>
        /// <param name="startIndex">The 0-based start index (not the range-index).</param>
        /// <returns></returns>
        public static Range GetSubRange2(Range r, int startIndex)
        {
            if (startIndex == 0)
                return GetCopy(r);

            int charCount = r.Characters.Count;
            return GetSubRange2(r, startIndex, charCount - 1);
        }

        /// <summary>
        /// Gets the index in range which corresponds to the 
        /// index in the string (Range.Text) of the same range.
        /// Actually it returns the start index of the word in which the
        /// given index has occurred.
        /// [Seemingly this method is not used]
        /// </summary>
        public static int GetIndexInRange(Range r, int textIndex)
        {
            string text = r.Text;

            // get the word Count (i.e. 1-based index of words) in text ignoring erabs. (why ignoring erabs???)
            int wordCount = StringUtil.WordCountTillIndex(text, textIndex, false);

            if (wordCount <= 0)
                return r.Start;

            Range rWord = r.Words[wordCount];
            TrimRange(rWord);
            return rWord.Start;
        }

        /// <summary>
        /// Checks whether a given range cannot have any content. This will hapen when
        /// 1. the range itself is null,
        /// 2. the range.Text is null,
        /// 3. the range's start limit exceeds its end limit
        /// </summary>
        public static bool IsRangeEmpty(Range r)
        {
            // TODO: shouldn't it be  r.End < r.Start ????
            return ((r == null) || (r.Text == null) || (r.End <= r.Start));
        }

        /// <summary>
        /// Checks if two ranges are exactly equal, i.e. if they're boundaries are the same,
        /// and they both belog to a same story type.
        /// </summary>
        public static bool AreRangesEqual(Range r1, Range r2)
        {
            if (r1.StoryType == r2.StoryType)
                if (r1.Start == r2.Start && r1.End == r2.End)
                    return true;

            return false;
        }

        /// <summary>
        /// Gets the word containing the selection area. If the selection area contains more
        /// than one word, it returns the first word.
        /// </summary>
        public static Range GetWordOfSelection(Selection sel)
        {
            Debug.Assert(sel != null);

            if (sel.Words.Count <= 0)
                return sel.Range;

            //if (sel.Type == WdSelectionType.wdNoSelection)
            if (sel.Text.Length == 1)
            {
                return sel.Range.Words[1];
            }
            else
            {
                return sel.Range;
            }
        }

        /// <summary>
        /// Returns the range object for the word right before the cursor (if any).
        /// e.g. Assume that pipe (|) indicates the cursor, the return value for:
        /// "Unive|rsity" is "Unive", and for "|University" is empty.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns></returns>
        public static Range GetWordBeforeCursor(Selection selection)
        {
            Range selRange = selection.Range;
            int newWordEnd = selRange.Start;
            int newWordStart = newWordEnd;
            Range rNewWord = selRange.Paragraphs[1].Range;
            string strNewWord = null;

            rNewWord.SetRange(newWordStart - 1, newWordStart);
            strNewWord = rNewWord.Text;
            while (strNewWord != null && rNewWord.Text.Trim().Length > 0)
            {
                newWordStart--;
                rNewWord.SetRange(newWordStart - 1, newWordStart);
                strNewWord = rNewWord.Text;
            }

            rNewWord.SetRange(newWordStart, newWordEnd);
            return rNewWord;
        }

        /// <summary>
        /// Sets the source range to become equal to the destination range.
        /// If the source range is null, then it is assinged a non-null range.
        /// </summary>
        /// <param name="src">The source range.</param>
        /// <param name="dst">The destination range.</param>
        public static void SetOrInitializeRange(ref Range src, Range dst)
        {
            if (dst == null)
            {
                src = null;
                return;
            }

            if (src == null)
                src = dst.GetFirstWord();

            src.SetRange(dst);
        }


        /// <summary>
        /// If required, changes the values of startLimit and endLimit so that
        /// startLimit is smaller than endLimit and their values do not exceed
        /// the Range's limits which are provided by the rangeStartLimit and
        /// rangeEndLimit.
        /// </summary>
        /// <param name="startLimit">the start limit to be changed</param>
        /// <param name="endLimit">the end limit to be changed</param>
        /// <param name="rangeStartLimit">the start of the range to be checked upon</param>
        /// <param name="rangeEndLimit">the end of the range to be checked upon</param>
        private static void NormalizeLimits(ref int startLimit, ref int endLimit, int rangeStartLimit, int rangeEndLimit)
        {
            if (startLimit < rangeStartLimit) startLimit = rangeStartLimit;
            if (startLimit > rangeEndLimit) startLimit = rangeEndLimit;
            if (endLimit < rangeStartLimit) endLimit = rangeStartLimit;
            if (endLimit > rangeEndLimit) endLimit = rangeEndLimit;

            if (startLimit > endLimit)
            {
                int temp = startLimit;
                startLimit = endLimit;
                endLimit = temp;
            }
        }

        /// <summary>
        /// Tries to select the specified range.
        /// This method is provided, because there are some kind of ranges that
        /// are not selectable. e.g. ranges happening in comments.
        /// </summary>
        /// <param name="r">The range to select.</param>
        public static void TrySelect(Range r)
        {
            try
            {
                r.Select();
            }
            catch
            {
            }
        }


        #region Obsolete
        /// <summary>
        /// Trims the beginning of the range.
        /// </summary>
        /// <param name="r">The range to trim</param>
        [Obsolete("Use TrimRange()", true)]
        public static void TrimStartRange(Range r)
        {
            if (IsRangeEmpty(r)) return;

            string content = r.Text;
            int len = content.Length;

            int i = 0;
            for (; i < len; ++i)
                if (!StringUtil.IsWhiteSpace(content[i]))
                    break;

            if (i == 0)
            {
                return;
            }
            else if (i >= len) // i.e. the whole string are whitespace
            {
                r.SetRange(1, 1);
            }
            else
            {
                int start = r.Start + i;
                int end = r.End;
                if (start > end) start = end;
                r.SetRange(start, end);
            }
        }

        /// <summary>
        /// Trims the end of the range.
        /// </summary>
        /// <param name="r">The range to trim.</param>
        [Obsolete("Use TrimRange()", true)]
        public static void TrimEndRange(Range r)
        {
            if (IsRangeEmpty(r)) return;

            string content = r.Text;
            int len = content.Length;

            int wsCount = 0, ctrlCount = 0;
            int i = 0;
            for (i = 0; i < len; ++i)
            {
                if (Char.IsControl(content[len - 1 - i]))
                    ctrlCount++;
                else if (Char.IsWhiteSpace(content[len - 1 - i]))
                    wsCount++;
                else
                    break;
            }

            i = wsCount + (ctrlCount / 2) + (ctrlCount % 2); // this is ceiling

            if (i == 0)
                return;
            else if (i >= len) // i.e. the whole string are whitespace
            {
                r.SetRange(1, 1);
            }
            else
            {
                int start = r.Start;
                int end = r.End - i;
                if (start > end) start = end;
                r.SetRange(start, end);
            }
        }

        #endregion
    }

    #region RangeEqualityComparer Class an IEqualityComparer<Range>

    /// <summary>
    /// An IEqualityComparer of Range to provide means to check whether two 
    /// ranges are equal if one is placed inside another. It does not check
    /// exact equality.
    /// This class is used by the content-reading methods of MSWordBlock.
    /// </summary>
    internal class RangeEqualityComparer : IEqualityComparer<Range>
    {
        #region IEqualityComparer<Range> Members

        /// <summary>
        /// Returns true if one range is inside another range, 
        /// and they both belong to a same story type.
        /// </summary>
        public bool Equals(Range x, Range y)
        {
            if (x.StoryType == y.StoryType)
            {
                if((x.Start <= y.Start && x.End >= y.End) ||
                    (x.Start >= y.Start && x.End <= y.End))
                    return true;
            }
            return false;
        }

        public int GetHashCode(Range obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.Text;
using SCICT.NLP.Persian;
using SCICT.NLP.Persian.Constants;
using System.Text.RegularExpressions;

namespace SCICT.NLP.Utility
{
    /// <summary>
    /// String Utility Class, with special focus on Persian and Arabaic characters.
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// A static reference to an instance of <see cref="PersianCharFilter"/> class.
        /// </summary>
        private static readonly PersianCharFilter s_persianCharFilter;

        /// <summary>
        /// Initializes the <see cref="StringUtil"/> class.
        /// </summary>
        static StringUtil()
        {
            s_persianCharFilter = new PersianCharFilter();
        }

        /// <summary>
        /// Replaces all matches of the given regex pattern with the specified replacement pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="regex">The regex pattern to be searched.</param>
        /// <param name="with">The string (or pattern) to be replaced.</param>
        /// <returns></returns>
        public static string ReplaceAllRegex(string str, string regex, string with)
        {
            return ReplaceAllRegex(str, regex, with, false);
        }

        /// <summary>
        /// Replaces all matches of the given regex pattern with the specified replacement pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="regex">The regex pattern to be searched.</param>
        /// <param name="with">The string (or pattern) to be replaced.</param>
        /// <param name="ignoreCase">Specifies whether the character casing should be ignored.</param>
        /// <returns></returns>
        public static string ReplaceAllRegex(string str, string regex, string with, bool ignoreCase)
        {
            RegexOptions regexOpts = RegexOptions.None;
            if (ignoreCase)
                regexOpts = RegexOptions.IgnoreCase;

            return Regex.Replace(str, regex, with, regexOpts);
        }

        /// <summary>
        /// Replaces the first instance of the found regex pattern with the specified replacement pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="regex">The regex pattern to be searched.</param>
        /// <param name="with">The string (or pattern) to be replaced.</param>
        /// <param name="ignoreCase">Specifies whether the character casing should be ignored.</param>
        /// <returns></returns>
        public static string ReplaceFirstRegex(string str, string regex, string with)
        {
            return ReplaceFirstRegex(str, regex, with, false);
        }

        /// <summary>
        /// Replaces the first instance of the found regex pattern with the specified replacement pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="regex">The regex pattern to be searched.</param>
        /// <param name="with">The string (or pattern) to be replaced.</param>
        /// <param name="ignoreCase">Specifies whether the character casing should be ignored.</param>
        /// <returns></returns>
        public static string ReplaceFirstRegex(string str, string regex, string with, bool ignoreCase)
        {
            RegexOptions regexOpts = RegexOptions.None;
            if (ignoreCase)
                regexOpts = RegexOptions.IgnoreCase;

            string result = str;
            var match = Regex.Match(str, regex, regexOpts);
            if (match != null && match.Success)
            {
                result = str.Remove(match.Index, match.Length);
                result = result.Insert(match.Index, with);
            }

            return result;
        }

        /// <summary>
        /// Replaces the last instance of the regex pattern found in the given string
        /// with the specified replacement pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="regex">The regex pattern to be searched.</param>
        /// <param name="with">The string (or pattern) to be replaced.</param>
        /// <returns></returns>
        public static string ReplaceLastRegex(string str, string regex, string with)
        {
            Regex r = new Regex(regex);
            var matches = r.Matches(str);
            if (matches.Count > 0)
            {
                int lastMatchIndex = matches[matches.Count - 1].Index;
                return r.Replace(str, with, 1, lastMatchIndex);
            }
            return str;
        }

        /// <summary>
        /// Specified whether the given string matcheses the given regex pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="pattern">The regex pattern to be searched.</param>
        /// <returns></returns>
        public static bool MatchesRegex(string str, string pattern)
        {
            return MatchesRegex(str, pattern, false);
        }

        /// <summary>
        /// Specified whether the given string matcheses the given regex pattern.
        /// </summary>
        /// <param name="str">The string to search and replace in.</param>
        /// <param name="pattern">The regex pattern to be searched.</param>
        /// <param name="ignoreCase">Specifies whether the character casing should be ignored.</param>
        /// <returns></returns>
        public static bool MatchesRegex(string str, string pattern, bool ignoreCase)
        {
            RegexOptions regexOpts = RegexOptions.None;
            if (ignoreCase)
                regexOpts = RegexOptions.IgnoreCase;

            var m = Regex.Match(str, pattern, regexOpts);
            return m != null && m.Success && m.Index == 0 && m.Length == str.Length;
        }


        /// <summary>
        /// Extracts the non arabic content. The return value includes none of the 
        /// arabic characters in the input string.
        /// </summary>
        /// <param name="word">The word</param>
        public static string ExtractNonArabicContent(string word)
        {
            StringBuilder sb = new StringBuilder(word.Length);

            for (int i = 0; i < word.Length; ++i)
            {
                if (IsInArabicWord(word[i]))
                    continue;
                sb.Append(word[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Determines whether is white space the specified character.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is white space; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWhiteSpace(char ch)
        {
            // Code 2 is a Control character but is used by MS-Word for foot-notes
            // Code 1 is a Control character but is used by MS-Word for formulas
            if ((ch == '\u0002') || (ch == '\u0001')) return false;
            else return Char.IsWhiteSpace(ch) || Char.IsControl(ch);
        }

        /// <summary>
        /// Determines whether the specified string includes only white space characters.
        /// </summary>
        /// <param name="word">The input string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string contains only white space characters; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWhiteSpace(string word)
        {
            int i;
            for (i = 0; i < word.Length; ++i)
                if (!IsWhiteSpace(word[i]))
                    break;

            return i >= word.Length;
        }

        /// <summary>
        /// Trims a string, by removing whitespace chars as well as control chars.
        /// </summary>
        /// <param name="word">The word to trim.</param>
        /// <returns></returns>
        public static string TrimWithControlChars(string word)
        {
            return TrimStartWithControlChars(TrimEndWithControlChars(word));
        }

        /// <summary>
        /// Trims the start of the string, by removing whitespace chars as well as control chars.
        /// </summary>
        /// <param name="word">The word to trim.</param>
        /// <returns></returns>
        private static string TrimStartWithControlChars(string word)
        {
            int len = word.Length;
            int i;
            for (i = 0; i < len; ++i)
            {
                if (!(IsWhiteSpace(word[i])))
                    break;
            }

            if (i < len) // i.e. breaked
                return word.Substring(i);
            else
                return word;
        }

        /// <summary>
        /// Trims end of the string, by removing whitespace chars as well as control chars.
        /// </summary>
        /// <param name="word">The word to trim.</param>
        /// <returns></returns>
        private static string TrimEndWithControlChars(string word)
        {
            int len = word.Length;
            int i;
            for (i = len - 1; i >= 0; --i)
            {
                if (!(IsWhiteSpace(word[i])))
                    break;
            }

            if (i >= 0) // i.e. breaked
                return word.Substring(0, i + 1);
            else
                return ""; // i.e. It was all made of white-spaces
        }

        /// <summary>
        /// Trims a string only considering control chars.
        /// i.e. it does not remove whitespace chars.
        /// </summary>
        /// <param name="word">The word to trim.</param>
        /// <returns></returns>
        public static string TrimOnlyControlChars(string word)
        {
            return TrimStartOnlyControlChars(TrimEndOnlyControlChars(word));
        }

        /// <summary>
        /// Trims the start of a string only considering control chars.
        /// i.e. it does not remove whitespace chars.
        /// </summary>
        /// <param name="word">The word to trim.</param>
        /// <returns></returns>
        public static string TrimStartOnlyControlChars(string word)
        {
            int len = word.Length;
            int i;
            for (i = 0; i < len; ++i)
            {
                if (!(!Char.IsWhiteSpace(word[i]) && Char.IsControl(word[i])))
                    break;
            }

            if (i < len) // i.e. breaked
                return word.Substring(i);
            else
                return word;
        }

        /// <summary>
        /// Trims the end of a string only considering control chars.
        /// i.e. it does not remove whitespace chars.
        /// </summary>
        /// <param name="word">The word to trim.</param>
        /// <returns></returns>
        public static string TrimEndOnlyControlChars(string word)
        {
            int len = word.Length;
            int i;
            for (i = len - 1; i >= 0; --i)
            {
                if (!(!Char.IsWhiteSpace(word[i]) && Char.IsControl(word[i])))
                    break;
            }

            if (i >= 0) // i.e. breaked
                return word.Substring(0, i + 1);
            else
                return ""; // i.e. It was all made of white-spaces
        }

        /// <summary>
        /// Trims the beginning of the arabic word. It trims and removes leading white-spaces,
        /// together with the half spaces.
        /// TrimStart means Trim-Left in English (i.e. Left to Right) Context.
        /// </summary>
        /// <param name="word">The input word</param>
        /// <returns>The input string that its beginning characters has been trimmed.</returns>
        public static string TrimStartArabicWord(string word)
        {
            int len = word.Length;
            int i;
            for (i = 0; i < len; ++i)
            {
                if (!(IsWhiteSpace(word[i]) || IsHalfSpace(word[i]) || IsMidWordSpace(word[i])))
                    break;
            }

            if (i < len) // i.e. breaked
                return word.Substring(i);
            else
                return word;
        }

        /// <summary>
        /// Trims the end of an Arabic word. It trims and removes trailing white-spaces,
        /// together with the half spaces.
        /// TrimEnd means trim-right in English (i.e. Left to Right) context.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string TrimEndArabicWord(string word)
        {
            int len = word.Length;
            int i;
            for (i = len - 1; i >= 0; --i)
            {
                if (!(IsWhiteSpace(word[i]) || IsHalfSpace(word[i]) || IsMidWordSpace(word[i])))
                    break;
            }

            if (i >= 0) // i.e. breaked
                return word.Substring(0, i + 1);
            else
                return ""; // i.e. It was all made of white-spaces

        }

        /// <summary>
        /// Normalizes the spaces and half spaces in word. 
        /// It trims the word, removes trailing and leading spaces and half-spaces,
        /// and replaces multiple occurrences of half-spaces with only one half-space.
        /// Also half-spaces right after Persian/Arabic separate characters are removed.
        /// For example, half spaces after "Daal" are completely removed.
        /// </summary>
        /// <param name="word">The word</param>
        /// <returns>The normalized copy of the input string.</returns>
        public static string NormalizeSpacesAndHalfSpacesInWord(string word)
        {
            // IMPORTANT NOTE: Any change in this function should be reflected also in 
            //   Utility.PersianContentReader.RangeUtils.GetRangeIndex
            // Any change here must be reflected there, and 
            //  any change there must be reflected here.

            int len = word.Length;
            if (len <= 0) return String.Empty;
            else if (len == 1) return word;

            bool isContentMet = false;
            StringBuilder sb = new StringBuilder();

            char ch0, ch1;
            ch0 = word[0];

            int i;
            for (i = 1; i < len; ch0 = ch1, ++i)
            {
                ch1 = word[i];
                if (isContentMet)
                {
                    if (IsHalfSpace(ch0))
                    {
                        string contentSoFar = sb.ToString();
                        if (!((contentSoFar.Length > 0) &&
                            (Array.FindIndex(PersianAlphabets.NonStickerChars, charInArray => charInArray == contentSoFar[contentSoFar.Length - 1]) >= 0)))
                        {
                            if (!(IsWhiteSpace(ch1) || IsHalfSpace(ch1)))
                                sb.Append(ch0);
                        }
                    }
                    else
                    {
                        sb.Append(ch0);
                    }
                }
                else // i.e. we are still in leading spaces area
                {
                    if (!(IsWhiteSpace(ch0) || IsHalfSpace(ch0)))
                    {
                        isContentMet = true;
                        sb.Append(ch0);
                    }
                }
            }

            if (!(IsWhiteSpace(ch0) || IsHalfSpace(ch0)))
                sb.Append(ch0);

            return TrimEndArabicWord(sb.ToString());
        }

        /// <summary>
        /// returns the number of words that can be counter until we reach the given index.
        /// This can be done ignoring erabs (and mid-word spaces) or not.
        /// If <code>includeErabs</code> is true, the counting occurs normally, otherwise it is 
        /// assumed that we want to count in the refined version of the input string. In this case 
        /// the input string might contain erabs and mid-word spaces, but we assume that <code>index</code>
        /// is provided from a refined version of this string, so the method ignores erabs and mid-word
        /// spaces while counting.
        /// </summary>
        /// <param name="exp">input string</param>
        /// <param name="index">if <code>includeErabs</code> is true index in the given string,
        /// otherwise index in the refined version of the string</param>
        /// <param name="includeErabs">if true counts erabs and mid-word spaces as characters, 
        /// otherwise works as if erabs and mid-word spaces do not exist, index is also
        /// passed to the function from a refined version of the string.</param>
        /// <returns></returns>
        public static int WordCountTillIndex(string exp, int index, bool includeErabs)
        {
            const int LETTER = 0, DIGIT = 1, SPACE = 3, OTHER = 4; // e.g. punctuation
            int charState = SPACE;

            int wordCount = 0;

            int len = exp.Length;
            if (index > len - 1) index = len - 1;

            int i = -1;
            foreach (char curChar in exp)
            {
                //                curChar = exp[strIndex];

                if (!includeErabs && (IsErabSign(curChar) || IsMidWordSpace(curChar)))
                {
                    continue;
                }

                if (IsWhiteSpace(curChar))
                {
                    charState = SPACE;
                }
                else if (IsInArabicWord(curChar) || Char.IsLetter(curChar))
                {
                    if (charState != LETTER)
                    {
                        wordCount++;
                    }
                    charState = LETTER;
                }
                else if (Char.IsDigit(curChar))
                {
                    if (charState != DIGIT)
                    {
                        wordCount++;
                    }
                    charState = DIGIT;
                }
                else // e.g. on punctuations
                {
                    wordCount++;
                    charState = OTHER;
                }

                i++;
                if (i >= index)
                    break;
            }

            return wordCount;
        }

        /// <summary>
        /// Returns the number of word in the expression in which or before which the index occurs
        /// Since it is a count it can be regarded as a 1-based index.
        /// </summary>
        public static int WordCountTillIndex(string exp, int index)
        {
            return WordCountTillIndex(exp, index, true);
        }

        /// <summary>
        /// returns the start index of the nth word in the expression
        /// if the expression contains less word than n then the function returns -1
        /// by word we mean characters between two whitespaces. e.g. "[123]" is one word
        /// and "[ 123]" is two words.
        /// </summary>
        /// <param name="exp">The input string</param>
        /// <param name="n">0-based index of the word</param>
        public static int WordStartIndex(string exp, int n)
        {
            if (n < 0) return -1;

            const int LETTER = 0, DIGIT = 1, SPACE = 3, OTHER = 4; // e.g. punctuation
            int charState = SPACE;

            int wordCount = -1;

            int index = 0;
            int len = exp.Length;

            for (index = 0; index < len; ++index)
            {
                char curChar = exp[index];
                if (IsWhiteSpace(curChar))
                {
                    charState = SPACE;
                }
                else if (IsInArabicWord(curChar) || Char.IsLetter(curChar))
                {
                    if (charState != LETTER)
                    {
                        wordCount++;
                    }
                    charState = LETTER;
                }
                else if (Char.IsDigit(curChar))
                {
                    if (charState != DIGIT)
                    {
                        wordCount++;
                    }
                    charState = DIGIT;
                }
                else // e.g. on punctuations
                {
                    wordCount++;
                    charState = OTHER;
                }

                if (wordCount >= n)
                    return index;
            }

            return -1;
        }

        /// <summary>
        /// returns the end index of the nth word in the expression
        /// if the expression contains less words than n then the function returns -1
        /// by word we mean characters between two whitespaces. e.g. "[123]" is one word
        /// and "[ 123]" is two words.
        /// </summary>
        /// <param name="exp">The input string</param>
        /// <param name="n">0-based index of the word</param>
        public static int WordEndIndex(string exp, int n)
        {
            if (n < 0) return -1;

            const int LETTER = 0, DIGIT = 1, SPACE = 3, OTHER = 4; // e.g. punctuation
            int charState = SPACE;

            int wordCount = -1;

            bool foundNthWord = false;

            int index = 0;
            int len = exp.Length;
            char curChar;

            for (index = 0; index < len; ++index)
            {
                curChar = exp[index];
                if (IsWhiteSpace(curChar))
                {
                    charState = SPACE;
                    if (foundNthWord)
                        return index - 1;
                }
                else if (IsInArabicWord(curChar) || Char.IsLetter(curChar))
                {
                    if (charState != LETTER)
                    {
                        if (foundNthWord)
                            return index - 1;
                        wordCount++;
                    }
                    charState = LETTER;
                }
                else if (Char.IsDigit(curChar))
                {
                    if (charState != DIGIT)
                    {
                        if (foundNthWord)
                            return index - 1;
                        wordCount++;
                    }
                    charState = DIGIT;
                }
                else // e.g. on punctuations
                {
                    if (foundNthWord)
                        return index - 1;
                    wordCount++;
                    charState = OTHER;
                }

                if (wordCount >= n)
                {
                    foundNthWord = true;
                }
            }

            if (foundNthWord)
                return index - 1;

            return -1;
        }

        /// <summary>
        /// Determines whether a character can be observed inside an Arabic word.
        /// i.e. if it is Arabic Letter or Erab or Haf Space or Mid-word Space
        /// </summary>
        public static bool IsInArabicWord(char ch)
        {
            return (IsArabicLetter(ch) || IsHalfSpace(ch) || IsMidWordSpace(ch) || IsErabSign(ch));
        }

        /// <summary>
        /// Determines whether the specified string is all consisting of arabic letters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// <c>true</c> if the specified string is all consisting of arabic letters; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArabicWord(string str)
        {
            foreach (char ch in str)
                if (!IsInArabicWord(ch))
                    return false;
            return true;
        }

        /// <summary>
        /// Removes the middle word spaces. Middle word spaces are characters that happen 
        /// in the middle of a word, but does not count as a word constructive character.
        /// e.g. مــــــن vs. من
        /// </summary>
        /// <param name="word">The input word</param>
        /// <returns>A copy of the input string with its mid-word-spaces removed.</returns>
        public static string RemoveMidWordSpace(string word)
        {
            StringBuilder sb = new StringBuilder(word.Length);

            for (int i = 0; i < word.Length; ++i)
            {
                if (IsMidWordSpace(word[i]))
                    continue;
                sb.Append(word[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the specified character can occur in the middle of a number.
        /// This does not include digits. E.g. 'e' can happen in a scientific form number, or 
        /// '.' and '/' are English and Persian/Arabic floating points respectively.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character can occur in the middle of a number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMidNumberChar(char ch)
        {
            return (ch == '+') ||
                (ch == '-') ||
                (ch == 'e') ||
                (ch == 'E') ||
                (ch == '.') ||
                (ch == '/');  // for Arabic floating point
        }

        /// <summary>
        /// Removes the erab characters from the input string except tashdid and fathatan
        /// </summary>
        /// <param name="word">The word to remove erab from</param>
        /// <returns>The copy of the input string with its erab removed</returns>
        public static string RemoveErab(string word)
        {
            StringBuilder sb = new StringBuilder(word.Length);

            for (int i = 0; i < word.Length; ++i)
            {
                if (!IsErabSignExceptFathatan(word[i]))
                    sb.Append(word[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes the erab characters from the input string including tashdid and fathatan
        /// </summary>
        /// <param name="word">The word to remove erab from</param>
        /// <returns>The copy of the input string with its erab removed</returns>
        public static string RemoveErabIncludingFathatan(string word)
        {
            StringBuilder sb = new StringBuilder(word.Length);

            for (int i = 0; i < word.Length; ++i)
            {
                if (IsErabSign(word[i]))
                    continue;
                sb.Append(word[i]);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Refines and filters Persian char. If the character is Erab or Mid-Word-Space it is removed.
        /// If it is a non standard Persian character it is replaced with its standard equivalant char(s).
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>A string containing the standard equivalant of the input character; or an empty string
        /// if the charactered is either erab or mid-word-space character.</returns>
        public static string RefineAndFilterPersianChar(char ch)
        {
            return RemoveErab(RemoveMidWordSpace(
                s_persianCharFilter.FilterChar(ch)
                ));
        }

        /// <summary>
        /// Trims and normalizes spaces and half-spaces and removes both Erab and Mid-Spaces.
        /// It does NOT apply Persian Char Filters.
        /// </summary>
        public static string RefinePersianWord(string word)
        {
            return RemoveErab(RemoveMidWordSpace(NormalizeSpacesAndHalfSpacesInWord(word)));
        }

        /// <summary>
        /// Trims and normalizes spaces and half-spaces and removes both Erab and Mid-Spaces
        /// and applies Persian Char Filters.
        /// </summary>
        public static string RefineAndFilterPersianWord(string word)
        {
            return RemoveErab(RemoveMidWordSpace(NormalizeSpacesAndHalfSpacesInWord(
                s_persianCharFilter.FilterString(word)
                )));
        }

        /// <summary>
        /// Filters the characters in the word, e.g. replaces non-standard Kaaf, and Yaa
        /// and half-spaces with the standard version.
        /// It does NOT remove erabs or mid-word-spaces.
        /// </summary>
        public static string FilterPersianWord(string word)
        {
            return s_persianCharFilter.FilterString(word);
        }

        /// <summary>
        /// Filters the persian word, ignoring some categories.
        /// </summary>
        /// <param name="word">The word</param>
        /// <param name="ignoreCats">The categories to IGNORE</param>
        public static string FilterPersianWord(string word, FilteringCharacterCategory ignoreCats)
        {
            return s_persianCharFilter.FilterString(word, ignoreCats);
        }

        /// <summary>
        /// Filters the Persian word, ignoring a set of characters.
        /// </summary>
        /// <param name="word">The word</param>
        /// <param name="ignoreList">The set of characters to be ignored.</param>
        public static string FilterPersianWord(string word, HashSet<char> ignoreList)
        {
            return s_persianCharFilter.FilterString(word, ignoreList);
        }

        /// <summary>
        /// Filters the Persian word, ignoring a set of characters, and ignoring some categories.
        /// </summary>
        /// <param name="word">The word</param>
        /// <param name="ignoreList">The set of characters to be ignored.</param>
        /// <param name="ignoreCats">The categories to IGNORE</param>
        public static string FilterPersianWord(string word, HashSet<char> ignoreList, FilteringCharacterCategory ignoreCats)
        {
            return s_persianCharFilter.FilterString(word, ignoreList, ignoreCats);
        }

        /// <summary>
        /// What would be the char index in the refined version of the string
        /// </summary>
        /// <param name="str">The not refined string; string should be trimmed beforehand.</param>
        /// <param name="index">index in the not refined string</param>
        /// <returns>corresponding index in the refined string</returns>
        public static int IndexInRefinedString(string str, int index)
        {
            int rindex = -1;
            for (int i = 0; i <= Math.Min(index, str.Length - 1); ++i)
            {
                if (IsErabSign(str[i]) || IsMidWordSpace(str[i]))
                    continue;
                rindex++;
            }
            return rindex;
        }

        /// <summary>
        /// Gets the char index in the original not-refined version of the refined string
        /// </summary>
        /// <param name="strNotRefined">The NOT refined string; string should be trimmed beforehand.</param>
        /// <param name="indexInRefined">index in the refined string</param>
        /// <returns>corresponding index in the not refined string</returns>
        public static int IndexInNotRefinedString(string strNotRefined, int indexInRefined)
        {
            int rindex = -1;
            int i;
            for (i = 0; i < strNotRefined.Length; ++i)
            {
                if (IsErabSign(strNotRefined[i]) || IsMidWordSpace(strNotRefined[i]))
                    continue;

                rindex++;
                if (rindex >= indexInRefined) break;
            }
            return i;
        }

        /// <summary>
        /// Determines whether the specified character is an Arabic letter.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an Arabic letter; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArabicLetter(char ch)
        {
            if (('\u0621' <= ch && ch <= '\u063A') ||
                ('\u0641' <= ch && ch <= '\u064A') ||
                ('\u066E' <= ch && ch <= '\u066F') ||
                ('\u0671' <= ch && ch <= '\u06D3') ||
                ('\u06FA' <= ch && ch <= '\u076D') ||
                ('\uFB50' <= ch && ch <= '\uFBFF') ||
                ('\uFDF2' <= ch && ch <= '\uFDFC') ||
                ('\uFE80' <= ch && ch <= '\uFEFC'))
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified character is a half-space character.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is a half-space character; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHalfSpace(char ch)
        {
            if ((ch == '\u200B') || (ch == '\u200C') || (ch == '\u00AC') || (ch == '\u001F') ||
                (ch == '\u200D') || (ch == '\u200E') || (ch == '\u200F'))
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether the specified string is all made up of half-space characters.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string is all made up of half-space character; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHalfSpace(string str)
        {
            foreach (char ch in str)
                if (!IsHalfSpace(ch))
                    return false;
            return true;
        }


        /// <summary>
        /// Determines whether the specified character, is mid-word-space.
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character, is mid-word-space; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMidWordSpace(char ch)
        {
            if ((ch == '\u0640') ||
                ('\uFE20' <= ch && ch <= '\uFE23') ||
                (ch == '\u200D'))
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified character is an Arabic or Persian digit.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an Arabic or Persian digit; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArabicDigit(char ch)
        {
            if (('\u0660' <= ch && ch <= '\u0669') ||
                ('\u06F0' <= ch && ch <= '\u06F9'))
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified character is Arabic punctuation.
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is Arabic punctuation; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArabicPunctuation(char ch)
        {
            // TODO: check English common punctuation signs
            if (('\u066A' <= ch && ch <= '\u066D') ||
                (ch == '\u06D4') || (ch == '\uFD3E') || (ch == '\uFD3F') || (ch == '\u061F'))
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether the specified character is an MS-Word paragraph delimiter.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an MS-Word paragraph delimiter; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsParagraphDelimiter(char ch)
        {
            if (ch == '\r' || ch == '\n')
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether the specified character is sentence delimiter.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is sentence delimiter; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSentenceDelimiter(char ch)
        {
            const string sentenceDelimiters = ".؟?!\r\n";
            foreach (char ch2 in sentenceDelimiters)
                if (ch == ch2)
                    return true;
            return false;
        }

        /// <summary>
        /// Determines whether the specified character is an erab sign.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an erab sign; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsErabSign(char ch)
        {
            if (('\u0610' <= ch && ch <= '\u0615') ||
                ('\u064B' <= ch && ch <= '\u065E') ||
                ('\u06D6' <= ch && ch <= '\u06DC') ||
                ('\u06DF' <= ch && ch <= '\u06E8') ||
                ('\u06EA' <= ch && ch <= '\u06EF') ||
                ('\uE820' <= ch && ch <= '\uE82D') ||
                ('\uFC5E' <= ch && ch <= '\uFC62') ||
                (ch == '\u0670') || (ch == '\uE818'))
                return true;
            return false;
        }

        public static bool IsErabSignExceptFathatan(char ch)
        {
            if (/* ch == StandardCharacters.StandardTashdid || */ ch == StandardCharacters.StandardFathatan)
                return false;

            return IsErabSign(ch);
        }


        /// <summary>
        /// Determines whether the specified string starts 
        /// with one of the characters in the second string.
        /// </summary>
        /// <param name="str">The string to be processed</param>
        /// <param name="chars">The string containing characters to be compared against</param>
        public static bool StringStartsWithOneOf(string str, string chars)
        {
            foreach (char ch in chars)
                if (str.StartsWith("" + ch))
                    return true;
            return false;
        }

        /// <summary>
        /// Determines whether a string is a sentence delimiter.
        /// </summary>
        /// <param name="str">The string.</param>
        public static bool StringIsASentenceDelim(string str)
        {
            return StringIsADelim(str, false);
        }

        /// <summary>
        /// Determines whether a string is a paragraph delimiter.
        /// </summary>
        /// <param name="str">The string.</param>
        public static bool StringIsAParagraphDelim(string str)
        {
            return StringIsADelim(str, true);
        }

        /// <summary>
        /// Extracts the persian sentences.
        /// Note that the sentences are neither trimmed nor normalized.
        /// </summary>
        /// <param name="text">The text to extract sentences from.</param>
        /// <returns></returns>
        public static string[] ExtractPersianSentences(string text)
        {
            return PersianSentenceTokenizer.Tokenize(text);
        }

        /// <summary>
        /// Determines whether the specified character, is word delimiter.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character, is word delimiter; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWordDelimiter(char ch)
        {
            return IsArabicDigit(ch) || IsArabicPunctuation(ch) ||
                IsMidNumberChar(ch) || IsWhiteSpace(ch) || Char.IsControl(ch) ||
                Char.IsDigit(ch) || Char.IsPunctuation(ch) || Char.IsSymbol(ch);
        }

        /// <summary>
        /// The base method that extracts the Persian words from a string of words.
        /// </summary>
        /// <param name="line">The string of words.</param>
        /// <param name="useCharFilter">if set to <c>true</c> uses Persian char 
        /// filter to refine the extracted words.</param>
        private static string[] ExtractPersianWordsBase(string line, bool useCharFilter)
        {
            List<string> listWords = new List<string>();
            StringBuilder curWord = new StringBuilder();
            string wordToBeAdded;
            foreach (char c in line)
            {
                if (IsWordDelimiter(c))
                {
                    if (curWord.ToString().Length > 0)
                    {
                        if (useCharFilter)
                            wordToBeAdded = RefineAndFilterPersianWord(curWord.ToString());
                        else
                            wordToBeAdded = RefinePersianWord(curWord.ToString());


                        listWords.Add(wordToBeAdded);
                        curWord = new StringBuilder();
                    }
                    continue;
                }

                if (IsInArabicWord(c))
                {
                    curWord.Append(c);
                }
            }

            if (curWord.ToString().Length > 0)
            {
                if (useCharFilter)
                    wordToBeAdded = RefineAndFilterPersianWord(curWord.ToString());
                else
                    wordToBeAdded = RefinePersianWord(curWord.ToString());

                listWords.Add(wordToBeAdded);
            }

            return listWords.ToArray();
        }

        /// <summary>
        /// Extracts the Persian words, without applying Persian word filters to them.
        /// </summary>
        /// <param name="line">The string of words.</param>
        public static string[] ExtractPersianWords(string line)
        {
            return ExtractPersianWordsBase(line, false);
        }

        /// <summary>
        /// Extracts the Persian words, and applies Persian word filters to them.
        /// </summary>
        /// <param name="line">The string of words.</param>
        public static string[] ExtractPersianWordsStandardized(string line)
        {
            return ExtractPersianWordsBase(line, true);
        }

        /// <summary>
        /// Determines whether the specified string is a sentence or paragraph delimiter.
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="isParagraph">if set to <c>true</c> checks for the being paragraph, 
        /// otherwise checks for being sentence.</param>
        public static bool StringIsADelim(string str, bool isParagraph)
        {
            if (str == String.Empty)
                return false;

            for (int i = str.Length - 1; i >= 0; --i)
            {
                if (isParagraph)
                {
                    if (IsParagraphDelimiter(str[i]))
                        return true;
                }
                else
                {
                    if (IsSentenceDelimiter(str[i]))
                        return true;
                }

                // if there's no chance of finding delims then break
                if (!Char.IsWhiteSpace(str[i]) && !Char.IsControl(str[i]))
                    break;
            }
            return false;
        }

        /// <summary>
        /// Gets a String by concatenating codes from parameters.
        /// </summary>
        /// <param name="charCodes">The integer char codes</param>
        public static string StringFromCodes(params int[] charCodes)
        {
            StringBuilder sb = new StringBuilder(charCodes.Length);
            foreach (int n in charCodes)
            {
                sb.Append(Convert.ToChar(n));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks whether the specified string contains any of the characters within the 
        /// specified character array.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="chars">The character array to look for.</param>
        /// <param name="index">The index of the found character or -1 if no such character is found.</param>
        /// <returns></returns>
        public static bool StringContainsAny(string str, char[] chars, out int index)
        {
            index = -1;
            int tmpIndex = -1;
            foreach (char ch in chars)
            {
                tmpIndex = str.IndexOf(ch);
                if (tmpIndex >= 0)
                {
                    index = tmpIndex;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the specified string contains any of the characters within the 
        /// specified character array.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="chars">The character array to look for.</param>
        /// <returns></returns>
        public static bool StringContainsAny(string str, char[] chars)
        {
            int dummy;
            return StringContainsAny(str, chars, out dummy);
        }

        //TODO: Do we need to "IsEnglishBlah" methods?

        /// <summary>
        /// Determines whether the specified character is an English character.
        /// </summary>
        /// <param name="ch">The character</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an English character; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnEnglishLetter(char ch)
        {
            int nCh = Convert.ToInt32(ch);

            if ((Convert.ToInt32('a') <= nCh && nCh <= Convert.ToInt32('z')) ||
                (Convert.ToInt32('A') <= nCh && nCh <= Convert.ToInt32('Z')))
                return true;
            else
                return false;
        }


        /// <summary>
        /// The characters who (may) represent a whole word in pinglish.
        /// </summary>
        /// <remarks>All characters are in lowercase.</remarks>
        public static List<char> OneLetterPinglishWords = new List<char>() {
            /*'1', */'2', /*'3', */'4', /*'5', '6', '7', '8', '9', '0', */
            'b', 'c', 'd', 'e', 'i', 'k', 'o', 'r' , 'u', 'y',
            'B', 'C', 'D', 'E', 'I', 'K', 'O', 'R' , 'U', 'Y'
        };

        /// <summary>
        /// Determines whether the specified word is a pinglish word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>
        /// 	<c>true</c> if the specified word is pinglish word; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPinglishWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            // TODO: I think this would reduce the usability of pinglish converter, 
            // as there are many irrelevant 1 character words in documents
            if (word.Length == 1)
            {
                if (OneLetterPinglishWords.Contains(word[0]))
                    return true;
                else
                    return false;
            }
            else
            {
                int result = 0;
                if (int.TryParse(word, out result))
                    return false;
            }

            foreach (char ch in word)
            {
                if (!(IsAnEnglishLetter(ch) || Char.IsDigit(ch) || IsSingleQuote(ch)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the given character represents Single Quotation marks (or similar characters like 'Prime')
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsSingleQuote(char ch)
        {
            if (ch == QuotationMark.SingleQuotationMark ||
                ch == QuotationMark.Prime ||
                ch == QuotationMark.RightSingleQuotationMark ||
                ch == QuotationMark.SingleHighReveresed9QuotationMark)
                return true;

            return false;
        }

        /// <summary>
        /// Extracts words from the specified string of words. This is a general word extraction method.
        /// To extract words from Persian sentences specificaly call 
        /// <c>ExtractPersianWords</c> and <c>ExtractPersianWordsStandardized</c>.
        /// </summary>
        /// <param name="line">string of words to extract words from</param>
        /// <returns></returns>
        public static string[] ExtractWords(string line)
        {
            List<string> listWords = new List<string>();
            StringBuilder curWord = new StringBuilder();
            string wordToBeAdded;
            foreach (char c in line)
            {
                if (IsHappeningInWord(c))
                {
                    curWord.Append(c);
                }
                else
                {
                    if (curWord.ToString().Length > 0)
                    {
                        wordToBeAdded = curWord.ToString();

                        listWords.Add(ApplyWordBuildignRules(wordToBeAdded));
                        curWord = new StringBuilder();
                    }
                    continue;
                }

            }

            if (curWord.ToString().Length > 0)
            {
                wordToBeAdded = curWord.ToString();
                listWords.Add(ApplyWordBuildignRules(wordToBeAdded));
            }

            return listWords.ToArray();
        }

        /// <summary>
        /// applies word building rules to words recognized by the ExtractWords algorithm, 
        /// e.g. it removes dashes from the beginning of the words
        /// </summary>
        /// <param name="word">the word to apply the rules to</param>
        /// <returns></returns>
        private static string ApplyWordBuildignRules(string word)
        {
            string result = word.Trim(' ', '\t', '\r', '\n', '-', QuotationMark.SingleQuotationMark, 
                QuotationMark.Prime, QuotationMark.RightSingleQuotationMark, QuotationMark.SingleHighReveresed9QuotationMark);

            return result;
        }

        /// <summary>
        /// Determines whehter the specified character is allowed to occur inside a word
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsHappeningInWord(char c)
        {
            return (Char.IsLetterOrDigit(c) || IsSingleQuote(c) || c == '-');
        }



        //To DO
         internal void StandardiseApostrophesAndStripLeading(ref string E)
        {
            E= E.Replace('’', '\'').Replace('`', '\'').Replace('"', '\'');
        }

        /*/// <summary>
        /// Trims the word, if in a sentence works on the first word,
        /// determines the type of the word and returns the number of
        /// characters read.
        /// </summary>
        public static WordType TrimAndGetTypeOfWord(string word, out string outword)
        {
            WordType type = WordType.SPACE;

            StringBuilder sb = new StringBuilder(word.Length);
            int index;
            char c;
            int length = word.Length;
            for (index = 0; index < length; ++index)
            {
                c = word[index];

                if (Char.IsWhiteSpace(c) || Char.IsControl(c))
                {
                    // do nothing
                }
                else if (StringUtil.IsInArabicWord(c))
                {
                    sb.Append(c);
                    if (type == WordType.SPACE || type == WordType.ARABIC_WORD)
                        type = WordType.ARABIC_WORD;
                    else
                        type = WordType.ILLEGAL;
                }
                else if (StringUtil.IsArabicPunctuation(c))
                {
                    // FIXME: not sure about the Arabic-punctuation part
                    if (type == WordType.ARABIC_NUM || type == WordType.ENGLISH_NUM || type == WordType.ARABIC_PUNC)
                    {
                        // do nothing
                    }
                    else if (type == WordType.SPACE)
                    {
                        type = WordType.ARABIC_PUNC;
                    }
                    else
                    {
                        type = WordType.ILLEGAL;
                    }
                    sb.Append(c);
                }
                else if (StringUtil.IsArabicDigit(c))
                {
                    if (type == WordType.ARABIC_NUM || type == WordType.ARABIC_PUNC ||
                        type == WordType.ENGLISH_PUNC || type == WordType.SPACE)
                    {
                        type = WordType.ARABIC_NUM;
                    }
                    else
                    {
                        type = WordType.ILLEGAL;
                    }
                    sb.Append(c);
                }
                else if (Char.IsPunctuation(c) || Char.IsSymbol(c))
                {
                    if (type == WordType.ARABIC_NUM || type == WordType.ENGLISH_NUM || type == WordType.ENGLISH_PUNC)
                    {
                        // do nothing
                    }
                    else if (type == WordType.SPACE)
                    {
                        type = WordType.ENGLISH_PUNC;
                    }
                    else
                    {
                        type = WordType.ILLEGAL;
                    }
                    sb.Append(c);
                }
                else if (Char.IsDigit(c))
                {
                    if (type == WordType.ENGLISH_NUM || type == WordType.ARABIC_PUNC ||
                        type == WordType.ENGLISH_PUNC || type == WordType.SPACE)
                    {
                        type = WordType.ENGLISH_NUM;
                    }
                    else
                    {
                        type = WordType.ILLEGAL;
                    }
                    sb.Append(c);
                }
                else if (Char.IsLetter(c))
                {
                    if (type == WordType.ENGLISH_WORD || type == WordType.SPACE)
                    {
                        type = WordType.ENGLISH_WORD;
                    }
                    else
                    {
                        type = WordType.ILLEGAL;
                    }
                    sb.Append(c);
                }
                else
                {
                    type = WordType.ILLEGAL;
                    sb.Append(c);
                }
            }

            outword = sb.ToString();
            return type;
        }*/
    }
}

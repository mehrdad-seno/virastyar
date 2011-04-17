using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SCICT.NLP.Utility;

namespace SCICT.NLP.Utility
{
    public class PersianWordTokenizer
    {
        private static readonly Regex s_regexEngine;

        /// <summary>
        /// this word pattern is not enough, because Erab signs are word boundaries. 
        /// It is recommended to continue in the matched vicinity to include neighboring Erab signs.
        /// \uxxxx Characters are different types of Halfspace characters.
        /// \p{M} matches Erab sign and mid-word spaces
        /// </summary>
        private static readonly string s_wordPattern = @"\b[\w\p{M}\u200B\u200C\u00AC\u001F\u200D\u200E\u200F]+\b";

        static PersianWordTokenizer()
        {
            s_regexEngine = new Regex(s_wordPattern, RegexOptions.Multiline);
        }

        public static List<string> Tokenize(string text, bool removeSeparators, bool standardized)
        {
            List<string> tokens = new List<string>();

            int strIndex = 0;
            foreach (Match match in s_regexEngine.Matches(text))
            {
                int start = FindMatchStart(match.Index, text);
                int end = FindMatchEnd(match.Index + match.Length - 1, text);

                if (!removeSeparators)
                {
                    for (int j = strIndex; j < start; j++)
                    {
                        tokens.Add(text[j].ToString());
                    }
                }

                if (standardized)
                {
                    tokens.Add(StringUtil.RefineAndFilterPersianWord( text.Substring(start, end - start + 1)) );
                }
                else
                {
                    tokens.Add(text.Substring(start, end - start + 1));
                }

                strIndex = end + 1;
            }

            // now add trailing strings if any

            if (!removeSeparators)
            {
                for (int j = strIndex; j < text.Length; j++)
                {
                    tokens.Add(text[j].ToString());
                }
            }

            return tokens;
        }

        public static List<string> Tokenize(string text, bool removeSeparators)
        {
            return Tokenize(text, removeSeparators, false);
        }


        public static List<string> Tokenize(string text)
        {
            return Tokenize(text, false, false);
        }

        /// <summary>
        /// The regex system does not match trailing erab signs so this code 
        /// is here to ensure that the matched word includes trailing characters
        /// </summary>
        /// <param name="matchEndIndex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static int FindMatchEnd(int matchEndIndex, string text)
        {
            int i;
            for (i = matchEndIndex + 1; i < text.Length; i++)
            {
                if (!StringUtil.IsInArabicWord(text[i]))
                    return i - 1;
            }

            if (i >= text.Length)
                return text.Length - 1;
            else
                return matchEndIndex;
        }

        private static int FindMatchStart(int matchStartIndex, string text)
        {
            int i;
            for (i = matchStartIndex - 1; i >= 0; i--)
            {
                if (!StringUtil.IsInArabicWord(text[i]))
                    return i + 1;
            }

            if (i < 0)
                return 0;
            else
                return matchStartIndex;
        }

    }
}

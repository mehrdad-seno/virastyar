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

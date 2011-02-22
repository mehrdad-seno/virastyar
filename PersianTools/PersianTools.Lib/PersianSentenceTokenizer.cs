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

namespace SCICT.NLP.Utility
{
    public class PersianSentenceTokenizer
    {
        protected static readonly string EOS = "\0";

        protected static readonly string PuncEos = @"[\.؟?!…]";
        protected static readonly string WhiteSpaceExceptNewLine = "[ \t\x0B\f]";
        protected static readonly string PuncEosGroup = String.Format(@"({0}+)", PuncEos);
        /// <summary>
        /// The order in the following definition is important
        /// </summary>
        protected static readonly string EndOfLine = @"\r\n|\n\r|\n|\r";
        protected static readonly string ParagraphPattern = String.Format(@"({0})", EndOfLine);

        protected static readonly string OpeningPunctuations = @"[\(\[\{«‘“]"; 
        protected static readonly string ClosingPunctuations = @"[\)\]\}»’”]"; 
        protected static readonly string SymmetricPunctuations = @"[\'\""]";

        // replace with $1$2 i.e. remove $3
        protected static readonly string InitialsPattern = String.Format(@"({0}|^)(\s*[\w\d]{{1,4}}\s*{0})({1})", PuncEos, EOS);
        
        // replace with $1 i.e. remove $2
        protected static readonly string OneLetterInitialsPattern = String.Format(@"(\b[\w\d]\s*{0}\s*)({1})", PuncEos, EOS);

        //replace with $2$1
        protected static readonly string ClosingPuncRepairPattern = String.Format(@"({0})(\s*{1}\s*)", EOS, ClosingPunctuations);

        // Symmetric Punctuations are only accepted if they are stuck to the EOS, because it might be meant opening a quotation.
        //replace with $2$1
        protected static readonly string SymmetricPuncRepairPattern = String.Format(@"({0})({1}\s*)", EOS, SymmetricPunctuations);

        // replace with $2$1$3
        // forces spaces at the beginning of sentences to be appended to the end of previous sentences
        protected static readonly string SenteceBeginWithNonSpace = String.Format(@"(.{0})({1}+)(\S|{2})", EOS, WhiteSpaceExceptNewLine, EndOfLine);

        /// <summary>
        /// Tokenizes the specified string into sentences.
        /// </summary>
        /// <param name="s">The string to extract sentences from.</param>
        /// <returns></returns>
        public static string[] Tokenize(string s)
        {
            // NOTE: the order of statements in this method is important

            // punctuations happenning at the end of sentence
            s = StringUtil.ReplaceAllRegex(s, PuncEosGroup, "$1" + EOS);

            while(StringUtil.MatchesRegex(s, InitialsPattern))
                s = StringUtil.ReplaceFirstRegex(s, InitialsPattern, "$1$2");

            while (StringUtil.MatchesRegex(s, OneLetterInitialsPattern))
                s = StringUtil.ReplaceFirstRegex(s, OneLetterInitialsPattern, "$1");


            s = StringUtil.ReplaceAllRegex(s, ClosingPuncRepairPattern, "$2$1");
            s = StringUtil.ReplaceAllRegex(s, SymmetricPuncRepairPattern, "$2$1");

            s = StringUtil.ReplaceAllRegex(s, SenteceBeginWithNonSpace, "$2$1$3");


            // remove EOS at the end of sentence
            s = StringUtil.ReplaceAllRegex(s, String.Format(@"({0})($|{1})", EOS, EndOfLine), "$2");

            // new-line means a new sentence
            s = StringUtil.ReplaceAllRegex(s, ParagraphPattern, "$1" + EOS);

            // this should be done in the end
            s = StringUtil.ReplaceAllRegex(s, String.Format("{0}+", EOS), EOS);
            return s.Split(EOS.ToCharArray());
        }
    }
}

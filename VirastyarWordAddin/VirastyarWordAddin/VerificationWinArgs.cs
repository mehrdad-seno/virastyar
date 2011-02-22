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

using Microsoft.Office.Interop.Word;
using SCICT.NLP.Utility.Parsers;

namespace VirastyarWordAddin
{
    public class BaseVerificationWinArgs
    {
        public Range bParagraph { get; protected set; }
        public Range bContent { get; protected set; }

        protected BaseVerificationWinArgs()
        {
        }

        public BaseVerificationWinArgs(Range bParagraph, Range bContent)
        {
            this.bContent = bContent;
            this.bParagraph = bParagraph;
        }
    }

    public class BaseVerificationWinArgsWithSugs : BaseVerificationWinArgs
    {
        public string[] Sugs { get; protected set; }

        protected BaseVerificationWinArgsWithSugs()
        {
        }

        public BaseVerificationWinArgsWithSugs(Range bParagraph, Range bContent, string[] sugs) 
            : base(bParagraph, bContent)
        {
            this.Sugs = sugs;
        }
    }

    public class SpellCheckVerificationWinArgs : BaseVerificationWinArgsWithSugs
    {

        public SpellCheckVerificationWinArgs(Range bParagraph, Range bContent, string[] sugs)
            : base(bParagraph, bContent, sugs)
        {
        }

        public SpellCheckVerificationWinArgs(Range bParagraph, Range word1, Range word2, string[] sugs)
        {
            this.bContent = word1;
            if (word2 != null)
            {
                object oMissing = System.Reflection.Missing.Value;
                if (word1 != null)
                {
                    Range r = word1.Next(ref oMissing, ref oMissing);
                    r.SetRange(word1.Start, word2.End);
                    this.bContent = r;
                }
                else
                {
                    this.bContent = word2;
                }
            }

            this.bParagraph = bParagraph;
            this.Sugs = sugs;
        }
    }

    public class NumberVerificationWinArgs : BaseVerificationWinArgsWithSugs
    {
        public NumberVerificationWinArgs(Range bParagraph, Range bContent, string[] sugs)
            : base(bParagraph, bContent, sugs)
        {
        }
    }

    public class PinglishVerificationWinArgs : BaseVerificationWinArgsWithSugs
    {
        public PinglishVerificationWinArgs(Range bParagraph, Range bContent, string[] sugs)
            : base(bParagraph, bContent, sugs)
        {
        }
    }

    public class DateVerificationWinArgs : BaseVerificationWinArgs
    {
        public IPatternInfo PatternInfo { get; protected set; }

        public DateVerificationWinArgs(Range bParagraph, Range bContent, IPatternInfo patternInfo)
            : base(bParagraph, bContent)
        {
            this.PatternInfo = patternInfo;
        }
    }

    public class PunctuationVerificationWinArgs : BaseVerificationWinArgsWithSugs
    {
        public PunctuationVerificationWinArgs(Range bParagraph, Range bContent, string[] sugs)
            : base(bParagraph, bContent, sugs)
        {
        }
    }
}

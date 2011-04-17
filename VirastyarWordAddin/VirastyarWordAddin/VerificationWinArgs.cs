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

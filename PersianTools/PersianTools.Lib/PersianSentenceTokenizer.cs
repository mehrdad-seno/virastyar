using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SCICT.NLP.Utility
{
    ///<summary>
    /// A sentence tokenizer for the Persian language.
    ///</summary>
    public class PersianSentenceTokenizer
    {
        protected static readonly string Eos = "\0";

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
        protected static readonly string InitialsPattern = String.Format(@"({0}|^)(\s*[\w\d]{{1,4}}\s*{0})({1})", PuncEos, Eos);
        
        // replace with $1 i.e. remove $2

        protected static readonly string OneLetterInitialsPattern = String.Format(@"(\b\w{{1}}\s*\.\s*)({0})", Eos);
        //protected static readonly string OneLetterInitialsPattern = String.Format(@"(\b[\w\d]\s*{0}\s*)({1})", PuncEos, Eos);

        //replace with $2$1
        protected static readonly string ClosingPuncRepairPattern = String.Format(@"({0})(\s*{1}\s*)", Eos, ClosingPunctuations);

        // Symmetric Punctuations are only accepted if they are stuck to the EOS, because it might be meant opening a quotation.
        //replace with $2$1
        protected static readonly string SymmetricPuncRepairPattern = String.Format(@"({0})({1}\s*)", Eos, SymmetricPunctuations);

        // replace with $2$1$3
        // forces spaces at the beginning of sentences to be appended to the end of previous sentences
        protected static readonly string SenteceBeginWithNonSpace = String.Format(@"({0})({1}+)(\S|{2})", Eos, WhiteSpaceExceptNewLine, EndOfLine);


        /// <summary>
        /// Tokenizes the specified string into sentences.
        /// </summary>
        /// <param name="s">The string to extract sentences from.</param>
        /// <returns></returns>
        public static string[] Tokenize(string s)
        {
            // NOTE: the order of statements in this method is important

            // punctuations happenning at the end of sentence
            s = StringUtil.ReplaceAllRegex(s, PuncEosGroup, "$1" + Eos);

            //while(StringUtil.MatchesRegex(s, InitialsPattern))
                //s = StringUtil.ReplaceFirstRegex(s, InitialsPattern, "$1$2");

            //while (StringUtil.MatchesRegex(s, OneLetterInitialsPattern))
            //    s = StringUtil.ReplaceFirstRegex(s, OneLetterInitialsPattern, "$1");

            s = StringUtil.ReplaceAllRegex(s, OneLetterInitialsPattern, "$1");


            s = StringUtil.ReplaceAllRegex(s, ClosingPuncRepairPattern, "$2$1");
            s = StringUtil.ReplaceAllRegex(s, SymmetricPuncRepairPattern, "$2$1");

            s = StringUtil.ReplaceAllRegex(s, SenteceBeginWithNonSpace, "$2$1$3");


            // remove EOS at the end of sentence
            s = StringUtil.ReplaceAllRegex(s, String.Format(@"({0})($|{1})", Eos, EndOfLine), "$2");

            // new-line means a new sentence
            s = StringUtil.ReplaceAllRegex(s, ParagraphPattern, "$1" + Eos);

            // this should be done in the end
            s = StringUtil.ReplaceAllRegex(s, String.Format("{0}+", Eos), Eos);
            return s.Split(Eos.ToCharArray());
        }
    }
}

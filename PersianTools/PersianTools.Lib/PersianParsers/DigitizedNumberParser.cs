using System;
using System.Collections.Generic;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Tries to parse numbers in digits. Also tries to avoid numbers as part of calculations.
    /// </summary>
    public class DigitizedNumberParser
    {
        #region Public Interfaces

        /// <summary>
        /// Searches the specified string for digitized number patterns, and tries to parse the patterns found.
        /// </summary>
        /// <param name="str">The string to be searched.</param>
        /// <returns>A sequence of <see cref="DigitizedNumberPatternInfo"/> objects which contain 
        /// information about the patterns found.</returns>
        public IEnumerable<DigitizedNumberPatternInfo> FindAndParse(string str)
        {
            var l = new List<DigitizedNumberPatternInfo>();

            var wordTokenizer = new WordTokenizer(WordTokenizerOptions.None);
            foreach(var wordInfo in wordTokenizer.Tokenize(str))
            {
                if(wordInfo.IsNumber)
                {
                    var refinedWord = PostProcessNumberWordInfo(wordInfo);
                    double result;
                    if (Double.TryParse(ParsingUtils.ConvertNumber2English(refinedWord.Value), out result))
                    {
                        l.Add(new DigitizedNumberPatternInfo(refinedWord.Value, refinedWord.Index, refinedWord.Length, result));
                    }
                }
            }
            
            return l;
        }

        private WordTokenInfo PostProcessNumberWordInfo(WordTokenInfo wordInfo)
        {
            string strNumber = wordInfo.Value;
            // NOTE: this prevents detecting .123 as number and requires it to be as 0.123
            //       you might remove it later
            int preCutLength = 0;
            int postCutLength = 0;

            while (strNumber[preCutLength] == '.')
            {
                preCutLength++;
            }

            while (strNumber[strNumber.Length - 1 - postCutLength] == '.')
            {
                postCutLength++;
            }

            if(preCutLength > 0 || postCutLength > 0)
            {
                string token = wordInfo.Value;
                token = token.Substring(preCutLength);
                token = token.Substring(0, token.Length - postCutLength);
                var refinedWordInfo = new WordTokenInfo(token, wordInfo.Index + preCutLength, wordInfo.WordCategory);
                return refinedWordInfo;
            }

            return wordInfo;
        }

        #endregion 

        #region Commented out ( Extract (i.e. Parsing) Functions )

        //#region Regex Patterns

        ///// <summary>
        ///// Returns the whole pattern of a number in digits
        ///// </summary>
        //internal string NumericChunksPattern()
        //{
        //    // \u066B == Arabic decimal seperator
        //    // \u066C == Arabic thousand seperator

        //    const string allChars = @"\+|\-|\d|\t| |e|E|\.|/|,|\u066B|\u066C";
        //    return @"(" + allChars + @")+";
        //}

        //#endregion


        ///// <summary>
        ///// Extracts the digitized number we are looking for from the Regex <see cref="Match"/> object.
        ///// </summary>
        ///// <param name="m">The regex match object.</param>
        ///// <returns>An Instance of <see cref="DigitizedNumberPatternInfo"/> that contains the desired pattern found, 
        ///// or null if the pattern found, is not convertible to numeric data types.</returns>
        //private DigitizedNumberPatternInfo ExtractFromDigitizedNumber(Match m)
        //{
        //    string strNumber = ParsingUtils.ConvertNumber2English(m.Value);

        //    string matchContent = m.Value.Trim();
        //    int matchIndex = m.Index;
        //    int matchLength = m.Length;

        //    // NOTE: this prevents detecting .123 as number and requires it to be as 0.123
        //    //       you might remove it later
        //    while (strNumber.StartsWith("."))
        //    {
        //        strNumber = strNumber.Substring(1);
        //        matchContent = matchContent.Substring(1);
        //        matchLength--;
        //        matchIndex++;
        //    }

        //    while(strNumber.EndsWith("."))
        //    {
        //        strNumber = strNumber.Substring(0, strNumber.Length - 1);
        //        matchContent = matchContent.Substring(0, matchContent.Length - 1);
        //        matchLength--;
        //    }

        //    string str = strNumber.TrimStart(null);
        //    int diff = strNumber.Length - str.Length;
        //    matchIndex += diff;
        //    matchLength -= diff;
        //    strNumber = str;
        //    str = strNumber.TrimEnd(null);
        //    diff = strNumber.Length - str.Length;
        //    matchLength -= diff;

        //    double doubleNumber;
        //    if (Double.TryParse(strNumber, out doubleNumber))
        //    {
        //        return new DigitizedNumberPatternInfo(matchContent, matchIndex, matchLength, doubleNumber);
        //    }

        //    return null;
        //}

        #endregion
    }

    /// <summary>
    /// Will contain information about the Digitized Numbers found in a string 
    /// as returned by <see cref="DigitizedNumberParser"/> class.
    /// </summary>
    public class DigitizedNumberPatternInfo : IPatternInfo
    {
        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the number parsed from the pattern.
        /// </summary>
        /// <value>The number.</value>
        public double Number { get; private set; }

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.DigitizedNumber; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitizedNumberPatternInfo"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <param name="len">The length of the found pattern.</param>
        /// <param name="number">The parsed number.</param>
        public DigitizedNumberPatternInfo(string content, int index, int len, double number)
        {
            Content = content;
            Index = index;
            Length = len;
            Number = number;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="DigitizedNumberPatternInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="DigitizedNumberPatternInfo"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Content:{1}{0}At:{2}{0}\tValue:{3}{0}", Environment.NewLine,
                Content, Index , Number);
        }

    }
}

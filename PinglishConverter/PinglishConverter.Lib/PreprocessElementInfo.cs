//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//


using System.Collections.Generic;

namespace SCICT.NLP.Utility.PinglishConverter
{
    ///<summary>
    ///</summary>
    public class PreprocessElementInfo
    {
        public readonly bool IsWholeWord;

        public readonly TokenPosition Position;

        public string PinglishString { get; private set; }

        public readonly bool IsExactWord;

        public readonly List<string> Equivalents = new List<string>();

        public PreprocessElementInfo(string pinglishString)
            : this(pinglishString, false, false, TokenPosition.Any)
        {
        }

        public PreprocessElementInfo(string pinglishString, bool isWholeWord)
            : this(pinglishString, isWholeWord, false, TokenPosition.Any)
        {
        }

        public PreprocessElementInfo(string pinglishString, bool isWholeWord, bool isExactWord, TokenPosition position)
        {
            this.PinglishString = pinglishString;
            this.IsWholeWord = isWholeWord;
            this.IsExactWord = isExactWord;
            this.Position = position;
        }
    }
}

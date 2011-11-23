//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//

using System;

namespace SCICT.NLP.Utility.Transliteration.KNN
{
    /// <summary>
    /// Represents the position of a character in a word
    /// </summary>
    [Flags]
    public enum TokenPosition
    {
        None = 0,
        StartOfWord = 1,
        MiddleOfWord = 2,
        EndOfWord = 4,
        Any = StartOfWord | MiddleOfWord | EndOfWord,
    }
}
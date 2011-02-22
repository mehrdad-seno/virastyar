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
using System.Text;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Helps reading words inside a string in linear time.
    /// </summary>
    public class WordReadingUtility
    {
        /// <summary>
        /// Reads the words from the input string and returns a sequence of words. 
        /// The <paramref name="isHalfSpaceADelim"/> parameter specifies that the half-space 
        /// character is considered as word-delimiter, or is considered as a typical character
        /// that can occur inside a word.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="isHalfSpaceADelim">if set to <c>true</c> the half-space character is considered as a word delimiter.</param>
        /// <returns>The sequence of words inside the input string.</returns>
        public static IEnumerable<WordInfo> ReadWords(string input, bool isHalfSpaceADelim)
        {
            int startIndex = 0;
            int endIndex = 0;

            CharState charState = CharState.WhiteSpace;

            StringBuilder sb = new StringBuilder();
            int len = input.Length;
            char ch;

            bool addNewWord = false;
            bool addCharToWord = false;

            for (int i = 0; i < len; ++i)
            {
                ch = input[i];
                addNewWord = false;
                addCharToWord = false;

                if (StringUtil.IsWhiteSpace(ch))
                {
                    switch (charState)
                    {
                        case CharState.Digit:
                        case CharState.Other:
                        case CharState.Letter:
                            addNewWord = true;
                            break;
                        case CharState.WhiteSpace:
                        default:
                            break;
                    }
                    charState = CharState.WhiteSpace;
                    addCharToWord = false;
                }
                else if (StringUtil.IsHalfSpace(ch))
                {
                    if (isHalfSpaceADelim) // then treat it as a ws
                    {
                        switch (charState)
                        {
                            case CharState.Digit:
                            case CharState.Other:
                            case CharState.Letter:
                                addNewWord = true;
                                break;
                            case CharState.WhiteSpace:
                            default:
                                break;
                        }
                        charState = CharState.WhiteSpace;
                    }
                    else // otherwise treat it as a letter
                    {
                        addCharToWord = true;

                        switch (charState)
                        {
                            case CharState.WhiteSpace:
                            case CharState.Letter:
                                break;
                            case CharState.Digit:
                            case CharState.Other:
                            default:
                                addNewWord = true;
                                break;
                        }

                        charState = CharState.Letter;
                    }
                }
                else if (StringUtil.IsInArabicWord(ch) || Char.IsLetter(ch))
                {
                    addCharToWord = true;

                    switch (charState)
                    {
                        case CharState.WhiteSpace:
                        case CharState.Letter:
                            break;
                        case CharState.Digit:
                        case CharState.Other:
                        default:
                            addNewWord = true;
                            break;
                    }

                    charState = CharState.Letter;

                }
                else if (Char.IsDigit(ch))
                {
                    addCharToWord = true;

                    switch (charState)
                    {
                        case CharState.WhiteSpace:
                        case CharState.Digit:
                            break;
                        case CharState.Letter:
                        case CharState.Other:
                        default:
                            addNewWord = true;
                            break;
                    }

                    charState = CharState.Digit;
                }
                else
                {
                    addCharToWord = true;
                    if (charState != CharState.WhiteSpace)
                        addNewWord = true;
                    charState = CharState.Other;
                }

                if (addNewWord)
                {
                    endIndex = i - 1;
                    yield return new WordInfo(sb.ToString(), startIndex, endIndex);
                    //                    listWords.Add(sb.ToString());
                    sb = new StringBuilder();
                }

                if (addCharToWord)
                {
                    if (sb.Length <= 0)
                        startIndex = i;
                    sb.Append(ch);
                }
            }

            // treat EOS as a whitespace
            switch (charState)
            {
                case CharState.Digit:
                case CharState.Other:
                case CharState.Letter:
                    addNewWord = true;
                    break;
                case CharState.WhiteSpace:
                default:
                    addNewWord = false;
                    break;
            }

            if (addNewWord)
            {
                endIndex = len - 1;
                yield return new WordInfo(sb.ToString(), startIndex, endIndex);
            }
        }

        /// <summary>
        /// Current State of characters currently being parsed
        /// </summary>
        private enum CharState
        {
            /// <summary>
            /// Character is White Space
            /// </summary>
            WhiteSpace,
            /// <summary>
            /// Character is Letter
            /// </summary>
            Letter,
            /// <summary>
            /// Character is Digit
            /// </summary>
            Digit,
            /// <summary>
            /// Character is neither White-Space, Letter, nor Digit.
            /// </summary>
            Other
        }
    }

    /// <summary>
    /// Holds information about the words and their location
    /// </summary>
    public class WordInfo
    {
        /// <summary>
        /// Gets or sets the word content.
        /// </summary>
        /// <value>The word content.</value>
        public string Word { get; private set; }

        /// <summary>
        /// Gets or sets the start index of the word.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets or sets the end index of the word.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordInfo"/> class.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public WordInfo(string word, int startIndex, int endIndex)
        {
            Word = word;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}: ({1}, {2})", Word, StartIndex, EndIndex);
        }
    }
}

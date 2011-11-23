using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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
        public static IEnumerable<TokenInfo> ReadWords(string input, bool isHalfSpaceADelim)
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
                    //yield return new TokenInfo(sb.ToString(), startIndex, endIndex);
                    Debug.Assert((endIndex - startIndex + 1) == sb.ToString().Length);
                    yield return new TokenInfo(sb.ToString(), startIndex);
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

                //yield return new TokenInfo(sb.ToString(), startIndex, endIndex);
                Debug.Assert((endIndex - startIndex + 1) == sb.ToString().Length);
                yield return new TokenInfo(sb.ToString(), startIndex);
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
}

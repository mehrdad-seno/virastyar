using System;
using System.Collections.Generic;

namespace SCICT.NLP.Utility
{
    [Flags]
    public enum WordTokenizerOptions
    {
        None = 0x0,
        ReturnPunctuations = 0x1,
        ReturnWhitespaces = 0x2,
        /// <summary>
        /// Returns whitespace chunks character by character instead of returning them all in a single token.
        /// </summary>
        ReturnWhitespacesCharacterByCharacter = 0x4,

        /// <summary>
        /// Returns punctuation chunks character by character instead of returning them all in a single token.
        /// </summary>
        ReturnPunctuationsCharacterByCharacter = 0x8,

        TreatNumberArabicCharCombinationAsOneWords = 0x10,
        TreatNumberNonArabicCharCombinationAsOneWords = 0x11,
        TreatArabicNonArabicCharCombinationAsOneWords = 0x12
    }

    [Flags]
    public enum WordCategories
    {
        None = 0x0,
        AlphaArabic = 0x1,
        AlphaNonArabic = 0x2,
        Digit = 0x4,
        Punctuation = 0x8,
        WhiteSpace = 0x10,
    }

    /// <summary>
    /// A general purpose (English, or Persian), customizable, and fast word tokenizer
    /// </summary>
    public class WordTokenizer
    {
        private readonly bool m_retPuncs;
        private readonly bool m_retWs;
        private readonly bool m_retWsCharByChar;
        private readonly bool m_retPuncsCharByChar;
        private readonly bool m_isNumAr1Word;
        private readonly bool m_isNumNonAr1Word;
        private readonly bool m_isArNonAr1Word;

        public WordTokenizer(WordTokenizerOptions options)
        {
            // read options
            m_retPuncs = IsFlagOn(options, WordTokenizerOptions.ReturnPunctuations);
            m_retWs = IsFlagOn(options, WordTokenizerOptions.ReturnWhitespaces);
            m_retWsCharByChar = IsFlagOn(options, WordTokenizerOptions.ReturnWhitespacesCharacterByCharacter);
            m_retPuncsCharByChar = IsFlagOn(options, WordTokenizerOptions.ReturnPunctuationsCharacterByCharacter);
            m_isNumAr1Word = IsFlagOn(options, WordTokenizerOptions.TreatNumberArabicCharCombinationAsOneWords);
            m_isNumNonAr1Word = IsFlagOn(options, WordTokenizerOptions.TreatNumberNonArabicCharCombinationAsOneWords);
            m_isArNonAr1Word = IsFlagOn(options, WordTokenizerOptions.TreatArabicNonArabicCharCombinationAsOneWords);
        }

        public IEnumerable<WordTokenInfo> Tokenize(string line)
        {
            return Tokenize(line, 0);
        }

        private static bool CanHappenInEnglishTransliteration(char ch)
        {
            return StringUtil.IsSingleQuote(ch) || ch == '@';
        }

        public IEnumerable<WordTokenInfo> Tokenize(string line, int fromChar)
        {
            // check exceptional conditions
            if(String.IsNullOrEmpty(line) || fromChar >= line.Length)
            {
                yield break;
            }
            
            // now start reading words
            char ch0 = line[fromChar];
            var charCat0 = GetCharCat(ch0);
            var mainCharCat = (WordCategories) 0;

            int startIndex = fromChar;
            for (int i = fromChar + 1; i < line.Length; i++)
            {
                char ch1 = line[i];
                var charCat1 = GetCharCat(ch1);

                if(charCat0 != charCat1)
                {
                    // make this flag true, the upcoming rules have the chance to turn it off
                    bool isNewWordMet = true;

                    // apostraphe is considered inside word if preceeded by an english letter
                    if (CanHappenInEnglishTransliteration(ch1) && charCat0 == WordCategories.AlphaNonArabic)
                    {
                        charCat1 = WordCategories.AlphaNonArabic;
                        isNewWordMet = false;
                    }
                    else if(charCat1 == WordCategories.Digit && (ch0 == '+' || ch0 == '-') &&
                        (i <= 1 || (i >= 2 && Char.IsWhiteSpace(line[i - 2]))))
                    {
                        isNewWordMet = false;
                    }
                    // thousand separator is considered as part of a number iff it is surrounded by digits
                    else if (StringUtil.IsThousandSeparator(ch1) && Char.IsDigit(ch0) && i < line.Length - 1 && Char.IsDigit(line[i + 1]))
                    {
                        charCat1 = WordCategories.Digit;
                        isNewWordMet = false;
                    }
                    // decimal separator after a digit or + or - sign is considered as in number
                    else if(StringUtil.IsDecimalSeparator(ch1) && (charCat0 == WordCategories.Digit || ch0 == '+' || ch0 == '-'))
                    {
                        charCat1 = WordCategories.Digit;
                        isNewWordMet = false;
                    }
                    // decimal separator before a digit is considered as in number
                    else if (StringUtil.IsDecimalSeparator(ch1) && i < line.Length - 1 && Char.IsDigit(line[i + 1]))
                    {
                        charCat1 = WordCategories.Digit;
                        // but do not turn off isNewWordMet
                    }
                    else if((ch1 == 'e' || ch1 == 'E') && charCat0 == WordCategories.Digit)
                    {
                        charCat1 = WordCategories.Digit;
                        isNewWordMet = false;
                    }
                    else if (m_isNumAr1Word && charCat1 == WordCategories.Digit && charCat0 == WordCategories.AlphaArabic)
                    {
                        isNewWordMet = false;
                        charCat1 = WordCategories.AlphaArabic;
                    }
                    else if (m_isNumAr1Word && charCat1 == WordCategories.AlphaArabic && charCat0 == WordCategories.Digit)
                    {
                        mainCharCat |= charCat0 | charCat1;

                        isNewWordMet = false;
                        // no need to change the character category 1
                    }
                    else if (m_isNumNonAr1Word && charCat1 == WordCategories.Digit && charCat0 == WordCategories.AlphaNonArabic)
                    {
                        mainCharCat |= charCat0 | charCat1;
                        
                        isNewWordMet = false;
                        charCat1 = WordCategories.AlphaNonArabic;
                    }
                    else if (m_isNumNonAr1Word && charCat1 == WordCategories.AlphaNonArabic && charCat0 == WordCategories.Digit)
                    {
                        mainCharCat |= charCat0 | charCat1;

                        isNewWordMet = false;
                        // no need to change the character category 1
                    }
                    else if(m_isArNonAr1Word && charCat1 == WordCategories.AlphaArabic && charCat0 == WordCategories.AlphaNonArabic)
                    {
                        mainCharCat |= charCat0 | charCat1;

                        isNewWordMet = false;
                        // do not change character category
                    }
                    else if(m_isArNonAr1Word && charCat1 == WordCategories.AlphaNonArabic && charCat0 == WordCategories.AlphaArabic)
                    {
                        mainCharCat |= charCat0 | charCat1;

                        isNewWordMet = false;
                        // do not change character category
                    }


                    if (isNewWordMet)
                    {
                        string wordContent = line.Substring(startIndex, i - startIndex);

                        // NOTE: this condition is exactly duplicated in a few lines later
                        if (!((!m_retPuncs && charCat0 == WordCategories.Punctuation) ||
                            (!m_retWs && charCat0 == WordCategories.WhiteSpace)))
                        {
                            mainCharCat |= charCat0;

                            if ((charCat0 == WordCategories.WhiteSpace && m_retWsCharByChar) || 
                                (charCat0 == WordCategories.Punctuation && m_retPuncsCharByChar))
                            {
                                int cicount = wordContent.Length;
                                for(int ci = 0; ci < cicount; ci++)
                                {
                                    yield return new WordTokenInfo(wordContent[ci].ToString(), startIndex + ci, mainCharCat);
                                }
                            }
                            else
                            {
                                yield return new WordTokenInfo(wordContent, startIndex, mainCharCat);
                            }

                            mainCharCat = (WordCategories)0;
                        }

                        startIndex = i;
                    }
                }

                ch0 = ch1;
                charCat0 = charCat1;
            }

            // NOTE: this condition is exactly duplicated in a few lines earlier
            string lastWordContent = line.Substring(startIndex);
            if (!((!m_retPuncs && charCat0 == WordCategories.Punctuation) ||
                (!m_retWs && charCat0 == WordCategories.WhiteSpace)))
            {
                mainCharCat |= charCat0;

                if ((charCat0 == WordCategories.WhiteSpace && m_retWsCharByChar) ||
                    (charCat0 == WordCategories.Punctuation && m_retPuncsCharByChar))
                {
                    int cicount = lastWordContent.Length;
                    for (int ci = 0; ci < cicount; ci++)
                    {
                        yield return new WordTokenInfo(lastWordContent[ci].ToString(), startIndex + ci, mainCharCat);
                    }
                }
                else
                {
                    yield return new WordTokenInfo(lastWordContent, startIndex, mainCharCat);
                }
            }
        }

        private static WordCategories GetCharCat(char ch)
        {
            if (StringUtil.IsInArabicWord(ch))
                return WordCategories.AlphaArabic;
            else if (Char.IsLetter(ch))
                return WordCategories.AlphaNonArabic;
            else if (Char.IsDigit(ch))
                return WordCategories.Digit;
            else if (Char.IsSymbol(ch) || Char.IsPunctuation(ch))
                return WordCategories.Punctuation;
            else if (Char.IsControl(ch) || Char.IsWhiteSpace(ch))
                return WordCategories.WhiteSpace;

            //Debug.Assert(false, "Unknown character category in word-tokenizer!");
            return WordCategories.WhiteSpace;
        }

        private static bool IsFlagOn(WordTokenizerOptions allFlags, WordTokenizerOptions singleFlag)
        {
            var f = (int) singleFlag;
            var r = (int) allFlags & f;
            return (r == f);
        }
    }

    public class WordTokenInfo : TokenInfo
    {
        public WordTokenInfo(string token, int index)
            : this(token, index, WordCategories.None)
        {
        }

        public WordTokenInfo(string token, int index, WordCategories category)
            : base(token, index)
        {
            WordCategory = category;
        }

        public WordCategories WordCategory { get; private set; }

        private static bool IsFlagOn(WordCategories allFlags, WordCategories singleFlag)
        {
            var f = (int)singleFlag;
            var r = (int)allFlags & f;
            return (r == f);
        }

        public bool IsNumber
        {
            get
            {
                return WordCategory == WordCategories.Digit;
            }
        }

        public bool IsMixedLanguageLetters
        {
            get
            {
                return IsFlagOn(WordCategory, WordCategories.AlphaArabic) &&
                       IsFlagOn(WordCategory, WordCategories.AlphaNonArabic);
            }
        }

        public bool IsMixedLetters
        {
            get
            {
                int flagCount = 0;
                if (IsFlagOn(WordCategory, WordCategories.AlphaArabic))
                    flagCount++;
                if (IsFlagOn(WordCategory, WordCategories.AlphaNonArabic))
                    flagCount++;
                if (IsFlagOn(WordCategory, WordCategories.Digit))
                    flagCount++;

                return flagCount > 1;
            }
        }
    }
}

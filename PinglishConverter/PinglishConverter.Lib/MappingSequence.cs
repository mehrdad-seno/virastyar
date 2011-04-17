//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//

using System;
using System.Collections.Generic;

namespace SCICT.NLP.Utility.PinglishConverter
{
    using KeyValueList = Dictionary<string, int>;

    [Serializable]    
    class MappingSequence
    {
        public const char Separator = '|';
        protected readonly PatternStorage m_patternStorage = new PatternStorage();

        public KeyValueList this[char ch, string prefix, string postfix]
        {
            get
            {
                return m_patternStorage[ch, prefix, postfix];
            }
        }

        public void LearnWordMapping(PinglishString word, int prefixGram, int postfixGram)
        {
            var len = word.EnglishLetters.Count;

            for (int i = 0; i < len; ++i)
            {
                var prefix = GetPrefixForIndex(word.EnglishString, i, prefixGram);
                var postfix = GetPostfixForIndex(word.EnglishString, i, postfixGram);
                
                if (prefix.Length == prefixGram && postfix.Length == postfixGram)
                {
                    if (word.EnglishLetters[i]=='h' && prefix == "" && postfix=="laghi")
                    {
                        int kiloo;
                        kiloo = 4;
                        
                    }
                        
                    UpdateDictionary(
                            word.EnglishLetters[i], prefix, postfix, word.PersianLetters[i]);
                }
            }
        }

        public static string GetPrefixForIndex(string str, int index, int prefixGram)
        {
            if (prefixGram >= 0)
            {
                int length = prefixGram;
                if (prefixGram >= index)
                {
                    length = index;
                }
                return str.Substring(index - length, length);
            }
            return null;
        }

        public static string GetPostfixForIndex(string str, int index, int postfixGram)
        {
            if (postfixGram >= 0)
            {
                int length = postfixGram;
                if (str.Length <= index + postfixGram)
                {
                    length = (str.Length - index) - 1;
                }
                return str.Substring(index + 1, length);
            }
            return null;
        }

        private void UpdateDictionary(char ch, string prefix, string postfix, string mappedChar)
        {
            //if (mappedChar == "آ")
            //    mappedChar = "ا";
            m_patternStorage.AddOrUpdatePattern(ch, prefix, postfix, mappedChar);
        }

        public static string CreateKey(string prefix, string postfix, char ch)
        {
            return String.Format("{0}{1}{2}{1}{3}", prefix, Separator, ch, postfix);
        }
    }
}

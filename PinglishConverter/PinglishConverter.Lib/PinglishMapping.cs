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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using SCICT.NLP.Persian.Constants;

namespace SCICT.NLP.Utility.PinglishConverter
{
    using KeyValueList = Dictionary<string, int>;
    using System.Diagnostics;

    /// <summary>
    /// Instance of this class will learns the mappings from sample dataset.
    /// </summary>
    [Serializable]
    public class PinglishMapping
    {
        #region Private Members

        private readonly MappingSequence m_mappingSequences = new MappingSequence();

        private List<PinglishString> m_pinglishDataSet = new List<PinglishString>();

        #endregion

        #region Ctors
        public PinglishMapping()
        {

        }

        public PinglishMapping(string mappingFileName)
        {
            try
            {
                List<PinglishString> list = PinglishConverterUtils.LoadPinglishStrings(mappingFileName);
                Learn(list, false);
                m_pinglishDataSet.AddRange(list.RemoveDuplicates());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion

        #region Public Methods

        public void Learn(List<PinglishString> listOfWords, bool appendToInternalDataset)
        {
            foreach (var word in listOfWords)
            {
                Learn(word, false);
            }

            if (appendToInternalDataset)
            {
                m_pinglishDataSet = PinglishConverterUtils.MergePinglishStringLists(
                    m_pinglishDataSet, listOfWords, PinglishStringNormalizationOptions.NoDuplicatesEntries);
            }
        }

        public void Learn(PinglishString word, bool appendToInternalDataset)
        {
            for (int prefixGram = 2; prefixGram >= 0; prefixGram--)
            {
                for (int postfixGram = 5; postfixGram >= 0; postfixGram--)
                {
                    m_mappingSequences.LearnWordMapping(word, prefixGram, postfixGram);
                }
            }

            if (appendToInternalDataset && !m_pinglishDataSet.Contains(word))
                m_pinglishDataSet.Add(word);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinglishWord"></param>
        /// <returns></returns>
        public List<PinglishString> SuggestWords(string pinglishWord, bool removeDuplicates)
        {
            var words = SuggestByExactSearchInDataset(pinglishWord);

            if (words.Count > 0)
                return words;

            var len = pinglishWord.Length;

            words.Add(new PinglishString());

            var charSuggs = new List<string>();

            for (int index = 0; index < len; ++index)
            {
                charSuggs.Clear();

                #region Dist-5

                charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 0, 5);
                charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 1, 4);
                charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 2, 3);
                charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 3, 2);
                //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 4, 1);

                #endregion

                #region Dist-4

                //if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 0, 4);
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 1, 3);
                    //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 2, 2);
                    //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 3, 1);
                }

                #endregion

                #region Dist-3

                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 0, 3);
                }
                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 1, 2);
                }
                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 2, 1);
                    //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 3, 0);
                }

                #endregion

                #region Dist-2

                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 0, 2);
                }
                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 1, 1);
                    //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 2, 0);
                }

                #endregion

                #region Dist-1

                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 0, 1);
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 1, 0);
                }

                #endregion

                #region Dist-0

                if (charSuggs.Count == 0)
                {
                    charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 0, 0);
                }

                #endregion

                #region Heuristical techniques to improve results

                // No Erabs at the begining of the word
                if (index == 0)
                {
                    for (int i = charSuggs.Count - 1; i >= 0; i--)
                    {
                        if (PersianAlphabets.Erabs.Contains(charSuggs[i]))
                        {
                            charSuggs.RemoveAt(i);
                        }
                    }
                }

                // No Pseudo-space at the end of the word
                if (index == len - 1)
                {
                    for (int i = charSuggs.Count - 1; i >= 0; i--)
                    {
                        int endIndex = charSuggs[i].Length - 1;
                        if (endIndex >= 0 && charSuggs[i][endIndex] == PseudoSpace.ZWNJ)
                            charSuggs.RemoveAt(i);
                    }
                }

                #endregion

                if (charSuggs.Count == 0)
                {
                    // TODO: Generate every possible mapping
                    var map = SingleValueCharMappings.TryGetValue(pinglishWord[index]);
                    if (map != null)
                    {
                        charSuggs.Add(map.Value.ToString());
                    }
                    else
                    {
                        // TODO
                        //throw new Exception();
                    }
                }

                words.Update(pinglishWord[index], charSuggs);
            }

            if (removeDuplicates)
                return words.Distinct(new PinglishStringEqualityComparer()).ToList();
            else
                return words;
        }

        private List<PinglishString> SuggestByExactSearchInDataset(string pinglishWord)
        {
            var words = new List<PinglishString>();

            foreach (var pinglishSample in m_pinglishDataSet)
            {
                if (pinglishSample.EnglishString == pinglishWord)
                    words.Add(pinglishSample);
            }

            return words;
        }

        #region Static

        public static PinglishMapping LoadConverterEngine(string fileName)
        {
            // TODO: Exception Handling
            var file = File.OpenRead(fileName);
            var bf = new BinaryFormatter();

            var obj = (PinglishMapping)bf.Deserialize(file);
            file.Close();
            return obj;
        }

        public static void SaveConverterEngine(string fileName, PinglishMapping pinglishMapping)
        {
            Stream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            var bf = new BinaryFormatter();

            bf.Serialize(file, pinglishMapping);

            file.Close();
        }

        #endregion

        #endregion

        #region Private Methods

        private static List<string> GetCharSuggs(MappingSequence mappingSequences, string pinglishWord, int index, List<string> charSuggs, int prefixGram, int postfixGram)
        {
            var tmpSuggs = GetSuggsForKey(mappingSequences, pinglishWord, index, prefixGram, postfixGram);
            charSuggs = Union(charSuggs, tmpSuggs);
            return charSuggs;
        }

        private static List<string> GetSuggsForKey(MappingSequence mappingSequences, string pinglishWord, int index, int prefixGram, int postfixGram)
        {
            var prefix = MappingSequence.GetPrefixForIndex(pinglishWord, index, prefixGram);
            if (prefix.Length != prefixGram)
                return new List<string>();
            
            var postfix = MappingSequence.GetPostfixForIndex(pinglishWord, index, postfixGram);
            if (postfix.Length != postfixGram)
                return new List<string>();

            var ch = pinglishWord[index];

            List<string> charSuggs = null;
            try
            {
                // TODO: If Key does not exist ?!
                var listOfAllSuggs = mappingSequences[ch, prefix, postfix];

                if (listOfAllSuggs != null)
                {
                    var query = from KeyValuePair<string, int> pair in listOfAllSuggs
                                orderby pair.Value descending
                                select pair.Key;

                    charSuggs = query.ToList();
                }
                else
                {
                    charSuggs = new List<string>();
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }

            return charSuggs;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Affects the first parameter</remarks>
        /// <returns></returns>
        private static List<string> Union(List<string> firstList, List<string> secondList)
        {
            if (secondList == null)
                return firstList;

            foreach (var str in secondList.Where(str => !firstList.Contains(str)))
            {
                firstList.Add(str);
            }
            return firstList;
        }

        #endregion
    }
}

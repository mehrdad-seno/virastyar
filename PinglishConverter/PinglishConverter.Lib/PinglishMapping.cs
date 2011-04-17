//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//


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

        #region Public Members

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
            for (int prefixGram = 3; prefixGram >= 0; prefixGram--)
            {
                for (int postfixGram = 5 - prefixGram; postfixGram >= 0; postfixGram--)
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

            //var charSuggs = new List<string>();
            var charSuggsWithCount = new Dictionary<string, int>();

            for (int index = 0; index < len; ++index)
            {
                charSuggsWithCount.Clear();
                //#region old_approach

                //#region Dist-5

                //charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 0, 5);
                //charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 1, 4);
                //charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 2, 3);
                //charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 3, 2);
                ////charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 4, 1);

                //#endregion

                //#region Dist-4

                ////if (charSuggs.Count == 0)
                //{
                //    charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 0, 4);
                //    charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 1, 3);
                //    //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 2, 2);
                //    //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 3, 1);
                //}

                //#endregion

                //#region Dist-3

                //if (charSuggsWithCount.Count == 0)
                //{
                //    //if (charSuggs.Count == 0)
                //    {
                //        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 0,
                //                                          3);
                //    }
                //    //if (charSuggs.Count == 0)
                //    {
                //        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 1,
                //                                          2);
                //    }
                //    //if (charSuggs.Count == 0)
                //    {
                //        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 2,
                //                                          1);
                //        //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 3, 0);
                //    }
                //}

                //#endregion

                //#region Dist-2

                //if (charSuggsWithCount.Count == 0)
                //{
                //    //if (charSuggs.Count == 0)
                //    {
                //        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 0,
                //                                          2);
                //    }
                //    //if (charSuggs.Count == 0)
                //    {
                //        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 1,
                //                                          1);
                //        //charSuggs = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggs, 2, 0);
                //    }
                //}

                //#endregion

                //#region Dist-1

                //if (charSuggsWithCount.Count == 0)
                //{
                //    charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 0, 1);
                //    charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 1, 0);
                //}

                //#endregion

                //#region Dist-0

                //if (charSuggsWithCount.Count == 0)
                //{
                //    charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, 0, 0);
                //}
                //#endregion
                //#endregion

                #region new_approach

                int[] pre = {2, 3, 2, 1, 1, 3, 1, 2, 1, 0, 0, 0, 3, 0, 2, 0, 1, 0};
                int[] pst = {3, 2, 2, 4, 3, 1, 2, 1, 1, 5, 4, 3, 0, 2, 0, 1, 0, 0};

                
                int downCounter = 3;

                
                
                for (int i=0;i<pre.Length;i++)
                {
                    if (charSuggsWithCount.Count==0)
                    {
                        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, pre[i], pst[i]);
                    }
                    else if (charSuggsWithCount.Count!=0 && downCounter>0)
                    {
                        downCounter--;
                        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, pre[i], pst[i]);
                    }
                    else 
                        break;
                }

                #endregion

                #region Heuristical techniques to improve results

                // No Erabs at the begining of the word
                if (index == 0)
                {
                    charSuggsWithCount = charSuggsWithCount.Where(item => !PersianAlphabets.Erabs.Contains(item.Key))
                        .ToDictionary(x => x.Key, x => x.Value);
                }

                // No Pseudo-space at the end of the word
                if (index == len - 1)
                {
                    charSuggsWithCount = charSuggsWithCount.Where(item =>
                                                                      {
                                                                          int endIndex = item.Key.Length - 1;
                                                                          return !(endIndex >= 0 &&
                                                                                   item.Key[endIndex] ==
                                                                                   PseudoSpace.ZWNJ);
                                                                      }).ToDictionary(x => x.Key, x => x.Value);
                }

                #endregion

                if (charSuggsWithCount.Count == 0)
                {
                    // TODO: Generate every possible mapping
                    var map = SingleValueCharMappings.TryGetValue(pinglishWord[index]);
                    if (map != null)
                    {
                        charSuggsWithCount.Add(map.Value.ToString(), 1);
                    }
                    else
                    {
                        // TODO
                        //throw new Exception();
                    }
                }

                words.Update(pinglishWord[index], charSuggsWithCount);
            }

            if (removeDuplicates)
                return words.Distinct(new PinglishStringEqualityComparer()).ToList();
            else
                return words;
        }

        public IEnumerable<PinglishString> DataSet
        {
            get { return m_pinglishDataSet; }
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

        private static Dictionary<string, int> GetCharSuggs(MappingSequence mappingSequences, string pinglishWord, int index, Dictionary<string, int> charSuggs, int prefixGram, int postfixGram)
        {
            var tmpSuggs = GetSuggsForKey(mappingSequences, pinglishWord, index, prefixGram, postfixGram);
            charSuggs = Union(charSuggs, tmpSuggs);
            return charSuggs;
        }

        private static Dictionary<string, int> GetSuggsForKey(MappingSequence mappingSequences, string pinglishWord, int index, int prefixGram, int postfixGram)
        {
            var prefix = MappingSequence.GetPrefixForIndex(pinglishWord, index, prefixGram);
            if (prefix.Length != prefixGram)
                return new Dictionary<string, int>();
            
            var postfix = MappingSequence.GetPostfixForIndex(pinglishWord, index, postfixGram);
            if (postfix.Length != postfixGram)
                return new Dictionary<string, int>();

            var ch = pinglishWord[index];

            Dictionary<string, int> charSuggs = null;
            try
            {
                // TODO: If Key does not exist ?!
                var listOfAllSuggs = mappingSequences[ch, prefix, postfix];

                if (listOfAllSuggs != null)
                {
                    charSuggs = listOfAllSuggs.ToDictionary(sug => sug.Key, sug => sug.Value);
                }
                else
                {
                    charSuggs = new Dictionary<string, int>();
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

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Affects the first parameter</remarks>
        /// <returns></returns>
        private static Dictionary<string, int> Union(Dictionary<string, int> firstList, Dictionary<string, int> secondList)
        {
            if (secondList == null)
                return firstList;

            foreach (var keyValuePair in secondList)
            {
                if (firstList.ContainsKey(keyValuePair.Key))
                {
                    firstList[keyValuePair.Key] += keyValuePair.Value;
                }
                else
                {
                    firstList.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return firstList;
        }

        #endregion
    }
}

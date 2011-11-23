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
using SCICT.NLP.Morphology.Inflection;
using SCICT.NLP.Persian.Constants;
using System.Diagnostics;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Transliteration.PrioritySelection;

namespace SCICT.NLP.Utility.Transliteration.KNN
{
    /// <summary>
    /// Instance of this class will learns the mappings from sample dataset.
    /// </summary>
    [Serializable]
    public class PinglishMapping : ITransliterator
    {
        #region Private Members

        private readonly MappingSequence m_mappingSequences = new MappingSequence();
        private List<PinglishString> m_pinglishDataSet = new List<PinglishString>();

        private readonly PersianSuffixLemmatizer _suffixer;
        private List<string> _dic = new List<string>();
        private WordMapper _wordMapper;

        #endregion

        #region Ctors
        public PinglishMapping(string mappingFileName , string dicPath ,  PruneType pruneType)
        {
            try
            {
                List<PinglishString> list = PinglishConverterUtils.LoadPinglishStrings(mappingFileName);
                Learn(list, false);
                m_pinglishDataSet.AddRange(list.RemoveDuplicates());

                _suffixer = new PersianSuffixLemmatizer(true);
                Tools.LoadList(ref _dic , dicPath);
                _wordMapper = new WordMapper(_dic,pruneType);
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

            foreach (char enChar in m_sum.Keys)
            {
                int sum = m_sum[enChar];

                if (!m_mappingDistribution.ContainsKey(enChar))
                    m_mappingDistribution.Add(enChar, new Dictionary<string, double>());
                foreach (string faChar in m_counter[enChar].Keys)
                {
                    if (!m_mappingDistribution[enChar].ContainsKey(faChar))
                        m_mappingDistribution[enChar].Add(faChar, 0);

                    m_mappingDistribution[enChar][faChar] = m_counter[enChar][faChar] / (double)sum;
                }
            }

            if (appendToInternalDataset)
            {
                m_pinglishDataSet = PinglishConverterUtils.MergePinglishStringLists(
                    m_pinglishDataSet, listOfWords, PinglishStringNormalizationOptions.NoDuplicatesEntries);
            }
        }

        private Dictionary<char, Dictionary<string, double>> m_mappingDistribution = new Dictionary<char, Dictionary<string, double>>();
        Dictionary<char , Dictionary<string, int>> m_counter  = new Dictionary<char, Dictionary<string, int>>();
        Dictionary<char ,int> m_sum = new Dictionary<char, int>();

        public void Learn(PinglishString word, bool appendToInternalDataset)
        {
            for (int i = 0; i < word.Length; i++)
            {
                char enChar = word.EnglishLetters[i];
                string faChar = word.PersianLetters[i];
                if (!m_counter.ContainsKey(enChar))
                    m_counter.Add(enChar, new Dictionary<string, int>());

                if (!m_counter[enChar].ContainsKey(faChar))
                    m_counter[enChar].Add(faChar, 0);

                if (!m_sum.ContainsKey(enChar))
                    m_sum.Add(enChar, 0);

                m_counter[enChar][faChar] += 1;
                m_sum[enChar] += 1;
            }

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
        public List<ResultWord> SuggestWords(ResultWord piWord, bool justFirst, bool removeDuplicates)
        {
            string pinglishWord = piWord.Word;
            var distribution = new KeyValuePair<string, double>[pinglishWord.Length][];


            var exactWords = SuggestByExactSearchInDataset(pinglishWord);
           
            //if (words.Count > 0)
            //    return words;

            var words = new List<PinglishString>();
            var len = pinglishWord.Length;

            words.Add(new PinglishString());

            //var charSuggs = new List<string>();
            Dictionary<string, double> charSuggsWithCount = new Dictionary<string, double>();

            for (int index = 0; index < len; ++index)
            {
                charSuggsWithCount.Clear();
                int[] pre = { 2, 3, 2, 1, 1, 3, 1, 2, 1, 0, 0, 0, 3, 0, 2, 0, 1, 0 };
                int[] pst = { 3, 2, 2, 4, 3, 1, 2, 1, 1, 5, 4, 3, 0, 2, 0, 1, 0, 0 };

                for (int i = 0; i < pre.Length; i++)
                        charSuggsWithCount = GetCharSuggs(m_mappingSequences, pinglishWord, index, charSuggsWithCount, pre[i], pst[i]);
               

                // No Erabs at the begining of the word
                if (index == 0)
                {
                    charSuggsWithCount = charSuggsWithCount.Where(item => !PersianAlphabets.Erabs.Contains(item.Key))
                        .ToDictionary(x => x.Key, x => x.Value);
                }

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

                double sum = 0;
                var templist = new List<KeyValuePair<string, double>>();
                foreach (KeyValuePair<string, double> kv in charSuggsWithCount)
                    sum += kv.Value;
                foreach (KeyValuePair<string, double> kv in charSuggsWithCount)
                    templist.Add(new KeyValuePair<string, double>(kv.Key , kv.Value/sum));
                templist.Sort((kv1, kv2) => Math.Sign(kv2.Value - kv1.Value));
                distribution[index] = templist.ToArray();

                //words.Update(pinglishWord[index], charSuggsWithCount);
            }

            words.InsertRange(0, exactWords);
            List<string >listExact = exactWords.Select(v => Tools.NormalizeString(v.PersianString)).ToList();

            var semiFinRes = _wordMapper.SelectBests(piWord, distribution, justFirst);
            var finRes = new List<ResultWord>();

            foreach (var resultWord in semiFinRes)
            {
                if (listExact.Contains(resultWord.Word))
                {
                    finRes.Add(new ResultWord(resultWord.Word, resultWord.Type |ResultType.HittedToDic, resultWord.Probability + 1,
                                              resultWord.IsFinal));
                    listExact.Remove(resultWord.Word);
                }
                else
                    finRes.Add(resultWord);
            }

            foreach (var exactRest in listExact)
                finRes.Add(new ResultWord(exactRest , ResultType.Transliterate|ResultType.HittedToDic , 1.0,true));

          //  foreach (PinglishString s in exactWords)
              //  finRes.Add(new ResultWord(StringUtil.RemoveErab(s.PersianString), ResultType.Transliterate | piWord.Type | ResultType.HittedToDic, 1.0, true)););
            return finRes;
            /*

            string pinglishWord = piWord.Word;
            

            var exactWords = SuggestByExactSearchInDataset(pinglishWord);

            if (justFirst && exactWords.Count!=0)
            {
                List<ResultWord> list = new List<ResultWord>();
                foreach (var exactWord in exactWords)
                    list.Add(new ResultWord(Tools.NormalizeString( exactWord.PersianString),piWord.Type | ResultType.Transliterate|ResultType.HittedToDic , 1.0 ,true));

                return list;
            }

            //if (words.Count > 0)
            //    return words;

            var words = new List<PinglishString>();
            var len = pinglishWord.Length;

            words.Add(new PinglishString());

            //var charSuggs = new List<string>();
            var charSuggsWithCount = new Dictionary<string, int>();

            for (int index = 0; index < len; ++index)
            {
                charSuggsWithCount.Clear();

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
                    //charSuggsWithCount = charSuggsWithCount.Where(item =>
                    //                                                  {
                    //                                                      int endIndex = item.Key.Length - 1;
                    //                                                      return !(endIndex >= 0 &&
                    //                                                               item.Key[endIndex] ==
                    //                                                               PseudoSpace.ZWNJ);
                    //                                                  }).ToDictionary(x => x.Key, x => x.Value);
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

                if (justFirst)
                {
                    var seen = false;
                    foreach (var s in charSuggsWithCount.Keys)
                    {
                        if (seen)
                            charSuggsWithCount.Remove(s);

                        seen = true;
                    }
                }


                words.Update(pinglishWord[index], charSuggsWithCount);
            }

            words.InsertRange(0, exactWords);
            List<PinglishString> semiFinanllResult = new List<PinglishString>();

            if (removeDuplicates)
                semiFinanllResult =  words.Distinct(new PinglishStringEqualityComparer()).ToList();
            else
                semiFinanllResult =  words;

            var finRes = new List<ResultWord>();

            bool first = true;

            foreach (PinglishString s in semiFinanllResult)
            {
                string perWord = s.PersianString;
                perWord = StringUtil.RemoveErab(perWord);
                if (Tools.IsValidInDictionary(perWord, _dic, _suffixer, _pruneType))
                {
                    finRes.Add(new ResultWord(perWord, ResultType.Transliterate | piWord.Type | ResultType.HittedToDic,
                                              (first ? 1 : GetProbability(s)) * piWord.Probability,
                                                                           true));
                    first = false;
                }
                else
                {
                    finRes.Add(new ResultWord(perWord, ResultType.Transliterate | piWord.Type,
                                              (first ? 1 : GetProbability(s))*piWord.Probability,
                                              true));
                    first = false;
                }
            }

            foreach(PinglishString s in exactWords )
                finRes.Add(new ResultWord(StringUtil.RemoveErab( s.PersianString), ResultType.Transliterate | piWord.Type | ResultType.HittedToDic, 1.0, true));
            return finRes;
            */

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

        private static Dictionary<string, double > GetCharSuggs(MappingSequence mappingSequences, string pinglishWord, int index, Dictionary<string, double> charSuggs, int prefixGram, int postfixGram)
        {
            var tmpSuggs = GetSuggsForKey(mappingSequences, pinglishWord, index, prefixGram, postfixGram);
            charSuggs = Union(charSuggs, tmpSuggs);
            return charSuggs;
        }

        private static Dictionary<string, double> GetSuggsForKey(MappingSequence mappingSequences, string pinglishWord, int index, int prefixGram, int postfixGram)
        {
            var prefix = MappingSequence.GetPrefixForIndex(pinglishWord, index, prefixGram);
            if (prefix.Length != prefixGram)
                return new Dictionary<string, double>();
            
            var postfix = MappingSequence.GetPostfixForIndex(pinglishWord, index, postfixGram);
            if (postfix.Length != postfixGram)
                return new Dictionary<string, double>();

            var ch = pinglishWord[index];

            Dictionary<string, double> charSuggs = null;
            try
            {
                // TODO: If Key does not exist ?!
                var listOfAllSuggs = mappingSequences[ch, prefix, postfix];

                if (listOfAllSuggs != null)
                {
                    charSuggs = listOfAllSuggs.ToDictionary(sug => sug.Key, sug => (sug.Value * Math.Pow(PinglishConverterConfig.PowerFactor , prefixGram + postfixGram)));
                }
                else
                {
                    charSuggs = new Dictionary<string, double>();
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
        private static Dictionary<string, double > Union(Dictionary<string, double > firstList, Dictionary<string, double > secondList)
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

        public List<ResultWord> SuggestFarsiWords(ResultWord pinglishWord , bool justFirsts)
        {
            return SuggestWords(pinglishWord,justFirsts, true);
        }



        private double GetProbability (PinglishString s)
        {
            double prob = 1;

            try
            {
                for (int i = 0; i < s.Length; i++)
                    prob *= m_mappingDistribution[s.EnglishLetters[i]][s.PersianLetters[i]];
            }
            catch (Exception)
            {
                prob = 0.5;
            }
            return prob;
        }



    }
}

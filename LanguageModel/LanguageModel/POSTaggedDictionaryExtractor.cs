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
using System.Linq;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility.Parsers;
using SCICT.NLP.Morphology.Lemmatization;
using System.IO;
using SCICT.NLP.Utility.WordContainer;
using SCICT.Utility;
using SCICT.NLP.Morphology.Inflection;

namespace SCICT.NLP.Utility.LanguageModel
{
    internal struct FreqPOSPair
    {
        public int freq;
        public PersianPOSTag pos;
    }

    ///<summary>
    /// Extract a POS tagged dictionary from a text corpus
    ///</summary>
    public class POSTaggedDictionaryExtractor
    {

        #region Properties

        private StreamWriter m_streamWriter;

        private readonly WordFreqPOSContainerTree m_wordContainerExternal = new WordFreqPOSContainerTree();
      
        private readonly Dictionary<string, int> m_wordList = new Dictionary<string, int>();
        
        private readonly PersianSuffixRecognizer m_suffixRecognizer = new PersianSuffixRecognizer(false, true);

        private readonly Dictionary<string, FreqPOSPair> m_finalList = new Dictionary<string, FreqPOSPair>();

        private double m_progressPercent = 0.0;

        /// <summary>
        /// Gets the percentage of progress.
        /// </summary>
        /// <value>The progress percent.</value>
        public double ProgressPercent
        {
            get
            {
                return m_progressPercent;
            }
            set
            {
                if (value > 1.0) value = 1.0;
                if (value < 0) value = 0.0;
                m_progressPercent = value;
            }
        }

        #endregion

        private bool Init(string fileName)
        {
            try
            {
                m_streamWriter = new StreamWriter(fileName);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        ///<summary>
        /// Add a term
        ///</summary>
        ///<param name="term">Term</param>
        public void AddTerm(string term)
        {
            if (m_wordList.ContainsKey(term))
            {
                m_wordList[term]++;
            }
            else
            {
                m_wordList.Add(term, 1);
            }
        }

        ///<summary>
        /// Extract POS tagged dictionary to a file
        ///</summary>
        ///<param name="fileName">File name</param>
        ///<returns>Tru on success</returns>
        public bool ExtractPOSTaggedDictionary(string fileName)
        {
            PersianPOSTag pos;
            int totalCount = m_wordList.Count;
            int currentStep = 0;
            double remainingPregress = (1.0 - ProgressPercent) / 0.95;

            double denom = (2.0 / ((totalCount) * (totalCount + 1))) * remainingPregress;

            foreach (KeyValuePair<string, int> pair in m_wordList)
            {
                ProgressPercent += ((double)currentStep * denom);
                currentStep++;

                bool curWordAdded = false;
                string word = pair.Key;

                if (!m_wordContainerExternal.Contain(word, out pos))  //external dictionary does not contains the word
                {
                    ReversePatternMatcherPatternInfo[] suffixPatternArray = m_suffixRecognizer.MatchForSuffix(word);
                    if (suffixPatternArray.Length > 0)
                    {
                        foreach (ReversePatternMatcherPatternInfo suffixPattern in suffixPatternArray)
                        {
                            string stem = suffixPattern.BaseWord;

                            if (m_wordContainerExternal.Contain(stem, out pos)) //external dictionary contains the stem
                            {
                                curWordAdded = true;
                                AddWordToFinalList(stem, m_wordList[word], pos);
                                break;
                            }
                            else if (m_wordList.ContainsKey(stem))
                            {
                                curWordAdded = true;
                                AddToDictionary(stem, word);
                                break;
                            }
                        }
                        if (!curWordAdded)
                        {
                            AddToDictionary(word, word);
                        }
                    }
                    else
                    {
                        AddToDictionary(word, word);
                    }
                }
                else
                {
                    //if external dictionary contains the word, add it to file
                    AddWordToFinalList(word, m_wordList[word], pos);
                }
            }

            return DumpFinalList(fileName);
        }

        ///<summary>
        /// Add a text corpus
        ///</summary>
        ///<param name="text">Text string</param>
        public void AddPlainText(string text)
        {
            ProgressPercent += 0.05;
            string[] words = StringUtil.ExtractPersianWordsStandardized(text).Distinct().ToArray();
            ProgressPercent += 0.1;

            int length = words.Length;
            double denom = (2.0 / ((length) * (length + 1))) * 0.15;

            for(int i = 0; i < length; i++)
            {
                AddTerm(words[i]);
                ProgressPercent += ((double)i * denom);
            }
        }

        ///<summary>
        /// Append exteranl POS tagged dictionary
        ///</summary>
        ///<param name="fileName">File name</param>
        public void AppendExternalPOSTaggedDictionary(string fileName)
        {
            ProgressPercent += 0.15;
            m_wordContainerExternal.AppendDictionary(fileName);
            ProgressPercent += 0.15;
        }

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="userSelectedWord">Form of word which user select to add to dictionary</param>
        /// <param name="originalWord">Original word without lemmatization</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        private void AddToDictionary(string userSelectedWord, string originalWord)
        {
            string suffix = originalWord.Remove(0, userSelectedWord.Length);

            PersianPOSTag extractedPOSTag = PersianPOSTag.UserPOS;
            
            if (suffix.Length > 0)
            {
                PersianSuffixesCategory suffixCategory = m_suffixRecognizer.SuffixCategory(suffix);
                extractedPOSTag =  InflectionAnalyser.AcceptingPOS(suffixCategory);

                extractedPOSTag = extractedPOSTag.Set(PersianPOSTag.UserPOS);
            }

            AddWordToFinalList(userSelectedWord, m_wordList[userSelectedWord], extractedPOSTag);
        }

        private void AddWordToFinalList(string word, int freq, PersianPOSTag pos)
        {
            FreqPOSPair pair;

            if (m_finalList.ContainsKey(word))
            {
                pair.freq = m_finalList[word].freq + 1;
                pair.pos = m_finalList[word].pos.Set(pos);

                m_finalList[word] = pair;
            }
            else
            {
                pair.freq = freq;
                pair.pos = pos;

                m_finalList.Add(word, pair);
            }
        }

        private bool DumpFinalList(String fileName)
        {
            if (!Init(fileName))
            {
                return false;
            }

            try
            {
                int totalCount = m_wordList.Count;
                int currentStep = 0;
                double remainingPregress = (1.0 - ProgressPercent);
                double denom = (2.0 / ((totalCount) * (totalCount + 1))) * remainingPregress;

                foreach (KeyValuePair<string, FreqPOSPair> term in m_finalList.Distinct().ToArray())
                {
                    ProgressPercent += ((double)currentStep * denom);
                    currentStep++;

                    m_streamWriter.WriteLine(term.Key + "\t" + term.Value.freq + "\t" + term.Value.pos);
                }

                m_streamWriter.Close();
                ProgressPercent = 1.0;

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}

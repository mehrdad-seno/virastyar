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
using System.Diagnostics;
using System.IO;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;

namespace SCICT.Utility.SpellChecker
{
    ///<summary>
    /// An ignore list used to ignore desired words from processing
    ///</summary>
    public class IgnoreList
    {
        public class IgnoreItemChangeEventArgs : EventArgs
        {
            public string IgnoreItem;
        }

        public event EventHandler<IgnoreItemChangeEventArgs> IgnoreItemAdded;
        public event EventHandler<IgnoreItemChangeEventArgs> IgnoreItemRemoved;

        private readonly HashSet<string> m_list = new HashSet<string>();
        
        /// <summary>
        /// Add a word to ignore list 
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns></returns>
        public bool AddToIgnoreList(string word)
        {
            try
            {
                if (!this.m_list.Contains(word))
                {
                    this.m_list.Add(word);
                    OnIgnoreItemAdded(word);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Remove a word to ignore list 
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns></returns>
        public bool RemoveFromIgnoreList(string word)
        {
            try
            {
                if (this.m_list.Contains(word))
                {
                    this.m_list.Remove(word);
                    OnIgnoreItemRemoved(word);
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Clear ignore list
        /// </summary>
        /// <returns></returns>
        public void ClearIgnoreList()
        {
            try
            {
                this.m_list.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// check for word existance in ignore list
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsExistInIgnoreList(string word)
        {
            if (word.Length > 0)
            {
                return this.m_list.Contains(word);
            }

            return false;
        }

        protected void OnIgnoreItemAdded(string word)
        {
            if (IgnoreItemAdded != null)
                IgnoreItemAdded(this, new IgnoreItemChangeEventArgs(){IgnoreItem = word});
        }

        protected void OnIgnoreItemRemoved(string word)
        {
            if (IgnoreItemRemoved != null)
                IgnoreItemRemoved(this, new IgnoreItemChangeEventArgs() { IgnoreItem = word });
        }
    }

    ///<summary>
    /// Tools for dictionary
    ///</summary>
    public class DictionaryTools
    {
        //private Dictionary<string, int> m_entry = new Dictionary<string, int>();
        private readonly SortedDictionary<string, int> m_entry = new SortedDictionary<string, int>();

        ///<summary>
        /// Load a dictionary
        ///</summary>
        ///<param name="dictionaryFileName">Dictionary path</param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public bool LoadDic(string dictionaryFileName)
        {
            try
            {
                if (dictionaryFileName.Length == 0)
                {
                    return false;
                }

                if (!File.Exists(dictionaryFileName))
                {
                    return false;
                }

                using (StreamReader reader = new StreamReader(dictionaryFileName))
                {
                    int tmpLen = 0;
                    //int maxLen = 0;
                    //string tmpWord = "";

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(' ', '\t');

                        //words[0] = StringUtil.ExtractPersianWordsStandardized(words[0])[0];
                        if (words.Length == 2)
                        {
                            tmpLen = words[0].Length;
                            if (tmpLen > 0 && words[1].Length > 0)
                            {
                                int res;
                                if (int.TryParse(words[1], out res))
                                {
                                    if (!this.m_entry.ContainsKey(words[0]))
                                    {
                                        //if (tmpLen > maxLen)
                                        //{
                                        //    maxLen = tmpLen;
                                        //    tmpWord = words[0];
                                        //}
                                        // word does not exist
                                        this.m_entry.Add(words[0], res);
                                    }
                                    else
                                    {
                                        this.m_entry[words[0]] = Math.Max(this.m_entry[words[0]], res);
                                    }
                                }
                            }
                        }
                    }

                    reader.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /*
        public bool LoadDic3(string dictionaryFileName)
        {
            try
            {
                if (dictionaryFileName.Length == 0)
                {
                    return false;
                }

                if (!File.Exists(dictionaryFileName))
                {
                    return false;
                }

                using (StreamReader reader = new StreamReader(dictionaryFileName))
                {
                    //int maxLen = 0;
                    //string tmpWord = "";

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(' ', '\t');

                        //words[0] = StringUtil.ExtractPersianWordsStandardized(words[0])[0];
                        if (words.Length == 1)
                        {
                            if (!this.m_entry.ContainsKey(words[0]))
                            {
                                this.m_entry.Add(words[0], 1);
                            }
                            else
                            {
                                this.m_entry[words[0]]++;
                            }
                        }
                    }

                    reader.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */

        ///<summary>
        /// Load a dictionary into given data structure
        ///</summary>
        ///<param name="dictionaryFileName">Dictionary path</param>
        ///<param name="entry">Dictionary data structure</param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public bool LoadDic(string dictionaryFileName, Dictionary<string, int> entry)
        {
            try
            {
                if (dictionaryFileName.Length == 0)
                {
                    return false;
                }

                if (!File.Exists(dictionaryFileName))
                {
                    return false;
                }

                using (StreamReader reader = new StreamReader(dictionaryFileName))
                {
                    int tmpLen = 0;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(' ', '\t');

                        if (words.Length == 2)
                        {
                            tmpLen = words[0].Length;
                            if (tmpLen > 0 && words[1].Length > 0)
                            {
                                int res;
                                if (int.TryParse(words[1], out res))
                                {
                                    if (!entry.ContainsKey(words[0]))
                                    {
                                        entry.Add(words[0], res);
                                    }
                                    else
                                    {
                                        entry[words[0]] = Math.Max(entry[words[0]] + 1, res);
                                    }
                                }
                            }
                        }
                    }

                    reader.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        /// Load a dictionary into given data structure considering affix combination
        ///</summary>
        ///<param name="dictionaryFileName">Dictionary path</param>
        ///<param name="entry">Dictionary data structure</param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public bool LoadDic(string dictionaryFileName, Dictionary<string, int[]> entry)
        {
            try
            {
                if (dictionaryFileName.Length == 0)
                {
                    return false;
                }

                if (!File.Exists(dictionaryFileName))
                {
                    return false;
                }

                using (StreamReader reader = new StreamReader(dictionaryFileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(' ', '\t');

                        if (words.Length == 3)
                        {
                            if (words[0].Length > 0 && words[1].Length > 0 && words[2].Length > 0)
                            {
                                int count;
                                if (int.TryParse(words[1], out count))
                                {
                                    int containAffix;
                                    if (int.TryParse(words[2], out containAffix))
                                    {
                                        List<int> attr = new List<int>();
                                        if (!entry.ContainsKey(words[0]))
                                        {
                                            attr.Add(count);
                                            attr.Add(containAffix);
                                            
                                            entry.Add(words[0], attr.ToArray());
                                        }
                                        else
                                        {
                                            attr.Add(Math.Max(entry[words[0]][0] + 1, count));
                                            attr.Add(containAffix);

                                            entry[words[0]] = attr.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    reader.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        ///<summary>
        /// Genrate a dictionary (language model) from a text corpus
        ///</summary>
        ///<param name="corpusFileName">Courpus path</param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public bool GenerateLanguageModel(string corpusFileName)
        {
            try
            {
                if (corpusFileName.Length == 0)
                {
                    return false;
                }

                StreamReader reader = new StreamReader(corpusFileName);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] words = line.Split(PersianAlphabets.Delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    //string[] words = StringUtil.ExtractPersianWordsStandardized(line);

                    foreach (string word in words)
                    {
                        //if (word.Length > 0)
                        //{
                        if (!this.m_entry.ContainsKey(word))
                        {
                            // word does not exist
                            this.m_entry.Add(word, 1);
                        }
                        else
                        {
                            // word exist, add the count of word
                            this.m_entry[word]++;
                        }
                        //}
                    }
                }

                reader.Close();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ///<summary>
        /// Dump dictionary to disk
        ///</summary>
        ///<param name="fileName">File path</param>
        ///<param name="count">Word with smaller usage frequency does not dumped</param>
        ///<param name="length">Word with smaller length does not dumped</param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public bool DumpDic(string fileName, int count, int length)
        {
            try
            {
                if (fileName.Length > 0)
                {
                    string refineWord = "";
                    StreamWriter writer = new StreamWriter(fileName);
                    foreach (KeyValuePair<string, int> pair in this.m_entry)
                    {
                        if (pair.Key.Length > length)
                        {
                            if (pair.Value > count)
                            {
                                refineWord = StringUtil.RefineAndFilterPersianWord(pair.Key);
                                //refineWord = pair.Key;
                                writer.WriteLine(refineWord + " " + pair.Value.ToString());
                            }
                            else
                            {
                                Debug.WriteLine(pair.Key);
                            }
                        }
                        else
                        {
                            Debug.WriteLine(pair.Key);
                        }
                    }
                    writer.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        
        /*
        public bool ExtractDic(string fileName, int count, int length)
        {
            try
            {
                if (fileName.Length > 0)
                {
                    string refineWord = "";
                    StreamWriter writer = new StreamWriter(fileName);
                    foreach (KeyValuePair<string, int> pair in this.m_entry)
                    {
                        if (pair.Key.Length <= length)
                        {
                            if (pair.Value <= count)
                            {
                                refineWord = StringUtil.RefineAndFilterPersianWord(pair.Key);
                                //refineWord = pair.Key;
                                writer.WriteLine(refineWord + " " + pair.Value.ToString());
                            }
                        }
                    }
                    writer.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DumpRelaxDic(string fileName)
        {
            try
            {
                if (fileName.Length > 0)
                {
                    Dictionary<string, int> myDic = new Dictionary<string, int>();

                    //PersianSuffixRecognizer PersianSuffixRecognizer = new PersianSuffixRecognizer(false);
                    //ReversePatternMatcherPatternInfo[] affixeResults = null;
                    //string wordWithoutAffix = "";

                    //bool flag = false;
                    //string tmpword = "";

                    foreach (KeyValuePair<string, int> pair in this.m_entry)
                    {
                        if (pair.Key.Contains("می‌"))
                        {
                            if (myDic.ContainsKey(pair.Key))
                            {
                                myDic[pair.Key]++;
                            }
                            else
                            {
                                myDic.Add(pair.Key, 1);
                            }
                        }
                    }

                    #region Affix-Stripper
                    //wordWithoutAffix = pair.Key;

                    //affixeResults = PersianSuffixRecognizer.MatchForAffix(wordWithoutAffix);
                    //if (affixeResults.Length == 0)
                    //{
                    //    if (myDic.ContainsKey(wordWithoutAffix))
                    //    {
                    //        myDic[wordWithoutAffix]++;
                    //    }
                    //    else
                    //    {
                    //        myDic.Add(wordWithoutAffix, 1);
                    //    }

                    //    continue;
                    //}

                    //flag = false;

                    //foreach (ReversePatternMatcherPatternInfo rpm in affixeResults)
                    //{
                    //    tmpword = rpm.BaseWord;

                    //    if (this.m_entry.ContainsKey(tmpword))
                    //    {
                    //        if (myDic.ContainsKey(tmpword))
                    //        {
                    //            myDic[tmpword]++;
                    //        }
                    //        else
                    //        {
                    //            myDic.Add(tmpword, 1);
                    //        }

                    //        flag = true;
                    //        break;
                    //    }

                    //if (!flag)
                    //{
                    //    if (myDic.ContainsKey(wordWithoutAffix))
                    //    {
                    //        myDic[wordWithoutAffix]++;
                    //    }
                    //    else
                    //    {
                    //        myDic.Add(wordWithoutAffix, 1);
                    //    }
                    //}
                    #endregion

                    Dictionary<string, int> mibasedic = new Dictionary<string, int>();
                    Dictionary<string, int> mipsdic = new Dictionary<string, int>();
                    string meStriper = "";
                    foreach (KeyValuePair<string, int> pair in myDic)
                    {
                        this.m_entry.Remove(pair.Key);

                        //meStriper = pair.Key.Remove(0, 2);
                        //meStriper = StringUtil.RefineAndFilterPersianWord(meStriper);

                        //if (meStriper.Length == 0)
                        //{
                        //    continue;
                        //}

                        //if (!this.m_entry.ContainsKey(meStriper))
                        //{
                        //    if (mibasedic.ContainsKey(pair.Key))
                        //    {
                        //        mibasedic[pair.Key]++;
                        //    }
                        //    else
                        //    {
                        //        mibasedic.Add(pair.Key, pair.Value);
                        //    }
                        //}
                        //else
                        //{
                        //    if (mipsdic.ContainsKey("می‌" + meStriper))
                        //    {
                        //        mipsdic["می‌" + meStriper]++;
                        //    }
                        //    else
                        //    {
                        //        mipsdic.Add("می‌" + meStriper, pair.Value);
                        //    }
                        //}
                    }

                    StreamWriter writer = new StreamWriter(fileName + ".mibaseps");
                    foreach (KeyValuePair<string, int> pair in myDic)
                    {
                        writer.WriteLine(pair.Key + " " + pair.Value.ToString());
                    }
                    writer.Close();

                    StreamWriter writer2 = new StreamWriter(fileName + ".mibasewps");
                    foreach (KeyValuePair<string, int> pair in m_entry)
                    {
                        writer2.WriteLine(pair.Key + " " + pair.Value.ToString());
                    }
                    writer2.Close();

                    //StreamWriter writer3 = new StreamWriter(fileName + ".wmi");
                    //foreach (KeyValuePair<string, int> pair in this.m_entry)
                    //{
                    //    writer3.WriteLine(pair.Key + " " + pair.Value.ToString());
                    //}
                    //writer3.Close();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */

        private bool DumpSortedDicByCount(string fileName)
        {
            try
            {
                if (fileName.Length > 0)
                {
                    Dictionary<string, int> sortedDic = SortDicByCount();

                    StreamWriter writer = new StreamWriter(fileName);
                    foreach (KeyValuePair<string, int> pair in sortedDic)
                    {
                        writer.WriteLine(pair.Key + " " + pair.Value.ToString());
                    }
                    writer.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Dictionary<string, int> SortDicByCount()
        {
            if (this.m_entry.Count > 0)
            {
                List<string> myList = new List<string>();
                foreach (KeyValuePair<string, int> pair in this.m_entry)
                {
                    myList.Add(pair.Key);
                }

                string temp;
                for (int i = 0; i < myList.Count - 1; ++i)
                {
                    for (int j = i + 1; j < myList.Count; ++j)
                    {
                        if (this.m_entry[myList[i]] < this.m_entry[myList[j]])
                        {
                            temp = myList[i];
                            myList[i] = myList[j];
                            myList[j] = temp;
                        }
                    }
                }

                Dictionary<string, int> tmpDic = new Dictionary<string, int>();
                foreach (string str in myList)
                {
                    tmpDic.Add(str, this.m_entry[str]);
                }

                return tmpDic;
            }

            return new Dictionary<string, int>();
        }

        ///<summary>
        /// Append another dictionary
        ///</summary>
        ///<param name="dictionaryFileName">Dictionary path</param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public bool AppendDictionary(string dictionaryFileName)
        {
            try
            {
                if (dictionaryFileName.Length == 0)
                {
                    return false;
                }

                if (!File.Exists(dictionaryFileName))
                {
                    return false;
                }

                using (StreamReader reader = new StreamReader(dictionaryFileName))
                {
                    int tmpLen = 0;
                    //int maxLen = 0;
                    //string tmpWord = "";

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(' ', '\t');

                        //words[0] = StringUtil.ExtractPersianWordsStandardized(words[0])[0];
                        if (words.Length == 2)
                        {
                            tmpLen = words[0].Length;
                            if (tmpLen > 0 && words[1].Length > 0)
                            {
                                int res;
                                if (int.TryParse(words[1], out res))
                                {
                                    if (!this.m_entry.ContainsKey(words[0]))
                                    {
                                        //if (tmpLen > maxLen)
                                        //{
                                        //    maxLen = tmpLen;
                                        //    tmpWord = words[0];
                                        //}
                                        // word does not exist
                                        this.m_entry.Add(words[0], res);
                                    }
                                    else
                                    {
                                        this.m_entry[words[0]] = Math.Max(this.m_entry[words[0]], res);
                                    }
                                }
                            }
                        }
                    }

                    reader.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    ///<summary>
    /// Generate a dictionary and freqency of usage of each word from text corpus
    ///</summary>
    public class LanguageModel
    {
        private readonly Dictionary<string, int> m_entry = new Dictionary<string, int>();
        //private SortedDictionary<string, int> m_entry = new SortedDictionary<string, int>();

        ///<summary>
        /// Add a word
        ///</summary>
        ///<param name="word">Word</param>
        public void AddWord(string word)
        {
            if (!this.m_entry.ContainsKey(word))
            {
                // word does not exist
                this.m_entry.Add(word, 1);
            }
            else
            {
                // word exist, add the count of word
                this.m_entry[word]++;
            }
        }
        ///<summary>
        /// Add word with usage frequency
        ///</summary>
        ///<param name="word">Word</param>
        ///<param name="freq">Usage frequency</param>
        public void AddWord(string word, int freq)
        {
            if (!this.m_entry.ContainsKey(word))
            {
                // word does not exist
                this.m_entry.Add(word, freq);
            }
            else
            {
                // word exist, add the count of word
                this.m_entry[word] = freq;
            }
        }
        ///<summary>
        /// Add alist of word
        ///</summary>
        ///<param name="wordList">List of word</param>
        public void AddWord(string[] wordList)
        {
            foreach (string wrd in wordList)
            {
                if (!this.m_entry.ContainsKey(wrd))
                {
                    // word does not exist
                    this.m_entry.Add(wrd, 1);
                }
                else
                {
                    // word exist, add the count of word
                    this.m_entry[wrd]++;
                }
            }
        }
        ///<summary>
        /// Add a text corpus
        ///</summary>
        ///<param name="text">Text string</param>
        public void AddPlainText(string text)
        {
            string[] words = StringUtil.ExtractPersianWordsStandardized(text);

            foreach (string word in words)
            {
                if (!this.m_entry.ContainsKey(word))
                {
                    // word does not exist
                    this.m_entry.Add(word, 1);
                }
                else
                {
                    // word exist, add the count of word
                    this.m_entry[word]++;
                }
            }
        }
        ///<summary>
        /// Save dictionary to disk
        ///</summary>
        ///<param name="fileName">Absolute path of file</param>
        ///<exception cref="Exception"></exception>
        public void SaveOnDisk(string fileName)
        {
            try
            {
                if (fileName.Length > 0)
                {
                    StreamWriter writer = new StreamWriter(fileName);
                    foreach (KeyValuePair<string, int> pair in this.m_entry)
                    {
                        writer.WriteLine(pair.Key + " " + pair.Value.ToString());
                    }
                    writer.Close();
                }
                else
                {
                    throw new Exception("File name is not valid!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ///<summary>
        /// Save dictionary to disk
        ///</summary>
        ///<param name="fileName">Absolute path of file</param>
        ///<param name="append">Append dictionary to existing file</param>
        ///<exception cref="Exception"></exception>
        public void SaveOnDisk(string fileName, bool append)
        {
            try
            {
                if (fileName.Length > 0)
                {
                    if (append)
                    {
                        new DictionaryTools().LoadDic(fileName, this.m_entry); 
                    }

                    StreamWriter writer = new StreamWriter(fileName);
                    foreach (KeyValuePair<string, int> pair in this.m_entry)
                    {
                        writer.WriteLine(pair.Key + " " + pair.Value.ToString());
                    }
                    writer.Close();
                }
                else
                {
                    throw new Exception("File name is not valid!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    ///<summary>
    /// Log user's activity and sort by frequent of usage in each session
    ///</summary>
    public class SessionLogger
    {
        //private Dictionary<string, int> m_entry = new Dictionary<string, int>();
        private readonly List<string> m_entry = new List<string>();

        ///<summary>
        /// Add a usage log
        ///</summary>
        ///<param name="word">Word</param>
        public void AddUsage(string word)
        {
            if (!this.m_entry.Contains(word))
            {
                this.m_entry.Add(word);
            }
            else
            {
                this.m_entry.Remove(word);
                this.m_entry.Add(word);
            }
        }

        ///<summary>
        /// Sort a list of word by usage frequency
        ///</summary>
        ///<param name="words">Word</param>
        ///<returns>Sorted List</returns>
        public string[] Sort(string[] words)
        {
            string[] existingWords = ExtractModestUsedWordsFromList(ref words);

            List<string> finalList = new List<string>(existingWords);
            finalList.AddRange(words);

            return finalList.ToArray();
        }

        private string[] ExtractModestUsedWordsFromList(ref string[] words)
        {
            List<string> wordList = new List<string>(words);
            List<string> existingWords = new List<string>();
            
            //foreach (KeyValuePair<string, int> pair in this.m_entry)
            foreach (string str in this.m_entry)
            {
                if (wordList.Contains(str))
                {
                    existingWords.Add(str);
                    wordList.Remove(str);
                }
            }

            words = wordList.ToArray();
            existingWords.Reverse();
            return existingWords.ToArray();
        }
        
    }
}


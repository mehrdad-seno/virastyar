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
using SCICT.NLP.Persian.Constants;

namespace SCICT.NLP.Utility.WordContainer
{
    ///<summary>
    /// A data structure for efficient management of words with auto complete feature, This structure is a character level tree.
    ///</summary>
    public class AutoCompleteWordContainerTree : WordFreqPOSContainerTree
    {
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="wordContainerConfig">Configuration</param>
        public AutoCompleteWordContainerTree(WordContainerTreeConfig wordContainerConfig) :
            base(wordContainerConfig) { }

        ///<summary>
        /// Complete the rest of incomplete word
        ///</summary>
        /// <param name="subWord">Incomplete word</param>
        /// <returns>Completed words start with incomplete word</returns>
        public string[] Complete(string subWord)
        {
            NodeWithFreqandPOS node = IndexOf(subWord);
            if (node == null)
            {
                return new string[0];
            }

            string[] restOfWord = TraverseFrom(node);

            List<string> completeWords = new List<string>();
            //StringBuilder sb = new StringBuilder(subWord.Substring(0, subWord.Length - 1));
            //int start = subWord.Length - 1;
            string origstr = subWord.Substring(0, subWord.Length - 1);
            foreach (string str in restOfWord)
            {
                //sb.Append(str);
                //completeWords.Add(sb.ToString());
                completeWords.Add(origstr + str);
                //sb.Remove(start, str.Length);
            }

            return completeWords.ToArray();
        }

        ///<summary>
        /// Complete the rest of incomplete word
        ///</summary>
        /// <param name="subWord">Incomplete word</param>
        ///<param name="count">Number of returned suggestions</param>
        /// <returns>Completed words start with incomplete word</returns>
        public string[] Complete(string subWord, int count)
        {
            NodeWithFreqandPOS node = IndexOf(subWord);
            if (node == null)
            {
                return new string[0];
            }

            string[] restOfWord = TraverseFrom(node);

            string origstr = subWord.Substring(0, subWord.Length - 1);

            List<string> completeWords = new List<string>();
            
            for(int i = 0; i < Math.Min(count, restOfWord.Length); ++i)
            {
                completeWords.Add(origstr + restOfWord[i]);
            }

            return completeWords.ToArray();
        }

        ///<summary>
        /// Complete the rest of incomplete word considering PseudoSpase after current part
        ///</summary>
        /// <param name="subWord">Incomplete word</param>
        /// <returns>Completed words start with incomplete word</returns>
        public string[] CompleteWithPseudoSpase(string subWord)
        {
            string[] baseList = Complete(subWord);
            string[] pseudoList = Complete(subWord + PseudoSpace.ZWNJ);

            List<string> finalList = new List<string>();
            finalList.AddRange(pseudoList);
            foreach (string str in baseList)
            {
                if (!finalList.Contains(str))
                {
                    finalList.Add(str);
                }
            }
            
            return finalList.ToArray();
        }

        /// <summary>
        /// Retrive all existing words
        /// </summary>
        /// <returns>All words</returns>
        public string[] GetAllWords()
        {
            return TraverseTree(TreeTraveralType.DFS);
        }

        ///<summary>
        /// Save Loaded Dictionaryt to File
        ///</summary>
        ///<param name="fileName">File name</param>
        ///<returns></returns>
        public void SaveDictionaryToFile(string fileName)
        {
            string[] allWord = GetAllWords();

            int freq;
            PersianPOSTag POS;
            foreach (string str in allWord)
            {
                if (base.Contain(str, out freq, out POS))
                {
                    AddWordToFile(str, freq, POS.ToString(), fileName);
                }
            }
        }
    }
}

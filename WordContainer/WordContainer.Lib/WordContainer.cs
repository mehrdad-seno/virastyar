// Author: Omid Kashefi 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi at 2010-March-08
//


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

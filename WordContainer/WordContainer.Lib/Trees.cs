// Author: Omid Kashefi 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi at 2010-March-08
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SCICT.NLP.Persian.Constants;
using SCICT.Utility.IO;
using SCICT.Utility;

namespace SCICT.NLP.Utility.WordContainer
{

    ///<summary>
    /// Tree Traversal Method
    ///</summary>
    public enum TreeTraveralType
    {
        ///<summary>
        /// Depth First Search
        ///</summary>
        DFS = 1,
        ///<summary>
        /// Breadth First Search
        ///</summary>
        BFS = DFS + 1
    }

    ///<summary>
    /// Word container tree data structure's configuration
    ///</summary>
    public class WordContainerTreeConfig
    {
        ///<summary>
        /// The absolute path of dictionary
        ///</summary>
        public string DictionaryFileName;

        /// <summary>
        /// Number of returned words, 0 for all
        /// </summary>
        public long SuggestionCount;
    }


    ///<summary>
    /// A data structure for efficient management of words, This structure is a character level tree.
    ///</summary>
    public class WordContainerTree
    {
        #region Members

        protected string m_dictionaryFileName;
        protected long m_wordsCount;

        private readonly DictionaryWordLoader m_dicW = new DictionaryWordLoader();

        private readonly Node m_root;

        #endregion

        #region Constructor

        ///<summary>
        /// Class constructor
        ///</summary>
        public WordContainerTree()
        {
            this.m_root = new Node('*');
        }

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="wordContainerConfig">Configuration</param>
        ///<exception cref="Exception"></exception>
        public WordContainerTree(WordContainerTreeConfig wordContainerConfig) : this()
        {
            this.m_dictionaryFileName = wordContainerConfig.DictionaryFileName;

            if (!this.Load())
            {
                throw new Exception("Could not load dictionary file!");
            }
        }

        #endregion

        #region Private Methods

        private void AddWordToMemory(string word)
        {
            Node curNode = this.m_root;
            int i = 0, length = word.Length;
            foreach (char c in word)
            {
                if (++i == length)
                {
                    curNode = curNode.AddLink(GenerateNode(c, true));

                    //add nubmer of dictionary words
                    ++this.m_wordsCount;
                }
                else
                {
                    curNode = curNode.AddLink(GenerateNode(c, false));
                }
            }
        }

        private void AddWordToFile(string word)
        {
            m_dicW.AddTerm(word);
        }

        private void RemoveFromFile(string word)
        {
            try
            {
                FileStream fstream = new FileStream(this.m_dictionaryFileName, FileMode.Open, FileAccess.ReadWrite);

                long offset = FileTools.GetWordStartPositionInFile(fstream, word);

                if (offset == -1)
                {
                    //throw new Exception("Word was in the dictionary, but not in the file.");
                    return;
                }

                FileTools.RemoveLineFromPosition(fstream, offset);

                fstream.Close();
                fstream.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RemoveFromMemory(string word)
        {
            Node leaf = IndexOf(word);
            if (leaf != null)
            {
                leaf.LogicalRemove();
            }
        }

        /// <summary>
        /// Teaverse Tree on Depth First Type
        /// </summary>
        /// <returns></returns>
        private static void Clear(Node node)
        {
            foreach (Node curNode in node.GetLinks())
            {
                Clear(curNode);
            }

            node.Clear();
        }

        /// <summary>
        /// Teaverse Tree on Depth First Type
        /// </summary>
        /// <returns></returns>
        private string[] DoDFSTraverse(Node node)
        {
            if (node.HaveLinks())
            {
                List<string> retDFS = new List<string>();
                foreach (Node curNode in node.GetLinks())
                {
                    retDFS.AddRange(DoDFSTraverse(curNode));
                }

                List<string> suggestions = new List<string>();
                if (node.Value != this.m_root.Value)
                {
                    if (node.IsEndOfWord)
                    {
                        suggestions.Add(new StringBuilder().Append(node.Value).ToString());
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (string str in retDFS)
                    {
                        sb.Append(str);
                        sb.Append(node.Value);
                        suggestions.Add(sb.ToString());
                        sb.Remove(0, sb.Length);
                    }

                    return suggestions.ToArray();
                }
                else
                {
                    return retDFS.ToArray();
                }
            }

            if (node.IsEndOfWord)
            {
                return new string[] { new StringBuilder().Append(node.Value).ToString() };
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Teaverse Tree on Breath First Type
        /// </summary>
        /// <returns></returns>
        private string[] DoBFSTraverse(Node node)
        {
            if (node.HaveLinks())
            {
                List<string> suggestions = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (Node curNode in node.GetLinks())
                {
                    sb.Append(DoDFSTraverse(curNode));
                    suggestions.Add(sb.ToString());
                    sb.Remove(0, sb.Length);
                }

                return suggestions.ToArray();
            }

            return new string[] { node.Value.ToString() };
        }

        /// <summary>
        /// Utility Function, Generate Node from Letter
        /// </summary>
        /// <param name="c">Letter</param>
        /// <param name="isEndLetter"></param>
        /// <returns></returns>
        private static Node GenerateNode(char c, bool isEndLetter)
        {
            return new Node(c, isEndLetter);
        }

        private bool Load()
        {
            if (!m_dicW.LoadFile(m_dictionaryFileName))
            {
                return false;
            }

            string word;
            while (!m_dicW.EndOfStream)
            {
                m_dicW.NextTerm(out word);

                word = word.Normalize(NormalizationForm.FormC);

                this.AddWordToMemory(word);
            }

            return true;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word)
        {
            Node leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        public bool AddWord(string word)
        {
            try
            {
                if (this.Contain(word))
                {
                    return false;
                }

                AddWordToMemory(word);
                AddWordToFile(word);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        public bool AddWordBlind(string word)
        {
            try
            {
                AddWordToMemory(word);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Remove word from Dictinary
        /// </summary>
        /// <param name="word">Word</param>
        public void RemoveWord(string word)
        {
            try
            {
                RemoveFromFile(word);
                RemoveFromMemory(word);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        /// Append a new dictionary
        ///</summary>
        ///<param name="fileName">Absolute path of dictionary</param>
        ///<returns>True if successfully added</returns>
        ///<exception cref="Exception"></exception>
        public bool AppendDictionary(string fileName)
        {
            if (!m_dicW.LoadFile(fileName))
            {
                return false;
            }

            string word;
            while (!m_dicW.EndOfStream)
            {
                m_dicW.NextTerm(out word);
                this.AddWordToMemory(word);
            }

            return true;
        }

        ///<summary>
        /// Clear all words
        ///</summary>
        public void Clear()
        {
            Clear(this.m_root);
        }

        ///<summary>
        /// Number of existing words
        ///</summary>
        public long DictionaryWordsCount
        {
            get
            {
                return this.m_wordsCount;
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// Utility Function, Reverse a word
        /// </summary>
        /// <returns></returns>
        protected string ReverseString(string word)
        {
            StringBuilder sb = new StringBuilder();
            char[] letters = word.ToCharArray();

            for (int i = letters.Length - 1; i >= 0; --i)
            {
                sb.Append(letters[i]);
            }

            return sb.ToString();
        }

        protected Node IndexOf(string subWord)
        {
            Node curNode = this.m_root;
            foreach (char c in subWord)
            {
                if (curNode != null)
                {
                    curNode = curNode.GetNextNode(c);
                }
            }

            if (curNode == this.m_root)
            {
                curNode = null;
            }

            return curNode;
        }

        protected string[] TraverseFrom(Node node)
        {
            string[] words = DoDFSTraverse(node);
            List<string> strList = new List<string>();
            foreach (string str in words)
            {
                strList.Add(ReverseString(str));
            }
            return strList.ToArray();
        }

        /// <summary>
        /// Traverse tree to retrieve all words
        /// </summary>
        /// <param name="traversType">Traversal Type</param>
        /// <returns>All Words</returns>
        protected string[] TraverseTree(TreeTraveralType traversType)
        {
            if (traversType == TreeTraveralType.BFS)
            {
                return DoBFSTraverse(this.m_root);
            }
            else if (traversType == TreeTraveralType.DFS)
            {
                string[] words = DoDFSTraverse(this.m_root);
                List<string> strList = new List<string>();
                foreach (string str in words)
                {
                    strList.Add(ReverseString(str));
                }
                return strList.ToArray();
            }

            return new string[0];
        }

        #endregion
    }

    ///<summary>
    /// A data structure for efficient management of words, This structure is a character level tree.
    ///</summary>
    public class WordFreqContainerTree
    {
        #region Members

        protected string m_dictionaryFileName;
        protected long m_wordsCount;

        private readonly DictionaryWordFreqLoader m_dicWF = new DictionaryWordFreqLoader();

        private readonly NodeWithFreq m_root;

        private long m_freqSummation;
        private long m_totalWordsNumber;

        #endregion

        #region Constructor

        ///<summary>
        /// Class Constructor
        ///</summary>
        public WordFreqContainerTree()
        {
            m_root = new NodeWithFreq('*', false, 0);
        }

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="wordContainerConfig">Configuration</param>
        ///<exception cref="Exception"></exception>
        public WordFreqContainerTree(WordContainerTreeConfig wordContainerConfig) : this()
        {
            m_dictionaryFileName = wordContainerConfig.DictionaryFileName;

            if (!this.Load())
            {
                throw new Exception("Could not load dictionary file!");
            }

        }

        #endregion

        #region Private Methods
        
        private void AddWordToMemory(string word, int freq)
        {
            NodeWithFreq curNode = this.m_root;
            int i = 0, length = word.Length;
            foreach (char c in word)
            {
                if (++i == length)
                {
                    curNode = curNode.AddLink(GenerateNode(c, freq, true));

                    //add number of dictionary words
                    this.m_freqSummation += freq;
                    this.m_totalWordsNumber++;

                    ++this.m_wordsCount;
                }
                else
                {
                    curNode = curNode.AddLink(GenerateNode(c, 0, false));
                }
            }
        }

        private void AddWordToFile(string word, int freq)
        {
            m_dicWF.AddTerm(word, freq);
        }

        private void RemoveFromMemory(string word)
        {
            NodeWithFreq leaf = IndexOf(word);
            if (leaf != null)
            {
                leaf.LogicalRemove();
            }
        }

        private void RemoveFromFile(string word)
        {
            try
            {
                FileStream fstream = new FileStream(this.m_dictionaryFileName, FileMode.Open, FileAccess.ReadWrite);

                long offset = FileTools.GetWordStartPositionInFile(fstream, word);

                if (offset == -1)
                {
                    //throw new Exception("Word was in the dictionary, but not in the file.");
                    return;
                }

                FileTools.RemoveLineFromPosition(fstream, offset);

                fstream.Close();
                fstream.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Load()
        {
            if (!m_dicWF.LoadFile(m_dictionaryFileName))
            {
                return false;
            }

            string word;
            int freq;
            while (!m_dicWF.EndOfStream)
            {
                m_dicWF.NextTerm(out word, out freq);

                word = word.Normalize(NormalizationForm.FormC);

                this.AddWordToMemory(word, freq);
            }

            return true;
        }

        private static void Clear(NodeWithFreq node)
        {
            foreach (NodeWithFreq curNode in node.GetLinks())
            {
                Clear(curNode);
            }

            node.Clear();
        }

        /// <summary>
        /// Teaverse Tree on Depth First Type
        /// </summary>
        /// <returns></returns>
        private string[] DoDFSTraverse(NodeWithFreq node)
        {
            if (node.HaveLinks())
            {
                List<string> retDFS = new List<string>();
                foreach (NodeWithFreq curNode in node.GetLinks())
                {
                    retDFS.AddRange(DoDFSTraverse(curNode));
                }

                List<string> suggestions = new List<string>();
                if (node.Value != this.m_root.Value)
                {
                    if (node.IsEndOfWord)
                    {
                        suggestions.Add(new StringBuilder().Append(node.Value).ToString());
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (string str in retDFS)
                    {
                        sb.Append(str);
                        sb.Append(node.Value);
                        suggestions.Add(sb.ToString());
                        sb.Remove(0, sb.Length);
                    }

                    return suggestions.ToArray();
                }
                else
                {
                    return retDFS.ToArray();
                }
            }

            if (node.IsEndOfWord)
            {
                return new string[] { new StringBuilder().Append(node.Value).ToString() };
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Teaverse Tree on Breath First Type
        /// </summary>
        /// <returns></returns>
        private string[] DoBFSTraverse(NodeWithFreq node)
        {
            if (node.HaveLinks())
            {
                List<string> suggestions = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (NodeWithFreq curNode in node.GetLinks())
                {
                    sb.Append(DoDFSTraverse(curNode));
                    suggestions.Add(sb.ToString());
                    sb.Remove(0, sb.Length);
                }

                return suggestions.ToArray();
            }

            return new string[] { node.Value.ToString() };
        }

        /// <summary>
        /// Utility Function, Generate Node from Letter
        /// </summary>
        /// <param name="c">Letter</param>
        /// <param name="freq">Usage frequency</param>
        /// <param name="isEndLetter">end of word</param>
        /// <returns></returns>
        private static NodeWithFreq GenerateNode(char c, int freq, bool isEndLetter)
        {
            return new NodeWithFreq(c, isEndLetter, freq);
        }


        #endregion

        #region Public Members

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word)
        {
            NodeWithFreq leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word, out int freq)
        {
            freq = 0;
            NodeWithFreq leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }
            freq = leaf.WordFrequency;
            return true;
        }

        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        public bool AddWord(string word, int freq)
        {
            try
            {
                if (this.Contain(word))
                {
                    return false;
                }

                AddWordToMemory(word, freq);
                AddWordToFile(word, freq);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        public bool AddWordBlind(string word, int freq)
        {
            try
            {
                AddWordToMemory(word, freq);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Remove a word from dictionary
        /// </summary>
        /// <param name="word">Word</param>
        public void RemoveWord(string word)
        {
            try
            {
                RemoveFromFile(word);
                RemoveFromMemory(word);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        /// Append a new dictionary
        ///</summary>
        ///<param name="fileName">Absolute path of dictionary</param>
        ///<returns>True if successfully added</returns>
        ///<exception cref="Exception"></exception>
        public bool AppendDictionary(string fileName)
        {
            if (!m_dicWF.LoadFile(fileName))
            {
                return false;
            }

            string word;
            int freq;
            while (!m_dicWF.EndOfStream)
            {
                m_dicWF.NextTerm(out word, out freq);
                this.AddWordToMemory(word, freq);
            }

            return true;
        }

        /// <summary>
        /// Word frequency
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>usage frequency</returns>
        public int WordFrequency(string word)
        {
            NodeWithFreq leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return 0;
            }

            return leaf.WordFrequency;
        }

        ///<summary>
        /// Total frequency
        ///</summary>
        public long FreqSummation
        {
            get
            {
                return this.m_freqSummation;
            }
        }

        ///<summary>
        /// Total Number of Words
        ///</summary>
        public long TotalNumberofWords
        {
            get
            {
                return this.m_totalWordsNumber;
            }
        }

        ///<summary>
        /// Number of existing words
        ///</summary>
        public long DictionaryWordsCount
        {
            get
            {
                return this.m_wordsCount;
            }
        }

        ///<summary>
        /// Clear all words
        ///</summary>
        public void Clear()
        {
            Clear(this.m_root);
        }
        
        #endregion

        #region Protected

        protected NodeWithFreq IndexOf(string subWord)
        {
            NodeWithFreq curNode = this.m_root;
            foreach (char c in subWord)
            {
                if (curNode != null)
                {
                    curNode = curNode.GetNextNode(c);
                }
            }

            if (curNode == this.m_root)
            {
                curNode = null;
            }

            return curNode;
        }

        protected string[] TraverseFrom(NodeWithFreq node)
        {
            string[] words = DoDFSTraverse(node);
            List<string> strList = new List<string>();
            foreach (string str in words)
            {
                strList.Add(ReverseString(str));
            }
            return strList.ToArray();
        }

        /// <summary>
        /// Traverse Tree to Retrieve All Words
        /// </summary>
        /// <param name="traversType">Traversal Type</param>
        /// <returns>All Words</returns>
        protected string[] TraverseTree(TreeTraveralType traversType)
        {
            if (traversType == TreeTraveralType.BFS)
            {
                return DoBFSTraverse(this.m_root);
            }
            else if (traversType == TreeTraveralType.DFS)
            {
                string[] words = DoDFSTraverse(this.m_root);
                List<string> strList = new List<string>();
                foreach (string str in words)
                {
                    strList.Add(ReverseString(str));
                }
                return strList.ToArray();
            }

            return new string[0];
        }


        /// <summary>
        /// Utility Function, Reverse a word
        /// </summary>
        /// <returns></returns>
        protected string ReverseString(string word)
        {
            StringBuilder sb = new StringBuilder();
            char[] letters = word.ToCharArray();

            for (int i = letters.Length - 1; i >= 0; --i)
            {
                sb.Append(letters[i]);
            }

            return sb.ToString();
        }


        #endregion
    }

    ///<summary>
    /// A data structure for efficient management of words, This structure is a character level tree.
    ///</summary>
    public class WordFreqPOSContainerTree
    {
        #region Members

        protected string m_dictionaryFileName;
        protected long m_wordsCount;

        private readonly DictionaryWordFreqPOSLoader m_dicWFPOS = new DictionaryWordFreqPOSLoader();

        private readonly NodeWithFreqandPOS m_root;

        private long m_freqSummation;

        #endregion

        #region Constructor

        ///<summary>
        /// Class Constructor
        ///</summary>
        public WordFreqPOSContainerTree()
        {
            m_root = new NodeWithFreqandPOS('*', false, 0, PersianPOSTag.UserPOS.ToString());
        }

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="wordContainerConfig">Configuration</param>
        ///<exception cref="Exception"></exception>
        public WordFreqPOSContainerTree(WordContainerTreeConfig wordContainerConfig) : this()
        {
            m_dictionaryFileName = wordContainerConfig.DictionaryFileName;

            if (!this.Load())
            {
                throw new Exception("Could not load dictionary file!");
            }

        }

        #endregion

        #region Private Methods

        private void AddWordToMemory(string word, int freq, string pos)
        {
            NodeWithFreqandPOS curNode = this.m_root;
            int i = 0, length = word.Length;
            foreach (char c in word)
            {
                if (++i == length)
                {
                    curNode = curNode.AddLink(GenerateNode(c, freq, true, pos));

                    //add nubmer of dictionary words
                    this.m_freqSummation += freq;

                    ++this.m_wordsCount;
                }
                else
                {
                    curNode = curNode.AddLink(GenerateNode(c, 0, false, pos));
                }
            }
        }

        private bool AddWordToFile(string word, int freq, string pos)
        {
            return m_dicWFPOS.AddTerm(word, freq, pos);
        }

        protected bool AddWordToFile(string word, int freq, string pos, string fileName)
        {
            return m_dicWFPOS.AddTerm(word, freq, pos, fileName);
        }

        private void RemoveFromMemory(string word)
        {
            NodeWithFreqandPOS leaf = IndexOf(word);
            if (leaf != null)
            {
                leaf.LogicalRemove();
            }
        }

        private void RemoveFromFile(string word)
        {
            try
            {
                using (FileStream fstream = new FileStream(this.m_dictionaryFileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    long offset = FileTools.GetWordStartPositionInFile(fstream, word);

                    if (offset == -1)
                    {
                        //throw new Exception("Word was in the dictionary, but not in the file.");
                        return;
                    }

                    FileTools.RemoveLineFromPosition(fstream, offset);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Load()
        {
            if (!m_dicWFPOS.LoadFile(m_dictionaryFileName))
            {
                return false;
            }

            string word, pos;
            int freq;
            while (!m_dicWFPOS.EndOfStream)
            {
                m_dicWFPOS.NextTerm(out word, out freq, out pos);

                word = word.Normalize(NormalizationForm.FormC);

                this.AddWordToMemory(word, freq, pos);
            }

            m_dicWFPOS.CloseFile();

            return true;
        }

        private static void Clear(NodeWithFreqandPOS node)
        {
            foreach (NodeWithFreqandPOS curNode in node.GetLinks())
            {
                Clear(curNode);
            }

            node.Clear();
        }

        /// <summary>
        /// Teaverse Tree on Depth First Type
        /// </summary>
        /// <returns></returns>
        private string[] DoDFSTraverse(NodeWithFreqandPOS node)
        {
            if (node.HaveLinks())
            {
                List<string> retDFS = new List<string>();
                foreach (NodeWithFreqandPOS curNode in node.GetLinks())
                {
                    retDFS.AddRange(DoDFSTraverse(curNode));
                }

                List<string> suggestions = new List<string>();
                if (node.Value != this.m_root.Value)
                {
                    if (node.IsEndOfWord)
                    {
                        suggestions.Add(new StringBuilder().Append(node.Value).ToString());
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (string str in retDFS)
                    {
                        sb.Append(str);
                        sb.Append(node.Value);
                        suggestions.Add(sb.ToString());
                        sb.Remove(0, sb.Length);
                    }

                    return suggestions.ToArray();
                }
                else
                {
                    return retDFS.ToArray();
                }
            }

            if (node.IsEndOfWord)
            {
                return new string[] { new StringBuilder().Append(node.Value).ToString() };
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Teaverse Tree on Breath First Type
        /// </summary>
        /// <returns></returns>
        private string[] DoBFSTraverse(NodeWithFreqandPOS node)
        {
            if (node.HaveLinks())
            {
                List<string> suggestions = new List<string>();
                StringBuilder sb = new StringBuilder();
                foreach (NodeWithFreqandPOS curNode in node.GetLinks())
                {
                    sb.Append(DoDFSTraverse(curNode));
                    suggestions.Add(sb.ToString());
                    sb.Remove(0, sb.Length);
                }

                return suggestions.ToArray();
            }

            return new string[] { node.Value.ToString() };
        }

        /// <summary>
        /// Utility Function, Generate Node from Letter
        /// </summary>
        /// <param name="c">Letter</param>
        /// <param name="freq">Usage frequency</param>
        /// <param name="isEndLetter">end of word</param>
        /// <param name="pos">POS tag of word</param>
        /// <returns></returns>
        private static NodeWithFreqandPOS GenerateNode(char c, int freq, bool isEndLetter, string pos)
        {
            return new NodeWithFreqandPOS(c, isEndLetter, freq, pos);
        }


        #endregion

        #region Public Members

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word)
        {
            NodeWithFreqandPOS leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word, out int freq)
        {
            freq = 0;
            NodeWithFreqandPOS leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }
            freq = leaf.WordFrequency;
            return true;
        }

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="posTag">Word's POS tag</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word, out PersianPOSTag posTag)
        {
            posTag = PersianPOSTag.UserPOS;

            NodeWithFreqandPOS leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }
            
            posTag = leaf.POSTag.ToEnum<PersianPOSTag>();

            return true;
        }

        /// <summary>
        /// Check if a word exists
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        /// <param name="posTag">Word's POS tag</param>
        /// <returns>If the dictionary contains the word, returns true, else returns false.</returns>
        public bool Contain(string word, out int freq, out PersianPOSTag posTag)
        {
            freq = 0;
            posTag = PersianPOSTag.UserPOS;

            NodeWithFreqandPOS leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return false;
            }
            freq = leaf.WordFrequency;
            
            posTag = leaf.POSTag.ToEnum<PersianPOSTag>();

            return true;
        }
        
        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        /// <param name="posTag">Word's pos</param>
        public bool AddWord(string word, int freq, PersianPOSTag posTag)
        {
            try
            {
                int existingFreq;
                PersianPOSTag existingPOS;
                if (this.Contain(word, out existingFreq, out existingPOS))
                {
                    if(existingPOS.Has(posTag) && existingFreq == freq)
                    {
                        return false;
                    }
                    else
                    {
                        RemoveFromFile(word);
                    }
                }

                AddWordToMemory(word, freq, posTag.ToString());
                AddWordToFile(word, freq, posTag.ToString());

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        /// <param name="posTag">Word's pos</param>
        public bool AddWordBlind(string word, int freq, PersianPOSTag posTag)
        {
            try
            {
                AddWordToMemory(word, freq, posTag.ToString());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a word to tree
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="freq">Word's usage frequency</param>
        /// <param name="posTag">Word's pos</param>
        /// <param name="fileName">File name</param>
        public bool AddWord(string word, int freq, PersianPOSTag posTag, string fileName)
        {
            try
            {
                int existingFreq;
                PersianPOSTag existingPOS;
                if (this.Contain(word, out existingFreq, out existingPOS))
                {
                    if (existingPOS.Has(posTag) && existingFreq == freq)
                    {
                        return false;
                    }
                    else
                    {
                        RemoveFromFile(word);
                    }
                }

                AddWordToMemory(word, freq, posTag.ToString());
                return AddWordToFile(word, freq, posTag.ToString(), fileName);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove word from Dictinary
        /// </summary>
        /// <param name="word">Word</param>
        public void RemoveWord(string word)
        {
            try
            {
                RemoveFromFile(word);
                RemoveFromMemory(word);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        /// Append a new dictionary
        ///</summary>
        ///<param name="fileName">Absolute path of dictionary</param>
        ///<returns>True if successfully added</returns>
        ///<exception cref="Exception"></exception>
        public int AppendDictionary(string fileName)
        {
            if (!m_dicWFPOS.LoadFile(fileName))
            {
                return 0;
            }

            string word, pos;
            int freq;
            int extractedWordCount = 0;
            while (!m_dicWFPOS.EndOfStream)
            {
                m_dicWFPOS.NextTerm(out word, out freq, out pos);
                this.AddWordToMemory(word, freq, pos);
                extractedWordCount++;
            }

            return extractedWordCount;
        }

        /// <summary>
        /// Word frequency
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>usage frequency</returns>
        public int WordFrequency(string word)
        {
            NodeWithFreqandPOS leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return 0;
            }

            return leaf.WordFrequency;
        }

        /// <summary>
        /// Word pos
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>POS tag</returns>
        public PersianPOSTag WordPOS(string word)
        {
            NodeWithFreqandPOS leaf = IndexOf(word);

            if (leaf == null || !leaf.IsEndOfWord)
            {
                return 0;
            }

            return leaf.POSTag.ToEnum<PersianPOSTag>();
        }

        ///<summary>
        /// Total frequency
        ///</summary>
        public long FreqSummation
        {
            get
            {
                return this.m_freqSummation;
            }
        }

        ///<summary>
        /// Number of existing words
        ///</summary>
        public long DictionaryWordsCount
        {
            get
            {
                return this.m_wordsCount;
            }
        }
        
        ///<summary>
        /// Clear all words
        ///</summary>
        public void Clear()
        {
            Clear(this.m_root);
        }

        #endregion

        #region Protected

        protected NodeWithFreqandPOS IndexOf(string subWord)
        {
            NodeWithFreqandPOS curNode = this.m_root;
            foreach (char c in subWord)
            {
                if (curNode != null)
                {
                    curNode = curNode.GetNextNode(c);
                }
            }

            if (curNode == this.m_root)
            {
                curNode = null;
            }

            return curNode;
        }

        protected string[] TraverseFrom(NodeWithFreqandPOS node)
        {
            string[] words = DoDFSTraverse(node);
            List<string> strList = new List<string>();
            foreach (string str in words)
            {
                strList.Add(ReverseString(str));
            }
            return strList.ToArray();
        }

        /// <summary>
        /// Traverse Tree to Retrieve All Words
        /// </summary>
        /// <param name="traversType">Traversal Type</param>
        /// <returns>All Words</returns>
        protected string[] TraverseTree(TreeTraveralType traversType)
        {
            if (traversType == TreeTraveralType.BFS)
            {
                return DoBFSTraverse(this.m_root);
            }
            else if (traversType == TreeTraveralType.DFS)
            {
                string[] words = DoDFSTraverse(this.m_root);
                List<string> strList = new List<string>();
                foreach (string str in words)
                {
                    strList.Add(ReverseString(str));
                }
                return strList.ToArray();
            }

            return new string[0];
        }

        /// <summary>
        /// Utility Function, Reverse a word
        /// </summary>
        /// <returns></returns>
        protected string ReverseString(string word)
        {
            StringBuilder sb = new StringBuilder();
            char[] letters = word.ToCharArray();

            for (int i = letters.Length - 1; i >= 0; --i)
            {
                sb.Append(letters[i]);
            }

            return sb.ToString();
        }


        #endregion
    }
}

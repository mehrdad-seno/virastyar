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

using System.Collections.Generic;
using System;
using System.Linq;

namespace SCICT.NLP.Utility.WordContainer
{
    ///<summary>
    /// Simple character node
    ///</summary>
    public class Node
    {
        #region Private Members

        private readonly SortedDictionary<char, Node> m_links = new SortedDictionary<char, Node>();
        protected readonly char m_value;
        protected bool m_isEndLetter;

        #endregion

        #region Constructors

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="letter">character value</param>
        public Node(char letter)
        {
            this.m_value = letter;
        }

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="letter">character value</param>
        ///<param name="isEndLetter">Current letter is the final letter of corresponding word?</param>
        public Node(char letter, bool isEndLetter)
        {
            this.m_value = letter;
            this.m_isEndLetter = isEndLetter;
        }

        #endregion

        #region Public Methods

        ///<summary>
        /// Unicode value of current node
        ///</summary>
        public char Value
        {
            get
            {
                return this.m_value;
            }
        }

        ///<summary>
        /// Number of childs
        ///</summary>
        public int LinkCount
        {
            get
            {
                return this.m_links.Count;
            }
        }

        ///<summary>
        /// Search for having a child
        ///</summary>
        ///<param name="c">child's Unicode value</param>
        ///<returns>True if contain child, otherwise False</returns>
        public bool ContainLetter(char c)
        {
            return this.m_links.ContainsKey(c);
        }

        ///<summary>
        /// Check if current node have any child
        ///</summary>
        ///<returns>True if having any child, otherwise False</returns>
        public bool HaveLinks()
        {
            return this.m_links.Count == 0 ? false : true;
        }

        ///<summary>
        /// Check if current node is final letter of a word
        ///</summary>
        public bool IsEndOfWord
        {
            get
            {
                return this.m_isEndLetter;
            }
        }

        ///<summary>
        /// Remove a child node
        ///</summary>
        ///<param name="node">Child node</param>
        public void RemoveLink(Node node)
        {
            if (this.m_links.ContainsKey(node.m_value))
            {
                this.m_links.Remove(node.m_value);
            }
        }

        ///<summary>
        /// Remove a child by value
        ///</summary>
        ///<param name="c">Child's Unicode value</param>
        public void RemoveLink(char c)
        {
            if (this.m_links.ContainsKey(c))
            {
                this.m_links.Remove(c);
            }
        }

        ///<summary>
        /// Logically remove current word (if current node is final letter of a word)
        ///</summary>
        public void LogicalRemove()
        {
            this.m_isEndLetter = false;
        }

        ///<summary>
        /// Remove current word and all the words that start with this word (if current node is final letter of a word)
        ///</summary>
        public void Clear()
        {
            LogicalRemove();
            this.m_links.Clear();
        }

        ///<summary>
        /// Add a child node
        ///</summary>
        ///<param name="node">Child node</param>
        ///<returns>Node pointer to added child node</returns>
        public Node AddLink(Node node)
        {
            if (!this.m_links.ContainsKey(node.m_value))
            {
                this.m_links.Add(node.m_value, node);
            }
            //if (node.IsEndOfWord && !this.m_links[node.m_value].m_isEndLetter)
            if (node.IsEndOfWord)
            {
                this.m_links[node.m_value].m_isEndLetter = node.IsEndOfWord;
            }

            return this.m_links[node.m_value];
        }

        ///<summary>
        /// Get a pointer to a child node by child value
        ///</summary>
        ///<param name="letter">Child's Unicode value</param>
        ///<returns>Node pointer to child node</returns>
        public Node GetNextNode(char letter)
        {
            if (this.ContainLetter(letter))
            {
                return this.m_links[letter];
            }

            return null;
        }

        ///<summary>
        /// Get all child nodes
        ///</summary>
        ///<returns>Child nodes</returns>
        public Node[] GetLinks()
        {
            List<Node> nodes = new List<Node>();
            foreach (KeyValuePair<char, Node> link in this.m_links)
            {
                nodes.Add(link.Value);
            }

            return nodes.ToArray();
        }

        #endregion
    }

    ///<summary>
    /// Node structure which can store words' usage frequency
    ///</summary>
    public class NodeWithFreq
    {
        #region Private Members

        private readonly SortedDictionary<char, NodeWithFreq> m_links = new SortedDictionary<char, NodeWithFreq>();
        protected int m_freq;
        protected readonly char m_value;
        protected bool m_isEndLetter;

        #endregion

        #region Constructors

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="letter">character value</param>
        ///<param name="isEndLetter">Current letter is the final letter of corresponding word?</param>
        ///<param name="count">Usage frequency of word</param>
        public NodeWithFreq(char letter, bool isEndLetter, int count)
        {
            this.m_value = letter;
            this.m_isEndLetter = isEndLetter;

            this.m_freq = count;
        }

        #endregion

        #region Public Methods

        ///<summary>
        /// Unicode value of current node
        ///</summary>
        public char Value
        {
            get
            {
                return this.m_value;
            }
        }

        ///<summary>
        /// Number of childs
        ///</summary>
        public int LinkCount
        {
            get
            {
                return this.m_links.Count;
            }
        }

        ///<summary>
        /// Search for having a child
        ///</summary>
        ///<param name="c">child's unicode value</param>
        ///<returns>True if contain child, otherwise False</returns>
        public bool ContainLetter(char c)
        {
            return this.m_links.ContainsKey(c);
        }

        ///<summary>
        /// Check if current node have any child
        ///</summary>
        ///<returns>True if having any child, otherwise False</returns>
        public bool HaveLinks()
        {
            return this.m_links.Count == 0 ? false : true;
        }

        ///<summary>
        /// Check if current node is final letter of a word
        ///</summary>
        public bool IsEndOfWord
        {
            get
            {
                return this.m_isEndLetter;
            }
        }


        ///<summary>
        /// Usage frequency of word (if current node is final letter of word)
        ///</summary>
        public int WordFrequency
        {
            get
            {
                return this.m_freq;
            }
        }

        ///<summary>
        /// Add a child node
        ///</summary>
        ///<param name="node">Child node</param>
        ///<returns>Node pointer to added child node</returns>
        public NodeWithFreq AddLink(NodeWithFreq node)
        {
            if (!this.m_links.ContainsKey(node.m_value))
            {
                this.m_links.Add(node.m_value, node);
            }
            else if (node.IsEndOfWord)
            {
                this.m_links[node.m_value].m_isEndLetter = node.IsEndOfWord;
                this.m_links[node.m_value].m_freq = this.m_links[node.m_value].m_freq > node.WordFrequency ? this.m_links[node.m_value].m_freq : node.WordFrequency;
            }

            //if (node.IsEndOfWord && !this.m_links[node.m_value].m_isEndLetter)

            return this.m_links[node.m_value];
        }

        ///<summary>
        /// Get a pointer to a child node by child value
        ///</summary>
        ///<param name="letter">Child's unicode value</param>
        ///<returns>Node pointer to child node</returns>
        public NodeWithFreq GetNextNode(char letter)
        {
            if (this.ContainLetter(letter))
            {
                return this.m_links[letter];
            }

            return null;
        }

        ///<summary>
        /// Get all child nodes
        ///</summary>
        ///<returns>Child nodes</returns>
        public NodeWithFreq[] GetLinks()
        {
            List<NodeWithFreq> nodes = new List<NodeWithFreq>();
            foreach (KeyValuePair<char, NodeWithFreq> link in this.m_links)
            {
                nodes.Add(link.Value);
            }

            return nodes.ToArray();
        }

        ///<summary>
        /// Remove a child node
        ///</summary>
        ///<param name="node">Child node</param>
        public void RemoveLink(NodeWithFreq node)
        {
            if (this.m_links.ContainsKey(node.m_value))
            {
                this.m_links.Remove(node.m_value);
            }
        }

        ///<summary>
        /// Logically remove current word (if current node is final letter of a word)
        ///</summary>
        public void LogicalRemove()
        {
            this.m_isEndLetter = false;
        }

        ///<summary>
        /// Remove current word and all the words that start with this word (if current node is final letter of a word)
        ///</summary>
        public void Clear()
        {
            LogicalRemove();
            this.m_links.Clear();
        }


        #endregion
    }

    ///<summary>
    /// Node structure which can store words' usage frequency and POS tag
    ///</summary>
    public class NodeWithFreqandPOS
    {
        #region Private Members

        private readonly SortedDictionary<char, NodeWithFreqandPOS> m_links = new SortedDictionary<char, NodeWithFreqandPOS>();
        protected readonly char m_value;
        protected bool m_isEndLetter;

        protected int m_freq;
        protected string m_pos;

        #endregion

        #region Constructors

        /////<summary>
        ///// Class Constructor
        /////</summary>
        /////<param name="letter">character value</param>
        //public NodeWithFreq(char letter) : 
        //    base(letter){}

        /////<summary>
        ///// Class Constructor
        /////</summary>
        /////<param name="letter">character value</param>
        /////<param name="isEndLetter">Current letter is the final letter of corresponding word?</param>
        //public NodeWithFreq(char letter, bool isEndLetter) :
        //    base(letter, isEndLetter){}

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="letter">character value</param>
        ///<param name="isEndLetter">Current letter is the final letter of corresponding word?</param>
        ///<param name="count">Usage frequency of word</param>
        ///<param name="pos">POS tag</param>
        public NodeWithFreqandPOS(char letter, bool isEndLetter, int count, string pos)
        {
            this.m_value = letter;
            this.m_isEndLetter = isEndLetter;

            this.m_freq = count;
            this.m_pos = pos;
        }

        #endregion

        #region Public Methods

        ///<summary>
        /// Unicode value of current node
        ///</summary>
        public char Value
        {
            get
            {
                return this.m_value;
            }
        }

        ///<summary>
        /// Number of childs
        ///</summary>
        public int LinkCount
        {
            get
            {
                return this.m_links.Count;
            }
        }

        ///<summary>
        /// Search for having a child
        ///</summary>
        ///<param name="c">child's Unicode value</param>
        ///<returns>True if contain child, otherwise False</returns>
        public bool ContainLetter(char c)
        {
            return this.m_links.ContainsKey(c);
        }

        ///<summary>
        /// Check if current node have any child
        ///</summary>
        ///<returns>True if having any child, otherwise False</returns>
        public bool HaveLinks()
        {
            return this.m_links.Count == 0 ? false : true;
        }

        ///<summary>
        /// Check if current node is final letter of a word
        ///</summary>
        public bool IsEndOfWord
        {
            get
            {
                return this.m_isEndLetter;
            }
        }


        ///<summary>
        /// Usage frequency of word (if current node is final letter of word)
        ///</summary>
        public int WordFrequency
        {
            get
            {
                return this.m_freq;
            }
        }

        ///<summary>
        /// Add a child node
        ///</summary>
        ///<param name="node">Child node</param>
        ///<returns>Node pointer to added child node</returns>
        public NodeWithFreqandPOS AddLink(NodeWithFreqandPOS node)
        {
            if (!this.m_links.ContainsKey(node.m_value))
            {
                this.m_links.Add(node.m_value, node);
            }
            else if (node.IsEndOfWord)
            {
                this.m_links[node.m_value].m_isEndLetter = node.IsEndOfWord;
                this.m_links[node.m_value].m_freq = this.m_links[node.m_value].m_freq > node.WordFrequency ? this.m_links[node.m_value].m_freq : node.WordFrequency;
                
                if (this.m_links[node.m_value].m_pos != node.m_pos)
                {
                    this.m_links[node.m_value].m_pos = mergePOS(node.m_pos, this.m_links[node.m_value].m_pos);
                }
            }

            //if (node.IsEndOfWord && !this.m_links[node.m_value].m_isEndLetter)

            return this.m_links[node.m_value];
        }

        private string mergePOS(string posStr1, string posStr2)
        {
            string[] posList1 = posStr1.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] posList2 = posStr2.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> finalList = new List<string>();

            finalList.AddRange(posList1);
            finalList.AddRange(posList2);

            finalList = finalList.Distinct().ToList();

            string finalStr = "";
            foreach (string pos in finalList)
            {
                finalStr += pos + ", ";
            }

            finalStr = finalStr.Remove(finalStr.Length - 2, 2);

            return finalStr;
        }

        ///<summary>
        /// Get a pointer to a child node by child value
        ///</summary>
        ///<param name="letter">Child's unicode value</param>
        ///<returns>Node pointer to child node</returns>
        public NodeWithFreqandPOS GetNextNode(char letter)
        {
            if (this.ContainLetter(letter))
            {
                return this.m_links[letter];
            }

            return null;
        }

        ///<summary>
        /// Get all child nodes
        ///</summary>
        ///<returns>Child nodes</returns>
        public NodeWithFreqandPOS[] GetLinks()
        {
            List<NodeWithFreqandPOS> nodes = new List<NodeWithFreqandPOS>();
            foreach (KeyValuePair<char, NodeWithFreqandPOS> link in this.m_links)
            {
                nodes.Add(link.Value);
            }

            return nodes.ToArray();
        }

        ///<summary>
        /// Remove a child node
        ///</summary>
        ///<param name="node">Child node</param>
        public void RemoveLink(NodeWithFreqandPOS node)
        {
            if (this.m_links.ContainsKey(node.m_value))
            {
                this.m_links.Remove(node.m_value);
            }
        }

        ///<summary>
        /// Logically remove current word (if current node is final letter of a word)
        ///</summary>
        public void LogicalRemove()
        {
            this.m_isEndLetter = false;
        }

        ///<summary>
        /// Remove current word and all the words that start with this word (if current node is final letter of a word)
        ///</summary>
        public void Clear()
        {
            LogicalRemove();
            this.m_links.Clear();
        }

        ///<summary>
        /// POS tag of of word (if current node is final letter of word)
        ///</summary>
        public string POSTag
        {
            get
            {
                return this.m_pos;
            }
        }

        #endregion
    }
}

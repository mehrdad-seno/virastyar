//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SCICT.NLP.Utility.Transliteration.KNN
{
    [Serializable]
    class LetterPatternContainer
    {
        private readonly Dictionary<string, Dictionary<string, int>> m_patterns = new Dictionary<string, Dictionary<string, int>>();

        public LetterPatternContainer(char ch)
        {
            Letter = ch;
        }

        internal void AddPattern(string prefix, string postfix, string mapping)
        {
            Dictionary<string, int> mappings;
            if (!ContainsPattern(prefix, postfix, out mappings))
            {
                var key = GetKey(prefix, postfix);
                mappings = new Dictionary<string, int>();
                m_patterns.Add(key, mappings);
            }
            mappings.AddOrUpdate(mapping);
        }

        private string GetKey(string prefix, string postfix)
        {
            string key;
            if (prefix == null && postfix == null)
            {
                key = string.Empty;
            }
            else
            {
                Debug.Assert(prefix != null && postfix != null);
                key = string.Format("{0}{1}{2}", prefix, MappingSequence.Separator, postfix);
            }
            return key;
        }

        internal bool ContainsPattern(string prefix, string postfix, out Dictionary<string, int> mapping)
        {
            mapping = null;
            var key = GetKey(prefix, postfix);
            if (m_patterns.ContainsKey(key))
            {
                mapping = m_patterns[key];
                return true;
            }

            return false;
        }

        public Dictionary<string, int> this[char ch, string prefix, string postfix]
        {
            get
            {
                Dictionary<string, int> mapping;
                return this.ContainsPattern(prefix, postfix, out mapping) ? mapping : null;
            }
        }

        public char Letter { get; set; }
    }

    [Serializable]
    class PatternStorage
    {
        private SortedDictionary<char, LetterPatternContainer> characters = new SortedDictionary<char, LetterPatternContainer>();

        public bool AddOrUpdatePattern(char ch, string prefix, string postfix, string mapping)
        {
            LetterPatternContainer chTree;
            if (!characters.Keys.Contains(ch))
            {
                chTree = new LetterPatternContainer(ch);
                characters.Add(ch, chTree);
            }
            else
                chTree = characters[ch];

            chTree.AddPattern(prefix, postfix, mapping);
            return true;
        }

        public bool ContainsPattern(char ch, string prefix, string postfix)
        {
            Dictionary<string, int> mapping;
            return ContainsPattern(ch, prefix, postfix, out mapping);
        }

        public bool ContainsPattern(char ch, string prefix, string postfix, out Dictionary<string, int> mapping)
        {
            mapping = null;
            if (!characters.Keys.Contains(ch))
            {
                return false;
            }
            return characters[ch].ContainsPattern(prefix, postfix, out mapping);
        }

        public Dictionary<string, int> this[char ch, string prefix, string postfix]
        {
            get
            {
                Dictionary<string, int> mapping;
                if (ContainsPattern(ch, prefix, postfix, out mapping))
                {
                    return mapping;
                }

                return null;
            }
        }
    }
}

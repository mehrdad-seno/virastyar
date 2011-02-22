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
using System.Text;

namespace SCICT.NLP.Utility.PinglishConverter
{
    using KeyValueList = Dictionary<string, int>;
    using System.Diagnostics;

    [Serializable]
    class LetterPatternContainer
    {
        private readonly Dictionary<string, KeyValueList> m_patterns = new Dictionary<string, KeyValueList>();

        public LetterPatternContainer(char ch)
        {
            Letter = ch;
        }

        internal void AddPattern(string prefix, string postfix, string mapping)
        {
            KeyValueList mappings;
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

        internal bool ContainsPattern(string prefix, string postfix, out KeyValueList mapping)
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

        public KeyValueList this[char ch, string prefix, string postfix]
        {
            get
            {
                KeyValueList mapping;
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
            KeyValueList mapping;
            return ContainsPattern(ch, prefix, postfix, out mapping);
        }

        public bool ContainsPattern(char ch, string prefix, string postfix, out KeyValueList mapping)
        {
            mapping = null;
            if (!characters.Keys.Contains(ch))
            {
                return false;
            }
            return characters[ch].ContainsPattern(prefix, postfix, out mapping);
        }

        public KeyValueList this[char ch, string prefix, string postfix]
        {
            get
            {
                KeyValueList mapping;
                if (ContainsPattern(ch, prefix, postfix, out mapping))
                {
                    return mapping;
                }

                return null;
            }
        }
    }
}

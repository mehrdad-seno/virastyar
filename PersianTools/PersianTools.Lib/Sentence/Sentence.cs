using System;
using System.Collections.Generic;
using System.Linq;
using SCICT.NLP.Utility;
using System.Diagnostics;

namespace SCICT.NLP
{
    public class Sentence
    {
        private readonly SentenceToken[] m_tokens;
        private readonly List<int> m_nonWhitespaceIndices;
        private readonly SentenceToken[] m_nonWhitespaceTokens;

        /// <summary>
        /// Key: tag name; Value: per token array of tag objects
        /// </summary>
        private readonly Dictionary<string, object[]> m_tags = new Dictionary<string, object[]>();

        public string Locale { get; private set; }

        public Sentence(string locale, TokenInfo[] tokens)
        {
            Locale = locale.ToLower();
            
            m_tokens = new SentenceToken[tokens.Length];
            m_nonWhitespaceIndices = new List<int>();
            var lstNonWhitespaceTokens = new List<SentenceToken>();

            for (int i = 0; i < tokens.Length; i++)
            {
                m_tokens[i] = new SentenceToken(tokens[i], this, i);

                var tokenValue = m_tokens[i].Value;

                if (String.IsNullOrEmpty(tokenValue))
                {
                    if (i == 0) // empty token at the begnning of the sentence is the sentence-start token
                    {
                        m_nonWhitespaceIndices.Add(i);
                        lstNonWhitespaceTokens.Add(m_tokens[i]);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!StringUtil.IsWhiteSpace(tokenValue[0]))
                {
                    m_nonWhitespaceIndices.Add(i);
                    lstNonWhitespaceTokens.Add(m_tokens[i]);
                }
            }

            m_nonWhitespaceTokens = lstNonWhitespaceTokens.ToArray();
        }

        public int Count
        {
            get { return m_tokens.Length; }
        }

        public int CountNonWhitespace
        {
            get { return m_nonWhitespaceIndices.Count; }
        }


        public SentenceToken GetAt(int i)
        {
            return m_tokens[i];
        }

        public SentenceToken GetAtNonWhitespace(int i)
        {
            return m_nonWhitespaceTokens[i];
        }


        public SentenceToken this[int i]
        {
            get { return m_tokens[i]; }
        }

        public object this[int i, string tagName]
        {
            get { return GetTagAt(i, tagName, false); }
        }

        public bool Tag(string tagName)
        {
            if (!TaggerFactory.IsTagRegistered(this.Locale, tagName))
            {
                throw new ArgumentException("No Tagger is associated with this tag name!", "tagName");
                return false;
            }

            tagName = tagName.ToLower();
            object[] tagValues;

            if (!m_tags.TryGetValue(tagName, out tagValues))
            {
                var tagsValues = TaggerFactory.Tag(tagName, this);

                if (tagsValues == null)
                {
                    m_tags.Add(tagName, new object[m_tokens.Length]);
                }
                else
                {
                    foreach (var tagAndValues in tagsValues)
                    {
                        if (!m_tags.ContainsKey(tagAndValues.Key.ToLower()))
                        {
                            m_tags.Add(tagAndValues.Key.ToLower(), tagAndValues.Value);
                        }
                        else
                        {
                            m_tags[tagAndValues.Key.ToLower()] = tagAndValues.Value;
                        }
                    }
                }
            }

            return true;
        }

        public void SetTagAtNonWhitespace(int tokenIndex, string tagName, object tagValue)
        {
            int realTokenIndex = m_nonWhitespaceIndices[tokenIndex];
            SetTagAt(realTokenIndex, tagName, tagValue);
        }

        public void SetTagAt(int tokenIndex, string tagName, object tagValue)
        {
            if (!TaggerFactory.IsTagRegistered(this.Locale, tagName))
                throw new InvalidOperationException(string.Format("{0} is not registered", tagName));

            if (tokenIndex >= Count)
            {
                throw new ArgumentOutOfRangeException("tokenIndex");
            }

            tagName = tagName.ToLower();
            if (!m_tags.ContainsKey(tagName))
                m_tags.Add(tagName, new object[Count]);

            m_tags[tagName][tokenIndex] = tagValue;
        }


        /// <summary>
        /// Get the tag for the specified token. 
        /// If the token is not already tagged, tries to find the proper tagger and calls it.
        /// </summary>
        public object GetTagAt(int tokenIndex, string tagName)
        {
            return GetTagAt(tokenIndex, tagName, true);
        }

        public object GetTagAtNonWhitespace(int tokenIndex, string tagName)
        {
            int realTokenIndex = m_nonWhitespaceIndices[tokenIndex];
            return GetTagAt(realTokenIndex, tagName, true);
        }


        /// <summary>
        /// Get the tag for the specified token.
        /// If the token is not already tagged, tries to find the proper tagger and calls it.
        /// </summary>
        /// <typeparam name="T">type of the tagger to perform type-casting.</typeparam>
        /// <param name="tokenIndex">Index of the token.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public T GetTagAt<T>(int tokenIndex, string tagName)
        {
            object tag = GetTagAt(tokenIndex,  tagName, true);
            return (T)tag;
        }

        /// <summary>
        /// Gets the tag at non whitespace.
        /// </summary>
        /// <typeparam name="T">type of the tagger to perform type-casting.</typeparam>
        /// <param name="tokenIndex">Index of the token in an array of only non-whitespace tokens.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public T GetTagAtNonWhitespace<T>(int tokenIndex, string tagName)
        {
            int realTokenIndex = m_nonWhitespaceIndices[tokenIndex];
            return GetTagAt<T>(realTokenIndex, tagName);
        }


        private object GetTagAt(int tokenIndex, string tagName, bool createIfNotPresent)
        {
            if(tokenIndex < 0 || tokenIndex >= m_tokens.Length)
                throw new ArgumentOutOfRangeException("tokenIndex");

            if(!TaggerFactory.IsTagRegistered(this.Locale, tagName))
                throw new ArgumentException("No Tagger is associated with this tag name!", "tagName");

            object[] tagValues;
            if(!m_tags.TryGetValue(tagName, out tagValues))
            {
                if (createIfNotPresent)
                    Tag(tagName);
                else
                    throw new Exception("Tag does not exist!");
            }

            Debug.Assert(tagValues.Length == m_tokens.Length, "The length of provided tags does not match the length of tokens!");
            return tagValues[tokenIndex];
        }

        private object GetTagAtNonWhitespace(int tokenIndex, string tagName, bool createIfNotPresent)
        {
            int realTokenIndex = m_nonWhitespaceIndices[tokenIndex];
            return GetTagAt(realTokenIndex, tagName, createIfNotPresent);
        }

        public string[] CustomTagNames
        {
            get { return m_tags.Keys.ToArray(); }
        }

        /// <summary>
        /// Gets the tokens array (containing words, spaces, and punctuations).
        /// </summary>
        public SentenceToken[] Tokens
        {
            get
            {
                return m_tokens;
            }
        }

        /// <summary>
        /// Gets the non whitespace tokens (containing words and punctuations only).
        /// </summary>
        public SentenceToken[] NonWhitespaceTokens
        {
            get
            {
                return m_nonWhitespaceTokens;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// This class matches and finds patterns occuring in the end of a string.
    /// It makes use of some special wild-card symbols which suits the Persian language more.
    /// For a list of the possible wild-cards see the "Symbolic Character Constants" region of the code.
    /// </summary>
    public class ReversePatternMatcher
    {
        #region Private Fields
        /// <summary>
        /// List of patterns that should be checked in the end of each input
        /// </summary>
        private List<string> m_listEndingPatterns = new List<string>();

        /// <summary>
        /// List of search node which provide the means for reading from each ending pattern
        /// character by character or disable each node during the process.
        /// </summary>
        private List<SearchNode> m_listSearchNodes = new List<SearchNode>();

        /// <summary>
        /// An instance of the <see cref="ReverseStringReader"/> class that helps 
        /// reading a string content in reverse order in linear time.
        /// </summary>
        private readonly ReverseStringReader m_reverseStringReader = new ReverseStringReader();

        /// <summary>
        /// The delegate to post processing rules
        /// </summary>
        private PostProcessRule m_postProcessRule;

        #endregion

        #region Delegates

        /// <summary>
        /// Use this delegate to pass methods which performs post-processing on returning results.
        /// </summary>
        /// <param name="baseWord">The base word to be modified.</param>
        /// <param name="suffix">The suffix to be modified.</param>
        /// <param name="baseWords">The new base words to be added.</param>
        /// <param name="suffixes">The new suffixes to be added.</param>
        public delegate void PostProcessRule(string baseWord, string suffix, out string[] baseWords, out string[] suffixes);

        #endregion


        #region Methods

        /// <summary>
        /// Sets the ending patterns from the sequence of strings provided.
        /// </summary>
        /// <param name="patterns">The sequence of patterns to add.</param>
        public void SetEndingPatterns(IEnumerable<string> patterns)
        {
            m_listEndingPatterns.Clear();
            foreach (string str in patterns)
            {
                AddEndingPattern(str, false);
            }

            m_listEndingPatterns = m_listEndingPatterns.Distinct().ToList();

            m_listSearchNodes = SearchNode.CreateSearchNodeList(m_listEndingPatterns);
        }

        /// <summary>
        /// Adds all possible pattern-combinations of an ending-pattern string
        /// to the list of ending patterns.
        /// </summary>
        /// <param name="pattern">The pattern string to add</param>
        /// <param name="checkDuplicates">if set to <c>true</c> checks duplicate patterns.</param>
        private void AddEndingPattern(string pattern, bool checkDuplicates)
        {
            string revPattern = ReverseString(pattern.Trim());

            foreach (string str in GeneratePossiblePatterns(revPattern))
            {
                AddAtomicEndingPattern(str, checkDuplicates);
            }
        }

        /// <summary>
        /// Adds the ending pattern directly to the list of ending-patterns if it
        /// has not been already added.
        /// </summary>
        /// <param name="pattern">The pattern to add.</param>
        /// <param name="checkDuplicates">if set to <c>true</c> checks duplicate patterns.</param>
        private void AddAtomicEndingPattern(string pattern, bool checkDuplicates)
        {
            //if (pattern.Length > 0 && !listEndingPatterns.Contains(pattern))
            //    listEndingPatterns.Add(pattern);
            if (pattern.Length > 0)
            {
                if (checkDuplicates)
                {
                    if (!m_listEndingPatterns.Contains(pattern))
                        m_listEndingPatterns.Add(pattern);
                }
                else
                {
                    m_listEndingPatterns.Add(pattern);
                }
            }
        }

        /// <summary>
        /// Generates all possible non-optional patterns from a given pattern.
        /// e.g. A(Space*)B  --> AB , A(Space+)B
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <returns></returns>
        private static IEnumerable<string> GeneratePossiblePatterns(string pat)
        {
            var lstPatterns = new List<string>();
            foreach (char ch in pat)
            {
                if (IsOptionalSymbol(ch))
                {
                    var newList = new List<string>();
                    if (lstPatterns.Count <= 0)
                        lstPatterns.Add("");

                    foreach (string item in lstPatterns)
                    {
                        newList.Add(item);
                        //newList.Add(item + ch);
                        newList.Add(item + NonOptionalSymbolFor(ch));
                    }

                    lstPatterns = newList;
                }
                else
                {
                    if (lstPatterns.Count <= 0)
                    {
                        lstPatterns.Add(ch.ToString());
                    }
                    else
                    {
                        for(int i = 0; i < lstPatterns.Count; ++i)
                        {
                            lstPatterns[i] += ch;
                        }
                    }
                }
            }

            return lstPatterns;
        }

        /// <summary>
        /// Reverses the specified string.
        /// </summary>
        /// <param name="str">The string to reverse.</param>
        private static string ReverseString(string str)
        {
            var sb = new StringBuilder();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                sb.Append(str[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the character from the pattern string can be
        /// equal to the character from the input string. The pattern string 
        /// can contain regex symbol characters. e.g. A space character can be 
        /// equal to Space+ symbol.
        /// </summary>
        /// <param name="chPattern">The character from pattern string.</param>
        /// <param name="chInput">The character from input string.</param>
        /// <returns></returns>
        private static bool AreCharactersEqual(char chPattern, char chInput)
        {
            if (chPattern == chInput)
            {
                return true;
            }
            else if (chPattern == ' ')
            {
                switch (chInput)
                {
                    case ' ':
                        return true;
                    default:
                        return false;
                }
            }
            else if (chPattern == HalfSpace || chPattern == SymbolHalfSpace)
            {
                switch (chInput)
                {
                    case SymbolHalfSpace:
                    case HalfSpace:
                        return true;
                    default:
                        return false;
                }
            }
            else if (chPattern == SymbolSpaceOrHalfSpace)
            {
                switch (chInput)
                {
                    case SymbolHalfSpace:
                    case HalfSpace:
                    case ' ':
                    case SymbolSpaceOrHalfSpace:
                        return true;
                    default:
                        return false;
                }
            }
            else if (chPattern == SymbolSpaceOrHalfSpacePlus)
            {
                switch (chInput)
                {
                    case SymbolHalfSpace:
                    case HalfSpace:
                    case ' ':
                    case SymbolSpacePlus:
                    case SymbolSpaceOrHalfSpace:
                    case SymbolSpaceOrHalfSpacePlus:
                        return true;
                    default:
                        return false;
                }
            }
            else if (chPattern == SymbolSpaceOrHalfSpaceStar)
            {
                switch (chInput)
                {
                    case SymbolHalfSpace:
                    case HalfSpace:
                    case ' ':
                    case SymbolSpacePlus:
                    case SymbolSpaceOrHalfSpace:
                    case SymbolSpaceOrHalfSpacePlus:
                    case SymbolSpaceStar:
                    case SymbolSpaceOrHalfSpaceStar:
                        return true;
                    default:
                        return false;
                }
            }
            else if (chPattern == SymbolSpacePlus)
            {
                switch (chInput)
                {
                    case ' ':
                    case SymbolSpacePlus:
                        return true;
                    case SymbolSpaceOrHalfSpace:
                    case SymbolSpaceOrHalfSpacePlus:
                    case SymbolSpaceStar:
                    case SymbolSpaceOrHalfSpaceStar:
                    case SymbolHalfSpace:
                    case HalfSpace:
                    default:
                        return false;
                }
            }
            else if (chPattern == SymbolSpaceStar)
            {
                switch (chInput)
                {
                    case ' ':
                    case SymbolSpacePlus:
                    case SymbolSpaceStar:
                        return true;
                    default:
                        return false;
                }
            }
            else if (chPattern == SymbolHalfSpaceQuestionMark)
            {
                switch (chInput)
                {
                    case SymbolHalfSpace:
                    case HalfSpace:
                    case SymbolHalfSpaceQuestionMark:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Resets the search nodes at the start of each pattern-matching operation.
        /// </summary>
        private void ResetSearchNodes()
        {
            foreach (SearchNode node in m_listSearchNodes)
            {
                node.Reset();
            }
        }

        /// <summary>
        /// Sets the post process rule. The rule is passed in the form of a delagate.
        /// </summary>
        /// <param name="ruleMethod">The post process rule method.</param>
        public void SetPostProcessRule(PostProcessRule ruleMethod)
        {
            if (m_postProcessRule == null)
                m_postProcessRule = ruleMethod;
        }

        /// <summary>
        /// Matches the input string with the ending patterns provided before and returns a
        /// sequence of <see cref="ReversePatternMatcherPatternInfo"/> objects which will hold information of the matched pattern.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public ReversePatternMatcherPatternInfo[] Match(string input)
        {
            return Match(input, false);
        }

        /// <summary>
        /// Matches the input string with the ending patterns provided before and returns a
        /// sequence of <see cref="ReversePatternMatcherPatternInfo"/> objects which will hold information of the matched pattern.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="uniqueResults">if set to <c>true</c> returns unique results only.</param>
        /// <returns></returns>
        public ReversePatternMatcherPatternInfo[] Match(string input, bool uniqueResults)
        {
            var lstFoundPats = new List<ReversePatternMatcherPatternInfo>();
            if (input.Length <= 0)
                return lstFoundPats.ToArray();

            ResetSearchNodes();
            bool[] inactiveSearchNodes = new bool[m_listSearchNodes.Count]; // automatically initialized to false

            SearchNode curNode;
            bool removeNode = true;

            char curChar = m_reverseStringReader.ReadFirstChar(input);
            while (m_reverseStringReader.HasMoreChars())
            {
                for (int j = m_listSearchNodes.Count - 1; j >= 0; j--)
                {
                    if (!inactiveSearchNodes[j])
                    {
                        removeNode = true; // the node should be removed by default unless it matches the input
                        curNode = m_listSearchNodes[j];
                        if (curNode.Finished)
                        {
                            string baseWord = input.Substring(0, m_reverseStringReader.GetCurrentIndex() + 1);
                            string suffix = input.Substring(m_reverseStringReader.GetCurrentIndex() + 1);

                            string[] newBaseWords = null, newSuffixes = null;
                            if (m_postProcessRule != null)
                            {
                                m_postProcessRule(baseWord, suffix, out newBaseWords, out newSuffixes);
                            }
                            else
                            {
                                newBaseWords = new[] {baseWord};
                                newSuffixes = new[] {suffix};
                            }

                            int len = newBaseWords == null ? 0 : newBaseWords.Length;
                            for(int i = 0; i < len; i++)
                            {
                                var foundPattern =
                                    new ReversePatternMatcherPatternInfo(newBaseWords[i], newSuffixes[i]);

                                if (!uniqueResults)
                                {
                                    lstFoundPats.Insert(0, foundPattern);
                                }
                                else
                                {
                                    if (!lstFoundPats.Contains(foundPattern))
                                        lstFoundPats.Insert(0, foundPattern);
                                }
                            }
                        }
                        else // if not curNode is finished
                        {
                            try
                            {
                                if (AreCharactersEqual(curNode.GetChar(), curChar))
                                    removeNode = false;
                            }
                            catch { }
                        }

                        if (removeNode)
                        {
                            inactiveSearchNodes[j] = true;
                            //lstSearchNodes.RemoveAt(j);
                        }
                    }
                } // for ... nodes

                if (m_listSearchNodes.Count <= 0) break;

                curChar = m_reverseStringReader.ReadNextChar();
            } 

            return lstFoundPats.ToArray();
        }

        #endregion

        #region Symbolic Character Constants

        /// <summary>
        /// The character used to indicate a single half-space
        /// </summary>
        public const char SymbolHalfSpace = '*';

        /// <summary>
        /// The character used to indicate an optional half-space
        /// </summary>
        public const char SymbolHalfSpaceQuestionMark = '%';

        /// <summary>
        /// The character used to indicate one or more space characters
        /// </summary>
        public const char SymbolSpacePlus = '#';

        /// <summary>
        /// The character used to indicate zero or more space characters
        /// </summary>
        public const char SymbolSpaceStar = '@';

        /// <summary>
        /// The character used to indicate either space or half-space character
        /// </summary>
        public const char SymbolSpaceOrHalfSpace = '|';

        /// <summary>
        /// The character used to indicate one or more space or half-space characters
        /// </summary>
        public const char SymbolSpaceOrHalfSpacePlus = '$';

        /// <summary>
        /// The character used to indicate zero or more space or half-space characters
        /// </summary>
        public const char SymbolSpaceOrHalfSpaceStar = '&';

        #endregion

        #region Symbolic Characters Utilities

        /// <summary>
        /// The standard half-space character used in Persian
        /// </summary>
        public const char HalfSpace = '\u200C';

        /// <summary>
        /// Determines whether the specified symbolic character is an optional symbol. 
        /// Optional symbols are those that can occur or not, for example 
        /// star and question mark closures make a symbol optional.
        /// Here optional symbols are:
        /// <code>
        ///     SymbolHalfSpaceQuestionMark
        ///     SymbolSpaceOrHalfSpaceStar
        ///     SymbolSpaceStar
        /// </code>
        /// </summary>
        /// <param name="ch">The character to check</param>
        /// <returns>
        /// 	<c>true</c> if the specified symbolic character is an optional symbol; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOptionalSymbol(char ch)
        {
            switch (ch)
            {
                case SymbolHalfSpaceQuestionMark:
                case SymbolSpaceOrHalfSpaceStar:
                case SymbolSpaceStar:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the non-optional version of the specified symbolic character if it is
        /// an optional symbolic character. 
        /// e.g. Non-optional version for Space-Star is Space-Plus.
        /// </summary>
        /// <param name="ch">The character to process</param>
        /// <returns></returns>
        public static char NonOptionalSymbolFor(char ch)
        {
            switch (ch)
            {
                case SymbolHalfSpaceQuestionMark:
                    return SymbolHalfSpace;
                case SymbolSpaceOrHalfSpaceStar:
                    return SymbolSpaceOrHalfSpacePlus;
                case SymbolSpaceStar:
                    return SymbolSpacePlus;
            }
            return ch;
        }


        #endregion

        #region IN-CLASS: ReverseStringReader

        /// <summary>
        /// A Helper class that helps reading a string in the reverse order and checking the
        /// pattern symbols in a linear time.
        /// </summary>
        private class ReverseStringReader
        {
            /// <summary>
            /// The input string that is going to be read character by character
            /// </summary>
            string inputString = "";

            /// <summary>
            /// The index at which the input string has been read
            /// </summary>
            int readingIndex = 0;

            /// <summary>
            /// The length of the input string
            /// </summary>
            int inputStringLength;

            /// <summary>
            /// Initializes the reading-state variables and starts by returning the first character
            /// in reverse order.
            /// </summary>
            /// <param name="strInput">The input string.</param>
            /// <exception cref="ArgumentException">If the input string is null or empty</exception>
            /// <returns></returns>
            public char ReadFirstChar(string strInput)
            {
                if (strInput == null || strInput.Length <= 0)
                {
                    throw new ArgumentException("String argument is null or empty.", "strInput");
                }
                else
                {
                    inputString = strInput;
                    inputStringLength = inputString.Length;
                    readingIndex = inputStringLength;
                    return ReadChar();
                }
            }

            /// <summary>
            /// Reads the next character in the reverse order.
            /// This method does not necessarily read only one character. It may read 
            /// several characters if they make a symbol character. e.g. If a sequence of spaces
            /// are met a Space-Plus symbol is returned instead of them.
            /// </summary>
            /// <returns></returns>
            public char ReadNextChar()
            {
                return ReadChar();
            }

            /// <summary>
            /// Determines whether there are characters left that are not read yet.
            /// </summary>
            /// <returns>
            /// 	<c>true</c> if there are characters left that are not read yet; otherwise, <c>false</c>.
            /// </returns>
            public bool HasMoreChars()
            {
                // if it is equal to zero then it has finished reading the last char and there's nothing left to read.
                return readingIndex > 0;
            }

            /// <summary>
            /// Gets the index at which the string has been read so far.
            /// </summary>
            /// <returns></returns>
            public int GetCurrentIndex()
            {
                return readingIndex;
            }

            /// <summary>
            /// Reads the next character in the reverse order. This method is 
            /// aware of <see cref="ReversePatternMatcher"/>'s special symbols, and returns 
            /// those symbols if the characters at the input string match the symbol.
            /// This method does not necessarily read only one character. It may read 
            /// several characters if they make a symbol character. e.g. If a sequence of spaces
            /// are met a Space-Plus symbol is returned instead of them.
            /// </summary>
            /// <returns></returns>
            private char ReadChar()
            {
                readingIndex--;
                char ch = inputString[readingIndex];
                if (Char.IsWhiteSpace(ch) || ch == ReversePatternMatcher.HalfSpace)
                {
                    if (ch == ReversePatternMatcher.HalfSpace)
                    {
                        ch = ReversePatternMatcher.SymbolHalfSpace;
                    }
                    else
                    {
                        ch = ' ';
                    }

                    readingIndex--;
                    while (readingIndex >= 0 &&
                        (Char.IsWhiteSpace(inputString[readingIndex]) || inputString[readingIndex] == ReversePatternMatcher.HalfSpace))
                    {
                        if (inputString[readingIndex] == ReversePatternMatcher.HalfSpace)
                        {
                            switch (ch) // switch on previously read chars
                            {
                                case ' ':
                                case ReversePatternMatcher.SymbolHalfSpace:
                                case ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus:
                                case ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar: // won't happen, just in case
                                case ReversePatternMatcher.SymbolSpacePlus:
                                case ReversePatternMatcher.SymbolSpaceStar: // won't happen, just in case
                                case ReversePatternMatcher.SymbolSpaceOrHalfSpace: // won't happen, just in case
                                    ch = ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus;
                                    break;
                            }
                        }
                        else // i.e. I read a space character
                        {
                            switch (ch) // switch on previously read chars
                            {
                                case ' ':
                                case ReversePatternMatcher.SymbolSpacePlus:
                                case ReversePatternMatcher.SymbolSpaceStar: // won't happen, just in case
                                    ch = ReversePatternMatcher.SymbolSpacePlus;
                                    break;
                                case ReversePatternMatcher.SymbolHalfSpace:
                                case ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus:
                                case ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar: // won't happen, just in case
                                case ReversePatternMatcher.SymbolSpaceOrHalfSpace: // won't happen, just in case
                                    ch = ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus;
                                    break;
                            }
                        }

                        readingIndex--;
                    }

                    readingIndex++; // take back one extra char read, the one which violated while's condition
                }

                return ch;
            }
        }
        
        #endregion

        #region IN-CLASS: SearchNode

        /// <summary>
        /// Holds ending patterns, and provide the means for reading their content character
        /// by character, and makes it easy to enable and disable each node.
        /// </summary>
        private class SearchNode : IComparable
        {
            #region Public Properties
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="SearchNode"/> is 
            /// finished with being active in the list. A node is called Finished when its input is
            /// read completely or the input has no chance of matching the input string.
            /// </summary>
            /// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
            public bool Finished { get; private set; }

            /// <summary>
            /// Gets or sets the pattern.
            /// </summary>
            /// <value>The pattern.</value>
            public string Pattern { get; private set; }

            #endregion

            #region Private Fields

            /// <summary>
            /// The index at which the pattern string has been read.
            /// </summary>
            private int Index = 0;

            #endregion

            #region Public Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="SearchNode"/> class.
            /// </summary>
            /// <param name="pattern">The pattern.</param>
            public SearchNode(string pattern)
            {
                Pattern = pattern;
                Reset();
            }

            /// <summary>
            /// Resets this instance, by setting Finished to false, and the reading index to 0.
            /// </summary>
            public void Reset()
            {
                Finished = false;
                Index = 0;
            }

            /// <summary>
            /// Gets the current character.
            /// </summary>
            /// <returns></returns>
            public char GetChar()
            {
                if (!Finished)
                {
                    char ch = Pattern[Index];
                    Index++;
                    if (Index >= Pattern.Length)
                        Finished = true;
                    return ch;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            #endregion

            #region IComparable Members

            /// <summary>
            /// Compares the current instance with another object of the same type. 
            /// The comparison is made based upon the nodes' pattern strings only.
            /// </summary>
            /// <param name="obj">An object to compare with this instance.</param>
            /// <returns>
            /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
            /// The return value has these meanings: 
            /// Value Meaning Less than zero This instance is less than <paramref name="obj"/>. 
            /// Zero This instance is equal to <paramref name="obj"/>. 
            /// Greater than zero This instance is greater than <paramref name="obj"/>.
            /// </returns>
            /// <exception cref="T:System.ArgumentException">
            /// 	<paramref name="obj"/> is not the same type as this instance. </exception>
            public int CompareTo(object obj)
            {
                SearchNode otherNode = obj as SearchNode;
                if (otherNode != null)
                {
                    return this.Pattern.CompareTo(otherNode.Pattern);
                }
                else
                {
                    return 1;
                }
            }

            #endregion

            #region Public Static Member

            /// <summary>
            /// Returns a sequence of search node list created from the specified ending-patterns.
            /// </summary>
            /// <param name="listPatterns">The list of ending-patterns.</param>
            /// <returns></returns>
            public static List<SearchNode> CreateSearchNodeList(IEnumerable<string> listPatterns)
            {
                List<SearchNode> searchNodes = new List<SearchNode>();
                foreach (string str in listPatterns)
                {
                    searchNodes.Add(new SearchNode(str));
                }
                return searchNodes;
            }

            #endregion

        }

        #endregion
    }

    /// <summary>
    /// Holds information about the outputs from <see cref="ReversePatternMatcher"/>.
    /// </summary>
    public class ReversePatternMatcherPatternInfo
    {
        /// <summary>
        /// Gets or sets the body of the word.
        /// </summary>
        /// <value>The body of the word.</value>
        public string BaseWord { get; private set; }

        /// <summary>
        /// Gets or sets the suffix part of the word.
        /// </summary>
        /// <value>The Suffix.</value>
        public string Suffix { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReversePatternMatcherPatternInfo"/> class.
        /// It also applies some (hard-coded) word construction rules to the words.
        /// </summary>
        /// <param name="baseWord">The stem of the word.</param>
        /// <param name="suffix">The affix.</param>
        public ReversePatternMatcherPatternInfo(string baseWord, string suffix)
        {
            BaseWord = baseWord;
            Suffix = suffix;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Prefix: {0}, Affix: {1}", BaseWord, Suffix);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;

            var theObj = obj as ReversePatternMatcherPatternInfo;
            if(theObj == null) return false;

            return ((this.Suffix == theObj.Suffix) && (this.BaseWord == theObj.BaseWord));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Suffix.GetHashCode() + BaseWord.GetHashCode();
        }
    }

}

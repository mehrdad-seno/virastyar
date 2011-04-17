using System;
using System.Collections.Generic;
using SCICT.NLP.Persian.Constants;

namespace SCICT.NLP.Utility.StringDistance
{
    // NOTE: scope of this enumerator shoub be the same as NGramGenerator class
    public 
        enum NGramScopes
    {
        HybridLevel = 0,
        IntraWordLevel = HybridLevel + 1,
        WordLevel =  IntraWordLevel + 1,
        CleanWordLevel = WordLevel + 1,
    }

    // these functions can insert in Edit Distance class too, but we may need them in other context and other conditions.
    // so we decide to create a class here.
    public class NGramGenerator
    {
        #region Properties
        // Note: these parameters should be set by initiating the class
        private int nGram;
        public int NGram
        {
            get { return nGram; }
            set { nGram = value; }
        }

        private NGramScopes nGramScope;
        public NGramScopes NGramScope
        {
            get { return nGramScope; }
            set { nGramScope = value; }
        }
        
        #endregion


        #region  Constructors

        public NGramGenerator()
        {
            nGram = 3;
            nGramScope =  NGramScopes.HybridLevel;
        }

        public NGramGenerator(int _nGram, NGramScopes _nGramScopes)
        {
            nGram = _nGram;
            nGramScope = _nGramScopes;
        }


        #endregion

        #region Public members

        /// <summary>
        ///  N-Gram generator according to Constructor's default setting... 
        /// </summary>
        /// <param name="_aString">a sentense or a word.</param>
        /// <returns>list of generated n-gram according to input scope.</returns>
        public List<string> Generate(string _aString)
        {
            return Generate(NGram, NGramScope, _aString);
        }

        /// <summary>
        /// General N-Gram generator... 
        /// </summary>
        /// <param name="_nGramScopes">Gram (Example: 3).</param>
        /// <param name="_aString">a sentense or a word.</param>
        /// <returns>list of generated n-gram according to input scope.</returns>
        public List<string> Generate(NGramScopes _nGramScopes, string _aString)
        {
            return Generate(NGram, _nGramScopes, _aString);
        }

        /// <summary>
        /// General N-Gram generator... 
        /// </summary>
        /// <param name="_gram">Gram (Example: 3).</param>
        /// <param name="_nGramScope">Scope for partitioning.</param>
        /// <param name="_inputString">a sentense or a word.</param>
        /// <returns>list of generated n-gram according to input scope.</returns>
        public List<string> Generate(int _gram, NGramScopes _nGramScope, string _inputString)
        {
            // all public methods use this to call other n-gram generating functions.
            if (_nGramScope == NGramScopes.HybridLevel)
                return HybridNGram(_gram, _inputString);

            if (_nGramScope == NGramScopes.IntraWordLevel)
                return IntraWordNGram(_gram, _inputString);

            if (_nGramScope == NGramScopes.WordLevel)
                return WordNGram(_gram, _inputString);

            if (_nGramScope == NGramScopes.CleanWordLevel)
                return CleanWordNGram(_gram, _inputString);

            return null;
        }

        #endregion

        #region Private functions

        private List<string> CleanWordNGram(int _gram, string _inputString)
        {
            // step 1: read word separators form a CommonPersian source           
            string separators = PersianAlphabets.Delimiters;
            string[] splitList;

            // step 2: extract all words:
            splitList = _inputString.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // step 3: generate all n-gram candidates:
            List<string> tokens = new List<string>();
            int total = splitList.Length - _gram;
            string temp = "";

            for (int i = 0; i <= total; i++)
            {
                temp = "";
                for (int j = i; j < _gram + i; j++)
                {
                    temp = temp + splitList[j] + " ";
                }
                tokens.Add(temp.Trim());
            }

            return tokens;
        }

        private List<string> WordNGram(int _gram, string _inputString)
        {
            // step 1: read word separators form a CommonPersian source           
            string separators = PersianAlphabets.Delimiters;
            List<string> separatorCharacters = new List<string>() ;
            string currentSeparator = "";
            int lastIndex = 0, startIndex=0;
            List<string> splitedString = new List<string>();
            //string[] splitList;

            // step 2: extract all words:
            for (int i = 0; i < _inputString.Length; i++)
            {
                if (separators.IndexOf(_inputString[i]) >= 0)
                {
                    // separator found:
                    startIndex = i;
                    currentSeparator = _inputString[i].ToString();
                    while ((i<_inputString.Length )&&(separators.IndexOf(_inputString[i]) >= 0))
                    {
                        currentSeparator += _inputString[i].ToString();
                        i++;
                    }

                    separatorCharacters.Add(currentSeparator);
                    splitedString.Add(_inputString.Substring(lastIndex, startIndex - lastIndex));

                    lastIndex = i;
                }
            }
           // splitList = _inputString.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // step 3: generate all n-gram candidates:
            List<string> tokens = new List<string>();
            int total = splitedString.Count - _gram;
            string temp = "";

            for (int i = 0; i <= total; i++)
            {
                temp = "";
                for (int j = i; j < _gram+i; j++)
                {                    
                    temp = temp + splitedString[j] + separatorCharacters[j] ;
                }
                tokens.Add(temp);
            }

            return tokens;
        }

        /// <summary>
        /// Extracts all n-grams from input sentense regarding intera-word level partitioning.
        /// it means that if you enter "Hello John", 3gram results are {"Hel", "ell", "llo", "joh", "ohn"}
        /// </summary>
        /// <param name="_gram"></param>
        /// <param name="_aSentense"></param>
        /// <returns></returns>
        private List<string> IntraWordNGram(int _gram, string _inputWord)
        {
            // step 1: read word separators form a CommonPersian source           
            char[] separators = PersianAlphabets.Delimiters.ToCharArray();
            string[] splitList;
            
            // step 2: extract all words:
            splitList = _inputWord.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            
            int total = splitList.Length ;
            List<string> tokens = new List<string>();
                                
            // for each word, extract n-grams separately.
            for (int i = 0; i < splitList.Length; i++)
            {
                tokens.AddRange(HybridNGram(_gram, splitList[i].Trim() ));
            }            

            return tokens;
        }

        private List<string> HybridNGram(int _gram, string _inputString)
        {
            int total = _inputString.Length - _gram ;
            List<string> tokens = new List<string>();

            for (int i = 0; i <= total; i++)
                tokens.Add(_inputString.Substring(i, _gram ));

            return tokens;
        }

        #endregion

    }
}

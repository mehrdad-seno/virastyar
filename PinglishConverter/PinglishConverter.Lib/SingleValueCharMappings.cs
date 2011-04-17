//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//


using System.Collections.Generic;
using System.Linq;

namespace SCICT.NLP.Utility.PinglishConverter
{
    /// <summary>
    /// One-to-one corresponding letters in transliteration.
    /// </summary>
    public static class SingleValueCharMappings
    {
        /// <summary>
        /// A dictionary contains all single value mappings
        /// </summary>
        private static readonly SortedDictionary<char, char> s_singleValueCharMap = new SortedDictionary<char, char>();

        /// <summary>
        /// Initializes the <see cref="SingleValueCharMappings"/> class.
        /// </summary>
        static SingleValueCharMappings()
        {
            s_singleValueCharMap['b'] = 'ب';
            s_singleValueCharMap['x'] = 'خ';
            s_singleValueCharMap['d'] = 'د';
            s_singleValueCharMap['r'] = 'ر';
            s_singleValueCharMap['f'] = 'ف';
            s_singleValueCharMap['l'] = 'ل';
            s_singleValueCharMap['m'] = 'م';
            s_singleValueCharMap['n'] = 'ن';
            s_singleValueCharMap['v'] = 'و';
            s_singleValueCharMap['w'] = 'و';
            s_singleValueCharMap['y'] = 'ی';
        }

        /// <summary>
        /// Retrieves a mapping Persian letter for the given English character.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>The mapping letter, if it contains an entry for the give character;
        /// otherwise <value>null</value></returns>
        public static char? TryGetValue(char ch)
        {
            if (s_singleValueCharMap.Keys.Contains(ch))
                return s_singleValueCharMap[ch];

            return null;
        }

        ///<summary>
        ///</summary>
        public static List<char> SingleValueCharacters
        {
            get
            {
                return s_singleValueCharMap.Keys.ToList();
            }
        }
    }
}

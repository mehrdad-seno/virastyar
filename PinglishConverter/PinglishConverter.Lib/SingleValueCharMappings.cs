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

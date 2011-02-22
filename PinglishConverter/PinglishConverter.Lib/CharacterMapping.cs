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

namespace SCICT.NLP.Utility.PinglishConverter
{
    /// <summary>
    /// Holds all possible mapping information for a letter.
    /// </summary>
    public class CharacterMapping
    {
        #region Private Fields

        private readonly List<CharacterMappingInfo> m_values;
        private readonly char m_letter;
        private readonly bool m_caseSensitive;
        //private readonly object m_label;
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapping"/> class.
        /// </summary>
        /// <param name="letter">The letter which this instance will hold its mappings.</param>
        /// <param name="values">Mapping values for the given letter.</param>
        public CharacterMapping(char letter, CharacterMappingInfo[] values)
            : this(letter, false, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapping"/> class.
        /// </summary>
        /// <param name="letter">The letter which this instance will hold its mappings.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <param name="values">Mapping values for the given letter.</param>
        private CharacterMapping(char letter, bool caseSensitive, CharacterMappingInfo[] values)
        {
            this.m_letter = letter;
            this.m_caseSensitive = caseSensitive;
            this.m_values = new List<CharacterMappingInfo>(values);
            this.m_values.Sort();
        }

        /// <summary>
        /// Gets the letter which this instance holds its mapping information.
        /// </summary>
        /// <value>The letter.</value>
        public char Letter
        {
            get
            {
                return this.m_letter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is case sensitive.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is case sensitive; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaseSensitive
        {
            get { return this.m_caseSensitive; }
        }

        /// <summary>
        /// Gets the corresponding mapping information of the <see cref="Letter"/>
        /// </summary>
        /// <value>The values.</value>
        public CharacterMappingInfo[] Values
        {
            get
            {
                return (this.m_values != null) ? this.m_values.ToArray() : null;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.m_letter != '\0')
            {
                return this.m_letter.ToString();
            }
            return "";
            /*else
            {
                return this.m_label.ToString();
            }*/
        }
    }
}

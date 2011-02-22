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


namespace SCICT.NLP.Utility.PinglishConverter
{
    using System;

    /// <summary>
    /// Defines mapping information for a character. These information are used in conversion phase. 
    /// </summary>
    public class CharacterMappingInfo : IComparable<CharacterMappingInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="relativeIndex">The relative index of this instance. Instances with high value of <see cref="relativeIndex"/> 
        /// has less priority in generation phase.</param>
        public CharacterMappingInfo(string value, int relativeIndex)
            : this(value, EmptyChar, TokenPosition.Any, relativeIndex, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="relativeIndex">The relative index of this instance. Instances with high value of <see cref="relativeIndex"/> 
        /// has less priority in generation phase.</param>
        /// <param name="name">The name of this instance.</param>
        public CharacterMappingInfo(string value, int relativeIndex, string name)
            : this(value, EmptyChar, TokenPosition.Any, relativeIndex, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="postfix">The postfix character, if any. 
        /// For example: 'h' is a possible postfix for 's' character in Persian transliteration.</param>
        public CharacterMappingInfo(string value, char postfix)
            : this(value, postfix, TokenPosition.Any, 0, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="postfix">The postfix character, if any. 
        /// For example: 'h' is a possible postfix for 's' character in Persian transliteration.</param>
        /// <param name="name">The name of this instance.</param>
        public CharacterMappingInfo(string value, char postfix, string name)
            : this(value, postfix, TokenPosition.Any, 0, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="position">The position.</param>
        /// <param name="relativeIndex">The relative index of this instance. Instances with high value of <see cref="relativeIndex"/> 
        /// has less priority in generation phase.</param>
        public CharacterMappingInfo(string value, TokenPosition position, int relativeIndex)
            : this(value, EmptyChar, position, relativeIndex, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="position">The position.</param>
        /// <param name="relativeIndex">The relative index of this instance. Instances with high value of <see cref="relativeIndex"/> 
        /// has less priority in generation phase.</param>
        /// <param name="name">The name of this instance.</param>
        public CharacterMappingInfo(string value, TokenPosition position, int relativeIndex, string name)
            : this(value, EmptyChar, position, relativeIndex, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="postfix">The postfix character, if any. 
        /// For example: 'h' is a possible postfix for 's' character in Persian transliteration.</param>
        /// <param name="position">The position.</param>
        /// <param name="relativeIndex">The relative index of this instance. Instances with high value of <see cref="relativeIndex"/> 
        /// has less priority in generation phase.</param>
        public CharacterMappingInfo(string value, char postfix, TokenPosition position, int relativeIndex)
            : this(value, postfix, position, relativeIndex, value)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMappingInfo"/> class.
        /// </summary>
        /// <param name="value">The character which this instance holds its mapping information.</param>
        /// <param name="postfix">The postfix character, if any. 
        /// For example: 'h' is a possible postfix for 's' character in Persian transliteration.</param>
        /// <param name="position">The position.</param>
        /// <param name="relativeIndex">The relative index of this instance. Instances with high value of <see cref="relativeIndex"/> 
        /// has less priority in generation phase.</param>
        /// <param name="name">The name.</param>
        public CharacterMappingInfo(string value, char postfix, TokenPosition position, int relativeIndex, string name)
        {
            this.Value = value;
            this.Postfix = postfix;
            this.Position = position;
            this.RelativeIndex = relativeIndex;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets The character which this instance holds its mapping information.
        /// </summary>
        /// <value>The character which this instance holds its mapping information.</value>
        public string Value { get; private set; }
        
        /// <summary>
        /// Gets or sets the postfix.
        /// </summary>
        /// <value>The postfix character, if any.
        /// For example: 'h' is a possible postfix for 's' character in Persian transliteration.</value>
        public char Postfix { get; private set; }

        /// <summary>
        /// Empty character which is used by the classes of this namespace.
        /// </summary>
        public const char EmptyChar = '\0';

        /// <summary>
        /// Empty string which is used by the classes of this namespace.
        /// </summary>
        public const string EmptyString = "\0";
        
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public TokenPosition Position { get; private set; }
        
        /// <summary>
        /// Gets or sets the relative index of this instance.
        /// </summary>
        /// <value>The relative index of this instance. Instances with high value of <see cref="RelativeIndex"/> 
        /// has less priority in generation phase.</value>
        public int RelativeIndex { get; private set; }
        
        /// <summary>
        /// Gets or sets the name of this instance.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        #region Overroaled Methods

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CharacterMappingInfo)
            {
                CharacterMappingInfo otherObj = (CharacterMappingInfo)obj;
                return (otherObj.Value == this.Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        int IComparable<CharacterMappingInfo>.CompareTo(CharacterMappingInfo other)
        {
            if (other == null)
                return 1;

            return this.RelativeIndex.CompareTo(other.RelativeIndex);
        }

        #endregion
    }
}
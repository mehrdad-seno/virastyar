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
using System.Diagnostics;
using System.Text;

namespace SCICT.NLP.Utility.PinglishConverter
{
    /// <summary>
    /// Represents a Pinglish word, and its corresponding Persian word
    /// </summary>
    [Serializable]
    public class PinglishString
    {
        #region Constructors

        public PinglishString()
        {
            EnglishLetters = new List<char>();
            PersianLetters = new List<string>();
        }

        public PinglishString(string englishString)
        {
            EnglishLetters = new List<char>();
            PersianLetters = new List<string>();
            foreach (char ch in englishString)
            {
                Append(ch.ToString(), ch);
            }
        }

        #endregion

        #region Private Members

        ///<summary>
        ///</summary>
        public List<string> PersianLetters { get; set; }

        ///<summary>
        ///</summary>
        public List<char> EnglishLetters { get; set; }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the Persian string.
        /// </summary>
        /// <value>The Persian string.</value>
        public string PersianString
        {
            get
            {
                var sb = new StringBuilder();
                for (int i = 0; i < PersianLetters.Count; i++)
                {
                    if (PersianLetters[i] != null)
                        sb.Append(PersianLetters[i]);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the english string.
        /// </summary>
        /// <value>The english string.</value>
        public string EnglishString
        {
            get
            {
                var sb = new StringBuilder();
                for (int i = 0; i < EnglishLetters.Count; i++)
                {
                    sb.Append(EnglishLetters[i]);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get
            {
                return PersianLetters.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>
        /// Returns a KeyValuePair: 
        /// Key is the English character, and Value is its Persian equivalent.
        /// </returns>
        public KeyValuePair<char, string> this[int index]
        {
            get
            {
                return new KeyValuePair<char, string>(EnglishLetters[index], PersianLetters[index]);
            }
        }

        #endregion

        #region Methods

        public void Update(int index, string persianLetter, char englishLetter)
        {
            Debug.Assert(index <= Length);

            if (index == Length)
            {
                Append(persianLetter, englishLetter);
            }
            // Never uncomment this line, unless you know its side-effects in the library
            /*else if (index < Length)
            {
                UpdateAtPos(index, persianLetter, englishLetter);
            }*/
        }

        /*private void UpdateAtPos(int index, string persianLetter, char englishLetter)
        {
            englishLetters[index] = englishLetter;
            m_persianLetters[index] = persianLetter;
        }*/

        /// <summary>
        /// Appends the specified letters to this instance.
        /// </summary>
        /// <param name="persianLetter">The Persian letter.</param>
        /// <param name="englishLetter">The English letter.</param>
        public void Append(string persianLetter, char englishLetter)
        {
            PersianLetters.Add(persianLetter);
            EnglishLetters.Add(englishLetter);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PinglishString Clone()
        {
            var cloned = new PinglishString();
            cloned.PersianLetters.AddRange(this.PersianLetters);
            cloned.EnglishLetters.AddRange(this.EnglishLetters);
            return cloned;
        }

        public PinglishString ToLower()
        {
            PinglishString cloned = Clone();
            for (int i = 0; i < EnglishLetters.Count; i++)
            {
                cloned.EnglishLetters[i] = char.ToLower(EnglishLetters[i]);
            }

            return cloned;
        }

        #endregion

        #region override Elements

        public override bool Equals(object obj)
        {
            var objB = obj as PinglishString;
            if (object.ReferenceEquals(objB, null))
                return false;

            if (this.EnglishLetters.Count != objB.EnglishLetters.Count)
                return false;

            if (PersianLetters.Count != objB.PersianLetters.Count)
                return false;

            for (int i = 0; i < this.EnglishLetters.Count; i++)
            {
                if (this.EnglishLetters[i] != objB.EnglishLetters[i])
                    return false;
            }

            for (int i = 0; i < this.PersianLetters.Count; i++)
            {
                if (this.PersianLetters[i] != objB.PersianLetters[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return (EnglishLetters.GetHashCode() * PersianLetters.GetHashCode());
        }

        #endregion
    }

    ///<summary>
    ///</summary>
    public class PinglishStringEqualityComparer : IEqualityComparer<PinglishString>, IComparer<PinglishString>
    {
        #region IEqualityComparer<PinglishString> Members

        bool IEqualityComparer<PinglishString>.Equals(PinglishString x, PinglishString y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<PinglishString>.GetHashCode(PinglishString obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        #region IComparer<PinglishString> Members

        int IComparer<PinglishString>.Compare(PinglishString x, PinglishString y)
        {
            if (x.EnglishString != y.EnglishString)
                return string.Compare(x.EnglishString, y.EnglishString);

            return string.Compare(x.PersianString, y.PersianString);
        }

        #endregion
    }

}

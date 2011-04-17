//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//


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
        public List<string> PersianLetters { get;  set; }

        ///<summary>
        ///</summary>
        public List<char> EnglishLetters { get;  set; }

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

        public void UpdateAtPos(int index, string persianLetter, char englishLetter)
        {
            EnglishLetters[index] = englishLetter;
            PersianLetters[index] = persianLetter;
        }

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

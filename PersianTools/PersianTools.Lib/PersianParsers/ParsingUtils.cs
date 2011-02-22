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
using System.Text;

namespace SCICT.NLP.Utility.Parsers
{
    #region ParsingUtils class

    /// <summary>
    /// A Utility class which is mainly used by parsers.
    /// </summary>
    public class ParsingUtils
    {
        /// <summary>
        /// Converts the string containing numbers from Persian/Arabic to English. 
        /// This includes digits, decimal points, and thousand seperators.
        /// </summary>
        /// <param name="num">The string of number to convert.</param>
        /// <returns></returns>
        public static string ConvertNumber2English(string num)
        {
            StringBuilder sb = new StringBuilder(num.Length);
            char ch;
            foreach (char c in num)
            {
                ch = c;
                if ('\u06F0' <= c && c <= '\u06F9')
                    ch = Convert.ToChar((Convert.ToInt32(c) - 0x06f0) + Convert.ToInt32('0'));
                if ('\u0660' <= c && c <= '\u0669')
                    ch = Convert.ToChar((Convert.ToInt32(c) - 0x0660) + Convert.ToInt32('0'));

                switch (c)
                {
                    case '٫': // Arabic Decimal Seperator 0x066B
                        ch = '.';
                        break;
                    case '/': // Sometimes used as Arabic Seperator
                        ch = '.';
                        break;
                    case '٬': // Arabic Thousand Seperator 0x066C
                        ch = ',';
                        break;
                }

                sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts the string containing numbers from English/Arabic to Persian. 
        /// This includes digits, decimal points, and thousand seperators.
        /// </summary>
        /// <param name="num">The string of number to convert.</param>
        /// <returns></returns>
        public static string ConvertNumber2Persian(string num)
        {
            StringBuilder sb = new StringBuilder(num.Length);
            char ch;
            foreach (char c in num)
            {
                ch = c;
                if ('0' <= c && c <= '9')
                    ch = Convert.ToChar((Convert.ToInt32(c) - Convert.ToInt32('0')) + 0x06F0);
                if ('\u0660' <= c && c <= '\u0669')
                    ch = Convert.ToChar((Convert.ToInt32(c) - 0x0660) + 0x06F0);

                switch (c)
                {
                    case '.': // Arabic Decimal Seperator 0x066B
                        ch = '٫';
                        break;
                    case ',': // Arabic Thousand Seperator 0x066C
                        ch = '٬';
                        break;
                }

                sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Normalizes the spaces. Replaces multiple occurrances of 
        /// white-space characters to a single space character.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static string NormalizeSpaces(string str)
        {
            StringBuilder sb = new StringBuilder();

            bool metWS = false;
            foreach (char c in str)
            {
                if (Char.IsWhiteSpace(c))
                {
                    if (!metWS)
                    {
                        sb.Append(' ');
                    }
                    metWS = true;
                }
                else
                {
                    sb.Append(c);
                    metWS = false;
                }
            }

            return sb.ToString();
        }
    } // end of class

    #endregion

    #region PatternInfoTypes enum

    /// <summary>
    /// Enumerates different types of xPatternInfo classes.
    /// </summary>
    public enum PatternInfoTypes
    {
        /// <summary>
        /// used by EnglishDatePatternInfo class
        /// </summary>
        EnglishDate,
        /// <summary>
        /// used by NumericDatePatternInfo class
        /// </summary>
        NumericDate,
        /// <summary>
        /// used by PersianDatePatternInfo class
        /// </summary>
        PersianDate,
        /// <summary>
        /// used by PersianNumberPatternInfo class
        /// </summary>
        PersianNumber,
        /// <summary>
        /// used by DigitizedNumberPatternInfo class
        /// </summary>
        DigitizedNumber
    }

    #endregion

    #region IPatternInfo interface
    /// <summary>
    /// Defines the main interface to xPatternInfo classes which all implement this interface.
    /// </summary>
    public interface IPatternInfo
    {
        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        PatternInfoTypes PatternInfoType
        {
            get;
        }

        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The content.</value>
        string Content
        {
            get ;
        }

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        int Index
        {
            get ;
        }

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        int Length
        {
            get ;
        }
    }

    #endregion
}

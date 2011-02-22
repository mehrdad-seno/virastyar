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
using SCICT.NLP.Utility.Parsers;

namespace SCICT.NLP.Utility
{
    /// <summary>
    /// Provides the means to convert long integer and double numbers to a Persian descriptive string.
    /// </summary>
    public sealed class NumberToPersianString
    {
        /// <summary>
        /// Tries to convert a long integer number to a descriptive string in Persian language. A return value indicates
        /// whether the operation succeeded or not.
        /// </summary>
        /// <param name="n">The number to convert.</param>
        /// <param name="str">The string that holds the result.</param>
        /// <returns></returns>
        public static bool TryConvertNumberToPersianString(long n, out string str)
        {
            str = "";
            try
            {
                str = ToString(n);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to convert a double number to a descriptive string in Persian language. A return value indicates
        /// whether the operation succeeded or not.
        /// </summary>
        /// <param name="d">The double number to convert.</param>
        /// <param name="str">The string that holds the result.</param>
        /// <returns></returns>
        public static bool TryConvertNumberToPersianString(double d, out string str)
        {
            str = "";
            try
            {
                string strValue = d.ToString("F20");
                int dotIndex = strValue.IndexOf('.');
                if (dotIndex > 0)
                {
                    string preDot = strValue.Substring(0, dotIndex);
                    string postDot = MathUtils.RemoveTrailingZeros(strValue.Substring(dotIndex + 1));

                    int numPreDot, numPostDot;
                    if (!Int32.TryParse(preDot, out numPreDot))
                        return false;

                    if (postDot.Length > 0)
                    {
                        if (!Int32.TryParse(postDot, out numPostDot))
                            return false;

                        if (numPreDot != 0)
                        {
                            str += ToString(numPreDot) + " ممیز ";
                        }

                        str += String.Format("{0} {1}", ToString(numPostDot), MathUtils.CreateOrdinalNumber(ToString(Convert.ToInt64(Math.Pow(10, postDot.Length)))));
                    }
                    else
                    {
                        str += ToString(numPreDot);
                    }
                    return true;
                }
                else
                {
                    return TryConvertNumberToPersianString(Convert.ToInt64(d), out str);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts an integer number to its written form in Persian
        /// </summary>
        /// <param name="x">The integer to convert.</param>
        /// <returns></returns>
        public static string ToString(int x)
        {
            return (ToString((long) x));
        }

        /// <summary>
        /// Converts a long number to its written form in Persian
        /// </summary>
        /// <param name="x">The long integer number to convert</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToString(long x)
        {
            if (x > 999999999999999L)
                throw new ArgumentOutOfRangeException("Number is too large to process");

            long t;
            string result = string.Empty;
            string unit = "";

            if (x < 0L)
            {
                result = "منهای ";
                x = -x;
            }

            if (x == 0L)
            {
                if (PersianLiteral2NumMap.TryNum2PersianString(x, out result))
                {
                    return result;
                }
            }

            if (x > 999999999999L)
            {
                t = x / 1000000000000L;
                if (PersianLiteral2NumMap.TryNum2PersianString(1000000000000L, out unit))
                {
                    result += ConvertUpTo100(t) + " " + unit;
                    x = x - (t * 1000000000000L);

                    if (x <= 0)
                    {
                        return result;
                    }
                    else
                    {
                        result += " و ";
                    }
                }
            }


            if (x > 999999999L)
            {
                t = x / 1000000000L;
                if(PersianLiteral2NumMap.TryNum2PersianString(1000000000L, out unit))
                {
                    result += ConvertUpTo100(t) + " " + unit;
                    x = x - (t * 1000000000L);

                    if (x <= 0)
                    {
                        return result;
                    }
                    else
                    {
                        result += " و ";
                    }
                }
            }

            if (x > 999999)
            {
                t = x / 1000000L;
                if (PersianLiteral2NumMap.TryNum2PersianString(1000000L, out unit))
                {
                    result += ConvertUpTo100(t) + " " + unit;
                    x = x - (t * 1000000L);

                    if (x <= 0)
                    {
                        return result;
                    }
                    else
                    {
                        result += " و ";
                    }
                }
            }

            if (x > 999)
            {
                t = x / 1000;
                if (PersianLiteral2NumMap.TryNum2PersianString(1000L, out unit))
                {
                    if (t != 1)
                    {
                        result += ConvertUpTo100(t) + " ";
                    }
                    result += unit;

                    x = x - (t * 1000L);

                    if (x <= 0)
                    {
                        return result;
                    }
                    else
                    {
                        result += " و ";
                    }
                }
            }

            if (x > 0)
            {
                result += ConvertUpTo100(x);
            }

            return result;
        }

        /// <summary>
        /// Converts the number to its equivalant persian string. 
        /// The number should not have more than 3 digits.
        /// </summary>
        /// <param name="n">The number to convert</param>
        /// <returns></returns>
        private static string ConvertUpTo100(long n)
        {
            long t;
            string result = string.Empty;

            if (n > 999L)
                throw new ArgumentOutOfRangeException("Number is larger than 999");

            if (n > 99L)
            {
                t = n / 100;
                if (PersianLiteral2NumMap.TryNum2PersianString(t * 100, out result))
                {
                    n = n - (t * 100);

                    if (n <= 0)
                    {
                        return result;
                    }
                    else
                    {
                        result += " و ";
                    }
                }
            }

            if (n > 20)
            {
                t = n / 10;
                string tempResult = "";
                if (PersianLiteral2NumMap.TryNum2PersianString(t * 10, out tempResult))
                {
                    result = result + tempResult;
                    n = n - (t * 10);

                    if (n <= 0)
                    {
                        return result;
                    }
                    else
                    {
                        result += " و ";
                    }
                }
            }

            if (n > 0)
            {
                string tempStr = "";
                if (PersianLiteral2NumMap.TryNum2PersianString(n, out tempStr))
                {
                    result += tempStr;
                }
            }

            return result;
        }
    }
}

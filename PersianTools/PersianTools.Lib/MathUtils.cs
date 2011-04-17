using System;
using System.Text;

namespace SCICT.NLP.Utility
{
    /// <summary>
    /// Some mathematical utility methods, and string utility methods related to mathematics
    /// </summary>
    public class MathUtils
    {
        /// <summary>
        /// Determines whether the specified number is power of ten.
        /// </summary>
        /// <param name="n">The number</param>
        /// <returns>
        /// 	<c>true</c> if the specified number is power of ten; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTen(long n)
        {
            while (n % 10 == 0)
            {
                n /= 10;
            }

            if (n == 1) return true;
            else return false;
        }

        /// <summary>
        /// Gets the number of digits of the specified number.
        /// </summary>
        /// <param name="n">The number.</param>
        /// <returns></returns>
        public static int DigitCount(long n)
        {
            if (n < 0) n = -n;

            int count = 1;
            while (n >= 10)
            {
                n /= 10;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Removes the trailing zeros from the string representation of a number.
        /// Has some usage in dealing with the mantissa of numbers.
        /// </summary>
        /// <param name="p">The string representation of a number.</param>
        /// <returns></returns>
        public static string RemoveTrailingZeros(string p)
        {
            int len = p.Length;
            int i;
            for (i = len - 1; i >= 0; i--)
            {
                if (p[i] != '0')
                    break;
            }

            if (i >= 0) // i.e. loop breaked
            {
                return p.Substring(0, i + 1);
            }
            else // i.e. the string was all a bunch of zero characters
            {
                return "";
            }
        }

        /// <summary>
        /// Normalizes the string representation of a number that is converted to string with F20 format.
        /// It removes the trailing zeros, and if the mantissa consists all 
        /// of zeros the decimal point is also removed.
        /// </summary>
        /// <param name="p">The string representation of a number.</param>
        /// <returns></returns>
        public static string NormalizeForF20Format(string str)
        {
            string result = RemoveTrailingZeros(str);
            if (result.EndsWith("."))
                result = result.Substring(0, result.Length - 1);
            if (result.Length <= 0)
                result = "0";

            return result;
        }

        /// <summary>
        /// Inserts english thousand seperator characters in proper positions inside the string containig a number.
        /// </summary>
        /// <param name="str">The string containing a number.</param>
        /// <returns></returns>
        public static string InsertThousandSeperator(string str)
        {
            int len = str.Length;
            int dotIndex = str.IndexOf('.');
            if (dotIndex < 0) dotIndex = len;

            int startIndex = 0;
            for(startIndex = 0; startIndex < dotIndex; ++startIndex)
                if(Char.IsDigit(str[startIndex]))
                    break;

            if (startIndex >= dotIndex)
                return str;

            StringBuilder sb = new StringBuilder();

            if(startIndex > 0)
                sb.Append(str.Substring(0, startIndex)); // append everything before first digit

            if (Char.IsDigit(str[startIndex])) // append first digit also
                sb.Append(str[startIndex]);

            int digitsLeft = dotIndex - startIndex - 1;

            for (int i = 1; i <= digitsLeft; ++i)
            {
                if (((digitsLeft - i + 1) % 3) == 0)
                    sb.Append(",");
                sb.Append(str[startIndex+ i]);
            }

            if(dotIndex < len)
                sb.Append(str.Substring(dotIndex)); // append everything after and including the dot

            return sb.ToString();
        }

        /// <summary>
        /// Creates the ordinal string from the main number string. e.g. "سه" --> "سوم"
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string CreateOrdinalNumber(string word)
        {
            string result = "";

            if (word == "سه")
            {
                result = "سوم";
            }
            else if (word.EndsWith("ی"))
            {
                result = word + "‌ام";
            }
            else if (word.EndsWith("ن"))
            {
                result = word + "یم";
            }
            else
            {
                result = word + "م";
            }

            return result;
        }
    }
}

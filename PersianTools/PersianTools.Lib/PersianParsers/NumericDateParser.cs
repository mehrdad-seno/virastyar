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
using System.Text.RegularExpressions;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Provides the means to search some input string and finding and parsing 
    /// all occurrances of numeric forms of dates. e.g. 10/10/2007 or 1-7-74
    /// </summary>
    public class NumericDateParser
    {
        #region RegexPatterns

        /// <summary>
        /// Returns the regex pattern for the numeric dates
        /// </summary>
        /// <returns></returns>
        private string NumericDatePattern()
        {
            string seprators1Pat = @"(?<sep1>/|-|\.)";
            string seprators2Pat = @"(?<sep2>/|-|\.)";
            string datePattern = RegexPatternCreator.CreateGroup("NumericDate", @"(?<num1>\d+)", seprators1Pat, @"(?<num2>\d+)", seprators2Pat, @"(?<num3>\d+)");
            return @"\b" + datePattern + @"\b";
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Searches the specified string for patterns of numeric dates, and
        /// returns a sequnce of <see cref="NumericDatePatternInfo"/> that holds information about the pattern found.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <returns></returns>
        public NumericDatePatternInfo[] FindAndParse(string str)
        {
            List<NumericDatePatternInfo> l = new List<NumericDatePatternInfo>();

            NumericDatePatternInfo curInfo = null;

            Regex regex = new Regex(NumericDatePattern());
            foreach (Match m in regex.Matches(str))
            {
                if (CheckConsistency(m, str))
                {
                    curInfo = ExtractFromNumericDate(m);
                    if (curInfo != null)
                    {
                        l.Add(curInfo);
                    }
                }
            }

            return l.ToArray();
        }

        /// <summary>
        /// Checks the consistency of the found date pattern. 
        /// It first checks the consistancy of the seperators (see <see cref="CheckSeperators"/>).
        /// Then checks the consistancy of the date itself. e.g. 25.02.07 is a date but
        /// 25.02.07.05 might be an IP address. Thus patterns containing more than 3 digit sections
        /// are not considered as dates.
        /// </summary>
        /// <param name="m">The regex Match object containing numeric date.</param>
        /// <param name="str">The original string inside which the numeric date pattern was found.
        /// Actually it is the context of the found pattern.</param>
        /// <returns></returns>
        private bool CheckConsistency(Match m, string str)
        {
            string sep = "";
            if (!CheckSeperators(m, out sep))
                return false;

            if(m.Index + m.Length < str.Length)
            {
                string rem = str.Substring(m.Index + m.Length);

                if (rem.StartsWith(sep) && rem.Length > sep.Length)
                {
                    if (Char.IsDigit(rem[sep.Length]))
                        return false;
                }
                
            }

            return true;
        }

        /// <summary>
        /// Checks the consistency of seperators in a date string.
        /// e.g. 12/01/2007 and 12-01-2007 are consistant but 12-01/2007 is not.
        /// </summary>
        /// <param name="m">The regex match object containing the english date.</param>
        /// <param name="sep">The consistant seperator string (if any).</param>
        /// <returns></returns>
        private bool CheckSeperators(Match m, out string sep)
        {
            sep = "";
            string sep1 = "1";
            string sep2 = "2";

            try
            {
                foreach (Capture c in m.Groups["sep1"].Captures)
                {
                    sep1 = c.Value;
                }

                foreach (Capture c in m.Groups["sep2"].Captures)
                {
                    sep2 = c.Value;
                }

                sep = sep1;
                return sep1 == sep2;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Extracts

        /// <summary>
        /// Parses and extracts information from the found regex Match object 
        /// containing a numeric date.
        /// The parsed pattern info is returned, and null is returned if 
        /// the Match does not contain valid data.
        /// </summary>
        /// <param name="m">The regex Match object to be parsed.</param>
        /// <returns></returns>
        private NumericDatePatternInfo ExtractFromNumericDate(Match m)
        {
            int num1 = -1;
            int num2 = -1;
            int num3 = -1;

            num1 = ExtractNum(m, "num1");
            num2 = ExtractNum(m, "num2");
            num3 = ExtractNum(m, "num3");

            int year = -1, month = -1, day = -1;
            FindDateParts(num1, num2, num3, out year, out month, out day);

            return new NumericDatePatternInfo (m.Value, m.Index, m.Length, day , month, year);
        }

        /// <summary>
        /// Tries to guess the three main date parts from the provided three numbers.
        /// </summary>
        /// <param name="num1">The 1st number.</param>
        /// <param name="num2">The 2nd number.</param>
        /// <param name="num3">The 3rd number.</param>
        /// <param name="year">The year guessed.</param>
        /// <param name="month">The month guessed.</param>
        /// <param name="day">The day guessed.</param>
        private void FindDateParts(int num1, int num2, int num3, out int year, out int month, out int day)
        {
            year = -1;
            month = -1;
            day = -1;

            List<int> l = new List<int>(3);
            l.Add(num1);
            l.Add(num2);
            l.Add(num3);

            int yearIndex = -1;
            int i;
            for(i = l.Count - 1; i >= 0; --i)
            {
                if (l[i] >= 32)
                {
                    year = l[i];
                    yearIndex = i;
                    l.RemoveAt(i);
                    break;
                }
            }

            if (i < 0) // i.e. not found a proper year
            {
                year = l[l.Count - 1]; // assign the 3rd number as year
                yearIndex = l.Count - 1;
                l.RemoveAt(l.Count - 1);
            }

            int count = l.Count;
            // Now find day of month
            for(i = 0; i < count ; ++i)
            {
                if (12 < l[i] && l[i] < 32)
                {
                    day = l[i];
                    l.RemoveAt(i);
                    break;
                }
            }

            if (i >= count) // i.e. not found a proper year
            {
                if (yearIndex == 0)
                {
                    day = l[l.Count - 1];
                    l.RemoveAt(l.Count - 1);
                }
                else
                {
                    day = l[0];
                    l.RemoveAt(0);
                }
            }

            month = l[0];
        }

        /// <summary>
        /// Extracts and parses the number section of the specified 
        /// group name from the regex Match object.
        /// </summary>
        /// <param name="m">The regex Match object to be parsed.</param>
        /// <param name="grpName">Name of the (regex) group.</param>
        /// <returns></returns>
        private int ExtractNum(Match m, string grpName)
        {
            int dayNum = -1;

            foreach (Capture c in m.Groups[grpName].Captures)
            {
                string value = c.Value;
                if (Char.IsDigit(value[0]))
                {
                    int n;
                    value = ParsingUtils.ConvertNumber2English(value);
                    if (Int32.TryParse(value, out n))
                    {
                        dayNum = n;
                    }
                    else
                    {
                        dayNum = -1;
                    }
                }
            }

            return dayNum;
        }

        #endregion

        #region Weekdays

        /// <summary>
        /// Returns the Nth day of the week for numbers between 0 and 6 inclusive.
        /// Returns Illegal othewise.
        /// </summary>
        /// <param name="n">The number to return week-day based upon. 
        /// The number for Saturday is Zero.</param>
        /// <returns></returns>
        private Weekdays NthWeekday(int n)
        {
            switch (n)
            {
                case 0:
                    return Weekdays.Sat;
                case 1:
                    return Weekdays.Sun;
                case 2:
                    return Weekdays.Mon;
                case 3:
                    return Weekdays.Tue;
                case 4:
                    return Weekdays.Wed;
                case 5:
                    return Weekdays.Thu;
                case 6:
                    return Weekdays.Fri;
                default:
                    return Weekdays.Illeagal;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class to hold information about the parsed numeric date patterns.
    /// </summary>
    public class NumericDatePatternInfo : IPatternInfo
    {
        string content;

        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get { return content; }
        }

        private int index;

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private int length;

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return length; }
        }

        private int dayNumber;

        /// <summary>
        /// Gets the day number (in month).
        /// </summary>
        /// <value>The day number.</value>
        public int DayNumber
        {
            get { return dayNumber; }
        }

        private int monthNumber;

        /// <summary>
        /// Gets the month number.
        /// </summary>
        /// <value>The month number.</value>
        public int MonthNumber
        {
            get { return monthNumber; }
        }

        private int yearNumber;

        /// <summary>
        /// Gets the year number.
        /// </summary>
        /// <value>The year number.</value>
        public int YearNumber
        {
            get { return yearNumber; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericDatePatternInfo"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length of the pattern found.</param>
        /// <param name="dayNo">The day number (in month).</param>
        /// <param name="monthNo">The month number.</param>
        /// <param name="yearNo">The year number.</param>
        public NumericDatePatternInfo(string content, int index, int length, int dayNo, int monthNo, int yearNo)
        {
            this.content = content;
            this.index = index;
            this.length = length;
            this.dayNumber = dayNo;
            this.monthNumber = monthNo;
            this.yearNumber = yearNo;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Content:{1}{0}At:{2}{0}\tD:{3}{0}\tM:{4}{0}\tY:{5}{0}", Environment.NewLine,
                content, index, dayNumber, monthNumber, yearNumber);
        }

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.NumericDate; }
        }

    }

}

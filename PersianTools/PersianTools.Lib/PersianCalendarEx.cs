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
using System.Globalization;
using SCICT.NLP.Utility.Parsers;

namespace SCICT.NLP.Utility.Calendar
{
    /// <summary>
    /// Extension class for the Persian Calendar, to make working with this calendar more handy
    /// </summary>
    public class PersianCalendarEx
    {
        #region Protected Members

        /// <summary>
        /// The Gregorian date of the calendar.
        /// </summary>
        protected DateTime dt;

        /// <summary>
        /// The Persian Calendar object of the Framework
        /// </summary>
        protected PersianCalendar pc = new PersianCalendar();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianCalendarEx"/> class.
        /// </summary>
        /// <param name="dt">The date-time to be converted in Gregorian calendar.</param>
        public PersianCalendarEx(DateTime dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianCalendarEx"/> class from Gregorian date values.
        /// </summary>
        /// <param name="year">The year (in Jalali).</param>
        /// <param name="month">The month (in Jalali).</param>
        /// <param name="day">The day (in Jalali).</param>
        public PersianCalendarEx(int year, int month, int day)
        {
            this.dt = new DateTime(year, month, day, pc);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianCalendarEx"/> class from Gregorian date values.
        /// </summary>
        /// <param name="year">The year (in Jalali).</param>
        /// <param name="month">The month (in Jalali).</param>
        /// <param name="day">The day (in Jalali).</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="secs">The seconds.</param>
        public PersianCalendarEx(int year, int month, int day, int hour, int minute, int secs)
        {
            this.dt = new DateTime(year, month, day, hour, minute, secs, pc);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the date time object that holds the Gregorian representation of the current calendar object.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime
        {
            get
            {
                return dt;
            }
        }

        /// <summary>
        /// Gets a <see cref="PersianCalendarEx"/> instance that is set to the current date and time of the local machine.
        /// </summary>
        public static PersianCalendarEx Now
        {
            get
            {
                return new PersianCalendarEx(DateTime.Now);
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Gets the year in Jalali calendar.
        /// </summary>
        /// <returns></returns>
        public int GetYear()
        {
            return pc.GetYear(dt);
        }

        /// <summary>
        /// Returns the month (in the Jalali calendar)
        /// </summary>
        /// <returns></returns>
        public int GetMonth()
        {
            return pc.GetMonth(dt);
        }

        /// <summary>
        /// Gets the day of the month.
        /// </summary>
        /// <returns></returns>
        public int GetDayOfMonth()
        {
            return pc.GetDayOfMonth(dt);
        }

        /// <summary>
        /// Gets the day of week.
        /// </summary>
        /// <returns></returns>
        public DayOfWeek GetDayOfWeek()
        {
            return pc.GetDayOfWeek(dt);
        }

        /// <summary>
        /// Gets the hour value.
        /// </summary>
        /// <returns></returns>
        public int GetHour()
        {
            return pc.GetHour(dt);
        }

        /// <summary>
        /// Gets the minutes value.
        /// </summary>
        /// <returns></returns>
        public int GetMinute()
        {
            return pc.GetMinute(dt);
        }

        /// <summary>
        /// Gets the second value.
        /// </summary>
        /// <returns></returns>
        public int GetSecond()
        {
            return pc.GetSecond(dt);
        }

        /// <summary>
        /// Gets the milliseconds value.
        /// </summary>
        /// <returns></returns>
        public double GetMilliseconds()
        {
            return pc.GetMilliseconds(dt);
        }

        /// <summary>
        /// Gets the number of months in the specified year in the current era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <returns></returns>
        public int GetMonthsInYear(int year)
        {
            return pc.GetMonthsInYear(year);
        }

        /// <summary>
        /// Gets the day of the year.
        /// </summary>
        /// <returns></returns>
        public int GetDayOfYear()
        {
            return pc.GetDayOfYear(dt);
        }

        /// <summary>
        /// Calculates the leap month for a specified year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public int GetLeapMonth(int year)
        {
            return pc.GetLeapMonth(year);
        }

        /// <summary>
        /// Gets the era.
        /// </summary>
        /// <returns></returns>
        public int GetEra()
        {
            return pc.GetEra(dt);
        }

        /// <summary>
        /// Gets the number of days in th specified year of the current era.
        /// </summary>
        /// <param name="year">An integer representing year.</param>
        /// <returns></returns>
        public int GetDaysInYear(int year)
        {
            return pc.GetDaysInYear(year);
        }

        /// <summary>
        /// Gets the number of days in specified year and month of the current era.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="month">An integer representing the month.</param>
        /// <returns></returns>
        public int GetDaysInMonth(int year, int month)
        {
            return pc.GetDaysInMonth(year, month);
        }

        #endregion

        #region String Generation Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current Persian Calendar Format. 
        /// The format of the returned string is "yy/mm/dd".
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current Persian Calendar.
        /// </returns>
        public override string ToString()
        {
            return GetYear().ToString() + "/" + GetMonth().ToString() + "/" + GetDayOfMonth().ToString();

        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current Persian Calendar Format. 
        /// If the format string provided equals "D" then a string describing the current date in Persian language is returned.
        /// Otherwise, the format of the returned string would be "yy/mm/dd".
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            if (format == "D")
            {
                return CalendarStringUtils.GetPersianDateString(DateCalendarType.Jalali,
                    CalendarStringUtils.GetWeekdayFromDayOfWeek(GetDayOfWeek()),
                    GetDayOfMonth(), GetMonth(), GetYear());
            }
            else
            {
                return ToString();
            }
        }

        #endregion

        #region Date Parsing Methods

        /// <summary>
        /// Converts the string representation of a Persian Date, to a <see cref="PersianCalendarEx"/> Instance. A return 
        /// value indicates whether the operation succeeded. The string can contain a numberical representation of a date,
        /// or a literal date description in Persian language.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="pcOut">If the conversion operation succeeds this will hold the result, 
        /// otherwise it will be set to null.</param>
        /// <returns>true if the operation succeeds, otherwise false.</returns>
        public static bool TryParse(string str, out PersianCalendarEx pcOut)
        {
            PersianCalendarEx pc = TryParseLiteralDate(str);
            if (pc != null)
            {
                pcOut = pc;
                return true;
            }

            pc = TryParseNumericDate(str);
            if (pc != null)
            {
                pcOut = pc;
                return true;
            }

            pcOut = null;
            return false;
        }

        /// <summary>
        /// Converts the string representation of a Persian Date, to a <see cref="PersianCalendarEx"/> Instance. A return 
        /// value indicates whether the operation succeeded. The string can contain a literal date description in Persian language.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns></returns>
        protected static PersianCalendarEx TryParseLiteralDate(string str)
        {
            PersianDateParser pd = new PersianDateParser();
            PersianDatePatternInfo[] pis = pd.FindAndParse(str);

            DateTime dt;

            foreach (PersianDatePatternInfo pi in pis)
            {
                if (pi.CalendarType != DateCalendarType.Jalali && pi.CalendarType != DateCalendarType.Illegal) continue;

                if (pi.YearNumber >= 0 && pi.DayNumber > 0 && pi.MonthNumber > 0)
                {
                    try
                    {
                        dt = new DateTime(pi.YearNumber, pi.MonthNumber, pi.DayNumber, new PersianCalendar());
                        return new PersianCalendarEx(dt);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the string representation of a Persian Date, to a <see cref="PersianCalendarEx"/> Instance. A return 
        /// value indicates whether the operation succeeded. The string can contain a numberical representation of a date.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns></returns>
        protected static PersianCalendarEx TryParseNumericDate(string str)
        {
            NumericDateParser numericDateParser = new NumericDateParser();
            NumericDatePatternInfo[] patternInfos = numericDateParser.FindAndParse(str);

            DateTime dt;

            foreach (NumericDatePatternInfo pi in patternInfos)
            {
                if (pi.YearNumber >= 0 && pi.DayNumber > 0 && pi.MonthNumber > 0)
                {
                    try
                    {
                        dt = new DateTime(pi.YearNumber, pi.MonthNumber, pi.DayNumber, new PersianCalendar());
                        return new PersianCalendarEx(dt);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}

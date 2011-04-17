using System;
using System.Globalization;
using SCICT.NLP.Utility.Parsers;

namespace SCICT.NLP.Utility.Calendar
{
    /// <summary>
    /// Extension class for the Hijri Calendar, to make working with this calendar more handy
    /// </summary>
    public class HijriCalendarEx
    {
        #region Protected Members
        /// <summary>
        /// The Gregorian date of the calendar.
        /// </summary>
        DateTime dt;

        /// <summary>
        /// The Hijri Calendar object of the Framework
        /// </summary>
        HijriCalendar hc = new HijriCalendar();

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HijriCalendarEx"/> class.
        /// </summary>
        /// <param name="dt">The date-time to be converted in Gregorian calendar.</param>
        public HijriCalendarEx(DateTime dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HijriCalendarEx"/> class.
        /// </summary>
        /// <param name="year">The year (in Hijri Ghamari).</param>
        /// <param name="month">The month (in Hijri Ghamari).</param>
        /// <param name="day">The day (in Hijri Ghamari).</param>
        public HijriCalendarEx(int year, int month, int day)
        {
            this.dt = new DateTime(year, month, day, hc);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HijriCalendarEx"/> class.
        /// </summary>
        /// <param name="year">The year (in Hijri Ghamari).</param>
        /// <param name="month">The month (in Hijri Ghamari).</param>
        /// <param name="day">The day (in Hijri Ghamari).</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="secs">The seconds.</param>
        public HijriCalendarEx(int year, int month, int day, int hour, int minute, int secs)
        {
            this.dt = new DateTime(year, month, day, hour, minute, secs, hc);
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
        /// Gets a <see cref="HijriCalendarEx"/> instance that is set to the current date and time of the local machine.
        /// </summary>
        public static HijriCalendarEx Now
        {
            get
            {
                return new HijriCalendarEx(DateTime.Now);
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Gets the year in Hijri Ghamari calendar.
        /// </summary>
        /// <returns></returns>
        public int GetYear()
        {
            return hc.GetYear(dt);
        }

        /// <summary>
        /// Returns the month (in the Hijri Ghamari calendar)
        /// </summary>
        /// <returns></returns>
        public int GetMonth()
        {
            return hc.GetMonth(dt);
        }

        /// <summary>
        /// Gets the day of the month.
        /// </summary>
        /// <returns></returns>
        public int GetDayOfMonth()
        {
            return hc.GetDayOfMonth(dt);
        }

        /// <summary>
        /// Gets the day of week.
        /// </summary>
        /// <returns></returns>
        public DayOfWeek GetDayOfWeek()
        {
            return hc.GetDayOfWeek(dt);
        }

        /// <summary>
        /// Gets the hour value.
        /// </summary>
        /// <returns></returns>
        public int GetHour()
        {
            return hc.GetHour(dt);
        }

        /// <summary>
        /// Gets the minutes value.
        /// </summary>
        /// <returns></returns>
        public int GetMinute()
        {
            return hc.GetMinute(dt);
        }

        /// <summary>
        /// Gets the second value.
        /// </summary>
        /// <returns></returns>
        public int GetSecond()
        {
            return hc.GetSecond(dt);
        }

        /// <summary>
        /// Gets the milliseconds value.
        /// </summary>
        /// <returns></returns>
        public double GetMilliseconds()
        {
            return hc.GetMilliseconds(dt);
        }

        /// <summary>
        /// Gets the number of months in the specified year in the current era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <returns></returns>
        public int GetMonthsInYear(int year)
        {
            return hc.GetMonthsInYear(year);
        }

        /// <summary>
        /// Gets the day of the year.
        /// </summary>
        /// <returns></returns>
        public int GetDayOfYear()
        {
            return hc.GetDayOfYear(dt);
        }

        /// <summary>
        /// Calculates the leap month for a specified year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public int GetLeapMonth(int year)
        {
            return hc.GetLeapMonth(year);
        }

        /// <summary>
        /// Gets the era.
        /// </summary>
        /// <returns></returns>
        public int GetEra()
        {
            return hc.GetEra(dt);
        }

        /// <summary>
        /// Gets the number of days in th specified year of the current era.
        /// </summary>
        /// <param name="year">An integer representing year.</param>
        /// <returns></returns>
        public int GetDaysInYear(int year)
        {
            return hc.GetDaysInYear(year);
        }

        /// <summary>
        /// Gets the number of days in specified year and month of the current era.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="month">An integer representing the month.</param>
        /// <returns></returns>
        public int GetDaysInMonth(int year, int month)
        {
            return hc.GetDaysInMonth(year, month);
        }

        #endregion

        #region String Generation Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// The format of the returned string is "yy/mm/dd".
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return GetYear().ToString() + "/" + GetMonth().ToString() + "/" + GetDayOfMonth().ToString();

        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// If the format string provided equals "D" then a string describing the current date in Persian language is returned.
        /// Otherwise, the format of the returned string would be "yy/mm/dd".
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            if (format == "D")
            {
                return CalendarStringUtils.GetPersianDateString(DateCalendarType.HijriGhamari,
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
        /// Converts the string representation of a Hijri Ghamari Date, to a <see cref="HijriCalendarEx"/> Instance. A return 
        /// value indicates whether the operation succeeded. The string can contain a numberical representation of a date,
        /// or a literal date description in Persian language.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="pcOut">If the conversion operation succeeds this will hold the result, 
        /// otherwise it will be set to null.</param>
        /// <returns>true if the operation succeeds, otherwise false.</returns>
        public static bool TryParse(string str, out HijriCalendarEx pcOut)
        {
            HijriCalendarEx pc = TryParseLiteralDate(str);
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
        /// Converts the string representation of a Hijri Ghamari Date, to a <see cref="HijriCalendarEx"/> Instance. A return 
        /// value indicates whether the operation succeeded. The string can contain a literal date description in Persian language.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns></returns>
        protected static HijriCalendarEx TryParseLiteralDate(string str)
        {
            PersianDateParser pd = new PersianDateParser();
            PersianDatePatternInfo[] pis = pd.FindAndParse(str);

            foreach (PersianDatePatternInfo pi in pis)
            {
                if (pi.CalendarType != DateCalendarType.HijriGhamari && pi.CalendarType != DateCalendarType.Illegal) continue;

                if (pi.YearNumber >= 0 && pi.DayNumber > 0 && pi.MonthNumber > 0)
                {
                    try
                    {
                        return new HijriCalendarEx(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
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
        /// Converts the string representation of a Hijri Ghamari Date, to a <see cref="HijriCalendarEx"/> Instance. A return 
        /// value indicates whether the operation succeeded. The string can contain a numberical representation of a date.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns></returns>
        protected static HijriCalendarEx TryParseNumericDate(string str)
        {
            NumericDateParser pd = new NumericDateParser();
            NumericDatePatternInfo[] pis = pd.FindAndParse(str);

            foreach (NumericDatePatternInfo pi in pis)
            {
                if (pi.YearNumber >= 0 && pi.DayNumber > 0 && pi.MonthNumber > 0)
                {
                    try
                    {
                        return new HijriCalendarEx(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
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

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
    /// all occurrances of written-forms of dates in Persian language.
    /// By Persian Date we do not only mean Jalali date. It means all Jalali, 
    /// Gregorian, and Hijri Ghamari dates which are written in Persian Language.
    /// </summary>
    public class PersianDateParser
    {
        #region RegexPatterns

        /// <summary>
        /// Persian number parser to help parse written number parts in date descriptions
        /// </summary>
        private PersianNumberParser persianNumberParser = new PersianNumberParser();

        /// <summary>
        /// Returns regex pattern for week-day part of the date pattern
        /// </summary>
        /// <returns></returns>
        public string WeekdayPattern()
        {
            string weekdayDigits = "[12345۱۲۳۴۵]";
            string writtenNum = RegexPatternCreator.CreateGroup("WeekdayNum", RegexPatternCreator.CreateOR(false, "یک", "دو", "سه", "چهار", "پنج", weekdayDigits));

            string weekday =
            RegexPatternCreator.CreateGroup("Weekday",
                RegexPatternCreator.CreateOR(true,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.ClosureQuestionMark(
                            RegexPatternCreator.CreateGroup("", writtenNum, RegexPatternCreator.InWordWSStar)
                        ),
                        "شنبه"),
                    "جمعه"));

            return weekday;
        }


        /// <summary>
        /// Returns the regex pattern for the day-number (in month) part of the date description
        /// which can also be in persian written form.
        /// </summary>
        /// <returns></returns>
        private string DayNumPattern()
        {
            return RegexPatternCreator.CreateGroup("DayNum", persianNumberParser.ThreeDigitBlockPattern());
        }

        /// <summary>
        /// Returns the regex pattern for all possible month names in all the supported calendar types, ORed together.
        /// </summary>
        /// <returns></returns>
        private string MonthNamePattern()
        {
            return RegexPatternCreator.CreateGroup("MonthName",
                RegexPatternCreator.CreateOR(false,
                    "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "امرداد", "شهریور",
                    "مهر", "آبان", "ابان", "آذر", "دی", "بهمن", "اسفند", "اسپند",
                    "ژانویه", "جنوری", "فوریه", "فبروری", "مارس",
                    "مارچ","آوریل","آپریل","مه","می","ژوئن","جون","ژوئیه","ژوییه","جولای","ژولای"
                    , "اوت", "اگوست", "آگوست", "سپتامبر", "سپتمبر", "اکتبر", "نوامبر", "نومبر", "دسامبر", "دسبمر"
                    ,"محرم","محرم‌الحرام","محرم الحرام","صفر","ربیع‌الاول","ربیع الاول" 
                    ,"ربیع‌الثانی","ربیع الثانی","جمادی‌الاولی","جمادی‌الاول","جمادی الاولی","جمادی الاول"
                    ,"جمادی‌الثانی","جمادی الثانی","رجب","شعبان","رمضان","شوال","ذی‌القعده","ذی القعده","ذو‌القعده"
                    , "ذو القعده", "ذی‌الحجه", "ذوالحجه", "ذی الحجه", "ذو الحجه"));

        }

        /// <summary>
        /// Returns the regex pattern for the year part of the date,
        /// which can be either digits or written numbers in Persian.
        /// </summary>
        /// <returns></returns>
        private string YearPattern()
        {
            return RegexPatternCreator.CreateGroup("Year", 
                RegexPatternCreator.CreateOR(true,
                    @"\d+",
                    persianNumberParser.PersianNumberPattern()
                )
            );
        }

        /// <summary>
        /// Returns the complete regex date pattern in Persian.
        /// </summary>
        /// <returns></returns>
        private string PersianDatePattern()
        {
            string monthYearOptionalLiteral = RegexPatternCreator.CreateGroup("",
                RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.BetWordWSPlus, "ماه")),
                RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.BetWordWSPlus, "سال"))
            );

            string optionalWeekday = RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("", WeekdayPattern(), RegexPatternCreator.BetWordWSPlus));
            string optionalYear = RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("", RegexPatternCreator.BetWordWSPlus, YearPattern()));
            string datePattern = RegexPatternCreator.CreateGroup("PersianDate", optionalWeekday, DayNumPattern(), RegexPatternCreator.BetWordWSPlus, MonthNamePattern(), monthYearOptionalLiteral, optionalYear);
            return @"\b" + datePattern + @"\b";
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Searches the specified string for patterns of dates in a Persian descriptive string, and
        /// returns a sequnce of <see cref="PersianDatePatternInfo"/> that holds information about the pattern found.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <returns></returns>
        public PersianDatePatternInfo[] FindAndParse(string str)
        {
            List<PersianDatePatternInfo> l = new List<PersianDatePatternInfo>();

            Regex regex = new Regex(PersianDatePattern());
            foreach (Match m in regex.Matches(str))
                l.Add(ExtractFromPersianDate(m));

            return l.ToArray();
        }

        #endregion

        #region Extracts

        /// <summary>
        /// Extracts and parses date information from a regex Match instance.
        /// </summary>
        /// <param name="m">The regex Match instance containing date pattern.</param>
        /// <returns></returns>
        private PersianDatePatternInfo ExtractFromPersianDate(Match m)
        {
            Weekdays weekday = Weekdays.Illeagal;
            int dayNum = -1;
            int monthNum = -1;
            int yearNum = -1;

            DateCalendarType ct = DateCalendarType.Illegal;
            weekday = ExtractWeekday(m);
            dayNum = ExtractDayNum(m);
            monthNum = ExtractMonthNum(m);

            if (1 <= monthNum && monthNum <= 12)
            {
                ct = DateCalendarType.Jalali;
            }
            else if (13 <= monthNum && monthNum <= 24)
            {
                ct = DateCalendarType.Gregorian;
                monthNum -= 12;
            }
            else if (25 <= monthNum && monthNum <= 36)
            {
                ct = DateCalendarType.HijriGhamari;
                monthNum -= 24;
            }
            else
            {
                ct = DateCalendarType.Illegal;
            }

            yearNum = ExtractYearNum(m);

            return new PersianDatePatternInfo(m.Value, m.Index, m.Length, ct, weekday, dayNum, monthNum, yearNum);
        }

        /// <summary>
        /// Extracts and parses the year number from the given regex Match instance.
        /// </summary>
        /// <param name="m">The regex Match instance to extract year number from.</param>
        /// <returns></returns>
        private int ExtractYearNum(Match m)
        {
            int yearNum = -1;

            foreach (Capture c in m.Groups["Year"].Captures)
            {
                string value = c.Value;
                if (Char.IsDigit(value[0]))
                {
                    int n;
                    value = ParsingUtils.ConvertNumber2English(value);
                    if (Int32.TryParse(value, out n))
                    {
                        yearNum = n;
                    }
                    else
                    {
                        yearNum = -1;
                    }
                }
                else
                {
                    yearNum = -1;
                    foreach (PersianNumberPatternInfo pi in persianNumberParser.FindAndParse(c.Value))
                    {
                        yearNum = (int)pi.Number;
                    }
                }
            }

            return yearNum;
        }

        /// <summary>
        /// Extracts and parses the month number from the given regex Match instance.
        /// </summary>
        /// <param name="m">The regex Match instance to extract month number from.</param>
        /// <returns></returns>
        private int ExtractMonthNum(Match m)
        {
            int monthNum = -1;

            foreach (Capture c in m.Groups["MonthName"].Captures)
            {
                string value = c.Value;
                monthNum = PersianMonthNum(value);
            }

            return monthNum;
        }

        /// <summary>
        /// Extracts and parses the day number from the given regex Match instance.
        /// </summary>
        /// <param name="m">The regex Match instance to extract day number from.</param>
        /// <returns></returns>
        private int ExtractDayNum(Match m)
        {
            int dayNum = -1;

            foreach (Capture c in m.Groups["DayNum"].Captures)
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
                else
                {
                    dayNum = -1;
                    foreach (PersianNumberPatternInfo pi in persianNumberParser.FindAndParse(c.Value))
                    {
                        dayNum = (int)pi.Number;
                    }
                }
            }

            return dayNum;
        }

        /// <summary>
        /// Extracts and parses the week-day from the given regex Match instance.
        /// </summary>
        /// <param name="m">The regex Match instance to extract week-day from.</param>
        /// <returns></returns>
        private Weekdays ExtractWeekday(Match m)
        {
            Weekdays w = Weekdays.Illeagal;

            foreach (Capture c in m.Groups["Weekday"].Captures)
            {
                if (c.Value == "جمعه")
                {
                    w = Weekdays.Fri;
                }
                if (c.Value == "شنبه")
                {
                    w = Weekdays.Sat;
                }
                else
                {
                    foreach (Capture cc in m.Groups["WeekdayNum"].Captures)
                    {
                        string value = cc.Value;
                        if (Char.IsDigit(value[0]))
                        {
                            int n;
                            value = ParsingUtils.ConvertNumber2English(value);
                            if (Int32.TryParse(value, out n))
                            {
                                w = NthWeekday(n);
                            }
                            else
                            {
                                w = Weekdays.Illeagal;
                            }
                        }
                        else
                        {
                            Int64 n;
                            if (PersianLiteral2NumMap.TryPersianString2Num(value, out n))
                            {
                                w = NthWeekday((int)n);
                            }
                            else
                            {
                                w = Weekdays.Illeagal;
                            }
                        }
                    }
                }
            }

            return w;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the month number from the input string. The month number ranges vary for different calendar types.
        /// The return value ranges are as follows:
        /// -1      Illegal
        /// 1 - 12  Jalali
        /// 13 - 24 Gregorian
        /// 25 - 36 HijriGhamari
        /// </summary>
        private int PersianMonthNum(string str)
        {
            str = ParsingUtils.NormalizeSpaces(str);

            switch (str)
            {
                case "فروردین":
                    return 1;
                case "اردیبهشت":
                    return 2;
                case "خرداد":
                    return 3;
                case "تیر":
                    return 4;
                case "مرداد":
                    return 5;
                case "امرداد":
                    return 5;
                case "شهریور":
                    return 6;
                case "مهر":
                    return 7;
                case "آبان":
                    return 8;
                case "ابان":
                    return 8;
                case "آذر":
                    return 9;
                case "دی":
                    return 10;
                case "بهمن":
                    return 11;
                case "اسفند":
                    return 12;
                case "اسپند":
                    return 12;
                // Gregorian Begins Here
                case "ژانویه":
                    return 13;
                case "جنوری":
                    return 13;
                case "فوریه":
                    return 14;
                case "فبروری":
                    return 14;
                case "مارس":
                    return 15;
                case "مارچ":
                    return 15;
                case "آوریل":
                    return 16;
                case "آپریل":
                    return 16;
                case "مه":
                    return 17;
                case "می":
                    return 17;
                case "ژوئن":
                    return 18;
                case "جون":
                    return 18;
                case "ژوئیه":
                    return 19;
                case "ژوییه":
                    return 19;
                case "جولای":
                    return 19;
                case "ژولای":
                    return 19;
                case "اوت":
                    return 20;
                case "اگوست":
                    return 20;
                case "آگوست":
                    return 20;
                case "سپتامبر":
                    return 21;
                case "سپتمبر":
                    return 21;
                case "اکتبر":
                    return 22;
                case "نوامبر":
                    return 23;
                case "نومبر":
                    return 23;
                case "دسامبر":
                    return 24;
                case "دسبمر":
                    return 24;
                // Hijri Ghamari Begins Here
                case "محرم":
                    return 25;
                case "محرم‌الحرام":
                    return 25;
                case "محرم الحرام":
                    return 25;
                case "صفر":
                    return 26;
                case "ربیع‌الاول":
                    return 27;
                case "ربیع الاول":
                    return 27;
                case "ربیع‌الثانی":
                    return 28;
                case "ربیع الثانی":
                    return 28;
                case "جمادی‌الاولی":
                    return 29;
                case "جمادی‌الاول":
                    return 29;
                case "جمادی الاولی":
                    return 29;
                case "جمادی الاول":
                    return 29;
                case "جمادی‌الثانی":
                    return 30;
                case "جمادی الثانی":
                    return 30;
                case "رجب":
                    return 31;
                case "شعبان":
                    return 32;
                case "رمضان":
                    return 33;
                case "شوال":
                    return 34;
                case "ذی‌القعده":
                    return 35;
                case "ذی القعده":
                    return 35;
                case "ذو‌القعده":
                    return 35;
                case "ذو القعده":
                    return 35;
                case "ذی‌الحجه":
                    return 36;
                case "ذوالحجه":
                    return 36;
                case "ذی الحجه":
                    return 36;
                case "ذو الحجه":
                    return 36;
                default:
                    return -1;
            }
        }

        #endregion

        #region Methods about Weekdays

        /// <summary>
        /// Returns the  Nth day of the week for numbers between 0 and 6 inclusive.
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
    /// Class to hold information about the parsed persian date patterns
    /// </summary>
    public class PersianDatePatternInfo : IPatternInfo
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

        private DateCalendarType calType;

        /// <summary>
        /// Gets the type of the calendar.
        /// </summary>
        /// <value>The type of the calendar.</value>
        public DateCalendarType CalendarType
        {
            get { return calType; }
        }

        private Weekdays weekday;

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        /// <value>The weekday.</value>
        public Weekdays Weekday
        {
            get { return weekday; }
        }

        private int dayNumber;

        /// <summary>
        /// Gets the day number in the month.
        /// </summary>
        /// <value>The day number in the month.</value>
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
        /// Initializes a new instance of the <see cref="PersianDatePatternInfo"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length of the found pattern.</param>
        /// <param name="t">The calendar type.</param>
        /// <param name="w">The day of the week.</param>
        /// <param name="dayNo">The day number (in month).</param>
        /// <param name="monthNo">The month number.</param>
        /// <param name="yearNo">The year number.</param>
        public PersianDatePatternInfo(string content, int index, int length, DateCalendarType t, Weekdays w, int dayNo, int monthNo, int yearNo)
        {
            this.content = content;
            this.index = index;
            this.length = length;
            this.calType = t;
            this.weekday = w;
            this.dayNumber = dayNo;
            this.monthNumber = monthNo;
            this.yearNumber = yearNo;
        }


        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.PersianDate; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Content:{1}{0}At:{2}{0}\tT:{7}{0}\tW:{3}{0}\tD:{4}{0}\tM:{5}{0}\tY:{6}{0}", Environment.NewLine,
                content, index, weekday, dayNumber, monthNumber, yearNumber, calType );
        }
    }

    /// <summary>
    /// Enumeration for the week-days
    /// </summary>
    public enum Weekdays
    {
        /// <summary>
        /// week-day not defined
        /// </summary>
        Illeagal,
        /// <summary>
        /// Saturday
        /// </summary>
        Sat,
        /// <summary>
        /// Sunday
        /// </summary>
        Sun,
        /// <summary>
        /// Monday
        /// </summary>
        Mon,
        /// <summary>
        /// Tuesday
        /// </summary>
        Tue,
        /// <summary>
        /// Wednesday
        /// </summary>
        Wed,
        /// <summary>
        /// Thursday
        /// </summary>
        Thu,
        /// <summary>
        /// Friday
        /// </summary>
        Fri
    }

    /// <summary>
    /// Enumerates different calendar types supported by this library
    /// </summary>
    public enum DateCalendarType
    {
        /// <summary>
        /// Calendar-Type not defined
        /// </summary>
        Illegal,
        /// <summary>
        /// Gregorian Calendar Type
        /// </summary>
        Gregorian,
        /// <summary>
        /// Jalali Calendar Type
        /// </summary>
        Jalali,
        /// <summary>
        /// Hijri Ghamari Calendar Type
        /// </summary>
        HijriGhamari
    }

}

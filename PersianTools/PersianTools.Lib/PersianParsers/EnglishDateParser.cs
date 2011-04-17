using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Provides the means to search some input string and finding and parsing 
    /// all occurrances of written-forms of dates in English language.
    /// By English Date we do not only mean Gregorian date. It means all Jalali, 
    /// Gregorian, and Hijri Ghamari dates which are written in English Language.
    /// </summary>
    public class EnglishDateParser
    {
        #region Regex Patterns

        /// <summary>
        /// Returns the regex pattern for the Week-day part in an English date.
        /// </summary>
        /// <returns></returns>
        public string WeekdayPattern()
        {
            string weekday =
            RegexPatternCreator.CreateGroup("Weekday",
                RegexPatternCreator.CreateOR(false,
                    "saturday", "sunday", "monday", "tuesday", "wednesday", "thursday", "friday",
                    "sat", "sun", "mon", "tue", "wed", "thu", "fri",
                    "sat\\.", "sun\\.", "mon\\.", "tue\\.", "wed\\.", "thu\\.", "fri\\."));

            return weekday;
        }


        /// <summary>
        /// Returns the regex pattern for the day number (in month) in an English date, which can only be digits.
        /// </summary>
        /// <returns></returns>
        private string DayNumPattern()
        {
            return RegexPatternCreator.CreateGroup("",
                RegexPatternCreator.CreateGroup("DayNum", @"\d\d?"),
                RegexPatternCreator.ClosureQuestionMark("th|st|rd|nd") );
        }

        /// <summary>
        /// Returns the regex pattern for all possible month-names in all possible calendar types in English language, ORed together.
        /// </summary>
        /// <returns></returns>
        private string MonthNamePattern()
        {
            return RegexPatternCreator.CreateGroup("MonthName",
                RegexPatternCreator.CreateOR(true,

                    "january", "jan", "jan\\.", "february", "feb", "feb\\.", "march", "mar", "mar\\.", "april", "apr", "apr\\.", "may",
                    "june", "jun", "jun\\.", "july", "jul", "jul\\.", "august", "aug", "aug\\.", "september", "sep", "sep\\.", "october", "oct", "oct\\.",
                    "november", "nov", "nov\\.", "december", "dec", "dec\\.",

                    "farvardin", "ordibehesht", "khordād", "khordad", "tir", "mordād", "amordād", "mordad",
                    "amordad", "shahrivar", "mehr", "aban", "aban", "āzar", "azar", "dey", "bahman", "esfand", "espand",

                    "muharram", "muḥarram ul ḥaram", "muharram ul haram", "safar", "ṣafar ul muzaffar", "safar ul muzaffar",
                    "rabi' al-awwal", "rabi' al-thani", "jumada al-ula", "jumada al-thani", "rajab",
                    "rajab al murajab", "sha'aban", "sha'abān ul moazam", "sha'aban ul moazam", "ramadan", "ramazān",
                    "ramaḍān ul mubarak", "ramadan ul mubarak", "ramazan", "ramazan ul mubarak", "shawwal",
                    "shawwal ul mukarram", "dhu al-qi'dah", "dhu al-hijjah"));
        }

        /// <summary>
        /// Returns the regex pattern for the year part of an English date, which can only be digits.
        /// </summary>
        /// <returns></returns>
        private string YearPattern()
        {
            return RegexPatternCreator.CreateGroup("Year",
                    @"\d+"
            );
        }

        /// <summary>
        /// Returns the complete regex pattern for an English date.
        /// </summary>
        /// <returns></returns>
        private string EnglishDatePattern()
        {
            string optionalWeekday = 
                RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("", 
                                                    WeekdayPattern(), 
                                                    RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.BetWordWSPlus), 
                                                    RegexPatternCreator.ClosureQuestionMark(","),
                                                    RegexPatternCreator.BetWordWSPlus
                                                ));
            string optionalYear = 
                RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("", 
                    RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.BetWordWSPlus), 
                    RegexPatternCreator.ClosureQuestionMark(","), 
                    RegexPatternCreator.BetWordWSPlus,
                    YearPattern()));
            string datePattern = RegexPatternCreator.CreateGroup("EnglishDate", optionalWeekday, MonthNamePattern(), RegexPatternCreator.BetWordWSPlus, DayNumPattern(), optionalYear);


            return @"\b" + datePattern + @"\b";
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Searches the specified string for patterns of English dates, and
        /// returns a sequnce of <see cref="EnglishDatePatternInfo"/> that holds information about the pattern found.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <returns></returns>
        public EnglishDatePatternInfo[] FindAndParse(string str)
        {
            List<EnglishDatePatternInfo> l = new List<EnglishDatePatternInfo>();

            Regex regex = new Regex(EnglishDatePattern(), RegexOptions.IgnoreCase);
            foreach (Match m in regex.Matches(str))
                l.Add(ExtractFromEnglishDate(m));

            return l.ToArray();
        }

        #endregion
        
        #region Extracts

        /// <summary>
        /// Parses and extracts information from the found regex match object 
        /// containing an English date.
        /// The parsed pattern info is returned, and null is returned if 
        /// the Match does not contain valid data.
        /// </summary>
        /// <param name="m">The regex match object to be parsed.</param>
        /// <returns></returns>
        private EnglishDatePatternInfo ExtractFromEnglishDate(Match m)
        {
            Weekdays weekday = Weekdays.Illeagal;
            int dayNum = -1;
            int monthNum = -1;
            int yearNum = -1;

            weekday = ExtractWeekday(m);
            dayNum = ExtractDayNum(m);
            monthNum = ExtractMonthNum(m);

            DateCalendarType ct = DateCalendarType.Illegal;
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

            return new EnglishDatePatternInfo(m.Value, m.Index, m.Length, ct, weekday, dayNum, monthNum, yearNum);
        }

        /// <summary>
        /// Extracts and parses the year number part.
        /// </summary>
        /// <param name="m">The regex Match object to parse.</param>
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
            }

            return yearNum;
        }

        /// <summary>
        /// Extracts and parses the Month number part.
        /// </summary>
        /// <param name="m">The regex Match object to parse.</param>
        /// <returns></returns>
        private int ExtractMonthNum(Match m)
        {
            int monthNum = -1;
            
            foreach (Capture c in m.Groups["MonthName"].Captures)
            {
                string value = c.Value;
                monthNum = EnglishMonthNum(value);
            }

            return monthNum;
        }

        /// <summary>
        /// Extracts and parses the day number.
        /// </summary>
        /// <param name="m">The regex Match object to parse.</param>
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
            }

            return dayNum;
        }

        /// <summary>
        /// Extracts and parses the week-day.
        /// </summary>
        /// <param name="m">The regex Match object to parse.</param>
        /// <returns></returns>
        private Weekdays ExtractWeekday(Match m)
        {
            Weekdays w = Weekdays.Illeagal;

            foreach (Capture c in m.Groups["Weekday"].Captures)
            {
                w = EnglishWeekday(c.Value);
            }

            return w;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the day of the week from the given string containing week-day in English.
        /// Returns Illegal if the specified string does not contain valid week-day.
        /// </summary>
        /// <param name="str">The string containing day of the week in English.</param>
        /// <returns></returns>
        private Weekdays EnglishWeekday(string str)
        {
            str = str.ToLower();

            switch (str)
            {
                case "saturday":
                    return Weekdays.Sat ;
                case "sunday":
                    return Weekdays.Sun ;
                case "monday":
                    return Weekdays.Mon;
                case "tuesday":
                    return Weekdays.Tue;
                case "wednesday":
                    return Weekdays.Wed;
                case "thursday":
                    return Weekdays.Thu;
                case "friday":
                    return Weekdays.Fri;
                case "sat":
                case "sat.":
                    return Weekdays.Sat;
                case "sun":
                case "sun.":
                    return Weekdays.Sun;
                case "mon":
                case "mon.":
                    return Weekdays.Mon;
                case "tue":
                case "tue.":
                    return Weekdays.Tue;
                case "wed":
                case "wed.":
                    return Weekdays.Wed;
                case "thu":
                case "thu.":
                    return Weekdays.Thu;
                case "fri":
                case "fri.":
                    return Weekdays.Fri;
                default:
                    return Weekdays.Illeagal;
            }
        }

        /// <summary>
        /// Returns the month number from the input string. The month number ranges vary for different calendar types.
        /// The return value ranges are as follows:
        /// -1      Illegal
        /// 1 - 12  Jalali
        /// 13 - 24 Gregorian
        /// 25 - 36 HijriGhamari
        /// </summary>
        private int EnglishMonthNum(string str)
        {
            // the items that end in dot are for the abbreviated form of the dates.
            str = ParsingUtils.NormalizeSpaces( str.ToLower() );

            switch (str)
            {
                case "farvardin":
                    return 1;
                case "ordibehesht":
                    return 2;
                case "khordād":
                    return 3;
                case "khordad":
                    return 3;
                case "tir":
                    return 4;
                case "mordād":
                    return 5;
                case "amordād":
                    return 5;
                case "mordad":
                    return 5;
                case "amordad":
                    return 5;
                case "shahrivar":
                    return 6;
                case "mehr":
                    return 7;
                case "ābān":
                    return 8;
                case "aban":
                    return 8;
                case "āzar":
                    return 9;
                case "azar":
                    return 9;
                case "dey":
                    return 10;
                case "bahman":
                    return 11;
                case "esfand":
                    return 12;
                case "espand":
                    return 12;

                // Gregorian Begins Here
                case "january":
                case "jan":
                case "jan.":
                    return 13;
                case "february":
                case "feb":
                case "feb.":
                    return 14;
                case "march":
                case "mar":
                case "mar.":
                    return 15;
                case "april":
                case "apr":
                case "apr.":
                    return 16;
                case "may":
                    return 17;
                case "june":
                case "jun":
                case "jun.":
                    return 18;
                case "july":
                case "jul":
                case "jul.":
                    return 19;
                case "august":
                case "aug":
                case "aug.":
                    return 20;
                case "september":
                case "sep":
                case "sep.":
                    return 21;
                case "october":
                case "oct":
                case "oct.":
                    return 22;
                case "november":
                case "nov":
                case "nov.":
                    return 23;
                case "december":
                case "dec":
                case "dec.":
                    return 24;

                // Hijri Ghamari Begins Here
                case "muharram":
                case "muḥarram ul ḥaram":
                case "muharram ul haram":
                    return 25;
                case "safar":
                case "ṣafar ul muzaffar":
                case "safar ul muzaffar":
                    return 26;
                case "rabi' al-awwal":
                    return 27;
                case "rabi' al-thani":
                    return 28;
                case "jumada al-ula":
                    return 29;
                case "jumada al-thani":
                    return 30;
                case "rajab":
                    return 31;
                case "rajab al murajab":
                    return 31;
                case "sha'aban":
                    return 32;
                case "sha'abān ul moazam":
                    return 32;
                case "sha'aban ul moazam":
                    return 32;
                case "ramadan":
                    return 33;
                case "ramazān":
                    return 33;
                case "ramaḍān ul mubarak":
                    return 33;
                case "ramadan ul mubarak":
                    return 33;
                case "ramazan":
                    return 33;
                case "ramazan ul mubarak":
                    return 33;
                case "shawwal":
                    return 34;
                case "shawwal ul mukarram":
                    return 34;
                case "dhu al-qi'dah":
                    return 35;
                case "dhu al-hijjah":
                    return 36;
                default:
                    return -1;
            }
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
    /// Class to hold information about the parsed English date patterns.
    /// </summary>
    public class EnglishDatePatternInfo : IPatternInfo
    {
        readonly string m_content;

        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The m_content.</value>
        public string Content
        {
            get { return m_content; }
        }

        private int m_index;

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        private readonly int m_length;

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return m_length; }
        }

        private readonly DateCalendarType m_calType;

        /// <summary>
        /// Gets the type of the calendar.
        /// </summary>
        /// <value>The type of the calendar.</value>
        public DateCalendarType CalendarType
        {
            get { return m_calType; }
        }

        private readonly Weekdays m_weekday;

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        /// <value>The day of the week.</value>
        public Weekdays Weekday
        {
            get { return m_weekday; }
        }

        private readonly int m_dayNumber;

        /// <summary>
        /// Gets the day number (in month).
        /// </summary>
        /// <value>The day number.</value>
        public int DayNumber
        {
            get { return m_dayNumber; }
        }

        private readonly int m_monthNumber;

        /// <summary>
        /// Gets the month number.
        /// </summary>
        /// <value>The month number.</value>
        public int MonthNumber
        {
            get { return m_monthNumber; }
        }

        private readonly int m_yearNumber;

        /// <summary>
        /// Gets the year number.
        /// </summary>
        /// <value>The year number.</value>
        public int YearNumber
        {
            get { return m_yearNumber; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnglishDatePatternInfo"/> class.
        /// </summary>
        /// <param name="content">The m_content.</param>
        /// <param name="index">The m_index.</param>
        /// <param name="length">The length of the pattern found.</param>
        /// <param name="t">The type of the calendar.</param>
        /// <param name="w">The day of the week.</param>
        /// <param name="dayNo">The day number (in month).</param>
        /// <param name="monthNo">The month number.</param>
        /// <param name="yearNo">The year number.</param>
        public EnglishDatePatternInfo(string content, int index, int length, DateCalendarType t, Weekdays w, int dayNo, int monthNo, int yearNo)
        {
            this.m_content = content;
            this.m_index = index;
            this.m_length = length;
            this.m_calType = t;
            this.m_weekday = w;
            this.m_dayNumber = dayNo;
            this.m_monthNumber = monthNo;
            this.m_yearNumber = yearNo;
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
                m_content, m_index, m_weekday, m_dayNumber, m_monthNumber, m_yearNumber, m_calType);
        }

        #region IPatternInfo Members

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.EnglishDate; }
        }

        #endregion
    }

}

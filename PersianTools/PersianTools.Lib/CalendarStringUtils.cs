using System;
using System.Text;
using SCICT.NLP.Utility.Parsers;

namespace SCICT.NLP.Utility.Calendar
{
    /// <summary>
    /// Some string utility functions for creating date-strings.
    /// </summary>
    public class CalendarStringUtils
    {
        #region Static Fields

        private static string Farvardin = "فروردین";
        private static string Ordibehesht = "ارديبهشت";
        private static string Khordad = "خرداد";
        private static string Tir = "تير";
        private static string Mordad = "مرداد";
        private static string Shahrivar = "شهریور";
        private static string Mehr = "مهر";
        private static string Aban = "آبان";
        private static string Azar = "آذر";
        private static string Day = "دی";
        private static string Bahman = "بهمن";
        private static string Esfand = "اسفند";

        private static string PerJanuary = "ژانویه";
        private static string PerFebruary = "فوریه";
        private static string PerMarch = "مارس";
        private static string PerApril = "آوریل";
        private static string PerMay = "می";
        private static string PerJune = "ژوئن";
        private static string PerJuly = "جولای";
        private static string PerAugust = "آگوست";
        private static string PerSeptember = "سپتامبر";
        private static string PerOctober = "اکتبر";
        private static string PerNovember = "نوامبر";
        private static string PerDecember = "دسامبر";

        private static string PerMoharram = "محرم";
        private static string PerSafar = "صفر";
        private static string PerRabiAvval = "ربیع الاول";
        private static string PerRabiThani = "ربیع الثانی";
        private static string PerJamadiAvval = "جمادی الاول";
        private static string PerJamadiThani = "جمادی الثانی";
        private static string PerRajab = "رجب";
        private static string PerShaban = "شعبان";
        private static string PerRamazan = "رمضان";
        private static string PerShavval = "شوال";
        private static string PerZuGhade = "ذی القعده";
        private static string PerZuHajja = "ذی الحجه";


        private static string Shanbeh = "شنبه";
        private static string Yekshanbeh = "یک‌شنبه";
        private static string Doshanbeh = "دوشنبه";
        private static string Seshanbeh = "سه‌شنبه";
        private static string Chaharshanbeh = "چهارشنبه";
        private static string Panjshanbeh = "پنج‌شنبه";
        private static string Jomeh = "جمعه";

        #endregion

        /// <summary>
        /// Gets the name of the Nth Jalali month in Presian.
        /// </summary>
        /// <param name="n">The month number.</param>
        public static string GetPresianJalaliMonthName(int n)
        {
            switch (n)
            {
                case 1:
                    return Farvardin;
                case 2:
                    return Ordibehesht;
                case 3:
                    return Khordad;
                case 4:
                    return Tir;
                case 5:
                    return Mordad;
                case 6:
                    return Shahrivar;
                case 7:
                    return Mehr;
                case 8:
                    return Aban;
                case 9:
                    return Azar;
                case 10:
                    return Day;
                case 11:
                    return Bahman;
                case 12:
                    return Esfand;
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the name of the Nth Gregorian month in Persian.
        /// </summary>
        /// <param name="n">The month number.</param>
        /// <returns></returns>
        public static string GetPersianGregorianMonthName(int n)
        {
            switch (n)
            {
                case 1:
                    return PerJanuary;
                case 2:
                    return PerFebruary;
                case 3:
                    return PerMarch;
                case 4:
                    return PerApril;
                case 5:
                    return PerMay;
                case 6:
                    return PerJune;
                case 7:
                    return PerJuly;
                case 8:
                    return PerAugust;
                case 9:
                    return PerSeptember;
                case 10:
                    return PerOctober;
                case 11:
                    return PerNovember;
                case 12:
                    return PerDecember;
                default:
                    return "";
            }

        }

        /// <summary>
        /// Gets the name of the Nth Ghamari month in Persian .
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static string GetPersianGhamariMonthName(int n)
        {
            switch (n)
            {
                case 1:
                    return PerMoharram;
                case 2:
                    return PerSafar;
                case 3:
                    return PerRabiAvval;
                case 4:
                    return PerRabiThani;
                case 5:
                    return PerJamadiAvval;
                case 6:
                    return PerJamadiThani;
                case 7:
                    return PerRajab;
                case 8:
                    return PerShaban;
                case 9:
                    return PerRamazan;
                case 10:
                    return PerShavval;
                case 11:
                    return PerZuGhade;
                case 12:
                    return PerZuHajja;
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the name of the weekday in Persian.
        /// </summary>
        /// <param name="w">The weekday to get its name</param>
        /// <returns></returns>
        public static string GetPersianWeekdayName(Weekdays w)
        {
            switch (w)
            {
                case Weekdays.Sat:
                    return Shanbeh;
                case Weekdays.Sun:
                    return Yekshanbeh;
                case Weekdays.Mon:
                    return Doshanbeh;
                case Weekdays.Tue:
                    return Seshanbeh;
                case Weekdays.Wed:
                    return Chaharshanbeh;
                case Weekdays.Thu:
                    return Panjshanbeh;
                case Weekdays.Fri:
                    return Jomeh;
                case Weekdays.Illeagal:
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the <see cref="Weekdays"/> value for the <see cref="DayOfWeek"/>.
        /// <see cref="DayOfWeek"/> is an enum defined in the System namespace of the .NET Framework.
        /// </summary>
        /// <param name="d">The day of the week.</param>
        /// <returns></returns>
        public static Weekdays GetWeekdayFromDayOfWeek(DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Friday:
                    return Weekdays.Fri;
                case DayOfWeek.Monday:
                    return Weekdays.Mon;
                case DayOfWeek.Saturday:
                    return Weekdays.Sat;
                case DayOfWeek.Sunday:
                    return Weekdays.Sun;
                case DayOfWeek.Thursday:
                    return Weekdays.Thu;
                case DayOfWeek.Tuesday:
                    return Weekdays.Tue;
                case DayOfWeek.Wednesday:
                    return Weekdays.Wed;
                default:
                    return Weekdays.Illeagal;
            }
        }

        /// <summary>
        /// Gets the persian date string from its Gregorian equivalant.
        /// The string is a descriptive statement in Persian Language.
        /// </summary>
        /// <param name="dt">The date-time instance for the Gregorian date.</param>
        /// <returns></returns>
        public static string GetPersianDateString(DateTime dt)
        {
            return GetPersianDateString(DateCalendarType.Gregorian,
                GetWeekdayFromDayOfWeek(dt.DayOfWeek),
                dt.Day, dt.Month, dt.Year);
        }

        /// <summary>
        /// Gets a date description string from the specified values and calendar types.
        /// The string is a descriptive statement in Persian Language.
        /// </summary>
        /// <param name="t">The type of calendar.</param>
        /// <param name="w">The day of the week.</param>
        /// <param name="dayNumber">The day of the month.</param>
        /// <param name="monthNum">The month number.</param>
        /// <param name="yearNum">The year number.</param>
        /// <returns></returns>
        public static string GetPersianDateString(DateCalendarType t, Weekdays w, int dayNumber, int monthNum, int yearNum )
        {
            string strWeekday = GetPersianWeekdayName(w);
            string strMonth = "";
            switch (t)
            {
                case DateCalendarType.Gregorian:
                    strMonth = GetPersianGregorianMonthName(monthNum);
                    break;
                case DateCalendarType.Jalali:
                    strMonth = GetPresianJalaliMonthName(monthNum);
                    break;
                case DateCalendarType.HijriGhamari:
                    strMonth = GetPersianGhamariMonthName(monthNum);
                    break;
            }

            string day = "";
            if (0 < dayNumber && dayNumber < 35)
                day = NumberToPersianString.ToString(dayNumber);

            string year = "";
            if (0 <= yearNum)
                year = NumberToPersianString.ToString(yearNum);

            StringBuilder sb = new StringBuilder();
            if (strWeekday.Length > 0)
                sb.Append(strWeekday + " ");
            if (day.Length > 0)
                sb.Append(day + " ");
            if (strMonth.Length > 0)
                sb.Append(strMonth + " ");
            if (year.Length > 0)
                sb.Append(year + " ");

            return sb.ToString().Trim();
        }
    }
}

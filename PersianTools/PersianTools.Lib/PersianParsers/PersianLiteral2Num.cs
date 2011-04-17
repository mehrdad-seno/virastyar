using System;
using System.Collections.Generic;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Utility class that helps convert string of 
    /// Persian literals of long numbers to long numbers, and vice versa.
    /// </summary>
    class PersianLiteral2NumMap
    {
        /// <summary>
        /// Dictionary that maps string of numbers in Persian to their long value. 
        /// </summary>
        private static Dictionary<string, long> perStr2IntTable = new Dictionary<string, long>();

        /// <summary>
        /// Dictionary that maps long values to the their string in Persian.
        /// </summary>
        private static Dictionary<long, string> perInt2StrTable = new Dictionary<long, string>();

        /// <summary>
        /// Initializes the Persian string tables (i.e. dictionaries).
        /// </summary>
        private static void InitPersianStringTable()
        {
            perStr2IntTable.Add("صفر", 0L);
            perStr2IntTable.Add("صفرم", 0L);

            perStr2IntTable.Add("یک", 1L);
            perStr2IntTable.Add("دو", 2L);
            perStr2IntTable.Add("سه", 3L);
            perStr2IntTable.Add("چهار", 4L);
            perStr2IntTable.Add("پنج", 5L);
            perStr2IntTable.Add("پون", 5L);
            perStr2IntTable.Add("پان", 5L);
            perStr2IntTable.Add("شش", 6L);
            perStr2IntTable.Add("شیش", 6L);
            perStr2IntTable.Add("هفت", 7L);
            perStr2IntTable.Add("هشت", 8L);
            perStr2IntTable.Add("نه", 9L);

            perStr2IntTable.Add("یکم", 1L);
            perStr2IntTable.Add("اول", 1L);
            perStr2IntTable.Add("دوم", 2L);
            perStr2IntTable.Add("سوم", 3L);
            perStr2IntTable.Add("چهارم", 4L);
            perStr2IntTable.Add("پنجم", 5L);
            perStr2IntTable.Add("ششم", 6L);
            perStr2IntTable.Add("هفتم", 7L);
            perStr2IntTable.Add("هشتم", 8L);
            perStr2IntTable.Add("نهم", 9L);

            perStr2IntTable.Add("ده", 10L);
            perStr2IntTable.Add("یازده", 11L);
            perStr2IntTable.Add("دوازده", 12L);
            perStr2IntTable.Add("سیزده", 13L);
            perStr2IntTable.Add("چهارده", 14L);
            perStr2IntTable.Add("پانزده", 15L);
            perStr2IntTable.Add("پونزده", 15L);
            perStr2IntTable.Add("شونزده", 16L);
            perStr2IntTable.Add("شانزده", 16L);
            perStr2IntTable.Add("هفده", 17L);
            perStr2IntTable.Add("هیفده", 17L);
            perStr2IntTable.Add("هجده", 18L);
            perStr2IntTable.Add("هیجده", 18L);
            perStr2IntTable.Add("هژده", 18L);
            perStr2IntTable.Add("هیژده", 18L);
            perStr2IntTable.Add("نوزده", 19L);

            perStr2IntTable.Add("بیست", 20L);
            perStr2IntTable.Add("سی", 30L);
            perStr2IntTable.Add("چهل", 40L);
            perStr2IntTable.Add("پنجاه", 50L);
            perStr2IntTable.Add("شصت", 60L);
            perStr2IntTable.Add("هفتاد", 70L);
            perStr2IntTable.Add("هشتاد", 80L);
            perStr2IntTable.Add("نود", 90L);

            perStr2IntTable.Add("یکصد", 100L);
            perStr2IntTable.Add("صد", 100L);
            perStr2IntTable.Add("دویست", 200L);
            perStr2IntTable.Add("سیصد", 300L);
            perStr2IntTable.Add("چهارصد", 400L);
            perStr2IntTable.Add("چارصد", 400L);
            perStr2IntTable.Add("پانصد", 500L);
            perStr2IntTable.Add("پونصد", 500L);
            perStr2IntTable.Add("ششصد", 600L);
            perStr2IntTable.Add("شیصد", 600L);
            perStr2IntTable.Add("هفتصد", 700L);
            perStr2IntTable.Add("هشتصد", 800L);
            perStr2IntTable.Add("نهصد", 900L);

            perStr2IntTable.Add("هزار", 1000L);
            perStr2IntTable.Add("ملیون", 1000000L);
            perStr2IntTable.Add("میلیون", 1000000L);
            perStr2IntTable.Add("ملیارد", 1000000000L);
            perStr2IntTable.Add("میلیارد", 1000000000L);
            perStr2IntTable.Add("بلیون", 1000000000L);
            perStr2IntTable.Add("بیلیون", 1000000000L);
            perStr2IntTable.Add("ترلیارد", 1000000000000L);
            perStr2IntTable.Add("تریلیارد", 1000000000000L);
            perStr2IntTable.Add("تریلیون", 1000000000000L);
            perStr2IntTable.Add("ترلیون", 1000000000000L);

            perInt2StrTable.Add(0L, "صفر");
            perInt2StrTable.Add(1L, "یک");
            perInt2StrTable.Add(2L, "دو");
            perInt2StrTable.Add(3L, "سه");
            perInt2StrTable.Add(4L, "چهار");
            perInt2StrTable.Add(5L, "پنج");
            perInt2StrTable.Add(6L, "شش");
            perInt2StrTable.Add(7L, "هفت");
            perInt2StrTable.Add(8L, "هشت");
            perInt2StrTable.Add(9L, "نه");

            perInt2StrTable.Add(10L, "ده");
            perInt2StrTable.Add(11L, "یازده");
            perInt2StrTable.Add(12L, "دوازده");
            perInt2StrTable.Add(13L, "سیزده");
            perInt2StrTable.Add(14L, "چهارده");
            perInt2StrTable.Add(15L, "پانزده");
            perInt2StrTable.Add(16L, "شانزده");
            perInt2StrTable.Add(17L, "هفده");
            perInt2StrTable.Add(18L, "هجده");
            perInt2StrTable.Add(19L, "نوزده");

            perInt2StrTable.Add(20L, "بیست");
            perInt2StrTable.Add(30L, "سی");
            perInt2StrTable.Add(40L, "چهل");
            perInt2StrTable.Add(50L, "پنجاه");
            perInt2StrTable.Add(60L, "شصت");
            perInt2StrTable.Add(70L, "هفتاد");
            perInt2StrTable.Add(80L, "هشتاد");
            perInt2StrTable.Add(90L, "نود");

            perInt2StrTable.Add(100L, "صد");
            perInt2StrTable.Add(200L, "دویست");
            perInt2StrTable.Add(300L, "سیصد");
            perInt2StrTable.Add(400L, "چهارصد");
            perInt2StrTable.Add(500L, "پانصد");
            perInt2StrTable.Add(600L, "ششصد");
            perInt2StrTable.Add(700L, "هفتصد");
            perInt2StrTable.Add(800L, "هشتصد");
            perInt2StrTable.Add(900L, "نهصد");

            perInt2StrTable.Add(1000L,          "هزار");
            perInt2StrTable.Add(1000000L,       "میلیون");
            perInt2StrTable.Add(1000000000L,    "میلیارد");
            perInt2StrTable.Add(1000000000000L, "تریلیارد");
        }

        /// <summary>
        /// Initializes the <see cref="PersianLiteral2NumMap"/> class and fills the dictionaries.
        /// </summary>
        static PersianLiteral2NumMap()
        {
            InitPersianStringTable();
        }

        /// <summary>
        /// Tries to convert a Persian string containing litteral form of a number to its equivalant value.
        /// A return value indicates whether the operation succeeded or not.
        /// </summary>
        /// <param name="str">The string containing litteral form of a number to convert.</param>
        /// <param name="n">The converted number.</param>
        /// <returns></returns>
        public static bool TryPersianString2Num(string str, out Int64 n)
        {
            n = -1;

            object o;
            try
            {
                o = perStr2IntTable[str];
            }
            catch
            {
                return false;
            }

            if (o != null)
            {
                n = (Int64)o;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Tries to convert a long number to its equivalant Persian string containing litteral form of that number.
        /// A return value indicates whether the operation succeeded or not.
        /// </summary>
        /// <param name="n">The number to convert.</param>
        /// <param name="str">The converted string containing litteral form of a number to convert.</param>
        /// <returns></returns>
        public static bool TryNum2PersianString(long n, out string str)
        {
            str = "";

            object o;
            try
            {
                o = perInt2StrTable[n];
            }
            catch
            {
                return false;
            }

            if (o != null)
            {
                str = (string)o;
                return true;
            }
            else
                return false;
        }


    }
}

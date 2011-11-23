using System;
using System.Collections.Generic;
using System.Text;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;

namespace SCICT.NLP.Persian
{
    /// <summary>
    /// Filter for the Persian characters that provide means for replacing non-standard characters with their standard ones.
    /// </summary>
    public class PersianCharFilter : ICharFilter
    {
        /// <summary>
        /// dictionary that maps character codes (i.e. their integer value) to their refined string.
        /// </summary>
        protected Dictionary<int, string> m_dicCharFilterings = new Dictionary<int,string>();

        /// <summary>
        /// dictionary that maps character codes to their filtering category that the character belongs
        /// </summary>
        protected Dictionary<int, FilteringCharacterCategory> m_dicCharCategories = new Dictionary<int, FilteringCharacterCategory>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianCharFilter"/> class. And fills the 
        /// data-structures holding filtering data in a hard-coded way.
        /// </summary>
        public PersianCharFilter()
        {
            // Change to Persian Kaaf
            m_dicCharFilterings.Add(0x06AA, StringUtil.StringFromCodes(StandardCharacters.StandardKaaf));
            m_dicCharFilterings.Add(0x0643, StringUtil.StringFromCodes(StandardCharacters.StandardKaaf));
            m_dicCharFilterings.Add(0xFEDA, StringUtil.StringFromCodes(StandardCharacters.StandardKaaf));
            m_dicCharFilterings.Add(0xFED9, StringUtil.StringFromCodes(StandardCharacters.StandardKaaf));

            AddCodesToCategory(FilteringCharacterCategory.Kaaf, 0x06AA, 0x0643, 0xFEDA, 0xFED9);

            // Change to Persian Yaa
            m_dicCharFilterings.Add(0x0649, StringUtil.StringFromCodes(StandardCharacters.StandardYaa));
            m_dicCharFilterings.Add(0x064A, StringUtil.StringFromCodes(StandardCharacters.StandardYaa));
            m_dicCharFilterings.Add(0xFEF1, StringUtil.StringFromCodes(StandardCharacters.StandardYaa));
            m_dicCharFilterings.Add(0xFEF2, StringUtil.StringFromCodes(StandardCharacters.StandardYaa));

            AddCodesToCategory(FilteringCharacterCategory.Yaa, 0x0649, 0x064A, 0xFEF1, 0xFEF2);

            // Change arabic digits to their persian counter-part
            // 0x0660 to 0x0669 -->  0x06F0 to 0x06F9
            m_dicCharFilterings.Add(0x0660, StringUtil.StringFromCodes(StandardCharacters.StandardDigit0));
            m_dicCharFilterings.Add(0x0661, StringUtil.StringFromCodes(StandardCharacters.StandardDigit1));
            m_dicCharFilterings.Add(0x0662, StringUtil.StringFromCodes(StandardCharacters.StandardDigit2));
            m_dicCharFilterings.Add(0x0663, StringUtil.StringFromCodes(StandardCharacters.StandardDigit3));
            m_dicCharFilterings.Add(0x0664, StringUtil.StringFromCodes(StandardCharacters.StandardDigit4));
            m_dicCharFilterings.Add(0x0665, StringUtil.StringFromCodes(StandardCharacters.StandardDigit5));
            m_dicCharFilterings.Add(0x0666, StringUtil.StringFromCodes(StandardCharacters.StandardDigit6));
            m_dicCharFilterings.Add(0x0667, StringUtil.StringFromCodes(StandardCharacters.StandardDigit7));
            m_dicCharFilterings.Add(0x0668, StringUtil.StringFromCodes(StandardCharacters.StandardDigit8));
            m_dicCharFilterings.Add(0x0669, StringUtil.StringFromCodes(StandardCharacters.StandardDigit9));

            AddCodesToCategory(FilteringCharacterCategory.ArabicDigit, 
                0x0660, 0x0661, 0x0662, 0x0663, 0x0664, 
                0x0665, 0x0666, 0x0667, 0x0668, 0x0669 );

            // Change Half-Spaces (i.e. Zero-Width-Non-Jointers) to 0x200C
            m_dicCharFilterings.Add(0x200B, StringUtil.StringFromCodes(StandardCharacters.StandardHalfSpace));
            m_dicCharFilterings.Add(0x00AC, StringUtil.StringFromCodes(StandardCharacters.StandardHalfSpace));
            m_dicCharFilterings.Add(0x001F, StringUtil.StringFromCodes(StandardCharacters.StandardHalfSpace));
            m_dicCharFilterings.Add(0x200D, StringUtil.StringFromCodes(StandardCharacters.StandardHalfSpace));
            m_dicCharFilterings.Add(0x200E, StringUtil.StringFromCodes(StandardCharacters.StandardHalfSpace));
            m_dicCharFilterings.Add(0x200F, StringUtil.StringFromCodes(StandardCharacters.StandardHalfSpace));

            AddCodesToCategory(FilteringCharacterCategory.HalfSpace, 0x200B, 0x00AC, 0x001F, 0x200D, 0x200E, 0x200F);

            // Change Erabs to standard erab
            m_dicCharFilterings.Add(0xE818, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardFathatan));
            m_dicCharFilterings.Add(0xE820, StringUtil.StringFromCodes(StandardCharacters.StandardFatha));
            m_dicCharFilterings.Add(0xE821, StringUtil.StringFromCodes(StandardCharacters.StandardZamma));
            m_dicCharFilterings.Add(0xE822, StringUtil.StringFromCodes(StandardCharacters.StandardSaaken));
            m_dicCharFilterings.Add(0xE823, StringUtil.StringFromCodes(StandardCharacters.StandardFathatan));
            m_dicCharFilterings.Add(0xE824, StringUtil.StringFromCodes(StandardCharacters.StandardZammatan));
            m_dicCharFilterings.Add(0xE825, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid));
            m_dicCharFilterings.Add(0xE826, StringUtil.StringFromCodes(StandardCharacters.StandardKasra));
            m_dicCharFilterings.Add(0xE827, StringUtil.StringFromCodes(StandardCharacters.StandardKasratan));
            m_dicCharFilterings.Add(0xE828, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardFatha));
            m_dicCharFilterings.Add(0xE829, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardZamma));
            m_dicCharFilterings.Add(0xE82A, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardFathatan));
            m_dicCharFilterings.Add(0xE82B, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardZammatan));
            m_dicCharFilterings.Add(0xE82C, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardKasra));
            m_dicCharFilterings.Add(0xE82D, StringUtil.StringFromCodes(StandardCharacters.StandardTashdid, StandardCharacters.StandardKasratan));
            //m_dicCharFilterings.Add(0x06C0, HeYe.StandardShortHeYe);
            //m_dicCharFilterings.Add(0x06C2, HeYe.StandardShortHeYe);

            AddCodesToCategory(FilteringCharacterCategory.Erab, 
                0xE818, 0xE820, 0xE821, 0xE822, 0xE823, 0xE824,
                0xE825, 0xE826, 0xE827, 0xE828, 0xE829, 0xE82A,
                0xE82B, 0xE82C, 0xE82D);
            //, 0x06C0, 0x06C2);

            m_dicCharFilterings.Add(0xFE8D, "ا");
            m_dicCharFilterings.Add(0xFE81, "آ");
            m_dicCharFilterings.Add(0xFE83, "أ");
            m_dicCharFilterings.Add(0xFE85, "ؤ");
            m_dicCharFilterings.Add(0xFE87, "إ");
            m_dicCharFilterings.Add(0xFE8F, "ب");
            m_dicCharFilterings.Add(0xFE93, "ۀ");
            m_dicCharFilterings.Add(0xFE95, "ت");
            m_dicCharFilterings.Add(0xFE99, "ث");
            m_dicCharFilterings.Add(0xFE9D, "ج");
            m_dicCharFilterings.Add(0xFEA1, "ح");
            m_dicCharFilterings.Add(0xFEA5, "خ");
            m_dicCharFilterings.Add(0xFEA9, "د");
            m_dicCharFilterings.Add(0xFEAB, "ذ");
            m_dicCharFilterings.Add(0xFEAD, "ر");
            m_dicCharFilterings.Add(0xFEAF, "ز");
            m_dicCharFilterings.Add(0xFEB1, "س");
            m_dicCharFilterings.Add(0xFEB5, "ش");
            m_dicCharFilterings.Add(0xFEB9, "ص");
            m_dicCharFilterings.Add(0xFEBD, "ض");
            m_dicCharFilterings.Add(0xFEC1, "ط");
            m_dicCharFilterings.Add(0xFEC5, "ظ");
            m_dicCharFilterings.Add(0xFEC9, "ع");
            m_dicCharFilterings.Add(0xFECD, "غ");
            m_dicCharFilterings.Add(0xFED1, "ف");
            m_dicCharFilterings.Add(0xFED5, "ق");
            //dicCharFilterings.Add(0xFED9, "ک");
            m_dicCharFilterings.Add(0xFEDD, "ل");
            m_dicCharFilterings.Add(0xFEE1, "م");
            m_dicCharFilterings.Add(0xFEE5, "ن");
            m_dicCharFilterings.Add(0xFEE9, "ه");
            m_dicCharFilterings.Add(0xFEED, "و");
            m_dicCharFilterings.Add(0xFEEF, "ی");
            //dicCharFilterings.Add(0xFEF1, "ی");
        }

        /// <summary>
        /// Adds a sequence of character codes to a filtering category
        /// </summary>
        /// <param name="category">The filtering category.</param>
        /// <param name="codes">The codes.</param>
        private void AddCodesToCategory(FilteringCharacterCategory category, params int[] codes)
        {
            foreach (int code in codes)
            {
                m_dicCharCategories.Add(code, category);
            }
        }

        /// <summary>
        /// Filters the char and returns the string for its filtered (i.e. standardized) equivalant.
        /// The string may contain 0, 1, or more characters.
        /// If the length of the string is 0, then the character should have been left out.
        /// If the length of the string is 1, then the character might be left intact or replaced with another character.
        /// If the length of the string is more than 1, then there have been no 1-character replacement for this character.
        /// It is replaced with 2 or more characters. e.g. some fonts have encoded Tashdid, and Tanvin in one character.
        /// To make it standard this character is replaced with 2 characters, one for Tashdid, and the other for Tanvin.
        /// </summary>
        /// <param name="ch">The character to filter.</param>
        /// <returns></returns>
        public string FilterChar(char ch)
        {
            int nCh = Convert.ToInt32(ch);
            string result;
            return m_dicCharFilterings.TryGetValue(nCh, out result) ? result : ch.ToString();
        }

        /// <summary>
        /// Filters the char and returns the string for its filtered (i.e. standardized) equivalant.
        /// The string may contain 0, 1, or more characters.
        /// If the length of the string is 0, then the character should have been left out.
        /// If the length of the string is 1, then the character might be left intact or replaced with another character.
        /// If the length of the string is more than 1, then there have been no 1-character replacement for this character.
        /// It is replaced with 2 or more characters. e.g. some fonts have encoded Tashdid, and Tanvin in one character.
        /// To make it standard this character is replaced with 2 characters, one for Tashdid, and the other for Tanvin.
        /// </summary>
        /// <param name="ch">The character to filter.</param>
        /// <param name="ignoreCats">The filtering categories to be ignored.</param>
        /// <returns></returns>
        public string FilterChar(char ch, FilteringCharacterCategory ignoreCats)
        {
            int nCh = Convert.ToInt32(ch);
            FilteringCharacterCategory cat;
            if (m_dicCharCategories.TryGetValue(nCh, out cat))
            {
                return (cat & ignoreCats) == cat ? ch.ToString() : FilterChar(ch);
            }

            return FilterChar(ch);
        }

        /// <summary>
        /// Filters the char and returns the string for its filtered (i.e. standardized) equivalant.
        /// The string may contain 0, 1, or more characters.
        /// If the length of the string is 0, then the character should have been left out.
        /// If the length of the string is 1, then the character might be left intact or replaced with another character.
        /// If the length of the string is more than 1, then there have been no 1-character replacement for this character.
        /// It is replaced with 2 or more characters. e.g. some fonts have encoded Tashdid, and Tanvin in one character.
        /// To make it standard this character is replaced with 2 characters, one for Tashdid, and the other for Tanvin.
        /// </summary>
        /// <param name="ch">The character to filter.</param>
        /// <param name="ignoreList">The characters to be ignored.</param>
        /// <returns></returns>
        public string FilterChar(char ch, HashSet<char> ignoreList)
        {
            if(ignoreList.Contains(ch))
                return ch.ToString();

            return FilterChar(ch);
        }

        /// <summary>
        /// Filters the char and returns the string for its filtered (i.e. standardized) equivalant.
        /// The string may contain 0, 1, or more characters.
        /// If the length of the string is 0, then the character should have been left out.
        /// If the length of the string is 1, then the character might be left intact or replaced with another character.
        /// If the length of the string is more than 1, then there have been no 1-character replacement for this character.
        /// It is replaced with 2 or more characters. e.g. some fonts have encoded Tashdid, and Tanvin in one character.
        /// To make it standard this character is replaced with 2 characters, one for Tashdid, and the other for Tanvin.
        /// </summary>
        /// <param name="ch">The character to filter.</param>
        /// <param name="ignoreList">The characters to be ignored.</param>
        /// <param name="ignoreCats">The filtering categories to be ignored.</param>
        /// <returns></returns>
        public string FilterChar(char ch, HashSet<char> ignoreList, FilteringCharacterCategory ignoreCats)
        {
            if (ignoreList.Contains(ch))
                return ch.ToString();

            return FilterChar(ch, ignoreCats);
        }

        /// <summary>
        /// Filters every character of the string and returns the filtered string. To see how each character is 
        /// filtered see: <see cref="FilterChar(char)"/>
        /// </summary>
        /// <param name="str">The string to be filtered.</param>
        /// <returns></returns>
        public string FilterString(string str)
        {
            var sb = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                sb.Append(FilterChar(c));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Filters every character of the string and returns the filtered string. To see how each character is 
        /// filtered see: <see cref="FilterChar(char)"/>
        /// </summary>
        /// <param name="str">The string to be filtered.</param>
        /// <param name="ignoreCats">The filtering categories to be ignored.</param>
        /// <returns></returns>
        public string FilterString(string str, FilteringCharacterCategory ignoreCats)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                sb.Append(FilterChar(c, ignoreCats));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Filters every character of the string and returns the filtered string. To see how each character is 
        /// filtered see: <see cref="FilterChar(char)"/>
        /// </summary>
        /// <param name="str">The string to be filtered.</param>
        /// <param name="ignoreList">The characters to be ignored.</param>
        /// <returns></returns>
        public string FilterString(string str, HashSet<char> ignoreList)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                sb.Append(FilterChar(c, ignoreList));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Filters every character of the string and returns the filtered string. To see how each character is 
        /// filtered see: <see cref="FilterChar(char)"/>
        /// </summary>
        /// <param name="str">The string to be filtered.</param>
        /// <param name="ignoreList">The characters to be ignored.</param>
        /// <param name="ignoreCats">The filtering categories to be ignored.</param>
        /// <returns></returns>
        public string FilterString(string str, HashSet<char> ignoreList, FilteringCharacterCategory ignoreCats)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                sb.Append(FilterChar(c, ignoreList, ignoreCats));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Filters the given string with the given options while returning the filtered string as well as filtering statistics.
        /// </summary>
        /// <param name="str">The string to filter.</param>
        /// <param name="ignoreList">list of characters to ignore.</param>
        /// <param name="ignoreCats">The character-categories to ignore.</param>
        /// <returns></returns>
        public FilterResultsWithStats FilterStringWithStats(string str, HashSet<char> ignoreList, FilteringCharacterCategory ignoreCats)
        {
            var sbResult = new StringBuilder(str.Length);

            int numLetters = 0;
            int numErabs = 0;
            int numDigits = 0;
            int numHlafSpaces = 0;

            foreach(var ch in str)
            {
                if (ignoreList != null && ignoreList.Contains(ch))
                    sbResult.Append(ch);

                int nCh = Convert.ToInt32(ch);
                string filterResult;
                // if there's something to filter
                if(m_dicCharFilterings.TryGetValue(nCh, out filterResult))
                {
                    // try to find the filtering category
                    FilteringCharacterCategory cat;
                    if (m_dicCharCategories.TryGetValue(nCh, out cat))
                    {
                        if ((cat & ignoreCats) == cat)
                        {
                            sbResult.Append(ch);
                        }
                        else
                        {
                            sbResult.Append(filterResult);

                            switch (cat)
                            {
                                case FilteringCharacterCategory.ArabicDigit:
                                    numDigits++;
                                    break;
                                case FilteringCharacterCategory.Erab:
                                    numErabs++;
                                    break;
                                case FilteringCharacterCategory.HalfSpace:
                                    numHlafSpaces++;
                                    break;
                                case FilteringCharacterCategory.Kaaf:
                                    numLetters++;
                                    break;
                                case FilteringCharacterCategory.Yaa:
                                    numLetters++;
                                    break;
                            }

                        }
                    }
                    else // if there's no filtering category specified for the given character
                    {
                        numLetters++;
                    }
                }
                else // there's no need to filter the given character
                {
                    sbResult.Append(ch);
                }
            } // end of foreach char in str

            return new FilterResultsWithStats(sbResult.ToString(), numLetters, numDigits, numErabs, numHlafSpaces);
            
        }
    }

    /// <summary>
    /// The main filtering categories used by Persian Char Filters to replace non-standard characters,
    /// with their standard equivalents.
    /// </summary>
    [Flags]
    public enum FilteringCharacterCategory
    {
        /// <summary>
        /// Filter all kinds of Kaaf
        /// </summary>
        Kaaf = 0x0001,
        /// <summary>
        /// Filter all kinds of Yaa
        /// </summary>
        Yaa = 0x0002,
        /// <summary>
        /// Filter all kinds of Half-space
        /// </summary>
        HalfSpace = 0x0004,
        /// <summary>
        /// Filter arabic digits, and replaces them with their Persian counter-part.
        /// </summary>
        ArabicDigit = 0x0008,
        /// <summary>
        /// Filter all kinds of Erabs. Some fonts have their own customized 
        /// erab characters, which are considered as non-standard.
        /// </summary>
        Erab = 0x0010
    }
}

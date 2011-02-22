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
    /// Provides the means to search some input string and find and parse
    /// all occurrances of written-forms of persian integer numbers.
    /// </summary>
    public class PersianNumberParser
    {
        #region Regex Pattern Creators

        /// <summary>
        /// Returns the whole pattern of a persian written number
        /// </summary>
        internal string PersianNumberPattern()
        {
            string ThreeDB = ThreeDigitBlockPattern();
            string BlockAndMult = BlockPlusMultiplier();

            string MultPlus3DB =
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.ClosurePlus(
                        RegexPatternCreator.CreateGroup("",
                            BlockAndMult, RegexPatternCreator.BetWordWSPlus, "و", RegexPatternCreator.BetWordWSPlus
                        )
                    ),
                    RegexPatternCreator.ClosureQuestionMark(
                        ThreeDB
                    )
                );

            string MultPlus =
                RegexPatternCreator.CreateGroup("",
                    BlockAndMult, 
                    RegexPatternCreator.ClosureStar(RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.BetWordWSPlus, "و", RegexPatternCreator.BetWordWSPlus, BlockAndMult
                    )),
                    RegexPatternCreator.ClosureQuestionMark(RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.BetWordWSPlus, "و", RegexPatternCreator.BetWordWSPlus, ThreeDB
                    ))
                );

            string numberPattern = RegexPatternCreator.CreateGroup("",
                RegexPatternCreator.CreateOR(true, MultPlus, ThreeDB )
                );

            return @"\b" + RegexPatternCreator.CreateOR(false, numberPattern, "(صفرم)", "(صفر)") + @"\b";
        }

        /// <summary>
        /// Returns the pattern of 3-digit blocks plus the multipliers.
        /// Named As MULTIPLIER_GROUP_NAME
        /// </summary>
        private string BlockPlusMultiplier()
        {
            return
                RegexPatternCreator.CreateGroup(MULTIPLIER_GROUP_NAME,
                    RegexPatternCreator.CreateOR(true, 
                        RegexPatternCreator.CreateGroup("",
                            ThreeDigitBlockPattern(), RegexPatternCreator.BetWordWSPlus, MultipliersPattern()
                        ),
                        "هزار"
                    )
                );
        }

        /// <summary>
        /// Returns the regex pattern of three-digit-blocks 
        /// (i.e. umbers lower than 1000 which are main building blocks of larger numbers).
        /// </summary>
        /// <returns></returns>
        internal string ThreeDigitBlockPattern()
        {
            string numberPattern = RegexPatternCreator.CreateGroup(THREEDB_NAME,
                RegexPatternCreator.CreateOR(false,
                    AllUpToOnesAndTenOnes(), AllUpToTens(), HundsPattern(),@"\d\d?\d?"
                )
            );

            return @"\b" + numberPattern + @"\b";
        }

        /// <summary>
        /// Returns the regex pattern for the Hundreds and Tens (not lower, and not between). e.g. 110, 920 (NOT 117).
        /// </summary>
        /// <returns></returns>
        private string AllUpToTens()
        {
            string hundsGroup = RegexPatternCreator.ClosureQuestionMark(
                RegexPatternCreator.CreateGroup("", HundsPattern(),
                        RegexPatternCreator.BetWordWSPlus, "و", RegexPatternCreator.BetWordWSPlus
                    )
                );

            string numberPattern = RegexPatternCreator.CreateGroup("", hundsGroup,TensPattern());
            return @"\b" + numberPattern + OrdinalityPattern() + @"\b";
        }

        /// <summary>
        /// Returns regex pattern for 3-digit numbers from ones to ten-and ones. e.g. 117, 308 (NOT 120).
        /// </summary>
        /// <returns></returns>
        private string AllUpToOnesAndTenOnes()
        {
            string hundsGroup = RegexPatternCreator.ClosureQuestionMark(
                RegexPatternCreator.CreateGroup("", HundsPattern(),
                        RegexPatternCreator.BetWordWSPlus, "و", RegexPatternCreator.BetWordWSPlus
                     )
                 );

            string tensGroup =
            RegexPatternCreator.ClosureQuestionMark(
                RegexPatternCreator.CreateGroup("", TensPattern(),
                        RegexPatternCreator.BetWordWSPlus, "و", RegexPatternCreator.BetWordWSPlus
                )
            );

            string numberPattern =
            RegexPatternCreator.CreateGroup("", hundsGroup,
                RegexPatternCreator.CreateGroup("", RegexPatternCreator.CreateOR(true,
                    TenOnesPattern(),
                    RegexPatternCreator.CreateGroup("", tensGroup, OnesPattern())
                ))
            );

            return @"\b" + numberPattern + OrdinalityPattern() +  @"\b";
        }

        /// <summary>
        /// returns the regex pattern for numbers from 1 to 9.
        /// </summary>
        private string OnesPattern()
        {
            string writtenNum =
                RegexPatternCreator.CreateGroup(ONES_NAME,
                    RegexPatternCreator.CreateOR(false, "یک", "دو", "سه", "چهار", "پنج", "شش", "شیش", "هفت", "هشت", "نه", "اول", "سوم"));

            return writtenNum;
        }

        /// <summary>
        /// Returns the pattern for 10 mulitipliers from 20 to 90 ORed together.
        /// </summary>
        /// <returns></returns>
        private string TensPattern()
        {
            string writtenNum =
                RegexPatternCreator.CreateGroup(TENS_NAME,
                    RegexPatternCreator.CreateOR(false, "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" ));

            return writtenNum;
        }

        /// <summary>
        /// Returns the regex pattern for 10 to 19 ORed together
        /// </summary>
        /// <returns></returns>
        private string TenOnesPattern()
        {
            string writtenNum =
                RegexPatternCreator.CreateGroup(TENONES_NAME,
                    RegexPatternCreator.CreateOR(false, "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "پونزده", "شونزده", "هفده", "هجده", "نوزده", "شانزده", "هیفده", "هیجده", "هژده", "هیژده"));

            return writtenNum;
        }

        /// <summary>
        /// Returns regex patterns for the multipliers of 100. e.g. سیصد، دویست، نهصد، صد
        /// </summary>
        /// <returns></returns>
        private string HundsPattern()
        {
            string writtenNum =
                RegexPatternCreator.CreateGroup(HUNDSNUM_NAME,
                    RegexPatternCreator.CreateOR(false, "یک", "دو", "سی", "چهار", "پان", "پون", "پنج", "شش", "شیش", "هفت", "هشت", "نه"));
            string hunds =
                RegexPatternCreator.CreateGroup(HUNDS_NAME,
                    RegexPatternCreator.CreateOR(true,
                        RegexPatternCreator.CreateGroup("",
                            RegexPatternCreator.ClosureQuestionMark(
                                RegexPatternCreator.CreateGroup("", writtenNum, RegexPatternCreator.InWordWSStar)
                            ),
                            "صد"),
                        "دویست", "سیصد"));
            return hunds;
        }

        /// <summary>
        /// returns the pattern that makes numbers ordinal. i.e. adds "م" and "ام" in the end.
        /// </summary>
        /// <returns></returns>
        private string OrdinalityPattern()
        {
            string ordinalityPat = RegexPatternCreator.ClosureQuestionMark(
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("", RegexPatternCreator.CreateOR(true, "م", "ام"))
                )
            );

            return ordinalityPat;
        }

        /// <summary>
        /// Named As MULTIPLIER_NAME
        /// Containing only the grand coefficients ORed together, e.g. "میلیون", "میلیارد"
        /// </summary>
        private string MultipliersPattern()
        {
            string writtenNum =
                RegexPatternCreator.CreateGroup(MULTIPLIER_NAME,
                    RegexPatternCreator.CreateOR(false, "هزار", "ملیون", "میلیون", "میلیارد", "ملیارد", "بلیون", "بیلیون", "ترلیارد", "تریلیارد", "تریلیون", "ترلیون"));

            return writtenNum;
        }

        #endregion

        #region Persian Regex Group Names
        private const string ONES_NAME = "Ones";
        private const string TENS_NAME = "Tens";
        private const string TENONES_NAME = "TenOnes";
        private const string HUNDSNUM_NAME = "HundsNum";
        private const string HUNDS_NAME = "Hunds";
        private const string THREEDB_NAME = "ThreeDB";
        private const string MULTIPLIER_NAME = "Multiplier";
        private const string MULTIPLIER_GROUP_NAME = "MultiplierGroup";
        #endregion

        #region Public Interfaces

        /// <summary>
        /// Searches the specified string for patterns of integer numbers in a persian descriptive string, and
        /// returns a sequnce of <see cref="PersianNumberPatternInfo"/> that holds information about the pattern found.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <returns></returns>
        public PersianNumberPatternInfo[] FindAndParse(string str)
        {
            List<PersianNumberPatternInfo> l = new List<PersianNumberPatternInfo>();

            Regex regex = new Regex(PersianNumberPattern());
            foreach (Match m in regex.Matches(str))
                l.Add(ExtractFromPersianNumber(m));

            return l.ToArray();
        }

        #endregion 

        #region Extract (i.e. Parsing) Functions

        /// <summary>
        /// Parses and extracts information from the found regex match object 
        /// for the persian integer number in descriptive written form.
        /// The parsed pattern info is returned, and null is returned if 
        /// the Match does not contain valid data.
        /// </summary>
        /// <param name="m">The regex match object to be parsed.</param>
        /// <returns></returns>
        private PersianNumberPatternInfo ExtractFromPersianNumber(Match m)
        {
            long number = 0L;
            string matchValue = m.Value;
            int curPos = 0;

            foreach (Capture c in m.Groups[MULTIPLIER_GROUP_NAME].Captures)
            {
                number += ExtractMultiplierAndNumber(c.Value);
                curPos = c.Index + c.Length;
            }

            foreach(Capture c in m.Groups[THREEDB_NAME].Captures)
            {
                if(c.Index >= curPos)
                    number += ExtractThreeDB(c.Value);
            }

            return new PersianNumberPatternInfo(m.Value, m.Index, m.Length, number);
        }

        /// <summary>
        /// Extracts and parses the multiplier and number from the specified string.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <returns></returns>
        private long ExtractMultiplierAndNumber(string str)
        {
            long num = 0L;
            long tempNum = 0L;
            if (!PersianLiteral2NumMap.TryPersianString2Num(str.Trim(), out num))
            {
                num = 0L;
                Regex regex = new Regex(BlockPlusMultiplier());
                foreach (Match m in regex.Matches(str))
                {
                    // should iterate only once
                    foreach (Capture c in m.Groups[THREEDB_NAME].Captures)
                    {
                        num += ExtractThreeDB(c.Value);
                    }

                    num = (num == 0)? 1 : num;

                    foreach (Capture c in m.Groups[MULTIPLIER_NAME].Captures)
                    {
                        if (PersianLiteral2NumMap.TryPersianString2Num(c.Value, out tempNum))
                            num *= tempNum;
                    }
                }
            }

            return num;
        }

        /// <summary>
        /// Extracts and parses the three digit block from the specified string.
        /// </summary>
        /// <param name="str">The string to extract number from.</param>
        /// <returns></returns>
        private long ExtractThreeDB(string str)
        {

            long num = 0L;
            long tempNum = 0L;
            if (Char.IsDigit(str[0]))
            {
                string strNum = ParsingUtils.ConvertNumber2English(str.Trim());
                int n;
                if (Int32.TryParse(strNum, out n))
                    num = (long)n;
            }
            else
            {
                Regex regex = new Regex(ThreeDigitBlockPattern());
                foreach (Match m in regex.Matches(str))
                {
                    foreach (Capture c in m.Groups[ONES_NAME].Captures)
                    {
                        if (PersianLiteral2NumMap.TryPersianString2Num(c.Value, out tempNum))
                            num += tempNum;
                    }

                    foreach (Capture c in m.Groups[TENS_NAME].Captures)
                    {
                        if (PersianLiteral2NumMap.TryPersianString2Num(c.Value, out tempNum))
                            num += tempNum;
                    }

                    foreach (Capture c in m.Groups[TENONES_NAME].Captures)
                    {
                        if (PersianLiteral2NumMap.TryPersianString2Num(c.Value, out tempNum))
                            num += tempNum;
                    }

                    num += ExtractHundreds(m);
                }
            }
            return num;
        }

        /// <summary>
        /// Extracts and parses the hundreds from the specified regex Match object provided.
        /// </summary>
        /// <param name="m">The regex match object to extract from.</param>
        /// <returns></returns>
        private long ExtractHundreds(Match m)
        {
            long num = 0L;
            foreach (Capture c in m.Groups[HUNDS_NAME].Captures)
            {
                if (!PersianLiteral2NumMap.TryPersianString2Num(c.Value, out num))
                {
                    num = 0L;
                    foreach (Capture cc in m.Groups[HUNDSNUM_NAME].Captures)
                    {
                        long coef = 0L;
                        if (PersianLiteral2NumMap.TryPersianString2Num(cc.Value, out coef))
                        {
                            if (10 < coef && coef < 100) coef = coef / 10;
                            else if (coef >= 100) coef = 0;
                        }

                        num += coef * 100;
                    }
                }
            }
            return num;
        }
        
        #endregion
    }

    /// <summary>
    /// Class to hold information about the parsed persian number patterns
    /// </summary>
    public class PersianNumberPatternInfo : IPatternInfo
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

        private long number;

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <value>The number.</value>
        public long Number
        {
            get { return number; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianNumberPatternInfo"/> class.
        /// </summary>
        /// <param name="content">The content of the pattern.</param>
        /// <param name="index">The index.</param>
        /// <param name="len">The length of the pattern.</param>
        /// <param name="number">The number parsed from the pattern.</param>
        public PersianNumberPatternInfo (string content, int index, int len, long number)
        {
            this.content = content;
            this.index = index;
            this.length = len;
            this.number = number;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{1}{0}{3}{0}", Environment.NewLine,
                content, index, number);
            //return String.Format("Content:{1}{0}At:{2}{0}\tValue:{3}{0}", Environment.NewLine,
            //    content, index , number);
        }

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.PersianNumber; }
        }
    }
}

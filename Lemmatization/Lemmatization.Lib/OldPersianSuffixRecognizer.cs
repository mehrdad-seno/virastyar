// Author: Sina Iravanian
// Created on: 2010-March-08
// Last Modified: Omid Kashefi at 2010-March-08
//


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SCICT.NLP.Persian;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility.Parsers;

namespace SCICT.NLP.Utility.Stemming.Suffix
{
    /// <summary>
    /// Helps recognize suffixes from Persian words.
    /// This class is obsolete for efficiency problems.
    /// Use <see cref="PersianSuffixLemmatizer"/> instead.
    /// </summary>
    [Obsolete("Use PersianSuffixLemmatizer instead", true)]
    public class OldPersianSuffixRecognizer
    {
        #region Construction and Initialization
        private delegate string GetSuffixDel();
        private static GetSuffixDel suffixDelegates;

        /// <summary>
        /// Initializes the <see cref="OldPersianSuffixRecognizer"/> class,
        /// by evaluating delegates to regex methods.
        /// </summary>
        static OldPersianSuffixRecognizer()
        {
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaCompAdj);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaCompAdjToBe);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaCompAdjAaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaCompAdjHaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaCompAdjObjPron);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaObjPron);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaHaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaAa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaaToBe);

            suffixDelegates += new GetSuffixDel(GetAffixRegExCompAdj);
            suffixDelegates += new GetSuffixDel(GetAffixRegExCompAdjToBe);
            suffixDelegates += new GetSuffixDel(GetAffixRegExCompAdjAaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExCompAdjHaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExCompAdjObjPron);
            suffixDelegates += new GetSuffixDel(GetAffixRegExObjPron);
            suffixDelegates += new GetSuffixDel(GetAffixRegExHaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExAaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExToBe);

            suffixDelegates += new GetSuffixDel(GetAffixRegExGaanObjPron);
            suffixDelegates += new GetSuffixDel(GetAffixRegExGaanYaaToBe);
            suffixDelegates += new GetSuffixDel(GetAffixRegExGaanToBe);
            suffixDelegates += new GetSuffixDel(GetAffixRegExGaanYaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExGaan);


            suffixDelegates += new GetSuffixDel(GetAffixRegExAanObjPron);
            suffixDelegates += new GetSuffixDel(GetAffixRegExAanYaaToBe);
            suffixDelegates += new GetSuffixDel(GetAffixRegExAanToBe);
            suffixDelegates += new GetSuffixDel(GetAffixRegExAanYaa);
            suffixDelegates += new GetSuffixDel(GetAffixRegExAan);
            
            suffixDelegates += new GetSuffixDel(GetAffixRegExYaa);
        }

        private static string WORD_PART_NAME = "word";
        private static string[] ObjectivePronouns = new string[] { "مان", "تان", "شان", "ش", "ت", "م" };
        private static string[] ObjectivePronounsColloqual = new string[] { "شون", "مون", "تون", "م", "ت", "ش" };
        
        private static string[] ToBeVerbs = new string[] { "ایم", "اید", "اند", "ای", "ام", "است" };
        private static string[] ToBeVerbsWithoutHamzaPart1 = new string[] { "یم", "ید", "ند", "ی" };
        private static string[] ToBeVerbsWithoutHamzaPart2 = new string[] { "م", "ست" };
        // for their conflict in their usage in regular expressions they sould be divided in several parts they 
        // should be used in asscending order of their number
        private static string[] ToBeVerbsColloqualPart1 = new string[] { "ایم" };
        private static string[] ToBeVerbsColloqualPart2 = new string[] { "یم", "ام", "ای", "اه", "این" };
        private static string[] ToBeVerbsColloqualPart3 = new string[] { "م", "ی", "ه", "ین", "ان" };
        private static string[] ToBeVerbsColloqualPart4 = new string[] { "ن" };

        private static string[] ComparativeAdjectives = new string[] { "تر", "ترین" };

        //        private static string AFFIX_PART_NAME = "affix";

        #endregion

        #region Pattern Creation Utilities
        private static string CreateHaaXPattern(string wordPart) 
        {
            return
            RegexPatternCreator.CreateGroup("",
                wordPart,
                RegexPatternCreator.InWordWSStar,
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.CreateOR(true,
                        "ها", "های", "هایی", "هائی",
                        StringMultiplication("های", ObjectivePronouns),
                        StringMultiplication("های‌", ObjectivePronouns),
                        StringMultiplication("ها", ObjectivePronounsColloqual)
                   )
                )
             );
        }

        private static string CreateAaaXPattern(string wordPart) 
        {
            return
            RegexPatternCreator.CreateGroup("",
                wordPart,
                RegexPatternCreator.InWordWSStar,
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.CreateOR(true,
                        "ا", "ای", "ایی", "ائی",
                        StringMultiplication("ای", ObjectivePronouns),
                        StringMultiplication("ای‌", ObjectivePronouns),
                        StringMultiplication("ا", ObjectivePronounsColloqual)
                   )
                )
             );
        }

        private static string CreateObjPronounPattern(string wordPart) 
        {
            return
            RegexPatternCreator.CreateGroup("",
                wordPart,
                RegexPatternCreator.InWordWSStar,
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.CreateOR(true,
                        ObjectivePronouns,
                        ObjectivePronounsColloqual
                   )
                )
             );
        }

        private static string CreateToBePattern(string wordPart)
        {
            string strToBeOnly = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbs
                       ))
                );

            string strToBeNoHamza1 = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbsWithoutHamzaPart1
                       ))
                );

            string strToBeNoHamza2 = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbsWithoutHamzaPart2
                       ))
                );

            string strToBeCol1 = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbsColloqualPart1
                       ))
                );

            string strToBeCol2 = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbsColloqualPart2
                       ))
                );

            string strToBeCol3 = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbsColloqualPart3
                       ))
                );

            string strToBeCol4 = RegexPatternCreator.CreateGroup("",
                    wordPart,
                    RegexPatternCreator.InWordWSStar,
                    RegexPatternCreator.CreateGroup("",
                        RegexPatternCreator.CreateOR(true,
                            ToBeVerbsColloqualPart4
                       ))
                );


            return
            RegexPatternCreator.CreateGroup("", RegexPatternCreator.CreateOR(true,
                strToBeOnly, strToBeNoHamza1, strToBeNoHamza2, strToBeCol1, strToBeCol2, strToBeCol3, strToBeCol4
            ));
        } 
        #endregion

        #region Pattern Creation Methods

        private static string GetWordPattern() 
        {
            string wordPattern = String.Format(@"(?<{0}>(\w|{1})+)", WORD_PART_NAME, RegexPatternCreator.InWordWSChar);
            //return wordPattern;
            return RegexPatternCreator.CreateGroup("", wordPattern, RegexPatternCreator.InWordWSStar);
        }

        private static string GetAffixRegExYaa() 
        {
            string wordPattern = GetWordPattern();
            return RegexPatternCreator.CreateGroup("", wordPattern, "ی");
        }

        private static string GetAffixRegExGaan() 
        {
            string wordPattern = GetWordPattern();
            return RegexPatternCreator.CreateGroup("", wordPattern, RegexPatternCreator.InWordWSStar, "گان");
        }

        private static string GetAffixRegExGaanYaa() 
        {
            string wordPattern = GetWordPattern();
            return RegexPatternCreator.CreateGroup("", wordPattern, RegexPatternCreator.InWordWSStar, "گانی");
        }

        private static string GetAffixRegExAan() 
        {
            string wordPattern = GetWordPattern();
            return RegexPatternCreator.CreateGroup("", wordPattern, "ان");
        }

        private static string GetAffixRegExAanYaa() 
        {
            string wordPattern = GetWordPattern();
            return RegexPatternCreator.CreateGroup("", wordPattern, "انی");
        }

        private static string GetAffixRegExYaaHaa() 
        {
            return CreateHaaXPattern(GetAffixRegExYaa());
        }

        private static string GetAffixRegExYaaObjPron()  
        {
            return CreateObjPronounPattern(GetAffixRegExYaa());
        }

        private static string GetAffixRegExObjPron() 
        {
            return CreateObjPronounPattern(GetWordPattern());
        }

        private static string GetAffixRegExAanObjPron() 
        {
            return CreateObjPronounPattern(GetAffixRegExAan());
        }

        private static string GetAffixRegExGaanObjPron() 
        {
            return CreateObjPronounPattern(GetAffixRegExGaan());
        }

        private static string GetAffixRegExAanYaaToBe() 
        {
            return CreateToBePattern(GetAffixRegExAanYaa());
        }

        private static string GetAffixRegExGaanYaaToBe() 
        {
            return CreateToBePattern(GetAffixRegExGaanYaa());
        }

        private static string GetAffixRegExAanToBe() 
        {
            return CreateToBePattern(GetAffixRegExAan());
        }

        private static string GetAffixRegExGaanToBe() 
        {
            return CreateToBePattern(GetAffixRegExGaan());
        }

        private static string GetAffixRegExYaaAa() 
        {
            return CreateAaaXPattern(GetAffixRegExYaa());
        }

        private static string GetAffixRegExYaaCompAdjHaa() 
        {
            return CreateHaaXPattern(GetAffixRegExYaaCompAdj());
        }

        private static string GetAffixRegExYaaCompAdjObjPron() 
        {
            return CreateObjPronounPattern(GetAffixRegExYaaCompAdj());
        }

        private static string GetAffixRegExCompAdjObjPron() 
        {
            return CreateObjPronounPattern(GetAffixRegExCompAdj());
        }

        private static string GetAffixRegExYaaCompAdjAaa() 
        {
            return CreateAaaXPattern(GetAffixRegExYaaCompAdj());
        }

        private static string GetAffixRegExCompAdjHaa() 
        {
            return CreateHaaXPattern(GetAffixRegExCompAdj());
        }

        private static string GetAffixRegExCompAdjAaa() 
        {
            return CreateAaaXPattern(GetAffixRegExCompAdj());
        }

        private static string GetAffixRegExYaaCompAdj() 
        {
            string wordWithYaa = GetAffixRegExYaa();

            return
            RegexPatternCreator.CreateGroup("",
                wordWithYaa,
                RegexPatternCreator.InWordWSStar,
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.CreateOR(true,
                    ComparativeAdjectives
                   )
                )
             );
        }

        private static string GetAffixRegExCompAdj() 
        {
            string word = GetWordPattern();

            return
            RegexPatternCreator.CreateGroup("",
                word,
                RegexPatternCreator.InWordWSStar,
                RegexPatternCreator.CreateGroup("",
                    RegexPatternCreator.CreateOR(true,
                    ComparativeAdjectives
                   )
                )
             );
        }

        private static string GetAffixRegExCompAdjToBe()
        {
            return CreateToBePattern(GetAffixRegExCompAdj());
        }

        private static string GetAffixRegExYaaCompAdjToBe()
        {
            return CreateToBePattern(GetAffixRegExYaaCompAdj());
        }

        private static string GetAffixRegExAaa() 
        {
            return CreateAaaXPattern(GetWordPattern());
        }

        private static string GetAffixRegExHaa() 
        {
            return CreateHaaXPattern(GetWordPattern());
        }

        private static string GetAffixRegExToBe() 
        {
            return CreateToBePattern(GetWordPattern());
        }

        private static string GetAffixRegExYaaToBe() 
        {
            return CreateToBePattern(GetAffixRegExYaa());
        }

        #endregion

        #region Some Utils
        private static string[] StringMultiplication(string first, string[] sec)
        {
            return StringMultiplication(new string[] { first }, sec);
        }

        private static string[] StringMultiplication(string[] first, string [] sec)
        {
            List<string> result = new List<string>();

            foreach(string str1 in first)
            {
                foreach(string str2 in sec)
                {
                    result.Add(str1 + str2);
                }
            }

            return result.ToArray();
        }
        #endregion

        #region Public Interface

        /// <summary>
        /// Matches the input string for affixes and returns an array of <see cref="PersianAffixPatternInfo"/> instances
        /// which hold information about the found patterns.
        /// </summary>
        /// <param name="exp">The input string to find affixes at.</param>
        /// <param name="startat">The index at which the matching process starts.</param>
        /// <returns></returns>
        public static PersianSuffixPatternInfo[] GetWordAndAffix(string exp, int startat)
        {
            // initializing some params

            List<PersianSuffixPatternInfo> lstPInfos = new List<PersianSuffixPatternInfo>();

            string word = "";
            string affix = "";
            int length = 0;

            List<Match> lstMatches = new List<Match>();

            Delegate[] dels = suffixDelegates.GetInvocationList();
            if (dels != null)
            {
                string input;
                if (exp.Length < 200)
                {
                    input = RefineInputExpression(exp, startat);
                }
                else
                {
                    input = exp;
                }

                bool isMatchOK = false;
                foreach (Delegate del in dels)
                {
                    string regex = @"(\A)(" + (string)del.DynamicInvoke() + @")(\b)";
                    Regex r = new Regex(regex);

                    Match m = r.Match(input);
                    isMatchOK = false;
                    if (m.Index == 0 && m.Length > 0)
                    {
                        isMatchOK = true;
                        // TODO: you can add more rules here 
                        if (m.Index + m.Length < input.Length) // there are still chars after the pattern
                            if (input[m.Index + m.Length] == PseudoSpace.ZWNJ)
                                isMatchOK = false;
                    }

                    if (isMatchOK)
                        lstMatches.Add(m);
                }

                if (lstMatches.Count > 0)
                {
                    foreach (Match m in lstMatches)
                    {
                        foreach (Capture c in m.Groups[WORD_PART_NAME].Captures)
                        {
                            word = c.Value;
                        }

                        affix = input.Substring(m.Index + word.Length, m.Length - word.Length);
                        length = m.Length;

                        if (word.Length <= 0)
                        {
                            word = "";
                            affix = "";
                            length = 0;
                        }

                        if (affix.Length > 0)
                        {
                            string str = input.Substring(0, length);
                            lstPInfos.Add(new PersianSuffixPatternInfo(str, word, affix, length));
                        }
                    }
                }
            }

            return PostProcessFoundItems(lstPInfos).ToArray();
        }

        private static List<PersianSuffixPatternInfo> PostProcessFoundItems(List<PersianSuffixPatternInfo> lstPInfos)
        {
            List<PersianSuffixPatternInfo> lstRet = new List<PersianSuffixPatternInfo>();

            int i, comp;
            PersianSuffixPatternInfo piComp;
            bool exists = false;
            foreach (PersianSuffixPatternInfo piIn in lstPInfos)
            {
                exists = false;
                for (i = 0; i < lstRet.Count; ++i)
                {
                    piComp = lstRet[i];
                    comp = PersianSuffixPatternInfo.Compare(piIn, piComp);
                    if (comp == 0)
                    {
                        exists = true;
                        break;
                    }
                    else if (comp < 0)
                        break;

                }

                if (!exists)
                    lstRet.Insert(i, piIn);            
            }

            return lstRet;
        }

        private static int MaxInputWordCount = 5;

        /// <summary>
        /// Reads and returns the first 5 words of the exp in order to make it as small as possible
        /// </summary>
        private static string RefineInputExpression(string exp, int startat)
        {
            string regex = String.Format(@"(\b)((\w)|({0}))+(\b)", PseudoSpace.ZWNJ);
            Regex r = new Regex(regex);
            Match m = r.Match(exp, startat);
            int i;
            for (i = 1; i < MaxInputWordCount; ++i)
            {
                if (m != null)
                    m = m.NextMatch();
                else
                    break;
            }

            if (m == null || m.Index < startat || m.Length <= 0 || i < MaxInputWordCount) // i.e. loop was breaked
            {
                return exp.Substring(startat);
            }
            else
            {
                return exp.Substring(startat, m.Index + m.Length - startat);
            }
        }

        #region OBSOLETE
        /// <summary>
        /// OBSOLETE
        /// if lenght is > 0 then it means it had found something, otherwise it did not find anything
        /// so ignore the value of all out parameters.
        /// </summary>
        public static void GetWordAndAffix(string exp, int startat, out string word, out string affix, out int length)
        {
            // initializing out params
            word = "";
            affix = "";
            length = 0;

            List<Match> lstMatches = new List<Match>();

            Delegate[] dels = suffixDelegates.GetInvocationList();
            if (dels == null) return;

            foreach (Delegate del in dels)
            {
                string regex = @"\b" + (string)del.DynamicInvoke() + @"\b";
                Regex r = new Regex(regex);

                Match m = r.Match(exp, startat);
                if(m.Index == startat)
                    lstMatches.Add( m );
            }

            if (lstMatches.Count <= 0) 
                return; // return with initial values for out

            foreach (Match m in lstMatches)
            {
                foreach (Capture c in m.Groups[WORD_PART_NAME].Captures)
                {
                    word = c.Value;
                }

                affix = exp.Substring(m.Index + word.Length, m.Length - word.Length);
                length = m.Length;

                if (word.Length <= 0)
                {
                    word = "";
                    affix = "";
                    length = 0;
                }

                if (affix.Length > 0)
                    break; // i.e. found and ignore forthcomming delegates
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Holds information about words and their affix in persian as found in some context
    /// </summary>
    public class PersianSuffixPatternInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersianSuffixPatternInfo"/> class.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <param name="word">The word.</param>
        /// <param name="Suffix">The Suffix.</param>
        /// <param name="length">The length of the word.</param>
        public PersianSuffixPatternInfo(string exp, string word, string suffix, int length)
        {
            int i;
            for (i = word.Length - 1; i >= 0; --i)
            {
                if (word[i] == PseudoSpace.ZWNJ)
                {
                    suffix = word[i] + suffix;
                }
                else
                {
                    break;
                }
            }

            word = word.Substring(0, i + 1);

            this.expression = exp;
            this.word = word;
            this.suffix = suffix;
            this.length = length;

            ApplyWordConstructionRules();
        }

        private void ApplyWordConstructionRules()
        {
            if (suffix.StartsWith("ان") && word.EndsWith("ای")) // e.g. خدایان
            {
                word = word.Substring(0, word.Length - 1); // remove last letter
                suffix = "ی" + suffix;
            }
            else if (suffix.StartsWith("گان") && !word.EndsWith("ه")) // e.g. پرندگان
            {
                word = word + "ه";
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersiansuffixPatternInfo"/> class.
        /// </summary>
        public PersianSuffixPatternInfo()
            : this("", "", "", 0)
        {
        }

        private string expression;
        
        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression
        {
            get
            {
                return expression;
            }
        }

        private string word;

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <value>The word.</value>
        public string Word
        {
            get
            {
                return word;
            }
        }

        private string suffix;

        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public string Suffix 
        {
            get
            {
                return suffix;
            }
        }

        private int length;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length 
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}:{1} + {2}", expression, word, suffix.Length > 0 ? suffix : "-");
        }

        /// <summary>
        /// Determines whether the specified pattern informations are equal.
        /// </summary>
        /// <param name="pi1">The 1st pattern info.</param>
        /// <param name="pi2">The 2nd pattern info.</param>
        /// <returns>
        /// 	<c>true</c> if the specified pi1 is equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqual(PersianSuffixPatternInfo pi1, PersianSuffixPatternInfo pi2)
        {
            if (pi1.Length == pi2.Length && pi1.Word == pi2.Word && pi1.Suffix == pi2.Suffix && pi1.Expression == pi2.Expression)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Compares the specified pattern information objects.
        /// </summary>
        /// <param name="pi1">The 1st pattern info.</param>
        /// <param name="pi2">The 2nd pattern info.</param>
        /// <returns></returns>
        public static int Compare(PersianSuffixPatternInfo pi1, PersianSuffixPatternInfo pi2)
        {
            if (IsEqual(pi1, pi2))
                return 0;

            int len1 = pi1.Length;
            int len2 = pi2.Length;

            if (len1 != len2)
                return len2 - len1; // the reason is more the length is sooner the expression should appear
            else
            {
                if (pi1.Word.Length != pi2.Word.Length)
                    return pi1.Word.Length - pi2.Word.Length; 
                else
                {
                    if (pi1.Suffix.Length != pi2.Suffix.Length)
                        return pi2.Suffix.Length - pi1.Suffix.Length;
                    else // in this special scenario this should not happen
                        return String.Compare(pi1.expression, pi2.expression);
                }
            }
        }
    }
}

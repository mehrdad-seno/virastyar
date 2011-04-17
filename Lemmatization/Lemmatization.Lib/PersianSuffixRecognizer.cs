// Author: Omid Kashefi, Sina Iravanian 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Sina Iravanian at 2010-March-08
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SCICT.NLP.Utility.Parsers;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Morphology.Inflection;
using SCICT.Utility;

namespace SCICT.NLP.Morphology.Lemmatization
{
    /// <summary>
    /// Helps recognize suffixes in Persian words.
    /// </summary>
    public class PersianSuffixRecognizer
    {
        #region Suffix Constants

        private static readonly string[] m_indefiniteYaaPermutation = IndefiniteYaaPermutation().ToArray();

        private static readonly string[] m_yaaBadalAzKasraPermutation = YaaBadalAzKasraPermutation().ToArray();

        private static readonly string[] m_toBeVerbPermutation = ToBeVerbPermutation().ToArray();

        private static readonly string[] m_ojectivePronounPermutation = ObjectivePronounPermutation().ToArray();

        private static readonly string[] m_pluralHaaPermutation = PluralHaaPermutation().ToArray();

        private static readonly string[] m_pluralAnnPermutation = PluralAnnPermutation().ToArray();

        private static readonly string[] m_comparativeAdjectivePermutation = ComparativeAdjectivePermutation().ToArray();

        private static readonly string[] m_yaaNesbatPermutation = YaaNesbatPermutation().ToArray();

        private static readonly string[] m_enumerableAdjectivePermutation = EnumerableAdjectivePermutation().ToArray();
        
        #endregion

        #region Construction Stuff

        /// <summary>
        /// An instance of reverse pattern matcher to help find Suffixes patterns in the end of 
        /// each word.
        /// </summary>
        private readonly ReversePatternMatcher m_revPatternMatcher = new ReversePatternMatcher();

        /// <summary>
        /// List of Suffixes pattern
        /// </summary>
        private List<string> m_lstPatterns = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianSuffixLemmatizer"/> class.
        /// </summary>
        /// <param name="useColloquals">if set to <c>true</c> it will recognize colloqual affixes as well.</param>
        public PersianSuffixRecognizer(bool useColloquals) : this(useColloquals, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersianSuffixLemmatizer"/> class.
        /// </summary>
        /// <param name="useColloquals">if set to <c>true</c> it will recognize colloqual affixes as well.</param>
        /// <param name="uniqueResults">if set to <c>true</c> unique results will be returned.</param>
        public PersianSuffixRecognizer(bool useColloquals, bool uniqueResults)
        {
            UseColloquals = useColloquals;
            ReturnUniqueResults = uniqueResults;

            PersianSuffixesCategory suffixCategory = 0;
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.ComparativeAdjectives);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.IndefiniteYaa);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.ObjectivePronoun);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.OrdinalEnumerableAdjective);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.PluralSignAan);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.PluralSignHaa);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.ToBeVerb);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.YaaBadalAzKasre);
            suffixCategory = suffixCategory.Set(PersianSuffixesCategory.YaaNesbat);

            InitPatternsList(suffixCategory);

            m_revPatternMatcher.SetEndingPatterns(m_lstPatterns);
        }

        /// <summary>
        /// Creates the list of Persian Suffixes patterns.
        /// </summary>
        private void InitPatternsList(PersianSuffixesCategory suffixCategory)
        {
            if (suffixCategory.Has(PersianSuffixesCategory.IndefiniteYaa))
            {
                m_lstPatterns.AddRange(m_indefiniteYaaPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.YaaBadalAzKasre))
            {
                m_lstPatterns.AddRange(m_yaaBadalAzKasraPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.ToBeVerb))
            {
                m_lstPatterns.AddRange(m_toBeVerbPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.ObjectivePronoun))
            {
                m_lstPatterns.AddRange(m_ojectivePronounPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.PluralSignHaa))
            {
                m_lstPatterns.AddRange(m_pluralHaaPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.PluralSignAan))
            {
                m_lstPatterns.AddRange(m_pluralAnnPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.YaaNesbat))
            {
                m_lstPatterns.AddRange(m_yaaNesbatPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.ComparativeAdjectives))
            {
                m_lstPatterns.AddRange(m_comparativeAdjectivePermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.OrdinalEnumerableAdjective))
            {
                m_lstPatterns.AddRange(m_enumerableAdjectivePermutation);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the colloqual Suffixes should be recognized.
        /// </summary>
        /// <value><c>true</c> if the colloqual Suffixes should be recognized; otherwise, <c>false</c>.</value>
        public bool UseColloquals { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the returned results should be unique.
        /// </summary>
        /// <value><c>true</c> if the returned results should be unique; otherwise, <c>false</c>.</value>
        public bool ReturnUniqueResults { get; private set; }

        #endregion

        #region Create Patterns

        private static IEnumerable<string> IndefiniteYaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(), 
                        PersianSuffixes.IndefiniteYaa);
        }

        private static IEnumerable<string> YaaBadalAzKasraPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpace.ToString(), 
                        PersianSuffixes.YaaBadalAzKasre);
        }

        private static IEnumerable<string> ToBeVerbPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(), 
                        PersianSuffixes.ToBeVerbs);
        }

        private static IEnumerable<string> ObjectivePronounPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        MultiplyStrings(PersianSuffixes.ObjectivePronouns, PersianSuffixes.ToBeVerbsBase).Concat(
                            PersianSuffixes.ObjectivePronouns));
        }

        private static IEnumerable<string> PluralHaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignHaa, m_ojectivePronounPermutation).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianSuffixes.ToBeVerbs).Concat(
                               MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianSuffixes.IndefiniteYaa).Concat(
                                   MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianSuffixes.YaaBadalAzKasre).Concat(
                                            PersianSuffixes.PluralSignHaa)))));
        }

        private static IEnumerable<string> PluralAnnPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignAan, m_ojectivePronounPermutation).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignAan, PersianSuffixes.ToBeVerbsBase).Concat(
                               MultiplyStrings(PersianSuffixes.PluralSignAan, PersianSuffixes.IndefiniteYaa).Concat(
                                   PersianSuffixes.PluralSignAan))));
        }

        private static IEnumerable<string> ComparativeAdjectivePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.ComparativeAdjectives, m_ojectivePronounPermutation).Concat(
                           MultiplyStrings(PersianSuffixes.ComparativeAdjectives, ToBeVerbPermutation()).Concat(
                               MultiplyStrings(PersianSuffixes.ComparativeAdjectives, IndefiniteYaaPermutation()).Concat(
                                   MultiplyStrings(PersianSuffixes.ComparativeAdjectives, m_pluralHaaPermutation).Concat(
                                       PersianSuffixes.ComparativeAdjectives)))));

        }

        private static IEnumerable<string> EnumerableAdjectivePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.EnumerableAdjective, m_ojectivePronounPermutation).Concat(
                           MultiplyStrings(PersianSuffixes.EnumerableAdjective, ToBeVerbPermutation()).Concat(
                               MultiplyStrings(PersianSuffixes.EnumerableAdjective, IndefiniteYaaPermutation()).Concat(
                                   MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, PluralHaaPermutation()).Concat(
                                       PersianSuffixes.EnumerableAdjective)))));

        }

        private static IEnumerable<string> YaaNesbatPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.YaaNesbat, m_ojectivePronounPermutation).Concat(
                           MultiplyStrings(PersianSuffixes.YaaNesbat, ToBeVerbPermutation()).Concat(
                               MultiplyStrings(PersianSuffixes.YaaNesbat, IndefiniteYaaPermutation()).Concat(
                                   MultiplyStrings(PersianSuffixes.YaaNesbat, m_pluralHaaPermutation).Concat(
                                       MultiplyStrings(PersianSuffixes.YaaNesbat, m_pluralAnnPermutation).Concat(
                                           MultiplyStrings(PersianSuffixes.YaaNesbat, m_comparativeAdjectivePermutation).Concat(
                                               PersianSuffixes.YaaNesbat)))))));

        }
        
        private IEnumerable<string> CreateHaaXPatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar,
                        MultiplyStrings("ها", PersianColloqualSuffixes.ObjectivePronouns)
                );
        }

        private IEnumerable<string> CreateAanXPatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart,
                        MultiplyStrings("ان", PersianColloqualSuffixes.ObjectivePronouns)
                );
        }

        /// <summary>
        /// A Concatenation of CreateHaaxPatternColloqual and CreateAanXPatternColloqual
        /// </summary>
        private IEnumerable<string> CreatePluralXPatternColloqual(string wordPart)
        {
            return CreateHaaXPatternColloqual(wordPart).Concat(CreateAanXPatternColloqual(wordPart));
        }

        private IEnumerable<string> CreateAaaXPatternColloqual(string wordPart)
        {
            return MultiplyStrings(
                wordPart /* + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar */,
                        (new string[] { "ا", "ای", "ایی", "ائی" }).Concat(
                        MultiplyStrings("ای", PersianSuffixes.ObjectivePronouns)).Concat(
                        MultiplyStrings("ای" + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa)).Concat(
                        MultiplyStrings("ا", PersianColloqualSuffixes.ObjectivePronouns))
                   );
        }

        private IEnumerable<string> CreateObjPronounPatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar,
                        PersianColloqualSuffixes.ObjectivePronouns);
        }

        private IEnumerable<string> CreateToBePatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar,
                        PersianColloqualSuffixes.ToBeVerbs).Concat(
                    MultiplyStrings(wordPart,
                        PersianColloqualSuffixes.ToBeVerbs)
                        );
        }
        

        #endregion

        #region Public Interface

        /// <summary>
        /// Matches the input string for finding Persian Suffixes.
        /// </summary>
        /// <param name="input">The input string to find Suffixes.</param>
        /// <returns></returns>
        public ReversePatternMatcherPatternInfo[] MatchForSuffix(string input)
        {
            return m_revPatternMatcher.Match(input, this.ReturnUniqueResults);
        }

        ///<summary>
        /// Return the suffix category
        ///</summary>
        ///<param name="suffix">Suffix</param>
        ///<returns>Suffix category</returns>
        ///<exception cref="NotImplementedException"></exception>
        public PersianSuffixesCategory SuffixCategory(string suffix)
        {
            PersianSuffixesCategory suffixCategory = 0;
            suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

            if (IsInSuffixPattern(suffix, m_comparativeAdjectivePermutation))
            {
                suffixCategory |= PersianSuffixesCategory.ComparativeAdjectives;
            }
            if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.IndefiniteYaa;
            }
            if (IsInSuffixPattern(suffix, m_ojectivePronounPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.ObjectivePronoun;
            }
            if (IsInSuffixPattern(suffix, m_enumerableAdjectivePermutation))
            {
                suffixCategory |= PersianSuffixesCategory.OrdinalEnumerableAdjective;
            }
            if (IsInSuffixPattern(suffix, m_pluralAnnPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.PluralSignAan;
            }
            if (IsInSuffixPattern(suffix, m_pluralHaaPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.PluralSignHaa;
            }
            if (IsInSuffixPattern(suffix, m_toBeVerbPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.ToBeVerb;
            }
            if (IsInSuffixPattern(suffix, m_yaaBadalAzKasraPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.YaaBadalAzKasre;
            }
            if (IsInSuffixPattern(suffix, m_yaaNesbatPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.YaaNesbat;
            }

            return suffixCategory;
        }

        ///<summary>
        /// Return equal suffix with spacing symbols
        ///</summary>
        ///<param name="suffix">Suffix</param>
        ///<returns>Suffix with spacing symbols</returns>
        public string EqualSuffixWithSpacingSymbols(string suffix)
        {
            string equalSuffix = suffix;

            suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

            if (IsInSuffixPattern(suffix, m_comparativeAdjectivePermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_ojectivePronounPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_enumerableAdjectivePermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_pluralAnnPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_pluralHaaPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_toBeVerbPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_yaaBadalAzKasraPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutation, out equalSuffix))
            {
            }

            return equalSuffix;
        }

        #endregion

        #region Some Utils

        /// <summary>
        /// Returns a sequence of strings gained from concatenating string <paramref name="first"/>
        /// with all the strings in <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The string to form the left-hand-side of the concatenations.</param>
        /// <param name="second">The sequence of strings to form the right-hand-side of the concatenations.</param>
        /// <returns></returns>
        private static IEnumerable<string> MultiplyStrings(string first, IEnumerable<string> second)
        {
            return MultiplyStrings(new string[] { first }, second);
        }

        /// <summary>
        /// Returns a sequence of strings gained from concatenating all the strings 
        /// in <paramref name="first"/> with all the strings in <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The sequence of strings to form the left-hand-side of the concatenations.</param>
        /// <param name="second">The sequence of strings to form the right-hand-side of the concatenations.</param>
        private static IEnumerable<string> MultiplyStrings(IEnumerable<string> first, IEnumerable<string> second)
        {
            List<string> result = new List<string>();

            foreach (string str1 in first)
            {
                foreach (string str2 in second)
                {
                    if (IsSpaceOrPseudoSpaceMark(str1))
                    {
                        result.Add(str1 + str2);
                    }
                    else
                    {
                        #region Redundant
                        //PersianCombinationSpacingState combinationSpacingState;
                        //if (StartWithSpaceOrPseudoSpacePlus(str2) || StartWithSpaceOrPseudoSpaceStar(str2))
                        //{
                        //    combinationSpacingState = PersianCombinationSpacingState.PseudoSpace;
                        //    if (InflectionAnalyser.IsValidPhoneticComposition(PurifySymbols(str1), PurifySymbols(str2), combinationSpacingState))
                        //    {
                        //        result.Add(str1 + str2);
                        //    }
                        //}
                        //if (!StartWithSpaceOrPseudoSpacePlus(str2) || StartWithSpaceOrPseudoSpaceStar(str2))
                        //{
                        //    combinationSpacingState = PersianCombinationSpacingState.Continous;
                        //    if (InflectionAnalyser.IsValidPhoneticComposition(PurifySymbols(str1), PurifySymbols(str2), combinationSpacingState))
                        //    {
                        //        result.Add(str1 + str2);
                        //    }
                        //}
                        #endregion

                        //if (InflectionAnalyser.IsValidPhoneticComposition(PurifySymbols(str1), PurifySymbols(str2)))
                        //{
                        //    result.Add(str1 + str2);
                        //}
                    }
                }
            }

            return result;
        }

        private static bool IsSpaceOrPseudoSpaceMark(string str)
        {
            if (str == ReversePatternMatcher.SymbolHalfSpace.ToString() ||
                str == ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString() ||
                str == ReversePatternMatcher.SymbolSpaceOrHalfSpace.ToString() ||
                str == ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus.ToString() ||
                str == ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar.ToString() ||
                str == ReversePatternMatcher.SymbolSpacePlus.ToString() ||
                str == ReversePatternMatcher.SymbolSpaceStar.ToString())
            {
                return true;
            }

            return false;
        }

        private static string PurifySymbols(string str)
        {
            string tmpStr = str;

            if (tmpStr.Contains(ReversePatternMatcher.SymbolHalfSpace))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolHalfSpace.ToString(), "");
            }
            if (tmpStr.Contains(ReversePatternMatcher.SymbolHalfSpaceQuestionMark))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(), "");
            }
            if (tmpStr.Contains(ReversePatternMatcher.SymbolSpaceOrHalfSpace))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolSpaceOrHalfSpace.ToString(), "");
            }
            if (tmpStr.Contains(ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus.ToString(), "");
            }
            if (tmpStr.Contains(ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar.ToString(), "");
            }
            if (tmpStr.Contains(ReversePatternMatcher.SymbolSpacePlus))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolSpacePlus.ToString(), "");
            }
            if (tmpStr.Contains(ReversePatternMatcher.SymbolSpaceStar))
            {
                tmpStr = tmpStr.Replace(ReversePatternMatcher.SymbolSpaceStar.ToString(), "");
            }


            return tmpStr;
        }

        private static bool StartWithSpaceOrPseudoSpaceStar(string str)
        {
            if (str.StartsWith(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString()) ||
                str.StartsWith(ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar.ToString()) ||
                str.StartsWith(ReversePatternMatcher.SymbolSpaceStar.ToString()))
            {
                return true;
            }

            return false;
        }

        private static bool StartWithSpaceOrPseudoSpacePlus(string str)
        {
            if (str.StartsWith(ReversePatternMatcher.SymbolHalfSpace.ToString()) ||
                str.StartsWith(ReversePatternMatcher.SymbolSpaceOrHalfSpace.ToString()) ||
                str.StartsWith(ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus.ToString()) ||
                str.StartsWith(ReversePatternMatcher.SymbolSpacePlus.ToString()))
            {
                return true;
            }

            return false;
        }

        private static bool IsInSuffixPattern(string suffix, string[] patterns)
        {
            foreach (string pattern in patterns)
            {
                if (suffix == PurifySymbols(pattern))
                {
                    return true;
                }
            }

            return false;
        }
        private static bool IsInSuffixPattern(string suffix, string[] patterns, out string suffixWithSpacingSymbol)
        {
            suffixWithSpacingSymbol = suffix;

            foreach (string pattern in patterns)
            {
                if (suffix == PurifySymbols(pattern))
                {
                    suffixWithSpacingSymbol = pattern;
                    return true;
                }
            }

            return false;
        }
        
        #endregion
    }
}

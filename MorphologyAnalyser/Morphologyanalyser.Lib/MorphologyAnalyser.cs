// Author: Omid Kashefi 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi at 2010-March-08
//

using System;
using SCICT.NLP.Persian.Constants;
using SCICT.Utility;
using SCICT.NLP.Utility.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace SCICT.NLP.Morphology.Inflection
{
    ///<summary>
    /// Analysis Persian inflection rules 
    ///</summary>
    public static class InflectionAnalyser
    {
        #region Suffix Constants

        #region old
        //public static string[] IndefiniteYaaPermutation
        //{
        //    get
        //    {
        //        List<string> lst = new List<string>();
                
        //        lst.AddRange(m_indefiniteYaaPermutationHaaYaa);
        //        lst.AddRange(m_indefiniteYaaPermutationBase);
        //        lst.AddRange(m_indefiniteYaaPermutationAlef);

        //        return lst.ToArray();
        //    }

        //    private set { }
        //}
        //public static string[] m_indefiniteYaaPermutationHaaYaa = CreateIndefiniteYaaForHaaYaaPermutation().Distinct().ToArray();
        //public static string[] m_indefiniteYaaPermutationHaaYaa
        //{
        //    get
        //    {
        //        return m_indefiniteYaaPermutationHaaYaa;
        //    }

        //    private set { }
        //}
        //public static string[] m_ndefiniteYaaPermutationBase = CreateIndefiniteYaaBasePermutation().Distinct().ToArray();
        //public static string[] m_indefiniteYaaPermutationBase
        //{
        //    get
        //    {
        //        return m_ndefiniteYaaPermutationBase;
        //    }

        //    private set { }
        //}
        //public static string[] m_indefiniteYaaPermutationAlef = CreateIndefiniteYaaForAlefPermutation().Distinct().ToArray();
        //public static string[] m_indefiniteYaaPermutationAlef
        //{
        //    get
        //    {
        //        return m_indefiniteYaaPermutationAlef;
        //    }

        //    private set { }
        //}

        //public static string[] m_yaaBadalAzKasraPermutation = CreateYaaBadalAzKasraPermutation().Distinct().ToArray();
        //public static string[] YaaBadalAzKasraPermutation
        //{
        //    get
        //    {
        //        return m_yaaBadalAzKasraPermutation;
        //    }

        //    private set { }
        //}

        //public static string[] ToBeVerbPermutation
        //{
        //    get
        //    {
        //        List<string> lst = new List<string>();

        //        lst.AddRange(m_toBeVerbPermutationBase);
        //        lst.AddRange(m_toBeVerbPermutationAlef);
        //        lst.AddRange(m_toBeVerbPermutationHaaYaa);

        //        return lst.ToArray();
        //    }

        //    private set { }
        //}
        //public static string[] m_toBeVerbPermutationBase = CreateToBeVerbBasePermutation().Distinct().ToArray();
        //public static string[] m_toBeVerbPermutationBase
        //{
        //    get
        //    {
        //        return m_toBeVerbPermutationBase;
        //    }

        //    private set { }
        //}
        //public static string[] m_toBeVerbPermutationAlef = CreateToBeVerbForAlefPermutation().Distinct().ToArray();
        //public static string[] m_toBeVerbPermutationAlef
        //{
        //    get
        //    {
        //        return m_toBeVerbPermutationAlef;
        //    }

        //    private set { }
        //}
        //public static string[] m_toBeVerbPermutationHaaYaa = CreateToBeVerbForHaaYaaPermutation().Distinct().ToArray();
        //public static string[] m_toBeVerbPermutationHaaYaa
        //{
        //    get
        //    {
        //        return m_toBeVerbPermutationHaaYaa;
        //    }

        //    private set { }
        //}

        //public static string[] ObjectivePronounPermutation
        //{
        //    get
        //    {
        //        List<string> lst = new List<string>();

        //        lst.AddRange(m_objectivePronounPermutationBase);
        //        lst.AddRange(m_objectivePronounPermutationAlef);
        //        lst.AddRange(m_objectivePronounPermutationHaaYaa);

        //        return lst.ToArray();
        //    }

        //    private set { }
        //}
        //public static string[] m_objectivePronounPermutationBase = CreateObjectivePronounBasePermutation().Distinct().ToArray();
        //public static string[] m_objectivePronounPermutationBase
        //{
        //    get
        //    {
        //        return m_objectivePronounPermutationBase;
        //    }

        //    private set { }
        //}
        //public static string[] m_objectivePronounPermutationHaaYaa = CreateObjectivePronounForHaaYaaPermutation().Distinct().ToArray();
        //public static string[] m_objectivePronounPermutationHaaYaa
        //{
        //    get
        //    {
        //        return m_objectivePronounPermutationHaaYaa;
        //    }

        //    private set { }
        //}
        //public static string[] m_objectivePronounPermutationAlef = CreateObjectivePronounForAlefPermutation().Distinct().ToArray();
        //public static string[] m_objectivePronounPermutationAlef
        //{
        //    get
        //    {
        //        return m_objectivePronounPermutationAlef;
        //    }

        //    private set { }
        //}

        //public static string[] m_pluralHaaPermutation = CreatePluralHaaPermutation().Distinct().ToArray();
        //public static string[] PluralHaaPermutation
        //{
        //    get
        //    {
        //        return m_pluralHaaPermutation;
        //    }

        //    private set { }
        //}

        //public static string[] PluralAnnPermutation
        //{
        //    get
        //    {
        //        List<string> lst = new List<string>();

        //        lst.AddRange(m_pluralAnnPermutationBase);
        //        lst.AddRange(m_pluralAnnPermutationAlef);
        //        lst.AddRange(m_pluralAnnPermutationHaa);

        //        return lst.ToArray();
        //    }

        //    private set { }
        //}
        //public static string[] m_pluralAnnPermutationBase = CreatePluralAnnBasePermutation().Distinct().ToArray();
        //public static string[] m_pluralAnnPermutationBase
        //{
        //    get
        //    {
        //        return m_pluralAnnPermutationBase;
        //    }

        //    private set { }
        //}
        //public static string[] m_pluralAnnPermutationAlef = CreatePluralAnnForAlefPermutation().Distinct().ToArray();
        //public static string[] m_pluralAnnPermutationAlef
        //{
        //    get
        //    {
        //        return m_pluralAnnPermutationAlef;
        //    }

        //    private set { }
        //}
        //public static string[] m_pluralAnnPermutationHaa = CreatePluralAnnForHaaPermutation().Distinct().ToArray();
        //public static string[] m_pluralAnnPermutationHaa
        //{
        //    get
        //    {
        //        return m_pluralAnnPermutationHaa;
        //    }

        //    private set { }
        //}

        //public static string[] m_comparativeAdjectivePermutation = CreateGetComparativeAdjectivePermutation().Distinct().ToArray();
        //public static string[] ComparativeAdjectivePermutation
        //{
        //    get
        //    {
        //        return m_comparativeAdjectivePermutation;
        //    }

        //    private set { }
        //}

        //public static string[] EnumerableAdjectivePermutation
        //{
        //    get
        //    {
        //        List<string> lst = new List<string>();

        //        lst.AddRange(EnumerableAdjectivePermutationAmbigus);
        //        lst.AddRange(EnumerableAdjectivePermutationOrdinal);

        //        return lst.ToArray();
        //    }

        //    private set { }
        //}
        //public static string[] m_enumerableAdjectivePermutationOrdinal = CreateGetEnumerableAdjectiveOrdinalPermutation().Distinct().ToArray();
        //public static string[] EnumerableAdjectivePermutationOrdinal
        //{
        //    get
        //    {
        //        return m_enumerableAdjectivePermutationOrdinal;
        //    }

        //    private set { }
        //}
        //public static string[] m_enumerableAdjectivePermutationAmbigus = CreateGetEnumerableAdjectiveAmbigusPermutation().Distinct().ToArray();
        //public static string[] EnumerableAdjectivePermutationAmbigus
        //{
        //    get
        //    {
        //        return m_enumerableAdjectivePermutationAmbigus;
        //    }

        //    private set { }
        //}

        //public static string[] YaaNesbatPermutation
        //{
        //    get
        //    {
        //        List<string> lst = new List<string>();

        //        lst.AddRange(m_yaaNesbatPermutationBase);
        //        lst.AddRange(m_yaaNesbatPermutationAlef);
        //        lst.AddRange(m_yaaNesbatPermutationHaaYaa);

        //        return lst.ToArray();
        //    }

        //    private set { }
        //}
        //public static string[] m_yaaNesbatPermutationBase = CreateGetYaaNesbatBasePermutation().Distinct().ToArray();
        //public static string[] m_yaaNesbatPermutationBase
        //{
        //    get
        //    {
        //        return m_yaaNesbatPermutationBase;
        //    }

        //    private set { }
        //}
        //public static string[] m_yaaNesbatPermutationAlef = CreateGetYaaNesbatForAlefPermutation().Distinct().ToArray();
        //public static string[] m_yaaNesbatPermutationAlef
        //{
        //    get
        //    {
        //        return m_yaaNesbatPermutationAlef;
        //    }

        //    private set { }
        //}
        //public static string[] m_yaaNesbatPermutationHaaYaa = CreateGetYaaNesbatForHaaYaaPermutation().Distinct().ToArray();
        //public static string[] m_yaaNesbatPermutationHaaYaa
        //{
        //    get
        //    {
        //        return m_yaaNesbatPermutationHaaYaa;
        //    }

        //    private set { }
        //}
        #endregion

        public static string[] IndefiniteYaaPermutation
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(m_indefiniteYaaPermutationHaaYaa);
                lst.AddRange(m_indefiniteYaaPermutationBase);
                lst.AddRange(m_indefiniteYaaPermutationAlef);

                return lst.ToArray();
            }
        }
        private static string[] m_indefiniteYaaPermutationHaaYaa = CreateIndefiniteYaaForHaaYaaPermutation().Distinct().ToArray();
        private static string[] m_indefiniteYaaPermutationBase = CreateIndefiniteYaaBasePermutation().Distinct().ToArray();
        private static string[] m_indefiniteYaaPermutationAlef = CreateIndefiniteYaaForAlefPermutation().Distinct().ToArray();

        private static string[] m_yaaBadalAzKasraPermutation = CreateYaaBadalAzKasraPermutation().Distinct().ToArray();
        public static string[] YaaBadalAzKasraPermutation
        {
            get
            {
                return m_yaaBadalAzKasraPermutation;
            }
        }

        public static string[] ToBeVerbPermutation
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(m_toBeVerbPermutationBase);
                lst.AddRange(m_toBeVerbPermutationAlef);
                lst.AddRange(m_toBeVerbPermutationHaaYaa);

                return lst.ToArray();
            }
        }
        private static string[] m_toBeVerbPermutationBase = CreateToBeVerbBasePermutation().Distinct().ToArray();
        private static string[] m_toBeVerbPermutationAlef = CreateToBeVerbForAlefPermutation().Distinct().ToArray();
        private static string[] m_toBeVerbPermutationHaaYaa = CreateToBeVerbForHaaYaaPermutation().Distinct().ToArray();

        public static string[] ObjectivePronounPermutation
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(m_objectivePronounPermutationBase);
                lst.AddRange(m_objectivePronounPermutationAlef);
                lst.AddRange(m_objectivePronounPermutationHaaYaa);

                return lst.ToArray();
            }
        }
        private static string[] m_objectivePronounPermutationBase = CreateObjectivePronounBasePermutation().Distinct().ToArray();
        private static string[] m_objectivePronounPermutationHaaYaa = CreateObjectivePronounForHaaYaaPermutation().Distinct().ToArray();
        private static string[] m_objectivePronounPermutationAlef = CreateObjectivePronounForAlefPermutation().Distinct().ToArray();

        private static string[] m_pluralHaaPermutation = CreatePluralHaaPermutation().Distinct().ToArray();
        public static string[] PluralHaaPermutation
        {
            get
            {
                return m_pluralHaaPermutation;
            }
        }

        public static string[] PluralAnnPermutation
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(m_pluralAnnPermutationBase);
                lst.AddRange(m_pluralAnnPermutationAlef);
                lst.AddRange(m_pluralAnnPermutationHaa);

                return lst.ToArray();
            }
        }
        private static string[] m_pluralAnnPermutationBase = CreatePluralAnnBasePermutation().Distinct().ToArray();
        private static string[] m_pluralAnnPermutationAlef = CreatePluralAnnForAlefPermutation().Distinct().ToArray();
        private static string[] m_pluralAnnPermutationHaa = CreatePluralAnnForHaaPermutation().Distinct().ToArray();

        private static string[] m_comparativeAdjectivePermutation = CreateComparativeAdjectivePermutation().Distinct().ToArray();
        public static string[] ComparativeAdjectivePermutation
        {
            get
            {
                return m_comparativeAdjectivePermutation;
            }
        }

        public static string[] EnumerableAdjectivePermutation
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(m_enumerableAdjectivePermutationAmbigus);
                lst.AddRange(m_enumerableAdjectivePermutationOrdinal);

                return lst.ToArray();
            }
        }
        private static string[] m_enumerableAdjectivePermutationOrdinal = CreateEnumerableAdjectiveOrdinalPermutation().Distinct().ToArray();
        private static string[] m_enumerableAdjectivePermutationAmbigus = CreateEnumerableAdjectiveAmbigusPermutation().Distinct().ToArray();

        public static string[] YaaNesbatPermutation
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(m_yaaNesbatPermutationBase);
                lst.AddRange(m_yaaNesbatPermutationAlef);
                lst.AddRange(m_yaaNesbatPermutationHaaYaa);

                return lst.ToArray();
            }
        }
        private static string[] m_yaaNesbatPermutationBase = CreateYaaNesbatBasePermutation().Distinct().ToArray();
        private static string[] m_yaaNesbatPermutationAlef = CreateYaaNesbatForAlefPermutation().Distinct().ToArray();
        private static string[] m_yaaNesbatPermutationHaaYaa = CreateYaaNesbatForHaaYaaPermutation().Distinct().ToArray();

        public static string[] ColloqualInflectionPatters
        {
            get
            {
                List<string> lst = new List<string>();

                lst.AddRange(CreateColloqualToBeVerbBasePermutation());
                lst.AddRange(CreateColloqualObjectivePronounBasePermutation());
                lst.AddRange(CreateColloqualPluralHaaPermutation());
                lst.AddRange(CreateColloqualPluralAnnBasePermutation());
                lst.AddRange(CreateColloqualComparativeAdjectivePermutation());
                lst.AddRange(CreateColloqualEnumerableAdjectiveOrdinalPermutation());
                lst.AddRange(CreateColloqualEnumerableAdjectiveAmbigusPermutation());
                lst.AddRange(CreateColloqualYaaNesbatBasePermutation());

                return lst.Distinct().ToArray();
            }
        }

        #endregion

        #region Create Suffixual Inflection Patterns

        private static IEnumerable<string> CreateIndefiniteYaaBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianSuffixes.IndefiniteYaaBase);
        }
        private static IEnumerable<string> CreateIndefiniteYaaForAlefPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianSuffixes.IndefiniteYaaPermutedForAlef);
        }
        private static IEnumerable<string> CreateIndefiniteYaaForHaaYaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianSuffixes.IndefiniteYaaPermutedForHaaYaa);
        }

        private static IEnumerable<string> CreateYaaBadalAzKasraPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpace.ToString(),
                        PersianSuffixes.YaaBadalAzKasre);
        }

        private static IEnumerable<string> CreateToBeVerbBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianSuffixes.ToBeVerbsBase);
        }
        private static IEnumerable<string> CreateToBeVerbForAlefPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianSuffixes.ToBeVerbsPermutedForAlef);
        }
        private static IEnumerable<string> CreateToBeVerbForHaaYaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianSuffixes.ToBeVerbsPermutedForHaaYaa);
        }

        private static IEnumerable<string> CreateObjectivePronounBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        MultiplyStrings(PersianSuffixes.ObjectivePronounsBase, PersianSuffixes.ToBeVerbsBase).Concat(
                            PersianSuffixes.ObjectivePronounsBase));
        }
        private static IEnumerable<string> CreateObjectivePronounForAlefPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        MultiplyStrings(PersianSuffixes.ObjectivePronounsPermutedForAlef, PersianSuffixes.ToBeVerbsBase).Concat(
                            PersianSuffixes.ObjectivePronounsPermutedForAlef));
        }
        private static IEnumerable<string> CreateObjectivePronounForHaaYaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        MultiplyStrings(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsBase).Concat(
                            PersianSuffixes.ObjectivePronounsPermutedForHaaYaa));
        }

        private static IEnumerable<string> CreatePluralHaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignHaa, m_objectivePronounPermutationAlef).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianSuffixes.ToBeVerbsPermutedForAlef).Concat(
                               MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianSuffixes.IndefiniteYaaPermutedForAlef).Concat(
                                   MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianSuffixes.YaaBadalAzKasre).Concat(
                                            PersianSuffixes.PluralSignHaa)))));
        }

        private static IEnumerable<string> CreatePluralAnnBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignAanBase, m_objectivePronounPermutationBase).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignAanBase, PersianSuffixes.ToBeVerbsBase).Concat(
                               MultiplyStrings(PersianSuffixes.PluralSignAanBase, PersianSuffixes.IndefiniteYaaBase).Concat(
                                   PersianSuffixes.PluralSignAan))));
        }
        private static IEnumerable<string> CreatePluralAnnForAlefPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignAanPermutedForAlef, m_objectivePronounPermutationBase).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignAanPermutedForAlef, PersianSuffixes.ToBeVerbsBase).Concat(
                               MultiplyStrings(PersianSuffixes.PluralSignAanPermutedForAlef, PersianSuffixes.IndefiniteYaaBase).Concat(
                                   PersianSuffixes.PluralSignAanPermutedForAlef))));
        }
        private static IEnumerable<string> CreatePluralAnnForHaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignAanPermutedForHaa, m_objectivePronounPermutationBase).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignAanPermutedForHaa, PersianSuffixes.ToBeVerbsBase).Concat(
                               MultiplyStrings(PersianSuffixes.PluralSignAanPermutedForHaa, PersianSuffixes.IndefiniteYaaBase).Concat(
                                   PersianSuffixes.PluralSignAanPermutedForHaa))));
        }

        private static IEnumerable<string> CreateComparativeAdjectivePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.ComparativeAdjectives, m_objectivePronounPermutationBase).Concat(
                           MultiplyStrings(PersianSuffixes.ComparativeAdjectives, PersianSuffixes.ToBeVerbsBase).Concat(
                               MultiplyStrings(PersianSuffixes.ComparativeAdjectives, PersianSuffixes.IndefiniteYaaBase).Concat(
                                   MultiplyStrings(PersianSuffixes.ComparativeAdjectives, PluralHaaPermutation).Concat(
                                       MultiplyStrings(PersianSuffixes.ComparativeAdjectives, m_pluralAnnPermutationBase).Concat(
                                           PersianSuffixes.ComparativeAdjectives))))));

        }

        private static IEnumerable<string> CreateEnumerableAdjectiveOrdinalPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, m_objectivePronounPermutationBase).Concat(
                           MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, PersianSuffixes.ToBeVerbsBase).Concat(
                               MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, PersianSuffixes.IndefiniteYaaBase).Concat(
                                   MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, PluralHaaPermutation).Concat(
                                       PersianSuffixes.EnumerableAdjectiveOrdinal)))));

        }
        private static IEnumerable<string> CreateEnumerableAdjectiveAmbigusPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, m_objectivePronounPermutationHaaYaa).Concat(
                           MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, PersianSuffixes.ToBeVerbsPermutedForHaaYaa).Concat(
                               MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa).Concat(
                                   MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, PluralHaaPermutation).Concat(
                                       PersianSuffixes.EnumerableAdjectiveAmbigus)))));

        }

        private static IEnumerable<string> CreateYaaNesbatBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.YaaNesbatBase, m_objectivePronounPermutationHaaYaa).Concat(
                           MultiplyStrings(PersianSuffixes.YaaNesbatBase, PersianSuffixes.ToBeVerbsPermutedForHaaYaa).Concat(
                               MultiplyStrings(PersianSuffixes.YaaNesbatBase, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa).Concat(
                                   MultiplyStrings(PersianSuffixes.YaaNesbatBase, PluralHaaPermutation).Concat(
                                       MultiplyStrings(PersianSuffixes.YaaNesbatBase, m_pluralAnnPermutationBase).Concat(
                                           MultiplyStrings(PersianSuffixes.YaaNesbatBase, ComparativeAdjectivePermutation).Concat(
                                               PersianSuffixes.YaaNesbatBase)))))));

        }
        private static IEnumerable<string> CreateYaaNesbatForAlefPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForAlef, m_objectivePronounPermutationHaaYaa).Concat(
                           MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForAlef, PersianSuffixes.ToBeVerbsPermutedForHaaYaa).Concat(
                               MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForAlef, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa).Concat(
                                   MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForAlef, PluralHaaPermutation).Concat(
                                       MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForAlef, m_pluralAnnPermutationBase).Concat(
                                           MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForAlef, ComparativeAdjectivePermutation).Concat(
                                               PersianSuffixes.YaaNesbatPermutedForAlef)))))));

        }
        private static IEnumerable<string> CreateYaaNesbatForHaaYaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForHaaYaa, m_objectivePronounPermutationHaaYaa).Concat(
                           MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.ToBeVerbsPermutedForHaaYaa).Concat(
                               MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa).Concat(
                                   MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForHaaYaa, PluralHaaPermutation).Concat(
                                       MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForHaaYaa, m_pluralAnnPermutationBase).Concat(
                                           MultiplyStrings(PersianSuffixes.YaaNesbatPermutedForHaaYaa, ComparativeAdjectivePermutation).Concat(
                                               PersianSuffixes.YaaNesbatPermutedForHaaYaa)))))));

        }

        private static IEnumerable<string> CreateColloqualToBeVerbBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        PersianColloqualSuffixes.ToBeVerbs);
        }
        private static IEnumerable<string> CreateColloqualObjectivePronounBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                        MultiplyStrings(PersianColloqualSuffixes.ObjectivePronouns, PersianSuffixes.ToBeVerbsBase).Concat(
                            MultiplyStrings(PersianColloqualSuffixes.ObjectivePronouns, PersianColloqualSuffixes.ToBeVerbs).Concat(
                                MultiplyStrings(PersianSuffixes.ObjectivePronouns, PersianColloqualSuffixes.ToBeVerbs).Concat(
                                    PersianColloqualSuffixes.ObjectivePronouns))));
        }
        private static IEnumerable<string> CreateColloqualPluralHaaPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignHaa, CreateColloqualObjectivePronounBasePermutation()).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignHaa, PersianColloqualSuffixes.ToBeVerbs)));
        }
        private static IEnumerable<string> CreateColloqualPluralAnnBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.PluralSignAanBase, CreateColloqualObjectivePronounBasePermutation()).Concat(
                           MultiplyStrings(PersianSuffixes.PluralSignAanBase, PersianColloqualSuffixes.ToBeVerbs)));
        }
        private static IEnumerable<string> CreateColloqualComparativeAdjectivePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.ComparativeAdjectives, CreateColloqualObjectivePronounBasePermutation()).Concat(
                           MultiplyStrings(PersianSuffixes.ComparativeAdjectives, PersianColloqualSuffixes.ToBeVerbs).Concat(
                                   MultiplyStrings(PersianSuffixes.ComparativeAdjectives, CreateColloqualPluralHaaPermutation()).Concat(
                                       MultiplyStrings(PersianSuffixes.ComparativeAdjectives, CreateColloqualPluralAnnBasePermutation())))));

        }
        private static IEnumerable<string> CreateColloqualEnumerableAdjectiveOrdinalPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, CreateColloqualObjectivePronounBasePermutation()).Concat(
                           MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, PersianColloqualSuffixes.ToBeVerbs).Concat(
                                   MultiplyStrings(PersianSuffixes.EnumerableAdjectiveOrdinal, CreateColloqualPluralHaaPermutation()))));

        }
        private static IEnumerable<string> CreateColloqualEnumerableAdjectiveAmbigusPermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, CreateColloqualObjectivePronounBasePermutation()).Concat(
                           MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, PersianColloqualSuffixes.ToBeVerbs).Concat(
                                   MultiplyStrings(PersianSuffixes.EnumerableAdjectiveAmbigus, CreateColloqualPluralHaaPermutation()))));

        }
        private static IEnumerable<string> CreateColloqualYaaNesbatBasePermutation()
        {
            return MultiplyStrings(ReversePatternMatcher.SymbolHalfSpaceQuestionMark.ToString(),
                       MultiplyStrings(PersianSuffixes.YaaNesbatBase, CreateColloqualObjectivePronounBasePermutation()).Concat(
                           MultiplyStrings(PersianSuffixes.YaaNesbatBase, PersianColloqualSuffixes.ToBeVerbs).Concat(
                                   MultiplyStrings(PersianSuffixes.YaaNesbatBase, CreateColloqualPluralHaaPermutation()).Concat(
                                       MultiplyStrings(PersianSuffixes.YaaNesbatBase, CreateColloqualPluralAnnBasePermutation()).Concat(
                                           MultiplyStrings(PersianSuffixes.YaaNesbatBase, CreateColloqualComparativeAdjectivePermutation()))))));

        }

        /*
        private IEnumerable<string> CreateHaaXPatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar,
                        MultiplyStrings("ها", PersianColloqualSuffixes.ObjectivePronounsColloqual)
                );
        }

        private IEnumerable<string> CreateAanXPatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart,
                        MultiplyStrings("ان", PersianColloqualSuffixes.ObjectivePronounsColloqual)
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
                wordPart // + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar //,
                        (new string[] { "ا", "ای", "ایی", "ائی" }).Concat(
                        MultiplyStrings("ای", PersianSuffixes.ObjectivePronouns)).Concat(
                        MultiplyStrings("ای" + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa)).Concat(
                        MultiplyStrings("ا", PersianColloqualSuffixes.ObjectivePronounsColloqual))
                   );
        }

        private IEnumerable<string> CreateObjPronounPatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar,
                        PersianColloqualSuffixes.ObjectivePronounsColloqual);
        }

        private IEnumerable<string> CreateToBePatternColloqual(string wordPart)
        {
            return MultiplyStrings(wordPart + ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar,
                        PersianColloqualSuffixes.ToBeVerbsColloqualSeperable).Concat(
                    MultiplyStrings(wordPart,
                        PersianColloqualSuffixes.ToBeVerbsColloqualInseperable)
                        );
        }
*/

        #endregion

        #region Public Interface

 
        ///<summary>
        /// Check whether that given composition is a valid composition as phonetic rules
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">Suffix word</param>
        ///<param name="pos">POS Tag of the stem</param>
        ///<param name="suffixCategory">Suffix category that matchs the rule</param>
        ///<returns>True if given composition is correct</returns>
        public static bool IsValidPhoneticComposition(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("ا"))
            {
                if (PhoneticCompositinRulesForAlef(stem, suffix)) //start with Alef
                {
                    return true;
                }
            }
            else if (stem.EndsWith("ی"))
            {
                if (PhoneticCompositinRulesForYaa(stem, suffix)) // Start With Yaa
                {
                    return true;
                }
            }
            else if (stem.EndsWith("ه"))
            {
                if (PhoneticCompositinRulesForHeh(stem, suffix, pos)) // Start with Heh
                {
                    return true;
                }
            }
            else if (stem.EndsWith("و"))
            {
                if (PhoneticCompositinRulesForVav(stem, suffix, pos)) // Start with Heh
                {
                    return true;
                }
            }
            else
            {
                if (PhoneticCompositinRulesForConsonants(stem, suffix)) // Start with Consonants
                {
                    return true;
                }
            }

            return false;
        }

        ///<summary>
        /// Check whether that given composition is a valid suffix declension referring stem's POS
        ///</summary>
        ///<param name="pos">Stem's POS tag</param>
        ///<param name="suffixCategory">Suffix Category</param>
        ///<returns>True if given composition is correct</returns>
        ///<exception cref="NotImplementedException"></exception>
        public static bool IsValidDeclension(PersianPOSTag pos, PersianSuffixesCategory suffixCategory)
        {
            if (pos.Has(PersianPOSTag.N) || pos.Has(PersianPOSTag.CL) || pos.Has(PersianPOSTag.PRO))
            {
                if (suffixCategory.Has(PersianSuffixesCategory.IndefiniteYaa))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.ObjectivePronoun))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.PluralSignAan))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.PluralSignHaa))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.ToBeVerb))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.YaaBadalAzKasre))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.YaaNesbat))
                {
                    return true;
                }
            }

            if (pos.Has(PersianPOSTag.AJ))
            {
                if (suffixCategory.Has(PersianSuffixesCategory.IndefiniteYaa))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.ObjectivePronoun))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.ToBeVerb))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.YaaBadalAzKasre))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.YaaNesbat))
                {
                    return true;
                }
                if (suffixCategory.Has(PersianSuffixesCategory.ComparativeAdjectives))
                {
                    return true;
                }
            }

            return false;
        }

        ///<summary>
        /// Get equal suffix which is correct as phonetic rules 
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">Suffix</param>
        ///<param name="suffixCategory">Suffix Category</param>
        ///<returns>Equal correct suffix</returns>
        public static string EqualSuffixWithCorrectPhonetic(string stem, string suffix, PersianSuffixesCategory suffixCategory)
        {
            //if (IsValidPhoneticComposition(stem, suffix))
            //{
            //    return suffix;
            //}
            
            if (stem.EndsWith("ا"))
            {
                suffix = ModifySuffixForAlef(suffix, suffixCategory);
            }
            else if (stem.EndsWith("ی"))
            {
                suffix = ModifySuffixForYaa(suffix, suffixCategory);
            }
            else if (stem.EndsWith("ه"))
            {
                suffix = ModifySuffixForHeh(suffix, suffixCategory);
            }
            else
            {
                suffix = ModifySuffixForConsonant(suffix, suffixCategory);
            }

            return suffix;
        }

        ///<summary>
        /// Get equal suffix which is correct as phonetic rules 
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">Suffix</param>
        ///<param name="suffixCategory">Suffix Category</param>
        ///<param name="pos">Persian POS tag</param>
        ///<returns>Equal correct suffix</returns>
        public static string EqualSuffixWithCorrectPhonetic(string stem, string suffix, PersianSuffixesCategory suffixCategory, PersianPOSTag pos)
        {

            if (stem.EndsWith("ا"))
            {
                suffix = ModifySuffixForAlef(suffix, suffixCategory);
            }
            else if (stem.EndsWith("ی"))
            {
                suffix = ModifySuffixForYaa(suffix, suffixCategory);
            }
            else if (stem.EndsWith("ه"))
            {
                suffix = ModifySuffixForHeh(suffix, suffixCategory, pos);
            }
            else
            {
                suffix = ModifySuffixForConsonant(suffix, suffixCategory);
            }

            return suffix;
        }

        ///<summary>
        /// Calculate spacing state of combining given stem and suffix
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">suffix</param>
        ///<param name="pos">Pos Tag</param>
        ///<returns>Spacing state</returns>
        public static PersianCombinationSpacingState CalculateSpacingState(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("ا"))
            {
                return SpacingStateForAlef(stem, suffix); // Start with Alef
            }
            else if (stem.EndsWith("ی"))
            {
                return SpacingStateForYaa(stem, suffix); // Start With Yaa
            }
            else if (stem.EndsWith("ه"))
            {
                return SpacingStateForHeh(stem, suffix, pos); // Start with Heh
            }
            else
            {
                return SpacingStateForConsonants(stem, suffix); // Start with Consonants
            }
        }

        ///<summary>
        /// Return POS category of possible words that an accept suffixes in given category
        ///</summary>
        ///<param name="suffixCategory">Suffix categoy</param>
        ///<returns>POS tag(s)</returns>
        public static PersianPOSTag AcceptingPOS(PersianSuffixesCategory suffixCategory)
        {
            PersianPOSTag posTag = PersianPOSTag.UserPOS;


            if (suffixCategory.Has(PersianSuffixesCategory.IndefiniteYaa) ||
                suffixCategory.Has(PersianSuffixesCategory.ObjectivePronoun) ||
                suffixCategory.Has(PersianSuffixesCategory.ToBeVerb) ||
                suffixCategory.Has(PersianSuffixesCategory.YaaBadalAzKasre) ||
                suffixCategory.Has(PersianSuffixesCategory.YaaNesbat))
            {
                //posTag = posTag.Set(PersianPOSTag.AJ);
                //posTag = posTag.Set(PersianPOSTag.CL);
                posTag = posTag.Set(PersianPOSTag.N);
                //posTag = posTag.Set(PersianPOSTag.PRO);
                posTag = posTag.Clear(PersianPOSTag.UserPOS);
            }

            if (suffixCategory.Has(PersianSuffixesCategory.PluralSignAan) ||
                suffixCategory.Has(PersianSuffixesCategory.PluralSignHaa))
            {
                //posTag = posTag.Clear(PersianPOSTag.AJ);
                //posTag = posTag.Set(PersianPOSTag.CL);
                posTag = posTag.Set(PersianPOSTag.N);
                //posTag = posTag.Set(PersianPOSTag.PRO);
                posTag = posTag.Clear(PersianPOSTag.UserPOS);
            }

            if (suffixCategory.Has(PersianSuffixesCategory.OrdinalEnumerableAdjective))
            {
                posTag = posTag.Set(PersianPOSTag.NUM);
                posTag = posTag.Set(PersianPOSTag.N);
                posTag = posTag.Clear(PersianPOSTag.UserPOS);
            }

            if (suffixCategory.Has(PersianSuffixesCategory.ComparativeAdjectives))
            {
                posTag = posTag.Set(PersianPOSTag.AJ);
                posTag = posTag.Clear(PersianPOSTag.UserPOS);
            }

            return posTag;
        }

        /// <summary>
        /// Determine the state of ending with consonant or vowel letter
        /// </summary>
        /// <param name="word">Inflected Word</param>
        /// <param name="suffix">Suffix</param>
        /// <param name="stem">Stem</param>
        /// <param name="suffixCategory">Suffix Category</param>
        /// <returns></returns>
        public static PersianPOSTag ConsonantVowelState(string word, string suffix, string stem, PersianSuffixesCategory suffixCategory)
        {
            if (!stem.EndsWith("ه") && !stem.EndsWith("و"))
            {
                return 0;
            }

            if (suffixCategory.Is(PersianSuffixesCategory.ComparativeAdjectives) ||
                suffixCategory.Is(PersianSuffixesCategory.OrdinalEnumerableAdjective) ||
                suffixCategory.Is(PersianSuffixesCategory.PluralSignHaa))
            {
                return 0;
            }
            
            if (stem.EndsWith("ه"))
            {
                if (PhoneticCompositinRulesForHeh(stem, suffix, PersianPOSTag.VowelEnding))
                {
                    if (word == (stem + PseudoSpace.ZWNJ + suffix) ||
                        suffix == PersianSuffixes.PluralSignAanPermutedForHaa[0])
                    {
                        return PersianPOSTag.VowelEnding;
                    }
                }
                if (PhoneticCompositinRulesForHeh(stem, suffix, PersianPOSTag.ConsonantalEnding))
                {
                    if (word == (stem + suffix))
                    {
                        return PersianPOSTag.ConsonantalEnding;
                    }
                }
            }
            else if (stem.EndsWith("و"))
            {
                if (!suffixCategory.Has(PersianSuffixesCategory.YaaBadalAzKasre))
                {
                    if (PhoneticCompositinRulesForVav(stem, suffix, PersianPOSTag.VowelEnding))
                    {
                        return PersianPOSTag.VowelEnding;
                    }
                    else if (PhoneticCompositinRulesForVav(stem, suffix, PersianPOSTag.ConsonantalEnding))
                    {
                        return PersianPOSTag.ConsonantalEnding;
                    }
                }
            }


            return 0;
        }

        ///<summary>
        /// Return the suffix category
        ///</summary>
        ///<param name="suffix">Suffix</param>
        ///<returns>Suffix category</returns>
        ///<exception cref="NotImplementedException"></exception>
        public static PersianSuffixesCategory SuffixCategory(string suffix)
        {
            PersianSuffixesCategory suffixCategory = 0;
            suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

            if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
            {
                suffixCategory |= PersianSuffixesCategory.ComparativeAdjectives;
            }
            if (IsInSuffixPattern(suffix, IndefiniteYaaPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.IndefiniteYaa;
            }
            if (IsInSuffixPattern(suffix, ObjectivePronounPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.ObjectivePronoun;
            }
            if (IsInSuffixPattern(suffix, EnumerableAdjectivePermutation))
            {
                suffixCategory |= PersianSuffixesCategory.OrdinalEnumerableAdjective;
            }
            if (IsInSuffixPattern(suffix, PluralAnnPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.PluralSignAan;
            }
            if (IsInSuffixPattern(suffix, PluralHaaPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.PluralSignHaa;
            }
            if (IsInSuffixPattern(suffix, ToBeVerbPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.ToBeVerb;
            }
            if (IsInSuffixPattern(suffix, YaaBadalAzKasraPermutation))
            {
                suffixCategory |= PersianSuffixesCategory.YaaBadalAzKasre;
            }
            if (IsInSuffixPattern(suffix, YaaNesbatPermutation))
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
        public static string EqualSuffixWithSpacingSymbols(string suffix)
        {
            string equalSuffix = suffix;

            suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

            if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, IndefiniteYaaPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, ObjectivePronounPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, EnumerableAdjectivePermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, PluralAnnPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, PluralHaaPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, ToBeVerbPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, YaaBadalAzKasraPermutation, out equalSuffix))
            {
            }
            else if (IsInSuffixPattern(suffix, YaaNesbatPermutation, out equalSuffix))
            {
            }

            return equalSuffix;
        }


        #endregion

        #region private methods

        #region Spacing State

        private static PersianCombinationSpacingState SpacingStateForYaa(string stem, string suffix)
        {
            if (stem.EndsWith("ی"))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationHaaYaa)) // Ye nakareh
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                if (IsInSuffixPattern(suffix, m_objectivePronounPermutationHaaYaa))
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationHaaYaa))
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationHaaYaa)) // yaa nesbat
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation)) // sefate tafsili
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                //{
                //    return PersianCombinationSpacingState.PseudoSpace;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
            }

            return PersianCombinationSpacingState.WhiteSpace;
        }

        private static PersianCombinationSpacingState SpacingStateForHeh(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("ه"))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (pos.Has(PersianPOSTag.VowelEnding))
                {
                    //if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // ye badal az kasre ezafe
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa)) // ye nakare
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForHaaYaa))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa)) // yaa nesbat
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForHaa) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    //{
                    //    return PersianCombinationSpacingState.Continous;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}

                    /*
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal))
                    //{
                    //    return PersianCombinationSpacingState.Continous;
                    //}
                    */

                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else
                {
                    if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationBase)) // ye nakare
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationBase))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationBase))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return PersianCombinationSpacingState.PseudoSpace;
                    }
                    else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationBase)) // yaa nesbat
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
                    {
                        return PersianCombinationSpacingState.PseudoSpace;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal))
                    //{
                    //    return PersianCombinationSpacingState.Continous;
                    //}
                }
            }

            return PersianCombinationSpacingState.WhiteSpace;
        }

        private static PersianCombinationSpacingState SpacingStateForAlef(string stem, string suffix)
        {
            if (stem.EndsWith("ا"))
            {
                //if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // yaa badal az kasre ezafe
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForAlef)) // yaa nakare
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForAlef))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForAlef))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForAlef))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForAlef)) // yaa nesbat
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}

                /*
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                 */

                return PersianCombinationSpacingState.Continous;
            }

            return PersianCombinationSpacingState.WhiteSpace;
        }

        private static PersianCombinationSpacingState SpacingStateForConsonants(string stem, string suffix)
        {
            if (stem.EndsWith(PersianAlphabets.Consonants.ToStringArray()))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationBase)) // ye nakare
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                else if (stem.EndsWith(PersianAlphabets.ConsonantsNonStickers.ToStringArray()))
                {
                    if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    //{
                    //    return PersianCombinationSpacingState.Continous;
                    //}
                }
                else if (stem.EndsWith(PersianAlphabets.ConsonantsStickers.ToStringArray()))
                {
                    if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return PersianCombinationSpacingState.PseudoSpace;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
                    {
                        return PersianCombinationSpacingState.PseudoSpace;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    //{
                    //    return PersianCombinationSpacingState.PseudoSpace;
                    //}
                }
            }

            return PersianCombinationSpacingState.WhiteSpace;
        }
        
        #endregion

        #region Phonetic Rules

        private static bool PhoneticCompositinRulesForYaa(string stem, string suffix)
        {
            if (stem.EndsWith("ی"))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationHaaYaa)) // Ye nakareh
                {
                    return true;
                }
                if (IsInSuffixPattern(suffix, m_objectivePronounPermutationHaaYaa))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationHaaYaa))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationHaaYaa)) // yaa nesbat
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool PhoneticCompositinRulesForHeh(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("ه"))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (pos.Has(PersianPOSTag.VowelEnding))
                {
                    if (IsInSuffixPattern(suffix, YaaBadalAzKasraPermutation)) // ye badal az kasre ezafe
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationHaaYaa)) // ye nakare
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationHaaYaa))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationHaaYaa))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationHaaYaa))
                    // yaa nesbat
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationHaa))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationBase)) // ye nakare
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationBase))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationBase))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationBase))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation)) // sefate tafsili
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                    {
                        return true;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }

        private static bool PhoneticCompositinRulesForAlef(string stem, string suffix)
        {
            if (stem.EndsWith("ا"))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");
                
                if (IsInSuffixPattern(suffix, YaaBadalAzKasraPermutation)) // yaa badal az kasre ezafe
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationAlef)) // yaa nakare
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationAlef))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationAlef))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationAlef))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationAlef)) // yaa nesbat
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation)) // sefate tafsili
                {
                    return true;
                }
            }

            return false;
        }

        private static bool PhoneticCompositinRulesForVav(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("و"))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (pos.Has(PersianPOSTag.VowelEnding))
                {
                    if (IsInSuffixPattern(suffix, YaaBadalAzKasraPermutation)) // yaa badal az kasre ezafe
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationAlef)) // yaa nakare
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationAlef))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationAlef))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationAlef))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationAlef))
                        // yaa nesbat
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation)) // sefate tafsili
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationBase)) // ye nakare
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationBase))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationBase))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationBase))
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation)) // sefate tafsili
                    {
                        return true;
                    }
                    else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                    {
                        return true;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }
        
        private static bool PhoneticCompositinRulesForConsonants(string stem, string suffix)
        {
            if (stem.EndsWith(PersianAlphabets.Consonants.ToStringArray()))
            {
                suffix = suffix.Replace(PseudoSpace.ZWNJ.ToString(), "");

                if (IsInSuffixPattern(suffix, m_indefiniteYaaPermutationBase)) // ye nakare
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_objectivePronounPermutationBase))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_toBeVerbPermutationBase))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_pluralAnnPermutationBase))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, m_yaaNesbatPermutationBase))
                {
                    return true;
                }
                else if (IsInSuffixPattern(suffix, PluralHaaPermutation))
                {
                    return true;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                //{
                //    return true;
                //}
                else if (IsInSuffixPattern(suffix, ComparativeAdjectivePermutation))
                {
                    return true;
                }

            }

            return false;
        }

        #endregion

        #region Equivallent Suffix With Correct Phonetic

        private static string ModifySuffixForAlef(string suffix, PersianSuffixesCategory category)
        {
            if (category.Has(PersianSuffixesCategory.IndefiniteYaa))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForAlef, PersianSuffixes.IndefiniteYaaBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForAlef, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa);
            }
            else if (category.Has(PersianSuffixesCategory.YaaNesbat))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForAlef, PersianSuffixes.YaaNesbatBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForAlef, PersianSuffixes.YaaNesbatPermutedForHaaYaa);
            }
            else if (category.Has(PersianSuffixesCategory.ObjectivePronoun))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForAlef, PersianSuffixes.ObjectivePronounsBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForAlef, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa);
            }
            else if (category.Has(PersianSuffixesCategory.ToBeVerb))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForAlef, PersianSuffixes.ToBeVerbsBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForAlef, PersianSuffixes.ToBeVerbsPermutedForHaaYaa);
            }
            else if (category.Has(PersianSuffixesCategory.PluralSignAan))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanPermutedForAlef, PersianSuffixes.PluralSignAanBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanPermutedForAlef, PersianSuffixes.PluralSignAanPermutedForHaa);
            }

            return suffix;
        }
        private static string ModifySuffixForYaa(string suffix, PersianSuffixesCategory category)
        {
            if (category.Has(PersianSuffixesCategory.IndefiniteYaa))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.YaaNesbat))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.YaaNesbatBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.YaaNesbatPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.ObjectivePronoun))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ObjectivePronounsBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ObjectivePronounsPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.ToBeVerb))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsPermutedForAlef);
            }

            return suffix;
        }
        private static string ModifySuffixForHeh(string suffix, PersianSuffixesCategory category)
        {
            if (category.Has(PersianSuffixesCategory.IndefiniteYaa))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.YaaNesbat))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.YaaNesbatBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.YaaNesbatPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.ObjectivePronoun))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ObjectivePronounsBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ObjectivePronounsPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.ToBeVerb))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.PluralSignAan))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanPermutedForHaa, PersianSuffixes.PluralSignAanBase);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanPermutedForHaa, PersianSuffixes.PluralSignAanPermutedForAlef);
            }

            return suffix;
        }
        private static string ModifySuffixForHeh(string suffix, PersianSuffixesCategory category, PersianPOSTag pos)
        {
            if (pos.Has(PersianPOSTag.VowelEnding))
            {
                if (category.Has(PersianSuffixesCategory.IndefiniteYaa))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaBase);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa, PersianSuffixes.IndefiniteYaaPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.YaaNesbat))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.YaaNesbatBase);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatPermutedForHaaYaa, PersianSuffixes.YaaNesbatPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.ObjectivePronoun))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ObjectivePronounsBase);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, PersianSuffixes.ObjectivePronounsPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.ToBeVerb))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsBase);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsPermutedForHaaYaa, PersianSuffixes.ToBeVerbsPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.PluralSignAan))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanPermutedForHaa, PersianSuffixes.PluralSignAanBase);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanPermutedForHaa, PersianSuffixes.PluralSignAanPermutedForAlef);
                }
            }
            else
            {
                if (category.Has(PersianSuffixesCategory.IndefiniteYaa))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaBase, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaBase, PersianSuffixes.IndefiniteYaaPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.YaaNesbat))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatBase, PersianSuffixes.YaaNesbatPermutedForHaaYaa);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatBase, PersianSuffixes.YaaNesbatPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.ObjectivePronoun))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsBase, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsBase, PersianSuffixes.ObjectivePronounsPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.ToBeVerb))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsBase, PersianSuffixes.ToBeVerbsPermutedForHaaYaa);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsBase, PersianSuffixes.ToBeVerbsPermutedForAlef);
                }
                else if (category.Has(PersianSuffixesCategory.PluralSignAan))
                {
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanBase, PersianSuffixes.PluralSignAanPermutedForHaa);
                    suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanBase, PersianSuffixes.PluralSignAanPermutedForAlef);
                }
            }

            return suffix;
        }
        private static string ModifySuffixForConsonant(string suffix, PersianSuffixesCategory category)
        {
            if (category.Has(PersianSuffixesCategory.IndefiniteYaa))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaBase, PersianSuffixes.IndefiniteYaaPermutedForHaaYaa);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.IndefiniteYaaBase, PersianSuffixes.IndefiniteYaaPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.YaaNesbat))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatBase, PersianSuffixes.YaaNesbatPermutedForHaaYaa);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.YaaNesbatBase, PersianSuffixes.YaaNesbatPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.ObjectivePronoun))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsBase, PersianSuffixes.ObjectivePronounsPermutedForHaaYaa);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ObjectivePronounsBase, PersianSuffixes.ObjectivePronounsPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.ToBeVerb))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsBase, PersianSuffixes.ToBeVerbsPermutedForHaaYaa);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.ToBeVerbsBase, PersianSuffixes.ToBeVerbsPermutedForAlef);
            }
            else if (category.Has(PersianSuffixesCategory.PluralSignAan))
            {
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanBase, PersianSuffixes.PluralSignAanPermutedForHaa);
                suffix = ReplaceWrongSuffixWithCorrectEquivallent(suffix, PersianSuffixes.PluralSignAanBase, PersianSuffixes.PluralSignAanPermutedForAlef);
            }

            return suffix;
        }

        private static string ReplaceWrongSuffixWithCorrectEquivallent(string suffix, string[] correcSuffixes, string[] wrongSuffixes)
        {
            if (suffix.StartsWith(wrongSuffixes))
            {
                for (int i = 0; i < wrongSuffixes.Length; ++i)
                {
                    if (suffix.StartsWith(wrongSuffixes[i]))
                    {
                        suffix = suffix.Remove(0, wrongSuffixes[i].Length);
                        if (correcSuffixes.Length > i)
                        {
                            suffix = correcSuffixes[i] + suffix;
                        }
                        break;
                    }
                }
            }

            return suffix;
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

                        result.Add(str1 + str2);
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

        #endregion
    }

    /// <summary>
    /// Helps recognize suffixes in Persian words.
    /// </summary>
    public class PersianSuffixLemmatizer
    {
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
        /// Initializes a new instance of the <see cref="PersianSuffixRecognizer"/> class.
        /// </summary>
        /// <param name="useColloquals">if set to <c>true</c> it will recognize colloqual affixes as well.</param>
        public PersianSuffixLemmatizer(bool useColloquals)
            : this(useColloquals, false)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PersianSuffixRecognizer"/> class.
        /// </summary>
        /// <param name="useColloquals">if set to <c>true</c> it will recognize colloqual affixes as well.</param>
        /// <param name="uniqueResults">if set to <c>true</c> unique results will be returned.</param>
        public PersianSuffixLemmatizer(bool useColloquals, bool uniqueResults) 
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

            InitPatternsList(suffixCategory, useColloquals);

            m_revPatternMatcher.SetEndingPatterns(m_lstPatterns);
        }

        public PersianSuffixLemmatizer(bool useColloquals, bool uniqueResults, PersianSuffixesCategory suffixCategory)
        {
            UseColloquals = useColloquals;
            ReturnUniqueResults = uniqueResults;

            InitPatternsList(suffixCategory, useColloquals);

            m_revPatternMatcher.SetEndingPatterns(m_lstPatterns);
        }

        /// <summary>
        /// Creates the list of Persian Suffixes patterns.
        /// </summary>
        private void InitPatternsList(PersianSuffixesCategory suffixCategory, bool useColloquals)
        {
            if (suffixCategory.Has(PersianSuffixesCategory.IndefiniteYaa))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.IndefiniteYaaPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.YaaBadalAzKasre))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.YaaBadalAzKasraPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.ToBeVerb))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.ToBeVerbPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.ObjectivePronoun))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.ObjectivePronounPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.PluralSignHaa))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.PluralHaaPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.PluralSignAan))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.PluralAnnPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.YaaNesbat))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.YaaNesbatPermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.ComparativeAdjectives))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.ComparativeAdjectivePermutation);
            }
            if (suffixCategory.Has(PersianSuffixesCategory.OrdinalEnumerableAdjective))
            {
                m_lstPatterns.AddRange(InflectionAnalyser.EnumerableAdjectivePermutation);
            }

            if (useColloquals)
            {
                m_lstPatterns.AddRange(InflectionAnalyser.ColloqualInflectionPatters);
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

        #endregion

    }

}

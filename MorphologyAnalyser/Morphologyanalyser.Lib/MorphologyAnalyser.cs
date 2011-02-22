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
using SCICT.NLP.Persian.Constants;
using SCICT.Utility;

namespace SCICT.NLP.Morphology.Inflection
{
    ///<summary>
    /// Analysis Persian inflection rules 
    ///</summary>
    public static class InflectionAnalyser
    {
        ///<summary>
        /// Check whether that given composition is a valid composition as phonetic rules
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">Suffix word</param>
        ///<param name="combinationSpacingState">Spacing state</param>
        ///<returns>True if given composition is correct</returns>
        public static bool IsValidPhoneticComposition(string stem, string suffix)
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
                if (PhoneticCompositinRulesForHeh(stem, suffix)) // Start with Heh
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
        /// Check whether that given composition is a valid composition as phonetic rules
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">Suffix word</param>
        ///<param name="pos">POS Tag of the stem</param>
        ///<param name="suffixCategory">Suffix category that matchs the rule</param>
        ///<returns>True if given composition is correct</returns>
        public static bool IsValidPhoneticComposition(string stem, string suffix, PersianPOSTag pos, out PersianSuffixesCategory suffixCategory)
        {
            if (stem.EndsWith("ا"))
            {
                if (PhoneticCompositinRulesForAlef(stem, suffix, out suffixCategory)) //start with Alef
                {
                    return true;
                }
            }
            else if (stem.EndsWith("ی"))
            {
                if (PhoneticCompositinRulesForYaa(stem, suffix, out suffixCategory)) // Start With Yaa
                {
                    return true;
                }
            }
            else if (stem.EndsWith("ه"))
            {
                if (PhoneticCompositinRulesForHeh(stem, suffix, pos, out suffixCategory)) // Start with Heh
                {
                    return true;
                }
            }
            else if (stem.EndsWith("و"))
            {
                if (PhoneticCompositinRulesForVav(stem, suffix, pos, out suffixCategory)) // Start with Heh
                {
                    return true;
                }
            }
            else
            {
                if (PhoneticCompositinRulesForConsonants(stem, suffix, out suffixCategory)) // Start with Consonants
                {
                    return true;
                }
            }

            return false;
        }

        ///<summary>
        /// Check whether that given composition is a valid suffix declension referring stem's POS
        ///</summary>
        ///<param name="stem">Stem word</param>
        ///<param name="suffix">Suffix word</param>
        ///<param name="combinationSpacingState">Spacing state</param>
        ///<param name="pos">Stem's POS tag</param>
        ///<param name="suffixCategory">Suffix Category</param>
        ///<returns>True if given composition is correct</returns>
        ///<exception cref="NotImplementedException"></exception>
        public static bool IsValidDeclension(string stem, string suffix, PersianCombinationSpacingState combinationSpacingState, PersianPOSTag pos, PersianSuffixesCategory suffixCategory)
        {
            throw new NotImplementedException();
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


        #region private methods

        #region Spacing State

        private static PersianCombinationSpacingState SpacingStateForYaa(string stem, string suffix)
        {
            if (stem.EndsWith("ی"))
            {
                if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa)) // Ye nakareh
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                if (Array.IndexOf(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, suffix) > -1)
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (Array.IndexOf(PersianSuffixes.ToBeVerbsPermutedForHaaYaa, suffix) > -1)
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa)) // yaa nesbat
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                {
                    return PersianCombinationSpacingState.PseudoSpace;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
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

            return PersianCombinationSpacingState.WhiteSpace;
        }

        private static PersianCombinationSpacingState SpacingStateForHeh(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("ه"))
            {
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
                    if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return PersianCombinationSpacingState.PseudoSpace;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase)) // yaa nesbat
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
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
                if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase))
                {
                    return PersianCombinationSpacingState.Continous;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal))
                //{
                //    return PersianCombinationSpacingState.Continous;
                //}
                else if (stem.EndsWith(PersianAlphabets.ConsonantsNonStickers.ToStringArray()))
                {
                    if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return PersianCombinationSpacingState.Continous;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
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
                    if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return PersianCombinationSpacingState.PseudoSpace;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
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
                if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa)) // Ye nakareh
                {
                    return true;
                }
                if (Array.IndexOf(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, suffix) > -1)
                {
                    return true;
                }
                else if (Array.IndexOf(PersianSuffixes.ToBeVerbsPermutedForHaaYaa, suffix) > -1)
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 2), suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), ""))) // yaa nesbat
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool PhoneticCompositinRulesForYaa(string stem, string suffix, out PersianSuffixesCategory suffixCategory)
        {
            if (stem.EndsWith("ی"))
            {
                if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa)) // Ye nakareh
                {
                    suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                    return true;
                }
                if (Array.IndexOf(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa, suffix) > -1)
                {
                    suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                    return true;
                }
                else if (Array.IndexOf(PersianSuffixes.ToBeVerbsPermutedForHaaYaa, suffix) > -1)
                {
                    suffixCategory = PersianSuffixesCategory.ToBeVerb;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 2), suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), ""))) // yaa nesbat
                {
                    suffixCategory = PersianSuffixesCategory.YaaNesbat;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                {
                    suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    suffixCategory = PersianSuffixesCategory.PluralSignAan;
                    return true;
                }
            }
            
            suffixCategory = 0;
            return false;
        }

        private static bool PhoneticCompositinRulesForHeh(string stem, string suffix)
        {
            if (stem.EndsWith("ه"))
            {
                if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // ye badal az kasre ezafe
                {
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa) || suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa) ||
                         (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0])))
                {
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForHaaYaa) || suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    return true;
                }
                else if ((suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 2),
                                                       suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), ""))) ||
                    (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                           suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), ""))))
                    // yaa nesbat
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                {
                    return true;
                }
                else if ((suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForHaa) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus)) ||
                          suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                {
                    return true;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                //{
                //    return true;
                //}
            }

            return false;
        }
        private static bool PhoneticCompositinRulesForHeh(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("ه"))
            {
                if (pos.Has(PersianPOSTag.VowelEnding))
                {
                    if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // ye badal az kasre ezafe
                    {
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa)) // ye nakare
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa))
                    {
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForHaaYaa))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 2),
                                                           suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    // yaa nesbat
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForHaa) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                    {
                        return true;
                    }
                }
                else
                {
                    if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                    {
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                           suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
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
        private static bool PhoneticCompositinRulesForHeh(string stem, string suffix, PersianPOSTag pos, out PersianSuffixesCategory suffixCategory)
        {
            if (stem.EndsWith("ه"))
            {
                if (pos.Has(PersianPOSTag.VowelEnding))
                {
                    if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // ye badal az kasre ezafe
                    {
                        suffixCategory = PersianSuffixesCategory.YaaBadalAzKasre;
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForHaaYaa)) // ye nakare
                    {
                        suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForHaaYaa))
                    {
                        suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForHaaYaa))
                    {
                        suffixCategory = PersianSuffixesCategory.ToBeVerb;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForHaaYaa) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 2),
                                                           suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    // yaa nesbat
                    {
                        suffixCategory = PersianSuffixesCategory.YaaNesbat;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForHaa) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveAmbigus))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignAan;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                    {
                        suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                        return true;
                    }
                }
                else
                {
                    if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                    {
                        suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                    {
                        suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                    {
                        suffixCategory = PersianSuffixesCategory.ToBeVerb;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                           suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    {
                        suffixCategory = PersianSuffixesCategory.YaaNesbat;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                    {
                        suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignAan;
                        return true;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                    //{
                    //    suffixCategory = PersianSuffixesCategory.OrdinalEnumerableAdjective;
                    //    return true;
                    //}
                }
            }

            suffixCategory = 0;
            return false;
        }

        private static bool PhoneticCompositinRulesForAlef(string stem, string suffix)
        {
            if (stem.EndsWith("ا"))
            {
                if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // yaa badal az kasre ezafe
                {
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForAlef)) // yaa nakare
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForAlef))
                {
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForAlef))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForAlef))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForAlef) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 2), suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), ""))) // yaa nesbat
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                {
                    return true;
                }
            }
            return false;
        }
        private static bool PhoneticCompositinRulesForAlef(string stem, string suffix, out PersianSuffixesCategory suffixCategory)
        {
            if (stem.EndsWith("ا"))
            {
                if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // yaa badal az kasre ezafe
                {
                    suffixCategory = PersianSuffixesCategory.YaaBadalAzKasre;
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForAlef)) // yaa nakare
                {
                    suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForAlef))
                {
                    suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForAlef))
                {
                    suffixCategory = PersianSuffixesCategory.ToBeVerb;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForAlef))
                {
                    suffixCategory = PersianSuffixesCategory.PluralSignAan;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForAlef) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 2), suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), ""))) // yaa nesbat
                {
                    suffixCategory = PersianSuffixesCategory.YaaNesbat;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                {
                    suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                    return true;
                }
            }

            suffixCategory = 0;
            return false;
        }

        private static bool PhoneticCompositinRulesForVav(string stem, string suffix, PersianPOSTag pos)
        {
            if (stem.EndsWith("و"))
            {
                if (pos.Has(PersianPOSTag.VowelEnding))
                {

                    if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // yaa badal az kasre ezafe
                    {
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForAlef)) // yaa nakare
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForAlef))
                    {
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForAlef))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForAlef))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForAlef) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 2),
                                                           suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                        // yaa nesbat
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                    {
                        return true;
                    }
                }
                else
                {
                    if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                    {
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                           suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                    {
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
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
        private static bool PhoneticCompositinRulesForVav(string stem, string suffix, PersianPOSTag pos, out PersianSuffixesCategory suffixCategory)
        {
            if (stem.EndsWith("و"))
            {
                if (pos.Has(PersianPOSTag.VowelEnding))
                {

                    if (suffix.IsIn(PersianSuffixes.YaaBadalAzKasre)) // yaa badal az kasre ezafe
                    {
                        suffixCategory = PersianSuffixesCategory.YaaBadalAzKasre;
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.IndefiniteYaaPermutedForAlef)) // yaa nakare
                    {
                        suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsPermutedForAlef))
                    {
                        suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsPermutedForAlef))
                    {
                        suffixCategory = PersianSuffixesCategory.ToBeVerb;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForAlef))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignAan;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatPermutedForAlef) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 2),
                                                           suffix.Remove(0, 2).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    // yaa nesbat
                    {
                        suffixCategory = PersianSuffixesCategory.YaaNesbat;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives)) // sefate tafsili
                    {
                        suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                        return true;
                    }
                }
                else
                {
                    if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                    {
                        suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                    {
                        suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                        return true;
                    }
                    else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                    {
                        suffixCategory = PersianSuffixesCategory.ToBeVerb;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                             PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                           suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                    {
                        suffixCategory = PersianSuffixesCategory.YaaNesbat;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                    {
                        suffixCategory = PersianSuffixesCategory.PluralSignAan;
                        return true;
                    }
                    else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                    {
                        suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                        return true;
                    }
                    //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                    //{
                    //    suffixCategory = PersianSuffixesCategory.OrdinalEnumerableAdjective;
                    //    return true;
                    //}
                }
            }

            suffixCategory = 0;
            return false;
        }


        private static bool PhoneticCompositinRulesForConsonants(string stem, string suffix)
        {
            if (stem.EndsWith(PersianAlphabets.Consonants.ToStringArray()))
            {
                if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                {
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                       suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                {
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    return true;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                //{
                //    return true;
                //}
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                {
                    return true;
                }

            }

            return false;
        }
        private static bool PhoneticCompositinRulesForConsonants(string stem, string suffix, out PersianSuffixesCategory suffixCategory)
        {
            if (stem.EndsWith(PersianAlphabets.Consonants.ToStringArray()))
            {
                if (suffix.IsIn(PersianSuffixes.IndefiniteYaaBase)) // ye nakare
                {
                    suffixCategory = PersianSuffixesCategory.IndefiniteYaa;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.ObjectivePronounsBase) && !suffix.StartsWith(PersianSuffixes.ComparativeAdjectives) && !suffix.StartsWith(PersianSuffixes.EnumerableAdjectiveOrdinal[0]))
                {
                    suffixCategory = PersianSuffixesCategory.ObjectivePronoun;
                    return true;
                }
                else if (suffix.IsIn(PersianSuffixes.ToBeVerbsBase))
                {
                    suffixCategory = PersianSuffixesCategory.ToBeVerb;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignAanBase))
                {
                    suffixCategory = PersianSuffixesCategory.PluralSignAan;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.YaaNesbatBase) &&
                         PhoneticCompositinRulesForYaa(suffix.Substring(0, 1),
                                                       suffix.Remove(0, 1).Replace(PseudoSpace.ZWNJ.ToString(), "")))
                {
                    suffixCategory = PersianSuffixesCategory.YaaNesbat;
                    return true;
                }
                else if (suffix.StartsWith(PersianSuffixes.PluralSignHaa))
                {
                    suffixCategory = PersianSuffixesCategory.PluralSignHaa;
                    return true;
                }
                //else if (suffix.StartsWith(PersianSuffixes.EnumerableAdjective))
                //{
                //    suffixCategory = PersianSuffixesCategory.OrdinalEnumerableAdjective;
                //    return true;
                //}
                else if (suffix.StartsWith(PersianSuffixes.ComparativeAdjectives))
                {
                    suffixCategory = PersianSuffixesCategory.ComparativeAdjectives;
                    return true;
                }

            }

            suffixCategory = 0;
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

        #endregion

    }
}

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
using System.Diagnostics;
using System.Text;
using System.Threading;
using SCICT.NLP.Morphology.Inflection;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility.Parsers;
using SCICT.NLP.Utility.WordGenerator;
using SCICT.Utility;
using System.Linq;
using SCICT.NLP.Utility;

namespace SCICT.NLP.TextProofing.SpellChecker
{
    ///<summary>
    /// Differnet state of correct spacing problems
    ///</summary>
    public enum SpaceCorrectionState
    {
        ///<summary>
        /// Mistakenly omission of a white space (e.g. computersoftware)
        ///</summary>
        SpaceDeletation = 1,
        ///<summary>
        /// Mistakenly omission of many white spaces (e.g. computersoftwarearchitecture)
        ///</summary>
        SpaceDeletationSerrially = SpaceDeletation + 1,
        ///<summary>
        /// Mistakenly Insertion of a white space in left side of word (e.g. comput er software)
        ///</summary>
        SpaceInsertationLeft = SpaceDeletationSerrially + 1,
        ///<summary>
        /// Mistakenly Insertion of white spaces in left side of word and its parted words (e.g. com p u ter software)
        ///</summary>
        SpaceInsertationLeftSerrially = SpaceInsertationLeft + 1,
        ///<summary>
        /// Mistakenly Insertion of a white space in right side of word (e.g. computer so ftware)
        ///</summary>
        SpaceInsertationRight = SpaceInsertationLeftSerrially + 1,
        ///<summary>
        /// Mistakenly Insertion of white spaces in right side of word and its parted words (e.g. computer s o ftware)
        ///</summary>
        SpaceInsertationRightSerrially = SpaceInsertationRight + 1,
        ///<summary>
        /// Spacing is correct
        ///</summary>
        None = SpaceInsertationRightSerrially + 1
    }

    ///<summary>
    /// Type of suggestion, warning or error
    ///</summary>
    public enum SuggestionType
    {
        ///<summary>
        /// Green for warning types
        ///</summary>
        Green = 1,
        ///<summary>
        /// Red for explicit error types
        ///</summary>
        Red   = Green + 1
    }

    ///<summary>
    /// Different rules for spellchecking
    ///</summary>
    [Flags]
    public enum SpellingRules
    {
        None = 0,
        ///<summary>
        /// Consider space correction for dictionary words
        ///</summary>
        VocabularyWordsSpaceCorrection = 1,
        ///<summary>
        /// Consider suggestions that may appear by assuming word as incomplete and complete rest of it (e.g. compu -> computer)
        ///</summary>
        CheckForCompletetion = VocabularyWordsSpaceCorrection * 2,
        ///<summary>
        /// Ignore writing of mocker Yeh of Kasra as "Heh with Yeh above" (e.g. "خانۀ" instead of "خانه‌ی")
        ///</summary>
        IgnoreHehYa = CheckForCompletetion * 2,
        ///<summary>
        /// Ignore single letters
        ///</summary>
        IgnoreLetters = IgnoreHehYa * 2,
        ///<summary>
        /// Ignore existnce of compund words with pseudo space in dictionary and accept them if the parts exists
        ///</summary>
        IgnorePseudoSpaceCompoundWords = IgnoreLetters * 2
    }

    ///<summary>
    /// Rules of Prespelling
    ///</summary>
    [Flags]
    public enum OnePassCorrectionRules
    {
        None = 0,
        ///<summary>
        /// Correct spacing of Prefixes such as Mee and NeMee (e.g. "می توانم" and "میتوانم" to "می‌توانم")
        ///</summary>
        CorrectPrefix = 1,
        ///<summary>
        /// Correct spacing of Suffixes such as Haa and objective pronouns (e.g. "شرکت ها" and "شرکتها" to "شرکت‌ها")
        ///</summary>
        CorrectSuffix = CorrectPrefix * 2,
        ///<summary>
        /// Correct sticked Be to Be with white space (e.g. "بعنوان" to "به عنوان")
        ///</summary>
        CorrectBe = CorrectSuffix * 2,
    }

    internal enum EzafeState
    {
        None                = 0,
        YaWithPseudoSpace   = 1,
        UrduYaOverHeh       = 2,
        PersianHamzaOverHeh = 3
    }
    
    internal class ReSpellingData
    {
        public string m_word;
        public string[] m_suggestions;
    }

    internal class AffixCorrectionData
    {
        public string m_word;
        public int m_suggestionCount;
        public bool m_havePhoneticSuffixProblem;
        public string[] m_suggestions;
    }

    ///<summary>
    /// configuration Class of Spell Checker
    ///</summary>
    public class PersianSpellCheckerConfig
    {
        ///<summary>
        /// The absolute path of stem's file.
        ///</summary>
        public string StemPath;
        ///<summary>
        /// The absolute path of dictionary file.
        ///</summary>
        public string DicPath;
        ///<summary>
        /// Indicates the Maximum Edit Distance that searched for finding suggestions
        ///</summary>
        public int EditDistance;
        ///<summary>
        /// Number of Suggestions
        ///</summary>
        public int SuggestionCount;

        /// <summary>
        /// Support for colloquial words inflection
        /// </summary>
        public bool ColloquialSupport = false;

        ///<summary>
        /// Constructor Class
        ///</summary>
        public PersianSpellCheckerConfig()
        {

        }

        ///<summary>
        /// Constructor Class
        ///</summary>
        ///<param name="dicPath">Absolute path of dictionary file</param>
        ///<param name="stemPath">Absolute path of verb stem file</param>
        ///<param name="editDist">Maximum Edit Distance that searched for finding suggestions</param>
        ///<param name="sugCnt">Number of Suggestions</param>
        ///<param name="colloquialSupport">Support for colloquial word inflection</param>
        public PersianSpellCheckerConfig(string dicPath, string stemPath, int editDist, int sugCnt, bool colloquialSupport)
        {
            this.DicPath = dicPath;
            this.StemPath = stemPath;
            this.EditDistance = editDist;
            this.SuggestionCount = sugCnt;
            this.ColloquialSupport = colloquialSupport;
        }
    }


    ///<summary>
    /// Persian Spell Checker
    /// This Class find and rank respelling suggestions for a incorrectly spelled Persian word
    ///</summary>
    public class PersianSpellChecker : SpellCheckerEngine
    {
        #region Private Members

        readonly PersianSuffixLemmatizer m_persianSuffixRecognizer;

        private bool m_isAffixStripped = false;
        private bool m_isRefinedforHehYa = false;

        private string m_wordWithoutSuffix, m_suffix;

        private bool m_ruleVocabularyWordsSpaceingCorrection = false;
        private bool m_ruleCheckForCompletetion = false;
        private bool m_ruleIgnoreHehYa = false;
        private bool m_ruleIgnoreLetters = false;
        private bool m_ruleIgnorePseudoSpaceCompoundWords = false;

        private bool m_ruleOnePassCorrectSuffixes = false;
        private bool m_ruleOnePassCorrectPrefixes = false;
        private bool m_ruleOnePassCorrectBe = false;

        private const string SpacingSuggestionMessage = "اصلاح فاصله‌گذاری";
        private const string ItterationSuggestionMessage = "تکرار اضافی";
        private const string SuffixSuggestionMessage = "اصلاح پسوند";
        private const string PrefixSuggestionMessage = "اصلاح پیشوند";
        private const string RuleBasedSuggestionMessage = "اصلاح مبتنی بر قوانین";
        private const string KasraRedundantSuggestionMessage = "اصلاح کسره‌ی اضافه";

        private string[] m_affixCorrectionSuggestions;
        private SpaceCorrectionState m_affixCorrectionSpacingState;

        private EzafeState EzafeStateOfWord; 

        #endregion

        #region Constructor
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="config">Spellchecker configuration</param>
        public PersianSpellChecker(SpellCheckerConfig config)
            : base(config)
        {
            this.SetDefaultSpellingRules();
            this.SetDefaultOnePassSpacingRules();

            this.m_persianSuffixRecognizer = new PersianSuffixLemmatizer(false, true);

        }

        public PersianSpellChecker(PersianSpellCheckerConfig config)
            : base(config.DicPath, config.StemPath, config.EditDistance, config.SuggestionCount)
        {
            this.SetDefaultSpellingRules();
            this.SetDefaultOnePassSpacingRules();

            this.m_persianSuffixRecognizer = new PersianSuffixLemmatizer(config.ColloquialSupport, true);
        }
        
        #endregion

        #region Public Members

        ///<summary>
        /// Set rules of spellchecking
        ///</summary>
        ///<param name="rules">Spellchecking rules (Logically OR rules)</param>
        public void SetSpellingRules(SpellingRules rules)
        {
            if ((rules & SpellingRules.VocabularyWordsSpaceCorrection) != 0)
            {
                this.m_ruleVocabularyWordsSpaceingCorrection = true;
            }
            if ((rules & SpellingRules.CheckForCompletetion) != 0)
            {
                this.m_ruleCheckForCompletetion = true;
            }
            if ((rules & SpellingRules.IgnoreHehYa) != 0)
            {
                this.m_ruleIgnoreHehYa = true;
            }
            if ((rules & SpellingRules.IgnoreLetters) != 0)
            {
                this.m_ruleIgnoreLetters = true;
            }
            if ((rules & SpellingRules.IgnorePseudoSpaceCompoundWords) != 0)
            {
                this.m_ruleIgnorePseudoSpaceCompoundWords = true;
            }
        }

        ///<summary>
        /// Remove rules of spellchecking
        ///</summary>
        ///<param name="rules">Spellchecking rules (Logically OR rules)</param>
        public void UnsetSpellingRules(SpellingRules rules)
        {
            if ((rules & SpellingRules.VocabularyWordsSpaceCorrection) != 0)
            {
                this.m_ruleVocabularyWordsSpaceingCorrection = false;
            }
            if ((rules & SpellingRules.CheckForCompletetion) != 0)
            {
                this.m_ruleCheckForCompletetion = false;
            }
            if ((rules & SpellingRules.IgnoreHehYa) != 0)
            {
                this.m_ruleIgnoreHehYa = false;
            }
            if ((rules & SpellingRules.IgnoreLetters) != 0)
            {
                this.m_ruleIgnoreLetters = false;
            }
            if ((rules & SpellingRules.IgnorePseudoSpaceCompoundWords) != 0)
            {
                this.m_ruleIgnorePseudoSpaceCompoundWords = false;
            }
        }

        ///<summary>
        /// Set rules of prespelling
        ///</summary>
        ///<param name="rules">Prespelling rules (Logically OR rules)</param>
        public void SetOnePassCorrectionRules(OnePassCorrectionRules rules)
        {
            if ((rules & OnePassCorrectionRules.CorrectSuffix) != 0)
            {
                this.m_ruleOnePassCorrectSuffixes = true;
            }
            if ((rules & OnePassCorrectionRules.CorrectPrefix) != 0)
            {
                this.m_ruleOnePassCorrectPrefixes = true;
            }
            if ((rules & OnePassCorrectionRules.CorrectBe) != 0)
            {
                this.m_ruleOnePassCorrectBe = true;
            }
        }

        ///<summary>
        /// Remove rules of prespelling
        ///</summary>
        ///<param name="rules">Prespelling rules (Logically OR rules)</param>
        public void UnsetOnePassCorrectionRules(OnePassCorrectionRules rules)
        {
            if ((rules & OnePassCorrectionRules.CorrectSuffix) != 0)
            {
                this.m_ruleOnePassCorrectSuffixes = false;
            }
            if ((rules & OnePassCorrectionRules.CorrectPrefix) != 0)
            {
                this.m_ruleOnePassCorrectPrefixes = false;
            }
            if ((rules & OnePassCorrectionRules.CorrectBe) != 0)
            {
                this.m_ruleOnePassCorrectBe = false;
            }
        }

        /// <summary>
        /// Get affix-striped word
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>List of plausible simple forms of word</returns>
        public string[] GetSimpleFormOfWord(string word)
        {
            try
            {
                word = word.Normalize(NormalizationForm.FormC);

                List<string> simpleWordList = new List<string>();

                ReversePatternMatcherPatternInfo[] suffixeResults = this.m_persianSuffixRecognizer.MatchForSuffix(word);
                if (suffixeResults.Length == 0)
                {
                    simpleWordList.Add(word);
                    return simpleWordList.ToArray();
                }

                foreach (ReversePatternMatcherPatternInfo rpmpi in suffixeResults)
                {
                    simpleWordList.Add(rpmpi.BaseWord);
                }

                simpleWordList.Add(word);
                return simpleWordList.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Data: {1}\n", word), ex);
            }
        }

        ///<summary>
        /// Prespell text
        ///</summary>
        ///<param name="word">Current word</param>
        ///<param name="preWord">Previous word in context</param>
        ///<param name="nxtWord">Next word in context</param>
        ///<param name="suggestionCount">Number of returned suggestions</param>
        ///<param name="suggestions">List of suggestions</param>
        ///<param name="suggestionType">Type of suggestins (Warning or Error)</param>
        ///<param name="spaceCorrectionState">State of space correction</param>
        ///<returns>True if the current word is correct, Flase if current word is incorrect</returns>
        public bool OnePassCorrection(string word, string preWord, string nxtWord, int suggestionCount, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState)
        {
            try
            {
                word = word.Normalize(NormalizationForm.FormC);
                preWord = preWord.Normalize(NormalizationForm.FormC);
                nxtWord = nxtWord.Normalize(NormalizationForm.FormC);
                
                
                suggestionType = SuggestionType.Green;
                spaceCorrectionState = SpaceCorrectionState.None;

                List<string> localSug = new List<string>();
                string tmpSug;

                #region Obsolete: Convert HaaYaa
                /*
                if (this.OnePassConvertingRuleExist(OnePassCorrectionRules.ConvertHehYa))
                {
                    tmpSug = RefineforHehYa(word);
                    if (tmpSug != word)
                    {
                        localSug.Add(tmpSug);
                        suggestions = localSug.ToArray();
                        if (suggestions.Length > 0)
                        {
                            return false;
                        }
                    }
                }
                */
                #endregion

                #region Convert Be

                if (this.OnePassConvertingRuleExist(OnePassCorrectionRules.CorrectBe))
                {
                    if (ContainPrefix(word) && !this.ContainWord(word))
                    {
                        string wordWithoutPrefix;
                        string prefix = ExtractPrefix(word, out wordWithoutPrefix);
                        if (wordWithoutPrefix != "")
                        {
                            if (prefix == "به" && this.ContainWord(wordWithoutPrefix))
                            {
                                string[] spacingSug = CheckForSpaceDeletation(word, out spaceCorrectionState);
                                if (spacingSug.Length == 0)
                                {
                                    tmpSug = prefix + " " + wordWithoutPrefix;

                                    if (!localSug.Contains(tmpSug))
                                    {
                                        localSug.Add(tmpSug);
                                        suggestions = localSug.ToArray();
                                        if (suggestions.Length > 0)
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Convert Prefixes

                if (this.OnePassConvertingRuleExist(OnePassCorrectionRules.CorrectPrefix))
                {
                    if (ContainPrefix(word))
                    {
                        if (IsStickerPrefix(word))
                        {
                            if (IsValidPrefix(word, nxtWord))
                            {
                                tmpSug = CheckForSpaceInsertation(word, word, nxtWord, out spaceCorrectionState);
                                if (tmpSug != "" && tmpSug != word)
                                {
                                    if (!localSug.Contains(tmpSug))
                                    {
                                        localSug.Add(tmpSug);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!this.ContainWord(word))
                            {
                                string wordWithoutPrefix;
                                string prefix = ExtractPrefix(word, out wordWithoutPrefix);
                                if (wordWithoutPrefix != "")
                                {
                                    if ((prefix == "می" || prefix == "نمی") && this.ContainWord(wordWithoutPrefix))
                                    {
                                        tmpSug = prefix + PseudoSpace.ZWNJ + wordWithoutPrefix;
                                        if (this.ContainWord(tmpSug))
                                        {
                                            if (!localSug.Contains(tmpSug))
                                            {
                                                localSug.Add(tmpSug);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    suggestions = localSug.ToArray();
                    if (suggestions.Length > 0)
                    {
                        return false;
                    }
                }

                #endregion

                #region Convert Suffixes
                bool isPhoneticallyChanged;
                if (this.OnePassConvertingRuleExist(OnePassCorrectionRules.CorrectSuffix))
                {
                    if (!IsPrefix(nxtWord))
                    {
                        if (!CheckSuffixSpacing(word, nxtWord, out suggestions, out suggestionType, out spaceCorrectionState, out isPhoneticallyChanged))
                        {
                            if (!isPhoneticallyChanged)
                            {
                                return false;
                            }
                        }
                    }
                }

                #endregion

                #region Obsolete: Covert Haa
                /*
                if (this.OnePassConvertingRuleExist(OnePassCorrectionRules.ConvertHaa))
                {
                    if (this.ContainWord(word) && !base.IsRealWord(word))
                    {
                        if (!word.Contains(PseudoSpace.ZWNJ.ToString()))
                        {
                            SpaceCorrectionState scs;
                            ReversePatternMatcherPatternInfo[] rpmp = this.StripAffixs(word);
                            if (rpmp.Length > 0)
                            {
                                foreach (ReversePatternMatcherPatternInfo pair in rpmp)
                                {
                                    if (pair.Suffix.StartsWith("ها"))
                                    {
                                        tmpSug = CheckForSpaceInsertation(pair.BaseWord, pair.BaseWord, pair.Suffix, out scs);

                                        if (tmpSug == word)
                                        {
                                            break;
                                        }
                                        else if (tmpSug != "")
                                        {
                                            if (!localSug.Contains(tmpSug))
                                            {
                                                localSug.Add(tmpSug);

                                                suggestions = localSug.ToArray();
                                                if (suggestions.Length > 0)
                                                {
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //check Vocabulary Words Space Correction Rule
                    if (nxtWord.StartsWith("ها"))
                    {
                        tmpSug = CheckForSpaceInsertation(word, word, nxtWord, out spaceCorrectionState);
                        if (tmpSug != "" && tmpSug != word)
                        {
                            if (!localSug.Contains(tmpSug))
                            {
                                localSug.Add(tmpSug);

                                suggestions = localSug.ToArray();
                                if (suggestions.Length > 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                */
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Data: {1}, {2}, {3}\n", preWord, word, nxtWord), ex);
            }

            suggestions = new string[0];
            return true;
        }

        ///<summary>
        /// Check and correct spelling
        ///</summary>
        ///<param name="word">Current word</param>
        ///<param name="preWord">Previous word in context</param>
        ///<param name="nxtWord">Next word in context</param>
        ///<param name="suggestionCount">Number of returned suggestions</param>
        ///<param name="suggestions">List of suggestions</param>
        ///<param name="suggestionType">Type of suggestins (Warning or Error)</param>
        ///<param name="spaceCorrectionState">State of space correction</param>
        ///<returns>True if the current word is correct, Flase if current word is incorrect</returns>
        public bool CheckSpelling(string word, string preWord, string nxtWord, int suggestionCount, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState)
        {
            try
            {
                word = word.Normalize(NormalizationForm.FormC);
                preWord = preWord.Normalize(NormalizationForm.FormC);
                nxtWord = nxtWord.Normalize(NormalizationForm.FormC);

                m_affixCorrectionSpacingState = SpaceCorrectionState.None;
                m_affixCorrectionSuggestions = new string[0];
                
                //Clear ranking detail
                base.m_rankingDetail.Clear();

                #region Ignore Spell Checking of Letters

                if (SpellingRuleExist(SpellingRules.IgnoreLetters))
                {
                    if (word.Length == 1)
                    {
                        suggestions = new string[0];
                        suggestionType = SuggestionType.Green;
                        spaceCorrectionState = SpaceCorrectionState.None;
                        return true;
                    }
                }

                #endregion

                #region Refine Word for Heh + Ya

                word = RefineforEzafe(word, out this.EzafeStateOfWord);

                #endregion

                #region Check Iteration

                bool iterationRes = CheckForIteration(word, nxtWord, suggestionCount, out suggestions, out suggestionType, out spaceCorrectionState, true);
                if (iterationRes == false)
                {
                    return false;
                }

                #endregion

                #region Check for Affix Spacing

                bool needsSpellChecking;
                if (!CheckAffixSpacing(word, preWord, nxtWord, out suggestions, out suggestionType, out spaceCorrectionState, out needsSpellChecking))
                {
                    if (!needsSpellChecking)
                    {
                       return false;
                    }

                    m_affixCorrectionSuggestions = suggestions;
                    m_affixCorrectionSpacingState = spaceCorrectionState;
                }

                #endregion

                #region Check for Vocabulary Word Wrong Spacing

                if (SpellingRuleExist(SpellingRules.VocabularyWordsSpaceCorrection))
                {

                    if (word != nxtWord)
                    {
                        string sug = word + PseudoSpace.ZWNJ + nxtWord;

                        if (this.ContainWord(sug) && sug != word)
                        {
                            spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;
                            suggestionType = SuggestionType.Green;
                            suggestions = new string[] { sug };
                            return false;
                        }

                        
                        if (PersianAlphabets.NonStickerChars.Contains(word[word.Length - 1]))
                        {
                            sug = word + nxtWord;

                            if (this.ContainWord(sug) && sug != word)
                            {
                                spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;
                                suggestionType = SuggestionType.Green;
                                suggestions = new string[] { sug };
                                return false;
                            }
                        }
                    }
                }

                #endregion

                #region Check for Existance

                if (this.ContainWord(word))
                {
                    if (this.m_isRefinedforHehYa == false | this.SpellingRuleExist(SpellingRules.IgnoreHehYa))
                    {
                        return true;
                    }
                    else if (this.m_isRefinedforHehYa == true)
                    {
                        suggestions = new string[] { word };
                        suggestionType = SuggestionType.Red;
                        spaceCorrectionState = SpaceCorrectionState.None;
                        RankingDetail.Add(word, KasraRedundantSuggestionMessage);

                        return false;
                    }
                }

                #endregion

                DoAdvancedSpellCheck(word, preWord, nxtWord, suggestionCount, out suggestions, out suggestionType, out spaceCorrectionState);

                #region Revert HahYa

                if (this.SpellingRuleExist(SpellingRules.IgnoreHehYa))
                {
                    suggestions = RevertEzafe(suggestions, this.EzafeStateOfWord);
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Data: {1}, {2}, {3}\n", preWord, word, nxtWord), ex);
            }

            return false;
        }

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="userSelectedWord">Form of word which user select to add to dictionary</param>
        /// <param name="originalWord">Original word without lemmatization</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public new bool AddToDictionary(string userSelectedWord, string originalWord)
        {
            try
            {
                userSelectedWord = userSelectedWord.Normalize(NormalizationForm.FormC);
                userSelectedWord = StringUtil.RefineAndFilterPersianWord(userSelectedWord);

                originalWord = originalWord.Normalize(NormalizationForm.FormC);
                originalWord = StringUtil.RefineAndFilterPersianWord(originalWord);

                string suffix = originalWord.Remove(0, userSelectedWord.Length);

                PersianSuffixesCategory suffixCategory = InflectionAnalyser.SuffixCategory(suffix);
                PersianPOSTag extractedPOSTag = InflectionAnalyser.AcceptingPOS(suffixCategory);

                extractedPOSTag.Set(InflectionAnalyser.ConsonantVowelState(originalWord, suffix, userSelectedWord, suffixCategory));

                PersianPOSTag existingPOStag = WordPOS(userSelectedWord);
                if (existingPOStag.Has(extractedPOSTag))
                {
                    return false;
                }
                else
                {
                    extractedPOSTag = extractedPOSTag.Set(existingPOStag);
                }

                return AddToDictionary(userSelectedWord, 0, extractedPOSTag);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Data: {1}, {2}\n", userSelectedWord, originalWord), ex);
            }
        }

        /// <summary>
        /// Add a correct word to dictionary
        /// </summary>
        /// <param name="userSelectedWord">Form of word which user select to add to dictionary</param>
        /// <param name="originalWord">Original word without lemmatization</param>
        /// <param name="fileName">File Name</param>
        ///<returns>True if word is successfully added, otherwise False</returns>
        public bool AddToDictionary(string userSelectedWord, string originalWord, string fileName)
        {
            try
            {
                userSelectedWord = userSelectedWord.Normalize(NormalizationForm.FormC);
                userSelectedWord = StringUtil.RefineAndFilterPersianWord(userSelectedWord);

                originalWord = originalWord.Normalize(NormalizationForm.FormC);
                originalWord = StringUtil.RefineAndFilterPersianWord(originalWord);

                string suffix;
                if (originalWord.Contains("گان") && !originalWord.Contains("ه") && userSelectedWord.EndsWith("ه"))
                {
                    suffix = originalWord.Remove(0, userSelectedWord.Length - 1);
                }
                else
                {
                    suffix = originalWord.Remove(0, userSelectedWord.Length);
                    if (suffix.Length > 0)
                    {
                        if (suffix[0] == PseudoSpace.ZWNJ)
                        {
                            suffix = suffix.Remove(0, 1);
                        }
                    }
                }

                PersianSuffixesCategory suffixCategory = InflectionAnalyser.SuffixCategory(suffix);
                PersianPOSTag extractedPOSTag = InflectionAnalyser.AcceptingPOS(suffixCategory);

                extractedPOSTag = extractedPOSTag.Set(InflectionAnalyser.ConsonantVowelState(originalWord, suffix, userSelectedWord, suffixCategory));

                PersianPOSTag existingPOStag = WordPOS(userSelectedWord);
                if (existingPOStag.Has(extractedPOSTag))
                {
                    return false;
                }
                else
                {
                    extractedPOSTag = extractedPOSTag.Set(existingPOStag);
                }

                return AddToDictionary(userSelectedWord, 0, extractedPOSTag, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Data: {1}, {2}, {3}\n", userSelectedWord, originalWord, fileName), ex);
            }
        }

        ///<summary>
        /// Return suggestion ranking detail
        ///</summary>
        public Dictionary<string, string> RankingDetail
        {
            get
            {
                return base.m_rankingDetail;
            }
        }

        /// <summary>
        /// check if the word is correct or exist in Ignore List
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns></returns>
        public bool ContainWord(string word)
        {
            try
            {
                word = word.Normalize(NormalizationForm.FormC);

                if (base.IsInIgnoreList(word))
                {
                    return true;
                }

                if (!base.IsRealWord(word))
                {
                    this.m_isAffixStripped = true;
                    if (!this.IsRealWordConsideringAffixes(word, out this.m_wordWithoutSuffix, out this.m_suffix))
                    {
                        if (SpellingRuleExist(SpellingRules.IgnorePseudoSpaceCompoundWords) && word.Contains(PseudoSpace.ZWNJ))
                        {
                            string[] parts = word.Split(PseudoSpace.ZWNJ);
                            foreach (string str in parts)
                            {
                                if (!ContainWord(str))
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                this.m_isAffixStripped = false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Data: {1}\n", word), ex);
            }
        }

        /*
        public bool CheckSpelling2(string word, string preWord, string nxtWord, int suggestionCount, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState)
        {
            spaceCorrectionState = SpaceCorrectionState.None;
            suggestionType = SuggestionType.Red;

            #region Check for: Affix Spacing, Existance and Iteration

            if (!CheckAffixSpacing(word, preWord, nxtWord, out suggestions, out suggestionType, out spaceCorrectionState))
            {
                return false;
            }

            bool iterationRes = CheckForIteration(word, nxtWord, suggestionCount, out suggestions, out suggestionType, out spaceCorrectionState, true);
            if (this.ContainWord2(word))
            {
                return iterationRes;
            }
            else if (iterationRes == false)
            {
                return false;
            }

            #endregion

            doAdvancedSpellCheck2(word, preWord, nxtWord, suggestionCount, out suggestions, out suggestionType, out spaceCorrectionState);

            return false;
        }
        */

        #endregion

        #region Private Method

        private void DoAdvancedSpellCheck(string word, string preWord, string nxtWord, int suggestionCount, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState)
        {
            spaceCorrectionState = SpaceCorrectionState.None;
            suggestionType = SuggestionType.Red;

            List<string> tmpSpellingSuggestions = new List<string>();
            List<string> tmpSpacingSuggestions = new List<string>();

            #region Check for Spacing Correction & Add

            string[] spacingSugs;
            try
            {
                spacingSugs = this.CheckSpellingWithSpacingConsideration(word, preWord, nxtWord,
                                                                         out spaceCorrectionState);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (spacingSugs.Length != 0)
            {
                if (spaceCorrectionState == SpaceCorrectionState.SpaceDeletationSerrially ||
                    spaceCorrectionState == SpaceCorrectionState.SpaceInsertationLeftSerrially ||
                    spaceCorrectionState == SpaceCorrectionState.SpaceInsertationRightSerrially)
                {
                    // =============== Check Length ================/
                    if (word.Length > MaxWordLengthToCheck + 5)
                    {
                        spacingSugs = AffixSpacingSeriallyCheck(SortSpacingSugs(spacingSugs));

                        spacingSugs = SortSpacingSugs(spacingSugs);

                        suggestions = spacingSugs;
                        return;
                    }
                }
            }

            #endregion

            #region  Correct Word Spelling

            string[] tmpSpellingSuggestion;
            Thread spellCorrectionThread;

            ReSpellingData spellingDataClass = new ReSpellingData();
            spellingDataClass.m_word = word;
            try
            {

                spellCorrectionThread = new Thread(new ParameterizedThreadStart(ReSpellingSuggestionsThreadSafe));
                spellCorrectionThread.Start((Object)spellingDataClass);

                //tmpSpellingSuggestion = ReSpellingSuggestions(word);

                //if (tmpSpellingSuggestion.Length > 0)
                //{
                //    suggestionType = SuggestionType.Red;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            #region Check for Affix Corrction

            string[] tmpAffixSuggestions;
            bool havePhoneticSuffixProblem = false;

            Thread affixlCorrectionThread;

            AffixCorrectionData affixDataClass = new AffixCorrectionData();
            affixDataClass.m_word = word;
            affixDataClass.m_suggestionCount = suggestionCount;
            
            try
            {
                affixlCorrectionThread = new Thread(new ParameterizedThreadStart(CheckSpellingWithAffixConsiderationThreadSafe));
                affixlCorrectionThread.Start((Object)affixDataClass);

                //tmpAffixSuggestions = this.CheckSpellingWithAffixConsideration(word, suggestionCount, out havePhoneticSuffixProblem);
                //tmpAffixSuggestions = SortSuggestions(word, tmpAffixSuggestions, suggestionCount / 2);

                spellCorrectionThread.Join();
                affixlCorrectionThread.Join();

                tmpSpellingSuggestion = spellingDataClass.m_suggestions;
                tmpAffixSuggestions = affixDataClass.m_suggestions;

                #region Peak proper number of suggestions considering affix issue

                int removeIndex = Math.Min(tmpAffixSuggestions.Length, suggestionCount / 2);
                List<string> tmpAffixSugList = tmpAffixSuggestions.ToList();
                tmpAffixSugList.RemoveRange(removeIndex, tmpAffixSuggestions.Length - removeIndex);
                tmpAffixSuggestions = tmpAffixSugList.ToArray();

                #endregion


                if (tmpAffixSuggestions.Length > 0 || tmpSpellingSuggestion.Length > 0)
                {
                    suggestionType = SuggestionType.Red;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            #region Auto Complete Suggestions

            string[] tmpAutoCompleteSuggestions = new string[0];

            if (spacingSugs.Length == 0)
            {
                if (this.SpellingRuleExist(SpellingRules.CheckForCompletetion))
                {
                    try
                    {
                        tmpAutoCompleteSuggestions = this.CompleteWord(word, suggestionCount);

                        if (tmpAutoCompleteSuggestions.Length > 0)
                        {
                            suggestionType = SuggestionType.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }


            #endregion

            #region Rule-based Suggestion

            string[] tmpRuleBaseSugs = new string[0];
            try
            {
                tmpRuleBaseSugs = this.RuleBasedSuggestions(word);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            #region Add Corrected Words

            tmpSpellingSuggestions.AddRange(tmpSpellingSuggestion);
            tmpSpellingSuggestions.AddRange(tmpAffixSuggestions);
            tmpSpellingSuggestions.AddRange(tmpAutoCompleteSuggestions);
            tmpSpellingSuggestions = tmpSpellingSuggestions.Distinct().ToList();

            tmpSpacingSuggestions.AddRange(spacingSugs);
            tmpSpacingSuggestions = tmpSpacingSuggestions.Distinct().ToList();

            string[] finalSpacingSeg = AffixSpacingSeriallyCheck(SortSpacingSugs(tmpSpacingSuggestions.ToArray())).Distinct().ToArray();

            //remove iterated results
            //string[] restSugs = tmpSuggestions;// RemoveIteration(tmpSuggestions.ToArray(), finalSpacingSeg);

            string[] finalSpellingSeg = SortSuggestions(word, tmpSpellingSuggestions.ToArray(),
                                                        Math.Min(tmpSpellingSuggestions.Count,
                                                                 suggestionCount - m_affixCorrectionSuggestions.Length));

            #region Rule-based Correction

            try
            {
                finalSpellingSeg = this.RuleBasedCorrection(finalSpellingSeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            #region Correct Spacing of Spelling Suggestion if Needed for an Spacing suggestion

            bool haveSpacingSuggestion = false;
            if (spaceCorrectionState != SpaceCorrectionState.SpaceInsertationLeftSerrially &&
                spaceCorrectionState != SpaceCorrectionState.SpaceInsertationRightSerrially &&
                finalSpellingSeg.Length != 0)
            {
                SortSpacingAndSpellingSugs(SortSuggestions(word, finalSpacingSeg, Math.Min(suggestionCount / 2,
                                                                                           suggestionCount - finalSpellingSeg.Length)), finalSpellingSeg, out haveSpacingSuggestion);

                if (haveSpacingSuggestion)
                {
                    finalSpellingSeg = CorrectFinalSuggestionsSpacing(preWord, nxtWord, finalSpellingSeg, spaceCorrectionState);
                }
                else
                {
                    spaceCorrectionState = SpaceCorrectionState.None;
                }
            }

            #endregion

            #region Finally sort spacing and spelling suggestions together

            string[] finalSugs;
            if (spaceCorrectionState == SpaceCorrectionState.SpaceDeletation ||
                spaceCorrectionState == SpaceCorrectionState.SpaceInsertationLeft ||
                spaceCorrectionState == SpaceCorrectionState.SpaceInsertationRight || 
                spaceCorrectionState == SpaceCorrectionState.SpaceDeletationSerrially)
            {
                finalSugs = SortSpacingAndSpellingSugs(SortSuggestions(word, finalSpacingSeg, Math.Min(suggestionCount / 2,
                                                                                           suggestionCount - finalSpellingSeg.Length)), finalSpellingSeg, out haveSpacingSuggestion);

                if (!haveSpacingSuggestion)
                {
                    spaceCorrectionState = SpaceCorrectionState.None;
                }
            }
            else
            {
                if (finalSpellingSeg.Length == 0)
                {
                    finalSugs = SortSpacingAndSpellingSugs(SortSuggestions(word, finalSpacingSeg, Math.Min(suggestionCount / 2,
                                                                                           suggestionCount - finalSpellingSeg.Length)), finalSpellingSeg, out haveSpacingSuggestion);

                    if (!haveSpacingSuggestion)
                    {
                        spaceCorrectionState = SpaceCorrectionState.None;
                    }
                }
                else
                {
                    finalSugs = finalSpellingSeg;
                    
                    spaceCorrectionState = SpaceCorrectionState.None;
                }
            }

            finalSugs = finalSugs.Distinct().ToArray();

            #endregion
                
            #region Add Rule-base Suggestion

            List<string> finalList = new List<string>();
            if (spaceCorrectionState != SpaceCorrectionState.SpaceInsertationLeftSerrially &&
                spaceCorrectionState != SpaceCorrectionState.SpaceInsertationRightSerrially &&
                finalSpellingSeg.Length != 0)
            {
                if (haveSpacingSuggestion)
                {
                    finalList.AddRange(CorrectFinalSuggestionsSpacing(preWord, nxtWord, tmpRuleBaseSugs, spaceCorrectionState));
                }
                else
                {
                    finalList.AddRange(tmpRuleBaseSugs);
                    spaceCorrectionState = SpaceCorrectionState.None;
                }
            }

            #endregion

            if (havePhoneticSuffixProblem)
            {
                finalList.AddRange(SortSuggestions(word, tmpAffixSuggestions, tmpAffixSuggestions.Length));
            }

            finalList.AddRange(finalSugs);

            if(finalList.Count == 0)
            {
                finalList.AddRange(spacingSugs);
            }

            #region Add Affix Correction Suggestions

            if (m_affixCorrectionSuggestions.Length > 0)
            {
                if (spaceCorrectionState == SpaceCorrectionState.None)
                {
                    spaceCorrectionState = m_affixCorrectionSpacingState;
                }

                foreach (string str in m_affixCorrectionSuggestions)
                {
                    finalList.Add(str);
                }
            }

            #endregion

            suggestions = finalList.Distinct().ToArray();

            #endregion
        }

        private string[] CorrectFinalSuggestionsSpacing(string preWord, string nxtWord, string[] sugList, SpaceCorrectionState spaceCorrectionState)
        {
            //Add Pre/Next Word to Corrected Words by: Spelling Correction
            for (int i = 0; i < sugList.Length; ++i)
            {
                if (spaceCorrectionState == SpaceCorrectionState.SpaceInsertationLeft ||
                    spaceCorrectionState == SpaceCorrectionState.SpaceInsertationLeftSerrially)
                {
                    if (!sugList[i].Contains(" "))
                    {
                        if (nxtWord.Length > 0)
                        {
                            string rankingKey = sugList[i];

                            sugList[i] = sugList[i] + " " + nxtWord;

                            string rankingValue = RankingDetail[rankingKey];
                            RankingDetail.Remove(rankingKey);
                            RankingDetail.Add(sugList[i], rankingValue);
                        }
                    }
                }
                else if (spaceCorrectionState == SpaceCorrectionState.SpaceInsertationRight ||
                         spaceCorrectionState == SpaceCorrectionState.SpaceInsertationRightSerrially)
                {
                    if (!sugList[i].Contains(" "))
                    {
                        if (preWord.Length > 0)
                        {
                            string rankingKey = sugList[i];

                            sugList[i] = preWord + " " + sugList[i];

                            string rankingValue = RankingDetail[rankingKey];
                            RankingDetail.Remove(rankingKey);
                            RankingDetail.Add(sugList[i], rankingValue);
                        }
                    }
                }
            }

            return sugList;
        }

        private void ReSpellingSuggestionsThreadSafe(Object data)
        {
            try
            {
                Thread.BeginThreadAffinity();

                ReSpellingData inputData = (ReSpellingData)data;
                string word = inputData.m_word;

                string[] wordParts = word.Split(new char[] { PseudoSpace.ZWNJ }, StringSplitOptions.RemoveEmptyEntries);

                if ((wordParts.Length == 1 || 
                     (wordParts[0].Length <= 3) &&  wordParts.Length < 4) ||
                    (wordParts[0].EndsWith("ه") && wordParts[1].StartsWith("گ")))
                {

                    inputData.m_suggestions = base.SpellingSuggestions2(word);

                    if (wordParts[0].EndsWith("ء"))
                    {
                        List<string> secondDistance = new List<string>(inputData.m_suggestions);
                        secondDistance.AddRange(base.SpellingSuggestions2(word.Remove(word.Length - 1, 1)));
                        inputData.m_suggestions = secondDistance.ToArray();
                    }

                    #region Magic Generation
                    //List<string> respellingSuggetions = new List<string>();
                    //for (int i = 1; i < EditDistance; ++i)
                    //{
                    //    string[] tmpSugArray = base.SpellingSuggestions3(word, i);
                    //    foreach (string tmpSug in tmpSugArray)
                    //    {
                    //        if (this.ContainWord(tmpSug))
                    //        {
                    //            respellingSuggetions.Add(tmpSug);
                    //        }
                    //    }

                    //    if (respellingSuggetions.Count > 0)
                    //    {
                    //        break;
                    //    }
                    //}

                    //inputData.m_suggestions = respellingSuggetions.Distinct().ToArray();
                    #endregion

                    return;
                }
                else if (wordParts.Length < 4)
                {
                    List<string> suggestions = new List<string>();
                    List<string> tmpNextPartSugs = new List<string>();
                    List<string> tmpPrePartSugs = new List<string>();
                    foreach (string str in wordParts)
                    {
                        suggestions.Clear();
                        tmpNextPartSugs.Clear();
                        tmpNextPartSugs.AddRange(base.SpellingSuggestions2(str));
                        if (tmpPrePartSugs.Count == 0)
                        {
                            tmpPrePartSugs.AddRange(tmpNextPartSugs);
                            continue;
                        }

                        foreach (string tmpNextPart in tmpNextPartSugs)
                        {
                            foreach (string tmpPrePart in tmpPrePartSugs)
                            {
                                suggestions.Add(tmpPrePart + PseudoSpace.ZWNJ + tmpNextPart);
                            }
                        }
                        tmpPrePartSugs.Clear();
                        tmpPrePartSugs.AddRange(suggestions);
                    }

                    //List<string> finalSug = new List<string>(ExtractRealWords(suggestions.Distinct().ToArray()));
                    List<string> finalSug = new List<string>(base.ExtractRealWords(suggestions.Distinct().ToArray()));
                    if (base.IsRealWord(wordParts[0]))
                    {
                        finalSug.AddRange(CompleteWord(wordParts[0]));
                    }

                    inputData.m_suggestions = finalSug.Distinct().ToArray();
                    Thread.EndThreadAffinity();
                    return;
                }
                else
                {
                    inputData.m_suggestions = new string[0];
                }
            }
            catch (Exception)
            {
                ((ReSpellingData)data).m_suggestions = new string[0];
                
                Thread.EndThreadAffinity();
                return;
            }
        }

        #region Obsolete ReSpellingSuggestion
        /*
        private string[] ReSpellingSuggestions(string word)
        {
            string[] wordParts = word.Split(new char[] {PseudoSpace.ZWNJ}, StringSplitOptions.RemoveEmptyEntries);

            if (wordParts.Length == 1)
            {
                return base.SpellingSuggestions(word);
                //WordGenerator wordGenerator = new WordGenerator();
                //string[] sugList = wordGenerator.GenerateRespelling(word.Substring(0, 3), 1);

                //List<string> sugs = new List<string>();
                //foreach(string str in sugList)
                //{
                //    sugs.AddRange(CompleteWord(str));
                //}
                //return sugs.Distinct().ToArray();
            }
            else
            {
                List<string> suggestions = new List<string>();
                List<string> tmpNextPartSugs = new List<string>();
                List<string> tmpPrePartSugs = new List<string>();
                foreach (string str in wordParts)
                {
                    suggestions.Clear();
                    tmpNextPartSugs.Clear();
                    tmpNextPartSugs.AddRange(base.SpellingSuggestions(str));
                    if (tmpPrePartSugs.Count == 0)
                    {
                        tmpPrePartSugs.AddRange(tmpNextPartSugs);
                        continue;
                    }

                    foreach (string tmpNextPart in tmpNextPartSugs)
                    {
                        foreach (string tmpPrePart in tmpPrePartSugs)
                        {
                            suggestions.Add(tmpPrePart + PseudoSpace.ZWNJ + tmpNextPart);
                        }
                    }
                    tmpPrePartSugs.Clear();
                    tmpPrePartSugs.AddRange(suggestions);
                }

                List<string> finalSug = new List<string>(ExtractRealWords(suggestions.Distinct().ToArray()));
                if (base.IsRealWord(wordParts[0]))
                {
                    finalSug.AddRange(CompleteWord(wordParts[0]));
                }

                return finalSug.Distinct().ToArray();

            }
        }
        */
        #endregion

        private string[] ReSpellingSuggestions(string word, int editDistance)
        {
            string[] wordParts = word.Split(new char[] { PseudoSpace.ZWNJ }, StringSplitOptions.RemoveEmptyEntries);

            if (wordParts.Length == 1)
            {
                return base.SpellingSuggestions(word, editDistance);
            }
            else
            {
                List<string> suggestions = new List<string>();
                List<string> tmpNextPartSugs = new List<string>();
                List<string> tmpPrePartSugs = new List<string>();
                foreach (string str in wordParts)
                {
                    suggestions.Clear();
                    tmpNextPartSugs.Clear();
                    tmpNextPartSugs.AddRange(base.SpellingSuggestions(str));
                    if (tmpPrePartSugs.Count == 0)
                    {
                        tmpPrePartSugs.AddRange(tmpNextPartSugs);
                        continue;
                    }

                    foreach (string tmpNextPart in tmpNextPartSugs)
                    {
                        foreach (string tmpPrePart in tmpPrePartSugs)
                        {
                            suggestions.Add(tmpPrePart + PseudoSpace.ZWNJ + tmpNextPart);
                        }
                    }
                    tmpPrePartSugs.Clear();
                    tmpPrePartSugs.AddRange(suggestions);
                }

                List<string> finalSug = new List<string>(base.ExtractRealWords(suggestions.Distinct().ToArray()));
                if (base.IsRealWord(wordParts[0]))
                {
                    finalSug.AddRange(CompleteWord(wordParts[0]));
                }

                return finalSug.Distinct().ToArray();

            }
        }

        private string[] ReSpellingSuggestions2(string word)
        {
            string[] wordParts = word.Split(new char[] {PseudoSpace.ZWNJ}, StringSplitOptions.RemoveEmptyEntries);

            if (wordParts.Length == 1)
            {
                string[] wordMayParts = SplitFromNonStickerLetters(word);

                if (wordMayParts.Length == 1)
                {
                    return base.SpellingSuggestions(word);
                }
                else
                {
                    wordParts = wordMayParts;
                }
            }
            
            List<string> suggestions = new List<string>();
            List<string> tmpNextPartSugs = new List<string>();
            List<string> tmpPrePartSugs = new List<string>();
            foreach (string str in wordParts)
            {
                suggestions.Clear();
                tmpNextPartSugs.Clear();
                tmpNextPartSugs.AddRange(base.SpellingSuggestions(str));
                if (tmpPrePartSugs.Count == 0)
                {
                    tmpPrePartSugs.AddRange(tmpNextPartSugs);
                    continue;
                }

                foreach (string tmpNextPart in tmpNextPartSugs)
                {
                    foreach (string tmpPrePart in tmpPrePartSugs)
                    {
                        suggestions.Add(tmpPrePart + PseudoSpace.ZWNJ + tmpNextPart);
                    }
                }
                tmpPrePartSugs.Clear();
                tmpPrePartSugs.AddRange(suggestions);
            }

            List<string> finalSug = new List<string>(base.ExtractRealWords(suggestions.Distinct().ToArray()));
            if (base.IsRealWord(wordParts[0]))
            {
                finalSug.AddRange(CompleteWord(wordParts[0]));
            }

            return finalSug.ToArray();
        }

        private string[] SplitFromNonStickerLetters(string word)
        {
            int start = 0;

            List<int> index = new List<int>();
            while (start > -1)
            {
                index.Add(word.IndexOfAny(PersianAlphabets.NonStickerChars, start + 1));
                start = index.Last();
            }

            double min = word.Length;
            int bestIndex = 0;
            foreach(int idx in index)
            {
                if (Math.Abs(word.Length / 2 - idx) < min)
                {
                    min = Math.Abs(word.Length / 2 - idx);
                    bestIndex = idx;
                }
            }

            return new string[] { word.Substring(0, bestIndex + 1), word.Substring(bestIndex + 1, word.Length - bestIndex - 1) };
        }

        private string[] RuleBasedSuggestions(string word)
        {
            return RuleBasedCorrection(new string[] {word});
        }

        private string[] RuleBasedCorrection(string[] wordArray)
        {
            List<string> sugList = new List<string>();

            foreach (string word in wordArray)
            {
                if (word.EndsWith(RuleBasedSpelling.PluralAT))
                {
                    sugList.AddRange(SuffixReplacement(word, RuleBasedSpelling.PluralAT,
                                                      RuleBasedSpelling.PluralATReplacement));
                }
                if(this.ContainWord(word))
                {
                    sugList.Add(word);
                }
            }

            return sugList.ToArray();
        }

        private string[] SuffixReplacement(string word, string[] replacee, string[] replacement)
        {
            string tmpWord = "";
            foreach(string rep in replacee)
            {
                if (word.EndsWith(rep))
                {
                    if (word.LastIndexOf(rep) >= 0)
                    {
                        tmpWord = word.Remove(word.LastIndexOf(rep), rep.Length);
                    }
                    //if (tmpWord.EndsWith(PseudoSpace.ZWNJ.ToString()))
                    if (tmpWord.Length > 1)
                    {
                        if (tmpWord[tmpWord.Length - 1] == PseudoSpace.ZWNJ)
                        {
                            tmpWord = tmpWord.Remove(tmpWord.Length - 1, 1);
                        }
                    }
                    if (base.IsRealWord(tmpWord))
                    {
                        word = tmpWord;
                        break;
                    }
                }
            }

            if (tmpWord != word)
            {
                return new string[0];
            }

            List<string> tmpLst = new List<string>();
            foreach (string rep in replacement)
            {
                string sug = tmpWord + rep;
                tmpLst.Add(sug);

                //Add ranking detail
                if (!base.m_rankingDetail.ContainsKey(sug))
                {
                    base.m_rankingDetail.Add(sug, RuleBasedSuggestionMessage);
                }
            }

            return tmpLst.ToArray();
        }
       
        private static string[] RemoveIteration(string[] baseWord, string[] array)
        {
            List<string> res = new List<string>();

            foreach (string str in array)
            {
                if (Array.IndexOf(baseWord, str) == -1)
                {
                    res.Add(str);
                }
            }

            return res.ToArray();
        }

        #region Iteration

        private bool CheckForIteration(string word, string nxtWord, int suggestionCount, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState, bool realWord)
        {
            if (word != nxtWord)
            {
                suggestions = new string[0];
                suggestionType = SuggestionType.Green;
                spaceCorrectionState = SpaceCorrectionState.None;

                return true;
            }

            List<string> tmpSug = new List<string>();
            string sug = CheckForSpaceInsertation(word, word, nxtWord, out spaceCorrectionState);
            if (sug.Length != 0)
            {
                tmpSug.Add(sug);
            }
            spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;

            if (realWord)
            {
                suggestionType = SuggestionType.Green;
                //Add One iteration in place of twice
                tmpSug.Add(word);
            }
            else
            {
                #region Check Spelling and Add
                string[] outSug;
                DoAdvancedSpellCheck(word, word, nxtWord, suggestionCount, out outSug, out suggestionType, out spaceCorrectionState);
                foreach (string str in outSug)
                {
                    if (!tmpSug.Contains(str))
                    {
                        if (tmpSug.Count < suggestionCount)
                        {
                            tmpSug.Add(str);
                        }
                    }
                }
                #endregion
            }

            #region Add ranking detail

            foreach (string str in tmpSug)
            {
                if (!base.m_rankingDetail.ContainsKey(str))
                {
                    base.m_rankingDetail.Add(str, ItterationSuggestionMessage);
                }
            }

            #endregion
            
            suggestions = tmpSug.Distinct().ToArray();

            return false;
        }

        #endregion

        #region Spacing

        /// <summary>
        /// correct wrong insert/delete of white space between words
        /// </summary>
        /// <param name="word">Input word</param>
        /// <param name="preWord">Previous word</param>
        /// <param name="nxtWord">Next word</param>
        /// <param name="spaceCorrectionState">return the spacing correction state</param>
        /// <returns>
        /// word with corrected spacing
        /// </returns>
        private string[] CheckSpellingWithSpacingConsideration(string word, string preWord, string nxtWord, out SpaceCorrectionState spaceCorrectionState)
        {
            List<string> sugs = new List<string>();

            #region Space Deletatin, No: 4, 5

            //string[] aser = AffixSpacingSeriallyCheck(this.CheckForSpaceDeletation(word, out spaceCorrectionState));
            string[] aser = this.CheckForSpaceDeletation(word, out spaceCorrectionState);
            sugs.AddRange(aser);

            #endregion

            #region Space Insertation, No: 1
            
            //if (sugs.Count <= 0)
            {
                SpaceCorrectionState tmpState = spaceCorrectionState;
                string sug = this.CheckForSpaceInsertation(word, preWord, nxtWord, out spaceCorrectionState);
                if (spaceCorrectionState == SpaceCorrectionState.None)
                {
                    spaceCorrectionState = tmpState;
                }

                if (sug.Length > 0)
                {
                    List<string> newSugs = new List<string>();

                    foreach (string str in sugs)
                    {
                        if (spaceCorrectionState == SpaceCorrectionState.SpaceInsertationLeft ||
                            spaceCorrectionState == SpaceCorrectionState.SpaceInsertationLeftSerrially)
                        {
                            newSugs.Add(str + ' ' + nxtWord);
                        }
                        else if (spaceCorrectionState == SpaceCorrectionState.SpaceInsertationRight ||
                                 spaceCorrectionState == SpaceCorrectionState.SpaceInsertationRightSerrially)
                        {
                            newSugs.Add(preWord + ' ' + str);
                        }
                    }
                    sugs.Clear();

                    sugs.AddRange(newSugs.ToArray());
                    sugs.Add(sug);
                }
            }

            #endregion

            #region CheckForSpaceDeletation for sequence of Insertaion and Deletation, No: 2, 3

            if (sugs.Count <= 0)
            {
                aser = this.CheckForSpaceInsertainDeletation(word, preWord, nxtWord, out spaceCorrectionState);
                sugs.AddRange(RemoveIteration(sugs.ToArray(), aser));
            }

            #endregion

            #region Check For Serrially Space Deletation, No 7

            if (sugs.Count <= 0)
            {

                if (word.Length < 60)
                {
                    aser = this.CheckForSpaceDeletationSerially(word, 0);
                    sugs.AddRange(RemoveIteration(sugs.ToArray(), aser));

                    if (sugs.Count > 0)
                    {
                        spaceCorrectionState = SpaceCorrectionState.SpaceDeletationSerrially;
                    }
                }
            }

            #endregion

            #region Check For Serrially Space Deletation and An Space Insertation, No 6

            if (sugs.Count <= 0)
            {
                if (word.Length + preWord.Length < 70 && word.Length + nxtWord.Length < 70)
                {
                    aser = CheckForSpaceDeletationSeriallyInsertation(word, preWord, nxtWord, out spaceCorrectionState);
                    sugs.AddRange(RemoveIteration(sugs.ToArray(), aser));
                }
            }

            #endregion


            #region Add ranking detail

            foreach (string str in sugs)
            {
                if (!base.m_rankingDetail.ContainsKey(str))
                {
                    base.m_rankingDetail.Add(str, SpacingSuggestionMessage);
                }
            }

            #endregion

            return sugs.ToArray();
        }

        #region Sort Spacing Suggestions

        private string[] SortSpacingSugs(string[] words)
        {
            if (words.Length <= 1)
            {
                return words;
            }

            Dictionary<string, double> dic = new Dictionary<string, double>();

            string[] splitedWords;
            double maturityCo = 0, avgFreq;
            foreach (string str in words)
            {
                if (!str.Contains(" "))
                {
                    return words;
                }

                splitedWords = str.Split(' ');
                avgFreq = CalcAvgFreq(splitedWords);
                if (avgFreq > CalcAvgFreq())
                {
                    maturityCo = avgFreq /
                                 (splitedWords.Length * (1 + CalcNumberofIteration(splitedWords)) +
                                  CalcNumberofEqualWords(splitedWords));
                    
                    if (CalcNumberofIteration(splitedWords) == 0)
                    {
                        dic.Add(str, maturityCo);
                    }
                }
            }

            List<string> suggestions = new List<string>();
            
            double avg = 0;
            if (dic.Count > 2)
            {
                avg = dic.Values.Average();
            }
            foreach (KeyValuePair<string, double> pair in dic)
            {
                if (pair.Value >= avg)
                {
                    suggestions.Add(pair.Key);
                }
            }

            string temp;
            for (int i = 0; i < suggestions.Count - 1; ++i)
            {
                for (int j = i + 1; j < suggestions.Count; ++j)
                {
                    if (dic[suggestions[i]] < dic[suggestions[j]])
                    {
                        temp = suggestions[i];
                        suggestions[i] = suggestions[j];
                        suggestions[j] = temp;
                    }
                }
            }
            
            return suggestions.ToArray();
        }

        private static int CalcNumberofEqualWords(string[] splitedWords)
        {
            int iteration = 0;

            List<string> myList = new List<string>(splitedWords);

            foreach (string str in splitedWords)
            {
                myList.RemoveAt(0);
                if (myList.Contains(str))
                {
                    ++iteration;
                }
            }

            return iteration;

        }

        private static int CalcNumberofIteration(string[] splitedWords)
        {
            int iteration = 0;

            for (int i = 0; i < splitedWords.Length - 1; ++i)
            {
                if (splitedWords[i] == splitedWords[i + 1])
                {
                    ++iteration;
                }
            }

            return iteration;
        }

        /*
private static string[] SortSpacingSugs(string[] words, int count)
{
    Dictionary<string, int> dic = new Dictionary<string, int>();

    foreach (string str in words)
    {
        dic.Add(str, str.Split(' ').Length);
    }

    string temp;
    for (int i = 0; i < words.Length - 1; ++i)
    {
        for (int j = i + 1; j < words.Length; ++j)
        {
            if (dic[words[i]] > dic[words[j]])
            {
                temp = words[i];
                words[i] = words[j];
                words[j] = temp;
            }
        }
    }

    string[] suggestions = new string[Math.Min(count, words.Length)];
    for (int i = 0; i < suggestions.Length; ++i)
    {
        suggestions[i] = words[i];
    }

    return suggestions;
}
*/

        #endregion

        private string CheckForSpaceInsertation(string word, string preWord, string nxtWord, out SpaceCorrectionState spaceCorrectionState)
        {
            spaceCorrectionState = SpaceCorrectionState.None;
            string suggestion;

            #region Word Joining Next Word

            if (word != nxtWord)
            {
                suggestion = word + PseudoSpace.ZWNJ + nxtWord;

                if (!this.ContainWord(suggestion) && suggestion != word)
                {
                    suggestion = word + nxtWord;
                }

                if (this.ContainWord(suggestion) && suggestion != word)
                {
                    spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;

                    return suggestion;
                }
            }

            #endregion

            #region Word Joining Previous Word

            if (word != preWord)
            {
                suggestion = preWord + PseudoSpace.ZWNJ + word;

                if (!this.ContainWord(suggestion) && suggestion != word)
                {
                    suggestion = preWord + word;
                }

                if (this.ContainWord(suggestion) && suggestion != word)
                {
                    spaceCorrectionState = SpaceCorrectionState.SpaceInsertationRight;

                    return suggestion;
                }
            }

            #endregion

            return "";
        }

        private string[] CheckForSpaceDeletation(string word, out SpaceCorrectionState spaceCorrectionState)
        {
            spaceCorrectionState = SpaceCorrectionState.None;

            List<string> sugs = new List<string>();

            // Do not replace ZWNJ with white space
            string[] words = SubstituteStepBy(word, PseudoSpace.ZWNJ, ' ');
            sugs.AddRange(ExtractTwoRealWordParts(words));

            string tmp = ExtractSeriallyPseudoStickedWords(word);
            if (tmp != "")
            {
                sugs.Add(tmp);
            }

            words = WordGenerator.GenerateRespelling(word, 1, RespellingGenerationType.Insert, ' ');
            sugs.AddRange(ExtractTwoRealWordParts(words));

            words = WordGenerator.GenerateRespelling(word, 1, RespellingGenerationType.Insert, PseudoSpace.ZWNJ);
            sugs.AddRange(ExtractRealWords(words));

            if (sugs.Count > 0)
            {
                spaceCorrectionState = SpaceCorrectionState.SpaceDeletation;
            }

            return sugs.Distinct().ToArray();
        }

        private string ExtractSeriallyPseudoStickedWords(string word)
        {
            string[] words = word.Split(PseudoSpace.ZWNJ);

            if (words.Length >= 3)
            {
                StringBuilder sb = new StringBuilder();
                int i;
                for (i = 0; i < words.Length; ++i)
                {
                    if (!ContainWord(words[i]))
                    {
                        break;
                    }
                    sb.Append(words[i] + " ");
                }

                if (i == words.Length)
                {
                    return sb.ToString().Trim();
                }
            }

            return "";
        }

        private static string[] SubstituteStepBy(string word, char cBase, char cSub)
        {
            List<string> words = new List<string>();

            int index = 0;
            do
            {
                index = word.IndexOf(cBase, index);
                if (index != -1)
                {
                    words.Add(word.Remove(index, 1).Insert(index, cSub.ToString()));
                    ++index;
                }
            } while (index != -1);

            return words.ToArray();
        }
        private string[] ExtractTwoRealWordParts(string[] words)
        {
            List<string> sugs = new List<string>();

            string[] wordParts;
            foreach (string word in words)
            {
                //wordParts = word.Split(PseudoSpace.ZWNJ);
                //if (wordParts.Length == 2)
                //{
                //    if (wordParts[0].Length > 0 && wordParts[1].Length > 0)
                //    {
                //        if (!wordParts[0].Contains(' ') && !wordParts[1].Contains(' '))
                //        {
                //            //Commented on 30 March 2010
                //            //if (this.ContainWord(word))
                //            if (base.IsRealWord(word))
                //            {
                //                sugs.Add(word);
                //            }
                //        }
                //    }
                //}

                wordParts = word.Split(' ');
                if (wordParts.Length == 2)
                {
                    if (wordParts[0].Length > 0 && wordParts[1].Length > 0)
                    {
                        //Commented on 25 Oct 2010
                        //if (((ContainWord(wordParts[0]) && wordParts[0].Length >= 4) || base.IsRealWord(wordParts[0])) && 
                        //    ((ContainWord(wordParts[1]) && wordParts[1].Length >= 4) || base.IsRealWord(wordParts[1])))
                        if ((ContainWord(wordParts[0]) && (wordParts[0][wordParts[0].Length - 1] != PseudoSpace.ZWNJ) && 
                            (ContainWord(wordParts[1]) && (wordParts[1][0] != PseudoSpace.ZWNJ))))
                        {
                            sugs.Add(word);
                        }
                    }
                }
            }

            return sugs.Distinct().ToArray();
        }
        private string[] ExtractRealWords(string[] words)
        {
            try
            {
                List<string> suggestion = new List<string>();
                foreach (string word in words)
                {
                    if (ContainWord(word) || word.Length == 0)
                    {
                        suggestion.Add(word);
                        //if (!suggestion.Contains(word))
                        //{
                        //    suggestion.Add(word);
                        //}
                    }
                }

                return suggestion.Distinct().ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private string[] CheckForSpaceInsertainDeletation(string word, string preWord, string postWord, out SpaceCorrectionState spaceCorrectionState)
        {
            spaceCorrectionState = SpaceCorrectionState.None;
            List<string> sugs = new List<string>();

            string tmpWord = word + postWord;
            if ((word != postWord) && (tmpWord != word))
            {
                sugs.AddRange(CheckForSpaceDeletation(tmpWord, out spaceCorrectionState));

                if (sugs.Count > 0)
                {
                    spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;
                }
            }

            if (sugs.Count == 0)
            {
                tmpWord = preWord + word;
                if ((word != postWord) && (tmpWord != word))
                {
                    sugs.AddRange(CheckForSpaceDeletation(tmpWord, out spaceCorrectionState));

                    if (sugs.Count > 0)
                    {
                        spaceCorrectionState = SpaceCorrectionState.SpaceInsertationRight;
                    }
                }
            }

            return sugs.ToArray();
        }
        
        private string[] CheckForSpaceDeletationSerially(string word, int input)
        {
            string incompleteWord;
            string[] midRes;
            string[] midApp;
            List<string> lst = new List<string>();
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (char c in word)
            {
                ++i;
                sb.Append(c);
                incompleteWord = sb.ToString();
                word = word.Remove(0, 1);

                if (i >= input)
                {
                    if (this.ContainWord(incompleteWord))
                    {
                        if (word == "")
                        {
                            return new string[] { incompleteWord };
                        }
                        else
                        {
                            midRes = CheckForSpaceDeletationSerially(word, 0);
                            if (midRes.Length > 0)
                            {
                                midApp = AppendList(incompleteWord, midRes);
                                lst.AddRange(midApp);
                                
                                midRes = CheckForSpaceDeletationSerially(incompleteWord + word, i + 1);
                                if (midRes.Length > 0)
                                {
                                    //midApp = AppendList(incompleteWord, midRes);
                                    //lst.AddRange(midApp);
                                    lst.AddRange(midRes);

                                }

                                return lst.ToArray();
                            }
                        }
                    }
                }
            }

            return new string[0];
        }

        private static string[] AppendList(string incompleteWord, string[] midRes)
        {
            List<string> lst = new List<string>();

            foreach (string str in midRes)
            {
                lst.Add(incompleteWord + " " + str);
            }

            return lst.ToArray();
        }
        
        private string[] CheckForSpaceDeletationSeriallyInsertation(string word, string preWord, string postWord, out SpaceCorrectionState spaceCorrectionState)
        {
           
            spaceCorrectionState = SpaceCorrectionState.None;
            List<string> sugs = new List<string>();

            string tmpWord = word + postWord;
            if ((word != postWord) && (tmpWord != word))
            {
                sugs.AddRange(CheckForSpaceDeletationSerially(tmpWord, 0));

                if (sugs.Count > 0)
                {
                    spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeftSerrially;
                }
            }

            if (sugs.Count == 0)
            {
                tmpWord = preWord + word;
                if ((word != postWord) && (tmpWord != word))
                {
                    sugs.AddRange(CheckForSpaceDeletationSerially(tmpWord, 0));

                    if (sugs.Count > 0)
                    {
                        spaceCorrectionState = SpaceCorrectionState.SpaceInsertationRightSerrially;
                    }
                }
            }

            return sugs.ToArray();
        }

        private string[] SortSpacingAndSpellingSugs(string[] finalSpacingSeg, string[] finalSpellingSeg, out bool haveSpacingSug)
        {
            haveSpacingSug = false;
            
            List<string> tmpSugs = new List<string>(finalSpellingSeg);

            double avgFreq;
            foreach (string strSpace in finalSpacingSeg)
            {
                bool inserted = false;

                if (!strSpace.Contains(" "))
                {
                    tmpSugs.Insert(0, strSpace);
                    haveSpacingSug = true;
                    inserted = true;
                    continue;
                }
             
                avgFreq = CalcAvgFreq(strSpace.Split(' '));

                foreach (string strSpell in finalSpellingSeg)
                {
                    if (WordFrequency(strSpell) < avgFreq)
                    {
                        tmpSugs.Insert(tmpSugs.IndexOf(strSpell), strSpace);
                        haveSpacingSug = true;
                        inserted = true;
                        break;
                    }
                }

                //if (finalSpellingSeg.Length == 0)
                if (!inserted)
                {
                    tmpSugs.Add(strSpace);
                    haveSpacingSug = true;
                }
            }
            return tmpSugs.ToArray();
        }

        #endregion

        #region Affix-Stripping

        /// <summary>
        /// If the word contain the persian affix, this method check or try to correct its spelling without affix 
        /// </summary>
        /// <param name="word">Input word</param>
        /// <param name="suggestionCount">Number of returned suggestions</param>
        /// <returns>
        /// if word don't have affix return an array with one string with lenght 0
        /// if word without its affix being correct, return word
        /// if word without its affix being incorrect, correct the word without affix and return the corrected word with corresponding affix
        /// </returns>
        private string[] CheckSpellingWithAffixConsideration(string word, int suggestionCount)
        {
            List<string> suggestion = new List<string>();
            string tmpSug, stem, suffix;

            ReversePatternMatcherPatternInfo[] rpmpiSet = StripAffixs(word);
            if (rpmpiSet.Length == 0)
            {
                return new string[0];
            }

            for (int i = 0; i < Math.Min(2, rpmpiSet.Length); ++i)
            {
                suffix = rpmpiSet[i].Suffix;
                stem = rpmpiSet[i].BaseWord;

                foreach (string str in base.SpellingSuggestions(word))
                {
                    PersianPOSTag posTag = base.WordPOS(stem);
                    suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(str, suffix,
                                                                               InflectionAnalyser.SuffixCategory(suffix), posTag);

                    tmpSug = CorrectSuffixSpacing(str, suffix, posTag);

                    if (!suggestion.Contains(tmpSug))
                    {
                        if (IsRealWordConsideringAffixes(tmpSug))
                        {
                            suggestion.Add(tmpSug);
                        }
                    }
                }

            }

            return SortSuggestions(word, suggestion.ToArray(), suggestionCount);
        }
        private string[] CheckSpellingWithAffixConsideration(string word, int suggestionCount, out bool phoneticSuffixProblem)
        {
            phoneticSuffixProblem = false;
                
            List<string> suggestion = new List<string>();
            
            string tmpSug, stem, suffix;

            //Dictionary<string, PersianPOSTag> posDic;
            //Dictionary<string, int> freqDic;

            PersianPOSTag posTag;
            ReversePatternMatcherPatternInfo[] rpmpiSet = StripAffixs(word);
            if (rpmpiSet.Length == 0)
            {
                return new string[0];
            }
            
            #region If JUST Phonetic Correction is required

            if (!base.IsRealWord(word))
            {
                foreach (ReversePatternMatcherPatternInfo suffixPatternInfo in rpmpiSet)
                {
                    suffix = suffixPatternInfo.Suffix;
                    stem = suffixPatternInfo.BaseWord;


                    posTag = base.WordPOS(stem);
                    suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(stem, suffix,
                                                                               InflectionAnalyser.SuffixCategory(suffix), posTag);

                    tmpSug = CorrectSuffixSpacing(stem, suffix, posTag);

                    if (tmpSug != word && tmpSug != stem)
                    {
                        if (base.IsRealWord(stem) && IsRealWordConsideringAffixes(tmpSug))
                        {
                            suggestion.Add(tmpSug);

                            //add ranking detail
                            if (!base.m_rankingDetail.ContainsKey(tmpSug))
                            {
                                base.m_rankingDetail.Add(tmpSug, SuffixSuggestionMessage);
                            }
                        }
                    }
                }

                if (suggestion.Count > 0)
                {
                    phoneticSuffixProblem = true;
                    return suggestion.Distinct().ToArray();
                }
            }

            #endregion

            //for (int i = 0; i < Math.Min(2, rpmpiSet.Length); ++i)
            for (int i = 0; i < rpmpiSet.Length; ++i)
            {
                suffix = rpmpiSet[i].Suffix;
                stem = rpmpiSet[i].BaseWord;

                //string[] spellingSugs = AllSpellingSuggestions(stem, out freqDic, out posDic);
                //string[] spellingSugs = SortSuggestions(stem, AllSpellingSuggestions(stem, out freqDic, out posDic), suggestionCount);

                string[] spellingSugs = SortSuggestions(stem, ReSpellingSuggestions(stem, 1), suggestionCount);
                //string[] spellingSugs = SortSuggestions(stem, base.SpellingSuggestion(stem), suggestionCount);

                foreach (string spellSug in spellingSugs)
                {
                    //suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(spellSug, suffix,
                    //                                                           InflectionAnalyser.SuffixCategory(suffix));

                    posTag = base.WordPOS(spellSug);
                    if (IsValidSuffixDeclension(spellSug, suffix, posTag))
                    {
                        tmpSug = CorrectSuffixSpacing(spellSug, suffix, posTag);
                        suggestion.Add(tmpSug);

                        //if (freqDic[spellSug] > 0)
                        //{
                        //    freqAffixDic.Add(tmpSug, freqDic[spellSug] / 3);
                        //}
                        //else
                        //{
                        //    freqAffixDic.Add(tmpSug, 1);
                        //}
                    }
                }
            }



            //return SortSuggestions(word, suggestion.ToArray(), freqAffixDic, suggestionCount);
            return suggestion.Distinct().ToArray();
        }
        private void CheckSpellingWithAffixConsiderationThreadSafe(Object data)
        {
            try
            {
                Thread.BeginThreadAffinity();

                AffixCorrectionData inputData = (AffixCorrectionData)data;
                string word = inputData.m_word;
            
                int suggestionCount = inputData.m_suggestionCount;

                List<string> suggestion = new List<string>();

                string tmpSug, stem, suffix;

                //Dictionary<string, PersianPOSTag> posDic;
                //Dictionary<string, int> freqDic;

                PersianPOSTag posTag;
                ReversePatternMatcherPatternInfo[] rpmpiSet = StripAffixs(word);
                if (rpmpiSet.Length == 0)
                {
                    inputData.m_suggestions = new string[0];
                    inputData.m_havePhoneticSuffixProblem = false;
                    return;
                }

                #region If JUST Phonetic Correction is required

                if (!base.IsRealWord(word))
                {
                    foreach (ReversePatternMatcherPatternInfo suffixPatternInfo in rpmpiSet)
                    {
                        suffix = suffixPatternInfo.Suffix;
                        stem = suffixPatternInfo.BaseWord;


                        posTag = base.WordPOS(stem);
                        suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(stem, suffix,
                                                                                   InflectionAnalyser.SuffixCategory(suffix), posTag);

                        tmpSug = CorrectSuffixSpacing(stem, suffix, posTag);

                        if (tmpSug != word && tmpSug != stem)
                        {
                            if (base.IsRealWord(stem) && IsRealWordConsideringAffixes(tmpSug))
                            {
                                suggestion.Add(tmpSug);

                                //add ranking detail
                                if (!base.m_rankingDetail.ContainsKey(tmpSug))
                                {
                                    base.m_rankingDetail.Add(tmpSug, SuffixSuggestionMessage);
                                }
                            }
                        }
                    }

                    if (suggestion.Count > 0)
                    {
                        inputData.m_suggestions = suggestion.Distinct().ToArray();
                        inputData.m_havePhoneticSuffixProblem = true;
                        return;
                    }
                }

                #endregion

                //for (int i = 0; i < Math.Min(2, rpmpiSet.Length); ++i)
                for (int i = 0; i < rpmpiSet.Length; ++i)
                {
                    suffix = rpmpiSet[i].Suffix;
                    stem = rpmpiSet[i].BaseWord;

                    //string[] spellingSugs = AllSpellingSuggestions(stem, out freqDic, out posDic);
                    //string[] spellingSugs = SortSuggestions(stem, AllSpellingSuggestions(stem, out freqDic, out posDic), suggestionCount);

                    //string[] spellingSugs = SortSuggestions(stem, ReSpellingSuggestions(stem, 1), suggestionCount);
                    //string[] spellingSugs = SortSuggestions(stem, base.SpellingSuggestion(stem), suggestionCount);
                    string[] spellingSugs = SortSuggestions(stem, base.SpellingSuggestions2(stem), suggestionCount);

                    foreach (string spellSug in spellingSugs)
                    {
                        //suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(spellSug, suffix,
                        //                                                           InflectionAnalyser.SuffixCategory(suffix));

                        posTag = base.WordPOS(spellSug);
                        if (IsValidSuffixDeclension(spellSug, suffix, posTag))
                        {
                            tmpSug = CorrectSuffixSpacing(spellSug, suffix, posTag);
                            suggestion.Add(tmpSug);

                            //if (freqDic[spellSug] > 0)
                            //{
                            //    freqAffixDic.Add(tmpSug, freqDic[spellSug] / 3);
                            //}
                            //else
                            //{
                            //    freqAffixDic.Add(tmpSug, 1);
                            //}
                        }
                    }
                }


                //return SortSuggestions(word, suggestion.ToArray(), freqAffixDic, suggestionCount);
                inputData.m_suggestions = suggestion.Distinct().ToArray();
                inputData.m_havePhoneticSuffixProblem = false;

                Thread.EndThreadAffinity();

                return;
            }
            catch (Exception)
            {
                ((AffixCorrectionData)data).m_suggestions = new string[0];
                ((AffixCorrectionData)data).m_havePhoneticSuffixProblem = false;
                
                Thread.EndThreadAffinity();
                return;
            }
        }

        private ReversePatternMatcherPatternInfo[] StripAffixs(string word)
        {
            return this.m_persianSuffixRecognizer.MatchForSuffix(word);
        }

        private bool IsRealWordConsideringAffixes(string word)
        {
            ReversePatternMatcherPatternInfo[] affixeResults = this.StripAffixs(word);
            if (affixeResults.Length == 0)
            {
                return false;
            }

            PersianPOSTag posTag;
            int freq;
            string stem, suffix;

            foreach (ReversePatternMatcherPatternInfo rpmpi in affixeResults)
            {
                stem = rpmpi.BaseWord;
                suffix = rpmpi.Suffix;

                if (base.IsRealWord(stem, out freq, out posTag))
                {
                    if (word == CorrectSuffixSpacing(stem, suffix, posTag)) //Spacing
                    {
                        if (IsValidSuffixDeclension(stem, suffix, posTag))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private bool IsRealWordConsideringAffixes(string word, out string wordWithoutAffix, out string affix)
        {
            wordWithoutAffix = "";
            affix = "";

            ReversePatternMatcherPatternInfo[] affixeResults = this.StripAffixs(word);
            if (affixeResults.Length == 0)
            {
                return false;
            }

            PersianPOSTag posTag;
            int freq;
            //int threshold = (int)(0.05 * base.CalcAvgFreq());

            string stem, suffix;

            foreach (ReversePatternMatcherPatternInfo rpmpi in affixeResults)
            {
                stem = rpmpi.BaseWord;
                suffix = rpmpi.Suffix;

                if (base.IsRealWord(stem, out freq, out posTag))
                {
                    if (word == CorrectSuffixSpacing(stem, suffix, posTag)) //Spacing
                    {
                        if (IsValidSuffixDeclension(stem, suffix, posTag) || this.m_persianSuffixRecognizer.UseColloquals)
                        {
                            wordWithoutAffix = rpmpi.BaseWord;
                            affix = rpmpi.Suffix;

                            //if (freq > threshold)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckAffixSpacing(string word, string preWord, string nxtWord, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState, out bool needsSpellChecking)
        {
            suggestionType = SuggestionType.Green;
            spaceCorrectionState = SpaceCorrectionState.None;
            needsSpellChecking = false;

            List<string> spacingSug = new List<string>();
            bool exitFlag = false;
            string[] tmpSugArray;

            #region Prefix Spacing

            bool isInitialBeCorrected;
            if (!CheckPrefixSpacing(word, nxtWord, out tmpSugArray, out suggestionType, out spaceCorrectionState, out isInitialBeCorrected))
            {
                if (isInitialBeCorrected)
                {
                    needsSpellChecking = true;
                }

                spacingSug.AddRange(tmpSugArray);
                exitFlag = true;

                #region Add ranking detail

                foreach (string str in spacingSug)
                {
                    if (!base.m_rankingDetail.ContainsKey(str))
                    {
                        base.m_rankingDetail.Add(str, PrefixSuggestionMessage);
                    }
                }

                #endregion
            }
            #endregion

            if (!exitFlag)
            {
                #region Suffix Spacing
                
                bool isPhoneticallyChanged;
                
                if (!CheckSuffixSpacing(word, nxtWord, out tmpSugArray, out suggestionType, out spaceCorrectionState, out isPhoneticallyChanged))
                {
                    if (isPhoneticallyChanged)
                    {
                        needsSpellChecking = true;
                    }

                    spacingSug.AddRange(tmpSugArray);
                    exitFlag = true;

                    #region Add ranking detail

                    foreach (string str in spacingSug)
                    {
                        if (!base.m_rankingDetail.ContainsKey(str))
                        {
                            base.m_rankingDetail.Add(str, SuffixSuggestionMessage);
                        }
                    }

                    #endregion

                }

                #endregion
            }

            if (exitFlag)
            {
                suggestions = spacingSug.ToArray();
                return false;
            }

            suggestions = new string[0];
            return true;
        }

        private bool CheckPrefixSpacing(string word, string nxtWord, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState, out bool isInitialBeCorrected)
        {
            suggestionType = SuggestionType.Red;
            spaceCorrectionState = SpaceCorrectionState.None;
            isInitialBeCorrected = false;

            string sug;
            List<string> tmpSugList = new List<string>(0);

            //check Vocabulary Words Space Correction Rule
            if (ContainPrefix(word) || this.SpellingRuleExist(SpellingRules.VocabularyWordsSpaceCorrection))
            {
                //seperate written prefix
                if (IsStickerPrefix(word))
                {
                    if (IsValidPrefix(word, nxtWord))
                    {
                        sug = CheckForSpaceInsertation(word, word, nxtWord, out spaceCorrectionState);
                        if (sug != "" && sug != word)
                        {
                            suggestionType = SuggestionType.Red;

                            tmpSugList.Add(sug);
                        }
                    }
                }
                else
                {
                    if (!this.ContainWord(word))
                    {
                        string wordWithoutPrefix;
                        string prefix = ExtractPrefix(word, out wordWithoutPrefix);
                        if (wordWithoutPrefix != "")
                        {
                            if (prefix == "به" && this.ContainWord(wordWithoutPrefix))
                            {
                                sug = prefix + " " + wordWithoutPrefix;

                                spaceCorrectionState = SpaceCorrectionState.SpaceDeletation;
                                suggestionType = SuggestionType.Red;
                                isInitialBeCorrected = true;

                                tmpSugList.Add(sug);
                            }

                            //wrong separate written prefix
                            if (ContainPrefix(word))
                            {
                                if (IsStickerPrefix(word))
                                {
                                    if (IsValidPrefix(word, nxtWord))
                                    {
                                        sug = CheckForSpaceInsertation(word, word, nxtWord, out spaceCorrectionState);
                                        if (sug != "" && sug != word)
                                        {
                                            suggestionType = SuggestionType.Red;

                                            tmpSugList.Add(sug);
                                        }
                                    }
                                }
                                else //wrong continuously written prefix
                                {
                                    if (!this.ContainWord(word))
                                    {
                                        prefix = ExtractPrefix(word, out wordWithoutPrefix);
                                        if (wordWithoutPrefix != "")
                                        {
                                            if ((prefix == "می" || prefix == "نمی") && this.ContainWord(wordWithoutPrefix))
                                            {
                                                sug = prefix + PseudoSpace.ZWNJ + wordWithoutPrefix;
                                                if (this.ContainWord(sug))
                                                {
                                                    spaceCorrectionState = SpaceCorrectionState.None;
                                                    suggestionType = SuggestionType.Red;

                                                    tmpSugList.Add(sug);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            suggestions = tmpSugList.ToArray().Distinct().ToArray();
            if (suggestions.Length > 0)
            {
                return false;
            }

            return true;
        }

        // Correct affixes in spacing suggestions
        private string[] AffixSpacingSeriallyCheck(string[] sugs)
        {
            SpaceCorrectionState scs = SpaceCorrectionState.None;
            SuggestionType st;
            bool isPhoneticallyChanged;

            if (sugs.Length == 0)
            {
                return new string [0];
            }

            string[] affixSpacingSugs;

            List<string> tmpFinalSuggestions = new List<string>();

            List<string> tmpSuggestions = new List<string>();
            List<string> tmpPrePartSugs = new List<string>();
            foreach (string str in sugs)
            {
                string[] parts = str.Split(' ');
                for (int i = 0; i < parts.Length; ++i)
                {
                    affixSpacingSugs = new string[0];

                    if (i < parts.Length - 1)
                    {
                        CheckAffixSpacing(parts[i], "", parts[i + 1], out affixSpacingSugs, out st, out scs, out isPhoneticallyChanged);
                    }
                    if (affixSpacingSugs.Length > 0)
                    {
                        if (i == 0)
                        {
                            if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationRight)
                            {
                                i++;
                            }

                            tmpPrePartSugs.AddRange(affixSpacingSugs);
                            continue;
                        }

                        foreach (string affixCorectedSug in affixSpacingSugs)
                        {
                            foreach (string prePart in tmpPrePartSugs)
                            {
                                tmpSuggestions.Add(prePart + " " + affixCorectedSug);
                            }
                        }

                        if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationRight)
                        {
                            i++;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            tmpPrePartSugs.Add(parts[i]);

                            if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationRight)
                            {
                                i++;
                            }

                            continue;
                        }

                        foreach (string prePart in tmpPrePartSugs)
                        {
                            tmpSuggestions.Add(prePart + " " + parts[i]);
                        }

                        if (scs == SpaceCorrectionState.SpaceInsertationLeft || scs == SpaceCorrectionState.SpaceInsertationRight)
                        {
                            i++;
                        }
                    }

                    tmpPrePartSugs.Clear();
                    tmpPrePartSugs.AddRange(tmpSuggestions);
                    tmpSuggestions.Clear();
                }
                
                tmpFinalSuggestions.AddRange(tmpPrePartSugs);
                tmpPrePartSugs.Clear();
            }

            return tmpFinalSuggestions.Distinct().ToArray();
        }

        private string RefineforEzafe(string word, out EzafeState ezafaState)
        {
            ezafaState = EzafeState.None;

            if (word.EndsWith("ۀ"))
            {
                word = word.Remove(word.Length - 1, 1);
                word = word + "ه‌ی";

                ezafaState = EzafeState.UrduYaOverHeh;
            }
            else if (word[word.Length - 1] == 'ٔ' && word[word.Length - 2] == 'ه')
            {
                word = word.Remove(word.Length - 1, 1);
                word = word + "‌ی";

                ezafaState = EzafeState.PersianHamzaOverHeh;
            }
            else if (word.EndsWith("ه‌ی", StringComparison.Ordinal))
            {
                ezafaState = EzafeState.YaWithPseudoSpace;
            }

            return word;
        }

        private static string[] RevertEzafe(string[] suggestions, EzafeState ezafeState)
        {
            if (ezafeState == EzafeState.None || ezafeState == EzafeState.YaWithPseudoSpace)
            {
                return suggestions;
            }

            List<string> MyList = new List<string>();
            string tmpStr;
            foreach (string lstStr in suggestions)
            {
                tmpStr = lstStr;
                if (tmpStr.EndsWith("ه‌ی", StringComparison.Ordinal))
                {
                    if (ezafeState == EzafeState.UrduYaOverHeh)
                    {
                        tmpStr = tmpStr.Remove(tmpStr.Length - 3, 3);
                        tmpStr = tmpStr + "ۀ";
                    }
                    else if (ezafeState == EzafeState.PersianHamzaOverHeh)
                    {
                        tmpStr = tmpStr.Remove(tmpStr.Length - 3, 3);
                        tmpStr = tmpStr + "هٔ";
                    }
                }

                MyList.Add(tmpStr);
            }

            return MyList.ToArray();
        }
        
        #region Suffix

        private bool CheckSuffixSpacing(string word, string nxtWord, out string[] suggestions, out SuggestionType suggestionType, out SpaceCorrectionState spaceCorrectionState, out bool isPhoneticallyChanged)
        {
            suggestionType = SuggestionType.Red;
            spaceCorrectionState = SpaceCorrectionState.None;
            isPhoneticallyChanged = false;

            string tmpSug, stem, suffix;
            List<string> tmpSugList = new List<string>();

            #region Wrong Continious Spacing

            //if (!base.IsRealWord(word))
            if (!ContainWord(word))
            {
                ReversePatternMatcherPatternInfo[] suffixPatternInfoArray =
                    m_persianSuffixRecognizer.MatchForSuffix(word);
                if (suffixPatternInfoArray.Length > 0)
                {
                    foreach (ReversePatternMatcherPatternInfo suffixPatternInfo in suffixPatternInfoArray)
                    {
                        suffix = suffixPatternInfo.Suffix;
                        stem = suffixPatternInfo.BaseWord;

                        #region Exceptions

                        if (IsSuffixException(suffix) && base.IsRealWord(stem) && !IsRealWordConsideringAffixes(word))
                        {
                            if (tmpSugList.Count == 0)
                            {
                                suggestions = new string[0];
                                return true;
                            }
                        }

                        #endregion

                        PersianPOSTag posTag = base.WordPOS(stem);

                        string tmpSuffix = suffix;
                        suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(stem, suffix,
                                                           InflectionAnalyser.SuffixCategory(suffix), posTag);

                        if (suffix != tmpSuffix)
                        {
                            isPhoneticallyChanged = true;
                        }

                        tmpSug = CorrectSuffixSpacing(stem, suffix, posTag);

                        if (tmpSug != word && tmpSug != stem)
                        {
                            if (base.IsRealWord(stem) && IsRealWordConsideringAffixes(tmpSug))
                            {
                                tmpSugList.Add(tmpSug);
                            }
                        }
                        else if (tmpSug == word && tmpSug != stem)
                        {
                            if (base.IsRealWord(stem) && IsRealWordConsideringAffixes(tmpSug))
                            {
                                tmpSugList.Clear();
                            }
                        }
                    }

                    if (tmpSugList.Count > 0)
                    {
                        suggestions = tmpSugList.ToArray().Distinct().ToArray();
                        return false;
                    }
                }
            }

            #endregion

            #region Wrong White Space Insertation

            suffix = nxtWord;
            stem = word;

            if (InflectionAnalyser.SuffixCategory(suffix) != 0)
            {
                /*
                 * Here a real word mistakenly considered as suffix and must be ignored
                 * Some suffixes may also be real word like امید and مانند
                 * I want to make sure these suffixes do not process like other suffixes to correct spacing
                 * also some others like ای and ها is real word too, but the frequency of use them as suffix is far more than a real word
                 * so I add check length part
                 */
                if (base.IsRealWord(suffix))
                {
                    if (suffix.Length > 3)
                    {
                        suggestions = new string[0];
                        return true;
                    }

                    tmpSug = CorrectSuffixSpacing(stem, suffix, base.WordPOS(stem));
                    if (tmpSug != (stem + PseudoSpace.ZWNJ + suffix) ||
                        ((tmpSug == (stem + PseudoSpace.ZWNJ + suffix)) && stem.EndsWith("ه")))
                    {
                        suggestions = new string[0];
                        return true;
                    }
                }

                //suffix is not a real word
                PersianPOSTag posTag = base.WordPOS(stem);
                tmpSug = CorrectSuffixSpacing(stem, suffix, posTag);

                if (tmpSug != word)
                {
                    if (IsRealWordConsideringAffixes(tmpSug))
                    {
                        spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;
                        suggestions = new string[] { tmpSug };
                        return false;
                    }
                }
                
                string tmpSuffix = suffix;
                suffix = InflectionAnalyser.EqualSuffixWithCorrectPhonetic(stem, suffix,
                                                                           InflectionAnalyser.SuffixCategory(suffix), posTag);

                if (suffix != tmpSuffix)
                {
                    isPhoneticallyChanged = true;
                }

                tmpSug = CorrectSuffixSpacing(stem, suffix, posTag);

                if (tmpSug != word)
                {
                    if (IsRealWordConsideringAffixes(tmpSug))
                    {
                        spaceCorrectionState = SpaceCorrectionState.SpaceInsertationLeft;
                        suggestions = new string[] { tmpSug };
                        return false;
                    }
                }
            }

            #endregion

            suggestions = new string[0];
            return true;
        }

        private bool IsValidSuffixDeclension(string stem, string suffix, PersianPOSTag posTag)
        {
            PersianSuffixesCategory suffixCategory = InflectionAnalyser.SuffixCategory(suffix);
            if (InflectionAnalyser.IsValidDeclension(posTag, suffixCategory))
            {
                if (InflectionAnalyser.IsValidPhoneticComposition(stem, suffix, posTag))
                {
                    //PersianSuffixesCategory suffixCategory = InflectionAnalyser.SuffixCategory(suffix);
                    return true;
                }
            }

            return false;
        }

        private static bool IsSuffixException(string suffix)
        {
            if (suffix.IsIn(RuleBasedSpelling.PluralAT))
            {
                return true;
            }

            return false;
        }
        private string CorrectSuffixSpacing(string stem, string suffix, PersianPOSTag pos)
        {
            string equalSuffixWithSpacingSymbols = stem + InflectionAnalyser.EqualSuffixWithSpacingSymbols(suffix);

            for (int i = 0; i < equalSuffixWithSpacingSymbols.Length; ++i)
            {
                if (IsStartWithSpaceOrPseudoSpacePlusSymbol(equalSuffixWithSpacingSymbols[i]))
                {
                    equalSuffixWithSpacingSymbols.Remove(i, 1).Insert(i, PseudoSpace.ZWNJ.ToString());
                }
                else if (IsStartWithSpaceOrPseudoSpaceStarSymbol(equalSuffixWithSpacingSymbols[i]))
                {
                    string tmpStem = equalSuffixWithSpacingSymbols.Substring(0, i);
                    string tmpSuffix = equalSuffixWithSpacingSymbols.Substring(i, equalSuffixWithSpacingSymbols.Length - i);
                    tmpSuffix = PurifySymbols(tmpSuffix);
                    PersianCombinationSpacingState spacingState = InflectionAnalyser.CalculateSpacingState(tmpStem, tmpSuffix, pos);

                    if (spacingState == PersianCombinationSpacingState.PseudoSpace)
                    {
                        equalSuffixWithSpacingSymbols = equalSuffixWithSpacingSymbols.Remove(i, 1).Insert(i, PseudoSpace.ZWNJ.ToString());
                    }
                    else if (spacingState == PersianCombinationSpacingState.Continous)
                    {
                        equalSuffixWithSpacingSymbols = equalSuffixWithSpacingSymbols.Remove(i, 1);
                    }
                    else
                    {
                        equalSuffixWithSpacingSymbols = equalSuffixWithSpacingSymbols.Remove(i, 1);
                    }

                    if (tmpStem.EndsWith("ه") && tmpSuffix.StartsWith(PersianSuffixes.PluralSignAanPermutedForHaa))
                    {
                        equalSuffixWithSpacingSymbols = equalSuffixWithSpacingSymbols.Remove(i - 1, 2);
                        
                        //if (equalSuffixWithSpacingSymbols[i - 1] == PseudoSpace.ZWNJ &&
                        //    PersianAlphabets.NonStickerChars.Contains(equalSuffixWithSpacingSymbols[i - 2]))
                        //{
                        //    equalSuffixWithSpacingSymbols = equalSuffixWithSpacingSymbols.Remove(i - 1, 1);
                        //}
                    }
                }
            }

            return equalSuffixWithSpacingSymbols;
        }

        private static bool IsStartWithSpaceOrPseudoSpaceStarSymbol(char c)
        {
            if (c == ReversePatternMatcher.SymbolHalfSpaceQuestionMark ||
                c == ReversePatternMatcher.SymbolSpaceOrHalfSpaceStar ||
                c == ReversePatternMatcher.SymbolSpaceStar)
            {
                return true;
            }

            return false;
        }
        private static bool IsStartWithSpaceOrPseudoSpacePlusSymbol(char c)
        {
            if (c == ReversePatternMatcher.SymbolHalfSpace ||
                c == ReversePatternMatcher.SymbolSpaceOrHalfSpace ||
                c == ReversePatternMatcher.SymbolSpaceOrHalfSpacePlus ||
                c == ReversePatternMatcher.SymbolSpacePlus)
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

        #endregion

        #region Prefix
        
        private static bool IsStickerPrefix(string word)
        {
            if (word == "می" || word == "نمی")
            {
                return true;
            }

            return false;
        }
        private static bool IsPrefix(string word)
        {
            if (word == "می" || word == "نمی" || word == "به")
            {
                return true;
            }

            return false;
        }
        private bool IsValidPrefix(string prefix, string word)
        {
            if (IsPrefix(prefix) && this.ContainWord(word))
            {
                return true;
            }

            return false;
        }
        private static string ExtractPrefix(string word, out string wordWithoutPrefix)
        {
            wordWithoutPrefix = word;

            string prefix = "";

            if (word.StartsWith("می"))
            {
                prefix = "می";
                wordWithoutPrefix = word.Remove(0, 2);
            }
            else if (word.StartsWith("نمی"))
            {
                prefix = "نمی";
                wordWithoutPrefix = word.Remove(0, 3);
            }
            else if (word.StartsWith("ب"))
            {
                prefix = "به";
                wordWithoutPrefix = word.Remove(0, 1);
            }

            return prefix;
        }
        private static bool ContainPrefix(string word)
        {
            if (word.StartsWith("می") || word.StartsWith("نمی") || word.StartsWith("ب"))
            {
                return true;
            }

            return false;
        }
        
        #endregion

        #endregion

        #region Spelling Rules
        
        private bool SpellingRuleExist(SpellingRules rule)
        {
            if (rule == SpellingRules.VocabularyWordsSpaceCorrection)
            {
                return this.m_ruleVocabularyWordsSpaceingCorrection;
            }
            if (rule == SpellingRules.CheckForCompletetion)
            {
                return this.m_ruleCheckForCompletetion;
            }
            if (rule == SpellingRules.IgnoreHehYa)
            {
                return this.m_ruleIgnoreHehYa;
            }
            if (rule == SpellingRules.IgnoreLetters)
            {
                return this.m_ruleIgnoreLetters;
            }
            if (rule == SpellingRules.IgnorePseudoSpaceCompoundWords)
            {
                return this.m_ruleIgnorePseudoSpaceCompoundWords;
            }

            return false;
        }
        
        private bool OnePassConvertingRuleExist(OnePassCorrectionRules rule)
        {
            if (rule == OnePassCorrectionRules.CorrectBe)
            {
                return this.m_ruleOnePassCorrectBe;
            }
            if (rule == OnePassCorrectionRules.CorrectPrefix)
            {
                return this.m_ruleOnePassCorrectPrefixes;
            }
            if (rule == OnePassCorrectionRules.CorrectSuffix)
            {
                return this.m_ruleOnePassCorrectSuffixes;
            }

            return false;
        }

        private void SetDefaultSpellingRules()
        {
            this.m_ruleVocabularyWordsSpaceingCorrection = false;
            this.m_ruleCheckForCompletetion           = true;
            this.m_ruleIgnoreHehYa = true;
            this.m_ruleIgnoreLetters = true;
            this.m_ruleIgnorePseudoSpaceCompoundWords = false;
        }

        private void SetDefaultOnePassSpacingRules()
        {
            SetOnePassCorrectionRules(OnePassCorrectionRules.CorrectBe);
            SetOnePassCorrectionRules(OnePassCorrectionRules.CorrectPrefix);
            SetOnePassCorrectionRules(OnePassCorrectionRules.CorrectSuffix);
        }
        #endregion
        
        #endregion
    }
}

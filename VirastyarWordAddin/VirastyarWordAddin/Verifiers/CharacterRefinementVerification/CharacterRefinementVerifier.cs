using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Persian;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.GUI;
using VirastyarWordAddin.Configurations;
using VirastyarWordAddin.Verifiers.Basics;
using System;
using VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox;
using SCICT.NLP;
using SCICT.NLP.Persian.Constants;

namespace VirastyarWordAddin.Verifiers.CharacterRefinementVerification
{
    public class CharacterRefinementVerifier : NGramVerifierBase
    {
        private enum RefinementTypes
        {
            RefineLetter,
            RefineDigit,
            RefineHalfSpace,
            RefineErab,
            RefineHalfSpacePosition,
            RefineHeYe
        }

        private readonly Dictionary<RefinementTypes, long> m_dicRefinementStats = new Dictionary<RefinementTypes, long>();
        private readonly Dictionary<RefinementTypes, long> m_dicCurStepStats = new Dictionary<RefinementTypes, long>();

        private const string LabelRefineLetter = "اصلاح حروف";
        private const string LabelRefineDigit = "اصلاح ارقام";
        private const string LabelRefineHalfSpace = "اصلاح نویسهٔ نیم‌فاصله";
        private const string LabelRefineErab = "اصلاح اعراب";
        private const string LabelRefineHalfSpacePosition = "حذف و جابجایی نیم‌فاصله‌ها";
        private const string LabelConvertHeYe = "اصلاح «ه‌ی» یا «هٔ»";


        private readonly AllCharactersRefinerSettings m_settings = null;
        private readonly FilteringCharacterCategory m_filteringCategory;
        private readonly HashSet<char> m_ignoreList;

        public CharacterRefinementVerifier(AllCharactersRefinerSettings settings)
            : base(0, 1)
        {
            m_settings = settings;

            ResetStats();
            ResetCurStepStats();

            m_filteringCategory = settings.GetIgnoredCategories();
            m_ignoreList = settings.GetIgnoreList();
        }

        protected void ResetStats()
        {
            m_dicRefinementStats.Clear();
            foreach (var i in Enum.GetValues(typeof(RefinementTypes)))
            {
                m_dicRefinementStats.Add((RefinementTypes)(i), 0L);
            }
        }

        protected void ResetCurStepStats()
        {
            if (m_dicCurStepStats.Count == 0)
            {
                foreach (var i in Enum.GetValues(typeof(RefinementTypes)))
                {
                    m_dicCurStepStats.Add((RefinementTypes)(i), 0L);
                }
            }
            else
            {
                foreach (var i in Enum.GetValues(typeof(RefinementTypes)))
                {
                    m_dicCurStepStats[(RefinementTypes)(i)] = 0L;
                }
            }
        }

        public override string Title
        {
            get { return "اصلاح همهٔ نویسه‌های متن"; }
        }

        public override string HelpTopicFileName
        {
            get { return HelpConstants.RefineAllChars; }
        }

        public override UserSelectedActions ActionsToDisable
        {
            get
            {
                return UserSelectedActions.AddToDictionary | UserSelectedActions.ChangeAll |
                       UserSelectedActions.Ignore | UserSelectedActions.IgnoreAll;
            }
        }

        public override Type SuggestionViewerType
        {
            get { return typeof (TitledListBoxSuggestionViewer); }
        }

        public override bool ShowBatchModeStats()
        {
            PersianMessageBox.Show(ThisAddIn.GetWin32Window(), StatsticsMessage(), Constants.UIMessages.SuccessRefinementTitle);
            return true;
        }

        protected override bool NeedRefinedStrings
        {
            get { return false; }
        }

        protected override WordTokenizerOptions TokenizerOptions
        {
            get { return WordTokenizerOptions.None; }
        }

        private static readonly string s_charCode2 = Convert.ToChar(2).ToString();
        private static readonly string s_charCode1 = Convert.ToChar(1).ToString();
        protected override bool IsProperWord(string word)
        {
            // these words are used to refer to graphs, or footnotes
            if (word.Contains(s_charCode2) || word.Contains(s_charCode1))
                return false;
            return true;
        }

        public override ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verRes)
        {
            throw new NotImplementedException();
        }

        public override void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion)
        {
            //if (rangeToChange.TryChangeTextCharSensitive(suggestion))
            if (rangeToChange.TryChangeText(suggestion))
            {
                AddCurStatsToGlobalStats();
                base.RefreshForChangeCalled(m_curVerData.RangeToHighlight, suggestion);
            }

            ResetCurStepStats();
        }

        protected override StringVerificationData CheckNGramWordsList(TokenInfo[] readItems, int mainItemIndex)
        {
            //Debug.Assert(readItems.Length == 1);
            //Debug.Assert(mainItemIndex == 0);

            string word = readItems[mainItemIndex].Value;
            string nextWord = null;
            if (readItems.Length > 1 && mainItemIndex == 0)
                nextWord = readItems[1].Value;

            if (StringUtil.IsWhiteSpace(word))
                return null;

            string result = word;

            if (m_settings.RefineHalfSpacePositioning)
            {
                int numHsChanges;
                result = StringUtil.NormalizeSpacesAndHalfSpacesInWord(result, out numHsChanges);
                m_dicCurStepStats[RefinementTypes.RefineHalfSpacePosition] = numHsChanges;
            }

            var filterStats = StringUtil.FilterPersianWordWithStats(result, m_ignoreList, m_filteringCategory);

            if(result != filterStats.Result)
            {
                result = filterStats.Result;
                m_dicCurStepStats[RefinementTypes.RefineDigit] = filterStats.NumDigits;
                m_dicCurStepStats[RefinementTypes.RefineErab] = filterStats.NumErabs;
                m_dicCurStepStats[RefinementTypes.RefineHalfSpace] = filterStats.NumHalfSpaces;
                m_dicCurStepStats[RefinementTypes.RefineLetter] = filterStats.NumLetters;
            }

            bool isErrorSplitIn2Words = false;

            if(m_settings.NormalizeHeYe) // perform refine and convert all together
            {
                // if the long HeYe is split in two words
                if (nextWord != null && HeYe.IsTwoWordsFormingLongHeYe(result, nextWord))
                {
                    int lastWord0Index = StringUtil.LastWordCharIndex(result);
                    var sb = new StringBuilder(result);
                    sb.Remove(lastWord0Index, result.Length - lastWord0Index);

                    if(m_settings.ConvertLongHeYeToShort)
                        sb.Insert(lastWord0Index, HeYe.StandardShortHeYe);
                    else
                        sb.Insert(lastWord0Index, HeYe.StandardLongHeYe);

                    result = sb.ToString();
                    m_dicCurStepStats[RefinementTypes.RefineHeYe] = 1;
                    isErrorSplitIn2Words = true;
                }
                else // if long heye is contained in one word
                {
                    string resultConverted;
                    if (m_settings.ConvertLongHeYeToShort)
                    {
                        resultConverted = StringUtil.NormalizeShortHeYe(result);
                        resultConverted = StringUtil.ConvertLongHeYeToShort(resultConverted);
                    }
                    else if(m_settings.ConvertShortHeYeToLong)
                    {
                        resultConverted = StringUtil.NormalizeLongHeYe(result);
                        resultConverted = StringUtil.ConvertShortHeYeToLong(resultConverted);
                    }
                    else
                    {
                        resultConverted = StringUtil.NormalizeLongHeYe(result);
                        resultConverted = StringUtil.NormalizeShortHeYe(resultConverted);
                    }

                    if (resultConverted != result)
                    {
                        m_dicCurStepStats[RefinementTypes.RefineHeYe] = 1;
                        result = resultConverted;
                    }
                }
            }
            // now apply heye conversion rules
            else if(m_settings.ConvertLongHeYeToShort)
            {
                // if the long HeYe is split in two words
                if(nextWord != null && HeYe.IsTwoWordsFormingLongHeYe(result, nextWord))
                {
                    int lastWord0Index = StringUtil.LastWordCharIndex(result);
                    var sb = new StringBuilder(result);
                    sb.Remove(lastWord0Index, result.Length - lastWord0Index);
                    sb.Insert(lastWord0Index, HeYe.StandardShortHeYe);
                    result = sb.ToString();
                    m_dicCurStepStats[RefinementTypes.RefineHeYe] = 1;
                    isErrorSplitIn2Words = true;
                }
                else // if long heye is contained in one word
                {
                    string resultConverted = StringUtil.ConvertLongHeYeToShort(result);
                    if (resultConverted != result)
                    {
                        m_dicCurStepStats[RefinementTypes.RefineHeYe] = 1;
                        result = resultConverted;
                    }
                }
            }
            else if(m_settings.ConvertShortHeYeToLong)
            {
                string resultConverted = StringUtil.ConvertShortHeYeToLong(result);
                if(resultConverted != result)
                {
                    m_dicCurStepStats[RefinementTypes.RefineHeYe] = 1;
                    result = resultConverted;
                }
            }

            Debug.Assert(result != null);
            if (result == null)
            {
                throw new Exception("Unexpected condition met in character refiner!");
            }

            if (word == result)
                return null;

            int errorLength = readItems[mainItemIndex].Length;
            if(isErrorSplitIn2Words)
            {
                errorLength = readItems[mainItemIndex + 1].EndIndex - readItems[mainItemIndex].Index + 1;
            }

            return new StringVerificationData
                       {
                           ErrorIndex = readItems[mainItemIndex].Index,
                           ErrorLength = errorLength,
                           ErrorType = VerificationTypes.Warning,
                           Suggestions = new TitledListBoxSuggestion
                                             {
                                                 Message = "پیشنهادها",
                                                 SuggestionItems = new[] {result}
                                             }
                       };
        }

        
            
        // ---------------------

        private void AddCurStatsToGlobalStats()
        {
            foreach (var i in Enum.GetValues(typeof(RefinementTypes)))
            {
                m_dicRefinementStats[(RefinementTypes)(i)] += m_dicCurStepStats[(RefinementTypes)(i)];
            }
        }

        private string StatsticsMessage()
        {
            var sb = new StringBuilder();

            long sum = 0L;
            foreach (var pair in m_dicRefinementStats)
            {
                sum += pair.Value;
                sb.AppendLine(String.Format("{0}: {1}", GetRefinementTypeLabel(pair.Key), ParsingUtils.ConvertNumber2Persian(pair.Value.ToString())));
            }

            return String.Format("{0}: {1}{2}{2}", "تعداد کل نویسه‌های اصلاح شده", ParsingUtils.ConvertNumber2Persian(sum.ToString()), Environment.NewLine) + sb;
        }

        private static string GetRefinementTypeLabel(RefinementTypes refinementType)
        {
            switch (refinementType)
            {
                case RefinementTypes.RefineLetter:
                    return LabelRefineLetter;
                case RefinementTypes.RefineDigit:
                    return LabelRefineDigit;
                case RefinementTypes.RefineHalfSpace:
                    return LabelRefineHalfSpace;
                case RefinementTypes.RefineErab:
                    return LabelRefineErab;
                case RefinementTypes.RefineHalfSpacePosition:
                    return LabelRefineHalfSpacePosition;
                case RefinementTypes.RefineHeYe:
                    return LabelConvertHeYe;
                default:
                    return "";
            }
        }

    }
}

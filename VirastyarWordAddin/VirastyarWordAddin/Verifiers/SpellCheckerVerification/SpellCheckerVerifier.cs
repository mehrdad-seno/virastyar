using System;
using System.Text;
using System.Windows.Forms;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.GUI;
using SCICT.Utility.SpellChecker;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.Verifiers.Basics;
using SCICT.Microsoft.Office.Word.ContentReader;
using VirastyarWordAddin.Verifiers.CustomGUIs.SpellCheckerSuggestions;
using SCICT.NLP;

namespace VirastyarWordAddin.Verifiers.SpellCheckerVerification
{
    public class SpellCheckerVerifier : NGramVerifierBase
    {
        private readonly PersianSpellChecker m_engine;
        private readonly SessionLogger m_sessionLogger;

        private SpaceCorrectionState m_lastSCS;
        private string[] m_lastSugs;
        private string m_curWordCombToCheck;
        private string m_curWord0, m_curWord1, m_curWord2;

        private readonly bool m_isPrespellChecker = false;
        private int m_bachModeStats = 0;

        public SpellCheckerVerifier(bool isPrespellChecker, PersianSpellChecker engine, SessionLogger sessionLogger) 
            : base(1, 1)
        {
            m_engine = engine;
            m_sessionLogger = sessionLogger;
            m_isPrespellChecker = isPrespellChecker;
        }

        public SpellCheckerVerifier(bool isPrespellChecker, PersianSpellChecker engine)
            : this(isPrespellChecker, engine, new SessionLogger())            
        {

        }

        protected override bool IsProperWord(string word)
        {
            return !String.IsNullOrEmpty(word) && StringUtil.IsArabicWord(word.Trim()) && 
                !StringUtil.IsWhiteSpace(word) && !StringUtil.IsHalfSpace(word);
        }

        public override ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verResult)
        {
            UserSelectedActions userAction = verResult.UserAction;
            string selectedSug = verResult.SelectedSuggestion;

            var proceedType = ProceedTypes.IdleProceed;

            if (userAction == UserSelectedActions.Change)
            {
                if (m_curVerData.RangeToHighlight.Text != selectedSug)
                {
                    m_sessionLogger.AddUsage(selectedSug);
                    if (!m_curVerData.RangeToHighlight.TryChangeText(selectedSug))
                    {
                        this.VerificationWindowInteractive.InvokeMethod(() => 
                            PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(), "تغییر مورد نظر قابل اعمال نیست!"));

                        return ProceedTypes.InvalidUserAction;
                    }
                    else
                    {
                        base.RefreshForChangeCalled(m_curVerData.RangeToHighlight, selectedSug);
                        return ProceedTypes.IdleProceed;
                    }
                }
            }
            else if(userAction == UserSelectedActions.ChangeAll)
            {
                m_sessionLogger.AddUsage(selectedSug);

                var par = m_curVerData.RangeToHighlight.FirstParagraph;
                var priorRange = par.GetRange(par.Start, m_curVerData.RangeToHighlight.Start);
                if (priorRange != null && !priorRange.IsRangeValid)
                    priorRange = null;

                if (m_lastSCS == SpaceCorrectionState.SpaceInsertationLeft ||
                    m_lastSCS == SpaceCorrectionState.SpaceInsertationLeftSerrially)
                {
                    if (priorRange != null)
                    {
                        try
                        {
                            priorRange.ReplaceAllTwoWordsCombination(m_curWord1, m_curWord2, selectedSug);
                        }
                        catch
                        {
                            priorRange = null;

#if DEBUG
                            throw;
#endif
                        }
                    }

                    DocumentUtils.ReplaceAllTwoWordsCombinationInDocument(this.Document,
                                                                          m_curWord1, m_curWord2, selectedSug);
                }
                else if (m_lastSCS == SpaceCorrectionState.SpaceInsertationRight ||
                         m_lastSCS == SpaceCorrectionState.SpaceInsertationRightSerrially)
                {
                    if (priorRange != null)
                    {
                        try
                        {
                            priorRange.ReplaceAllTwoWordsCombination(m_curWord0, m_curWord1, selectedSug);
                        }
                        catch
                        {
                            priorRange = null;

#if DEBUG
                            throw;
#endif
                        }
                    }

                    DocumentUtils.ReplaceAllTwoWordsCombinationInDocument(this.Document,
                                                                          m_curWord0, m_curWord1, selectedSug);

                }
                else if (m_lastSCS == SpaceCorrectionState.SpaceDeletation ||
                         m_lastSCS == SpaceCorrectionState.SpaceDeletationSerrially)
                {
                    if (priorRange != null)
                    {
                        try
                        {
                            priorRange.ReplaceAllWordsStandardized(m_curWord1, selectedSug);
                        }
                        catch
                        {
                            priorRange = null;

#if DEBUG
                            throw;
#endif
                        }
                    }

                    DocumentUtils.ReplaceAllWordsStandardizedInDocument(Document, m_curWord1, selectedSug);
                }
                else
                {
                    if (priorRange != null)
                    {
                        try
                        {
                            priorRange.ReplaceAllWordsStandardized(m_curWord1, selectedSug);
                        }
                        catch
                        {
                            priorRange = null;

#if DEBUG
                            throw;
#endif
                        }
                    }

                    DocumentUtils.ReplaceAllWordsStandardizedInDocument(this.Document, m_curWord1, selectedSug);
                }

                int priorRangeLength = 0;
                if (priorRange == null || !priorRange.IsRangeValid)
                {
                    priorRangeLength = 0;
                }
                else
                {
                    try
                    {
                        priorRangeLength = priorRange.Text.Length;
                    }
                    catch (Exception)
                    {
                        priorRangeLength = 0;
                        throw;
                    }
                }

                base.RefreshForChangeAllCalled(priorRangeLength, m_curVerData.RangeToHighlight, selectedSug);
                return ProceedTypes.IdleProceed;

            }
            else if(userAction == UserSelectedActions.IgnoreAll)
            {
                Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.AddToIgnoreList(m_curWordCombToCheck);
                return ProceedTypes.IdleProceed;

            }
            else if (userAction == UserSelectedActions.AddToDictionary)
            {
                string strToAdd = m_curWord1;

                if (verResult.ViewerControlArg != null)
                    strToAdd = (string)verResult.ViewerControlArg;

                if (!AddToDictionary(strToAdd))
                    return ProceedTypes.InvalidUserAction;

                base.RefreshForAddToDictionaryCalled();
                return ProceedTypes.ActiveProceed;
            }

            return proceedType;
        }

        public bool AddToDictionary(string wordToAdd)
        {
            string[] dicSuggestions = m_engine.GetSimpleFormOfWord(wordToAdd);
            string selectedDicSuggestion = null;

            if (dicSuggestions.Length <= 0)
            {
                this.VerificationWindowInteractive.InvokeMethod(() =>
                    PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(), 
                        "افزودن این واژه به واژه‌نامه ممکن نیست!"));
            }
            else if (dicSuggestions.Length == 1 && dicSuggestions[0] == wordToAdd)
            {
                selectedDicSuggestion = dicSuggestions[0];
            }
            else
            {
                this.VerificationWindowInteractive.InvokeMethod(() =>
                    {
                        selectedDicSuggestion = ListBoxForm.ShowListBoxForm(
                            VerificationWindowInteractive.GetWin32Window(), 
                            dicSuggestions, wordToAdd,
                            "کدام کلمه به واژه‌نامه افزوده شود؟ لطفاً ساده‌ترین شکل معنادار کلمه را انتخاب کنید.",
                            "افزودن به واژه‌نامه");

                    });
            }

            if (selectedDicSuggestion != null)
            {
                if (m_engine.AddToDictionary(selectedDicSuggestion, wordToAdd, Globals.ThisAddIn.SpellCheckerWrapper.UserDictionary))
                {
                    //PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(), "کلمه با موفقیت به واژه‌نامه افزوده شد");
                    LogHelper.Info(Constants.LogKeywords.EntryAddedToDictionary, selectedDicSuggestion);
                    return true;
                }

                this.VerificationWindowInteractive.InvokeMethod(() =>
                    PersianMessageBox.Show(
                        VerificationWindowInteractive.GetWin32Window(), "این شکل از کلمه در واژه‌نامه وجود دارد!"));
            }

            return false;
        }

        public override void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion)
        {
            if (rangeToChange.Text != suggestion)
            {
                if (rangeToChange.TryChangeText(suggestion))
                {
                    base.RefreshForChangeCalled(m_curVerData.RangeToHighlight, suggestion);
                    m_bachModeStats++;
                }
            }
        }

        protected override bool NeedRefinedStrings
        {
            get { return true; }
        }

        public override string Title
        {
            get { return m_isPrespellChecker ? "پیش‌پردازش املایی متن" : "غلط‌یاب املایی"; }
        }

        public override string HelpTopicFileName
        {
            get { return m_isPrespellChecker ? HelpConstants.SpellCheckerPreprocessing : HelpConstants.SpellChecker; }
        }

        public override UserSelectedActions ActionsToDisable
        {
            get { return UserSelectedActions.None; }
        }

        public override Type SuggestionViewerType
        {
            get { return typeof(SpellCheckerSuggestionViewer); }
        }

        public override bool ShowBatchModeStats()
        {
            string message = string.Format("تعداد اصلاحات انجام شده: {0}", ParsingUtils.ConvertNumber2Persian(m_bachModeStats.ToString()));
            PersianMessageBox.Show(ThisAddIn.GetWin32Window(), message, Constants.UIMessages.SuccessRefinementTitle);
            return false;
        }

        protected override StringVerificationData CheckNGramWordsList(TokenInfo[] readItems, int mainItemIndex)
        {
            if (String.IsNullOrEmpty(readItems[mainItemIndex].Value))
            {
                return null;
            }

            

            m_curWord1 = readItems[mainItemIndex].Value;
            m_curWord1 = StringUtil.NormalizeSpacesAndHalfSpacesInWord(m_curWord1).Normalize(NormalizationForm.FormC);

            m_curWord0 = mainItemIndex > 0 ? readItems[mainItemIndex - 1].Value : "";
            m_curWord0 = StringUtil.NormalizeSpacesAndHalfSpacesInWord(m_curWord0).Normalize(NormalizationForm.FormC);

            m_curWord2 = mainItemIndex < readItems.Length - 1 ? readItems[mainItemIndex + 1].Value : "";
            m_curWord2 = StringUtil.NormalizeSpacesAndHalfSpacesInWord(m_curWord2).Normalize(NormalizationForm.FormC);

            if (Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.IsExistInIgnoreList(m_curWord1))
            {
                return null;
            }

            SpaceCorrectionState scs;
            SuggestionType st;

            try
            {
                string[] sugs;
                if (m_isPrespellChecker)
                {
                    if (m_engine.OnePassCorrection(m_curWord1, m_curWord0, m_curWord2, m_engine.SuggestionCount,
                                                   out sugs, out st, out scs))
                    {
                        return null;
                    }
                }
                else
                {
                    if (m_engine.CheckSpelling(m_curWord1, m_curWord0, m_curWord2, m_engine.SuggestionCount, out sugs,
                                               out st, out scs))
                    {
                        return null;
                    }
                }

                if(!m_isPrespellChecker)
                    sugs = m_sessionLogger.Sort(sugs);

                m_lastSugs = sugs;
                m_lastSCS = scs;
            }
            catch (Exception)
            {
                // TODO: log
                //LogHelper.ErrorException("Exception in CheckSpelling", ex);
                return null;
            }

            VerificationTypes recentErrorType;
            switch (st)
            {
                case SuggestionType.Green:
                    recentErrorType = VerificationTypes.Warning;
                    break;
                //case SuggestionType.Red:
                default:
                    recentErrorType = VerificationTypes.Error;
                    break;
            }

            m_curWordCombToCheck = m_curWord1;

            int startItemIndex, endItemIndex;
            if (scs == SpaceCorrectionState.SpaceInsertationLeft ||
                scs == SpaceCorrectionState.SpaceInsertationLeftSerrially)
            {
                m_curWordCombToCheck = m_curWord1 + ' ' + m_curWord2;
                startItemIndex = mainItemIndex;
                endItemIndex = mainItemIndex + 1;
            }
            else if (scs == SpaceCorrectionState.SpaceInsertationRight ||
                     scs == SpaceCorrectionState.SpaceInsertationRightSerrially)
            {
                m_curWordCombToCheck = m_curWord0 + ' ' + m_curWord1;
                startItemIndex = mainItemIndex - 1;
                endItemIndex = mainItemIndex;
            }
            else
            {
                startItemIndex = mainItemIndex;
                endItemIndex = mainItemIndex;
            }

            if (Globals.ThisAddIn.SpellCheckerWrapper.IgnoreList.IsExistInIgnoreList(m_curWordCombToCheck))
            {
                return null;
            }

            return new StringVerificationData
                       {
                           ErrorIndex = readItems[startItemIndex].Index,
                           ErrorLength = readItems[endItemIndex].EndIndex - readItems[startItemIndex].Index + 1,
                           ErrorType = recentErrorType,
                           Suggestions = new SpellCheckerSuggestion
                            {
                               Message = "درج پیشنهاد:",
                               SuggestionItems = m_lastSugs
                            }
                       };
        }

        protected override WordTokenizerOptions TokenizerOptions
        {
            get { return WordTokenizerOptions.ReturnPunctuations; }
        }

    }
}

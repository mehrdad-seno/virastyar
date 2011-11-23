using System;
using System.Windows.Forms;
using SCICT.NLP.TextProofing.Punctuation;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.GUI;
using VirastyarWordAddin.Verifiers.Basics;
using VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox;
using SCICT.NLP.Utility;
using SCICT.Microsoft.Office.Word.ContentReader;

namespace VirastyarWordAddin.Verifiers.PunctuationVerification
{
    public class PunctuationVerifier : StateMachineVerifierBase
    {
        private readonly PunctuationCheckerEngine m_punctuationCheckerEngine = null;
        private string[] m_lastSugs = null;
        private int m_stats = 0;

        public PunctuationVerifier()
        {
            m_punctuationCheckerEngine = new PunctuationCheckerEngine(
                SettingsHelper.GetFullPath(Constants.PatternsFileName, VirastyarFilePathTypes.AllUsersFiles));
        }

        private static int SearchArray(string[] ar, string key)
        {
            for (int i = 0; i < ar.Length; ++i)
                if (ar[i] == key)
                    return i;
            return -1;
        }

        public override string Title
        {
            get { return "تصحیح نشانه‌گذاری"; }
        }

        public override UserSelectedActions ActionsToDisable
        {
            get { return UserSelectedActions.AddToDictionary; }
        }

        public override string HelpTopicFileName
        {
            get { return HelpConstants.Punctuations; }
        }

        public override Type SuggestionViewerType
        {
            get { return typeof(TitledListBoxSuggestionViewer); }
        }

        protected override bool NeedRefinedStrings
        {
            get { return true; }
        }

        protected override StringVerificationData FindNextPattern()
        {
            m_punctuationCheckerEngine.FindMistake();
            if (!m_punctuationCheckerEngine.IsErrorFound())
            {
                m_lastSugs = null;
                return null;
            }

            int ind = m_punctuationCheckerEngine.GetMistakeIndex();
            int len = m_punctuationCheckerEngine.GetMistakeLength();
            m_lastSugs = m_punctuationCheckerEngine.GetMultiSubstitutionString();

            if (this.VerificationWindowInteractive != null)
            {
                if (m_punctuationCheckerEngine.IsAllChangeable())
                {
                    this.VerificationWindowInteractive.EnableAction(UserSelectedActions.ChangeAll);
                }
                else
                {
                    this.VerificationWindowInteractive.DisableAction(UserSelectedActions.ChangeAll);
                }
            }

            return new StringVerificationData
                       {
                           ErrorIndex = ind,
                           ErrorLength = len,
                           ErrorType = VerificationTypes.Error,
                           Suggestions = new TitledListBoxSuggestion
                                             {
                                                 Message = StringUtil.RefineAndFilterPersianWord(
                                                     m_punctuationCheckerEngine.GetMistakeDescription()),
                                                 SuggestionItems = m_lastSugs
                                                     
                                             }
                       };
        }

        public override void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion)
        {
            int selectedIndex = SearchArray(m_lastSugs, suggestion);

            if (rangeToChange.Text != suggestion)
            {
                //string preText = rangeToChange.Text;
                if (rangeToChange.TryChangeText(suggestion))
                {
                    VerificationWindowBatchMode.CurrentStatus = m_punctuationCheckerEngine.GetMistakeDescription(); // preText + ": " + suggestion;
                    m_punctuationCheckerEngine.CorrectMistake(selectedIndex);
                    base.RefreshForChangeCalled(null, suggestion);
                    m_stats++;
                }
                else
                {
                    m_punctuationCheckerEngine.SkipMistake();
                    // Do nothing, or do it without making the window lose focus
                    // i.e., do NOT show a message box
                    //MessageBox.Show("تغییر مورد نظر قابل اعمال نیست!");
                }
            } // end of common sections between change and change-all
            else
            {
                m_punctuationCheckerEngine.SkipMistake();
            }
        }

        public override ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verResult)
        {
            UserSelectedActions userAction = verResult.UserAction;
            var data = m_curVerifData;

            string selectedSug = verResult.SelectedSuggestion;

            if (userAction == UserSelectedActions.Change || userAction == UserSelectedActions.ChangeAll)
            {
                int selectedIndex = SearchArray(m_lastSugs, selectedSug);

                if (data.RangeToHighlight.Text != selectedSug)
                {
                    if (data.RangeToHighlight.TryChangeText(selectedSug))
                    {
                        m_punctuationCheckerEngine.CorrectMistake(selectedIndex);
                    }
                    else
                    {
                        m_punctuationCheckerEngine.SkipMistake();
                        if(userAction == UserSelectedActions.Change)
                        {
                            this.VerificationWindowInteractive.InvokeMethod(
                                () => PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(), "تغییر مورد نظر قابل اعمال نیست!"));
                            
                            return ProceedTypes.InvalidUserAction;
                        }
                    }
                } // end of common sections between change and change-all

                // it is an if by purpose
                if (userAction == UserSelectedActions.ChangeAll)
                {
                    m_punctuationCheckerEngine.SetGoldenRule();
                    // Now the new change-all stuff
                    m_punctuationCheckerEngine.BookMarkSkipIndex();

                    PerformReplaceAll(selectedIndex);

                    string parTextNew = data.RangeToHighlight.FirstParagraph.Text;
                    if (this.NeedRefinedStrings)
                        parTextNew = StringUtil.RefineAndFilterPersianWord(parTextNew);

                    m_punctuationCheckerEngine.InitInputString(parTextNew);
                    m_punctuationCheckerEngine.RecallSkipIndex();
                    m_punctuationCheckerEngine.UnsetGoldenRule();
                }

                if (userAction == UserSelectedActions.Change)
                    base.RefreshForChangeCalled(data.ErrorText, selectedSug);
                else
                    base.RefreshForChangeAllCalled(data.ErrorText, selectedSug);


                return ProceedTypes.IdleProceed;
            }
            else if (userAction == UserSelectedActions.Ignore)
            {
                m_punctuationCheckerEngine.SkipMistake();
                return ProceedTypes.IdleProceed;
            }
            else if (userAction == UserSelectedActions.IgnoreAll)
            {
                m_punctuationCheckerEngine.DisableLastRule();
                return ProceedTypes.IdleProceed;
            }

            return ProceedTypes.IdleProceed;
        }


        private void PerformReplaceAll(int selectedIndex)
        {
            foreach (var par in RangeWrapper.ReadParagraphs(Document))
            {
                if (CancelationPending)
                    break;

                if (!par.IsRangeValid)
                    continue;

                string rawParText = par.Text;
                string parText = rawParText;
                if (NeedRefinedStrings)
                    parText = StringUtil.RefineAndFilterPersianWord(parText);

                m_punctuationCheckerEngine.InitInputString(parText);

                m_punctuationCheckerEngine.FindMistake();
                while (m_punctuationCheckerEngine.IsErrorFound())
                {
                    if (CancelationPending)
                        break;

                    var curSug = m_punctuationCheckerEngine.GetMultiSubstitutionString()[selectedIndex];

                    int startIndex = m_punctuationCheckerEngine.GetMistakeIndex();
                    int endIndex = m_punctuationCheckerEngine.GetMistakeLength() + startIndex - 1;

                    RangeWrapper foundRange;

                    if (NeedRefinedStrings)
                    {
                        foundRange = par.GetRangeWithCharIndex(
                            StringUtil.IndexInNotFilterAndRefinedString(rawParText, startIndex),
                            StringUtil.IndexInNotFilterAndRefinedString(rawParText, endIndex)
                            );
                    }
                    else
                    {
                        foundRange = par.GetRangeWithCharIndex(startIndex, endIndex);
                    }

                    if (foundRange.IsRangeValid && foundRange.Text != curSug)
                    {
                        if (foundRange.TryChangeText(curSug))
                            m_punctuationCheckerEngine.CorrectMistake(selectedIndex);
                        else
                            m_punctuationCheckerEngine.SkipMistake();
                    }
                    else
                    {
                        m_punctuationCheckerEngine.SkipMistake();
                    }

                    m_punctuationCheckerEngine.FindMistake();
                }

            }
        }

        public override bool ShowBatchModeStats()
        {
            string message = string.Format("تعداد اصلاحات انجام شده: {0}", ParsingUtils.ConvertNumber2Persian(m_stats.ToString()));
            PersianMessageBox.Show(ThisAddIn.GetWin32Window(), message, Title);
            return true;
        }

        protected override bool InitParagraphString(string par)
        {
            m_punctuationCheckerEngine.InitInputString(par);
            return true;
        }

    }
}

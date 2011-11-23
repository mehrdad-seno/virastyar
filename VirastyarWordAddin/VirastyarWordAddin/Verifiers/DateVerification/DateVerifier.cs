using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility.Parsers;
using VirastyarWordAddin.Verifiers.Basics;
using VirastyarWordAddin.Verifiers.CustomGUIs.DateSuggestions;
using SCICT.Utility.GUI;

namespace VirastyarWordAddin.Verifiers.DateVerification
{
    public class DateVerifier : ShrinkingVerifierBase
    {
        private readonly PersianDateParser m_perianDataParser = new PersianDateParser();
        private readonly EnglishDateParser m_englishDateParser = new EnglishDateParser();
        private readonly NumericDateParser m_numericDateParser = new NumericDateParser();

        private readonly List<IPatternInfo> m_lstPatternInfo = new List<IPatternInfo>();
        private IPatternInfo m_minPi = null;

        protected override bool NeedRefinedStrings
        {
            get { return true; }
        }

        public override void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion)
        {
            throw new NotImplementedException();
        }

        public override string Title
        {
            get { return "مبدل تاریخ"; }
        }

        public override UserSelectedActions ActionsToDisable
        {
            get
            {
                return UserSelectedActions.AddToDictionary |
                    UserSelectedActions.IgnoreAll |
                    UserSelectedActions.ChangeAll;
            }
        }

        public override string HelpTopicFileName
        {
            get { return HelpConstants.DateConvertor; }
        }

        public override Type SuggestionViewerType
        {
            get { return typeof(DateSuggestionsViewer); }
        }

        public override ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verRes)
        {
            UserSelectedActions userAction = verRes.UserAction;
            string selectedSug = verRes.SelectedSuggestion;
            //Debug.WriteLine(userAction.ToString());

            if (userAction == UserSelectedActions.Change)
            {
                if (m_curVerifData.RangeToHighlight.Text != selectedSug)
                {
                    if (!m_curVerifData.RangeToHighlight.TryChangeText(selectedSug))
                    {
                        this.VerificationWindowInteractive.InvokeMethod(() =>
                            PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(), "تغییر مورد نظر قابل اعمال نیست!")
                        );

                        return ProceedTypes.InvalidUserAction;
                    }
                    else
                    {
                        base.RefreshForChangeCalled(m_curVerifData.ErrorText, selectedSug);
                        return ProceedTypes.IdleProceed;
                    }
                }
            }

            return ProceedTypes.IdleProceed;
        }

        private IPatternInfo PeakFirstVerification()
        {
            IPatternInfo minPi = null;

            int minIndex = Int32.MaxValue;
            foreach (var pi in m_lstPatternInfo)
            {
                if (pi.Index < minIndex)
                {
                    minIndex = pi.Index;
                    minPi = pi;
                }
            }

            return minPi;
        }


        protected override StringVerificationData FindPattern(string content)
        {
            m_lstPatternInfo.Clear();

            foreach (var pi in m_perianDataParser.FindAndParse(content))
            {
                if(pi.YearNumber >= 0)
                    m_lstPatternInfo.Add(pi);
            }

            foreach (var pi in m_englishDateParser.FindAndParse(content))
            {
                if (pi.YearNumber >= 0)
                    m_lstPatternInfo.Add(pi);
            }

            foreach (var pi in m_numericDateParser.FindAndParse(content))
            {
                m_lstPatternInfo.Add(pi);
            }

            if (m_lstPatternInfo.Count <= 0)
            {
                return null;
            }

            m_minPi = PeakFirstVerification();

            return new StringVerificationData
                       {
                           ErrorIndex = m_minPi.Index,
                           ErrorLength = m_minPi.Length,
                           ErrorType = VerificationTypes.Information,
                           Suggestions = new DateSuggestion
                                             {
                                                 Message = "موارد یافت شده:",
                                                 MainPattern = m_minPi
                                             }
                       };

        }

        public override bool ShowBatchModeStats()
        {
            return false;
        }
    }
}

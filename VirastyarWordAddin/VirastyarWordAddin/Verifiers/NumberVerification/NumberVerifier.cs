using System;
using System.Collections.Generic;
using SCICT.NLP.Utility;
using VirastyarWordAddin.Verifiers.Basics;
using SCICT.NLP.Utility.Parsers;
using VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.Utility.GUI;
using SCICT.NLP.Utility.PersianParsers;
using System.Text;

namespace VirastyarWordAddin.Verifiers.NumberVerification
{
    public class NumberVerifier : ShrinkingVerifierBase
    {
        private readonly PersianRealNumberParser m_perianRealNumberParser = new PersianRealNumberParser();
        private readonly DigitizedNumberParser m_digitizedNumberParser = new DigitizedNumberParser();
        private readonly NumberChangeRule m_changeRules = new NumberChangeRule();


        private List<IPatternInfo> m_lstPatterns;

        protected override bool NeedRefinedStrings
        {
            get { return true; }
        }

        public override string Title
        {
            get { return "مبدل اعداد"; }
        }

        public override UserSelectedActions ActionsToDisable
        {
            get
            {
                return UserSelectedActions.AddToDictionary | 
                    UserSelectedActions.IgnoreAll; 
                //|UserSelectedActions.ChangeAll;
            }
        }

        public override string HelpTopicFileName
        {
            get { return HelpConstants.NumberConvertor; }
        }

        public override Type SuggestionViewerType
        {
            get { return typeof(TitledListBoxSuggestionViewer); }
        }

        private List<IPatternInfo> FindNumberPatterns(string content)
        {
            var lstPatternInfos = new List<IPatternInfo>();

            lstPatternInfos.Clear();

            IPatternInfo firstPi = null;
            foreach (IPatternInfo pi in m_perianRealNumberParser.FindAndParse(content))
            {
                if (firstPi == null)
                {
                    firstPi = pi;
                    lstPatternInfos.Add(pi);
                }
                else
                {
                    if (pi.Index == firstPi.Index && pi.Length == firstPi.Length)
                        lstPatternInfos.Add(pi);
                    else
                        break;
                }
            }

            foreach (IPatternInfo pi in m_digitizedNumberParser.FindAndParse(content))
            {
                if (firstPi != null)
                {
                    if ((pi.Index < firstPi.Index) || (pi.Index == firstPi.Index && pi.Length > firstPi.Length))
                    {
                        lstPatternInfos.Clear();
                        lstPatternInfos.Add(pi);
                    }
                }
                else
                {
                    //firstPi = pi;
                    lstPatternInfos.Add(pi);
                }

                break;
            }

            return lstPatternInfos;
        }

        #region Suggestionns

        private string[] CreateSuggestions(IPatternInfo minPi)
        {
            return NumberParsersSuggestions.CreateSuggestions(m_changeRules, minPi);
        }

        #endregion

        public override bool ShowBatchModeStats()
        {
            return false;
        }

        public override void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion)
        {
            throw new NotImplementedException();
        }

        public override ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verRes)
        {
            UserSelectedActions userAction = verRes.UserAction;
            string selectedSug = verRes.SelectedSuggestion;
            //Debug.WriteLine(userAction.ToString());

            if (userAction == UserSelectedActions.Change)
            {
                m_changeRules.AddNumberChangeRule(m_curVerifData.ErrorText, selectedSug);

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
            else if(userAction == UserSelectedActions.ChangeAll)
            {
                KeyValuePair<NumberChangeRule.InputFormats, NumberChangeRule.InputDigitLanguages> ruleKey;
                KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages> ruleValue;
                m_changeRules.AddNumberChangeRule(m_curVerifData.ErrorText, selectedSug, out ruleKey, out ruleValue);

                if (m_curVerifData.RangeToHighlight.Text != selectedSug)
                {
                    if (!m_curVerifData.RangeToHighlight.TryChangeText(selectedSug))
                    {
                        this.VerificationWindowInteractive.InvokeMethod(() =>
                            PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(),"تغییر مورد نظر قابل اعمال نیست!"));
                    }
                }

                PerformReplaceAll(ruleKey, ruleValue);
                base.RefreshForChangeCalled(m_curVerifData.ErrorText, selectedSug); ;
            }

            return ProceedTypes.IdleProceed;

        }

        private void PerformReplaceAll(KeyValuePair<NumberChangeRule.InputFormats, NumberChangeRule.InputDigitLanguages> ruleKey, KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages> ruleValue)
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


                int startFrom = 0;
                int len = parText.Length;

                var sbRawText = new StringBuilder(rawParText);
                var sbParText = new StringBuilder(parText);

                string parToVerify = parText; // parToVerify is going to shrink while parText is fixed
                while(startFrom < len)
                {
                    var lstPats = FindNumberPatterns(parToVerify);
                    if(lstPats == null || lstPats.Count <= 0)
                        break;

                    if (CancelationPending)
                        break;

                    IPatternInfo minPi = lstPats[0];

                    int startIndex = minPi.Index + startFrom;
                    int endIndex = minPi.Index + minPi.Length - 1 + startFrom;
                    int rawStartIndex = StringUtil.IndexInNotFilterAndRefinedString(sbRawText.ToString(), startIndex);
                    int rawEndIndex = StringUtil.IndexInNotFilterAndRefinedString(sbRawText.ToString(), endIndex);


                    NumberChangeRule.InputFormats inpFormat;
                    NumberChangeRule.InputDigitLanguages inpLang;
                    NumberChangeRule.DectectInputFormat(minPi.Content, out inpFormat, out inpLang);

                    int forwardingOffset = 0;
                    if (ruleKey.Key == inpFormat && ruleKey.Value == inpLang)
                    {
                        string suggestion = NumberParsersSuggestions.CreateSuggestionFor(minPi, ruleValue);

                        if (!String.IsNullOrEmpty(suggestion))
                        {
                            RangeWrapper foundRange;

                            if (NeedRefinedStrings)
                            {
                                foundRange = par.GetRangeWithCharIndex(rawStartIndex, rawEndIndex);
                            }
                            else
                            {
                                foundRange = par.GetRangeWithCharIndex(startIndex, endIndex);
                            }

                            if (foundRange.IsRangeValid && foundRange.Text != suggestion)
                            {
                                if(foundRange.TryChangeText(suggestion))
                                {
                                    if(NeedRefinedStrings)
                                    {
                                        sbRawText.Remove(rawStartIndex, rawEndIndex - rawStartIndex + 1);
                                        sbRawText.Insert(rawStartIndex, suggestion);
                                    }

                                    sbParText.Remove(startIndex, endIndex - startIndex + 1);
                                    sbParText.Insert(startIndex, suggestion);

                                    //System.Diagnostics.Debug.WriteLine(sbParText);
                                    //System.Diagnostics.Debug.WriteLine("---------------------------");

                                    len = sbParText.Length;
                                    forwardingOffset = suggestion.Length - minPi.Length;
                                }
                            }
                        } // end if (suggestions count > 0)
                    } // end if (minPi content matches change rule)

                    startFrom = endIndex + 1 + forwardingOffset;
                    if(startFrom >= len)
                        break;
                    parToVerify = sbParText.ToString().Substring(startFrom);
                }


            }
        }


        /// <summary>
        /// Finds the first and the most prominent pattern in the string.
        /// </summary>
        /// <param name="content">The content to search the pattern in.</param>
        protected override StringVerificationData FindPattern(string content)
        {
            m_lstPatterns = FindNumberPatterns(content);
            if (m_lstPatterns == null || m_lstPatterns.Count <= 0)
            {
                return null;
            }

            IPatternInfo minPi = m_lstPatterns[0];

            var listSugs = new List<string>();
            foreach (IPatternInfo pi in m_lstPatterns)
            {
                listSugs.AddRange(CreateSuggestions(pi));
            }

            return new StringVerificationData
                       {
                           ErrorIndex = minPi.Index,
                           ErrorLength = minPi.Length,
                           ErrorType = VerificationTypes.Information,
                           Suggestions = new TitledListBoxSuggestion
                                       {
                                           Message = "پیشنهادها",
                                           SuggestionItems = listSugs
                                       }
                       };
        }
    }
}

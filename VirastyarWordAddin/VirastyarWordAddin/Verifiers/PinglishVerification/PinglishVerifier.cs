using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility.Transliteration;
using SCICT.NLP.Utility.Transliteration.KNN;
using SCICT.Utility.GUI;
using SCICT.Utility.SpellChecker;
using VirastyarWordAddin.Verifiers.Basics;
using SCICT.NLP.Utility;
using VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox;
using SCICT.Microsoft.Office.Word.ContentReader;
using System.Windows.Forms;
using SCICT.NLP;

namespace VirastyarWordAddin.Verifiers.PinglishVerification
{
    public class PinglishVerifier : NGramVerifierBase
    {
        private readonly SessionLogger m_sessionLogger;
        private readonly PinglishConverter m_pinglishConverter;

        public PinglishVerifier() : base(0, 0)
        {
            #region Transliteration Config

            var config = new PinglishConverterConfig();
            
            config.ExceptionWordDicPath = SettingsHelper.GetFullPath(
                Constants.ExceptionWordsFileName, VirastyarFilePathTypes.AllUsersFiles);
            config.GoftariDicPath = SettingsHelper.GetFullPath(
                    Constants.GoftariDicFileName, VirastyarFilePathTypes.AllUsersFiles);
            config.StemDicPath = SettingsHelper.GetFullPath(
                    Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles);

            config.XmlDataPath = SettingsHelper.GetFullPath(
                Constants.PinglishFileName, VirastyarFilePathTypes.AllUsersFiles);

            #endregion

            m_pinglishConverter = new PinglishConverter(config, PruneType.Stem, false);

            this.m_sessionLogger = new SessionLogger();
        }

        public override string Title
        {
            get { return "تبدیل از پینگلیش به پارسی"; }
        }

        public override string HelpTopicFileName
        {
            get { return HelpConstants.NumberConvertor; }
        }

        public override UserSelectedActions ActionsToDisable
        {
            get { return UserSelectedActions.AddToDictionary | UserSelectedActions.IgnoreAll; }
        }

        public override Type SuggestionViewerType
        {
            get { return typeof(TitledListBoxSuggestionViewer); }
        }


        protected override bool NeedRefinedStrings
        {
            get { return false; }
        }

        protected override WordTokenizerOptions TokenizerOptions
        {
            get { return WordTokenizerOptions.TreatNumberNonArabicCharCombinationAsOneWords; }
        }

        protected override bool IsProperWord(string word)
        {
            return StringUtil.IsPinglishWord(word);
        }

        protected override StringVerificationData CheckNGramWordsList(TokenInfo[] readItems, int mainItemIndex)
        {
            Debug.Assert(readItems.Length == 1);
            Debug.Assert(mainItemIndex == 0);

            string word = readItems[mainItemIndex].Value;

            var sugs = new string[0];
            try
            {
                sugs = m_pinglishConverter.Convert(word, false);
            }
            catch
            {
                throw;
            }

            // TODO: Consider its usability
            if (sugs.Length == 0)
                return null;

            return new StringVerificationData
                       {
                           ErrorIndex = readItems[mainItemIndex].Index,
                           ErrorLength = readItems[mainItemIndex].Length,
                           ErrorType = VerificationTypes.Information,
                           Suggestions = new TitledListBoxSuggestion
                                             {
                                                 Message = "پیشنهادها",
                                                 SuggestionItems = sugs
                                             }
                       };
        }

        public override ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verResult)
        {
            UserSelectedActions userAction = verResult.UserAction;
            string selectedSug = verResult.SelectedSuggestion;
            string wordToChange = m_curVerData.RangeToHighlight.Text;

            if (userAction == UserSelectedActions.Change)
            {
                if (wordToChange != selectedSug)
                {
                    if (!m_curVerData.RangeToHighlight.TryChangeText(selectedSug))
                    {
                        this.VerificationWindowInteractive.InvokeMethod(() =>
                            PersianMessageBox.Show(VerificationWindowInteractive.GetWin32Window(), "تغییر مورد نظر قابل اعمال نیست!")
                        );

                        return ProceedTypes.InvalidUserAction;
                    }
                    else
                    {
                        this.m_sessionLogger.AddUsage(selectedSug);
                        base.RefreshForChangeCalled(m_curVerData.RangeToHighlight, selectedSug);
                        return ProceedTypes.IdleProceed;
                    }
                }
            }
            else if(userAction == UserSelectedActions.ChangeAll)
            {
                this.m_sessionLogger.AddUsage(selectedSug);

                var par = m_curVerData.RangeToHighlight.FirstParagraph;
                var priorRange = par.GetRange(par.Start, m_curVerData.RangeToHighlight.Start);
                if (priorRange != null && !priorRange.IsRangeValid)
                    priorRange = null;

                try
                {
                    if (priorRange != null)
                        priorRange.ReplaceAllWordsCaseInsensitive(wordToChange, selectedSug);
                }
                catch
                {
                    priorRange = null;
#if DEBUG
                    throw;
#endif
                }

                DocumentUtils.ReplaceAllWordsCaseInsensitiveInDocument(this.Document, wordToChange, selectedSug);

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
#if DEBUG
                        throw;
#endif
                    }
                }

                base.RefreshForChangeAllCalled(priorRangeLength, m_curVerData.RangeToHighlight, selectedSug);
                return ProceedTypes.IdleProceed;
            }

            return ProceedTypes.IdleProceed;
        }


        public override void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion)
        {
            string wordToChange = rangeToChange.Text;
            if (wordToChange != suggestion)
            {
                if (!m_curVerData.RangeToHighlight.TryChangeText(suggestion))
                {
                    //this.VerificationWindowInteractive.InvokeMethod(() =>
                    //{
                    //    //MessageBox.Show(VerificationWindowInteractive.GetWin32Window(),
                    //    //                "تغییر مورد نظر قابل اعمال نیست!");
                    //});

                }
                else
                {
                    m_sessionLogger.AddUsage(suggestion);
                    base.RefreshForChangeCalled(m_curVerData.RangeToHighlight, suggestion);
                }
            }
        }

        public override bool ShowBatchModeStats()
        {
            return false;
        }
    }
}

using System;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.PinglishConverter;
using SCICT.Utility.SpellChecker;
using SCICT.NLP.TextProofing.SpellChecker;
using System.Collections.Generic;
using VirastyarWordAddin.Log;

namespace VirastyarWordAddin
{
    class PinglishVerifier : VerifierBase
    {
        readonly SessionLogger m_sessionLogger;
        private readonly PinglishConverter m_pinglishConverter;

        public PinglishVerifier(string mainDicPath) : base()
        {
            this.m_pinglishConverter = new PinglishConverter();

            /*this.m_pinglishConverter.LoadConverter(Globals.ThisAddIn.GetFullPath(
                Constants.PinglishFileName, VirastyarFilePathTypes.AllUsersFiles));*/
            var lstWords = PinglishConverterUtils.LoadPinglishStrings(SettingsHelper.GetFullPath(
                Constants.PinglishFileName, VirastyarFilePathTypes.AllUsersFiles));
            ((IPinglishLearner)this.m_pinglishConverter).Learn(lstWords);

            this.m_pinglishConverter.LoadPreprocessElements(SettingsHelper.GetFullPath(
                Constants.PinglishPreprocessFileName, VirastyarFilePathTypes.AllUsersFiles));


            SpellCheckerConfig spellerConfig = new SpellCheckerConfig
                {
                    DicPath = SettingsHelper.GetFullPath(
                        Constants.InformalDicFileName, VirastyarFilePathTypes.AllUsersFiles),
                    StemPath = SettingsHelper.GetFullPath(
                        Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles)
                };

            SpellCheckerEngine spellerEngine = new SpellCheckerEngine(spellerConfig);
            this.m_pinglishConverter.SetSpellerEngine(spellerEngine);

            this.m_sessionLogger = new SessionLogger();
        }

        protected override void InitVerifWin()
        {
            m_verificationWindow = new SimpleVerificationWindow();
            m_verificationWindow.SetCaption("تبدیل پینگلیش");
            m_verificationWindow.SetContentCaption("موارد یافت شده:");
            ((SimpleVerificationWindow)m_verificationWindow).SetSuggestionCaption(Constants.UIMessages.Suggestions);
            m_verificationWindow.DisableButtionsPermanently(VerificationWindowButtons.AddToDictionary | VerificationWindowButtons.IgnoreAll);
            m_verificationWindow.Verifier = this;
            // TODO: help is not correct
            m_verificationWindow.SetHelp(HelpConstants.NumberConvertor);
        }

        protected override bool ProcessParagraphForVerification(MSWordBlock b)
        {
            string realWord;
            foreach (Range w in b.Range.Words)
            {
                if (this.CancellationPending)
                    return false;

                realWord = w.Text.Trim();
                RangeUtils.TrimRange(w);
                if (realWord.Length > 0)
                {
                    if (!ProcessForPinglishConversion(w, b))
                        return false;
                }
            }
            return true;
        }

        private bool ProcessForPinglishConversion(Range curword, MSWordBlock wholeParagraph)
        {
            if (curword == null || curword.Text == null) return true;

            string word = curword.Text.Trim();

            if (StringUtil.IsPinglishWord(word))
            {
                string[] sugs = new string[0];
                try
                {
                    sugs = m_pinglishConverter.SuggestFarsiWords(word, true);
                }
                catch(Exception ex)
                {
                    LogHelper.DebugException("Exception in SuggestFarsiWords", ex);
                    throw;
                }

                // TODO: Consider its usability
                if (sugs.Length == 0)
                    return true;

                string selectedSug = "";
                if (VerifierType == TextProcessType.Interactive)
                {
                    VerificationWindowButtons buttonPressed = 
                        m_verificationWindow.ShowDialog(new PinglishVerificationWinArgs(wholeParagraph.Range, curword, sugs), 
                        out selectedSug);

                    switch (buttonPressed)
                    {
                        case VerificationWindowButtons.Change:
                            this.m_sessionLogger.AddUsage(selectedSug);
                            if (curword.Text != selectedSug)
                                Globals.ThisAddIn.SetRangeContent(curword, selectedSug, true);
                            break;
                        case VerificationWindowButtons.ChangeAll:
                            this.m_sessionLogger.AddUsage(selectedSug);
                            Globals.ThisAddIn.ReplaceAllCaseInsensitive(word, selectedSug);
                            break;
                        case VerificationWindowButtons.Ignore:
                            break;
                        case VerificationWindowButtons.IgnoreAll:
                            break;
                        case VerificationWindowButtons.AddToDictionary:
                            break;
                        case VerificationWindowButtons.Stop:
                        default:
                            return false;
                    }
                }
                else if (VerifierType == TextProcessType.Batch)
                {
                    if(sugs.Length > 0)
                        Globals.ThisAddIn.SetRangeContent(curword, sugs[0], false);
                }
            }

            return true;
        }
    }
}

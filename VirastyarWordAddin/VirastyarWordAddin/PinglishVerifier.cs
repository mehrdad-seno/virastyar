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

using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.PinglishConverter;
using SCICT.Utility.SpellChecker;
using SCICT.NLP.TextProofing.SpellChecker;
using System.Collections.Generic;

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
            m_verificationWindow.SetCaption("تبدیل از پینگلیش به پارسی");
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
                catch 
                {
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

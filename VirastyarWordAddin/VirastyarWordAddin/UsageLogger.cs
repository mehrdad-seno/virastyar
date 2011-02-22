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
using System.Diagnostics;
using log4net;

namespace VirastyarWordAddin
{
    public class UsageLogger
    {
        private const string loggerName = "PWA_UsageLogger";
        private ILog logger;

        string lastAction = "";
        string lastContent = "";
        string lastSuggestion = "";
        int lastSuggestionIndex = -1;

        public CallerTypes LastCaller { get; set; }

        public UsageLogger()
        {
            this.logger = LogManager.GetLogger(loggerName);
            LastCaller = CallerTypes.Unknown;
        }

        [Conditional("DEBUG")]
        public void WriteMessage(string msg)
        {
            this.logger.Info(msg);
        }

        [Conditional("DEBUG")]
        public void LogRefineAllPressed()
        {
            WriteMessage("Refine-All Pressed");
        }

        [Conditional("DEBUG")]
        public void SetAction(string str)
        {
            lastAction = str;
        }

        [Conditional("DEBUG")]
        public void SetContent(string str)
        {
            lastContent = str;
        }

        [Conditional("DEBUG")]
        public void SetSelectedSuggestions(string suggestion, int suggestionIndex)
        {
            this.lastSuggestion = suggestion;
            this.lastSuggestionIndex = suggestionIndex;
        }

        [Conditional("DEBUG")]
        internal void LogLastAction()
        {
            if (lastAction == "Change" || lastAction == "ChangeAll")
            {
                WriteMessage(String.Format("Caller: {1}{0}Action: {2}{0}From: {3}{0}To: {4}{0}SuggestionIndex: {5}",
                    Environment.NewLine, this.LastCaller.ToString(), lastAction, lastContent, lastSuggestion, lastSuggestionIndex));
            }
            else
            {
                WriteMessage(String.Format("Caller: {1}{0}Action: {2}{0}Content: {3}",
                    Environment.NewLine, this.LastCaller.ToString(), lastAction, lastContent));
            }
        }
    }

    public enum CallerTypes
    {
        Unknown,
        SpellChecker,
        SpellPreprocessing,
        PinglishVerifier,
        NumberVerifier,
        DateVerifier,
        PunctuationVerifer
    }
}

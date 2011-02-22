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
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.Utility.Keyboard;

namespace VirastyarWordAddin.Configurations
{
    #region Delegates

    //public delegate void ShortcutChangedEventHandler(ShortcutChangedEventArgs e);
    public delegate void RefineAllSettingsChangedEventHandler(RefineAllSettingsChangedEventArgs e);
    public delegate void SpellCheckSettingsChangedEventHandler(SpellCheckSettingsChangedEventArgs e);
    public delegate void PreprocessSpellSettingsChangedEventHandler();

    #endregion

    #region EventArgs

    public class RefineAllSettingsChangedEventArgs : EventArgs
    {
        public AllCharactersRefinerSettings Settings { get; set; }
    }

    public class ShortcutChangedEventArgs : EventArgs
    {
        public string ShortcutName { get; set; }
        public Hotkey NewHotkey { get; set; }
        public Hotkey OldHotkey { get; set; }
        public bool Cancel { get; set; }
    }

    public class SpellCheckSettingsChangedEventArgs : EventArgs
    {
        public SpellCheckSettingsChangedEventArgs()
        {
            ErroneousUserDictionaries = new List<string>();
        }

        public SpellCheckerConfig Settings { get; set; }

        public string[] CustomDictionaries { get; set; }

        public bool CancelLoadingUserDictionary { get; set; }
        public List<string> ErroneousUserDictionaries  { get; set; }

        public bool RuleVocabWordSpacingCorrection { get; set; }
        public bool RuleDontCheckSingleLetters { get; set; }

        public bool RuleHeYeConvertion { get; set; }

        public bool RefineHaa { get; set; }
        public bool RefineMee { get; set; }
        public bool RefineHeYe { get; set; }
        public bool RefineBe { get; set; }
        public bool RefineAllAffixes { get; set; }
    }

    #endregion
}

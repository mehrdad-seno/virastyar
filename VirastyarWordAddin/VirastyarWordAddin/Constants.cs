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
using Microsoft.Win32;

namespace VirastyarWordAddin
{
    public enum OfficeVersion
    {
        Office2003,
        Office2007,
        Office2010
    }

    public class Constants
    {
        public const string Office2003Version = "11.0";
        public const string Office2007Version = "12.0";
        public const string Office2010Version = "14.0";

        public const string PinglishFileName = "Pinglish.dat";
        public const string PatternsFileName = "Patterns.txt";
        public const string UserDicFileName = "UserDictionary.dat";
        public const string MainDicFileName = "dic.dat";
        public const string PinglishPreprocessFileName = "PinglishPreprocesses.dat";
        public const string InformalDicFileName = "dic_informal.dat";

        public const string VirastyarApplicationRegKey = "Software\\SCICT\\Virastyar";
        public const string VirastyarApplicationUsersRegKey = VirastyarApplicationRegKey + "\\Users";
        public static readonly string FullApplicationRegKey = Registry.LocalMachine.Name + "\\" + VirastyarApplicationRegKey;
        
        [Obsolete("The template mechansim has been changed.", true)]
        public const string CustomTemplateName = "VirastyarCustomTemplate.dot";

        public const string Virastyar2003TemplateName = "Virastyar2003.dot";
        public const string Virastyar2007TemplateName = "Virastyar2007.dotm";
        public const string Virastyar2010TemplateName = "Virastyar2010.dotm";

        public const string Virastyar = "Virastyar";

        [Obsolete("Use Virastyar Instead", true)]
        public const string PersianWordAddin = "PersianWordAddin";

        public const string PerUserFolderName = Virastyar;

        public const string StemFileName = "stem.dat";

        public const string OfficeRegKey = "Software\\Microsoft\\Office";

        public const string OfficeUserTemplatesName = "UserTemplates";

        public const string TipsOfTheDayFileName = "Tips.tod";

        #region Addin Toolbar

        public const string ToolbarPrefix = "Virastyar";
        public const string OldToolbarPrefix = "Noor";
        public const string TextToolbarName = ToolbarPrefix + "-Text";
        public const string SettingsToolbarName = ToolbarPrefix + "-Settings";
        public const string RefinementToolbarName = ToolbarPrefix + "-Refinement";
        public const string ConversionToolbarName = ToolbarPrefix + "-Conversion";

        #endregion

        public class MacroNames
        {
            public const string VirastyarPinglishConvert_Action = "VirastyarPinglishConvert_Action";
            public const string VirastyarPinglishConvertAll_Action = "VirastyarPinglishConvertAll_Action";
            public const string VirastyarCheckDates_Action = "VirastyarCheckDates_Action";
            public const string VirastyarCheckNumbers_Action = "VirastyarCheckNumbers_Action";
            public const string VirastyarCheckSpell_Action = "VirastyarCheckSpell_Action";
            public const string VirastyarPreCheckSpell_Action = "VirastyarPreCheckSpell_Action";
            public const string VirastyarCheckPunctuation_Action = "VirastyarCheckPunctuation_Action";
            public const string VirastyarCheckAllPunctuation_Action = "VirastyarCheckAllPunctuation_Action";
            public const string VirastyarRefineAllCharacters_Action = "VirastyarRefineAllCharacters_Action";
            public const string VirastyarAddinSettings_Action = "VirastyarAddinSettings_Action";
            public const string VirastyarAbout_Action = "VirastyarAbout_Action";
            public const string VirastyarAutoComplete_Action = "VirastyarAutoComplete_Action";
            public const string VirastyarHelp_Action = "VirastyarHelp_Action";
        }

        public class UIMessages
        {
            public const string SuccessRefinementTitle = "عملیات اصلاح با موفقیت به پایان رسید";
            public const string Suggestions = "پیشنهادها";
        }
    }
}

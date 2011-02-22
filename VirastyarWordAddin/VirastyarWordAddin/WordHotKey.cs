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
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using VirastyarWordAddin;
using SCICT.Utility.Keyboard;

namespace SCICT.Microsoft.Word
{
    public class WordHotKey
    {
        private static WdKey GetWdKeyFromString(string keyString, out bool withShift)
        {
            withShift = false;
            keyString = keyString.Trim().ToLower();

            if (keyString == "control" || keyString == "ctrl")
                return WdKey.wdKeyControl;
            else if (keyString == "alt")
                return WdKey.wdKeyAlt;
            else if (keyString == "shift")
                return WdKey.wdKeyShift;
            else if (keyString.Length == 1)
            {
                if (Char.IsLetterOrDigit(keyString[0]))
                {
                    try
                    {
                        return (WdKey)Enum.Parse(typeof(WdKey), "wdKey" + keyString.ToUpper());
                    }
                    catch
                    {
                        throw new Exception("Invalid Letter or digit to create keybinding from");
                    }
                }
                else if (Char.IsPunctuation(keyString[0]))
                {
                    switch (keyString[0])
                    {
                        case '`':
                            return WdKey.wdKeyBackSingleQuote;
                        case '\\':
                            return WdKey.wdKeyBackSlash;
                        case '=':
                            return WdKey.wdKeyEquals;
                        case '.':
                            return WdKey.wdKeyPeriod;
                        case '/':
                            return WdKey.wdKeySlash;
                        case '~':
                            withShift = true;
                            return WdKey.wdKeyBackSingleQuote;
                        case '!':
                            withShift = true;
                            return WdKey.wdKey1;
                        case '@':
                            withShift = true;
                            return WdKey.wdKey2;
                        case '#':
                            withShift = true;
                            return WdKey.wdKey3;
                        case '$':
                            withShift = true;
                            return WdKey.wdKey4;
                        case '%':
                            withShift = true;
                            return WdKey.wdKey5;
                        case '^':
                            withShift = true;
                            return WdKey.wdKey6;
                        case '&':
                            withShift = true;
                            return WdKey.wdKey7;
                        case '*':
                            withShift = true;
                            return WdKey.wdKey8;
                        case '(':
                            withShift = true;
                            return WdKey.wdKey9;
                        case ')':
                            withShift = true;
                            return WdKey.wdKey0;
                        case '_':
                            withShift = true;
                            return WdKey.wdKeyHyphen;
                        case '+': // this will not happen since + is a separator
                            withShift = true;
                            return WdKey.wdKeyEquals;
                        case '|':
                            withShift = true;
                            return WdKey.wdKeyBackSlash;

                        case '}':
                            withShift = true;
                            return WdKey.wdKeyCloseSquareBrace;
                        case '{':
                            withShift = true;
                            return WdKey.wdKeyOpenSquareBrace;
                        case '"':
                            withShift = true;
                            return WdKey.wdKeySingleQuote;
                        case ':':
                            withShift = true;
                            return WdKey.wdKeySemiColon;
                        case '?':
                            withShift = true;
                            return WdKey.wdKeySlash;
                        case '>':
                            withShift = true;
                            return WdKey.wdKeyPeriod;
                        case '<':
                            withShift = true;
                            return WdKey.wdKeyComma;

                        case ']':
                            return WdKey.wdKeyCloseSquareBrace;
                        case '-':
                            return WdKey.wdKeyHyphen;
                        case '[':
                            return WdKey.wdKeyOpenSquareBrace;
                        case ',':
                            return WdKey.wdKeyComma;
                        case '\'':
                            return WdKey.wdKeySingleQuote;

                    }
                }
            }
            else
            {
                switch (keyString)
                {
                    case "minus":
                    case "subtract":
                    case "dash":
                    case "hyphen":
                        return WdKey.wdKeyHyphen;
                    case "plus":
                    case "add":
                        withShift = true;
                        return WdKey.wdKeyEquals;
                    case "f1":
                        return WdKey.wdKeyF1;
                    case "f2":
                        return WdKey.wdKeyF2;
                    case "f3":
                        return WdKey.wdKeyF3;
                    case "f4":
                        return WdKey.wdKeyF4;
                    case "f5":
                        return WdKey.wdKeyF5;
                    case "f6":
                        return WdKey.wdKeyF6;
                    case "f7":
                        return WdKey.wdKeyF7;
                    case "f8":
                        return WdKey.wdKeyF8;
                    case "f9":
                        return WdKey.wdKeyF9;
                    case "f10":
                        return WdKey.wdKeyF10;
                    case "f11":
                        return WdKey.wdKeyF11;
                    case "f12":
                        return WdKey.wdKeyF12;
                    case "tab":
                        return WdKey.wdKeyTab;
                    case "space":
                        return WdKey.wdKeySpacebar;
                    case "esc":
                        return WdKey.wdKeyEsc;
                    case "escape":
                        return WdKey.wdKeyEsc;

                    case "back space":
                    case "back-space":
                    case "backspace":
                    case "backsp":
                    case "bcksp":
                        return WdKey.wdKeyBackspace;

                    case "insert":
                    case "ins":
                        return WdKey.wdKeyInsert;
                    case "del":
                    case "delete":
                        return WdKey.wdKeyDelete;
                    case "home":
                        return WdKey.wdKeyHome;
                    case "end":
                        return WdKey.wdKeyEnd;
                    case "page up":
                    case "page-up":
                    case "pg up":
                    case "pg-up":
                        return WdKey.wdKeyPageUp;
                    case "page down":
                    case "page-down":
                    case "pg down":
                    case "pg-down":
                        return WdKey.wdKeyPageDown;
                    case "enter":
                    case "return":
                        return WdKey.wdKeyReturn;
                    default:
                        break;
                }
            }

            return WdKey.wdNoKey;
        }

        private static int GetKeyCodeFromString(Application wordApp, string keyString)
        {
            string[] keys = keyString.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

            int length = Math.Min(4, keys.Length);

            object missing = Missing.Value;
            List<object> lstKeys = new List<object>();

            for (int i = 0; i < keys.Length; i++)
            {
                bool isWithShift = false;
                object curKey = GetWdKeyFromString(keys[i], out isWithShift);
                if ((WdKey)curKey == WdKey.wdNoKey)
                    continue;

                lstKeys.Add(curKey);
                if (isWithShift)
                    lstKeys.Add(WdKey.wdKeyShift);
            }

            // remove duplicate shifts if any
            bool isShiftFound = false;
            for (int i = lstKeys.Count - 1; i >= 0; i--)
            {
                if ((WdKey)lstKeys[i] == WdKey.wdKeyShift)
                {
                    if (isShiftFound)
                    {
                        lstKeys.RemoveAt(i);
                    }
                    else
                    {
                        isShiftFound = true;
                    }
                }
            }

            int remainings = 4 - lstKeys.Count;
            for (int i = 0; i < remainings; i++)
            {
                lstKeys.Add(missing);
            }

            object key1 = lstKeys[1]; object key2 = lstKeys[2]; object key3 = lstKeys[3];
            return wordApp.BuildKeyCode((WdKey)lstKeys[0], ref key1, ref key2, ref key3);
        }

        public static bool IsKeyAlreadyAssigned(Application wordApp, Hotkey hotkey, out string command)
        {
            command = "";
            try
            {
                object missing = Missing.Value;
                int keyCode = GetKeyCodeFromString(wordApp, hotkey.ToString());
                KeyBinding keyBinding = wordApp.get_FindKey(keyCode, ref missing);

                if (String.IsNullOrEmpty(keyBinding.Command)) // if it is not a word default shortcut...
                {
                    // check if it is a custom shortcut or not
                    bool isCustomShortcut = false;

                    string keyString = keyBinding.KeyString;

                    int count = wordApp.KeyBindings.Count;
                    for (int i = 1; i <= count; i++)
                    {
                        KeyBinding curKeyBinding = wordApp.KeyBindings[i];
                        if (curKeyBinding.KeyString == keyString && curKeyBinding.KeyCategory != WdKeyCategory.wdKeyCategoryDisable)
                        {
                            command = keyBinding.KeyCategory == WdKeyCategory.wdKeyCategoryMacro ? "MACRO" : keyBinding.Command;
                            isCustomShortcut = true;
                            keyBinding = curKeyBinding;
                            break;
                        }
                    }

                    return isCustomShortcut;
                }
                else
                {
                    command = keyBinding.Command;
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool AssignKeyToCommand(Template templateToSave, Hotkey hotkey, string macroCommand)
        {
            Application wordApp = templateToSave.Application;
            object oldTemplate = wordApp.CustomizationContext;
            wordApp.CustomizationContext = templateToSave;

            //macroCommand = "TemplateProject.ThisDocument." + macroCommand;

            bool assinged = AssignKeyToCommand(wordApp, hotkey, macroCommand);
            if (assinged)
            {
                templateToSave.Save();
            }

            wordApp.CustomizationContext = oldTemplate;
            return assinged;
        }

        public static bool AssignKeyToCommand(Application wordApp, Hotkey hotkey, string macroCommand)
        {
            object missing = Missing.Value;
            int keyCode = GetKeyCodeFromString(wordApp, hotkey.ToString());

            try
            {
                wordApp.KeyBindings.Add(WdKeyCategory.wdKeyCategoryMacro, macroCommand, keyCode, ref missing, ref missing);
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Hint: DOES NOT WORKS CORRECTLY, or in the expected manner.
        /// </summary>
        public static string RetreiveCurrentKey(Template template, string macroName)
        {
            object oldTemplate = template.Application.CustomizationContext;
            template.Application.CustomizationContext = template;
            int count = template.Application.KeyBindings.Count;
            string result = "";
            object missing = Missing.Value;
            for (int i = 1; i <= count; i++)
            {
                KeyBinding curKeyBinding = template.Application.KeyBindings[i];
                curKeyBinding = template.Application.get_FindKey(curKeyBinding.KeyCode, ref missing);
                if (curKeyBinding.KeyCategory != WdKeyCategory.wdKeyCategoryDisable
                    && curKeyBinding.Command.Contains(macroName))
                {
                    result = curKeyBinding.KeyString;
                    break;
                }
            }

            template.Application.CustomizationContext = oldTemplate;
            return result;
        }

        public static bool RemoveAssignedKey(Template templateToSave, Hotkey hotkey, string macroCommand)
        {
            Application wordApp = templateToSave.Application;
            object oldTemplate = wordApp.CustomizationContext;
            wordApp.CustomizationContext = templateToSave;

            bool result = RemoveAssignedKey(wordApp, hotkey, macroCommand);
            if (result)
            {
                templateToSave.Save();
            }

            wordApp.CustomizationContext = oldTemplate;
            return result;
        }

        public static bool RemoveAssignedKey(Application wordApp, Hotkey hotkey, string macroCommand)
        {
            int count = wordApp.KeyBindings.Count;
            for (int i = count; i >= 1; i--)
            {
                KeyBinding curKeyBinding = wordApp.KeyBindings[i];
                if (curKeyBinding.KeyCode == GetKeyCodeFromString(wordApp, hotkey.ToString())) //if (curKeyBinding.Command == macroCommand)
                {
                    try
                    {
                        curKeyBinding.Disable();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}

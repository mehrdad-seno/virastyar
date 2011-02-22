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
using System.Windows.Forms;
using SCICT.Microsoft.Win32;
using ManagedWinapi;

namespace SCICT.Utility.Keyboard
{
    public static class KeyboardHelper
    {
        /// <summary>
        /// Converts a keycode to its corresponding string representation
        /// </summary>
        public static string KeyCodeToChar(Keys key)
        {
            return new KeyboardKey(key).KeyName;
        }

        public static bool IsNavigationKey(Keys key)
        {
            if (key == Keys.Up || key == Keys.Down
                || key == Keys.Right || key == Keys.Left
                || key == Keys.PageDown || key == Keys.PageUp
                || key == Keys.Home || key == Keys.End)
            {
                return true;
            }

            return false;
        }

        #region Comment
        //public static string ScanCodeToString(int scanCode)
        //{
        //    uint procId;
        //    uint thread = GetWindowThreadProcessId(Process.GetCurrentProcess().MainWindowHandle, out procId);
        //    IntPtr hkl = GetKeyboardLayout(thread);

        //    if (hkl == IntPtr.Zero)
        //    {
        //        Console.WriteLine("Sorry, that keyboard does not seem to be valid.");
        //        return string.Empty;
        //    }

        //    Key[] keyStates = new Key[256];
        //    if (!GetKeyboardState(keyStates))
        //        return string.Empty;

        //    StringBuilder sb = new StringBuilder(10);
        //    int rc = ToUnicodeEx((uint)scanCode, (uint)scanCode, keyStates, sb, sb.Capacity, 0, hkl);
        //    return sb.ToString();
        //}
        #endregion

    }
}

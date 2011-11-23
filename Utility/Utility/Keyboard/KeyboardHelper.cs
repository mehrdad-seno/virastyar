using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SCICT.Microsoft.Windows;
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

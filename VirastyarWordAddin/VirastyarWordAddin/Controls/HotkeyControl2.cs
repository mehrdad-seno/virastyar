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
using System.Windows.Forms;
using SCICT.Utility.Keyboard;
using System.Diagnostics;

namespace VirastyarWordAddin.Controls
{
    public partial class HotkeyControl2 : UserControl
    {
        public event EventHandler HotkeyChanged;

        #region Private Members

        private Keys m_key = Keys.None;

        private Keys Key 
        { 
            get
            {
                return m_key;
            }
            set
            {
                if (m_key == value)
                    return;
                this.m_key = value;
                txtKey.Text = KeyboardHelper.KeyCodeToChar(m_key);
                if (m_key == Keys.None)
                {
                    txtKey.SelectAll();
                }
                OnHotkeyChanged();
            }
        }

        private void OnHotkeyChanged()
        {
            if (HotkeyChanged != null)
            {
                HotkeyChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        /// <summary>
        /// Used to make sure that there is no right-click menu available
        /// </summary>
        public override ContextMenu ContextMenu
        {
            get
            {
                return null;
            }
            set
            {
                // Nothing
            }
        }

        /// <summary>
        /// Creates a new HotkeyControl
        /// </summary>
        public HotkeyControl2()
        {
            InitializeComponent();

            // Handle events that occurs when keys are pressed
            txtKey.KeyPress += HotkeyControl_KeyPress;
            //txtKey.KeyUp += HotkeyControl_KeyUp;
            txtKey.KeyDown += HotkeyControl_KeyDown;

            Clear();
        }

        /// <summary>
        /// Fires when a key is pushed down. Here, we'll want to update the text in the box
        /// to notify the user what combination is currently pressed.
        /// </summary>
        void HotkeyControl_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("KeyDown_Modifiers:" + e.Modifiers);
        }

        private void UnsetAllModifiers()
        {
            chkAlt.Checked = false;
            chkCtrl.Checked = false;
            chkShift.Checked = false;
        }

        private bool OnlyShiftIsChecked()
        {
            return (chkShift.Checked && !chkAlt.Checked && !chkCtrl.Checked);
        }

        private bool NoModifierIsChecked()
        {
            if (chkAlt.Checked || chkCtrl.Checked || chkShift.Checked)
                return false;

            return true;
        }

        /// <summary>
        /// Prevents the letter/whatever entered to show up in the TextBox
        /// Without this, a "A" key press would appear as "aControl, Alt + A"
        /// </summary>
        void HotkeyControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handles some misc keys, such as Ctrl+Delete and Shift+Insert
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete || keyData == (Keys.Control | Keys.Delete))
            {
                txtKey.Text = "";
                return true;
            }

            if (keyData == (Keys.Shift | Keys.Insert)) // Paste
                return true; // Don't allow

            // Allow the rest
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Clears the current hotkey and resets the TextBox
        /// </summary>
        public void Clear()
        {
            this.Key = Keys.None;
            UnsetAllModifiers();
        }

        /// <summary>
        /// Used to get/set the hotkey 
        /// </summary>
        public Hotkey Hotkey
        {
            get
            {
                return new Hotkey(Key, chkAlt.Checked, chkCtrl.Checked, chkShift.Checked);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                Key = value.Key;

                chkAlt.Checked = value.Alt;
                chkCtrl.Checked = value.Control;
                chkShift.Checked = value.Shift;
            }
        }

        private void chkCtrl_CheckedChanged(object sender, EventArgs e)
        {
            OnHotkeyChanged();
        }
    }
}

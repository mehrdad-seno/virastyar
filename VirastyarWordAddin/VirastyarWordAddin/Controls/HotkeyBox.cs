using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using SCICT.Utility;
using SCICT.Utility.Keyboard;
using System.Diagnostics;


namespace VirastyarWordAddin.Controls
{
    public class HotkeyBox : TextBox
    {
        public event EventHandler HotkeyChanged;

        private void OnHotkeyChanged()
        {
            if (HotkeyChanged != null)
                HotkeyChanged(this, EventArgs.Empty);
        }

        // These variables store the current hotkey and modifier(s)
        private Keys m_key = Keys.None;
        private Keys m_modifiers = Keys.None;

        // ArrayLists used to enforce the use of proper modifiers.
        // Shift+A isn't a valid hotkey, for instance, as it would screw up when the user is typing.
        private static List<int> needNonShiftModifier = new List<int>();
        
        static HotkeyBox()
        {
            // Fill the ArrayLists that contain all invalid hotkey combinations
            PopulateModifierLists();
        }

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

            }
        }

        /// <summary>
        /// Forces the control to be non-multiline
        /// </summary>
        public override bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                // Ignore what the user wants; force Multiline to false
                base.Multiline = false;
            }
        }

        /// <summary>
        /// Creates a new HotkeyControl
        /// </summary>
        public HotkeyBox()
        {
            base.Multiline = false;

            // Handle events that occurs when keys are pressed
            this.KeyPress += new KeyPressEventHandler(HotkeyControl_KeyPress);
            this.KeyUp += new KeyEventHandler(HotkeyControl_KeyUp);
            this.KeyDown += new KeyEventHandler(HotkeyControl_KeyDown);
        }

        /// <summary>
        /// Populates the ArrayLists specifying disallowed hotkeys
        /// such as Shift+A, Ctrl+Alt+4 (would produce a dollar sign) etc
        /// </summary>
        private static void PopulateModifierLists()
        {
            needNonShiftModifier.Clear();

            // Shift + 0 - 9, A - Z
            for (Keys k = Keys.D0; k <= Keys.Z; k++)
                needNonShiftModifier.Add((int)k);

            // Shift + Numpad keys
            for (Keys k = Keys.NumPad0; k <= Keys.NumPad9; k++)
                needNonShiftModifier.Add((int)k);

            // Shift + Misc (,;<./ etc)
            for (Keys k = Keys.Oem1; k <= Keys.OemBackslash; k++)
                needNonShiftModifier.Add((int)k);

            // Shift + Space, PgUp, PgDn, End, Home
            for (Keys k = Keys.Space; k <= Keys.Home; k++)
                needNonShiftModifier.Add((int)k);

            // Misc keys that we can't loop through
            needNonShiftModifier.Add((int)Keys.Insert);
            needNonShiftModifier.Add((int)Keys.Help);
            needNonShiftModifier.Add((int)Keys.Multiply);
            needNonShiftModifier.Add((int)Keys.Add);
            needNonShiftModifier.Add((int)Keys.Subtract);
            needNonShiftModifier.Add((int)Keys.Divide);
            needNonShiftModifier.Add((int)Keys.Decimal);
            needNonShiftModifier.Add((int)Keys.Return);
            needNonShiftModifier.Add((int)Keys.Escape);
            needNonShiftModifier.Add((int)Keys.NumLock);
            needNonShiftModifier.Add((int)Keys.Scroll);
            needNonShiftModifier.Add((int)Keys.Pause);
        }

        /// <summary>
        /// Resets this hotkey control to None
        /// </summary>
        public new void Clear()
        {
            this.Hotkey = Hotkey.None;
        }

        /// <summary>
        /// Fires when a key is pushed down. Here, we'll want to update the text in the box
        /// to notify the user what combination is currently pressed.
        /// </summary>
        void HotkeyControl_KeyDown(object sender, KeyEventArgs e)
        {
            // Clear the current hotkey
            ThisAddIn.DebugWriteLine("KeyCode:" + e.KeyCode);
            ThisAddIn.DebugWriteLine("KeyData:" + e.KeyData);
            ThisAddIn.DebugWriteLine("KeyValue:" + e.KeyValue);
            ThisAddIn.DebugWriteLine("-----------------");

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                SetHotkey(Keys.None, Keys.None);
            }
            else
            {
                SetHotkey(e.KeyCode, e.Modifiers);
            }	
        }

        /// <summary>
        /// Fires when all keys are released. If the current hotkey isn't valid, reset it.
        /// Otherwise, do nothing and keep the text and hotkey as it was.
        /// </summary>
        void HotkeyControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.m_key == Keys.None && Control.ModifierKeys == Keys.None)
            {
                SetHotkey(Keys.None, Keys.None);
                return;
            }
        }

        /// <summary>
        /// Prevents the letter/whatever entered to show up in the TextBox
        /// Without this, a "A" key press would appear as "aControl, Alt + A"
        /// </summary>
        void HotkeyControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (m_modifiers.Has<Keys>(Keys.Shift) && e.KeyChar != (char)m_key)
            //{
            //    m_modifiers = m_modifiers.Clear<Keys>(Keys.Shift);
            //    //m_key = (int)e.KeyChar;
            //}

            e.Handled = true;
        }

        /// <summary>
        /// Handles some misc keys, such as Ctrl+Delete and Shift+Insert
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete || keyData == (Keys.Control | Keys.Delete))
            {
                SetHotkey(Keys.None, Keys.None);
                return true;
            }

            if (keyData == (Keys.Shift | Keys.Insert)) // Paste
                return true; // Don't allow

            // Allow the rest
            return base.ProcessCmdKey(ref msg, keyData);
        }

        ///// <summary>
        ///// Clears the current hotkey and resets the TextBox
        ///// </summary>
        //public void ResetHotkey()
        //{
        //    SetHotkey(Keys.None, Keys.None);
        //    Redraw();
        //}

        private void SetHotkey(Keys key, Keys modifiers)
        {
            //this.m_key = key;
            //this.m_modifiers = modifiers;
            Hotkey = ValidateHotkey(key, modifiers);
        }

        public Hotkey Hotkey
        {
            get
            {
                return new Hotkey(m_key,
                    m_modifiers.Has<Keys>(Keys.Alt),
                    m_modifiers.Has<Keys>(Keys.Control),
                    m_modifiers.Has<Keys>(Keys.Shift));
            }
            set
            {
                if (value == null)
                    throw new ArgumentException(); 
                
                Hotkey old = Hotkey;
                if (value == old)
                    return;
                
                this.m_key = value.Key;
                this.m_modifiers = Keys.None;
                if (value.Alt)
                    this.m_modifiers |= Keys.Alt;
                if (value.Shift)
                    this.m_modifiers |= Keys.Shift;
                if (value.Control)
                    this.m_modifiers |= Keys.Control;

                OnHotkeyChanged();
                Text = Hotkey.ToString();
            }
        }

        ///// <summary>
        ///// Helper function
        ///// </summary>
        //private void Redraw()
        //{
        //    ValidateHotkey(false);
        //}

        private static bool NoModifierIsNeededFor(Keys key)
        {
            if (key >= Keys.F2 && key <= Keys.F12)
                return true;
            if (KeyboardHelper.IsNavigationKey(key))
                return true;

            return false;
        }

        /// <summary>
        /// Redraws the TextBox when necessary.
        /// </summary>
        private static Hotkey ValidateHotkey(Keys key, Keys modifiers)
        {
            if (key == Keys.LWin || key == Keys.RWin)
                key = Keys.None;

            Hotkey validatedHotkey = new Hotkey(key, Hotkey.ConvertToWin32Modifiers(modifiers));

            // No hotkey set
            if (key == Keys.None || 
                (modifiers == Keys.None && !NoModifierIsNeededFor(key)))
            {
                validatedHotkey = new Hotkey(Keys.None, SCICT.Utility.Windows.Modifiers.None);
            }

            else if (modifiers.Has(Keys.Shift) && 
                !modifiers.Has(Keys.Control) && 
                !modifiers.Has(Keys.Alt) && 
                needNonShiftModifier.Contains((int)key))
            {
                validatedHotkey = new Hotkey(Keys.None, SCICT.Utility.Windows.Modifiers.None);
            }
            // I have no idea why this is needed, but it is. Without this code, pressing only Ctrl
            // will show up as "Control + ControlKey", etc.
            else if (key == Keys.Menu /* Alt */ || key == Keys.ShiftKey || key == Keys.ControlKey)
            {
                validatedHotkey = new Hotkey(Keys.None, SCICT.Utility.Windows.Modifiers.None);
            }

            ManagedWinapi.KeyboardKey kKey = new ManagedWinapi.KeyboardKey(key);
            ThisAddIn.DebugWriteLine(kKey.KeyName);
            
            return validatedHotkey;
        }
    }
}

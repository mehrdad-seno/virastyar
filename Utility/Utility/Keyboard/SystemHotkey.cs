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
using System.Windows.Forms;
using SCICT.Microsoft.Win32;

namespace SCICT.Utility.Keyboard
{
    /// <summary>
    /// Handles a System Hotkey
    /// </summary>
    public class SystemHotkey : System.ComponentModel.Component, IDisposable
    {
        private System.ComponentModel.Container components = null;
        protected DummyWindowWithEvent m_window = new DummyWindowWithEvent();	//window for WM_Hotkey Messages
        protected Hotkey m_hotKey = Hotkey.None;
        protected bool m_isRegistered = false;
        public event EventHandler Pressed;
        public event EventHandler Error;

        private bool m_isEnabled = true;

        public SystemHotkey(System.ComponentModel.IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            m_window.ProcessMessage += MessageEvent;
        }

        public SystemHotkey()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                m_window.ProcessMessage += MessageEvent;
            }
        }

        public new void Dispose()
        {
            if (m_isRegistered)
            {
                if (UnregisterHotkey())
                    System.Diagnostics.Debug.WriteLine("Unreg: OK");
            }
            System.Diagnostics.Debug.WriteLine("Disposed");
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        protected void MessageEvent(object sender, ref Message m, ref bool handled)
        {	//Handle WM_Hotkey event
            if ((m.Msg == (int)Msgs.WM_HOTKEY) && (m.WParam == (IntPtr)this.GetType().GetHashCode()) && m_isEnabled)
            {
                handled = true;
                System.Diagnostics.Debug.WriteLine("HOTKEY pressed!");
                if (Pressed != null) Pressed(this, EventArgs.Empty);
            }
        }

        protected bool UnregisterHotkey()
        {	//unregister hotkey
            return User32.UnregisterHotKey(m_window.Handle, this.GetType().GetHashCode());
        }

        protected bool RegisterHotkey(Hotkey key)
        {	//register hotkey

            return User32.RegisterHotKey(m_window.Handle, this.GetType().GetHashCode(), (int)key.Modifiers, (int)key.Key);
        }

        protected bool RegisterHotkey(Shortcut key)
        {	//register hotkey
            int mod = 0;
            Keys k2 = Keys.None;
            if (((int)key & (int)Keys.Alt) == (int)Keys.Alt) { mod += (int)Modifiers.Alt; k2 = Keys.Alt; }
            if (((int)key & (int)Keys.Shift) == (int)Keys.Shift) { mod += (int)Modifiers.Shift; k2 = Keys.Shift; }
            if (((int)key & (int)Keys.Control) == (int)Keys.Control) { mod += (int)Modifiers.Control; k2 = Keys.Control; }

            System.Diagnostics.Debug.Write(mod + " ");
            System.Diagnostics.Debug.WriteLine((((int)key) - ((int)k2)).ToString());

            return User32.RegisterHotKey(m_window.Handle, this.GetType().GetHashCode(), mod, ((int)key) - ((int)k2));
        }

        public bool IsRegistered
        {
            get { return m_isRegistered; }
        }

        public bool IsEnabled
        {
            get
            {
                return m_isEnabled;
            }
            set
            {
                m_isEnabled = value;
            }
        }

        public Hotkey Hotkey
        {
            get { return m_hotKey; }
            set
            {
                if (DesignMode) { m_hotKey = value; return; }	//Don't register in Designmode
                if ((m_isRegistered) && (m_hotKey != value))	//Unregister previous registered Hotkey
                {
                    if (UnregisterHotkey())
                    {
                        System.Diagnostics.Debug.WriteLine("Unreg: OK");
                        m_isRegistered = false;
                    }
                    else
                    {
                        if (Error != null) Error(this, EventArgs.Empty);
                        System.Diagnostics.Debug.WriteLine("Unreg: ERR");
                    }
                }
                if (value == Hotkey.None) { m_hotKey = value; return; }
                if (RegisterHotkey(value))	//Register new Hotkey
                {
                    System.Diagnostics.Debug.WriteLine("Reg: OK");
                    m_isRegistered = true;
                }
                else
                {
                    if (Error != null) Error(this, EventArgs.Empty);
                    System.Diagnostics.Debug.WriteLine("Reg: ERR");
                }
                m_hotKey = value;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SCICT.Microsoft.Windows;
using SCICT.Utility.Windows;

namespace SCICT.Utility.Keyboard
{
    public delegate bool ApplicationIsActiveDelegate();

    /// <summary>
    /// 
    /// </summary>
    public interface IHotkeyEngine
    {
        bool RegisterHotkey(Hotkey hotkey, EventHandler handler);
        bool UnregisterHotkey(Hotkey hotkey);
        bool IsAlreadyRegistered(Hotkey hotkey);
        bool IsEnabled { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HotkeyEngine : IHotkeyEngine, IDisposable
    {
        private readonly Dictionary<Hotkey, EventHandler> m_hotkeyHandlers = new Dictionary<Hotkey, EventHandler>();
        private readonly DummyWindowWithEvent m_dummyWindow = new DummyWindowWithEvent();
        private readonly ApplicationIsActiveDelegate m_applicationIsActive = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyEngine"/> class.
        /// </summary>
        private HotkeyEngine()
        {
            m_dummyWindow.ProcessMessage += MessageEvent;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyEngine"/> class.
        /// </summary>
        /// <param name="applicationIsActiveDelegate">A function to determine whether application is active or not</param>
        public HotkeyEngine(ApplicationIsActiveDelegate applicationIsActiveDelegate) : this()
        {
            this.m_applicationIsActive = applicationIsActiveDelegate;
        }

        void MessageEvent(object sender, ref Message msg, ref bool handled)
        {
            handled = false;

            if (!IsEnabled)
                return;

            if (msg.Msg == (int)Msgs.WM_HOTKEY)
            {
                foreach (Hotkey hotKey in m_hotkeyHandlers.Keys)
                {
                    if ((UInt32)msg.WParam == hotKey.GetHashCode())
                    {
                        Debug.WriteLine(hotKey + " pressed!");
                        m_hotkeyHandlers[hotKey](this, EventArgs.Empty);
                        handled = true;
                    }
                }
            }
        }

        #region IMessageFilter Members

        /*bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_HOTKEY)
            {
                foreach (Hotkey hotKey in m_hotkeyHandlers.Keys)
                {
                    if ((UInt32)m.WParam == hotKey.GetHashCode())
                    {
                        m_hotkeyHandlers[hotKey](this, EventArgs.Empty);
                        return true;
                    }
                }
            }
            return false;
        }*/

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                return m_applicationIsActive();
            }
        }

        /// <summary>
        /// Registers the hotkey.
        /// </summary>
        /// <param name="hotkey">The hotkey.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentException">If the given hotkey is already registered in our application.</exception>
        /// <returns></returns>
        public bool RegisterHotkey(Hotkey hotkey, EventHandler handler)
        {
            if (hotkey == Hotkey.None)
                return true;

            if (m_hotkeyHandlers.Keys.Contains(hotkey))
                throw new ArgumentException("Hotkey " + hotkey + " is already registered");

            if (handler == null)
                throw new ArgumentException("", "handler");

            if (User32.RegisterHotKey(m_dummyWindow.Handle, hotkey.GetHashCode(), (int)hotkey.Modifiers, (int)hotkey.Key))
            {
                m_hotkeyHandlers[hotkey] = handler;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        /// <param name="hotkey">The hotkey.</param>
        /// <returns></returns>
        public bool UnregisterHotkey(Hotkey hotkey)
        {
            m_hotkeyHandlers.Remove(hotkey);
            return User32.UnregisterHotKey(m_dummyWindow.Handle, hotkey.GetHashCode());
        }

        public bool IsAlreadyRegistered(Hotkey hotkey)
        {
            if (User32.RegisterHotKey(m_dummyWindow.Handle, hotkey.GetHashCode(), (int)hotkey.Modifiers, (int)hotkey.Key))
            {
                bool success = User32.UnregisterHotKey(m_dummyWindow.Handle, hotkey.GetHashCode());
                Debug.Assert(success);
                return false;
            }

            return true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // TODO: Is it needed or not ?
        }

        #endregion
    }
}
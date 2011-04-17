using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using SCICT.Microsoft.Win32;

namespace SCICT.Utility.Keyboard
{
    /// <summary>
    /// 
    /// </summary>
    public class Hotkey
    {
        public static readonly Hotkey None = new Hotkey();

        private readonly Keys m_key = Keys.None;
        private readonly Modifiers m_modifiers = Modifiers.None;

        /// <summary>
        /// Gets the m_key.
        /// </summary>
        /// <value>The m_key.</value>
        public Keys Key
        {
            get { return m_key; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Hotkey"/> contains alt.
        /// </summary>
        /// <value><c>true</c> if alt; otherwise, <c>false</c>.</value>
        public bool Alt
        {
            get { return (m_modifiers & Modifiers.Alt) == Modifiers.Alt; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Hotkey"/> contains control.
        /// </summary>
        /// <value><c>true</c> if control; otherwise, <c>false</c>.</value>
        public bool Control
        {
            get { return (m_modifiers & Modifiers.Control) == Modifiers.Control; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Hotkey"/> contains win.
        /// </summary>
        /// <value><c>true</c> if win; otherwise, <c>false</c>.</value>
        public bool Win
        {
            get { return (m_modifiers & Modifiers.Win) == Modifiers.Win; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Hotkey"/> contains shift.
        /// </summary>
        /// <value><c>true</c> if shift; otherwise, <c>false</c>.</value>
        public bool Shift
        {
            get { return (m_modifiers & Modifiers.Shift) == Modifiers.Shift; }
        }

        public Hotkey()
        {
        }

        public Hotkey(Keys key, bool alt, bool ctrl, bool shift) : this(key, alt, ctrl, shift, false)
        {
        }

        public Hotkey(Keys key, Modifiers modifiers)
        {
            this.m_key = key;
            this.m_modifiers = modifiers;

            if (key == Keys.None)
                this.m_modifiers = Modifiers.None;
        }

        public Hotkey(Keys key, bool alt, bool control, bool shift, bool win)
        {
            this.m_key = key;

            if (alt)
                m_modifiers |= Modifiers.Alt;
            if (control)
                m_modifiers |= Modifiers.Control;
            if (shift)
                m_modifiers |= Modifiers.Shift;
            if (win)
                m_modifiers |= Modifiers.Win;

            Debug.Assert(Alt == alt);
            Debug.Assert(Control == control);
            Debug.Assert(Shift == shift);

            if (key == Keys.None)
                this.m_modifiers = Modifiers.None;
        }

        /// <summary>
        /// Gets the m_modifiers.
        /// </summary>
        /// <value>The m_modifiers.</value>
        public Modifiers Modifiers
        {
            get
            {
                return this.m_modifiers;
            }
        }

        public override string ToString()
        {
            if (this == Hotkey.None)
                return "";

            var sb = new StringBuilder();
            sb.Append(Shift ? "Shift " : "");
            sb.Append(Control ? "Control " : "");
            sb.Append(Alt ? "Alt " : "");
            sb.Append(Win ? "Win " : "");
            string curStr = sb.ToString();

            string[] comps = curStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            sb = new StringBuilder();
            for (int i = 0; i < comps.Length; i++)
            {
                if (i == 0)
                    sb.Append(comps[i]);
                else
                    sb.Append(" + " + comps[i]);
            }

            if (sb.ToString() != "")
                sb.Append(" + "); 
            sb.Append(KeyboardHelper.KeyCodeToChar(m_key));

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Hotkey))
                return false;

            Hotkey hotkey = obj as Hotkey;
            return (hotkey.Key == this.Key && hotkey.Modifiers == Modifiers);
        }

        public static bool operator ==(Hotkey hotkey1, Hotkey hotkey2)
        {
            if (Object.Equals(hotkey1, null))
                return Object.Equals(hotkey2, null);

            return hotkey1.Equals(hotkey2);
        }

        public static bool operator !=(Hotkey hotkey1, Hotkey hotkey2)
        {
            return !(hotkey1 == hotkey2);
        }

        public override int GetHashCode()
        {
            //return Key.GetHashCode() + Modifiers.GetHashCode();
            return ToString().GetHashCode();
        }

        public static bool TryParse(string hotkey, out Hotkey result)
        {
            try
            {
                result = Parse(hotkey);
                return true;
            }
            catch
            {
                result = Hotkey.None;
                return false;
            }
        }

        /// <summary>
        /// Parses and returns a new instance of Hotkey, from the given string
        /// </summary>
        public static Hotkey Parse(string hotkey)
        {
            bool shift = false;
            bool control = false;
            bool alt = false;
            bool win = false;
            Keys key = Keys.None;

            string[] tokens =  hotkey.Split(' ', '+', ',');
            foreach (string token in tokens)
            {
                switch (token.ToLower())
                {
                    case "shift":
                        shift = true;
                        break;
                    case "control":
                        control = true;
                        break;
                    case "alt":
                        alt = true;
                        break;
                    case "win":
                        win = true;
                        break;
                    case "":
                        break;
                    default:
                        key = (Keys)Enum.Parse(typeof(Keys), token);
                        break;
                }
            }

            return new Hotkey(key, alt, control, shift, win);
        }

        public static Modifiers ConvertToWin32Modifiers(Keys modifiers)
        {
            Modifiers win32Modifiers = Modifiers.None;

            if ((modifiers & Keys.Alt) == Keys.Alt)
                win32Modifiers |= Modifiers.Alt;

            if ((modifiers & Keys.Control) == Keys.Control)
                win32Modifiers |= Modifiers.Control;

            if ((modifiers & Keys.Shift) == Keys.Shift)
                win32Modifiers |= Modifiers.Shift;

            return win32Modifiers;
        }

        public static Keys ConvertToModifiers(Modifiers win32Modifiers)
        {
            Keys modifiers = Keys.None;

            if ((win32Modifiers & Modifiers.Alt) == Modifiers.Alt)
                modifiers |= Keys.Alt;

            if ((win32Modifiers & Modifiers.Control) == Modifiers.Control)
                modifiers |= Keys.Control;

            if ((win32Modifiers & Modifiers.Shift) == Modifiers.Shift)
                modifiers |= Keys.Shift;

            return modifiers;
        }
    }
}

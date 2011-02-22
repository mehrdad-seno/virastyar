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

namespace SCICT.NLP.Utility.StringDistance
{
    ///<summary>
    /// Keboard Key (Letters) data structure in Cartesian Coordinate System
    ///</summary>
    public class KeyboardKey : IComparable<KeyboardKey>
    {
        ///<summary>
        /// Indicates the x-axes position of a key (letter) in Cartesian Coordinate System
        ///</summary>
        public double X;
        ///<summary>
        /// Indicates the y-axes position of a key (letter) in Cartesian Coordinate System
        ///</summary>
        public double Y;
        ///<summary>
        /// Indicates the needs of pressing Shift key to type corresponding key.
        ///</summary>
        public bool UseShift;
        ///<summary>
        /// Indicates the Unicode value of the key in current keyboard layout
        ///</summary>
        public char Value;

        private void Init()
        {
            X = 0.0;
            Y = 0.0;
            UseShift = false;
            Value = 'a';
        }

        #region Constructors
        ///<summary>
        /// Class Constructor
        ///</summary>
        public KeyboardKey()
        {
            Init();
        }
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="ch">Unicode value of the key in current keyboard layout</param>
        public KeyboardKey(char ch)
        {
            Init();
            Value = ch;
        }
        ///<summary>
        /// Class constructor
        ///</summary>
        ///<param name="x">x-axes position of a key (letter) in Cartesian Coordinate System</param>
        ///<param name="y">y-axes position of a key (letter) in Cartesian Coordinate System</param>
        public KeyboardKey(float x, float y)
        {
            Init();
            X = x;
            Y = y;
        }
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="x">x-axes position of a key (letter) in Cartesian Coordinate System</param>
        ///<param name="y">y-axes position of a key (letter) in Cartesian Coordinate System</param>
        ///<param name="ch">Unicode value of the key in current keyboard layout</param>
        public KeyboardKey(float x, float y, char ch)
        {
            Init();
            X = x;
            Y = y;
            Value = ch;
        }
        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="x">x-axes position of a key (letter) in Cartesian Coordinate System</param>
        ///<param name="y">y-axes position of a key (letter) in Cartesian Coordinate System</param>
        ///<param name="ch">Unicode value of the key in current keyboard layout</param>
        ///<param name="useShift">Is pressing Shift key needed to type corresponding key.</param>
        public KeyboardKey(float x, float y, char ch, bool useShift)
        {
            Init();
            X = x;
            Y = y;
            Value = ch;
            UseShift = useShift;
        }
        #endregion
        
        public int CompareTo(KeyboardKey k)
        {
            if (this.Value < k.Value)
                return -1;

            if (this.Value > k.Value)
                return 1;

            return 0;
        }

    }
}

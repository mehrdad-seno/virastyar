// Author: Omid Kashefi, Mitra Nasri 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mitra Nasri at 2010-March-08
//

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

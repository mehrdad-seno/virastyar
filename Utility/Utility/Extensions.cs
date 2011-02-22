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
using System.Diagnostics;

// http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations

namespace SCICT.Utility
{
    /// <summary>
    /// <example>
    /// SomeType value = SomeType.A; 
    /// bool isGrapes = value.Is(SomeType.A); //true 
    /// bool hasGrapes = value.Has(SomeType.A); //true 
     
    /// value = value.Set(SomeType.B); 
    /// value = value.Set(SomeType.C); 
    /// value = value.Clear(SomeType.A); 
     
    /// bool hasB = value.Has(SomeType.A); //true 
    /// bool isB = value.Is(SomeType.B); //false 
    /// bool hasA = value.Has(SomeType.A); //false
    ///</example>
    /// </summary>
    public static class EnumExtensions
    {
        public static bool Has<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }
        
        public static T Set<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }

        public static T Clear<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }
    }

    public static class StringExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            string[] values = value.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(value.Length >= 1);

            T enumVal;
            try
            {
                enumVal = (T) Enum.Parse(typeof (T), values[0], true);

                string enumStr;
                for (int i = 1; i < values.Length; i++)
                {
                    enumStr = values[i];
                    enumVal = (T) (enumVal as Enum).Set<T>((T) Enum.Parse(typeof (T), enumStr));
                }
            }
            catch (Exception)
            {
                enumVal = (T) (object) 0;
            }


            return enumVal;
            //return (T)Enum.Parse(typeof(T), value);
        }

        public static bool StartsWith(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (value.StartsWith(arrStr))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EndsWith(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (value.EndsWith(arrStr))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIn(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (value == arrStr)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool FindsIn(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (arrStr.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] ToStringArray(this string value)
        {
            char[] charArray = value.ToCharArray();

            List<string> strList = new List<string>();

            foreach(char c in charArray)
            {
                strList.Add(c.ToString());
            }

            return strList.ToArray();
        }
    }
}

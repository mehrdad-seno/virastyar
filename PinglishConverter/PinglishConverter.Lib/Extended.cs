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

using System.Collections.Generic;
using System.Linq;

namespace SCICT.NLP.Utility.PinglishConverter
{
    /// <summary>
    /// Useful Extension methods
    /// </summary>
    public static class Extensions
    {
        public static string[] ToStringArray(this List<CharacterMappingInfo> thisObj)
        {
            var array = new string[thisObj.Count];

            int i = 0;
            foreach (var val in thisObj)
            {
                array[i++] = val.Value;
            }
            return array;
        }

        public static void UpdateClone(this List<PinglishString> list, int index, 
            List<CharacterMappingInfo> values, List<CharacterMappingInfo> values2, 
            char letter, char nextLetter)
        {
            if (values.Count == 0)
                return;

            int count = list.Count;

            while (count > 0)
            {
                var original = list[0];
                foreach (var value in values)
                {
                    var fs = original.Clone();
                    fs.Update(index, value.Value, letter);
                    list.Add(fs);
                }

                foreach (var value in values2)
                {
                    var fs = original.Clone();
                    fs.Update(index, value.Value, letter);
                    fs.Update(index + 1, string.Empty, nextLetter);
                    list.Add(fs);
                }

                list.RemoveAt(0);

                --count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="englishLetter"></param>
        /// <param name="persianLetters">Must be sorted based on their weights</param>
        public static void Update(this List<PinglishString> list, char englishLetter, List<string> persianLetters)
        {
            int count = list.Count;

            while (count > 0)
            {
                PinglishString original = list[0];
                foreach (var value in persianLetters)
                {
                    PinglishString fs = original.Clone();
                    fs.Append(value, englishLetter);
                    list.Add(fs);
                }

                list.RemoveAt(0);

                --count;
            }
        }

        public static string[] ToStringArray(this PinglishString[] array, bool removeErab, bool removeDuplicates)
        {
            return (new List<PinglishString>(array)).ToStringList(removeErab, removeDuplicates).ToArray();
        }

        public static List<string> ToStringList(this List<PinglishString> list, bool removeErab, bool removeDuplicates)
        {
            var pinglishResults = new List<string>();
            foreach (var fstr in list)
            {
                if (removeErab)
                    pinglishResults.Add(StringUtil.RemoveErab(fstr.PersianString));
                else
                    pinglishResults.Add(fstr.PersianString);
            }
            if (removeDuplicates)
            {
                pinglishResults = pinglishResults.Distinct().ToList();
            }
            return pinglishResults;
        }

        public static List<PinglishString> ToLower(this List<PinglishString> list)
        {
            var results = new List<PinglishString>();

            foreach (var pinglishString in list)
            {
                results.Add(pinglishString.ToLower());
            }

            return results;
        }

        /// <summary>
        /// Remove duplicate elements within the given list.
        /// </summary>
        public static List<PinglishString> RemoveDuplicates(this List<PinglishString> list)
        {
            return list.Distinct(new PinglishStringEqualityComparer()).ToList();
        }

        /// <summary>
        /// Compares the specified characters.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns></returns>
        public static bool Compare(this char ch1, char ch2, bool caseSensitive)
        {
            if (caseSensitive)
                return (ch1 == ch2);

            return (string.Compare(ch1.ToString(), ch2.ToString(), true) == 0);
        }

        public static void AddOrUpdate(this Dictionary<string, int> list, string value)
        {
            if (list.ContainsKey(value))
            {
                // TODO: Test this
                list[value] += 1;
                //list[mappedChar] = 1;
            }
            else
            {
                list.Add(value, 1);
            }
        }
    }
}
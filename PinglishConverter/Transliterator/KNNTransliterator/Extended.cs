//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//


using System.Collections.Generic;
using System.Linq;
using SCICT.NLP.Utility;

namespace SCICT.NLP.Utility.Transliteration.KNN
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

        public static void Update(this List<PinglishString> list, char englishLetter, Dictionary<string, double > persianLetters)
        {
            if (persianLetters.Count == 0)
                return;

            int count = list.Count;
            while (count > 0)
            {
                PinglishString original = list[0];
                foreach (var value in persianLetters.OrderByDescending(item => item.Value))
                {
                    PinglishString fs = original.Clone();
                    fs.Append(value.Key, englishLetter);
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

        public static string[] ToStringArray(this IEnumerable<PinglishString> list, bool removeErab, bool removeDuplicates)
        {
            return list.ToStringList(removeErab, removeDuplicates).ToArray();
        }

        public static List<string> ToStringList(this IEnumerable<PinglishString> list, bool removeErab, bool removeDuplicates)
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
                //list[value] = 1;
            }
            else
            {
                list.Add(value, 1);
            }
        }
    }
}
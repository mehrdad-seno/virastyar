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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using SCICT.Utility;

namespace SCICT.NLP.Utility.PinglishConverter
{
    /// <summary>
    /// Represents options in PinglishString normalization.
    /// </summary>
    [Flags]
    public enum PinglishStringNormalizationOptions
    {
        /// <summary>
        /// Use the default settings
        /// </summary>
        None = 0,

        /// <summary>
        /// Lowercase English letters
        /// </summary>
        LowercaseEnglishLetters = 1,

        /// <summary>
        /// No Erab in Persian letters
        /// </summary>
        NoErabPersianLetters = 2,

        /// <summary>
        /// No duplicate entries
        /// </summary>
        NoDuplicatesEntries = 4,

        /// <summary>
        /// Sort entries
        /// </summary>
        SortBasedOnEnglishLetters = 8,
    }

    /// <summary>
    /// Generic methods used by other classes of this library.
    /// </summary>
    public static class PinglishConverterUtils
    {
        /// <summary>
        /// Merges the two PinglishString lists. 
        /// <returns>A reference to the merged list.</returns>
        /// </summary>
        public static List<PinglishString> MergePinglishStringLists(List<PinglishString> list1, List<PinglishString> list2,
            PinglishStringNormalizationOptions options)
        {
            if (list1 == null)
            {
                throw new ArgumentNullException("list1");
            }

            if (list2 == null)
            {
                throw new ArgumentNullException("list2");
            }

            // TODO: Implement removeErab
            bool removeErab = options.Has(PinglishStringNormalizationOptions.NoErabPersianLetters);
            bool noDuplicates = options.Has(PinglishStringNormalizationOptions.NoDuplicatesEntries);
            bool lowerCase = options.Has(PinglishStringNormalizationOptions.LowercaseEnglishLetters);
            bool sort = options.Has(PinglishStringNormalizationOptions.SortBasedOnEnglishLetters);

            var lstResult = noDuplicates ? list1.RemoveDuplicates() : new List<PinglishString>(list1);

            if (lowerCase)
                lstResult = lstResult.ToLower();

            // Now, lstResult is modified with noDuplicates and lowerCase options
            // Time to add list2 items

            PinglishString ps;
            for (int i = 0; i < list2.Count; ++i)
            {
                ps = list2[i];
                if (lowerCase)
                    ps = ps.ToLower();

                if (noDuplicates)
                {
                    if (!lstResult.Contains(ps))
                        lstResult.Add(ps);
                }
                else
                {
                    lstResult.Add(ps);
                }
            }

            if (sort)
            {
                lstResult.Sort(new PinglishStringEqualityComparer());
            }

            return lstResult;
        }

        public static List<PinglishString> MergePinglishStringLists(List<List<PinglishString>> lists, PinglishStringNormalizationOptions options)
        {
            var result = new List<PinglishString>();
            if (lists.Count < 1)
                return result;

            for (int i = 0; i < lists.Count; i++)
            {
                result = MergePinglishStringLists(result, lists[i], options);
            }

            return result;
        }

        /// <summary>
        /// Loads a serialized list of PinglishString from a file.
        /// Note: May throws Exception
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<PinglishString> LoadPinglishStrings(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<PinglishString>));
            TextReader stream = null;
            try
            {
                stream = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read), Encoding.UTF8);
                List<PinglishString> list = serializer.Deserialize(stream) as List<PinglishString> ?? new List<PinglishString>();
                return list;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Serialize a list of PinglishString into the given file.
        /// </summary>
        /// <returns>True if the operation was successful, and false otherwise.</returns>
        public static bool SavePinglishStrings(List<PinglishString> list, string targetFile)
        {
            TextWriter stream = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<PinglishString>));
                stream = new StreamWriter(File.Open(targetFile, FileMode.Create), Encoding.UTF8);
                serializer.Serialize(stream, list);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static List<PinglishString> NormalizePinglishStrings(List<PinglishString> list, 
            PinglishStringNormalizationOptions options)
        {
            if (options == PinglishStringNormalizationOptions.None)
                return list;

            var results = list;
            if ((options & PinglishStringNormalizationOptions.LowercaseEnglishLetters)
                == PinglishStringNormalizationOptions.LowercaseEnglishLetters)
            {
                results = results.ToLower();
            }

            if ((options & PinglishStringNormalizationOptions.NoDuplicatesEntries)
                == PinglishStringNormalizationOptions.NoDuplicatesEntries)
            {
                results = results.RemoveDuplicates();
            }

            if ((options & PinglishStringNormalizationOptions.NoErabPersianLetters)
                == PinglishStringNormalizationOptions.NoErabPersianLetters)
            {
                throw new NotImplementedException();
            }

            return results;
        }

        /// <summary>
        /// Each row of a preprocess file has more that 1 column, each column is separated by these characters.
        /// </summary>
        private static readonly char[] PreprocessElementInfoSeparators = new char[] { ',', ' ', '\t', ';' };

        /// <summary>
        /// Loads Pinglish preprocess elements from a file.
        /// </summary>
        public static List<PreprocessElementInfo> LoadPreprocessElements(string filePath)
        {
            string[] elementWithInfos;

            var preprocessElementInfos = new List<PreprocessElementInfo>();

            PreprocessElementInfo elementInfo;

            if (File.Exists(filePath))
            {
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(filePath);
                    string line;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#")) // It's for comments
                            continue;

                        elementWithInfos = line.Split(PreprocessElementInfoSeparators, StringSplitOptions.RemoveEmptyEntries);
                        if (elementWithInfos.Length <= 1)
                            continue;

                        string pinglishStr = elementWithInfos[0];
                        bool isWholeWord = false;
                        bool isExactWord = false;
                        var position = TokenPosition.None;
                        var equivalents = new List<string>();
                        
                        for (int i = 1; i < elementWithInfos.Length; i++)
                        {
                            if (!elementWithInfos[i].StartsWith("#"))
                            {
                                equivalents.Add(elementWithInfos[i]);
                            }
                            else
                            {
                                string info = elementWithInfos[i].TrimStart('#');
                                switch (info.ToLower())
                                {
                                    case "start":
                                        position = position.Set(TokenPosition.StartOfWord);
                                        break;
                                    case "middle":
                                        position = position.Set(TokenPosition.MiddleOfWord);
                                        break;
                                    case "end":
                                        position = position.Set(TokenPosition.EndOfWord);
                                        break;
                                    case "wholeword":
                                        isWholeWord = true;
                                        break;
                                    case "exact":
                                        isExactWord = true;
                                        break;
                                }
                            }
                        }
                        if (position == TokenPosition.None || isWholeWord)
                            position = TokenPosition.Any;

                        elementInfo = new PreprocessElementInfo(pinglishStr, isWholeWord, isExactWord, position);
                        elementInfo.Equivalents.AddRange(equivalents);
                        preprocessElementInfos.Add(elementInfo);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }

            return preprocessElementInfos;
        }
    }
}

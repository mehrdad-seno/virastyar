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
using System.Xml;
using System.Xml.Serialization;

namespace SCICT.Utility
{
    public class Entry<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public Entry()
        {
        }

        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class DictionarySerializer<TKey, TValue>
    {
        public static void SaveToXMLFile(string fileName, Dictionary<TKey, TValue> dictionary)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName ))
            {
                Serialize(writer, dictionary);
            }
        }

        public static Dictionary<TKey, TValue> LoadFromXMLFile(string fileName)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey,TValue>();
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                Deserialize(reader, dictionary);
            }
            return dictionary;
        }

        public static void Serialize(XmlWriter writer, Dictionary<TKey, TValue> dictionary)
        {
            List<Entry<TKey, TValue>> entries = new List<Entry<TKey, TValue>>(dictionary.Count);
            foreach (KeyValuePair<TKey, TValue> item in dictionary)
            {
                entries.Add(new Entry<TKey, TValue>(item.Key, item.Value));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry<TKey, TValue>>));
            serializer.Serialize(writer, entries);
        }

        public static void Deserialize(XmlReader reader, Dictionary<TKey, TValue> dictionary)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry<TKey, TValue>>));
            object o = serializer.Deserialize(reader);

            List<Entry<TKey, TValue>> entries = o as List<Entry<TKey, TValue>>;

            dictionary.Clear();
            if (entries != null)
            {
                foreach (Entry<TKey, TValue> item in entries)
                {
                    dictionary[item.Key] = item.Value;
                }
            }
        }
    }
}

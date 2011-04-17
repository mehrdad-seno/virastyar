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

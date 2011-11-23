using System.Collections.Generic;
using System.Linq;

namespace SCICT.NLP.Utility.Transliteration.PrioritySelection
{
    class WordMapping
    {
        
        private readonly LetterMapping[] _letters;

        public WordMapping(LetterMapping[] letterMapping)
        {
            _letters = letterMapping;
        }
        public WordMapping(string word)
        {
            _letters = new LetterMapping[word.Length];
            for (int i = 0; i < _letters.Length; i++)
                _letters[i] = new LetterMapping(word[i], 0);
        }

        internal string GetHash()
        {
            return _letters.Aggregate("", (current, t) => current + (t.State + "_"));
        }
        internal WordMapping[] GetNexts( KeyValuePair<string, double>[][] distribution)
        {

            var list = new List<WordMapping>();
            for (int i = 0; i < _letters.Length; i++)
            {
                if (_letters[i].HaveNext(distribution[i].Length))
                {
                    var newLetters = new LetterMapping[_letters.Length];

                    for (int j=0;j<newLetters.Length;j++)
                        newLetters[j] = new LetterMapping (_letters[j].Letter,_letters[j].State);

                    newLetters[i] = newLetters[i].GetNext(distribution[i].Length);
                    list.Add(new WordMapping(newLetters));
                }
            }

            return list.ToArray();
        }
        internal double GetProb(KeyValuePair<string, double>[][] distribution)
        {
            double p = 1;
            for (int i=0;i<_letters.Length;i++)
                p *= distribution[i][_letters[i].State].Value;
            return p;
        }

        public string ToString(KeyValuePair<string, double>[][] distribution)
        {
            string result = "";
            for (int i = 0; i < _letters.Length; i++)
                result += distribution[i][_letters[i].State].Key;
            return result;
        }
    }
}

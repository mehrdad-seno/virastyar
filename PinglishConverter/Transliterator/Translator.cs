using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SCICT.NLP.Utility.Transliteration
{
    class Translator
    {
        private Dictionary<string, List<string>> _dictioraty;
        private bool _translatingAbility;

        public Translator(string dictionaryPath)
        {
            LoadDictionary(dictionaryPath);
            _translatingAbility = true;
        }
        public Translator()
        {
            _translatingAbility = false;
        }

        public void ActiveTranslating(string dictionaryPath)
        {
            LoadDictionary(dictionaryPath);
            _translatingAbility = true;
        }
        public void DeActiveTranslating()
        {
            _translatingAbility = false;
            _dictioraty = null;
        }

        public List<ResultWord> Translate (ResultWord word)
        {
            if (!_translatingAbility)
                return new List<ResultWord>();

            IList<ResultWord> resultWords = new List<ResultWord>();
            string newWord = word.Word;
            
                

            newWord = newWord.ToLower();
            if (_translatingAbility && _dictioraty.Keys.Contains(newWord))
            {
                List<string> dicList= _dictioraty[newWord];
                int max = Math.Max(PinglishConverterConfig.MaxTranslationCount, dicList.Count);

                for (int i=0;i<max;i++)
                {
                    resultWords.Add(new ResultWord(dicList[i],ResultType.Translate|word.Type,1.0-i*0.1 , true));
                }
            }
            return null;
        }

        private void LoadDictionary(string path)
        {
            _dictioraty = new Dictionary<string, List<string>>();
            var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                List<string> words = new List<string>(Tools.GetTokens(line));

                string word = words[0];
                words.Remove(word);
                _dictioraty.Add(words[0], words);
            }
            reader.Close();
            reader.Dispose();
        }
        public ResultWord AcronymeCovert(string acronymeWrod)
        {
            string result = "";
            result = Tools.MapAccronymToString(acronymeWrod);
            return (new ResultWord(result, ResultType.AcronymeConvert, 1.0, true));
        }
    }

}

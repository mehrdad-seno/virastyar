using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SCICT.NLP.Utility.Transliteration.KNN;

namespace SCICT.NLP.Utility.Transliteration
{
    public class PinglishConverter
    {
        private Dictionary<string, List<string>> _exceptionsC;  //this present Casensitive exceptionss
        private Dictionary<string, List<string>> _exceptionsNC; //this present Not Casensitive exceptions
        private readonly PreProcessor _preProcessor;            //this present Preprocessor Object
        private readonly Translator _translator;                //this present Translator Object
        private readonly ITransliterator _transliterater;       //this present Transliterator Object
        public PinglishConverter(PinglishConverterConfig config, PruneType pruneType):this(config,pruneType,false)
        {

        }

        public PinglishConverter(PinglishConverterConfig config, PruneType pruneType, bool supportTranslating)
        {
            if (supportTranslating)
            {
                throw new NotImplementedException("Currently not supported");
            }

            ReadExceptionWords(config.ExceptionWordDicPath);
            _preProcessor = new PreProcessor();

            // TODO: Translation is not supported for now
            _translator = supportTranslating ? new Translator(config.EnToPeDicPath) : new Translator();

            if (PruneType.Stem == pruneType)
            {
                _transliterater = new PinglishMapping(config.XmlDataPath, config.StemDicPath, pruneType);
            }
            else
            {
                _transliterater = new PinglishMapping(config.XmlDataPath, config.GoftariDicPath, pruneType);
            }

        }

        public string[] Convert(string enWord, bool justFirst)
        {
            //check this is exception
            if (_exceptionsC.Keys.Contains(enWord))
                return _exceptionsC[enWord].ToArray();

            if (_exceptionsNC.Keys.Contains(enWord.ToLower()))
                return _exceptionsNC[enWord.ToLower()].ToArray();


            //preprocess word
            List<ResultWord> preProccessedWords = _preProcessor.PreProccess(enWord);

            if (Tools.IsUpperCase(enWord))
                preProccessedWords.Add(_translator.AcronymeCovert(enWord));

            //process traslate and Transliterates
            var wordsPool = new List<ResultWord>();
            foreach (ResultWord preProccessedWord in preProccessedWords)
            {
                if (preProccessedWord.IsFinal)
                    wordsPool.Add(preProccessedWord);
                else
                {
                    var temp = new List<ResultWord>();
                    if (Tools.IsLowwerCase(preProccessedWord.Word))
                        temp = _transliterater.SuggestFarsiWords(preProccessedWord, justFirst);
                    else
                    {
                        List<string> midre = Tools.PartString(preProccessedWord.Word);
                        string result = "";
                        foreach (string str in midre)
                        {
                            if (Tools.IsLowwerCase(str))
                            {
                                List<ResultWord> temp1 = _transliterater.SuggestFarsiWords(new ResultWord(str), true);
                                if (temp1 != null)
                                    if (temp1.Count != 0)
                                        result += temp1[0].Word;
                            }
                            else
                            {
                                result += str;
                            }
                        }
                        temp.Add(new ResultWord(result, ResultType.Transliterate, 1.0, true));
                    }

                    if (temp != null)
                    {
                        wordsPool = new List<ResultWord>(wordsPool.Union(temp));
                    }
                    if (_translator != null)
                    {
                        wordsPool = wordsPool.Union(_translator.Translate(preProccessedWord)).ToList();
                    }
                }
            }

            //add main word
            // wordsPool.Add(new ResultWord(enWord , ResultType.NoChange , 0.8 ));

            //sort results
            List<string> finalResults = SortResults(wordsPool);
            List<string> tempList = new List<string>();
            tempList.Add(enWord);
            finalResults.InsertRange(Math.Min(6, finalResults.Count), tempList);
            return finalResults.ToArray();
        }

        private List<string> SortResults(List<ResultWord> words)
        {

            var result = new List<string>();

            //add ACRONYME
            AddResut(words, ref result, (ResultType.AcronymeConvert), 0);
            //Add BEST HITED TRASLITERATES
            AddResut(words, ref result, (ResultType.Transliterate | ResultType.HittedToDic), 0, 0.08);
            //ADD TRANSLATED 
            AddResut(words, ref result, (ResultType.Translate), 0);
            //Add other HITED TRASLITERATES
            AddResut(words, ref result, (ResultType.Transliterate | ResultType.HittedToDic), 0, 0.08, false);
            ////Add all NOT-HITED TRASLITERATES
            AddResut(words, ref result, ResultType.Transliterate, ResultType.HittedToDic);

            return result;
        }

        #region RareUseFunctions
        private void ReadExceptionWords(string path)
        {
            _exceptionsC = new Dictionary<string, List<string>>();
            _exceptionsNC = new Dictionary<string, List<string>>();
            var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                var words = new List<string>(Tools.GetTokens(line));
                if (words.Count == 0)
                    continue;

                bool casen = words[0].Equals(("#Casensitive")) ? true : false;

                string word = words[1];
                words.RemoveAt(1);
                words.RemoveAt(0);

                if (casen)
                {
                    if (!_exceptionsC.ContainsKey(word))
                    {
                        _exceptionsC.Add(word, words);
                    }
                    else
                    {
                        var oldWords = _exceptionsC[word];
                        oldWords.AddRange(words);
                        _exceptionsC[word] = oldWords.Distinct().ToList();
                    }
                }
                else
                {
                    if (!_exceptionsNC.ContainsKey(word.ToLower()))
                    {
                        _exceptionsNC.Add(word.ToLower(), words);
                    }
                    else
                    {
                        var oldWords = _exceptionsC[word.ToLower()];
                        oldWords.AddRange(words);
                        _exceptionsC[word] = oldWords.Distinct().ToList();
                    }
                }
            }
            reader.Close();
            reader.Dispose();
        }

        private void AddResut(List<ResultWord> source, ref List<string> dist, ResultType mustHave, ResultType mustDontHave)
        {
            AddResut(source, ref dist, mustHave, mustDontHave, 0, true);
        
        }
        private void AddResut(List<ResultWord> source, ref List<string> dist, ResultType mustHave, ResultType mustDontHave, double treshhold )
        {
            AddResut(source, ref dist, mustHave, mustDontHave, treshhold, true);
        }

        private void AddResut(List<ResultWord> source, ref List<string> dist, ResultType mustHave, ResultType mustDontHave, double treshhold , bool ascending )
        {
            var tempList = new List<ResultWord>();
            foreach (ResultWord word in source)
            {
                if ((word.Type & (mustHave | mustDontHave)) == (mustHave))
                {
                    if (ascending && word.Probability > treshhold)
                        tempList.Add(word);
                    else if ((!ascending) && (word.Probability < treshhold))
                        tempList.Add(word);
                }
            }

            tempList.Sort((t, k) => (Math.Sign(k.Probability - t.Probability)));
            foreach (ResultWord resultWord in tempList)
                dist.Add(resultWord.Word);
        }

        #endregion
    }
}

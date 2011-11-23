using System.Collections.Generic;
using System.Linq;

namespace SCICT.NLP.Utility.Transliteration
{
    class PreProcessor
    {
        public List<ResultWord> PreProccess(string word)
        {
            var resuList = new List<ResultWord> { new ResultWord(word, ResultType.NoChange, 1.0) };
            resuList = HandleAt(resuList);
            resuList = HandleNumber(resuList);
            resuList = HandleCase(resuList);
            resuList . Add(new ResultWord(word,ResultType.NoChange , 1.0,true));
            resuList = DeleteRepeated(resuList);
            return resuList;
        }

        private static List<ResultWord> HandleCase(IEnumerable<ResultWord> resuList)
        {
            var resultWords = new List<ResultWord>();
            foreach (ResultWord word in resuList)
            {
                if (word.IsFinal)
                    resultWords.Add(word);
                else if (!Tools.HaveUpperCase(word.Word))
                    resultWords.Add(word);
                else 
                {
                    if (Tools.IsUpperCase(word.Word))
                        resultWords.Add(new ResultWord(Tools.MapAccronymToString(word.Word),
                                                   ResultType.AcronymeConvert|word.Type, 1.0, true));
               
                    resultWords.Add(new ResultWord(word.Word.ToLower(), word.Type|ResultType.UpperCaseTolower, 1.0));
                    resultWords.Add(new ResultWord(Tools.MapUpperCaseStringenToString(word.Word), word.Type|ResultType.UpperCaseToSpell, 1.0));
                }

            }
            return resultWords;
        }

        private static List<ResultWord> DeleteRepeated(IEnumerable<ResultWord> list)
        {
            var resultList = new List<ResultWord>();
            foreach (ResultWord resultWord in list)
            {
                if (resultWord.IsFinal)
                {
                    resultList.Add(resultWord);
                    continue;
                }

                string word = resultWord.Word;
                string result = "";
                for (int i = 0; i < word.Length; i++)
                {
                    if (i == 0 || i == 1)
                        result += word[i];
                    else if (word[i] != word[i - 1] || word[i] != word[i - 2])
                        result += word[i];
                }
                resultList.Add(new ResultWord(result , resultWord.Type , resultWord.Probability , resultWord.IsFinal));
            }
            return resultList;
        }

        private static List<ResultWord> HandleNumber(IEnumerable<ResultWord> words)
        {
            var result = new List<ResultWord>();
            foreach (ResultWord wordResults in words)
            {
                string word = wordResults.Word;
                for (int i = 0; i < word.Length; i++)
                    HandleNumberAtChar(ref word, i , true);
                if (word!= wordResults.Word)
                    result.Add(new ResultWord(word , wordResults.Type|ResultType.NumberConvert , 0.08));

                word = wordResults.Word;
                for (int i = 0; i < word.Length; i++)
                    HandleNumberAtChar(ref word, i, false);
                if (word != wordResults.Word)
                    result.Add(new ResultWord(word, wordResults.Type | ResultType.NumberConvert, 1.0));
                else
                {
                    result.Add(wordResults);
                }
            }
            return result;
        }

        private static void HandleNumberAtChar(ref string word, int i ,bool englishPerefer)
        {
            if (Tools.IsNumber(word[i]))
            {
                if (i == word.Length - 1)
                {
                    word = word.Substring(0, i) + Tools.MapNumberToString(word[i].ToString(), englishPerefer);

                }
                else
                {
                    if (Tools.IsNumber(word[i + 1]))
                    {
                        if (word.Substring(i, 2) == "10" || word.Substring(i, 2) == "30" || word.Substring(i, 2) == "40")
                        {
                            word = word.Substring(0, i) + Tools.MapNumberToString(word.Substring(i, 2), englishPerefer) + word.Substring(i + 2, word.Length - i - 2);
                        }
                        else
                        {
                            word = word.Substring(0, i) + Tools.MapNumberToString(word[i].ToString(), englishPerefer) + word.Substring(i + 1, word.Length - i - 1);
                        }
                    }
                    else
                    {
                        word = word.Substring(0, i) + Tools.MapNumberToString(word[i].ToString(), englishPerefer) + word.Substring(i + 1, word.Length - i - 1);

                    }
                }
            }
        }

        private static List<ResultWord> HandleAt(IEnumerable<ResultWord> words)
        {
            return words.Select(word => word.Word.Contains("@") ? new ResultWord(word.Word.Replace("@", "at"), ResultType.AtConvert | word.Type, word.Probability) : word).ToList();
        }
    }
}

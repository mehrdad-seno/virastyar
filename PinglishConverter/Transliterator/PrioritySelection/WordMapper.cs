using System.Collections.Generic;
using SCICT.NLP.Morphology.Inflection;
using SCICT.NLP.Utility;

namespace SCICT.NLP.Utility.Transliteration.PrioritySelection
{
    class WordMapper
    {
        
        private readonly PersianSuffixLemmatizer _suffixer;
        private readonly PruneType _pruneType;
        private readonly List<string> _dictionary;
        public WordMapper(List<string> dictionary , PruneType pruneType) 
        {
            if (dictionary == null) _dictionary = new List<string>();
            _dictionary = dictionary;

            _pruneType = pruneType;
            _suffixer = new PersianSuffixLemmatizer(true ,false);
        }

        public List<ResultWord> SelectBests(
            ResultWord enWord,
            KeyValuePair<string, double>[][] sortedDistribution 
            )
        {
            return SelectBests(enWord,sortedDistribution,false,PinglishConverterConfig.ThreshHoldForSearchingProb,PinglishConverterConfig.ThreshHoldForSearchingCounter);
        }
        public List<ResultWord> SelectBests(
            ResultWord enWord,
            KeyValuePair<string, double>[][] sortedDistribution,
            bool justFirst
            )
        {
            return SelectBests(enWord, sortedDistribution, justFirst, PinglishConverterConfig.ThreshHoldForSearchingProb, PinglishConverterConfig.ThreshHoldForSearchingCounter);
        }

        public List<ResultWord> SelectBests(
            ResultWord enWord,
            KeyValuePair<string, double>[][] sortedDistribution , 
            bool justFirst  , 
            double probeTreshHold,
            int counterTreshHold 
            )
        {
            bool firstSeen = false;
            if (enWord.Word == null) 
                return new List<ResultWord>();

            var queue = new PriorityQueue<double, WordMapping>();
            var beforSeen = new HashSet<string>();

            var currentWordMapping = new WordMapping(enWord.Word);
            beforSeen.Add(currentWordMapping.GetHash());

            var list  = new List<ResultWord>();
            var watchList = new List<string>();
           
            int counter =1;

            while (currentWordMapping.GetProb(sortedDistribution) > probeTreshHold && counter < counterTreshHold)
            {
                string currentWord = Tools.NormalizeString(currentWordMapping.ToString(sortedDistribution));
                if ((currentWord != null) && (!watchList.Contains(currentWord)))
                {
                    watchList.Add(currentWord);

                    bool hitted = Tools.IsValidInDictionary(currentWord,_dictionary,_suffixer ,_pruneType);
                    if (hitted)
                    {
                        var newResultWord = new ResultWord(currentWord,
                                                           ((ResultType.Transliterate | ResultType.HittedToDic) |enWord.Type),
                                                           currentWordMapping.GetProb(sortedDistribution), true);
                        list.Add(newResultWord);

                        if (justFirst)
                            return list;
                    
                    }
                    else
                    {
                        if ((!justFirst) ||(!firstSeen))
                        list.Add(new ResultWord(currentWord,
                                            ((ResultType.Transliterate) |enWord.Type),
                                            currentWordMapping.GetProb(sortedDistribution), true));
                        firstSeen = true;
                    }
                    
                }
                WordMapping[] wordMappings = currentWordMapping.GetNexts(sortedDistribution);
                foreach (WordMapping wordMapping in wordMappings)
                    if (!beforSeen.Contains(wordMapping.GetHash()))
                    {
                        queue.Add(new KeyValuePair<double, WordMapping>(1 - wordMapping.GetProb(sortedDistribution),
                                                                        wordMapping));
                        beforSeen.Add(wordMapping.GetHash());
                    }
                if (queue.Count == 0)
                    break;
                currentWordMapping = queue.Dequeue().Value;
                counter++;
            }

            return list;
        }

    }
}

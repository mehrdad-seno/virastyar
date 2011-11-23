using System.Collections.Generic;

namespace SCICT.NLP.Utility.Transliteration
{
    public interface ITransliterator
    {
        List<ResultWord> SuggestFarsiWords(ResultWord pinglishWord , bool firstWords);
    }
}

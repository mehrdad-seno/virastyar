using System;

namespace SCICT.NLP.Utility.Transliteration
{
    public enum PruneType
    {
        FullKeyword,
        Stem,
        NoPrune
    }

    public enum TransliterateMethod
    {
        NeuralNetworkMethod,
        SimpleDistributionMethod,
        KNearestNeightborMethod
    }

    [Flags]
    public enum ResultType
    {
        NoChange = 0,
        AcronymeConvert = 2,
        Translate = 4,
        Transliterate = 8,
        UpperCaseTolower = 16,
        UpperCaseToSpell = 32,
        HittedToDic = 64,
        NumberConvert = 128,
        AtConvert = 256,
    }


}

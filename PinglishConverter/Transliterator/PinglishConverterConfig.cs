namespace SCICT.NLP.Utility.Transliteration
{
    public class PinglishConverterConfig
    {
        public const int MaxTranslationCount = 3;
        public const double ThreshHoldForSearchingProb = .00005;
        public const int ThreshHoldForSearchingCounter = 50;

        public const int PowerFactor = 26;

        public string ExceptionWordDicPath { get; set; }
        public string GoftariDicPath { get; set; }

        public string StemDicPath { get; set; }
        public string XmlDataPath { get; set; }
        public string EnToPeDicPath { get; set; }
    }
}

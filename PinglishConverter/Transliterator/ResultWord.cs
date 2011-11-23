namespace SCICT.NLP.Utility.Transliteration
{
    public class ResultWord
    {
        public string Word { get; private set; }
        public ResultType Type { get; private set; }
        public double Probability { get; set; }
        public bool IsFinal { get; set; }
        public ResultWord(string word) :this( word ,  ResultType.NoChange , 1.0, false){}
        public ResultWord(string word, ResultType type ):this(word, type, 1.0, false){}
        public ResultWord(string word, ResultType type, double prob)
            : this(word, type, prob, false)
        {
        }
        public ResultWord(string word ,ResultType type , double prob  , bool isfianal)
        {
            Word = word;
            Type = type;
            Probability = prob;
            IsFinal = isfianal;
        }

       
    }
}

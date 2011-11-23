using VirastyarWordAddin.Verifiers.Basics;
using SCICT.NLP.Utility.Parsers;

namespace VirastyarWordAddin.Verifiers.CustomGUIs.DateSuggestions
{
    public class DateSuggestion : ISuggestions
    {
        public string Message { get; set; }

        public string DefaultSuggestion
        {
            get 
            { 
                // TODO: implement sth 
                return ""; 
            }
        }

        public IPatternInfo MainPattern { get; set; }


    }
}

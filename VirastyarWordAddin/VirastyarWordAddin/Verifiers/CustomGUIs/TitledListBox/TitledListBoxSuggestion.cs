using System;
using System.Collections.Generic;
using VirastyarWordAddin.Verifiers.Basics;

namespace VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox
{
    public class TitledListBoxSuggestion : ISuggestions
    {
        public string Message { get; set; }

        public string DefaultSuggestion
        {
            get
            {
                if(SuggestionItems == null)
                    return null;

                foreach (var sug in SuggestionItems)
                {
                    return sug;
                }
                return null;
            }
        }

        public IEnumerable<string> SuggestionItems { get; set; }
    }
}

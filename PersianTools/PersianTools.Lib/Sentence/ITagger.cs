using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Utility
{
    public interface ITagger
    {
        string[] SupportedTagNames { get; }
        string Locale { get; }

        Dictionary<string, object[]> Tag(Sentence sentence);
    }
}



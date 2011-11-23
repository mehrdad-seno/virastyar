using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public class VerificationResult
    {
        public string SelectedSuggestion { get; set; }
        public UserSelectedActions UserAction { get; set; }
        public object ViewerControlArg { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Utility
{
    public class SentenceToken : TokenInfo
    {
        public SentenceToken(TokenInfo token, Sentence parent, int tokenIndex)
            : this(token.Value, token.Index, parent, tokenIndex)
        {
            
        }

        public SentenceToken(string token, int index, Sentence parent, int tokenIndex)
            : base(token, index)
        {
            this.Sentence = parent;
            this.TokenIndex = tokenIndex;
        }

        public Sentence Sentence { get; private set; }

        public int TokenIndex { get; private set; }
    }
}

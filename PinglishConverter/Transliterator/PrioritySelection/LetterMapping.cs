using System;

namespace SCICT.NLP.Utility.Transliteration.PrioritySelection
{
    class LetterMapping
    {
        public char Letter { get; private set; }
        public int State { get; private set; }

        public LetterMapping(char letter, int state)
        {
            Letter = letter;
            State = state;
        }

        public bool HaveNext (int states)
        {
             return State != (states-1); 
        }
        public LetterMapping GetNext(int states)
        {
            if (!HaveNext( states))
            {
               new Exception("this dont have next!");
               return null;
            }
            return  new LetterMapping(Letter, State + 1);
        }
    }
}

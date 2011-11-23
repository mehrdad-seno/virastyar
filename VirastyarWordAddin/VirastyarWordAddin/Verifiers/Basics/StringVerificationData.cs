namespace VirastyarWordAddin.Verifiers.Basics
{
    public class StringVerificationData
    {
        public StringVerificationData()
        {
        }

        public StringVerificationData(StringVerificationData copy)
        {
            SetContent(copy);
        }

        public void SetContent(StringVerificationData copy)
        {
            ErrorIndex = copy.ErrorIndex;
            ErrorLength = copy.ErrorLength;
            ErrorType = copy.ErrorType;
            Suggestions = copy.Suggestions;
        }

        public int ErrorIndex { get; set; }
        public int ErrorLength { get; set; }
        public int ErrorEnd 
        { 
            get { return ErrorIndex + ErrorLength - 1; }
        }

        public VerificationTypes ErrorType { get; set; }
        public ISuggestions Suggestions { get; set; }

        public virtual bool IsValid
        {
            get { return ErrorIndex >= 0 && ErrorLength > 0; }
        }
    }
}

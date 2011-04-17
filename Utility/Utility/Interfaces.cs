namespace SCICT.Utility.IO
{
    ///<summary>
    /// Dictionary of words with usage frequency intreface
    ///</summary>
    public interface IDictionaryWordFreqLoader
    {
        bool NextTerm(out string word, out int freq);
        bool AddTerm(string word, int freq);
    }

    ///<summary>
    /// Dictionary of words with usage frequency intreface
    ///</summary>
    public interface IDictionaryWordFreqPOSLoader
    {
        bool NextTerm(out string word, out int freq, out string pos);
        bool AddTerm(string word, int freq, string pos );
    }

}

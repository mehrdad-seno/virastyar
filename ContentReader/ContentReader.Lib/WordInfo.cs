namespace SCICT.Microsoft.Office.Word.ContentReader.Shared
{
    /// <summary>
    /// WordInfo Class that encapsualtes some information about words, to be used for statiscal purposes.
    /// This class has no usage in the whole ContentReader Library, but since the clients
    /// (i.e. PersianContentReader.Console, and PersianContentReader.UI projects) use this 
    /// class extensively it is placed in the PeresianContentReader.Lib project, so that 
    /// it is shared with the clients also.
    /// </summary>
    public class WordInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordInfo"/> class.
        /// </summary>
        public WordInfo() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WordInfo"/> class.
        /// </summary>
        /// <param name="noSpace">word without space.</param>
        /// <param name="noErab">The word without erab.</param>
        /// <param name="count">The Count of the word.</param>
        public WordInfo(string noSpace, string noErab, int count)
        {
            WordNoSpace = noSpace;
            WordNoErab = noErab;
            this.Count = count;
        }

        /// <summary>
        /// The word without spaces
        /// </summary>
        public string WordNoSpace;
        
        /// <summary>
        /// The word without erabs
        /// </summary>
        public string WordNoErab;

        /// <summary>
        /// The Count of the words
        /// </summary>
        public int Count;
    }
}

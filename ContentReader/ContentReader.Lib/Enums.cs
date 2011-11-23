namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// Enumerates different locations where a block (i.e. IBlock) can exist.
    /// The values are based upon Range.StoryType. 
    /// </summary>
    public enum StoryType
    {
        /// <summary>
        /// Illegal story type
        /// </summary>
        Illegal = -1,
        /// <summary>
        /// unknown story type
        /// </summary>
        Other,
        /// <summary>
        /// comments
        /// </summary>
        CommentsStory,
        /// <summary>
        /// end-notes
        /// </summary>
        EndnotesStory,
        /// <summary>
        /// even pages footer
        /// </summary>
        EvenPagesFooterStory,
        /// <summary>
        /// even pages header
        /// </summary>
        EvenPagesHeaderStory,
        /// <summary>
        /// first page footer
        /// </summary>
        FirstPageFooterStory,
        /// <summary>
        /// first page header
        /// </summary>
        FirstPageHeaderStory,
        /// <summary>
        /// footnotes
        /// </summary>
        FootnotesStory,
        /// <summary>
        /// main text
        /// </summary>
        MainTextStory,
        /// <summary>
        /// primary footer
        /// </summary>
        PrimaryFooterStory,
        /// <summary>
        /// primary header
        /// </summary>
        PrimaryHeaderStory,
        /// <summary>
        /// text frame
        /// </summary>
        TextFrameStory
    }

    /// <summary>
    /// Enumerates different kind of blocks (i.e. IBlock).
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// illegal block type
        /// </summary>
        Illegal = -1,
        /// <summary>
        /// word
        /// </summary>
        Word,
        /// <summary>
        /// sentence
        /// </summary>
        Sentence,
        /// <summary>
        /// paragraph
        /// </summary>
        Paragraph,
        /// <summary>
        /// the whole document content
        /// </summary>
        Everything
    }

    /// <summary>
    /// Enumerates different kind of documents.
    /// At the time of this documentation this Enum is not used.
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// illegal document type
        /// </summary>
        Illegal = -1,
        /// <summary>
        /// an MS-Word .doc or .docx file
        /// </summary>
        Doc,
        /// <summary>
        /// HTML file
        /// </summary>
        HTML,
        /// <summary>
        /// Plain text file
        /// </summary>
        PlainText,
        /// <summary>
        /// a .rtf file
        /// </summary>
        RTF
    }

    /// <summary>
    /// Enumerates a word type according to its characters.
    /// This enum is not used in the library, but has usage in the client projects.
    /// </summary>
    public enum WordType
    {
        /// <summary>
        /// illegal word type
        /// </summary>
        Illegal,
        /// <summary>
        /// all spaces
        /// </summary>
        Space, // Should also be considered as illegal
        /// <summary>
        /// arabic word
        /// </summary>
        ArabicWord,
        /// <summary>
        /// arabic pucntuation
        /// </summary>
        ArabicPunc,
        /// <summary>
        /// arabic number
        /// </summary>
        ArabicNum,
        /// <summary>
        /// english word
        /// </summary>
        EnglishWord,
        /// <summary>
        /// english punctuation
        /// </summary>
        EnglishPunc,
        /// <summary>
        /// english number
        /// </summary>
        EnglishNum
    }

    /// <summary>
    /// Enumerates different outcomes of a range trim operation.
    /// </summary>
    public enum TrimRangeResult
    {
        /// <summary>
        /// Range is null, or empty or full of control characters
        /// </summary>
        InvalidRange,

        /// <summary>
        /// The range is trimmed and fit successfully
        /// </summary>
        Success,

        /// <summary>
        /// The range is valid, and has valid contents; but it cannot be trimmed
        /// e.g., because of hyperlinks.
        /// </summary>
        Failure,

        /// <summary>
        /// The range-text equals the desired trimmed text. But the length of the
        /// range is larger than the length of the text.
        /// i.e., the range contains the text, together with some meta-data
        /// </summary>
        TrimmedButLengthMismatch
    }


}

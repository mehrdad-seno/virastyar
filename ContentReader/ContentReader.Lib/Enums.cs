// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

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
}

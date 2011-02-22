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

using System.Collections.Generic;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// The parent interface for the objects carrying MS-Word document content.
    /// </summary>
    public abstract class IBlock
    {
        /// <summary>
        /// Reference to the document owning this block
        /// </summary>
        private readonly IDocument m_parentDocument = null;

        /// <summary>
        /// Gets a reference to the document owning this block
        /// </summary>
        public IDocument ParentDocument
        {
            get { return m_parentDocument; }
            //set { m_parentDocument = value; }
        }

        /// <summary>
        /// Protected Constructor, sets the reference to the parent document object.
        /// </summary>
        /// <param name="parentDocument"></param>
        protected IBlock(IDocument parentDocument)
        {
            this.m_parentDocument = parentDocument;
        }

        /// <summary>
        /// The main content of the Block.
        /// </summary>
        public abstract string Content { get; }

        /// <summary>
        /// The type of the block, whether it is Paragraph, Sentence, Word, and so on.
        /// </summary>
        public abstract BlockType BlockType
        {
            get;
            //protected set;
        }

        /// <summary>
        /// Where this block is located. It is based upon Range.StoryType
        /// StoryTypes could be FootNote, EndNote, MainText, and so on
        /// </summary>
        public abstract StoryType StoryType
        {
            get;
            //protected set;
        }

        /// <summary>
        /// Sequence of paragraphs within this block
        /// </summary>
        public abstract IEnumerable<IBlock> Paragraphs
        {
            get;
        }

        /// <summary>
        /// Sequence of sentences within this block
        /// </summary>
        public abstract IEnumerable<IBlock> Sentences
        {
            get;
        }

        /// <summary>
        /// Sequence of words within this block
        /// </summary>
        public abstract IEnumerable<IBlock> Words
        {
            get;
        }
    }
}

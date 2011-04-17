using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Utility;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// Any kind of contents of a MS-Word Document.
    /// </summary>
    public class MSWordBlock : IBlock
    {
        #region Private Fields

        /// <summary>
        /// Reference to the Range object from Microsoft Word Object Model, corresponding
        /// to this MSWordBlock instance
        /// </summary>
        private Range range = null;

        /// <summary>
        /// The type of the Block (e.g. Word, Sentence, Paragraph, ...)
        /// </summary>
        private BlockType blockType = BlockType.Illegal;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MSWordBlock"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="range">The range.</param>
        /// <param name="blockType">Type of the block.</param>
        public MSWordBlock(IDocument parent, Range range, BlockType blockType) : base(parent)
        {
            // base(parent) sets the reference to the parent document, the code is provided in the
            // base class ctor.

            this.range = range;
            this.blockType = blockType;
        }

        #endregion

        #region General Properties

        /// <summary>
        /// Gets a reference to the Range object from the MS-Word Object model,
        /// corresponding to this instance of MSWordBlock
        /// </summary>
        public Range Range
        {
            get { return range; }
        }

        /// <summary>
        /// Gets a filtered content of the block.
        /// For a non-filtered content of the block use the RawContent property.
        /// </summary>
        public override string Content
        {
            get 
            { 
                string str = RawContent;
                if (str == null)
                    return "";
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < str.Length; ++i)
                {
                    sb.Append(ParentDocument.FilterChar(str[i]));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the content of the block intact.
        /// For a filtered version of the content use the Content property.
        /// </summary>
        public string RawContent
        {
            get { return range.Text; }
        }


        /// <summary>
        /// Gets the type of the block (e.g. word, sentence, paragraph, ...)
        /// </summary>
        public override BlockType BlockType
        {
            get
            {
                return blockType;
            }
        }

        /// <summary>
        /// Gets the location where the block is located (e.g. footnote, end-note, main-story, ...)
        /// </summary>
        public override StoryType StoryType
        {
            get
            {
                switch (Range.StoryType)
                {
                    case WdStoryType.wdCommentsStory:
                        return StoryType.CommentsStory;
                    case WdStoryType.wdEndnotesStory:
                        return StoryType.EndnotesStory;
                    case WdStoryType.wdEvenPagesFooterStory:
                        return StoryType.EvenPagesFooterStory;
                    case WdStoryType.wdEvenPagesHeaderStory:
                        return StoryType.EvenPagesHeaderStory;
                    case WdStoryType.wdFirstPageFooterStory:
                        return StoryType.FirstPageFooterStory;
                    case WdStoryType.wdFirstPageHeaderStory:
                        return StoryType.FirstPageHeaderStory;
                    case WdStoryType.wdFootnotesStory:
                        return StoryType.FootnotesStory;
                    case WdStoryType.wdMainTextStory:
                        return StoryType.MainTextStory;
                    case WdStoryType.wdPrimaryFooterStory:
                        return StoryType.PrimaryFooterStory;
                    case WdStoryType.wdPrimaryHeaderStory:
                        return StoryType.PrimaryHeaderStory;
                    case WdStoryType.wdTextFrameStory:
                        return StoryType.TextFrameStory;
                    case WdStoryType.wdFootnoteContinuationNoticeStory:
                    case WdStoryType.wdFootnoteContinuationSeparatorStory:
                    case WdStoryType.wdFootnoteSeparatorStory:
                    case WdStoryType.wdEndnoteContinuationNoticeStory:
                    case WdStoryType.wdEndnoteContinuationSeparatorStory:
                    case WdStoryType.wdEndnoteSeparatorStory:
                    default:
                        return StoryType.Other;
                }
            }
        }

        #endregion

        #region Content Reading Properties

        /// <summary>
        /// Gets the sequence of non empty paragraphs within this block.
        /// The content is trimmed before being returned.
        /// For a sequence of intact paragraphs within this block use 
        /// the RawParagraphs property.
        /// </summary>
        public override IEnumerable<IBlock> Paragraphs
        {
            get
            {
                Range r;
                foreach (MSWordBlock block in RawParagraphs)
                {
                    r = block.Range;
                    RangeUtils.TrimRange(r);
                    if (RangeUtils.IsRangeEmpty(r))
                        continue;
                    yield return block;
                }
            }
        }

        /// <summary>
        /// Gets the sequence of non empty sentences within this block.
        /// The content is trimmed before being returned.
        /// For a sequence of intact sentences within this block use 
        /// the RawSentences property.
        /// </summary>
        public override IEnumerable<IBlock> Sentences
        {
            get
            {
                Range r;
                foreach (MSWordBlock block in RawSentences)
                {
                    r = block.Range;
                    RangeUtils.TrimRange(r);
                    if (RangeUtils.IsRangeEmpty(r))
                        continue;
                    yield return block;
                }
            }
        }

        /// <summary>
        /// Gets the sequence of non empty words within this block.
        /// The content is trimmed before being returned.
        /// For a sequence of intact words within this block use 
        /// the RawWords property.
        /// </summary>
        public override IEnumerable<IBlock> Words
        {
            get
            {
                Range r;
                foreach (MSWordBlock block in RawWords)
                {
                    r = block.Range;
                    RangeUtils.TrimRange(r);
                    if (RangeUtils.IsRangeEmpty(r))
                        continue;
                    yield return block;
                }
            }
        }

        /// <summary>
        /// Gets the sequence of paragraphs within this block as returned by
        /// Microsoft Mord Object Word Model.
        /// For a sequence of non-empty trimmed paragraphs within this block use 
        /// the Paragraphs property.
        /// </summary>
        public IEnumerable<MSWordBlock> RawParagraphs
        {
            get
            {
                return new BlockEnumerable(new BlockEnumerator(this, BlockType.Paragraph));
            }
        }

        /// <summary>
        /// Gets the sequence of sentences within this block as returned by
        /// Microsoft Mord Object Word Model.
        /// For a sequence of non-empty trimmed sentences within this block use 
        /// the Sentences property.
        /// </summary>
        public IEnumerable<MSWordBlock> RawSentences
        {
            get
            {
                return new BlockEnumerable(new BlockEnumerator(this, BlockType.Sentence));
            }
        }

        /// <summary>
        /// Gets the sequence of words within this block as returned by
        /// Microsoft Mord Object Word Model.
        /// For a sequence of non-empty trimmed words within this block use 
        /// the Words property.
        /// </summary>
        public IEnumerable<MSWordBlock> RawWords
        {
            get
            {
                return new BlockEnumerable(new BlockEnumerator(this, BlockType.Word));
            }
        }

        #endregion

        # region IN-CLASS: BlockEnumerable an IEnumerable

        /// <summary>
        /// An enumerable class, used for enumerating blocks within another block.
        /// </summary>
        protected class BlockEnumerable : IEnumerable<MSWordBlock>
        {
            BlockEnumerator etor = null;

            /// <summary>
            /// recieves an enumerator instance and returns it upon GetEnumerator method call.
            /// </summary>
            public BlockEnumerable(BlockEnumerator etor)
            {
                this.etor = etor;
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator GetEnumerator()
            {
                return etor;
            }

            IEnumerator<MSWordBlock> IEnumerable<MSWordBlock>.GetEnumerator()
            {
                return etor;
            }
        }
        #endregion

        #region IN-CLASS: BlockEnumerator an IEnumerator

        /// <summary>
        /// An enumerator class to help walk through the blocks within another block.
        /// The enumerable class that uses this enumerator is BlockEnumerable.
        /// </summary>
        protected class BlockEnumerator : IEnumerator<MSWordBlock>
        {
            #region Private Fields
            /// <summary>
            /// The block which we plan to read its content
            /// </summary>
            private MSWordBlock block;

            /// <summary>
            /// The type of reading (i.e. word-by-word, sentence-by-sentence, ...)
            /// </summary>
            private BlockType readOrderType = BlockType.Illegal;

            /// <summary>
            /// List of enumerators gained from Ranges inside the block
            /// </summary>
            private List<IEnumerator> listEtors = new List<IEnumerator>();

            /// <summary>
            /// Points to the current index of the list of enumerators namely listEtors.
            /// </summary>
            private int curListIndex = 0;

            /// <summary>
            /// Current block which is going to be returned by the Current Property
            /// </summary>
            private MSWordBlock curBlock = null;

            /// <summary>
            /// A List of visited ranges, to help check not returning some range more than once.
            /// </summary>
            private List<Range> listVisitedRanges = new List<Range>();

            /// <summary>
            /// A Comparer object used for checking duplicate ranges.
            /// For more information on the Comparer identity check algorithm 
            /// refer to the RangeEqualityComparer class.
            /// </summary>
            private RangeEqualityComparer rangeEqualityComparerObj = new RangeEqualityComparer();


            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the current block to be read
            /// </summary>
            public object Current
            {
                get { return curBlock; }
            }

            /// <summary>
            /// Gets the current block to be read
            /// </summary>
            MSWordBlock IEnumerator<MSWordBlock>.Current
            {
                get { return curBlock; }
            }

            /// <summary>
            /// Reads the next block if any, or returns false if there are no next blocks.
            /// The read block will be stored in curBlock, which consequently 
            /// will be returned by the Current method.
            /// </summary>
            public bool MoveNext()
            {
                return MoveNext(readOrderType);
            }

            /// <summary>
            /// Resets the enumerator. Referring to MSDN this method is useful for COM-Interoperability.
            /// Regular applications do not need to implement this. 
            /// But anyway the preferred implementation would be to reset the state of the enumerator.
            /// </summary>
            public void Reset()
            {
                curListIndex = 0;
            }

            #endregion

            #region ctor
            /// <summary>
            /// Initializes a new instance of the <see cref="BlockEnumerator"/> class.
            /// </summary>
            /// <param name="block">The block.</param>
            /// <param name="readType">Type of the read operation.</param>
            public BlockEnumerator(MSWordBlock block, BlockType readType)
            {
                this.block = block;
                this.readOrderType = readType;
                
                ReadWordContents(block.Range, listEtors);
            }
            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // Do Nothing
            }

            #endregion

            #region Reading MS-Word Document Content

            /// <summary>
            /// Reads the next block of type blockType if any, 
            /// or returns false if there are no blocks remaining.
            /// The read block will be stored in curBlock, which consequently 
            /// will be returned by the Current method.
            /// </summary>
            public bool MoveNext(BlockType blockType)
            {
                if (blockType == BlockType.Word)
                {
                    if (curListIndex >= listEtors.Count)
                        return false;

                    IEnumerator etor = null;

                    // fixes the value of curListIndex
                    FindFirstNonNullEtorIndex();

                    if (curListIndex >= listEtors.Count)
                        return false;
                    else
                        etor = listEtors[curListIndex];

                    while (true) // !!!
                    {
                        if (etor.MoveNext())
                        {
                            curBlock = new MSWordBlock(block.ParentDocument, (Range)etor.Current, readOrderType);
                            return true;
                        }
                        else
                        {
                            curListIndex++;

                            FindFirstNonNullEtorIndex();

                            if (curListIndex >= listEtors.Count)
                                return false;
                            else
                                etor = listEtors[curListIndex];
                        }
                    }
                    //return false;
                }
                else if ((blockType == BlockType.Sentence) || (blockType == BlockType.Paragraph))
                {
                    // I create sentences and paragraphs myself (why??? why not use range.Paragraphs, or Sentences?)
                    int start = int.MaxValue;
                    int end;
                    bool isParagraph = (blockType == BlockType.Paragraph);
                    Range someRange = null;
                    MSWordBlock bl;
                    while (MoveNext(BlockType.Word))
                    {
                        bl = (MSWordBlock)curBlock;
                        someRange = bl.Range;
                        start = Math.Min(start, someRange.Start);
                        if (StringUtil.StringIsADelim( bl.Content, isParagraph ))
                        {
                            end = bl.Range.End;
                            object ostart = (object)start;
                            object oend = (object)end;
                            someRange.SetRange(start, end);
                            curBlock = new MSWordBlock(block.ParentDocument, someRange, isParagraph ? BlockType.Paragraph : BlockType.Sentence);
//                            curBlock = new MSWordBlock(block.ParentDocument.Range(ref ostart, ref oend), BlockType.Paragraph);
                            return true;
                        }
                    }
                    return false;
                }
                return false;
            }

            /// <summary>
            /// Moves the value of curListIndex to the 
            /// next member of listEtors which is not null.
            /// That's necessary because the list may contain null members.
            /// </summary>
            private void FindFirstNonNullEtorIndex()
            {
                for (; curListIndex < listEtors.Count; ++curListIndex)
                {
                    if (listEtors[curListIndex] != null)
                        break;
                }
            }

            /// <summary>
            /// We only read document contents Word-by-Word. Also Paragraphs and Sentences are 
            /// recognized from words.
            /// </summary>
            private IEnumerable GetContentEnumerable(Range r)
            {
                return r.Words;
                //switch (readOrderType)
                //{
                //    case BlockType.Word:
                //        return r.Words;
                //    case BlockType.Sentence:
                //        return r.Sentences;
                //    case BlockType.Paragraph:
                //        return r.Paragraphs;
                //    case BlockType.Everything:
                //    case BlockType.Illegal:
                //    default:
                //        return null;
                //}
            }

            #region Read* Methods

            /// <summary>
            /// Reads the whole word contents from the whole word's object model.
            /// The type of content to be read (i.e. word-by-word, sentence-by-sentence, ...)
            /// is determined by the GetContentEnumerable method.
            /// This is the base content reading method to be called. Since all the other methods
            /// work recursively one should call this method to start the operation.
            /// </summary>
            /// <param name="range">The range object whose content is to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadWordContents(Range range, List<IEnumerator> list)
            {
                // empty the list of visited ranges
                listVisitedRanges.Clear();

                ReadMainStory(range, list);
                ReadComments(range, list);
                ReadEndNotes(range, list);
                ReadBookmarks(range, list);
                ReadFootNotes(range, list);
                ReadFormFields(range, list);
                ReadFrames(range, list);
                ReadInlineShapes(range, list);
//                ReadOMaths(range, list);
                
                foreach(Range storyRange in ((MSWordDocument)block.ParentDocument).CurrentMSDocument.StoryRanges)
                {
                    var storyType = storyRange.StoryType;
                    if(storyType == WdStoryType.wdEvenPagesFooterStory 
                        || storyType == WdStoryType.wdEvenPagesHeaderStory 
                        || storyType == WdStoryType.wdFirstPageFooterStory
                        || storyType == WdStoryType.wdFirstPageHeaderStory
                        || storyType == WdStoryType.wdPrimaryFooterStory
                        || storyType == WdStoryType.wdPrimaryHeaderStory)
                    {
                        ReadMainStory( storyRange, list);
                    }
                }
            }

            /// <summary>
            /// Receives a range and reads its content as well as content of all 
            /// the shapes inside that range. Also this function checks if the 
            /// range has been already read, so prevnets reading something twice.
            /// Every content-reading method makes its subject range, and must call 
            /// this method to read the contents of the mentioned range.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadMainStory(Range range, List<IEnumerator> list)
            {
                // checks if the range has been already read. Uses the RangeEqualityComparer
                // for checking the identity of the ranges.
                if (listVisitedRanges.Contains(range, rangeEqualityComparerObj))
                    return;

                // the enumerable (i.e. sequence) of content objects to be read.
                IEnumerable enumerable = null;

                // reads the main contents of the range
                try
                {
                    enumerable = GetContentEnumerable(range);
                    IEnumerator etor = enumerable.GetEnumerator();
                    if (etor != null)
                        list.Add(etor);
                }
                catch { }

                // reads the shapes inside the range.
                try
                {
                    foreach (global::Microsoft.Office.Interop.Word.Shape s in range.ShapeRange)
                    {
                        try
                        {
                            ReadMainStory(s.TextFrame.TextRange, list);
                        }
                        catch
                        {
                            try
                            {
                                foreach (global::Microsoft.Office.Interop.Word.Shape ss in s.CanvasItems)
                                {
                                    ReadMainStory(ss.TextFrame.TextRange, list);
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch
                {
                }
                
            }

            // reads the math objects inside the range.
            // this is only visible to Word 2007 object model.
            //private void ReadOMaths(Range range, List<IEnumerator> list)
            //{
            //    try
            //    {
            //        foreach (OMath b in range.OMaths)
            //        {
            //            try
            //            {
            //                ReadMainStory(b.Range, list);
            //            }
            //            catch { }
            //        }
            //    }
            //    catch { }
            //}

            /// <summary>
            /// Reads the inline-shapes within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadInlineShapes(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (InlineShape b in range.InlineShapes)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            /// <summary>
            /// Reads the frames within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadFrames(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (Frame b in range.Frames)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            /// <summary>
            /// Reads the form-fields within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadFormFields(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (FormField b in range.FormFields)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            /// <summary>
            /// Reads the footnotes within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadFootNotes(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (Footnote b in range.Footnotes)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            /// <summary>
            /// Reads the bookmarks within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadBookmarks(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (Bookmark b in range.Bookmarks)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            /// <summary>
            /// Reads the endnotes within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadEndNotes(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (Endnote b in range.Endnotes)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);                    
                }
            }

            /// <summary>
            /// Reads the comments within the given range.
            /// By reading we mean adding the enumerator to the list of enumerators.
            /// This list will be read later.
            /// </summary>
            /// <param name="range">The range to be read.</param>
            /// <param name="list">List of enumerators to be filled.</param>
            private void ReadComments(Range range, List<IEnumerator> list)
            {
                try
                {
                    foreach (Comment b in range.Comments)
                    {
                        try
                        {
                            ReadMainStory(b.Range, list);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion

            #endregion
        }

        #endregion
    }
}

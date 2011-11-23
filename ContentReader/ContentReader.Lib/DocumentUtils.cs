using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// This class provides document-wide utility functions
    /// </summary>
    public class DocumentUtils
    {
        #region Document Opening and Closing
        public static bool OpenDocument(string fileName, bool openReadOnly, out Document doc, out Application app)
        {
            app = new global::Microsoft.Office.Interop.Word.ApplicationClass();
            app.Visible = false;

            object oFileName = fileName;
            object nullobj = System.Reflection.Missing.Value;
            object falseObj = false;
            object trueObj = true;
            object readOnlyObj = openReadOnly ? trueObj : nullobj;

            try
            {
                doc = app.Documents.Open(ref oFileName, ref nullobj, ref readOnlyObj, ref falseObj,
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                        ref nullobj, ref nullobj, ref falseObj, 
                        ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            }
            catch (Exception)
            {
                doc = null;

                if (app.Documents.Count == 0)
                {
                    try
                    {
                        object saveChanges = false;
                        app.Quit(ref saveChanges, ref nullobj, ref nullobj);
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                    finally
                    {
                        app = null;
                    }
                }
            }

            return doc != null;


        }

        public static bool OpenDocument(string fileName, out Document doc, out Application app)
        {
            return OpenDocument(fileName, false, out doc, out app);
        }

        public static bool CloseDocument(Document doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc", "The document object cannot be null!");

            object nullobj = System.Reflection.Missing.Value;

            try
            {
                doc.Close(ref nullobj, ref nullobj, ref nullobj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CloseDocument(Document doc, Application app)
        {
            bool succeeded = CloseDocument(doc);

            object nullobj = System.Reflection.Missing.Value;

            try
            {
                if (app != null && app.Documents.Count == 0)
                {
                    app.Quit(ref nullobj, ref nullobj, ref nullobj);
                }
            }
            catch
            {
                return false;
            }

            return succeeded;
        }

        #endregion

        #region Replacing content

        public static void ReplaceAllInDocument(Document doc, string oldValue, string newValue)
        {
            foreach(var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAll(oldValue, newValue);
            }
        }

        public static void ReplaceAllCaseInsensitiveInDocument(Document doc, string oldValue, string newValue)
        {
            foreach (var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAllCaseInsensitive(oldValue, newValue);
            }
        }

        public static void ReplaceAllWordsCaseInsensitiveInDocument(Document doc, string oldValue, string newValue)
        {
            foreach (var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAllWordsCaseInsensitive(oldValue, newValue);
            }
        }


        public static void ReplaceAllStandardizedInDocument(Document doc, string oldValue, string newValue)
        {
            foreach(var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAllStandardized(oldValue, newValue);
            }
        }

        public static void ReplaceAllWordsStandardizedInDocument(Document doc, string oldValue, string newValue)
        {
            foreach (var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAllWordsStandardized(oldValue, newValue);
            }
        }


        public static void ReplaceAllRegexpInDocument(Document doc, string regexp, string newValue)
        {
            foreach (var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAllRegexp(regexp, newValue);
            }
        }

        public static void ReplaceAllRegexpStandardizedInDocument(Document doc, string regexp, string newValue)
        {
            foreach (var par in RangeWrapper.ReadParagraphs(doc))
            {
                par.ReplaceAllRegexpStandardized(regexp, newValue);
            }
        }

        public static void ReplaceAllTwoWordsCombinationInDocument(Document doc, string word1, string word2, string newValue)
        {
            string regexp = @"\b" + word1 + @"\s+" + word2 + @"\b";
            ReplaceAllRegexpStandardizedInDocument(doc, regexp, newValue);
        }



        #endregion

        #region Revisions Enabling and Disabling
        public static bool IsRevisionsEnabled(Document document)
        {
            try
            {
				return document.ShowRevisions;
            }
            catch(COMException)
            {
                return false;
            }
        }

        public static void ChangeShowingRevisions(Document document, bool enable)
        {
            try
            {
                document.ShowRevisions = enable;
            }
            catch (COMException)
            {
                // Do nothing
            }
        }

        #endregion

        public static Range GetWordAtCursor(Document document)
        {
            return document.Application.Selection.Range.Words[1];
        }

        public static Range GetParagraphAtCursor(Document document)
        {
            return document.Application.Selection.Range.Paragraphs[1].Range;
        }

        public static Range GetCharacterAtCursor(Document document)
        {
            return document.Application.Selection.Range.Characters[1];
        }

        public static Selection GetSelection(Document document)
        {
            return document.Application.Selection;
        }

        public static Range GetSelectionRange(Document document)
        {
            var selection = document.Application.Selection;
            if(selection != null)
                return selection.Range;
            else
                return null;
        }

        public static void SetSelectionPosition(Document document, int pos)
        {
            SetSelectionPosition(document, pos, pos);
        }

        public static void SetSelectionPosition(Document document, int start, int end)
        {
            var sel = GetSelection(document);
            if(sel == null)
            {
                document.Content.Characters[1].Select();
                sel = GetSelection(document);
                if(sel == null)
                    throw new Exception("Cannot acquire selection object!");
            }
            sel.Start = start;
            sel.End = end;
        }

        #region Reading Words
        public static IEnumerable<Range> ReadWords(Document document)
        {
            Range firstWord = document.Words[1];
            return ReadWordsStartingFrom(document, firstWord);
        }

        public static IEnumerable<Range> ReadWordsStartingFrom(Document document, Range firstWord)
        {
            var firstStory = firstWord.StoryType;

            foreach (Range r in ReadWordsInAllInstancesOfStoryStartingFrom(document, firstWord))
            {
                yield return r;
            }

            foreach (Range r in ReadAllWordsExceptThoseInStory(document, firstStory))
            {
                yield return r;
            }
        }

        public static IEnumerable<Range> ReadWordsStartingFromCursor(Document document)
        {
            Range firstWord = document.Application.Selection.Range.Words[1];
            return ReadWordsStartingFrom(document, firstWord);
        }

        public static IEnumerable<Range> ReadAllWordsExceptThoseInStory(Document document, WdStoryType storyTypeToIgnore)
        {
            foreach (Range storyRange in document.StoryRanges)
            {
                var storyType = storyRange.StoryType;

                if (storyType == storyTypeToIgnore)
                    continue;

                if (storyType == WdStoryType.wdMainTextStory
                    || storyType == WdStoryType.wdTextFrameStory
                    || storyType == WdStoryType.wdEvenPagesFooterStory
                    || storyType == WdStoryType.wdEvenPagesHeaderStory
                    || storyType == WdStoryType.wdFirstPageFooterStory
                    || storyType == WdStoryType.wdFirstPageHeaderStory
                    || storyType == WdStoryType.wdPrimaryFooterStory
                    || storyType == WdStoryType.wdPrimaryHeaderStory
                    || storyType == WdStoryType.wdEndnotesStory
                    || storyType == WdStoryType.wdCommentsStory)
                {
                    foreach (Range r in ReadWordsInAllInstancesOfStory(document, storyType))
                    {
                        yield return r;
                    }
                }
            }
        }

        public static IEnumerable<Range> ReadWordsInCurrentInstanceOfStoryStartingFrom(Document document, Range firstWord)
        {
            object movementCount = 1;
            object unit = WdUnits.wdWord;

            Range nextWord = firstWord;
            while (nextWord != null)
            {
                yield return nextWord;

                //nextWord = nextWord.NextStoryRange;
                nextWord = nextWord.Next(ref unit, ref movementCount);
            }
        }

        public static IEnumerable<Range> ReadWordsInAllInstancesOfStoryStartingFrom(Document document, Range firstWord)
        {
            foreach (Range r in ReadWordsInCurrentInstanceOfStoryStartingFrom(document, firstWord))
            {
                yield return r;
            }


            Range rnextinStory = firstWord.NextStoryRange;
            while (rnextinStory != null)
            {
                foreach (Range rword in ReadWordsInCurrentInstanceOfStoryStartingFrom(document, rnextinStory.Words[1]))
                {
                    yield return rword;
                }

                rnextinStory = rnextinStory.NextStoryRange;
            }
        }

        public static IEnumerable<Range> ReadWordsInAllInstancesOfStory(Document document, WdStoryType storyType)
        {
            Range r = document.StoryRanges[storyType];
            while (r != null)
            {
                foreach (Range rword in ReadWordsInCurrentInstanceOfStoryStartingFrom(document, r.Words[1]))
                {
                    yield return rword;
                }

                r = r.NextStoryRange;
            }
        }

        #endregion

        #region Reading Paragraphs

        /// <summary>
        /// Reads the paragraphs of the document starting from the beginning.
        /// </summary>
        /// <param name="document">The document to read from.</param>
        /// <returns>Sequence of ranges containing paragraphs</returns>
        public static IEnumerable<Range> ReadParagraphs(Document document)
        {
            Range firstPar = document.Paragraphs[1].Range;
            return ReadParagraphsStartingFrom(document, firstPar);
        }

        /// <summary>
        /// Reads the paragraphs of the document starting from the specified paragraph.
        /// </summary>
        /// <param name="document">The document to read from.</param>
        /// <param name="firstPar">The first paragraph to start reading from.</param>
        /// <returns>Sequence of ranges containing paragraphs</returns>
        public static IEnumerable<Range> ReadParagraphsStartingFrom(Document document, Range firstPar)
        {
            var firstStory = firstPar.StoryType;

            foreach (Range r in ReadParagrahsInAllInstancesOfStoryStartingFrom(document, firstPar))
            {
                yield return r;
            }

            foreach (Range r in ReadAllParagrahsExceptThoseInStory(document, firstStory))
            {
                yield return r;
            }
        }

        /// <summary>
        /// Reads the paragraphs of the document starting from cursor.
        /// </summary>
        /// <param name="document">The document to read from.</param>
        /// <returns>Sequence of ranges containing paragraphs</returns>
        public static IEnumerable<Range> ReadParagraphsStartingFromCursor(Document document)
        {
            Range firstPar = document.Application.Selection.Range.Paragraphs[1].Range;
            return ReadParagraphsStartingFrom(document, firstPar);
        }

        public static IEnumerable<Range> ReadAllParagrahsExceptThoseInStory(Document document, WdStoryType storyTypeToIgnore)
        {
            foreach (Range storyRange in document.StoryRanges)
            {
                var storyType = storyRange.StoryType;
                
                if (storyType == storyTypeToIgnore)
                    continue;

                if (storyType ==  WdStoryType.wdMainTextStory 
                    || storyType ==  WdStoryType.wdTextFrameStory 
                    || storyType == WdStoryType.wdEvenPagesFooterStory
                    || storyType == WdStoryType.wdEvenPagesHeaderStory
                    || storyType == WdStoryType.wdFirstPageFooterStory
                    || storyType == WdStoryType.wdFirstPageHeaderStory
                    || storyType == WdStoryType.wdPrimaryFooterStory
                    || storyType == WdStoryType.wdPrimaryHeaderStory
                    || storyType == WdStoryType.wdEndnotesStory
                    || storyType == WdStoryType.wdCommentsStory)
                {
                    foreach (Range r in ReadParagrahsInAllInstancesOfStory(document, storyType))
                    {
                        yield return r;
                    }
                }
            }
        }

        public static IEnumerable<Range> ReadParagraphsInCurrentInstanceOfStoryStartingFrom(Document document, Range firstPar)
        {
            object movementCount = 1;
            object unit = WdUnits.wdParagraph;

            Range nextPar = firstPar;
            while(nextPar != null)
            {
                yield return nextPar;

                //nextPar = nextPar.NextStoryRange;
                nextPar = nextPar.Next(ref unit, ref movementCount);
            }
        }

        public static IEnumerable<Range> ReadParagrahsInAllInstancesOfStoryStartingFrom(Document document, Range firstPar)
        {
            foreach (Range r in ReadParagraphsInCurrentInstanceOfStoryStartingFrom(document, firstPar))
            {
                yield return r;
            }


            Range rnextinStory = firstPar.NextStoryRange;
            while (rnextinStory != null)
            {
                foreach (Range par in ReadParagraphsInCurrentInstanceOfStoryStartingFrom(document, rnextinStory.Paragraphs[1].Range))
                {
                    yield return par;
                }

                rnextinStory = rnextinStory.NextStoryRange;
            }
        }

        public static IEnumerable<Range> ReadParagrahsInAllInstancesOfStory(Document document, WdStoryType storyType)
        {
            Range r = document.StoryRanges[storyType];
            while (r != null)
            {
                foreach (Range par in ReadParagraphsInCurrentInstanceOfStoryStartingFrom(document, r.Paragraphs[1].Range))
                {
                    yield return par;
                }

                r = r.NextStoryRange;
            }
        }

        #endregion
    }
}

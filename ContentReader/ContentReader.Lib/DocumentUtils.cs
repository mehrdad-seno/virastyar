using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    public class DocumentUtils
    {
        public static IEnumerable<Range> ReadParagraphs(Document document)
        {
            Range firstPar = document.Paragraphs[1].Range;
            return ReadParagraphsStartingFrom(document, firstPar);
        }

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
    }
}

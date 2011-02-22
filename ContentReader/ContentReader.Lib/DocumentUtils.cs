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

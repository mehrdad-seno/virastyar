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
    public class RangeWrapper
    {
        #region static methods
        
        public readonly static char InvisibleChar = '\uE83D';

            public static IEnumerable<RangeWrapper> ReadParagraphsStartingFromCursor(Document document)
        {
            foreach (Range r in DocumentUtils.ReadParagraphsStartingFromCursor(document))
            {
                yield return new RangeWrapper(r);
            }
        }

        public static IEnumerable<RangeWrapper> ReadParagraphs(Document document)
        {
            foreach (Range r in DocumentUtils.ReadParagraphs(document))
            {
                yield return new RangeWrapper(r);
            }
        }


        #endregion

        private Range m_range = null;

        private int? m_start = null;
        private int? m_end = null;

        private bool m_isTextRead = false;
        private string m_rawtext = null;
        private string m_text = null;

        private bool m_isMappingMade = false;
        private string m_textMapping = null;

        public RangeWrapper(Range range)
        {
            m_range = range;
        }

        public bool HasRange
        {
            get
            {
                return m_range == null;
            }
        }

        public Range Range
        {
            get
            {
                return m_range;
            }
        }

        public void Invalidate()
        {
            m_start = null;
            m_end = null;
            m_isTextRead = false;
            m_isMappingMade = false;
            m_textMapping = null;
        }

        public int Start
        {
            get
            {
                if (!m_start.HasValue)
                {
                    m_start = m_range.Start;
                }

                return m_start.Value;
            }

            set
            {
                m_range.Start = value;
                Invalidate();
            }
        }

        public int End
        {
            get
            {
                if (!m_end.HasValue)
                {
                    m_end = m_range.End;
                }

                return m_end.Value;
            }

            set
            {
                m_range.End = value;
                Invalidate();
            }
        }

        public string Text
        {
            get
            {
                if (!m_isTextRead)
                {
                    ReadText();
                }
                return m_text;
            }

            set
            {
                m_range.Text = value;
                Invalidate();
            }
        }

        public string RawText
        {
            get
            {
                if (!m_isTextRead)
                {
                    ReadText();
                }
                return m_rawtext;
            }

            set
            {
                m_range.Text = value;
                Invalidate();
            }
        }

        public string Mapping
        {
            get
            {
                if (!m_isMappingMade)
                {
                    BuildMapping();
                }

                return m_textMapping;
            }
        }

        private void BuildMapping()
        {
            string text = this.Text;

            // TODO: write a true method building the mapping
            m_textMapping = text;
            m_isMappingMade = true;
        }

        private void ReadText()
        {
            m_rawtext = m_range.Text;
            m_text = m_rawtext ?? "";
            m_text = m_text.Replace("\r\a", "\r");
            m_isTextRead = true;
        }

        public int RangeLength
        {
            get
            {
                return this.End - this.Start;
            }
        }

        public RangeWrapper FirstParagraph
        {
            get
            {
                return new RangeWrapper(m_range.Paragraphs[1].Range);
            }
        }

        public RangeWrapper FirstSentence
        {
            get
            {
                return new RangeWrapper(m_range.Sentences[1]);
            }
        }

        public RangeWrapper FirstWord
        {
            get
            {
                return new RangeWrapper(m_range.Words[1]);
            }
        }

        public RangeWrapper FirstCharacter
        {
            get
            {
                return new RangeWrapper(m_range.Characters[1]);
            }
        }

        /// <summary>
        /// levels: 
        /// 0: para
        /// 1: sent
        /// 2: wrod
        /// 3: char
        /// 4+: return
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public RangeWrapper GetFirst(int level)
        {
            switch (level)
            {
                case 0:
                    return FirstParagraph;
                case 1:
                    return FirstSentence;
                case 2:
                    return FirstWord;
                case 3:
                    return FirstCharacter;
                default:
                    throw new ArgumentException("level cannot exceed 3");
            }
        }

        public RangeWrapper GetNext(WdUnits units, int count)
        {
            object moveCount = count;
            object unitsOfMovement = units;

            Range nextRange = m_range.Next(ref unitsOfMovement, ref moveCount);
            return new RangeWrapper(nextRange);
        }

        /// <summary>
        /// levels: 
        /// 0: para
        /// 1: sent
        /// 2: wrod
        /// 3: char
        /// 4+: return
        /// </summary>
        /// <param name="level"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RangeWrapper GetNext(int level, int count)
        {
            WdUnits unitsOfMovement = WdUnits.wdParagraph;
            switch (level)
            {
                case 0:
                    unitsOfMovement = WdUnits.wdParagraph;
                    break;
                case 1:
                    unitsOfMovement = WdUnits.wdSentence;
                    break;
                case 2:
                    unitsOfMovement = WdUnits.wdWord;
                    break;
                case 3:
                    unitsOfMovement = WdUnits.wdCharacter;
                    break;
                default:
                    throw new ArgumentException("level cannot exceed 3");
            }

            return GetNext(unitsOfMovement, count);
        }

        public RangeWrapper GetCopy()
        {
            return GetRange(this.Start, this.End);
        }

        public RangeWrapper GetRange(int start, int end)
        {
            Range r2 = m_range.Words[1];
            r2.SetRange(start, end);
            return new RangeWrapper(r2);
        }

        public RangeWrapper GetRangeWithOffset(int startOffset, int length)
        {
            int baseIndex = this.Start;
            return GetRange(baseIndex + startOffset, baseIndex + startOffset + length);
        }

        public IntervalOverlapKinds DetectOverlapKind(RangeWrapper second)
        {
            return IntervalOverlap.Detect(this.Start, this.End, second.Start, second.End);
        }

        public bool IsGreatlyHealthy
        {
            get
            {
                // sometimes there are extra chars n range text that is not visible in the range itself
                // such as \a some \r's and \u0001 and so on.
                // If there's such a case the range is not GREATLY healthy 
                // but might yet be considered as healthy
                return this.RangeLength == this.RawText.Length;
            }
        }

        public bool IsHealthy
        {
            get
            {
                // the combination of "\r\a" sometimes occur at the end of table cells (or rows)
                // the range length is 1 while the text length is 2!
                return this.RangeLength == this.Text.Length;
            }
        }


        public void Select()
        {
            try
            {
                m_range.Select();
            }
            catch
            { 
                // some ranges cannot be selected; e.g., those in the comments
            }
        }
    }
}

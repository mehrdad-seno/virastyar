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
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using VirastyarWordAddin.Configurations;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Persian;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;
using SCICT.Utility.GUI;

namespace VirastyarWordAddin
{
    public class AllCharactersRefiner : VerifierBase
    {
        private static readonly Dictionary<RefinementType, long> dicRefinementStats = new Dictionary<RefinementType, long>();

        private const string LabelRefineLetter = "اصلاح حروف";
        private const string LabelRefineDigit = "اصلاح ارقام";
        private const string LabelRefineHalfSpace = "اصلاح نویسه‌ی نیم‌فاصله";
        private const string LabelRefineErab = "اصلاح اعراب";
        private const string LabelRefineHalfSpacePosition = "حذف و جابجایی نیم‌فاصله‌ها";

        private AllCharactersRefinerSettings settings = null;
        private bool useIgnoreList;
        private bool useFilterCategories;
        private FilteringCharacterCategory filteringCategory;
        private HashSet<char> ignoreList;


        public AllCharactersRefiner(AllCharactersRefinerSettings settings)
        {
            this.settings = settings;

            ResetStats();
            useIgnoreList = !settings.IsEmptyIgnoreList();
            useFilterCategories = settings.HasAnyFilteringCategory();
            filteringCategory = settings.GetIgnoredCategories();
            ignoreList = settings.GetIgnoreList();
        }

        protected override void InitVerifWin()
        {
            base.InitVerifWin();
            m_verificationWindow.SetCaption("اصلاح تمام نویسه‌های متن");
            this.m_verificationWindow.SetHelp(HelpConstants.RefineAllChars);
        }

        protected override void ResetStats()
        {
            dicRefinementStats.Clear();
            foreach (var i in Enum.GetValues(typeof(RefinementType)))
            {
                dicRefinementStats.Add((RefinementType)(i), 0);
            }
        }

        private void AddToStats(string originalString, string refinedString)
        {
            Dictionary<char, int> charCounts = new Dictionary<char, int>();

            foreach (char och in originalString)
            {
                if (charCounts.ContainsKey(och))
                {
                    charCounts[och]++;
                }
                else
                {
                    charCounts.Add(och, 1);
                }
            }

            foreach (char rch in refinedString)
            {
                if (charCounts.ContainsKey(rch))
                {
                    charCounts[rch]--;
                    if (charCounts[rch] <= 0)
                        charCounts.Remove(rch);
                }
                else
                {
                    // ?
                }
            }

            foreach (var pair in charCounts)
            {
                if (StringUtil.IsArabicDigit(pair.Key))
                    dicRefinementStats[RefinementType.RefineDigit] += pair.Value;
                else if (StringUtil.IsArabicLetter(pair.Key))
                    dicRefinementStats[RefinementType.RefineLetter] += pair.Value;
                else if (PseudoSpace.ZWNJ == pair.Key)
                    dicRefinementStats[RefinementType.RefineHalfSpacePosition] += pair.Value;
                else if (StringUtil.IsHalfSpace(pair.Key))
                    dicRefinementStats[RefinementType.RefineHalfSpace] += pair.Value;
                else if (StringUtil.IsErabSign(pair.Key))
                    dicRefinementStats[RefinementType.RefineErab] += pair.Value;
                else
                {
                    Debug.Assert(true);
                    throw new Exception("Unexpected condition met in character refiner!");
                }
            }
        }

        private string StatsticsMessage()
        {
            StringBuilder sb = new StringBuilder();

            long sum = 0L;
            foreach(var pair in dicRefinementStats)
            {
                sum += pair.Value;
                sb.AppendLine(String.Format("{0}: {1}", GetRefinementTypeLabel( pair.Key ), ParsingUtils.ConvertNumber2Persian( pair.Value.ToString())));
            }

            return String.Format("{0}: {1}{2}{2}", "تعداد کل نویسه‌های اصلاح شده", ParsingUtils.ConvertNumber2Persian( sum.ToString() ), Environment.NewLine) + sb.ToString();
        }

        private string GetRefinementTypeLabel(RefinementType refinementType)
        {
            switch (refinementType)
            {
                case RefinementType.RefineLetter:
                    return LabelRefineLetter;
                case RefinementType.RefineDigit:
                    return LabelRefineDigit;
                case RefinementType.RefineHalfSpace:
                    return LabelRefineHalfSpace;
                case RefinementType.RefineErab:
                    return LabelRefineErab;
                case RefinementType.RefineHalfSpacePosition:
                    return LabelRefineHalfSpacePosition;
                default:
                    return "";
            }
        }

        /*
        public void RefineAll(AllCharactersRefinerSettings settings)
        {
            ResetStats();
            using (MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument)
            {
                if (d == null) return;

                bool useIgnoreList = !settings.IsEmptyIgnoreList();
                bool useFilterCategories = settings.HasAnyFilteringCategory();

                FilteringCharacterCategory filteringCategory = settings.GetIgnoredCategories();
                HashSet<char> ignoreList = settings.GetIgnoreList();


                foreach (MSWordBlock b in ((MSWordBlock)d.GetContent()).Paragraphs)
                {
                    Range rPar = b.Range;

                    rPar.Select();
//                    System.Windows.Forms.MessageBox.Show("");

                    foreach (Range word in rPar.Words)
                    {
                        RangeUtils.TrimRange(word);

                        if (word != null && word.Text != null && !StringUtil.IsWhiteSpace(word.Text))
                        {
                            try
                            {
                                string originalString = word.Text;

                                if (!originalString.Trim().StartsWith("ref", true, null))
                                {
                                    // Character with code 2 is used by word to denote footnotes.
                                    // Character with code 1 is used by word to denote formulas.
                                    if (!(originalString.Contains(Convert.ToChar(2)) || (originalString.Contains(Convert.ToChar(1)))))
                                    {
                                        //word.Select();
                                        //System.Windows.Forms.MessageBox.Show(word.Text);

                                        string result = originalString;

                                        if(settings.RefineHalfSpacePositioning)
                                            result = StringUtil.NormalizeSpacesAndHalfSpacesInWord(originalString);

                                        if (!useIgnoreList) // no ignore list
                                        {
                                            if (useFilterCategories) // use cats
                                            {
                                                result = StringUtil.FilterPersianWord(result, filteringCategory);
                                            }
                                            else // don't use cats
                                            {
                                                result = StringUtil.FilterPersianWord(result);
                                            }
                                        }
                                        else // should use ignore list
                                        {
                                            if (useFilterCategories) // use cats
                                            {
                                                result = StringUtil.FilterPersianWord(result, ignoreList, filteringCategory);
                                            }
                                            else // don't use cats
                                            {
                                                result = StringUtil.FilterPersianWord(result, ignoreList);
                                            }
                                        }

                                        Debug.Assert(result != null);
                                        if (originalString != result)
                                        {
                                            word.Text = result;
                                            AddToStats(originalString, result);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //word.Select();
                                //MessageBox.Show(word.Text + "\r\n" + ex.ToString());
                            }
                        }
                    }
                }
            }
            PersianMessageBox.Show(StatsticsMessage(), "عملیات اصلاح با موفقیت به پایان رسید");
        }
        */

        public override void ShowStats()
        {
            PersianMessageBox.Show(StatsticsMessage(), Constants.UIMessages.SuccessRefinementTitle);
        }

        protected override bool ProcessParagraphForVerification(MSWordBlock b)
        {
            Range rPar = b.Range;
            rPar.SelectIfPossible();

            foreach (Range word in rPar.Words)
            {
                RangeUtils.TrimRange(word);

                if (word != null && word.Text != null && !StringUtil.IsWhiteSpace(word.Text))
                {
                    try
                    {
                        string originalString = word.Text;

                        if (!originalString.Trim().StartsWith("ref", true, null))
                        {
                            // Character with code 2 is used by word to denote footnotes.
                            // Character with code 1 is used by word to denote formulas.
                            if (!(originalString.Contains(Convert.ToChar(2)) || (originalString.Contains(Convert.ToChar(1)))))
                            {
                                //word.Select();
                                //System.Windows.Forms.MessageBox.Show(word.Text);

                                string result = originalString;

                                if (settings.RefineHalfSpacePositioning)
                                    result = StringUtil.NormalizeSpacesAndHalfSpacesInWord(originalString);

                                if (!useIgnoreList) // no ignore list
                                {
                                    if (useFilterCategories) // use cats
                                    {
                                        result = StringUtil.FilterPersianWord(result, filteringCategory);
                                    }
                                    else // don't use cats
                                    {
                                        result = StringUtil.FilterPersianWord(result);
                                    }
                                }
                                else // should use ignore list
                                {
                                    if (useFilterCategories) // use cats
                                    {
                                        result = StringUtil.FilterPersianWord(result, ignoreList, filteringCategory);
                                    }
                                    else // don't use cats
                                    {
                                        result = StringUtil.FilterPersianWord(result, ignoreList);
                                    }
                                }

                                Debug.Assert(result != null);
                                if (result == null)
                                {
                                    throw new Exception("Unexpected condition met in character refiner!");
                                }

                                if (originalString != result)
                                {
                                    if (Globals.ThisAddIn.SetRangeContent(word, result, false))
                                    {
                                        AddToStats(originalString, result);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //word.Select();
                        //MessageBox.Show(word.Text + "\r\n" + ex.ToString());
                    }
                }
            }
            return true;
        }
    }

    public enum RefinementType
    {
        RefineLetter,
        RefineDigit,
        RefineHalfSpace,
        RefineErab,
        RefineHalfSpacePosition
    }
}

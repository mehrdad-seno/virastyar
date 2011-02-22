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
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Utility.Calendar;
using SCICT.NLP.Utility.Parsers;

namespace VirastyarWordAddin
{
    public partial class DateVerificationWindow : VerificationWindowBase
    {
        #region Private Fields
        
        private const string GroupNameJalali = "listViewGroupJalali";
        private const string GroupNameGregorian = "listViewGroupGregorian";
        private const string GroupNameGhamari = "listViewGroupGhamari";
        
        #endregion

        #region Constructors

        public DateVerificationWindow()
        {
            Point oldPanelProgressLocation = panelProgressMode.Location;
            Point oldPanelVerifLocation = panelVerificationMode.Location;
            Point oldPanelConfirmLocation = panelConfirmation.Location;

            InitializeComponent();
            PostInitComponent();

            this.panelProgressMode.Location = oldPanelProgressLocation;
            this.panelVerificationMode.Location = oldPanelVerifLocation;
            this.panelConfirmation.Location = oldPanelConfirmLocation;
        }

        private void PostInitComponent()
        {
            this.btnChange.Top += 10;
            this.btnChangeAll.Top += 10;
            // nothing yet
        }

        #endregion

        #region Inner Types

        private enum UIDateTypes
        {
            NumericalEnglish,
            NumericalPersian,
            Literal
        }

        private enum UIDateGroups
        {
            Jalali,
            Gregorian,
            Ghamari
        }

        #endregion

        public override string SelectedSuggestion
        {
            get
            {
                int n = ListViewSelectedIndex(lstSuggestions);
                if (n >= 0)
                {
                    Globals.ThisAddIn.UsageLogger.SetSelectedSuggestions(
                        lstSuggestions.Items[n].SubItems[0].Text, n);

                    return lstSuggestions.Items[n].SubItems[0].Text;
                }
                else
                    return "";
            }
        }

        private int ListViewSelectedIndex(ListView listView)
        {
            if (listView.SelectedIndices.Count > 0)
                return listView.SelectedIndices[0];
            else
                return -1;
        }

        private bool SetContent(Range bParagraph, Range bContent, IPatternInfo patternInfo)
        {
            if (!base.SetContent(bParagraph, bContent))
            {
                return false;
            }

            comboYearNumber.Enabled = false;
            cmboGuessedCalendarType.Enabled = false;

            lstSuggestions.Items.Clear();
            lstSuggestions.Items.AddRange(CreateSuggestions(patternInfo));

            if (lstSuggestions.Items.Count > 0)
            {
                btnChangeAll.Enabled = CanEnableButton(VerificationWindowButtons.ChangeAll);
                btnChange.Enabled = CanEnableButton(VerificationWindowButtons.Change);
                lstSuggestions.Items[0].Selected = true;
            }
            else
            {
                btnChangeAll.Enabled = false;
                btnChange.Enabled = false;
            }
            lstSuggestions.Select();

            return true;
        }

        private ListViewItem CreateListViewItem(string item, UIDateTypes dateType, UIDateGroups group)
        {
            ListViewGroup listGroup;

            switch (group)
            {
                case UIDateGroups.Jalali:
                    listGroup = lstSuggestions.Groups[GroupNameJalali];
                    break;
                case UIDateGroups.Gregorian:
                    listGroup = lstSuggestions.Groups[GroupNameGregorian];
                    break;
                case UIDateGroups.Ghamari:
                default:
                    listGroup = lstSuggestions.Groups[GroupNameGhamari];
                    break;
            }

            return CreateListViewItem(item, dateType, listGroup);
        }

        private ListViewItem CreateListViewItem(string item, UIDateTypes dateType, ListViewGroup group)
        {
            string strDateType = "";

            switch (dateType)
            {
                case UIDateTypes.NumericalEnglish:
                    strDateType = "تاریخ عددی با ارقام انگلیسی";
                    break;
                case UIDateTypes.NumericalPersian:
                    strDateType = "تاریخ عددی با ارقام فارسی";
                    break;
                case UIDateTypes.Literal:
                    strDateType = "تاریخ حرفی";
                    break;
                default:
                    strDateType = "";
                    break;
            }

            ListViewItem listViewItem = new ListViewItem(new string[] { item, strDateType });
            listViewItem.ToolTipText = item;
            listViewItem.Group = group;

            return listViewItem;
        }


        #region Suggestionns

        int originalYearNumber = -1;

        private ListViewItem[] CreateSuggestions(IPatternInfo minPi)
        {
            switch (minPi.PatternInfoType)
            {
                case PatternInfoTypes.EnglishDate:
                    return CreateEnglishDateSuggestions(minPi as EnglishDatePatternInfo);
                case PatternInfoTypes.NumericDate:
                    return CreateNumericDateSuggestions(minPi as NumericDatePatternInfo);
                case PatternInfoTypes.PersianDate:
                    return CreatePersianDateSuggestions(minPi as PersianDatePatternInfo);
                default:
                    return new ListViewItem[0];
            }
        }

        private ListViewItem[] CreatePersianDateSuggestions(PersianDatePatternInfo pi)
        {
            if (pi == null)
            {
                return new ListViewItem[0];
            }

            originalYearNumber = pi.YearNumber;
            AddPossibleYearNumbers(pi.YearNumber, pi.CalendarType);

            return CreateGeneralDateSuggestions(new NumericDatePatternInfo(pi.Content, pi.Index, pi.Length, pi.DayNumber, pi.MonthNumber, GetSelectedYearNumber()), pi.CalendarType);
        }

        public int GetSelectedYearNumber()
        {
            int yearNumber = -1;
            if (comboYearNumber.SelectedIndex >= 0 && comboYearNumber.Items.Count > 0)
            {
                string selectedYear = comboYearNumber.Items[comboYearNumber.SelectedIndex] as string;

                if (selectedYear != null)
                {
                    if (Int32.TryParse(ParsingUtils.ConvertNumber2English(selectedYear), out yearNumber))
                        return yearNumber;
                }
            }
            return -1;
        }

        private void AddPossibleYearNumbers(int yearNumber, DateCalendarType calendarType)
        {
            comboYearNumber.Items.Clear();
            if (yearNumber <= 0) return;

            if (yearNumber < 100)
            {
                int newYear = yearNumber;
                switch (calendarType)
                {
                    case DateCalendarType.Gregorian:
                        if ((2000 - (1900 + yearNumber)) < ((2000 + yearNumber) - 2000))
                            newYear += 1900;
                        else
                            newYear += 2000;
                        break;
                    case DateCalendarType.HijriGhamari:
                        newYear += 1400;
                        break;
                    case DateCalendarType.Jalali:
                        newYear += 1300;
                        break;
                    default:
                        break;
                }

                comboYearNumber.Items.Add(ParsingUtils.ConvertNumber2Persian(newYear.ToString()));

                comboYearNumber.Visible = true;
                comboYearNumber.Enabled = true;
            }

            comboYearNumber.Items.Add(ParsingUtils.ConvertNumber2Persian(yearNumber.ToString()));

            comboYearNumber.SelectedIndex = 0;
        }

        private DateCalendarType GuessCalendarType(int yearNumber)
        {
            DateTime today = DateTime.Today;
            int gregYear = today.Year;
            int jalaliYear = (new PersianCalendarEx(today)).GetYear();
            int ghamariYear = (new HijriCalendarEx(today)).GetYear();

            int minYearDiff = Math.Abs(yearNumber - gregYear);
            DateCalendarType minCalendarType = DateCalendarType.Gregorian;

            int curDiff = Math.Abs(yearNumber - jalaliYear);

            if (curDiff < minYearDiff)
            {
                minYearDiff = curDiff;
                minCalendarType = DateCalendarType.Jalali;
            }

            curDiff = Math.Abs(yearNumber - ghamariYear);

            if (curDiff < minYearDiff)
            {
                minYearDiff = curDiff;
                minCalendarType = DateCalendarType.HijriGhamari;
            }

            return minCalendarType;
        }

        private DateCalendarType GuessCalendarTypeAndAddPossibleYearNumbers(int yearNumber)
        {
            DateTime today = DateTime.Today;
            int gregYear = today.Year;
            int jalaliYear = (new PersianCalendarEx(today)).GetYear();
            int ghamariYear = (new HijriCalendarEx(today)).GetYear();

            int minYearDiff = Math.Abs(yearNumber - gregYear);
            DateCalendarType minCalendarType = DateCalendarType.Gregorian;

            int curDiff = Math.Abs(yearNumber - jalaliYear);

            if (curDiff < minYearDiff)
            {
                minYearDiff = curDiff;
                minCalendarType = DateCalendarType.Jalali;
            }

            curDiff = Math.Abs(yearNumber - ghamariYear);

            if (curDiff < minYearDiff)
            {
                minYearDiff = curDiff;
                minCalendarType = DateCalendarType.HijriGhamari;
            }

            if (yearNumber < 100)
            {
                // IT's better not to guess ghamari calendar
                //curDiff = Math.Abs(1400 + yearNumber - ghamariYear);
                //if (curDiff < minYearDiff)
                //{
                //    minYearDiff = curDiff;
                //    minCalendarType = DateCalendarType.HijriGhamari;
                //}

                curDiff = Math.Abs(1300 + yearNumber - jalaliYear);
                if (curDiff < minYearDiff)
                {
                    minYearDiff = curDiff;
                    minCalendarType = DateCalendarType.Jalali;
                }

                curDiff = Math.Abs(2000 + yearNumber - gregYear);
                if (curDiff < minYearDiff)
                {
                    minYearDiff = curDiff;
                    minCalendarType = DateCalendarType.Gregorian;
                }
            }

            return minCalendarType;
        }

        private ListViewItem[] CreateNumericDateSuggestions(NumericDatePatternInfo pi)
        {
            if (pi == null)
            {
                return new ListViewItem[0];
            }

            originalYearNumber = pi.YearNumber;

            DateCalendarType calType = GuessCalendarTypeAndAddPossibleYearNumbers(pi.YearNumber);

            AddPossibleYearNumbers(pi.YearNumber, calType);

            ListViewItem[] items = CreateGeneralDateSuggestions(
                new NumericDatePatternInfo(pi.Content, pi.Index, pi.Length, pi.DayNumber, pi.MonthNumber, GetSelectedYearNumber()),
                calType);

            cmboGuessedCalendarType.Enabled = true;
            return items;
        }

        private ListViewItem[] CreateEnglishDateSuggestions(EnglishDatePatternInfo pi)
        {
            if (pi == null)
            {
                return new ListViewItem[0];
            }
            originalYearNumber = pi.YearNumber;
            AddPossibleYearNumbers(pi.YearNumber, pi.CalendarType);

            return CreateGeneralDateSuggestions(new NumericDatePatternInfo(pi.Content, pi.Index, pi.Length, pi.DayNumber, pi.MonthNumber, GetSelectedYearNumber()), pi.CalendarType);
        }

        NumericDatePatternInfo currentNumericDatePatternInfo = null;

        private ListViewItem[] CreateGeneralDateSuggestions(NumericDatePatternInfo pi, DateCalendarType calendarType)
        {
            if (pi == null)
            {
                return new ListViewItem[0];
            }

            currentNumericDatePatternInfo = pi;

            List<ListViewItem> lstSug = new List<ListViewItem>();

            if (pi.YearNumber >= 0 && pi.DayNumber > 0 && pi.MonthNumber > 0)
            {
                if (calendarType == DateCalendarType.Jalali)
                {
                    try
                    {
                        cmboGuessedCalendarType.SelectedIndex = 0;

                        PersianCalendarEx pc = new PersianCalendarEx(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(pc.ToString()), UIDateTypes.NumericalPersian, UIDateGroups.Jalali));
                        lstSug.Add(CreateListViewItem(pc.ToString(), UIDateTypes.NumericalEnglish, UIDateGroups.Jalali));
                        lstSug.Add(CreateListViewItem(pc.ToString("D"), UIDateTypes.Literal, UIDateGroups.Jalali));

                        DateTime dt = pc.DateTime;
                        lstSug.Add(CreateListViewItem(CalendarStringUtils.GetPersianDateString(dt), UIDateTypes.Literal, UIDateGroups.Gregorian));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(dt.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Gregorian));
                        lstSug.Add(CreateListViewItem(dt.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Gregorian));

                        HijriCalendarEx hc = new HijriCalendarEx(pc.DateTime);
                        lstSug.Add(CreateListViewItem(hc.ToString("D"), UIDateTypes.Literal, UIDateGroups.Ghamari));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(hc.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Ghamari));
                        lstSug.Add(CreateListViewItem(hc.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Ghamari));
                    }
                    catch { }
                }
                else if (calendarType == DateCalendarType.Gregorian)
                {
                    try
                    {
                        cmboGuessedCalendarType.SelectedIndex = 1;

                        DateTime dt = new DateTime(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
                        lstSug.Add(CreateListViewItem(dt.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Gregorian));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(dt.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Gregorian));
                        lstSug.Add(CreateListViewItem(CalendarStringUtils.GetPersianDateString(dt), UIDateTypes.Literal, UIDateGroups.Gregorian));

                        PersianCalendarEx pc = new PersianCalendarEx(dt);
                        lstSug.Add(CreateListViewItem(pc.ToString("D"), UIDateTypes.Literal, UIDateGroups.Jalali));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(pc.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Jalali));
                        lstSug.Add(CreateListViewItem(pc.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Jalali));

                        HijriCalendarEx hc = new HijriCalendarEx(dt);
                        lstSug.Add(CreateListViewItem(hc.ToString("D"), UIDateTypes.Literal, UIDateGroups.Ghamari));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(hc.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Ghamari));
                        lstSug.Add(CreateListViewItem(hc.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Ghamari));
                    }
                    catch /*(Exception ex)*/ { }
                }
                else if (calendarType == DateCalendarType.HijriGhamari)
                {
                    try
                    {
                        cmboGuessedCalendarType.SelectedIndex = 2;

                        HijriCalendarEx hc = new HijriCalendarEx(pi.YearNumber, pi.MonthNumber, pi.DayNumber);
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(hc.ToString()), UIDateTypes.NumericalPersian, UIDateGroups.Ghamari));
                        lstSug.Add(CreateListViewItem(hc.ToString(), UIDateTypes.NumericalEnglish, UIDateGroups.Ghamari));
                        lstSug.Add(CreateListViewItem(hc.ToString("D"), UIDateTypes.Literal, UIDateGroups.Ghamari));

                        DateTime dt = hc.DateTime;
                        lstSug.Add(CreateListViewItem(CalendarStringUtils.GetPersianDateString(dt), UIDateTypes.Literal, UIDateGroups.Gregorian));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(dt.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Gregorian));
                        lstSug.Add(CreateListViewItem(dt.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Gregorian));

                        PersianCalendarEx pc = new PersianCalendarEx(hc.DateTime);
                        lstSug.Add(CreateListViewItem(pc.ToString("D"), UIDateTypes.Literal, UIDateGroups.Jalali));
                        lstSug.Add(CreateListViewItem(ParsingUtils.ConvertNumber2Persian(pc.ToString("d")), UIDateTypes.NumericalPersian, UIDateGroups.Jalali));
                        lstSug.Add(CreateListViewItem(pc.ToString("d"), UIDateTypes.NumericalEnglish, UIDateGroups.Jalali));
                    }
                    catch (Exception ex) { }
                }
            }

            return lstSug.ToArray();
        }

        #endregion


        private void lstSuggestions_DoubleClick(object sender, EventArgs e)
        {
            OnChangeButtonPressed();
        }

        private void cmboGuessedCalendarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboGuessedCalendarType.Enabled == false) return;

            lstSuggestions.Items.Clear();

            comboYearNumber.Enabled = false;

            switch (cmboGuessedCalendarType.SelectedIndex)
            {
                case 0: // i.e. jalali
                    lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(currentNumericDatePatternInfo, DateCalendarType.Jalali));
                    AddPossibleYearNumbers(originalYearNumber, DateCalendarType.Jalali);
                    break;
                case 1:
                    lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(currentNumericDatePatternInfo, DateCalendarType.Gregorian));
                    AddPossibleYearNumbers(originalYearNumber, DateCalendarType.Gregorian);
                    break;
                case 2:
                    lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(currentNumericDatePatternInfo, DateCalendarType.HijriGhamari));
                    AddPossibleYearNumbers(originalYearNumber, DateCalendarType.HijriGhamari);
                    break;
                default:
                    return;
            }

            if (lstSuggestions.Items.Count > 0)
            {
                lstSuggestions.Items[0].Selected = true;// ; e.SelectedIndex = 0;
            }
            else
            {
                btnChangeAll.Enabled = false;
                btnChange.Enabled = false;
            }
            lstSuggestions.Select();
        }

        private void comboYearNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboYearNumber.Enabled == false || comboYearNumber.Visible == false) return;
            if (lstSuggestions.Items.Count == 0) return;

            int yearNumber = GetSelectedYearNumber();
            if (yearNumber >= 0)
            {
                lstSuggestions.Items.Clear();

                NumericDatePatternInfo newPatternInfo = new NumericDatePatternInfo(
                    currentNumericDatePatternInfo.Content,
                    currentNumericDatePatternInfo.Index,
                    currentNumericDatePatternInfo.Length,
                    currentNumericDatePatternInfo.DayNumber,
                    currentNumericDatePatternInfo.MonthNumber,
                    yearNumber);

                switch (cmboGuessedCalendarType.SelectedIndex)
                {
                    case 0: // i.e. jalali
                        lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(newPatternInfo, DateCalendarType.Jalali));
                        break;
                    case 1:
                        lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(newPatternInfo, DateCalendarType.Gregorian));
                        break;
                    case 2:
                        lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(newPatternInfo, DateCalendarType.HijriGhamari));
                        break;
                    default:
                        return;
                }

                if (lstSuggestions.Items.Count > 0)
                {
                    lstSuggestions.Items[0].Selected = true;// ; e.SelectedIndex = 0;
                }
                else
                {
                    btnChangeAll.Enabled = false;
                    btnChange.Enabled = false;
                }
                lstSuggestions.Select();
            }
        }

        private void comboYearNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                string text = ParsingUtils.ConvertNumber2English(comboYearNumber.Text).Trim();
                int newYear;
                if (Int32.TryParse(text, out newYear))
                {
                    string persianNumber = ParsingUtils.ConvertNumber2Persian(newYear.ToString());
                    if (comboYearNumber.Items.Contains(persianNumber))
                    {
                        comboYearNumber.SelectedItem = persianNumber;
                    }
                    else
                    {
                        comboYearNumber.Items.Add(persianNumber);
                        comboYearNumber.SelectedItem = persianNumber;
                    }
                }
            }
        }

        protected override bool PrepareAlert(BaseVerificationWinArgs args)
        {
            DateVerificationWinArgs dateArgs = (DateVerificationWinArgs)args;
            return this.SetContent(dateArgs.bParagraph, dateArgs.bContent, dateArgs.PatternInfo);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SCICT.NLP.Utility.Calendar;
using SCICT.NLP.Utility.Parsers;
using VirastyarWordAddin.Verifiers.Basics;

namespace VirastyarWordAddin.Verifiers.CustomGUIs.DateSuggestions
{
    public partial class DateSuggestionsViewer : UserControl, ISuggestionsViwer
    {
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

        #region Private Fields

        private const string GroupNameJalali = "listViewGroupJalali";
        private const string GroupNameGregorian = "listViewGroupGregorian";
        private const string GroupNameGhamari = "listViewGroupGhamari";

        int m_originalYearNumber = -1;

        #endregion

        public DateSuggestionsViewer()
        {
            InitializeComponent();
        }

        #region ISuggestionsViwer Members

        public string SelectedSuggestion
        {
            get 
            {
                int n = ListViewSelectedIndex(lstSuggestions);
                if (n >= 0)
                {
                    return lstSuggestions.Items[n].SubItems[0].Text;
                }

                return "";
            }
        }

        private static int ListViewSelectedIndex(ListView listView)
        {
            if (listView.SelectedIndices.Count > 0)
                return listView.SelectedIndices[0];
            return -1;
        }


        public int MainControlTop
        {
            get { return lstSuggestions.Top; }
        }

        public event EventHandler SelectedSuggestionChanged;

        public event EventHandler MainControlTopChanged;

        public event EventHandler SuggestionSelected;

        public event ActionInvokedEventHandler ActionInvoked;

        public IInteractiveVerificationWindow ParentVerificationWindow { get; set; }

        public void Clear()
        {
            lstSuggestions.Items.Clear();
            //cmboGuessedCalendarType.Items.Clear();
            //comboYearNumber.Items.Clear();
        }

        public void SetFocus()
        {
            if (this.lstSuggestions.Items.Count > 0)
            {
                lstSuggestions.Items[0].Selected = true; 
                lstSuggestions.Focus();
            }
        }

        public void SetSuggestions(ISuggestions suggestions)
        {
            var dateSugs = suggestions as DateSuggestion;
            if (dateSugs == null)
            {
                throw new ArgumentException("The specified suggestions object must be of type \"DateSuggestion\".", "suggestions");
            }

            lstSuggestions.Items.Clear();
            lstSuggestions.Items.AddRange(CreateSuggestions(dateSugs.MainPattern));

            if (lstSuggestions.Items.Count > 0)
            {
                this.ParentVerificationWindow.EnableAction(UserSelectedActions.Change);
                lstSuggestions.Items[0].Selected = true;
            }
            else
            {
                this.ParentVerificationWindow.DisableAction(UserSelectedActions.Change);
            }
        }

        #endregion

        #region Suggestionns

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

            var listViewItem = new ListViewItem(new string[] { item, strDateType });
            listViewItem.ToolTipText = item;
            listViewItem.Group = group;

            return listViewItem;
        }


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

            m_originalYearNumber = pi.YearNumber;
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

            m_originalYearNumber = pi.YearNumber;

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
            m_originalYearNumber = pi.YearNumber;
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
            if (this.SuggestionSelected != null)
                this.SuggestionSelected.Invoke(sender, e);

        }

        private void lstSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedSuggestionChanged != null)
                this.SelectedSuggestionChanged.Invoke(sender, e);

        }

        private void lstSuggestions_Resize(object sender, EventArgs e)
        {
            if (this.MainControlTopChanged != null)
                this.MainControlTopChanged.Invoke(sender, e);
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
                    AddPossibleYearNumbers(m_originalYearNumber, DateCalendarType.Jalali);
                    break;
                case 1:
                    lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(currentNumericDatePatternInfo, DateCalendarType.Gregorian));
                    AddPossibleYearNumbers(m_originalYearNumber, DateCalendarType.Gregorian);
                    break;
                case 2:
                    lstSuggestions.Items.AddRange(CreateGeneralDateSuggestions(currentNumericDatePatternInfo, DateCalendarType.HijriGhamari));
                    AddPossibleYearNumbers(m_originalYearNumber, DateCalendarType.HijriGhamari);
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
                ParentVerificationWindow.DisableAction(UserSelectedActions.Change);
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

                var newPatternInfo = new NumericDatePatternInfo(
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
                    ParentVerificationWindow.DisableAction(UserSelectedActions.Change);
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

        private IButtonControl m_parentAcceptButton = null;
        private void comboYearNumber_Leave(object sender, EventArgs e)
        {
            if (m_parentAcceptButton != null)
                this.ParentVerificationWindow.AcceptButton = m_parentAcceptButton;
        }

        private void comboYearNumber_Enter(object sender, EventArgs e)
        {
            m_parentAcceptButton = this.ParentVerificationWindow.AcceptButton;
            this.ParentVerificationWindow.AcceptButton = null;
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SCICT.NLP.Utility;
using SCICT.NLP.Persian.Constants;
using VirastyarWordAddin.Verifiers.Basics;

namespace VirastyarWordAddin.Verifiers.CustomGUIs.TitledListBox
{
    public partial class TitledListBoxSuggestionViewer : UserControl, ISuggestionsViwer
    {
        public TitledListBoxSuggestionViewer()
        {
            InitializeComponent();
        }

        #region ISuggestionsViwer Members


        public string SelectedSuggestion
        {
            get
            {
                if (lbSuggestions.Items.Count > 0 && lbSuggestions.SelectedIndex >= 0)
                {
                    string selectedSug = "";
                    object o = lbSuggestions.SelectedItem;
                    if (o is TextValuePair)
                    {
                        selectedSug = (o as TextValuePair).Value.ToString();
                    }
                    else
                    {
                        selectedSug = lbSuggestions.SelectedItem.ToString();
                    }

                    return selectedSug;
                }
                else
                    return "";
            }
        }


        public int MainControlTop
        {
            get { return this.lbSuggestions.Top; }
        }

        public event EventHandler SelectedSuggestionChanged;

        public event EventHandler MainControlTopChanged;

        public event EventHandler SuggestionSelected;

        public event ActionInvokedEventHandler ActionInvoked;

        public IInteractiveVerificationWindow ParentVerificationWindow { get; set; }

        public void SetSuggestions(ISuggestions suggestions)
        {
            if (!(suggestions is TitledListBoxSuggestion))
            {
                throw new ArgumentException("The specified suggestions object must be of type \"TitledListBoxSuggestion\".", "suggestions");
            }

            var theSugs = suggestions as TitledListBoxSuggestion;

            lblTitle.Text = theSugs.Message;
            lbSuggestions.Items.Clear();
            lbSuggestions.Items.AddRange(RefineSuggestionsText(theSugs.SuggestionItems.ToArray()));
        }

        public static object[] RefineSuggestionsText(string[] sugs)
        {
            object[] os = new object[sugs.Length];
            for (int i = 0; i < os.Length; ++i)
            {
                if(String.IsNullOrEmpty(sugs[i]))
                {
                    os[i] = new TextValuePair("(حذف)", sugs[i]);
                }
                else if (StringUtil.StringContainsAny(sugs[i],
                    WordSpecialCharacters.SpecialCharsArray))
                {
                    os[i] = new TextValuePair(
                        sugs[i].Replace(
                            WordSpecialCharacters.FootnoteDelimiter.ToString(),
                            WordSpecialCharacters.FootnoteDelimiterReplacementString)
                        .Replace(
                            WordSpecialCharacters.FormulaDelimiter.ToString(),
                            WordSpecialCharacters.FormulaDelimiterReplacementString)
                            ,
                        sugs[i]
                        );
                }
                else
                {
                    os[i] = sugs[i];
                }
            }

            return os;
        }


        #endregion

        private void lbSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedSuggestionChanged != null)
                this.SelectedSuggestionChanged.Invoke(sender, e);
        }

        private void lbSuggestions_Resize(object sender, EventArgs e)
        {
            if (this.MainControlTopChanged != null)
                this.MainControlTopChanged.Invoke(sender, e);
        }


        private void lbSuggestions_DoubleClick(object sender, EventArgs e)
        {
            if (this.SuggestionSelected != null)
                this.SuggestionSelected.Invoke(sender, e);
        }

        #region ISuggestionsViwer Members


        public void Clear()
        {
            this.lbSuggestions.Items.Clear();
        }

        public void SetFocus()
        {
            if (this.lbSuggestions.Items.Count > 0)
            {
                lbSuggestions.SelectedIndex = 0;
                lbSuggestions.Focus();
            }
        }

        #endregion

    }
}

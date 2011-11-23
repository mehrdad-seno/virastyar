using System;
using System.Linq;
using System.Windows.Forms;
using SCICT.NLP.Utility;
using SCICT.NLP.Persian.Constants;
using SCICT.Utility.GUI;
using VirastyarWordAddin.Verifiers.Basics;

namespace VirastyarWordAddin.Verifiers.CustomGUIs.SpellCheckerSuggestions
{
    public partial class SpellCheckerSuggestionViewer : UserControl, ISuggestionsViwer
    {
        private bool m_disableChange = false;

        public SpellCheckerSuggestionViewer()
        {
            InitializeComponent();
        }

        #region ISuggestionsViwer Members

        public string SelectedSuggestion
        {
            get
            {
                return txtCustomSuggestions.Text;

                //if (lbSuggestions.Items.Count > 0 && lbSuggestions.SelectedIndex >= 0)
                //{
                //    string selectedSug = "";
                //    object o = lbSuggestions.SelectedItem;
                //    if (o is TextValuePair)
                //    {
                //        selectedSug = (o as TextValuePair).Value.ToString();
                //    }
                //    else
                //    {
                //        selectedSug = lbSuggestions.SelectedItem.ToString();
                //    }

                //    return selectedSug;
                //}
                //else
                //    return "";
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
            var theSugs = suggestions as SpellCheckerSuggestion;
            if (theSugs == null)
            {
                throw new ArgumentException("The specified suggestions object must be of type \"TitledListBoxSuggestion\".", "suggestions");
            }

            lblTitle.Text = theSugs.Message;
            lbSuggestions.Items.Clear();
            lbSuggestions.Items.AddRange(RefineSuggestionsText(theSugs.SuggestionItems.ToArray()));

            if (lbSuggestions.Items.Count > 0)
            {
                m_disableChange = false;

                lbSuggestions.SelectedIndex = 0;

                this.ParentVerificationWindow.EnableAction(UserSelectedActions.Change);
                this.ParentVerificationWindow.EnableAction(UserSelectedActions.ChangeAll);
            }
            else
            {
                m_disableChange = true;

                this.ParentVerificationWindow.DisableAction(UserSelectedActions.Change);
                this.ParentVerificationWindow.DisableAction(UserSelectedActions.ChangeAll);

                txtCustomSuggestions.Text = "";
            }

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
            this.txtCustomSuggestions.Text = lbSuggestions.SelectedItem.ToString();

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

        private void txtCustomSuggestions_TextChanged(object sender, EventArgs e)
        {
            if (m_disableChange)
            {
                string curText = ((TextBox)sender).Text.Trim();
                if (!String.IsNullOrEmpty(curText))
                {
                    this.ParentVerificationWindow.EnableAction(UserSelectedActions.Change);
                    this.ParentVerificationWindow.EnableAction(UserSelectedActions.ChangeAll);
                }
                else
                {
                    this.ParentVerificationWindow.DisableAction(UserSelectedActions.Change);
                    this.ParentVerificationWindow.DisableAction(UserSelectedActions.ChangeAll);
                }
            }

        }

        private void txtCustomSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.Shift) && ((e.KeyCode == Keys.D2) || (e.KeyCode == Keys.NumPad2)))
            {
                var tb = sender as TextBox;
                if (tb == null) return;
                tb.Paste(PseudoSpace.ZWNJ.ToString());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        private void btnAddCustomToDic_Click(object sender, EventArgs e)
        {
            string customSuggestion = StringUtil.RefineAndFilterPersianWord(txtCustomSuggestions.Text.Trim());
            if (customSuggestion.Length <= 0)
            {
                PersianMessageBox.Show(GetWin32Window(), "عبارت درج شده خالی است!");
                return;
            }

            if (customSuggestion.IndexOf(' ') >= 0)
            {
                PersianMessageBox.Show(GetWin32Window(), "لطفاً از عبارت تک کلمه‌ای استفاده کنید.");
                return;
            }

            if (ActionInvoked != null)
            {
                ActionInvoked.Invoke(this, UserSelectedActions.AddToDictionary, customSuggestion);
            }

        }

        public IWin32Window GetWin32Window()
        {
            return new WindowWrapper(Handle);
        }
    }
}

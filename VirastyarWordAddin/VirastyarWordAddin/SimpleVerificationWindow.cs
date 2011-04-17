using System;
using System.Drawing;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;

namespace VirastyarWordAddin
{
    public partial class SimpleVerificationWindow : VerificationWindowBase
    {
        #region Constructors

        public SimpleVerificationWindow()
        {
            IsChangeAllPossible = true;

            System.Drawing.Point oldPanelProgressLocation = panelProgressMode.Location;

            InitializeComponent();
            PostInitComponent();

            this.panelProgressMode.Location = oldPanelProgressLocation;
        }

        private void PostInitComponent()
        {
            this.lblSuggestionCaption.Size = new System.Drawing.Size(463, 23);
            this.lstSuggestions.ItemHeight = 16;
            this.lstSuggestions.Size = new System.Drawing.Size(463, 196);

            this.lblSuggestionCaption.TextAlign = ContentAlignment.BottomLeft;
        }

        #endregion

        public override string SelectedSuggestion
        {
            get
            {
                if (lstSuggestions.Items.Count > 0 && lstSuggestions.SelectedIndex >= 0)
                {
                    string selectedSug = "";
                    object o = lstSuggestions.SelectedItem;
                    if (o is TextValuePair)
                    {
                        selectedSug = (o as TextValuePair).Value.ToString();
                    }
                    else
                    {
                        selectedSug = lstSuggestions.SelectedItem.ToString();
                    }

                    return selectedSug;
                }
                else
                    return "";
            }
        }

        public bool IsChangeAllPossible { get; set; }

        protected bool SetContent(Range bParagraph, Range bContent, string[] sugs)
        {
            if (!base.SetContent(bParagraph, bContent))
            {
                return false;
            }

            lstSuggestions.Items.Clear();
            lstSuggestions.Items.AddRange(RefineSuggestionsText(sugs));

            if (lstSuggestions.Items.Count > 0)
            {
                lstSuggestions.SelectedIndex = 0;
                btnChange.Enabled = CanEnableButton(VerificationWindowButtons.Change);
                btnChangeAll.Enabled = CanEnableButton(VerificationWindowButtons.ChangeAll);
                if (btnChangeAll.Enabled && !IsChangeAllPossible)
                    btnChangeAll.Enabled = false;
            }
            else
            {
                btnChange.Enabled = false;
                btnChangeAll.Enabled = false;
//                DisableButtions(VerificationWindowButtons.Change | VerificationWindowButtons.ChangeAll);
            }
            lstSuggestions.Select();
            return true;
        }

        private object[] RefineSuggestionsText(string[] sugs)
        {
            object[] os = new object[sugs.Length];
            for (int i = 0; i < os.Length; ++i)
            {
                if (StringUtil.StringContainsAny(sugs[i],
                    WordSpecialCharacters.SpecialCharsArray))
                {
                    os[i] = new TextValuePair(sugs[i].Replace(
                        WordSpecialCharacters.FootnoteDelimiter.ToString(),
                        WordSpecialCharacters.FootnoteDelimiterReplacementString)
                        .Replace(
                        WordSpecialCharacters.FormulaDelimiter.ToString(),
                        WordSpecialCharacters.FormulaDelimiterReplacementString),

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

        protected override bool PrepareAlert(BaseVerificationWinArgs args)
        {
            BaseVerificationWinArgsWithSugs spellArgs = (BaseVerificationWinArgsWithSugs)args;
            return SetContent(spellArgs.bParagraph, spellArgs.bContent, spellArgs.Sugs);
        }

        private void lstSuggestions_DoubleClick(object sender, EventArgs e)
        {
            OnChangeButtonPressed();
        }

        public void SetSuggestionCaption(string description)
        {
            lblSuggestionCaption.Text = description;
        }

        protected override void AfterConfirmationModeDesignChanges()
        {
            base.AfterConfirmationModeDesignChanges();
        }

        protected override void AfterProgressModeDesignChanges()
        {
            base.AfterProgressModeDesignChanges();
        }

        protected override void AfterVerificationModeDesignChanges()
        {
            base.AfterVerificationModeDesignChanges();
        }

        private class TextValuePair
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public TextValuePair(string text, object value)
            {
                this.Text = text;
                this.Value = value;
            }

            public TextValuePair(string text)
                : this(text, text)
            {
            }

            public TextValuePair()
                : this("")
            {
            }

            public override string ToString()
            {
                return this.Text;
            }
        }
    }
}

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

            Point oldPanelProgressLocation = panelProgressMode.Location;

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

                    Globals.ThisAddIn.UsageLogger.SetSelectedSuggestions(
                        selectedSug, lstSuggestions.SelectedIndex);
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

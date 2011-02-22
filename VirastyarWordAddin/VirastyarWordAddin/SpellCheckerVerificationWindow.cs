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
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Utility;
using SCICT.Utility.GUI;
using System.Collections.Generic;

namespace VirastyarWordAddin
{
    public partial class SpellCheckerVerificationWindow : VerificationWindowBase
    {
        #region Constructors

        public SpellCheckerVerificationWindow()
        {
            IsCustomAddToDicPressed = false;

            Point oldPanelProgressLocation = panelProgressMode.Location;

            InitializeComponent();
            PostInitComponent();

            this.panelProgressMode.Location = oldPanelProgressLocation;
        }

        private void PostInitComponent()
        {
            //this.lblSuggestionCaption.Size = new System.Drawing.Size(200, 20);
            this.lstSuggestions.ItemHeight = 16;
            this.lstSuggestions.Size = new System.Drawing.Size(463, 196 - 30);

            //lblSuggestionCaption.Left -= 12;
            this.btnChange.Top += 7;
            this.btnChangeAll.Top += 7;

            pnlRight.Height = pnlRight.Height - 22;
            pnlLeft.Height = pnlLeft.Height - 22;

            statusSugs.Visible = true;
            statusSugs.BringToFront();

            //this.lblSuggestionCaption.TextAlign = ContentAlignment.BottomLeft;
        }

        #endregion

        public bool IsCustomAddToDicPressed { get; private set; }

        public override string SelectedSuggestion
        {
            get
            {
                //Globals.ThisAddIn.UsageLogger.SetSelectedSuggestions(
                //    this.txtCustomSuggestion.Text, lstSuggestions.SelectedIndex);
                return this.txtCustomSuggestion.Text;
                //return lstSuggestions.SelectedItem as string;
            }
        }

        Dictionary<string, string> m_rankingDetail = null;
        private bool m_disableChange = false;

        protected bool SetContent(Range bParagraph, Range bContent, string[] sugs)
        {
            this.IsCustomAddToDicPressed = false;

            if (!base.SetContent(bParagraph, bContent))
            {
                return false;
            }

            lstSuggestions.Items.Clear();
            lstSuggestions.Items.AddRange(sugs);

            m_rankingDetail = (this.Verifier as SpellCheckerVerifier).CurrentRankingDetail;
            statusSugsLabel1.Text = "";
            statusSugsLabel2.Text = "";

            if (lstSuggestions.Items.Count > 0)
            {
                m_disableChange = false;

                lstSuggestions.SelectedIndex = 0;

                btnChange.Enabled = CanEnableButton(VerificationWindowButtons.Change);
                btnChangeAll.Enabled = CanEnableButton(VerificationWindowButtons.ChangeAll);
            }
            else
            {
                m_disableChange = true;
                btnChange.Enabled = false;
                btnChangeAll.Enabled = false;
                txtCustomSuggestion.Text = "";
                //DisableButtions(VerificationWindowButtons.Change | VerificationWindowButtons.ChangeAll);
            }
//                btnChange.Enabled = CanEnableButton(VerificationWindowButtons.Change);
//                btnChangeAll.Enabled = CanEnableButton(VerificationWindowButtons.ChangeAll);
//            }
//            else
//            {
//                btnChange.Enabled = false;
//                btnChangeAll.Enabled = false;
//                txtCustomSuggestion.Text = "";
////                DisableButtions(VerificationWindowButtons.Change | VerificationWindowButtons.ChangeAll);
//            }
            lstSuggestions.Select();
            return true;
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

        private void lstSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSuggestions.Items.Count > 0 && lstSuggestions.SelectedItem != null)
            {
                this.txtCustomSuggestion.Text = lstSuggestions.SelectedItem.ToString();
                if (m_rankingDetail != null)
                {
                    string details;
                    if (m_rankingDetail.TryGetValue(lstSuggestions.SelectedItem.ToString(), out details))
                    {
                        string[] elems = details.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (elems.Length < 1)
                        {
                            statusSugsLabel1.Text = "جزئیات برای این کلمه در دست نیست!";
                            statusSugsLabel2.Text = "";
                        }
                        else if (elems.Length == 1)
                        {
                            statusSugsLabel1.Text = elems[0];
                            statusSugsLabel2.Text = "";
                        }
                        else
                        {
                            statusSugsLabel1.Text = String.Format("میزان کاربرد: {0}", elems[0]);

                            double dSim = 0.0;
                            if (Double.TryParse(elems[1], out dSim))
                            {
                                dSim = Math.Round(dSim, 5);
                                statusSugsLabel2.Text = String.Format("شباهت ظاهری: {0}", dSim);
                            }
                            else
                            {
                                statusSugsLabel2.Text = String.Format("شباهت ظاهری: {0}", elems[1]);
                            }
                        }
                    }
                    else
                    {
                        statusSugsLabel1.Text = "جزئیات برای این کلمه در دست نیست!";
                        statusSugsLabel2.Text = "";
                    }
                }
                else
                {
                    statusSugsLabel1.Text = "جزئیات در دست نیست!";
                    statusSugsLabel2.Text = "";
                }
            }
        }

        private void btnAddCustomToDic_Click(object sender, EventArgs e)
        {
            string customSuggestion = StringUtil.RefineAndFilterPersianWord(txtCustomSuggestion.Text.Trim());
            if (customSuggestion.Length <= 0)
            {
                PersianMessageBox.Show("عبارت درج شده خالی است!");
                return;
            }

            if (customSuggestion.IndexOf(' ') >= 0)
            {
                PersianMessageBox.Show("لطفاً از عبارت تک کلمه‌ای استفاده کنید.");
                return;
            }

            if (((SpellCheckerVerifier)this.Verifier).AddToDictionary(customSuggestion))
            {
                IsCustomAddToDicPressed = true;
                OnIgnoreButtonPressed();
            }
        }

        private void txtCustomSuggestion_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.Shift) && ((e.KeyCode == Keys.D2) || (e.KeyCode == Keys.NumPad2)))
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;
                tb.Paste(PseudoSpace.ZWNJ.ToString());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtCustomSuggestion_TextChanged(object sender, EventArgs e)
        {
            if(m_disableChange)
            {
                string curText = (sender as TextBox).Text.Trim();
                if (!String.IsNullOrEmpty(curText))
                {
                    btnChange.Enabled = CanEnableButton(VerificationWindowButtons.Change);
                    btnChangeAll.Enabled = CanEnableButton(VerificationWindowButtons.ChangeAll);
                }
                else
                {
                    btnChange.Enabled = false;
                    btnChangeAll.Enabled = false;
                }
            }
            
            
        }
    }
}

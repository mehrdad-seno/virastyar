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

            System.Drawing.Point oldPanelProgressLocation = panelProgressMode.Location;

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

            m_rankingDetail = ((SpellCheckerVerifier)Verifier).CurrentRankingDetail;
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
#if DEBUG
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
#endif
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

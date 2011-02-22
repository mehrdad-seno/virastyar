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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SCICT.NLP.Utility.LanguageModel;
using VirastyarWordAddin.Properties;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Persian;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.Utility.GUI;
using SCICT.Utility.Keyboard;
using System.Threading;
using SCICT.Microsoft.Word;
using SCICT.Utility;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Office.Interop.Word;

namespace VirastyarWordAddin.Configurations
{
    /// <summary>
    /// GUI to modify various parameters of Addin
    /// </summary>
    public partial class AddinConfigurationDialog : Form, IAddinConfigurationDialog
    {
        const string MainDicDescription = "واژه‌نامه‌ی ویراستیار";
        const string CustomDicDescription = "(واژه‌نامه‌ی کاربر)";

        #region Private Members

        //private bool isSettingChanged_SpellCheck = false;

        private readonly Settings settings = null;
        //private readonly IHotkeyEngine hotkeyEngine = null;
        private AllCharactersRefinerSettings allCharactersRefinerSettings = null;

        #endregion

        #region Events

        public event EventHandler RepairMenubarsClicked;
        public event SpellCheckSettingsChangedEventHandler SpellCheckSettingsChanged;
        //public event ShortcutChangedEventHandler ShortcutChanged;
        public event RefineAllSettingsChangedEventHandler RefineAllSettingsChanged;

        #endregion

        #region Constructors

        internal AddinConfigurationDialog()
        {
            InitializeComponent();
            tabCtrlSettings.TabPages.Remove(tbPageWords);
        }

        internal AddinConfigurationDialog(Settings settings)
            : this()
        {
            this.settings = settings;
            //this.hotkeyEngine = hotkeyEngine;
            this.allCharactersRefinerSettings = new AllCharactersRefinerSettings(settings);
            LoadConfigurations();
        }

        #endregion

        #region Save, Load

        /// <summary>
        /// Update UI elements based on the current settings
        /// </summary>
        public void LoadConfigurations()
        {
            // TODO: Load Configuration and update UI elements
            UpdateSpellCheckSettingsUI();
            UpdateShortcutsSettingsUI();
            UpdateAllCharsRefinerSettingsUI();
            UpdateSpellCheckRefinementUI();
            UpdateWordCompletionSettingsUI();
        }

        private void SaveConfigurations()
        {
            #region Spell-Check Settings

            SpellCheckerConfig spellCheckerSettings = GetSpellCheckSettings();

            if (OnSpellCheckSettingsChanged(spellCheckerSettings))
            {
                settings.Config_UserDictionaryPath = spellCheckerSettings.DicPath;
                settings.Config_EditDistance = (uint)spellCheckerSettings.EditDistance;
                settings.Config_MaxSuggestions = (uint)spellCheckerSettings.SuggestionCount;

                settings.SpellChecker_VocabSpaceCorrection = cbVocabSpaceCorrection.Checked;
                settings.SpellChecker_DontCheckSingleLetters = cbDontCheckSingleLetters.Checked;
                settings.SpellChecker_HeYeConvertion = cbHaaShisakiToHaaYaa.Checked;

                settings.PreprocessSpell_RefineHaa = cbRefineHaa.Checked;
                settings.PreprocessSpell_RefineMee = cbRefineMee.Checked;
                settings.PreprocessSpell_RefineHeYe = cbRefineHeYe.Checked;
                settings.PreprocessSpell_RefineBe = cbRefineBe.Checked;
                settings.PreprocessSpell_RefineAllAffixes = 
                    (cbRefineAllAffixes.CheckState == CheckState.Checked) ? true : false;

                Debug.Assert(listViewUserDictionaries.Items.Count > 0);

                if (listViewUserDictionaries.Items.Count > 0)
                {
                    settings.SpellChecker_MainDictionaryPath = GetFileNameFromItem(listViewUserDictionaries.Items[0]);
                    settings.SpellChecker_MainDictionarySelected = listViewUserDictionaries.Items[0].Checked;

                    StringBuilder sbPaths = new StringBuilder();
                    StringBuilder sbDescs = new StringBuilder();
                    int selectionFlags = 0;
                    for (int i = 1; i < listViewUserDictionaries.Items.Count; ++i)
                    {
                        sbPaths.AppendFormat("{0};", GetFileNameFromItem(listViewUserDictionaries.Items[i]));
                        sbDescs.AppendFormat("{0};", GetDescriptionFromItem(listViewUserDictionaries.Items[i]));

                        if (listViewUserDictionaries.Items[i].Checked)
                            selectionFlags = selectionFlags | (1 << (i - 1));
                    }

                    settings.SpellChecker_CustomDictionaries = sbPaths.ToString();
                    settings.SpellChecker_CustomDictionariesDescription = sbDescs.ToString();
                    settings.SpellChecker_CustomDictionariesSelectionFlag = selectionFlags;
                }
            }
            //}

            #endregion

            #region Storing All-Chars-Refiner settings

            allCharactersRefinerSettings = GetAllCharsRefinerSettings();
            settings.RefineCategoriesFlag = (int)allCharactersRefinerSettings.NotIgnoredCategories;
            settings.RefineIgnoreListConcated = allCharactersRefinerSettings.GetIgnoreListAsString();
            settings.RefineHalfSpacePositioning = allCharactersRefinerSettings.RefineHalfSpacePositioning;

            #endregion

            #region Storing Word-Completion Settings

            UpdateWordCompletionSettings();
            settings.WordCompletionCompleteWithoutHotKey = cbWCCompleteWithoutHotkey.Checked;
            settings.WordCompletionInsertSpace = cbWCInsertSpace.Checked;
            if (rbWCShowAllWords.Checked)
                settings.WordCompletionSugCount = -1;
            else
                settings.WordCompletionSugCount = (int)numUpDownWCWordCount.Value;

            settings.WordCompletionMinWordLength = (int)numUpDownWCMinWordLength.Value;
            settings.WordCompletionFontSize = (int)numUpDownWCFontSize.Value;

            #endregion

            settings.Save();

            OnRefineAllCharactersSettingsChanged(allCharactersRefinerSettings);
        }

        private SpellCheckerConfig GetSpellCheckSettings()
        {
            SpellCheckerConfig spellCheckSettings = new SpellCheckerConfig(txtFileName.Text,
                (int)nmrEditDistance.Value, (int)nmrMaxSuggestionsCount.Value);
            spellCheckSettings.StemPath = SettingsHelper.GetFullPath(Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles);
            return spellCheckSettings;
        }

        private void OnRefineAllCharactersSettingsChanged(AllCharactersRefinerSettings refineAllCharSettings)
        {
            if (RefineAllSettingsChanged != null)
            {
                RefineAllSettingsChangedEventArgs e = new RefineAllSettingsChangedEventArgs();
                e.Settings = refineAllCharSettings;
                RefineAllSettingsChanged(e);
            }
        }

        #endregion

        #region Ok, Apply, Cancel

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            errorProvider.Clear();

            try
            {
                SaveConfigurations();
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.OnExceptionOccured(ex);
            }

            bool error = false;
            error |= !string.IsNullOrEmpty(errorProvider.GetError(txtFileName));
//            error |= !string.IsNullOrEmpty(errorProvider.GetError(gbWCHotkey));
            error |= !string.IsNullOrEmpty(errorProvider.GetError(linkLabelSpellCheckerCreateDictionary));
            //error |= string.IsNullOrEmpty(errorProvider.GetError(lstShortcuts));

            if (!error)
                Close();

            this.Cursor = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //errorProvider.Clear();
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            // Test
            // settings.Config_Shortcut_PinglishCheck = "";
        }

        #endregion

        #region Spell Check Settings

        private void UpdateSpellCheckSettingsUI()
        {
            txtFileName.Text = settings.Config_UserDictionaryPath;
            nmrEditDistance.Value = settings.Config_EditDistance;
            if (settings.Config_MaxSuggestions <= 0)
                settings.Config_MaxSuggestions = 1;

            cbVocabSpaceCorrection.Checked = settings.SpellChecker_VocabSpaceCorrection;
            cbDontCheckSingleLetters.Checked = settings.SpellChecker_DontCheckSingleLetters;
            cbHaaShisakiToHaaYaa.Checked = settings.SpellChecker_HeYeConvertion;

            nmrMaxSuggestionsCount.Value = settings.Config_MaxSuggestions;

            listViewUserDictionaries.Items.Clear();

            var item = new ListViewItem(new string[] 
            { 
                MainDicDescription, 
                settings.SpellChecker_MainDictionaryPath.Trim() 
            });
            listViewUserDictionaries.Items.Add(item);
            item.Checked = settings.SpellChecker_MainDictionarySelected;
            AddCustomDictionaryFiles(settings.SpellChecker_CustomDictionaries, settings.SpellChecker_CustomDictionariesDescription, settings.SpellChecker_CustomDictionariesSelectionFlag);
        }

        private void AddCustomDictionaryFiles(string dictionaryPaths, string dictionaryDescs, long selectionFlags)
        {
            string[] paths = dictionaryPaths.Split(';');
            string[] descs = new string[paths.Length];

            string[] readDescs = dictionaryDescs.Split(';');
            for (int i = 0; i < paths.Length; ++i)
            {
                if (i < readDescs.Length)
                {

                    descs[i] = readDescs[i].Length > 0 ? readDescs[i] : CustomDicDescription;
                }
                else
                {
                    descs[i] = CustomDicDescription;
                }
            }

            for (int i = 0; i < paths.Length; ++i)
            {
                string path = paths[i].Trim();
                if (path.Length <= 0) continue;
                ListViewItem item = new ListViewItem(new string[] { descs[i], path });
                listViewUserDictionaries.Items.Add(item);
                if (((1 << i) & selectionFlags) > 0)
                    item.Checked = true;
                else
                    item.Checked = false;
            }
        }

        private void linkLabelSpellCheckerAddExistingDic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                string fileName = dlg.FileName;
                //listViewUserDictionaries.Items.Con
                foreach (ListViewItem item in listViewUserDictionaries.Items)
                {
                    if (GetFileNameFromItem(item) == fileName)
                    {
                        PersianMessageBox.Show("این فایل قبلاً افزوده شده است.");
                        return;
                    }
                }

                var lvitem = listViewUserDictionaries.Items.Add(new ListViewItem(new string[] { CustomDicDescription, dlg.FileName }));
                lvitem.Checked = true;
            }
        }

        private void linkLabelDeleteItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (int index in listViewUserDictionaries.SelectedIndices)
            {
                if (index == 0)
                {
                    PersianMessageBox.Show("فایل اصلی واژه‌نامه را نمی‌توانید حذف کنید.");
                }
                else
                {
                    listViewUserDictionaries.Items.RemoveAt(index);
                }
            }
        }

        private string GetFileNameFromItem(ListViewItem item)
        {
            return item.SubItems[1].Text;
        }

        private string GetDescriptionFromItem(ListViewItem item)
        {
            return item.SubItems[0].Text.Replace(';', ',').Trim();
        }

        private void linkLabelSpellCheckerCreateDictionary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MSWordDocument doc = Globals.ThisAddIn.ActiveMSWordDocument;
            if (doc == null)
            {
                PersianMessageBox.Show("در حال حاضر سندی باز نیست.");
                return;
            }

            string fileName = "";
            bool doAppend = false;
            if (listViewUserDictionaries.SelectedIndices != null && listViewUserDictionaries.SelectedIndices.Count > 0)
            {
                int selectedIndex = listViewUserDictionaries.SelectedIndices[0];
                if (selectedIndex > 0)
                {
                    string selectedDescription = GetDescriptionFromItem(listViewUserDictionaries.Items[selectedIndex]);
                    string selectedDicPath = GetFileNameFromItem(listViewUserDictionaries.Items[selectedIndex]);

                    DialogResult dlgRes = PersianMessageBox.Show("واژه‌نامه تولید شده به سند زیر افزوده شود؟" + "\r\n" + "عنوان واژه‌نامه: " + selectedDescription + "\r\n" + "مسیر: " + selectedDicPath, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dlgRes == DialogResult.Cancel) return;
                    doAppend = (dlgRes == DialogResult.Yes);
                    if (doAppend)
                        fileName = selectedDicPath;
                }
            }

            if (!doAppend)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Dictionary|*.dat";

                if (DialogResult.OK != dlg.ShowDialog())
                {
                    return;
                }

                fileName = dlg.FileName;
                try
                {
                    StreamWriter sw = File.CreateText(fileName);
                    sw.Close();
                }
                catch
                {
                    PersianMessageBox.Show("فایل انتخاب شده قابل نوشتن نیست.");
                    return;
                }
            }

            this.Cursor = Cursors.WaitCursor;

            //LanguageModel languageModel = new LanguageModel();
            //languageModel.AddPlainText(doc.CurrentMSDocument.Content.Text);


            try
            {
                //languageModel.SaveOnDisk(fileName, doAppend);
                try
                {
                    this.Enabled = false;
                    this.progressBarBuildDictionary.Visible = true;

                    // it is forced and should not be ignored
                    System.Windows.Forms.Application.DoEvents();

                    POSTaggedDictionaryExtractor dictionaryExtractor = new POSTaggedDictionaryExtractor();
                    string content = doc.CurrentMSDocument.Content.Text;
                    string mainDictionary = Globals.ThisAddIn.SpellCheckerWrapper.MainDictionary;

                    Thread thread = new Thread(new ThreadStart(delegate
                    {

                        dictionaryExtractor.AddPlainText(content);
                        dictionaryExtractor.AppendExternalPOSTaggedDictionary(mainDictionary);
                        dictionaryExtractor.ExtractPOSTaggedDictionary(fileName);
                    }
                    ));
                    thread.Start();

                    while (thread.IsAlive)
                    {
                        this.progressBarBuildDictionary.Value = Math.Min((int)(dictionaryExtractor.ProgressPercent * 100), 100);
                        // it is also forced!
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(100);
                    }

                    this.progressBarBuildDictionary.Value = 100;
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(250);
                }
                finally
                {
                    this.progressBarBuildDictionary.Visible = false;
                    this.Enabled = true;
                }

                if (!doAppend)
                {
                    foreach (ListViewItem item in listViewUserDictionaries.Items)
                    {
                        if (GetFileNameFromItem(item) == fileName)
                        {
                            PersianMessageBox.Show("فایل انتخاب شده قبلاً افزوده شده است.");
                            return;
                        }
                    }
                    var lstViewItem = listViewUserDictionaries.Items.Add(new ListViewItem(new string[] { CustomDicDescription, fileName }));
                    lstViewItem.Selected = true;
                }
            }
            catch (IOException)
            {
                PersianMessageBox.Show("فایل انتخاب شده قابل نوشتن نیست.");
                return;
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            if (d.ShowDialog() != DialogResult.Cancel)
            {
                if (txtFileName.Text != d.FileName)
                {
                    //isSettingChanged_SpellCheck = true;
                    txtFileName.Text = d.FileName;
                }
            }
        }

        private void chkShowAllSuggestions_CheckedChanged(object sender, EventArgs e)
        {
            // nmrMaxSuggestionsCount.Enabled = !chkShowAllSuggestions.Checked;
        }

        private void nmrMaxSuggestionsCount_ValueChanged(object sender, EventArgs e)
        {
            //if (settings.Config_MaxSuggestions != nmrMaxSuggestionsCount.Value)
            //isSettingChanged_SpellCheck = true;
        }

        private void nmrEditDistance_ValueChanged(object sender, EventArgs e)
        {
            //if (settings.Config_EditDistance != nmrEditDistance.Value)
            //isSettingChanged_SpellCheck = true;
        }

        private void listViewUserDictionaries_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Item == 0)
                e.CancelEdit = true;
        }

        #endregion

        #region Words Tab

        // TODO: This tab and all its functionalities were removed.

        private void lnkImpolite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*WordEntryWindow win = new WordEntryWindow();
            win.LoadType(Common.WordType.Impolite);
            win.ShowDialog();*/
        }

        private void lnkOral_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*WordEntryWindow win = new WordEntryWindow();
            win.LoadType(Common.WordType.Oral);
            win.ShowDialog();*/
        }

        private void lnkVulgar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*WordEntryWindow win = new WordEntryWindow();
            win.LoadType(Common.WordType.Vulgar);
            win.ShowDialog();*/
        }

        private void lnkObsolete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*WordEntryWindow win = new WordEntryWindow();
            win.LoadType(Common.WordType.Obsolete);
            win.ShowDialog();*/
        }

        private void lnkPersian_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*WordEntryWindow win = new WordEntryWindow();
            win.LoadType(Common.WordType.Persian);
            win.ShowDialog();*/
        }

        private void lnkRhyme_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*RimeEntryWindow win = new RimeEntryWindow();
            win.ShowDialog();*/
        }

        #endregion

        #region Shortcut Tab

        private void lstShortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstShortcuts.SelectedItems.Count <= 0)
                return;

            ListViewItem selectedItem = lstShortcuts.SelectedItems[0];
            Hotkey hotkey = Hotkey.None;

            // TODO: Check for null values

            string strKey = settings.PropertyValues["Config_Shortcut_" + selectedItem.SubItems[2].Text].PropertyValue as string;
            Hotkey.TryParse(strKey, out hotkey);
            hotkeyControl.Hotkey = hotkey;

            lblShortcutDesc.Text = string.Format("\n{0}:{1}\n\n{2}:{3}\n",
                            "نام", selectedItem.SubItems[0].Text,
                            "میانبر فعلی", hotkey);
        }

        private void btnAssignShortcut_Click(object sender, EventArgs e)
        {
            Hotkey newHotkey = GetNewShortcutForSelectedItem();
            Hotkey oldHotkey = GetOldShortcutForSelectedItem();

            if (lstShortcuts.SelectedItems.Count <= 0)
            {
                PersianMessageBox.Show("لطفاً یک دستور را انتخاب نمایید");
                return;
            }

            ListViewItem selectedItem = lstShortcuts.SelectedItems[0];
            string configKey = selectedItem.SubItems[2].Text;

            if (Globals.ThisAddIn.TryChangeHotkey(configKey, newHotkey, oldHotkey, true))
            {
                selectedItem.SubItems[1].Text = newHotkey.ToString();
            }
            else
            {
                selectedItem.SubItems[1].Text = oldHotkey.ToString();
            }
        }

        private void btnClearHotkey_Click(object sender, EventArgs e)
        {
            var hotkey = hotkeyControl.Hotkey;
            if (hotkey == Hotkey.None)
                return;

            ListViewItem selectedItem = lstShortcuts.SelectedItems[0];
            string configKey = selectedItem.SubItems[2].Text;

            WordHotKey.RemoveAssignedKey(Globals.ThisAddIn.VirastyarTemplate, hotkey, configKey);
            settings["Config_Shortcut_" + configKey] = "";
            selectedItem.SubItems[1].Text = "";
            hotkeyControl.Text = "";
        }


        private void hotkeyControl_HotkeyChanged(object sender, EventArgs e)
        {
            string prevCommand;
            var newHotkey = hotkeyControl.Hotkey;
            if (newHotkey.Key == Keys.None)
            {
                lblCurrentHotkey.Text = "";
            }
            else if (WordHotKey.IsKeyAlreadyAssigned(Globals.ThisAddIn.Application, newHotkey, out prevCommand))
            {
                string message = string.Format("\tاین میانبر قبلاً به دستور {0} اختصاص یافته است", prevCommand);
                lblCurrentHotkey.Text = message;
            }
            else
            {
                lblCurrentHotkey.Text = "";
            }
        }

        private void UpdateShortcutsSettingsUI()
        {
            hotkeyControl.Hotkey = Hotkey.None;
            Template template = Globals.ThisAddIn.VirastyarTemplate;

            #region Check MS-Word shortcuts and updates settings

            //settings.Config_Shortcut_VirastyarCheckDates_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarCheckDates_Action);
            //settings.Config_Shortcut_VirastyarCheckNumbers_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarCheckNumbers_Action);
            //settings.Config_Shortcut_VirastyarAutoComplete_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarAutoComplete_Action);
            //settings.Config_Shortcut_VirastyarCheckPunctuation_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarCheckPunctuation_Action);
            //settings.Config_Shortcut_VirastyarCheckSpell_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarCheckSpell_Action);
            //settings.Config_Shortcut_VirastyarPinglishConvert_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarPinglishConvert_Action);
            //settings.Config_Shortcut_VirastyarPinglishConvertAll_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarPinglishConvertAll_Action);
            //settings.Config_Shortcut_VirastyarPreCheckSpell_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarPreCheckSpell_Action);
            //settings.Config_Shortcut_VirastyarRefineAllCharacters_Action = WordHotKey.RetreiveCurrentKey(template, Constants.MacroNames.VirastyarRefineAllCharacters_Action);

            #endregion

            lstShortcuts.Items.Clear();

            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                            {
                                "غلط‌یابی",
                                settings.Config_Shortcut_VirastyarCheckSpell_Action,
                                Constants.MacroNames.VirastyarCheckSpell_Action,
                            }
                ));

            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "پیش‌پردازش املایی",
                    settings.Config_Shortcut_VirastyarPreCheckSpell_Action,
                    Constants.MacroNames.VirastyarPreCheckSpell_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "اصلاح تمام نویسه‌های متن",
                    settings.Config_Shortcut_VirastyarRefineAllCharacters_Action,
                    Constants.MacroNames.VirastyarRefineAllCharacters_Action,
                }
                ));

            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "تبدیل تاریخ",
                    settings.Config_Shortcut_VirastyarCheckDates_Action,
                    Constants.MacroNames.VirastyarCheckDates_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "تبدیل اعداد",
                    settings.Config_Shortcut_VirastyarCheckNumbers_Action,
                    Constants.MacroNames.VirastyarCheckNumbers_Action,
                }
                ));

            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "تبدیل پینگلیش",
                    settings.Config_Shortcut_VirastyarPinglishConvert_Action,
                    Constants.MacroNames.VirastyarPinglishConvert_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                            new string[]
                {
                    "تبدیل یکباره پینگلیش",
                    settings.Config_Shortcut_VirastyarPinglishConvertAll_Action,
                    Constants.MacroNames.VirastyarPinglishConvertAll_Action,
                }
                            ));

            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "تصحیح نشانه‌گذاری",
                    settings.Config_Shortcut_VirastyarCheckPunctuation_Action,
                    Constants.MacroNames.VirastyarCheckPunctuation_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                new string[]
                {
                    "تکمیل خودکار کلمات",
                    settings.Config_Shortcut_VirastyarAutoComplete_Action,
                    Constants.MacroNames.VirastyarAutoComplete_Action,
                }
                ));
        }

        private Hotkey GetOldShortcutForSelectedItem()
        {
            if (lstShortcuts.SelectedItems.Count <= 0)
                return null;

            ListViewItem selectedItem = lstShortcuts.SelectedItems[0];
            Hotkey hotkey;
            Hotkey.TryParse(selectedItem.SubItems[1].Text, out hotkey);
            return hotkey;
        }

        private Hotkey GetNewShortcutForSelectedItem()
        {
            return hotkeyControl.Hotkey;
        }

        #endregion

        #region Repair Addin

        private void btnReloadAddinMenus_Click(object sender, EventArgs e)
        {
            OnRepairMenubarClicked();
        }

        private void btnReloadMenus_Click(object sender, EventArgs e)
        {
            OnRepairMenubarClicked();
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            DialogResult result = PersianMessageBox.Show(
                " آیا می‌خواهید مسیر لفت‌نامه‌ها نیز به حالت پیش‌فرض بازگردانده شود؟" + Environment.NewLine
                , " ",
                MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.No)
            {
                string mainDicPath = settings.SpellChecker_MainDictionaryPath;
                string userDicPath = settings.Config_UserDictionaryPath;
                string customDicsPaths = settings.SpellChecker_CustomDictionaries;
                string customDicsDescs = settings.SpellChecker_CustomDictionariesDescription;
                int customDicsFlags = settings.SpellChecker_CustomDictionariesSelectionFlag;
                bool mainDicSelectedFlag = settings.SpellChecker_MainDictionarySelected;

                settings.Reset();
                allCharactersRefinerSettings.Reload(settings);

                settings.SpellChecker_MainDictionaryPath = mainDicPath;
                settings.Config_UserDictionaryPath = userDicPath;
                settings.SpellChecker_CustomDictionaries = customDicsPaths;
                settings.SpellChecker_CustomDictionariesDescription = customDicsDescs;
                settings.SpellChecker_CustomDictionariesSelectionFlag = customDicsFlags;
                settings.SpellChecker_MainDictionarySelected = mainDicSelectedFlag;

                LoadConfigurations();
            }
            else if (result == DialogResult.Yes)
            {
                settings.Reset();
                allCharactersRefinerSettings.Reload(settings);

                settings.Config_UserDictionaryPath = Globals.ThisAddIn.FindUserDictionaryLocation();
                settings.SpellChecker_MainDictionaryPath = Globals.ThisAddIn.FindMainDictionaryLocation();

                LoadConfigurations();
            }

            if (result == DialogResult.Cancel)
            {
                // DO NOTHING
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region Refine Tab

        private void UpdateAllCharsRefinerSettingsUI()
        {
            FilteringCharacterCategory cats = allCharactersRefinerSettings.NotIgnoredCategories;

            cbRefineDigits.Checked = ((FilteringCharacterCategory.ArabicDigit & cats) == FilteringCharacterCategory.ArabicDigit);
            cbRefineErab.Checked = ((FilteringCharacterCategory.Erab & cats) == FilteringCharacterCategory.Erab);
            cbRefineHalfSpaceChar.Checked = ((FilteringCharacterCategory.HalfSpace & cats) == FilteringCharacterCategory.HalfSpace);
            cbRefineKaaf.Checked = ((FilteringCharacterCategory.Kaaf & cats) == FilteringCharacterCategory.Kaaf);
            cbRefineYaa.Checked = ((FilteringCharacterCategory.Yaa & cats) == FilteringCharacterCategory.Yaa);

            cbRemoveHalfSpaces.Checked = allCharactersRefinerSettings.RefineHalfSpacePositioning;
            tetLetterToIgnore.Text = "";
            listViewIgnoreList.Items.Clear();

            if (listViewIgnoreList.Items.Count <= 0) // if the form has been ewnewed with an empty list only
            {
                foreach (char c in allCharactersRefinerSettings.GetIgnoreListAsString())
                {
                    AddLetterToIgnoreList(c, true);
                }
            }
        }

        private void btnAddLetterToIgnoreList_Click(object sender, EventArgs e)
        {
            string input = tetLetterToIgnore.Text.Trim();
            if (input.Length > 1)
            {
                PersianMessageBox.Show("لطفاً تنها یک حرف درج کنید.");
            }
            else if (input.Length < 1)
            {
                PersianMessageBox.Show("لطفاً یک حرف درج کنید.");
            }
            else
            {
                if (GetAllCharsRefinerSettings().ContainsCharInIgnoreList(input[0]))
                {
                    PersianMessageBox.Show("این حرف قبلاً به لیست افزوده شده بود.");
                }
                else
                {
                    if (AddLetterToIgnoreList(input[0], false))
                        tetLetterToIgnore.Text = "";
                }
            }
        }

        private bool AddLetterToIgnoreList(char ch, bool onlyGUI)
        {
            listViewIgnoreList.Items.Add(new ListViewItem(new string[] 
                {
                    ch.ToString(), 
                    Convert.ToInt32(ch).ToString("X"),
                    Convert.ToInt32(ch).ToString()
                }
            ));

            if (!onlyGUI)
            {
                return GetAllCharsRefinerSettings().AddCharToIgnoreList(ch);
            }

            return true;
        }

        private void btnRemoveFromIgnoreList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewIgnoreList.SelectedItems)
            {
                if (GetAllCharsRefinerSettings().RemoveCharFromIgnoreList(item.Text[0]))
                    listViewIgnoreList.Items.Remove(item);
            }
        }

        private AllCharactersRefinerSettings GetAllCharsRefinerSettings()
        {
            FilteringCharacterCategory cats = (FilteringCharacterCategory)0;
            if (cbRefineDigits.Checked)
                cats = cats | FilteringCharacterCategory.ArabicDigit;
            if (cbRefineErab.Checked)
                cats = cats | FilteringCharacterCategory.Erab;
            if (cbRefineKaaf.Checked)
                cats = cats | FilteringCharacterCategory.Kaaf;
            if (cbRefineYaa.Checked)
                cats = cats | FilteringCharacterCategory.Yaa;
            if (cbRefineHalfSpaceChar.Checked)
                cats = cats | FilteringCharacterCategory.HalfSpace;

            allCharactersRefinerSettings.NotIgnoredCategories = cats;
            allCharactersRefinerSettings.RefineHalfSpacePositioning = cbRemoveHalfSpaces.Checked;
            return allCharactersRefinerSettings;
        }

        #region Refine Haa, Mee, HeYe, Be

        private void UpdateSpellCheckRefinementUI()
        {
            cbRefineHeYe.Checked = settings.PreprocessSpell_RefineHeYe;
            cbRefineHaa.Checked = settings.PreprocessSpell_RefineHaa;
            cbRefineMee.Checked = settings.PreprocessSpell_RefineMee;
            cbRefineBe.Checked = settings.PreprocessSpell_RefineBe;

            UpdateAllAffixesRefinementUI();
        }

        private void cbSpellCheckRefiners_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAllAffixesRefinementUI();
        }

        private void cbRefineAllAffixes_CheckStateChanged(object sender, EventArgs e)
        {
            var checkBoxes = new[]
             {
                 cbRefineHaa, cbRefineMee,
                 cbRefineHeYe, cbRefineBe,
             };

            switch (cbRefineAllAffixes.CheckState)
            {
                case CheckState.Unchecked:
                    foreach (var checkBox in checkBoxes)
                        checkBox.Checked = false;
                    break;
                case CheckState.Checked:
                    foreach (var checkBox in checkBoxes)
                        checkBox.Checked = true;
                    break;
                case CheckState.Indeterminate:
                    cbRefineAllAffixes.CheckState = CheckState.Unchecked;
                    break;
            }
        }

        private void UpdateAllAffixesRefinementUI()
        {
            var checkBoxes = new[] 
            { 
                cbRefineHaa, cbRefineMee, 
                cbRefineHeYe,cbRefineBe,
            };

            foreach (var checkBox in checkBoxes)
                checkBox.CheckedChanged -= cbSpellCheckRefiners_CheckedChanged;

            if (checkBoxes.All(chk => chk.Checked))
            {
                cbRefineAllAffixes.CheckState = CheckState.Checked;
            }
            else if (checkBoxes.All(chk => !chk.Checked))
            {
                cbRefineAllAffixes.CheckState = CheckState.Unchecked;
            }
            else
            {
                cbRefineAllAffixes.CheckStateChanged -= cbRefineAllAffixes_CheckStateChanged;
                cbRefineAllAffixes.CheckState = CheckState.Indeterminate;
                cbRefineAllAffixes.CheckStateChanged += cbRefineAllAffixes_CheckStateChanged;
            }

            foreach (var checkBox in checkBoxes)
                checkBox.CheckedChanged += cbSpellCheckRefiners_CheckedChanged;
        }
        
        #endregion

        #endregion

        #region General Settings



        #endregion

        #region Other Methods

        private void OnRepairMenubarClicked()
        {
            if (RepairMenubarsClicked != null)
                RepairMenubarsClicked(this, EventArgs.Empty);
        }

        private bool OnSpellCheckSettingsChanged(SpellCheckerConfig spellerConfig)
        {
            if (SpellCheckSettingsChanged != null)
            {
                var e = new SpellCheckSettingsChangedEventArgs();
                e.Settings = spellerConfig;
                e.CustomDictionaries = GetSelectedCustomDictionaries();

                e.RuleDontCheckSingleLetters = cbDontCheckSingleLetters.Checked;
                e.RuleVocabWordSpacingCorrection = cbVocabSpaceCorrection.Checked;
                e.RuleHeYeConvertion = cbHaaShisakiToHaaYaa.Checked;

                e.RefineHaa = cbRefineHaa.Checked;
                e.RefineHeYe = cbRefineHeYe.Checked;
                e.RefineMee = cbRefineMee.Checked;
                e.RefineBe = cbRefineBe.Checked;
                e.RefineAllAffixes = cbRefineAllAffixes.Checked;

                //e.CustomDictionaries = SpellCheckerWrapper.GetDictionariesArray(settings.SpellChecker_MainDictionaryPath, settings.SpellChecker_MainDictionarySelected,
                //settings.SpellChecker_CustomDictionaries, settings.SpellChecker_CustomDictionariesSelectionFlag);

                SpellCheckSettingsChanged(e);
                if (e.CancelLoadingUserDictionary)
                {
                    errorProvider.SetError(txtFileName, "فایل واژه‌نامه بدرستی انتخاب نشده است.");
                    //txtFileName.Text = this.settings.Config_DictionaryPath;
                    return false;
                }

                if (e.ErroneousUserDictionaries.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (string file in e.ErroneousUserDictionaries)
                    {
                        sb.AppendLine(file);
                    }

                    errorProvider.SetError(linkLabelSpellCheckerCreateDictionary, "فایل‌های زیر به درستی بارگذاری نشده‌اند:" + Environment.NewLine + sb.ToString());
                    return false;
                }
            }

            return true;
        }

        private string[] GetSelectedCustomDictionaries()
        {
            var lstPaths = new List<string>();
            for (int i = 0; i < listViewUserDictionaries.Items.Count; ++i)
            {
                if (listViewUserDictionaries.Items[i].Checked)
                    lstPaths.Add(GetFileNameFromItem(listViewUserDictionaries.Items[i]));
            }

            return lstPaths.ToArray();
        }

        #endregion

        #region Word-Completion Settings

        private void rbWCShowMaxWords_CheckedChanged(object sender, EventArgs e)
        {
            numUpDownWCWordCount.Enabled = rbWCShowMaxWords.Checked;
        }

        private void cbWCCompleteWithoutHotkey_CheckedChanged(object sender, EventArgs e)
        {
            numUpDownWCMinWordLength.Enabled = cbWCCompleteWithoutHotkey.Checked;
        }

        private void UpdateWordCompletionSettingsUI()
        {
            if (settings.WordCompletionSugCount <= 0)
            {
                this.rbWCShowAllWords.Checked = true;
            }
            else
            {
                this.rbWCShowMaxWords.Checked = true;
                TrySetNumericUpDownValue(this.numUpDownWCWordCount, settings.WordCompletionSugCount);
            }

            cbWCCompleteWithoutHotkey.Checked = settings.WordCompletionCompleteWithoutHotKey;
            cbWCInsertSpace.Checked = settings.WordCompletionInsertSpace;

            TrySetNumericUpDownValue(numUpDownWCMinWordLength, settings.WordCompletionMinWordLength);
            TrySetNumericUpDownValue(numUpDownWCFontSize, settings.WordCompletionFontSize);

            UpdateWordCompletionSettings();
        }

        private void UpdateWordCompletionSettings()
        {
            if (rbWCShowAllWords.Checked)
                WordCompletionForm.AutoCompletetionListMaxCount = Int32.MaxValue;
            else
                WordCompletionForm.AutoCompletetionListMaxCount = (int)numUpDownWCWordCount.Value;

            WordCompletionForm.AddSpaceAfterCompletion = cbWCInsertSpace.Checked;
            WordCompletionForm.CompleteWithoutHotKey = cbWCCompleteWithoutHotkey.Checked;
            WordCompletionForm.CompleteWithoutHotKeyMinLength = (int)numUpDownWCMinWordLength.Value;
            WordCompletionForm.FontSize = (int)numUpDownWCFontSize.Value;

            //UpdateWCHotkey(GetOldWCHotkey());
        }

        private void TrySetNumericUpDownValue(NumericUpDown numUpDown, int value)
        {
            if (value < numUpDown.Minimum)
                value = (int)numUpDown.Minimum;

            if (value > numUpDown.Maximum)
                value = (int)numUpDown.Maximum;

            numUpDown.Value = value;
        }

        #endregion

        #region Help Button
        private void AddinConfigurationDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            string topicFileName = "";
            if (tabCtrlSettings.SelectedTab == tbPageSpellCheck)
            {
                topicFileName = HelpConstants.SettingsSpellChecker;
            }
            else if (tabCtrlSettings.SelectedTab == tbPageShortcut)
            {
                topicFileName = HelpConstants.SettingsShortcuts;
            }
            else if (tabCtrlSettings.SelectedTab == tbPageRefineAll)
            {
                topicFileName = HelpConstants.SettingsRefineAll;
            }
            else if (tabCtrlSettings.SelectedTab == tbPageRepairAddin)
            {
                topicFileName = HelpConstants.SettingsTroubleShooting;
            }
            else if (tabCtrlSettings.SelectedTab == tbPageWordCompletion)
            {
                topicFileName = HelpConstants.SettingsWordCompletion;
            }
            else if (tabCtrlSettings.SelectedTab == tbPageWords)
            {
                //topicFileName = HelpConstants.SettingsWords;
            }
            else if (tabCtrlSettings.SelectedTab == tbPagePreprocessSpell)
            {
                topicFileName = HelpConstants.SettingsSpellPreprocessing;
            }


            Globals.ThisAddIn.ShowHelp(topicFileName);
        }
        #endregion

    }
}

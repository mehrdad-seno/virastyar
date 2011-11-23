using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SCICT.NLP.Utility.LanguageModel;
using VirastyarWordAddin.Properties;
using SCICT.NLP.Persian;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.Utility.GUI;
using SCICT.Utility.Keyboard;
using System.Threading;
using SCICT.Microsoft.Word;
using System.Linq;
using VirastyarWordAddin.Update;
using VirastyarWordAddin.Log;

namespace VirastyarWordAddin.Configurations
{
    /// <summary>
    /// GUI to modify various parameters of Addin
    /// </summary>
    public partial class AddinConfigurationDialog : Form, IAddinConfigurationDialog
    {
        const string MainDicDescription = "واژه‌نامهٔ ویراستیار";
        const string CustomDicDescription = "(واژه‌نامهٔ کاربر)";

        #region Private Members

        private readonly Settings m_appSettings = null;
        private AllCharactersRefinerSettings m_allCharactersRefinerSettings = null;

        private bool m_reloadSpellCheckerEngine = false;

        #endregion

        #region Events

        public event EventHandler RepairMenubarsClicked;
        public event SpellCheckSettingsChangedEventHandler SpellCheckSettingsChanged;
        public event RefineAllSettingsChangedEventHandler RefineAllSettingsChanged;

        #endregion

        #region Constructors

        internal AddinConfigurationDialog()
        {
            InitializeComponent();
        }

        internal AddinConfigurationDialog(Settings settings)
            : this()
        {
            m_appSettings = settings;
            m_allCharactersRefinerSettings = new AllCharactersRefinerSettings(settings);
            LoadConfigurations();
        }

        #endregion

        #region Save, Load

        /// <summary>
        /// Update UI elements based on the current settings
        /// </summary>
        public void LoadConfigurations()
        {
            // Load Configuration and update UI elements
            UpdateSpellCheckSettingsUI();
            UpdateShortcutsSettingsUI();
            UpdateAllCharsRefinerSettingsUI();
            UpdatePrespellCheckUI();
            UpdateWordCompletionSettingsUI();
            UpdateAutomaticReportSettingsUI();
        }

        private void SaveConfigurations()
        {
            #region Spell-Check Settings

            SpellCheckerConfig spellCheckerInternalConfig = GetSpellCheckSettings();

            if (OnSpellCheckSettingsChanged(spellCheckerInternalConfig))
            {
                m_appSettings.SpellChecker_UserDictionaryPath = spellCheckerInternalConfig.DicPath;
                m_appSettings.SpellChecker_EditDistance = spellCheckerInternalConfig.EditDistance;
                m_appSettings.SpellChecker_MaxSuggestions = spellCheckerInternalConfig.SuggestionCount;

                m_appSettings.PreprocessSpell_CorrectBe = cbPrespellCorrectBe.Checked;
                m_appSettings.PreprocessSpell_CorrectPrefix = cbPrespellCorrectPrefixes.Checked;
                m_appSettings.PreprocessSpell_CorrectPostfix = cbPrespellCorrectSuffixes.Checked;

                Debug.Assert(listViewUserDictionaries.Items.Count > 0);

                if (listViewUserDictionaries.Items.Count > 0)
                {
                    m_appSettings.SpellChecker_MainDictionaryPath = GetFileNameFromItem(listViewUserDictionaries.Items[0]);
                    m_appSettings.SpellChecker_MainDictionarySelected = listViewUserDictionaries.Items[0].Checked;

                    var sbPaths = new StringBuilder();
                    var sbDescs = new StringBuilder();
                    int selectionFlags = 0;
                    for (int i = 1; i < listViewUserDictionaries.Items.Count; ++i)
                    {
                        sbPaths.AppendFormat("{0};", GetFileNameFromItem(listViewUserDictionaries.Items[i]));
                        sbDescs.AppendFormat("{0};", GetDescriptionFromItem(listViewUserDictionaries.Items[i]));

                        if (listViewUserDictionaries.Items[i].Checked)
                            selectionFlags = selectionFlags | (1 << (i - 1));
                    }

                    m_appSettings.SpellChecker_CustomDictionaries = sbPaths.ToString();
                    m_appSettings.SpellChecker_CustomDictionariesDescription = sbDescs.ToString();
                    m_appSettings.SpellChecker_CustomDictionariesSelectionFlag = selectionFlags;
                }
            }

            #endregion

            #region Storing All-Chars-Refiner settings

            m_allCharactersRefinerSettings = GetAllCharsRefinerSettings();
            m_appSettings.RefineCategoriesFlag = (int)m_allCharactersRefinerSettings.NotIgnoredCategories;
            m_appSettings.RefineIgnoreListConcated = m_allCharactersRefinerSettings.GetIgnoreListAsString();
            m_appSettings.RefineHalfSpacePositioning = m_allCharactersRefinerSettings.RefineHalfSpacePositioning;
            m_appSettings.RefineNormalizeHeYe = m_allCharactersRefinerSettings.NormalizeHeYe;
            m_appSettings.RefineLongHeYeToShort = m_allCharactersRefinerSettings.ConvertLongHeYeToShort;
            m_appSettings.RefineShortHeYeToLong = m_allCharactersRefinerSettings.ConvertShortHeYeToLong;

            #endregion

            #region Storing Word-Completion Settings

            UpdateWordCompletionSettings();
            m_appSettings.WordCompletionCompleteWithoutHotKey = cbWCCompleteWithoutHotkey.Checked;
            m_appSettings.WordCompletionInsertSpace = cbWCInsertSpace.Checked;
            if (rbWCShowAllWords.Checked)
                m_appSettings.WordCompletionSugCount = -1;
            else
                m_appSettings.WordCompletionSugCount = (int)numUpDownWCWordCount.Value;

            m_appSettings.WordCompletionMinWordLength = (int)numUpDownWCMinWordLength.Value;
            m_appSettings.WordCompletionFontSize = (int)numUpDownWCFontSize.Value;

            #endregion

            #region Automatic Update and Report

            m_appSettings.LogReport_AutomaticReport = rdoSendReportAccept.Checked;

            #endregion

            m_appSettings.Save();

            OnRefineAllCharactersSettingsChanged(m_allCharactersRefinerSettings);
        }

        private SpellCheckerConfig GetSpellCheckSettings()
        {
            var spellCheckSettings = new SpellCheckerConfig(txtFileName.Text,
                m_appSettings.SpellChecker_EditDistance, m_appSettings.SpellChecker_MaxSuggestions)
             {
                 StemPath = SettingsHelper.GetFullPath(Constants.StemFileName,
                                                VirastyarFilePathTypes.AllUsersFiles)
             };

            return spellCheckSettings;
        }

        private void OnRefineAllCharactersSettingsChanged(AllCharactersRefinerSettings refineAllCharSettings)
        {
            if (RefineAllSettingsChanged != null)
            {
                var e = new RefineAllSettingsChangedEventArgs {Settings = refineAllCharSettings};
                RefineAllSettingsChanged(e);
            }
        }

        #endregion

        #region Ok, Apply, Cancel

        private void BtnOkClick(object sender, EventArgs e)
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
            error |= !string.IsNullOrEmpty(errorProvider.GetError(linkLabelSpellCheckerCreateDictionary));

            if (!error)
                Close();

            this.Cursor = Cursors.Default;
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Spell Check Settings

        private void UpdateSpellCheckSettingsUI()
        {
            txtFileName.Text = m_appSettings.SpellChecker_UserDictionaryPath;
            if (m_appSettings.SpellChecker_MaxSuggestions <= 0)
                m_appSettings.SpellChecker_MaxSuggestions = 1;

            listViewUserDictionaries.Items.Clear();

            var item = new ListViewItem(new [] 
            { 
                MainDicDescription, 
                m_appSettings.SpellChecker_MainDictionaryPath.Trim() 
            });
            listViewUserDictionaries.Items.Add(item);
            item.Checked = m_appSettings.SpellChecker_MainDictionarySelected;
            AddCustomDictionaryFiles(m_appSettings.SpellChecker_CustomDictionaries, m_appSettings.SpellChecker_CustomDictionariesDescription, m_appSettings.SpellChecker_CustomDictionariesSelectionFlag);
        }

        private void AddCustomDictionaryFiles(string dictionaryPaths, string dictionaryDescs, long selectionFlags)
        {
            var paths = dictionaryPaths.Split(';');
            var descs = new string[paths.Length];
            var readDescs = dictionaryDescs.Split(';');

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
                var item = new ListViewItem(new [] { descs[i], path });
                listViewUserDictionaries.Items.Add(item);
                item.Checked = ((1 << i) & selectionFlags) > 0;
            }
        }

        private void LinkLabelSpellCheckerAddExistingDicLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dlg = new OpenFileDialog();
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

                var lvitem = listViewUserDictionaries.Items.Add(new ListViewItem(new [] { CustomDicDescription, dlg.FileName }));
                lvitem.Checked = true;
                m_reloadSpellCheckerEngine = true;
            }
        }

        private void ToolStripMenuItemDeleteDicClick(object sender, EventArgs e)
        {
            DeleteSelectedDictionaries();
        }

        private void ToolStripMenuItemEditDicClick(object sender, EventArgs e)
        {
            EditSelectedDictionary();
        }

        private void LnkEditUserDicLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var dicEditor = new DictionaryEditor(txtFileName.Text);
                dicEditor.ShowDialog();
            }
            catch (Exception ex)
            {
                PersianMessageBox.Show("خطا در ویرایش واژه‌نامه");
                LogHelper.ErrorException("خطا در ویرایش واژه‌نامه", ex);
            }
        }

        private void EditSelectedDictionary()
        {
            foreach (int index in listViewUserDictionaries.SelectedIndices)
            {
                if (index == 0)
                {
                    PersianMessageBox.Show("فایل اصلی واژه‌نامه را نمی‌توانید ویرایش کنید");
                }
                else
                {
                    string dicPath = GetFileNameFromItem(listViewUserDictionaries.Items[index]);
                    try
                    {
                        var dicEditor = new DictionaryEditor(dicPath);
                        dicEditor.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        PersianMessageBox.Show("خطا در ویرایش واژه‌نامه");
                        LogHelper.ErrorException("خطا در ویرایش واژه‌نامه", ex);
                    }
                    finally
                    {
                        m_reloadSpellCheckerEngine = true;
                    }
                }
            }   
        }

        private void LnkEditDictionaryLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditSelectedDictionary();
        }

        private void LinkLabelDeleteItemLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DeleteSelectedDictionaries();
        }

        private void DeleteSelectedDictionaries()
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
                    m_reloadSpellCheckerEngine = true;
                }
            }
        }

        private static string GetFileNameFromItem(ListViewItem item)
        {
            return item.SubItems[1].Text;
        }

        private static string GetDescriptionFromItem(ListViewItem item)
        {
            return item.SubItems[0].Text.Replace(';', ',').Trim();
        }

        private void LinkLabelSpellCheckerCreateDictionaryLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var doc = Globals.ThisAddIn.Application.ActiveDocument;
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
                var dlg = new SaveFileDialog {Filter = "Dictionary|*.dat"};

                if (DialogResult.OK != dlg.ShowDialog())
                {
                    return;
                }

                fileName = dlg.FileName;
                try
                {
                    var sw = File.CreateText(fileName);
                    sw.Close();
                }
                catch
                {
                    PersianMessageBox.Show("فایل انتخاب شده قابل نوشتن نیست.");
                    return;
                }
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                try
                {
                    this.Enabled = false;
                    this.progressBarBuildDictionary.Visible = true;

                    // it is forced and should not be ignored
                    System.Windows.Forms.Application.DoEvents();

                    var dictionaryExtractor = new POSTaggedDictionaryExtractor();
                    string content = doc.Content.Text;
                    string mainDictionary = Globals.ThisAddIn.SpellCheckerWrapper.MainDictionary;

                    var thread = new Thread(new ThreadStart(delegate
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
                    var lstViewItem = listViewUserDictionaries.Items.Add(new ListViewItem(new [] { CustomDicDescription, fileName }));
                    lstViewItem.Selected = true;
                }
            }
            catch (IOException)
            {
                PersianMessageBox.Show("فایل انتخاب شده قابل نوشتن نیست.");
                return;
            }
            catch(Exception ex)
            {
                LogHelper.DebugException("", ex);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                m_reloadSpellCheckerEngine = true;
            }
        }

        private void BtnBrowseClick(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.Cancel)
            {
                txtFileName.Text = openFileDialog.FileName;
            }
            m_reloadSpellCheckerEngine = true;
        }

        private void ListViewUserDictionariesBeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Item == 0)
                e.CancelEdit = true;
        }

        #endregion

        #region Shortcut Tab

        private void LstShortcutsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstShortcuts.SelectedItems.Count <= 0)
                return;

            ListViewItem selectedItem = lstShortcuts.SelectedItems[0];
            Hotkey hotkey = Hotkey.None;

            // TODO: Check for null values

            string strKey = m_appSettings.PropertyValues["Config_Shortcut_" + selectedItem.SubItems[2].Text].PropertyValue as string;
            Hotkey.TryParse(strKey, out hotkey);
            hotkeyControl.Hotkey = hotkey;

            lblShortcutDesc.Text = string.Format("\n{0}:{1}\n\n{2}:{3}\n",
                            "نام", selectedItem.SubItems[0].Text,
                            "میانبر فعلی", hotkey);
        }

        private void BtnAssignShortcutClick(object sender, EventArgs e)
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

        private void BtnClearHotkeyClick(object sender, EventArgs e)
        {
            Hotkey hotkey = hotkeyControl.Hotkey;
            if (hotkey == Hotkey.None)
                return;

            ListViewItem selectedItem = lstShortcuts.SelectedItems[0];
            string configKey = selectedItem.SubItems[2].Text;

            WordHotKey.RemoveAssignedKey(Globals.ThisAddIn.VirastyarTemplate, hotkey, configKey);
            m_appSettings["Config_Shortcut_" + configKey] = "";
            selectedItem.SubItems[1].Text = "";
            hotkeyControl.Text = "";
        }


        private void HotkeyControlHotkeyChanged(object sender, EventArgs e)
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

            #region Check MS-Word shortcuts and updates settings
            //var template = Globals.ThisAddIn.VirastyarTemplate;

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
                new []
                            {
                                "غلط‌یابی",
                                m_appSettings.Config_Shortcut_VirastyarCheckSpell_Action,
                                Constants.MacroNames.VirastyarCheckSpell_Action,
                            }
                ));

            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "پیش‌پردازش املایی",
                    m_appSettings.Config_Shortcut_VirastyarPreCheckSpell_Action,
                    Constants.MacroNames.VirastyarPreCheckSpell_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "اصلاح تمام نویسه‌های متن",
                    m_appSettings.Config_Shortcut_VirastyarRefineAllCharacters_Action,
                    Constants.MacroNames.VirastyarRefineAllCharacters_Action,
                }
                ));

            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "تبدیل تاریخ",
                    m_appSettings.Config_Shortcut_VirastyarCheckDates_Action,
                    Constants.MacroNames.VirastyarCheckDates_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "تبدیل اعداد",
                    m_appSettings.Config_Shortcut_VirastyarCheckNumbers_Action,
                    Constants.MacroNames.VirastyarCheckNumbers_Action,
                }
                ));

            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "تبدیل پینگلیش",
                    m_appSettings.Config_Shortcut_VirastyarPinglishConvert_Action,
                    Constants.MacroNames.VirastyarPinglishConvert_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                            new []
                {
                    "تبدیل یکباره پینگلیش",
                    m_appSettings.Config_Shortcut_VirastyarPinglishConvertAll_Action,
                    Constants.MacroNames.VirastyarPinglishConvertAll_Action,
                }
                            ));

            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "تصحیح نشانه‌گذاری",
                    m_appSettings.Config_Shortcut_VirastyarCheckPunctuation_Action,
                    Constants.MacroNames.VirastyarCheckPunctuation_Action,
                }
                ));


            lstShortcuts.Items.Add(new ListViewItem(
                new []
                {
                    "تکمیل خودکار کلمات",
                    m_appSettings.Config_Shortcut_VirastyarAutoComplete_Action,
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

        private void BtnRestoreDataFilesClick(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.CheckDataDependencies(true))
            {
                PersianMessageBox.Show("فایل‌های دادگان با موفقیت به حالت پیش‌فرض برگردانده شدند", "برگرداندن فایل‌های دادگان",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnResetSettingsClick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            DialogResult result = PersianMessageBox.Show(
                " آیا می‌خواهید مسیر لفت‌نامه‌ها نیز به حالت پیش‌فرض بازگردانده شود؟" + Environment.NewLine
                , " ",
                MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.No)
            {
                string mainDicPath = m_appSettings.SpellChecker_MainDictionaryPath;
                string userDicPath = m_appSettings.SpellChecker_UserDictionaryPath;
                string customDicsPaths = m_appSettings.SpellChecker_CustomDictionaries;
                string customDicsDescs = m_appSettings.SpellChecker_CustomDictionariesDescription;
                int customDicsFlags = m_appSettings.SpellChecker_CustomDictionariesSelectionFlag;
                bool mainDicSelectedFlag = m_appSettings.SpellChecker_MainDictionarySelected;

                m_appSettings.Reset();
                m_allCharactersRefinerSettings.Reload(m_appSettings);

                m_appSettings.SpellChecker_MainDictionaryPath = mainDicPath;
                m_appSettings.SpellChecker_UserDictionaryPath = userDicPath;
                m_appSettings.SpellChecker_CustomDictionaries = customDicsPaths;
                m_appSettings.SpellChecker_CustomDictionariesDescription = customDicsDescs;
                m_appSettings.SpellChecker_CustomDictionariesSelectionFlag = customDicsFlags;
                m_appSettings.SpellChecker_MainDictionarySelected = mainDicSelectedFlag;

                LoadConfigurations();
            }
            else if (result == DialogResult.Yes)
            {
                m_appSettings.Reset();
                m_allCharactersRefinerSettings.Reload(m_appSettings);

                m_appSettings.SpellChecker_UserDictionaryPath = Globals.ThisAddIn.FindUserDictionaryLocation();
                m_appSettings.SpellChecker_MainDictionaryPath = Globals.ThisAddIn.FindMainDictionaryLocation();

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
            FilteringCharacterCategory cats = m_allCharactersRefinerSettings.NotIgnoredCategories;

            cbRefineDigits.Checked = ((FilteringCharacterCategory.ArabicDigit & cats) == FilteringCharacterCategory.ArabicDigit);
            cbRefineErab.Checked = ((FilteringCharacterCategory.Erab & cats) == FilteringCharacterCategory.Erab);
            cbRefineHalfSpaceChar.Checked = ((FilteringCharacterCategory.HalfSpace & cats) == FilteringCharacterCategory.HalfSpace);
            cbRefineKaaf.Checked = ((FilteringCharacterCategory.Kaaf & cats) == FilteringCharacterCategory.Kaaf);
            cbRefineYaa.Checked = ((FilteringCharacterCategory.Yaa & cats) == FilteringCharacterCategory.Yaa);

            cbRemoveHalfSpaces.Checked = m_allCharactersRefinerSettings.RefineHalfSpacePositioning;

            cbRefineAndNormalizeHeYe.Checked = m_allCharactersRefinerSettings.NormalizeHeYe;
            cbConvertLongHeYeToShort.Checked = m_allCharactersRefinerSettings.ConvertLongHeYeToShort;
            cbConvertShortHeYeToLong.Checked = m_allCharactersRefinerSettings.ConvertShortHeYeToLong;

            tetLetterToIgnore.Text = "";
            listViewIgnoreList.Items.Clear();

            if (listViewIgnoreList.Items.Count <= 0) // if the form has been ewnewed with an empty list only
            {
                foreach (char c in m_allCharactersRefinerSettings.GetIgnoreListAsString())
                {
                    AddLetterToIgnoreList(c, true);
                }
            }
        }

        private void BtnAddLetterToIgnoreListClick(object sender, EventArgs e)
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

        private bool AddLetterToIgnoreList(char ch, bool onlyGui)
        {
            listViewIgnoreList.Items.Add(new ListViewItem(new [] 
                {
                    ch.ToString(), 
                    Convert.ToInt32(ch).ToString("X"),
                    Convert.ToInt32(ch).ToString()
                }
            ));

            if (!onlyGui)
            {
                return GetAllCharsRefinerSettings().AddCharToIgnoreList(ch);
            }

            return true;
        }

        private void BtnRemoveFromIgnoreListClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewIgnoreList.SelectedItems)
            {
                if (GetAllCharsRefinerSettings().RemoveCharFromIgnoreList(item.Text[0]))
                    listViewIgnoreList.Items.Remove(item);
            }
        }

        private AllCharactersRefinerSettings GetAllCharsRefinerSettings()
        {
            var cats = (FilteringCharacterCategory) 0;
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

            m_allCharactersRefinerSettings.NotIgnoredCategories = cats;
            m_allCharactersRefinerSettings.RefineHalfSpacePositioning = cbRemoveHalfSpaces.Checked;

            m_allCharactersRefinerSettings.NormalizeHeYe = cbRefineAndNormalizeHeYe.Checked;
            m_allCharactersRefinerSettings.ConvertLongHeYeToShort = cbConvertLongHeYeToShort.Checked;
            m_allCharactersRefinerSettings.ConvertShortHeYeToLong = cbConvertShortHeYeToLong.Checked;

            return m_allCharactersRefinerSettings;
        }

        #region Refine Haa, Mee, HeYe, Be

        private void UpdatePrespellCheckUI()
        {
            cbPrespellCorrectBe.Checked = m_appSettings.PreprocessSpell_CorrectBe;
            cbPrespellCorrectPrefixes.Checked = m_appSettings.PreprocessSpell_CorrectPrefix;
            cbPrespellCorrectSuffixes.Checked = m_appSettings.PreprocessSpell_CorrectPostfix;

            UpdateAllAffixesRefinementUI();
        }

        private void CbSpellCheckRefinersClicked(object sender, EventArgs e)
        {
            UpdateAllAffixesRefinementUI();
        }

        private void CbRefineAllAffixesCheckStateChanged(object sender, EventArgs e)
        {
            var checkBoxes = new[]
             {
                 cbPrespellCorrectBe, cbPrespellCorrectPrefixes, cbPrespellCorrectSuffixes
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
                cbPrespellCorrectBe, cbPrespellCorrectPrefixes, cbPrespellCorrectSuffixes
            };

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
                cbRefineAllAffixes.CheckStateChanged -= CbRefineAllAffixesCheckStateChanged;
                cbRefineAllAffixes.CheckState = CheckState.Indeterminate;
                cbRefineAllAffixes.CheckStateChanged += CbRefineAllAffixesCheckStateChanged;
            }
        }
        
        #endregion

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

                e.PrespellCorrectBe = cbPrespellCorrectBe.Checked;
                e.PrespellCorrectPrefixes = cbPrespellCorrectPrefixes.Checked;
                e.PrespellCorrectSuffixes = cbPrespellCorrectSuffixes.Checked;
                e.ReloadSpellCheckerEngine = m_reloadSpellCheckerEngine;

                SpellCheckSettingsChanged(e);
                if (e.CancelLoadingUserDictionary)
                {
                    errorProvider.SetError(txtFileName, "فایل واژه‌نامه بدرستی انتخاب نشده است.");
                    return false;
                }

                if (e.ErroneousUserDictionaries.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (string file in e.ErroneousUserDictionaries)
                    {
                        sb.AppendLine(file);
                    }

                    errorProvider.SetError(linkLabelSpellCheckerCreateDictionary, "فایل‌های زیر به درستی بارگذاری نشده‌اند:" + Environment.NewLine + sb);
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

        private void RbWcShowMaxWordsCheckedChanged(object sender, EventArgs e)
        {
            numUpDownWCWordCount.Enabled = rbWCShowMaxWords.Checked;
        }

        private void CbWcCompleteWithoutHotkeyCheckedChanged(object sender, EventArgs e)
        {
            numUpDownWCMinWordLength.Enabled = cbWCCompleteWithoutHotkey.Checked;
        }

        private void UpdateWordCompletionSettingsUI()
        {
            if (m_appSettings.WordCompletionSugCount <= 0)
            {
                this.rbWCShowAllWords.Checked = true;
            }
            else
            {
                this.rbWCShowMaxWords.Checked = true;
                TrySetNumericUpDownValue(this.numUpDownWCWordCount, m_appSettings.WordCompletionSugCount);
            }

            cbWCCompleteWithoutHotkey.Checked = m_appSettings.WordCompletionCompleteWithoutHotKey;
            cbWCInsertSpace.Checked = m_appSettings.WordCompletionInsertSpace;

            TrySetNumericUpDownValue(numUpDownWCMinWordLength, m_appSettings.WordCompletionMinWordLength);
            TrySetNumericUpDownValue(numUpDownWCFontSize, m_appSettings.WordCompletionFontSize);

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
        }

        private static void TrySetNumericUpDownValue(NumericUpDown numUpDown, int value)
        {
            if (value < numUpDown.Minimum)
                value = (int)numUpDown.Minimum;

            if (value > numUpDown.Maximum)
                value = (int)numUpDown.Maximum;

            numUpDown.Value = value;
        }

        #endregion

        #region Help Button

        private void AddinConfigurationDialogHelpButtonClicked(object sender, CancelEventArgs e)
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
            else if (tabCtrlSettings.SelectedTab == tbPageAddinSettings)
            {
                topicFileName = HelpConstants.SettingsTroubleShooting;
            }
            else if (tabCtrlSettings.SelectedTab == tbPageWordCompletion)
            {
                topicFileName = HelpConstants.SettingsWordCompletion;
            }
            else if (tabCtrlSettings.SelectedTab == tbPagePreprocessSpell)
            {
                topicFileName = HelpConstants.SettingsSpellPreprocessing;
            }

            Globals.ThisAddIn.ShowHelp(topicFileName);
        }
        #endregion

        #region AutoUpdate and Report

        private void UpdateAutomaticReportSettingsUI()
        {
            if (m_appSettings.LogReport_AutomaticReport)
            {
                rdoSendReportAccept.Checked = true;
            }
            else
            {
                rdoSendReportDecline.Checked = true;
            }
        }

        private void BtnCheckForUpdateClick(object sender, EventArgs e)
        {
            var updateNotificationWindow = new UpdateNotificationWindow(CloseThisDialog, true);
            updateNotificationWindow.ShowDialog(this);
        }
        
        private void CloseThisDialog(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabelViewGatheredInfoLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string logPath = LogReporter.GetLogPath();
            var pInfo = new ProcessStartInfo {FileName = logPath, Verb = "Open", UseShellExecute = true};

            try
            {
                Process.Start(pInfo);
            }
            catch 
            {
                // Ignore
            }
        }

        private void ConvertHeYeToCheckBoxesClick(object sender, EventArgs e)
        {
            var cbMain = sender as System.Windows.Forms.CheckBox;
            if(cbMain == null) return;

            //cbMain.Checked = !cbMain.Checked;

            if(!cbMain.Checked) // if it's been disabled then thre's no need to disable any other thing
                return;
            
            var cbsToChange = new[] {cbConvertLongHeYeToShort, cbConvertShortHeYeToLong};

            for (int i = 0; i < cbsToChange.Length; i++)
            {
                if (cbsToChange[i] != cbMain && cbsToChange[i].Checked)
                    cbsToChange[i].Checked = false;
            }

        }

        private void listViewUserDictionaries_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            m_reloadSpellCheckerEngine = true;
        }

        private void AddinConfigurationDialog_Shown(object sender, EventArgs e)
        {
            // this call is cruicial. After this call the state of this flag becomes Off,
            // after any call to the dictionary modification events turn this flag On.
            m_reloadSpellCheckerEngine = false;
        }

        #endregion
    }
}

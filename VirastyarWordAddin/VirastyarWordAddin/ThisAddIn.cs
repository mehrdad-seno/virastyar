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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using log4net;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using VirastyarWordAddin.Configurations;
using VirastyarWordAddin.Properties;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.Microsoft.Win32;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.ResourceManagement;
using SCICT.Utility.GUI;
using SCICT.Utility.SpellChecker;
using Office = Microsoft.Office.Core;
using SCICT.Utility.Keyboard;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SCICT.Microsoft.Word;
using VirastyarWordAddin.Updater;

namespace VirastyarWordAddin
{
    /// <summary>
    /// Each file in Virastyar goes to its special path, based on its functionality.
    /// </summary>
    public enum VirastyarFilePathTypes
    {
        /// <summary>
        /// Non-user specific files; such as dictionary or help file.
        /// Windows provides location for non-user specific files, which is use here.
        /// </summary>
        AllUsersFiles,

        /// <summary>
        /// User specific files; such as customized dictionaries, etc.
        /// </summary>
        UserFiles,

        /// <summary>
        /// Files that are placed in the installation directory; such as dll files.
        /// </summary>
        InstallationFiles,
    }

    /// <summary>
    /// The base class for Addin
    /// </summary>
    public partial class ThisAddIn : IVirastyarAddin
    {
        #region Private Members

        private Microsoft.Office.Interop.Word.Application m_wordApp;
        private Template m_virastyarTemplate;

        private SpellCheckerVerifier m_spellCheckerVerifier;
        private PreprocessSpellVerifier m_preprocessSpellVerifier;

        private SpellCheckerWrapper m_spellCheckerWrapper;

        private AllCharactersRefinerSettings m_allCharactersRefinerSettings;

        private PinglishVerifier m_pinglishVerifier;
        private NumberVerifier m_numberVerifier;
        private PunctuationVerifier m_puncVerfier;
        private DateVerifier m_dateVerifier;

#if DEBUG
        private ILog m_log;
#endif

        private UsageLogger m_usageLogger;

        private Settings m_appSettings;

        private AddinConfigurationDialog m_addinConfigurationDialog;

        private Mutex m_appSpecificMutex = null;

        #endregion

        #region Public Members

        /// <summary>
        /// Indicates the whether the addin is loaded or not
        /// </summary>
        public bool Loaded
        {
            get
            {
                return (m_virastyarTemplate != null);
            }
        }
       
        public SpellCheckerWrapper SpellCheckerWrapper
        {
            get
            {
                return this.m_spellCheckerWrapper;
            }
        }

        public UsageLogger UsageLogger
        {
            get
            {
                return this.m_usageLogger;
            }
        }

        public MSWordDocument ActiveMSWordDocument
        {
            get
            {
                try
                {
                    Document doc = Globals.ThisAddIn.Application.ActiveDocument;
                    return new SCICT.Microsoft.Office.Word.ContentReader.MSWordDocument(doc);
                }
                catch
                {
                    // Ignore
                    return null;
                }
            }
        }

        public Template VirastyarTemplate
        {
            get
            {
                return m_virastyarTemplate;
            }
        }

        #endregion

        #region ThisAddIn

        private readonly VirastyarAddin m_virastyarAddin = new VirastyarAddin();

        protected override object RequestComAddInAutomationService()
        {
            return m_virastyarAddin;
        }

        private bool IsFirstAddinInstance()
        {
            bool createdNew;
            string appSpecificName = Environment.UserName + "_" + Constants.Virastyar + "_Word_" + Application.Version;
            this.m_appSpecificMutex = new Mutex(true, appSpecificName, out createdNew);
            return createdNew;
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            if (!IsFirstAddinInstance())
            {
                //MessageBox.Show("Ok, there is another instance running!");
                return;
            }

            this.m_wordApp = Application;

            this.m_appSettings = (Settings)SettingsBase.Synchronized(Settings.Default);

            try
            {
#if DEBUG
                this.m_log = LogManager.GetLogger(Constants.Virastyar);
                this.m_log.Info("Session Started");
#endif
                CheckAddinPrerequisites();

                //TemplateGenerator.GetGenerator(OfficeVersion.Office2003).
                //    Generate(this, this.m_wordApp, @"D:\Virastyar2003.dot");

                //return;

                m_virastyarTemplate = LoadAddinTemplate();

                #region Init Fields

                this.m_allCharactersRefinerSettings = new AllCharactersRefinerSettings(Settings.Default);

                // Initializing a new instance of SpellCheckerWrapper, and give all necessary configurations to it
                // Every new configuration parameter must be passed to SpellCheckerWrapper.
                // TODO: An instance object is needed to reduce parameter counts here.
                this.m_spellCheckerWrapper = new SpellCheckerWrapper(FindUserDictionaryLocation(),
                    (int)Settings.Default.Config_EditDistance, (int)Settings.Default.Config_MaxSuggestions,
                    SpellCheckerWrapper.GetDictionariesArray(
                        FindMainDictionaryLocation(),
                        this.m_appSettings.SpellChecker_MainDictionarySelected,
                        this.m_appSettings.SpellChecker_CustomDictionaries,
                        this.m_appSettings.SpellChecker_CustomDictionariesSelectionFlag
                    ), this.m_appSettings.SpellChecker_VocabSpaceCorrection, this.m_appSettings.SpellChecker_DontCheckSingleLetters, this.m_appSettings.SpellChecker_HeYeConvertion,
                    this.m_appSettings.PreprocessSpell_RefineAllAffixes,this.m_appSettings.PreprocessSpell_RefineBe,
                    this.m_appSettings.PreprocessSpell_RefineHaa, this.m_appSettings.PreprocessSpell_RefineHeYe, this.m_appSettings.PreprocessSpell_RefineMee,
                    SettingsHelper.GetFullPath(Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles)
                );

                this.m_pinglishVerifier = null;
                this.m_dateVerifier = new DateVerifier();
                this.m_numberVerifier = new NumberVerifier();
                this.m_puncVerfier = new PunctuationVerifier();

                this.m_usageLogger = new UsageLogger();

                #endregion

                #region Hotkey-Related - To be removed in future

                TryChangeHotkey(
                    Constants.MacroNames.VirastyarAbout_Action,
                    m_appSettings.Config_Shortcut_VirastyarAbout_Action, true);

                TryChangeHotkey(
                    Constants.MacroNames.VirastyarAutoComplete_Action,
                    m_appSettings.Config_Shortcut_VirastyarAutoComplete_Action, true);


                TryChangeHotkey(
                    Constants.MacroNames.VirastyarCheckDates_Action,
                    m_appSettings.Config_Shortcut_VirastyarCheckDates_Action, true);

                TryChangeHotkey(
                    Constants.MacroNames.VirastyarCheckSpell_Action,
                    m_appSettings.Config_Shortcut_VirastyarCheckSpell_Action, true);

                TryChangeHotkey(
                    Constants.MacroNames.VirastyarPreCheckSpell_Action,
                    m_appSettings.Config_Shortcut_VirastyarPreCheckSpell_Action, true);


                TryChangeHotkey(
                    Constants.MacroNames.VirastyarRefineAllCharacters_Action,
                    m_appSettings.Config_Shortcut_VirastyarRefineAllCharacters_Action, true);

                TryChangeHotkey(
                    Constants.MacroNames.VirastyarCheckNumbers_Action,
                    m_appSettings.Config_Shortcut_VirastyarCheckNumbers_Action, true);
                
                TryChangeHotkey(
                    Constants.MacroNames.VirastyarPinglishConvert_Action,
                    m_appSettings.Config_Shortcut_VirastyarPinglishConvert_Action, true);
                
                TryChangeHotkey(
                    Constants.MacroNames.VirastyarPinglishConvertAll_Action,
                    m_appSettings.Config_Shortcut_VirastyarPinglishConvertAll_Action, true);


                TryChangeHotkey(
                    Constants.MacroNames.VirastyarCheckPunctuation_Action,
                    m_appSettings.Config_Shortcut_VirastyarCheckPunctuation_Action, true);


                #endregion

                Debug.Assert(this.m_wordApp != null);

                if (this.m_wordApp == null)
                {
                    throw new Exception("The reference to Word application is null!");
                }

                this.m_addinConfigurationDialog = new AddinConfigurationDialog(this.m_appSettings);
                this.m_addinConfigurationDialog.RepairMenubarsClicked += addinConfigurationDialog_RepairMenubarsClicked;
                this.m_addinConfigurationDialog.RefineAllSettingsChanged += addinConfigurationDialog_RefineAllSettingsChanged;
                this.m_addinConfigurationDialog.SpellCheckSettingsChanged += addinConfigurationDialog_SpellCheckSettingsChanged;

                if (!m_virastyarTemplate.Saved)
                {
                    m_virastyarTemplate.Save();
                }

                if (Settings.Default.TipOfTheDayShowOnStartup)
                {
                    Tip_Action();
                }

                UpdateNotificationWindow.CheckForUpdates(5000);
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        #region Addin Prerequisites -- Startup

        private void CheckAddinPrerequisites()
        {
            // Assuming that we have access to everything ;)

            // 1. AppData\Virastyar (or: "\ProgramData\Virastyar" in Vista and Win7) Folder Exist
            CheckAppDataFolder();

            // 4.
            CheckResourceDependencies();

            // 5.
            FixConnectionString();
        }

        /// <summary>
        /// Only call it by CheckAddinPrerequisites
        /// </summary>
        private void CheckAppDataFolder()
        {
            #region User Data
            
            string userDataFolder = SettingsHelper.GetUserDataPath();
            if (!Directory.Exists(userDataFolder))
            {
                Directory.CreateDirectory(userDataFolder);
            }
            if (string.IsNullOrEmpty(this.m_appSettings.Config_UserDictionaryPath))
            {
                string userDicPath = SettingsHelper.GetFullPath(Constants.UserDicFileName, VirastyarFilePathTypes.UserFiles);
                File.Create(userDicPath).Close();
                this.m_appSettings.Config_UserDictionaryPath = userDicPath;
            }

            #endregion

            #region All Users Data

            string allUsersDataFolder = SettingsHelper.GetCommonDataPath();
            if (!Directory.Exists(allUsersDataFolder))
            {
                Directory.CreateDirectory(allUsersDataFolder);
            }

            #endregion
        }

        /// <summary>
        /// Only call it by CheckAddinPrerequisites
        /// </summary>
        private bool CheckUserSettings()
        {
            try
            {
                RegistryKey virastyarUsersRegKey = Registry.LocalMachine.OpenSubKey(Constants.VirastyarApplicationUsersRegKey, true);
                if (virastyarUsersRegKey == null)
                {
                    virastyarUsersRegKey = Registry.LocalMachine.CreateSubKey(Constants.VirastyarApplicationUsersRegKey,
                        RegistryKeyPermissionCheck.ReadWriteSubTree);
                }

                Debug.Assert(virastyarUsersRegKey != null);

                object exists = virastyarUsersRegKey.GetValue(Environment.UserName, null);

                if (exists == null) // Clean Installation
                {
                    this.m_appSettings.Reset();
                    this.m_appSettings.Save();
                    try
                    {
                        virastyarUsersRegKey.SetValue(Environment.UserName, "Cleaned", RegistryValueKind.String);
                    }
                    catch(Exception ex)
                    {
                        ThisAddIn.DebugWriteLine(ex);
                    }
                }
                virastyarUsersRegKey.Close();
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                // Ignore, limited user
                ThisAddIn.DebugWriteLine("Ignored Exception: " + ex);
                return false;
            }
            catch (SecurityException ex)
            {
                // Ignore, limited user
                ThisAddIn.DebugWriteLine("Ignored Exception: " + ex);
                return false;
            }
        }

        /// <summary>
        /// Only call it by CheckAddinPrerequisites
        /// </summary>
        private void CheckResourceDependencies()
        {
            ResourceManager.Init(Assembly.GetAssembly(typeof(ThisAddIn)));

            CheckAndRestoreDependency(Constants.PatternsFileName, VirastyarFilePathTypes.AllUsersFiles);
            CheckAndRestoreDependency(Constants.PinglishFileName, VirastyarFilePathTypes.AllUsersFiles);
            CheckAndRestoreDependency(Constants.PinglishPreprocessFileName, VirastyarFilePathTypes.AllUsersFiles);
            CheckAndRestoreDependency(Constants.InformalDicFileName, VirastyarFilePathTypes.AllUsersFiles);
            CheckAndRestoreDependency(Constants.MainDicFileName, VirastyarFilePathTypes.AllUsersFiles);
            CheckAndRestoreDependency(Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles);
        }

        private void CheckAndRestoreDependency(string dependencyFileName, VirastyarFilePathTypes fileType)
        {
            string dependencyFilePath = SettingsHelper.GetFullPath(dependencyFileName, fileType);
            CheckAndRestoreDependency(dependencyFileName, dependencyFilePath);
        }

        private void CheckAndRestoreDependency(string dependencyFileName, string dependencyFilePath, bool forceOverwrite)
        {
            if (forceOverwrite)
            {
                try
                {
                    File.Delete(dependencyFilePath);
                    ResourceManager.SaveResourceAs(dependencyFileName, dependencyFilePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
#if DEBUG
                    m_log.Error("Error while deleting " + dependencyFilePath, ex);
#endif
                }
            }
            if (!File.Exists(dependencyFilePath))
            {
                ResourceManager.SaveResourceAs(dependencyFileName, dependencyFilePath);
            }
        }

        private void CheckAndRestoreDependency(string dependencyFileName, string dependencyFilePath)
        {
            CheckAndRestoreDependency(dependencyFileName, dependencyFilePath, false);
        }

        /// <summary>
        /// Only call it by CheckAddinPrerequisites
        /// </summary>
        private void FixConnectionString()
        {
            ConfigurationSettings.AppSettings["connectionString"] =
                String.Format(Settings.Default.SynConnectionStringFormat,
                SettingsHelper.GetFullPath(Settings.Default.SynDBBaseFileName, VirastyarFilePathTypes.AllUsersFiles));
        }

        #endregion

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            WordCompletionForm.BeforeApplicationShutDown();

            try
            {
                UnloadAddinTemplate();
            }
            catch(Exception ex)
            {
                ThisAddIn.DebugWriteLine(ex);
#if DEBUG
                throw ex;
#endif
            }
#if DEBUG
            this.m_log.Info("Session Finished");
            LogManager.Shutdown();
#endif
        }

        #endregion

        #region Button Click Handlers

        public void EnableAllShortcuts()
        {
            // TOOD:
        }

        public void DisableAllShortcuts()
        {
            // TODO
        }

        private void btnAddinSettings_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            AddinSettings_Action();
        }

        private void btnCheckDates_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            CheckDates_Action();
        }

        private void btnCheckNumbers_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            CheckNumbers_Action();
        }

        private void btnCheckPunctuation_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            CheckPunctuation_Action();
        }

        private void btnCheckAllPunctuation_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            CheckAllPunctuation_Action();
        }

        private void btnPinglishConvertAll_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            PinglishConvertAll_Action();
        }

        private void btnPinglishConvert_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            PinglishConvert_Action();
        }

        private void btnSuggestSynonym_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            SuggestSynonym();
        }

        private void btnSuggestRhyme_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            SuggestRhyme();
        }

        private void btnInsertAutoText_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            throw new NotImplementedException();
        }

        private void btnRefineAllCharacters_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            RefineAllCharacters_Action();
        }

        private void btnHelp_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            About_Action();
        }

        private void btnPreCheckSpell_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            PreCheckSpell_Action();
        }


        private void btnCheckSpell_Click(Microsoft.Office.Core.CommandBarButton
            barButton, ref bool someBool)
        {
            CheckSpell_Action();
        }

        #endregion

        #region Hotkey Event Handlers

        //private void PinglishHotkey_Pressed(object sender, EventArgs e)
        //{
        //    PinglishCheck(TextProcessType.Interactive);
        //}

        //private void SpellCheckHotkey_Pressed(object sender, EventArgs e)
        //{
        //    SpellCheck();
        //}

        //private void PunctuationHotkey_Pressed(object sender, EventArgs e)
        //{
        //    PunctuationCheck(TextProcessType.Batch);
        //}

        //private void RefineCharachtersHotkey_Pressed(object sender, EventArgs e)
        //{
        //    RefineAllCharacters();
        //}

        //private void NumberCheckHotkey_Pressed(object sender, EventArgs e)
        //{
        //    VerifyNumbers();
        //}

        //private void SynonymSuggestionHotkey_Pressed(object sender, EventArgs e)
        //{
        //    SuggestSynonym();
        //}

        //private void RhymeSuggestionHotkey_Pressed(object sender, EventArgs e)
        //{
        //    SuggestRhyme();
        //}

        //private void DateCheckHotkey_Pressed(object sender, EventArgs e)
        //{
        //    DateCheck();
        //}

        //private void WordCompletionHotkey_Pressed(object sender, EventArgs e)
        //{
        //}

        #endregion

        #region Main Functionalities

        private void VerifyNumbers()
        {
            try
            {
                this.m_usageLogger.LastCaller = CallerTypes.NumberVerifier;
                this.m_numberVerifier.RunVerify();
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        private void PreSpellCheck()
        {
            try
            {
                if (DialogResult.Yes == PersianMessageBox.Show(
                    "این عملیات ممکن است بسیاری از کلمه‌های سند شما را تغییر دهد." + "\r\n" + 
                    "لطفاً قبل از شروع عملیات از ذخیره بودن تغییرات متن خود اطمینان حاصل کنید." + "\r\n" + 
                    "آیا موافقید که عمل پیش‌پردازش املایی متن آغاز شود؟", 
                    "پیغام", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    this.m_usageLogger.LastCaller = CallerTypes.SpellPreprocessing;


                    if (!this.m_spellCheckerWrapper.Enabled)
                        return;

                    if (this.m_preprocessSpellVerifier == null)
                    {
                        // TODO: What's this mess?
                        PersianSpellChecker engine;
                        if (!GetSpellCheckerEngine(out engine))
                            return;

                        this.m_preprocessSpellVerifier = new PreprocessSpellVerifier(engine);
                    }

                    this.m_preprocessSpellVerifier.RunVerify(TextProcessType.Batch);
                    this.m_preprocessSpellVerifier.ShowStats();
                }
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }

        }

        private void SpellCheck()
        {
            try
            {
                this.m_usageLogger.LastCaller = CallerTypes.SpellChecker;


                if (!this.m_spellCheckerWrapper.Enabled)
                    return;

                if (this.m_spellCheckerVerifier == null)
                {
                    PersianSpellChecker engine;
                    SessionLogger sessionLogger;
                    if (!GetSpellCheckerEngine(out engine) || !GetSpellCheckerUsageSession(out sessionLogger))
                        return;

                    this.m_spellCheckerVerifier = new SpellCheckerVerifier(engine);
                }

                this.m_spellCheckerVerifier.RunVerify();
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        private void PinglishCheck(TextProcessType tpt)
        {
            try
            {
                this.m_usageLogger.LastCaller = CallerTypes.PinglishVerifier;

                if (!this.m_spellCheckerWrapper.Enabled)
                    return;

                if (this.m_pinglishVerifier == null)
                {
                    // TODO: Mehrdad: Experimental
                    this.m_pinglishVerifier = new PinglishVerifier(m_appSettings.SpellChecker_MainDictionaryPath);
                }

                this.m_pinglishVerifier.RunVerify(tpt);
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        private void DateCheck()
        {
            try
            {
                this.m_usageLogger.LastCaller = CallerTypes.DateVerifier;
                this.m_dateVerifier.RunVerify();
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        private void PunctuationCheck(TextProcessType processType)
        {
            try
            {
                this.m_usageLogger.LastCaller = CallerTypes.PunctuationVerifer;
                this.m_puncVerfier.RunVerify(processType);
                if (processType == TextProcessType.Batch)
                    this.m_puncVerfier.ShowStats();
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        private void RefineAllCharacters()
        {
            try
            {
                if (DialogResult.Yes == PersianMessageBox.Show("این عملیات ممکن است بسیاری از نویسه‌های سند شما را تغییر دهد." + "\r\n" + "لطفاً قبل از شروع عملیات از ذخیره بودن تغییرات متن خود اطمینان حاصل کنید." + "\r\n" + "آیا موافقید که عمل اصلاح تمامی نویسه‌های متن آغاز شود؟", "پیغام", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    AllCharactersRefiner refiner = new AllCharactersRefiner(this.m_allCharactersRefinerSettings);
                    refiner.RunVerify(TextProcessType.Batch);
                    refiner.ShowStats();
                }
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        private void SuggestRhyme()
        {
            SuggestSynonymOrRime(false);
        }

        private void SuggestSynonym()
        {
            SuggestSynonymOrRime(true);
        }

        private void SuggestSynonymOrRime(bool isSysnonym)
        {
            /*try
            {
                if (m_wordApp.Selection == null)
                    return;

                Range cursorWord = RangeUtils.GetWordOfSelection(m_wordApp.Selection);
                if (cursorWord == null)
                    return;

                RangeUtils.TrimRange(cursorWord);
                if (cursorWord.Text == null)
                    return;

                cursorWord.Select();

                string sug;
                bool dialogResult;
                if (isSysnonym)
                    dialogResult = SynonymSuggester.WordSuggestionWindow.ShowDialog(cursorWord.Text, out sug);
                else
                    dialogResult = SynonymSuggester.RhymeSuggestionWindow.ShowDialog(cursorWord.Text, out sug);

                if (dialogResult)
                {
                    if (cursorWord.Text != sug)
                        SetRangeContent(cursorWord, sug, true);
                }
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }*/
        }

        void TryCompactMemory()
        {
            GC.Collect();

            // Mehrdad: It's added in 3.5 SP1 !!!
            //GCNotificationStatus status = GC.WaitForFullGCComplete(2500);
        }

        #endregion

        /// <summary>
        /// Gets the spell checker engine.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <returns></returns>
        public bool GetSpellCheckerEngine(out PersianSpellChecker engine)
        {
            engine = null;
            const int maxTry = 5;

            for (int tryCount = 0; tryCount < maxTry; tryCount++)
            {
                engine = this.m_spellCheckerWrapper.Engine;
                if (this.m_spellCheckerWrapper.IsInitialized)
                {
                    break;
                }

                // Else
                if (PersianMessageBox.Show("بنظر می‌رسد فایل واژه‌نامه بدرستی انتخاب نشده‌است. آیا مایل به انتخاب مجدد هستید؟",
                   "اشکال در واژه‌نامه", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    break;
                }

                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (this.m_spellCheckerWrapper.Initialize(dlg.FileName))
                        break;
                }
            }

            if (!this.m_spellCheckerWrapper.IsInitialized)
            {
                PersianMessageBox.Show("قابلیت‌های مرتبط با تصحیح واژه غیرفعال گردید. برای فعال کردن مجدد، در بخش تنظیمات محل فایل واژه‌نامه را تعیین کنید.",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisableSpellCheckFunctionalities();
                this.m_spellCheckerWrapper.Disable();
            }
            else
            {
                this.m_appSettings.Config_UserDictionaryPath = this.m_spellCheckerWrapper.Config.DicPath;
                this.m_appSettings.Save();
                engine = this.m_spellCheckerWrapper.Engine;
            }

            bool check = this.m_spellCheckerWrapper.IsInitialized == (engine != null);
            Debug.Assert(check);
            if (!check)
            {
                throw new Exception("SpellCheckerEngine is not initialize properly!");
            }

            return (engine != null);
        }

        /// <summary>
        /// Gets the spell checker usage session.
        /// </summary>
        /// <returns></returns>
        public bool GetSpellCheckerUsageSession(out SessionLogger sessionLogger)
        {
            sessionLogger = this.m_spellCheckerWrapper.SessionLogger;

            return (sessionLogger != null);
        }

        #region Other Methods

        /// <summary>
        /// Disables the spell-check functionalities.
        /// </summary>
        private void DisableSpellCheckFunctionalities()
        {
            try
            {
                WordCompletionForm.GlobalEnabled = false;
                //m_btnCheckSpell.Enabled = false;
                //m_btnPreCheckSpell.Enabled = false;
                //m_btnPinglishConvert.Enabled = false;
                //m_btnPinglishConvertAll.Enabled = false;
            }
            catch (Exception ex)
            {
#if DEBUG
                this.m_log.Debug("DisableSpellCheckFunctionalities", ex);
#endif
                DebugWriteLine("DisableSpellCheckFunctionalities:" + ex);
            }
        }

        /// <summary>
        /// Enables the spell-check functionalities.
        /// </summary>
        private void EnableSpellCheckFunctionalities()
        {
            try
            {
                WordCompletionForm.GlobalEnabled = true;
                //m_btnCheckSpell.Enabled = true;
                //m_btnPreCheckSpell.Enabled = true;
                //m_btnPinglishConvert.Enabled = true;
                //m_btnPinglishConvertAll.Enabled = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                this.m_log.Debug("EnableSpellCheckFunctionalities:", ex);
#endif
                DebugWriteLine("EnableSpellCheckFunctionalities:" + ex);
            }
        }

        public bool IsWordActive()
        {
            bool isActive = false;

            IntPtr activeWindow = User32.GetForegroundWindow();
            isActive = (Process.GetCurrentProcess().MainWindowHandle == activeWindow);

            //ThisAddIn.DebugWriteLine("Word Handle: " + Process.GetCurrentProcess().MainWindowHandle +
            //                "\nActiveWin Handle: " + activeWindow.ToString());
            //ThisAddIn.DebugWriteLine("I think Word is " + (isActive ? "active" : "not active"));

            return isActive;
        }

        /// <summary>
        /// Finds the dictionary location.
        /// </summary>
        /// <returns></returns>
        public string FindUserDictionaryLocation()
        {
            string userDicPath = "";
            if (File.Exists(this.m_appSettings.Config_UserDictionaryPath))
            {
                userDicPath = this.m_appSettings.Config_UserDictionaryPath;
            }
            else
            {
                userDicPath = SettingsHelper.GetFullPath(Constants.UserDicFileName, VirastyarFilePathTypes.UserFiles);

                if (!File.Exists(userDicPath))
                {
                    PersianMessageBox.Show("فایل واژه‌نامه‌ی شخصی کاربر یافت نشد. لطفاً محل آن را تعیین کنید."
                        + Environment.NewLine + "در صورت انصراف، محل واژه‌نامه‌ی شخصی کاربر را می‌توانید از طریق صفحه تنظیمات نیز تغییر دهید.",
                        "واژه‌نامه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var fileDiag = new OpenFileDialog();
                    if (fileDiag.ShowDialog() == DialogResult.OK)
                    {
                        userDicPath = fileDiag.FileName;
                    }
                    else
                    {
                        try
                        {
                            File.Create(userDicPath);
                        }
                        catch (Exception ex)
                        {
                            DebugWriteLine(ex);
                        }
                    }
                }
            }

            // Save Settings
            this.m_appSettings.Config_UserDictionaryPath = userDicPath;
            this.m_appSettings.Save();
            return userDicPath;
        }

        public string FindMainDictionaryLocation()
        {
            string dicPath = "";
            if (File.Exists(this.m_appSettings.SpellChecker_MainDictionaryPath))
            {
                dicPath = this.m_appSettings.SpellChecker_MainDictionaryPath;
            }
            else
            {
                dicPath = SettingsHelper.GetFullPath(Constants.MainDicFileName, VirastyarFilePathTypes.AllUsersFiles);
                Debug.Assert(File.Exists(dicPath));
                // Save Settings
                this.m_appSettings.SpellChecker_MainDictionaryPath = dicPath;
                this.m_appSettings.Save();
            }

            return dicPath;
        }

        public void ShowHelp(string helpTopicFileName)
        {
            if (helpTopicFileName.Length > 0)
            {
                try
                {
                    Help.ShowHelp(null, SettingsHelper.GetFullPath(HelpConstants.HelpFileName, VirastyarFilePathTypes.InstallationFiles), HelpNavigator.Topic, helpTopicFileName);
                }
                catch
                {
                    PersianMessageBox.Show("خطا در نمایش فایل راهنما");
                }
            }
            else
            {
                if (HelpConstants.MainIntro.Length > 0)
                    ShowHelp(HelpConstants.MainIntro);
            }
        }

        public void OnExceptionOccured(Exception ex)
        {
            ExceptionForm.ShowExceptionForm(ex);
        }

        #endregion

        #region AddinConfiguration

        private void addinConfigurationDialog_SpellCheckSettingsChanged(SpellCheckSettingsChangedEventArgs e)
        {
            try
            {
                this.m_spellCheckerWrapper.Initialize(e);                
                e.CancelLoadingUserDictionary = !this.m_spellCheckerWrapper.IsInitialized;
                if (this.m_spellCheckerWrapper.IsInitialized)
                    EnableSpellCheckFunctionalities();
                else
                    DisableSpellCheckFunctionalities();

                TryCompactMemory();
            }
            catch (Exception ex)
            {
#if DEBUG
                this.m_log.Debug("Error in loading dictionary.", ex);
#endif
                OnExceptionOccured(ex);
            }
        }

        private void addinConfigurationDialog_RepairMenubarsClicked(object sender, EventArgs e)
        {
            // TODO: Mouse cursor
            WordUIHelper.DeleteOldToolbars(this.m_wordApp, Constants.ToolbarPrefix);
            WordUIHelper.DeleteOldToolbars(this.m_wordApp, Constants.OldToolbarPrefix);
            //TODO: CreateMenuAndToolbars(false);
        }

        private void addinConfigurationDialog_RefineAllSettingsChanged(RefineAllSettingsChangedEventArgs e)
        {
            this.m_allCharactersRefinerSettings = e.Settings;
        }

        #endregion

        #region Some Utilities

        public bool IsSelectionStartedInBlock(Selection s, MSWordBlock b)
        {
            if (s.StoryType == b.Range.StoryType)
            {
                if ((s.Start >= b.Range.Start) &&
                    (s.Start <= b.Range.End))
                    return true;
            }
            return false;
        }

        public bool IsSelectionInBlock(Selection s, MSWordBlock b)
        {
            if (s.StoryType == b.Range.StoryType)
            {
                if ((s.Start >= b.Range.Start) &&
                    (s.End <= b.Range.End))
                    return true;
            }
            return false;
        }

        public bool AreBlocksEqual(MSWordBlock a, MSWordBlock b)
        {
            if (a == b) return true;

            if (a.StoryType == b.StoryType)
            {
                if ((a.Range.Start == b.Range.Start) &&
                    (a.Range.End == b.Range.End))
                    return true;
            }
            return false;
        }

        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key, bool isCaseInsensitive)
        {
            return EnumerateMatchingParagraphs(key, null, isCaseInsensitive);
        }

        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key)
        {
            return EnumerateMatchingParagraphs(key, null, false);
        }

        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key1, string key2)
        {
            return EnumerateMatchingParagraphs(key1, key2, false);
        }

        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key1, string key2, bool isCaseInsensitive)
        {
            MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument;
            if (d == null) yield break;

            string regexPattern = "";

            if (String.IsNullOrEmpty(key1) && String.IsNullOrEmpty(key2))
                yield break;
            else if (!String.IsNullOrEmpty(key1) && !String.IsNullOrEmpty(key2))
                regexPattern = String.Format(@"\b{0}\b\s*\b{1}\b", key1, key2);
            else if (String.IsNullOrEmpty(key1))
                regexPattern = String.Format(@"\b{0}\b", key2);
            else if (String.IsNullOrEmpty(key2))
                regexPattern = String.Format(@"\b{0}\b", key1);

            foreach (MSWordBlock bp in ((MSWordBlock)d.GetContent()).RawParagraphs)
            {
                string parContent = StringUtil.RefineAndFilterPersianWord(bp.Range.Text);

                if (isCaseInsensitive)
                {
                    parContent = parContent.ToLower();
                    regexPattern = regexPattern.ToLower();
                }

                Match m = Regex.Match(parContent, regexPattern);
                if (m.Success)
                    yield return bp;
            }
        }

        #region Replace All Stuff : The newer version with less COM calls
        public void ReplaceAll(string what, string with)
        {
            if (what == with)
                return;

            foreach (MSWordBlock p in EnumerateMatchingParagraphs(what))
                foreach (MSWordBlock w in p.Words)
                {
                    if (w.Content == what)
                        SetRangeContent(w.Range, with, false);
                }
        }

        public void ReplaceAllCaseInsensitive(string what, string with)
        {
            what = what.ToLower();
            if (what == with)
                return;

            foreach (MSWordBlock p in EnumerateMatchingParagraphs(what, true))
                foreach (MSWordBlock w in p.Words)
                {
                    if (w.Content.ToLower() == what)
                        SetRangeContent(w.Range, with, false);
                }
        }

        public void ReplaceAllTwoWordsWithOne(string firstStr, string secondStr, string with)
        {
            foreach (MSWordBlock p in EnumerateMatchingParagraphs(firstStr, secondStr))
            {
                MSWordBlock first = null, second = null;
                bool isFirst = true;
                foreach (MSWordBlock w in p.Words)
                {
                    if (isFirst)
                    {
                        second = w;
                        isFirst = false;
                    }
                    else
                    {
                        first = second;
                        second = w;
                        if (first != null && second != null &&
                            StringUtil.RefineAndFilterPersianWord(first.Content) == firstStr &&
                            StringUtil.RefineAndFilterPersianWord(second.Content) == secondStr)
                        {
                            ReplaceTwoRangesWithOne(first.Range, second.Range, with, false);
                        }
                    }
                }
            }
        }
        #endregion

        #region Replace All Stuff: The older version with more COM calls (DO NOT DELETE)
        //public void ReplaceAll(string what, string with)
        //{
        //    if (what == with)
        //        return;

        //    using (MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument)
        //    {
        //        if (d == null) return;

        //        foreach (MSWordBlock w in d.GetContent().Words)
        //        {
        //            if (w.Content == what)
        //                SetRangeContent(w.Range, with, false);
        //        }
        //    }
        //}

        //public void ReplaceAllCaseInsensitive(string what, string with)
        //{
        //    what = what.ToLower();
        //    if (what == with)
        //        return;

        //    using (MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument)
        //    {
        //        if (d == null) return;

        //        foreach (MSWordBlock w in d.GetContent().Words)
        //        {
        //            if (w.Content.ToLower() == what)
        //                SetRangeContent(w.Range, with, false);
        //        }
        //    }
        //}

        //public void ReplaceAllTwoWordsWithOne(string firstStr, string secondStr, string with)
        //{
        //    MSWordDocument d = Globals.ThisAddIn.ActiveMSWordDocument;
        //    if (d == null) return;

        //    MSWordBlock first = null, second = null;
        //    bool isFirst = true;
        //    foreach (MSWordBlock w in d.GetContent().Words)
        //    {
        //        if (isFirst)
        //        {
        //            second = w;
        //            isFirst = false;
        //        }
        //        else
        //        {
        //            first = second;
        //            second = w;
        //            if (first != null && second != null &&
        //                StringUtil.RefineAndFilterPersianWord(first.Content) == firstStr &&
        //                StringUtil.RefineAndFilterPersianWord(second.Content) == secondStr)
        //            {
        //                ReplaceTwoRangesWithOne(first.Range, second.Range, with, false);
        //            }
        //        }
        //    }
        //}

        #endregion

        public void ReplaceTwoRangesWithOne(Range first, Range second, string with, bool showErrMsg)
        {
            //Debug.Assert(first != null && second != null);

            Range r = second;

            if (first == null && second == null)
            {
                return;
            }
            else if (first == null && second != null)
            {
                r = second;
                if (r.Text != with)
                {
                    SetRangeContent(r, with, showErrMsg);
                    RangeUtils.ShrinkRangeToLastWord(r);
                }
            }
            else if (first != null && second == null)
            {
                r = first;
                if (r.Text != with)
                {
                    SetRangeContent(r, with, showErrMsg);
                    RangeUtils.ShrinkRangeToLastWord(r);
                }
            }
            else
            {
                int lastSpaceIndex = with.LastIndexOf(' ');
                if (0 < lastSpaceIndex && lastSpaceIndex < with.Length - 1)
                {
                    string sugPart2 = with.Substring(lastSpaceIndex + 1);
                    string sugPart1 = with.Substring(0, with.Length - sugPart2.Length - 1);

                    Globals.ThisAddIn.SetRangeContent(first, sugPart1, showErrMsg);
                    Globals.ThisAddIn.SetRangeContent(second, sugPart2, showErrMsg);
                    RangeUtils.ShrinkRangeToLastWord(first);
                }
                else
                {
                    r = second;
                    r.SetRange(first.Start, second.End);
                    if (r.Text != with)
                        SetRangeContent(r, with, showErrMsg);
                }
            }
        }

        public void ReplaceTwoRangesWithOne(Range first, Range second, string with)
        {
            ReplaceTwoRangesWithOne(first, second, with, true);
        }


        public bool SetRangeContent(Range r, string content, bool showErrorMessage)
        {
            try
            {
                string rText = r.Text;
                if (rText != content)
                {
                    string beforeSpace, afterSpace;
                    string orgBeforeSpChar, orgAfterSpChar, repBeforeSpChar, repAfterSpChar;
                    if (IsSpecialCodeReplacement(rText, content, out orgBeforeSpChar, out orgAfterSpChar, out repBeforeSpChar, out repAfterSpChar))
                    {
                        //MessageBox.Show("Special Code Replacement!");

                        if(orgBeforeSpChar.Length > 0)
                        {
                            Range prior = r.GetSubRange(0, orgBeforeSpChar.Length - 1);
                            SetRangeContent(prior, repBeforeSpChar, showErrorMessage);
                        }

                        if (orgAfterSpChar.Length > 0)
                        {
                            Range posterior = r.GetSubRange(orgBeforeSpChar.Length /* - 1 + 1 */ + 1);
                            SetRangeContent(posterior, repAfterSpChar, showErrorMessage);
                        }
                    }
                    else
                    {
                        if (!IsOneSpaceInsertion(rText, content, out beforeSpace, out afterSpace))
                        {
                            r.Text = content;
                        }
                        else
                        {
                            //MessageBox.Show(String.Format("One Space Insertion!\r\nBefore:*{0}*\r\nAfter:*{1}*",
                            //    beforeSpace, afterSpace));

                            Range rFirstChar = null;
                            bool isFirstWordPersian = false;
                            if (beforeSpace.Length > 0)
                            {
                                rFirstChar = r.GetValidCharAt(beforeSpace.Length /* - 1 + 1 */);
                                isFirstWordPersian = StringUtil.IsArabicLetter(rFirstChar.Text[0]);
                            }

                            Range rSecondChar = null;
                            bool isSecondWordPersian = false;
                            if (afterSpace.Length > 0)
                            {
                                rSecondChar = r.GetValidCharAt(beforeSpace.Length /* - 1 + 1 */ + 1);
                                isSecondWordPersian = StringUtil.IsArabicLetter(rSecondChar.Text[0]);
                            }

                            bool tryAddToSecondFirst = false;

                            if (rFirstChar == null && rSecondChar == null)
                                return false;
                            else if (rFirstChar != null && rSecondChar == null)
                                tryAddToSecondFirst = false;
                            else if (rFirstChar == null && rSecondChar != null)
                                tryAddToSecondFirst = true;
                            else
                                tryAddToSecondFirst = (isSecondWordPersian && !isFirstWordPersian);

                            if (tryAddToSecondFirst)
                            {
                                try
                                {
                                    if (rSecondChar != null)
                                        rSecondChar.Text = " " + rSecondChar.Text;
                                }
                                catch (COMException)
                                {
                                    try
                                    {
                                        if (rFirstChar != null)
                                            rFirstChar.Text = rFirstChar.Text + " ";
                                    }
                                    catch (COMException)
                                    {
                                        throw new COMException();
                                    }
                                }
                            }
                            else // if try add to first first!
                            {
                                try
                                {
                                    if (rFirstChar != null)
                                        rFirstChar.Text = rFirstChar.Text + " ";
                                }
                                catch (COMException)
                                {
                                    try
                                    {
                                        if (rSecondChar != null)
                                            rSecondChar.Text = " " + rSecondChar.Text;
                                    }
                                    catch (COMException)
                                    {
                                        throw new COMException();
                                    }
                                }
                            }
                        } // end of else (if isOneSpaceInsertion)
                    } // end of else (if isSpecialCodeReplacement)
                } // end of if (rText != content)
            } // end of try
            catch (COMException)
            {
                if(showErrorMessage)
                    PersianMessageBox.Show("تغییر موردنظر قابل اعمال نیست.\r\nابرپیوندها یا ارجاعات درون متنی را بررسی کنید.");
                return false;
            }

            return true;
        }

        public bool IsSpecialCodeReplacement(string original, string replacement, out string orgBeforeSpCode, out string orgAfterSpCode, out string repBeforeSpCode, out string repAfterSpCode)
        {
            repBeforeSpCode = repAfterSpCode = orgBeforeSpCode = orgAfterSpCode = "";

            int orgSpIndex = -1;
            int repSpIndex = -1;
            char spChar = ' ';
            if (StringUtil.StringContainsAny(original, WordSpecialCharacters.SpecialCharsArray, out orgSpIndex))
            {
                spChar = original[orgSpIndex];
                repSpIndex = replacement.IndexOf(spChar);
                if (repSpIndex >= 0)
                {
                    orgBeforeSpCode = original.Substring(0, orgSpIndex);
                    if (orgSpIndex < original.Length - 1)
                        orgAfterSpCode = original.Substring(orgSpIndex + 1);

                    repBeforeSpCode = replacement.Substring(0, repSpIndex);
                    if (repSpIndex < replacement.Length - 1)
                        repAfterSpCode = replacement.Substring(repSpIndex + 1);

                    return true;
                }
            }

            return false;
        }

        public bool IsOneSpaceInsertion(string original, string replacement, out string beforeSpace, out string afterSpace)
        {
            beforeSpace = afterSpace = "";
            int firstSpaceIndex = replacement.IndexOf(" ");
            if (firstSpaceIndex < 0)
                return false;
            string repCopy = replacement.Remove(firstSpaceIndex, 1);
            if (repCopy != original)
                return false;

            beforeSpace = replacement.Substring(0, firstSpaceIndex);
            if(firstSpaceIndex < replacement.Length - 1)
                afterSpace = replacement.Substring(firstSpaceIndex + 1);

            return true;
        }

        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += ThisAddIn_Startup;
            this.Shutdown += ThisAddIn_Shutdown;
        }

        #endregion

        private Template LoadAddinTemplate()
        {
            object install = true;

            string templatePath;
            try
            {
                templatePath = SettingsHelper.GetOfficeUserTemplatesLocation();
            }
            catch (InvalidOperationException ex)
            {
                PersianMessageBox.Show("هیچ محل امنی (Trusted Location) در تنظیمات یافت نشد." + Environment.NewLine +
                    "برای فعال شدن ویراستیار، با استفاده از مسیر زیر یک محل امن در تنظمیات اضافه کنید. سپس برنامه را بسته و مجدداً باز کنید." +
                        Environment.NewLine +
                    "Word Options-> Trust Center-> Trust Center Settings-> Trusted Locations-> Add New Location",
                    "No Trusted Location found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                // TODO: Disable addin
                return null;
            }

            string templateName = SettingsHelper.GetVirastyarTemplateName();
            object templateFullPath = Path.Combine(templatePath, templateName);

            foreach (Template installedTemplate in this.m_wordApp.Templates)
            {
                if (installedTemplate.Name.CompareTo(templateName) == 0)
                {
                    // Check it's path - it MUST be in the user templates folder, which is trusted by Office
                    // If it does not - remove and add it again
                    if (installedTemplate.Path.CompareTo(templatePath) != 0)
                    {
                        SettingsHelper.RemoveCustomization(installedTemplate.Name);
                    }
                    else
                    {
                        //install = false;
                    }
                }
            }
            
            // Always overwrite with the embedded template - Problem: Users customization will be lost!
            install = true;

            if ((bool)install)
            {
                CheckAndRestoreDependency(templateName, templateFullPath.ToString(), true);
                this.m_wordApp.AddIns.Add(templateFullPath.ToString(), ref install);
            }
            return this.m_wordApp.Templates.get_Item(ref templateFullPath);
        }

        private void UnloadAddinTemplate()
        {
            string templatePath;
            try
            {
                templatePath = SettingsHelper.GetOfficeUserTemplatesLocation();
            }
            catch (InvalidOperationException ex)
            {
                return;
            }

            string templateName = SettingsHelper.GetVirastyarTemplateName();
            object templateFullPath = Path.Combine(templatePath, templateName);

            if (this.m_wordApp != null)
            {
                foreach (Template installedTemplate in this.m_wordApp.Templates)
                {
                    if (installedTemplate.Name.ToLowerInvariant().CompareTo(templateName.ToLowerInvariant()) == 0)
                    {
                        installedTemplate.Save();
                        Debug.Assert(installedTemplate.Saved);
                        SettingsHelper.RemoveCustomization(installedTemplate.Name);
                    }
                }
            }
            try
            {
                File.Delete(templateFullPath.ToString());
            }
            catch (Exception)
            {
                Debug.WriteLine("Could not delete " + templateFullPath);
            }
            m_virastyarTemplate = null;
        }

        private object m_oldTemplate = null;

        public void PushOldTemplateAndSetCustom(Template template)
        {
            lock(this)
            {
                m_oldTemplate = this.m_wordApp.CustomizationContext;
                this.m_wordApp.CustomizationContext = template;
            }
        }

        public void PopOldTemplate(Template template)
        {
            lock(this)
            {
                this.m_wordApp.CustomizationContext = m_oldTemplate;
                try
                {
                    template.Save();
                    Debug.Assert(template.Saved);
                }
                catch(Exception ex)
                {
                    ThisAddIn.DebugWriteLine(ex);
                }
            }
        }

        public static void DebugWriteLine(object value, params object[] args)
        {
#if DEBUG
            Debug.WriteLine(String.Format("Virastyar: " + value, args));
#endif
        }

        private const int NotInitialized = -1;
        private static int lastDoEvent = NotInitialized;

        public static void ApplicationDoEvents()
        {
            if (lastDoEvent == NotInitialized)
            {
                lastDoEvent = Environment.TickCount;
            }

            int currentTickCount = Environment.TickCount;

            if (currentTickCount - lastDoEvent >= 10)
            {
                lastDoEvent = currentTickCount;
                System.Windows.Forms.Application.DoEvents();
            }
            else
            {
                // Ignore
            }
        }

        public bool TryChangeHotkey(string configKey, string hotkey, bool showMessage)
        {
            Hotkey newHotkey;
            if (Hotkey.TryParse(hotkey, out newHotkey))
                return TryChangeHotkey(configKey, newHotkey, showMessage);
            else
                return false;
        }

        public bool TryChangeHotkey(string configKey, Hotkey hotkey, bool showMessage)
        {
            if (hotkey == Hotkey.None)
                return true;

            if (WordHotKey.AssignKeyToCommand(m_virastyarTemplate, hotkey, configKey))
            {
                m_appSettings["Config_Shortcut_" + configKey] = hotkey.ToString();
                return true;
            }
            else
            {
                if (showMessage)
                    PersianMessageBox.Show("خطا در تغییر میانبر", MessageBoxIcon.Error);

                return false;
            }
        }

        public bool TryChangeHotkey(string configKey, Hotkey newHotkey, Hotkey oldHotkey, bool showMessage)
        {
            if (oldHotkey != newHotkey)
            {
                if (oldHotkey != Hotkey.None)
                    WordHotKey.RemoveAssignedKey(m_virastyarTemplate, oldHotkey, configKey);

                if (WordHotKey.AssignKeyToCommand(m_virastyarTemplate, newHotkey, configKey))
                {
                    m_appSettings["Config_Shortcut_" + configKey] = newHotkey.ToString();
                    return true;
                }
                else
                {
                    if (showMessage)
                        PersianMessageBox.Show("خطا در تغییر میانبر", MessageBoxIcon.Error);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        #region Pre and Post Actions

        private bool m_isShowRevisionsEnabled = false;

        protected void PreAction()
        {
            m_isShowRevisionsEnabled = Application.ActiveDocument.ShowRevisions;
            if (m_isShowRevisionsEnabled)
            {
                Application.ActiveDocument.ShowRevisions = false;
            }
        }

        protected void PostAction()
        {
            Application.ActiveDocument.ShowRevisions = m_isShowRevisionsEnabled;
        }

        #endregion

        #region IVirastyarAddin Members

        public void PinglishConvert_Action()
        {
            PreAction();
            PinglishCheck(TextProcessType.Interactive);
            PostAction();
        }

        public void PinglishConvertAll_Action()
        {
            PreAction();
            this.m_usageLogger.WriteMessage("Pinglish-Convert-All Pressed");
            PinglishCheck(TextProcessType.Batch);
            PostAction();
        }

        public void CheckDates_Action()
        {
            PreAction();
            DateCheck();
            PostAction();
        }

        public void CheckNumbers_Action()
        {
            PreAction();
            VerifyNumbers();
            PostAction();
        }

        public void CheckSpell_Action()
        {
            PreAction();
            SpellCheck();
            PostAction();
        }

        public void PreCheckSpell_Action()
        {
            PreAction();
            PreSpellCheck();
            PostAction();
        }

        public void CheckPunctuation_Action()
        {
            PreAction();
            this.m_usageLogger.LastCaller = CallerTypes.PunctuationVerifer;
            PunctuationCheck(TextProcessType.Interactive);
            PostAction();
        }

        public void CheckAllPunctuation_Action()
        {
            PreAction();
            this.m_usageLogger.WriteMessage("Check-All-Punctuation Pressed");
            PunctuationCheck(TextProcessType.Batch);
            PostAction();
        }

        public void RefineAllCharacters_Action()
        {
            PreAction();
            RefineAllCharacters();
            PostAction();
        }

        public void AddinSettings_Action()
        {
            this.m_addinConfigurationDialog.LoadConfigurations();
            this.m_addinConfigurationDialog.ShowDialog();
        }

        public void About_Action()
        {
            AboutWindow winAbout = new AboutWindow();
            winAbout.ShowDialog();
        }

        public void AutoComplete_Action()
        {
            if (WordCompletionForm.GlobalEnabled)
                WordCompletionForm.WordCompletionEventHandler(this, EventArgs.Empty);
        }

        public void Help_Action()
        {
            Globals.ThisAddIn.ShowHelp(HelpConstants.MainIntro);
        }

        public void Tip_Action()
        {
            try
            {
                Stream tipsStream = ResourceManager.GetResource(Constants.TipsOfTheDayFileName);
                TipOfTheDayForm.ShowTipOfTheDay(tipsStream);
            }
            catch (Exception ex)
            {
                ExceptionForm.ShowExceptionForm(ex);
            }
        }

        #endregion
    }
}

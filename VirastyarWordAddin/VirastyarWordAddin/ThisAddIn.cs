using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using SCICT.Utility.Windows;
using VirastyarWordAddin.Configurations;
using VirastyarWordAddin.Log;
using VirastyarWordAddin.Properties;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.ResourceManagement;
using SCICT.Utility.GUI;
using SCICT.Utility.SpellChecker;
using SCICT.Utility.Keyboard;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SCICT.Microsoft.Word;
using VirastyarWordAddin.Update;
using VirastyarWordAddin.Verifiers.Basics;
using VirastyarWordAddin.Verifiers.CharacterRefinementVerification;
using VirastyarWordAddin.Verifiers.PunctuationVerification;
using VirastyarWordAddin.Verifiers.PinglishVerification;
using VirastyarWordAddin.Verifiers.DateVerification;
using VirastyarWordAddin.Verifiers.NumberVerification;
using VirastyarWordAddin.Verifiers.SpellCheckerVerification;

namespace VirastyarWordAddin
{
    #region VirastyarFilePathTypes enum
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
    #endregion

    /// <summary>
    /// The base class for Add-in
    /// </summary>
    public partial class ThisAddIn : IVirastyarAddin
    {
        #region Private Members

        private Template m_virastyarTemplate;

        private NLog.Logger m_logger;

        private Settings m_appSettings;
        private AllCharactersRefinerSettings m_allCharactersRefinerSettings;

        private AddinConfigurationDialog m_addinConfigurationDialog;

        private Mutex m_appSpecificMutex = null;

        private Form m_currentDialog;
        private ProcessStartInfo m_processStartInfo;

        private object m_oldTemplate = null;

        private const int NotInitialized = -1;
        private static int s_lastDoEvent = NotInitialized;

        private readonly VirastyarAddin m_virastyarAddin = new VirastyarAddin();

        #endregion

        #region Public Members

        public static System.Version InstalledVersion
        {
            get
            {
                try
                {
                    string virastyarAppData = SettingsHelper.GetCommonDataPath();
                    System.Version[] versions = Directory.GetFiles(virastyarAppData, "version-*")
                        .OrderByDescending(verStr => verStr)
                        .Select(verStr =>
                            new System.Version(Path.GetFileName(verStr).ToLower().Replace("version-", "")))
                        .ToArray();

                    if (versions.Length > 0)
                        return CheckVersionNumbers(versions[0]);

                    Assembly assembly = Assembly.GetAssembly(typeof(ThisAddIn));
                    return CheckVersionNumbers(assembly.GetName().Version);
                }
                catch (Exception)
                {
                    return new System.Version(1, 3, 1, 0);
                }
            }
        }

        public static string InstallationGuid
        {
            get
            {
                return Settings.Default.InstallationGuid ?? "";
            }
            private set
            {
                try
                {
                    Settings.Default.InstallationGuid = value;
                    Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException("Unable to Set/Save the Installation Guid", ex);
                }
            }
        }
        
        /// <summary>
        /// Indicates the whether the add-in is loaded or not
        /// </summary>
        public bool IsLoaded { get; private set; }

        public SpellCheckerWrapper SpellCheckerWrapper { get; private set; }

        public Template VirastyarTemplate
        {
            get
            {
                return m_virastyarTemplate;
            }
        }

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
                engine = SpellCheckerWrapper.Engine;
                if (SpellCheckerWrapper.IsInitialized)
                {
                    break;
                }

                // Else
                if (PersianMessageBox.Show(GetWin32Window(),
                    "به نظر می‌رسد فایل واژه‌نامه به درستی انتخاب نشده ‌است." + Environment.NewLine + 
                    "آیا مایل به انتخاب مجدد هستید؟",
                   "اشکال در واژه‌نامه", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    break;
                }

                var dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (SpellCheckerWrapper.Initialize(dlg.FileName))
                        break;
                }
            }

            if (!SpellCheckerWrapper.IsInitialized)
            {
                PersianMessageBox.Show(GetWin32Window(),
                    "قابلیت‌های مرتبط با تصحیح واژه غیرفعال شد." + Environment.NewLine + 
                    "برای فعال کردن مجدد، در بخش تنظیمات محل فایل واژه‌نامه را تعیین کنید.",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisableSpellCheckFunctionalities();
                SpellCheckerWrapper.Disable();
            }
            else
            {
                m_appSettings.SpellChecker_UserDictionaryPath = SpellCheckerWrapper.UserDictionary;
                m_appSettings.Save();
                engine = SpellCheckerWrapper.Engine;
            }

            bool check = SpellCheckerWrapper.IsInitialized == (engine != null);
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
            sessionLogger = this.SpellCheckerWrapper.SessionLogger;
            return (sessionLogger != null);
        }

        public static void ApplicationDoEvents()
        {
            if (s_lastDoEvent == NotInitialized)
            {
                s_lastDoEvent = Environment.TickCount;
            }

            int currentTickCount = Environment.TickCount;

            if (currentTickCount - s_lastDoEvent >= 10)
            {
                s_lastDoEvent = currentTickCount;
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

        public void PushOldTemplateAndSetCustom(Template template)
        {
            lock (this)
            {
                m_oldTemplate = this.Application.CustomizationContext;
                this.Application.CustomizationContext = template;
            }
        }

        public void PopOldTemplate(Template template)
        {
            lock (this)
            {
                this.Application.CustomizationContext = m_oldTemplate;
                try
                {
                    template.Save();
                    Debug.Assert(template.Saved);
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException("", ex);
                }
            }
        }

        public static void DebugWriteLine(object value, params object[] args)
        {
            Debug.WriteLine(String.Format("Virastyar: " + value, args));
        }

        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            AppDomain.CurrentDomain.DomainUnload += (e, s) =>
            {
                try
                {
                    if (m_processStartInfo != null)
                        Process.Start(m_processStartInfo);

                    if (UpdateChecker.RunUpdateProgramBeforeClose)
                        UpdateChecker.RunUpdateProgram();
                }
                catch (Exception ex)
                {
                    // the process couldn't be started. This happens for 1 of 3 reasons:

                    // 1. The user cancelled the UAC box
                    // 2. The limited user tried to elevate to an Admin that has a blank password
                    // 3. The limited user tries to elevate as a Guest account                    
                    Debug.WriteLine(ex.ToString());
                }
            };

            this.Startup += ThisAddIn_Startup;
            this.Shutdown += ThisAddIn_Shutdown;
        }

        #endregion

        #region ThisAddIn

        protected override object RequestComAddInAutomationService()
        {
            return m_virastyarAddin;
        }

        private bool IsFirstAddinInstance()
        {
            bool createdNew;
            string appSpecificName = Environment.UserName + "_" + Constants.Virastyar + "_Word_" + OfficeVersion;
            m_appSpecificMutex = new Mutex(true, appSpecificName, out createdNew);
            return createdNew;
        }

        public static string OfficeVersion
        {
            get
            {
                try
                {
                    return Globals.ThisAddIn.Application.Version;
                }
                catch (Exception)
                {
                    return "Unavailable";
                }
            }
        }

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            IsLoaded = false;

            //Debugger.Launch();
            //Debugger.Break();

            var startupThread = new Thread(() =>
            {
                #region Startup Mechanism

                if (!IsFirstAddinInstance())
                {
                    return;
                }

                try
                {
                    if (Application == null)
                    {
                        throw new ArgumentNullException("Application");
                    }
                    var stopWatch = Stopwatch.StartNew();

                    this.m_appSettings = (Settings)SettingsBase.Synchronized(Settings.Default);

                    CheckAndSetInstallationGuid();

                    this.m_logger = NLog.LogManager.GetCurrentClassLogger();

                    LogHelper.Trace("Virastyar version {0} is loading", InstalledVersion);

                    CheckAddinPrerequisites();

                    bool failed = false;
                    Exception failException = null;
                    try
                    {
                        bool needsReset;
                        m_virastyarTemplate = LoadAddinTemplate(out needsReset);
                        if (needsReset)
                            throw new OperationCanceledException("Virastyar loading canceled");
                        failed = (m_virastyarTemplate == null);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        failed = true;
                        failException = ex;
                    }

                    if (failed)
                    {
                        PersianMessageBox.Show(
                            "متأسفانه در بارگذاری ویراستیار خطایی اتفاق افتاده و افزونه غیرفعال شده است",
                            MessageBoxIcon.Error);
                        if (failException != null)
                        {
                            LogHelper.ErrorException("Error in loading Virastyar template", failException);
                        }
                        else
                        {
                            LogHelper.Error("Error in loading Virastyar template");
                        }
                        return;
                    }

                    #region Init Fields

                    this.m_allCharactersRefinerSettings = new AllCharactersRefinerSettings(Settings.Default);

                    // Initializing a new instance of SpellCheckerWrapper, and give all necessary configurations to it
                    // Every new configuration parameter must be passed to SpellCheckerWrapper.
                    // TODO: An instance object is needed to reduce parameter counts here.
                    this.SpellCheckerWrapper =
                        new SpellCheckerWrapper(FindUserDictionaryLocation(),
                                                m_appSettings.SpellChecker_EditDistance,
                                                m_appSettings.SpellChecker_MaxSuggestions,
                                                SpellCheckerWrapper.
                                                    GetDictionariesArray(
                                                        FindMainDictionaryLocation(),
                                                        m_appSettings.SpellChecker_MainDictionarySelected,
                                                        m_appSettings.
                                                            SpellChecker_CustomDictionaries,
                                                        m_appSettings.
                                                            SpellChecker_CustomDictionariesSelectionFlag
                                                    ),
                                                m_appSettings.PreprocessSpell_CorrectPrefix,
                                                m_appSettings.PreprocessSpell_CorrectPostfix,
                                                m_appSettings.PreprocessSpell_CorrectBe,
                                                SettingsHelper.GetFullPath(Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles)
                            );

                    #endregion

                    #region Hotkey-Related

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarAbout_Action,
                        m_appSettings.Config_Shortcut_VirastyarAbout_Action, true);

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarAutoComplete_Action,
                        m_appSettings.Config_Shortcut_VirastyarAutoComplete_Action,
                        true);


                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarCheckDates_Action,
                        m_appSettings.Config_Shortcut_VirastyarCheckDates_Action,
                        true);

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarCheckSpell_Action,
                        m_appSettings.Config_Shortcut_VirastyarCheckSpell_Action,
                        true);

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarPreCheckSpell_Action,
                        m_appSettings.Config_Shortcut_VirastyarPreCheckSpell_Action,
                        true);


                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarRefineAllCharacters_Action,
                        m_appSettings.
                            Config_Shortcut_VirastyarRefineAllCharacters_Action, true);

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarCheckNumbers_Action,
                        m_appSettings.Config_Shortcut_VirastyarCheckNumbers_Action,
                        true);

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarPinglishConvert_Action,
                        m_appSettings.Config_Shortcut_VirastyarPinglishConvert_Action,
                        true);

                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarPinglishConvertAll_Action,
                        m_appSettings.
                            Config_Shortcut_VirastyarPinglishConvertAll_Action, true);


                    TryChangeHotkey(
                        Constants.MacroNames.VirastyarCheckPunctuation_Action,
                        m_appSettings.
                            Config_Shortcut_VirastyarCheckPunctuation_Action, true);


                    #endregion

                    #region Configuration

                    this.m_addinConfigurationDialog =
                        new AddinConfigurationDialog(this.m_appSettings);
                    this.m_addinConfigurationDialog.RepairMenubarsClicked +=
                        AddinConfigurationDialogRepairMenubarsClicked;
                    this.m_addinConfigurationDialog.RefineAllSettingsChanged +=
                        AddinConfigurationDialogRefineAllSettingsChanged;
                    this.m_addinConfigurationDialog.SpellCheckSettingsChanged +=
                        AddinConfigurationDialogSpellCheckSettingsChanged;

                    #endregion

                    if (!m_virastyarTemplate.Saved)
                    {
                        try
                        {
                            m_virastyarTemplate.Save();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.DebugException(
                                "Error in saving the virastyar template", ex);
                        }
                    }

                    IsLoaded = true;

                    #region AutoUpdate, AutomaticReport

                    SetUpdateAvailableToolbarVisibility(false);
                    UpdateChecker.UpdateAvailable += UpdateNotificationWindowUpdateAvailable;
                    UpdateChecker.CheckForUpdate(false);

                    LogReporter.AutomaticReport();

                    #endregion

                    stopWatch.Stop();
                    LogHelper.Trace("Virastyar loaded successfully: " + stopWatch.ElapsedMilliseconds);

                    if (Settings.Default.TipOfTheDayShowOnStartup)
                    {
                        Tip_Action();
                    }
                }
                catch(OperationCanceledException)
                {
                    // Ignore
                }
                catch (Exception ex)
                {
                    OnExceptionOccured(ex);
                    UnloadAddinTemplate();
                }

                #endregion

            });

            try
            {
                startupThread.Start();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorException("Could not start the Startup thread", ex);
            }

            AutomaticReportConfirm.ShowConfirmTray();
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            WordCompletionForm.BeforeApplicationShutDown();

            try
            {
                UnloadAddinTemplate();
                m_appSpecificMutex.Close();
            }
            catch (Exception ex)
            {
                LogHelper.TraceException(ex);
            }
        }

        #region Addin Prerequisites -- Startup

        private void CheckAddinPrerequisites()
        {
            // Assuming that we have access to everything ;)

            // 1. AppData\Virastyar (or: "\ProgramData\Virastyar" in Vista and Win7) Folder Exist
            CheckAppDataFolder();

            // 2.
            CheckDataDependencies(false);

            // 3.
            CheckUpdaterDependencies(false);
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
            if (string.IsNullOrEmpty(m_appSettings.SpellChecker_UserDictionaryPath))
            {
                string userDicPath = SettingsHelper.GetFullPath(Constants.UserDicFileName, VirastyarFilePathTypes.UserFiles);
                if (!File.Exists(userDicPath))
                {
                    File.Create(userDicPath).Close();
                }
                m_appSettings.SpellChecker_UserDictionaryPath = userDicPath;
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

        public bool CheckDataDependencies(bool overwrite)
        {
            LogHelper.Trace("CheckDataDependencies: force={0}", overwrite);
            ResourceManager.Init(Assembly.GetAssembly(typeof(ThisAddIn)));

            bool result = true;
            result &= CheckAndRestoreDependency(Constants.PatternsFileName, VirastyarFilePathTypes.AllUsersFiles, overwrite);
            result &= CheckAndRestoreDependency(Constants.PinglishFileName, VirastyarFilePathTypes.AllUsersFiles, overwrite);
            result &= CheckAndRestoreDependency(Constants.GoftariDicFileName, VirastyarFilePathTypes.AllUsersFiles, overwrite);
            result &= CheckAndRestoreDependency(Constants.ExceptionWordsFileName, VirastyarFilePathTypes.AllUsersFiles, overwrite);
            result &= CheckAndRestoreDependency(Constants.MainDicFileName, VirastyarFilePathTypes.AllUsersFiles, overwrite);
            result &= CheckAndRestoreDependency(Constants.StemFileName, VirastyarFilePathTypes.AllUsersFiles, overwrite);

            // TODO: Some dependencies are vital and some are not. We must distinguish them
            return result;
        }

        public bool CheckUpdaterDependencies(bool overwrite)
        {
            ResourceManager.Init(Assembly.GetAssembly(typeof(ThisAddIn)));

            var fileNames = new[] { Constants.WyUpdateFileName, Constants.WyUpdateClientFileName };

            foreach (var fileName in fileNames)
            {
                string filePath = SettingsHelper.GetFullPath(fileName, VirastyarFilePathTypes.InstallationFiles);
                bool showElevationConfirm = false;
                try
                {
                    if (overwrite)
                    {
                        DeleteFile(filePath);
                    }
                    if (overwrite || !File.Exists(filePath))
                    {
                        File.Create(filePath).Close();
                        File.Delete(filePath);
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    showElevationConfirm = true;
                }

                if (showElevationConfirm)
                {
                    string message = "برخی از فایل‌های مورد نیاز ویراستیار از روی دیسک پاک شده‌اند و یا نیاز به بازنویسی مجدد دارند." + Environment.NewLine +
                                        "برای بازگرداندن آن‌ها، برنامهٔ Word باید بسته شده و با سطح دسترسی مدیر اجرا شود." + Environment.NewLine +
                                        "آیا این عملیات را تأیید می‌کنید؟" + Environment.NewLine +
                                        "------------" + Environment.NewLine +
                                        "توجه: پیش از تأیید مطمئن شوید که هیچ پنجره‌ای در Word باز نیست";
                    ShowElevationConfirm(message);
                    break;
                }
            }

            bool result = CheckAndRestoreDependency(Constants.WyUpdateFileName, VirastyarFilePathTypes.InstallationFiles, overwrite);
            result &= CheckAndRestoreDependency(Constants.WyUpdateClientFileName, VirastyarFilePathTypes.InstallationFiles, overwrite);

            return result;
        }

        private static bool CheckAndRestoreDependency(string dependencyFileName, VirastyarFilePathTypes fileType, bool overwrite)
        {
            string dependencyFilePath = SettingsHelper.GetFullPath(dependencyFileName, fileType);
            return CheckAndRestoreDependency(dependencyFileName, dependencyFilePath, overwrite);
        }

        private void ShowElevationConfirm(string confirmMessage)
        {
            if (IsAdministrator())
                return;

            var result = PersianMessageBox.Show(confirmMessage, "نیاز به سطح دسترسی مدیر", 
                MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    m_processStartInfo = new ProcessStartInfo()
                    {
                        Arguments = "-justelevated",
                        ErrorDialog = true,

                        // Handle is the handle for your form
                        ErrorDialogParentHandle = WindowWrapper.GetWordActiveWindowWrapper().Handle,
                        FileName = Path.Combine(Application.Path, "WINWORD.EXE"),
                        Verb = "runas"
                    };

                    ThisAddIn_Shutdown(this, EventArgs.Empty);
                    
                    object missing = Missing.Value;
                    Globals.ThisAddIn.Application.Quit(ref missing, ref missing, ref missing);

                    throw new OperationCanceledException("Virastyar loading canceled");
                }
                catch (OperationCanceledException)
                {
                    throw;     
                }
                catch (Exception ex)
                {
                    m_processStartInfo = null;
                    LogHelper.TraceException(ex);
                }
            }
        }

        private static bool CheckAndRestoreDependency(string dependencyFileName, string dependencyFilePath, bool overwrite)
        {
            if (overwrite)
            {
                try
                {
                    DeleteFile(dependencyFilePath);
                    return ResourceManager.SaveResourceAs(dependencyFileName, dependencyFilePath);
                }
                catch (Exception ex)
                {
                    ThisAddIn.DebugWriteLine(ex);
                    LogHelper.ErrorException("Error while deleting " + dependencyFilePath, ex);
                }
            }
            else if (!File.Exists(dependencyFilePath))
            {
                return ResourceManager.SaveResourceAs(dependencyFileName, dependencyFilePath);
            }
            return false;
        }

        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }
        }

        #endregion

        private Template LoadAddinTemplate(out bool needsReset)
        {
            needsReset = false;
            string templatePath;
            try
            {
                templatePath = SettingsHelper.GetOfficeUserTemplatesLocation();
            }
            catch (InvalidOperationException ex)
            {
                LogHelper.DebugException("No trusted location found", ex);
                if (AddVirastyarTrustedLocation())
                    needsReset = true;
                return null;
            }

            string templateName = SettingsHelper.GetVirastyarTemplateName();
            object templateFullPath = Path.Combine(templatePath, templateName);

            for (int i = Application.Templates.Count; i >= 1; i--)
            {
                object index = i;
                Template installedTemplate;
                try
                {
                    installedTemplate = Application.Templates.get_Item(ref index);
                }
                catch (COMException ex)
                {
                    LogHelper.ErrorException("Got an exception from get_Item", ex);
                    continue;
                }

                if (installedTemplate == null)
                {
                    LogHelper.Trace("Got a null template from Templates.get_Item");
                    continue;
                }

                if (string.Compare(installedTemplate.Name, templateName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    bool removed = SettingsHelper.RemoveCustomization(installedTemplate.Name);
                    Debug.Assert(removed);
                }
            }

            // Always overwrite with the embedded template
            if (CheckAndRestoreDependency(templateName, templateFullPath.ToString(), true))
            {
                Document newDoc = null;
                object missingValue = Missing.Value;

                var documentsCount = Application.Documents.Count;
                if (documentsCount == 0 || 
                    (Application.ActiveDocument != null && Application.ActiveDocument.ProtectionType != WdProtectionType.wdNoProtection))
                {
                    newDoc = Application.Documents.Add(ref missingValue, ref missingValue, ref missingValue, ref missingValue);
                }

                object install = true;
                Application.AddIns.Add(templateFullPath.ToString(), ref install);

                if (newDoc != null)
                {
                    object saveChanges = false;
                    newDoc.Close(ref saveChanges, ref missingValue, ref missingValue);
                }
                return Application.Templates.get_Item(ref templateFullPath);
            }
            else // if (templateFullPath.ToString().Contains("Program Files"))
            {
                // I'm trying to be cool!
                // We need to add a TrustedLocation manually, wear your seat belts
                if (AddVirastyarTrustedLocation())
                    needsReset = true;
                return null;
            }
        }

        private static bool AddVirastyarTrustedLocation()
        {
            // 1. Create a .reg file
            string userPath = SettingsHelper.GetUserDataPath();
            string regFilePath = Path.Combine(userPath, "Virastyar.reg");

            using (var writer = File.CreateText(regFilePath))
            {
                string regContent = string.Format(
                    Constants.VirastyarTrustedLocationRegString,
                    OfficeVersion,
                    SettingsHelper.GetLastOfficeUserTemplateLocationIndex(),
                    userPath.Replace("\\", "\\\\"));
                writer.Write(regContent);
            }

            // 2. Notify user
            if (PersianMessageBox.Show(GetWin32Window(),
                " ویراستیار نیاز به افزودن یک Trusted Location در تنظیمات برنامهٔ Word دارد" + Environment.NewLine +
                " آیا موافقید که ویراستیار این تغییر را انجام دهد؟" + Environment.NewLine + Environment.NewLine +
                "اگر موافق هستید، پیغام‌های بعدی را نیز تأیید کنید و در انتها، Word را بسته و دوباره اجرا کنید  ",
                "No suitable Trusted Location found",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                // 2. Run regedit
                var pInfo = new ProcessStartInfo("regedit");
                pInfo.Arguments = regFilePath;

                var regedit = Process.Start(pInfo);
                regedit.WaitForExit();
                return (regedit.ExitCode == 0);
            }

            return false;
        }

        /// <summary>
        /// This method does its best to unload the addin's template from Word's templates.
        /// In case of any failures or unexpected behaviour, it does not throws any exception.
        /// So it's safe to call it when the Addin is shuting down.
        /// </summary>
        private void UnloadAddinTemplate()
        {
            string templateName = SettingsHelper.GetVirastyarTemplateName();
            
            try
            {
                if (this.Application != null)
                {
                    for (int i = Application.Templates.Count; i >= 1; i--)
                    {
                        object index = i;
                        Template installedTemplate;
                        try
                        {
                            installedTemplate = Application.Templates.get_Item(ref index);
                        }
                        catch (COMException ex)
                        {
                            LogHelper.ErrorException("Got an exception from get_Item", ex);
                            continue;
                        }

                        if (installedTemplate == null)
                        {
                            LogHelper.Trace("Got a null template from Templates.get_Item");
                            continue;
                        }

                        if (string.Compare(installedTemplate.Name, templateName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (!installedTemplate.Saved)
                                installedTemplate.Save();
                            Debug.Assert(installedTemplate.Saved);
                            SettingsHelper.RemoveCustomization(installedTemplate.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Could not unload the addin template", ex);
            }

            string templateFullPath = "";
            try
            {
                string templatePath = SettingsHelper.GetOfficeUserTemplatesLocation();
                templateFullPath = Path.Combine(templatePath, templateName);

                DeleteFile(templateFullPath);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Could not delete " + templateFullPath, ex);
            }
            m_virastyarTemplate = null;
        }

        #endregion

        #region Main Functionalities

        //private void VerifyNumbers()
        //{
        //    try
        //    {
        //        this.m_numberVerifier.RunVerify();
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

        //private void PreSpellCheck()
        //{
        //    try
        //    {
        //        if (DialogResult.Yes == PersianMessageBox.Show(
        //            "این عملیات ممکن است بسیاری از کلمه‌های سند شما را تغییر دهد." + "\r\n" + 
        //            "لطفاً قبل از شروع عملیات از ذخیره بودن تغییرات متن خود اطمینان حاصل کنید." + "\r\n" + 
        //            "آیا موافقید که عمل پیش‌پردازش املایی متن آغاز شود؟", 
        //            "پیش‌پردازش املایی", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
        //        {
        //            if (!this.SpellCheckerWrapper.Enabled)
        //                return;

        //            if (this.m_preprocessSpellVerifier == null)
        //            {
        //                // TODO: What's this mess?
        //                PersianSpellChecker engine;
        //                if (!GetSpellCheckerEngine(out engine))
        //                    return;

        //                this.m_preprocessSpellVerifier = new PreprocessSpellVerifier(engine);
        //            }

        //            this.m_preprocessSpellVerifier.RunVerify(TextProcessType.Batch);
        //            this.m_preprocessSpellVerifier.ShowStats();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

        //private void SpellCheck()
        //{
        //    try
        //    {
        //        if (!this.SpellCheckerWrapper.Enabled)
        //            return;

        //        if (this.m_spellCheckerVerifier == null)
        //        {
        //            PersianSpellChecker engine;
        //            SessionLogger sessionLogger;
        //            if (!GetSpellCheckerEngine(out engine) || !GetSpellCheckerUsageSession(out sessionLogger))
        //                return;

        //            this.m_spellCheckerVerifier = new SpellCheckerVerifier(engine, sessionLogger);
        //        }

        //        this.m_spellCheckerVerifier.RunVerify();
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

        //private void PinglishCheck(TextProcessType tpt)
        //{
        //    try
        //    {
        //        if (!this.SpellCheckerWrapper.Enabled)
        //            return;

        //        if (this.m_pinglishVerifier == null)
        //        {
        //            // TODO: Mehrdad: Experimental
        //            this.m_pinglishVerifier = new PinglishVerifier(m_appSettings.SpellChecker_MainDictionaryPath);
        //        }

        //        this.m_pinglishVerifier.RunVerify(tpt);
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

        //private void DateCheck()
        //{
        //    try
        //    {
        //        this.m_dateVerifier.RunVerify();
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

        //private void PunctuationCheck(TextProcessType processType)
        //{
        //    try
        //    {
        //        this.m_puncVerfier.RunVerify(processType);
        //        if (processType == TextProcessType.Batch)
        //            this.m_puncVerfier.ShowStats();
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

        //private void RefineAllCharacters()
        //{
        //    try
        //    {
        //        if (DialogResult.Yes == PersianMessageBox.Show(
        //            "این عملیات ممکن است بسیاری از نویسه‌های سند شما را تغییر دهد." + "\r\n" + "لطفاً قبل از شروع عملیات از ذخیره بودن تغییرات متن خود اطمینان حاصل کنید." + "\r\n" + "آیا موافقید که عمل اصلاح تمامی نویسه‌های متن آغاز شود؟", 
        //            "اصلاح نویسه‌های متن", 
        //            MessageBoxButtons.YesNo, 
        //            MessageBoxIcon.Question))
        //        {
        //            AllCharactersRefiner refiner = new AllCharactersRefiner(this.m_allCharactersRefinerSettings);
        //            refiner.RunVerify(TextProcessType.Batch);
        //            refiner.ShowStats();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        OnExceptionOccured(ex);
        //    }
        //}

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

        static void TryCompactMemory()
        {
            GC.Collect();

            // Mehrdad: It's added in 3.5 SP1 !!!
            //GCNotificationStatus status = GC.WaitForFullGCComplete(2500);
        }

        #endregion

        #region Other Methods

        private static void CheckAndSetInstallationGuid()
        {
            if (string.IsNullOrEmpty(InstallationGuid))
            {
                var guid = Guid.NewGuid();
                InstallationGuid = guid.ToString("B").ToUpper();
            }
        }

        private void CloseThisDialog(object sender, EventArgs e)
        {
            if (m_currentDialog != null)
                m_currentDialog.Close();

            //object missing = Missing.Value;
            //Globals.ThisAddIn.Application.Quit(ref missing, ref missing, ref missing);
        }

        /// <summary>
        /// Disables the spell-check functionalities.
        /// </summary>
        private static void DisableSpellCheckFunctionalities()
        {
            try
            {
                WordCompletionForm.GlobalEnabled = false;
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("DisableSpellCheckFunctionalities", ex);
            }
        }

        /// <summary>
        /// Enables the spell-check functionalities.
        /// </summary>
        private static void EnableSpellCheckFunctionalities()
        {
            try
            {
                WordCompletionForm.GlobalEnabled = true;
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("EnableSpellCheckFunctionalities:", ex);
            }
        }

        /// <summary>
        /// Finds the dictionary location.
        /// </summary>
        /// <returns></returns>
        public string FindUserDictionaryLocation()
        {
            string userDicPath = "";
            if (File.Exists(m_appSettings.SpellChecker_UserDictionaryPath))
            {
                userDicPath = m_appSettings.SpellChecker_UserDictionaryPath;
            }
            else
            {
                userDicPath = SettingsHelper.GetFullPath(Constants.UserDicFileName, VirastyarFilePathTypes.UserFiles);

                if (!File.Exists(userDicPath))
                {
                    PersianMessageBox.Show("فایل واژه‌نامهٔ شخصی کاربر یافت نشد. لطفاً محل آن را تعیین کنید."
                        + Environment.NewLine + "در صورت انصراف، محل واژه‌نامهٔ شخصی کاربر را می‌توانید از طریق صفحه تنظیمات نیز تغییر دهید.",
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
                            LogHelper.DebugException("", ex);
                        }
                    }
                }
            }

            // Save Settings
            m_appSettings.SpellChecker_UserDictionaryPath = userDicPath;
            m_appSettings.Save();
            return userDicPath;
        }

        public string FindMainDictionaryLocation()
        {
            string dicPath = "";
            if (File.Exists(m_appSettings.SpellChecker_MainDictionaryPath))
            {
                dicPath = m_appSettings.SpellChecker_MainDictionaryPath;
            }
            else
            {
                dicPath = SettingsHelper.GetFullPath(Constants.MainDicFileName, VirastyarFilePathTypes.AllUsersFiles);
                Debug.Assert(File.Exists(dicPath));
                // Save Settings
                m_appSettings.SpellChecker_MainDictionaryPath = dicPath;
                m_appSettings.Save();
            }

            return dicPath;
        }

        public void ShowHelp(string helpTopicFileName)
        {
            if (helpTopicFileName.Length > 0)
            {
                try
                {
                    Help.ShowHelp(null, SettingsHelper.GetFullPath(
                        HelpConstants.HelpFileName, VirastyarFilePathTypes.InstallationFiles), 
                        HelpNavigator.Topic, helpTopicFileName);
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

        public void OnExceptionOccured(Exception ex, string message)
        {
            try
            {
                LogHelper.ErrorException("[ExceptionForm] " + message, ex);
                ExceptionForm.ShowExceptionForm(ex);
            }
            catch (Exception internalEx)
            {
                Debug.WriteLine(internalEx);
            }
        }

        public void OnExceptionOccured(Exception ex)
        {
            try
            {
                LogHelper.ErrorException("[ExceptionForm] Unhandled Exception occured in Virastyar", ex);
                ExceptionForm.ShowExceptionForm(ex);
            }
            catch (Exception internalEx)
            {
                Debug.WriteLine(internalEx);
            }
        }

        void UpdateNotificationWindowUpdateAvailable(object sender, EventArgs e)
        {
            SetUpdateAvailableToolbarVisibility(true);

            // If it's the first time for this version, show the dialog
            try
            {
                bool showNotificationWindow = false;
                if (string.IsNullOrEmpty(m_appSettings.Updater_LastShownVersion))
                {
                    showNotificationWindow = true;
                }
                else if (CheckVersionNumbers(new System.Version(m_appSettings.Updater_LastShownVersion))
                    .CompareTo(CheckVersionNumbers(new System.Version(UpdateChecker.LatestVersion))) < 0)
                {
                    showNotificationWindow = true;
                }

                if (showNotificationWindow)
                {
                    m_appSettings.Updater_LastShownVersion = UpdateChecker.LatestVersion;
                    m_appSettings.Save();

                    VirastyarUpdate_Action();
                }
            }
            catch(Exception ex)
            {
                m_logger.ErrorException("Unable to show the UpdateAvailable dialog", ex);
            }
        }

        private void SetUpdateAvailableToolbarVisibility(bool visible)
        {
            var officeVersion = SettingsHelper.GetOfficeVersion();
            if (officeVersion ==  OfficeVersions.Office2003)
            {
                CommandBar cmdBar = WordUIHelper.FindOldToolbar(Application, Constants.UpdateToolbarName);
                if (cmdBar != null)
                {
                    foreach (CommandBarButton cmdButton in cmdBar.Controls)
                    {
                        cmdButton.Enabled = visible;
                    }
                }
                try
                {
                    m_virastyarTemplate.Save();
                }
                catch (Exception ex)
                {
                    LogHelper.DebugException(m_logger, "Exception while saving the template", ex);
                }
            }
            else // Office 2007-2010
            {
                // Do nothing, the "getVisible" callback will do the job
            }
        }

        public static System.Version CheckVersionNumbers(System.Version version)
        {
            int major = version.Major > 0 ? version.Major : 1;
            int minor = version.Minor > 0 ? version.Minor : 2;
            int build = version.Build >= 0 ? version.Build : 0;
            int revision = version.Revision >= 0 ? version.Revision : 0;

            return new System.Version(major, minor, build, revision);
        }

        /// <summary>
        /// Method used to check for administrative privileges
        /// </summary>
        public static bool IsAdministrator()
        {
            // Check administrative role
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator));
        }

        #endregion

        #region AddinConfiguration

        private void AddinConfigurationDialogSpellCheckSettingsChanged(SpellCheckSettingsChangedEventArgs e)
        {
            try
            {
                SpellCheckerWrapper.Initialize(e);
                e.CancelLoadingUserDictionary = !SpellCheckerWrapper.IsInitialized;
                if (SpellCheckerWrapper.IsInitialized)
                    EnableSpellCheckFunctionalities();
                else
                    DisableSpellCheckFunctionalities();

                TryCompactMemory();
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex, "Error in loading dictionary");
            }
        }

        private void AddinConfigurationDialogRepairMenubarsClicked(object sender, EventArgs e)
        {
            // TODO: Mouse cursor
            WordUIHelper.DeleteOldToolbars(this.Application, Constants.ToolbarPrefix);
            WordUIHelper.DeleteOldToolbars(this.Application, Constants.OldToolbarPrefix);
            //TODO: CreateMenuAndToolbars(false);
        }

        private void AddinConfigurationDialogRefineAllSettingsChanged(RefineAllSettingsChangedEventArgs e)
        {
            m_allCharactersRefinerSettings = e.Settings;
        }

        #endregion

        #region IVirastyarAddin Members

        public void PinglishConvert_Action()
        {
            try
            {
                if (!this.SpellCheckerWrapper.Enabled)
                    return;

                VerificationController.CreateAndStartInteractive<PinglishVerifier>(Application);
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        public void PinglishConvertAll_Action()
        {
            try
            {
                if (!ConfirmBatchStart(false, "تبدیل یکبارهٔ پینگلیش"))
                    return;

                if (!this.SpellCheckerWrapper.Enabled)
                    return;

                VerificationController.CreateAndStartBatchMode<PinglishVerifier>(Application);
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        public void CheckDates_Action()
        {
            VerificationController.CreateAndStartInteractive<DateVerifier>(Application);
        }

        public void CheckNumbers_Action()
        {
            VerificationController.CreateAndStartInteractive<NumberVerifier>(Application);
        }

        public void CheckSpell_Action()
        {
            try
            {
                if (!SpellCheckerWrapper.Enabled)
                    return;

                PersianSpellChecker engine;
                SessionLogger sessionLogger;
                if (!GetSpellCheckerEngine(out engine) || !GetSpellCheckerUsageSession(out sessionLogger))
                    return;

                VerificationController.CreateAndStartInteractive<SpellCheckerVerifier>(Application, false, engine, sessionLogger);
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        public void PreCheckSpell_Action()
        {
            try
            {
                if (!ConfirmBatchStart(false, "پیش‌پردازش املایی متن"))
                    return;

                if (!SpellCheckerWrapper.Enabled)
                    return;

                PersianSpellChecker engine;
                SessionLogger sessionLogger;
                if (!GetSpellCheckerEngine(out engine) || !GetSpellCheckerUsageSession(out sessionLogger))
                    return;

                VerificationController.CreateAndStartBatchMode<SpellCheckerVerifier>(Application, true, engine);
            }
            catch (Exception ex)
            {
                OnExceptionOccured(ex);
            }
        }

        public void CheckPunctuation_Action()
        {
            VerificationController.CreateAndStartInteractive<PunctuationVerifier>(Application);
        }

        public void CheckAllPunctuation_Action()
        {
            if (!ConfirmBatchStart(false, "تصحیح یکبارهٔ نشانه‌گذاری"))
                return;

            VerificationController.CreateAndStartBatchMode<PunctuationVerifier>(Application);
        }

        public void RefineAllCharacters_Action()
        {
            if (!ConfirmBatchStart(true, "اصلاح نویسه‌های متن"))
                return;


            VerificationController.CreateAndStartBatchMode<CharacterRefinementVerifier>(Application, m_allCharactersRefinerSettings);
        }

        public void AddinSettings_Action()
        {
            m_addinConfigurationDialog.LoadConfigurations();
            m_addinConfigurationDialog.ShowDialog();
        }

        public void About_Action()
        {
            var winAbout = new AboutWindow();
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
                OnExceptionOccured(ex, "Unable to show Tip of the day");
            }
        }

        public void SendReport_Action()
        {
            // TODO:
        }

        public bool Get_IsUpdateAvailable()
        {
            return UpdateChecker.UpdateIsAvailable;
        }

        public void VirastyarUpdate_Action()
        {
            var updateNotificationWindow = new UpdateNotificationWindow(CloseThisDialog, false);
            m_currentDialog = updateNotificationWindow;
            var activeWindow = WindowWrapper.GetWordActiveWindowWrapper();
            if (activeWindow.Handle.ToInt64() != 0)
            {
                updateNotificationWindow.Show(WindowWrapper.GetWordActiveWindowWrapper());
            }
            else
            {
                updateNotificationWindow.ShowDialog();
            }
            m_currentDialog = null;
        }

        #endregion

        #region Obsolete Members

        [Obsolete("", true)]
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

        [Obsolete("", true)]
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

        [Obsolete("", true)]
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

        [Obsolete("", true)]
        private MSWordDocument ActiveMSWordDocument
        {
            get
            {
                return new MSWordDocument(Application.ActiveDocument);

            }
        }

        [Obsolete("", true)]
        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key, bool isCaseInsensitive)
        {
            return EnumerateMatchingParagraphs(key, null, isCaseInsensitive);
        }

        [Obsolete("", true)]
        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key)
        {
            return EnumerateMatchingParagraphs(key, null, false);
        }

        [Obsolete("", true)]
        private IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key1, string key2)
        {
            return EnumerateMatchingParagraphs(key1, key2, false);
        }

        [Obsolete("", true)]
        private static IEnumerable<MSWordBlock> EnumerateMatchingParagraphs(string key1, string key2, bool isCaseInsensitive)
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

        [Obsolete("", true)]
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

        [Obsolete("", true)]
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

        [Obsolete("", true)]
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

        [Obsolete("", true)]
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

        [Obsolete("", true)]
        public void ReplaceTwoRangesWithOne(Range first, Range second, string with)
        {
            ReplaceTwoRangesWithOne(first, second, with, true);
        }

        [Obsolete("", true)]
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

                        if (orgBeforeSpChar.Length > 0)
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
                if (showErrorMessage)
                    PersianMessageBox.Show("تغییر موردنظر قابل اعمال نیست.\r\nابرپیوندها یا ارجاعات درون متنی را بررسی کنید.");
                return false;
            }

            return true;
        }

        [Obsolete("", true)]
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

        [Obsolete("", true)]
        public bool IsOneSpaceInsertion(string original, string replacement, out string beforeSpace, out string afterSpace)
        {
            beforeSpace = afterSpace = "";
            int firstSpaceIndex = replacement.IndexOf(' ');
            if (firstSpaceIndex < 0)
                return false;
            string repCopy = replacement.Remove(firstSpaceIndex, 1);
            if (repCopy != original)
                return false;

            beforeSpace = replacement.Substring(0, firstSpaceIndex);
            if (firstSpaceIndex < replacement.Length - 1)
                afterSpace = replacement.Substring(firstSpaceIndex + 1);

            return true;
        }
        /// <summary>
        /// Only call it by CheckAddinPrerequisites
        /// </summary>
        [Obsolete("", true)]
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
                    catch (Exception ex)
                    {
                        LogHelper.DebugException("", ex);
                    }
                }
                virastyarUsersRegKey.Close();
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                // Ignore, limited user
                LogHelper.DebugException("Ignored Exception: ", ex);
                return false;
            }
            catch (SecurityException ex)
            {
                // Ignore, limited user
                LogHelper.DebugException("Ignored Exception: ", ex);
                return false;
            }
        }


        [Obsolete]
        public bool IsWordActive()
        {
            bool isActive = false;

            IntPtr activeWindow = User32.GetForegroundWindow();
            isActive = (Process.GetCurrentProcess().MainWindowHandle == activeWindow);

            //LogHelper.DebugException("Word Handle: " + Process.GetCurrentProcess().MainWindowHandle +
            //                "\nActiveWin Handle: " + activeWindow.ToString());
            //LogHelper.DebugException("I think Word is " + (isActive ? "active" : "not active"));

            return isActive;
        }

        #endregion

        private bool ConfirmBatchStart(bool isCharBased, string name)
        {
            string first = String.Format("این عملیات ممکن است بسیاری از {0}‌های سند شما را تغییر دهد.",
                                         isCharBased ? "نویسه" : "واژه");
            string second = "لطفاً قبل از آغاز عملیات از ذخیره بودن متن خود اطمینان حاصل کنید.";
            string third = String.Format("آیا موافقید که {0} آغاز شود؟", name);

            string msg = String.Format("{1}{0}{2}{0}{3}", Environment.NewLine,
                                       first, second, third);

            var dialogResult = PersianMessageBox.Show(GetWin32Window(), msg, name, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button2);

            return dialogResult == DialogResult.Yes;
        }

        public static IWin32Window GetWin32Window()
        {
            try
            {
                var handle = Process.GetCurrentProcess().MainWindowHandle;
                return new WindowWrapper(handle);
            }
            catch(Exception ex)
            {
                // Log Debug
                return null;
            }
        }
    }
}

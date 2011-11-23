using System;
using System.Collections.Generic;
using System.IO;
using VirastyarWordAddin.Configurations;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.Utility.SpellChecker;
using VirastyarWordAddin.Log;

namespace VirastyarWordAddin
{
    public class SpellCheckerWrapper
    {
        private SpellCheckerConfig m_spellCheckerSettings = null;
        private PersianSpellChecker m_spellCheckerEngine = null;
        private readonly IgnoreList m_ignList = new IgnoreList();
        private readonly SessionLogger m_sessionLogger = new SessionLogger();

        private string[] m_dictionaries = new string[0];

        private bool m_prespellCorrectPrefixes = false;
        private bool m_prespellCorrectSuffixes = false;
        private bool m_prespellCorrectBe = false;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the path to the user dictionary
        /// </summary>
        public string UserDictionary { get; private set; }

        public string MainDictionary
        {
            get
            {
                return m_dictionaries[0];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SpellCheckerWrapper"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public void Disable()
        {
            Enabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellCheckerWrapper"/> class.
        /// </summary>
        /// <param name="userDicPath">The user dic path.</param>
        /// <param name="editDistance">The edit distance.</param>
        /// <param name="maxSug">The maximum number of suggestions.</param>
        /// <param name="dics">array of main and custom dictionary paths. It is expected that the main dictionary has the index 0.</param>
        /// <param name="prespellCorrectPrefixes">if set to <c>true</c> enable correct-prefixes prespelling rule.</param>
        /// <param name="prespellCorrectSuffixes">if set to <c>true</c> enable correct-suffixes prespelling rule.</param>
        /// <param name="prespellCorrectBe">if set to <c>true</c> enable correct-suffixes prespelling rule.</param>
        /// <param name="stemPath">The path to the stem file.</param>
        public SpellCheckerWrapper(string userDicPath, int editDistance, int maxSug, string[] dics,
            bool prespellCorrectPrefixes, bool prespellCorrectSuffixes, bool prespellCorrectBe, string stemPath)
        {
            UserDictionary = userDicPath;
            m_spellCheckerSettings = new SpellCheckerConfig(userDicPath, editDistance, maxSug) {StemPath = stemPath};
            IsInitialized = false;
            Enabled = true;
            SetDictionaries(dics);

            m_prespellCorrectBe = prespellCorrectBe;
            m_prespellCorrectPrefixes = prespellCorrectPrefixes;
            m_prespellCorrectSuffixes = prespellCorrectSuffixes;
        }

        public bool SetDictionaries(string[] dics)
        {
            m_dictionaries = dics;
            return true;
        }

        private bool Initialize()
        {
            return InitializeCore(null);
        }

        public bool Initialize(SpellCheckSettingsChangedEventArgs e)
        {
            m_spellCheckerSettings = new SpellCheckerConfig(e.Settings.DicPath, e.Settings.EditDistance, e.Settings.SuggestionCount)
                                         {StemPath = e.Settings.StemPath};
            SetDictionaries(e.CustomDictionaries);

            m_prespellCorrectBe = e.PrespellCorrectBe;
            m_prespellCorrectPrefixes = e.PrespellCorrectPrefixes;
            m_prespellCorrectSuffixes = e.PrespellCorrectSuffixes;
            
            return InitializeCore(e);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public bool InitializeCore(SpellCheckSettingsChangedEventArgs e)
        {
            bool isFirstLoad = (e == null);
            try
            {
                if (m_spellCheckerEngine == null)
                {
                    isFirstLoad = true;
                    m_spellCheckerEngine = new PersianSpellChecker(m_spellCheckerSettings);
                }
                else if (e != null && e.ReloadSpellCheckerEngine)
                {
                    m_spellCheckerEngine.ClearDictionary();
                    m_spellCheckerEngine.Reconfigure(m_spellCheckerSettings);
                }

                #region Prespelling Rules

                if (m_prespellCorrectPrefixes)
                {
                    m_spellCheckerEngine.SetOnePassCorrectionRules(OnePassCorrectionRules.CorrectPrefix);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassCorrectionRules(OnePassCorrectionRules.CorrectPrefix);
                }


                if (m_prespellCorrectSuffixes)
                {
                    m_spellCheckerEngine.SetOnePassCorrectionRules(OnePassCorrectionRules.CorrectSuffix);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassCorrectionRules(OnePassCorrectionRules.CorrectSuffix);
                }

                if (m_prespellCorrectBe)
                {
                    m_spellCheckerEngine.SetOnePassCorrectionRules(OnePassCorrectionRules.CorrectBe);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassCorrectionRules(OnePassCorrectionRules.CorrectBe);
                }

                #endregion

            }
            catch (FileLoadException ex)
            {
                LogHelper.DebugException("Some DLLs could not be loaded.", ex);
                IsInitialized = false;
                m_spellCheckerEngine = null;
                if (e != null)
                    e.CancelLoadingUserDictionary = true;
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
                IsInitialized = false;
                m_spellCheckerEngine = null;
                if (e != null)
                    e.CancelLoadingUserDictionary = true;
                return false;
            }

            if (isFirstLoad || e.ReloadSpellCheckerEngine)
            {
                foreach (string t in m_dictionaries)
                {
                    try
                    {
                        if (!m_spellCheckerEngine.AppendDictionary(t))
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        if (e != null)
                            e.ErroneousUserDictionaries.Add(t);
                    }
                }
            }

            IsInitialized = true;
            Enabled = true;

            return IsInitialized;
        }

        public bool Initialize(string userDicPath)
        {
            return Initialize(userDicPath, m_spellCheckerSettings.EditDistance, m_spellCheckerSettings.SuggestionCount, m_dictionaries);
        }

        public bool Initialize(string userDicPath, int editDistance, int maxSug, string[] customDics)
        {
            m_spellCheckerSettings = new SpellCheckerConfig(userDicPath, editDistance, maxSug);
            SetDictionaries(customDics);
            return Initialize();
        }

        /// <summary>
        /// Gets the engine.
        /// </summary>
        /// <value>The engine.</value>
        public PersianSpellChecker Engine
        {
            get
            {
                if (!IsInitialized & Enabled)
                    Initialize();

                return m_spellCheckerEngine;
            }
        }

        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <value>The config.</value>
        public SpellCheckerConfig Config
        {
            get
            {
                return m_spellCheckerSettings;
            }
        }

        /// <summary>
        /// Gets the ignore list.
        /// </summary>
        /// <value>The ignore list.</value>
        public IgnoreList IgnoreList
        {
            get
            {
                return m_ignList;
            }
        }

        /// <summary>
        /// Gets the session logger
        /// </summary>
        /// <value>The session logger.</value>
        public SessionLogger SessionLogger
        {
            get
            {
                return m_sessionLogger;
            }
        }

        public static string[] GetDictionariesArray(string mainDictionary, bool useMainDictionary,
            string dicPaths, int dicSelectionFlags)
        {
            var lstDics = new List<string>();
            if (useMainDictionary)
                lstDics.Add(mainDictionary);

            string[] paths = dicPaths.Split(';');
            for (int i = 0; i < paths.Length; ++i)
            {
                string path = paths[i].Trim();
                if (path.Length <= 0) continue;
                if (((1 << i) & dicSelectionFlags) > 0)
                    lstDics.Add(path);
            }

            return lstDics.ToArray();
        }
    }
}

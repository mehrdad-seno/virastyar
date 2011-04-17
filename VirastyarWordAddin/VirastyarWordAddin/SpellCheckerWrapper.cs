using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool m_ruleVocabWordSpacingCorrection = false;
        private bool m_ruleDontCheckSingleLetters = false;
        private bool m_ruleHeYeConvertion = true;

        private bool m_refineHaa = false;
        private bool m_refineMee = false;
        private bool m_refineHeYe = false;
        private bool m_refineBe = false;
        private bool m_refineAllAffixes = false;

        private bool m_isInitialized = false;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get
            {
                return m_isInitialized;
            }
        }

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
        /// <param name="dicPath">The user dictionary path.</param>
        /// <param name="editDistance">The edit distance.</param>
        /// <param name="maxSug">The max sug.</param>
        /// <param name="dics"></param>
        /// <param name="ruleVocabWordSpacingCorrection"></param>
        /// <param name="ruleHeYeConvertion"></param>
        /// <param name="refineAllAffixes"></param>
        /// <param name="refineBe"></param>
        /// <param name="refineHaa"></param>
        /// <param name="refineHeYe"></param>
        /// <param name="refineMee"></param>
        public SpellCheckerWrapper(string dicPath, int editDistance, int maxSug, string[] dics,
            bool ruleVocabWordSpacingCorrection, bool ruleDontCheckSingleLetters, bool ruleHeYeConvertion,
            bool refineAllAffixes, bool refineBe, bool refineHaa, bool refineHeYe, bool refineMee, string stemPath)
        {
            this.UserDictionary = dicPath;
            this.m_spellCheckerSettings = new SpellCheckerConfig(dicPath, editDistance, maxSug);
            this.m_spellCheckerSettings.StemPath = stemPath;
            this.m_isInitialized = false;
            this.Enabled = true;
            SetDictionaries(dics);
            this.m_ruleVocabWordSpacingCorrection = ruleVocabWordSpacingCorrection;
            this.m_ruleDontCheckSingleLetters = ruleDontCheckSingleLetters;
            this.m_ruleHeYeConvertion = ruleHeYeConvertion;

            this.m_refineAllAffixes = refineAllAffixes;
            this.m_refineBe = refineBe;
            this.m_refineHaa = refineHaa;
            this.m_refineHeYe = refineHeYe;
            this.m_refineMee = refineMee;

            //
            //new SpellCheckerEngine(new SpellCheckerConfig(
        }

        public bool SetDictionaries(string[] dics)
        {
            // TODO: check each
            this.m_dictionaries = dics;
            return true;
        }

        private bool Initialize()
        {
            return InitializeCore(null);
        }

        public bool Initialize(SpellCheckSettingsChangedEventArgs e)
        {
            this.m_spellCheckerSettings = new SpellCheckerConfig(e.Settings.DicPath, e.Settings.EditDistance, e.Settings.SuggestionCount);
            m_spellCheckerSettings.StemPath = e.Settings.StemPath;

            this.SetDictionaries(e.CustomDictionaries);
            this.m_ruleVocabWordSpacingCorrection = e.RuleVocabWordSpacingCorrection;
            this.m_ruleDontCheckSingleLetters = e.RuleDontCheckSingleLetters;
            this.m_ruleHeYeConvertion = e.RuleHeYeConvertion;
            
            this.m_refineHaa = e.RefineHaa;
            this.m_refineMee = e.RefineMee;
            this.m_refineHeYe = e.RefineHeYe;
            this.m_refineBe = e.RefineBe;
            this.m_refineAllAffixes = e.RefineAllAffixes;

            return InitializeCore(e);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public bool InitializeCore(SpellCheckSettingsChangedEventArgs e)
        {
            try
            {
                if (m_spellCheckerEngine == null)
                {
                    m_spellCheckerEngine = new PersianSpellChecker(m_spellCheckerSettings);
                }
                else
                {
                    m_spellCheckerEngine.ClearDictionary();
                    m_spellCheckerEngine.Reconfigure(m_spellCheckerSettings);
                }

                #region Spelling Rules

                if (this.m_ruleVocabWordSpacingCorrection)
                {
                    m_spellCheckerEngine.SetSpellingRules(SpellingRules.VocabularyWordsSpaceCorrection);
                }
                else
                {
                    m_spellCheckerEngine.UnsetSpellingRules(SpellingRules.VocabularyWordsSpaceCorrection);
                }

                if (this.m_ruleDontCheckSingleLetters)
                {
                    m_spellCheckerEngine.SetSpellingRules(SpellingRules.IgnoreLetters);
                }
                else
                {
                    m_spellCheckerEngine.UnsetSpellingRules(SpellingRules.IgnoreLetters);
                }

                if (!this.m_ruleHeYeConvertion)
                {
                    m_spellCheckerEngine.SetSpellingRules(SpellingRules.IgnoreHehYa);
                }
                else
                {
                    m_spellCheckerEngine.UnsetSpellingRules(SpellingRules.IgnoreHehYa);
                }

                #endregion

                #region OnePassConverting Rules

                if (this.m_refineAllAffixes)
                {
                    m_spellCheckerEngine.SetOnePassConvertingRules(OnePassConvertingRules.ConvertAll);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassConvertingRules(OnePassConvertingRules.ConvertAll);
                }

                if (this.m_refineHaa)
                {
                    m_spellCheckerEngine.SetOnePassConvertingRules(OnePassConvertingRules.ConvertHaa);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassConvertingRules(OnePassConvertingRules.ConvertHaa);
                }


                if (this.m_refineHeYe)
                {
                    m_spellCheckerEngine.SetOnePassConvertingRules(OnePassConvertingRules.ConvertHehYa);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassConvertingRules(OnePassConvertingRules.ConvertHehYa);

                }

                if (this.m_refineMee)
                {
                    m_spellCheckerEngine.SetOnePassConvertingRules(OnePassConvertingRules.ConvertMee);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassConvertingRules(OnePassConvertingRules.ConvertMee);
                }

                if (this.m_refineBe)
                {
                    m_spellCheckerEngine.SetOnePassConvertingRules(OnePassConvertingRules.ConvertBe);
                }
                else
                {
                    m_spellCheckerEngine.UnsetOnePassConvertingRules(OnePassConvertingRules.ConvertBe);
                }

                #endregion

            }
            catch (Exception ex)
            {
                LogHelper.DebugException("", ex);
                this.m_isInitialized = false;
                this.m_spellCheckerEngine = null;
                if (e != null)
                    e.CancelLoadingUserDictionary = true;
                return false;
            }

            for (int i = 0; i < m_dictionaries.Length; ++i)
            {
                try
                {
                    if (!m_spellCheckerEngine.AppendDictionary(m_dictionaries[i]))
                        throw new Exception();
                }
                catch (Exception)
                {
                    if (e != null)
                        e.ErroneousUserDictionaries.Add(m_dictionaries[i]);
                }
            }

            this.m_isInitialized = true;
            this.Enabled = true;

            return IsInitialized;
        }

        public bool Initialize(string dicPath)
        {
            return Initialize(dicPath, m_spellCheckerSettings.EditDistance, m_spellCheckerSettings.SuggestionCount, m_dictionaries);
        }

        public bool Initialize(string dicPath, int editDistance, int maxSug, string[] customDics)
        {
            this.m_spellCheckerSettings = new SpellCheckerConfig(dicPath, editDistance, maxSug);
            this.SetDictionaries(customDics);
            return this.Initialize();
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
            List<string> lstDics = new List<string>();
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

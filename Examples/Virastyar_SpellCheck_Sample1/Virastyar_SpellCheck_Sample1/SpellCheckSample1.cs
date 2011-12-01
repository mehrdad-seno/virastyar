using System;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SCICT.VirastyarInlineVerifiers;
using SCICT.NLP.TextProofing.SpellChecker;
using System.IO;

namespace VirastyarSpellCheckSample1
{
    public partial class SpellCheckSample1 : Form
    {
        #region Private Fields

        private SpellCheckerInlineVerifier m_verifier;
        private PersianSpellChecker m_spellEngine;

        #endregion

        #region Ctors

        public SpellCheckSample1()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        #region Event Handlers

        private void btnSpellCheck_Click(object sender, EventArgs e)
        {
            SpellCheck(rtbInput.Text);
        }

        #endregion

        private void Initialize()
        {
            var config = new PersianSpellCheckerConfig()
            {
                DicPath = GetFullPath("Dic.dat"),
                StemPath = GetFullPath("Stem.dat"),
                EditDistance = 2,
                SuggestionCount = 7,
            };
            m_spellEngine = new PersianSpellChecker(config);
            m_verifier = new SpellCheckerInlineVerifier(false, m_spellEngine);
        }

        private string GetFullPath(string dataFileName)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(basePath, dataFileName);
        }

        private void SpellCheck(string text)
        {
            lstResult.Items.Clear();

            // Enumerate all instances of misspelled words
            foreach (var errorInstance in Verifier.VerifyText(text))
            {
                if (!errorInstance.IsValid)
                    continue;

                lstResult.Items.Add(new ErrorItem(errorInstance, text));
            }
        }

        #endregion

        #region Private Properties

        private SpellCheckerInlineVerifier Verifier
        {
            get
            {
                if (m_verifier == null)
                    Initialize();

                return m_verifier;
            }
        }

        #endregion

        private void lstResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstResult.SelectedItem != null)
            {
                var errorItem = (ErrorItem)lstResult.SelectedItem;
                rtbInput.Select(errorItem.Error.Index, errorItem.Error.Length);
            }
        }
    }

    public class ErrorItem
    {
        private readonly string m_misspelledWord;

        public VerificationInstance Error { get; private set; }
        
        public ErrorItem(VerificationInstance error, string text)
        {
            Error = error;
            m_misspelledWord = text.Substring(Error.Index, Error.Length);
        }

        public override string ToString()
        {

            var suggestions = new StringBuilder();
            for (int i = 0; i < Error.Suggestions.Length; i++)
            {
                string suggestion = Error.Suggestions[i];
                if (i != Error.Suggestions.Length - 1)
                    suggestions.Append(suggestion + "،");
                else
                    suggestions.Append(suggestion);
            }

            return string.Format("{0} - پیشنهادات: {1}", m_misspelledWord, suggestions);
        }
    }
}

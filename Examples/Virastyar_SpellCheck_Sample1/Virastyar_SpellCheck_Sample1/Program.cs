using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SCICT.NLP.TextProofing.SpellChecker;
using SCICT.VirastyarInlineVerifiers;

namespace VirastyarSpellCheckSample1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SpellCheckSample1());
        }
    }
}

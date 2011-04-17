//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//


using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.NLP.TextProofing.SpellChecker;

namespace SCICT.NLP.Utility.PinglishConverter
{
    using Persian.Constants;
    using System.Diagnostics;

    /// <summary>
    /// Base class for Pinglish conversion. Provides methods to generate or convert possible words for a 
    /// given Pinglish string.
    /// </summary>
    public class PinglishConverter : IPinglishLearner
    {
        #region Private Fields

        private readonly Dictionary<string, PreprocessElementInfo> m_preprocessReplacements = new Dictionary<string, PreprocessElementInfo>();
        private readonly Dictionary<char, string[]> m_bigLettersAtBeginOrEnd = new Dictionary<char, string[]>();

        private readonly Dictionary<char, string[]> m_oneLetterWords = new Dictionary<char, string[]>();

        private PinglishMapping m_converter = new PinglishMapping();
        private SpellCheckerEngine m_speller;
        
        private bool m_isSpellerEngineSet;

        private static readonly IEqualityComparer<PinglishString> s_pinglishStringComparer = new PinglishStringEqualityComparer();

        #endregion

        #region Public Fields

        ///<summary>
        /// 
        ///</summary>
        public static readonly CharacterMapping[] MultipleValueCharMap;

        /// <summary>
        /// Gets a value indicating whether the speller engine is set for this instance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the speller engine is set for this instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsSpellerEngineSet
        {
            get { return m_isSpellerEngineSet; }
        }

        /// <summary>
        /// Gets the dataset.
        /// </summary>
        /// <value>The dataset.</value>
        public IEnumerable<PinglishString> Dataset
        {
            get
            {
                return m_converter.DataSet;
            }
        }

        #endregion

        #region Constructors

        static PinglishConverter()
        {
            #region Creating Attributes

            var attr_b = new CharacterMapping('b', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("ب", 0),
                                                                new CharacterMappingInfo("ب", 'b', TokenPosition.MiddleOfWord, 0),
                                                                new CharacterMappingInfo("بع", 'b', TokenPosition.EndOfWord, 0),
                                                            });

            var attr_d = new CharacterMapping('d', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("د", 0),
                                                                new CharacterMappingInfo("د", 'd', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_r = new CharacterMapping('r', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("ر", 0),
                                                                new CharacterMappingInfo("ر", 'r', TokenPosition.MiddleOfWord, 0),
                                                                new CharacterMappingInfo("", 2),
                                                            });

            var attr_f = new CharacterMapping('f', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("ف", 0),
                                                                new CharacterMappingInfo("ف", 'f', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_l = new CharacterMapping('l', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("ل", 0),
                                                                new CharacterMappingInfo("ل", 'l', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_m = new CharacterMapping('m', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("م", 0),
                                                                new CharacterMappingInfo("م", 'm', TokenPosition.MiddleOfWord, 0),
                                                                new CharacterMappingInfo("مع", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1),
                                                            });

            var attr_n = new CharacterMapping('n', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("ن", 0),
                                                                new CharacterMappingInfo("", TokenPosition.EndOfWord, 2), 
                                                                new CharacterMappingInfo("ً", TokenPosition.EndOfWord, 3),
                                                                new CharacterMappingInfo("ن", 'n', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_v = new CharacterMapping('v', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("و", 0),
                                                                new CharacterMappingInfo("و", 'v', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_w = new CharacterMapping('w', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("و", 0),
                                                                new CharacterMappingInfo("و", 'w', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_y = new CharacterMapping('y', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("ی", 0),
                                                                new CharacterMappingInfo("ی", 'y', TokenPosition.MiddleOfWord, 0),
                                                            });

            var attr_t = new CharacterMapping('t', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ت", 0),
                                                                new CharacterMappingInfo("ط", 1),
                                                                new CharacterMappingInfo("طع", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 2),
                                                                new CharacterMappingInfo("تع", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 2),

                                                                new CharacterMappingInfo("ت", 't'),
                                                                new CharacterMappingInfo("ط", 't'),

                                                                new CharacterMappingInfo("", TokenPosition.MiddleOfWord, 3),
                                                            });

            //CharacterMapping attr_T = new CharacterMapping('T', true, new CharacterMappingInfo[] 
            //        { 
            //            new CharacterMappingInfo("ت", 1),
            //            new CharacterMappingInfo("ط", 2),
            //            new CharacterMappingInfo("تی", LetterPosition.EndOfWord, 0),

            //            new CharacterMappingInfo("ت", 'T'),
            //            new CharacterMappingInfo("ط", 'T'),
            //        });

            var attr_s = new CharacterMapping('s', new CharacterMappingInfo[]
                                                            { 
                                                                new CharacterMappingInfo("س", 0),
                                                                new CharacterMappingInfo("ص", 1),
                                                                new CharacterMappingInfo("ث", 2),
                                                                new CharacterMappingInfo("ش", 'h'),

                                                                new CharacterMappingInfo("س", 's'),
                                                                new CharacterMappingInfo("ص", 's'),
                                                                new CharacterMappingInfo("ث", 's'),

                                                                new CharacterMappingInfo("", 'h', TokenPosition.MiddleOfWord, 3),
                                                                new CharacterMappingInfo("", TokenPosition.MiddleOfWord, 3),
                                                            });

            var attr_c = new CharacterMapping('c', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("س", 0),
                                                                new CharacterMappingInfo("ص", 2),
                                                                new CharacterMappingInfo("ث", 3),
                                                                new CharacterMappingInfo("ک", 4),
                                                                new CharacterMappingInfo("چ", 'h'),
                                                                new CharacterMappingInfo("سی", TokenPosition.EndOfWord, 1),

                                                                new CharacterMappingInfo("س", 'c'),
                                                                new CharacterMappingInfo("ص", 'c'),
                                                                new CharacterMappingInfo("ک", 'c'),
                                                                new CharacterMappingInfo("ث", 'c'),
                                                            });

            //CharacterMapping attr_C = new CharacterMapping('C', true, new CharacterMappingInfo[] 
            //        { 
            //            new CharacterMappingInfo("س", 1),
            //            new CharacterMappingInfo("ص", 2),
            //            new CharacterMappingInfo("ث", 3),
            //            new CharacterMappingInfo("ک", 4),
            //            new CharacterMappingInfo("سی", LetterPosition.EndOfWord, 0),

            //            new CharacterMappingInfo("س", 'C'),
            //            new CharacterMappingInfo("ص", 'C'),
            //            new CharacterMappingInfo("ث", 'C'),
            //        });

            var attr_h = new CharacterMapping('h', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ه", 0),
                                                                new CharacterMappingInfo("ح", 1),
                                                                new CharacterMappingInfo(PseudoSpace.ZWNJ.ToString(), TokenPosition.MiddleOfWord, 2),

                                                                new CharacterMappingInfo("ه", 'h'),
                                                                new CharacterMappingInfo("ح", 'h'),
                                                            });

            var attr_p = new CharacterMapping('p', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("پ", 0),
                                                                new CharacterMappingInfo("ف", 'h'),

                                                                new CharacterMappingInfo("پ", 'p'),
                                                            });


            var attr_j = new CharacterMapping('j', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ج", 0),
                                                                new CharacterMappingInfo("ژ", 1),

                                                                new CharacterMappingInfo("ج", 'j'),
                                                                new CharacterMappingInfo("ژ", 'j')
                                                            });

            var attr_g = new CharacterMapping('g', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("گ", 0),
                                                                new CharacterMappingInfo("ج", 1),
                                                                new CharacterMappingInfo("ق", 'h', TokenPosition.Any, 0),
                                                                new CharacterMappingInfo("غ", 'h', TokenPosition.Any, 0),

                                                                new CharacterMappingInfo("گ", 'g'),
                                                                new CharacterMappingInfo("ج", 'g'),
                                                            });

            var attr_a = new CharacterMapping('a', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("\u064E", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1, "فتحه"),
                                                                new CharacterMappingInfo("آ", TokenPosition.StartOfWord, 1)
                                                                ,
                                                                new CharacterMappingInfo("ا", 0),
                                                                new CharacterMappingInfo("‌ا", 'l'),
                                                                new CharacterMappingInfo("", 1),
                                                                new CharacterMappingInfo("أ", TokenPosition.StartOfWord | TokenPosition.MiddleOfWord, 2),
                                                                new CharacterMappingInfo("ع", TokenPosition.StartOfWord, 2)
                                                                ,
                                                                //new CharacterMappingInfo("ع", '\'',TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 0),
                                                                new CharacterMappingInfo("ع",
                                                                                      TokenPosition.MiddleOfWord |
                                                                                      TokenPosition.EndOfWord, 0),
                                                                new CharacterMappingInfo("ا", 'a',
                                                                                      TokenPosition.MiddleOfWord, 0),
                                                                new CharacterMappingInfo("آ", 'a',
                                                                                      TokenPosition.StartOfWord, 0),
                                                                new CharacterMappingInfo("اع", TokenPosition.StartOfWord, 2),
                                                                //new CharacterMappingInfo("\u0647" + PseudoSpace.ZWNJ + "\u0633",'s', TokenPosition.MiddleOfWord, 0),
                                                                new CharacterMappingInfo("عا", 5),
                                                                new CharacterMappingInfo("اع", 5),
                                                                //new CharacterMappingInfo("عاً", 'n',
                                                                //                      TokenPosition.MiddleOfWord, 2),
                                                                new CharacterMappingInfo("ه", TokenPosition.EndOfWord, 2),
                                                                new CharacterMappingInfo("ی", TokenPosition.EndOfWord, 2),
                                                                new CharacterMappingInfo("وا", TokenPosition.MiddleOfWord, 2),
                                                                new CharacterMappingInfo(PseudoSpace.ZWNJ + "ال", 'l', TokenPosition.MiddleOfWord, 3),
                                                                new CharacterMappingInfo("ئ", TokenPosition.MiddleOfWord, 3),
                                                                //new CharacterMappingInfo("هم", 'm', TokenPosition.MiddleOfWord, 4),
                                                                //new CharacterMappingInfo("ه‌ت", 't', TokenPosition.MiddleOfWord, 4),
                                                                //new CharacterMappingInfo("ه‌ر", 'r', TokenPosition.MiddleOfWord, 4),
                                                                // TODO:
                                                                //new CharacterMappingInfo("ه‌ش", 's', TokenPosition.MiddleOfWord, 4),
                                                                new CharacterMappingInfo("ه‌", TokenPosition.MiddleOfWord, 4)

                                                            });

            var attr_e = new CharacterMapping('e', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("\u0650", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 0, "کسره"),
                                                                new CharacterMappingInfo("ا", TokenPosition.Any, 2),
                                                                new CharacterMappingInfo("ه", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1),
                                                                new CharacterMappingInfo("\u0647" + PseudoSpace.ZWNJ, TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1),
                                                                new CharacterMappingInfo("ی", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1),
                                                                new CharacterMappingInfo("یی", TokenPosition.EndOfWord, 2), 
                                                                new CharacterMappingInfo("ع", 2),
                                                                new CharacterMappingInfo("ئ", 2),
                                                                new CharacterMappingInfo("ه", 'h'),
                                                                new CharacterMappingInfo("اع", TokenPosition.StartOfWord, 0),
                                                                new CharacterMappingInfo("ی", 'i'),
                                                                new CharacterMappingInfo("ی", 'e'),

                                                                new CharacterMappingInfo("ه‌ی", 'i', TokenPosition.MiddleOfWord, 5),
                                                                new CharacterMappingInfo("ه‌ی", 'y', TokenPosition.MiddleOfWord, 5),
                                                                new CharacterMappingInfo("ه‌ه", 'h', TokenPosition.MiddleOfWord, 5),
                                                                new CharacterMappingInfo("ه‌", TokenPosition.MiddleOfWord, 5),

                                                                new CharacterMappingInfo("عه‌", 'i', TokenPosition.MiddleOfWord, 5),
                                                                //new CharacterMappingInfo("عه‌", 'i', TokenPosition.MiddleOfWord, 5),
                                                                new CharacterMappingInfo("عه‌", 'y', TokenPosition.MiddleOfWord, 5),
                                                                new CharacterMappingInfo("عه‌", 'h', TokenPosition.MiddleOfWord, 5),

                                                                new CharacterMappingInfo("ه‌", 'i', TokenPosition.MiddleOfWord, 5),
                                                                new CharacterMappingInfo("ه‌", 'e', TokenPosition.MiddleOfWord, 5),

                                                                new CharacterMappingInfo(PseudoSpace.ZWNJ + "ال", 'l', TokenPosition.MiddleOfWord, 3),

                                                                new CharacterMappingInfo("", TokenPosition.MiddleOfWord, 5),
                                                            });

            var attr_i = new CharacterMapping('i', new CharacterMappingInfo[] 
                                                            {
                                                                new CharacterMappingInfo("ی", 0),
                                                                new CharacterMappingInfo("ای", 'i'),
                                                                new CharacterMappingInfo("ی", 'e'),
                                                                new CharacterMappingInfo("ی", 'y'),
                                                                new CharacterMappingInfo("ای", TokenPosition.StartOfWord, 1),
                                                                new CharacterMappingInfo("یی", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1),
                                                                new CharacterMappingInfo("ی", 'i'),
                                                                new CharacterMappingInfo("", TokenPosition.MiddleOfWord, 2),
                                                                new CharacterMappingInfo("عی",0),
                                                           });

            var attr_k = new CharacterMapping('k', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ک", 0),
                                                                new CharacterMappingInfo("خ", 'h'),

                                                                new CharacterMappingInfo("ک", 'k'),
                                                                new CharacterMappingInfo("", 'k'),
                                                            });


            var attr_o = new CharacterMapping('o', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ا", TokenPosition.StartOfWord|TokenPosition.MiddleOfWord, 2),
                                                                new CharacterMappingInfo("او", TokenPosition.StartOfWord, 3),
                                                                new CharacterMappingInfo("و", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 0),
                                                                new CharacterMappingInfo("\u064f", TokenPosition.MiddleOfWord | TokenPosition.EndOfWord, 1, "ضمه"),
                                                                new CharacterMappingInfo("ع", 2),
                                                                new CharacterMappingInfo("ئو",'o'),
                                                                new CharacterMappingInfo("وع", TokenPosition.EndOfWord, 2),

                                                                new CharacterMappingInfo("و", 'o'),
                                                                new CharacterMappingInfo("و", 'u'),
                                                                new CharacterMappingInfo("‌ا", 'l'),
                                                                new CharacterMappingInfo(PseudoSpace.ZWNJ + "ال", 'l', TokenPosition.MiddleOfWord, 3),
                                                            });

            var attr_u = new CharacterMapping('u', new CharacterMappingInfo[]
                                                            { 
                                                                new CharacterMappingInfo("و", 0),
                                                                new CharacterMappingInfo("او", TokenPosition.StartOfWord, 1),
                                                                new CharacterMappingInfo("و", 'u'),
                                                                new CharacterMappingInfo("", 2),
                                                            });

            var attr_z = new CharacterMapping('z', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ز", 0),
                                                                new CharacterMappingInfo("ذ", 1),
                                                                new CharacterMappingInfo("ض", 1),
                                                                new CharacterMappingInfo("ظ", 1),
                                                                new CharacterMappingInfo("ژ", 'h'),
                        
                                                                new CharacterMappingInfo("ز", 'z'),
                                                                new CharacterMappingInfo("ذ", 'z'),
                                                                new CharacterMappingInfo("ض", 'z'),
                                                                new CharacterMappingInfo("ظ", 'z'),

                                                            });

            var attr_quotation = new CharacterMapping('\'', new CharacterMappingInfo[] 
                                                                     { 
                                                                         new CharacterMappingInfo("ع", 0),
                                                                         new CharacterMappingInfo(PseudoSpace.ZWNJ.ToString(), TokenPosition.MiddleOfWord, 1),
                                                                         new CharacterMappingInfo("", 2),
                                                                         new CharacterMappingInfo("آ", 'a', TokenPosition.MiddleOfWord, 3),
                                                                         new CharacterMappingInfo("عه", TokenPosition.EndOfWord, 1),
                                                                     });

            var attr_x = new CharacterMapping('x', new CharacterMappingInfo[]
                                                            {
                                                                new CharacterMappingInfo("خ", 0),
                                                                new CharacterMappingInfo("کس", 1),

                                                                new CharacterMappingInfo("خ", 'x'),
                                                                new CharacterMappingInfo("کس", 'x'),
                                                            });


            var attr_q = new CharacterMapping('q', new CharacterMappingInfo[] 
                                                            { 
                                                                new CharacterMappingInfo("ق", 0),
                                                                new CharacterMappingInfo("غ", 1),

                                                                new CharacterMappingInfo("ق", 'q'),
                                                                new CharacterMappingInfo("غ", 'q'),
                                                            });

            #endregion

            MultipleValueCharMap = new CharacterMapping[]
                                       {
                                           attr_b, attr_d, attr_r, attr_f, attr_l, attr_m, attr_n, attr_v, attr_w, attr_y,
                                           attr_a, attr_c, /*attr_C, */attr_e, attr_g, attr_h, attr_i, attr_j, 
                                           attr_k, attr_o, attr_p, attr_s, attr_t, /*attr_T, */attr_u, attr_x, attr_q,
                                           /*attr_y, */
                                           attr_z, attr_quotation, 
                                       };
        }

        /// <summary>
        /// Initializes the <see cref="PinglishConverter"/> class.
        /// </summary>
        public PinglishConverter()
        {
            #region Preprocess Replacements

            //TODO: It's better to load them from a file --> In progress -> DONE

            PreprocessElementInfo entryInfo = null;
            ///////////////////////////////////////////////////////////////
            entryInfo = new PreprocessElementInfo("U", true);
            entryInfo.Equivalents.Add("too");
            this.m_preprocessReplacements.Add(entryInfo.PinglishString, entryInfo);
            ///////////////////////////////////////////////////////////////
            entryInfo = new PreprocessElementInfo("I", true);
            entryInfo.Equivalents.Add("man");
            this.m_preprocessReplacements.Add(entryInfo.PinglishString, entryInfo);
            /////////////////////////////////////////////////////////////////
            entryInfo = new PreprocessElementInfo(QuotationMark.RightSingleQuotationMark.ToString(), false);
            entryInfo.Equivalents.Add(QuotationMark.SingleQuotationMark.ToString());
            this.m_preprocessReplacements.Add(entryInfo.PinglishString, entryInfo);
            /////////////////////////////////////////////////////////////////
            entryInfo = new PreprocessElementInfo(QuotationMark.Prime.ToString(), false);
            entryInfo.Equivalents.Add(QuotationMark.SingleQuotationMark.ToString());
            this.m_preprocessReplacements.Add(entryInfo.PinglishString, entryInfo);
            /////////////////////////////////////////////////////////////////
            entryInfo = new PreprocessElementInfo(QuotationMark.SingleHighReveresed9QuotationMark.ToString(), false);
            entryInfo.Equivalents.Add(QuotationMark.SingleQuotationMark.ToString());
            this.m_preprocessReplacements.Add(entryInfo.PinglishString, entryInfo);
            /////////////////////////////////////////////////////////////////
            entryInfo = new PreprocessElementInfo("k", true);
            entryInfo.Equivalents.Add("kei");
            this.m_preprocessReplacements.Add(entryInfo.PinglishString, entryInfo);
            /////////////////////////////////////////////////////////////////

            #region m_bigLettersAtBeginOrEnd

            this.m_bigLettersAtBeginOrEnd.Add('B', new[] { "b", "bi" });
            this.m_bigLettersAtBeginOrEnd.Add('C', new[] { "c", "si" });
            this.m_bigLettersAtBeginOrEnd.Add('D', new[] { "d", "di" });
            this.m_bigLettersAtBeginOrEnd.Add('E', new[] { "e", "ee" });
            this.m_bigLettersAtBeginOrEnd.Add('F', new[] { "f", "ef" });
            this.m_bigLettersAtBeginOrEnd.Add('G', new[] { "g", "ji" });
            this.m_bigLettersAtBeginOrEnd.Add('K', new [] { "k", "kei" });
            this.m_bigLettersAtBeginOrEnd.Add('M', new [] { "m","em" });
            this.m_bigLettersAtBeginOrEnd.Add('N', new [] { "n","en" });
            this.m_bigLettersAtBeginOrEnd.Add('P', new [] { "p","pi" });
            this.m_bigLettersAtBeginOrEnd.Add('Q', new [] { "q", "kioo" });
            this.m_bigLettersAtBeginOrEnd.Add('S', new [] { "s", "es" });
            this.m_bigLettersAtBeginOrEnd.Add('T', new [] { "t", "ti" });
            this.m_bigLettersAtBeginOrEnd.Add('U', new [] { "u", "yoo" });
            this.m_bigLettersAtBeginOrEnd.Add('V', new [] {  "v", "vi", });
            this.m_bigLettersAtBeginOrEnd.Add('Y', new [] { "y", "vay" });
            this.m_bigLettersAtBeginOrEnd.Add('Z', new [] { "z", "zed" });

            #endregion

            #region m_oneLetterWords

            m_oneLetterWords.Add('4', new string[] { "baraye" });
            m_oneLetterWords.Add('K', new string[] { "kei" });
            m_oneLetterWords.Add('U', new string[] { "to" });
            /*m_oneLetterWords.Add('', new string[] { "" });
            m_oneLetterWords.Add('', new string[] { "" });
            m_oneLetterWords.Add('', new string[] { "" });
            m_oneLetterWords.Add('', new string[] { "" });
            m_oneLetterWords.Add('', new string[] { "" });
            m_oneLetterWords.Add('', new string[] { "" });
            m_oneLetterWords.Add('', new string[] { "" });*/

            foreach (char key in m_oneLetterWords.Keys)
            {
                Debug.Assert(StringUtil.OneLetterPinglishWords.Contains(char.ToLower(key)));
            }

            #endregion

            #endregion
        }

        #endregion

        #region Public Methods

        #region Learn

        /// <summary>
        /// Extracts and learns all the mapping information from the given <see cref="PinglishString"/>
        /// </summary>
        /// <param name="pinglishString">The instance of <see cref="PinglishString"/> 
        /// which mapping information will be extracted from. </param>
        void IPinglishLearner.Learn(PinglishString pinglishString)
        {
            m_converter.Learn(pinglishString, true);
        }

        /// <summary>
        /// Learns from the specified list of <see cref="PinglishString"/>
        /// </summary>
        /// <param name="listOfWords">The list of words.</param>
        /// <seealso cref="Learn(PinglishString)"/>
        void IPinglishLearner.Learn(List<PinglishString> listOfWords)
        {
            m_converter.Learn(listOfWords, true);
        }

        #endregion

        #region Load/Save

        /// <summary>
        /// Loads the converter engine from the given file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadConverter(string fileName)
        {
            m_converter = PinglishMapping.LoadConverterEngine(fileName);
        }

        /// <summary>
        /// Saves the converter engine to the given file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveConverter(string fileName)
        {
            PinglishMapping.SaveConverterEngine(fileName, m_converter);
        }

        /// <summary>
        /// Loads the preprocess elements from the file
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public int LoadPreprocessElements(string filePath)
        {
            var list = PinglishConverterUtils.LoadPreprocessElements(filePath);
            foreach (var item in list)
            {
                this.m_preprocessReplacements[item.PinglishString] = item;
            }
            return list.Count;
        }

        #endregion

        /// <summary>
        /// Sets the speller engine.
        /// </summary>
        /// <param name="spellerEngine">The speller engine.</param>
        public void SetSpellerEngine(SpellCheckerEngine spellerEngine)
        {
            m_speller = spellerEngine;
            m_isSpellerEngineSet = true;
        }

        /// <summary>
        /// Returns the equivalent Farsi words, based on the previously learned data.
        /// </summary>
        /// <param name="pinglishWord">The Pinglish word.</param>
        /// <returns></returns>
        public PinglishString SuggestFarsiWordWithMapping(string pinglishWord)
        {
            var results = SuggestFarsiWordsWithMapping(pinglishWord);

            if (results.Length != 0)
            {
                return results[0];
            }

            return new PinglishString(pinglishWord);
        }

        /// <summary>
        /// Suggests Farsi words for the given Pinglish word, based on the learned dataset.
        /// </summary>
        /// <param name="pinglishWord">The Pinglish word.</param>
        /// <returns></returns>
        public PinglishString[] SuggestFarsiWordsWithMapping(string pinglishWord)
        {
            var revisedPinglishWords = PreprocessWord(pinglishWord);
            var results = new List<PinglishString>();

            var listOfResults = new List<List<PinglishString>>();

            foreach (var word in revisedPinglishWords)
            {
                listOfResults.Add(m_converter.SuggestWords(word, false));
            }

            int maxCount = listOfResults.Max(list => list.Count);
            for (int i = 0; i < maxCount; i++)
            {
                results.AddRange(from list in listOfResults where i < list.Count select list[i]);
            }

            return results.Distinct(s_pinglishStringComparer).ToArray();
        }

        /// <summary>
        /// Suggests Farsi word for the given Pinglish word, based on the learned dataset.
        /// </summary>
        public string SuggestFarsiWord(string pinglishWord, bool sortWithSpeller)
        {
            var results = SuggestFarsiWords(pinglishWord, sortWithSpeller);

            if (results.Length != 0)
            {
                return results[0];
            }

            return pinglishWord;
        }

        /// <summary>
        /// Suggests Farsi words for the given Pinglish word, based on the learned dataset.
        /// </summary>
        public string[] SuggestFarsiWords(string pinglishWord, bool sortWithSpeller)
        {
            // 1. Search for exact words
            var exactResults = SearchForExactWords(pinglishWord);

            if (exactResults.Length != 0)
                return exactResults;

            // 2. Suggest from mappings
            var results = SuggestFarsiWordsWithMapping(pinglishWord);

            var pinglishResults = results.ToStringArray(true, true);

            if (sortWithSpeller)
            {
                if (m_speller == null)
                    throw new InvalidOperationException("Speller engine has not been set. User SetSpellerEngine to set it.");

                var spellerResults = m_speller.SortSuggestions(pinglishResults, pinglishResults.Length);

                return (spellerResults.Length != 0) ? spellerResults : pinglishResults;
            }
            else
            {
                return pinglishResults;
            }
        }

        /// <summary>
        /// Generates all possible words. 
        /// Note that this method may return hundreds of words if the given Pinglish word contains many letters that have
        /// more that one mapping.
        /// A preprocess phase is applied to normalize characters in the word.
        /// </summary>
        /// <param name="pinglishWord">The Pinglish word.</param>
        /// <returns></returns>
        public PinglishString[] GenerateAllPossibleWords(string pinglishWord)
        {
            var revisedPinglishWords = PreprocessWord(pinglishWord);
            
            var results = new List<PinglishString>();

            foreach (var revisedPinglish in revisedPinglishWords)
            {
                results.AddRange(PrivateGenerateAllPossibleWords(revisedPinglish));
            }

            return results.Distinct(s_pinglishStringComparer).ToArray();
        }

        #endregion

        #region Private Methods

        private static IEnumerable<PinglishString> PrivateGenerateAllPossibleWords(string revisedPinglish)
        {
            var wordsList = new List<PinglishString> { new PinglishString() };

            #region Loop variables
            char ch;
            char nextChar;
            TokenPosition position;
            var possibleValues = new List<CharacterMappingInfo>();
            var possible2Values = new List<CharacterMappingInfo>();
            #endregion

            #region iterate through all characters
            for (int chIndex = 0; chIndex < revisedPinglish.Length; chIndex++)
            {
                #region Prepare Loop Variables
                ch = revisedPinglish[chIndex];//char.ToLower(revisedPinglish[chIndex]);

                if (chIndex < revisedPinglish.Length - 1)
                {
                    nextChar = revisedPinglish[chIndex + 1];

                    if (chIndex == 0)
                        position = TokenPosition.StartOfWord;
                    else
                        position = TokenPosition.MiddleOfWord;
                }
                else
                {
                    nextChar = CharacterMappingInfo.EmptyChar;
                    position = TokenPosition.EndOfWord;
                }
                #endregion

                // Oh, no! I must generate all possible words from this PinglishWord

                foreach (var attr in MultipleValueCharMap)
                {

                    if (!attr.Letter.Compare(ch, attr.IsCaseSensitive))
                        continue;

                    possibleValues.Clear();
                    possible2Values.Clear();

                    foreach (CharacterMappingInfo attrValue in attr.Values)
                    {
                        if (attrValue.Postfix == CharacterMappingInfo.EmptyChar)
                        {
                            if ((attrValue.Position & position) == position)
                                possibleValues.Add(attrValue);
                        }
                        else if (nextChar != CharacterMappingInfo.EmptyChar && attrValue.Postfix.Compare(nextChar, false))
                        {
                            possible2Values.Add(attrValue);
                        }
                    }

                    wordsList.UpdateClone(chIndex, possibleValues, possible2Values, ch, nextChar);
                    continue;
                }
            }
            #endregion

            return wordsList;
        }

        private string[] SearchForExactWords(string pinglishWord)
        {
            var results = new List<string>();

            var query = m_preprocessReplacements.Where(element =>
                element.Value.IsExactWord &&
                string.Compare(element.Key, pinglishWord, true) == 0);

            foreach (var element in query)
            {
                results.AddRange(element.Value.Equivalents);
            }

            return results.Distinct().ToArray();
        }

        /// <summary>
        /// Applies preprocess rules to the given word.
        /// </summary>
        /// <param name="pinglishWord"></param>
        /// <returns></returns>
        private IEnumerable<string> PreprocessWord(string pinglishWord)
        {
            pinglishWord = pinglishWord.Trim();
            var results = new List<string>();
            if (string.IsNullOrEmpty(pinglishWord))
                return results;

            string[] replacements;

            bool firstUpperLetterReplacement = true;
            bool lastUpperLetterReplacement = true;

            if (this.m_bigLettersAtBeginOrEnd.Keys.Contains(pinglishWord[0]))
            {
                for (int i = 1; i < pinglishWord.Length - 2; i++)
                {
                    if (!char.IsLower(pinglishWord[i]))
                    {
                        firstUpperLetterReplacement = false;
                        break;
                    }
                }
                if (firstUpperLetterReplacement)
                {
                    replacements = this.m_bigLettersAtBeginOrEnd[pinglishWord[0]];
                    for (int i = 0; i < replacements.Length; i++)
                    {
                        results.Add(replacements[i] + pinglishWord.Substring(1));
                    }
                }
            }
            if (this.m_bigLettersAtBeginOrEnd.Keys.Contains(pinglishWord[pinglishWord.Length - 1]))
            {
                for (int i = pinglishWord.Length - 2; i > 0; i--)
                {
                    if (!char.IsLower(pinglishWord[i]))
                    {
                        lastUpperLetterReplacement = false;
                        break;
                    }
                }
                if (lastUpperLetterReplacement && pinglishWord.Length > 1)
                {
                    replacements = this.m_bigLettersAtBeginOrEnd[pinglishWord[pinglishWord.Length - 1]];
                    for (int i = 0; i < replacements.Length; i++)
                    {
                        results.Add(pinglishWord.Substring(0, pinglishWord.Length - 1) + replacements[i]);
                    }
                }
            }

            foreach (string key in this.m_preprocessReplacements.Keys)
            {
                var preprocess = this.m_preprocessReplacements[key];

                if (preprocess.IsWholeWord  && string.Compare(key, pinglishWord, true) != 0)
                    continue;

                if (!preprocess.IsWholeWord)
                {
                    if (preprocess.Position == TokenPosition.StartOfWord && 
                        !pinglishWord.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    if (preprocess.Position == TokenPosition.EndOfWord &&
                        !pinglishWord.EndsWith(key, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    if ((preprocess.Position == TokenPosition.MiddleOfWord || preprocess.Position == TokenPosition.Any)
                        && !pinglishWord.ToLowerInvariant().Contains(key.ToLowerInvariant()))
                        continue;
                }

                replacements = preprocess.Equivalents.ToArray();
                for (int i = 0; i < replacements.Length; i++)
                {
                    results.Add(pinglishWord.ToLowerInvariant().Replace(key.ToLowerInvariant(), replacements[i]));
                }
            }

            if (results.Count == 0 || !results.Contains(pinglishWord))
                results.Add(pinglishWord);

            #region Remove Duplicate characters -- Salaaaaam ==> Salaam (Max 2)

            for (int index = 0; index < results.Count; index++)
            {
                string word = results[index].ToLower();
                char prevChar = (char)0;

                var wordWithNo3Dup = new StringBuilder();
                for (int i = 0; i < word.Length; i++)
                {
                    char currChar = word[i];

                    if (prevChar.Compare(currChar, false))
                    {
                        wordWithNo3Dup.Append(currChar);
                        while (i < word.Length)
                        {
                            if (word[i].Compare(currChar, false))
                                ++i;
                            else
                            {
                                --i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        wordWithNo3Dup.Append(currChar);
                        prevChar = currChar;
                    }
                }
                results[index] = wordWithNo3Dup.ToString();
            }

            #endregion

            return results;
        }

        #endregion
    }
}

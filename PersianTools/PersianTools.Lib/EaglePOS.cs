using System;
using System.Collections.Generic;
using System.Linq;


namespace SCICT.NLP.Persian.Constants
{
    /// <summary>
    /// Persian EAGLE Compatible POS Tag Representer
    /// </summary>
    public class PartOfSpeech
    {
        public POSAnnotation.Major Major;
        public POSAnnotation.Type Type;
        public POSAnnotation.Gender Gender;
        public POSAnnotation.Number Number;
        public POSAnnotation.Case Case;
        public POSAnnotation.Person Person;
        public POSAnnotation.Finiteness Finiteness;
        public POSAnnotation.Mood Mood;
        public POSAnnotation.Tense Tense;
        public POSAnnotation.Voice Voice;
        public POSAnnotation.Status Status;
        public POSAnnotation.Degree Degree;
        public POSAnnotation.Possessive Possessive;
        public POSAnnotation.CategoryProDet CategoryProDet;
        public POSAnnotation.PronType PronType;
        public POSAnnotation.DetType DetType;
        public POSAnnotation.ArticleType ArticleType;
        public POSAnnotation.AdPosType AdPosType;
        public POSAnnotation.ConType ConType;
        public POSAnnotation.NumType NumType;
        public POSAnnotation.NumFunction NumFunction;
        public POSAnnotation.ResType ResType;
        public POSAnnotation.PuncType PuncType;

        public override string ToString()
        {
            List<string> result = new List<string>();
            #region ToString
            if (Major != POSAnnotation.Major.Unknown) result.Add(POSAnnotation.ToString(Major));
            if (Type != POSAnnotation.Type.Unknown) result.Add(POSAnnotation.ToString(Type));
            if (Gender != POSAnnotation.Gender.Unknown) result.Add(POSAnnotation.ToString(Gender));
            if (Number != POSAnnotation.Number.Unknown) result.Add(POSAnnotation.ToString(Number));
            if (Case != POSAnnotation.Case.Unknown) result.Add(POSAnnotation.ToString(Case));
            if (Person != POSAnnotation.Person.Unknown) result.Add(POSAnnotation.ToString(Person));
            if (Finiteness != POSAnnotation.Finiteness.Unknown) result.Add(POSAnnotation.ToString(Finiteness));
            if (Mood != POSAnnotation.Mood.Unknown) result.Add(POSAnnotation.ToString(Mood));
            if (Tense != POSAnnotation.Tense.Unknown) result.Add(POSAnnotation.ToString(Tense));
            if (Voice != POSAnnotation.Voice.Unknown) result.Add(POSAnnotation.ToString(Voice));
            if (Status != POSAnnotation.Status.Unknown) result.Add(POSAnnotation.ToString(Status));
            if (Degree != POSAnnotation.Degree.Unknown) result.Add(POSAnnotation.ToString(Degree));
            if (Possessive != POSAnnotation.Possessive.Unknown) result.Add(POSAnnotation.ToString(Possessive));
            if (CategoryProDet != POSAnnotation.CategoryProDet.Unknown) result.Add(POSAnnotation.ToString(CategoryProDet));
            if (PronType != POSAnnotation.PronType.Unknown) result.Add(POSAnnotation.ToString(PronType));
            if (DetType != POSAnnotation.DetType.Unknown) result.Add(POSAnnotation.ToString(DetType));
            if (ArticleType != POSAnnotation.ArticleType.Unknown) result.Add(POSAnnotation.ToString(ArticleType));
            if (AdPosType != POSAnnotation.AdPosType.Unknown) result.Add(POSAnnotation.ToString(AdPosType));
            if (ConType != POSAnnotation.ConType.Unknown) result.Add(POSAnnotation.ToString(ConType));
            if (NumType != POSAnnotation.NumType.Unknown) result.Add(POSAnnotation.ToString(NumType));
            if (NumFunction != POSAnnotation.NumFunction.Unknown) result.Add(POSAnnotation.ToString(NumFunction));
            if (ResType != POSAnnotation.ResType.Unknown) result.Add(POSAnnotation.ToString(ResType));
            if (PuncType != POSAnnotation.PuncType.Unknown) result.Add(POSAnnotation.ToString(PuncType));
            #endregion
            return result.Aggregate((left, right) => left + ", " + right);
        }

        public static PartOfSpeech Parse(string value)
        {
            PartOfSpeech result = new PartOfSpeech();
            var annotations = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var annotation in annotations)
            {
                var annot = POSAnnotation.Parse(annotation.Trim().ToUpper());
                var annotType = annot.GetType();

                #region Checking Annotation Type
                if (annotType == typeof(POSAnnotation.Major))
                {
                    if (result.Major != POSAnnotation.Major.Unknown) throw new Exception("Major Annotation Redefinition");
                    result.Major = (POSAnnotation.Major)annot;
                }
                else if (annotType == typeof(POSAnnotation.Type))
                {
                    if (result.Type != POSAnnotation.Type.Unknown) throw new Exception("Type Annotation Redefinition");
                    result.Type = (POSAnnotation.Type)annot;
                }
                else if (annotType == typeof(POSAnnotation.Gender))
                {
                    if (result.Gender != POSAnnotation.Gender.Unknown) throw new Exception("Gender Annotation Redefinition");
                    result.Gender = (POSAnnotation.Gender)annot;
                }
                else if (annotType == typeof(POSAnnotation.Number))
                {
                    if (result.Number != POSAnnotation.Number.Unknown) throw new Exception("Number Annotation Redefinition");
                    result.Number = (POSAnnotation.Number)annot;
                }
                else if (annotType == typeof(POSAnnotation.Case))
                {
                    if (result.Case != POSAnnotation.Case.Unknown) throw new Exception("Case Annotation Redefinition");
                    result.Case = (POSAnnotation.Case)annot;
                }
                else if (annotType == typeof(POSAnnotation.Person))
                {
                    if (result.Person != POSAnnotation.Person.Unknown) throw new Exception("Person Annotation Redefinition");
                    result.Person = (POSAnnotation.Person)annot;
                }
                else if (annotType == typeof(POSAnnotation.Finiteness))
                {
                    if (result.Finiteness != POSAnnotation.Finiteness.Unknown) throw new Exception("Finiteness Annotation Redefinition");
                    result.Finiteness = (POSAnnotation.Finiteness)annot;
                }
                else if (annotType == typeof(POSAnnotation.Mood))
                {
                    if (result.Mood != POSAnnotation.Mood.Unknown) throw new Exception("Mood Annotation Redefinition");
                    result.Mood = (POSAnnotation.Mood)annot;
                }
                else if (annotType == typeof(POSAnnotation.Tense))
                {
                    if (result.Tense != POSAnnotation.Tense.Unknown) throw new Exception("Tense Annotation Redefinition");
                    result.Tense = (POSAnnotation.Tense)annot;
                }
                else if (annotType == typeof(POSAnnotation.Voice))
                {
                    if (result.Voice != POSAnnotation.Voice.Unknown) throw new Exception("Voice Annotation Redefinition");
                    result.Voice = (POSAnnotation.Voice)annot;
                }
                else if (annotType == typeof(POSAnnotation.Status))
                {
                    if (result.Status != POSAnnotation.Status.Unknown) throw new Exception("Status Annotation Redefinition");
                    result.Status = (POSAnnotation.Status)annot;
                }
                else if (annotType == typeof(POSAnnotation.Degree))
                {
                    if (result.Degree != POSAnnotation.Degree.Unknown) throw new Exception("Degree Annotation Redefinition");
                    result.Degree = (POSAnnotation.Degree)annot;
                }
                else if (annotType == typeof(POSAnnotation.Possessive))
                {
                    if (result.Possessive != POSAnnotation.Possessive.Unknown) throw new Exception("Possessive Annotation Redefinition");
                    result.Possessive = (POSAnnotation.Possessive)annot;
                }
                else if (annotType == typeof(POSAnnotation.CategoryProDet))
                {
                    if (result.CategoryProDet != POSAnnotation.CategoryProDet.Unknown) throw new Exception("CategoryProDet Annotation Redefinition");
                    result.CategoryProDet = (POSAnnotation.CategoryProDet)annot;
                }
                else if (annotType == typeof(POSAnnotation.PronType))
                {
                    if (result.PronType != POSAnnotation.PronType.Unknown) throw new Exception("PronType Annotation Redefinition");
                    result.PronType = (POSAnnotation.PronType)annot;
                }
                else if (annotType == typeof(POSAnnotation.DetType))
                {
                    if (result.DetType != POSAnnotation.DetType.Unknown) throw new Exception("DetType Annotation Redefinition");
                    result.DetType = (POSAnnotation.DetType)annot;
                }
                else if (annotType == typeof(POSAnnotation.ArticleType))
                {
                    if (result.ArticleType != POSAnnotation.ArticleType.Unknown) throw new Exception("ArticleType Annotation Redefinition");
                    result.ArticleType = (POSAnnotation.ArticleType)annot;
                }
                else if (annotType == typeof(POSAnnotation.AdPosType))
                {
                    if (result.AdPosType != POSAnnotation.AdPosType.Unknown) throw new Exception("AdPosType Annotation Redefinition");
                    result.AdPosType = (POSAnnotation.AdPosType)annot;
                }
                else if (annotType == typeof(POSAnnotation.ConType))
                {
                    if (result.ConType != POSAnnotation.ConType.Unknown) throw new Exception("ConType Annotation Redefinition");
                    result.ConType = (POSAnnotation.ConType)annot;
                }
                else if (annotType == typeof(POSAnnotation.NumType))
                {
                    if (result.NumType != POSAnnotation.NumType.Unknown) throw new Exception("NumType Annotation Redefinition");
                    result.NumType = (POSAnnotation.NumType)annot;
                }
                else if (annotType == typeof(POSAnnotation.NumFunction))
                {
                    if (result.NumFunction != POSAnnotation.NumFunction.Unknown) throw new Exception("NumFunction Annotation Redefinition");
                    result.NumFunction = (POSAnnotation.NumFunction)annot;
                }
                else if (annotType == typeof(POSAnnotation.ResType))
                {
                    if (result.ResType != POSAnnotation.ResType.Unknown) throw new Exception("ResType Annotation Redefinition");
                    result.ResType = (POSAnnotation.ResType)annot;
                }
                else if (annotType == typeof(POSAnnotation.PuncType))
                {
                    if (result.PuncType != POSAnnotation.PuncType.Unknown) throw new Exception("PuncType Annotation Redefinition");
                    result.PuncType = (POSAnnotation.PuncType)annot;
                }
                #endregion
            }
            return result;
        }
    }

    ///<summary>
    /// Set of annotations used for defining a part fo speech
    ///</summary>
    public static class POSAnnotation
    {
        #region Constants
        // For performance reasons we have to implement a two-way mapping
        private static readonly Dictionary<Enum, string> Straight;
        private static readonly Dictionary<string, Enum> Reverse;
        static POSAnnotation()
        {
            Straight = new Dictionary<Enum, string>();
            Reverse = new Dictionary<string, Enum>();

            #region Major
            Straight.Add(Major.Noun, "N");
            Reverse.Add("N", Major.Noun);
            Straight.Add(Major.Verb, "V");
            Reverse.Add("V", Major.Verb);
            Straight.Add(Major.Adjective, "AJ");
            Reverse.Add("AJ", Major.Adjective);
            Straight.Add(Major.PronounDeterminer, "PRODET");
            Reverse.Add("PRODET", Major.PronounDeterminer);
            Straight.Add(Major.Article, "ART");
            Reverse.Add("ART", Major.Article);
            Straight.Add(Major.Adverb, "ADV");
            Reverse.Add("ADV", Major.Adverb);
            Straight.Add(Major.Adposition, "ADP");
            Reverse.Add("ADP", Major.Adposition);
            Straight.Add(Major.Conjuntion, "CONJ");
            Reverse.Add("CONJ", Major.Conjuntion);
            Straight.Add(Major.Numeral, "NUM");
            Reverse.Add("NUM", Major.Numeral);
            Straight.Add(Major.Interjection, "INT");
            Reverse.Add("INT", Major.Interjection);
            Straight.Add(Major.UniqueUnassigend, "UNQ");
            Reverse.Add("UNQ", Major.UniqueUnassigend);
            Straight.Add(Major.Residual, "RES");
            Reverse.Add("RES", Major.Residual);
            Straight.Add(Major.Punctuation, "PUNC");
            Reverse.Add("PUNC", Major.Punctuation);
            #endregion

            #region Type
            Straight.Add(Type.Common, "COM");
            Reverse.Add("COM", Type.Common);
            Straight.Add(Type.Proper, "PROP");
            Reverse.Add("PROP", Type.Proper);
            #endregion

            #region Gender
            Straight.Add(Gender.Masculine, "MASC");
            Straight.Add(Gender.Feminine, "FEMI");
            Straight.Add(Gender.Neuter, "NEUT");
            Reverse.Add("MASC", Gender.Masculine);
            Reverse.Add("FEMI", Gender.Feminine);
            Reverse.Add("NEUT", Gender.Neuter);
            #endregion

            #region Number
            Straight.Add(Number.Singular, "SING");
            Reverse.Add("SING", Number.Singular);
            Straight.Add(Number.Plural, "PL");
            Reverse.Add("PL", Number.Plural);
            #endregion

            #region Case
            Straight.Add(Case.Acuccative, "ACUC");
            Reverse.Add("ACUC", Case.Acuccative);
            Straight.Add(Case.Dative, "DATV");
            Reverse.Add("DATV", Case.Dative);
            Straight.Add(Case.Genetive, "GENET");
            Reverse.Add("GENET", Case.Genetive);
            Straight.Add(Case.Nominative, "NOMIN");
            Reverse.Add("NOMIN", Case.Nominative);
            Straight.Add(Case.NonGenetive, "NONGEN");
            Reverse.Add("NONGEN", Case.NonGenetive);
            Straight.Add(Case.Oblique, "OBLQ");
            Reverse.Add("OBLQ", Case.Oblique);
            Straight.Add(Case.Vocative, "VOCA");
            Reverse.Add("VOCA", Case.Vocative);
            #endregion

            #region Person
            Straight.Add(Person.First, "1");
            Straight.Add(Person.Second, "2");
            Straight.Add(Person.Third, "3");
            Reverse.Add("1", Person.First);
            Reverse.Add("2", Person.Second);
            Reverse.Add("3", Person.Third);
            #endregion

            #region Voice
            Straight.Add(Voice.Active, "ACTV");
            Straight.Add(Voice.Passive, "PASV");
            Reverse.Add("ACTV", Voice.Active);
            Reverse.Add("PASV", Voice.Passive);
            #endregion

            #region Status
            Straight.Add(Status.Main, "MAIN");
            Straight.Add(Status.Auxiliary, "AUXL");
            Reverse.Add("MAIN", Status.Main);
            Reverse.Add("AUXL", Status.Auxiliary);
            #endregion

            #region Possessive
            Straight.Add(Possessive.Singular, "PSING");
            Straight.Add(Possessive.Plural, "PPLUR");
            Reverse.Add("PSING", Possessive.Singular);
            Reverse.Add("PPLUR", Possessive.Plural);
            #endregion

            #region CategoryProDet
            Straight.Add(CategoryProDet.Pronoun, "PRO");
            Straight.Add(CategoryProDet.Determiner, "DET");
            Straight.Add(CategoryProDet.Both, "PDBOTH");
            Reverse.Add("PRO", CategoryProDet.Pronoun);
            Reverse.Add("DET", CategoryProDet.Determiner);
            Reverse.Add("PDBOTH", CategoryProDet.Both);
            #endregion

            #region PronType
            Straight.Add(PronType.Demonestrative, "PDEMO");
            Straight.Add(PronType.Indefinite, "PINDEF");
            Straight.Add(PronType.IntRel, "PINTREL");
            Straight.Add(PronType.PersRefl, "PREFL");
            Straight.Add(PronType.Possessive, "PPOSS");
            Reverse.Add("PDEMO", PronType.Demonestrative);
            Reverse.Add("PINDEF", PronType.Indefinite);
            Reverse.Add("PINTREL", PronType.IntRel);
            Reverse.Add("PREFL", PronType.PersRefl);
            Reverse.Add("PPOSS", PronType.Possessive);
            #endregion

            #region DetType
            Straight.Add(DetType.Demonestrative, "DDEMO");
            Straight.Add(DetType.Indefinite, "DINDEF");
            Straight.Add(DetType.IntRel, "DINTREL");
            Straight.Add(DetType.Partitive, "DPARIT");
            Straight.Add(DetType.Possessive, "DPOSS");
            Reverse.Add("DDEMO", DetType.Demonestrative);
            Reverse.Add("DINDEF", DetType.Indefinite);
            Reverse.Add("DINTREL", DetType.IntRel);
            Reverse.Add("DPARIT", DetType.Partitive);
            Reverse.Add("DPOSS", DetType.Possessive);
            #endregion

            #region Finiteness
            Straight.Add(Finiteness.Finite, "FIN");
            Straight.Add(Finiteness.NonFinite, "NFIN");
            Reverse.Add("FIN", Finiteness.Finite);
            Reverse.Add("NFIN", Finiteness.NonFinite);
            #endregion

            #region Mood
            Straight.Add(Mood.Conditional, "COND");
            Reverse.Add("COND", Mood.Conditional);
            Straight.Add(Mood.Gerund, "GERND");
            Reverse.Add("GERND", Mood.Gerund);
            Straight.Add(Mood.Imperetive, "IPRTV");
            Reverse.Add("IPRTV", Mood.Imperetive);
            Straight.Add(Mood.Indicative, "IDCTV");
            Reverse.Add("IDCTV", Mood.Indicative);
            Straight.Add(Mood.Infinitive, "INFTV");
            Reverse.Add("INFTV", Mood.Infinitive);
            Straight.Add(Mood.Participle, "PRCPL");
            Reverse.Add("PRCPL", Mood.Participle);
            Straight.Add(Mood.Subjunctive, "SUBJN");
            Reverse.Add("SUBJN", Mood.Subjunctive);
            Straight.Add(Mood.Supine, "SUPIN");
            Reverse.Add("SUPIN", Mood.Supine);
            #endregion

            #region Tense
            Straight.Add(Tense.Present, "PR");
            Reverse.Add("PR", Tense.Present);
            Straight.Add(Tense.Imperfect, "IMP");
            Reverse.Add("IMP", Tense.Imperfect);
            Straight.Add(Tense.Future, "FUT");
            Reverse.Add("FUT", Tense.Future);
            Straight.Add(Tense.Past, "PA");
            Reverse.Add("PA", Tense.Past);
            #endregion

            #region Degree
            Straight.Add(Degree.Possitive, "POSS");
            Straight.Add(Degree.Comparative, "COMP");
            Straight.Add(Degree.Superlative, "SUP");
            Reverse.Add("POSS", Degree.Possitive);
            Reverse.Add("COMP", Degree.Comparative);
            Reverse.Add("SUP", Degree.Superlative);
            #endregion

            #region ConType
            Straight.Add(ConType.Coordinating, "COORD");
            Straight.Add(ConType.Subordinating, "SUBOR");
            Reverse.Add("COORD", ConType.Coordinating);
            Reverse.Add("SUBOR", ConType.Subordinating);
            #endregion

            #region NumType
            Straight.Add(NumType.Cordinal, "CARD");
            Straight.Add(NumType.Ordinal, "ORD");
            Reverse.Add("CARD", NumType.Cordinal);
            Reverse.Add("ORD", NumType.Ordinal);
            #endregion

            #region ArticleType
            Straight.Add(ArticleType.Definite, "DEF");
            Straight.Add(ArticleType.Indefinite, "IDEF");
            Reverse.Add("DEF", ArticleType.Definite);
            Reverse.Add("IDEF", ArticleType.Indefinite);
            #endregion

            #region AdPosType
            Straight.Add(AdPosType.Preposition, "PREP");
            Straight.Add(AdPosType.Postposition, "POSTP");
            Reverse.Add("PREP", AdPosType.Preposition);
            Reverse.Add("POSTP", AdPosType.Postposition);
            #endregion

            #region ResType
            Straight.Add(ResType.Abbreviation, "ABBR");
            Straight.Add(ResType.Accrunim, "ACCR");
            Straight.Add(ResType.ForeignWord, "FW");
            Straight.Add(ResType.Formula, "FORM");
            Straight.Add(ResType.Symbol, "SYM");
            Straight.Add(ResType.Unclassified, "UNCL");
            Reverse.Add("ABBR", ResType.Abbreviation);
            Reverse.Add("ACCR", ResType.Accrunim);
            Reverse.Add("FW", ResType.ForeignWord);
            Reverse.Add("FORM", ResType.Formula);
            Reverse.Add("SYM", ResType.Symbol);
            Reverse.Add("UNCL", ResType.Unclassified);
            #endregion

            #region NumFunction
            Straight.Add(NumFunction.Adjective, "NFAJ");
            Straight.Add(NumFunction.Determiner, "NFDET");
            Straight.Add(NumFunction.Pronoun, "NFPRO");
            Reverse.Add("NFAJ", NumFunction.Adjective);
            Reverse.Add("NFDET", NumFunction.Determiner);
            Reverse.Add("NFPRO", NumFunction.Pronoun);
            #endregion

            #region PuncType
            Straight.Add(PuncType.Apostrophe, "APOSTRO");
            Reverse.Add("APOSTRO", PuncType.Apostrophe);
            Straight.Add(PuncType.BracketsLeft, "LBRACK");
            Reverse.Add("LBRACK", PuncType.BracketsLeft);
            Straight.Add(PuncType.BracketsRight, "RBRACK");
            Reverse.Add("RBRACK", PuncType.BracketsRight);
            Straight.Add(PuncType.Colon, "COLON");
            Reverse.Add("COLON", PuncType.Colon);
            Straight.Add(PuncType.Comma, "COMMA");
            Reverse.Add("COMMA", PuncType.Comma);
            Straight.Add(PuncType.Ellipsis, "ELLIP");
            Reverse.Add("ELLIP", PuncType.Ellipsis);
            Straight.Add(PuncType.ExclamationMark, "EXCLAM");
            Reverse.Add("EXCLAM", PuncType.ExclamationMark);
            Straight.Add(PuncType.GuillemetsLeft, "LGUIL");
            Reverse.Add("LGUIL", PuncType.GuillemetsLeft);
            Straight.Add(PuncType.GuillemetsRight, "RGUIL");
            Reverse.Add("RGUIL", PuncType.GuillemetsRight);
            Straight.Add(PuncType.HyphenDash, "HDASH");
            Reverse.Add("HDASH", PuncType.HyphenDash);
            Straight.Add(PuncType.Other, "OTHPUNC");
            Reverse.Add("OTHPUNC", PuncType.Other);
            Straight.Add(PuncType.ParanthesisLeft, "LPARAN");
            Reverse.Add("LPARAN", PuncType.ParanthesisLeft);
            Straight.Add(PuncType.ParanthesisRight, "RPARAN");
            Reverse.Add("RPARAN", PuncType.ParanthesisRight);
            Straight.Add(PuncType.Period, "PERIOD");
            Reverse.Add("PERIOD", PuncType.Period);
            Straight.Add(PuncType.QuestionMark, "QUST");
            Reverse.Add("QUST", PuncType.QuestionMark);
            Straight.Add(PuncType.QuotationMarksLeft, "LQUOT");
            Reverse.Add("LQUOT", PuncType.QuotationMarksLeft);
            Straight.Add(PuncType.QuotationMarksRight, "RQUOT");
            Reverse.Add("RQUOT", PuncType.QuotationMarksRight);
            Straight.Add(PuncType.Semicolon, "SEMI");
            Reverse.Add("SEMI", PuncType.Semicolon);
            Straight.Add(PuncType.Slash, "SLASH");
            Reverse.Add("SLASH", PuncType.Slash);
            #endregion
        }
        #endregion

        /// <summary>
        /// Converts a annotation into standard string.
        /// </summary>
        /// <param name="annotation">The annotation (defined in this class) to be converted</param>
        /// <returns>Equivalant string of annotation</returns>
        public static string ToString(Enum annotation)
        {
            if (!Straight.ContainsKey(annotation)) throw new Exception("Unknown Annotation Type");
            return Straight[annotation];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public static Enum Parse(string annotation)
        {
            if (!Reverse.ContainsKey(annotation)) throw new Exception("Unknown Annotation String");
            return Reverse[annotation];
        }

        #region Enums

        public enum Major
        {
            Unknown = 0,
            Noun,
            Verb,
            Adjective,
            PronounDeterminer,
            Article,
            Adverb,
            Adposition,
            Conjuntion,
            Numeral,
            Interjection,
            UniqueUnassigend,
            Residual,
            Punctuation
        }

        public enum Type : byte
        {
            Unknown = 0,
            Common,
            Proper
        }

        public enum Gender : byte
        {
            Unknown = 0,
            Masculine,
            Feminine,
            Neuter
        }

        public enum Number : byte
        {
            Unknown = 0,
            Singular,
            Plural
        }

        public enum Case : byte
        {
            Unknown = 0,
            Nominative,
            Genetive,
            NonGenetive,
            Oblique,
            Dative,
            Acuccative,
            Vocative
        }

        public enum Person : byte
        {
            Unknown = 0,
            First,
            Second,
            Third
        }

        public enum Finiteness : byte
        {
            Unknown = 0,
            Finite,
            NonFinite
        }

        public enum Mood : byte
        {
            Unknown = 0,
            Indicative,
            Subjunctive,
            Imperetive,
            Conditional,
            Infinitive,
            Participle,
            Gerund,
            Supine
        }

        public enum Tense : byte
        {
            Unknown = 0,
            Present,
            Imperfect,
            Future,
            Past
        }

        public enum Voice : byte
        {
            Unknown = 0,
            Active,
            Passive
        }

        public enum Status : byte
        {
            Unknown = 0,
            Main,
            Auxiliary
        }

        public enum Degree : byte
        {
            Unknown = 0,
            Possitive,
            Comparative,
            Superlative
        }

        public enum Possessive : byte
        {
            Unknown = 0,
            Singular,
            Plural
        }

        public enum CategoryProDet : byte
        {
            Unknown = 0,
            Pronoun,
            Determiner,
            Both
        }

        public enum PronType : byte
        {
            Unknown = 0,
            Demonestrative,
            Indefinite,
            Possessive,
            IntRel,
            PersRefl
        }

        public enum DetType : byte
        {
            Unknown = 0,
            Demonestrative,
            Indefinite,
            Possessive,
            IntRel,
            Partitive
        }

        public enum ArticleType : byte
        {
            Unknown = 0,
            Definite,
            Indefinite
        }

        public enum AdPosType : byte
        {
            Unknown = 0,
            Preposition,
            Postposition
        }

        public enum ConType : byte
        {
            Unknown = 0,
            Coordinating,
            Subordinating
        }

        public enum NumType : byte
        {
            Unknown = 0,
            Cordinal,
            Ordinal
        }

        public enum NumFunction : byte
        {
            Unknown = 0,
            Pronoun,
            Determiner,
            Adjective
        }

        public enum ResType : byte
        {
            Unknown = 0,
            ForeignWord,
            Formula,
            Symbol,
            Accrunim,
            Abbreviation,
            Unclassified,
        }

        public enum PuncType
        {
            Unknown = 0,
            Period,
            Comma,
            QuestionMark,
            ParanthesisLeft,
            ParanthesisRight,
            GuillemetsLeft,
            GuillemetsRight,
            Apostrophe,
            BracketsLeft,
            BracketsRight,
            Colon,
            Ellipsis,
            ExclamationMark,
            HyphenDash,
            QuotationMarksLeft,
            QuotationMarksRight,
            Semicolon,
            Slash,
            Other
        }

        #endregion
    }
}


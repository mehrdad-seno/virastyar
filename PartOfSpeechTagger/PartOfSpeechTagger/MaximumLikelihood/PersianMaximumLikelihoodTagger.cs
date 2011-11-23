using System;
using System.Collections.Generic;
using System.Linq;
using SCICT.NLP.Morphology.Inflection.Conjugation;
using SCICT.NLP.Persian.Constants;
using SCICT.NLP.Morphology.Inflection;

namespace SCICT.NLP.PartOfSpeechTagger.MaximumLikelihood
{
    public class PersianMaximumLikelihoodTagger : IPosTagger
    {
        #region Tagger Configuration Class
        public class Config
        {
            public string DictionaryFilename { get; set; }
            public string StemFilename { get; set; }

            public PersianPartOfSpeech[] DefaultTags { get; set; }
            public double[] DefaultWeights { get; set; }

            public bool SuffixBasedTagging { get; set; }
            public bool UseRecursiveTagging { get; set; }

            public Config()
            {
                DefaultTags = new[] { PersianPartOfSpeech.Unknown };
                DefaultWeights = new[] { Double.NaN };
            }

            public Config(string dic, string stem)
                : this()
            {
                DictionaryFilename = dic;
                StemFilename = stem;
            }
        }
        #endregion

        private readonly MaximumLikelihoodTagger _baseTagger;
        private readonly Config _config;
        private readonly PersianSuffixLemmatizer _lemmatizer;
        private readonly Dictionary<string, List<Conjugator.VerbInfo>> _verbs;

        public PersianMaximumLikelihoodTagger(Config config)
        {
            _config = config;

            MaximumLikelihoodTagger.Config baseConfig = new MaximumLikelihoodTagger.Config();
            baseConfig.DictionaryFilename = _config.DictionaryFilename;
            baseConfig.Normalize = true;
            baseConfig.DefaultTags = new[] { PersianPartOfSpeech.Unknown };
            baseConfig.DefaultWeights = new[] { Double.NaN };
            _baseTagger = new MaximumLikelihoodTagger(baseConfig);

            _lemmatizer = new PersianSuffixLemmatizer(false, true);
            _verbs = new Dictionary<string, List<Conjugator.VerbInfo>>();

            VerbInfoContainer dic = new VerbInfoContainer();
            dic.LoadStemFile(_config.StemFilename);
            Conjugator conjugator = new Conjugator(dic);
            foreach (ENUM_TENSE_PERSON person in Enum.GetValues(typeof(ENUM_TENSE_PERSON)))
            {
                foreach (var verbinfo in conjugator.ConjugateInfo(ENUM_VERB_TYPE.SADE, person))
                {
                    if (!_verbs.ContainsKey(verbinfo.Verb))
                    {
                        _verbs.Add(verbinfo.Verb, new List<Conjugator.VerbInfo>());
                    }
                    if (!_verbs[verbinfo.Verb].Contains(verbinfo))
                        _verbs[verbinfo.Verb].Add(verbinfo);
                }

                foreach (var verbinfo in conjugator.ConjugateInfo(ENUM_VERB_TYPE.PISHVANDI, person))
                {
                    if (!_verbs.ContainsKey(verbinfo.Verb))
                    {
                        _verbs.Add(verbinfo.Verb, new List<Conjugator.VerbInfo>());
                    }

                    if(!_verbs[verbinfo.Verb].Contains(verbinfo))
                        _verbs[verbinfo.Verb].Add(verbinfo);
                }
            }
        }

        private void GetPossibleTagsRecursive(string lexeme, out string[] lemmas, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            if (_baseTagger.IsKnown(lexeme))
            {
                _baseTagger.GetPossibleTags(lexeme, out lemmas, out tags, out weights);
                return;
            }

            var patinfos = _lemmatizer.MatchForSuffix(lexeme);
            if (patinfos.Length == 0)
            {
                tags = _config.DefaultTags;
                weights = _config.DefaultWeights;
                lemmas = new [] { lexeme };
                return;
            }

            // Starting from the longest lemma (=baseWord)
            for (int i = patinfos.Length - 1; i >= 0; i--)
            {
                var lemmaPat = patinfos[i];
                if (_baseTagger.IsKnown(lemmaPat.BaseWord))
                {
                    PersianPartOfSpeech[] baseTags;
                    double[] baseWeights;
                    _baseTagger.GetPossibleTags(lemmaPat.BaseWord, out lemmas, out baseTags, out baseWeights);
                    ApplyPersianDeclensionRules(baseTags, baseWeights, lemmaPat.Suffix, out tags, out weights);
                    return;
                }
                else
                {
                    PersianPartOfSpeech[] baseTags;
                    double[] baseWeights;
                    GetPossibleTagsRecursive(lemmaPat.BaseWord, out lemmas, out baseTags, out baseWeights);
                    ApplyPersianDeclensionRules(baseTags, baseWeights, lemmaPat.Suffix, out tags, out weights);
                    if (tags[0] != PersianPartOfSpeech.Unknown) return;
                }
            }

            tags = _config.DefaultTags;
            weights = _config.DefaultWeights;
            lemmas = new [] { lexeme };
        }

        private void GetPossibleTagsIterative(string lexeme, out string[] lemmas, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            if (_baseTagger.IsKnown(lexeme))
            {
                _baseTagger.GetPossibleTags(lexeme, out lemmas, out tags, out weights);
                return;
            }

            var patInfos = _lemmatizer.MatchForSuffix(lexeme);
            if (patInfos.Length == 0)
            {
                tags = _config.DefaultTags;
                weights = _config.DefaultWeights;
                lemmas = new [] { lexeme };
                return;
            }

            // Starting from the longest lemma (=baseWord)
            for (int i = patInfos.Length - 1; i >= 0; i--)
            {
                var curPatLemma = patInfos[i];
                if (_baseTagger.IsKnown(curPatLemma.BaseWord))
                {
                    PersianPartOfSpeech[] baseTags;
                    double[] baseWeights;
                    _baseTagger.GetPossibleTags(curPatLemma.BaseWord, out lemmas, out baseTags, out baseWeights);
                    ApplyPersianDeclensionRules(baseTags, baseWeights, curPatLemma.Suffix, out tags, out weights);
                    return;
                }

                // This is the last iteration and non of possible lemmas were known
                // Trying to use only suffixes to recognize the part of speech
                if (_config.SuffixBasedTagging && i == 0)
                {
                    // TODO: Currently I'm not sure about whether to use longest or shortest lemma for this phase!
                    curPatLemma = patInfos[patInfos.Length - 1];
                    lemmas = new [] { curPatLemma.BaseWord };
                    ApplyPersianDeclensionRules(new[] { PersianPartOfSpeech.Unknown }, new[] { double.NaN }, curPatLemma.Suffix, out tags, out weights);
                    if (tags[0] != PersianPartOfSpeech.Unknown) 
                        return;
                }
            }

            tags = _config.DefaultTags;
            weights = _config.DefaultWeights;
            lemmas = new [] { lexeme };
        }

        private void ApplyPersianDeclensionRules(PersianPartOfSpeech[] baseTags, double[] baseWeights, string suffix, out PersianPartOfSpeech[] targetTags, out double[] targetWeights)
        {
            Dictionary<PersianPartOfSpeech, double> results = new Dictionary<PersianPartOfSpeech, double>();
            PersianSuffixesCategory suffixCat = InflectionAnalyser.SuffixCategory(suffix);
            for (int i = 0; i < baseTags.Length; i++)
            {
                foreach (PersianSuffixesCategory cat in Enum.GetValues(typeof(PersianSuffixesCategory)))
                {
                    if ((suffixCat & cat) == cat)
                    {
                        PersianPartOfSpeech pos = ApplyDeclension(baseTags[i], cat);
                        if (pos == (PersianPartOfSpeech.Adjective | PersianPartOfSpeech.Comparative) && suffix.Equals("ترین"))
                        {
                            pos = PersianPartOfSpeech.Adjective | PersianPartOfSpeech.Superlative;
                        }
                        if (!results.ContainsKey(pos))
                        {
                            results.Add(pos, baseWeights[i]);
                        }
                        else
                        {
                            results[pos] = Math.Max(baseWeights[i], results[pos]);
                        }
                    }
                }
            }
            double sum = results.Values.Sum();
            targetTags = (from result in results orderby result.Value descending select result.Key).ToArray();
            targetWeights = (from result in results orderby result.Value descending select result.Value / sum).ToArray();
        }

        // TODO: Examin the rules! We also might want to add probability to rule application.
        // TODO: Also consider revising the order of rules
        private static PersianPartOfSpeech ApplyDeclension(PersianPartOfSpeech baseTag, PersianSuffixesCategory suffixCat)
        {
            if (baseTag == PersianPartOfSpeech.Adjective && (suffixCat == PersianSuffixesCategory.PluralSignHaa || suffixCat == PersianSuffixesCategory.PluralSignAan))
            {
                return PersianPartOfSpeech.Noun | PersianPartOfSpeech.Plural;
            }

            if (baseTag == PersianPartOfSpeech.Noun && suffixCat == PersianSuffixesCategory.YaaNesbat)
            {
                return PersianPartOfSpeech.Adjective;
            }

            if (baseTag == PersianPartOfSpeech.Adjective && suffixCat == PersianSuffixesCategory.ToBeVerb)
            {
                return PersianPartOfSpeech.Noun;
            }

            if (baseTag == PersianPartOfSpeech.Noun && suffixCat == PersianSuffixesCategory.ComparativeAdjectives)
            {
                return PersianPartOfSpeech.Adjective | PersianPartOfSpeech.Comparative;
            }

            if (baseTag == PersianPartOfSpeech.Unknown)
            {
                if (suffixCat == PersianSuffixesCategory.ComparativeAdjectives) return PersianPartOfSpeech.Adjective | PersianPartOfSpeech.Comparative;
                if (suffixCat == PersianSuffixesCategory.PluralSignHaa) return PersianPartOfSpeech.Noun | PersianPartOfSpeech.Plural;
                if (suffixCat == PersianSuffixesCategory.PluralSignAan) return PersianPartOfSpeech.Noun | PersianPartOfSpeech.Plural;
            }

            return baseTag;
        }

        public void GetPossibleTags(string lexeme, out string[] lemmas, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            if (_config.UseRecursiveTagging)
            {
                GetPossibleTagsRecursive(lexeme, out lemmas, out tags, out weights);
            }
            else
            {
                GetPossibleTagsIterative(lexeme, out lemmas, out tags, out weights);
            }

            if (tags[0] != PersianPartOfSpeech.Unknown && !tags.Contains(PersianPartOfSpeech.Verb)) 
                return;

            // Checking Special Types of Words
            if (_verbs.ContainsKey(lexeme))
            {
                tags = (from info in _verbs[lexeme] select GetVerbPOS(info)).ToArray();
                weights = new double[tags.Length];
                lemmas = (from info in _verbs[lexeme] select info.Stem).ToArray();
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = 1.0 / weights.Length;
                }
                return;
            }
            
            // Or Is It a Closed-Type of Words?!
            // TODO: Handle PUNCs, PROs, CONJs, DETs, Ps, ...
        }

        private PersianPartOfSpeech GetVerbPOS(Conjugator.VerbInfo verbInfo)
        {
            PersianPartOfSpeech pos = PersianPartOfSpeech.Verb;
            switch (verbInfo.Person)
            {
                case ENUM_TENSE_PERSON.SINGULAR_FIRST:
                    pos |= PersianPartOfSpeech.Singular | PersianPartOfSpeech.FirstPerson;
                    break;
                case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                    pos |= PersianPartOfSpeech.Singular | PersianPartOfSpeech.SecondPerson;
                    break;
                case ENUM_TENSE_PERSON.SINGULAR_THIRD:
                    pos |= PersianPartOfSpeech.Singular | PersianPartOfSpeech.ThirdPerson;
                    break;
                case ENUM_TENSE_PERSON.PLURAL_FIRST:
                    pos |= PersianPartOfSpeech.Plural | PersianPartOfSpeech.FirstPerson;
                    break;
                case ENUM_TENSE_PERSON.PLURAL_SECOND:
                    pos |= PersianPartOfSpeech.Plural | PersianPartOfSpeech.SecondPerson;
                    break;
                case ENUM_TENSE_PERSON.PLURAL_THIRD:
                    pos |= PersianPartOfSpeech.Plural | PersianPartOfSpeech.ThirdPerson;
                    break;
            }

            switch (verbInfo.Positivity)
            {
                case ENUM_TENSE_POSITIVITY.POSITIVE:
                    pos |= PersianPartOfSpeech.Positive;
                    break;
                case ENUM_TENSE_POSITIVITY.NEGATIVE:
                    pos |= PersianPartOfSpeech.Negative;
                    break;
            }

            switch (verbInfo.Time)
            {
                case ENUM_TENSE_TIME.AMR:
                    pos |= PersianPartOfSpeech.AMR;
                    break;
                case ENUM_TENSE_TIME.AYANDE:
                    pos |= PersianPartOfSpeech.AYANDE;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                    pos |= PersianPartOfSpeech.MAZI_E_BAEID;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                    pos |= PersianPartOfSpeech.MAZI_E_BAEIDE_NAGHLI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    pos |= PersianPartOfSpeech.MAZI_E_ELTEZAMI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                    pos |= PersianPartOfSpeech.MAZI_E_ESTEMRARI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                    pos |= PersianPartOfSpeech.MAZI_E_ESTEMRARIE_NAGHLI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    pos |= PersianPartOfSpeech.MAZI_E_MOSTAMAR;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    pos |= PersianPartOfSpeech.MAZI_E_MOSTAMARE_NAGHLI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    pos |= PersianPartOfSpeech.MAZI_E_SADE;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    pos |= PersianPartOfSpeech.MAZI_E_SADEYE_NAGHLI;
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    pos |= PersianPartOfSpeech.MOZARE_E_EKHBARI;
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    pos |= PersianPartOfSpeech.MOZARE_E_ELTEZAMI;
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    pos |= PersianPartOfSpeech.MOZARE_E_MOSTAMAR;
                    break;
            }

            return pos;
        }


        public void GetPossibleTags(string lexeme, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            string[] dummyLemmas;
            GetPossibleTags(lexeme, out dummyLemmas, out tags, out weights);
        }

        public void GetPossibleTags(string lexeme, Context context, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            GetPossibleTags(lexeme, out tags, out weights);
        }

        public PersianPartOfSpeech[] Tag(string[] lexemes)
        {
            List<PersianPartOfSpeech> results = new List<PersianPartOfSpeech>();
            foreach (var lexeme in lexemes)
            {
                PersianPartOfSpeech[] tags;
                double[] weights;
                GetPossibleTags(lexeme, out tags, out weights);
                results.Add(tags.Length < 1 ? PersianPartOfSpeech.Unknown : tags[0]);
            }
            return results.ToArray();
        }

        public void TagAndLemmatize(string[] lexemes, out PersianPartOfSpeech[] tags, out string[] lemmas)
        {
            var posResults = new List<PersianPartOfSpeech>();
            var lemResults = new List<string>();
            foreach (var lexeme in lexemes)
            {
                PersianPartOfSpeech[] curtags;
                double[] weights;
                string[] curlemmas;
                GetPossibleTags(lexeme, out curlemmas, out curtags, out weights);
                posResults.Add(curtags.Length < 1 ? PersianPartOfSpeech.Unknown : curtags[0]);
                lemResults.Add(curlemmas[0]);
            }

            tags = posResults.ToArray();
            lemmas = lemResults.ToArray();
        }
    }
}

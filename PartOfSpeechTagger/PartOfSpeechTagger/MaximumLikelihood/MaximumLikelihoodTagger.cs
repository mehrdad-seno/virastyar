using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SCICT.NLP.Persian.Constants;

namespace SCICT.NLP.PartOfSpeechTagger.MaximumLikelihood
{
    public class MaximumLikelihoodTagger : IPosTagger
    {
        #region Dictionary Entry Structure
        /// <summary>
        /// Every line in dictionary file will form a <code>POSDictionaryEntry</code> instance.
        /// </summary>
        /// <remarks>This class is just to be used internally.</remarks>
        internal struct POSDictionaryEntry
        {
            public static POSDictionaryEntry? Parse(string line)
            {
                POSDictionaryEntry entry = new POSDictionaryEntry
                {
                    Lexeme = "",
                    Tags = new Dictionary<PersianPartOfSpeech, double>()
                };
                string[] parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) return null;
                entry.Lexeme = parts[0];

                string[] elements = parts[1].Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var element in elements)
                {
                    PersianPartOfSpeech pos = (PersianPartOfSpeech)Enum.Parse(typeof(PersianPartOfSpeech), element.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    double weight = double.Parse(element.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    entry.Tags.Add(pos, weight);
                }
                return entry;
            }

            public string Lexeme { get; private set; }
            public Dictionary<PersianPartOfSpeech, double> Tags { get; private set; }

            public void GetTags(out PersianPartOfSpeech[] tags, out double[] weights)
            {
                tags = (from tag in Tags orderby tag.Value descending select tag.Key).ToArray();
                weights = (from tag in Tags orderby tag.Value descending select tag.Value).ToArray();
            }
        }
        #endregion

        #region Tagger Configuration Class
        public class Config
        {
            public string DictionaryFilename { get; set; }
            public PersianPartOfSpeech[] DefaultTags { get; set; }
            public double[] DefaultWeights { get; set; }
            public bool Normalize { get; set; }

            public Config()
            {
                Normalize = false;
                DefaultTags = new[] { PersianPartOfSpeech.Unknown };
                DefaultWeights = new[] { Double.NaN };
            }

            public Config(string filename)
                : this()
            {
                DictionaryFilename = filename;
            }
        }
        #endregion

        private readonly Dictionary<string, POSDictionaryEntry> _dictionary;
        private readonly Config _config;

        public MaximumLikelihoodTagger(Config config)
        {
            _config = config;
            _dictionary = new Dictionary<string, POSDictionaryEntry>();

            StreamReader reader = new StreamReader(_config.DictionaryFilename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();
                if (line.StartsWith("#")) continue;
                POSDictionaryEntry? entry = POSDictionaryEntry.Parse(line);
                if (entry.HasValue)
                {
                    _dictionary.Add(entry.Value.Lexeme, entry.Value);
                }
            }
            reader.Close();
        }

        public void GetPossibleTags(string lexeme, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            if (_dictionary.ContainsKey(lexeme))
            {
                _dictionary[lexeme].GetTags(out tags, out weights);
            }
            else
            {
                tags = _config.DefaultTags;
                weights = _config.DefaultWeights;
            }
            if (_config.Normalize)
            {
                double sum = weights.Sum();
                weights = (from weight in weights select weight / sum).ToArray();
            }
        }

        public void GetPossibleTags(string lexeme, out string[] lemmas, out PersianPartOfSpeech[] tags, out double[] weights)
        {
            lemmas = new string[] {lexeme};
            GetPossibleTags(lexeme, out tags, out weights);
        }

        /// <summary>
        /// Computes possible part fo speech tags based on word and context. Be notified that maximum liklihood taggers ignore context information.
        /// </summary>
        /// <param name="lexeme">Focused token which its possible part of speeches should be calculated.</param>
        /// <param name="context">The context regarding the focused word.</param>
        /// <param name="tags">List of possible part of speech tags</param>
        /// <param name="weights">List of possibility weights</param>
        /// <remarks>List of tags is sorted in descending order of weights.</remarks>
        /// <seealso cref="GetPossibleTags(string,out PersianPartOfSpeech[],out double[])"/>
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

        internal bool IsKnown(string lexeme)
        {
            return _dictionary.ContainsKey(lexeme);
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

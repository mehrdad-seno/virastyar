using SCICT.NLP.Persian.Constants;

namespace SCICT.NLP.PartOfSpeechTagger
{
    internal interface IPosTagger
    {
        /// <summary>
        /// Computes possible part of speech tags and corresponding weights for each given <paramref name="lexeme"/>.
        /// </summary>
        /// <param name="lexeme">The word, token or in all lexemes whose part of speech must be computed.</param>
        /// <param name="tags">Resulting part of speech tags</param>
        /// <param name="weights">Weights corresponding each tag</param>
        /// <remarks>It's guaranteed that tags would be in descending order of weights and length of <paramref name="tags"/> and <paramref name="weights"/> are equal.</remarks>
        /// <seealso cref="GetPossibleTags(string,SCICT.NLP.PartOfSpeechTagger.Context,out SCICT.NLP.Persian.Constants.PersianPartOfSpeech[],out double[])"/>
        void GetPossibleTags(string lexeme, out PersianPartOfSpeech[] tags, out double[] weights);

        /// <summary>
        /// Computes possible part of speech tags and corresponding weights for each given <paramref name="lexeme"/>.
        /// </summary>
        /// <param name="lexeme">The word, token or in all lexemes whose part of speech must be computed.</param>
        /// <param name="lemmas">Possible lemmas for the given lexeme</param>
        /// <param name="tags">Resulting part of speech tags</param>
        /// <param name="weights">Weights corresponding each tag</param>
        /// <remarks>It's guaranteed that tags would be in descending order of weights and length of <paramref name="tags"/> and <paramref name="weights"/> are equal.</remarks>
        /// <seealso cref="GetPossibleTags(string,SCICT.NLP.PartOfSpeechTagger.Context,out SCICT.NLP.Persian.Constants.PersianPartOfSpeech[],out double[])"/>
        void GetPossibleTags(string lexeme, out string[] lemmas, out PersianPartOfSpeech[] tags, out double[] weights);

        /// <summary>
        /// Computes possible tag for given <paramref name="lexeme"/> regarding both word and <paramref name="context"/>.
        /// </summary>
        /// <param name="lexeme">The word, token or in all lexemes whose part of speech must be computed.</param>
        /// <param name="context">Context of the focused word (e.g. It's previous and next lexemes or tags)</param>
        /// <param name="tags">Resulting part of speech tags</param>
        /// <param name="weights">Weights corresponding each tag</param>
        /// <remarks>It's guaranteed that tags would be in descending order of weights and length of <paramref name="tags"/> and <paramref name="weights"/> are equal.</remarks>
        /// <seealso cref="GetPossibleTags(string,out SCICT.NLP.Persian.Constants.PersianPartOfSpeech[],out double[])"/>
        void GetPossibleTags(string lexeme, Context context, out PersianPartOfSpeech[] tags, out double[] weights);

        /// <summary>
        /// Tags an array of <code>System.String</code>, i.e. <paramref name="lexems"/>, and returns an array of <code>PersianPartOfSpeech</code>.
        /// </summary>
        /// <param name="lexems">Array of lexemes to be tagged</param>
        /// <returns>Corresponding part of speech tags</returns>
        PersianPartOfSpeech[] Tag(string[] lexems);

        /// <summary>
        /// Tags an array of <code>System.String</code>, i.e. <paramref name="lexems"/>, and returns an array of <code>PersianPartOfSpeech</code>.
        /// </summary>
        /// <param name="lexems">Array of lexemes to be tagged</param>
        /// <param name="tags">Resulting part of speech tags</param>
        /// <param name="lemmas">Possible lemmas for the given lexeme</param>
        void TagAndLemmatize(string[] lexems, out PersianPartOfSpeech[] tags, out string[] lemmas);

    }
}

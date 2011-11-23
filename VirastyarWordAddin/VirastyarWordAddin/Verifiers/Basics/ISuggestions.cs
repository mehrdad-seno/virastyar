namespace VirastyarWordAddin.Verifiers.Basics
{
    /// <summary>
    /// The interface to the list of suggestions sent to the verification window
    /// The interface contains only a field to specify the caption of the control
    /// that shows the list of suggestions. To include the collection of suggstions
    /// the programmer must add his/her own container (e.g., an array of strings, or 
    /// an array of string-description pairs).
    /// </summary>
    public interface ISuggestions
    {
        /// <summary>
        /// The message to be shown as the caption of the list of suggestions
        /// </summary>
        string Message { get; set; }

        string DefaultSuggestion { get; }
    }
}

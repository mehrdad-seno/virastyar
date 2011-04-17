namespace SCICT.NLP.Persian
{
    /// <summary>
    /// Interface to Character Filters that provide means to replace non-standard characters with
    /// their standard ones.
    /// </summary>
    public interface ICharFilter
    {
        /// <summary>
        /// Filters the char and returns the string for its filtered (i.e. standardized) equivalant.
        /// The string may contain 0, 1, or more characters.
        /// If the length of the string is 0, then the character should have been left out.
        /// If the length of the string is 1, then the character might be left intact or replaced with another character.
        /// If the length of the string is more than 1, then there have been no 1-character replacement for this character.
        /// It is replaced with 2 or more characters. e.g. some fonts have encoded Tashdid, and Tanvin in one character. 
        /// To make it standard this character is replaced with 2 characters, one for Tashdid, and the other for Tanvin.
        /// </summary>
        /// <param name="ch">The character to filter.</param>
        /// <returns></returns>
        string FilterChar(char ch);
    }
}

namespace SCICT.NLP.Utility
{
    /// <summary>
    /// Contains the result of persian char filters applied and the statistics of the changes made
    /// </summary>
    public class FilterResultsWithStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterResultsWithStats"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="numLetters">number of letters.</param>
        /// <param name="numDigits">number of digits.</param>
        /// <param name="numErabs">number of erabs.</param>
        /// <param name="numHalfSpaces">number of half-space characters.</param>
        public FilterResultsWithStats(string result, int numLetters, int numDigits, int numErabs, int numHalfSpaces)
        {
            this.Result = result;
            this.NumDigits = numDigits;
            this.NumErabs = numErabs;
            this.NumHalfSpaces = numHalfSpaces;
            this.NumLetters = numLetters;
        }

        /// <summary>
        /// Gets or sets the result of filtering
        /// </summary>
        /// <value>The result of filtering.</value>
        public string Result { get; private set; }

        /// <summary>
        /// Gets or sets the number of letters affected
        /// </summary>
        /// <value>The number of letters affected.</value>
        public int NumLetters { get; private set; }

        /// <summary>
        /// Gets or sets the number of digits affected
        /// </summary>
        /// <value>The number of digits affected.</value>
        public int NumDigits { get; private set; }

        /// <summary>
        /// Gets or sets the number of erabs affected
        /// </summary>
        /// <value>The number of erabs affected.</value>
        public int NumErabs { get; private set; }
        
        /// <summary>
        /// Gets or sets the number of half-spaces affected
        /// </summary>
        /// <value>The number of half-spaces affected.</value>
        public int NumHalfSpaces { get; private set; }

    }
}

using System;
using System.Collections.Generic;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// Atrribute class that specifies which IDocument derivatives support which file extensions.
    /// For a usage example see: MSWordDocument class.
    /// </summary>
    [Obsolete("", true)]
    [AttributeUsage(AttributeTargets.Class)]
    public class SupportedDocTypesAtrribute : Attribute
    {
        /// <summary>
        /// list of supported file types
        /// </summary>
        private readonly List<string> m_supportedDocTypes = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDocTypesAtrribute"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public SupportedDocTypesAtrribute(params string[] args)
        {
            m_supportedDocTypes.AddRange(args);
        }

        /// <summary>
        /// Gets the list of supported file types for the IDocument-derived instance.
        /// </summary>
        public List<string> SupportedTypes
        {
            get { return m_supportedDocTypes; }
        }
    }
}

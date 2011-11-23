using System;
using System.Collections.Generic;
using SCICT.NLP.Persian;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// Parent for objects manipulating MS-Word Document.
    /// </summary>
    [Obsolete("Use RangeWrapper and DocumentUtils instead!", true)]
    public abstract class IDocument
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="IDocument"/> class.
        /// It simply initializes the Persian character filters.
        /// </summary>
        public IDocument()
        {
            AddCharFilter(new PersianCharFilter());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDocument"/> class.
        /// </summary>
        /// <param name="documentPath">The document path.</param>
        public IDocument(string documentPath) : this()
        {
            // TODO: [Question] Automatic Loading, 
            // or let the user decide when to call LoadDocument on this instance
            LoadDocument(documentPath);
        }
        #endregion

        #region Character Filters

        /// <summary>
        /// List of Character Filters already added to the document
        /// </summary>
        private List<ICharFilter> listCharFilters = new List<ICharFilter>();

        /// <summary>
        /// Adds a character filter. Added Character filters are used by FilterChar method.
        /// </summary>
        public void AddCharFilter(ICharFilter filter)
        {
            listCharFilters.Add(filter);
        }

        /// <summary>
        /// Sequence of character filters added to the document.
        /// Added Character filters are used by FilterChar method.
        /// </summary>
        public IEnumerable<ICharFilter> CharFilters
        {
            get
            {
                return listCharFilters;
            }
        }

        /// <summary>
        /// Filters the character using all the CharFilters added.
        /// Since it may use several char filters this method is different form
        /// StringUtils.Filter* methods of PersianUtils, since they only use PersianCharFilter.
        /// This method is public, because IBlock and the children use it.
        /// </summary>
        /// <param name="chin">character to filter</param>
        /// <returns>
        /// The filtered version of the input character. If there are several applicable 
        /// CharFilters applicable to the input character, the one added first is only applied.
        /// If there are no CharFilters applicable, the original character is returned.
        /// </returns>
        public string FilterChar(char chin)
        {
            string str = chin.ToString();
            foreach (ICharFilter cf in listCharFilters)
            {
                str = cf.FilterChar(chin);
                if (str == chin.ToString())
                    continue;
                else
                    return str;
            }
            return str;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// The path to the document, if it is opened directly.
        /// If the document is not opened directly (e.g. it refers to a document provieded by MS Word Addin)
        /// then this property will not have proper value.
        /// </summary>
        public string DocumentPath { get; protected set; }
        
        /// <summary>
        /// Whether the document is loaded from a file, or via an MS-Word-Addin.
        /// </summary>
        public bool IsLoaded { get; protected set; }

        /// <summary>
        /// Abstract method which should open some document from file with the given path.
        /// This method is visible to the class children only. 
        /// Clients should call LoadDocument method instead.
        /// </summary>
        /// <returns>true if load has succeeded.</returns>
        protected abstract bool Load(string documentPath);

        /// <summary>
        /// returns an IBlock to the content of the document. 
        /// This reference will refer to the whole document content.
        /// </summary>
        public abstract IBlock GetContent();

        /// <summary>
        /// public method to load a document from file with the given path.
        /// This method first checks whether the document is already loaded.
        /// It not calls the Load method, which is going to be 
        /// implemented by the class children. It the document is already open
        /// it throws an exception.
        /// </summary>
        public bool LoadDocument(string documentPath)
        {
            if (IsLoaded)
                throw new Exception("This instance is already loaded, try to create a new instance instead.");

            return Load(documentPath);
        }

        //internal bool Cleanup();
        #endregion
    }
}

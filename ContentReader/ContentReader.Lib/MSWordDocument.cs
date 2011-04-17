using System;
using Microsoft.Office.Interop.Word;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// This class manipulates an MS-Word Document.
    /// </summary>
    [SupportedDocTypesAtrribute("doc", "docx", "rtf", "docm", "dot", "dotx", "dotm", "txt", "html", "htm")]
    public class MSWordDocument : IDocument, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// a reference to the Word Application owning this document
        /// </summary>
        private static global::Microsoft.Office.Interop.Word.Application app = null;

        /// <summary>
        /// Determines whether the MS-Word Application object is created internally by this object,
        /// or provided externally (e.g. via an MS-Word Addin).
        /// </summary>
        private static bool hasInternalApp = true;

        /// <summary>
        /// a reference to the Word Document of the Microsoft Object Model,
        /// corresponding to this object of the MSWordDocument class.
        /// </summary>
        private Document currentDoc = null;

        /// <summary>
        /// Determines whether the MS-Word Document object is created internally by this object,
        /// or provided externally (e.g. via an MS-Word Addin).
        /// </summary>
        private bool hasInternalDoc = false;

        /// <summary>
        /// Gets a reference to the Word Document of the Microsoft Object Model,
        /// corresponding to this object of the MSWordDocument class.
        /// </summary>
        public Document CurrentMSDocument
        {
            get { return currentDoc; }
        }

        #endregion

        #region Ctors & Dtors

        /// <summary>
        /// creates an instance of this class by loading a document from file specified by the given path.
        /// </summary>
        public MSWordDocument(string documentPath) : base(documentPath)
        {
        }

        /// <summary>
        /// Creates an instance of this class by providing a reference to the 
        /// Word Document of the Microsoft Object Model, corresponding to this object 
        /// of the MSWordDocument class.
        /// This approach is used when working with Microsoft Word Addins.
        /// </summary>
        public MSWordDocument(global::Microsoft.Office.Interop.Word.Document document)
        {
            currentDoc = document;
            hasInternalDoc = false;
            app = document.Application;
            hasInternalApp = false;

            // prevent loading a file by clients of this class by calling LoadDocument of the base class
            IsLoaded = true;
        }

        /// <summary>
        /// Loads the file. This method is protected, thus clients should call 
        /// LoadDocument method of the IDocument abstract class.
        /// </summary>
        protected override bool Load(string documentPath)
        {
            if (app == null)
            {
                app = new global::Microsoft.Office.Interop.Word.ApplicationClass();
                app.Visible = false;
                hasInternalApp = true;
            }

            object oFileName = documentPath;
            object nullobj = System.Reflection.Missing.Value;

            // TODO: If its already loaded ?!

            try
            {
                currentDoc = app.Documents.Open(ref oFileName, ref nullobj, ref nullobj, ref nullobj,
                                ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj);
                hasInternalDoc = true;
            }
            catch (Exception)
            {
                // TODO: Consider this exception
                // sina: How about only returning false, and not throwing any exceptions?
                throw;
            }

            IsLoaded = true;
            return true;
        }

        /// <summary>
        /// Destructor of the class :)
        /// </summary>
        ~MSWordDocument()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// returns an IBlock reference to the whole content of the document.
        /// thus the BlockType will be Everything.
        /// </summary>
        public override IBlock GetContent()
        {
            return new MSWordBlock(this, currentDoc.Content, BlockType.Everything);
        }

        #endregion

        #region IDisposable Members and Disposing Related Stuff

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // dispose managed resources
                    // e.g. call their Dispose()
                }
                // now dispose unmanaged resources
                // e.g. close files or database connections

                object nullobj = System.Reflection.Missing.Value;

                if (hasInternalDoc && currentDoc != null)
                {
                    try
                    {
                        //object saveChanges = false;
                        currentDoc.Close(ref nullobj, ref nullobj, ref nullobj);
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                    finally // FIXME: sina: finally block added by me, but not sure if it's correct
                    {
                        currentDoc = null;
                        hasInternalDoc = false;
                    }
                }

                if (hasInternalApp && app.Documents.Count == 0)
                {
                    try
                    {
                        app.Quit(ref nullobj, ref nullobj, ref nullobj);
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                    finally
                    {
                        app = null;
                        hasInternalApp = false;
                    }
                }

                disposed = true;
            }
        }

        private bool disposed;

        /// <summary>
        /// This method is called by DocFactory's Dispose mthod.
        /// </summary>
        internal static void Cleanup()
        {
            try
            {
                object nullobj = System.Reflection.Missing.Value;
                foreach (Document doc in app.Documents)
                {
                    doc.Close(ref nullobj, ref nullobj, ref nullobj);
                }
                if (hasInternalApp)
                    app.Quit(ref nullobj, ref nullobj, ref nullobj);
            }
            catch (Exception)
            {
            }
            finally
            {
                app = null;
                hasInternalApp = false;
            }
        }

        #endregion
    }
}

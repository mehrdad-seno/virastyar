using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// this class is responsible for opening documents based upon their extensions, 
    /// and reclaiming their resources in the end.
    /// </summary>
    [Obsolete("Use DocumentUtils instead!", true)]
    public class DocFactory : IDisposable
    {
        /// <summary>
        /// A dictionary of extension-to-document-class-type, which specifies which file extension 
        /// should be opened by which document class (derived form IDocument).
        /// </summary>
        private static readonly Dictionary<string, Type> DocTypeHandlers = new Dictionary<string,Type>();

        /// <summary>
        /// static constructor which fills the dictionary of different file extensions.
        /// </summary>
        static DocFactory()
        {
            LoadDocTypeHandlers();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocFactory"/> class.
        /// </summary>
        public DocFactory()
        {
            // Nothing To Do
        }

        /// <summary>
        /// Loads all classes attributed by SupportedDocTypesAtrribute through reflection.
        /// Then creates a dictionary of extensions to document class type, which is responsible to load 
        /// documents of that extension.
        /// </summary>
        private static void LoadDocTypeHandlers()
        {
            // TODO: We may want to load handlers from other assemblies, too
            // TODO: What happens if 2 different classes handle the same document type ?! --> Exception

            Assembly assmbl = Assembly.GetAssembly(typeof(DocFactory));
            Type[] types = assmbl.GetTypes();

            foreach (Type type in types)
            {
                SupportedDocTypesAtrribute[] supportedTypes = 
                    type.GetCustomAttributes(typeof(SupportedDocTypesAtrribute), false) 
                    as SupportedDocTypesAtrribute[];

                Debug.Assert(supportedTypes != null);

                if (supportedTypes.Length == 0)
                    continue;

                foreach (string docType in supportedTypes[0].SupportedTypes)
	            {
                    DocTypeHandlers.Add(docType, type);
                }
            }
        }

        /// <summary>
        /// Finds the proper document class according to the file extension, then opens the file by
        /// the found document class, and returns an (IDocument) reference to the document object.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IDocument LoadDocument(string filePath)
        {
            string fileExtention = Path.GetExtension(filePath).Remove(0, 1);
            Type handlerType;
            try
            {
                handlerType = DocTypeHandlers[fileExtention];
            }
            catch (Exception)
            {
                return null;
            }

            ConstructorInfo ctorInfo = handlerType.GetConstructor(new Type[] { typeof(string) });
            Debug.Assert(ctorInfo != null);
            IDocument doc = (IDocument)ctorInfo.Invoke(new object[] { filePath });
            Debug.Assert(doc != null);
            return doc;
        }

        /// <summary>
        /// Gets a list of all extensions supported by all document classes 
        /// (derived from IDocument) in this assembly.
        /// </summary>
        public static List<string> SupportedFileTypes
        {
            get
            {
                List<string> list = new List<string>();
                list.AddRange(DocTypeHandlers.Keys);
                return list;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DocFactory"/> is reclaimed by garbage collection.
        /// </summary>
// ReSharper disable EmptyDestructor
        ~DocFactory()
// ReSharper restore EmptyDestructor
        {
            // TODO: @sina: Why doesn't it call Dispose(false); ????
        }

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
            if (!m_disposed)
            {
                if (disposing)
                {
                    // dispose managed resources
                    // e.g. call their Dispose()
                }

                // now dispose unmanaged resources
                // e.g. close files or database connections

                MSWordDocument.Cleanup();

                m_disposed = true;
            }
        }

        private bool m_disposed;

        #endregion
    }
}

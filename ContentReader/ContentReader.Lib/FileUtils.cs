using System.Collections.Generic;
using System.IO;

namespace SCICT.Microsoft.Office.Word.ContentReader.Shared
{
    /// <summary>
    /// FileUtils Class provides some static utilities for accessing files, in a directory structure.
    /// This class has no usage in the whole ContentReader Library, but since the clients
    /// (i.e. PersianContentReader.Console, and PersianContentReader.UI projects) use this 
    /// class it is placed in the PeresianContentReader.Lib project, so that 
    /// it is shared with the clients also.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// returns a list of file names in a directory structure 
        /// with extensions provided by DocFactory.
        /// </summary>
        public static List<string> GetAllProcessableFiles(string directory, bool includeSubDirs)
        {
            List<string> result = new List<string>();

            List<string> supportedTypes = DocFactory.SupportedFileTypes;

            foreach (string docType in supportedTypes)
            {
                result.AddRange(GetAllProcessableFiles(directory, includeSubDirs, "*." + docType));
            }

            return result;
        }

        /// <summary>
        /// returns a list of file names in a directory structure 
        /// with a given naming pattern.
        /// </summary>
        public static List<string> GetAllProcessableFiles(string directory, bool includeSubDirs, string pattern)
        {
            List<string> result = new List<string>();
            string[] directories = Directory.GetDirectories(directory);

            if (includeSubDirs)
            {
                foreach (string dir in directories)
                {
                    result.AddRange(GetAllProcessableFiles(dir, true, pattern));
                }
            }

            result.AddRange(Directory.GetFiles(directory, pattern));

            return result;
        }
    }
}

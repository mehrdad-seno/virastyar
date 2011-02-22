// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

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

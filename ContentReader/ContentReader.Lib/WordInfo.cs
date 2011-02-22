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

namespace SCICT.Microsoft.Office.Word.ContentReader.Shared
{
    /// <summary>
    /// WordInfo Class that encapsualtes some information about words, to be used for statiscal purposes.
    /// This class has no usage in the whole ContentReader Library, but since the clients
    /// (i.e. PersianContentReader.Console, and PersianContentReader.UI projects) use this 
    /// class extensively it is placed in the PeresianContentReader.Lib project, so that 
    /// it is shared with the clients also.
    /// </summary>
    public class WordInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordInfo"/> class.
        /// </summary>
        public WordInfo() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WordInfo"/> class.
        /// </summary>
        /// <param name="noSpace">word without space.</param>
        /// <param name="noErab">The word without erab.</param>
        /// <param name="count">The Count of the word.</param>
        public WordInfo(string noSpace, string noErab, int count)
        {
            WordNoSpace = noSpace;
            WordNoErab = noErab;
            this.Count = count;
        }

        /// <summary>
        /// The word without spaces
        /// </summary>
        public string WordNoSpace;
        
        /// <summary>
        /// The word without erabs
        /// </summary>
        public string WordNoErab;

        /// <summary>
        /// The Count of the words
        /// </summary>
        public int Count;
    }
}

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

using System;
using System.Collections.Generic;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// Atrribute class that specifies which IDocument derivatives support which file extensions.
    /// For a usage example see: MSWordDocument class.
    /// </summary>
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

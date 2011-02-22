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

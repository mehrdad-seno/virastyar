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
using System.Text;
using VirastyarWordAddin.Properties;
using SCICT.NLP.Persian;

namespace VirastyarWordAddin.Configurations
{
    public class AllCharactersRefinerSettings
    {
        private HashSet<char> ignoreList = new HashSet<char>();
        public FilteringCharacterCategory NotIgnoredCategories { get; set; }
        public bool RefineHalfSpacePositioning { get; set; }

        internal AllCharactersRefinerSettings(Settings settings)
        {
            Reload(settings);
        }

        private AllCharactersRefinerSettings()
        {
            NotIgnoredCategories = 
                FilteringCharacterCategory.ArabicDigit | FilteringCharacterCategory.Erab | 
                FilteringCharacterCategory.HalfSpace | FilteringCharacterCategory.Kaaf | 
                FilteringCharacterCategory.Yaa;

            RefineHalfSpacePositioning = true;
        }

        internal void Reload(Settings settings)
        {
            NotIgnoredCategories = (FilteringCharacterCategory)settings.RefineCategoriesFlag;
            LoadIgnoreListFromString(settings.RefineIgnoreListConcated);
            RefineHalfSpacePositioning = settings.RefineHalfSpacePositioning;
        }

        public HashSet<char> GetIgnoreList()
        {
            return ignoreList;
        }

        public void SetFilteringCategory(FilteringCharacterCategory cat)
        {
            NotIgnoredCategories = (NotIgnoredCategories | cat);
        }

        public void SetOffFilteringCategory(FilteringCharacterCategory cat)
        {
            NotIgnoredCategories = (NotIgnoredCategories & (~cat));
        }

        public bool HasAnyFilteringCategory()
        {
            return (int)NotIgnoredCategories > 0;
        }

        public bool HasAllFilteringCategories()
        {
            return (int)NotIgnoredCategories == 31;
        }

        public FilteringCharacterCategory GetIgnoredCategories()
        {
            // should negate the NotIgnoredCategories
            return (FilteringCharacterCategory)((int)NotIgnoredCategories ^ 31);
        }

        public bool IsEmptyIgnoreList()
        {
            return ignoreList == null || ignoreList.Count <= 0;
        }

        public bool ContainsCharInIgnoreList(char ch)
        {
            return ignoreList.Contains(ch);
        }

        public bool AddCharToIgnoreList(char ch)
        {
            return ignoreList.Add(ch);
        }

        public bool RemoveCharFromIgnoreList(char ch)
        {
            return ignoreList.Remove(ch);
        }

        public string GetIgnoreListAsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in ignoreList)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        public void LoadIgnoreListFromString(string str)
        {
            ignoreList.Clear();
            foreach (char c in str)
            {
                AddCharToIgnoreList(c);
            }
        }
    }
}

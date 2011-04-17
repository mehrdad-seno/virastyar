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

using System.Collections.Generic;
using System.Text;
using VirastyarWordAddin.Properties;
using SCICT.NLP.Persian;

namespace VirastyarWordAddin.Configurations
{
    public class AllCharactersRefinerSettings
    {
        private readonly HashSet<char> m_ignoreList = new HashSet<char>();

        public FilteringCharacterCategory NotIgnoredCategories { get; set; }
        public bool RefineHalfSpacePositioning { get; set; }
        public bool NormalizeHeYe { get; set; }
        public bool ConvertLongHeYeToShort { get; set; }
        public bool ConvertShortHeYeToLong { get; set; }

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
            NormalizeHeYe = true;
            ConvertLongHeYeToShort = false;
            ConvertShortHeYeToLong = true;
        }

        internal void Reload(Settings settings)
        {
            NotIgnoredCategories = (FilteringCharacterCategory)settings.RefineCategoriesFlag;
            LoadIgnoreListFromString(settings.RefineIgnoreListConcated);
            RefineHalfSpacePositioning = settings.RefineHalfSpacePositioning;
            NormalizeHeYe = settings.RefineNormalizeHeYe;
            ConvertLongHeYeToShort = settings.RefineLongHeYeToShort;
            ConvertShortHeYeToLong = settings.RefineShortHeYeToLong;
        }

        public HashSet<char> GetIgnoreList()
        {
            return m_ignoreList;
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
            return m_ignoreList == null || m_ignoreList.Count <= 0;
        }

        public bool ContainsCharInIgnoreList(char ch)
        {
            return m_ignoreList.Contains(ch);
        }

        public bool AddCharToIgnoreList(char ch)
        {
            return m_ignoreList.Add(ch);
        }

        public bool RemoveCharFromIgnoreList(char ch)
        {
            return m_ignoreList.Remove(ch);
        }

        public string GetIgnoreListAsString()
        {
            var sb = new StringBuilder();
            foreach (char c in m_ignoreList)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        public void LoadIgnoreListFromString(string str)
        {
            m_ignoreList.Clear();
            foreach (char c in str)
            {
                AddCharToIgnoreList(c);
            }
        }
    }
}

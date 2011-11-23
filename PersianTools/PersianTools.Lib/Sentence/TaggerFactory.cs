using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace SCICT.NLP.Utility
{
    public class TaggerFactory
    {
        //                                 locale,            tagName
        private static readonly Dictionary<string, Dictionary<string, ITagger>> s_taggers = new Dictionary<string, Dictionary<string, ITagger>>();

        static TaggerFactory()
        {
            RegisterAssembly(typeof(TaggerFactory).Assembly);
        }

        private static void RegisterTagger(string locale, string tagName, ITagger tagger)
        {
            if (IsTagRegistered(locale, tagName))
            {
                throw new Exception(String.Format("{0} and {1} taggers for locale \"{2}\" share common tag name {3}",
                                                  tagger.GetType().Name,
                                                  s_taggers[locale][tagName].GetType().Name, locale,
                                                  tagName));
            }

            locale = locale.ToLower();
            tagName = tagName.ToLower();

            if (!s_taggers.ContainsKey(locale))
                s_taggers.Add(locale, new Dictionary<string, ITagger>());
            
            s_taggers[locale].Add(tagName, tagger);
        }

        public static bool IsTagRegistered(string locale, string tagName)
        {
            locale = locale.ToLower();
            if(s_taggers.ContainsKey(locale.ToLower()))
                return s_taggers[locale].ContainsKey(tagName.ToLower());

            return false;
        }

        public static Dictionary<string, object[]> Tag(string tagName, Sentence sentence)
        {
            if(!IsTagRegistered(sentence.Locale, tagName))
                throw new InvalidOperationException("No tagger is registered for tag-name: " + tagName);

            ITagger tagger = s_taggers[sentence.Locale][tagName.ToLower()];
            return tagger.Tag(sentence);
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            foreach (var taggerType in assembly.GetTypes())
            {
                foreach (var iface in taggerType.GetInterfaces())
                {
                    if (iface == typeof(ITagger) && !taggerType.IsAbstract)
                    {
                        var tagger = Activator.CreateInstance(taggerType) as ITagger;
                        Debug.Assert(tagger != null);
                        var tagNames = tagger.SupportedTagNames;
                        Debug.Assert(tagNames != null && tagNames.Length > 0);
                        string locale = tagger.Locale;
                        Debug.Assert(!String.IsNullOrEmpty(locale));

                        foreach (string tagName in tagNames)
                        {
                            RegisterTagger(locale, tagName, tagger);
                        }

                        break;
                    }
                }
            } 
        }

    }
}

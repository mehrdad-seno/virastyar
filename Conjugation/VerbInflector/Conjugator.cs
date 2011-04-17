using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SCICT.Utility;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{
    public class Conjugator
    {
        private static List<string> GetGozashtehNaghliSadehInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbInflection = new VerbConjugationElement(inflection.VerbStem, ZamirPeyvastehType.ZamirPeyvasteh_NONE,
                                                    ShakhsType.Shakhs_NONE, TenseFormationType.PAYEH_MAFOOLI,
                                                    inflection.Positivity);
            var tempLst = GetPayehFelInflections(verbInflection);
            string fel = tempLst[0];
            switch (inflection.Shakhs)
            {
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    fel += "‌اند";
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    fel += "‌ای";
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    fel += "‌اید";
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    fel += "‌ام";
                    break;
                case ShakhsType.AVALSHAKHS_JAM:
                    fel += "‌ایم";
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(fel, inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetGozashtehNaghliEstemraiSadehInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder = new StringBuilder();
            verbBuilder.Append(inflection.VerbStem.Pishvand);
            switch (inflection.Positivity)
            {
                case TensePositivity.POSITIVE:
                    verbBuilder.Append("می‌");
                    break;
                case TensePositivity.NEGATIVE:
                    verbBuilder.Append("نمی‌");
                    break;
            }
            var verb = new Verb("", inflection.VerbStem.HastehMazi,
                                 inflection.VerbStem.HastehMozareh, "",
                                 "", inflection.VerbStem.Transitivity, VerbType.SADEH,
                                 inflection.VerbStem.AmrShodani);
            var verbInflection = new VerbConjugationElement(verb, ZamirPeyvastehType.ZamirPeyvasteh_NONE,
                                                    ShakhsType.Shakhs_NONE, TenseFormationType.PAYEH_MAFOOLI,
                                                    TensePositivity.POSITIVE);
            var tempLst = GetPayehFelInflections(verbInflection);
            verbBuilder.Append(tempLst[0]);
            switch (inflection.Shakhs)
            {
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    verbBuilder.Append("‌اند");
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("‌ای");
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    verbBuilder.Append("‌اید");
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    verbBuilder.Append("‌ام");
                    break;
                case ShakhsType.AVALSHAKHS_JAM:
                    verbBuilder.Append("‌ایم");
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetGozashtehEstemrariInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder = new StringBuilder();
            verbBuilder.Append(inflection.VerbStem.Pishvand);
            switch (inflection.Positivity)
            {
                case TensePositivity.POSITIVE:
                    verbBuilder.Append("می‌" + inflection.VerbStem.HastehMazi);
                    break;
                case TensePositivity.NEGATIVE:
                    verbBuilder.Append("نمی‌" + inflection.VerbStem.HastehMazi);
                    break;
            }
            if (inflection.VerbStem.HastehMazi.EndsWith("آ"))
            {
                verbBuilder.Remove(verbBuilder.Length - 1, 1);
                verbBuilder.Append("ی");
            }
            else if (inflection.VerbStem.HastehMazi.EndsWith("ا") || inflection.VerbStem.HastehMazi.EndsWith("و"))
            {
                verbBuilder.Append("ی");
            }
            switch (inflection.Shakhs)
            {
                case ShakhsType.AVALSHAKHS_JAM:
                    verbBuilder.Append("یم");
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    verbBuilder.Append("م");
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    verbBuilder.Append("ید");
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("ی");
                    break;
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    verbBuilder.Append("ند");
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetGozashtehSadehInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder = new StringBuilder();
            verbBuilder.Append(inflection.VerbStem.Pishvand);
            if (inflection.Positivity == TensePositivity.NEGATIVE)
            {
                verbBuilder.Append("ن");
            }

            if (inflection.VerbStem.HastehMazi.StartsWith("ا") && inflection.Positivity == TensePositivity.NEGATIVE)
            {
                verbBuilder.Append("ی");
                verbBuilder.Append(inflection.VerbStem.HastehMazi);
            }
            else if (inflection.VerbStem.HastehMazi.StartsWith("آ") && inflection.Positivity == TensePositivity.NEGATIVE)
            {
                verbBuilder.Append("یا");
                verbBuilder.Append(inflection.VerbStem.HastehMazi.Remove(0, 1));
            }
            else
            {
                verbBuilder.Append(inflection.VerbStem.HastehMazi);
            }

            if (inflection.VerbStem.HastehMazi.EndsWith("آ"))
            {
                verbBuilder.Remove(verbBuilder.Length - 1, 1);
                verbBuilder.Append("ی");
            }
            else if (inflection.VerbStem.HastehMazi.EndsWith("ا") || inflection.VerbStem.HastehMazi.EndsWith("و"))
            {
                verbBuilder.Append("ی");
            }
            switch (inflection.Shakhs)
            {
                case ShakhsType.AVALSHAKHS_JAM:
                    verbBuilder.Append("یم");
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    verbBuilder.Append("م");
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    verbBuilder.Append("ید");
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("ی");
                    break;
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    verbBuilder.Append("ند");
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetHaalSaadehEkhbaariInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder = new StringBuilder();
            verbBuilder.Append(inflection.VerbStem.Pishvand);
            if (inflection.VerbStem.HastehMazi != "داشت")
            {
                switch (inflection.Positivity)
                {
                    case TensePositivity.POSITIVE:
                        verbBuilder.Append("می‌" + inflection.VerbStem.HastehMozareh);
                        break;
                    case TensePositivity.NEGATIVE:
                        verbBuilder.Append("نمی‌" + inflection.VerbStem.HastehMozareh);
                        break;
                }
            }
            else
            {
                verbBuilder.Append(inflection.VerbStem.HastehMozareh);
            }
            if (inflection.VerbStem.HastehMozareh.EndsWith("آ"))
            {
                //verbBuilder.Remove(verbBuilder.Length - 1, 1);
                verbBuilder.Append("ی");
            }
            else if (inflection.VerbStem.HastehMozareh.EndsWith("ا") || inflection.VerbStem.HastehMozareh.EndsWith("و"))
            {
                verbBuilder.Append("ی");
            }
            switch (inflection.Shakhs)
            {
                case ShakhsType.AVALSHAKHS_JAM:
                    verbBuilder.Append("یم");
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    verbBuilder.Append("م");
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    verbBuilder.Append("ید");
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("ی");
                    break;
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    verbBuilder.Append("ند");
                    break;
                case ShakhsType.SEVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("د");
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetHaalEltezamiInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder = new StringBuilder();
            verbBuilder.Append(inflection.VerbStem.Pishvand);

            switch (inflection.Positivity)
            {
                case TensePositivity.POSITIVE:
                    verbBuilder.Append("ب");
                    break;
                case TensePositivity.NEGATIVE:
                    verbBuilder.Append("ن");
                    break;
            }
            if (inflection.VerbStem.HastehMozareh.StartsWith("ا"))
            {
                verbBuilder.Append("ی");
                verbBuilder.Append(inflection.VerbStem.HastehMozareh);
            }
            else if (inflection.VerbStem.HastehMozareh.StartsWith("آ"))
            {
                verbBuilder.Append("یا");
                verbBuilder.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
            }
            else
            {
                verbBuilder.Append(inflection.VerbStem.HastehMozareh);
            }

            if (inflection.VerbStem.HastehMozareh.EndsWith("آ"))
            {
                verbBuilder.Remove(verbBuilder.Length - 1, 1);
                verbBuilder.Append("ای");
            }
            else if (inflection.VerbStem.HastehMozareh.EndsWith("ا") || inflection.VerbStem.HastehMozareh.EndsWith("و"))
            {
                verbBuilder.Append("ی");
            }
            switch (inflection.Shakhs)
            {
                case ShakhsType.AVALSHAKHS_JAM:
                    verbBuilder.Append("یم");
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    verbBuilder.Append("م");
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    verbBuilder.Append("ید");
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("ی");
                    break;
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    verbBuilder.Append("ند");
                    break;
                case ShakhsType.SEVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("د");
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetHaalSaadehInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder = new StringBuilder();
            verbBuilder.Append(inflection.VerbStem.Pishvand);
            verbBuilder.Append(inflection.VerbStem.HastehMozareh);
            //switch (inflection.Positivity)
            //{
            //    case TensePositivity.POSITIVE:
            //        verbBuilder.Append("می‌" + inflection.VerbStem.HastehMozareh);
            //        break;
            //    case TensePositivity.NEGATIVE:
            //        verbBuilder.Append("نمی‌" + inflection.VerbStem.HastehMozareh);
            //        break;
            //}
            if (inflection.VerbStem.HastehMozareh.EndsWith("آ"))
            {
                verbBuilder.Remove(verbBuilder.Length - 1, 1);
                verbBuilder.Append("ای");
            }
            else if (inflection.VerbStem.HastehMozareh.EndsWith("ا") || inflection.VerbStem.HastehMozareh.EndsWith("و"))
            {
                verbBuilder.Append("ی");
            }
            switch (inflection.Shakhs)
            {
                case ShakhsType.AVALSHAKHS_JAM:
                    verbBuilder.Append("یم");
                    break;
                case ShakhsType.AVALSHAKHS_MOFRAD:
                    verbBuilder.Append("م");
                    break;
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    verbBuilder.Append("ید");
                    break;
                case ShakhsType.DOVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("ی");
                    break;
                case ShakhsType.SEVVOMSHAKHS_JAM:
                    verbBuilder.Append("ند");
                    break;
                case ShakhsType.SEVVOMSHAKHS_MOFRAD:
                    verbBuilder.Append("د");
                    break;
            }
            lstInflections.Add(addZamirPeyvasteh(verbBuilder.ToString(), inflection.ZamirPeyvasteh));
            return lstInflections;
        }
        private static List<string> GetAmrInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            var verbBuilder1 = new StringBuilder();
            var verbBuilder2 = new StringBuilder();
            var verbBuilder3 = new StringBuilder();
            if (inflection.VerbStem.Pishvand != "")
            {
                verbBuilder1.Append(inflection.VerbStem.Pishvand);
                if (inflection.Positivity == TensePositivity.NEGATIVE)
                {
                    verbBuilder3.Append(inflection.VerbStem.Pishvand);
                }
                if (inflection.Positivity == TensePositivity.POSITIVE)
                {
                    verbBuilder2.Append(inflection.VerbStem.Pishvand);
                }
            }
            switch (inflection.Positivity)
            {
                case TensePositivity.POSITIVE:
                    verbBuilder1.Append("ب");
                    break;
                case TensePositivity.NEGATIVE:
                    verbBuilder1.Append("ن");
                    verbBuilder3.Append("م");
                    break;
            }
            if (inflection.VerbStem.HastehMozareh.StartsWith("ا"))
            {
                verbBuilder1.Append("ی");
                verbBuilder1.Append(inflection.VerbStem.HastehMozareh);
                if (inflection.Positivity == TensePositivity.NEGATIVE)
                {
                    verbBuilder3.Append("ی");
                    verbBuilder3.Append(inflection.VerbStem.HastehMozareh);
                }
                if (inflection.Positivity == TensePositivity.POSITIVE)
                {
                    verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
                }
            }
            else if (inflection.VerbStem.HastehMozareh.StartsWith("آ"))
            {
                verbBuilder1.Append("یا");
                verbBuilder1.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
                if (inflection.Positivity == TensePositivity.NEGATIVE)
                {
                    verbBuilder3.Append("یا");
                    verbBuilder3.Append(inflection.VerbStem.HastehMozareh.Remove(0, 1));
                }
                if (inflection.Positivity == TensePositivity.POSITIVE)
                {
                    verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
                }
            }
            else
            {
                verbBuilder1.Append(inflection.VerbStem.HastehMozareh);
                if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Pishvand != "")
                {
                    verbBuilder2.Append(inflection.VerbStem.HastehMozareh);
                }
                if (inflection.Positivity == TensePositivity.NEGATIVE)
                {
                    verbBuilder3.Append(inflection.VerbStem.HastehMozareh);
                }
            }

            switch (inflection.Shakhs)
            {
                case ShakhsType.DOVVOMSHAKHS_JAM:
                    if (inflection.VerbStem.HastehMozareh.EndsWith("ا") || inflection.VerbStem.HastehMozareh.EndsWith("آ") || inflection.VerbStem.HastehMozareh.EndsWith("و"))
                    {
                        verbBuilder1.Append("یید");
                        if (inflection.Positivity == TensePositivity.NEGATIVE)
                        {
                            verbBuilder3.Append("یید");
                        }
                        if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Pishvand != "")
                        {
                            verbBuilder2.Append("یید");
                        }
                    }
                    else
                    {
                        verbBuilder1.Append("ید");
                        if (inflection.Positivity == TensePositivity.NEGATIVE)
                        {
                            verbBuilder3.Append("ید");
                        }
                        if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Pishvand != "")
                        {
                            verbBuilder2.Append("ید");
                        }
                    }
                    break;
            }
            if (inflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
            {
                if (!(inflection.VerbStem.HastehMozareh == "نه" && inflection.Positivity == TensePositivity.NEGATIVE))
                    lstInflections.Add(verbBuilder1.ToString());
                if (inflection.Positivity == TensePositivity.NEGATIVE)
                    lstInflections.Add(verbBuilder3.ToString());
                if (inflection.Positivity == TensePositivity.POSITIVE && inflection.VerbStem.Pishvand != "")
                    lstInflections.Add(verbBuilder2.ToString());
                if (inflection.VerbStem.Type == VerbType.PISHVANDI && inflection.Positivity == TensePositivity.POSITIVE &&
                    (inflection.VerbStem.HastehMozareh.EndsWith("و") || inflection.VerbStem.HastehMozareh.EndsWith("آ")))
                {
                    lstInflections.Add(verbBuilder2.Append("ی").ToString());
                }
            }
            else
            {
                if (!(inflection.VerbStem.HastehMozareh == "نه" && inflection.Positivity == TensePositivity.NEGATIVE))
                    lstInflections.Add(addZamirPeyvasteh(verbBuilder1.ToString(), inflection.ZamirPeyvasteh));
                if (inflection.Positivity == TensePositivity.NEGATIVE)
                    lstInflections.Add(addZamirPeyvasteh(verbBuilder3.ToString(), inflection.ZamirPeyvasteh));
                if (inflection.VerbStem.Type == VerbType.PISHVANDI && inflection.Positivity == TensePositivity.POSITIVE)
                    lstInflections.Add(addZamirPeyvasteh(verbBuilder2.ToString(), inflection.ZamirPeyvasteh));
                if (inflection.VerbStem.Type == VerbType.PISHVANDI && inflection.Positivity == TensePositivity.POSITIVE &&
                    (inflection.VerbStem.HastehMozareh.EndsWith("و") || inflection.VerbStem.HastehMozareh.EndsWith("آ")) && inflection.ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE)
                {
                    lstInflections.Add(verbBuilder2.Append("ی").ToString());
                }
            }
            return lstInflections;
        }
        private static List<string> GetPayehFelInflections(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            switch (inflection.Positivity)
            {
                case TensePositivity.POSITIVE:
                    lstInflections.Add(inflection.VerbStem.Pishvand + inflection.VerbStem.HastehMazi + "ه");
                    break;
                case TensePositivity.NEGATIVE:
                    if (inflection.VerbStem.HastehMazi.StartsWith("آ") || inflection.VerbStem.HastehMazi.StartsWith("ا"))
                    {
                        string verb = inflection.VerbStem.Pishvand + "نیا" + inflection.VerbStem.HastehMazi.Remove(0, 1) + "ه";
                        lstInflections.Add(verb);
                    }
                    else
                    {
                        lstInflections.Add(inflection.VerbStem.Pishvand + "ن" + inflection.VerbStem.HastehMazi + "ه");
                    }
                    break;
            }
            return lstInflections;
        }

        private static string addZamirPeyvasteh(string verb, ZamirPeyvastehType zamirPeyvastehType)
        {
            string inflectedVerb = verb;
            switch (zamirPeyvastehType)
            {
                case ZamirPeyvastehType.SEVVOMSHAKHS_MOFRAD:
                    if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
                    {
                        inflectedVerb += "یش";
                    }
                    else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
                    {
                        inflectedVerb += "‌اش";
                    }
                    else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
                    {
                        inflectedVerb += "‌اش";
                    }
                    else
                    {
                        inflectedVerb += "ش";
                    }
                    break;
                case ZamirPeyvastehType.SEVVOMSHAKHS_JAM:
                    if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
                    {
                        inflectedVerb += "یشان";
                    }
                    else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
                    {
                        inflectedVerb += "‌شان";
                    }
                    //else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
                    //{
                    //    inflectedVerb += "‌شان";

                    //}
                    else
                    {
                        inflectedVerb += "شان";
                    }
                    break;
                case ZamirPeyvastehType.DOVVOMSHAKHS_JAM:
                    if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
                    {
                        inflectedVerb += "یتان";
                    }
                    else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
                    {
                        inflectedVerb += "‌تان";
                    }
                    //else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
                    //{
                    //    inflectedVerb += "‌تان";
                    //}
                    else
                    {
                        inflectedVerb += "تان";
                    } break;
                case ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD:
                    if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
                    {
                        inflectedVerb += "یت";
                    }
                    else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
                    {
                        inflectedVerb += "‌ات";
                    }
                    else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
                    {
                        inflectedVerb += "‌ات";
                    }
                    else
                    {
                        inflectedVerb += "ت";
                    }
                    break;
                case ZamirPeyvastehType.AVALSHAKHS_JAM:
                    if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
                    {
                        inflectedVerb += "یمان";
                    }
                    else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
                    {
                        inflectedVerb += "‌مان";
                    }
                    //else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
                    //{
                    //    inflectedVerb += "‌مان";
                    //}
                    else
                    {
                        inflectedVerb += "مان";
                    }
                    break;
                case ZamirPeyvastehType.AVALSHAKHS_MOFRAD:
                    if (verb.EndsWith("آ") || verb.EndsWith("ا") || verb.EndsWith("و"))
                    {
                        inflectedVerb += "یم";
                    }
                    else if (verb.EndsWith("ه") && !verb.EndsWith("اه") && !verb.EndsWith("وه"))
                    {
                        inflectedVerb += "‌ام";
                    }
                    else if (verb.EndsWith("ی") && !verb.EndsWith("ای") && !verb.EndsWith("وی"))
                    {
                        inflectedVerb += "‌ام";
                    }
                    else
                    {
                        inflectedVerb += "م";
                    }
                    break;
            }
            return inflectedVerb;
        }

        private static string[] GetVerbConjugations(string pishvand, string bonMazi, string bonMozareh, bool trnst, bool amrshodani, bool objectivePronoun)
        {
            List<string> strList = new List<string>();

            var vrb = new Verb("", bonMazi, bonMozareh, pishvand, "", trnst == true ? VerbTransitivity.GOZARA : VerbTransitivity.NAGOZAR,
                               pishvand == "" ? VerbType.SADEH : VerbType.PISHVANDI, amrshodani);

            foreach (TensePositivity positivity in Enum.GetValues(typeof(TensePositivity)))
            {
                foreach (ShakhsType shakhsType in Enum.GetValues(typeof(ShakhsType)))
                {
                    VerbConjugationElement inflection = null;
                    foreach (
                        TenseFormationType tenseFormationType in
                            Enum.GetValues(typeof(TenseFormationType)))
                    {
                        if (objectivePronoun)
                        {
                            foreach (
                                ZamirPeyvastehType zamirPeyvastehType in
                                    Enum.GetValues(typeof(ZamirPeyvastehType)))
                            {
                                inflection = new VerbConjugationElement(vrb, zamirPeyvastehType,
                                                                    shakhsType,
                                                                    tenseFormationType, positivity);

                                if (inflection.IsValid())
                                {
                                    var output = GetConjugates(inflection);
                                    foreach (string s in output)
                                    {
                                        strList.Add(s);
                                    }
                                }
                            }
                        }
                        else
                        {
                            inflection = new VerbConjugationElement(vrb, ZamirPeyvastehType.ZamirPeyvasteh_NONE,
                                    shakhsType,
                                    tenseFormationType, positivity);

                            if (inflection.IsValid())
                            {
                                var output = GetConjugates(inflection);
                                foreach (string s in output)
                                {
                                    strList.Add(s);
                                }
                            }
                        }
                    }
                }
            }
            return strList.Distinct().ToArray();
        }

        private static string[] GetConjugates(VerbConjugationElement inflection)
        {
            var lstInflections = new List<string>();
            switch (inflection.TenseForm)
            {
                case TenseFormationType.AMR:
                    lstInflections = GetAmrInflections(inflection);
                    break;
                case TenseFormationType.GOZASHTEH_ESTEMRAARI:
                    lstInflections = GetGozashtehEstemrariInflections(inflection);
                    break;
                case TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI:
                    lstInflections = GetGozashtehNaghliEstemraiSadehInflections(inflection);
                    break;
                case TenseFormationType.GOZASHTEH_NAGHLI_SADEH:
                    lstInflections = GetGozashtehNaghliSadehInflections(inflection);
                    break;
                case TenseFormationType.GOZASHTEH_SADEH:
                    lstInflections = GetGozashtehSadehInflections(inflection);
                    break;
                case TenseFormationType.HAAL_ELTEZAMI:
                    lstInflections = GetHaalEltezamiInflections(inflection);
                    break;
                case TenseFormationType.HAL_SAADEH_EKHBARI:
                    lstInflections = GetHaalSaadehEkhbaariInflections(inflection);
                    break;
                case TenseFormationType.PAYEH_MAFOOLI:
                    lstInflections = GetPayehFelInflections(inflection);
                    break;
            }
            return lstInflections.Distinct().ToArray();
        }

        public static Verb[] ExtractVerbsFromFile(string fileName)
        {
            List<Verb> verbs = new List<Verb>();

            string[] records = File.ReadAllText(fileName).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int vtype;
            foreach (string recordStr in records)
            {
                string[] fields = recordStr.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (!int.TryParse(fields[0], out vtype))
                {
                    continue;
                }

                VerbType verbType = VerbType.SADEH;
                if (vtype == 1)
                    verbType = VerbType.SADEH;

                if (vtype == 2)
                    verbType = VerbType.PISHVANDI;

                int trans = int.Parse(fields[1]);
                VerbTransitivity transitivity = VerbTransitivity.GOZARA;
                if (trans == 0)
                    transitivity = VerbTransitivity.NAGOZAR;
                else if (trans == 2)
                    transitivity = VerbTransitivity.DOVAJHI;

                string pishvand = "";
                if (fields[5] != "-")
                    pishvand = fields[5];

                Verb verb;
                bool amrShodani = true;
                if (fields.Length == 8)
                    if (fields[7] == "*")
                        amrShodani = false;
                if (fields[3] != "-")
                    verb = new Verb("", fields[2], fields[3], pishvand, "", transitivity, verbType, amrShodani);
                else
                    verb = new Verb("", fields[2], null, pishvand, "", transitivity, verbType, amrShodani);

                verbs.Add(verb);
            }

            return verbs.ToArray();
        }

        public static string[] GetAllConjugations(Verb[] verbs, VerbType verbType, bool objectivePronoun)
        {
            List<string> results = new List<string>();

            foreach (Verb verb in verbs)
            {
                if (verbType.Has(VerbType.PISHVANDI))
                {
                    if (verb.Type.Is(VerbType.PISHVANDI))
                    {
                        results.AddRange(GetVerbConjugations(verb.Pishvand, verb.HastehMazi, verb.HastehMozareh, verb.Transitivity == VerbTransitivity.GOZARA ? true : false, verb.AmrShodani, objectivePronoun));
                    }
                }
                if (verbType.Has(VerbType.SADEH))
                {
                    if (verb.Type.Is(VerbType.SADEH))
                    {
                        results.AddRange(GetVerbConjugations("", verb.HastehMazi, verb.HastehMozareh, verb.Transitivity == VerbTransitivity.GOZARA ? true : false, true, objectivePronoun));
                    }
                }
            }

            return results.Distinct().ToArray();
        }

        public static string[] GetAllInfinitives(Verb[] verbs)
        {
            List<string> results = new List<string>();

            foreach (Verb verb in verbs)
            {
                results.Add(verb.HastehMazi + "ن");
            }

            return results.Distinct().ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SCICT.NLP.Morphology.Inflection;
using SCICT.NLP.Utility;
using SCICT.NLP.Utility.Parsers;

namespace SCICT.NLP.Utility.Transliteration
{
    public class Tools
    {
        public static bool IsUpperCase(char ch)
        {
            return ((ch >= 'A' && ch <= 'Z') || ch == '\'');
        }
        public static bool IsLowwerCase(char ch)
        {
            return ((ch >= 'a' && ch <= 'z') || ch == '\'');
        }
        public static bool IsNumber(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }
        public static bool IsUpperCase(string word)
        {
            return word.All(IsUpperCase);
        }
        public static bool IsNumber(string word)
        {
            return word.All(ch => (IsLowwerCase(ch) || ch == '.'));
        }
        public static bool IsLowwerCase(string word)
        {
            return word.All(IsLowwerCase);
        }
        public static bool HaveUpperCase(string word)
        {
            return word.Any(IsUpperCase);
        }
        public static bool HaveLowwerCase(string word)
        {
            return word.Any(IsLowwerCase);
        }

        public static string MapUpperCaseCharToPeString(char ch)
        {
            switch (ch)
            {
                case 'A':
                    return "اِی";
                case 'B':
                    return "بی";
                case 'C':
                    return "سی";
                case 'D':
                    return "دی";
                case 'E':
                    return "ای";
                case 'F':
                    return "اِف";
                case 'G':
                    return "جی";
                case 'H':
                    return "اِچ";
                case 'I':
                    return "آی";
                case 'J':
                    return "جِی";
                case 'K':
                    return "کِی";
                case 'L':
                    return "اِل";
                case 'M':
                    return "اِم";
                case 'N':
                    return "اِن";
                case 'O':
                    return "اُ";
                case 'P':
                    return "پی";
                case 'Q':
                    return "کیو";
                case 'R':
                    return "آر";
                case 'S':
                    return "اِس";
                case 'T':
                    return "تی";
                case 'U':
                    return "یو";
                case 'V':
                    return "وی";
                case 'W':
                    return "دابلیو";
                case 'X':
                    return "اِکس";
                case 'Y':
                    return "وای";
                case 'Z':
                    return "زی";
                case '\'':
                    return "";
                default:
                    throw new Exception("parameter is not uppercase char");
            }
        }
        public static string MapUpperCaseCharToEnString(char ch)
        {
            switch (ch)
            {
                case 'A':
                    return "ey";
                case 'B':
                    return "bi";
                case 'C':
                    return "si";
                case 'D':
                    return "di";
                case 'E':
                    return "ee";
                case 'F':
                    return "ef";
                case 'G':
                    return "ji";
                case 'H':
                    return "ech";
                case 'I':
                    return "ay";
                case 'J':
                    return "jey";
                case 'K':
                    return "key";
                case 'L':
                    return "el";
                case 'M':
                    return "em";
                case 'N':
                    return "en";
                case 'O':
                    return "o";
                case 'P':
                    return "pi";
                case 'Q':
                    return "kio";
                case 'R':
                    return "ar";
                case 'S':
                    return "es";
                case 'T':
                    return "ti";
                case 'U':
                    return "uo";
                case 'V':
                    return "vi";
                case 'W':
                    return "dabeliuo";
                case 'X':
                    return "ex";
                case 'Y':
                    return "vay";
                case 'Z':
                    return "zed";
                case '\'':
                    return "\'";
                default:
                    throw new Exception("parameter is not uppercase char");

            }
        }
        public static string MapUpperCaseStringenToString(string word)
        {

            string result = "";
            foreach (char t in word)
                if (IsUpperCase(t))
                    result += MapUpperCaseCharToEnString(t);
                else
                    result += t;
            return result;
        }
        public static string MapAccronymToString(string word)
        {
            if (!IsUpperCase(word))
                return word;
            string result = "";
            if (word.Length == 0)
                return "";
            result += MapUpperCaseCharToPeString(word[0]);
            for (int i = 1; i < word.Length; i++)
                result += "‌" + MapUpperCaseCharToPeString(word[i]);
            return result;
        }
        public static string MapNumberToString(string word, bool englishPreferd)
        {
            switch (word)
            {
                case "0":
                    return "o";
                case "1":
                    if (englishPreferd)
                        return "van";
                    return "yek";
                case "2":
                    if (englishPreferd)
                        return "too";
                    return "do";
                case "3":
                    if (englishPreferd)
                        return "tiri";
                    return "se";
                case "4":
                    if (englishPreferd)
                        return "for";
                    return "char";
                case "5":
                    if (englishPreferd)
                        return "faiv";
                    return "pang";
                case "6":
                    if (englishPreferd)
                        return "six";
                    return "sh";
                case "7":
                    if (englishPreferd)
                        return "seven";
                    return "haf";
                case "9":
                    if (englishPreferd)
                        return "nain";
                    return "no";
                case "8":
                    if (englishPreferd)
                        return "eyt";
                    return "hash";
                case "10":
                    if (englishPreferd)
                        return "ten";
                    return "da";
                case "30":
                    if (englishPreferd)
                        return "terti";
                    return "si";
                case "40":
                    if (englishPreferd)
                        return "forti";
                    return "chel";
            }
            return null;
        }

        public static List<string> GetTokens(string line)
        {
            if (line == null)
                return new List<string>();

            return new List<string>(line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }
        public static List<string> PartString(string word)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(word))
                return result;

            string current = "";
            bool preState = IsLowwerCase(word[0]) ? true : false;
            foreach (char t in word)
            {
                bool state = IsLowwerCase(t);
                if (preState == state)
                    current += t;
                else
                {
                    preState = state;
                    result.Add(current);
                    current = "";
                    current += t;
                }
            }
            result.Add(current);
            return result;

        }

        public static bool IsFainal(ResultType type)
        {
            if (((type & ResultType.Translate) == ResultType.NoChange) &&
                ((type & ResultType.Transliterate) == ResultType.NoChange) &&
                ((type & ResultType.AcronymeConvert) == ResultType.NoChange))
                return true;
            return false;
        }
        public static void LoadList(ref List<string> list, string path)
        {
            list = new List<string>();
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                list.Add(reader.ReadLine().Split("\t".ToArray())[0]);
            }
            reader.Close();
            reader.Dispose();
        }
        public static bool IsValidInDictionary(string word, List<string> dic, PersianSuffixLemmatizer suffixer, PruneType prouneType)
        {
            if (PruneType.NoPrune == prouneType)
                return false;

            if (dic.Contains(word))
            {
                return true;
            }
            else if (PruneType.Stem == prouneType)
            {
                ReversePatternMatcherPatternInfo[] inf = suffixer.MatchForSuffix(word);
                foreach (ReversePatternMatcherPatternInfo info in inf)
                {
                    if (dic.Contains(info.BaseWord))
                        return true;
                }
            }
            return false;
        }

        public static string NormalizeString(string word)
        {
            string currentWord = StringUtil.RemoveErab(word);

            currentWord = currentWord.Replace("$", "");
            currentWord = currentWord.Replace(new string((char)8204, 1), "");
            return currentWord;
        }
    }
}

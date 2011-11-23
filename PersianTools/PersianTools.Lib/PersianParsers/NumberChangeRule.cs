using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCICT.NLP.Utility.PersianParsers
{
    public class NumberChangeRule
    {
        #region enums
        public enum InputDigitLanguages
        {
            None,
            Persian,
            English,
        }

        public enum InputFormats
        {
            Digits,
            DigitsWithSign,
            Letters
        }

        public enum OutputDigitLanguages
        {
            None,
            Persian,
            English
        }

        public enum OutputFormats
        {
            Fractional,
            FloatingPoint,
            FloatingPointWithSep,
            Letters,
            LettersWithSign
        }
        #endregion

        private readonly Dictionary<KeyValuePair<InputFormats, InputDigitLanguages>, KeyValuePair<OutputFormats, OutputDigitLanguages>> m_dicRules =
            new Dictionary<KeyValuePair<InputFormats, InputDigitLanguages>, KeyValuePair<OutputFormats, OutputDigitLanguages>>();

        public void AddNumberChangeRule(string changeFrom, string changeTo)
        {
            KeyValuePair<InputFormats, InputDigitLanguages> ruleKey;
            KeyValuePair<OutputFormats, OutputDigitLanguages> ruleValue;
            AddNumberChangeRule(changeFrom, changeTo, out ruleKey, out ruleValue);
        }

        public void AddNumberChangeRule(string changeFrom, string changeTo, out KeyValuePair<InputFormats, InputDigitLanguages> ruleKey, out KeyValuePair<OutputFormats, OutputDigitLanguages> ruleValue)
        {
            if (String.IsNullOrEmpty(changeFrom))
                throw new ArgumentNullException("changeFrom", "arguments cannot be null");
            if (String.IsNullOrEmpty(changeTo))
                throw new ArgumentNullException("changeTo", "arguments cannot be null");

            InputDigitLanguages inputDigitLanguage;
            InputFormats inputFormat;
            OutputDigitLanguages outputDigitLanguage;
            OutputFormats outputFormat;

            DectectInputFormat(changeFrom, out inputFormat, out inputDigitLanguage);
            DetectOutputFormat(changeTo, out outputFormat, out outputDigitLanguage);

            ruleKey = new KeyValuePair<InputFormats, InputDigitLanguages>(inputFormat, inputDigitLanguage);
            ruleValue = new KeyValuePair<OutputFormats, OutputDigitLanguages>(outputFormat, outputDigitLanguage);
            if (m_dicRules.ContainsKey(ruleKey))
            {
                m_dicRules[ruleKey] = ruleValue;
            }
            else
            {
                m_dicRules.Add(ruleKey, ruleValue);
            }
        }


        public KeyValuePair<InputFormats, InputDigitLanguages>[] Keys
        {
            get { return m_dicRules.Keys.ToArray(); }
        }

        public bool ContainsKey(InputFormats inpFormat, InputDigitLanguages inpLang)
        {
            return ContainsKey(new KeyValuePair<InputFormats, InputDigitLanguages>(inpFormat, inpLang));
        }

        public bool ContainsKey(KeyValuePair<InputFormats, InputDigitLanguages> key)
        {
            return m_dicRules.ContainsKey(key);
        }

        public KeyValuePair<OutputFormats, OutputDigitLanguages> GetValueForKey(InputFormats inpFormat, InputDigitLanguages inpLang)
        {
            return GetValueForKey(new KeyValuePair<InputFormats, InputDigitLanguages>(inpFormat, inpLang));
        }

        public KeyValuePair<OutputFormats, OutputDigitLanguages> GetValueForKey(KeyValuePair<InputFormats, InputDigitLanguages> key)
        {
            return m_dicRules[key];
        }

        public static void DetectOutputFormat(string str, out OutputFormats outputFormat, out OutputDigitLanguages outputDigitLanguage)
        {
            outputFormat = OutputFormats.LettersWithSign;
            outputDigitLanguage = OutputDigitLanguages.None;

            int len = str.Length;
            if ((str[0] == '-' || str[0] == '+') && (len > 1 && Char.IsLetter(str[1])))
            {
                outputDigitLanguage = OutputDigitLanguages.None;
                outputFormat = OutputFormats.LettersWithSign;
                return;
            }

            for (int i = 0; i < len; i++)
            {
                char ch = str[i];
                if (ch == '/')
                {
                    outputFormat = OutputFormats.Fractional;
                    if (outputDigitLanguage != OutputDigitLanguages.None)
                        break;
                }
                else if (Char.IsLetter(ch) && ch != 'e' && ch != 'E')
                {
                    outputFormat = OutputFormats.Letters;
                    outputDigitLanguage = OutputDigitLanguages.None;
                    break;
                }
                //else if(StringUtil.IsDecimalSeparator(ch))
                //{
                //    outputFormat = OutputFormats.FloatingPoint;
                //    if(outputDigitLanguage != OutputDigitLanguages.None)
                //        break;
                //}
                else if (StringUtil.IsThousandSeparator(ch))
                {
                    outputFormat = OutputFormats.FloatingPointWithSep;
                    if (outputDigitLanguage != OutputDigitLanguages.None)
                        break;
                }
                else if (Char.IsDigit(ch))
                {
                    if (StringUtil.IsPersianDigit(ch))
                    {
                        outputDigitLanguage = OutputDigitLanguages.Persian;
                        if (outputFormat != OutputFormats.LettersWithSign)
                            break;
                    }
                    else
                    {
                        outputDigitLanguage = OutputDigitLanguages.English;
                        if (outputFormat != OutputFormats.LettersWithSign)
                            break;
                    }
                }
            }

            if (outputFormat == OutputFormats.LettersWithSign) // i.e., it was not set in the loop above
            {
                outputFormat = OutputFormats.FloatingPoint;
                Debug.Assert(outputDigitLanguage != OutputDigitLanguages.None);
            }
        }

        public static void DectectInputFormat(string str, out InputFormats inputFormat, out InputDigitLanguages inputDigitLanguage)
        {
            inputFormat = InputFormats.Digits;

            if (str[0] == '-' || str[0] == '+')
                inputFormat = InputFormats.DigitsWithSign;

            inputDigitLanguage = NumberParsersSuggestions.DetectInputDigitLanguage(str);

            if (inputDigitLanguage == InputDigitLanguages.None)
                inputFormat = InputFormats.Letters;

        }

    }

}


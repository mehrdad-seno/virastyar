using System;
using System.Collections.Generic;
using System.Linq;
using SCICT.NLP.Utility.Parsers;
using System.Diagnostics;

namespace SCICT.NLP.Utility.PersianParsers
{
    /// <summary>
    /// Contains utility methods for creating and managing suggestions for number verifiers
    /// </summary>
    public class NumberParsersSuggestions
    {
        /// <summary>
        /// Creates suggestions for the given pattern info
        /// </summary>
        /// <param name="pi">the pattern info object to create suggestions for</param>
        /// <returns></returns>
        public static string[] CreateSuggestions(IPatternInfo pi)
        {
            return CreateSuggestions(null, pi);
        }


        /// <summary>
        /// Creates suggestions for the given pattern info
        /// </summary>
        /// <param name="rule">the change rules that give order to the suggestions</param>
        /// <param name="pi">the pattern info object to create suggestions for</param>
        /// <returns></returns>
        public static string[] CreateSuggestions(NumberChangeRule rule, IPatternInfo pi)
        {
            switch (pi.PatternInfoType)
            {
                case PatternInfoTypes.PersianNumber:
                    return CreateGeneralNumberSuggestions(rule, pi as GeneralNumberInfo);
                case PatternInfoTypes.DigitizedNumber:
                    return CreateDigitizedNumberSuggestions(rule, pi as DigitizedNumberPatternInfo);
                default:
                    return new string[0];
            }
        }

        public static string CreateSuggestionFor(IPatternInfo pi, KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages> ruleValue)
        {
            switch (pi.PatternInfoType)
            {
                case PatternInfoTypes.PersianNumber:
                    return CreateGeneralNumberSuggestionFor(pi as GeneralNumberInfo, ruleValue);
                case PatternInfoTypes.DigitizedNumber:
                    return CreateDigitizedNumberSuggestionFor(pi as DigitizedNumberPatternInfo, ruleValue);
                default:
                    return null;
            }
        }

        private static string CreateDigitizedNumberSuggestionFor(DigitizedNumberPatternInfo pi, KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages> ruleValue)
        {
            if (pi == null)
            {
                return null;
            }

            //string f20NormalizedEng = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
            //string f20CurrencyEng = MathUtils.InsertThousandSeperator(f20NormalizedEng);
            //bool addCurrency = (f20NormalizedEng != f20CurrencyEng);

            //string f20NormPer = ParsingUtils.ConvertNumber2Persian(f20NormalizedEng);
            //string f20CurrencyPer = ParsingUtils.ConvertNumber2Persian(f20CurrencyEng);

            //string perLetterNumber;
            //if (!NumberToPersianString.TryConvertNumberToPersianString(pi.Number, out perLetterNumber))
            //{
            //    perLetterNumber = null;
            //}

            //string perLetterNegNumber = null;
            //if (pi.Content[0] == '-')
            //{
            //    if (!NumberToPersianString.TryConvertNumberToPersianString(-pi.Number, out perLetterNegNumber))
            //    {
            //        perLetterNegNumber = null;
            //    }
            //    else
            //    {
            //        perLetterNegNumber = "-" + perLetterNegNumber;
            //    }
            //}
            //else if (pi.Content[0] == '+' && !String.IsNullOrEmpty(perLetterNumber))
            //{
            //    perLetterNegNumber = "+" + perLetterNumber;
            //}

            // now giving orders
            if ((ruleValue.Value == NumberChangeRule.OutputDigitLanguages.Persian) &&
                    (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPoint || ruleValue.Key == NumberChangeRule.OutputFormats.Fractional))
            {
                string f20NormalizedEng = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
                string f20NormPer = ParsingUtils.ConvertNumber2Persian(f20NormalizedEng);
                return f20NormPer;
            }
            else if ((ruleValue.Value == NumberChangeRule.OutputDigitLanguages.Persian) &&
                (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep))
            {
                string f20NormalizedEng = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
                string f20CurrencyEng = MathUtils.InsertThousandSeperator(f20NormalizedEng);
                string f20CurrencyPer = ParsingUtils.ConvertNumber2Persian(f20CurrencyEng);
                return f20CurrencyPer;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.Letters)
            {
                string perLetterNumber;
                if (!NumberToPersianString.TryConvertNumberToPersianString(pi.Number, out perLetterNumber))
                {
                    perLetterNumber = null;
                }
                return perLetterNumber;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.LettersWithSign)
            {
                string perLetterNumber;
                if (!NumberToPersianString.TryConvertNumberToPersianString(pi.Number, out perLetterNumber))
                {
                    perLetterNumber = null;
                }
                string perLetterNegNumber = null;
                if (pi.Content[0] == '-')
                {
                    if (!NumberToPersianString.TryConvertNumberToPersianString(-pi.Number, out perLetterNegNumber))
                    {
                        perLetterNegNumber = null;
                    }
                    else
                    {
                        perLetterNegNumber = "-" + perLetterNegNumber;
                    }
                }
                else if (pi.Content[0] == '+' && !String.IsNullOrEmpty(perLetterNumber))
                {
                    perLetterNegNumber = "+" + perLetterNumber;
                }

                return perLetterNegNumber;
            }
            else if ((ruleValue.Value == NumberChangeRule.OutputDigitLanguages.English) && 
                (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPoint || ruleValue.Key == NumberChangeRule.OutputFormats.Fractional))
            {
                string f20NormalizedEng = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
                return f20NormalizedEng;
            }
            else if ((ruleValue.Value == NumberChangeRule.OutputDigitLanguages.English) && 
                (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep))
            {
                string f20NormalizedEng = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
                string f20CurrencyEng = MathUtils.InsertThousandSeperator(f20NormalizedEng);
                return f20CurrencyEng;
            }

            return null;
        }

        private static string CreateGeneralNumberSuggestionFor(GeneralNumberInfo pi, KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages> ruleValue)
        {
            if (pi == null)
            {
                return null;
            }

            string fracStr = pi.IsFraction ? pi.FractionString : null;
            string fracStrPer = fracStr == null ? null : ParsingUtils.ConvertNumber2Persian(fracStr);

            double value = pi.GetValue();
            if (pi.IsFraction) value = Math.Round(value, 3);
            string f20Normalized = MathUtils.NormalizeForF20Format(value.ToString("F20"));
            string f20Currency = MathUtils.InsertThousandSeperator(f20Normalized);
            bool addCurrency = (f20Normalized != f20Currency);
            string f20NormalizedPer = ParsingUtils.ConvertNumber2Persian(f20Normalized);
            string f20CurrencyPer = ParsingUtils.ConvertNumber2Persian(f20Currency);

            string strWritten;
            if (!NumberToPersianString.TryConvertNumberToPersianString(value, out strWritten))
                strWritten = null;

            // now giving orders

            if (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPoint &&
                ruleValue.Value == NumberChangeRule.OutputDigitLanguages.Persian)
            {
                // add in persian digits
                return f20NormalizedPer;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep &&
                     ruleValue.Value == NumberChangeRule.OutputDigitLanguages.Persian)
            {
                return f20CurrencyPer;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.Fractional &&
                     ruleValue.Value == NumberChangeRule.OutputDigitLanguages.Persian)
            {
                return fracStrPer;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.Letters)
            {
                return strWritten;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPoint &&
                     ruleValue.Value == NumberChangeRule.OutputDigitLanguages.English)
            {
                return f20Normalized;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep &&
                     ruleValue.Value == NumberChangeRule.OutputDigitLanguages.English)
            {
                return f20Currency;
            }
            else if (ruleValue.Key == NumberChangeRule.OutputFormats.Fractional &&
                     ruleValue.Value == NumberChangeRule.OutputDigitLanguages.English)
            {
                return fracStr;
            }

            return null;
        }


        /// <summary>
        /// Creates suggestions for the given pattern info
        /// </summary>
        /// <param name="rule">the change rules that give order to the suggestions</param>
        /// <param name="pi">the pattern info object to create suggestions for</param>
        /// <returns></returns>
        private static string[] CreateDigitizedNumberSuggestions(NumberChangeRule rule, DigitizedNumberPatternInfo pi)
        {
            if (pi == null)
            {
                return new string[0];
            }

            var lstSug = new List<string>();

            var inpFormat = NumberChangeRule.InputFormats.Digits;
            var inpLang = NumberParsersSuggestions.DetectInputDigitLanguage(pi.Content);
            if (inpLang == NumberChangeRule.InputDigitLanguages.None)
                inpLang = NumberChangeRule.InputDigitLanguages.English;

            string f20NormalizedEng = ParsingUtils.ConvertNumber2English(MathUtils.NormalizeForF20Format(pi.Number.ToString("F20")));
            string f20CurrencyEng = MathUtils.InsertThousandSeperator(f20NormalizedEng);
            bool addCurrency = (f20NormalizedEng != f20CurrencyEng);

            string f20NormPer = ParsingUtils.ConvertNumber2Persian(f20NormalizedEng);
            string f20CurrencyPer = ParsingUtils.ConvertNumber2Persian(f20CurrencyEng);

            string perLetterNumber;
            if (!NumberToPersianString.TryConvertNumberToPersianString(pi.Number, out perLetterNumber))
            {
                perLetterNumber = null;
            }

            string perLetterNegNumber = null;
            if (pi.Content[0] == '-')
            {
                if (!NumberToPersianString.TryConvertNumberToPersianString(-pi.Number, out perLetterNegNumber))
                {
                    perLetterNegNumber = null;
                }
                else
                {
                    perLetterNegNumber = "-" + perLetterNegNumber;
                }
            }
            else if (pi.Content[0] == '+' && !String.IsNullOrEmpty(perLetterNumber))
            {
                perLetterNegNumber = "+" + perLetterNumber;
            }


            KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages>? sugPrior = null;
            if(rule != null)
            {
                if (!String.IsNullOrEmpty(perLetterNegNumber)) inpFormat = NumberChangeRule.InputFormats.DigitsWithSign;

                if(rule.ContainsKey(inpFormat, inpLang))
                {
                    sugPrior = rule.GetValueForKey(inpFormat, inpLang);
                }
                else if(inpFormat == NumberChangeRule.InputFormats.DigitsWithSign && 
                    rule.ContainsKey(NumberChangeRule.InputFormats.Digits, inpLang))
                {
                    sugPrior = rule.GetValueForKey(NumberChangeRule.InputFormats.Digits, inpLang);
                }
            }

            // now giving orders
            if ((sugPrior != null && (sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.Persian) &&
                    (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPoint || sugPrior.Value.Key == NumberChangeRule.OutputFormats.Fractional )) 
                ||
                (sugPrior == null && inpLang != NumberChangeRule.InputDigitLanguages.Persian))
            {
                // adding floating in Persian digits
                lstSug.Add(f20NormPer);
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);

                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }

                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in english digits
                lstSug.Add(f20NormalizedEng);
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);
            }
            else if ((sugPrior != null && (sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.Persian) &&
                (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep)))
            {
                // adding floating in Persian digits
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);
                lstSug.Add(f20NormPer);

                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }

                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in english digits
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);
                lstSug.Add(f20NormalizedEng);
            }
            else if ((sugPrior != null && (sugPrior.Value.Key == NumberChangeRule.OutputFormats.Letters)) ||
                (sugPrior == null && inpLang == NumberChangeRule.InputDigitLanguages.Persian))
            {
                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }

                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in english digits
                lstSug.Add(f20NormalizedEng);
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);

                // adding floating in Persian digits
                lstSug.Add(f20NormPer);
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);
            }
            else if (sugPrior != null && (sugPrior.Value.Key == NumberChangeRule.OutputFormats.LettersWithSign))
            {
                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }

                // adding number in english digits
                lstSug.Add(f20NormalizedEng);
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);

                // adding floating in Persian digits
                lstSug.Add(f20NormPer);
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);
            }
            else if (sugPrior != null && (sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.English) && (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPoint || sugPrior.Value.Key == NumberChangeRule.OutputFormats.Fractional))
            {
                // adding number in english digits
                lstSug.Add(f20NormalizedEng);
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);

                // adding floating in Persian digits
                lstSug.Add(f20NormPer);
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);

                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }
            }
            else if (sugPrior != null && (sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.English) && (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep))
            {
                // adding number in english digits
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);
                lstSug.Add(f20NormalizedEng);

                // adding floating in Persian digits
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);
                lstSug.Add(f20NormPer);

                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }
            }
            else
            {
                // adding number in persian letters
                if (!String.IsNullOrEmpty(perLetterNumber))
                {
                    lstSug.Add(perLetterNumber);
                }

                // adding number in persian letters and sign
                if (!String.IsNullOrEmpty(perLetterNegNumber))
                {
                    lstSug.Add(perLetterNegNumber);
                }

                // adding number in english digits
                lstSug.Add(f20NormalizedEng);
                if (addCurrency)
                    lstSug.Add(f20CurrencyEng);

                // adding floating in Persian digits
                lstSug.Add(f20NormPer);
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);

            }


            return lstSug.ToArray();
        }

        /// <summary>
        /// Creates suggestions for the given pattern info
        /// </summary>
        /// <param name="rule">the change rules that give order to the suggestions</param>
        /// <param name="pi">the pattern info object to create suggestions for</param>
        /// <returns></returns>
        private static string[] CreateGeneralNumberSuggestions(NumberChangeRule rule, GeneralNumberInfo pi)
        {
            if (pi == null)
            {
                return new string[0];
            }

            var lstSug = new List<string>();

            string fracStr = pi.IsFraction ? pi.FractionString : null;
            string fracStrPer = fracStr == null ? null : ParsingUtils.ConvertNumber2Persian(fracStr);

            double value = pi.GetValue();
            if (pi.IsFraction) value = Math.Round(value, 3);
            string f20Normalized = MathUtils.NormalizeForF20Format(value.ToString("F20"));
            string f20Currency = MathUtils.InsertThousandSeperator(f20Normalized);
            bool addCurrency = (f20Normalized != f20Currency);
            string f20NormalizedPer = ParsingUtils.ConvertNumber2Persian(f20Normalized);
            string f20CurrencyPer = ParsingUtils.ConvertNumber2Persian(f20Currency);

            string strWritten;
            if (!NumberToPersianString.TryConvertNumberToPersianString(value, out strWritten))
                strWritten = null;

            var inpFormat = NumberChangeRule.InputFormats.Letters;
            var inpLang = NumberChangeRule.InputDigitLanguages.None;

            KeyValuePair<NumberChangeRule.OutputFormats, NumberChangeRule.OutputDigitLanguages>? sugPrior = null;
            if (rule != null)
            {
                if (rule.ContainsKey(inpFormat, inpLang))
                {
                    sugPrior = rule.GetValueForKey(inpFormat, inpLang);
                }
                else if(rule.ContainsKey(inpFormat, NumberChangeRule.InputDigitLanguages.Persian))
                {
                    sugPrior = rule.GetValueForKey(inpFormat, NumberChangeRule.InputDigitLanguages.Persian);
                }
            }

            // now giving orders

            if (sugPrior != null)
            {
                if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPoint &&
                    sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.Persian)
                {
                    // add in persian digits
                    lstSug.Add(f20NormalizedPer);
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);

                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);

                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStrPer);
                        lstSug.Add(fracStr);
                    }

                    // add in english digits
                    lstSug.Add(f20Normalized);
                    if (addCurrency)
                        lstSug.Add(f20Currency);
                }
                else if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep &&
                    sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.Persian)
                {
                    // add in persian digits
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);
                    lstSug.Add(f20NormalizedPer);

                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);

                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStrPer);
                        lstSug.Add(fracStr);
                    }

                    // add in english digits
                    if (addCurrency)
                        lstSug.Add(f20Currency);
                    lstSug.Add(f20Normalized);
                }
                else if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.Fractional &&
                    sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.Persian)
                {
                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStrPer);
                        lstSug.Add(fracStr);
                    }

                    // add in persian digits
                    lstSug.Add(f20NormalizedPer);
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);

                    // add in english digits
                    lstSug.Add(f20Normalized);
                    if (addCurrency)
                        lstSug.Add(f20Currency);

                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);
                }
                else if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.Letters)
                {
                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);

                    // add in persian digits
                    lstSug.Add(f20NormalizedPer);
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);

                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStrPer);
                        lstSug.Add(fracStr);
                    }

                    // add in english digits
                    lstSug.Add(f20Normalized);
                    if (addCurrency)
                        lstSug.Add(f20Currency);

                }
                else if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPoint &&
                    sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.English)
                {
                    // add in english digits
                    lstSug.Add(f20Normalized);
                    if (addCurrency)
                        lstSug.Add(f20Currency);

                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStr);
                        lstSug.Add(fracStrPer);
                    }

                    // add in persian digits
                    lstSug.Add(f20NormalizedPer);
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);

                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);


                }
                else if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.FloatingPointWithSep &&
                    sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.English)
                {
                    // add in english digits
                    if (addCurrency)
                        lstSug.Add(f20Currency);
                    lstSug.Add(f20Normalized);

                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);

                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStr);
                        lstSug.Add(fracStrPer);
                    }

                    // add in persian digits
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);
                    lstSug.Add(f20NormalizedPer);

                }
                else if (sugPrior.Value.Key == NumberChangeRule.OutputFormats.Fractional &&
                    sugPrior.Value.Value == NumberChangeRule.OutputDigitLanguages.English)
                {
                    // add fractions
                    if (!String.IsNullOrEmpty(fracStr))
                    {
                        lstSug.Add(fracStr);
                        lstSug.Add(fracStrPer);
                    }

                    // add in english digits
                    lstSug.Add(f20Normalized);
                    if (addCurrency)
                        lstSug.Add(f20Currency);

                    // add in persian digits
                    lstSug.Add(f20NormalizedPer);
                    if (addCurrency)
                        lstSug.Add(f20CurrencyPer);

                    // add the number in letters
                    if (!String.IsNullOrEmpty(strWritten))
                        lstSug.Add(strWritten);
                }

            }

            if(lstSug.Count <= 0)
            {
                // add fractions
                if (!String.IsNullOrEmpty(fracStr))
                {
                    lstSug.Add(fracStrPer);
                    lstSug.Add(fracStr);
                }

                // add in persian digits
                lstSug.Add(f20NormalizedPer);
                if (addCurrency)
                    lstSug.Add(f20CurrencyPer);

                // add in english digits
                lstSug.Add(f20Normalized);
                if (addCurrency)
                    lstSug.Add(f20Currency);

                // add the number in letters
                if (!String.IsNullOrEmpty(strWritten))
                    lstSug.Add(strWritten);
            }

            return lstSug.ToArray();
        }

        public static NumberChangeRule.InputDigitLanguages DetectInputDigitLanguage(string str)
        {
            var inputDigitLanguage = NumberChangeRule.InputDigitLanguages.None;
            int len = str.Length;

            for (int i = 0; i < len; i++)
            {
                char ch = str[i];
                if (Char.IsLetter(ch) && ch != 'e' && ch != 'E')
                {
                    // return none
                    return NumberChangeRule.InputDigitLanguages.None;
                }
                else if (Char.IsDigit(ch))
                {
                    if (StringUtil.IsPersianDigit(ch))
                    {
                        switch (inputDigitLanguage)
                        {
                            case NumberChangeRule.InputDigitLanguages.None:
                                inputDigitLanguage = NumberChangeRule.InputDigitLanguages.Persian;
                                break;
                            case NumberChangeRule.InputDigitLanguages.English:
                            case NumberChangeRule.InputDigitLanguages.Persian:
                                // do nothing
                                break;
                        }
                    }
                    else if (StringUtil.IsEnglishDigit(ch))
                    {
                        switch (inputDigitLanguage)
                        {
                            case NumberChangeRule.InputDigitLanguages.Persian:
                            case NumberChangeRule.InputDigitLanguages.None:
                                inputDigitLanguage = NumberChangeRule.InputDigitLanguages.English;
                                break;
                            case NumberChangeRule.InputDigitLanguages.English:
                                // do nothing
                                break;
                        }
                    }
                    // there's a digit which is neither Persian nor English, 
                    // so our number's language is detected as English language
                    else
                    {
                        inputDigitLanguage = NumberChangeRule.InputDigitLanguages.English;
                    }
                }
            }

            return inputDigitLanguage;
        }
    }
}

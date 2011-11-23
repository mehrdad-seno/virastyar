using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// Provides the means to search some input string and find and parse
    /// all occurrances of written-forms of persian real numbers.
    /// The numbers can be integer, floating point, or fractions.
    /// </summary>
    public class PersianRealNumberParser
    {
        #region Symbolic Constants

        // The symbolic constants are numbers that are going to 
        // represent special tokens in the input string.
        // Their value are negative to avoid conflict will the valid number tokens.

        private const long InvalidNumber = -5;
        private const long MiimNumber = -4;
        private const long VaavNumber = -2;
        private const long MomayezNumber = -3;
        private const long ManfiNumber = -1;

        #endregion

        /// <summary>
        /// HashSet that holds all possible words which can occur in a written form of a number in Persian language.
        /// </summary>
        private static readonly HashSet<string> s_setAllChunkWords = new HashSet<string>();

        /// <summary>
        /// Initializes the <see cref="PersianRealNumberParser"/> class. 
        /// By adding all possible words which might be encountered into a Persian
        /// real number string to the set of words.
        /// </summary>
        static PersianRealNumberParser()
        {
            AddWordToSet("صفر");
            AddWordToSet("یک");
            AddWordToSet("دو");
            AddWordToSet("سه");
            AddWordToSet("چهار");
            AddWordToSet("پنج");
            AddWordToSet("شش");
            AddWordToSet("شیش");
            AddWordToSet("هفت");
            AddWordToSet("هشت");
            AddWordToSet("نه");

            AddWordToSet("ده");
            AddWordToSet("یازده");
            AddWordToSet("دوازده");
            AddWordToSet("سیزده");
            AddWordToSet("چارده");
            AddWordToSet("چهارده");
            AddWordToSet("پانزده");
            AddWordToSet("پونزده");
            AddWordToSet("شونزده");
            AddWordToSet("شانزده");
            AddWordToSet("هفده");
            AddWordToSet("هیفده");
            AddWordToSet("هجده");
            AddWordToSet("هیجده");
            AddWordToSet("هژده");
            AddWordToSet("هیژده");
            AddWordToSet("نوزده");
            AddWordToSet("بیست");
            AddWordToSet("سی");
            AddWordToSet("چهل");
            AddWordToSet("پنجاه");
            AddWordToSet("شصت");
            AddWordToSet("هفتاد");
            AddWordToSet("هشتاد");
            AddWordToSet("نود");

            AddWordToSet("یک‌صد");
            AddWordToSet("یکصد");
            AddWordToSet("صد");
            AddWordToSet("دویست");
            AddWordToSet("سیصد");
            AddWordToSet("چهارصد");
            AddWordToSet("چارصد");
            AddWordToSet("پانصد");
            AddWordToSet("پونصد");
            AddWordToSet("ششصد");
            AddWordToSet("شیشصد");
            AddWordToSet("شش‌صد");
            AddWordToSet("شیش‌صد");
            AddWordToSet("هفتصد");
            AddWordToSet("هشتصد");
            AddWordToSet("نهصد");
            AddWordToSet("هفت‌صد");
            AddWordToSet("هشت‌صد");
            AddWordToSet("نه‌صد");

            AddWordToSet("هزار");
            AddWordToSet("ملیون");
            AddWordToSet("میلیون");
            AddWordToSet("ملیارد");
            AddWordToSet("میلیارد");
            AddWordToSet("بلیون");
            AddWordToSet("بیلیون");
            AddWordToSet("ترلیارد");
            AddWordToSet("تریلیارد");
            AddWordToSet("تریلیون");
            AddWordToSet("ترلیون");

            s_setAllChunkWords.Add("نیم");

            s_setAllChunkWords.Add("و");
            s_setAllChunkWords.Add("ممیز");
            s_setAllChunkWords.Add("منفی");
            s_setAllChunkWords.Add("منهای");
        }

        /// <summary>
        /// Adds the word of number and its ordinal form to set of possible words.
        /// </summary>
        /// <param name="word">The word to add.</param>
        private static void AddWordToSet(string word)
        {
            s_setAllChunkWords.Add(word);

            if (word == "یک")
            {
                s_setAllChunkWords.Add("اول");
                s_setAllChunkWords.Add(word + "م");
            }
            if (word == "سه")
            {
                s_setAllChunkWords.Add("سوم");
            }
            else if (word.EndsWith("ی"))
            {
                s_setAllChunkWords.Add(word + "‌ام");
            }
            else if (word.EndsWith("ن"))
            {
                s_setAllChunkWords.Add(word + "یم");
            }
            else
            {
                s_setAllChunkWords.Add(word + "م");
            }

        }

        /// <summary>
        /// Finds the chunk of words in the input string in which there could probably numbers be found.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns></returns>
        private IEnumerable<ChunkInfo> FindChunks(string input)
        {
            int chunkStart = 0;
            int chunkEnd = 0;

            var lstTags = new List<ChunkElement>();

            bool isInsideChunk = false;
            bool isWordProper;
            string word;

            foreach (var wordInfo in WordReadingUtility.ReadWords(input, true))
            {
                word = wordInfo.Value;
                isWordProper = false;

                #region First See if word is proper or not
                if (s_setAllChunkWords.Contains(word))
                {
                    isWordProper = true;
                }
                else if(word.EndsWith("و"))
                {
                    string wordWithoutVaav = word.Substring(0, word.Length - 1);
                    if (s_setAllChunkWords.Contains(wordWithoutVaav))
                        isWordProper = true;
                }
                else if (word.Length > 0 && Char.IsDigit(word[0]))
                {
                    isWordProper = true;
                }
                #endregion

                #region Then use the properness of the word to create or continue chunks
                if (isWordProper)
                {
                    if (word == "نیم")
                    {
                        if (lstTags.Count <= 0)
                        {
                            lstTags.Add(new ChunkElement(word, new [] { 5L, 10L, MiimNumber },
                                wordInfo.Index, wordInfo.EndIndex));
                        }
                        else if (lstTags[lstTags.Count - 1].ElementValues.GetLastElement() == VaavNumber)
                        {
                            lstTags[lstTags.Count - 1].ElementValues.SetLastElement(MomayezNumber);
                            lstTags.Add(new ChunkElement(word, new [] { 5L, 10L, MiimNumber },
                                wordInfo.Index, wordInfo.EndIndex));
                        }
                        else
                        {
                            lstTags.Add(new ChunkElement(word, new [] { MomayezNumber, 5L, 10L, MiimNumber },
                                wordInfo.Index, wordInfo.EndIndex));
                        }
                    }
                    else
                    {
                        long numValue = GetWordNumericValue(word);
                        if (numValue == InvalidNumber && word.EndsWith("و"))
                        {
                            string wordWithoutVaav = word.Substring(0, word.Length - 1);
                            numValue = GetWordNumericValue(wordWithoutVaav);
                            if (numValue == InvalidNumber)
                            {
                                lstTags.Add(new ChunkElement(word, numValue, wordInfo.Index, wordInfo.EndIndex ));
                            }
                            else
                            {
                                lstTags.Add(new ChunkElement(wordWithoutVaav, numValue, wordInfo.Index, wordInfo.EndIndex - 1 ));
                                lstTags.Add(new ChunkElement("و", VaavNumber, wordInfo.EndIndex, wordInfo.EndIndex));
                            }
                        }
                        else
                        {
                            if (numValue != InvalidNumber && word.EndsWith("م"))
                            {
                                //if (lstTags.Count <= 0 || lstTags.GetLastElement().ElementValues.GetLastElement() == VaavNumber)
                                //{
                                //    lstTags.Add(new ChunkElement(word, numValue, wordInfo.StartIndex, wordInfo.EndIndex));
                                //}
                                //else
                                //{
                                    lstTags.Add(new ChunkElement(word, new [] { numValue, MiimNumber }, wordInfo.Index, wordInfo.EndIndex));
                                //}
                            }
                            else
                            {
                                lstTags.Add(new ChunkElement(word, numValue, wordInfo.Index, wordInfo.EndIndex));
                            }
                        }
                    }

                    if (!isInsideChunk)
                        chunkStart = wordInfo.Index;
                    chunkEnd = wordInfo.EndIndex;

                    if ( lstTags[lstTags.Count - 1].ElementValues.GetLastElement() == MiimNumber)
                    {
                        yield return new ChunkInfo(input.Substring(chunkStart, chunkEnd - chunkStart + 1), chunkStart, chunkEnd, lstTags);
                        lstTags.Clear();
                        isInsideChunk = false;
                    }
                    else
                    {
                        isInsideChunk = true;
                    }
                }
                else
                {
                    if(isInsideChunk)
                        yield return new ChunkInfo(input.Substring(chunkStart, chunkEnd - chunkStart + 1), chunkStart, chunkEnd, lstTags);
                    lstTags.Clear();
                    isInsideChunk = false;
                }
                #endregion
            }

            #region having the words finished is like encountering an improper word

            if(isInsideChunk)
                yield return new ChunkInfo(input.Substring(chunkStart, chunkEnd - chunkStart + 1), chunkStart, chunkEnd, lstTags);

            #endregion
        }

        /// <summary>
        /// Searches the specified string for patterns of real numbers in a persian descriptive string, and
        /// returns a sequnce of <see cref="GeneralNumberInfo"/> that holds information about the pattern found.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <returns></returns>
        public IEnumerable<GeneralNumberInfo> FindAndParse(string input)
        {
            var lstNumberInfos = new List<GeneralNumberInfo>();
            
            foreach (ChunkInfo chunk in FindChunks(input))
            {
                foreach (GeneralNumberInfo numberinfo in ExtractGeneralNumber(chunk))
                {
                    lstNumberInfos.Add(numberinfo);
                }
            }

            lstNumberInfos.Sort(new GeneralNumberInfoComparer());

            return lstNumberInfos;
        }

        /// <summary>
        /// Determines whether the list of numbers need to be trimmed. 
        /// Since the list of numbers is gained from chunks in input, they may contain 
        /// some tokens at their beginning or ending, which are valid in chunks, but does not make
        /// a valid beginning or ending for a number. e.g. vaav is an example.
        /// </summary>
        /// <param name="lstValues">The list of long values.</param>
        /// <param name="start">The start index after trimming the list.</param>
        /// <param name="end">The end index after trimming the list.</param>
        /// <returns></returns>
        private bool ListNeedToBeTrimmed(List<long> lstValues, out int start, out int end)
        {
            start = 0;
            end = lstValues.Count - 1;

            if (lstValues.Count <= 0) return false;

            for (int i = 0; i < lstValues.Count; i++)
            {
                if (lstValues[i] < -1) start++;
                else break;
            }

            for (int i = lstValues.Count - 1; i >= 0; i--)
            {
                if (lstValues[i] < 0 && lstValues[i] != MiimNumber) end--;
                else break;
            }

            if (start != 0 || end != lstValues.Count - 1) return true;
            else return false;
        }

        /// <summary>
        /// Extracts and returns a sequence of all occurrances of real numbers in the specified chunk.
        /// </summary>
        /// <param name="chunk">The chunk to extract real numbers from.</param>
        /// <returns></returns>
        private IEnumerable<GeneralNumberInfo> ExtractGeneralNumber(ChunkInfo chunk)
        {
            Debug.Assert(chunk != null);

            List<long> lstValues = chunk.GetAllChunkElementValues().ToList();

            Debug.Assert(lstValues.Count > 0);

            // NOTE: this if section prevents recognizing digits only
            //       you might remove it later
            if (lstValues.Count == 1)
            {
                if (Char.IsDigit(chunk.ListChunkElements[0].Content[0]))
                    return new GeneralNumberInfo[0];
            }

            int startTrimIndex, endTrimIndex;
            if(ListNeedToBeTrimmed(lstValues, out startTrimIndex, out endTrimIndex))
            {
                if (startTrimIndex <= endTrimIndex && startTrimIndex >= 0 && endTrimIndex < lstValues.Count)
                {
                    int startInChunk = chunk.GetChunkIndexFromValueIndex(startTrimIndex);
                    int endInChunk = chunk.GetChunkIndexFromValueIndex(endTrimIndex);

                    if (startInChunk <= endInChunk && startInChunk >= 0 && endInChunk < chunk.ListChunkElements.Count)
                    {
                        return ExtractGeneralNumber(chunk.GetSubChunk(startInChunk, endInChunk));
                    }
                }
                return new GeneralNumberInfo[0];
            }

            if (lstValues.Count > 0)
            {
                var lstNumbers = new List<GeneralNumberInfo>();

                if (lstValues.Count == 1)
                {
                    #region if lstValues contains only 1 number
                    if (lstValues[0] >= 0) // i.e. it is a number
                    {
                        return new GeneralNumberInfo[] { new GeneralNumberInfo(chunk.Content, lstValues[0], null, chunk.StartIndex, chunk.EndIndex, chunk.ListChunkElements) };
                    }
                    else // i.e. it is not a number e.g. it is a sign
                    {
                        return new GeneralNumberInfo[0];

                        // returns an invalid number
                        //return new GeneralNumberInfo[] { new GeneralNumberInfo(chunk.Content, chunk.StartIndex, chunk.EndIndex, chunk.ListTags) };
                    }
                    #endregion
                }
                else // if there are more than one values in the chunk
                {
                    #region if lstValues contain more than [or less than] one number
                    if (lstValues.Contains(MomayezNumber))
                    {
                        #region if lstValues contains Momayez

                        // decimal point index in the list of values
                        int dotIndex = lstValues.FindIndex(l => l == MomayezNumber);

                        // decimal point index in the chunks
                        int dotIndexInChunk = chunk.GetChunkIndexFromValueIndex(dotIndex);

                        // this will hold floating part information
                        FloatingPartInfo pi = null;

                        // what if it has both momayez and miim
                        if (lstValues.GetLastElement() == MiimNumber)
                        {
                            try
                            {
                                pi = GenerateFloatingPartFrom(lstValues, dotIndex);
                            }
                            catch (NumberSeperationException ex)
                            {
                                int exIndex = ex.Index;
                                if (exIndex < 0) exIndex = 0;
                                int chunkIndex = chunk.GetChunkIndexFromValueIndex(exIndex);

                                if (chunkIndex >= 0 && chunkIndex < chunk.ListChunkElements.Count - 1)
                                {
                                    lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(0, chunkIndex)));
                                    lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(chunkIndex + 1)));
                                }
                                else
                                {
                                    //lstNumbers.Add(new GeneralNumberInfo(chunk.Content, chunk.StartIndex, chunk.EndIndex, chunk.ListTags));
                                }

                                return lstNumbers;
                            }
                            catch (Exception ex)
                            {
                                Debug.Print("Met general exception: " + ex.ToString());
                            }
                        }
                        else
                        {
                            if (dotIndexInChunk >= chunk.ListChunkElements.Count - 1) // i.e. momayez is the last word
                            {
                                lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(0, dotIndexInChunk - 1)));
                                return lstNumbers;
                            }
                            else if (dotIndexInChunk == 0) // i.e. momayez was the first word
                            {
                                lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(1)));
                                return lstNumbers;
                            }
                            else
                            {
                                // extract as many integers as possible that exist after the decimal point
                                GeneralNumberInfo[] nums = ExtractIntegerNumber(chunk.GetSubChunk(dotIndexInChunk + 1)).ToArray();

                                if (nums.Length == 1)
                                {
                                    // if num[0] is right after the dot
                                    if (nums[0].ListChunkElements[0].IsEqualTo(chunk.ListChunkElements[dotIndexInChunk + 1]))
                                    {
                                        pi = new FloatingPartInfo(nums[0].IntegralPart);
                                    }
                                    else
                                    {
                                        lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(0, dotIndexInChunk - 1)));
                                        lstNumbers.Add(nums[0]);
                                        return lstNumbers;
                                    }
                                }
                                else if (nums.Length > 1) // if there are more than one integer numbers after the decimal point...
                                {
                                    // extract numbers from beginning to the end of 1st number
                                    lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(0, dotIndexInChunk + nums[0].ListChunkElements.Count)));
                                    // extract numbers after the first number right after the decimal point
                                    lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(dotIndexInChunk + nums[0].ListChunkElements.Count + 1)));
                                    return lstNumbers;
                                }
                                else  // if there are no numbers after the decimal point...
                                {
                                    // extract numbers right before the decimal point
                                    lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(0, dotIndexInChunk - 1)));
                                    return lstNumbers;
                                }
                            }
                        }

                        //GeneralNumberInfo info;

                        // if the above code is not returned we hope that pi has a correct value 
                        // so we generate all combinations of the integers prior to decimal point with pi.
                        foreach (GeneralNumberInfo numPriorToDot in ExtractIntegerNumber(chunk.GetSubChunk(0, dotIndexInChunk - 1)))
                        {
                            // check if num is exactly prior to dot
                            // if last chunk element in number is equal to the chunk element right before the dot then the number is right before the dot
                            if (numPriorToDot.ListChunkElements.GetLastElement().IsEqualTo(chunk.ListChunkElements[dotIndexInChunk - 1]))
                            {
                                ChunkInfo subChunk = chunk.GetSubChunk(dotIndexInChunk - numPriorToDot.ListChunkElements.Count);
                                // add the integer part combined with the previously evaluated floating part (pi)
                                lstNumbers.Add(new GeneralNumberInfo(subChunk.Content, numPriorToDot.IntegralPart, pi,
                                    numPriorToDot.StartIndex, subChunk.EndIndex, subChunk.ListChunkElements));

                            }
                            else // it was not right before the dot
                            {
                                lstNumbers.Add(numPriorToDot); // add the number itself without combining it to pi
                            }
                        }

                        if (lstNumbers.Count > 0)
                        {
                            return lstNumbers;
                        }

                        #endregion
                    }
                    else if (lstValues.GetLastElement() == MiimNumber)
                    {
                        #region if lstValues ends with Miim [Number]

                        int[] vaavIndeces = lstValues.FindAllIndeces(VaavNumber).ToArray();
                        if (vaavIndeces.Length > 0)
                        {
                            for (int i = vaavIndeces.Length - 1; i >= 0; i--)
                            {
                                GeneralNumberInfo num1, num2;
                                int chunkStart = chunk.GetChunkIndexFromValueIndex(vaavIndeces[i] + 1);
                                if (chunkStart >= 0)
                                {
                                    if (CanExtractTwoIntegersFrom(chunk.GetSubChunk(chunkStart), out num1, out num2))
                                    {
                                        FloatingPartInfo fpi = new FloatingPartInfo(num1.IntegralPart, num2.IntegralPart);

                                        // now check values before vaav and form a chunk with the most leading part right before vaav
                                        // if there are no such integer part right before vaav then try next vaav
                                        // 
                                        int vaavIndexInChunk = chunk.GetChunkIndexFromValueIndex(vaavIndeces[i]);
                                        int lastLetterBeforeVaavIndex = chunk.ListChunkElements[vaavIndexInChunk - 1].EndIndex;
                                        List<GeneralNumberInfo> numbersBeforeVaav = ExtractIntegerNumber(chunk.GetSubChunk(0, vaavIndexInChunk - 1)).ToList();

                                        for (int chindex = 0; chindex < numbersBeforeVaav.Count; chindex++)
                                        {
                                            if (numbersBeforeVaav[chindex].EndIndex == lastLetterBeforeVaavIndex)
                                            {
                                                int numberElemCount = numbersBeforeVaav[chindex].ListChunkElements.Count;

                                                if (vaavIndexInChunk - numberElemCount - 1 >= 0)
                                                    lstNumbers.AddRange(ExtractGeneralNumber(chunk.GetSubChunk(0, vaavIndexInChunk - numberElemCount - 1)));

                                                ChunkInfo subChunk = chunk.GetSubChunk(vaavIndexInChunk - numberElemCount);
                                                lstNumbers.Add(new GeneralNumberInfo(subChunk.Content, numbersBeforeVaav[chindex].IntegralPart, fpi, subChunk.StartIndex, subChunk.EndIndex, subChunk.ListChunkElements));
                                            }
                                            //else
                                            //{
                                            //    numbers.Add(numbersBeforeVaav[chindex]);
                                            //}
                                        }
                                    }
                                }
                            }
                        }

                        // first see if it is a fraction if not return all integers
                        //long fracNum1, fracNum2;
                        GeneralNumberInfo fracNum1, fracNum2;
                        //int chunkEnd_here = chunk.GetChunkIndexFromValueIndex(lstValues.Count - 1);
                        //if (chunkEnd_here >= 0)
                        //{
                            //if (CanExtractTwoIntegersFrom(chunk.GetSubChunk(0, chunkEnd_here), out fracNum1, out fracNum2))
                            if (CanExtractTwoIntegersFrom(chunk, out fracNum1, out fracNum2))
                            {
                                FloatingPartInfo fpi = new FloatingPartInfo(fracNum1.IntegralPart, fracNum2.IntegralPart);
                                lstNumbers.Add(new GeneralNumberInfo(chunk.Content, 0L, fpi, chunk.StartIndex, chunk.EndIndex, chunk.ListChunkElements));
                            }
                            else if (chunk.Content == "نیم")
                            {
                                FloatingPartInfo fpi = new FloatingPartInfo(5, 10);
                                lstNumbers.Add(new GeneralNumberInfo(chunk.Content, 0L, fpi, chunk.StartIndex, chunk.EndIndex, chunk.ListChunkElements));
                            }
                        //}

                        if (lstNumbers.Count <= 0)
                        {
                            lstNumbers.AddRange(ExtractIntegerNumber(chunk, true));
                        }

                        //if (numbers.Count <= 0) // e.g. (20)(va)(5)(miim)
                        //{
                        //    return ExtractIntegerNumber(chunk, true);
                        //}


                        return lstNumbers;

                        #endregion
                    } // end of if ends with MiimNumber
                    else
                    {
                        // if values do not contain momayez or end with miim so it is a chunk of integer numbers
                        return ExtractIntegerNumber(chunk);
                    }
                    #endregion
                }
            }

            return new GeneralNumberInfo[0];
            //return new GeneralNumberInfo[] { new GeneralNumberInfo(chunk.Content, chunk.StartIndex, chunk.EndIndex, chunk.ListTags ) };
        }

        /// <summary>
        /// Determines whether two adjacent integers can be extracted from the specified chunk.
        /// </summary>
        /// <param name="chunk">The chunk holding elements and values.</param>
        /// <param name="num1">The 1st number.</param>
        /// <param name="num2">The 2nd number.</param>
        /// <returns>
        /// 	<c>true</c> if two adjacent integers can be extracted from the specified chunk; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExtractTwoIntegersFrom(ChunkInfo chunk, out GeneralNumberInfo num1, out GeneralNumberInfo num2)
        {
            num1 = num2 = null;
            List<GeneralNumberInfo> lstAllNums = ExtractIntegerNumber(chunk, true).ToList();

            if (lstAllNums.Count == 2)
            {
                num1 = lstAllNums[0];
                num2 = lstAllNums[1];
            }
            else if (lstAllNums.Count == 1)
            {
                List<long> lstValues = chunk.GetAllChunkElementValues().ToList();
                if (lstValues.GetLastElement() == MiimNumber) lstValues.RemoveAt(lstValues.Count - 1);

                if (lstValues.Count >= 2 && lstValues[lstValues.Count - 1] > 0 && lstValues[lstValues.Count - 2] > 0)
                {
                    int lastElemChunkIndex = chunk.GetChunkIndexFromValueIndex(lstValues.Count - 1);
                    if (lastElemChunkIndex - 1 >= 0)
                    {
                        GeneralNumberInfo[] remInts = ExtractIntegerNumber(chunk.GetSubChunk(0, lastElemChunkIndex - 1), true).ToArray();
                        if (remInts.Length == 1)
                        {
                            num1 = remInts[0];
                            GeneralNumberInfo[] arnum2 = ExtractIntegerNumber(chunk.GetSubChunk(lastElemChunkIndex),true).ToArray();
                            if(arnum2.Length == 1)
                            {
                                num2 = arnum2[0];
                            }
                        }
                    }
                }
            }

            if (num1 == null || num2 == null)
                return false;

            if (num1.ListChunkElements[0].IsEqualTo(chunk.ListChunkElements[0]) && num2.ListChunkElements[0].IsEqualTo(chunk.ListChunkElements[num1.ListChunkElements.Count]))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Extracts all integer numbers that can be extracted from the specified list of element-values.
        /// </summary>
        /// <param name="lstValues">The List of values to extract integer numbers from.</param>
        /// <returns></returns>
        private IEnumerable<long> ExtractAllIntsFromList(List<long> lstValues)
        {
            try
            {
                if (lstValues.Count == 1)
                {
                    if (lstValues[0] >= 0)
                        return new [] {lstValues[0]};
                }
                else
                {
                    long n = GenerateIntegralPartFrom(lstValues, 0, lstValues.Count - 1);
                    return new [] { n };
                }
            }
            catch (NumberSeperationException ex)
            {
                int exIndex = ex.Index;
                if (exIndex < 0) exIndex = 0;

                var lstChunks = new List<long>();

                if (exIndex >= 0 && exIndex < lstValues.Count - 1)
                {
                    lstChunks.AddRange(ExtractAllIntsFromList(lstValues.GetRange(0, exIndex + 1)));
                    lstChunks.AddRange(ExtractAllIntsFromList(lstValues.GetRange(exIndex + 1, lstValues.Count - exIndex - 1)));
                }

                return lstChunks.ToArray();
            }
            catch (Exception ex)
            {
                Debug.Print("Met General Exception\r\n" + ex);
            }
            return new long[0];
        }

        /// <summary>
        /// Extracts and returns sequence of all occurrances of integer numbers from the specified chunk.
        /// This method does NOT ignore possible ordinal-Miim at the end of the number.
        /// </summary>
        /// <param name="chunk">The chunk to extract integer numbers from.</param>
        /// <returns></returns>
        private IEnumerable<GeneralNumberInfo> ExtractIntegerNumber(ChunkInfo chunk)
        {
            return ExtractIntegerNumber(chunk, false);
        }

        /// <summary>
        /// Extracts and returns sequence of all occurrances of integer numbers from the specified chunk.
        /// </summary>
        /// <param name="chunk">The chunk to extract integer numbers from.</param>
        /// <param name="ignoreMiim">if set to <c>true</c> ignores the ordinal-Miim at the end of the number (if any).</param>
        /// <returns></returns>
        private IEnumerable<GeneralNumberInfo> ExtractIntegerNumber(ChunkInfo chunk, bool ignoreMiim)
        {
            Debug.Assert(chunk != null);

            List<long> lstValues = chunk.GetAllChunkElementValues().ToList();
            
            if(ignoreMiim)
            {
                if (lstValues.GetLastElement() == MiimNumber)
                    lstValues.RemoveAt(lstValues.Count - 1);
            }

            Debug.Assert(lstValues.Count > 0);

            if (lstValues.Count == 1)
            {
                if (lstValues[0] >= 0)
                {
                    return new [] { new GeneralNumberInfo(chunk.Content, lstValues[0], null, chunk.StartIndex, chunk.EndIndex, chunk.ListChunkElements) };
                }
                else
                {
                    return new GeneralNumberInfo[0];
                    //return new GeneralNumberInfo[] { new GeneralNumberInfo(chunk.Content, chunk.StartIndex, chunk.EndIndex, chunk.ListTags) };
                }
            }
            else
            {
                try
                {
                    long n = GenerateIntegralPartFrom(lstValues, 0, lstValues.Count - 1);

                    var info = new GeneralNumberInfo(chunk.Content, n, null, chunk.StartIndex, chunk.EndIndex, chunk.ListChunkElements);
                    return new [] { info };
                }
                catch (NumberSeperationException ex)
                {
                    int exIndex = ex.Index;
                    if (exIndex < 0) exIndex = 0;

                    var lstChunks = new List<GeneralNumberInfo>();
                    int chunkIndex = chunk.GetChunkIndexFromValueIndex(exIndex);

                    if (chunkIndex >= 0 && chunkIndex < chunk.ListChunkElements.Count - 1)
                    {
                        lstChunks.AddRange(ExtractIntegerNumber(chunk.GetSubChunk(0, chunkIndex), ignoreMiim));
                        lstChunks.AddRange(ExtractIntegerNumber(chunk.GetSubChunk(chunkIndex + 1), ignoreMiim));
                    }
                    else
                    {
                        //lstChunks.Add(new GeneralNumberInfo(chunk.Content, chunk.StartIndex, chunk.EndIndex, chunk.ListTags));
                    }

                    return lstChunks.ToArray();
                }
                catch
                {
                    Debug.Print("Met General Exception upon: " + chunk.Content);
                }
            }

            return new GeneralNumberInfo[0];
//            return new GeneralNumberInfo[] { new GeneralNumberInfo(chunk.Content, chunk.StartIndex, chunk.EndIndex, chunk.ListTags ) };
        }

        /// <summary>
        /// Gets the numeric value of the specified word which can occur in the written-form of a Persian real number. 
        /// This includes the symbolic constants also.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        private long GetWordNumericValue(string word)
        {
            if (word.Length <= 0) return InvalidNumber;

            long n;
            if (Char.IsDigit(word[0]))
            {
                if (Int64.TryParse(ParsingUtils.ConvertNumber2English(word), out n))
                {
                    return n;
                }
                else
                {
                    return InvalidNumber;
                }
            }

            if (PersianLiteral2NumMap.TryPersianString2Num(word, out n))
            {
                return n;
            }
            //if (word.EndsWith("و") && word.Length > 1)
            //{
            //    string wordWithoutVaav = word.Substring(0, word.Length - 1);
            //    if (PersianLiteral2NumMap.TryPersianString2Num(wordWithoutVaav, out n))
            //    {
            //        return n;
            //    }
            //}
            if (word.EndsWith("م"))
            {
                string wordWithoutMiim = word.Substring(0, word.Length - 1);
                if (PersianLiteral2NumMap.TryPersianString2Num(wordWithoutMiim, out n))
                {
                    return n;
                }

            }
            if (word.EndsWith("یم"))
            {
                string wordWithoutYam = word.Substring(0, word.Length - 2);
                if (PersianLiteral2NumMap.TryPersianString2Num(wordWithoutYam, out n))
                {
                    return n;
                }
            }
            if (word.EndsWith("‌ام"))
            {
                string wordWithoutYam = word.Substring(0, word.Length - 3);
                if (PersianLiteral2NumMap.TryPersianString2Num(wordWithoutYam, out n))
                {
                    return n;
                }
            }
            if (word == "منفی")
            {
                return ManfiNumber;
            }
            if (word == "منهای")
            {
                return ManfiNumber;
            }
            if (word == "و")
            {
                return VaavNumber;
            }
            if (word == "ممیز")
            {
                return MomayezNumber;
            }

            return InvalidNumber;
        }

        /// <summary>
        /// Generates the floating part of a number from the values specified by the list of values starting from
        /// the specified floating point index.
        /// </summary>
        /// <param name="lstValues">The list of values.</param>
        /// <param name="dotIndex">Index of the floating dot.</param>
        /// <returns></returns>
        private static FloatingPartInfo GenerateFloatingPartFrom(List<long> lstValues, int dotIndex)
        {
            // TODO: maybe it's better to throw an exception
            if (dotIndex >= lstValues.Count - 1) return new FloatingPartInfo(0L, 0L);

            // indicates the number of values consumed to form the unit
            int consumedIndexes = 0;
            long unit = -1L;

            if (lstValues[lstValues.Count - 1] == MiimNumber)
            {
                unit = lstValues[lstValues.Count - 2];

                consumedIndexes = 2;
                if (MathUtils.IsPowerOfTen(unit) && unit >= 1000 && lstValues.Count - 3 > dotIndex + 1)
                {
                    if (lstValues[lstValues.Count - 3] == 10 || lstValues[lstValues.Count - 3] == 100)
                    {
                        unit *= lstValues[lstValues.Count - 3];
                        consumedIndexes = 3;
                    }
                }
            }

            try
            {
                if (dotIndex + 1 <= lstValues.Count - consumedIndexes - 1)
                {
                    long nominator = GenerateIntegralPartFrom(lstValues, dotIndex + 1, lstValues.Count - consumedIndexes - 1);

                    if (unit > 0)
                        return new FloatingPartInfo(nominator, unit);
                    else
                        return new FloatingPartInfo(nominator, Convert.ToInt64(Math.Pow(10, MathUtils.DigitCount(nominator))));
                }
                else
                {
                    throw new NumberSeperationException("Values cannot from a floating part", dotIndex, 0);
                }
            }
            catch (NumberSeperationException)
            {
                throw;
            }
            catch /* (Exception ex) */
            {
                return null;
            }

        }

        /// <summary>
        /// Creates a long integer number from the list of chunk-element values starting from
        /// the index specified by the <paramref name="from"/> parameter to the index specified by
        /// the <paramref name="to"/> parameter inclusively.
        /// </summary>
        /// <param name="lstValues">The list of values.</param>
        /// <param name="from">Index of the lower bound.</param>
        /// <param name="to">Index of the upper bound.</param>
        /// <returns></returns>
        private static long GenerateIntegralPartFrom(List<long> lstValues, int from, int to)
        {
            Debug.Assert(from <= to && from >= 0 && to >= 0);

            List<long> lstNumberPart = lstValues.GetRange(from, to - from + 1);


            long result = 0L;
            long threeDigitResult = 0L;
            int index = from - 1;
            // the number of non-deterministic numbers read 
            int nonDetReadCount = 0;
            int prevGoodGroup = 0;

            long curNumber;
            long nextNumber;

            bool isNegative = false;
            if (lstNumberPart[0] == ManfiNumber)
            {
                isNegative = true;
                nonDetReadCount++;
                if (!ReadTopNumber(lstNumberPart, out curNumber, ref index))
                {
                    throw new NumberSeperationException("Namana?", index, result);  // this should not happen
                }
            }

            while (lstNumberPart.Count > 0)
            {
                if (ReadTopNumber(lstNumberPart, out curNumber, ref index))
                {
                    if (curNumber < 0) // e.g. chunk beginning with vaav, or manfi vaav
                    {
                        throw new NumberSeperationException("Seperate from here", index - nonDetReadCount, (isNegative ? -1 : +1) * (result + threeDigitResult));
                    }
                    else
                    {
                        if (curNumber < 1000)
                        {
                            if (threeDigitResult != 0 && !AreNumbersAddable(threeDigitResult, curNumber)) // e.g. (10) , (12)
                            {
                                throw new NumberSeperationException("Nonaddable " + threeDigitResult + ", " + curNumber, index - nonDetReadCount - 1, (isNegative ? -1 : +1) * (result + threeDigitResult));
                            }
                            else if (curNumber == 0) // (1000) (&) (0)
                            {
                                throw new NumberSeperationException("Nonaddable " + threeDigitResult + ", " + curNumber, index - nonDetReadCount - 1, (isNegative ? -1 : +1) * (result + threeDigitResult));
                            }
                            threeDigitResult += curNumber;
                        }
                        else // if curNumber >= 1000
                        {
                            if (result != 0 && !AreNumbersAddable(threeDigitResult, curNumber))
                            {
                                throw new NumberSeperationException("Not addable groups " + result + ", " + curNumber, prevGoodGroup /* index */, (isNegative ? -1 : +1) * (result));
                            }
                            result += curNumber;
                            prevGoodGroup = index;
                        }

                        nonDetReadCount = 0;

                        if (ReadTopNumber(lstNumberPart, out nextNumber, ref index))
                        {
                            bool needToReadMore = false;
                            if (curNumber < 10 && nextNumber == 100) // e.g. 7 - 100
                            {
                                if (curNumber == 2 || curNumber == 3 || curNumber == 5)
                                {
                                    throw new NumberSeperationException("Numbers not multable: " + curNumber + ", " + nextNumber, index - 1, (isNegative ? -1 : +1) * (result));
                                }
                                else
                                {
                                    threeDigitResult -= curNumber;
                                    curNumber *= nextNumber;
                                    threeDigitResult += curNumber;
                                    needToReadMore = true;
                                }
                            }
                            else if (nextNumber >= 1000 && MathUtils.IsPowerOfTen(nextNumber))
                            {
                                if (threeDigitResult != 0)
                                {
                                    if (result != 0 && !AreNumbersAddable(result, threeDigitResult * nextNumber))
                                    {
                                        throw new NumberSeperationException("Not addable groups " + result + ", " + threeDigitResult * nextNumber, prevGoodGroup /* index */, (isNegative ? -1 : +1) * (result));
                                    }
                                    else
                                    {
                                        result += threeDigitResult * nextNumber;
                                        threeDigitResult = 0L;
                                    }
                                }
                                else
                                {
                                    result *= nextNumber;
                                }

                                prevGoodGroup = index;

                                needToReadMore = true;
                            }
                            else if (nextNumber != VaavNumber)
                            {
                                if (nextNumber >= 0)
                                {
                                    throw new NumberSeperationException("NonAddable " + nextNumber, index - 1, (isNegative ? -1 : +1) * (result + threeDigitResult));
                                }
                                else
                                {
                                    throw new NumberSeperationException("Met " + nextNumber, index - 1, (isNegative ? -1 : +1) * (result + threeDigitResult));
                                }
                            }
                            else
                            {
                                nonDetReadCount++;
                            }

                            while (needToReadMore)
                            {
                                if (ReadTopNumber(lstNumberPart, out curNumber, ref index))
                                {
                                    if (curNumber >= 1000 && MathUtils.IsPowerOfTen(curNumber))
                                    {
                                        result += threeDigitResult;
                                        threeDigitResult = 0L;
                                        result *= curNumber;
                                    }
                                    else if (curNumber >= 0)
                                    {
                                        throw new NumberSeperationException("Not mult-able " + curNumber, index -1 , (isNegative ? -1 : +1) * (result + threeDigitResult));
                                    }
                                    else 
                                    {
                                        if (curNumber != VaavNumber)
                                        {
                                            lstNumberPart.Insert(0, curNumber);
                                            index--;
                                        }
                                        else
                                        {
                                            nonDetReadCount++;
                                        }

                                        needToReadMore = false;
                                    }
                                }
                                else
                                {
                                    needToReadMore = false;
                                }
                            }
                        }

                    } // if curNumber >= 0
                } // if ReadTopNumber
            } // while

            if (nonDetReadCount > 0) // e.g. chunks ending in vaav
            {
                throw new NumberSeperationException("BadEnding", index - nonDetReadCount, (isNegative ? -1 : +1) * (result + threeDigitResult));
            }

            result += threeDigitResult;
            threeDigitResult = 0L;

            return (isNegative ? -1 : +1 )* result;
        }

        /// <summary>
        /// Reads and removes the top number from the list of values. 
        /// Returns the number read via an out parameter, and 
        /// updates the index variable from the caller context by incrementing it.
        /// </summary>
        /// <param name="lstValues">The list of values.</param>
        /// <param name="number">The number read.</param>
        /// <param name="outsideIndex">Index variable in the caller context.</param>
        /// <returns></returns>
        private static bool ReadTopNumber(List<long> lstValues, out long number, ref int outsideIndex)
        {
            number = 0L;
            if (lstValues.Count <= 0) return false;

            number = lstValues[0];
            lstValues.RemoveAt(0);

            outsideIndex++;

            return true;
        }

        /// <summary>
        /// Checks whether two numbers can be added if they come next to each other in the written form of a Persian number.
        /// e.g. 10 and 12 are not addable, 300 and 400 are not addable, but 300 and 50 are addable.
        /// </summary>
        /// <param name="big">The bigger number.</param>
        /// <param name="small">The smaller number.</param>
        /// <returns></returns>
        private static bool AreNumbersAddable(long big, long small)
        {
            if (big == 0 || small == 0 || small >= big)
                return false;

            if (big < 20) return false;

            long dividor = (long)Math.Pow(10, MathUtils.DigitCount(small));
            return (big % dividor == 0);
        }

        #region Commented Out

        //private static void RefineBeginning(List<long> lstValues)
        //{
        //    int count = 0;
        //    long curNumber;
        //    for (; count < lstValues.Count; ++count)
        //    {
        //        curNumber = lstValues[count];

        //        if (!(curNumber == VaavNumber || curNumber == InvalidNumber || curNumber == MiimNumber))
        //            break;
        //    }

        //    for (int i = count - 1; i >= 0; --i)
        //    {
        //        lstValues.RemoveAt(i);
        //    }
        //}

        //private static void RefineEnding(List<long> lstValues)
        //{
        //    long curNumber;
        //    for (int i = lstValues.Count - 1; i >= 0; i--)
        //    {
        //        curNumber = lstValues[i];
        //        if (curNumber == InvalidNumber || curNumber == VaavNumber || curNumber == ManfiNumber || curNumber == MomayezNumber)
        //            lstValues.RemoveAt(i);
        //        else
        //            break;
        //    }
        //}

        //protected bool CanExtractTwoIntegersFrom(List<long> lstTags, out long num1, out long num2)
        //{
        //    num1 = num2 = 0L;
        //    long[] allInts = ExtractAllIntsFromList(lstTags).ToArray();
        //    if (allInts.Length == 2)
        //    {
        //        num1 = allInts[0];
        //        num2 = allInts[1];
        //        return true;
        //    }
        //    else if (allInts.Length == 1) // e.g. 7-100 which is interpreted as 700, but can be 7-100th
        //    {
        //        if (lstTags.Count >= 2 && lstTags[lstTags.Count - 1] > 0 && lstTags[lstTags.Count - 2] > 0)
        //        {
        //            long[] remInts = ExtractAllIntsFromList(lstTags.GetRange(0, lstTags.Count - 1)).ToArray();
        //            if (remInts.Length == 1)
        //            {
        //                num1 = remInts[0];
        //                num2 = lstTags[lstTags.Count - 1];
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //private static string ProcessListTags(List<long> lstValues)
        //{
        //    if (lstValues.Count <= 0) return (0.0).ToString();

        //    if (lstValues[lstValues.Count - 1] == MiimNumber)
        //    {
        //        if (lstValues.Contains(MomayezNumber))
        //        {
        //            int dotIndex = lstValues.FindIndex(l => l == MomayezNumber);
        //            FloatingPartInfo pi = GenerateFloatingPartFrom(lstValues, dotIndex);
        //            return pi.ToString() + " --> " + pi.GetDoubleValue().ToString();
        //        }
        //        else
        //        {
        //            try
        //            {
        //                if(lstValues.Count > 0)
        //                    return GenerateIntegralPartFrom(lstValues, 0, lstValues.Count - 1).ToString();
        //            }
        //            catch (NumberSeperationException ex)
        //            {
        //                return String.Format("{0}\r\nindex:{1}\r\nParsedNumber:{2} ", ex.Message, ex.Index, ex.ParsedNumber);
        //            }
        //            catch (Exception ex)
        //            {
        //                return ex.ToString();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (lstValues.Count > 0)
        //                return GenerateIntegralPartFrom(lstValues, 0, lstValues.Count - 1).ToString();
        //        }
        //        catch (NumberSeperationException ex)
        //        {
        //            return String.Format("{0}\r\nindex:{1}\r\nparsedNumber:{2} ", ex.Message, ex.Index, ex.ParsedNumber);
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.ToString();
        //        }
        //    }

        //    return (0.0).ToString();
        //}

        #endregion
    }

    #region class GeneralNumberInfo

    /// <summary>
    /// Holds information about the content and location of the real numbers found.
    /// </summary>
    public class GeneralNumberInfo : IPatternInfo
    {
        /// <summary>
        /// Gets the content of the found pattern.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the integral part of the number
        /// </summary>
        /// <value>The integral part.</value>
        public long IntegralPart { get; private set; }

        /// <summary>
        /// Gets the floating part of the number (if any). Could be null.
        /// </summary>
        /// <value>The floating part.</value>
        public FloatingPartInfo FloatingPart { get; private set; }

        /// <summary>
        /// Gets the start index of the found pattern of the number.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the end index of the found pattern of the number.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Gets the list of chunk elements.
        /// </summary>
        /// <value>The list of chunk elements.</value>
        public List<ChunkElement> ListChunkElements { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the floating part of this instance is fraction. e.g. 1/3 (one third).
        /// If the return value is false, then the floating part is an ordinary floating number. e.g. 0.333
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fraction; otherwise, <c>false</c>.
        /// </value>
        public bool IsFraction { get { return FloatingPart != null && FloatingPart.Type == FloatingPartInfo.FloatingPartType.Fraction; } }

        /// <summary>
        /// Gets the string reperesentation of the floating part of the number shown as a fraction.
        /// The string returned by this property is suitable for showing a right-to-left context.
        /// </summary>
        /// <value>The fraction string.</value>
        public string FractionString
        {
            get
            {
                if (IsFraction)
                {
                    double fracValue = FloatingPart.GetDoubleValue();
                    var strFraction = new StringBuilder();

                    if (fracValue != 0.0)
                    {
                        // In right to left context denominator should be placed first so that it is observed at the right side of the numerator.
                        strFraction.AppendFormat("{0} / {1}", FloatingPart.Denominator, FloatingPart.Numerator);
                    }


                    if (IntegralPart != 0)
                    {
                        if (fracValue != 0.0)
                            strFraction.Append(" + ");

                        strFraction.Append(IntegralPart.ToString());
                    }


                    return strFraction.ToString();
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is invalid.
        /// Invalid instance holds just parsed chunk-elements which do not construct a valid number together.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is invalid; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvalid { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNumberInfo"/> class which holds a valid chunk of numbers.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="integralPart">The integral part.</param>
        /// <param name="floatingPart">The floating part.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="lstChunkElements">The list of chunk elements.</param>
        public GeneralNumberInfo(string content, long integralPart, FloatingPartInfo floatingPart, int startIndex, int endIndex, List<ChunkElement> lstChunkElements)
        {
            Debug.Assert(lstChunkElements != null && lstChunkElements.Count > 0);

            this.Content = content;
            this.IntegralPart = integralPart;
            this.FloatingPart = floatingPart;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.ListChunkElements = lstChunkElements;

            IsInvalid = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNumberInfo"/> class which holds an INVALID chunk of numbers.
        /// The content created by this constructor is used for debugging purposes.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="lstChunkElement">The list of chunk elements.</param>
        public GeneralNumberInfo(string content, int startIndex, int endIndex, List<ChunkElement> lstChunkElement)
        {
            Debug.Assert(lstChunkElement != null && lstChunkElement.Count > 0);

            this.Content = content;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.ListChunkElements = lstChunkElement;

            IsInvalid = true;
        }

        /// <summary>
        /// Gets the value for this instance of <see cref="GeneralNumberInfo"/>.
        /// </summary>
        /// <returns></returns>
        public double GetValue()
        {
            double value = IntegralPart;
            if (FloatingPart != null)
            {
                value += FloatingPart.GetDoubleValue();
            }
            return value;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="GeneralNumberInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="GeneralNumberInfo"/>.
        /// </returns>
        public override string ToString()
        {
            if (IsInvalid)
            {
                return String.Format("{1}{0}({2},{3}){0}{4}{0}{5}{0}------------------", Environment.NewLine,
                    Content, StartIndex, EndIndex, ChunkInfo.ListToString(ListChunkElements), "Invalid Number");
            }
            else
            {
                if (FloatingPart != null)
                {
                    return String.Format("{1}{0}({2},{3}){0}{4}{0}{5} &{0}{6}{0}-->  {7}{0}------------------", Environment.NewLine,
                        Content, StartIndex, EndIndex, ChunkInfo.ListToString(ListChunkElements), IntegralPart, FloatingPart, GetValue());
                }
                else
                {
                    return String.Format("{1}{0}({2},{3}){0}{4}{0}{5}{0}------------------", Environment.NewLine,
                        Content, StartIndex, EndIndex, ChunkInfo.ListToString(ListChunkElements), IntegralPart);
                }
            }
        }

        #region IPatternInfo Members

        /// <summary>
        /// Gets the type of the pattern info.
        /// </summary>
        /// <value>The type of the pattern info.</value>
        public PatternInfoTypes PatternInfoType
        {
            get { return PatternInfoTypes.PersianNumber; }
        }

        /// <summary>
        /// Gets the index of the original string at which the found pattern begins.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return StartIndex; }
        }

        /// <summary>
        /// Gets the length of the found pattern.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return Content.Length; }
        }

        #endregion
    }
    #endregion

    #region class GeneralNumberInfoComparer
    /// <summary>
    /// An implementation of IComparer to compare instances of <see cref="GeneralNumberInfo"/>.
    /// The comparison is mainly based upon the Numbers' location.
    /// </summary>
    public class GeneralNumberInfoComparer : IComparer<GeneralNumberInfo>
    {
        /// <summary>
        /// Compares two instances of <see cref="GeneralNumberInfo"/> and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// The comparison is mainly based upon the Numbers' location.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(GeneralNumberInfo x, GeneralNumberInfo y)
        {
            if (x.StartIndex != y.StartIndex)
                return x.StartIndex - y.StartIndex;
            else
            {
                int lenDiff = y.Content.Length - x.Content.Length;
                if (lenDiff != 0)
                {
                    return lenDiff;
                }
                else
                {
                    return Convert.ToInt32(y.IntegralPart - x.IntegralPart);
                }
            }
        }
    }
    #endregion

    #region class FloatingPartInfo

    /// <summary>
    /// Holds information about the Floating Part of a number, including its nominator and denominator.
    /// </summary>
    public class FloatingPartInfo
    {
        /// <summary>
        /// Gets the numerator.
        /// </summary>
        /// <value>The numerator.</value>
        public long Numerator { get; private set; }

        /// <summary>
        /// Gets the denominator.
        /// </summary>
        /// <value>The denominator.</value>
        public long Denominator { get; private set; }

        /// <summary>
        /// Gets the type of the floating part: fraction or floating.
        /// </summary>
        /// <value>The type of the floating part.</value>
        public FloatingPartType Type { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="FloatingPartInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="FloatingPartInfo"/>.
        /// </returns>
        public override string ToString()
        {
            if (Denominator == 0L)
                return "Illigal";

            if(Type == FloatingPartType.Floating)
            {
                return ((double)Numerator / Denominator).ToString();
            }
            else
            {
                return String.Format("{0} / {1}", Numerator, Denominator);
            }
        }

        /// <summary>
        /// Gets the double value for this instance of floating part.
        /// </summary>
        /// <returns></returns>
        public double GetDoubleValue()
        {
            return (double)Numerator / Denominator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingPartInfo"/> class.
        /// The value for denominator is deduced from the number of digits of the numerator.
        /// e.g. if numerator is 5, denominator would be 10, or if numerator is 123 then the denominator would be 1000.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        public FloatingPartInfo(long numerator) : this(numerator, (long)Math.Pow(10, MathUtils.DigitCount(numerator)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingPartInfo"/> class.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        public FloatingPartInfo(long numerator, long denominator)
        {
            this.Numerator = numerator;
            this.Denominator = denominator;

            if ((Numerator < Denominator) && MathUtils.IsPowerOfTen(Denominator))
            {
                Type = FloatingPartType.Floating;
            }
            else
            {
                Type = FloatingPartType.Fraction;
            }
        }

        /// <summary>
        /// Enumerates different types of Floating Part of a number
        /// </summary>
        public enum FloatingPartType
        {
            /// <summary>
            /// Having a decimal point and floating part numbers following. e.g. 0.3333
            /// </summary>
            Floating,
            /// <summary>
            /// Expressing floating part by a fraction e.g. 1/3 (one-third).
            /// </summary>
            Fraction
        }
    }
    #endregion

    #region class ChunkInfo

    /// <summary>
    /// Holds information about each chunk of possible real numbers in written form in Persian
    /// language. This information contains the content and location of the whole chunk, and the
    /// collection of information about all the elements of the chunks.
    /// </summary>
    internal class ChunkInfo
    {
        /// <summary>
        /// Gets the content of the chunk.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the chunk start index.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the chunk end index.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex    { get; private set; }

        /// <summary>
        /// Gets the list of chunk elements.
        /// </summary>
        /// <value>The list of chunk elements.</value>
        public List<ChunkElement> ListChunkElements { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkInfo"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="stIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="lstChunkElements">The List of chunk elements.</param>
        public ChunkInfo(string content, int stIndex, int endIndex, List<ChunkElement> lstChunkElements)
        {
            Debug.Assert(lstChunkElements != null && lstChunkElements.Count > 0);

            this.Content = content;
            this.StartIndex = stIndex;
            this.EndIndex = endIndex;
            this.ListChunkElements = lstChunkElements;
        }

        /// <summary>
        /// Returns the subset of the chunk, with elements starting at the specified index.
        /// </summary>
        /// <param name="startIndex">The index of the chunk element from which the new chunk will be made.</param>
        /// <returns></returns>
        public ChunkInfo GetSubChunk(int startIndex)
        {
            return GetSubChunk(startIndex, ListChunkElements.Count - 1);
        }

        /// <summary>
        /// Returns the subset of the chunk, with elements starting at the specified start index, and 
        /// finishing with the element at the specified end index.
        /// </summary>
        /// <param name="startIndex">The index of the chunk element from which the new chunk will be made.</param>
        /// <param name="endIndex">The index of the chunk element which form the upper bound of the sub-chunk.</param>
        /// <returns></returns>
        public ChunkInfo GetSubChunk(int startIndex, int endIndex)
        {
            Debug.Assert(startIndex >= 0 && endIndex >= 0 && startIndex <= endIndex);

            if (endIndex >= ListChunkElements.Count) endIndex = ListChunkElements.Count - 1;
            int strStartIndex = ListChunkElements[startIndex].StartIndex;
            int strEndIndex = ListChunkElements[endIndex].EndIndex;

            string strSubContent = this.Content.Substring(strStartIndex - this.StartIndex, strEndIndex - strStartIndex + 1);
            List<ChunkElement> subElements = ListChunkElements.GetRange(startIndex, endIndex - startIndex + 1);

            return new ChunkInfo(strSubContent, strStartIndex, strEndIndex, subElements);
        }

        /// <summary>
        /// Retrieves the index of the chunk element from the specified index of the chunk-element values.
        /// Note that chunk-element indeces and chunk-element value indices are not one to one.
        /// e.g. "نیم" comprises one chunk-element but 4 chunk-element values: { MOMAYEZ, 5, 10, MIIM }.
        /// </summary>
        /// <param name="n">The chunk-element VALUES index.</param>
        /// <returns></returns>
        public int GetChunkIndexFromValueIndex(int n)
        {
            Debug.Assert(n >= 0);

            int sum = 0;
            for (int i = 0; i < ListChunkElements.Count; i++)
            {
                if (sum <= n && n < ListChunkElements[i].ElementValues.Length + sum)
                    return i;
                sum += ListChunkElements[i].ElementValues.Length;
            }

            return ListChunkElements.Count;    
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="ChunkInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="ChunkInfo"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{1}{0}({2},{3}){0}{4}{0}-----------------", 
                Environment.NewLine, Content, StartIndex, EndIndex, ListToString(ListChunkElements)); 
        }

        /// <summary>
        /// Gets all chunk element values.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<long> GetAllChunkElementValues()
        {
            foreach (ChunkElement elem in ListChunkElements)
            {
                foreach (long l in elem.ElementValues)
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// Utility function that converts a sequence of values to string.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="seq">The sequence of values to be converted to string.</param>
        /// <returns></returns>
        public static string ListToString<T>(IEnumerable<T> seq)
        {
            var sb = new StringBuilder();

            foreach (var t in seq)
            {
                sb.AppendFormat("{0}", t);
            }
            return sb.ToString();
        }
    }
    #endregion

    #region class ChunkElement

    /// <summary>
    /// Holds information about each building block of a real number in written form 
    /// and their location. 
    /// That would be the digits, and the symbols such as vaav, [ordinal] miim, momayez
    /// (i.e. floating point) and so on.
    /// </summary>
    public class ChunkElement
    {
        /// <summary>
        /// Gets an array of the value of the elements.
        /// </summary>
        /// <value>The element values.</value>
        public long[] ElementValues { get; private set; }

        /// <summary>
        /// Gets or sets the start index at which the chunk element has been met.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the end index of the chunk element.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Gets the content of the chunk element.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkElement"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="elementValues">The element values.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public ChunkElement(string content, long[] elementValues, int startIndex, int endIndex)
        {
            Debug.Assert(elementValues != null && elementValues.Length > 0);

            this.Content = content;
            this.ElementValues = elementValues;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkElement"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="elementValue">The element value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public ChunkElement(string content, long elementValue, int startIndex, int endIndex)
            :this(content, new [] {elementValue}, startIndex, endIndex)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is numeric, i.e. the chunk-element itself is 
        /// made of digits.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is numeric; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumeric
        {
            get
            {
                return Char.IsDigit(Content[0]);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="ChunkElement"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="ChunkElement"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (long l in ElementValues)
            {
                sb.AppendFormat("({0})", l);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the specified chunk element is equal to this instance.
        /// </summary>
        /// <param name="c">The chunk element to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified chunk element is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEqualTo(ChunkElement c)
        {
            return AreEqual(this, c);
        }

        /// <summary>
        /// Determines whether two chunk elements are equal.
        /// Two chunk elements are considered to be equal if they have the same content
        /// and are at the same location.
        /// </summary>
        /// <param name="a">The 1st chunk-element.</param>
        /// <param name="b">The 2nd chunk-element.</param>
        /// <returns></returns>
        public static bool AreEqual(ChunkElement a, ChunkElement b)
        {
            if (Object.Equals(a, b))
                return true;

            if (a.StartIndex == b.StartIndex &&
                a.EndIndex == b.EndIndex &&
                a.Content == b.Content)
                return true;
            else
                return false;
        }

    }
    #endregion

    #region Exception class NumberSeperationException

    /// <summary>
    /// An Exception class used internally by the <see cref="PersianRealNumberParser"/> class to process real number parsing.
    /// </summary>
    internal class NumberSeperationException : Exception
    {
        /// <summary>
        /// Gets the index at which the chunk should divided into two chunks.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the number parsed so far, right before meeting the illegal chunk element.
        /// </summary>
        /// <value>The parsed number.</value>
        public long ParsedNumber { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberSeperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="index">The index at which the chunk should divided into two chunks.</param>
        /// <param name="number">The number parsed so far.</param>
        public NumberSeperationException(string message, int index, long number)
            : base(message)
        {
            this.Index = index;
            this.ParsedNumber = number;
        }
    }

    #endregion

    #region static class ArrayExtensions

    /// <summary>
    /// Provides extension methods for Array and List classes to make it more handy working with.
    /// </summary>
    public static class ListAndArrayExtensions
    {
        /// <summary>
        /// Returns the last element of an array.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source array to work with.</param>
        /// <returns></returns>
        public static TSource GetLastElement<TSource>(this TSource[] source)
        {
            return source[source.Length - 1];
        }

        /// <summary>
        /// Sets the last element of an array to the specified value.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source array to work with.</param>
        /// <param name="value">The value to be assigned to the last element of the array.</param>
        public static void SetLastElement<TSource>(this TSource[] source, TSource value)
        {
            source[source.Length - 1] = value;
        }

        /// <summary>
        /// Returns the last element of a list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source List instance to work with.</param>
        /// <returns></returns>
        public static TSource GetLastElement<TSource>(this List<TSource> source)
        {
            return source[source.Count - 1];
        }

        /// <summary>
        /// Sets the last element of a List to the specified value.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source array to work with.</param>
        /// <param name="value">The value to be assigned to the last element of the List.</param>
        public static void SetLastElement<TSource>(this List<TSource> source, TSource value)
        {
            source[source.Count - 1] = value;
        }

        /// <summary>
        /// Finds all indeces at which the element of the specified List equals the specified key.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source List to work with.</param>
        /// <param name="key">The key value to look for in the List.</param>
        /// <returns></returns>
        public static IEnumerable<int> FindAllIndeces<TSource>(this List<TSource> source, TSource key)
        {
            var indexes = new List<int>();
            for (int i = 0; i < source.Count; ++i)
            {
                if (source[i].Equals(key))
                    indexes.Add(i);
            }

            return indexes;
        }
    }
    #endregion
}

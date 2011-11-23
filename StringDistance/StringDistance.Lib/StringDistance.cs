// Author: Omid Kashefi, Mitra Nasri 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi at 2010-March-08
//

using System;
using SCICT.NLP.Persian.Constants;
using SCICT.Utility;

namespace SCICT.NLP.Utility.StringDistance
{
    class NeedlemanConfig
    {
        private readonly KeyboardKeyDistance m_keyboard;
        public KeyboardKeyDistance  Keyboard 
        {   
            get 
            {
                return this.m_keyboard;
            }
        }
        
        private readonly double m_gapCost = 1.0;
        public double GapCost 
        {
            get
            {
                return this.m_gapCost;
            }
        }

        private readonly double m_needlemanMaxSubstituteRange = 2.0;
        public double NeedlemanMaxSubstituteRange 
        {
            get
            {
                return this.m_needlemanMaxSubstituteRange;
            }
        }

        public NeedlemanConfig(KeyboardKeyDistance keyboardKeyDistance, double needlemanGapCost, double needlemanMaxSubstituteRange)
        {
            this.m_needlemanMaxSubstituteRange = needlemanMaxSubstituteRange;
            this.m_gapCost                     = needlemanGapCost;
            this.m_keyboard                    = keyboardKeyDistance;
        }
        public NeedlemanConfig()
        {
            this.m_keyboard = new KeyboardKeyDistance();
        }
    }

    ///<summary>
    /// Kashefi String Distnace Metric Configuration Class
    ///</summary>
    public class KashefiConfig
    {
        private readonly KeyboardKeyDistance m_keyboard;
        ///<summary>
        /// Define keyboard layout
        ///</summary>
        public KeyboardKeyDistance Keyboard
        {
            get
            {
                return this.m_keyboard;
            }
        }

        private readonly double m_insertGapCost = .679;
        ///<summary>
        /// Gap Cost of Mistakanly Insertaion of a letter
        ///</summary>
        public double InsertGapCost
        {
            get
            {
                return this.m_insertGapCost;
            }
        }

        private readonly double m_deleteGapCost = .569;
        ///<summary>
        /// Gap Cost of Mistakenly Omission of a letter
        ///</summary>
        public double DeleteGapCost
        {
            get
            {
                return this.m_deleteGapCost;
            }
        }

        private readonly double m_transpositionGapCost = .725;
        ///<summary>
        /// Gap Cost of Mistakenly Transposition of two adjacent letter
        ///</summary>
        public double TranspositionGapCost
        {
            get
            {
                return this.m_transpositionGapCost;
            }
        }

        private readonly double m_substituteGapCost = .367;
        ///<summary>
        /// Maximum Cost of Substitution of Two Letters
        ///</summary>
        public double SubstituteGapCost
        {
            get
            {
                return this.m_substituteGapCost;
            }
        }

        ///<summary>
        /// Class Constructor
        ///</summary>
        ///<param name="keyboardKeyDistance">Keboard Layout</param>
        ///<param name="kashefiInsertGapCost">Gap Cost of Mistakanly Insertaion of a letter</param>
        ///<param name="kashefiDeleteGapCost">Gap Cost of Mistakenly Omission of a letter</param>
        ///<param name="kashefiMaxSubstituteRange">Maximum Cost of Substitution of Two Letters</param>
        ///<param name="kashefiTransCost">Transposition Cost</param>
        public KashefiConfig(KeyboardKeyDistance keyboardKeyDistance, double kashefiInsertGapCost, double kashefiDeleteGapCost, double kashefiMaxSubstituteRange, double kashefiTransCost)
        {
            this.m_substituteGapCost = kashefiMaxSubstituteRange;
            this.m_insertGapCost = kashefiInsertGapCost;
            this.m_deleteGapCost = kashefiDeleteGapCost;
            this.m_transpositionGapCost = kashefiTransCost;

            this.m_keyboard = keyboardKeyDistance;
        }
        ///<summary>
        /// Class Constructor
        /// Set Default values
        ///</summary>
        public KashefiConfig()
        {
            this.m_keyboard = new KeyboardKeyDistance();
        }
    }

    static class StringDistanceAlgorithms
    {
        # region static variables

        private static bool s_exportResultAsSimilarity = true;
        public static bool ExportResultAsSimilarity
        {
            get { return s_exportResultAsSimilarity; }
            set { s_exportResultAsSimilarity = value; }
        }
                     	        
        #endregion
        
        /// <summary>
        /// The Hamming distance H is defined only for strings of the same length. 
        /// For two strings s and t, H(s, t) is the number of places in which the two string differ, i.e., have different characters.
        /// ref: http://www.cut-the-knot.org/do_you_know/Strings.shtml
        /// We also add difference of length of two strings to result.
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        public static double Hamming(string word1, string word2)
        {
            // this algorithm normally computes un-normalized distance between two string.
            int difference = 0;

            // iStep 1: count differing characters:
            for (int counter1 = 0, counter2 = 0; (counter1 < word1.Length) && (counter2 < word2.Length); counter1++, counter2++)
            {
                if (word1[counter1] != word2[counter2])
                    difference++;
            }
            // iStep 2: add |len2-len1| to difference variable:
            difference += Math.Abs(word1.Length - word2.Length);

            return ExportResult(difference, word1.Length, word2.Length, false);
        }

        /// <summary>
        /// Cosine similarity is a common vector based similarity measure similar to dice coefficient. 
        /// Whereby the input string is transformed into vector space so that the Euclidean cosine rule can be used to determine similarity. 
        /// The cosine similarity is often paired with other approaches to limit the dimensionality of the problem. 
        /// For instance with simple strings at list of stopwords are used to exclude from the dimensionality of the comparison.
        /// ref: http://www.dcs.shef.ac.uk/~sam/stringmetrics.html#cosine
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        public static double Cosine(string word1, string word2)
        {
            // this algorithm normally computes normalized similarity between two string.                      
            string  uniqueCharacters = "" ;

            for (int i = 0; i < word1.Length; i++)
            {
                if (uniqueCharacters.IndexOf(word1[i]) < 0)
                    uniqueCharacters += word1[i]; 
            }

            int uniqueCharactersOfWord1 = uniqueCharacters.Length;

            for (int i = 0; i < word2.Length; i++)
            {
                if (uniqueCharacters.IndexOf(word2[i]) < 0)
                    uniqueCharacters += word2[i];
            }

            int uniqueCharactersOfBothWords = uniqueCharacters.Length;

            // uniquCharacter variable is not useful later. so we use it to count uniqu characters of string 2
            uniqueCharacters = "";

            for (int i = 0; i < word2.Length; i++)
            {
                if (uniqueCharacters.IndexOf(word2[i]) < 0)
                    uniqueCharacters += word2[i];
            }

            int uniqueCharactersOfWord2 = uniqueCharacters.Length;

            int commonTerms = uniqueCharactersOfWord1 + uniqueCharactersOfWord2 - uniqueCharactersOfBothWords;
            // iStep 2: add |len2-len1| to difference variable:
            double similarity = commonTerms / (Math.Sqrt(uniqueCharactersOfWord1) * Math.Sqrt(uniqueCharactersOfWord2));


            return ExportResult(similarity, true);
        }
        
        /// <summary>
        /// The Levenshtein distance between two strings is given by the minimum number of operations needed to transform one string into the other, 
        /// where an operation is an insertion, deletion, or substitution of a single character.
        /// ref: http://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        public static double Levenstein(string word1, string word2)
        {
            // this algorithm normally computes un-normalized distance between two string.
            int len1 = word1.Length;
            int len2 = word2.Length;
            int[,] distance = new int[len1 + 1, len2 + 1];

            for (int i = 0; i < len1 + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int i = 0; i < len2 + 1; i++)
            {
                distance[0, i] = i;
            }
            
            int cost;
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    cost = (word1[i] == word2[j]) ? 0 : 1;

                    distance[i + 1, j + 1] = Minimum(
                        distance[i, j + 1] + 1, // deletion
                        distance[i + 1, j] + 1, // insertion
                        distance[i, j] + cost // substitution
                        );
                }
            }
            double levensteinResult = distance[len1, len2];

            // normalize result:
            return ExportResult( levensteinResult ,len1, len2, false);
        }

        /// <summary>
        /// The Levenshtein distance between two strings is given by the minimum number of operations needed to transform one string into the other, 
        /// where an operation is an insertion, deletion, or substitution of a single character.
        /// ref: http://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        public static double DamerauLevenstein(string word1, string word2)
        {
            // this algorithm normally computes un-normalized distance between two string.
            int len1 = word1.Length;
            int len2 = word2.Length;
            int[,] distance = new int[len1 + 1, len2 + 1];

            for (int i = 0; i < len1 + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int i = 0; i < len2 + 1; i++)
            {
                distance[0, i] = i;
            }

            
            int cost;
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    cost = (word1[i] == word2[j]) ? 0 : 1;

                    int transposeCost = 2;
                    if (j > 0 && i > 0)
                    {
                        if (word1[i] == word2[j - 1] && word1[i - 1] == word2[j])
                        {
                            transposeCost = 1;
                        }

                        distance[i + 1, j + 1] = Minimum(
                            distance[i, j + 1] + 1, // deletion
                            distance[i + 1, j] + 1, // insertion
                            distance[i, j] + cost, // substitution
                            distance[i - 1, j - 1] + transposeCost);

                    }
                    else
                    {
                        distance[i + 1, j + 1] = Minimum(
                            distance[i, j + 1] + 1, // deletion
                            distance[i + 1, j] + 1, // insertion
                            distance[i, j] + cost);
                    }
                }
            }
            double levensteinResult = distance[len1, len2];

            // normalize result:
            return ExportResult(levensteinResult, len1, len2, false);
        }

        /// <summary>
        /// The Wagner-Fischer distance between two strings is given by the minimum number of operations needed to transform one string into the other, 
        /// where an operation is an insertion, deletion, or substitution of a single character.
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        public static double WagnerFischer(string word1, string word2)
        {
            // this algorithm normally computes un-normalized distance between two string.
            int len1 = word1.Length;
            int len2 = word2.Length;
            double[,] distance = new double[len1 + 1, len2 + 1];

            for (int i = 0; i < len1 + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int i = 0; i < len2 + 1; i++)
            {
                distance[0, i] = i;
            }


            double cost;
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    cost = (word1[i] == word2[j]) ? 0 : .367;

                    double transposeCost = 2;
                    if (j > 0 && i > 0)
                    {
                        if (word1[i] == word2[j - 1] && word1[i - 1] == word2[j])
                        {
                            transposeCost = .733;
                        }

                        distance[i + 1, j + 1] = Minimum(
                            distance[i, j + 1] + .569, // deletion
                            distance[i + 1, j] + .679, // insertion
                            distance[i, j] + cost, // substitution
                            distance[i - 1, j - 1] + transposeCost);

                    }
                    else
                    {
                        distance[i + 1, j + 1] = Minimum(
                            distance[i, j + 1] + .8, // deletion
                            distance[i + 1, j] + .9, // insertion
                            distance[i, j] + cost);
                    }
                }
            }
            double levensteinResult = distance[len1, len2];

            // normalize result:
            return ExportResult(levensteinResult, len1, len2, false);
        }

        /// <summary>
        /// The Levenshtein distance between two strings is given by the minimum number of operations needed to transform one string into the other, 
        /// where an operation is an insertion, deletion, or substitution of a single character.
        /// ref: http://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        /// <returns>Not normalized result obtain here</returns>
        public static unsafe double GNULevenstein(string word1, string word2)
        {
            // this algorithm normally computes un-normalized distance between two string.
            fixed (char* word1Ptr = word1)
            fixed (char* word2Ptr = word2)
            {
                char* pointerToWord1 = word1Ptr;
                char* pointerToWord2 = word2Ptr;
                
                /* skip equal start sequence, if any */
                if (word1.Length >= word2.Length)
                {
                    while (*pointerToWord1 == *pointerToWord2)
                    {
                        /* if we already used up one string,
                         * then the result is the length of the other */
                        if (*pointerToWord1 == '\0') break;
                        pointerToWord1++; 
                        pointerToWord2++;
                    }
                }
                else // wordl < word2
                {
                    while (*pointerToWord1 == *pointerToWord2)
                    {
                        /* if we already used up one string,
                         * then the result is the length of the other */
                        if (*pointerToWord2 == '\0') break;
                        pointerToWord1++; 
                        pointerToWord2++;
                    }
                }

                /* length count #1*/
                int len1 = word1.Length - (int)(pointerToWord1 - word1Ptr);
                int len2 = word2.Length - (int)(pointerToWord2 - word2Ptr);

                
                /* if we already used up one string, then
                 the result is the length of the other */
                if (*pointerToWord1 == '\0') 
                    return ExportResult( len2 , word1.Length,word2.Length , false);
                if (*pointerToWord2 == '\0')
                    return ExportResult(len1, word1.Length, word2.Length, false);

                /* length count #2*/
                pointerToWord1 += len1;
                pointerToWord2 += len2;

                /* cut of equal tail sequence, if any */
                while (*--pointerToWord1 == *--pointerToWord2)
                {
                    len1--; 
                    len2--;
                }

                /* reset pointers, adjust length */
                pointerToWord1 -= len1++;
                pointerToWord2 -= len2++;

                /* possible dist to great? */
                //if ((len1 - len2 >= 0 ? len1 - len2 : -(len1 - len2)) >= char.MaxValue) return 1;
                if (Math.Abs(len1 - len2) >= char.MaxValue)
                    return ExportResult(1, false);  // no similarity

                char* tmp;
                /* swap if l2 longer than l1 */
                if (len1 < len2)
                {
                    tmp = pointerToWord1; 
                    pointerToWord1 = pointerToWord2; 
                    pointerToWord2 = tmp;
                    len1 ^= len2; 
                    len2 ^= len1; 
                    len1 ^= len2;
                }

                /* fill initial row */
                
                int i, j, n;

                n = (*pointerToWord1 != *pointerToWord2) ? 1 : 0;
                char* r = stackalloc char[len1 * 2];

                char* p1, p2;
                for (i = 0, p1 = r; i < len1; i++, *p1++ = (char)n++, p1++) 
                { /*empty*/}


                /* calc. rowwise */
                for (j = 1; j < len2; j++)
                {
                    /* init pointers and col#0 */
                    p1 = r + ((j & 1) == 0 ? 1 : 0);
                    p2 = r + (j & 1);
                    n = *p1 + 1;
                    *p2++ = (char)n; p2++;
                    pointerToWord2++;

                    /* foreach column */
                    for (i = 1; i < len1; i++)
                    {
                        if (*p1 < n) n = *p1 + (*(pointerToWord1 + i) != *pointerToWord2 ? 1 : 0); /* replace cheaper than delete? */
                        p1++;
                        if (*++p1 < n) n = *p1 + 1; /* insert cheaper then insert ? */
                        *p2++ = (char)n++; /* update field and cost for next col's delete */
                        p2++;
                    }
                }

                /* return result */
                return ExportResult( n - 1, word1.Length, word2.Length, false);
            }

            
        }
               
        /// <summary>
        /// The Jaro distance metric takes into account typical spelling deviations, this work comes from the following paper.
        /// Jaro, M. A. 1989 "Advances in record linking methodology as applied to the 1985 census of Tampa Florida". Journal of the American Statistical Society 64:1183-1210 
        /// Briefly, for two strings s and t, let s' be the characters in s that are “common with” t, and let t' be the charcaters in t that are "common with" s; 
        /// roughly speaking, a character a in s is “in common” with t if the same character a appears in about the place in t.
        /// Note that Jaro result is Normalized. 1 means maximum similarity, 0 means maximum difference.
        /// ref:  http://en.wikipedia.org/wiki/Jaro-Winkler_distance
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        public static double JaroWinckler(string word1, string word2)
        {
            // this algorithm normally computes normalized similarity between two string.
            int len1 = word1.Length;
            int len2 = word2.Length;

            //get half the length of the string rounded up - (this is the distance used for acceptable transpositions)
            int halflen = ((Math.Min(len1, len2)) / 2) + ((Math.Min(len1, len2)) % 2);

            string common1 = GetCommonCharactersForJaroMethod(word1, word2, halflen);
            string common2 = GetCommonCharactersForJaroMethod(word2, word1, halflen);

            //check for zero in common (no similarity)
            if (common1.Length == 0 || common2.Length == 0)
                return 0.0;

            //check for same length common strings returning 0.0f is not the same
            if (common1.Length != common2.Length)
                return 0.0;

            //get the number of transpositions
            double transpositions = 0;
            for (int i = 0; i < common1.Length; i++)
            {
                if (common1[i] != common2[i])
                    transpositions++;
            }
            transpositions /= 2.0;

            double m = common1.Length;

            //calculate jaro metric
            double similarity = (m / len1 + m / len2 + (m - transpositions) / m) / 3.0;

            return ExportResult(similarity , true);
        }

        /// <summary>
        /// compute number of common characters and transpositions:
        /// </summary>
        /// <param name="s1">first string</param>
        /// <param name="s2">second string</param>
        /// <param name="windowLen">windows length</param>
        /// <returns>common characters string</returns>
        private static string GetCommonCharactersForJaroMethod(string s1, string s2, int windowLen)
        {
            //int windowLen = Math.Min (3 , ((int)( Math.Max(len1 , len2)/2.0))-1); // window len = floor[max(l1,l2)/2]-1 OR  3
            string commonString = "";

            // create a copy of string 2:
            string copy = s2;

            for (int i = 0; i < s1.Length; i++)
            {
                char ch = s1[i];

                //set boolean for quick loop exit if found
                bool foundIt = false;

                //compare char with range of characters to either side
                for (int j = Math.Max(0, i - windowLen); !foundIt && j <= Math.Min(i + windowLen, s2.Length - 1); j++)
                {
                    //check if found
                    if (copy[j] == ch)
                    {
                        foundIt = true;

                        //append character found
                        commonString = commonString + ch;

                        //alter copied s2 for processing
                        copy = copy.Remove(j, 1).Insert(j, "-");
                    }
                }
            }
            return commonString;
        }

        /// <summary>
        /// TThis approach is known by various names, Needleman-Wunch, Needleman-Wunch-Sellers, Sellers and the Improving Sellers algorithm. 
        /// This is similar to the basic edit distance metric, Levenshtein distance, this adds an variable cost adjustment to the cost of a gap, i.e. insert/deletion, in the distance metric. 
        /// So the Levenshtein distance can simply be seen as the Needleman-Wunch distance with G=1.
        /// Where G = “gap cost” and SubstitutionCost is again an arbitrary distance function on characters (e.g. related to typographic frequencies, amino acid substitutibility, etc). 
        /// ref:  http://www.dcs.shef.ac.uk/~sam/stringmetrics.html#needleman
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        /// <param name="needlemanConfig">NeedlemanConfig</param>
        /// <returns> Normalized similarity between [0..1]. 0 means minimum similarity and 1 means maxumim smilarity</returns>
        public static double NeedlemanWunch(string word1, string word2, NeedlemanConfig needlemanConfig)
        {
            // this algorithm normally computes normalized similarity between two string.
            double needlemanWunch = GetUnNormalisedSimilarityForNeedleman(word1, word2, needlemanConfig);

            //normalize into zero to one region from min max possible
            double maxValue = Math.Max(word1.Length, word1.Length);

            if (needlemanConfig.NeedlemanMaxSubstituteRange > needlemanConfig.GapCost)
                maxValue *= needlemanConfig.NeedlemanMaxSubstituteRange;
            else
                maxValue *= needlemanConfig.GapCost;

            //check for 0 maxLen
            if (maxValue == 0)
            {
                return ExportResult(1.0, true); //as both strings identically zero length
            }
            else
            {
                return ExportResult(needlemanWunch / maxValue, false);
            }

        }
        private static double ComputeNeedlemanSubstitutionCost(char p1, char p2, NeedlemanConfig needlemanConfig)
        {
            // we can use normalized euclidean distance between characters of keyboard here.
            // but cost of insertion and deletion errors should be specified according to substitution. 

            // euclidean distance is normalized between [0..1].
            return needlemanConfig.NeedlemanMaxSubstituteRange * needlemanConfig.Keyboard.NormalizedEuclideanDistance(p1, p2);
        }
        private static double GetUnNormalisedSimilarityForNeedleman(string word1, string word2, NeedlemanConfig needlemanConfig)
        {
            // this algorithm normally computes un-normalized distance between two string.
            int len1 = word1.Length;
            int len2 = word2.Length;
            double[,] distance = new double[len1 + 1, len2 + 1];

            for (int i = 0; i < len1 + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int i = 0; i < len2 + 1; i++)
            {
                distance[0, i] = i;
            }

            double substitutionCost;
            double gapCost = needlemanConfig.GapCost;

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    substitutionCost = ComputeNeedlemanSubstitutionCost(word1[i], word2[j], needlemanConfig);

                    distance[i + 1, j + 1] = Minimum(
                                        distance[i, j + 1] + gapCost,       // deletion
                                        distance[i + 1, j] + gapCost,       // insertion
                                        distance[i, j] + substitutionCost   // substitution
                                    );
                }
            }
            double result = (float)distance[len1, len2];

            // to normalized this result you should use Needleman normalization function directly, 
            // because normal normalization functions do not work to normalize this result!

            return result;
        }

        /// <summary>
        /// This is a New Perisan String Distance Metric Based on Needleman and Levenstein Similarity Metric.
        /// </summary>
        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>

        /// <param name="word1">First word</param>
        /// <param name="word2">Second word</param>
        /// <param name="kashefiConfig">KashefiConfig</param>
        /// <returns> Normalized similarity between [0..1]. 0 means minimum similarity and 1 means maxumim smilarity</returns>
        public static double KashefiMeasure(string word1, string word2, KashefiConfig kashefiConfig)
        {
            // this algorithm normally computes normalized similarity between two string.
            double kashefiMeasure = GetUnNormalisedSimilarityForKashefiMeasure(word1, word2, kashefiConfig);

            //normalize into zero to one region from min max possible

            double maxValue = Math.Min(kashefiConfig.SubstituteGapCost, kashefiConfig.TranspositionGapCost) * Math.Min(word1.Length, word2.Length);
            maxValue += Math.Max(kashefiConfig.InsertGapCost, kashefiConfig.DeleteGapCost) * (Math.Max(word1.Length, word2.Length) - Math.Min(word1.Length, word2.Length));

            //check for 0 maxLen
            if (maxValue == 0)
            {
                return ExportResult(1.0, true); //as both strings identically zero length
            }
            else
            {
                //return ExportResult(kashefiMeasure / maxValue, false);
                return ExportResult(kashefiMeasure / Math.Max(word1.Length, word2.Length), false);
            }

        }
        private static double GetUnNormalisedSimilarityForKashefiMeasure(string word1, string word2, KashefiConfig kashefiConfig)
        {
            // this algorithm normally computes un-normalized distance between two string.
            int len1 = word1.Length;
            int len2 = word2.Length;
            double[,] distance = new double[len1 + 1, len2 + 1];

            for (int i = 0; i < len1 + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int j = 0; j < len2 + 1; j++)
            {
                distance[0, j] = j;
            }

            double substitutionCost = 0;
            double insertGapCost = kashefiConfig.InsertGapCost;
            double deleteGapCost = kashefiConfig.DeleteGapCost;
            double transposeCost;

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    //substitutionCost = 0;

                    //if (i == j)
                    //{
                    //    substitutionCost = ComputeKashefiSubstitutionCost(word1[i], word2[j], kashefiConfig);
                    //}

                    substitutionCost = ComputeKashefiSubstitutionCost(word1[i], word2[j], kashefiConfig);


                    if ((j > 0 && i > 0))
                    {
                        transposeCost = ComputeKashefiTranspositionCost(word1, i, word2, j, kashefiConfig);
                        
                        insertGapCost = ComputeKashefiInsertionCost(word1, i, word2, j, kashefiConfig);

                        deleteGapCost = ComputeKashefiDeletionCost(word1, i, word2, j, kashefiConfig);

                        distance[i + 1, j + 1] = Minimum(
                                distance[i, j + 1] + deleteGapCost,       // deletion
                                distance[i + 1, j] + insertGapCost,       // insertion
                                distance[i, j] + substitutionCost,   // substitution
                                distance[i - 1, j - 1] + transposeCost);

                    }
                    else
                    {
                        distance[i + 1, j + 1] = Minimum(
                                            distance[i, j + 1] + deleteGapCost,       // deletion
                                            distance[i + 1, j] + insertGapCost,       // insertion
                                            distance[i, j] + substitutionCost);
                    }
                }
            }
            double result = (float)distance[len1, len2];

            // to normalized this result you should use Needleman normalization function directly, 
            // because normal normalization functions do not work to normalize this result!

            return result;
        }

        private static double ComputeKashefiDeletionCost(string word1, int i, string word2, int j, KashefiConfig kashefiConfig)
        {
            double deletionCost = kashefiConfig.DeleteGapCost;

            if (Array.IndexOf(PersianHomophoneLetters.HamzaFamily1, word2[j]) > -1)
            {
                    deletionCost = .1;
            }

            return deletionCost;

        }
        private static double ComputeKashefiTranspositionCost(string word1, int i, string word2, int j, KashefiConfig kashefiConfig)
        {
            double transpositionCost = 1;
            if (word1[i] == word2[j - 1] && word1[i - 1] == word2[j])
            {
                transpositionCost = kashefiConfig.TranspositionGapCost;
            }

            return transpositionCost;
        }
        private static double ComputeKashefiInsertionCost(string word1, int i, string word2, int j, KashefiConfig kashefiConfig)
        {
            double insertionCost = kashefiConfig.InsertGapCost;
            //if (word1[i] == word2[j-1])
            //{
            //    if (word2.Substring(j, 1).IsIn(PersianAlphabets.Diacritics.ToStringArray()))
            //    {
            //        insertionCost = .1;
            //    }
            //}

            if (word2.Substring(j, 1).IsIn(PersianAlphabets.Diacritics.ToStringArray()))
            {
                insertionCost = .1;
            }
            if (Array.IndexOf(PersianHomophoneLetters.HamzaFamily1, word2[j]) > -1)
            {
                insertionCost = .1;
            }
         
            return insertionCost;
        }
        private static double ComputeKashefiSubstitutionCost(char p1, char p2, KashefiConfig kashefiConfig)
        {
            if (p1 == p2)
            {
                return 0;
            }

            if (PersianHomophoneLetters.AreHomophone(p1, p2))
            {
                return 0.236;
            }

            if (PersianHomoshapeLetters.AreHomoshape(p1, p2))
            {
                return 0.369;
            }

            double substitutionCost = kashefiConfig.Keyboard.NormalizedEuclideanDistance(p1, p2);
                
            substitutionCost = substitutionCost * (1 - kashefiConfig.SubstituteGapCost) + kashefiConfig.SubstituteGapCost;

            return substitutionCost;
        }

        #region Private utilities

        private static int Minimum(int a, int b, int c)
        {
            int min = Math.Min(a, b);
            min = Math.Min(min, c);

            //min = a;
            //if (b < min) min = b;
            //if (c < min) min = c;

            return min;
        }
        private static int Minimum(int a, int b, int c, int d)
        {
            int min = Math.Min(a, b);
            min = Math.Min(min, c);
            min = Math.Min(min, d);

            //min = a;
            //if (b < min) min = b;
            //if (c < min) min = c;

            return min;
        }
        private static double Minimum(double a, double b, double c)
        {
            double min = Math.Min(a, b);
            min = Math.Min(min, c);

            //min = a;
            //if (b < min) min = b;
            //if (c < min) min = c;

            return min;
        }
        private static double Minimum(double a, double b, double c, double d)
        {
            double min = Math.Min(a, b);
            min = Math.Min(min, c);
            min = Math.Min(min, d);

            //min = a;
            //if (b < min) min = b;
            //if (c < min) min = c;

            return min;
        }

        private static int Maximum(int a, int b, int c)
        {
            int max = Math.Max(a, b);
            max = Math.Max(max, c);

            return max;
        }
        private static double Maximum(double a, double b, double c)
        {
            double max = Math.Max(a, b);
            max = Math.Max(max, c);

            return max;
        }

        private static double ConvertSimilarityToDistance(double normalizedSimilarity)
        {
            return 1 - normalizedSimilarity;
        }

        private static double ConvertDistanceToSimilarity(double normalizedDistance)
        {
            return 1 - normalizedDistance;
        }

        private static double ExportResult(double currentValue, int word1Len, int word2Len, bool inputExplainesSimilarity)
        {
            if (ExportResultAsSimilarity)
            {
                // we should export results as similarity metric:
                if (inputExplainesSimilarity)
                    return currentValue / GetMaxLength(word1Len, word2Len);
                else
                    return ConvertDistanceToSimilarity(currentValue / GetMaxLength(word1Len, word2Len));
            }
            else
            {
                // we should export results as distance:
                if (inputExplainesSimilarity)
                    return ConvertSimilarityToDistance(currentValue / GetMaxLength(word1Len, word2Len));
                else
                    return currentValue / GetMaxLength(word1Len, word2Len);
            }
        }

        private static double ExportResult(double currentValue, bool inputExplainesSimilarity)
        {
            if (ExportResultAsSimilarity)
            {
                // we should export results as similarity metric:
                if (inputExplainesSimilarity)
                    return currentValue;
                else
                    return ConvertDistanceToSimilarity(currentValue);
            }
            else
            {
                // we should export results as distance:
                if (inputExplainesSimilarity)
                    return ConvertSimilarityToDistance(currentValue);
                else
                    return currentValue;
            }
        }

        private static int GetMaxLength(int word1Len, int word2Len)
        {
            return (word1Len < word2Len) ? word2Len : word1Len;
        }

        private static int GetMaxLength(string word1, string word2)
        {
            return (word1.Length < word2.Length) ? word2.Length : word1.Length;
        }

        #endregion
    } // class

    ///<summary>
    /// Indicates String Distance Algorithm
    ///</summary>
    public enum StringDistanceAlgorithm
    {
        ///<summary>
        /// Hamming Distnace Algorithm
        ///</summary>
        Hamming = 1,
        ///<summary>
        /// Levenestain Distnace Algorithm
        ///</summary>
        Levenestain = Hamming + 1,
        ///<summary>
        /// JaroWinkler Distnace Algorithm
        ///</summary>
        JaroWinkler = Levenestain + 1,
        ///<summary>
        /// Damerau-Levenestain Distnace Algorithm
        ///</summary>
        DamerauLevenestain = JaroWinkler + 1,
        ///<summary>
        /// Wagner-Fischer Distnace Algorithm
        ///</summary>
        WagnerFischer = DamerauLevenestain + 1,
        ///<summary>
        /// Needleman Distnace Algorithm
        ///</summary>
        Needleman = WagnerFischer + 1,
        ///<summary>
        /// GNULevenesain Distnace Algorithm
        ///</summary>
        GNULevenesain = Needleman + 1,
        ///<summary>
        /// Cosine Distnace Algorithm
        ///</summary>
        Cosine = GNULevenesain + 1,
        ///<summary>
        /// Kashefi Distnace Algorithm
        ///</summary>
        Kashefi = Cosine + 1
    }

    ///<summary>
    /// String Distance Class
    ///</summary>
    public class StringDistanceLayout
    {
        ///<summary>
        /// Get String Distance
        ///</summary>
        ///<param name="word1">First Word</param>
        ///<param name="word2">Second Word</param>
        ///<param name="algorithm">String Distance Algorithm</param>
        ///<returns>String Distance</returns>
        public double GetStringDistance(string word1, string word2, StringDistanceAlgorithm algorithm)
        {
            return GetDifference(word1, word2, algorithm, false, new KashefiConfig());
        }
        ///<summary>
        /// Get String Distance
        ///</summary>
        ///<param name="word1">First Word</param>
        ///<param name="word2">Second Word</param>
        ///<param name="algorithm">String Distance Algorithm</param>
        ///<param name="kashefiConfig">Configuration of Kashefi's String Distance Method</param>
        ///<returns>String Distance</returns>
        public double GetStringDistance(string word1, string word2, StringDistanceAlgorithm algorithm, KashefiConfig kashefiConfig)
        {
            return GetDifference(word1, word2, algorithm, false, kashefiConfig);
        }
        ///<summary>
        /// Get Similarity Score
        ///</summary>
        ///<param name="word1">First Word</param>
        ///<param name="word2">Second Word</param>
        ///<param name="algorithm">String Distance Algorithm</param>
        ///<returns>Similarity Score</returns>
        public double GetWordSimilarity(string word1, string word2, StringDistanceAlgorithm algorithm)
        {
            return GetDifference(word1, word2, algorithm, true, new KashefiConfig());
        }
        ///<summary>
        /// Get Similarity Score
        ///</summary>
        ///<param name="word1">First Word</param>
        ///<param name="word2">Second Word</param>
        ///<param name="algorithm">String Distance Algorithm</param>
        ///<param name="kashefiConfig">Configuration of Kashefi's String Distance Method</param>
        ///<returns>Similarity Score</returns>
        public double GetWordSimilarity(string word1, string word2, StringDistanceAlgorithm algorithm, KashefiConfig kashefiConfig)
        {
            return GetDifference(word1, word2, algorithm, true, kashefiConfig);
        }

        private static double GetDifference(string word1, string word2, StringDistanceAlgorithm algorithm, bool exportAsSimilarity, KashefiConfig kashefiConfig)
        {
            double distance = -1;
            StringDistanceAlgorithms.ExportResultAsSimilarity = exportAsSimilarity;

            if (algorithm == StringDistanceAlgorithm.Hamming)
            {
                distance = StringDistanceAlgorithms.Hamming(word1, word2);
            }
            else if (algorithm == StringDistanceAlgorithm.Levenestain)
            {
                distance = StringDistanceAlgorithms.Levenstein(word1, word2);
            }
            else if (algorithm == StringDistanceAlgorithm.GNULevenesain)
            {
                distance = StringDistanceAlgorithms.GNULevenstein(word1, word2);
            }
            else if (algorithm == StringDistanceAlgorithm.Kashefi)
            {
                distance = StringDistanceAlgorithms.KashefiMeasure(word1, word2, kashefiConfig);
            }
            else if (algorithm == StringDistanceAlgorithm.Needleman)
            {
                double nGapCost = (kashefiConfig.DeleteGapCost + kashefiConfig.InsertGapCost) / 2.0;
                NeedlemanConfig nc = new NeedlemanConfig(kashefiConfig.Keyboard, nGapCost, kashefiConfig.SubstituteGapCost);

                distance = StringDistanceAlgorithms.NeedlemanWunch(word1, word2, nc);
            }
            else if (algorithm == StringDistanceAlgorithm.JaroWinkler)
            {
                distance = StringDistanceAlgorithms.JaroWinckler(word1, word2);
            }
            else if (algorithm == StringDistanceAlgorithm.Cosine)
            {
                distance = StringDistanceAlgorithms.Cosine(word1, word2);
            }
            else if (algorithm == StringDistanceAlgorithm.DamerauLevenestain)
            {
                distance = StringDistanceAlgorithms.DamerauLevenstein(word1, word2);
            }
            else if (algorithm == StringDistanceAlgorithm.WagnerFischer)
            {
                distance = StringDistanceAlgorithms.WagnerFischer(word1, word2);
            }

            return distance;
        }
    }

}

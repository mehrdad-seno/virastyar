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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SCICT.NLP.TextProofing.Punctuation
{
    /// <summary>
    /// Main Punctuation Corrector class.
    /// </summary>
    public class PunctuationCheckerEngine
    {
        #region Private Members
        private List<PuncMistake> mistakeList = new List<PuncMistake>();
        private int m_firstMistakeIndex = 0;
        private int m_firstMistakeLength = 0;
        private int m_firstPuncMistake = 0;
        private string m_firstMistakeDescription="";
        private string m_firstMistakeSuggestion = "";
        private readonly string[] m_substitutionString = new string[5];
        private string m_inputString="";
        private int m_skipIndex=0;
        private int m_skipIndexRecord = 0;
        private int m_lastMistakeID=0;
        private int m_goldenRule=0;
        private bool m_coupleEnable = true;

        private readonly string m_patternsFilePath;
        private const char EOP = '\n'; //define End Of Paragraph here
        #endregion

        private void InitMistakeList()
        {
            String pat="", ermsg="", crmsg="", rep="";
            TextReader tr = new StreamReader(m_patternsFilePath, Encoding.Unicode);
            var repPat = new string[5];
            int i_rep=0;

            //read header line
            pat = tr.ReadLine();

            while (true)
            {
                for(i_rep=0; i_rep<5; i_rep++)
                    repPat[i_rep] = "";
                i_rep = 0;

                pat = tr.ReadLine();
                if (pat == null)
                    break;
                ermsg = tr.ReadLine();
                crmsg = tr.ReadLine();
                rep = tr.ReadLine();
                while (rep != null && rep.Substring(0, ((rep.Length<2)?rep.Length:2)).CompareTo("--")!=0 && i_rep < 5)
                {
                    repPat[i_rep++] = rep;
                    rep = tr.ReadLine();
                }
                if(i_rep>0)
                    this.mistakeList.Add(GetMistakeListObject(pat, ermsg, crmsg, repPat));
            }

            tr.Close();

        }

        private static PuncMistake GetMistakeListObject(string p, string p2, string p3, string[] p4)
        {
            return new PuncMistake(p, p2, p3, p4);
        }

        private static char GetCouple(char inCh)
        {
            switch (inCh)
            {
                case ')':
                    return '(';
                case '(':
                    return ')';
                case '«':
                    return '»';
                case '»':
                    return '«';
                case '[':
                    return ']';
                case ']':
                    return '[';
                default:
                    return ' ';
            }
        }

        private void AddMistakePattern(PuncMistake puncMistake)
        {
            this.mistakeList.Add(puncMistake);
        }

        private int MistakeCount
        {
            get
            {
                return this.mistakeList.Count;
            }
        }

        private bool MatchCouples(
            out int outIndex, out int outLen,
            out string outDesc, out string outSugg, out string outSubstit)
        {
            //  TODO: correct this: lastCoupleIndex should be an array
            int i, j, p, context = 1 /* 1: farsi 2: eng */;
            char ch;

            var myStack = new Stack<StackNode>();
            var n = new StackNode {CoupleChar = '#', CoupleIndex = 0};
            myStack.Push(n);

            bool errorFound = false;

            outSubstit = "";
            outDesc = "";
            outSugg = "";
            outIndex = 0;
            outLen = 0;

            for (i = 0; i < this.m_inputString.Length && !errorFound; i++)
            {
                switch (this.m_inputString[i])
                {
                    case EOP:
                        if (myStack.Peek().CoupleChar != '#')
                        {
                            errorFound = true;
                            outSubstit = GetCouple(myStack.Peek().CoupleChar).ToString();
                            outDesc = "علامت باز بدون بستن آن";
                            outSugg = "";
                        }
                        break;

                    case '(':
                        if (myStack.Peek().CoupleChar == '(')
                        {
                            errorFound = true;
                            outSubstit = ")";
                            outDesc = "در داخل پرانتز نمي‌توان پرانتز باز كرد";
                            outSugg = "پرانتز را ببنديد";
                            break;
                        }
                        n = new StackNode();
                        n.CoupleChar = '(';
                        n.CoupleIndex = i;
                        myStack.Push(n);
                        break;

                    case ')':
                        if (myStack.Peek().CoupleChar != '(')
                        {
                            errorFound = true;
                            if (myStack.Peek().CoupleChar == '#')
                            {
                                outSubstit = "";
                                outDesc = "پرانتز بسته بدون پرانتز باز";
                                outSugg = "پرانتز باز را قرار دهيد";
                            }
                            else
                            {
                                outSubstit = GetCouple(myStack.Peek().CoupleChar).ToString();
                                outDesc = "اشتباه در علامتهاي دوتايي";
                                outSugg = "";
                            }
                            break;
                        }
                        //TODO: err: pop tow times
                        myStack.Pop();
                        break;
                    case '«':
                        if (myStack.Peek().CoupleChar == '«')
                        {
                            errorFound = true;
                            outSubstit = "»";
                            outDesc = "در داخل گيومه نمي‌توان گيومه باز كرد";
                            outSugg = "گيومه را ببنديد";
                            break;
                        }
                        n = new StackNode();
                        n.CoupleChar = '«';
                        n.CoupleIndex = i;
                        myStack.Push(n);
                        break;
                    case '»':
                        if (context == 2)
                            break;

                        if (myStack.Peek().CoupleChar != '«')
                        {
                            errorFound = true;
                            if (myStack.Peek().CoupleChar == '#')
                            {
                                outSubstit = "";
                                outDesc = "گيومه بسته بدون گيومه باز";
                                outSugg = "گيومه باز را قرار دهيد";
                            }
                            else
                            {
                                outSubstit = GetCouple(myStack.Peek().CoupleChar).ToString();
                                outDesc = "اشتباه در علامتهاي دوتايي";
                                outSugg = "";
                            }
                            break;
                        }
                        myStack.Pop();
                        break;

                    case '"':
                        if (myStack.Peek().CoupleChar == '«')
                        {
                            errorFound = true;
                            outSubstit = "»";
                            outDesc = "اين علامت در نمادگذاري فارسي وجود ندارد";
                            outSugg = "گيومه بسته را قرار دهيد";
                            myStack.Pop();
                        }
                        else
                        {
                            p = 0;
                            for (j = 1; j <= 5; j++)
                            {
                                if (i - j > 0) ch = this.m_inputString[i - j];
                                else ch = '\0';
                                if ('a' < ch && ch < 'z' || 'A' < ch && ch < 'Z')
                                    p++;
                                if (i + j < this.m_inputString.Length) ch = this.m_inputString[i + j];
                                else ch = '\0';
                                if ('a' < ch && ch < 'z' || 'A' < ch && ch < 'Z')
                                    p++;
                            }
                            if (p > 0) break;

                            errorFound = true;
                            outSubstit = "«";
                            outDesc = "اين علامت در نمادگذاري فارسي وجود ندارد";
                            outSugg = "گيومه باز قرار دهيد";
                        }
                        break;

                    case ']':   //close
                        if (myStack.Peek().CoupleChar != '[')
                        {
                            errorFound = true;
                            if (myStack.Peek().CoupleChar == '#')
                            {
                                outSubstit = "";
                                outDesc = "براكت بسته بدون براكت باز";
                                outSugg = "براكت باز را قرار دهيد";
                            }
                            else
                            {
                                outSubstit = GetCouple(myStack.Peek().CoupleChar).ToString();
                                outDesc = "اشتباه در علامتهاي دوتايي";
                                outSugg = "";
                            }
                            break;
                        }
                        myStack.Pop();
                        break;
                    case '[':   //open 
                        if (myStack.Peek().CoupleChar == '[')
                        {
                            errorFound = true;
                            outSubstit = "]";
                            outDesc = "در داخل براكت نمي‌توان براكت باز كرد";
                            outSugg = "براكت را ببنديد";
                            break;
                        }
                        n = new StackNode();
                        n.CoupleChar = '[';
                        n.CoupleIndex = i;
                        myStack.Push(n);
                        break;
                    default:
                        break;
                }

                if (i < this.m_skipIndex)
                    errorFound = false;
            }

            outIndex = i - 1;
            outLen = 1;

            if ((myStack.Peek().CoupleChar != '#') && (i >= m_inputString.Length))
            {
                errorFound = true;
                outSubstit = m_inputString.Substring(outIndex, 1);
                outSubstit += GetCouple(myStack.Peek().CoupleChar).ToString();
                outDesc = "علامت باز بدون بستن آن";
                outSugg = "";
            }

            return errorFound;
        }

        /// <summary>
        /// Constractor for PuncEngine class.
        /// </summary>
        public PunctuationCheckerEngine()
            : this("Patterns.txt")
        {
        }

        /// <summary>
        /// Constractor for PuncEngine class.
        /// </summary>
        /// <param name="patternsFilePath">Pattern File Name (full path or relative name).</param>
        public PunctuationCheckerEngine(string patternsFilePath)
        {
            this.m_patternsFilePath = patternsFilePath;
            InitMistakeList();
        }

        /// <summary>
        /// Everytime you build a PuncEngine class you MUST initialize it with an input string. This is the string you want to find its punctuational error; e.g. a paraphaph. 
        /// You just need to pass a string as an input parameter to InitInputString() function.
        /// </summary>
        /// <param name="inStr">Input String; e.g. paragraph or sentence. there is no limit on its length but we suggest to keep it less than 2000 character</param>
        public void InitInputString(string inStr)
        {
            this.m_inputString = inStr;
            /*if (this.m_inputString[this.m_inputString.Length - 1] != EOP)
                this.m_inputString += EOP;*/

            this.m_firstMistakeIndex = 0;
            this.m_firstMistakeLength = 0;
            this.m_firstPuncMistake = 0;
            this.m_firstMistakeDescription = "";
            this.m_firstMistakeSuggestion = "";
            this.m_substitutionString[0] = "";
            this.m_skipIndex = 0;
        }

        /// <summary>
        /// When you have done with input text get back corrected text (whole tetx) by calling GetCorrectedString().
        /// </summary>
        /// <returns>Corrected string</returns>
        public string GetCorrectedString()
        {
            return this.m_inputString;
        }

        /// <summary>
        /// Set an index in the text as a skip point. Correction process will continue after this point and preceding text will be ignored.
        /// </summary>
        /// <param name="indx">Skip index; must be less than length of input string </param>
        /// <seealso cref="GetSkipIndex"/>
        public void SetSkipIndex(int indx)
        {
            this.m_skipIndex = indx;
        }
        
        /// <summary>
        /// Get skip point set by SetSkipIndex.
        /// </summary>
        /// <returns>An integer value denotes skip point</returns>
        /// <seealso cref="SetSkipIndex"/>
        public int GetSkipIndex()
        {
            return this.m_skipIndex;
        }

        /// <summary>
        /// After class initiation you can find mistakes.
        /// This function works in two mode. (1) Normal (2) When Golden Rule is set.
        /// In the first case, this function makes the class to focus on the first mistake in the text.
        /// After focus you can do additional tasks such as CorrectMistake().
        /// In the second case, this will find and correct all mistakes according to Golden Rule.
        /// </summary>
        /// <seealso cref="InitInputString"/>
        /// <seealso cref="SetGoldenRule"/>
        /// <seealso cref="UnsetGoldenRule"/>
        public void FindMistake()
        {
            int i, im, iRep;
            string strT;
            Regex r;
            Match m;
            
            for (i = 0; i < 5; i++)
                this.m_substitutionString[i] = "";

            if (this.m_goldenRule != 0 && this.m_goldenRule != 999)
            {
                r = mistakeList[this.m_goldenRule].ErrReg;
                m = r.Match(this.m_inputString, 0 /*this.m_skipIndex*/);
                if (m.Success)
                {
                    this.m_firstMistakeIndex = m.Index;
                    this.m_firstMistakeLength = m.Length;
                    this.m_firstPuncMistake = this.m_goldenRule;
                    this.m_firstMistakeDescription = mistakeList[this.m_goldenRule].ErrDescription;
                    this.m_firstMistakeSuggestion = mistakeList[this.m_goldenRule].ErrSugestion;
                    strT = this.m_inputString.Substring(m.Index, m.Length);
                    for (iRep = 0; iRep < 5 && mistakeList[this.m_firstPuncMistake].ReplacePatern[iRep].Length > 0; iRep++)
                    {
                        this.m_substitutionString[iRep] = r.Replace
                            (strT, mistakeList[this.m_firstPuncMistake].ReplacePatern[iRep]);
                    }

                    if (this.m_skipIndex != 0 &&  this.m_firstMistakeIndex < this.m_skipIndex)
                    {
                        im = this.m_substitutionString[0].Length - this.m_firstMistakeLength;
                        this.m_skipIndex += im ;
                    }

                    this.m_lastMistakeID = this.m_goldenRule;
                }
                return;
            }

            if (this.m_inputString.Length == 0)
                return;

            if (this.m_skipIndex >= this.m_inputString.Length)
                return;
            
            this.m_firstMistakeIndex = 0;
            this.m_firstMistakeLength = 0;
            this.m_firstPuncMistake = 0;
            this.m_firstMistakeDescription ="";
            this.m_firstMistakeSuggestion = "";

            for(i=0; i<5; i++)
                this.m_substitutionString[i] = "";

            for (i = 0; i < this.MistakeCount; i++)
            {
                if (!mistakeList[i].Enabled)
                    continue;

                /* Must be corrected { */
                if (i == 21 && this.m_inputString.Length<30)
                    continue;
                /* } */

                r = mistakeList[i].ErrReg;

                m = r.Match(this.m_inputString, this.m_skipIndex);

                if (m.Success && (m.Index < this.m_firstMistakeIndex || this.m_firstMistakeLength == 0))
                {
                    this.m_firstMistakeIndex = m.Index;
                    this.m_firstMistakeLength = m.Length;
                    this.m_firstPuncMistake = i;
                    this.m_firstMistakeDescription = mistakeList[i].ErrDescription;
                    this.m_firstMistakeSuggestion = mistakeList[i].ErrSugestion;
                    strT = this.m_inputString.Substring(m.Index, m.Length);
                    for (iRep = 0; iRep < 5; iRep++)
                    {
                        if(mistakeList[this.m_firstPuncMistake].ReplacePatern[iRep].Length > 0)
                            this.m_substitutionString[iRep] = r.Replace
                                (strT, mistakeList[this.m_firstPuncMistake].ReplacePatern[iRep]);
                        else
                            this.m_substitutionString[iRep] = "";
                    }
                    this.m_lastMistakeID = i;
                }
            }

            //------------
            int tempIndex, tempLen;
            string strDesc, strSugg, strCorr;

            if (this.m_coupleEnable &&
                this.MatchCouples(out tempIndex, out tempLen,
                out strDesc, out strSugg, out strCorr))
            {
                if (this.m_firstMistakeIndex+this.m_firstMistakeLength >= tempIndex ||
                    this.m_firstMistakeLength==0)
                {
                    this.m_firstMistakeIndex = tempIndex;
                    this.m_firstMistakeLength = tempLen;
                    this.m_firstMistakeDescription = strDesc;
                    this.m_firstMistakeSuggestion = strSugg;
                    this.m_substitutionString[0] = strCorr;
                    this.m_lastMistakeID = 999;
                }
            }
            //-----------

        }

        /// <summary>
        /// Correct focused mistake. You must focus on a mistake beforehand by calling FindMistake().
        /// </summary>
        /// <param name="rep">Every mistake pattern may have several correction patterns which indicated by an input integer.</param>
        /// <seealso cref="FindMistake"/>
        public void CorrectMistake(int rep)
        {
            int i;

            if (this.m_inputString.Length > 0)
            {
                if (this.m_firstMistakeIndex < this.m_inputString.Length)
                    this.m_inputString =
                        this.m_inputString.Substring(0, this.m_firstMistakeIndex) +
                        this.m_substitutionString[rep] +
                        this.m_inputString.Substring(this.m_firstMistakeIndex + this.m_firstMistakeLength);
                else
                    this.m_inputString =
                        this.m_inputString.Substring(0, this.m_firstMistakeIndex) +
                        this.m_substitutionString[rep];
            }

            this.m_firstMistakeIndex = 0;
            this.m_firstMistakeLength = 0;
            this.m_firstPuncMistake = 0;
            this.m_firstMistakeDescription = "";
            this.m_firstMistakeSuggestion = "";
            for(i=0; i<5; i++)
                this.m_substitutionString[i] = "";
        }

        /// <summary>
        /// Skip focused mistake. It will set the skip point to the point which the mistake was found.
        /// </summary>
        /// <seealso cref="SetSkipIndex"/>
        public void SkipMistake()
        {
            this.m_skipIndex = this.m_firstMistakeIndex + this.m_firstMistakeLength;
            this.m_firstMistakeIndex = 0;
            this.m_firstMistakeLength = 0;
            this.m_firstPuncMistake = 0;
            this.m_firstMistakeDescription = "";
            this.m_firstMistakeSuggestion = "";
            this.m_substitutionString[0] = "";
        }

        /// <summary>
        /// Verify result of last call for FindMistake() to see whether mistake found or not.
        /// </summary>
        /// <returns>Returns True when mistake found otherwise returns False</returns>
        public bool IsErrorFound()
        {
            if (this.m_firstMistakeLength == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Get index of focused mistake. You must focus on a mistake beforehand by calling FindMistake().
        /// This function accompanies wiht its couple GetMistakeLength() to determine mistake position.
        /// </summary>
        /// <returns>An integer value denotes index of mistake</returns>
        /// <seealso cref="GetMistakeLength"/>
        /// <seealso cref="FindMistake"/>
        public int GetMistakeIndex()
        {
            if (IsErrorFound())
                return this.m_firstMistakeIndex;
            else
                return 0;
        }

        /// <summary>
        /// Get length of focused mistake. You must focus on a mistake beforehand by calling FindMistake().
        /// This function accompanies wiht its couple GetMistakeIndex() to determine mistake position.
        /// </summary>
        /// <returns>An integer value denotes length of mistake</returns>
        /// <seealso cref="GetMistakeIndex"/>
        /// <seealso cref="FindMistake"/>
        public int GetMistakeLength()
        {
            if (IsErrorFound())
                return this.m_firstMistakeLength;
            else 
                return 0;
        }

        /// <summary>
        /// Sometime we need to bookmark Skip Index to recall it in the future. E.g. when correcting all mistake by a rule we must save current skip index. In this case we call BookMarkSkipIndex().
        /// This function go together wiht its couple BookMarkSkipIndex().
        /// </summary>
        /// <seealso cref="RecallSkipIndex"/>
        /// <seealso cref="SetGoldenRule"/>
        /// <seealso cref="UnsetGoldenRule"/>
        public void BookMarkSkipIndex()
        {
            this.m_skipIndexRecord = this.m_skipIndex; 
        }

        /// <summary>
        /// Used to recll last saved Skip Index.
        /// </summary>
        /// <see cref="BookMarkSkipIndex"/>
        public void RecallSkipIndex()
        {
            this.m_skipIndex = this.m_skipIndexRecord;
            this.m_skipIndexRecord = 0;
        }

        /// <summary>
        /// Returns a string which describes focused mistake.
        /// </summary>
        /// <returns>String description of mistake</returns>
        /// <seealso cref="GetMistakeSuggestion"/>
        public string GetMistakeDescription()
        {
            if (IsErrorFound())
                return this.m_firstMistakeDescription;
            else
                return "";
        }

        /// <summary>
        /// Returns a string which suggest an action to correct focused mistake.
        /// </summary>
        /// <returns>String, suggesting an action.</returns>
        /// <seealso cref="GetMistakeDescription"/>
        public string GetMistakeSuggestion()
        {
            if (IsErrorFound())
                return this.m_firstMistakeSuggestion;
            else
                return "";
        }

        /// <summary>
        /// Returns a string that should substitutes "Mistake String" which determined by GetMistakeIndex() and GetMistakeLength().
        /// </summary>
        /// <returns>Substitution string</returns>
        /// <seealso cref="GetSubstitutionString"/>
        /// <seealso cref="GetMistakeIndex"/>
        /// <seealso cref="GetMistakeLength"/>
        public string GetSubstitutionString()
        {
            if (IsErrorFound())
                return m_substitutionString[0];
            else
                return "";
        }

        /// <summary>
        /// Similar to GetSubstitutionString() but returns multiple suggestions. You can use one of those suggestion to correct text.
        /// you can correct text inside class by calling CorrectMistake(int rep) with an input integer which selects one of suggestions.
        /// </summary>
        /// <returns>Array of suggestion string.</returns>
        /// <seealso cref="CorrectMistake"/>
        /// <seealso cref="GetSubstitutionString"/>
        public string[] GetMultiSubstitutionString()
        {
            int len = 0;
            for (len = 0; len < m_substitutionString.Length; ++len)
                if (m_substitutionString[len] == null || (len > 0 && m_substitutionString[len] == ""))
                    break;
            var sugs = new string[len];
            if (len > 0)
                Array.Copy(m_substitutionString, 0, sugs, 0, len);
            return sugs;
        }

        /// <summary>
        /// Disables last focused rule.
        /// </summary>
        /// <seealso cref="EnableAllRules"/>
        public void DisableLastRule()
        {
            if (this.m_lastMistakeID != 999)
                this.mistakeList[this.m_lastMistakeID].Enabled = false;
            else
                this.m_coupleEnable = false;
        }

        /// <summary>
        /// Enables all rules.
        /// </summary>
        /// <seealso cref="DisableLastRule"/>
        public void EnableAllRules()
        {
            int i;
            for (i = 0; i < this.MistakeCount; i++)
            {
                mistakeList[i].Enabled = true;
            }
            this.m_coupleEnable = true;
        }

        /// <summary>
        /// Set current rule which focused mistake was found by, as Golden Rule. A Golden Rule is defined to correct all mistakes at ones.
        /// </summary>
        /// <seealso cref="SetGoldenRule"/>
        /// <seealso cref="FindMistake"/>
        public void SetGoldenRule()
        {
            this.m_goldenRule = this.m_lastMistakeID;
        }

        /// <summary>
        /// Unset current Golden Rule. 
        /// </summary>
        /// <see cref="SetGoldenRule"/>
        /// <seealso cref="FindMistake"/>
        public void UnsetGoldenRule()
        {
            this.m_goldenRule = 0;
        }

        /// <summary>
        /// Determine whether Golden Rule could be applied or not.
        /// </summary>
        /// <returns>True, when a Golden rule is applicable; False, when a Golden rule is not applicable.</returns>
        /// <seealso cref="SetGoldenRule"/>
        /// <seealso cref="UnsetGoldenRule"/>
        /// <seealso cref="FindMistake"/>
        public bool IsAllChangeable()
        {
            if (this.IsErrorFound() && this.m_lastMistakeID != 999)
                return true;
            else
                return false;
        }

    }

    internal class PuncMistake
    {
        public string ErrPattern;
        public Regex ErrReg;
        public string ErrDescription;
        public string ErrSugestion;
        public string[] ReplacePatern = new string[5];
        public Boolean Enabled;

        public PuncMistake(string errPat, string errDesc, string errSug, string[] replacePat)
        {
            int i;
            this.ErrPattern = errPat;
            this.ErrReg = new Regex(errPat, RegexOptions.IgnoreCase);
            this.ErrDescription = errDesc;
            this.ErrSugestion = errSug;
            for (i = 0; i < 5; i++)
                this.ReplacePatern[i] = replacePat[i];
            this.Enabled = true;
        }
    }

    internal class StackNode
    {
        public int CoupleIndex;
        public char CoupleChar;
    }

}

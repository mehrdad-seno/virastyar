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
using System.Linq;
using System.Text;

namespace SCICT.NLP.Utility.Parsers
{
    /// <summary>
    /// A Utility class to help create regular expression patterns programmatically. 
    /// Recommanded for creating expressions full of right-to-left stuff.
    /// </summary>
    public class RegexPatternCreator
    {
        /// <summary>
        /// Creates a string by seperating any of the arguments by Regex OR operator "|".
        /// The parameters can be string or an array of strings. For other objects their ToString()
        /// return value will be put.
        /// If the putEachInAGroup parameter is set to true then each of the items will be put inside
        /// parantheses (i.e. regex group operator), otherwise they will be put intact.
        /// The space characters inside each item will be replaced by a "white-space plus" pattern.
        /// </summary>
        /// <param name="putEachInAGroup">if set to <c>true</c> puts each item in a group 
        /// (i.e. inside parantheses).</param>
        /// <param name="args">The items that should be prefereably string or string array.</param>
        public static string CreateOR(bool putEachInAGroup, params object[] args)
        {
            List<string> pars = new List<string>();

            foreach (object o in args)
            {
                if (o is string[])
                {
                    string[] ar = o as string[];
                    foreach (string str in ar)
                    {
                        pars.Add(str);
                    }
                }
                else if (o is string)
                {
                    pars.Add(o as string);
                }
                else
                {
                    pars.Add(o.ToString());
                }
            }

            return CreateOR(putEachInAGroup, pars.ToArray());
        }

        /// <summary>
        /// Creates a string by seperating any of the arguments by Regex OR operator "|".
        /// If the putEachInAGroup parameter is set to true then each of the items will be put inside
        /// parantheses (i.e. regex group operator), otherwise they will be put intact.
        /// The space characters inside each item will be replaced with a "white-space plus" pattern.
        /// </summary>
        /// <param name="putEachInAGroup">if set to <c>true</c> puts each item in a group 
        /// (i.e. inside parantheses).</param>
        /// <param name="args">The items.</param>
        public static string CreateOR(bool putEachInAGroup, params string[] args)
        {
            if (args.Length <= 0) return "";
            StringBuilder sb = new StringBuilder();

            string format;
            if (putEachInAGroup)
            {
                sb.AppendFormat("({0})", ResolveSpacePattern(args[0]));
                format = "|({0})";
            }
            else
            {
                sb.AppendFormat("{0}", ResolveSpacePattern(args[0]));
                format = "|{0}";
            }

            for(int i = 1; i < args.Length; ++i)
            {
                sb.AppendFormat(format, ResolveSpacePattern(args[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Resolves the space pattern in the given string. 
        /// The space characters inside the given string will be replaced with a "white-space plus" pattern.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        private static string ResolveSpacePattern(string str)
        {
            if (str.Contains('(')) // if string contains inner groups ignore any resolutions
                return str;

            str = str.Trim();
            if (str.Contains(' '))
            {
                return str.Replace(" ", BetWordWSPlus);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Creates a regex group with an optional name. A regex group is a pattern inside parantheses.
        /// If the group-name parameter is an empty string then the name part will not be created.
        /// The content strings will be simply concatenated together.
        /// </summary>
        /// <param name="grpName">Name of the group.</param>
        /// <param name="args">The content arguments which will be concatenated to form the group content.</param>
        /// <returns></returns>
        public static string CreateGroup(string grpName, params string[] args)
        {
            if (args.Length <= 0) return "";
            StringBuilder sb = new StringBuilder();

            sb.Append("(");

            grpName = grpName.Trim();
            if (grpName.Length > 0)
            {
                sb.AppendFormat("?<{0}>", grpName);
            }

            foreach (string str in args)
            {
                sb.Append(str);
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Creates the plus closure pattern for the given string (i.e. (pat)+ ).
        /// If the string is not inside a group, it will be put inside a group first.
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <returns></returns>
        public static string ClosurePlus(string pat)
        {
            if (!IsEnclosedInParantheses(pat))
                pat = "(" + pat + ")";

            return String.Format("{0}+", pat);
        }

        /// <summary>
        /// Creates the star closure pattern for the given string (i.e. (pat)* ).
        /// If the string is not inside a group, it will be put inside a group first.
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <returns></returns>
        public static string ClosureStar(string pat)
        {
            if (!IsEnclosedInParantheses(pat))
                pat = "(" + pat + ")";

            return String.Format("{0}*", pat);
        }

        /// <summary>
        /// Creates the optional closure pattern for the given string (i.e. (pat)? ).
        /// If the string is not inside a group, it will be put inside a group first.
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <returns></returns>
        public static string ClosureQuestionMark(string pat)
        {
            if (!IsEnclosedInParantheses(pat))
                pat = "(" + pat + ")";

            return String.Format("{0}?", pat);
        }

        /// <summary>
        /// Creates the exact repetition closure pattern for the given string (i.e. (pat){n} ).
        /// If the string is not inside a group, it will be put inside a group first.
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <param name="n">The number</param>
        /// <returns></returns>
        public static string ClosureExactNum(string pat, int n)
        {
            if (!IsEnclosedInParantheses(pat))
                pat = "(" + pat + ")";

            return String.Format("{0}{{{1}}}", pat, n);
        }

        /// <summary>
        /// Creates the at least repetition closure pattern for the given string (i.e. (pat){n,} ).
        /// If the string is not inside a group, it will be put inside a group first.
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <param name="n">The number</param>
        /// <returns></returns>
        public static string ClosureAtLeastNum(string pat, int n)
        {
            if (!IsEnclosedInParantheses(pat))
                pat = "(" + pat + ")";
            return String.Format("{0}{{{1},}}", pat, n);
        }

        /// <summary>
        /// Creates the range repetition closure pattern for the given string (i.e. (pat){min,max} ).
        /// If the string is not inside a group, it will be put inside a group first.
        /// </summary>
        /// <param name="pat">The input pattern.</param>
        /// <param name="min">The min number</param>
        /// <param name="max">The max number</param>
        /// <returns></returns>
        public static string ClosureRangeNum(string pat, int min, int max)
        {
            if (!IsEnclosedInParantheses(pat))
                pat = "(" + pat + ")";

            return String.Format("{0}{{{1},{2}}}", pat, min, max);
        }

        /// <summary>
        /// Determines whether the specified string is enclosed in parantheses. 
        /// (e.g. returns true for "(..)", and false for "(..)(..)".
        /// </summary>
        /// <param name="str">The given string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string is enclosed in parantheses; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnclosedInParantheses(string str)
        {
            if (str[0] == '(' && str[str.Length - 1] == ')')
            {
                int count = 0;
                int ulimit = str.Length - 2;
                char c;
                
                // to avoid "()()"
                for (int i = 1; i <= ulimit; ++i)
                {
                    c = str[i];
                    if (c == ')') count--;
                    if (c == '(') count++;
                    if (count < 0)
                        return false;
                }
                return (count == 0);
            }
            return false;
        }

        /// <summary>
        /// The whitespace characters that can occur between words (not inside them).
        /// </summary>
        public static string BetWordWSChars = @" \t\f";

        /// <summary>
        /// The half-space character. In other words, the white-space that can occur inside words.
        /// </summary>
        public static string InWordWSChar = @"\u200C";

        /// <summary>
        /// The regex representation of the set of whitespace characters that can occur between words (not inside them).
        /// </summary>
        public static string BetWordWS = @"[" + BetWordWSChars + "]";

        /// <summary>
        /// The regex representation of the set of whitespace characters that can occur both between and inside words.
        /// </summary>
        public static string InWordWS = @"[" + BetWordWSChars + InWordWSChar + "]";

        /// <summary>
        /// The regex representation of the star closure of the set of whitespace characters that can occur both between and inside words.
        /// </summary>
        public static string InWordWSStar = InWordWS + "*";

        /// <summary>
        /// The regex representation of the plus closure of the set of whitespace characters that can occur both between and inside words.
        /// </summary>
        public static string InWordWSPlus = InWordWS + "+";

        /// <summary>
        /// The regex representation of the star closure of the set of whitespace characters that can occur between words (not inside them).
        /// </summary>
        public static string BetWordWSStar = BetWordWS + "*";

        /// <summary>
        /// The regex representation of the plus closure of the set of whitespace characters that can occur between words (not inside them).
        /// </summary>
        public static string BetWordWSPlus = BetWordWS + "+";
    }
}

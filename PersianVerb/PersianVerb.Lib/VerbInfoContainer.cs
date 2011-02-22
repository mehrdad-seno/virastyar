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
using System.Text.RegularExpressions;
using System.IO;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{

    public enum STEM_ALPHA
    {
        A_B,
        B_A,
        A_A,
        B_B,
        A_X,
        B_X,
        X_A,
        X_B,
        X_X
    }

    public enum STEM_TIME
    {
        MAZI,
        MOZARE,
        UNSET
    }

    public class VerbEntry
    {

        public string pastStem;
        public string presentStem;
        public ENUM_VERB_TRANSITIVITY transitivity;
        public ENUM_VERB_TYPE verbType;
        public string pishvand;
        public string felyar;
        public string harfe_ezafe;

        private STEM_ALPHA alph_start_mazi;
        private STEM_ALPHA alph_start_moza;
        private STEM_ALPHA alph_end_mazi;
        private STEM_ALPHA alph_end_moza;
        private STEM_ALPHA alph_bound_mazi;
        private STEM_ALPHA alph_bound_moza;

        public VerbEntry()
        {
            this.pastStem = "";
            this.presentStem = "";
            this.transitivity = ENUM_VERB_TRANSITIVITY.INVALID;
            this.verbType = ENUM_VERB_TYPE.INVALID;
            this.pishvand = "";
            this.felyar = "";
            this.harfe_ezafe = "";
        }

        public VerbEntry(string past_stem, string present_stem, ENUM_VERB_TRANSITIVITY transitivity)
        {
            this.SetVerbEntry(past_stem, present_stem, transitivity, ENUM_VERB_TYPE.SADE, "", "", "");
        }

        public VerbEntry(
            string past_stem,
            string present_stem,
            ENUM_VERB_TRANSITIVITY transitivity,
            ENUM_VERB_TYPE verb_type,
            string _pishvand,
            string _felyar,
            string h_ezafe)
        {
            this.SetVerbEntry(past_stem, present_stem, transitivity, verb_type, _pishvand, _felyar, h_ezafe);
        }

        public void SetVerbEntry(
            string past_stem, 
            string present_stem, 
            ENUM_VERB_TRANSITIVITY transitivity,
            ENUM_VERB_TYPE verb_type,
            string _pishvand,
            string _felyar,
            string h_ezafe)
        {
            this.pastStem = past_stem;
            this.presentStem = present_stem;
            this.transitivity = transitivity;
            this.verbType = verb_type;
            this.pishvand = _pishvand;
            this.felyar = _felyar;
            this.harfe_ezafe = h_ezafe;
            this.SetStemAlpha();
        }

        public void SetVerbEntry(
            string pastStem, 
            string presentStem, 
            ENUM_VERB_TRANSITIVITY transitivity)
        {
            this.SetVerbEntry(pastStem, presentStem, transitivity, ENUM_VERB_TYPE.SADE, "", "", "");
        }

        public string GetStem(ENUM_TENSE_TIME time, ENUM_TENSE_PASSIVITY passivity)
        {
            string verbStem = "";

            switch (time)
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AYANDE:
                    verbStem = this.pastStem;
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AMR:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbStem = this.presentStem;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbStem = this.pastStem;
                            break;
                        default:
                            verbStem = "";
                            break;
                    }
                    break;

                default:
                    verbStem = "";
                    break;
            }
            return verbStem;
        }

        public string GetStem(Verb v)
        {
            return this.GetStem(v.getTenseTime(), v.getTensePassivity());
        }

        public string GetTail(STEM_TIME st)
        {
            string stem = "";

            switch (st)
            { 
                case STEM_TIME.MAZI:
                    stem = this.pastStem;
                    break;
                case STEM_TIME.MOZARE:
                    stem = this.presentStem;
                    break;
            }

            stem = stem.Substring(1);

            return stem;
        }

        public STEM_ALPHA StartingAlpha(STEM_TIME stemTime)
        {
            switch (stemTime)
            {
                case STEM_TIME.MAZI:
                    return this.alph_start_mazi;
                case STEM_TIME.MOZARE:
                    return this.alph_start_moza;
                default:
                    break;
            }
            return STEM_ALPHA.X_X;
        }

        public STEM_ALPHA EndingAlpha(STEM_TIME stemTime)
            /* only for present stem */
        {
            switch (stemTime)
            {
                case STEM_TIME.MAZI:
                    return alph_end_mazi;
                case STEM_TIME.MOZARE:
                    return alph_end_moza;

                default:
                    break;
            }
            return STEM_ALPHA.X_X;
        }

        public STEM_ALPHA BoundingAlpha(STEM_TIME stemTime)
        {
            STEM_ALPHA start, end, bound;

            start = this.StartingAlpha(stemTime);
            end = this.EndingAlpha(stemTime);
            bound = STEM_ALPHA.X_X;

            if (start == STEM_ALPHA.A_X && end == STEM_ALPHA.X_A)
                bound = STEM_ALPHA.A_A;
            else if (start == STEM_ALPHA.A_X && end == STEM_ALPHA.X_B)
                bound = STEM_ALPHA.A_B;
            else if (start == STEM_ALPHA.B_X && end == STEM_ALPHA.X_A)
                bound = STEM_ALPHA.B_A;
            else if (start == STEM_ALPHA.B_X && end == STEM_ALPHA.X_B)
                bound = STEM_ALPHA.B_B;

            return bound;
        }

        public bool IsDoHamkard()
        {
            if (this.pastStem == "کرد" || this.pastStem == "نمود")
                return true;
            else
                return false;
        }

        private void SetStemAlpha()
        {
            string alpha = "";

            alpha = this.pastStem.Substring(0, 1);
            alph_start_mazi = (alpha == "آ" || alpha == "ا") ? STEM_ALPHA.A_X : STEM_ALPHA.B_X;
            if (alpha == "v")
                pastStem = pastStem.Substring(1);

            alpha = this.presentStem.Substring(0, 1);
            alph_start_moza = (alpha == "آ" || alpha == "ا") ? STEM_ALPHA.A_X : STEM_ALPHA.B_X;
            if (alpha == "v")
                presentStem = presentStem.Substring(1);

            alpha = this.presentStem.Substring(presentStem.Length - 1);
            alph_end_moza = (alpha == "ا" || alpha == "آ" || alpha == "و") ? STEM_ALPHA.X_A : STEM_ALPHA.X_B;
            if (alpha == "v")
                presentStem = presentStem.Substring(0, presentStem.Length - 1);

            alph_end_mazi = STEM_ALPHA.X_X;
        }
    }

    public class GroupPattern
    {
        public string pattern;
        public STEM_ALPHA stemAlpha;
        public STEM_TIME stemTime;
        public int stemCount;

        public GroupPattern()
        {
            this.pattern = "";
            this.stemAlpha = STEM_ALPHA.X_X;
            this.stemTime = STEM_TIME.UNSET;
            this.stemCount = 0;
        }

        public void addUnit(string stem)
        {
            if (this.stemCount > 0)
            {
                this.pattern += "|" + stem;
            }
            else
            {
                this.pattern += stem;
            }
            this.stemCount++;
        }

        public bool verbsEndWithVowel()
        {
            if (this.stemAlpha == STEM_ALPHA.A_A ||
                this.stemAlpha == STEM_ALPHA.B_A ||
                this.stemAlpha == STEM_ALPHA.X_A)
                return true;
            else
                return false;
        }

        public bool verbsStartWithA()
        {
            if (this.stemAlpha == STEM_ALPHA.A_A ||
                this.stemAlpha == STEM_ALPHA.A_B ||
                this.stemAlpha == STEM_ALPHA.A_X)
                return true;
            else
                return false;
        }

    }

    public class VerbInfoContainer
    {
        private List<VerbEntry> entrys;
        private int m_index;

        public VerbInfoContainer()
        {
            entrys = new List<VerbEntry>();
            m_index = 0;
        }

        public void ResetIndex()
        {
            m_index = 0;
        }

        private void addVerbEntry(
            string mazi, 
            string mozare, 
            ENUM_VERB_TRANSITIVITY transitivity,
            ENUM_VERB_TYPE verb_type,
            string _pishvand,
            string _felyar,
            string h_ezafe)
        {
            VerbEntry ve = new VerbEntry();
            ve.SetVerbEntry(mazi, mozare, transitivity, verb_type, _pishvand, _felyar, h_ezafe);
            this.entrys.Add(ve);
        }

        public bool LoadStemFile(string fileName)
        {
            this.entrys.Clear();

            // Specify file, instructions, and privelegdes
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // Create a new stream to read from a file
            StreamReader sr = new StreamReader(file);
            //int startPos = 0;
            Regex r = new Regex("([\\d])\\s+([\\d])\\s+([^\\s]+)\\s+([^\\s]+)\\s+([^\\s]+)\\s+([^\\s]+)\\s+([^\\s]+)");
            Match m;

            string mazi = "";
            string mozare = "";
            ENUM_VERB_TRANSITIVITY trans = ENUM_VERB_TRANSITIVITY.INVALID;
            ENUM_VERB_TYPE vtype = ENUM_VERB_TYPE.INVALID;
            string felyar = "";
            string pishvand = "";
            string h_ezafe = "";

            string input = null;
            while ((input = sr.ReadLine()) != null)
            {
                m = r.Match(input);

                if (m.Success)
                {
                    // Verb Type
                    switch (Convert.ToInt32(m.Groups[1].Value))
                    {
                        case 1:
                            vtype = ENUM_VERB_TYPE.SADE;
                            break;
                        case 2:
                            vtype = ENUM_VERB_TYPE.PISHVANDI;
                            break;
                        case 3:
                            vtype = ENUM_VERB_TYPE.MORAKKAB;
                            break;
                        case 4:
                            vtype = ENUM_VERB_TYPE.PISHVANDI_MORAKKAB;
                            break;
                        case 5:
                            vtype = ENUM_VERB_TYPE.EBARATE_FELI;
                            break;
                        case 6:
                            vtype = ENUM_VERB_TYPE.NAGOZAR;
                            break;
                        case 7:
                            break;
                        default:
                            vtype = ENUM_VERB_TYPE.INVALID;
                            break;
                    }

                    // Verb Transitivity
                    if (m.Groups[2].Value == "0")
                    {
                        trans = ENUM_VERB_TRANSITIVITY.INTRANSITIVE;
                    }
                    else if (m.Groups[2].Value == "1")
                    {
                        trans = ENUM_VERB_TRANSITIVITY.TRANSITIVE;
                    }
                    else if (m.Groups[2].Value == "2")
                    {
                        trans = ENUM_VERB_TRANSITIVITY.BILATERAL;
                    }

                    // Verb Stem
                    mazi = m.Groups[3].Value;
                    mozare = m.Groups[4].Value;

                    // Components
                    felyar = m.Groups[5].Value;
                    if (felyar == "-") felyar = "";
                    pishvand = m.Groups[6].Value;
                    if (pishvand == "-") pishvand = "";
                    h_ezafe = m.Groups[7].Value;
                    if (h_ezafe == "-") h_ezafe = "";

                    this.addVerbEntry(mazi, mozare, trans, vtype, pishvand, felyar, h_ezafe);
                }
            }

            // Close StreamReader
            sr.Close();

            // Close file
            file.Close();

            if (this.entrys.Count > 0)
                return true;
            else
                return false;
        }

        public GroupPattern GetStemPatten(STEM_TIME st, STEM_ALPHA vs, ENUM_VERB_TYPE verb_type, ENUM_PATTERN_GENERALITY generality)
        {
            GroupPattern dp = new GroupPattern();
            dp.stemAlpha = vs;
            dp.stemTime = st;
            string generalVerb = ".{2,4}";

            switch (st)
            {
                case STEM_TIME.MAZI:
                    switch (vs)
                    {
                        case STEM_ALPHA.A_X:
                        case STEM_ALPHA.A_B:
                        case STEM_ALPHA.A_A:
                            if (generality == ENUM_PATTERN_GENERALITY.EXPLICIT)
                            {
                                this.entrys.ForEach(delegate(VerbEntry ve)
                                {
                                    if ((ve.StartingAlpha(STEM_TIME.MAZI) == STEM_ALPHA.A_X) && (ve.verbType == verb_type))
                                        dp.addUnit(ve.GetTail(STEM_TIME.MAZI));
                                });
                                dp.pattern = "[آا](?:" + dp.pattern + ")";
                            }
                            else
                            {
                                dp.pattern = "[آا](?:" + generalVerb + ")";
                            }
                            break;

                        case STEM_ALPHA.B_X:
                        case STEM_ALPHA.B_B:
                        case STEM_ALPHA.B_A:
                            if (generality == ENUM_PATTERN_GENERALITY.EXPLICIT)
                            {
                                this.entrys.ForEach(delegate(VerbEntry ve)
                                {
                                    if ((ve.StartingAlpha(STEM_TIME.MAZI) == STEM_ALPHA.B_X) && (ve.verbType == verb_type))
                                        dp.addUnit(ve.pastStem);
                                });
                            }
                            else
                            {
                                dp.pattern = generalVerb;
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case STEM_TIME.MOZARE:
                    switch (vs)
                    {
                        case STEM_ALPHA.A_B:
                            if (generality == ENUM_PATTERN_GENERALITY.EXPLICIT)
                            {
                                this.entrys.ForEach(delegate(VerbEntry ve)
                                {
                                    if ((ve.BoundingAlpha(STEM_TIME.MOZARE) == STEM_ALPHA.A_B) && (ve.verbType == verb_type))
                                        dp.addUnit(ve.GetTail(STEM_TIME.MOZARE));
                                });
                                dp.pattern = "[آا](?:" + dp.pattern + ")";
                            }
                            else
                            {
                                dp.pattern = "[آا](?:" + generalVerb + ")";
                            }
                            break;

                        case STEM_ALPHA.A_A:
                            if (generality == ENUM_PATTERN_GENERALITY.EXPLICIT)
                            {
                                this.entrys.ForEach(delegate(VerbEntry ve)
                                {
                                    if ((ve.BoundingAlpha(STEM_TIME.MOZARE) == STEM_ALPHA.A_A) && (ve.verbType == verb_type))
                                        dp.addUnit(ve.GetTail(STEM_TIME.MOZARE));
                                });
                                dp.pattern = "[آا](?:" + dp.pattern + ")";
                            }
                            else
                            {
                                dp.pattern = "[آا](?:" + generalVerb + ")";
                            }
                            break;

                        case STEM_ALPHA.B_B:
                            if (generality == ENUM_PATTERN_GENERALITY.EXPLICIT)
                            {
                                this.entrys.ForEach(delegate(VerbEntry ve)
                                {
                                    if ((ve.BoundingAlpha(STEM_TIME.MOZARE) == STEM_ALPHA.B_B) && (ve.verbType == verb_type))
                                        dp.addUnit(ve.presentStem);
                                });
                            }
                            else
                            {
                                dp.pattern = generalVerb;
                            }
                            break;

                        case STEM_ALPHA.B_A:
                            if (generality == ENUM_PATTERN_GENERALITY.EXPLICIT)
                            {
                                this.entrys.ForEach(delegate(VerbEntry ve)
                                {
                                    if ((ve.BoundingAlpha(STEM_TIME.MOZARE) == STEM_ALPHA.B_A) && (ve.verbType == verb_type))
                                        dp.addUnit(ve.presentStem);
                                });
                            }
                            else
                            {
                                dp.pattern = "[آا](?:" + generalVerb + ")";
                            }
                            break;

                        default:
                            break;
                    }
                    break;
            }

            return dp;
        }

        public GroupPattern GetPishvandPattern(ENUM_VERB_TYPE vtype)
        {
            GroupPattern gp = new GroupPattern();
            List<string> strList = new List<string>();

            this.entrys.ForEach(delegate(VerbEntry ve)
            {
                if ((ve.pishvand != "") && (ve.verbType==vtype) && !strList.Contains(ve.pishvand))
                {
                    strList.Add(ve.pishvand);
                }
            });

            strList.ForEach(delegate(string s)
            {
                gp.addUnit(s);
            });

            return gp;
        }

        public GroupPattern GetFelyarPattern(ENUM_VERB_TYPE vtype)
        {
            GroupPattern gp = new GroupPattern();
            List<string> strList = new List<string>();

            this.entrys.ForEach(delegate(VerbEntry ve)
            {
                if ((ve.felyar != "") && (ve.verbType==vtype) && !strList.Contains(ve.felyar))
                {
                    strList.Add(ve.felyar);
                }
            });

            strList.ForEach(delegate(string s)
            {
                gp.addUnit(s);
            });

            return gp;
        }

        public VerbEntry GetVerbEntry()
        {
            if (m_index < entrys.Count)
                return entrys[m_index++];
            else
                return null;
        }

    }


}

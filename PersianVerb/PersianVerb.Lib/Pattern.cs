using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{
    public enum ENUM_PATTERN_NAMING
    {
        NAMED,
        UNNAMED,
        NO_CAPTURE,
        INVALID
    }

    public enum ENUM_PATTERN_GENERALITY
    {
        EXPLICIT,
        GENERAL
    }

    public class ConstPatterns
    {
        /* Pattern Names */
        public string pn_stem;
        public string pn_pronoun_1;
        public string pn_pronoun_2;
        public string pn_posit_1;
        public string pn_posit_2;
        public string pn_posit_3;
        public string pn_object_1; // main part
        public string pn_object_2; // mostamar part
        public string pn_object_3; // pishvand
        public string pn_object_4; // felyar
        public string pn_pishvand;
        public string pn_felyar;
        public string pn_hezafe;

        private ENUM_PATTERN_NAMING namingMode;

        /* Persons */
        public string const_pronoun_g = "یم|ید|ند|م|ی|";
        public string const_pronoun_h = "یم|ید|ند|م|ی|د";
        public string const_pronoun_n = "‌ایم|‌اید|‌اند|‌ام|‌ای| است|";
        public string const_pronoun_o = "م|ت|ش|مان|تان|شان|";

        public ConstPatterns()
        {
            this.pn_posit_1 = "";
            this.pn_posit_2 = "";
            this.pn_posit_3 = "";
            this.pn_pronoun_1 = "";
            this.pn_pronoun_2 = "";
            this.pn_stem = "";
            this.pn_object_1 = "";
            this.pn_object_2 = "";
            this.pn_object_3 = "";
            this.pn_object_4 = "";
            this.pn_pishvand = "";
            this.pn_felyar = "";
            this.pn_hezafe = "";
            this.namingMode = ENUM_PATTERN_NAMING.UNNAMED;
        }

        public void setMode(ENUM_PATTERN_NAMING nm)
        {
            this.namingMode = nm;
            this.setNames();
        }

        private void setNames()
        {
            switch (this.namingMode)
            {
                case ENUM_PATTERN_NAMING.NAMED:
                    this.pn_posit_1 = "?<posit1>";
                    this.pn_posit_2 = "?<posit2>";
                    this.pn_posit_3 = "?<posit3>";
                    this.pn_pronoun_1 = "?<pronoun1>";
                    this.pn_pronoun_2 = "?<pronoun2>";
                    this.pn_stem = "?<stem>";
                    this.pn_object_1 = "?<object1>";
                    this.pn_object_2 = "?<object2>";
                    this.pn_object_3 = "?<object3>";
                    this.pn_object_4 = "?<object4>";
                    this.pn_pishvand = "?<pishvand>";
                    this.pn_felyar = "?<felyar>";
                    this.pn_hezafe = "?<harfe_ezafe>";
                    break;
                case ENUM_PATTERN_NAMING.UNNAMED:
                    this.pn_posit_1 = "";
                    this.pn_posit_2 = "";
                    this.pn_posit_3 = "";
                    this.pn_pronoun_1 = "";
                    this.pn_pronoun_2 = "";
                    this.pn_stem = "";
                    this.pn_object_1 = "";
                    this.pn_object_2 = "";
                    this.pn_object_3 = "";
                    this.pn_object_4 = "";
                    this.pn_pishvand = "";
                    this.pn_felyar = "";
                    this.pn_hezafe = "";
                    break;
                case ENUM_PATTERN_NAMING.NO_CAPTURE:
                    this.pn_posit_1 = "?:";
                    this.pn_posit_2 = "?:";
                    this.pn_posit_3 = "?:";
                    this.pn_pronoun_1 = "?:";
                    this.pn_pronoun_2 = "?:";
                    this.pn_stem = "?:";
                    this.pn_object_1 = "?:";
                    this.pn_object_2 = "?:";
                    this.pn_object_3 = "?:";
                    this.pn_object_4 = "?:";
                    this.pn_pishvand = "?:";
                    this.pn_felyar = "?:";
                    this.pn_hezafe = "?:";
                    break;
                default:
                    break;
            }
        }

    }

    public class VerbPattern
    {
        public int count;
        public string [] regularPattern = {"", "", ""};
        public ENUM_TENSE_TIME verbTime = ENUM_TENSE_TIME.INVALID;
        public ENUM_TENSE_PASSIVITY verbPassivity = ENUM_TENSE_PASSIVITY.INVALID;
        public ENUM_VERB_TYPE verbType = ENUM_VERB_TYPE.INVALID;

        private int index;

        private string boundPattrn(string pattern)
        {
            string pattern_boundary = "(?<!" + "‌" + ")\\b";
            return pattern_boundary + pattern + pattern_boundary;
        }

        public VerbPattern()
        {
            this.count = 0;
            this.index = 0;
            this.regularPattern[0] = "";
        }

        private bool getPart(out string pattern)
        {
            if (this.index < this.count)
            {
                pattern = this.boundPattrn(this.regularPattern[this.index]);
                this.index++;
                return true;
            }
            else
            {
                pattern = "";
                return false;
            }
        }

        public void resetIndex()
        {
            this.index = 0;
        }

        public void addPart(string pattern)
        {
            this.regularPattern[this.count] = pattern;
            this.count++;
        }

        public string printPattrn()
        {
            string pattern = "";
            int i;
            for (i = 0; i < this.count; i++)
                pattern += this.regularPattern[i] + "\r\n";
            return pattern;
        }

        public List<VerbMatch> findMe(string sentence)
        {
            PatternIncoder pi = new PatternIncoder();
            List<VerbMatch> vmList = new List<VerbMatch>();

            string pattern;
            Match m;
            int machNo = 0;
            bool patternFound = true;
            int nextStartPos = 0;

            while (patternFound)
            {
                VerbMatch vm = new VerbMatch();
                this.resetIndex();
                pi.resetPatternIncoder();
                machNo = 0;

                while (this.getPart(out pattern))
                {
                    /* Q: how does it work? */
                    Regex r = new Regex(pattern);
                    m = r.Match(sentence, nextStartPos);
                    if (m.Success)
                    {
                        vm.addSegment(m.Index, m.Length);
                        pi.setTenseTime(this.verbTime);
                        pi.setTensePassivity(this.verbPassivity);
                        pi.setTenseType(this.verbType);
                        pi.setSubPats(m);
                        machNo++;
                    }
                }

                if (this.count == machNo)
                {
                    pi.recogniseVerb();
                    vm.verbStem = pi.getVerbStem();
                    vm.pishvand = pi.getVerbPishvand();
                    vm.felyar = pi.getVerbFelyar();
                    vm.h_ezafe = pi.getVerbHezafe();
                    vm.setVerbID(pi.getVerbID());
                    vmList.Add(vm);
                    nextStartPos = vm.getFirstEndPos();
                    if(nextStartPos >= sentence.Length)
                        patternFound = false;
                }
                else
                {
                    patternFound = false;
                }
            }

            return vmList;
        }

    }


    public class PatternMaker
    {
        private Verb v;
        private ConstPatterns cp;
        private VerbInfoContainer dictionary;
        private GroupPattern stemPattern;
        private GroupPattern pishvandPattern;
        private GroupPattern felyarPattern;
        private ENUM_PATTERN_GENERALITY patternGenerality;

        public PatternMaker(VerbInfoContainer dic)
        {
            this.v = new Verb();
            this.cp = new ConstPatterns();
            this.dictionary = dic;

            this.pishvandPattern = this.dictionary.GetPishvandPattern(ENUM_VERB_TYPE.PISHVANDI);
            this.felyarPattern = this.dictionary.GetFelyarPattern(ENUM_VERB_TYPE.MORAKKAB);
            this.patternGenerality = ENUM_PATTERN_GENERALITY.EXPLICIT;
        }

        public void setNamingMode(ENUM_PATTERN_NAMING naming)
        {
            this.cp.setMode(naming);
        }

        public void setGeneralityMode(ENUM_PATTERN_GENERALITY generality)
        {
            this.patternGenerality = generality;
        }

        public List<VerbPattern> getPattern(ENUM_TENSE_TIME time, ENUM_TENSE_PASSIVITY passivity, ENUM_VERB_TYPE vtype)
        {
            List<VerbPattern> patList = new List<VerbPattern>();
            STEM_TIME st;

            this.v.setTenseTime(time);
            this.v.setTensePassivity(passivity);
            this.v.setTenseType(vtype);
            st = this.v.getStemTime();

            switch (st)
            {
                case STEM_TIME.MAZI:
                    this.stemPattern = this.dictionary.GetStemPatten(STEM_TIME.MAZI, STEM_ALPHA.A_X, vtype, this.patternGenerality);
                    if (this.stemPattern.stemCount > 0 || (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL))
                        patList.Add(this.makePattern());
                    this.stemPattern = this.dictionary.GetStemPatten(STEM_TIME.MAZI, STEM_ALPHA.B_X, vtype, this.patternGenerality);
                    if (this.stemPattern.stemCount > 0 || (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL))
                        patList.Add(this.makePattern());
                    break;
                case STEM_TIME.MOZARE:
                    this.stemPattern = this.dictionary.GetStemPatten(STEM_TIME.MOZARE, STEM_ALPHA.B_B, vtype, this.patternGenerality);
                    if (this.stemPattern.stemCount > 0 || (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL))
                        patList.Add(this.makePattern());
                    this.stemPattern = this.dictionary.GetStemPatten(STEM_TIME.MOZARE, STEM_ALPHA.A_B, vtype, this.patternGenerality);
                    if (this.stemPattern.stemCount > 0 || (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL))
                        patList.Add(this.makePattern());
                    this.stemPattern = this.dictionary.GetStemPatten(STEM_TIME.MOZARE, STEM_ALPHA.B_A, vtype, this.patternGenerality);
                    if (this.stemPattern.stemCount > 0 || (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL))
                        patList.Add(this.makePattern());
                    this.stemPattern = this.dictionary.GetStemPatten(STEM_TIME.MOZARE, STEM_ALPHA.A_A, vtype, this.patternGenerality);
                    if (this.stemPattern.stemCount > 0 || (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL))
                        patList.Add(this.makePattern());
                    break;
            }

            return patList;
        }

        public string printTitlePattern()
        {
            return this.v.printTitleTime() + " " + this.v.printTitlePassivity();
        }

        private VerbPattern makePattern()
        {
            VerbPattern vp = null;

            switch (this.v.getTenseType())
            {
                case ENUM_VERB_TYPE.SADE:
                    vp = this.makePattern_Sade();
                    break;
                case ENUM_VERB_TYPE.PISHVANDI:
                    vp = this.makePattern_Pishvandi();
                    break;
                case ENUM_VERB_TYPE.MORAKKAB:
                    vp = this.makePattern_Felyar();
                    break;
                case ENUM_VERB_TYPE.PISHVANDI_MORAKKAB:
                    break;
                case ENUM_VERB_TYPE.EBARATE_FELI:
                    break;
                default:
                    break;
            }
            return vp;
        }

        private VerbPattern makePattern_Pishvandi()
        {
            string re = "";
            VerbPattern vp = new VerbPattern();

            vp.verbPassivity = this.v.getTensePassivity();
            vp.verbTime = this.v.getTenseTime();
            vp.verbType = this.v.getTenseType();

            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "بود" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(2);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "بود" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "داشت" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "داشت" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "بوده" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "بوده" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "داشته" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "داشته" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "باش" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "باش" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "دار" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "دار" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AYANDE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                "خواه" +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(2) + " " +
                                this.getPatternPositivity(2) +
                                this.getGroupPatternStem() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "خواه" +
                                this.getPatternPronoun(2) + " " +
                                this.getPatternPositivity(3) +
                                this.getPassiveModalVerb();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternPishvand() +
                                this.getPattrnObjectPronoun(3) + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternPishvand() + "[ ]?" +
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                default:
                    break;
            }

            vp.addPart(re);

            return vp;
        }

        private VerbPattern makePattern_Sade()
        {
            string re = "";
            VerbPattern vp = new VerbPattern();

            vp.verbPassivity = this.v.getTensePassivity();
            vp.verbTime = this.v.getTenseTime();
            vp.verbType = this.v.getTenseType();

            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "بود" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "بود" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "داشت" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "داشت" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "بوده" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "بوده" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "داشته" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "داشته" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "باش" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "باش" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "دار" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "دار" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AYANDE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                "خواه" +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(2) + " " +
                                this.getPatternPositivity(2) +
                                this.getGroupPatternStem() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "خواه" +
                                this.getPatternPronoun(2) + " " +
                                this.getPatternPositivity(3) +
                                this.getPassiveModalVerb();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                default:
                    break;
            }

            vp.addPart(re);

            return vp;
        }

        private VerbPattern makePattern_Felyar()
        {
            string re = "";
            VerbPattern vp = new VerbPattern();

            vp.verbPassivity = this.v.getTensePassivity();
            vp.verbTime = this.v.getTenseTime();
            vp.verbType = this.v.getTenseType();

            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "بود" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(2);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "بود" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "داشت" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "داشت" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "بوده" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "بوده" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "داشته" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "داشته" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "باش" +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPositivity(3) +
                                "باش" +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                "دار" +
                                this.getPatternPronoun(1) +
                                this.getPattrnObjectPronoun(2);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                "دار" +
                                this.getPatternPronoun(1);
                            vp.addPart(re);
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun(2);
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AYANDE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                "خواه" +
                                this.getPatternPronoun(2) +
                                this.getPattrnObjectPronoun(2) + " " +
                                this.getPatternPositivity(2) +
                                this.getGroupPatternStem() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                "خواه" +
                                this.getPatternPronoun(2) + " " +
                                this.getPatternPositivity(3) +
                                this.getPassiveModalVerb();
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            re =
                                this.getGroupPatternFelyar() +
                                this.getPattrnObjectPronoun(4);
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPronoun() +
                                this.getPattrnObjectPronoun(1);
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            re =
                                this.getGroupPatternFelyar();
                            vp.addPart(re);
                            re =
                                this.getPatternPositivity(1) +
                                this.getGroupPatternStem() +
                                this.getPatternPositivity(2) +
                                this.getPassiveModalVerb() +
                                this.getPatternPronoun();
                            break;
                    }
                    break;

                default:
                    break;
            }

            vp.addPart(re);

            return vp;
        }

        private string getPatternPronoun(int order)
        {
            string pattern_pronoun = "";

            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    pattern_pronoun = this.cp.const_pronoun_g;
                    if (this.v.getTenseTime() == ENUM_TENSE_TIME.MAZI_E_SADE && 
                        this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL)
                            pattern_pronoun = "یم|ید|ند";
                    break;

                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    pattern_pronoun = this.cp.const_pronoun_n;
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AYANDE:
                    pattern_pronoun = this.cp.const_pronoun_h;
                    break;

                case ENUM_TENSE_TIME.AMR:
                    if (this.stemPattern.verbsEndWithVowel() && (this.v.getTensePassivity() == ENUM_TENSE_PASSIVITY.ACTIVE))
                        pattern_pronoun = "یید|ی|";
                    else
                        pattern_pronoun = "ید|";
                    break;

                default:
                    pattern_pronoun = "";
                    break;
            }

            if (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL && 
                pattern_pronoun.Substring(pattern_pronoun.Length-1) == "|" )
            {
                pattern_pronoun = pattern_pronoun.Substring(0, pattern_pronoun.Length - 1);
            }

            switch (order)
            {
                case 1:
                    pattern_pronoun = "(" + this.cp.pn_pronoun_1 + pattern_pronoun + ")";
                    break;
                case 2:
                    pattern_pronoun = "(" + this.cp.pn_pronoun_2 + pattern_pronoun + ")";
                    break;
                default:
                    break;
            }

            return pattern_pronoun;
        }

        private string getPatternPronoun()
        {
            return this.getPatternPronoun(2);
        }

        private string getPatternPositivity(int order)
        {
            string pospat = "";
            bool IS_STEM_STARTS_WITH_A = this.stemPattern.verbsStartWithA();

            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                            if (IS_STEM_STARTS_WITH_A)
                                pospat = "(" + pospat + "ی" + ")?";
                            else
                                pospat += "?";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            switch (order)
                            {
                                case 1:
                                    pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                                    if (IS_STEM_STARTS_WITH_A)
                                        pospat = "(" + pospat + "ی" + ")?";
                                    else
                                        pospat += "?";
                                    break;
                                case 2:
                                    pospat = "(" + this.cp.pn_posit_2 + "[بنم])?";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                    if (this.v.getTenseTime() == ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI)
                        if (this.patternGenerality == ENUM_PATTERN_GENERALITY.GENERAL)
                            if (pospat.Substring(pospat.Length - 1) == "?")
                            {
                                pospat = pospat.Substring(0, pospat.Length - 1);
                                pospat = pospat.Replace("بنم", "بن");
                            }

                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (order == 1)
                                pospat = "(" + this.cp.pn_posit_1 + "ن)?";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            switch (order)
                            {
                                case 1:
                                    pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                                    if (IS_STEM_STARTS_WITH_A)
                                        pospat = "(" + pospat + "ی" + ")?";
                                    else
                                        pospat += "?";
                                    break;
                                case 2:
                                    pospat = "(" + this.cp.pn_posit_2 + "ن)?";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (order == 1)
                            {
                                pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                                if (IS_STEM_STARTS_WITH_A)
                                    pospat = "(" + pospat + "ی" + ")?";
                                else
                                    pospat += "?";
                            }
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            switch (order)
                            {
                                case 1:
                                    pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                                    if (IS_STEM_STARTS_WITH_A)
                                        pospat = "(" + pospat + "ی" + ")?";
                                    else
                                        pospat += "?";
                                    break;
                                case 2:
                                    pospat = "(" + this.cp.pn_posit_2 + "[بنم])?";
                                    break;
                                case 3:
                                    pospat = "(" + this.cp.pn_posit_3 + "[نم])?";
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AYANDE:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            switch (order)
                            {
                                case 1:
                                    pospat = "(" + this.cp.pn_posit_1 + "[بنم])?";
                                    break;
                                case 2:
                                    pospat = "(" + this.cp.pn_posit_2 + "[بنم])";
                                    if (IS_STEM_STARTS_WITH_A)
                                        pospat = "(" + pospat + "ی" + ")?";
                                    else
                                        pospat += "?";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            switch (order)
                            {
                                case 1:
                                    pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                                    if (IS_STEM_STARTS_WITH_A)
                                        pospat = "(" + pospat + "ی" + ")?";
                                    else
                                        pospat += "?";
                                    break;
                                case 2:
                                    pospat = "(" + this.cp.pn_posit_2 + "[بنم])?";
                                    break;
                                case 3:
                                    pospat = "(" + this.cp.pn_posit_3 + "[بنم])?";
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                            if (IS_STEM_STARTS_WITH_A)
                                pospat = "(" + pospat + "ی" + ")";
                            pospat += "?";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            switch (order)
                            {
                                case 1:
                                    pospat = "(" + this.cp.pn_posit_1 + "[بنم])";
                                    if (IS_STEM_STARTS_WITH_A)
                                        pospat = "(" + pospat + "ی" + ")?";
                                    else
                                        pospat += "?";
                                    break;
                                case 2:
                                    pospat = "(" + this.cp.pn_posit_2 + "[بنم])?";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    pospat = "";
                    break;
            }

            return pospat;
        }

        private string getGroupPatternPishvand()
        {
            string pishvandS = "";

            pishvandS = "(" + this.cp.pn_pishvand + this.pishvandPattern.pattern + ")";

            return pishvandS;
        }

        private string getGroupPatternFelyar()
        {
            string felyarS = "";

            felyarS = "(" + this.cp.pn_felyar + this.felyarPattern.pattern + ")";

            return felyarS;
        }

        private string getGroupPatternStem()
        {
            string stemPat = "";
            
            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AYANDE:
                    if (stemPattern.verbsEndWithVowel())
                        stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")" + "ی";
                    else
                        stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")";

                    if (v.getTensePassivity() == ENUM_TENSE_PASSIVITY.PASSIVE)
                        stemPat += "ه ";
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (stemPattern.verbsEndWithVowel())
                                stemPat = "می‌" + "(" + this.cp.pn_stem + this.stemPattern.pattern + ")" + "ی";
                            else
                                stemPat = "می‌" + "(" + this.cp.pn_stem + this.stemPattern.pattern + ")";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")ه ";
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")ه ";
                    break;

                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")ه";
                    if (v.getTensePassivity() == ENUM_TENSE_PASSIVITY.PASSIVE)
                        stemPat += " ";
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            stemPat = "می‌" + "(" + this.cp.pn_stem + this.stemPattern.pattern + ")ه";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")ه ";
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    switch (this.v.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            stemPat = "(" + this.cp.pn_stem + this.stemPattern.pattern + ")ه ";
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    stemPat = "";
                    break;
            }

            return stemPat;
        }

        private string getPassiveModalVerb()
        {
            string modal = "";

            switch (this.v.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    modal = "(?:" + "شد|گشت|گردید" + ")";
                    break;
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    modal = "می‌" + "(?:" + "شد|گشت|گردید" + ")";
                    break;
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    modal = "(?:" + "شد|گشت|گردید" + ")ه ";
                    break;
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    modal = "(?:" + "شد|گشت|گردید" + ")ه";
                    break;
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    modal = "می‌" + "(?:" + "شد|گشت|گردید" + ")ه";
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    modal = "می‌" + "(?:" + "شو|گرد" + ")";
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AMR:
                    modal = "(?:" + "شو|گرد" + ")";
                    break;
                case ENUM_TENSE_TIME.AYANDE:
                    modal = "(?:" + "شد|گشت|گردید" + ")";
                    break;

                default:
                    modal = "";
                    break;
            }

            return modal;
        }

        private string getPattrnObjectPronoun()
        {
            return this.getPattrnObjectPronoun(1);
        }

        private string getPattrnObjectPronoun(int order)
        {
            string s = "";

            switch (order)
            {
                case 1:
                    s = "(" + this.cp.pn_object_1 + this.cp.const_pronoun_o + ")";
                    break;
                case 2:
                    s = "(" + this.cp.pn_object_2 + this.cp.const_pronoun_o + ")";
                    break;
                case 3:
                    s = "(" + this.cp.pn_object_3 + this.cp.const_pronoun_o + ")";
                    break;
                case 4:
                    s = "(" + this.cp.pn_object_4 + this.cp.const_pronoun_o + ")";
                    break;
            }

            return s;
        }

    }


    public class PatternIncoder : Verb
    {
        private string pi_pronoun1 = "";
        private string pi_pronoun2 = "";
        private string pi_posit1 = "";
        private string pi_posit2 = "";
        private string pi_posit3 = "";
        private string pi_object1 = "";
        private string pi_object2 = "";
        private string pi_object3 = "";
        private string pi_object4 = "";

        private string pi_stem = "";
        private string pi_pishvand = "";
        private string pi_felyar = "";
        private string pi_hezafe = "";

        public void setSubPats(Match m)
        {
            if (this.pi_pronoun2 == "")
                this.pi_pronoun2 = m.Groups["pronoun2"].Value;
            if (this.pi_pronoun1 == "")
                this.pi_pronoun1 = m.Groups["pronoun1"].Value;
            if (this.pi_posit1 == "")
                this.pi_posit1 = m.Groups["posit1"].Value;
            if (this.pi_posit2 == "")
                this.pi_posit2 = m.Groups["posit2"].Value;
            if (this.pi_posit3 == "")
                this.pi_posit3 = m.Groups["posit3"].Value;
            if (this.pi_object1 == "")
                this.pi_object1 = m.Groups["object1"].Value;
            if (this.pi_object2 == "")
                this.pi_object2 = m.Groups["object2"].Value;
            if (this.pi_object3 == "")
                this.pi_object3 = m.Groups["object3"].Value;
            if (this.pi_object4 == "")
                this.pi_object4 = m.Groups["object4"].Value;
            if (this.pi_stem == "")
                this.pi_stem = m.Groups["stem"].Value;
            if (this.pi_pishvand == "")
                this.pi_pishvand = m.Groups["pishvand"].Value;
            if (this.pi_felyar == "")
                this.pi_felyar = m.Groups["felyar"].Value;
            if (this.pi_hezafe == "")
                this.pi_hezafe = m.Groups["harfe_ezafe"].Value;
        }

        private void setTensePerson()
        {
            ENUM_TENSE_PERSON person;

            if (this.getTenseTime() == ENUM_TENSE_TIME.MAZI_E_MOSTAMAR)
            {
                if (this.pi_pronoun2 == "م" && this.pi_pronoun1 == "" && this.pi_object1 == "")
                {
                    this.pi_pronoun2 = "";
                    this.pi_object1 = "م";
                }
            }

            if (this.getTenseTime() == ENUM_TENSE_TIME.AMR)
            {
                if (this.pi_pronoun2 == "")
                    this.pi_pronoun2 = "ی";
                else if (this.pi_pronoun2 == "یید")
                    this.pi_pronoun2 = "ید";
            }

            if (this.pi_pronoun2 == "م" || this.pi_pronoun2 == "‌ام")
                person = ENUM_TENSE_PERSON.SINGULAR_FIRST;
            else if (this.pi_pronoun2 == "ی" || this.pi_pronoun2 == "‌ای")
                person = ENUM_TENSE_PERSON.SINGULAR_SECOND;
            else if (this.pi_pronoun2 == "" || this.pi_pronoun2 == " است" || this.pi_pronoun2 == "د")
                person = ENUM_TENSE_PERSON.SINGULAR_THIRD;
            else if (this.pi_pronoun2 == "یم" || this.pi_pronoun2 == "‌ایم")
                person = ENUM_TENSE_PERSON.PLURAL_FIRST;
            else if (this.pi_pronoun2 == "ید" || this.pi_pronoun2 == "‌اید")
                person = ENUM_TENSE_PERSON.PLURAL_SECOND;
            else if (this.pi_pronoun2 == "ند" || this.pi_pronoun2 == "‌اند")
                person = ENUM_TENSE_PERSON.PLURAL_THIRD;
            else
                person = ENUM_TENSE_PERSON.INVALID;

            if (this.pi_pronoun2 != this.pi_pronoun1 && this.getTenseTime() == ENUM_TENSE_TIME.MAZI_E_MOSTAMAR)
                person = ENUM_TENSE_PERSON.UNMACHED_SEGMENT;

            this.setTensePerson(person);
        }

        private void setTenseObject()
        {
            ENUM_TENSE_OBJECT object_person = ENUM_TENSE_OBJECT.NOT_SET;
            string[] objs = { "", "", "", "" };
            string obj = "";
            int i = 0;

            objs[0] = this.pi_object1;
            objs[1] = this.pi_object2;
            objs[2] = this.pi_object3;
            objs[3] = this.pi_object4;

            for (i = 0; i < 4; i++)
            {
                if (obj == "")
                {
                    obj = objs[i];
                }
                else if (objs[i] != "" && objs[i] != obj)
                {
                    object_person = ENUM_TENSE_OBJECT.WRONGE;
                    break;
                }
            }

            if (obj == "")
            {
                object_person = ENUM_TENSE_OBJECT.NOT_SET;
            }
            else if (object_person != ENUM_TENSE_OBJECT.WRONGE)
            {
                if (obj == "م")
                    object_person = ENUM_TENSE_OBJECT.SINGULAR_FIRST;
                else if (obj == "ت")
                    object_person = ENUM_TENSE_OBJECT.SINGULAR_SECOND;
                else if (obj == "ش")
                    object_person = ENUM_TENSE_OBJECT.SINGULAR_THIRD;
                else if (obj == "مان")
                    object_person = ENUM_TENSE_OBJECT.PLURAL_FIRST;
                else if (obj == "تان")
                    object_person = ENUM_TENSE_OBJECT.PLURAL_SECOND;
                else if (obj == "شان")
                    object_person = ENUM_TENSE_OBJECT.PLURAL_THIRD;
                else
                    object_person = ENUM_TENSE_OBJECT.INVALID;
            }

            this.setTenseObject(object_person);
        }

        private void setTensePositivity()
        {
            ENUM_TENSE_POSITIVITY positivity = ENUM_TENSE_POSITIVITY.INVALID;

            switch (this.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (this.pi_posit1 == "" || this.pi_posit1 == "ب")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if (this.pi_posit1 == "ن")
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if (this.pi_posit1 == "م")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            if (this.pi_posit1 == "" && this.pi_posit2 == "")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if ((this.pi_posit1 == "ب" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "ب"))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if ((this.pi_posit1 == "ن" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "ن"))
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if ((this.pi_posit1 == "م" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "م"))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (this.pi_posit1 == "")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if (this.pi_posit1 == "ن")
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            if (this.pi_posit1 == "" && this.pi_posit2 == "")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if ((this.pi_posit1 == "ب" && this.pi_posit2 == ""))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if ((this.pi_posit1 == "ن" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "ن"))
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if ((this.pi_posit1 == "م" && this.pi_posit2 == ""))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (this.pi_posit1 == "" || this.pi_posit1 == "ب")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if (this.pi_posit1 == "ن" || this.pi_posit1 == "م")
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            if (this.pi_posit1 == "ب" && this.pi_posit2 == "ب" && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if ((this.pi_posit1 == "" || this.pi_posit1 == "ب") && (this.pi_posit2 == "" || this.pi_posit2 == "ب") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if ((this.pi_posit1 == "ن" || this.pi_posit1 == "م") && (this.pi_posit2 == "ن" || this.pi_posit2 == "م") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            else if ((this.pi_posit1 == "" || this.pi_posit1 == "ن") && (this.pi_posit2 == "" || this.pi_posit2 == "ن") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if ((this.pi_posit1 == "م" ^ this.pi_posit2 == "م") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AYANDE:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (this.pi_posit1 == "" && this.pi_posit2 == "")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if ((this.pi_posit1 == "ب" && this.pi_posit2 == "") || (this.pi_posit2 == "ب" && this.pi_posit1 == ""))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if ((this.pi_posit1 == "ن" && this.pi_posit2 == ""))
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if ((this.pi_posit1 == "م" && this.pi_posit2 == ""))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            if (this.pi_posit1 == "ب" && this.pi_posit2 == "ب" && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if ((this.pi_posit1 == "" || this.pi_posit1 == "ب") && (this.pi_posit2 == "" || this.pi_posit2 == "ب") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if ((this.pi_posit1 == "ن" || this.pi_posit1 == "م") && (this.pi_posit2 == "ن" || this.pi_posit2 == "م") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            else if ((this.pi_posit1 == "" || this.pi_posit1 == "ن") && (this.pi_posit2 == "" || this.pi_posit2 == "ن") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if ((this.pi_posit1 == "م" ^ this.pi_posit2 == "م") && this.pi_posit3 == "")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            if (this.pi_posit1 == "ب")
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if (this.pi_posit1 == "ن")
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if (this.pi_posit1 == "")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if (this.pi_posit1 == "م")
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            if ((this.pi_posit1 == "ب" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "ب"))
                                positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                            else if ((this.pi_posit1 == "" && this.pi_posit2 == "") || (this.pi_posit1 == "ب" && this.pi_posit2 == "ب"))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE;
                            else if ((this.pi_posit1 == "ن" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "ن"))
                                positivity = ENUM_TENSE_POSITIVITY.NEGATIVE;
                            else if ((this.pi_posit1 == "م" && this.pi_posit2 == "") || (this.pi_posit1 == "" && this.pi_posit2 == "م"))
                                positivity = ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE;
                            else
                                positivity = ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED;
                            break;
                        default:
                            break;
                    }
                    if ((positivity == ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE) && (this.getTenseType() != ENUM_VERB_TYPE.SADE))
                        positivity = ENUM_TENSE_POSITIVITY.POSITIVE;
                    break;

                default:
                    break;
            }

            this.setTensePositivity(positivity);
        }

        public void recogniseVerb()
        {
            this.setTensePerson();
            this.setTensePositivity();
            this.setTenseObject();
        }

        public string getVerbStem()
        {
            return this.pi_stem;
        }

        public string getVerbPishvand()
        {
            return this.pi_pishvand;
        }

        public string getVerbFelyar()
        {
            return this.pi_felyar;
        }

        public string getVerbHezafe()
        {
            return this.pi_hezafe;
        }

        public void resetPatternIncoder()
        {
            this.pi_stem = "";
            this.pi_felyar = "";
            this.pi_hezafe = "";
            this.pi_pishvand = "";

            this.pi_pronoun1 = "";
            this.pi_pronoun2 = "";
            this.pi_posit1 = "";
            this.pi_posit2 = "";
            this.pi_posit3 = "";
            this.pi_object1 = "";
            this.pi_object2 = "";
            this.pi_object3 = "";
            this.pi_object4 = "";
        }
    }
}

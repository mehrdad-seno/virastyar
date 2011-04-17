using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{
    public enum ENUM_FIND_MODE
    {
        DATABASE,
        BLIND
    }

    public class VerbMatch : Verb
    {
        public string verbStem;
        public string pishvand;
        public string felyar;
        public string h_ezafe;

        public List<Segment> verbSegment;

        private int ID;

        public void setID(int id)
        {
            this.ID = id;
            this.verbSegment.ForEach(delegate(Segment s) {
                s.id = this.ID;
            });
        }

        public int getID()
        {
            return this.ID;
        }

        private int Count = 0;

        public VerbMatch()
        {
            this.verbSegment = new List<Segment>();
            this.ResetVerbMatch();
        }

        public void ResetVerbMatch()
        {
            this.resetVerb();
            this.verbStem = "";
            this.pishvand = "";
            this.felyar = "";
            this.h_ezafe = "";
            this.verbSegment.Clear();
            this.Count = 0;
        }

        public int GetBeginPos()
        {
            int min = -1;

            if (this.verbSegment.Count > 0)
                min = this.verbSegment.Min(seg => seg.start);

            return min;
        }

        public int getEndPos()
        {
            int max = -1;

            if (this.verbSegment.Count > 0)
                max = this.verbSegment.Max(seg => seg.getEndPos());

            return max;
        }

        public int getFirstEndPos()
        {
            int max = -1;

            if (this.verbSegment.Count > 0)
                max = this.verbSegment.Min(seg => seg.getEndPos());

            return max;            
        }

        public void addSegment(int start, int length)
        {
            Segment seg = new Segment(start, length, 0, this.Count+1);
            this.verbSegment.Add(seg);
            this.Count++;
        }

        public bool isMatchFound()
        {
            if (this.verbSegment.Count>0)
                return true;
            else
                return false;
        }

        public string printReference()
        {
            string reference;
            reference =
                this.getID().ToString() + " - " +
                this.printTitlePerson() + " " +
                this.printTitleTense() + " از بن " +
                this.printTitleStemTime() + " " + 
                ((this.felyar!="")?(this.felyar + " "):("")) +
                this.pishvand + "«" +
                this.verbStem + "» " +
                this.printTitleObjct() + "\r\n";

            return reference;
        }

        public string printStem()
        {
            string strStem = 
                this.getID().ToString() + "\t" +
                this.verbStem + "\t" +
                this.printTitleStemTime() + "\t" +
                this.printTitlePerson() + " " +
                this.printTitleTense() + "\r\n";
            return strStem;
        }

    }


    public class Finder
    {

        private List<VerbPattern> ptternList;
        private List<VerbMatch> vmList;
        private VerbInfoContainer stemDic;
        private string sentence;
        private ENUM_FIND_MODE findMethod;

        public Finder(VerbInfoContainer d, ENUM_FIND_MODE findingMethode)
        {
            this.ptternList = new List<VerbPattern>();
            this.vmList = new List<VerbMatch>();
            this.sentence = "";
            this.stemDic = d;
            this.findMethod = findingMethode;
            this.initPatternList();
        }

        public void setDictionary(VerbInfoContainer d)
        {
            this.stemDic = d;
            this.initPatternList();
        }

        public void setSentence(string text)
        {
            this.sentence = text;
            this.vmList.Clear();
        }

        private void initPatternList()
        {
            PatternMaker patternMaker = new PatternMaker(this.stemDic);

            patternMaker.setNamingMode(ENUM_PATTERN_NAMING.NAMED);
            switch (this.findMethod)
            {
                case ENUM_FIND_MODE.DATABASE:
                    patternMaker.setGeneralityMode(ENUM_PATTERN_GENERALITY.EXPLICIT);
                    break;
                case ENUM_FIND_MODE.BLIND:
                    patternMaker.setGeneralityMode(ENUM_PATTERN_GENERALITY.GENERAL);
                    break;
            }

            this.ptternList.Clear();
            //*
            if (this.findMethod == ENUM_FIND_MODE.DATABASE)
            {
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            }
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEID, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEID, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            if (this.findMethod == ENUM_FIND_MODE.DATABASE)
            {
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            }
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AYANDE, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
            this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AYANDE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));

            if (this.findMethod == ENUM_FIND_MODE.DATABASE)
            {
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.SADE));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));

                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEID, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEID, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AYANDE, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AYANDE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.PISHVANDI));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.PISHVANDI));

                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEID, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEID, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                //*/
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AYANDE, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AYANDE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.PASSIVE, ENUM_VERB_TYPE.MORAKKAB));
                this.ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.MORAKKAB));
            }
        }

        public void findVerb()
        {
            List<VerbMatch> vmL = new List<VerbMatch>();
            //List<Tag> tags = new List<Tag>();
            int j = 1;

            this.vmList.Clear();

            for (int i = 0; i < this.ptternList.Count; i++)
            {
                vmL = this.ptternList[i].findMe(this.sentence);
                vmL.ForEach(delegate(VerbMatch vm) {
                    if (vm.isMatchFound())
                    {
                        vm.setID(j++);
                        this.vmList.Add(vm);
                    }
                });
            }
        }

        public List<VerbMatch> getVerbMatch()
        {
            return this.vmList;
        }

        public string getMarkedSentence()
        {
            List<Segment> allSegments = new List<Segment>();
            List<Tag> tags = new List<Tag>();
            Tag tag;

            this.vmList.ForEach(delegate(VerbMatch v1)
            {
                v1.verbSegment.ForEach(delegate(Segment v2) { 
                    allSegments.Add(v2); 
                }); 
            });

            allSegments.Sort(delegate(Segment s1, Segment s2) {
                return s1.start.CompareTo(s2.start);
            });

            if (allSegments.Count > 0)
            {
                tag = new Tag();
                tag.setLocation(allSegments[0].start);
                tag.appendCaption(allSegments[0].id);
                tag.setDirection('o');

                for (int i = 1; i < allSegments.Count; i++)
                {
                    if (allSegments[i].start == tag.getLocation())
                    {
                        tag.appendCaption(allSegments[i].id);
                    }
                    else
                    {
                        tags.Add(tag);
                        tag = new Tag();
                        tag.setLocation(allSegments[i].start);
                        tag.appendCaption(allSegments[i].id);
                        tag.setDirection('o');
                    }
                }
                tags.Add(tag);
            }

            allSegments.Sort(delegate(Segment s1, Segment s2)
            {
                return s1.getEndPos().CompareTo(s2.getEndPos());
            });

            if (allSegments.Count > 0)
            {
                tag = new Tag();
                tag.setLocation(allSegments[0].getEndPos());
                tag.appendCaption(allSegments[0].id);
                tag.setDirection('c');

                for (int i = 1; i < allSegments.Count; i++)
                {
                    if (allSegments[i].getEndPos() == tag.getLocation())
                    {
                        tag.appendCaption(allSegments[i].id);
                    }
                    else
                    {
                        tags.Add(tag);
                        tag = new Tag();
                        tag.setLocation(allSegments[i].getEndPos());
                        tag.appendCaption(allSegments[i].id);
                        tag.setDirection('c');
                    }
                }
                tags.Add(tag);
            }

            tags.Sort(delegate(Tag t1, Tag t2) {
                return t2.getLocation().CompareTo(t1.getLocation());
            });

            for (int i = 0; i < tags.Count; i++)
            {
                this.sentence = this.sentence.Insert(tags[i].getLocation(), tags[i].getTag());
            }

            return this.sentence;
        }

        public string getReferences()
        {
            string strRef = "";

            this.vmList.ForEach(delegate(VerbMatch v1)
            {
                strRef += v1.printReference();
            });

            return strRef;
        }

        public string getReferenceStem()
        {
            string strStem = "";

            this.vmList.ForEach(delegate(VerbMatch v1)
            {
                strStem += v1.printStem();
            });

            return strStem;
        }
    }


    public class Segment
    {

        public int start;
        public int length;
        public int id;

        public Segment(int start, int end, int id, int part)
        {
            this.start = start;
            this.length = end;
            this.id = id;
        }

        public Segment()
        {
            this.start = 0;
            this.length = 0;
            this.id = 0;
        }

        public void setSegment(int start, int end, int id, int part)
        {
            this.start = start;
            this.length = end;
            this.id = id;
        }

        public int getEndPos()
        {
            return this.start + this.length;
        }

    }


    public class Tag
    {

        private int location;
        private string caption;
        private char direction;

        public void appendCaption(int cap)
        {
            if (caption.Length > 0)
                this.caption += ",";

            this.caption += cap.ToString();
        }

        public void setDirection(char d)
        {
            this.direction = d;
        }

        public void setLocation(int l)
        {
            this.location = l;
        }

        public int getLocation()
        {
            return this.location;
        }

        public string getTag()
        {
            string s;

            switch (this.direction)
            {
                case 'o':
                    s = "[<" + this.caption + ">";
                    break;
                case 'c':
                    s = "<" + this.caption + ">]";
                    break;
                default:
                    s = "";
                    break;
            }
            return s;
        }

        public Tag()
        {
            this.location = -1;
            this.caption = "";
            this.direction = '-';
        }

    }


}

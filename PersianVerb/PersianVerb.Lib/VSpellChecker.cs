using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{
    public class VSpellChecker
    {
        private readonly List<VerbPattern> m_ptternList;
        private VerbInfoContainer m_stemDic;

        public VSpellChecker(VerbInfoContainer d)
        {
            m_ptternList = new List<VerbPattern>();
            m_stemDic = d;
            InitPatternList();
        }

        public void SetDictionary(VerbInfoContainer d)
        {
            m_stemDic = d;
            InitPatternList();
        }

        private void InitPatternList()
        {
            var patternMaker = new PatternMaker(m_stemDic);

            patternMaker.setNamingMode(ENUM_PATTERN_NAMING.UNNAMED);

            if (m_ptternList == null) return;
            m_ptternList.Clear();
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADE, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_EKHBARI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
            m_ptternList.AddRange(patternMaker.getPattern(ENUM_TENSE_TIME.AMR, ENUM_TENSE_PASSIVITY.ACTIVE, ENUM_VERB_TYPE.SADE));
        }

        public bool CheckSpell(string word)
        {
            List<VerbMatch> vmL = new List<VerbMatch>();
            bool correct = false;

            for (int i = 0; i < this.m_ptternList.Count; i++)
            {
                vmL = this.m_ptternList[i].findMe(word);
                if(vmL.Count > 0)
                {
                    correct = true;
                    break;
                }
            }
            return correct;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.Utility;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{
    public class Conjugator
    {
        private VerbInfoContainer m_verbInfoContainer;
        private VerbWrapper m_verbWrapper;

        public Conjugator(VerbInfoContainer dic)
        {
            m_verbInfoContainer = dic;
            m_verbInfoContainer.ResetIndex();
        }

        private ENUM_TENSE_TIME[] m_timeList = 
        {
            ENUM_TENSE_TIME.MAZI_E_SADE, 
            ENUM_TENSE_TIME.MAZI_E_ESTEMRARI,
            ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI,
            ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI,
            ENUM_TENSE_TIME.MOZARE_E_EKHBARI,
            ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI,
            ENUM_TENSE_TIME.AMR
        };

        public string[] Conjugate(ENUM_VERB_TYPE verbType)
        {
            VerbEntry ve;
            m_verbWrapper = new VerbWrapper();

            List<string> lst = new List<string>();
            m_verbInfoContainer.ResetIndex();
            while ((ve = m_verbInfoContainer.GetVerbEntry()) != null)
            {
                m_verbWrapper.SetVerbEntry(ve);
                m_verbWrapper.setTensePassivity(ENUM_TENSE_PASSIVITY.ACTIVE);

                if (verbType.Has(ENUM_VERB_TYPE.INFINITIVE))
                {
                    lst.AddRange(m_verbWrapper.PrintInfinitive());
                }

                if (!verbType.Has(ve.verbType))
                    continue;

                #region Conjugate

                foreach (ENUM_TENSE_TIME tm in m_timeList)
                {
                    m_verbWrapper.setTenseTime(tm);
                    //foreach (ENUM_TENSE_OBJECT obj in Enum.GetValues(typeof(ENUM_TENSE_OBJECT)))
                    foreach (ENUM_TENSE_POSITIVITY pos in Enum.GetValues(typeof(ENUM_TENSE_POSITIVITY)))
                    {
                        m_verbWrapper.setTensePositivity(pos);
                        if (!m_verbWrapper.IsValidPositivity())
                            continue;

                        foreach (ENUM_TENSE_PERSON person in Enum.GetValues(typeof(ENUM_TENSE_PERSON)))
                        {
                            if (person == ENUM_TENSE_PERSON.INVALID || person == ENUM_TENSE_PERSON.UNMACHED_SEGMENT)
                                continue;

                            m_verbWrapper.setTensePerson(person);

                            if (m_verbWrapper.isValidTense(ENUM_VERB_TRANSITIVITY.INTRANSITIVE))
                                lst.Add(m_verbWrapper.PrintVerb().Split(' ')[0]);
                        }
                    }
                }

                #endregion
            }

            return lst.Distinct().ToArray();
        }

        public string[] Conjugate(ENUM_VERB_TYPE verbType, ENUM_TENSE_PERSON person)
        {
            VerbEntry ve;
            m_verbWrapper = new VerbWrapper();

            List<string> lst = new List<string>();
            m_verbInfoContainer.ResetIndex();
            while ((ve = m_verbInfoContainer.GetVerbEntry()) != null)
            {
                m_verbWrapper.SetVerbEntry(ve);
                m_verbWrapper.setTensePassivity(ENUM_TENSE_PASSIVITY.ACTIVE);

                if (verbType.Has(ENUM_VERB_TYPE.INFINITIVE))
                {
                    lst.AddRange(m_verbWrapper.PrintInfinitive());
                }

                if (!verbType.Has(ve.verbType))
                    continue;

                #region Conjugate

                foreach (ENUM_TENSE_TIME tm in m_timeList)
                {
                    m_verbWrapper.setTenseTime(tm);
                    //foreach (ENUM_TENSE_OBJECT obj in Enum.GetValues(typeof(ENUM_TENSE_OBJECT)))
                    foreach (ENUM_TENSE_POSITIVITY pos in Enum.GetValues(typeof(ENUM_TENSE_POSITIVITY)))
                    {
                        m_verbWrapper.setTensePositivity(pos);
                        if (!m_verbWrapper.IsValidPositivity())
                            continue;


                        if (person == ENUM_TENSE_PERSON.INVALID || person == ENUM_TENSE_PERSON.UNMACHED_SEGMENT)
                            continue;

                        m_verbWrapper.setTensePerson(person);

                        if (m_verbWrapper.isValidTense(ENUM_VERB_TRANSITIVITY.INTRANSITIVE))
                            lst.Add(m_verbWrapper.PrintVerb().Split(' ')[0]);
                    }
                }

                #endregion
            }

            return lst.Distinct().ToArray();
        }

        public VerbInfo[] ConjugateInfo(ENUM_VERB_TYPE verbType)
        {
            List<VerbInfo> lst = new List<VerbInfo>();
            foreach (ENUM_TENSE_PERSON pers in Enum.GetValues(typeof(ENUM_TENSE_PERSON)))
            {
                if(pers == ENUM_TENSE_PERSON.INVALID || pers == ENUM_TENSE_PERSON.UNMACHED_SEGMENT) continue;
                lst.AddRange(ConjugateInfo(verbType, pers));
            }
            return lst.ToArray();
        }

        public VerbInfo[] ConjugateInfo(ENUM_VERB_TYPE verbType, ENUM_TENSE_PERSON person)
        {
            VerbEntry ve;
            m_verbWrapper = new VerbWrapper();

            List<VerbInfo> lst = new List<VerbInfo>();
            m_verbInfoContainer.ResetIndex();
            while ((ve = m_verbInfoContainer.GetVerbEntry()) != null)
            {
                m_verbWrapper.SetVerbEntry(ve);
                m_verbWrapper.setTensePassivity(ENUM_TENSE_PASSIVITY.ACTIVE);

                if (verbType.Has(ENUM_VERB_TYPE.INFINITIVE))
                {
                    lst.Add(new VerbInfo
                                {
                                    Verb = m_verbWrapper.PrintInfinitive()[0],
                                    Stem = m_verbWrapper.PrintInfinitive()[0],
                                    Positivity = ENUM_TENSE_POSITIVITY.INVALID,
                                    Time = ENUM_TENSE_TIME.INVALID,
                                    Type = ENUM_VERB_TYPE.INFINITIVE,
                                    Person = ENUM_TENSE_PERSON.INVALID
                                });
                }

                if (!verbType.Has(ve.verbType))
                    continue;

                #region Conjugate

                foreach (ENUM_TENSE_TIME tm in m_timeList)
                {
                    m_verbWrapper.setTenseTime(tm);
                    //foreach (ENUM_TENSE_OBJECT obj in Enum.GetValues(typeof(ENUM_TENSE_OBJECT)))
                    foreach (ENUM_TENSE_POSITIVITY pos in Enum.GetValues(typeof(ENUM_TENSE_POSITIVITY)))
                    {
                        m_verbWrapper.setTensePositivity(pos);
                        if (!m_verbWrapper.IsValidPositivity())
                            continue;


                        if (person == ENUM_TENSE_PERSON.INVALID || person == ENUM_TENSE_PERSON.UNMACHED_SEGMENT)
                            continue;

                        m_verbWrapper.setTensePerson(person);

                        if (m_verbWrapper.isValidTense(ENUM_VERB_TRANSITIVITY.INTRANSITIVE))
                        {
                            lst.Add(new VerbInfo
                                        {
                                            Verb = m_verbWrapper.PrintVerb().Split(' ')[0],
                                            Stem = m_verbWrapper.PrintInfinitive()[0],
                                            Time = tm,
                                            Positivity = pos,
                                            Type = ve.verbType,
                                            Person = person

                                        });
                        }
                    }
                }

                #endregion
            }

            return lst.Distinct().ToArray();
        }

        public class VerbInfo
        {
            public string Verb { get; internal set; }

            public string Stem { get; internal set; }

            public ENUM_TENSE_TIME Time { get; internal set; }

            public ENUM_TENSE_POSITIVITY Positivity { get; internal set; }

            public ENUM_VERB_TYPE Type { get; internal set; }

            public ENUM_TENSE_PERSON Person { get; internal set; }

            public override bool Equals(object obj)
            {
                if (!(obj is VerbInfo)) return false;
                VerbInfo other = obj as VerbInfo;
                if (!other.Verb.Equals(Verb)) return false;
                if (!other.Stem.Equals(Stem)) return false;
                if (!other.Time.Equals(Time)) return false;
                if (!other.Positivity.Equals(Positivity)) return false;
                if (!other.Type.Equals(Type)) return false;
                if (!other.Person.Equals(Person)) return false;
                return true;
            }
        }
    }
}

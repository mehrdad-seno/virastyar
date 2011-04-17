using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.Utility;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{

    public enum ENUM_TENSE_PERSON
    {
        SINGULAR_FIRST           = 0x00000001,
        SINGULAR_SECOND          = 0x00000002,
        SINGULAR_THIRD           = 0x00000003,
        PLURAL_FIRST             = 0x00000004,
        PLURAL_SECOND            = 0x00000005,
        PLURAL_THIRD             = 0x00000006,
        UNMACHED_SEGMENT         = 0x00000007,
        INVALID                  = 0x0000000F
    }

    public enum ENUM_TENSE_TIME
    {
        MAZI_E_SADE              = 0x00000010,
        MAZI_E_ESTEMRARI         = 0x00000020,
        MAZI_E_BAEID             = 0x00000030,
        MAZI_E_MOSTAMAR          = 0x00000040,
        MAZI_E_SADEYE_NAGHLI     = 0x00000050,
        MAZI_E_ESTEMRARIE_NAGHLI = 0x00000060,
        MAZI_E_BAEIDE_NAGHLI     = 0x00000070,
        MAZI_E_MOSTAMARE_NAGHLI  = 0x00000080,
        MAZI_E_ELTEZAMI          = 0x00000090,
        MOZARE_E_EKHBARI         = 0x000000A0,
        MOZARE_E_MOSTAMAR        = 0x000000B0,
        MOZARE_E_ELTEZAMI        = 0x000000C0,
        AYANDE                   = 0x000000D0,
        AMR                      = 0x000000E0,
        INVALID                  = 0x000000F0
    }

    public enum ENUM_TENSE_PASSIVITY
    {
        PASSIVE                  = 0x00000100,
        ACTIVE                   = 0x00000200,
        INVALID                  = 0x00000F00
    }

    public enum ENUM_TENSE_POSITIVITY
    {
        POSITIVE                 = 0x00001000,
        NEGATIVE                 = 0x00002000,
        UNUSUAL_POSITIVE         = 0x00003000,
        UNUSUAL_NEGATIVE         = 0x00004000,
        WRONG_UNDETECTED         = 0x00005000,
        INVALID                  = 0x0000F000
    }

    public enum ENUM_TENSE_CASE
    {
        CASE_ONE                 = 0x00010000,
        CASE_TWO                 = 0x00020000,
        CASE_THREE               = 0x00030000,
        CASE_FOUR                = 0x00040000,
        CASE_FIVE                = 0x00050000,
        CASE_SIX                 = 0x00060000,
        CASE_SEVEN               = 0x00070000,
        CASE_EIGHT               = 0x00080000,
        CASE_NINE                = 0x00090000,
        CASE_TEN                 = 0x000A0000,
        INVALID                  = 0x000F0000
    }

    public enum ENUM_TENSE_OBJECT
    {
        NOT_SET                  = 0x00100000,
        SINGULAR_FIRST           = 0x00200000,
        SINGULAR_SECOND          = 0x00300000,
        SINGULAR_THIRD           = 0x00400000,
        PLURAL_FIRST             = 0x00500000,
        PLURAL_SECOND            = 0x00600000,
        PLURAL_THIRD             = 0x00700000,
        WRONGE                   = 0x00800000,
        INVALID                  = 0x00F00000
    }

    [Flags]
    public enum ENUM_VERB_TYPE
    {
        SADE                     = 0x01000000,
        PISHVANDI                = 0x02000000,
        PISHVANDI_MORAKKAB       = 0x04000000,
        MORAKKAB                 = 0x08000000,
        EBARATE_FELI             = 0x00100000,
        NAGOZAR                  = 0x00200000,
        ESNADI                   = 0x00400000,
        GHEIRE_SHAKHSI           = 0x00800000,
        INVALID                  = 0x00010000,
        INFINITIVE               = 0x00020000
    }

    public enum ENUM_VERB_TRANSITIVITY
    {
        INTRANSITIVE,
        TRANSITIVE,
        BILATERAL,
        INVALID
    }


    public class Verb
    {
        private Int32 ID;

        public Verb()
        {
            this.resetVerb();
        }

        public Verb(Int32 verbID)
        {
            this.ID = verbID;
        }

        public Verb(ENUM_TENSE_TIME time, ENUM_TENSE_PASSIVITY passivity, 
            ENUM_TENSE_POSITIVITY positivity, ENUM_TENSE_PERSON person)
        {
            this.setTense(time, passivity, positivity, person);
        }

        public void resetVerb()
        {
            this.ID = 0x00FFFFFF;
        }

        public void setTense(ENUM_TENSE_TIME time, ENUM_TENSE_PASSIVITY passivity, 
            ENUM_TENSE_POSITIVITY positivity, ENUM_TENSE_PERSON person)
        {
            this.resetVerb();
            this.setTensePerson(person);
            this.setTenseTime(time);
            this.setTensePassivity(passivity);
            this.setTensePositivity(positivity);
            this.setTenseType(ENUM_VERB_TYPE.SADE);
        }

        public void setTense(ENUM_TENSE_TIME time, ENUM_TENSE_PASSIVITY passivity,
            ENUM_TENSE_POSITIVITY positivity)
        {
            this.resetVerb();
            this.setTenseTime(time);
            this.setTensePassivity(passivity);
            this.setTensePositivity(positivity);
            this.setTenseType(ENUM_VERB_TYPE.SADE);
        }

        public void setTensePerson(ENUM_TENSE_PERSON person)
        {
            this.ID = this.ID & ~Convert.ToInt32(ENUM_TENSE_PERSON.INVALID);
            this.ID |= Convert.ToInt32(person);
        }

        public void setTenseTime(ENUM_TENSE_TIME time)
        {
            this.ID = this.ID & ~Convert.ToInt32(ENUM_TENSE_TIME.INVALID);
            this.ID |= Convert.ToInt32(time);
        }

        public void setTensePassivity(ENUM_TENSE_PASSIVITY passivity)
        {
            this.ID = this.ID & ~Convert.ToInt32(ENUM_TENSE_PASSIVITY.INVALID);
            this.ID |= Convert.ToInt32(passivity);
        }

        public void setTensePositivity(ENUM_TENSE_POSITIVITY positivity)
        {
            this.ID = this.ID & ~Convert.ToInt32(ENUM_TENSE_POSITIVITY.INVALID);
            this.ID |= Convert.ToInt32(positivity);
        }

        public void setTenseObject(ENUM_TENSE_OBJECT object_pro)
        {
            this.ID = this.ID & ~Convert.ToInt32(ENUM_TENSE_OBJECT.INVALID);
            this.ID |= Convert.ToInt32(object_pro);
        }

        public void setTenseType(ENUM_VERB_TYPE type)
        {
            this.ID = this.ID & ~Convert.ToInt32(ENUM_VERB_TYPE.INVALID);
            this.ID |= Convert.ToInt32(type);
        }

        public ENUM_TENSE_PERSON getTensePerson()
        {
            Int32 mask = Convert.ToInt32(ENUM_TENSE_PERSON.INVALID);
            
            ENUM_TENSE_PERSON vtp = (ENUM_TENSE_PERSON) (this.ID & mask);

            switch (vtp)
            {
                case ENUM_TENSE_PERSON.SINGULAR_FIRST:
                case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                case ENUM_TENSE_PERSON.SINGULAR_THIRD:
                case ENUM_TENSE_PERSON.PLURAL_FIRST:
                case ENUM_TENSE_PERSON.PLURAL_SECOND:
                case ENUM_TENSE_PERSON.PLURAL_THIRD:
                case ENUM_TENSE_PERSON.UNMACHED_SEGMENT:
                    return vtp;
                default:
                    return ENUM_TENSE_PERSON.INVALID;
            }
        }

        public ENUM_TENSE_TIME getTenseTime()
        {
            Int32 mask = Convert.ToInt32(ENUM_TENSE_TIME.INVALID);

            ENUM_TENSE_TIME vtt = (ENUM_TENSE_TIME)(this.ID & mask);

            switch (vtt)
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
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AYANDE:
                case ENUM_TENSE_TIME.AMR:
                    return vtt;
                default:
                    return ENUM_TENSE_TIME.INVALID;
            }

        }

        public ENUM_TENSE_PASSIVITY getTensePassivity()
        {
            Int32 mask = Convert.ToInt32(ENUM_TENSE_PASSIVITY.INVALID);

            ENUM_TENSE_PASSIVITY vtp = (ENUM_TENSE_PASSIVITY)(this.ID & mask);

            switch (vtp)
            {
                case ENUM_TENSE_PASSIVITY.ACTIVE:
                case ENUM_TENSE_PASSIVITY.PASSIVE:
                    return vtp;
                default:
                    return ENUM_TENSE_PASSIVITY.INVALID;
            }
        }

        public ENUM_TENSE_POSITIVITY getTensePositivity()
        {
            Int32 mask = Convert.ToInt32(ENUM_TENSE_POSITIVITY.INVALID);

            ENUM_TENSE_POSITIVITY vtp = (ENUM_TENSE_POSITIVITY)(this.ID & mask);

            switch (vtp)
            {
                case ENUM_TENSE_POSITIVITY.POSITIVE:
                case ENUM_TENSE_POSITIVITY.NEGATIVE:
                case ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE:
                case ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE:
                case ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED:
                    return vtp;
                default:
                    return ENUM_TENSE_POSITIVITY.INVALID;
            }
        }

        public ENUM_TENSE_OBJECT getTenseObjectPronoun()
        {
            Int32 mask = Convert.ToInt32(ENUM_TENSE_OBJECT.INVALID);

            ENUM_TENSE_OBJECT vto = (ENUM_TENSE_OBJECT)(this.ID & mask);

            switch (vto)
            {
                case ENUM_TENSE_OBJECT.SINGULAR_FIRST:
                case ENUM_TENSE_OBJECT.SINGULAR_SECOND:
                case ENUM_TENSE_OBJECT.SINGULAR_THIRD:
                case ENUM_TENSE_OBJECT.PLURAL_FIRST:
                case ENUM_TENSE_OBJECT.PLURAL_SECOND:
                case ENUM_TENSE_OBJECT.PLURAL_THIRD:
                case ENUM_TENSE_OBJECT.NOT_SET:
                case ENUM_TENSE_OBJECT.WRONGE:
                    return vto;
                default:
                    return ENUM_TENSE_OBJECT.INVALID;
            }
        }

        public ENUM_VERB_TYPE getTenseType()
        {
            Int32 mask = Convert.ToInt32(ENUM_VERB_TYPE.INVALID);

            ENUM_VERB_TYPE vtt = (ENUM_VERB_TYPE)(this.ID & mask);

            switch (vtt)
            {
                case ENUM_VERB_TYPE.SADE:
                case ENUM_VERB_TYPE.PISHVANDI:
                case ENUM_VERB_TYPE.PISHVANDI_MORAKKAB:
                case ENUM_VERB_TYPE.MORAKKAB:
                case ENUM_VERB_TYPE.EBARATE_FELI:
                case ENUM_VERB_TYPE.NAGOZAR:
                case ENUM_VERB_TYPE.ESNADI:
                case ENUM_VERB_TYPE.GHEIRE_SHAKHSI:
                    return vtt;
                default:
                    return ENUM_VERB_TYPE.INVALID;
            }
        }

        public STEM_TIME getStemTime()
        {
            STEM_TIME st = STEM_TIME.UNSET;

            switch (this.getTenseTime())
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
                    st = STEM_TIME.MAZI;
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AMR:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            st = STEM_TIME.MOZARE;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            st = STEM_TIME.MAZI;
                            break;
                    }
                    break;
                default:
                    break;
            }

            return st;
        }

        public string printTitlePerson()
        {
            switch (this.getTensePerson())
            {
                case ENUM_TENSE_PERSON.SINGULAR_FIRST:
                    return "اول شخص مفرد";
                case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                    return "دوم شخص مفرد";
                case ENUM_TENSE_PERSON.SINGULAR_THIRD:
                    return "سوم شخص مفرد";
                case ENUM_TENSE_PERSON.PLURAL_FIRST:
                    return "اول شخص جمع";
                case ENUM_TENSE_PERSON.PLURAL_SECOND:
                    return "دوم شخص جمع";
                case ENUM_TENSE_PERSON.PLURAL_THIRD:
                    return "سوم شخص جمع";
                default:
                    return "";
            }
        }

        public string printTitleTime()
        {
            switch (this.getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    return "ماضی ساده";
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                    return "ماضی استمراری";
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                    return "ماضی بعید";
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    return "ماضی مستمر";
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    return "ماضی ساده نقلی";
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                    return "ماضی استمراری نقلی";
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                    return "ماضی بعید نقلی";
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    return "ماضی مستمر نقلی";
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    return "ماضی التزامی";
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    return "مضارع اخباری";
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    return "مضارع مستمر";
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    return "مضارع التزامی";
                case ENUM_TENSE_TIME.AYANDE:
                    return "آینده";
                case ENUM_TENSE_TIME.AMR:
                    return "امر";
                default:
                    return "";
            }
        }

        public string printTitleStemTime()
        {
            string stemTitle = "";

            switch (this.getTenseTime())
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
                    stemTitle = "ماضی";
                    break;
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AMR:
                    switch (this.getTensePassivity())
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            stemTitle = "مضارع";
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            stemTitle = "ماضی";
                            break;
                        default:
                            stemTitle = "";
                            break;
                    }
                    break;

                default:
                    stemTitle = "";
                    break;
            }

            return stemTitle;
        }

        public string printTitlePassivity()
        {
            switch (this.getTensePassivity())
            {
                case ENUM_TENSE_PASSIVITY.ACTIVE:
                    return "معلوم";
                case ENUM_TENSE_PASSIVITY.PASSIVE:
                    return "مجهول";
                default:
                    return "";
            }
        }

        public string printTitlePositivity()
        {
            switch (this.getTensePositivity())
            {
                case ENUM_TENSE_POSITIVITY.POSITIVE:
                    return "مثبت";
                case ENUM_TENSE_POSITIVITY.NEGATIVE:
                    return "منفی";
                case ENUM_TENSE_POSITIVITY.WRONG_UNDETECTED:
                    return "غلط";
                case ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE:
                    return "منفی نامتعارف";
                case ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE:
                    return "مثبت نامتعارف";
                default:
                    return "";
            }
        }

        public string printTitleObjct()
        {
            string title = "دارای ضمیر مفعولی ";

            switch (this.getTenseObjectPronoun())
            {
                case ENUM_TENSE_OBJECT.SINGULAR_FIRST:
                    title += "اول شخص مفرد";
                    break;
                case ENUM_TENSE_OBJECT.SINGULAR_SECOND:
                    title += "دوم شخص مفرد";
                    break;
                case ENUM_TENSE_OBJECT.SINGULAR_THIRD:
                    title += "سوم شخص مفرد";
                    break;
                case ENUM_TENSE_OBJECT.PLURAL_FIRST:
                    title += "اول شخص جمع";
                    break;
                case ENUM_TENSE_OBJECT.PLURAL_SECOND:
                    title += "دوم شخص جمع";
                    break;
                case ENUM_TENSE_OBJECT.PLURAL_THIRD:
                    title += "سوم شخص جمع";
                    break;
                case ENUM_TENSE_OBJECT.WRONGE:
                    title += "نادرست";
                    break;
                default:
                    title = "";
                    break;
            }
            return title;
        }

        public string printTitleType()
        {
            string title = "نوع ";

            switch (this.getTenseType())
            {
                case ENUM_VERB_TYPE.SADE:
                    title += "ساده";
                    break;
                case ENUM_VERB_TYPE.PISHVANDI:
                    title += "پیشوندی";
                    break;
                case ENUM_VERB_TYPE.PISHVANDI_MORAKKAB:
                    title += "پیشوندی مركب";
                    break;
                case ENUM_VERB_TYPE.MORAKKAB:
                    title += "مركب";
                    break;
                case ENUM_VERB_TYPE.EBARATE_FELI:
                    title += "عبارت فعلی";
                    break;
                case ENUM_VERB_TYPE.NAGOZAR:
                    title += "ناگذر یك شخصه";
                    break;
                case ENUM_VERB_TYPE.ESNADI:
                    title += "اسنادی";
                    break;
                case ENUM_VERB_TYPE.GHEIRE_SHAKHSI:
                    title += "غیر شخصی";
                    break;
                default:
                    title += "نامعلوم";
                    break;
            }
            return title;
        }

        public string printTitleTense()
        {
            return
                this.printTitleTime() + " " +
                this.printTitlePassivity() + " " +
                this.printTitlePositivity();
        }

        public void setVerbID(Int32 id)
        {
            this.ID = id;
        }

        public Int32 getVerbID()
        {
            return this.ID;
        }

        public bool isValidTense(ENUM_VERB_TRANSITIVITY transitivity)
        {
            if (this.getTenseTime() == ENUM_TENSE_TIME.AMR)
            {
                if (this.getTensePerson() != ENUM_TENSE_PERSON.PLURAL_SECOND &&
                    this.getTensePerson() != ENUM_TENSE_PERSON.SINGULAR_SECOND)
                    return false;
            }

            if (transitivity == ENUM_VERB_TRANSITIVITY.INTRANSITIVE)
                if (this.getTensePassivity() == ENUM_TENSE_PASSIVITY.PASSIVE)
                    return false;

            return true;
        }

        public bool IsValidPositivity()
        {
            switch (getTenseTime())
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AMR:
                    if (getTensePositivity() == ENUM_TENSE_POSITIVITY.POSITIVE ||
                        getTensePositivity() == ENUM_TENSE_POSITIVITY.NEGATIVE ||
                        getTensePositivity() == ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE ||
                        getTensePositivity() == ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE)
                        return true;
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    if (getTensePositivity() == ENUM_TENSE_POSITIVITY.POSITIVE ||
                        getTensePositivity() == ENUM_TENSE_POSITIVITY.NEGATIVE)
                        return true;
                    break;

                default:
                    break;
            }

            return false;
        }
    }

    public enum PRONOUN_TYPE
    {
        MAZI,
        MOZARE,
        NAGHLI,
        AMR,
        INVALID
    }

    public class VerbWrapper : Verb
    {
        private VerbEntry entry;

        public VerbWrapper()
        {
            this.entry = new VerbEntry("كرد", "كن", ENUM_VERB_TRANSITIVITY.TRANSITIVE);
        }

        public VerbWrapper(VerbEntry ve)
        {
            entry = ve;
        }

        public VerbWrapper(string bon_mazi, string boe_mozare, string pishvand, string felyar, string hezafe, ENUM_VERB_TRANSITIVITY transitivity)
        {
            this.entry = new VerbEntry(bon_mazi, boe_mozare, transitivity, ENUM_VERB_TYPE.INVALID, pishvand, felyar, hezafe);
        }

        public void SetVerbEntry(VerbEntry ve)
        {
            entry = ve;
        }

        private PRONOUN_TYPE getPronounType()
        {
            PRONOUN_TYPE pt;
            ENUM_TENSE_TIME time = this.getTenseTime();

            switch (time)
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    pt = PRONOUN_TYPE.MAZI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    pt = PRONOUN_TYPE.NAGHLI;
                    break;
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AYANDE:
                    pt = PRONOUN_TYPE.MOZARE;
                    break;
                case ENUM_TENSE_TIME.AMR:
                    pt = PRONOUN_TYPE.AMR;
                    break;
                default:
                    pt = PRONOUN_TYPE.INVALID;
                    break;
            }

            return pt;
        }

        private string printPronoun()
        {
            ENUM_TENSE_TIME time = this.getTenseTime();
            ENUM_TENSE_PERSON person = this.getTensePerson();

            PRONOUN_TYPE pt = this.getPronounType();
            string postfix = "";

            switch (pt)
            {
                case PRONOUN_TYPE.MAZI:
                    switch (person)
                    {
                        case ENUM_TENSE_PERSON.SINGULAR_FIRST:
                            postfix = "م";
                            break;
                        case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                            postfix = "ی";
                            break;
                        case ENUM_TENSE_PERSON.SINGULAR_THIRD:
                            postfix = "";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_FIRST:
                            postfix = "یم";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_SECOND:
                            postfix = "ید";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_THIRD:
                            postfix = "ند";
                            break;
                        default:
                            postfix = "";
                            break;
                    }
                    break;

                case PRONOUN_TYPE.MOZARE:
                    switch (person)
                    {
                        case ENUM_TENSE_PERSON.SINGULAR_FIRST:
                            postfix = "م";
                            break;
                        case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                            postfix = "ی";
                            break;
                        case ENUM_TENSE_PERSON.SINGULAR_THIRD:
                            postfix = "د";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_FIRST:
                            postfix = "یم";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_SECOND:
                            postfix = "ید";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_THIRD:
                            postfix = "ند";
                            break;
                        default:
                            postfix = "";
                            break;
                    }
                    break;

                case PRONOUN_TYPE.NAGHLI:
                    switch (person)
                    {
                        case ENUM_TENSE_PERSON.SINGULAR_FIRST:
                            postfix = "‌ام";
                            break;
                        case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                            postfix = "‌ای";
                            break;
                        case ENUM_TENSE_PERSON.SINGULAR_THIRD:
                            postfix = " است";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_FIRST:
                            postfix = "‌ایم";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_SECOND:
                            postfix = "‌اید";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_THIRD:
                            postfix = "‌اند";
                            break;
                        default:
                            postfix = "";
                            break;
                    }
                    break;

                case PRONOUN_TYPE.AMR:
                    switch (person)
                    {
                        case ENUM_TENSE_PERSON.SINGULAR_SECOND:
                            postfix = "";
                            break;
                        case ENUM_TENSE_PERSON.PLURAL_SECOND:
                            postfix = "ید";
                            break;
                        default:
                            postfix = "";
                            break;
                    }
                    break;

                default:
                    break;
            }

            return postfix;
        }

        private string printObjectPronoun()
        {
            ENUM_TENSE_OBJECT obj = this.getTenseObjectPronoun();
            string sop = "";

            switch (obj)
            {
                case ENUM_TENSE_OBJECT.SINGULAR_FIRST:
                    sop = "م";
                    break;
                case ENUM_TENSE_OBJECT.SINGULAR_SECOND:
                    sop = "ت";
                    break;
                case ENUM_TENSE_OBJECT.SINGULAR_THIRD:
                    sop = "ش";
                    break;
                case ENUM_TENSE_OBJECT.PLURAL_FIRST:
                    sop = "مان";
                    break;
                case ENUM_TENSE_OBJECT.PLURAL_SECOND:
                    sop = "تان";
                    break;
                case ENUM_TENSE_OBJECT.PLURAL_THIRD:
                    sop = "شان";
                    break;
                default:
                    sop = "";
                    break;
            }

            return sop;
        }

        private string printStem(string before, string after)
        {
            string stem = "";

            switch (this.getStemTime())
            {
                case STEM_TIME.MAZI:
                    stem = this.entry.pastStem;
                    break;

                case STEM_TIME.MOZARE:
                    stem = this.entry.presentStem;
                    break;

                default:
                    stem = "";
                    break;
            }

            /****** Transform Stem ******************************/

            bool stemStartsWithA = (entry.StartingAlpha(getStemTime()) == STEM_ALPHA.A_X);
            bool No_Space_Before = (before.Length > 0 && before[before.Length - 1] != '‌' && before[before.Length - 1] != ' ');
            bool stemEndsWithA = (entry.EndingAlpha(getStemTime()) == STEM_ALPHA.X_A);
            bool No_Space_After = (after.Length > 0 && after[0] != '‌' && after[0] != ' ');

            if (stemStartsWithA)
            {
                if (No_Space_Before)
                {
                    if (stem.StartsWith("آ"))
                    {
                        stem = "یا" + stem.Substring(1);
                    }
                    else
                    {
                        stem = "ی" + stem.Substring(1);
                    }
                }
            }

            if (stemEndsWithA)
            {
                if (No_Space_After)
                    stem += "ی";
            }

            return stem;
        }

        private string printPositivity()
        {
            ENUM_TENSE_TIME time = this.getTenseTime();
            ENUM_TENSE_POSITIVITY positivity = this.getTensePositivity();

            switch (time)
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    switch (positivity)
                    {
                        case ENUM_TENSE_POSITIVITY.POSITIVE:
                            return "";
                        case ENUM_TENSE_POSITIVITY.NEGATIVE:
                            return "ن";
                        case ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE:
                            return "ب";
                        case ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE:
                            return "م";
                        default:
                            return "";
                    }

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                case ENUM_TENSE_TIME.AYANDE:
                    switch (positivity)
                    {
                        case ENUM_TENSE_POSITIVITY.POSITIVE:
                            return "";
                        case ENUM_TENSE_POSITIVITY.NEGATIVE:
                            return "ن";
                        default:
                            return "";
                    }

                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                case ENUM_TENSE_TIME.AMR:
                    switch (positivity)
                    {
                        case ENUM_TENSE_POSITIVITY.POSITIVE:
                            return "ب";
                        case ENUM_TENSE_POSITIVITY.NEGATIVE:
                            return "ن";
                        case ENUM_TENSE_POSITIVITY.UNUSUAL_POSITIVE:
                            return "";
                        case ENUM_TENSE_POSITIVITY.UNUSUAL_NEGATIVE:
                            return "م";
                        default:
                            return "";
                    }

                default:
                    return "";
            }

        }

        private string printPishvand()
        {
            return this.entry.pishvand;
        }

        private string printFelyar()
        {
            return this.entry.felyar + ((entry.felyar.Length == 0) ? "" : " ");
        }

        private string passiveHole(string v)
        {
            if (this.getTensePassivity() == ENUM_TENSE_PASSIVITY.PASSIVE && this.entry.IsDoHamkard())
            {
                return "";
            }
            else
            {
                return v;
            }
        }

        public string PrintVerb()
        {
            string verbWord = "";
            ENUM_TENSE_PASSIVITY passivity = this.getTensePassivity();
            ENUM_TENSE_TIME time = this.getTenseTime();

            string positivity = this.printPositivity();
            string pronoun = this.printPronoun();

            switch (time)
            {
                case ENUM_TENSE_TIME.MAZI_E_SADE:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, pronoun) + pronoun + this.printObjectPronoun();
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شد" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + "می‌" + this.printStem("می‌", pronoun) + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "می‌شد" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEID:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, "ه بود") + "ه بود" + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شده بود" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMAR:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = "داشت" + pronoun + " " + printFelyar() + printPishvand() + positivity + "می‌" + this.printStem("می‌", pronoun) + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = "داشت" + pronoun + " " + printFelyar() + printPishvand() + this.passiveHole(this.printStem(" ", "ه ") + "ه ") + positivity + "می‌شد" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_SADEYE_NAGHLI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, "ه") + "ه" + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شده" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ESTEMRARIE_NAGHLI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + "می‌" + this.printStem("می‌", "ه") + "ه" + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "می‌شده" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_BAEIDE_NAGHLI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, "ه بوده") + "ه بوده" + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شده بوده" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_MOSTAMARE_NAGHLI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = "داشته" + pronoun + " " + printFelyar() + printPishvand() + positivity + "می‌" + this.printStem("می‌", "ه") + "ه" + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = "داشته" + pronoun + " " + printFelyar() + printPishvand() + this.passiveHole(this.printStem(" ", "ه ") + "ه ") + positivity + "می‌شده" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MAZI_E_ELTEZAMI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, "ه باش") + "ه باش" + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شده باش" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_EKHBARI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + "می‌" + this.printStem("می‌", pronoun) + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "می‌شو" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_MOSTAMAR:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = "دار" + pronoun + " " + printFelyar() + printPishvand() + positivity + "می‌" + this.printStem("می‌", pronoun) + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = "دار" + pronoun + " " + printFelyar() + printPishvand() + this.passiveHole(this.printStem(" ", "ه ") + "ه ") + positivity + "می‌شو" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.MOZARE_E_ELTEZAMI:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, pronoun) + pronoun;
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شو" + pronoun;
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AYANDE:
                    switch (passivity)
                    {
                        case ENUM_TENSE_PASSIVITY.ACTIVE:
                            verbWord = printFelyar() + printPishvand() + positivity + "خواه" + pronoun + " " + this.printStem(" ", "");
                            break;
                        case ENUM_TENSE_PASSIVITY.PASSIVE:
                            verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "خواه" + pronoun + " شد";
                            break;
                        default:
                            break;
                    }
                    break;

                case ENUM_TENSE_TIME.AMR:
                    if (this.getTensePerson() == ENUM_TENSE_PERSON.SINGULAR_SECOND ||
                        this.getTensePerson() == ENUM_TENSE_PERSON.PLURAL_SECOND)
                        switch (passivity)
                        {
                            case ENUM_TENSE_PASSIVITY.ACTIVE:
                                verbWord = printFelyar() + printPishvand() + positivity + this.printStem(positivity, pronoun) + pronoun;
                                break;
                            case ENUM_TENSE_PASSIVITY.PASSIVE:
                                verbWord = printFelyar() + printPishvand() + this.passiveHole(this.printStem("", "ه ") + "ه ") + positivity + "شو" + pronoun;
                                break;
                            default:
                                break;
                        }
                    else verbWord = "";
                    break;

                default:
                    verbWord = "";
                    break;
            }

            return verbWord;
        }

        public string[] PrintInfinitive()
        {
            List<string> infinitives = new List<string>();

            if (this.entry.verbType.Is(ENUM_VERB_TYPE.PISHVANDI))
            {
                infinitives.Add(this.entry.pishvand + this.entry.pastStem + "ن");
                infinitives.Add(this.entry.pishvand + "ن" + this.entry.pastStem + "ن");
            }
            else if (this.entry.verbType.Is(ENUM_VERB_TYPE.SADE))
            {
                infinitives.Add(this.entry.pastStem + "ن");
                infinitives.Add("ن" + this.entry.pastStem + "ن");
            }

            return infinitives.ToArray();
        }

    }
}

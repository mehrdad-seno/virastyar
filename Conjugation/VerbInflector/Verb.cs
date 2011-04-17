using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Morphology.Inflection.Conjugation
{
    public class Verb
    {
        #region Costructor

        public Verb(string hz, string bonmazi, string bonmozareh, string psh, string flyar, VerbTransitivity trnst, VerbType type, bool amrshdn)
        {
            HarfeEzafeh = hz;
            Felyar = flyar;
            Pishvand = psh;
            HastehMazi = bonmazi;
            HastehMozareh = bonmozareh;
            Transitivity = trnst;
            Type = type;
            AmrShodani = amrshdn;
        }

        #endregion

        public string HarfeEzafeh { get; private set; }
        public string Felyar { get; private set; }
        public string Pishvand { get; private set; }
        public string HastehMazi { get; private set; }
        public string HastehMozareh { get; private set; }
        public VerbTransitivity Transitivity { get; private set; }
        public VerbType Type { get; private set; }
        public bool AmrShodani { get; private set; }
        public bool IsZamirPeyvastehValid()
        {
            return !(Transitivity == VerbTransitivity.NAGOZAR);
        }
        public override string ToString()
        {
            string verbStr = HarfeEzafeh + Felyar + " " + Pishvand + HastehMazi;
            verbStr = verbStr.Trim();
            verbStr += "\t" + Transitivity + "\t" + Type;
            return verbStr;
        }
    }

    [Flags]
    public enum VerbType
    {
        SADEH = 1,
        PISHVANDI = SADEH * 2,
        MORAKKAB = PISHVANDI * 2,
        MORAKKABPISHVANDI = MORAKKAB * 2,
        MORAKKABHARFE_EZAFEH = MORAKKABPISHVANDI * 2,
        EBAARATFELI = MORAKKABHARFE_EZAFEH * 2,
        LAZEM_TAKFELI = EBAARATFELI * 2
    }

    public enum VerbTransitivity
    {
        GOZARA,
        NAGOZAR,
        DOVAJHI
    }
}

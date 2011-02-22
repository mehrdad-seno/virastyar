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

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
    public class VerbConjugationElement
    {
        public VerbConjugationElement(Verb vrb, ZamirPeyvastehType zamir, ShakhsType shakhstype, TenseFormationType tenseFormationType, TensePositivity positivity)
        {
            VerbStem = vrb;
            ZamirPeyvasteh = zamir;
            Shakhs = shakhstype;
            TenseForm = tenseFormationType;
            Positivity = positivity;
        }

        //TODO think about 'ast' and 'nist'
        public Verb VerbStem;
        public ZamirPeyvastehType ZamirPeyvasteh;
        public ShakhsType Shakhs;
        public TenseFormationType TenseForm;
        public TensePositivity Positivity;
        public bool IsPayehFelMasdari()
        {
            if(Shakhs==ShakhsType.SEVVOMSHAKHS_MOFRAD)//TODO 
                return true;
            return false;
        }
        public override string ToString()
        {
            return VerbStem.ToString() + "\t" + ZamirPeyvasteh + "\t" + Shakhs + "\t" + TenseForm + "\t" + Positivity;
        }
        public bool IsValid()
        {
            if (TenseForm == TenseFormationType.HAL_SAADEH_EKHBARI || TenseForm == TenseFormationType.HAAL_ELTEZAMI || TenseForm == TenseFormationType.AMR)
                if (VerbStem.HastehMozareh == null)
                    return false;
            if (TenseForm == TenseFormationType.AMR && VerbStem.AmrShodani == false)
                return false;
            return (IsZamirPeyvastehValid() && IsShakhsValid() && IsNegativeValid());
        }

        private bool IsNegativeValid()
        {
            return true;
        }
        private bool IsShakhsValid()
        {
            if (TenseForm == TenseFormationType.PAYEH_MAFOOLI && Shakhs!=ShakhsType.Shakhs_NONE)
                return false;
            if (TenseForm != TenseFormationType.PAYEH_MAFOOLI && Shakhs == ShakhsType.Shakhs_NONE)
                return false;
            if ((TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_SADEH || TenseForm == TenseFormationType.GOZASHTEH_NAGHLI_ESTEMRAARI) && Shakhs == ShakhsType.SEVVOMSHAKHS_MOFRAD)
                return false;
            if (TenseForm == TenseFormationType.AMR &&
                !(Shakhs == ShakhsType.DOVVOMSHAKHS_JAM || Shakhs == ShakhsType.DOVVOMSHAKHS_MOFRAD))
                return false;
            return true;
        }
        private bool IsZamirPeyvastehValid()
        {
            if (TenseForm == TenseFormationType.AMR && (ZamirPeyvasteh == ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD || ZamirPeyvasteh == ZamirPeyvastehType.DOVVOMSHAKHS_JAM ))
                return false;
          if(isEqualShaksZamir(ZamirPeyvasteh,Shakhs))
              return false;
            if (ZamirPeyvasteh == ZamirPeyvastehType.ZamirPeyvasteh_NONE && TenseForm == TenseFormationType.PAYEH_MAFOOLI)
                return true;
            if(ZamirPeyvasteh==ZamirPeyvastehType.ZamirPeyvasteh_NONE)
                return true;
            return VerbStem.IsZamirPeyvastehValid() && TenseForm!=TenseFormationType.PAYEH_MAFOOLI;
        }
        private bool isEqualShaksZamir(ZamirPeyvastehType zamirPeyvastehType,ShakhsType shakhsType)
        {
            //if (zamirPeyvastehType == ZamirPeyvastehType.ZamirPeyvasteh_NONE && shakhsType == ShakhsType.Shakhs_NONE)
            //    return true;
            //if (zamirPeyvastehType == ZamirPeyvastehType.SEVVOMSHAKHS_MOFRAD && shakhsType == ShakhsType.SEVVOMSHAKHS_MOFRAD)
            //    return true;
            //if (zamirPeyvastehType == ZamirPeyvastehType.SEVVOMSHAKHS_JAM && shakhsType == ShakhsType.SEVVOMSHAKHS_JAM)
            //    return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD && shakhsType == ShakhsType.DOVVOMSHAKHS_MOFRAD)
                return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_JAM && shakhsType == ShakhsType.DOVVOMSHAKHS_JAM)
                return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_MOFRAD && shakhsType == ShakhsType.AVALSHAKHS_MOFRAD)
                return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_JAM && shakhsType == ShakhsType.AVALSHAKHS_JAM)
                return true;
            //if (zamirPeyvastehType == ZamirPeyvastehType.SEVVOMSHAKHS_JAM && shakhsType == ShakhsType.SEVVOMSHAKHS_MOFRAD)
            //    return true;
            //if (zamirPeyvastehType == ZamirPeyvastehType.SEVVOMSHAKHS_MOFRAD && shakhsType == ShakhsType.SEVVOMSHAKHS_JAM)
            //    return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_JAM && shakhsType == ShakhsType.DOVVOMSHAKHS_MOFRAD)
                return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.DOVVOMSHAKHS_MOFRAD && shakhsType == ShakhsType.DOVVOMSHAKHS_JAM)
                return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_JAM && shakhsType == ShakhsType.AVALSHAKHS_MOFRAD)
                return true;
            if (zamirPeyvastehType == ZamirPeyvastehType.AVALSHAKHS_MOFRAD && shakhsType == ShakhsType.AVALSHAKHS_JAM)
                return true;
            return false;
        }
    }

    public enum ZamirPeyvastehType 
    {
        ZamirPeyvasteh_NONE,
        AVALSHAKHS_MOFRAD,
        DOVVOMSHAKHS_MOFRAD,
        SEVVOMSHAKHS_MOFRAD,
        AVALSHAKHS_JAM,
        DOVVOMSHAKHS_JAM,
        SEVVOMSHAKHS_JAM
    }

    public enum ShakhsType
    {
        Shakhs_NONE,
        AVALSHAKHS_MOFRAD,
        DOVVOMSHAKHS_MOFRAD,
        SEVVOMSHAKHS_MOFRAD,
        AVALSHAKHS_JAM,
        DOVVOMSHAKHS_JAM,
        SEVVOMSHAKHS_JAM
    }

    public enum TenseFormationType
    {
        HAL_SAADEH_EKHBARI,
        HAAL_ELTEZAMI,
        AMR,
        GOZASHTEH_SADEH,
        GOZASHTEH_ESTEMRAARI,
        GOZASHTEH_NAGHLI_SADEH,
        GOZASHTEH_NAGHLI_ESTEMRAARI,
        PAYEH_MAFOOLI
    }

    public enum TensePositivity
    {
        POSITIVE,
        NEGATIVE
    }
}

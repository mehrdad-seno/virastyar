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
using System.Runtime.InteropServices;

namespace VirastyarWordAddin
{
    [ComVisible(true)]
    public class VirastyarAddin : IVirastyarAddin
    {
        #region IVirastyarAddin Members

        public void PinglishConvert_Action()
        {
            Globals.ThisAddIn.PinglishConvert_Action();
        }

        public void PinglishConvertAll_Action()
        {
            Globals.ThisAddIn.PinglishConvertAll_Action();
        }

        public void CheckDates_Action()
        {
            Globals.ThisAddIn.CheckDates_Action();
        }

        public void CheckNumbers_Action()
        {
            Globals.ThisAddIn.CheckNumbers_Action();
        }

        public void CheckSpell_Action()
        {
            Globals.ThisAddIn.CheckSpell_Action();
        }

        public void PreCheckSpell_Action()
        {
            Globals.ThisAddIn.PreCheckSpell_Action();
        }

        public void CheckPunctuation_Action()
        {
            Globals.ThisAddIn.CheckPunctuation_Action();
        }

        public void CheckAllPunctuation_Action()
        {
            Globals.ThisAddIn.CheckAllPunctuation_Action();
        }

        public void RefineAllCharacters_Action()
        {
            Globals.ThisAddIn.RefineAllCharacters_Action();
        }

        public void AddinSettings_Action()
        {
            Globals.ThisAddIn.AddinSettings_Action();
        }

        public void About_Action()
        {
            Globals.ThisAddIn.About_Action();
        }

        public void AutoComplete_Action()
        {
            Globals.ThisAddIn.AutoComplete_Action();
        }

        public void Help_Action()
        {
            Globals.ThisAddIn.Help_Action();
        }

        public void Tip_Action()
        {
            Globals.ThisAddIn.Tip_Action();
        }

        #endregion
    }
}

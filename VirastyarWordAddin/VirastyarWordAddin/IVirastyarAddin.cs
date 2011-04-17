using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VirastyarWordAddin
{
    [ComVisible(true)]
    interface IVirastyarAddin
    {
        void PinglishConvert_Action();
        void PinglishConvertAll_Action();
        void CheckDates_Action();
        void CheckNumbers_Action();
        void CheckSpell_Action();
        void PreCheckSpell_Action();
        void CheckPunctuation_Action();
        void CheckAllPunctuation_Action();
        void RefineAllCharacters_Action();
        void AddinSettings_Action();
        void About_Action();
        void AutoComplete_Action();
        void Help_Action();
        void Tip_Action();
        void SendReport_Action();
        void VirastyarUpdate_Action();
        bool Get_IsUpdateAvailable();
    }
}

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
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.PinglishConvert_Action();
        }

        public void PinglishConvertAll_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.PinglishConvertAll_Action();
        }

        public void CheckDates_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.CheckDates_Action();
        }

        public void CheckNumbers_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.CheckNumbers_Action();
        }

        public void CheckSpell_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.CheckSpell_Action();
        }

        public void PreCheckSpell_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.PreCheckSpell_Action();
        }

        public void CheckPunctuation_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.CheckPunctuation_Action();
        }

        public void CheckAllPunctuation_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.CheckAllPunctuation_Action();
        }

        public void RefineAllCharacters_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.RefineAllCharacters_Action();
        }

        public void AddinSettings_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.AddinSettings_Action();
        }

        public void About_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.About_Action();
        }

        public void AutoComplete_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.AutoComplete_Action();
        }

        public void Help_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.Help_Action();
        }

        public void Tip_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.Tip_Action();
        }

        public void SendReport_Action()
        {
            // TODO:
            // LogReporter.ShowLogReportWindow();
        }

        public bool Get_IsUpdateAvailable()
        {
            if (Globals.ThisAddIn.IsLoaded)
                return Globals.ThisAddIn.Get_IsUpdateAvailable();

            return false;
        }

        public void VirastyarUpdate_Action()
        {
            if (Globals.ThisAddIn.IsLoaded)
                Globals.ThisAddIn.VirastyarUpdate_Action();
        }

        #endregion
    }
}

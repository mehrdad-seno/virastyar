using System.Drawing;
using System.Windows.Forms;
using System;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public interface IInteractiveVerificationWindow
    {
        bool CancelationPending { get; }
        Color HighlightBackColor { get; set; }
        Color HighlightForeColorError { get; set; }
        Color HighlightForeColorWarning { get; set; }
        Color HighlightForeColorInformation { get; set; }

        IButtonControl AcceptButton { get; set; }
        UserSelectedActions LastUserAction { get; }

        bool AddTextStatusLabel(string name, string value);
        bool SetStatusValue(string name, string value);

        void EnableAction(UserSelectedActions action);
        void DisableAction(UserSelectedActions action);

        void InvokeMethod(Action act);
        IWin32Window GetWin32Window();

        //VerificationResult SetVerificationData(VerificationData data);
        //void InformVerifierProceedType(ProceedTypes proceedType);

    }
}

using System;
using Microsoft.Office.Interop.Word;
using SCICT.Microsoft.Office.Word.ContentReader;


namespace VirastyarWordAddin.Verifiers.Basics
{
    public abstract class VerifierBase
    {
        public void StartInteractive(Document document)
        {
            Document = document;
            IsInteractiveMode = true;
            InteractiveVerificationWindow.ShowWindowModeless(this);
        }

        public void StartBatchMode(Document document)
        {
            Document = document;
            IsInteractiveMode = false;
            BatchModeVerificationWindow.ShowWindowModal(this);
            ShowBatchModeStats();
        }

        /// <summary>
        /// Put initialization stuff here
        /// </summary>
        public virtual void OnInitWindow()
        {
        }

        /// <summary>
        /// Put resource disposal stuff here
        /// </summary>
        public virtual void OnReleaseWindow()
        {
        }

        public bool IsInteractiveMode { get; private set; }

        public IInteractiveVerificationWindow VerificationWindowInteractive { get; set; }

        public IBatchModeVerificationWindow VerificationWindowBatchMode { get; set; }

        public bool CancelationPending
        {
            get
            {
                if (IsInteractiveMode)
                    return VerificationWindowInteractive.CancelationPending;
                else
                    return VerificationWindowBatchMode.CancelationPending;
            }
        }

        public Document Document { get; protected set; }

        public abstract bool InitParagraph(RangeWrapper par);
        public abstract bool HasVerification();
        public abstract VerificationData GetNextVerificationData();
        public abstract ProceedTypes GetProceedTypeForVerificationResult(VerificationResult verRes);

        public abstract string Title { get; }

        public abstract string HelpTopicFileName { get; }

        public abstract UserSelectedActions ActionsToDisable { get; }

        public abstract Type SuggestionViewerType { get; }

        public abstract bool ShowBatchModeStats();

        protected abstract bool NeedRefinedStrings { get; }
        public abstract void ApplyBatchModeAction(RangeWrapper rangeToChange, string suggestion);
    }
}

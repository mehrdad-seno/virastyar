using System;
using System.Drawing;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public delegate void ActionInvokedEventHandler(object sender, UserSelectedActions selAction, string arg);

    /// <summary>
    /// Common interface for user controls which are to be used to demonstrate
    /// suggestions to users
    /// </summary>
    public interface ISuggestionsViwer
    {
        string SelectedSuggestion { get; }
        int MainControlTop { get; }
        Size MinimumSize { get; set; }
        Size Size { get; set; }

        event EventHandler SelectedSuggestionChanged;
        event EventHandler MainControlTopChanged;
        event EventHandler SuggestionSelected;
        event ActionInvokedEventHandler ActionInvoked;

        IInteractiveVerificationWindow ParentVerificationWindow { get; set; }

        void Clear();
        void SetFocus();
        void SetSuggestions(ISuggestions suggestions);
    }
}

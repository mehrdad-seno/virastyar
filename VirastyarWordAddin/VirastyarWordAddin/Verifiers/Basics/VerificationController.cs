using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SCICT.Utility.GUI;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public class VerificationController
    {

        /// <summary>
        /// key: the full type name of the verifier, value: a list of existing verifiers
        /// The idea behind list of verifiers is that each open document may have a verifier of its desired type.
        /// </summary>
        private static readonly Dictionary<string, List<VerifierBase>> s_existingVerifiers = new Dictionary<string, List<VerifierBase>>();

        #region Registeration and Unregisteration of Interactive Verifiers
        public static bool IsInteractiveVerifierRegistered<T>(Document docToCheck) where T: VerifierBase
        {
            var frm = GetRegisteredInteractiveVerifierForm<T>(docToCheck);
            return frm != null;
        }

        public static Form GetRegisteredInteractiveVerifierForm<T>(Document docToCheck) where T:VerifierBase
        {
            string typeFullName = typeof(T).FullName ?? "";

            if (s_existingVerifiers.ContainsKey(typeFullName))
            {
                var verifiers = s_existingVerifiers[typeFullName];
                for (int i = 0; i < verifiers.Count; i++)
                {
                    VerifierBase verif = verifiers[i];
                    if (verif.Document.Equals(docToCheck))
                    {
                        var verifWindow = (verif.VerificationWindowInteractive as Form);
                        if (verifWindow == null || verifWindow.Disposing || verifWindow.IsDisposed)
                        {
                            verifiers.RemoveAt(i);
                            return null;
                        }

                        return verifWindow;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Registers an interactive verifier.
        /// </summary>
        /// <param name="verifToReg">The verifier to register.</param>
        /// <param name="docToCheck">The document to check.</param>
        /// <returns>returns false if already registered or cannot register the verifier, true otherwise.</returns>
        private static bool RegisterInteractiveVerifier<T>(T verifToReg, Document docToCheck) where T: VerifierBase
        {
            // see if such verifier is already running
            string typeFullName = verifToReg.GetType().FullName ?? "";

            if (!IsInteractiveVerifierRegistered<T>(docToCheck))
            {
                if (s_existingVerifiers.ContainsKey(typeFullName))
                {
                    var verifiers = s_existingVerifiers[typeFullName];
                    verifiers.Add(verifToReg);
                }
                else // if the dictionary does not contain the key part
                {
                    s_existingVerifiers.Add(typeFullName, new List<VerifierBase> { verifToReg });
                }
                return true;
            }

            return false;
        }

        public static bool UnregisterInteractiveVerifier(Type verifierType, Document document)
        {
            string fullTypeName =  verifierType.FullName ?? "";
            if (s_existingVerifiers.ContainsKey(fullTypeName))
            {
                var verifs = s_existingVerifiers[fullTypeName];

                for (int i = verifs.Count - 1; i >= 0; i--)
                {
                    if (verifs[i].Document.Equals(document))
                    {
                        verifs.RemoveAt(i);
                    }
                }

                if (verifs.Count <= 0)
                {
                    s_existingVerifiers.Remove(fullTypeName);
                }
            }

            return true;
        }

        #endregion

        public static VerifierBase CreateAndStartInteractive<T>(Microsoft.Office.Interop.Word.Application application, params object[] args) where T : VerifierBase
        {
            Document doc = null;
            if (!GetActiveDocument(application, ref doc))
            {
                PersianMessageBox.Show(ThisAddIn.GetWin32Window(),
                    "متأسفانه به نظر می‌رسد این متن محافظت شده است و ویراستیار قادر به تغییر آن نیست",
                    "متنِ محافظت شده", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
            return CreateAndStartInteractive<T>(doc, args);
        }

        public static VerifierBase CreateAndStartInteractive<T>(Document document, params object[] args) where T: VerifierBase
        {
            if(document == null)
                return null;

            if(document.ProtectionType != WdProtectionType.wdNoProtection)
            {
                PersianMessageBox.Show("متأسفانه این متن محافظت شده است و ویراستیار قادر به تغییر آن نیست!", "متنِ محافظت شده", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            try
            {
                var frm = GetRegisteredInteractiveVerifierForm<T>(document);
                if(frm != null)
                {
                    frm.Activate();
                    return null;
                }

                var verif = (VerifierBase)Activator.CreateInstance(typeof(T), args);
                if (RegisterInteractiveVerifier(verif, document))
                {
                    verif.StartInteractive(document);
                    return verif;
                }
            }
            catch (TargetInvocationException)
            {
            }

            return null;
        }

        public static VerifierBase CreateAndStartBatchMode<T>(Microsoft.Office.Interop.Word.Application application, params object[] args) where T : VerifierBase
        {
            Document doc = null;
            if (!GetActiveDocument(application, ref doc))
            {
                PersianMessageBox.Show(ThisAddIn.GetWin32Window(),
                    "متأسفانه به نظر می‌رسد این متن محافظت شده است و ویراستیار قادر به تغییر آن نیست",
                    "متنِ محافظت شده",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            return CreateAndStartBatchMode<T>(doc, args);
        }

        public static VerifierBase CreateAndStartBatchMode<T>(Document document, params object[] args) where T: VerifierBase
        {
            if (document == null)
                return null;

            if (document.ProtectionType != WdProtectionType.wdNoProtection)
            {
                PersianMessageBox.Show("متأسفانه این متن محافظت شده است و ویراستیار قادر به تغییر آن نیست!", "متنِ محافظت شده", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            try
            {
                // if there's an interactive verifier running do not run the batch mode verifier
                var frm = GetRegisteredInteractiveVerifierForm<T>(document);
                if (frm != null)
                {
                    frm.Activate();
                    return null;
                }


                var verif = (VerifierBase)Activator.CreateInstance(typeof (T), args);
                verif.StartBatchMode(document);
                return verif;
            }
            catch (TargetInvocationException)
            {
            }

            return null;
        }

        private static bool GetActiveDocument(Microsoft.Office.Interop.Word.Application app, ref Document activeDoc)
        {
            try
            {
                activeDoc = app.ActiveDocument;
                Debug.Assert(app.Documents.Count > 0);
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

    }
}

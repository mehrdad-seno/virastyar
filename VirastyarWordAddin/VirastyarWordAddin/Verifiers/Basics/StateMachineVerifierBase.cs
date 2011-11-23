using System;
using System.Collections.Generic;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public abstract class StateMachineVerifierBase : VerifierBase
    {
        #region inner class for loop detection

        private class VerificationInstance
        {
            public VerificationInstance(string context, int verifIndex, int verifLength, string sugs)
            {
                this.Context = context;
                this.VerifIndex = verifIndex;
                this.VerifLength = verifLength;
                this.Suggestion = sugs;
            }

            private string Context { get; set; }
            private int VerifIndex { get; set; }
            private int VerifLength { get; set; }
            private string Suggestion { get; set; }

            public override bool Equals(object obj)
            {
                if(Object.ReferenceEquals(this, obj))
                    return true;
                var other = obj as VerificationInstance;
                if(other == null)
                    return false;

                return this.Context == other.Context &&
                    this.VerifIndex == other.VerifIndex &&
                    this.VerifLength == other.VerifLength &&
                    this.Suggestion == other.Suggestion;
            }

            //private static bool RangesAreEqual(RangeWrapper r1, RangeWrapper r2)
            //{
            //    if(r1 == null && r2 == null)
            //        return true;
            //    if(r1 == null || r2 == null)
            //        return false;

            //    return r1.Equals(r2);
            //}

            public override int GetHashCode()
            {
                int code = 0;
                if (this.Context != null)
                    code += this.Context.GetHashCode();

                code += VerifIndex.GetHashCode();
                code += VerifLength.GetHashCode();

                if (this.Suggestion != null)
                    code += this.Suggestion.GetHashCode();

                return code;
            }

            public override string ToString()
            {
                string ctx = Context;

                if(Context.Length > 10)
                    ctx = Context.Substring(0, 10) + "...";

                string verif = this.Context.Substring(VerifIndex, VerifLength);

                return String.Format("C: {0}, V: {1}, S: {2}", ctx, verif, Suggestion);
            }
        }

        #endregion

        private readonly HashSet<VerificationInstance> m_batchHistory = new HashSet<VerificationInstance>();
        private bool m_isLoopDetected = false;

        protected abstract bool InitParagraphString(string par);
        protected abstract StringVerificationData FindNextPattern();

        private RangeWrapper m_mainPar;
        private string m_rawContent;
        protected VerificationData m_curVerifData;

        public override sealed bool InitParagraph(RangeWrapper par)
        {
            if(par == null || !par.IsRangeValid)
                return false;

            m_mainPar = par.GetCopy();

            RefreshContent();

            if (String.IsNullOrEmpty(m_rawContent))
                return false;

            string refinedContent = StringUtil.RefineAndFilterPersianWord(m_rawContent);
            if (String.IsNullOrEmpty(refinedContent))
                return false;

            string strToVerify = this.NeedRefinedStrings ? refinedContent : m_rawContent;

            if (!base.IsInteractiveMode)
            {
                m_batchHistory.Clear();
                m_isLoopDetected = false;
            }

            return InitParagraphString(strToVerify);
        }

        private void RefreshContent()
        {
            m_mainPar.Invalidate();
            m_rawContent = m_mainPar.Text;
        }

        //public override void VerifyParagraphBatchMode(RangeWrapper par)
        //{
        //    m_batchHistory.Clear();

        //    var parToModify = par.GetCopy();

        //    string rawContent = par.Text;
        //    if (String.IsNullOrEmpty(rawContent))
        //        return;

        //    string refinedContent = StringUtil.RefineAndFilterPersianWord(rawContent);
        //    if (String.IsNullOrEmpty(refinedContent))
        //        return;

        //    string strToVerify = this.NeedRefinedStrings ? refinedContent : rawContent;

        //    if (!InitParagraph(strToVerify))
        //        return;

        //    while (!VerificationWindowBatchMode.CancelationPending)
        //    {
        //        if (parToModify == null || !parToModify.IsRangeValid)
        //            break;

        //        int len, ind;
        //        FindPatternIteration(out ind, out len);
        //        if (len <= 0 || ind < 0)
        //            break;

        //        RangeWrapper foundRange;

        //        if (NeedRefinedStrings)
        //        {
        //            foundRange = parToModify.GetRangeWithCharIndex(
        //                StringUtil.IndexInNotFilterAndRefinedString(rawContent, ind),
        //                StringUtil.IndexInNotFilterAndRefinedString(rawContent, ind + len - 1)
        //            );
        //        }
        //        else
        //        {
        //            foundRange = parToModify.GetRangeWithCharIndex(ind, ind + len - 1);
        //        }

        //        var defsug = GetDefaultSuggestionForRecentPattern();

        //        if (foundRange != null && foundRange.IsRangeValid)
        //        {
        //            if (VerificationWindowBatchMode.ShowProgressAtDocument)
        //                foundRange.Select();

        //            ApplyBatchModeAction(foundRange, defsug);

        //            // this call is crucial
        //            parToModify.Invalidate();
        //        }

        //        var verifInstance = new VerificationInstance(parToModify, foundRange, defsug);
        //        if(m_batchHistory.Contains(verifInstance))
        //        {
        //            VerificationWindowBatchMode.CurrentStatus = "(تشخیص حلقه)";
        //            return;
        //        }

        //        m_batchHistory.Add(verifInstance);
        //    }
        //}

        public override sealed bool HasVerification()
        {
            if(CancelationPending)
                return false;

            if(!this.IsInteractiveMode && m_isLoopDetected)
                return false;

            var strVerifData = FindNextPattern();

            if (strVerifData == null || !strVerifData.IsValid)
                return false;

            m_curVerifData = new VerificationData(strVerifData, m_mainPar, m_rawContent, NeedRefinedStrings);
            if (!m_curVerifData.IsValid)
                return false;
            return true;
        }

        protected void RefreshForChangeCalled(string errorText, string selectedSug)
        {
            RefreshContent();
            if (!IsInteractiveMode)
            {
                m_curVerifData.RangeToHighlight.Invalidate();

                var verifInstance = new VerificationInstance(m_rawContent, m_curVerifData.ErrorIndex, m_curVerifData.ErrorLength, selectedSug);
                if (m_batchHistory.Contains(verifInstance))
                {
                    VerificationWindowBatchMode.CurrentStatus = "(تشخیص حلقه)";
                    m_isLoopDetected = true;
                }
                else
                {
                    m_batchHistory.Add(verifInstance);
                }
            }
        }

        protected void RefreshForChangeAllCalled(string errorText, string selectedSug)
        {
            RefreshContent();
        }

        public override sealed VerificationData GetNextVerificationData()
        {
            return m_curVerifData;
        }
    }
}

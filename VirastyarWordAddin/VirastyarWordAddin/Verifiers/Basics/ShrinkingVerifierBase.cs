using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public abstract class ShrinkingVerifierBase : VerifierBase
    {
        private RangeWrapper m_mainPar;

        public override bool InitParagraph(RangeWrapper par)
        {
            m_mainPar = par;
            if(par == null || !par.IsRangeValid)
                return false;

            m_remainedParagraph = m_mainPar.GetCopy();
            m_progInd = 0;
            return true;
            //m_curProcType = ProceedTypes.Finished;
        }

        private int m_progInd = 0;
        private RangeWrapper m_remainedParagraph;
        protected VerificationData m_curVerifData;


        public override bool HasVerification()
        {
            if(CancelationPending)
                return false;

            //// if the text is shorter or equal than the length that is going to be cut from the paragraph;
            //// then we expect that no text should remain in the remainder paragraph; but in action it does, 
            //// so hereby we prevent it.
            if (m_remainedParagraph == null || !m_remainedParagraph.IsRangeValid || m_progInd >= m_remainedParagraph.Text.Length)
                return false;

            if(m_progInd > 0)
            {
                //m_remainedParagraph = m_remainedParagraph.GetRangeWithOffset(m_progInd - m_remainedParagraph.Start + 1);
                m_remainedParagraph = m_remainedParagraph.GetRangeWithCharIndex(m_progInd);
                if (m_remainedParagraph == null || !m_remainedParagraph.IsRangeValid)
                    return false;

                m_remainedParagraph = m_remainedParagraph.TrimRange();
                if (m_remainedParagraph == null || !m_remainedParagraph.IsRangeValid)
                    return false;
            }

            m_curVerifData = ProcessSubParagraph(m_remainedParagraph);

            if (m_curVerifData == null || !m_curVerifData.IsValid)
                return false;

            // m_progInd should be further changed by user actions
            m_progInd = m_curVerifData.ErrorEnd + 1;

            return true;
        }

        protected void RefreshForChangeCalled(string errorText, string selectedSug)
        {
            int inc = selectedSug.Length - errorText.Length;
            //Debug.WriteLine(String.Format("SugLen: {0}, rangeLen: {1}, inc: {2}", selectedSug.Length, errorText.Length, inc));

            m_progInd += inc;
            m_remainedParagraph.Invalidate();
        }

        private VerificationData ProcessSubParagraph(RangeWrapper subPar)
        {
            string rawContent = subPar.Text;
            if (rawContent == null)
                return null;

            string contentToVerify = rawContent;

            if (NeedRefinedStrings)
                contentToVerify = StringUtil.RefineAndFilterPersianWord(rawContent);

            var strVerifData = FindPattern(contentToVerify);
            if (strVerifData == null || !strVerifData.IsValid)
                return null;

            var verifData = new VerificationData(strVerifData, subPar, rawContent, NeedRefinedStrings);
            if(!verifData.IsValid)
                return null;
            return verifData;
        }

        public override VerificationData GetNextVerificationData()
        {
            return m_curVerifData;
        }


        /// <summary>
        /// Finds the first and the most prominent pattern in the string.
        /// </summary>
        /// <param name="content">The content to search the pattern in.</param>
        protected abstract StringVerificationData FindPattern(string content);

    }
}

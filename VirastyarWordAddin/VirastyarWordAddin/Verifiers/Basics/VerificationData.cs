using System.Diagnostics;
using SCICT.Microsoft.Office.Word.ContentReader;
using SCICT.NLP.Utility;
using System;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public class VerificationData : StringVerificationData
    {
        public VerificationData()
        {
        }

        public VerificationData(StringVerificationData copy, RangeWrapper rContext, string rawContent, bool needsRefinedString)
        {
            SetContent(copy, rContext, rawContent, needsRefinedString);
        }

        public void SetContent(StringVerificationData copy, RangeWrapper rContext, string rawContent, bool needsRefinedString)
        {
            int dummy = 0;
            SetContent(copy, rContext, rawContent, needsRefinedString, ref dummy);
        }
        
        public void SetContent(StringVerificationData copy, RangeWrapper rContext, string rawContent, bool needsRefinedString, ref int rangeRepairOffset)
        {
            base.SetContent(copy);

            ErrorContext = rawContent;

            RangeWrapper foundRange;
            string strToBeMatched;

            if (needsRefinedString)
            {
                int newInd = StringUtil.IndexInNotFilterAndRefinedString(rawContent, ErrorIndex);
                int newEnd = StringUtil.IndexInNotFilterAndRefinedString(rawContent, ErrorEnd);
                
                foundRange = rContext.GetRangeWithCharIndex(newInd + rangeRepairOffset, newEnd + rangeRepairOffset);
                strToBeMatched = rawContent.Substring(newInd, newEnd - newInd + 1);

                this.ErrorIndex = newInd;
                this.ErrorLength = newEnd - newInd + 1;
            }
            else
            {
                strToBeMatched = rawContent.Substring(ErrorIndex, ErrorLength);
                foundRange = rContext.GetRangeWithCharIndex(ErrorIndex + rangeRepairOffset, ErrorEnd + rangeRepairOffset);
            }

            this.RangeToHighlight = foundRange;


            // if it is an invalid range, no need to do anything, the IsValid property will return false automatically
            if (foundRange == null || !foundRange.IsRangeValid) 
                return;

            if(foundRange.Text == strToBeMatched) // Well Done no need to do any range recovery stuff
                return;


            // Try to recover here, by fixing rangeRepairOffset

            var remStrParText = rawContent.Substring(ErrorIndex); // error index before repair done above
            var remParRange = rContext.GetRangeWithCharIndex(ErrorIndex + rangeRepairOffset);
            string remRngParText = remParRange.Text;

            // see if any of the rem pars contain the other
            bool isStrInRng = true;
            int remi = remRngParText.IndexOf(remStrParText, StringComparison.Ordinal);
            if (remi < 0)
            {
                remi = remStrParText.IndexOf(remRngParText, StringComparison.Ordinal);
                isStrInRng = false;
            }

            if (remi < 0)
            {
                RangeToHighlight = null;
                return;
                //Debug.Assert(false, "Non recoverable situation met!");
            }

            if (!isStrInRng)
                remi = -remi;


            // make sure that you can find the appropriate range
            const int numTries = 3;
            int i;
            for (i = 0; i < numTries; i++)
            {
                var testRange = rContext.GetRangeWithCharIndex(ErrorIndex + rangeRepairOffset + remi, ErrorEnd + rangeRepairOffset + remi);
                if (testRange.Text == strToBeMatched)
                {
                    rangeRepairOffset += remi;
                    RangeToHighlight = testRange;
                    break;
                }

                remi = remi < 0 ? remi - 1 : remi + 1;
            }

            if (i >= 3) // i.e., if the above loop did not break
            {
                RangeToHighlight = null;
                //Debug.Assert(false, "Substrings found but ranges not found!");
            }             
        }

        public string ErrorContext { get; set; }
        public string ErrorText
        {
            get
            {
                return this.ErrorContext.Substring(this.ErrorIndex, this.ErrorLength);
            }
        }

        public RangeWrapper RangeToHighlight { get; set; }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && RangeToHighlight != null && RangeToHighlight.IsRangeValid;
            }
        }
    }
}

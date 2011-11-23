using System;
using System.Linq;
using Microsoft.Office.Interop.Word;
using SCICT.NLP.Utility;
using SCICT.Microsoft.Office.Word.ContentReader;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

namespace VirastyarWordAddin
{
    /// <summary>
    /// This class contains methods which test the functionality of the addin.
    /// These methods were formerly called from within addin buttons event handler, and 
    /// spread in the ThisAddin class. Now they are categorized in this class, and can be
    /// referenced whenever needed.
    /// </summary>
    public class AddinTestMethods
    {
        public static void MakeEachParagraphVisible()
        {
            foreach (RangeWrapper par in RangeWrapper.ReadParagraphsStartingFromCursor(Globals.ThisAddIn.Application.ActiveDocument))
            {
                string parContent = par.Text;

                par.Select();
                if (DialogResult.OK != MessageBox.Show(StringUtil.MakeStringVisible(parContent), "Paragraph Content", MessageBoxButtons.OKCancel))
                    return;
            }
        }

        public static void FindEquations()
        {
            foreach (RangeWrapper par in RangeWrapper.ReadParagraphsStartingFromCursor(Globals.ThisAddIn.Application.ActiveDocument))
            {
                string parContent = par.Text;

                par.Select();

                var sb = new StringBuilder();
                sb.AppendFormat("#Fields: {0}", par.Range.Fields.Count).AppendLine();
                int numRangeChars = par.Range.Characters.Count;
                int numTextChars = par.Text.Length;
                sb.AppendFormat("#RangeChars: {0}", numRangeChars).AppendLine();
                sb.AppendFormat("#TextChars: {0}", numTextChars).AppendLine();
                if(numTextChars != numRangeChars)
                {
                    sb.AppendFormat("#Diff: {0}", numTextChars - numRangeChars).AppendLine();
                    //sb.AppendFormat("#RRs: {0}", CountFoundSubstrings(parContent, "\r\r")).AppendLine();
                }

                sb.AppendLine().AppendLine("Content Made Visible:").AppendLine(StringUtil.MakeStringVisible(parContent));

                if (DialogResult.OK != MessageBox.Show(sb.ToString(), "Paragraph Content", MessageBoxButtons.OKCancel))
                    return;
            }
        }

        public static int CountFoundSubstrings(string main, string key)
        {
            int count = 0;
            int st = 0;
            while(main.IndexOf(key, st, StringComparison.Ordinal) >= st)
            {
                count++;
                st += key.Length;
            }

            return count;
        }

        public static void CheckWordTokenizerCorrespondenceWithRange(bool refine)
        {
            var tokenizer = new WordTokenizer(WordTokenizerOptions.ReturnPunctuations);

            foreach (
                RangeWrapper par in
                    RangeWrapper.ReadParagraphsStartingFromCursor(Globals.ThisAddIn.Application.ActiveDocument))
            {
                int rangeRepairOffset = 0;

                string parContent = par.Text;
                string tobeVerified = parContent;
                if (refine)
                    tobeVerified = StringUtil.RefineAndFilterPersianWord(tobeVerified);

                int count = 0;
                foreach (var wordInfo in tokenizer.Tokenize(tobeVerified))
                {
                    count++;
                    if(count % 5 != 0) continue;

                    // check for every 5 words

                    int widx = wordInfo.Index;
                    int wend = wordInfo.EndIndex;

                    if (refine)
                    {
                        widx = StringUtil.IndexInNotFilterAndRefinedString(parContent, widx);
                        wend = StringUtil.IndexInNotFilterAndRefinedString(parContent, wend);
                    }

                    var wordRange = par.GetRangeWithCharIndex(widx + rangeRepairOffset, wend + rangeRepairOffset);

                    if(wordRange == null || !wordRange.IsRangeValid)
                        continue;

                    string wordRangeText = wordRange.Text;
                    if (refine)
                        wordRangeText = StringUtil.RefineAndFilterPersianWord(wordRangeText);

                    if (wordRangeText != wordInfo.Value)
                    {
                        // Try to recover here, by fixing rangeRepairOffset
                        var originalWord = parContent.Substring(widx, wend - widx + 1);
                        var remStrParText = parContent.Substring(widx);

                        var remParRange = par.GetRangeWithCharIndex(widx + rangeRepairOffset);
                        string remRngParText = remParRange.Text;


                        // see if any of the rem pars contain the other
                        int remi = -1;
                        bool isStrInRng = true;
                        remi = remRngParText.IndexOf(remStrParText, StringComparison.Ordinal);
                        if (remi < 0)
                        {
                            remi = remStrParText.IndexOf(remRngParText, StringComparison.Ordinal);
                            isStrInRng = false;
                        }

                        if (remi < 0)
                        {
                            Debug.Assert(false, "Non recoverable situation met!");
                            //continue;
                        }

                        if(!isStrInRng)
                            remi = -remi;

                        // make sure that you can find the appropriate range

                        int i;
                        for (i = 0; i < 3; i++)
                        {
                            var testRange = par.GetRangeWithCharIndex(widx + rangeRepairOffset + remi, wend + rangeRepairOffset+ remi);
                            if (testRange.Text == originalWord)
                            {
                                rangeRepairOffset += remi;
                                wordRange = testRange;
                                wordRangeText = wordRange.Text;
                                if (refine)
                                    wordRangeText = StringUtil.RefineAndFilterPersianWord(wordRangeText);

                                break;
                            }

                            remi = remi < 0 ? remi - 1 : remi + 1;
                        }

                        if (i >= 3) // i.e., if the above loop did not break
                        {
                            Debug.Assert(false, "Substrings found but ranges not found!");
                            //continue;
                        }
                    }

                    string msg = String.Format("Range: {0}\r\n({1} - {2})\r\n\r\nTokenizer: {3}\r\n({4} - {5})\r\nRangeOffset:{6}",
                                               wordRange.Text, wordRange.Start, wordRange.End, wordInfo.Value,
                                               wordInfo.Index, wordInfo.EndIndex, rangeRepairOffset);

                    wordRange.Select();

                    Debug.Assert(wordRangeText == wordInfo.Value, String.Format(
                        "The word tokenizer's found word boundries do not match the corresponding range content." +
                        Environment.NewLine +
                        "Tokenizer: {0} -> ({1} - {2})" + Environment.NewLine +
                        "Range: {3} -> ({4} - {5})",
                        wordInfo.Value, wordInfo.Index, wordInfo.EndIndex,
                        wordRange.Text, wordRange.Start, wordRange.End));


                    if (DialogResult.OK != MessageBox.Show(msg, "word check", MessageBoxButtons.OKCancel))
                        return;

                }
            } // end of foreach par
        }
    }
}

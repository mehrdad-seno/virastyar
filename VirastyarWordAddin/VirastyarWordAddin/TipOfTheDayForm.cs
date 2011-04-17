using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VirastyarWordAddin.Properties;
using System.IO;
using System.Xml;
using VirastyarWordAddin.Verifiers.Basics;

namespace VirastyarWordAddin
{
    public partial class TipOfTheDayForm : Form
    {
        private readonly TipOfTheDayData m_tipsData;
        private readonly int[] m_shuffledInds;
        private int m_curLoc = 0;

        public TipOfTheDayForm()
        {
            InitializeComponent();
        }

        public TipOfTheDayForm(TipOfTheDayData tipsData)
            : this()
        {
            m_tipsData = tipsData;
            m_shuffledInds = ShuffleIndices(tipsData.TipRtfs.Count);
            m_curLoc = 0;
            ShowCurTip();

            this.cbShowOnStartup.Checked = Settings.Default.TipOfTheDayShowOnStartup;
        }


        private int[] ShuffleIndices(int length)
        {
            int[] inds = new int[length];
            for (int i = 0; i < length; i++)
                inds[i] = i;

            Random r = new Random();

            for (int t = 0; t < 3; t++)
            {
                for (int i = 1; i < length; i++)
                {
                    int ri = r.Next(i, length);

                    int temp = inds[i - 1];
                    inds[i - 1] = inds[ri];
                    inds[ri] = temp;
                }
            }

            return inds;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static void ShowTipOfTheDay(Stream stream)
        {
            var tipsData = new TipOfTheDayData(XmlReader.Create(stream));
            if (tipsData.TipRtfs.Count <= 0)
                return;

            var frm = new TipOfTheDayForm(tipsData);
            frm.ShowDialog(WindowWrapper.GetWordActiveWindowWrapper());
        }

        public static void ShowTipOfTheDay(string fileName)
        {
            var tipsData = new TipOfTheDayData(fileName);
            if (tipsData.TipRtfs.Count <= 0)
                return;

            var frm = new TipOfTheDayForm(tipsData);
            frm.ShowDialog(WindowWrapper.GetWordActiveWindowWrapper());
        }

        private void cbShowOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.TipOfTheDayShowOnStartup = cbShowOnStartup.Checked;
            Settings.Default.Save();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            m_curLoc = (m_curLoc + 1) % m_tipsData.TipRtfs.Count;
            ShowCurTip();
        }

        private void ShowCurTip()
        {
            try
            {
                int index = m_shuffledInds[m_curLoc];
                this.rtbTip.Rtf = m_tipsData.TipRtfs[index];
                string link = m_tipsData.TipLinks[index];

                if (!String.IsNullOrEmpty(link))
                {
                    this.linkLabelOuterLink.Tag = link;
                    this.linkLabelOuterLink.Visible = true;
                }
                else
                {
                    this.linkLabelOuterLink.Tag = "";
                    this.linkLabelOuterLink.Visible = false;
                }
            }
            catch
            {
                m_curLoc = (m_curLoc + 1) % m_shuffledInds.Length;
                ShowCurTip();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            m_curLoc--;
            if (m_curLoc < 0)
                m_curLoc = m_tipsData.TipRtfs.Count - 1;
            //m_curLoc = (m_curLoc - 1) % m_tipsData.TipRtfs.Count;
            ShowCurTip();
        }

        private void rtbTip_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.LinkText))
            {
                try
                {
                    System.Diagnostics.Process.Start(e.LinkText);
                }
                catch
                {
                }
            }
        }

        private void linkLabelOuterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lnk = sender as LinkLabel;
            if (lnk != null)
            {
                string addr = lnk.Tag as string;
                if (!String.IsNullOrEmpty(addr))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(addr);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}

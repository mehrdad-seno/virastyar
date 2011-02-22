// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

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

namespace VirastyarWordAddin
{
    public partial class TipOfTheDayForm : Form
    {
        private TipOfTheDayData m_tipsData;
        private int[] m_shuffledInds;
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
            TipOfTheDayData tipsData = new TipOfTheDayData(XmlReader.Create(stream));
            if (tipsData.TipRtfs.Count <= 0)
                return;

            TipOfTheDayForm frm = new TipOfTheDayForm(tipsData);
            frm.Show();
        }

        public static void ShowTipOfTheDay(string fileName)
        {
            TipOfTheDayData tipsData = new TipOfTheDayData(fileName);
            if (tipsData.TipRtfs.Count <= 0)
                return;

            TipOfTheDayForm frm = new TipOfTheDayForm(tipsData);
            frm.Show();
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

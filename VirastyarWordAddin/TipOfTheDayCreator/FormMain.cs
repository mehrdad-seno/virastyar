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
using System.IO;
using System.Xml.Linq;

namespace TipOfTheDayCreator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            UpdateTitle();
        }

        private const string MainTitle = "Tip of the Day Creator";

        private string m_curFileName = "";
        private bool m_isModified = false;

        private List<string> m_curTipRtfs = new List<string>();
        private List<string> m_curTipLinks = new List<string>();
        private int m_curSelectedListIndex = -1;

        private void UpdateTitle()
        {
            string title = MainTitle;
            if (!String.IsNullOrEmpty(m_curFileName))
            {
                title += " - " + Path.GetFileName(m_curFileName);
            }
            else
            {
                title += " - Untitled.tod";
            }

            if (m_isModified)
            {
                title += " *";
            }

            this.Text = title;
        }

        private void SetModified()
        {
            m_isModified = true;
            UpdateTitle();
        }

        private void ClearModified()
        {
            m_isModified = false;
            UpdateTitle();
        }

        private void newTipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnNew();
        }

        private void OnNew()
        {
            if (m_isModified)
            {
                DialogResult dr = MessageBox.Show("Save changes?", "", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                    return;
                else if (dr == DialogResult.Yes)
                {
                    if (!OnSave())
                        return;
                }
            }

            m_curFileName = "";
            m_curTipRtfs.Clear();
            m_curTipLinks.Clear();
            
            txtLink.Text = "";
            lstTipsList.Items.Clear();
            m_curSelectedListIndex = -1;
            rtbTipText.Text = "";
            ClearModified();
        }

        private bool OnSave()
        {
            if (String.IsNullOrEmpty(m_curFileName))
            {
                return OnSaveAs();
            }
            else
            {
                OnSaveTipToList();

                if (!SaveTipsToFile(m_curFileName))
                {
                    MessageBox.Show("Could not save to file!");
                    return false;
                }
                else
                {
                    ClearModified();
                    return true;
                }
            }
        }

        private bool SaveTipsToFile(string fileName)
        {
            try
            {
                XDocument xdoc = new XDocument();
                var rootElem = new XElement("root");

                for(int i = 0; i < m_curTipRtfs.Count; i++)
                {
                    var tip = m_curTipRtfs[i];
                    var tipLink = m_curTipLinks[i];

                    var tipElem = new XElement("Tip", tip);
                    if (!String.IsNullOrEmpty(tipLink))
                        tipElem.Add(new XAttribute("link", tipLink));
                    rootElem.Add(tipElem);
                }

                xdoc.Add(rootElem);

                xdoc.Save(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadTipsFromFile(string fileName)
        {
            this.m_curTipRtfs.Clear();
            this.m_curTipLinks.Clear();

            try
            {
                XDocument xdoc = XDocument.Load(fileName, LoadOptions.PreserveWhitespace);
                var rootElem = xdoc.Element("root");

                foreach (var tipElem in rootElem.Elements("Tip"))
                {
                    this.m_curTipRtfs.Add(tipElem.Value);
                    var linkAttr = tipElem.Attribute("link");
                    if (linkAttr != null)
                    {
                        m_curTipLinks.Add(linkAttr.Value.Trim());
                    }
                    else
                    {
                        m_curTipLinks.Add("");
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool OnSaveAs()
        {
            string newFileName = "";
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "*.tod|*.tod";
                var dr = dlg.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return false;

                newFileName = dlg.FileName;
            }

            OnSaveTipToList();
            if (SaveTipsToFile(newFileName))
            {
                m_curFileName = newFileName;
                ClearModified();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnOpen();
        }

        private bool OnOpen()
        {
            if (m_isModified)
            {
                if (!OnSave())
                    return false;
            }

            string fileToBeOpened = "";
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.tod|*.tod";
                var dr = dlg.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return false;
                fileToBeOpened = dlg.FileName;
            }

            if (!this.LoadTipsFromFile(fileToBeOpened))
                return false;

            m_isUpdatingText = true;
            m_isUpdatingList = true;

            lstTipsList.Items.Clear();
            m_curSelectedListIndex = -1;
            rtbTipText.Text = "";

            for (int i = 0; i < m_curTipRtfs.Count; i++)
            {

                try
                {
                    rtbTipText.Rtf = m_curTipRtfs[i];
                }
                catch
                {
                    m_curTipLinks[i] = "";
                    rtbTipText.Text = "";
                }

                string listTitle = GetListTitle(rtbTipText.Text);

                lstTipsList.Items.Add(listTitle);

            }

            m_isUpdatingText = false;
            m_isUpdatingList = false;


            if (m_curTipRtfs.Count > 0)
                lstTipsList.SelectedIndex = 0;

            m_curFileName = fileToBeOpened;
            m_isModified = false;
            UpdateTitle();

            return true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnSaveAs();
        }

        private void addNewTipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_curTipRtfs.Add("");
            m_curTipLinks.Add("");
            lstTipsList.Items.Add("[عنوان جدید]");
            lstTipsList.SelectedIndex = lstTipsList.Items.Count - 1;
            rtbTipText.Text = "";
            txtLink.Text = "";
            SetModified();

        }

        private bool m_isUpdatingList = false;
        private bool m_isUpdatingText = false;

        //private void saveTipToListToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    OnSaveTipToList();
        //}

        private void OnSaveTipToPreviousList()
        {
            int seli = m_curSelectedListIndex;

            if (seli < 0)
                return;

            string text = GetListTitle(rtbTipText.Text);

            m_isUpdatingList = true;

            lstTipsList.Items[seli] = text;
            m_curTipRtfs[seli] = rtbTipText.Rtf;
            m_curTipLinks[seli] = txtLink.Text.Trim();

            m_isUpdatingList = false;
        }


        private void OnSaveTipToList()
        {
            int seli = lstTipsList.SelectedIndex;

            if (seli < 0)
                return;

            string text = GetListTitle(rtbTipText.Text);

            m_isUpdatingList = true;

            lstTipsList.Items[seli] = text;
            m_curTipRtfs[seli] = rtbTipText.Rtf;
            m_curTipLinks[seli] = txtLink.Text.Trim();

            m_isUpdatingList = false;
        }

        private string GetListTitle(string text)
        {
            text = text.Trim().Replace("\t", " ").Replace("\r", "").Replace("\n", "");
            text = text.Substring(0, Math.Min(10, text.Length));
            text += "...";
            return text;
        }

        private void lstTipsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_isUpdatingList)
                return;


            int seli = this.lstTipsList.SelectedIndex;
            if (seli == m_curSelectedListIndex)
                return;

            OnSaveTipToPreviousList();

            m_curSelectedListIndex = seli;
            if (seli < 0)
                return;

            string rtf = this.m_curTipRtfs[seli];

            m_isUpdatingText = true;
            try
            {
                this.rtbTipText.Rtf = rtf;
            }
            catch
            {
                this.rtbTipText.Text = "";
                this.m_curTipRtfs[seli] = "";
            }
            this.txtLink.Text = this.m_curTipLinks[seli];
            m_isUpdatingText = false;
        }

        private void rtbTipText_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdatingList || m_isUpdatingText)
                return;

            if (!m_isModified)
            {
                SetModified();
            }
        }

        private void txtLink_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdatingList || m_isUpdatingText)
                return;

            if (!m_isModified)
            {
                SetModified();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_isModified)
            {
                var dr = MessageBox.Show("Save changes bofore exit?", "",  MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dr == DialogResult.OK)
                {
                    if (!OnSave())
                        e.Cancel = true;
                }
            }
        }

    }
}

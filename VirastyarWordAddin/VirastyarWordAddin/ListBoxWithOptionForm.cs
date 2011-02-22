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
using System.Windows.Forms;

namespace VirastyarWordAddin
{
    public partial class ListBoxWithOptionForm : Form
    {
        public ListBoxWithOptionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the list box form.
        /// returns null if cancelled or invalid item is selected.
        /// </summary>
        /// <returns></returns>
        public static string ShowListBoxForm(string[] itemsToShow, string message, string caption, string optionCaption, out bool optionSelected)
        {
            optionSelected = false;
            string selectedItem = null;
            using (ListBoxWithOptionForm frm = new ListBoxWithOptionForm())
            {
                frm.lstItems.Items.AddRange(itemsToShow);
                frm.lblMessage.Text = message;
                frm.Text = caption;
                frm.cbOption.Text = optionCaption;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    optionSelected = frm.cbOption.Checked;
                    if(frm.lstItems.SelectedIndex >= 0)
                    {
                        selectedItem = frm.lstItems.Items[frm.lstItems.SelectedIndex] as string;
                        if (selectedItem.Trim().Length <= 0)
                            selectedItem = null;
                    }
                }

            }

            return selectedItem;
        }

        private void lstItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstItems.SelectedIndex >= 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ListBoxForm_Load(object sender, EventArgs e)
        {
            if (lstItems.Items.Count > 0)
            {
                lstItems.SelectedIndex = 0;
            }
        }
    }
}

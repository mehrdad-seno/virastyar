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
    public partial class ExceptionForm : Form
    {
        const string ShowDetailsCaption = "نمایش جزئیات";
        const string LessDetailsCaption = "عدم نمایش جزئیات";
        public Exception ExceptionToShow { get; private set; }

        public ExceptionForm()
        {
            InitializeComponent();
            this.btnShowDetails.Text = ShowDetailsCaption;
        }

        public ExceptionForm(Exception ex) : this()
        {
            this.ExceptionToShow = ex;
            this.tbDetails.Text = ex.ToString();
        }

        public static void ShowExceptionForm(Exception ex)
        {
            using (ExceptionForm frm = new ExceptionForm(ex))
            {
                frm.ShowDialog();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            if (tbDetails.Visible == true)
            {
                btnShowDetails.Text = ShowDetailsCaption;
                tbDetails.Visible = false;
                this.Height = MinimumSize.Height;
            }
            else
            {
                btnShowDetails.Text = LessDetailsCaption;
                tbDetails.Visible = true;
                this.Height = MaximumSize.Height;
            }
        }
    }
}

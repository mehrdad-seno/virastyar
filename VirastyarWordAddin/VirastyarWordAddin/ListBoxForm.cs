using System;
using System.Linq;
using System.Windows.Forms;

namespace VirastyarWordAddin
{
    public partial class ListBoxForm : Form
    {
        public ListBoxForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the list box form.
        /// returns null if cancelled or invalid item is selected.
        /// </summary>
        /// <returns></returns>
        public static string ShowListBoxForm(string[] itemsToShow, string defaultItem, string message, string caption)
        {
            return ShowListBoxForm(null, itemsToShow, defaultItem, message, caption);
        }

        /// <summary>
        /// Shows the list box form.
        /// returns null if cancelled or invalid item is selected.
        /// </summary>
        /// <returns></returns>
        public static string ShowListBoxForm(IWin32Window owner, string[] itemsToShow, string defaultItem, string message, string caption)
        {
            string selectedItem = null;
            using (var frm = new ListBoxForm())
            {
                frm.lstItems.Items.AddRange(itemsToShow);
                frm.lblMessage.Text = message;
                frm.Text = caption;

                if (!string.IsNullOrEmpty(defaultItem))
                {
                    int index = -1;
                    for (int i = 0; i < itemsToShow.Length; i++)
                    {
                        if (itemsToShow[i] == defaultItem)
                        {
                            index = i;
                            break;
                        }
                    }

                    frm.lstItems.SelectedIndex = index;
                }

                var dr = owner != null ? frm.ShowDialog(owner) : frm.ShowDialog();
                if (dr == DialogResult.OK)
                {
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
            if (lstItems.Items.Count > 0 && lstItems.SelectedIndex < 0)
            {
                lstItems.SelectedIndex = 0;
            }
        }
    }
}

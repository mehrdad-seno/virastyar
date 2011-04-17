using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SCICT.NLP.Persian.Constants;
using SCICT.Utility.GUI;

namespace VirastyarWordAddin
{
    public partial class DictionaryEditor : Form
    {
        private BindingList<DictionaryElement> _dic;
        private readonly string _dicfilename;

        public DictionaryEditor(string dic)
        {
            InitializeComponent();

            _dicfilename = dic;

            _dic = new BindingList<DictionaryElement>((from line in File.ReadAllLines(_dicfilename).Distinct() select new DictionaryElement(line)).ToList());
            DicGridView.DataSource = _dic;

            DicGridView.Focus();

            var addNewEndings = new[] { new { Name = "مصوت", Value = EndingType.Vowel }, new { Name = "صامت", Value = EndingType.Consonantal }, new { Name = "نامشخص", Value = EndingType.Unknown } };
            EndingComboBox.DataSource = addNewEndings;
            EndingComboBox.DisplayMember = "Name";
            EndingComboBox.ValueMember = "Value";
            EndingComboBox.SelectedValue = EndingType.Unknown;

            var girdEndings = new[] { new { Name = "مصوت", Value = EndingType.Vowel }, new { Name = "صامت", Value = EndingType.Consonantal }, new { Name = "نامشخص", Value = EndingType.Unknown } };
            var endingCol = DicGridView.Columns["Ending"] as DataGridViewComboBoxColumn;
            endingCol.DataSource = girdEndings;
            endingCol.DisplayMember = "Name";
            endingCol.ValueMember = "Value";
        }

        private void AddWordButton_Click(object sender, EventArgs e)
        {
            if (NewWordText.Text.Trim() == "")
            {
                NewWordText.Text = "";
                MessageToolTip.Show("کلمه‌ای وارد نکرده‌اید!", this, AddWordGroupBox.Location, 1200);
                IsNounCheckBox.Checked = IsVerbCheckBox.Checked = IsAdjectiveCheckBox.Checked = IsVerbCheckBox.Checked = false;
                EndingComboBox.SelectedValue = EndingType.Unknown;
                return;
            }

            var query = (from entry in _dic where entry.Word.Equals(NewWordText.Text.Trim()) select entry);
            if (query.Count() != 0)
            {
                MessageToolTip.Show("این کلمه قبلاً وجود دارد!", this, AddWordGroupBox.Location, 1200);
                foreach (DataGridViewRow selectedRow in DicGridView.SelectedRows)
                {
                    selectedRow.Selected = false;
                }
                foreach (DataGridViewRow row in DicGridView.Rows)
                {
                    if (row.Cells["Word"].Value.Equals(NewWordText.Text.Trim()))
                    {
                        row.Selected = true;
                        DicGridView.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
                NewWordText.Text = "";
                IsNounCheckBox.Checked = IsVerbCheckBox.Checked = IsAdjectiveCheckBox.Checked = IsVerbCheckBox.Checked = false;
                EndingComboBox.SelectedValue = EndingType.Unknown;
                return;
            }

            var newElement = new DictionaryElement
                                 {
                                     Word = NewWordText.Text.Trim(),
                                     Tag = ((IsNounCheckBox.Checked) ? PersianPOSTag.N : 0) |
                                           ((IsVerbCheckBox.Checked) ? PersianPOSTag.V : 0) |
                                           ((IsAdjectiveCheckBox.Checked) ? PersianPOSTag.AJ : 0) |
                                           (((EndingType)EndingComboBox.SelectedValue == EndingType.Vowel) ? PersianPOSTag.VowelEnding : 0) |
                                           (((EndingType)EndingComboBox.SelectedValue == EndingType.Consonantal) ? PersianPOSTag.ConsonantalEnding : 0)
                                 };
            _dic.Add(newElement);
            IsChanged = true;
            NewWordText.Text = "";
            IsNounCheckBox.Checked = IsVerbCheckBox.Checked = IsAdjectiveCheckBox.Checked = IsVerbCheckBox.Checked = false;
            EndingComboBox.SelectedValue = EndingType.Unknown;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var result = PersianMessageBox.Show("آیا از صحت تغییراتی که داده‌اید اطمینان دارید؟\r\nبا استفاده از بارگذاری مجدد می‌توانید تمام تغییرات را خنثی کنید!", "ذخیره‌سازی", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            StreamWriter writer = new StreamWriter(_dicfilename);
            foreach (var element in _dic)
            {
                writer.WriteLine(element.ToString());
            }
            writer.Close();
            IsChanged = false;
        }

        private void DeleteCurrentButton_Click(object sender, EventArgs e)
        {
            if (DicGridView.SelectedRows.Count == 0) return;
            var result = PersianMessageBox.Show("از حذف این کلمه(‌ها) اطمینان دارید؟", "حذف کلمه", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            foreach (DataGridViewRow row in DicGridView.SelectedRows)
            {
                DicGridView.Rows.Remove(row);
                IsChanged = true;
            }
        }

        private void DeleteAllButton_Click(object sender, EventArgs e)
        {
            var result = PersianMessageBox.Show("تمام کلمه‌های لغت‌نامه شخصی شما پاک شوند؟", "حذف کل لغات", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            DicGridView.Rows.Clear();
            IsChanged = true;
        }

        public bool IsChanged { get; set; }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            var result = PersianMessageBox.Show("با بارگذاری مجدد لغت‌نامه، تمام تغییراتی که داده‌اید از دست می‌رود، موافقید؟", "بارگذاری مجدد", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;
            _dic.Clear();
            _dic = new BindingList<DictionaryElement>((from line in File.ReadAllLines(_dicfilename) select new DictionaryElement(line)).ToList());
            DicGridView.DataSource = _dic;
            IsChanged = false;
        }

        private void DicGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            IsChanged = true;
        }

        private void UserDicManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsChanged)
            {
                var result = PersianMessageBox.Show("لغت‌نامه تغییر کرده است، می‌خواهید آن‌را ذخیره کنید؟", "اعمال تغییرات", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        SaveButton_Click(sender, e);
                        break;

                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NewWordText_TextChanged(object sender, EventArgs e)
        {
            if (NewWordText.Text.Trim().EndsWith("و") || NewWordText.Text.Trim().EndsWith("ه"))
            {
                var font = new Font(EndingLabel.Font, FontStyle.Bold);
                EndingLabel.Font = font;
                EndingLabel.ForeColor = Color.Red;
            }
            else
            {
                var font = new Font(EndingLabel.Font, FontStyle.Regular);
                EndingLabel.Font = font;
                EndingLabel.ForeColor = Color.Black;
            }
        }

        private void HelpEndingLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PersianMessageBox.Show("برای کلمات مختوم به «و» در صورتی که انتهای کلمه آوای واو می‌دهد (مانند نحو) مختوم به صامت و در صورتی که صدای او می‌دهد (مانند رو) مختوم به مصوت است.\r\nبرای کلمات مختوم به «ه» در صورتی که انتهای کلمه آوای ها می‌دهد (مانند ماه) مختوم به صامت و در صورتی که صدای اِ (کسره) می‌دهد (مانند خانه) مختوم به مصوت است.\r\nدر رابطه با سایر کلمات این مقدار مهم نخواهد بود.", "آوای انتهای کلمه",
                                   MessageBoxButtons.OK);
        }
    }

    #region EndingType
    public enum EndingType
    {
        Vowel,
        Consonantal,
        Unknown
    }
    #endregion

    #region DictionaryElement
    public class DictionaryElement
    {
        public DictionaryElement(string line)
        {
            Word = line.Split('\t')[0];
            Tag = (PersianPOSTag)Enum.Parse(typeof(PersianPOSTag), line.Split('\t')[2]);
        }

        public DictionaryElement()
        {

        }

        public override string ToString()
        {
            return string.Format("{0}\t0\t{1}", Word, (Tag == PersianPOSTag.UserPOS || Tag == 0) ? PersianPOSTag.UserPOS : (Tag & ~PersianPOSTag.UserPOS));
        }

        public string Word { get; set; }
        public PersianPOSTag Tag { get; set; }

        public bool IsNoun
        {
            get { return (Tag & PersianPOSTag.N) == PersianPOSTag.N; }
            set
            {
                if (value)
                {
                    Tag |= PersianPOSTag.N;
                }
                else
                {
                    Tag &= ~PersianPOSTag.N;
                }
            }
        }

        public bool IsVerb
        {
            get { return (Tag & PersianPOSTag.V) == PersianPOSTag.V; }
            set
            {
                if (value)
                {
                    Tag |= PersianPOSTag.V;
                }
                else
                {
                    Tag &= ~PersianPOSTag.V;
                }
            }
        }

        public bool IsAdjective
        {
            get { return (Tag & PersianPOSTag.AJ) == PersianPOSTag.AJ; }
            set
            {
                if (value)
                {
                    Tag |= PersianPOSTag.AJ;
                }
                else
                {
                    Tag &= ~PersianPOSTag.AJ;
                }
            }
        }

        public EndingType Ending
        {
            get
            {
                if ((Tag & PersianPOSTag.VowelEnding) == PersianPOSTag.VowelEnding)
                {
                    return EndingType.Vowel;
                }
                if ((Tag & PersianPOSTag.ConsonantalEnding) == PersianPOSTag.ConsonantalEnding)
                {
                    return EndingType.Consonantal;
                }
                return EndingType.Unknown;
            }
            set
            {
                switch (value)
                {
                    case EndingType.Vowel:
                        Tag |= PersianPOSTag.VowelEnding;
                        Tag &= ~PersianPOSTag.ConsonantalEnding;
                        break;
                    case EndingType.Consonantal:
                        Tag |= PersianPOSTag.ConsonantalEnding;
                        Tag &= ~PersianPOSTag.VowelEnding;
                        break;
                    case EndingType.Unknown:
                        Tag &= ~PersianPOSTag.VowelEnding;
                        Tag &= ~PersianPOSTag.ConsonantalEnding;
                        break;
                }
            }
        }
    }
    #endregion
}
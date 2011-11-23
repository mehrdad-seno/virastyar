using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace VirastyarWordAddin.WinFormsUtils
{
    /// <summary>
    /// Provides utility to toggle status bar to a 
    /// progress-bar and reverse to its normal status
    /// </summary>
    public class StatusBarToProgressToggler
    {
        private bool[] m_prevStatusItemsVisibilities = null;
        private bool m_prevPanelEnableStatus = true;

        private ToolStripProgressBar m_statusProgressBar;
        private StatusStrip m_statusStrip;
        private Panel m_panelToDisable;
        private double m_progressBarExtent;


        /// <summary>
        /// Gets or sets the height of the progress bar.
        /// (default value is 16 pixels)
        /// </summary>
        /// <value>The height of the progress bar.</value>
        public int ProgressBarHeight { get; set; }

        /// <summary>
        /// A real number between 0 and 1.0 which indicates
        /// the proportion of progress-bar width with respect
        /// to the containing status-strip. (default is two third).
        /// </summary>
        /// <value>The progress bar width extent.</value>
        public double ProgressBarWidthExtent
        {
            get
            {
                return m_progressBarExtent;
            }

            set
            {
                m_progressBarExtent = value;
                if (m_progressBarExtent <= 0)
                    m_progressBarExtent = 0.1;

                if (m_progressBarExtent > 1.0)
                    m_progressBarExtent = 1.0;
            }
        }

        /// <summary>
        /// Gets or sets the progress-bar value.
        /// </summary>
        /// <value>The progress-bar value.</value>
        public int ProgressValue
        {
            get
            {
                return m_statusProgressBar.Value;
            }

            set
            {
                int valueToSet = value;
                if (valueToSet < m_statusProgressBar.Minimum)
                    valueToSet = m_statusProgressBar.Minimum;
                if (valueToSet > m_statusProgressBar.Maximum)
                    valueToSet = m_statusProgressBar.Maximum;

                m_statusProgressBar.Value = valueToSet;
            }
        }

        /// <summary>
        /// Gets or sets the minimum possible value for the 
        /// progress bar to be shown.
        /// </summary>
        /// <value>the minimum possible value for the 
        /// progress bar to be shown.</value>
        public int Minimum
        {
            get
            {
                return m_statusProgressBar.Minimum;
            }

            set
            {
                m_statusProgressBar.Minimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum possible value for the 
        /// progress bar to be shown.
        /// </summary>
        /// <value>The maximum possible value for the 
        /// progress bar to be shown.</value>
        public int Maximum
        {
            get
            {
                return m_statusProgressBar.Maximum;
            }

            set
            {
                m_statusProgressBar.Maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the style of progress bar.
        /// The default style is Marquee.
        /// </summary>
        /// <value>The progress bar style.</value>
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                return m_statusProgressBar.Style;
            }

            set
            {
                m_statusProgressBar.Style = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance 
        /// is in progress bar mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in progress bar mode; 
        /// 	otherwise, <c>false</c>.
        /// </value>
        public bool IsInProgressBarMode
        {
            get
            {
                return m_prevStatusItemsVisibilities != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBarToProgressToggler"/> class.
        /// </summary>
        /// <param name="statusStripToToggle">The StatusStrip to toggle.</param>
        /// <param name="panelToDisable">The (optional) Panel to disable.</param>
        public StatusBarToProgressToggler(
            StatusStrip statusStripToToggle, Panel panelToDisable)
        {
            if (statusStripToToggle == null)
                throw new ArgumentNullException("statusStripToToggle",
                    "The reference to the status-bar to be toggled should not be null");

            m_statusProgressBar = new ToolStripProgressBar();

            // set the default values for the properties
            this.ProgressBarHeight = 15;
            this.ProgressBarWidthExtent = 2.0 / 3.0;
            this.ProgressBarStyle = ProgressBarStyle.Marquee;

            m_statusStrip = statusStripToToggle;
            m_panelToDisable = panelToDisable;
        }

        /// <summary>
        /// Toggles the status bar to a status progress bar, and vice versa.
        /// </summary>
        /// <param name="showProgressBar">if set to <c>true</c> shows the progress 
        /// bar only in the status bar; <c>false</c> otherwise.</param>
        public void Toggle(bool showProgressBar)
        {
            if (showProgressBar)
            {
                if (m_panelToDisable != null)
                {
                    m_prevPanelEnableStatus = m_panelToDisable.Enabled;
                    m_panelToDisable.Enabled = false;
                }

                m_prevStatusItemsVisibilities = new bool[m_statusStrip.Items.Count];

                for (int i = 0; i < m_prevStatusItemsVisibilities.Length; i++)
                {
                    m_prevStatusItemsVisibilities[i] = m_statusStrip.Items[i].Visible;
                    m_statusStrip.Items[i].Visible = false;
                }

                m_statusStrip.Items.Add(m_statusProgressBar);
                ResizeStatusProgressBar(m_statusProgressBar, m_statusStrip);
                m_statusStrip.Resize += statusStrip_Resize;
            }
            else
            {
                if (m_prevStatusItemsVisibilities == null)
                {
                    // do nothing; you cannot make something visibile before
                    // it is made invisible
                    return;
                }

                if (m_panelToDisable != null)
                {
                    m_panelToDisable.Enabled = m_prevPanelEnableStatus;
                }

                m_statusStrip.Resize -= statusStrip_Resize;
                m_statusStrip.Items.Remove(m_statusProgressBar);
                for (int i = 0; i < m_prevStatusItemsVisibilities.Length; i++)
                {
                    m_statusStrip.Items[i].Visible = m_prevStatusItemsVisibilities[i];
                }
                m_prevStatusItemsVisibilities = null;
            }
        }

        /// <summary>
        /// Handles the Resize event of the statusStrip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void statusStrip_Resize(object sender, EventArgs e)
        {
            ResizeStatusProgressBar(m_statusProgressBar, sender as StatusStrip);
        }

        /// <summary>
        /// Resizes the status progress bar.
        /// </summary>
        /// <param name="progressBar">The progress bar.</param>
        /// <param name="statusStrip">The status strip.</param>
        private void ResizeStatusProgressBar(ToolStripProgressBar progressBar, StatusStrip statusStrip)
        {
            progressBar.Size = new Size((int)(statusStrip.Width * this.ProgressBarWidthExtent),
                this.ProgressBarHeight);
            Padding oldMargin = progressBar.Margin;
            Padding margin = new Padding((statusStrip.Width - progressBar.Width) / 2,
                oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
            progressBar.Margin = margin;
        }
    }
}

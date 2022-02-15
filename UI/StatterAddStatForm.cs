using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using ACT_Plugin.Model;

namespace ACT_Plugin.UI
{
    public partial class StatterAddStatForm : Form
    {
        protected StatterStat _addedStat = null;
        public StatterStat AddedStat { get { return _addedStat; } }

        public StatterAddStatForm()
        {
            InitializeComponent();
        }

        public void SetUsedStats(List<string> usedStats)
        {
            cmbStat.BeginUpdate();
            cmbStat.Items.Clear();
            foreach (string stat in StatterStat.GetAvailableStats(usedStats))
                cmbStat.Items.Add(stat);
            cmbStat.EndUpdate();

            if (cmbStat.Items.Count > 0)
                cmbStat.SelectedIndex = 0;
        }

        public void SetColour(Color colour)
        {
            dlgColour.Color = colour;
            pnlColour.BackColor = colour;
        }

        private void pnlColour_Click(object sender, EventArgs e)
        {
            dlgColour.Color = pnlColour.BackColor;
            if (dlgColour.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                pnlColour.BackColor = dlgColour.Color;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbStat.SelectedItem == null) return;

            _addedStat = new StatterStat(cmbStat.SelectedItem.ToString()) { Colour = pnlColour.BackColor };
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}

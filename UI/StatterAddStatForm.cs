using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using ActStatter.Model;
using Advanced_Combat_Tracker;

namespace ActStatter.UI
{
    public partial class StatterAddStatForm : Form
    {
        protected StatterStat _addedStat = null;
        public StatterStat AddedStat { get { return _addedStat; } }

        public StatterAddStatForm()
        {
            InitializeComponent();
        }

        private void StatterAddStatForm_Load(object sender, EventArgs e)
        {
            SetColours();
        }

        private void SetColours()
        {
            Color fg = ActGlobals.oFormActMain.ActColorSettings.MainWindowColors.ForeColorSetting;
            Color bg = ActGlobals.oFormActMain.ActColorSettings.MainWindowColors.BackColorSetting;

            this.BackColor = bg;
            lblName.ForeColor = fg;
        }

        public void SetUsedStats(List<string> usedStatNames)
        {
            cmbStat.BeginUpdate();
            cmbStat.Items.Clear();
            foreach (string statName in StatterStat.GetAvailableStatNames(usedStatNames))
                cmbStat.Items.Add(statName);
            cmbStat.EndUpdate();

            if (cmbStat.Items.Count > 0)
                cmbStat.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbStat.SelectedItem == null) return;

            _addedStat = new StatterStat(cmbStat.SelectedItem.ToString());
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

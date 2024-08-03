using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ActStatter.Model;

namespace ActStatter.UI
{
    public partial class StatterPluginTab : UserControl
    {
        // Limited the number of tracked stats, hard limit from /do_file_commands 
        // (16 lines per macro, with the first being the marker)
        public const int MAX_SELECTABLE_STATS = 15;

        private StatterMain _statter = null;
        private StatterSettings _settings = null;
        private bool _loading = true;

        public event Action SelectedStatsChanged;
        protected void OnSelectedStatsChanged()
        {
            if (SelectedStatsChanged != null)
                SelectedStatsChanged();

            btnAdd.Enabled = _settings.Stats.Count < MAX_SELECTABLE_STATS;
        }

        private StatterAddStatForm _dlgAddStat = new StatterAddStatForm();

        // Keep a list of suggested colours to rotate through
        private Color[] _cStatColours =
        {
            Color.FromArgb(255, 104, 164, 98),
            Color.FromArgb(255, 242, 175, 88),
            Color.FromArgb(255, 53, 108, 154),
            Color.FromArgb(255, 209, 65, 63),
            Color.FromArgb(255, 134, 81, 137),
            Color.FromArgb(255, 143, 146, 145),
            Color.FromArgb(255, 76, 125, 168),
            Color.FromArgb(255, 121, 176, 114),
            Color.FromArgb(255, 244, 186, 110),
            Color.FromArgb(255, 219, 88, 87),
            Color.FromArgb(255, 105, 105, 105)
        };

        public StatterPluginTab(StatterMain statter, StatterSettings settings)
        {
            InitializeComponent();

            _statter = statter;
            _settings = settings;
        }

        private void StatterUI_Load(object sender, EventArgs e)
        {
            // Wrap init code to fox an issue for Ombrette
            try
            {
                chkParseOnImport.Checked = _settings.ParseOnImport;
                chkRestrictChannels.Checked = _settings.RestrictToChannels;
                txtRestrictChannels.Text = StatterSettings.GetStringFromList(_settings.RestrictedChannels);
                txtRestrictChannels.Enabled = chkRestrictChannels.Checked;
            }
            catch { }
            try
            {
                SetSelectedStats();
            }
            catch { }
            _loading = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dlgAddStat.SetUsedStats(GetUsedStatNames());
            _dlgAddStat.SetColour(_cStatColours[_settings.Stats.Count % _cStatColours.Length]);

            _dlgAddStat.Location = btnAdd.PointToScreen(btnAdd.Location);
            if (_dlgAddStat.ShowDialog(this) == DialogResult.OK)
            {
                StatterStat newStat = _dlgAddStat.AddedStat;
                _settings.Stats.Add(newStat);
                if (!_loading)
                {
                    _settings.Save(_statter);
                }

                AddSelectedStat(newStat);

                OnSelectedStatsChanged();
            }
        }

        private void statDetails_StatDeleted(StatterStatDetailsPanel statDetailPanel)
        {
            _settings.Stats.Remove(statDetailPanel.Stat);
            if (!_loading)
            {
                _settings.Save(_statter);
            }

            RemoveSelectedStat(statDetailPanel);

            OnSelectedStatsChanged();
        }

        private void statDetails_StatModified(StatterStatDetailsPanel statDetailPanel)
        {
            if (!_loading)
            {
                _settings.Save(_statter);
            }

            OnSelectedStatsChanged();
        }

        public void SetSelectedStats()
        {
            pnlStatDetails.Controls.Clear();

            foreach (StatterStat stat in _settings.Stats)
                AddSelectedStat(stat);

            btnAdd.Enabled = _settings.Stats.Count < MAX_SELECTABLE_STATS;
        }

        public void RefreshLogView()
        {
            txtLogs.Lines = _statter.GetLogs();
        }

        private void AddSelectedStat(StatterStat stat)
        {
            StatterStatDetailsPanel statDetails = new StatterStatDetailsPanel(stat);
            statDetails.StatDeleted += new Action<StatterStatDetailsPanel>(statDetails_StatDeleted);
            statDetails.StatModified += new Action<StatterStatDetailsPanel>(statDetails_StatModified);
            pnlStatDetails.Controls.Add(statDetails);
        }

        private void RemoveSelectedStat(StatterStatDetailsPanel statDetailPanel)
        {
            pnlStatDetails.Controls.Remove(statDetailPanel);
        }

        private List<string> GetUsedStatNames()
        {
            List<string> usedStatNames = new List<string>();
            foreach (StatterStat stat in _settings.Stats)
                usedStatNames.Add(stat.Name);
            return usedStatNames;
        }

        private void chkParseOnImport_CheckedChanged(object sender, EventArgs e)
        {
            _settings.ParseOnImport = chkParseOnImport.Checked;
            if (!_loading)
            {
                _settings.Save(_statter);
            }
        }

        private void chkRestrictChannel_CheckedChanged(object sender, EventArgs e)
        {
            _settings.RestrictToChannels = chkRestrictChannels.Checked;
            if (!_loading)
            {
                _settings.Save(_statter);
            }

            txtRestrictChannels.Enabled = chkRestrictChannels.Checked;
        }

        private void txtRestrictChannels_Leave(object sender, EventArgs e)
        {
            _settings.RestrictedChannels = StatterSettings.GetListFromString(txtRestrictChannels.Text);
            if (!_loading)
            {
                _settings.Save(_statter);
            }
            txtRestrictChannels.Text = StatterSettings.GetStringFromList(_settings.RestrictedChannels);
        }

        private void tabExtra_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabExtra.SelectedTab == tabLogs)
            {
                txtLogs.Lines = _statter.GetLogs();
            }
        }

        private void btnCopyLogs_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLogs.Text))
            {
                Clipboard.SetText(txtLogs.Text);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshLogView();
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            _statter.ClearLogs();
            txtLogs.Lines = null;
        }

        private void lnkOpenHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(StatterMain.HELP_PAGE);
        }
    }
}

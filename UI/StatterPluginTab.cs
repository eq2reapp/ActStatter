using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ACT_Plugin.Model;

namespace ACT_Plugin.UI
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
            ShowInstructions();

            chkParseOnImport.Checked = _settings.ParseOnImport;
            chkSteppedLines.Checked = _settings.StepLines;
            SetSelectedStats();

            btnTestGraph.Visible = StatterMain.DEBUG;

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
                    _settings.Save();
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
                _settings.Save();
            }

            RemoveSelectedStat(statDetailPanel);

            OnSelectedStatsChanged();
        }

        private void statDetails_StatModified(StatterStatDetailsPanel statDetailPanel)
        {
            if (!_loading)
            {
                _settings.Save();
            }

            OnSelectedStatsChanged();
        }

        protected void ShowInstructions()
        {
            txtInstructions.Rtf = string.Format(
@"{{\rtf1\ansi\f0\pard
Create a macro that calls {{\i /do_file_commands {0}}} and bind this to a hotkey you naturally use often during combat.\par
\par
To view your stats, right-click an encounter listed in the encounter tree (note that this can also include the zone-wide {{\i ""All""}} encounter) and select {{\i View Encounter Stats}}. Doing so will open a window showing the minimum and maximum recorded values for each stat during the selected encounter. Clicking on one or more stat rows will display a graph of each selected stat over the course of the encounter. Hovering over the graph will show instantaneous values and times.\par
\par
Using this macro will spam up your chat window (using the chat category ""Command""), so you may want to redirect the output to a chat window that does not contain any other useful info.\par
\par
Finally, note that /do_file_commands currently limits the number of stats that can be tracked to {1}.\par
}}", StatterMain.MACRO_FILENAME, MAX_SELECTABLE_STATS);
        }

        public void SetSelectedStats()
        {
            pnlStatDetails.Controls.Clear();

            foreach (StatterStat stat in _settings.Stats)
                AddSelectedStat(stat);

            btnAdd.Enabled = _settings.Stats.Count < MAX_SELECTABLE_STATS;
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

        private void btnTestGraph_Click(object sender, EventArgs e)
        {
            List<StatterStat> stats = new List<StatterStat>();
            stats.Add(new StatterStat("Fervor"));
            stats.Add(new StatterStat("Potency"));

            stats[0].Colour = Color.Red;
            stats[1].Colour = Color.Green;

            int durationSeconds = 10;
            DateTime end = DateTime.Now;
            DateTime start = end.AddSeconds(-durationSeconds);
            for (int i = 0; i < durationSeconds; i++)
            {
                stats[0].ParseReading((i * 1).ToString(), start.AddSeconds(i));
                stats[1].ParseReading((i * 2).ToString(), start.AddSeconds(i));
            }

            var dlgViewStats = new StatterViewStatsForm(_statter, _settings);
            dlgViewStats.ShowStats(stats, start, end, "Test");
        }

        private void chkParseOnImport_CheckedChanged(object sender, EventArgs e)
        {
            _settings.ParseOnImport = chkParseOnImport.Checked;
            if (!_loading)
            {
                _settings.Save();
            }
        }

        private void chkSteppedLines_CheckedChanged(object sender, EventArgs e)
        {
            _settings.StepLines = chkSteppedLines.Checked;
            if (!_loading)
            {
                _settings.Save();
            }
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
            txtLogs.Lines = _statter.GetLogs();
        }
    }
}

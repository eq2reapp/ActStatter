using ActStatter.Model;
using ActStatter.Util;
using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ActStatter.UI
{
    public partial class StatterViewStatsForm : Form
    {
        private const string OPTION_ENC_NONE = "None";
        private const string OPTION_ENC_DPS = "DPS";
        private const string OPTION_ENC_HPS = "HPS";

        private List<StatterEncounterStat> _stats = new List<StatterEncounterStat>();
        private List<StatterEncounterStat> _selectedStats = new List<StatterEncounterStat>();
        private List<string> _selectedPlayers = new List<string>();
        private bool _selectingStat = false;
        private DateTime _start = DateTime.MinValue;
        private DateTime _end = DateTime.MinValue;
        private string _title = "";
        private StatterMain _statter = null;
        private List<StatterStatReading> _readings = null;
        private EncounterData _encData = null;
        private StatterSettings _settings = new StatterSettings();
        private bool _loaded = false;

        public StatterViewStatsForm(StatterMain statter, StatterSettings settings)
        {
            _statter = statter;
            _settings = settings;

            InitializeComponent();

            // Settings may be null if a static object is used to measure the bounds in GetDefaultSize()
            if (_settings != null)
                statGraph.UseSettings(_settings);

            // Add an easy way to test
            btnCreateData.Visible = StatterMain.DEBUG;
        }

        public static Size GetDefaultSize()
        {
            return (new StatterViewStatsForm(null, null)).Size;
        }

        private void StatterViewStatsForm_Shown(object sender, EventArgs e)
        {
            this.SetBounds(_settings.PopupLastX, _settings.PopupLastY, _settings.PopupLastW, _settings.PopupLastH);
        }

        private void StatterViewStatsForm_SizeOrLocationChanged(object sender, EventArgs e)
        {
            // Allow the initial load to complete before saving the bounds, otherwise the initial
            // load will just save default values.
            if (_loaded)
            {
                _settings.PopupLastX = this.Bounds.X;
                _settings.PopupLastY = this.Bounds.Y;
                _settings.PopupLastW = this.Bounds.Width;
                _settings.PopupLastH = this.Bounds.Height;
                _settings.Save();
            }
        }

        private void StatterViewStatsForm_Load(object sender, EventArgs e)
        {
            // Load some initial values from settings
            chkShowAverage.Checked = _settings.GraphShowAverage;

            cmbShowValues.Items.Clear();
            cmbShowValues.Items.AddRange(new string[] {
                OPTION_ENC_NONE,
                OPTION_ENC_DPS,
                OPTION_ENC_HPS
            });
            if (_settings.GraphShowEncDps)
                cmbShowValues.SelectedItem = OPTION_ENC_DPS;
            else if (_settings.GraphShowEncHps)
                cmbShowValues.SelectedItem = OPTION_ENC_HPS;
            else
                cmbShowValues.SelectedItem = OPTION_ENC_NONE;

            sliderEncDpsResolution.Value =
                Math.Min(sliderEncDpsResolution.Maximum, 
                         Math.Max(sliderEncDpsResolution.Minimum, _settings.EncDpsResolution));

            // Load the dynamic help text
            StringBuilder sbNotes = new StringBuilder();
            if (_stats.Count < 1)
            {
                sbNotes.AppendLine("It seems that you have no stats recording. Please review the help page to learn how to setup stats.");
                sbNotes.AppendLine();
            }
            if (_statter != null && _statter.DarqUIDetected)
            {
                sbNotes.AppendLine("Overcapped stats are shown as thicker line segments in the graph. Only stats from DarqUI can report overcap status.");
                sbNotes.AppendLine();
            }
            sbNotes.AppendLine("The shaded area under the graph line shows the average value over the full duration (only if 1 player selected).");
            sbNotes.AppendLine();
            sbNotes.AppendLine("Click inside the graph to set a line marker for the stat value. Right-click to clear all markers.");
            sbNotes.AppendLine();
            sbNotes.AppendLine("Use the Period slider to alter the smoothness/peakiness of the Encounter line. ACT's default is 5 seconds.");
            lblNotes.Text = sbNotes.ToString();

            _loaded = true;

            UpdateGraph();
        }

        private void StatterViewStatsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void btnCreateData_Click(object sender, EventArgs e)
        {
            List<StatterStatReading> readings = new List<StatterStatReading>();
            Random r = new Random((int)DateTime.Now.Ticks);
            string[] players = { StatterStatReading.DEFAULT_PLAYER_NAME, "Yoda", "Luke", "Leia", "Han", "Chewy", "Bill", "Ted", "Optimus", "Reapp" };
            string[] stats = { "Crit_Bonus", "Fervor", "Potency", "AbilityDoubleAttack", "Resolve" };
            int[] statMin = { 30000, 1100, 1100000, 550, 9400 };
            int[] statMax = { 41000, 2100, 1400000, 650, 9400 };
            Array.ForEach(players, player =>
            {
                for (int j = 0; j < stats.Length; j++)
                {
                    var stat = stats[j];
                    var statObj = StatterStat.GetStatForKey(stat);
                    DateTime lastTime = _start;
                    while (lastTime < _end)
                    {
                        readings.Add(new StatterStatReading()
                        {
                            Player = player,
                            Source = StatterStatReading.StatSource.Darq,
                            Stat = statObj,
                            Overcap = r.Next(0, 9) >= 7,
                            Time = lastTime,
                            Value = statMin[j] + (r.NextDouble() * (statMax[j] - statMin[j]))
                        });
                        lastTime = lastTime.AddSeconds(r.Next(1, 10));
                    }
                }
            });
            ShowStats(readings, _encData);
        }

        private void lbPlayers_MouseUp(object sender, MouseEventArgs e)
        {
            List<string> selectedPlayers = new List<string>();
            for (int i = 0; i < lbPlayers.CheckedItems.Count; i++)
                selectedPlayers.Add(lbPlayers.CheckedItems[i].ToString());

            // Only update if the players have changed - we need this because there's
            // no event on the control itself to indicate this.
            if (string.Join(":", selectedPlayers) != string.Join(":", _selectedPlayers))
            {
                // Keep track of these players for next time the listbox has a click
                _selectedPlayers = selectedPlayers;

                ShowTableStatsForPlayers(selectedPlayers);
                ClearSelectedStats();
                UpdateGraph();
            }
        }

        private void lbPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Allow the lbPlayers_MouseUp event to do all the actual work
            lbPlayers.SelectedIndices.Clear();
        }

        private void dgStats_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            Color oddBg = Color.White;
            Color evenBg = Color.FromArgb(230, 230, 230);

            bool isOdd = true;
            for (int i = 0; i < dgStats.RowCount; i++)
            {
                if (!string.IsNullOrEmpty(dgStats.Rows[i].Cells[0].Value.ToString()))
                    isOdd = !isOdd;
                dgStats.Rows[i].DefaultCellStyle.BackColor = isOdd ? oddBg : evenBg;
            }
        }

        /*
         * When the user selects one or more stats in the datagrid, render a graph
         * for the selected stats.
         */
        private void dgStats_SelectionChanged(object sender, EventArgs e)
        {
            if (_selectingStat || dgStats.SelectedCells.Count < 1 || dgStats.Rows.Count < 1)
                return;

            // Since we're manually changing selections in this logic, ensure the
            // manual selections don't trigger other selection logic until we're done
            _selectingStat = true;

            int selectedRowIndex = dgStats.SelectedCells[0].RowIndex;
            var selectedRow = dgStats.Rows[selectedRowIndex];
            //if (selectedRow.Cells.Count < 1 || selectedRow.Cells[0].Value == null)
            //    return;
            string statName = selectedRow.Cells[0].Value.ToString();
            List<string> playerNames = new List<string>();

            // First unselect all rows
            for (int i = 0; i < dgStats.Rows.Count; i++)
                dgStats.Rows[i].Selected = false;

            // Select the triggering row
            selectedRow.Selected = true;
            playerNames.Add(selectedRow.Cells[1].Value.ToString());

            // Step through the rows after the current one, and select them as long
            // as they don't have a stat name (so they're the same as the triggered stat)
            for (int i = selectedRowIndex + 1; i < dgStats.Rows.Count; i++)
            {
                var curRow = dgStats.Rows[i];
                if (!string.IsNullOrEmpty(curRow.Cells[0].Value.ToString()))
                    break;
                curRow.Selected = true;
                playerNames.Add(curRow.Cells[1].Value.ToString());
            }

            // If the triggering row does not have a stat name, step through the rows before
            // the current one, and select them until we find one that has a stat name
            if (string.IsNullOrEmpty(statName))
                for (int i = selectedRowIndex - 1; i >= 0; i--)
                {
                    var curRow = dgStats.Rows[i];
                    curRow.Selected = true;
                    playerNames.Add(curRow.Cells[1].Value.ToString());
                    statName = curRow.Cells[0].Value.ToString();
                    if (!string.IsNullOrEmpty(statName))
                        break;
                }

            // Select all the stat/player combos for whose rows were selected
            _selectedStats.Clear();
            playerNames.ForEach(playerName => _selectedStats.Add(_stats.Find(
                stat => stat.Stat.Name.Equals(statName) && stat.Player.Equals(playerName))));

            _selectingStat = false;
            chkShowAverage.Enabled = _selectedStats.Count < 2;
            UpdateGraph();
        }

        private void chkShowAverage_CheckedChanged(object sender, EventArgs e)
        {
            _settings.GraphShowAverage = chkShowAverage.Checked;
            _settings.Save();

            UpdateGraph();
        }

        private void cmbShowValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settings.GraphShowEncDps = false;
            _settings.GraphShowEncHps = false;
            switch (cmbShowValues.SelectedItem)
            {
                case OPTION_ENC_NONE:
                    break;
                case OPTION_ENC_DPS:
                    _settings.GraphShowEncDps = true;
                    break;
                case OPTION_ENC_HPS:
                    _settings.GraphShowEncHps = true;
                    break;
            }
            _settings.Save();

            UpdateGraph();
        }

        private void sliderEncDpsResolution_Scroll(object sender, EventArgs e)
        {
            _settings.EncDpsResolution = sliderEncDpsResolution.Value;
            _settings.Save();

            UpdateGraph();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start(StatterMain.HELP_PAGE);
        }

        /*
         * Show the window with all the provided readings, and meta data. A list of
         * players will be created from the readings
         */
        public void ShowStats(List<StatterStatReading> readings, EncounterData encounterData)
        {
            _readings = readings;
            _encData = encounterData;
            _start = encounterData.StartTime;
            _end = encounterData.EndTime;
            _title = string.Format("{0} - {1}", encounterData.ZoneName, encounterData.Title);

            // Load the player drop-down
            List<string> players = new List<string>();
            foreach (var reading in readings)
                if (reading.Player != StatterStatReading.DEFAULT_PLAYER_NAME && !players.Contains(reading.Player))
                    players.Add(reading.Player);
            players.Sort();
            players.Insert(0, StatterStatReading.DEFAULT_PLAYER_NAME);

            lbPlayers.BeginUpdate();
            lbPlayers.Items.Clear();
            lbPlayers.Items.AddRange(Array.ConvertAll(players.ToArray(), item => item));
            lbPlayers.EndUpdate();
            lbPlayers.SetItemChecked(0, true);
            _selectedPlayers.Clear();
            _selectedPlayers.Add(StatterStatReading.DEFAULT_PLAYER_NAME);

            // Calculate then show min/max etc for each stat for the initial selected player
            ShowTableStatsForPlayers(_selectedPlayers);

            this.Show();
        }

        public void ShowTableStatsForPlayers(List<string> players)
        {
            // Group all the readings by their stat and player
            _stats.Clear();
            foreach (string player in players)
            {
                var playerReadings = _readings.FindAll(reading => player == reading.Player);
                if (_statter != null)
                    _statter.Log(string.Format("Showing {0} stat reading(s) for {1}", playerReadings.Count, player));

                foreach (StatterStatReading reading in playerReadings)
                {
                    // Compare by name so that multiple stat collecters can aggregate readings
                    var encPlayerStat = _stats.Find(x => x.Stat.Name == reading.Stat.Name && x.Player == player);
                    if (encPlayerStat == null)
                    {
                        encPlayerStat = new StatterEncounterStat()
                        {
                            Stat = reading.Stat,
                            Player = player,
                            Readings = new List<StatterStatReading>(),
                            AvgReading = new StatterStatReading(),
                            PercentOvercap = new StatterStatReading()
                        };
                        _stats.Add(encPlayerStat);
                    }

                    encPlayerStat.Readings.Add(reading);
                }
            }

            // Iterate over the stats and do some high-level calculations and filtering
            double encounterDurationSeconds = (_end - _start).TotalSeconds;
            foreach (StatterEncounterStat encStat in _stats)
            {
                if (encStat.Readings.Count > 0)
                {
                    // Filter out any readings that occur more than once a second (take the latest)
                    // since that will make the graph dippy.
                    List<StatterStatReading> filteredReadings = new List<StatterStatReading>();
                    for (int i = 0; i < encStat.Readings.Count; i++)
                        if (i == encStat.Readings.Count - 1 ||
                            encStat.Readings[i].Time != encStat.Readings[i + 1].Time)
                        {
                            filteredReadings.Add(encStat.Readings[i]);
                        }
                    encStat.Readings = filteredReadings;

                    StatterStatReading prevReading = null;
                    StatterStatReading curReading;
                    double curDuration;
                    double statSum = 0.0;
                    double ocTime = 0.0;
                    for (int i = 0; i < encStat.Readings.Count; i++)
                    {
                        curReading = encStat.Readings[i];
                        if (i == 0)
                        {
                            curDuration = curReading.Time.Subtract(_start).TotalSeconds;
                            statSum += curDuration * curReading.Value;
                        }
                        else
                        {
                            curDuration = curReading.Time.Subtract(prevReading.Time).TotalSeconds;
                            statSum += curDuration * prevReading.Value;
                        }

                        if (encStat.MinReading == null || curReading.Value < encStat.MinReading.Value)
                            encStat.MinReading = curReading;

                        if (encStat.MaxReading == null || curReading.Value > encStat.MaxReading.Value)
                            encStat.MaxReading = curReading;

                        if (curReading.Overcap)
                            ocTime += curDuration;

                        prevReading = curReading;
                    }
                    // Add the last reading until the encounter end
                    curDuration = _end.Subtract(prevReading.Time).TotalSeconds;
                    statSum += curDuration * prevReading.Value;
                    if (prevReading.Overcap)
                        ocTime += curDuration;

                    encStat.AvgReading.Value = (statSum / encounterDurationSeconds);
                    encStat.PercentOvercap.Value = (ocTime / encounterDurationSeconds) * 100;
                }
            }

            ShowStatsTable();
        }

        private void ShowStatsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Stat", typeof(string));
            dt.Columns.Add("Player", typeof(string));
            dt.Columns.Add("Min", typeof(string));
            dt.Columns.Add("Max", typeof(string));
            dt.Columns.Add("Avg", typeof(string));
            dt.Columns.Add("OC", typeof(string));

            if ((_end - _start).TotalSeconds > 1)
            {
                // Sort by stat, then average
                _stats.Sort((x, y) => {
                    int cmp = x.Stat.Name.CompareTo(y.Stat.Name);
                    if (cmp == 0)
                    {
                        cmp = -x.AvgReading.Value.CompareTo(y.AvgReading.Value);
                    }
                    return cmp;
                });

                // Keep track of the stat name so we only show it for the first value
                string lastStat = "";
                foreach (StatterEncounterStat stat in _stats)
                {
                    List<object> values = new List<object>() {
                        stat.Stat.Name.Equals(lastStat) ? "" : stat.Stat.Name,
                        stat.Player,
                        stat.MinReading == null ? "" : Formatters.GetReadableNumber(stat.MinReading.Value),
                        stat.MaxReading == null ? "" : Formatters.GetReadableNumber(stat.MaxReading.Value),
                        stat.AvgReading == null ? "" : Formatters.GetReadableNumber(stat.AvgReading.Value),
                        stat.PercentOvercap == null ? "" : stat.PercentOvercap.Value.ToString("0") + "%"
                    };
                    DataRow dr = dt.Rows.Add(values.ToArray());

                    lastStat = stat.Stat.Name;
                }

                Text = string.Format("Stats for {0:h':'mm':'ss tt} - {1:h':'mm':'ss tt} ({2})", _start, _end, _title);
                dgStats.Columns["OC"].Visible = _statter != null && _statter.DarqUIDetected;
                dgStats.DataSource = dt;
            }
        }

        private void UpdateGraph()
        {
            statGraph.DrawStats(_selectedStats, _start, _end, _encData);
            pnlExtraControls.Enabled = _selectedStats.Count > 0;
        }

        private void ClearSelectedStats()
        {
            _selectingStat = true;
            for (int i = 0; i < dgStats.Rows.Count; i++)
                dgStats.Rows[i].Selected = false;
            _selectingStat = false;

            _selectedStats.Clear();
        }
    }
}

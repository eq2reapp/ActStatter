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

        protected List<StatterEncounterStat> _stats = new List<StatterEncounterStat>();
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
        }

        public static Size GetDefaultSize()
        {
            return (new StatterViewStatsForm(null, null)).Size;
        }

        private void ViewStats_Load(object sender, EventArgs e)
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
            if (_statter.DarqUIDetected)
            {
                sbNotes.AppendLine("Overcapped stats are shown as thicker line segments in the graph. Only stats from DarqUI can report overcap status.");
                sbNotes.AppendLine();
            }
            sbNotes.AppendLine("The shaded area under the graph line represents the average value over the full duration (if enabled).");
            sbNotes.AppendLine();
            sbNotes.AppendLine("Click inside the graph to set a line marker for the stat value. Right-click to clear all markers.");
            sbNotes.AppendLine();
            sbNotes.AppendLine("Use the Period slider to alter the smoothness/peakiness of the Encounter line. ACT's default is 5 seconds.");
            lblNotes.Text = sbNotes.ToString();

            if (!_statter.DarqUIDetected)
                dgStats.Columns.Remove("OC");

            _loaded = true;

            UpdateGraph();
        }

        // When the user selects one or more stats in the datagrid, render a graph
        // for the selected stats
        private void dgStats_SelectionChanged(object sender, EventArgs e)
        {
            UpdateGraph();
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

        private void UpdateGraph()
        {
            List<StatterEncounterStat> selectedStats = new List<StatterEncounterStat>();

            foreach (DataGridViewRow dgvr in dgStats.SelectedRows)
            {
                DataRowView drv = dgvr.DataBoundItem as DataRowView;
                if (drv != null && drv.Row != null && drv.Row.ItemArray.Length > 0)
                {
                    string statName = drv.Row.ItemArray[0].ToString();
                    StatterEncounterStat encStat = _stats.Find((es) => { return es.Stat.Name.Equals(statName); });
                    if (encStat != null)
                        selectedStats.Add(encStat);
                }
            }

            statGraph.DrawStats(selectedStats, _start, _end, _encData);
        }

        public void ShowStats(List<StatterStatReading> readings, EncounterData encounterData)
        {
            _readings = readings;
            _encData = encounterData;
            _start = encounterData.StartTime;
            _end = encounterData.EndTime;
            _title = string.Format("{0} - {1}", encounterData.ZoneName, encounterData.Title);

            // Load the player drop-down
            List<string> players = new List<string>();
            players.Add(StatterStatReading.DEFAULT_PLAYER_NAME);
            foreach (var reading in readings)
                if (reading.Player != StatterStatReading.DEFAULT_PLAYER_NAME && !players.Contains(reading.Player))
                    players.Add(reading.Player);
            cmbPlayer.BeginUpdate();
            cmbPlayer.Items.AddRange(Array.ConvertAll(players.ToArray(), item => item));
            cmbPlayer.EndUpdate();
            cmbPlayer.SelectedIndex = 0;
        }

        public void ShowStatsForPlayer(string player)
        {
            var readings = _readings.FindAll(reading => reading.Player.Equals(player));
            _statter.Log(string.Format("Showing {0} stat reading(s) for {1}", readings.Count, player));

            // Group all the readings by their stat
            _stats.Clear();
            foreach (StatterStatReading reading in readings)
            {
                // Compare by name so that multiple stat collecters can aggregate readings
                var encStat = _stats.Find(x => x.Stat.Name == reading.Stat.Name);
                if (encStat == null)
                {
                    encStat = new StatterEncounterStat()
                    {
                        Stat = reading.Stat,
                        Readings = new List<StatterStatReading>(),
                        AvgReading = new StatterStatReading(),
                        PercentOvercap = new StatterStatReading()
                    };
                    _stats.Add(encStat);
                }

                encStat.Readings.Add(reading);
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
            dt.Columns.Add("Min", typeof(string));
            dt.Columns.Add("Max", typeof(string));
            dt.Columns.Add("Avg", typeof(string));
            if (_statter.DarqUIDetected)
                dt.Columns.Add("OC", typeof(string));

            if ((_end - _start).TotalSeconds > 1)
            {
                _stats.Sort((x, y) => x.Stat.Name.CompareTo(y.Stat.Name));
                foreach (StatterEncounterStat stat in _stats)
                {
                    List<object> values = new List<object>() {
                        stat.Stat.Name,
                        stat.MinReading == null ? "" : Formatters.GetReadableNumber(stat.MinReading.Value),
                        stat.MaxReading == null ? "" : Formatters.GetReadableNumber(stat.MaxReading.Value),
                        stat.AvgReading == null ? "" : Formatters.GetReadableNumber(stat.AvgReading.Value),
                    };
                    if (_statter.DarqUIDetected)
                        values.Add(stat.PercentOvercap == null ? "" : stat.PercentOvercap.Value.ToString("0") + "%");
                    DataRow dr = dt.Rows.Add(values.ToArray());
                }

                Text = string.Format("Stats for {0:h':'mm':'ss tt} - {1:h':'mm':'ss tt} ({2})", _start, _end, _title);
                dgStats.DataSource = dt;
            }

            this.Show();
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

        private void chkShowAverage_CheckedChanged(object sender, EventArgs e)
        {
            _settings.GraphShowAverage = chkShowAverage.Checked;
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

        private void cmbPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            string player = cmbPlayer.SelectedItem.ToString();
            ShowStatsForPlayer(player);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using ActStatter.Model;

namespace ActStatter.UI
{
    public partial class StatterViewStatsForm : Form
    {
        protected List<StatterEncounterStat> _stats = new List<StatterEncounterStat>();
        private DateTime _start = DateTime.MinValue;
        private DateTime _end = DateTime.MinValue;
        private string _title = "";
        private StatterMain _statter = null;
        private StatterSettings _settings = null;

        public StatterViewStatsForm(StatterMain statter, StatterSettings settings)
        {
            InitializeComponent();

            _statter = statter;
            _settings = settings;
            statGraph.ShowSteppedStatLines = _settings.StepLines;
        }

        private void ViewStats_Load(object sender, EventArgs e)
        {
        }

        // When the user selects one or more stats in the datagrid, render a graph
        // for the selected stats
        private void dgStats_SelectionChanged(object sender, EventArgs e)
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

            if (selectedStats.Count > 0)
                statGraph.DrawStats(selectedStats, _start, _end);
        }

        public void ShowStats(List<StatterStatReading> readings, EncounterData encounterData)
        {
            _start = encounterData.StartTime;
            _end = encounterData.EndTime;
            _title = string.Format("{0} - {1}", encounterData.ZoneName, encounterData.Title);

            _statter.Log(string.Format("Showing {0} stat reading(s)", readings.Count));

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
                        AvgReading = new StatterStatReading()
                    };
                    _stats.Add(encStat);
                }

                encStat.Readings.Add(reading);
            }

            // Iterate over the stats and do some high-level calculations.
            foreach (StatterEncounterStat encStat in _stats)
            {
                double statSum = 0;
                foreach (StatterStatReading reading in encStat.Readings)
                {
                    if (encStat.MinReading == null || reading.Value < encStat.MinReading.Value)
                        encStat.MinReading = reading;

                    if (encStat.MaxReading == null || reading.Value > encStat.MaxReading.Value)
                        encStat.MaxReading = reading;

                    statSum += reading.Value;
                }
                if (encStat.Readings.Count > 0)
                {
                    encStat.AvgReading.Value = (statSum / encStat.Readings.Count);
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

            if ((_end - _start).TotalSeconds > 1)
            {
                foreach (StatterEncounterStat stat in _stats)
                {
                    DataRow dr = dt.Rows.Add(new object[] { 
                        stat.Stat.Name, 
                        stat.MinReading == null ? "" : stat.MinReading.Value.ToString("0"), 
                        stat.MaxReading == null ? "" : stat.MaxReading.Value.ToString("0"),
                        stat.AvgReading == null ? "" : stat.AvgReading.Value.ToString("0"),
                    });
                }

                Text = string.Format("Stats for {0:h':'mm':'ss tt} - {1:h':'mm':'ss tt} ({2})", _start, _end, _title);
                dgStats.DataSource = dt;
            }

            this.Show();
        }
    }
}

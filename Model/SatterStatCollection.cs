using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ActStatter.Model
{
    public class StatterStatCollection
    {
        private int _statGroupIndex = 0;
        private List<StatterStatReading> _readings = new List<StatterStatReading>();
        private StatterSettings _settings = new StatterSettings();

        public StatterStatCollection(StatterSettings settings)
        {
            _settings = settings;
        }

        public void StartStatGroup()
        {
            _statGroupIndex = 0;
        }

        public List<StatterStatReading> GetReadings(DateTime start, DateTime end)
        {
            var readings = _readings.Where(x => x.Time >= start && x.Time <= end).ToList();

            // For each stat, if that stat has readings from Darq, only include those. This
            // is so that we can reliably show ocercap info.
            Dictionary<string, bool> statsFromDarq = new Dictionary<string, bool>();
            foreach (var reading in readings)
            {
                if (!statsFromDarq.ContainsKey(reading.Stat.Key))
                    statsFromDarq.Add(reading.Stat.Key, false);

                if (reading.Source == StatterStatReading.StatSource.Darq)
                    statsFromDarq[reading.Stat.Key] = true;
            }

            List<StatterStatReading> filteredRadings = new List<StatterStatReading>();
            for (int i = 0; i < readings.Count; i++)
            {
                var reading = readings[i];
                if (!statsFromDarq[reading.Stat.Key] || reading.Source == StatterStatReading.StatSource.Darq)
                    filteredRadings.Add(reading);
            }

            return filteredRadings;
        }

        // Add a reading in the current stats group and return true if there's space for more.
        public bool AddStatGroupReading(string logLine, DateTime detectedTime)
        {
            List<StatterStat> stats = _settings.Stats;
            var stat = stats[_statGroupIndex];
            _readings.Add(ParseStatGroupReading(stat, logLine, detectedTime));

            _statGroupIndex++;
            return _statGroupIndex < stats.Count;
        }

        // Add a reading from DarqUI's StatMon logging.
        public void AddDarqReading(string logLine, DateTime detectedTime, string player)
        {
            try
            {
                var reading = ParseDarqReading(logLine, detectedTime, player);
                _readings.Add(reading);
            }
            catch { }
        }

        private StatterStatReading ParseStatGroupReading(StatterStat stat, string logReading, DateTime logTime)
        {
            EnsureLocaleForDoubleParsing();

            StatterStatReading reading = new StatterStatReading(StatterStatReading.StatSource.Native)
            {
                Stat = stat,
                Time = logTime
            };

            double temp;
            string[] parts;

            switch (stat.Key)
            {
                case "HealthRange":
                case "PowerRange":
                    parts = logReading.Split(new string[] { " ", "-", "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && double.TryParse(parts[1], out temp))
                        reading.Value = temp;
                    break;
                default:
                    string cleaned = logReading.Replace("%", "");
                    if (double.TryParse(cleaned, out temp))
                        reading.Value = temp;
                    break;
            }

            return reading;
        }

        private StatterStatReading ParseDarqReading(string logReading, DateTime logTime, string player)
        {
            EnsureLocaleForDoubleParsing();

            double temp;
            // Format is like: DarqUI_StatMon:Fervor:1,344.1%:OC:#68A462
            string[] parts = logReading.Split(new string[] { ":" }, StringSplitOptions.None);
            StatterStatReading reading = null;
            if (parts.Length >= 3)
            {
                var stat = StatterStat.GetStatForKey(parts[1]);
                reading = new StatterStatReading(StatterStatReading.StatSource.Darq)
                {
                    Stat = stat,
                    Time = logTime,
                    Player = player
                };

                string cleaned = parts[2].Replace("%", "").Replace(",", "");
                if (double.TryParse(cleaned, out temp))
                    reading.Value = temp;

                if (parts.Length >= 4)
                    reading.Overcap = parts[3] == "OC";

                if (parts.Length >= 5)
                    stat.Colour = ColorTranslator.FromHtml(parts[4]);

                if (stat.Key == "CurrentHealth")
                {
                    Int64 restored = Convert.ToInt64(temp);
                    restored &= 0xFFFFFFFF;
                    reading.Value = Convert.ToDouble(restored);
                    reading.Overcap = false; // Override this since it doesn't make sense to be OC
                }
            }

            return reading;
        }

        private void EnsureLocaleForDoubleParsing()
        {
            // Thanks Doxiah for the following fix for languages that use ',' and '.' differently
            // thanks english in decimal numbers:
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }
    }
}

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
            return _readings.Where(x => x.Time >= start && x.Time <= end).ToList();
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
        public void AddDarqReading(string logLine, DateTime detectedTime)
        {
            try
            {
                _readings.Add(ParseDarqReading(logLine, detectedTime));
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

            double temp = 0;
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

        private StatterStatReading ParseDarqReading(string logReading, DateTime logTime)
        {
            EnsureLocaleForDoubleParsing();

            double temp = 0;
            // Format is like: DarqUI_StatMon:Fervor:1,344.1%:OC:#68A462
            string[] parts = logReading.Split(new string[] { ":" }, StringSplitOptions.None);
            StatterStatReading reading = null;
            if (parts.Length >= 3)
            {
                var stat = StatterStat.GetStatForKey(parts[1]);
                reading = new StatterStatReading(StatterStatReading.StatSource.Darq)
                {
                    Stat = stat,
                    Time = logTime
                };

                string cleaned = parts[2].Replace("%", "");
                if (double.TryParse(cleaned, out temp))
                    reading.Value = temp;

                if (parts.Length >= 4)
                    reading.Overcap = parts[3] == "OC";

                if (parts.Length >= 5)
                    stat.Colour = ColorTranslator.FromHtml(parts[4]);
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

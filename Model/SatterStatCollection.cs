using System;
using System.Collections.Generic;
using System.Linq;
using Advanced_Combat_Tracker;

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

        public void Clear()
        {
            _readings.Clear();
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
        public void AddDarqReading(string statName, string statVal, string statOc, DateTime detectedTime, string playerKey)
        {
            try
            {
                var reading = ParseDarqReading(statName, statVal, statOc, detectedTime, playerKey);
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
                Time = logTime,
                Player = ActGlobals.charName,
                FirstPerson = true
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

        private StatterStatReading ParseDarqReading(string statName, string statVal, string statOc, DateTime logTime, string playerKey)
        {
            EnsureLocaleForDoubleParsing();

            string cleanedVal = statVal.Replace("%", "").Replace(",", "");
            double parsedVal = 0;
            double.TryParse(cleanedVal, out parsedVal);

            var stat = StatterStat.GetStatForKey(statName);
            if (stat.Key == "CurrentHealth")
            {
                Int64 restored = Convert.ToInt64(parsedVal);
                restored &= 0xFFFFFFFF;
                parsedVal = Convert.ToDouble(restored);
            }

            bool isFirstPerson = StatterStatReading.IsFirstPersonKey(playerKey);
            StatterStatReading reading = new StatterStatReading(StatterStatReading.StatSource.Darq)
            {
                Stat = stat,
                Time = logTime,
                Player = isFirstPerson ? ActGlobals.charName : playerKey,
                Value = parsedVal,
                Overcap = statOc == "OC",
                FirstPerson = isFirstPerson
            };
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

using System;
using System.Collections.Generic;
using System.Drawing;

namespace ACT_Plugin.Model
{
    public class StatterStat
    {
        public static readonly Color DEFAULT_COLOUR = Color.Black;

        private string _name = "";
        private string _clientAttribute = "";
        private List<StatterStatReading> _readings = new List<StatterStatReading>();

        public string Name { get { return _name; } }
        public string ClientAttribute { get { return _clientAttribute; } }
        public Color Colour { get; set; }
        public List<StatterStatReading> Readings { get { return _readings; } }

        // Trackable stats are pulled from <EQ2_Dir>\UI\Default\eq2ui_gamedata.xml
        // Search for: <DataSource description="Stats" Name="Stats">
        // TODO: Present an option to slurp this in?
        private static Dictionary<string, string> ClientAttributeLookupTable = new Dictionary<string, string>()
        {
            { "Stamina", "Stamina" },
            { "Wisdom", "Wisdom" },
            { "Agility", "Agility" },
            { "Strength", "Strength" },
            { "Intelligence", "Intelligence" },
            { "Max Health", "HealthRange" },
            { "Max Mana", "PowerRange" },
            { "Crit Chance", "Crit_Chance" },
            { "Crit Bonus", "Crit_Bonus" },
            { "Potency", "Potency" },
            { "Ability Mod", "Ability_Mod" },
            { "Fervor", "Fervor" },
            { "Ability Double Cast", "AbilityDoubleAttack" },
            { "DPS", "DPS" },
            { "Haste", "Haste" },
            { "Multi Attack", "Double_Atk_Percent" },
            { "Flurry", "Flurry" },
            { "Flurry Multiplier", "FlurryMult" },
            { "AE Auto", "AE_AutoAtk_Percent" },
            { "Weapon Damage Bonus", "Weapon_Damage_Bonus" },
            { "Mitigation", "Defense_Mitigation" },
            { "Mitigation %", "Defense_MitigationPercent" },
            { "Avoidance", "Defense_Avoidance" },
            { "Avoidance %", "Defense_AvoidanceBase" },
            { "Physical Damage Reduction %", "Damage_Reduction_Percentage_Physical" },
            { "Arcane Damage Reduction %", "Damage_Reduction_Percentage_Arcane" },
            { "Elemental Damage Reduction %", "Damage_Reduction_Percentage_Elemental" },
            { "Noxious Damage Reduction %", "Damage_Reduction_Percentage_Noxious" },
        };

        public StatterStat(string name)
        {
            if (!ClientAttributeLookupTable.ContainsKey(name))
                throw new Exception("Unknown stat: " + name);

            _name = name;
            _clientAttribute = ClientAttributeLookupTable[_name];

            Colour = DEFAULT_COLOUR;
        }

        public override bool Equals(object obj)
        {
            StatterStat other = obj as StatterStat;
            if (other == null) return false;

            return other.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        // Return a list of stat names we know how to track
        public static List<string> GetKnownStats()
        {
            List<string> knownStats = new List<string>();

            foreach (string key in ClientAttributeLookupTable.Keys)
                knownStats.Add(key);

            return knownStats;
        }

        // Return a list of stat names we know how to track, minus the ones specified
        public static List<string> GetAvailableStats(List<string> usedStats)
        {
            List<string> availableStats = new List<string>();

            foreach (string stat in GetKnownStats())
                if (!usedStats.Contains(stat))
                    availableStats.Add(stat);

            return availableStats;
        }

        // Extract a single value from the logline, and record it
        public void ParseReading(string reading, DateTime time)
        {
            double temp = 0;
            string[] parts;

            // Thanks Doxiah for the following fix for languages that use ',' and '.' differently
            // thanks english in decimal numbers:
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            switch (_clientAttribute)
            {
                case "HealthRange":
                case "PowerRange":
                    parts = reading.Split(new string[] { " ", "-", "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && double.TryParse(parts[1], out temp))
                        AddReading(temp, time);
                    break;
                default:
                    string cleaned = reading.Replace("%", "");
                    if (double.TryParse(cleaned, out temp))
                        AddReading(temp, time);
                    break;
            }
        }

        public void ClearReadings()
        {
            _readings.Clear();
        }

        // Get the reading at or immediately before the time specified
        public StatterStatReading GetReading(DateTime time)
        {
            return _readings.FindLast(x => { return x.Time <= time; });
        }

        // Get the largest reading during time specified
        public StatterStatReading GetMaxReading(DateTime start, DateTime end)
        {
            StatterStatReading maxReading = null;

            foreach (StatterStatReading reading in _readings)
                if (reading.Time >= start && reading.Time <= end && (maxReading == null || reading.Value > maxReading.Value))
                    maxReading = reading;

            return maxReading;
        }

        // Get the smallest reading during time specified
        public StatterStatReading GetMinReading(DateTime start, DateTime end)
        {
            StatterStatReading minReading = null;

            foreach (StatterStatReading reading in _readings)
                if (reading.Time >= start && reading.Time <= end && (minReading == null || reading.Value < minReading.Value))
                    minReading = reading;

            return minReading;
        }

        public List<StatterStatReading> GetReadings(DateTime start, DateTime end)
        {
            return _readings.FindAll(x => { return x.Time >= start && x.Time <= end; });
        }

        private void AddReading(double value, DateTime time)
        {
            StatterStatReading reading = new StatterStatReading() { Value = value, Time = time };
            _readings.Add(reading);
        }
    }
}

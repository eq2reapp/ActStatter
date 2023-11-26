using System;

namespace ActStatter.Model
{
    public class StatterStatReading
    {
        public const string DEFAULT_PLAYER_KEY = "You";

        public enum StatSource
        {
            Native,
            Darq
        }
        public StatSource Source { get; set; }
        public StatterStat Stat { get; set; }
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public bool Overcap { get; set; }
        public string Player { get; set; }
        public bool FirstPerson { get; set; }

        public StatterStatReading() : this(StatSource.Native) { }

        public StatterStatReading(StatSource source)
        {
            Time = DateTime.Now;
            Overcap = false;
            Source = source;
            Player = DEFAULT_PLAYER_KEY;
            FirstPerson = true;
        }

        public static bool IsFirstPersonKey(string playerKey)
        {
            return playerKey.Equals(DEFAULT_PLAYER_KEY);
        }

        public static bool IsFirstPersonLabel(string playerLabel)
        {
            return playerLabel.StartsWith($"{DEFAULT_PLAYER_KEY} ");
        }

        public string GetPlayerKey()
        {
            return FirstPerson ? DEFAULT_PLAYER_KEY : Player;
        }

        public string GetPlayerLabel()
        {
            return FirstPerson ? $"{DEFAULT_PLAYER_KEY} ({Player})" : Player;
        }
    }
}

﻿using System;

namespace ActStatter.Model
{
    public class StatterStatReading
    {
        public const string DEFAULT_PLAYER_NAME = "You";

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

        public StatterStatReading() : this(StatSource.Native) { }

        public StatterStatReading(StatSource source)
        {
            Time = DateTime.Now;
            Overcap = false;
            Source = source;
            Player = DEFAULT_PLAYER_NAME;
        }
    }
}

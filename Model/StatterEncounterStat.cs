using System.Collections.Generic;

namespace ActStatter.Model
{
    public class StatterEncounterStat
    {
        public StatterStat Stat { get; set; }
        public string PlayerKey { get; set; }
        public List<StatterStatReading> Readings { get; set; }
        public StatterStatReading MinReading { get; set; }
        public StatterStatReading MaxReading { get; set; }
        public StatterStatReading AvgReading { get; set; }
        public StatterStatReading PercentOvercap { get; set; }
    }
}

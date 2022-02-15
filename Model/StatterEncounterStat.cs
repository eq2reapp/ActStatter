using System.Collections.Generic;

namespace ACT_Plugin.Model
{
    public class StatterEncounterStat
    {
        public StatterStat Stat { get; set; }
        public List<StatterStatReading> Readings { get; set; }
        public StatterStatReading MinReading { get; set; }
        public StatterStatReading MaxReading { get; set; }
    }
}

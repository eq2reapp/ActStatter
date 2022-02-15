using System;

namespace ACT_Plugin.Model
{
    public class StatterStatReading
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public StatterStatReading()
        {
            Time = DateTime.Now;
        }
    }
}

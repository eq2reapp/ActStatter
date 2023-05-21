using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActStatter.Util
{
    public class Formatters
    {
        public static string GetReadableNumber(double number)
        {
            if (number > 1000000000000)
                return (number / 1000000000000.0).ToString("0.00") + "T";
            else if (number > 1000000000)
                return (number / 1000000000.0).ToString("0.00") + "B";
            else if (number > 1000000)
                return (number / 1000000.0).ToString("0.00") + "M";
            else if (number > 1000000)
                return (number / 1000000.0).ToString("0.00") + "M";
            return number.ToString("0");
        }
    }
}

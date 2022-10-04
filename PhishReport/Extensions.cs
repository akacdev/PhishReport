using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishReport
{
    public static class Extensions
    {
        private static readonly DateTime Unix = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            return Unix.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}

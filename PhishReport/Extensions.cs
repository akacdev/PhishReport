using System;
using System.Net;

namespace PhishReport
{
    internal static class Extensions
    {
        public static DateTime ToDate(this long value)
            => DateTime.UnixEpoch.AddSeconds(value);

        public static long ToUnixSeconds(this DateTime value)
            => (long)(value - DateTime.UnixEpoch).TotalSeconds;

        public static string HtmlDecode(this string value) => WebUtility.HtmlDecode(value);
    }
}
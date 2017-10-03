using System;
using System.Linq;

namespace WebProject.Utils
{
    public static class StringExtensions
    {
        public static int? ParseIntOrDefault(this string s)
        {
            return int.TryParse(s, out int result) ? result : (int?)null;
        }
        public static bool? ParseBoolOrDefault(this string s)
        {
            return bool.TryParse(s, out bool result) ? result : (bool?)null;
        }
    }
}

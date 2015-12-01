using System;
using System.Linq;

namespace WebProject.Utils
{
    public static class StringExtensions
    {
        public static int? ParseIntOrDefault(this string s)
        {
            int result;
            if (int.TryParse(s, out result))
                return result;
            return null;
        }
        public static bool? ParseBoolOrDefault(this string s)
        {
            bool result;
            if (bool.TryParse(s, out result))
                return result;
            return null;
        }
    }
}

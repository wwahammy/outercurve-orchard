using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.Projects
{
    public static class StringExtensions
    {
        public static string SafeSubstring(this string str, int startIndex) {
            if (str.Length <= startIndex) {
                return "";
            }
            return str.Substring(startIndex);
        }

        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (str.Length <= startIndex)
            {
                return "";
            }
            if (str.Length <= startIndex + length) {
                return str.Substring(startIndex);
            }

            return str.Substring(startIndex, length);


        }
    }
}
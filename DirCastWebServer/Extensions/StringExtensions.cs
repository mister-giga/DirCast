using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirCastWebServer
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FinalProject.Helpers
{
    public class TextHelper
    {
        public static string RemoveHTML(string raw)
        {
            string cleaned = Regex.Replace(raw, "<.*?>", string.Empty);

            return cleaned;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FinalProject.Models;
using System.Net;

namespace FinalProject.Helpers
{
    public class TextHelper
    {
        public static string RemoveHTML(string raw)
        {
            string cleaned = Regex.Replace(raw, "<.*?>", string.Empty);

            return cleaned;
        }

        public static bool CompareJobUrl(string dbUrl, string resultUrl)
        {
            string parsedDb = dbUrl.Substring(0, dbUrl.IndexOf('?') + 1);
            string parsedResult = resultUrl.Substring(0, dbUrl.IndexOf('?') + 1);

            return parsedDb.Equals(parsedResult);
        }

        public static string GetWhatParameter(List<Skill> skills)
        {
            string what = "";

            foreach(Skill s in skills)
            {
                what = String.Join(' ', what, s.Skill1);
            }

            return what;
        }

        public static string GetEncodedWhat(List<Skill> skills)
        {
            string encodedWhat = "";
            string encodedCharacter;

            foreach(Skill s in skills)
            {
                encodedCharacter = WebUtility.UrlEncode(s.Skill1);
                encodedWhat = String.Join(' ', encodedWhat, encodedCharacter);
            }

            return encodedWhat;
        }
    }
}

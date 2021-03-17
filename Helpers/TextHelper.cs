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
        //Our API will sometimes return data with embedded HTML tags, this method is called to remove them
        public static string RemoveHTML(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "";
            }
            string cleaned = Regex.Replace(raw, "<.*?>", string.Empty);

            return cleaned;
        }

        //The redirect urls returned by our API will sometimes be different even if they redirect to the same job,
        //so this method compares the part of the URL that is unique for each job
        public static bool CompareJobUrl(string dbUrl, string resultUrl)
        {
            string parsedDb = dbUrl.Substring(0, dbUrl.IndexOf('?') + 1);
            string parsedResult = resultUrl.Substring(0, dbUrl.IndexOf('?') + 1);

            return parsedDb.Equals(parsedResult);
        }

        //public static string GetWhatParameter(List<Skill> skills)
        //{
        //    string what = "";

        //    foreach(Skill s in skills)
        //    {
        //        what = String.Join(' ', what, s.Skill1);
        //    }

        //    return what;
        //}

        //Takes in a list of user skills, then encodes them and joins them into one string to plug into our API
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class JobDAL
    {
        // Request data from our API URL and return json response (string)
        // Using parameters:
        //  what / what_or - find jobs based on key words delimited by space characters
        //  where - find jobs based in different regions / states / cities (dependant on country parameter)
        //  page - api sends results by pages that we will need to crawl through
        //  country - find jobs based in different countries
        public string GetData(string country, int page = 1, string what = null, string where = null)
        {
            string url = $"https://api.adzuna.com/v1/api/jobs/{country}/search/{page}?what_or={what}&where={where}&app_id={Secret.ApiId}&app_key={Secret.ApiKey}&content-type=application/json&";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        // Convert given json reponse (string) and deserialize into a Rootobject object
        public Rootobject SearchJobs(string country, int page, string what, string where)
        {
            string trimmedWhat = what.Trim().Trim('+');
            string json = GetData(country, page, trimmedWhat, where);
            Rootobject j = JsonConvert.DeserializeObject<Rootobject>(json);
            return j;

        }
    }
}

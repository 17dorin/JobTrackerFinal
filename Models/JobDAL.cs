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
        public string GetData(string country, int page = 1)
        {
            string url = $"https://api.adzuna.com/v1/api/jobs/{country}/search/1?app_id=0e272246&app_key=b3f98fd2abc65138aec301152403b956&content-type=application/json&";


            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();
            return json;
        }

        public Rootobject SearchJobs(string country, int page)
        {
            string json = GetData(country, page);
            Rootobject j = JsonConvert.DeserializeObject<Rootobject>(json);
            return j;

        }
    }
}

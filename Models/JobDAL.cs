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
        public string GetData(string country, int page)
        {
            string url = $"https://api.adzuna.com/v1/api/jobs/{country}/search/{page}";


            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();
            return json;
        }

        public List<Job> SearchJobs(string country, int page)
        {
            string json = GetData(country, page);
            List<Job> j = JsonConvert.DeserializeObject<List<Job>>(json);
            return j;

        }
    }
}

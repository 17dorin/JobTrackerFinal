using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;

namespace FinalProject.Controllers
{
    public class JobController : Controller
    {
        private JobDAL jd = new JobDAL();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search(string country, int page)
        {
            string countryLower = country.Trim().ToLower();
            Rootobject r = jd.SearchJobs(countryLower, page);
            return View(r.results.ToList());
        }
    }
}

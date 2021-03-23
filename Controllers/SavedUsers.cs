using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class SavedUsers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

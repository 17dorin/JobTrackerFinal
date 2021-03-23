using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly FinalProjectContext _context;
        public HomeController(FinalProjectContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(a.IsEmployer == null)
            {
                return RedirectToAction("EmployerCheck", "Users");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

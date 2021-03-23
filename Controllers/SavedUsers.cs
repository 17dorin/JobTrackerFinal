using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class SavedUsers : Controller
    {
        private readonly FinalProjectContext _context;

        public SavedUsers(FinalProjectContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_context.SavedUsers.Where(x => x.Employer == User.FindFirst(ClaimTypes.NameIdentifier).Value).Distinct().ToList());
        }
    }
}

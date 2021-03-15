using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly FinalProjectContext _context;
        public UsersController(FinalProjectContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Skills()
        {
            List<Skill> skills = _context.Skills.ToList();
            skills.OrderBy(x => x);
            return View(skills);
        }
    }
}

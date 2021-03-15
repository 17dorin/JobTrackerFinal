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
    [Authorize]
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
            return View(skills);
        }

        public IActionResult AddSkills(List<int> skillId)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            if (skillId.Count != 0)
            {
                foreach(int i in skillId)
                {
                    UserSkill s = new UserSkill();
                    s.UserId = userId;
                    s.SkillId = i;
                    _context.UserSkills.Add(s);
                }
            }
            _context.SaveChanges();
            return View();
        }
    }
}

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

        [Authorize]
        public IActionResult Skills()
        {
            List<UserSkill> uSkill = _context.UserSkills
                .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            List<int> skillId = new List<int>();
            foreach(UserSkill u in uSkill)
            {
                skillId.Add((int)u.SkillId);
            }

            if (skillId.Count != 0)
            {
                TempData["OwnedSkills"] = skillId;
            }

            List<Skill> skills = _context.Skills.ToList();
            return View(skills);
        }

        [Authorize]
        public IActionResult AddSkills(List<int> skillId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (skillId.Count != 0)
            {
                var check = _context.UserSkills.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
                foreach(int i in skillId)
                {
                    if (check.Where(x => x.SkillId == i).ToList().Count <= 0)
                    {
                        UserSkill s = new UserSkill();
                        s.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        s.SkillId = i;
                        _context.UserSkills.Add(s);
                    }
                }

            }

            var check2 = _context.UserSkills.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            foreach(UserSkill u in check2)
            {
                if (skillId.IndexOf((int)u.SkillId) == -1)
                {
                    _context.UserSkills.Remove(u);
                }
            }

            _context.SaveChanges();
            return View();
        }
    }
}

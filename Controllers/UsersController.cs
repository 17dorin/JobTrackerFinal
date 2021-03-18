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


        public IActionResult UserProfile()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<int?> userSkills = _context.UserSkills.Where(x => x.UserId == userId).Select(x => x.SkillId).ToList();

            List<Skill> skills = _context.Skills.ToList();

            skills = skills.Where(x => userSkills.Contains(x.Id)).ToList();

            //TempData["skills"] = skills;

            ProfileViewModel pvm = new ProfileViewModel(_context.AspNetUsers.Where(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList()[0], skills);

            return View(pvm);

        }


        public IActionResult EditUserProfile()
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View(a);
        }
        [HttpPost]
        public IActionResult EditUserProfile(AspNetUser a)
        {
            if (ModelState.IsValid)
            {
                _context.AspNetUsers.Update(a);
                _context.SaveChanges();
            }
            return RedirectToAction("UserProfile");
        }

        // Returns View : Routes a List of Skills into the list
        public IActionResult Skills()
        {
            // Obtain all skills current user already has/checked from UserSkills table
            List<UserSkill> existingSkills = _context.UserSkills
                .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();

            // Add all current user's skills onto a new int list
            List<int> existingSkillIds = new List<int>();
            foreach(UserSkill skill in existingSkills)
            {
                existingSkillIds.Add((int)skill.SkillId);
            }

            // If user does not have any existing skills (newly registered users) then
            // insert into a TempData
            if (existingSkillIds.Count != 0)
            {
                TempData["ExistingSkills"] = existingSkillIds;
            }

            // List of skills for checklist labels and values of View
            List<Skill> skills = _context.Skills.ToList();
            return View(skills);
        }

        // Returns View : confirms that you saved your skills to the current user
        // Action: Saves checked off skills into the UserSkills table
        // Routed Data : list of integers representing skill ids from Skills table
        public IActionResult AddSkills(List<int> skillId)
        {
            // Check if skill ids aren't empty
            if (skillId.Count != 0)
            {
                // Get all existing skills current user already has
                var check = _context.UserSkills.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
                foreach(int i in skillId)
                {
                    // Avoid adding duplicates by checking if check skill ids exists within the current user
                    if (check.Where(x => x.SkillId == i).ToList().Count <= 0)
                    {
                        UserSkill s = new UserSkill();
                        s.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        s.SkillId = i;
                        _context.UserSkills.Add(s);
                    }
                }

            }

            // Check if skill is unchecked, if it is unchecked but skill is in UserSkills table, remove it
            var check2 = _context.UserSkills.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            foreach(UserSkill u in check2)
            {
                // If you cannot find skill in UserSkills table, remove it
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

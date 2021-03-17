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

        public IActionResult SearchUsers()
        {
            List<Skill> skills = _context.Skills.ToList();

            return View(skills);
        }
        [HttpPost]
        public IActionResult SearchUsers(List<int> skillId)
        {
            List<string> matchingUserIds = _context.UserSkills.Where(x => skillId.Contains((int)x.SkillId)).Select(x => x.UserId).Distinct().ToList();

            List<AspNetUser> matchingUsers = _context.AspNetUsers.Where(x => matchingUserIds.Contains(x.Id)).ToList();

            List<ProfileViewModel> profileResults = new List<ProfileViewModel>();

            foreach(AspNetUser u in matchingUsers)
            {
                List<int?> userSkills = _context.UserSkills.Where(x => x.UserId == u.Id).Select(x => x.SkillId).ToList();

                List<Skill> skills = _context.Skills.Where(x => userSkills.Contains(x.Id)).ToList();

                ProfileViewModel p = new ProfileViewModel(u, skills);

                profileResults.Add(p);
            }
            return View("SearchUsersResults", profileResults);



        }
        //public IActionResult SearchUsersResults(List<ProfileViewModel> profileResults)
        //{
        //    return View(profileResults);
        //}


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

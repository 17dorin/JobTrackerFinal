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

        //Returns a view containing all skills in our DB to search by
        public IActionResult SearchUsers()
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            List<Skill> skills = _context.Skills.ToList();

            return View(skills);


        }
        [HttpPost]
        //Displays a list of all users that match the skills searched by
        public IActionResult SearchUsers(List<int> skillId, string searchedName)
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                         //Gets all unique userIds that are paired with any of the passed skillIds in our UserSkills table
            List<string> matchingUserIds = _context.UserSkills.Where(x => skillId.Contains((int)x.SkillId)).Select(x => x.UserId).Distinct().ToList();
            //Gets all matching users based off of userIds
            List<AspNetUser> matchingUsers = _context.AspNetUsers.Where(x => matchingUserIds.Contains(x.Id) && (x.IsPrivate == false)).ToList();

            List<ProfileViewModel> profileResults = new List<ProfileViewModel>();
            //Makes a View Model for each AspNetUser that matched
            foreach (AspNetUser u in matchingUsers)
            {
                //Gets the IDs of the skills the user has
                List<int?> userSkills = _context.UserSkills.Where(x => x.UserId == u.Id).Select(x => x.SkillId).ToList();
                //Gets a list of skills based off of those IDs
                List<Skill> skills = _context.Skills.Where(x => userSkills.Contains(x.Id)).ToList();
                //Constructs model, consisting of the list of skills and non-sensitive information from our AspNetUsers table
                ProfileViewModel p = new ProfileViewModel(u, skills);
                //Add to results list
                profileResults.Add(p);
            }
            return View("SearchUsersResults", profileResults);
        }

        public IActionResult SearchUsersName()
        {

            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return View("SearchUsersName");
        }
        [HttpPost]
        public IActionResult SearchUsersName(string searchedName)
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);


                List<AspNetUser> MatchingUserName = _context.AspNetUsers.Where(x => x.UserName.Contains((string)searchedName)).ToList();
                List<AspNetUser> matchingUsers = MatchingUserName.Where(x => x.IsPrivate == false ).ToList();

                List<ProfileViewModel> profileResults = new List<ProfileViewModel>();
                foreach (AspNetUser u in matchingUsers)
                {
                    ProfileViewModel p = new ProfileViewModel(u);
                    profileResults.Add(p);
                }
                return View("SearchUsersResults", profileResults);
            

        }

        //Displays information about the user
        public IActionResult UserProfile()
        {
            //Gets currently logged in user ID
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //Gets the IDs of the skills the user has
            List<int?> userSkills = _context.UserSkills.Where(x => x.UserId == userId).Select(x => x.SkillId).ToList();
            //Gets a list of skills entries based off of the userSkill IDs
            List<Skill> skills = _context.Skills.Where(x => userSkills.Contains(x.Id)).ToList();
            //Makes a view model, consisting of the list of skills and the currently logged in AspNetUser
            ProfileViewModel pvm = new ProfileViewModel(_context.AspNetUsers.Where(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList()[0], skills);
            return View(pvm);
        }

        //"Overload" of user profile action, which our user search results view redirects to
        public IActionResult UserProfileResult(string userId)
        {
            //Gets the userSkills for the userId passed from the view
            List<int?> userSkills = _context.UserSkills.Where(x => x.UserId == userId).Select(x => x.SkillId).ToList();
            //Gets a list of Skills based off of those skill IDs
            List<Skill> skills = _context.Skills.Where(x => userSkills.Contains(x.Id)).ToList();
            //Makes a view model consisting of the list of skills and the AspNetUser matching the passed ID
            ProfileViewModel p = new ProfileViewModel(_context.AspNetUsers.Where(x => x.Id == userId).ToList()[0], skills);

            return View("UserProfile", p);
        }


        public IActionResult EditUserProfile()
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View(a);
        }
        [HttpPost]
        public IActionResult EditUserProfile(AspNetUser a, bool IsPrivate)
        {

            if (ModelState.IsValid)
            {

                if(IsPrivate == false)
                {
                    a.IsPrivate = false;
                }
                else
                {
                    a.IsPrivate = true;
                }

                _context.AspNetUsers.Update(a);
                _context.SaveChanges();
            }
                return RedirectToAction("UserProfile");

        }

        public IActionResult Skills()
        {
            AspNetUser a = _context.AspNetUsers.Find(User.FindFirst(ClaimTypes.NameIdentifier).Value);



                // Obtain all skills current user already has/checked from UserSkills table
                List<UserSkill> existingSkills = _context.UserSkills
                    .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    .ToList();

                // Add all current user's skills onto a new int list
                List<int> existingSkillIds = new List<int>();
                foreach (UserSkill skill in existingSkills)
                {
                    existingSkillIds.Add((int)skill.SkillId);
                }

                List<Skill> skills = _context.Skills.Where(x => x.Vote >= 5 || existingSkillIds.Contains(x.Id)).ToList();

                // If user does not have any existing skills (newly registered users) then
                // insert into a TempData
                if (existingSkillIds.Count != 0)
                {
                    TempData["ExistingSkills"] = existingSkillIds;
                }

                // List of skills for checklist labels and values of View
                skills = _context.Skills.Where(x => x.Vote >= 5).ToList();
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
                    Skill s = _context.Skills.Where(x => x.Id == u.SkillId).ToList()[0];

                    if (s.Vote < 5 && s.Vote > 0)
                    {
                        s.Vote--;
                        if(s.Vote == 0)
                        {
                            _context.Remove(s);
                        }
                        _context.SaveChanges();
                    }
                }
            }


            _context.SaveChanges();
            return View();
        }

        public IActionResult VoteSkill(string SkillToAdd)
        {
            if(_context.Skills.Where(x => x.Skill1.ToLower() == SkillToAdd.ToLower()).ToList().Count < 1)
            {
                Skill toAdd = new Skill();
                toAdd.Skill1 = SkillToAdd;
                toAdd.Vote = 1;
                _context.Skills.Add(toAdd);
                _context.SaveChanges();

                UserSkill u = new UserSkill();
                u.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                u.SkillId = _context.Skills.Where(x => x.Skill1.ToLower() == SkillToAdd.ToLower()).Select(x => x.Id).ToList()[0];

                _context.UserSkills.Add(u);
                _context.SaveChanges();
            }
            else
            {
                var toVote = _context.Skills.Where(x => x.Skill1.ToLower() == SkillToAdd.ToLower()).ToList();
                foreach(Skill s in toVote)
                {
                    if(!_context.UserSkills.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList().Select(x => x.SkillId).Contains(s.Id))
                    {
                        s.Vote += 1;
                        UserSkill u = new UserSkill();
                        u.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        u.SkillId = _context.Skills.Where(x => x.Skill1.ToLower() == SkillToAdd.ToLower()).Select(x => x.Id).ToList()[0];

                        _context.UserSkills.Add(u);
                        _context.SaveChanges();
                    }
                }
                _context.SaveChanges();
            }
            return View("AddSkills");
        }
    }
}

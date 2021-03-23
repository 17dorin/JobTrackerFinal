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
    public class SavedUsersController : Controller
    {
        private readonly FinalProjectContext _context;

        public SavedUsersController(FinalProjectContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            List<string> savedUsers = _context.SavedUsers.Where(x => x.Employer == User.FindFirst(ClaimTypes.NameIdentifier).Value).Distinct().Select(x => x.JobSeeker).ToList();
            List<AspNetUser> users = _context.AspNetUsers.Where(x => savedUsers.Contains(x.Id)).ToList();
            List<ProfileViewModel> usersToDisplay = new List<ProfileViewModel>();

            foreach(AspNetUser u in users)
            {
                ProfileViewModel p = new ProfileViewModel(u);
                usersToDisplay.Add(p);
            }

            return View(usersToDisplay);
        }

        [Authorize]
        public IActionResult SaveUser(string userId)
        {
            List<SavedUser> currentlySaved = _context.SavedUsers.Where(x => x.Employer == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();

            if(!currentlySaved.Select(x => x.JobSeeker).Contains(userId))
            {
                SavedUser u = new SavedUser();
                u.JobSeeker = userId;
                u.Employer = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                _context.SavedUsers.Add(u);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                TempData["UserError"] = "User already exists in your list";

                //List<AspNetUser> profile = _context.AspNetUsers.Where(x => x.Id == userId).ToList();

                //ProfileViewModel p = new ProfileViewModel(profile[0]);

                return RedirectToAction("UserProfileResult", "Users", new {userId = userId});
            }


        }
    }
}

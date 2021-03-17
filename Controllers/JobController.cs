using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FinalProject.Helpers;
using System.Text;
using System.Net;

namespace FinalProject.Controllers
{
    public class JobController : Controller
    {
        private JobDAL jd = new JobDAL();
        private readonly FinalProjectContext _context;

        public JobController(FinalProjectContext context)
        {
            _context = context;
        }

        // Index view displays a list of added jobs for the logged in user based on entity UserId
        // READ CRUD
        [Authorize]
        public IActionResult Index()
        {
            return View(_context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList());
        }

        public IActionResult Search(string country, string what, string where, int page = 1)
        {
            //Encodes any special characters in the search string, then gets data from the API and puts it in a results list
            string encodedWhat = WebUtility.UrlEncode(what);
            Rootobject r = jd.SearchJobs(country.ToLower(), page, encodedWhat, where);
            List<Result> jobResults = r.results.ToList();

            //If user is logged in, hide results the user already has saved in their tracker
            if(User.Identity.IsAuthenticated)
            {
                //Gets the Link property of all jobs the user has saved, remove any null values
                List<string> dbJobLinks = _context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(x => x.Link).ToList();
                dbJobLinks = dbJobLinks.Where(x => x != null).ToList();

                List<Result> duplicates = new List<Result>();

                //Adds job results from API to a duplicates list if their link matches one in the DB
                foreach (Result result in jobResults)
                {
                    if (dbJobLinks.Any(x => TextHelper.CompareJobUrl(x, result.redirect_url)))
                    {
                        duplicates.Add(result);
                    }
                }

                //Removes duplicates from results to display
                foreach (Result rd in duplicates)
                {
                    jobResults.Remove(rd);
                }
            }


            //Stores query parameters in TempData so user can page through results without having to search again
            TempData["country"] = country;
            TempData["page"] = page;
            TempData["what"] = what;
            TempData["where"] = where;

            return View(jobResults);
        }

        [Authorize]
        public IActionResult SearchRecommended()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult SearchRecommended(string country, string where, int page = 1)
        {
            //Gets the skill related to the logged in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<int?> userSkills = _context.UserSkills.Where(x => x.UserId == userId).Select(x => x.SkillId).ToList();

            //Gets all skills in our DB, then parses them down to a list of skills the user has
            List<Skill> skills = _context.Skills.ToList();
            skills = skills.Where(x => userSkills.Contains(x.Id)).ToList();

            //Method from a helper class, converts all skills into one url encoded string
            string what = TextHelper.GetEncodedWhat(skills).Trim();
            
            //Get job results
            Rootobject r = jd.SearchJobs(country.ToLower(), page, what, where);
            List<Result> jobResults = r.results.ToList();

            //If user is logged in, remove already saved jobs from search results
            if (User.Identity.IsAuthenticated)
            {
                //Get the Link property of every job the user has saved, remove null values
                List<string> dbJobLinks = _context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(x => x.Link).ToList();
                dbJobLinks = dbJobLinks.Where(x => x != null).ToList();

                List<Result> duplicates = new List<Result>();

                //Adds duplicate job results to a list
                foreach (Result result in jobResults)
                {
                    if (dbJobLinks.Any(x => TextHelper.CompareJobUrl(x, result.redirect_url)))
                    {
                        duplicates.Add(result);

                    }
                }

                //Removes duplicates in results to display
                foreach (Result rd in duplicates)
                {
                    jobResults.Remove(rd);
                }
            }


            //Saves query parameters in TempData for paging purposes
            TempData["country"] = country;
            TempData["page"] = page;
            TempData["what"] = what;
            TempData["where"] = where;

            return View("Search", jobResults);
        }

        [Authorize]
        //This is the Action that clicking the "Add to favorites" link redirects to. Gets the result ID from the view as route data
        public async Task<IActionResult> AddFromSearch(string id)
        {
            //Gets query parameters from TempData
            string country = TempData["country"].ToString();
            int page = int.Parse(TempData["page"].ToString());
            string what = TempData["what"].ToString();
            string where = TempData["where"].ToString();

            //Encodes any special characters, gets results from API
            string encodedWhat = WebUtility.UrlEncode(what);

            Rootobject r = jd.SearchJobs(country, page, encodedWhat, where);
            List<Result> jobResults = r.results.ToList();

            //Gets the job result matching the id passed back to the controller
            List<Result> toSave = jobResults.Where(x => x.id.Contains(id)).ToList();
            Job saved = Job.ToJob(toSave[0]);
            saved.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Adds job to DB and saves
            _context.Jobs.Add(saved);
            _context.SaveChanges();

            //After saving, removes duplicate results from the list to display
            if (User.Identity.IsAuthenticated)
            {
                //Get link property of all the users saved jobs
                List<string> dbJobLinks = _context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(x => x.Link).ToList();
                dbJobLinks = dbJobLinks.Where(x => x != null).ToList();

                List<Result> duplicates = new List<Result>();

                //Adds duplicate results into a list
                foreach (Result result in jobResults)
                {
                    if (dbJobLinks.Any(x => TextHelper.CompareJobUrl(x, result.redirect_url)))
                    {
                        duplicates.Add(result);

                    }
                }

                //Removes duplicates from main list to display
                foreach (Result rd in duplicates)
                {
                    jobResults.Remove(rd);
                }
            }

            //Stores query parameters in TempData for paging
            TempData["country"] = country;
            TempData["page"] = page;
            TempData["what"] = what;
            TempData["where"] = where;

            return View("Search", jobResults);
        }

        public IActionResult Add()
        {
            return View();
        }

        // Create/Add Job CRUD
        [HttpPost]
        public async Task<IActionResult> Add([Bind("Id,Company,Position,Contact,Method,DateOfApplication,Link,FollowUp,CompanySite,Responded,Notes,UserId")] Job job)
        {
            if (ModelState.IsValid)
            {
                job.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _context.Add(job);
                await _context.SaveChangesAsync();
                TempData["action"] = "add";
                return RedirectToAction("Success", job);
            }
            //return RedirectToAction("Failure", job);
            return View(job);
        }

        // Remove/DELETE CRUD
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            TempData["action"] = "delete";
            return RedirectToAction("Success", job);
        }

        // Edit/Update CRUD
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(Id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [Bind("Id,Company,Position,Contact,Method,DateOfApplication,Link,FollowUp,CompanySite,Responded,Notes,UserId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Jobs.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["action"] = "update";
                return RedirectToAction("Success", job);
            }
            return View(job);
        }

        public IActionResult Success(Job job)
        {
            return View(job);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }

    }
}

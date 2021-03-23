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
            var listViewModel = new ListViewModel();
            listViewModel.AllJobs = _context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            listViewModel.NeedsResponse = listViewModel.AllJobs.Where(x => x.Responded == false).ToList();
            listViewModel.PastFollowUp = listViewModel.NeedsResponse.Where(x => x.FollowUp <= DateTime.Now.AddDays(-1)).ToList();

            return View(listViewModel);
        }

        public IActionResult Search(string country, string what, string where, int page = 1)
        {
            //Encodes any special characters in the search string, then gets data from the API and puts it in a results list
            string encodedWhat;
            if (what.Contains('%'))
            {
                encodedWhat = what;
            }
            else
            {
                encodedWhat = WebUtility.UrlEncode(what);
            }

            string encodedWhere;
            if (where.Contains('%'))
            {
                encodedWhere = where;
            }
            else
            {
                encodedWhere = WebUtility.UrlEncode(where);
            }
            Rootobject r = jd.SearchJobs(country.ToLower(), page, encodedWhat, encodedWhere);
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
            string encodedWhere = WebUtility.UrlEncode(where);
            Rootobject r = jd.SearchJobs(country.ToLower(), page, what, encodedWhere);
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
        public IActionResult AddFromSearch(string id)
        {
            //Gets query parameters from TempData
            string country = TempData["country"].ToString();
            int page = int.Parse(TempData["page"].ToString());
            string what = TempData["what"].ToString();
            string where = TempData["where"].ToString();

            Rootobject r = jd.SearchJobs(country, page, what, where);
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

        // Simple view to display form asking user for job information
        public IActionResult Add()
        {
            return View();
        }

        // Create/Add Job CRUD
        [HttpPost]
        public IActionResult Add([Bind("Id,Company,Position,Contact,Method,DateOfApplication,Link,FollowUp,CompanySite,Responded,Notes,UserId")] Job job)
        {
            if (ModelState.IsValid)
            {
                // Ensuring job.UserId is set to the UserId of logged in user
                job.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                job.FollowUp = DateTime.Now.AddDays(2);

                // Add job and save changes in database
                _context.Add(job);
                _context.SaveChanges();

                // Storing action name in TempData to customize Success page
                TempData["action"] = "add";
                return RedirectToAction("Success", job);
            }
            return View(job);
        }

        // Renders delete view where job info is displayed to user and they're asked to confirm deletion
        public IActionResult Delete(int? id)
        {
            // Validation returning a 404 if no id passed to controller
            if (id == null)
            {
                return NotFound();
            }

            // Created job based on output from database matching id passed to controller
            Job job = _context.Jobs
                .FirstOrDefault(m => m.Id == id);

            // Returns 404 if no output from database
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // Remove/DELETE CRUD
        // Delete confirmation view posts to this action
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Create job based on id passed into controller
            Job job = _context.Jobs.Find(id);

            // Remove job from database and saves changes
            _context.Jobs.Remove(job);
            _context.SaveChanges();

            // Stores action name in tempdata to customize success vew
            TempData["action"] = "delete";
            return RedirectToAction("Success", job);
        }

        // Update view renders a form that displays job data based on id passed to controller where users can change details
        public IActionResult Update(int? id)
        {

            // Validation returning a 404 if no id passed to controller
            if (id == null)
            {
                return NotFound();
            }

            // Creates job based on passed in id
            Job job = _context.Jobs.Find(id);

            // If job is empty after pulling from db return 404
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // Edit/Update CRUD
        [HttpPost]
        public IActionResult Update(int id, [Bind("Id,Company,Position,Contact,Method,DateOfApplication,Link,FollowUp,CompanySite,Responded,Notes,UserId")] Job job)
        {
            // Validation ensuring the passed in id matches job's Id
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update database with job values that were passed in
                    _context.Jobs.Update(job);
                    _context.SaveChanges();
                }

                // If the above SaveChanges returns no rows updated we'll throw an exception
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

                // Storing action name within tempdata to customize the success page
                TempData["action"] = "update";
                return RedirectToAction("Success", job);
            }
            return View(job);
        }

        // All CRUD actions pass to this view to show a successful change
        public IActionResult Success(Job job)
        {
            return View(job);
        }

        // Method to check if a job exists which is used in earlier methods
        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }

    }
}

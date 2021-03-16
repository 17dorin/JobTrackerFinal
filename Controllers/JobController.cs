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
            Rootobject r = jd.SearchJobs(country.ToLower(), page, what, where);
            List<Result> jobResults = r.results.ToList();

            if(User.Identity.IsAuthenticated)
            {
                List<string> dbJobLinks = _context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(x => x.Link).ToList();
                dbJobLinks = dbJobLinks.Where(x => x != null).ToList();

                List<Result> duplicates = new List<Result>();

                foreach (Result result in jobResults)
                {
                    if (dbJobLinks.Any(x => TextHelper.CompareJobUrl(x, result.redirect_url)))
                    {
                        duplicates.Add(result);
                        
                    }
                }

                foreach (Result rd in duplicates)
                {
                    jobResults.Remove(rd);
                }
            }



            TempData["country"] = country;
            TempData["page"] = page;
            TempData["what"] = what;
            TempData["where"] = where;

            return View(jobResults);
        }

        [Authorize]
        public async Task<IActionResult> AddFromSearch(string id)
        {
            string country = TempData["country"].ToString();
            int page = int.Parse(TempData["page"].ToString());
            string what = TempData["what"].ToString();
            string where = TempData["where"].ToString();

            Rootobject r = jd.SearchJobs(country, page, what, where);
            List<Result> jobResults = r.results.ToList();

            List<Result> toSave = jobResults.Where(x => x.id.Contains(id)).ToList();
            Job saved = Job.ToJob(toSave[0]);
            saved.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            _context.Jobs.Add(saved);
            _context.SaveChanges();

            if (User.Identity.IsAuthenticated)
            {
                List<string> dbJobLinks = _context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(x => x.Link).ToList();
                dbJobLinks = dbJobLinks.Where(x => x != null).ToList();

                List<Result> duplicates = new List<Result>();

                foreach (Result result in jobResults)
                {
                    if (dbJobLinks.Any(x => TextHelper.CompareJobUrl(x, result.redirect_url)))
                    {
                        duplicates.Add(result);

                    }
                }

                foreach (Result rd in duplicates)
                {
                    jobResults.Remove(rd);
                }
            }

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

        // Create/Add CRUD
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

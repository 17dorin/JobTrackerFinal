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

namespace FinalProject.Controllers
{ [Authorize]
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
        public IActionResult Index()
        {
           return View(_context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList());
        }

        public IActionResult Search(string country, int page)
        {

            Rootobject r = jd.SearchJobs(country.ToLower(), page);
            return View(r.results.ToList());
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

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        // View CRUD action
        public IActionResult Index()
        {
           return View(_context.Jobs.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList());
        }

        public IActionResult Search(string country, int page)
        {

            Rootobject r = jd.SearchJobs(country.ToLower(), page);
            return View(r.results.ToList());
        }

        // Add CRUD action
        public IActionResult Add(Job job)
        {
            job.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.Jobs.Add(job);
            _context.SaveChangesAsync();
            return RedirectToAction("JobAdded", job);
        }
        
        public IActionResult Delete(int id)
        {
            Job job = _context.Jobs.Find(id);
            return View(job);
        }

        // Delete CRUD action
        public IActionResult Delete(Job job)
        {
            if (job == null)
            {
                return NotFound();
            }
            _context.Jobs.Remove(job);
            _context.SaveChanges();
            return RedirectToAction("DeleteSuccess", job);
        }

        public IActionResult DeleteSuccess(Job job)
        {
            return View(job);
        }

        //// Edit CRUD action
        //public IActionResult Edit(int id)
        //{
        //    Job job = _context.Jobs.Find(id);
        //    return View(job);
        //}

        //[HttpPost]
        //public IActionResult Edit(Job job)
        //{
        //    if (job == null)
        //    {
        //        return NotFound();
        //    }
        //    _context.Jobs.Update(job);
        //    _context.SaveChanges();
        //    return View(job);
        //}

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Company,Position,Contact,Method,DateOfApplication,Link,FollowUp,CompanySite,Responded,Notes,UserId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
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
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }

    }
}

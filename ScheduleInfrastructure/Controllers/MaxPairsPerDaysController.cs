using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleDomain.Model;
using ScheduleInfrastructure;

namespace ScheduleInfrastructure.Controllers
{
    public class MaxPairsPerDaysController : Controller
    {
        private readonly UniversityPracticeContext _context;

        public MaxPairsPerDaysController(UniversityPracticeContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: MaxPairsPerDays
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaxPairsPerDays.ToListAsync());
        }

        // GET: MaxPairsPerDays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maxPairsPerDay = await _context.MaxPairsPerDays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maxPairsPerDay == null)
            {
                return NotFound();
            }

            return View(maxPairsPerDay);
        }

        // GET: MaxPairsPerDays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaxPairsPerDays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaxPairs")] MaxPairsPerDay maxPairsPerDay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maxPairsPerDay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maxPairsPerDay);
        }

        // GET: MaxPairsPerDays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maxPairsPerDay = await _context.MaxPairsPerDays.FindAsync(id);
            if (maxPairsPerDay == null)
            {
                return NotFound();
            }
            return View(maxPairsPerDay);
        }

        // POST: MaxPairsPerDays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaxPairs")] MaxPairsPerDay maxPairsPerDay)
        {
            if (id != maxPairsPerDay.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maxPairsPerDay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaxPairsPerDayExists(maxPairsPerDay.Id))
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
            return View(maxPairsPerDay);
        }

        // GET: MaxPairsPerDays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maxPairsPerDay = await _context.MaxPairsPerDays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maxPairsPerDay == null)
            {
                return NotFound();
            }

            return View(maxPairsPerDay);
        }

        // POST: MaxPairsPerDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maxPairsPerDay = await _context.MaxPairsPerDays.FindAsync(id);
            if (maxPairsPerDay != null)
            {
                _context.MaxPairsPerDays.Remove(maxPairsPerDay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaxPairsPerDayExists(int id)
        {
            return _context.MaxPairsPerDays.Any(e => e.Id == id);
        }
    }
}

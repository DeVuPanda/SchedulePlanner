using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleDomain.Model;
using ScheduleInfrastructure;

namespace ScheduleInfrastructure.Controllers
{
    public class DaysOfWeeksController : Controller
    {
        private readonly UniversityPracticeContext _context;

        public DaysOfWeeksController(UniversityPracticeContext context)
        {
            _context = context;
        }

        // GET: DaysOfWeeks
        public async Task<IActionResult> Index()
        {
            return View(await _context.DaysOfWeeks.ToListAsync());
        }

        // GET: DaysOfWeeks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daysOfWeek = await _context.DaysOfWeeks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (daysOfWeek == null)
            {
                return NotFound();
            }

            return View(daysOfWeek);
        }

        // GET: DaysOfWeeks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DaysOfWeeks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DayName")] DaysOfWeek daysOfWeek)
        {
            if (ModelState.IsValid)
            {
                _context.Add(daysOfWeek);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(daysOfWeek);
        }

        // GET: DaysOfWeeks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daysOfWeek = await _context.DaysOfWeeks.FindAsync(id);
            if (daysOfWeek == null)
            {
                return NotFound();
            }
            return View(daysOfWeek);
        }

        // POST: DaysOfWeeks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DayName")] DaysOfWeek daysOfWeek)
        {
            if (id != daysOfWeek.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(daysOfWeek);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DaysOfWeekExists(daysOfWeek.Id))
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
            return View(daysOfWeek);
        }

        // GET: DaysOfWeeks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daysOfWeek = await _context.DaysOfWeeks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (daysOfWeek == null)
            {
                return NotFound();
            }

            return View(daysOfWeek);
        }

        // POST: DaysOfWeeks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var daysOfWeek = await _context.DaysOfWeeks.FindAsync(id);
            if (daysOfWeek != null)
            {
                _context.DaysOfWeeks.Remove(daysOfWeek);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DaysOfWeekExists(int id)
        {
            return _context.DaysOfWeeks.Any(e => e.Id == id);
        }
    }
}

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
    public class PairNumbersController : Controller
    {
        private readonly UniversityPracticeContext _context;

        public PairNumbersController(UniversityPracticeContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: PairNumbers
        public async Task<IActionResult> Index()
        {
            return View(await _context.PairNumbers.ToListAsync());
        }

        // GET: PairNumbers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pairNumber = await _context.PairNumbers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pairNumber == null)
            {
                return NotFound();
            }

            return View(pairNumber);
        }

        // GET: PairNumbers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PairNumbers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description")] PairNumber pairNumber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pairNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pairNumber);
        }

        // GET: PairNumbers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pairNumber = await _context.PairNumbers.FindAsync(id);
            if (pairNumber == null)
            {
                return NotFound();
            }
            return View(pairNumber);
        }

        // POST: PairNumbers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description")] PairNumber pairNumber)
        {
            if (id != pairNumber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pairNumber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PairNumberExists(pairNumber.Id))
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
            return View(pairNumber);
        }

        // GET: PairNumbers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pairNumber = await _context.PairNumbers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pairNumber == null)
            {
                return NotFound();
            }

            return View(pairNumber);
        }

        // POST: PairNumbers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pairNumber = await _context.PairNumbers.FindAsync(id);
            if (pairNumber != null)
            {
                _context.PairNumbers.Remove(pairNumber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PairNumberExists(int id)
        {
            return _context.PairNumbers.Any(e => e.Id == id);
        }
    }
}

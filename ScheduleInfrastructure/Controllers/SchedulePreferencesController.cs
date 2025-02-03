using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleDomain.Model;
using ScheduleInfrastructure;

namespace ScheduleInfrastructure.Controllers
{
    public class SchedulePreferencesController : Controller
    {
        private readonly UniversityPracticeContext _context;

        public SchedulePreferencesController(UniversityPracticeContext context)
        {
            _context = context;
        }

        // GET: SchedulePreferences
        public async Task<IActionResult> Index()
        {
            var userFullName = User.Identity.Name;
            var isAdmin = User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            IQueryable<SchedulePreference> query = _context.SchedulePreferences
                .Include(s => s.DayOfWeek)
                .Include(s => s.MaxPairsPerDay)
                .Include(s => s.PairNumber)
                .Include(s => s.Subject)
                .Include(s => s.Teacher);

            if (!isAdmin)
            {
                query = query.Where(s => s.Teacher.FullName == userFullName);
            }

            var schedulePreferences = await query
                .OrderBy(s => s.DayOfWeek.Id)
                .ThenBy(s => s.PairNumber.Id)
                .ToListAsync();

            return View(schedulePreferences);
        }


        // GET: SchedulePreferences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedulePreference = await _context.SchedulePreferences
                .Include(s => s.DayOfWeek)
                .Include(s => s.MaxPairsPerDay)
                .Include(s => s.PairNumber)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedulePreference == null)
            {
                return NotFound();
            }

            return View(schedulePreference);
        }

        // GET: SchedulePreferences/Create
        public IActionResult Create()
        {
            // Get the current user's full name from claims
            var userFullName = User.Identity.Name;

            // Find the user ID from the database using the full name
            var teacher = _context.Users
                .FirstOrDefault(u => u.FullName == userFullName);

            if (teacher == null)
            {
                return NotFound($"Unable to find user: {userFullName}");
            }

            // Get only subjects assigned to this teacher
            var teacherSubjects = _context.Subjects
                .Where(s => s.TeacherId == teacher.Id)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                });

            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName");
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id");
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description");
            ViewData["SubjectId"] = new SelectList(teacherSubjects, "Value", "Text");

            // Create a new SchedulePreference with the TeacherId pre-set
            var schedulePreference = new SchedulePreference { TeacherId = teacher.Id };
            return View(schedulePreference);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectId,DayOfWeekId,PairNumberId,MaxPairsPerDayId,Priority")] SchedulePreference schedulePreference)
        {
            // Get the current user's full name from claims
            var userFullName = User.Identity.Name;

            // Find the user ID from the database
            var teacher = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == userFullName);

            if (teacher == null)
            {
                return NotFound($"Unable to find user: {userFullName}");
            }

            // Set the TeacherId
            schedulePreference.TeacherId = teacher.Id;
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("MaxPairsPerDay");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            if (ModelState.IsValid)
            {
                _context.Add(schedulePreference);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we get here, something failed - redisplay form
            var teacherSubjects = _context.Subjects
                .Where(s => s.TeacherId == teacher.Id)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                });

            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", schedulePreference.DayOfWeekId);
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id", schedulePreference.MaxPairsPerDayId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", schedulePreference.PairNumberId);
            ViewData["SubjectId"] = new SelectList(teacherSubjects, "Value", "Text", schedulePreference.SubjectId);
            return View(schedulePreference);
        }

        // GET: SchedulePreferences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedulePreference = await _context.SchedulePreferences.FindAsync(id);
            if (schedulePreference == null)
            {
                return NotFound();
            }
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", schedulePreference.DayOfWeekId);
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id", schedulePreference.MaxPairsPerDayId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", schedulePreference.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", schedulePreference.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", schedulePreference.TeacherId);
            return View(schedulePreference);
        }

        // POST: SchedulePreferences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,SubjectId,DayOfWeekId,PairNumberId,MaxPairsPerDayId,Priority")] SchedulePreference schedulePreference)
        {
            if (id != schedulePreference.Id)
            {
                return NotFound();
            }
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("MaxPairsPerDay");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedulePreference);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchedulePreferenceExists(schedulePreference.Id))
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
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", schedulePreference.DayOfWeekId);
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id", schedulePreference.MaxPairsPerDayId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", schedulePreference.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", schedulePreference.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", schedulePreference.TeacherId);
            return View(schedulePreference);
        }

        // GET: SchedulePreferences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedulePreference = await _context.SchedulePreferences
                .Include(s => s.DayOfWeek)
                .Include(s => s.MaxPairsPerDay)
                .Include(s => s.PairNumber)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedulePreference == null)
            {
                return NotFound();
            }

            return View(schedulePreference);
        }

        // POST: SchedulePreferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedulePreference = await _context.SchedulePreferences.FindAsync(id);
            if (schedulePreference != null)
            {
                _context.SchedulePreferences.Remove(schedulePreference);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchedulePreferenceExists(int id)
        {
            return _context.SchedulePreferences.Any(e => e.Id == id);
        }
    }
}

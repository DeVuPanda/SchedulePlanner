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
                    .ThenInclude(s => s.Group) // Include Group information
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
                    .ThenInclude(s => s.Group) // Include Group information
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
            var userFullName = User.Identity.Name;
            var teacher = _context.Users
                .FirstOrDefault(u => u.FullName == userFullName);

            if (teacher == null)
            {
                return NotFound($"Unable to find user: {userFullName}");
            }

            // Get subjects assigned to this teacher, including group information
            var teacherSubjects = _context.Subjects
                .Include(s => s.Group)
                .Where(s => s.TeacherId == teacher.Id)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} ({s.Group.GroupName})" // Include group name in the display text
                });

            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName");
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id");
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description");
            ViewData["SubjectId"] = new SelectList(teacherSubjects, "Value", "Text");

            var schedulePreference = new SchedulePreference { TeacherId = teacher.Id };
            return View(schedulePreference);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectId,DayOfWeekId,PairNumberId,MaxPairsPerDayId,Priority")] SchedulePreference schedulePreference)
        {
            var userFullName = User.Identity.Name;
            var teacher = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == userFullName);

            if (teacher == null)
            {
                return NotFound($"Unable to find user: {userFullName}");
            }

            // Get the subject with its group
            var subject = await _context.Subjects
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == schedulePreference.SubjectId);

            if (subject == null)
            {
                return NotFound($"Unable to find subject with ID: {schedulePreference.SubjectId}");
            }

            schedulePreference.TeacherId = teacher.Id;
            schedulePreference.GroupId = subject.GroupId; // Set the GroupId from the subject

            // Remove navigation properties from ModelState
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("MaxPairsPerDay");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            ModelState.Remove("Group");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(schedulePreference);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the preference. Please try again.");
                    // Log the exception details
                    Console.WriteLine(ex);
                }
            }

            // If we get here, something failed - redisplay form
            var teacherSubjects = _context.Subjects
                .Include(s => s.Group)
                .Where(s => s.TeacherId == teacher.Id)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} ({s.Group.GroupName})"
                });

            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", schedulePreference.DayOfWeekId);
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id", schedulePreference.MaxPairsPerDayId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", schedulePreference.PairNumberId);
            ViewData["SubjectId"] = new SelectList(teacherSubjects, "Value", "Text", schedulePreference.SubjectId);

            return View(schedulePreference);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,SubjectId,DayOfWeekId,PairNumberId,MaxPairsPerDayId,Priority")] SchedulePreference schedulePreference)
        {
            if (id != schedulePreference.Id)
            {
                return NotFound();
            }

            // Verify the schedule preference exists and belongs to the teacher
            var originalPreference = await _context.SchedulePreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (originalPreference == null)
            {
                return NotFound();
            }

            schedulePreference.SubjectId = originalPreference.SubjectId;

            var subject = await _context.Subjects
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == schedulePreference.SubjectId);

            if (subject == null)
            {
                return NotFound($"Unable to find subject with ID: {schedulePreference.SubjectId}");
            }

            schedulePreference.GroupId = subject.GroupId; 

            ModelState.Remove("DayOfWeek");
            ModelState.Remove("MaxPairsPerDay");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            ModelState.Remove("Group");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedulePreference);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }

            // Get subject name for display in case of validation error
            ViewBag.SubjectName = subject?.Name;
            if (subject?.Group != null)
            {
                ViewBag.SubjectName += $" ({subject.Group.GroupName})";
            }

            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", schedulePreference.DayOfWeekId);
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id", schedulePreference.MaxPairsPerDayId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", schedulePreference.PairNumberId);

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
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedulePreference == null)
            {
                return NotFound();
            }

            return View(schedulePreference);
        }

        // GET: SchedulePreferences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedulePreference = await _context.SchedulePreferences
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedulePreference == null)
            {
                return NotFound();
            }

            // Get subject name for display
            ViewBag.SubjectName = schedulePreference.Subject?.Name;
            if (schedulePreference.Subject?.Group != null)
            {
                ViewBag.SubjectName += $" ({schedulePreference.Subject.Group.GroupName})";
            }

            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", schedulePreference.DayOfWeekId);
            ViewData["MaxPairsPerDayId"] = new SelectList(_context.MaxPairsPerDays, "Id", "Id", schedulePreference.MaxPairsPerDayId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", schedulePreference.PairNumberId);

            return View(schedulePreference);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule()
        {
            try
            {
                // Check if there are any schedule preferences
                var schedulePreferencesCount = await _context.SchedulePreferences.CountAsync();
                if (schedulePreferencesCount == 0)
                {
                    TempData["ScheduleCreationError"] = "No schedule preferences found. Please add preferences first.";
                    return RedirectToAction(nameof(Index));
                }

                // Retrieve all schedule preferences with careful null checking
                var schedulePreferences = await _context.SchedulePreferences
                    .Include(sp => sp.DayOfWeek)
                    .Include(sp => sp.PairNumber)
                    .Include(sp => sp.Subject)
                    .Include(sp => sp.Teacher)
                    .Include(sp => sp.Group)
                    .Include(sp => sp.MaxPairsPerDay)
                    .Where(sp =>
                        sp.DayOfWeek != null &&
                        sp.PairNumber != null &&
                        sp.Subject != null &&
                        sp.Teacher != null &&
                        sp.Group != null &&
                        sp.MaxPairsPerDay != null)
                    .OrderBy(sp => sp.Priority)
                    .ToListAsync();

                // Double-check if we have any valid preferences
                if (!schedulePreferences.Any())
                {
                    TempData["ScheduleCreationError"] = "No valid schedule preferences found. Some preferences are missing required information.";
                    return RedirectToAction(nameof(Index));
                }

                // Clear existing final schedules to prevent duplicates
                _context.FinalSchedules.RemoveRange(_context.FinalSchedules);

                // Tracking to prevent duplicate assignments
                var assignedSlots = new HashSet<(int DayOfWeekId, int PairNumberId, int GroupId)>();
                var teacherDailyPairs = new Dictionary<(int TeacherId, int DayOfWeekId), int>();
                var groupDailyPairs = new Dictionary<(int GroupId, int DayOfWeekId), int>();

                var finalSchedules = new List<FinalSchedule>();

                foreach (var preference in schedulePreferences)
                {
                    // Null checks before accessing properties
                    if (preference?.DayOfWeek == null ||
                        preference.PairNumber == null ||
                        preference.MaxPairsPerDay == null ||
                        preference.Group == null)
                    {
                        continue;
                    }

                    // Check if the slot is already taken for this group
                    var slotKey = (preference.DayOfWeekId, preference.PairNumberId, preference.GroupId);
                    if (assignedSlots.Contains(slotKey))
                        continue;

                    // Check teacher's daily pair limit
                    var teacherDailyKey = (preference.TeacherId, preference.DayOfWeekId);
                    if (!teacherDailyPairs.ContainsKey(teacherDailyKey))
                        teacherDailyPairs[teacherDailyKey] = 0;

                    // Check if teacher has reached max pairs for the day
                    if (teacherDailyPairs[teacherDailyKey] >= preference.MaxPairsPerDay.Id)
                        continue;

                    // Check group's daily pairs (assuming max 4 pairs per day for any group)
                    var groupDailyKey = (preference.GroupId, preference.DayOfWeekId);
                    if (!groupDailyPairs.ContainsKey(groupDailyKey))
                        groupDailyPairs[groupDailyKey] = 0;

                    const int MaxGroupPairsPerDay = 4;
                    if (groupDailyPairs[groupDailyKey] >= MaxGroupPairsPerDay)
                        continue;

                    // Create final schedule entry
                    var finalSchedule = new FinalSchedule
                    {
                        SubjectId = preference.SubjectId,
                        TeacherId = preference.TeacherId,
                        GroupId = preference.GroupId,
                        DayOfWeekId = preference.DayOfWeekId,
                        PairNumberId = preference.PairNumberId,
                        IsClassroomAssigned = false
                    };

                    finalSchedules.Add(finalSchedule);
                    _context.FinalSchedules.Add(finalSchedule);

                    // Mark slot as assigned
                    assignedSlots.Add(slotKey);
                    teacherDailyPairs[teacherDailyKey]++;
                    groupDailyPairs[groupDailyKey]++;
                }

                // Save changes
                await _context.SaveChangesAsync();

                // Redirect back to the index with a success message
                TempData["ScheduleCreationMessage"] = $"Schedule successfully created with {finalSchedules.Count} entries!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, use proper logging)
                Console.Error.WriteLine(ex);
                TempData["ScheduleCreationError"] = $"Error creating schedule: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SchedulePreferences/DeleteAll
        public IActionResult DeleteAll()
        {
            return View();
        }

        // POST: SchedulePreferences/DeleteAll
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllConfirmed()
        {
            var allPreferences = await _context.SchedulePreferences.ToListAsync();
            if (allPreferences.Any())
            {
                _context.SchedulePreferences.RemoveRange(allPreferences);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SchedulePreferenceExists(int id)
        {
            return _context.SchedulePreferences.Any(e => e.Id == id);
        }
    }
}

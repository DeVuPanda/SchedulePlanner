using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleDomain.Model;
using ScheduleInfrastructure;
using ClosedXML.Excel;

namespace ScheduleInfrastructure.Controllers
{
    public class FinalSchedulesController : Controller
    {
        private readonly UniversityPracticeContext _context;

        public FinalSchedulesController(UniversityPracticeContext context)
        {
            _context = context;
        }

        // GET: FinalSchedules
        public async Task<IActionResult> Index()
        {
            var universityPracticeContext = _context.FinalSchedules.Include(f => f.Classroom).Include(f => f.DayOfWeek).Include(f => f.PairNumber).Include(f => f.Subject).Include(f => f.Teacher);
            return View(await universityPracticeContext.ToListAsync());
        }

        // GET: FinalSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finalSchedule = await _context.FinalSchedules
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Subject)
                .Include(f => f.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finalSchedule == null)
            {
                return NotFound();
            }

            return View(finalSchedule);
        }

        // GET: FinalSchedules/Create
        public IActionResult Create()
        {
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "Id");
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName");
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: FinalSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeacherId,SubjectId,ClassroomId,DayOfWeekId,PairNumberId,IsClassroomAssigned")] FinalSchedule finalSchedule)
        {
            ModelState.Remove("Classroom");
            ModelState.Remove("ClassroomId");
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("IsClassroomAssigned");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            if (ModelState.IsValid)
            {
                _context.Add(finalSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "Id", finalSchedule.ClassroomId);
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", finalSchedule.DayOfWeekId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", finalSchedule.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", finalSchedule.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", finalSchedule.TeacherId);
            return View(finalSchedule);
        }

        // GET: FinalSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finalSchedule = await _context.FinalSchedules.FindAsync(id);
            if (finalSchedule == null)
            {
                return NotFound();
            }
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomNumber", finalSchedule.ClassroomId);
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", finalSchedule.DayOfWeekId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", finalSchedule.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", finalSchedule.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", finalSchedule.TeacherId);
            return View(finalSchedule);
        }

        // POST: FinalSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,SubjectId,ClassroomId,DayOfWeekId,PairNumberId,IsClassroomAssigned")] FinalSchedule finalSchedule)
        {
            if (id != finalSchedule.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Classroom");
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("IsClassroomAssigned");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            if (ModelState.IsValid)
            {

                if (finalSchedule.ClassroomId != null)
                {
                    finalSchedule.IsClassroomAssigned = true;
                }
                try
                {
                    _context.Update(finalSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinalScheduleExists(finalSchedule.Id))
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
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomNumber", finalSchedule.ClassroomId);
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", finalSchedule.DayOfWeekId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", finalSchedule.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", finalSchedule.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", finalSchedule.TeacherId);
            return View(finalSchedule);
        }

        // GET: FinalSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finalSchedule = await _context.FinalSchedules
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Subject)
                .Include(f => f.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finalSchedule == null)
            {
                return NotFound();
            }

            return View(finalSchedule);
        }

        // POST: FinalSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var finalSchedule = await _context.FinalSchedules.FindAsync(id);
            if (finalSchedule != null)
            {
                _context.FinalSchedules.Remove(finalSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: FinalSchedules/DeleteAll
        public IActionResult DeleteAll()
        {
            return View();
        }

        // POST: FinalSchedules/DeleteAll
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllConfirmed()
        {
            var allSchedules = await _context.FinalSchedules.ToListAsync();
            if (allSchedules.Any())
            {
                _context.FinalSchedules.RemoveRange(allSchedules);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: FinalSchedules/DownloadSchedule
        public async Task<IActionResult> DownloadSchedule()
        {
            // Get the data with all necessary includes
            var schedules = await _context.FinalSchedules
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Subject)
                .Include(f => f.Teacher)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Schedule");

                // Add headers
                worksheet.Cell(1, 1).Value = "Classroom";
                worksheet.Cell(1, 2).Value = "Day of Week";
                worksheet.Cell(1, 3).Value = "Pair Number";
                worksheet.Cell(1, 4).Value = "Subject";
                worksheet.Cell(1, 5).Value = "Teacher";
                worksheet.Cell(1, 6).Value = "Is Classroom Assigned";

                // Add data
                int row = 2;
                foreach (var schedule in schedules)
                {
                    worksheet.Cell(row, 1).Value = schedule.Classroom?.RoomNumber ?? "Not Assigned";
                    worksheet.Cell(row, 2).Value = schedule.DayOfWeek.DayName;
                    worksheet.Cell(row, 3).Value = schedule.PairNumber.Description;
                    worksheet.Cell(row, 4).Value = schedule.Subject.Name;
                    worksheet.Cell(row, 5).Value = schedule.Teacher.Email;
                    worksheet.Cell(row, 6).Value = schedule.IsClassroomAssigned ?? false;
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Style the header row
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Create a memory stream to save the workbook
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    // Return the Excel file
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Schedule.xlsx");
                }
            }
        }

        private bool FinalScheduleExists(int id)
        {
            return _context.FinalSchedules.Any(e => e.Id == id);
        }
    }
}

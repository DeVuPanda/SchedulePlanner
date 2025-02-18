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
        public async Task<IActionResult> Index(int? groupId)
        {
            var schedules = await _context.FinalSchedules
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Subject)
                .Include(f => f.Teacher)
                .Include(f => f.Group)
                .ToListAsync();

            // Filter by group if groupId is provided
            if (groupId.HasValue)
            {
                schedules = schedules.Where(s => s.GroupId == groupId).ToList();
            }

            // Get all groups for the dropdown
            ViewBag.Groups = await _context.Groups.OrderBy(g => g.GroupName).ToListAsync();
            ViewBag.SelectedGroupId = groupId;

            return View(schedules);
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
                .Include(f => f.Group)
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
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeacherId,SubjectId,GroupId,ClassroomId,DayOfWeekId,PairNumberId,IsClassroomAssigned")] FinalSchedule finalSchedule)
        {
            ModelState.Remove("Classroom");
            ModelState.Remove("ClassroomId");
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("IsClassroomAssigned");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            ModelState.Remove("Group");

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
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", finalSchedule.GroupId);

            return View(finalSchedule);
        }

        // GET: FinalSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finalSchedule = await _context.FinalSchedules
                .Include(f => f.Teacher)
                .Include(f => f.Subject)
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Group)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (finalSchedule == null)
            {
                return NotFound();
            }

            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomNumber", finalSchedule.ClassroomId);
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", finalSchedule.DayOfWeekId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", finalSchedule.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", finalSchedule.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", finalSchedule.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", finalSchedule.GroupId);
            return View(finalSchedule);
        }

        // POST: FinalSchedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,SubjectId,GroupId,ClassroomId,DayOfWeekId,PairNumberId,IsClassroomAssigned")] FinalSchedule finalSchedule)
        {
            if (id != finalSchedule.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Classroom");
            ModelState.Remove("DayOfWeek");
            ModelState.Remove("PairNumber");
            ModelState.Remove("Subject");
            ModelState.Remove("Teacher");
            ModelState.Remove("Group");

            if (ModelState.IsValid)
            {
                try
                {
                    if (finalSchedule.ClassroomId != null)
                    {
                        finalSchedule.IsClassroomAssigned = true;
                    }

                    _context.Update(finalSchedule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }

            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomNumber", finalSchedule.ClassroomId);
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", finalSchedule.DayOfWeekId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", finalSchedule.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", finalSchedule.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", finalSchedule.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", finalSchedule.GroupId);
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
                .Include(f => f.Group)
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

        public async Task<IActionResult> DownloadSchedule()
        {
            var schedules = await _context.FinalSchedules
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Subject)
                .Include(f => f.Teacher)
                .Include(f => f.Group)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                // Get all unique groups
                var groups = schedules.Select(s => s.Group)
                                    .OrderBy(g => g.GroupName)
                                    .Distinct()
                                    .ToList();

                // Create a worksheet for each group
                foreach (var group in groups)
                {
                    var groupSchedules = schedules.Where(s => s.GroupId == group.Id).ToList();
                    var worksheet = workbook.Worksheets.Add(group.GroupName);

                    // Get unique days and pair numbers for headers
                    var days = groupSchedules.Select(s => s.DayOfWeek)
                                           .OrderBy(d => d.Id)
                                           .Distinct()
                                           .ToList();
                    var pairs = groupSchedules.Select(s => s.PairNumber)
                                            .OrderBy(p => p.Id)
                                            .Distinct()
                                            .ToList();

                    // Add day headers starting from column 2
                    for (int i = 0; i < days.Count; i++)
                    {
                        worksheet.Cell(1, i + 2).Value = days[i].DayName;
                    }

                    // Add pair numbers in first column starting from row 2
                    for (int i = 0; i < pairs.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pairs[i].Description;
                    }

                    // Fill the schedule grid
                    for (int rowIndex = 0; rowIndex < pairs.Count; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < days.Count; colIndex++)
                        {
                            var currentSchedule = groupSchedules.FirstOrDefault(s =>
                                s.DayOfWeek.Id == days[colIndex].Id &&
                                s.PairNumber.Id == pairs[rowIndex].Id);

                            if (currentSchedule != null)
                            {
                                var cell = worksheet.Cell(rowIndex + 2, colIndex + 2);
                                var cellValue = $"{currentSchedule.Subject.Name}\n{currentSchedule.Teacher.FullName}";

                                if (currentSchedule.IsClassroomAssigned == true && currentSchedule.Classroom != null)
                                {
                                    cellValue += $"\nRoom: {currentSchedule.Classroom.RoomNumber}";
                                }

                                cell.Value = cellValue;
                                cell.Style.Alignment.WrapText = true;
                            }
                        }
                    }

                    // Style the worksheet
                    worksheet.Column(1).Style.Font.Bold = true;
                    var headerRow = worksheet.Row(1);
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Adjust column widths and row heights
                    worksheet.Columns().AdjustToContents();
                    worksheet.Rows().AdjustToContents();

                    // Add borders
                    var dataRange = worksheet.Range(1, 1, pairs.Count + 1, days.Count + 1);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
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

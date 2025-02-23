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
            // Get all days and pair numbers for complete grid
            var allDays = await _context.DaysOfWeeks.OrderBy(d => d.Id).ToListAsync();
            var allPairs = await _context.PairNumbers.OrderBy(p => p.Id).ToListAsync();

            // Get all groups for the dropdown
            var groups = await _context.Groups.OrderBy(g => g.GroupName).ToListAsync();

            // If no groupId is provided, redirect to the first group's schedule
            if (!groupId.HasValue && groups.Any())
            {
                return RedirectToAction("Index", new { groupId = groups.First().Id });
            }

            var schedules = await _context.FinalSchedules
                .Include(f => f.Classroom)
                .Include(f => f.DayOfWeek)
                .Include(f => f.PairNumber)
                .Include(f => f.Subject)
                .Include(f => f.Teacher)
                .Include(f => f.Group)
                .Where(s => s.GroupId == groupId)
                .ToListAsync();

            ViewBag.AllDays = allDays;
            ViewBag.AllPairs = allPairs;
            ViewBag.Groups = groups;
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
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName");
            return View();
        }

        private bool IsTeacherAvailable(int teacherId, int dayOfWeekId, int pairNumberId, int currentScheduleId)
        {
            return !_context.FinalSchedules.Any(fs =>
                fs.TeacherId == teacherId &&
                fs.DayOfWeekId == dayOfWeekId &&
                fs.PairNumberId == pairNumberId &&
                fs.Id != currentScheduleId);  
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
                if (!IsTeacherAvailable(
                    finalSchedule.TeacherId,
                    finalSchedule.DayOfWeekId,
                    finalSchedule.PairNumberId,
                     finalSchedule.Id))
                {
                    ModelState.AddModelError("TeacherId", "This teacher is already busy on this day on this pair.");
                    PrepareViewData(finalSchedule);
                    return View(finalSchedule);
                }

                _context.Add(finalSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PrepareViewData(finalSchedule);
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
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "FullName", finalSchedule.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", finalSchedule.GroupId);
            return View(finalSchedule);
        }

        private bool IsClassroomAvailable(int classroomId, int dayOfWeekId, int pairNumberId, int currentScheduleId)
        {
            return !_context.FinalSchedules.Any(fs =>
                fs.ClassroomId == classroomId &&
                fs.DayOfWeekId == dayOfWeekId &&
                fs.PairNumberId == pairNumberId &&
                fs.Id != currentScheduleId); 
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableClassrooms(int dayOfWeekId, int pairNumberId, int currentScheduleId)
        {
            var busyClassroomIds = await _context.FinalSchedules
                .Where(fs => fs.DayOfWeekId == dayOfWeekId &&
                            fs.PairNumberId == pairNumberId &&
                            fs.Id != currentScheduleId &&
                            fs.ClassroomId != null)
                .Select(fs => fs.ClassroomId.Value)
                .ToListAsync();

            var availableClassrooms = await _context.Classrooms
                .Where(c => !busyClassroomIds.Contains(c.Id))
                .Select(c => new { c.RoomNumber })
                .OrderBy(c => c.RoomNumber)
                .ToListAsync();

            return Json(availableClassrooms);
        }

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
                // Check teacher availability
                if (!IsTeacherAvailable(
                    finalSchedule.TeacherId,
                    finalSchedule.DayOfWeekId,
                    finalSchedule.PairNumberId,
                    finalSchedule.Id))
                {
                    ModelState.AddModelError("TeacherId", "This teacher is already busy on this day on this pair.");
                    PrepareViewData(finalSchedule);
                    return View(finalSchedule);
                }

                // Check classroom availability
                if (finalSchedule.ClassroomId != null)
                {
                    if (!IsClassroomAvailable(
                        finalSchedule.ClassroomId.Value,
                        finalSchedule.DayOfWeekId,
                        finalSchedule.PairNumberId,
                        finalSchedule.Id))
                    {
                        ModelState.AddModelError("ClassroomId", "This classroom is already busy on this day on this pair.");
                        PrepareViewData(finalSchedule);
                        return View(finalSchedule);
                    }
                    finalSchedule.IsClassroomAssigned = true;
                }

                try
                {
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
            PrepareViewData(finalSchedule);
            return View(finalSchedule);
        }

        private void PrepareViewData(FinalSchedule finalSchedule)
        {
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "RoomNumber", finalSchedule.ClassroomId);
            ViewData["DayOfWeekId"] = new SelectList(_context.DaysOfWeeks, "Id", "DayName", finalSchedule.DayOfWeekId);
            ViewData["PairNumberId"] = new SelectList(_context.PairNumbers, "Id", "Description", finalSchedule.PairNumberId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", finalSchedule.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", finalSchedule.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", finalSchedule.GroupId);
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


            var allDays = await _context.Set<DaysOfWeek>().OrderBy(d => d.Id).ToListAsync();
            var allPairs = await _context.Set<PairNumber>().OrderBy(p => p.Id).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var groups = schedules.Select(s => s.Group)
                                .OrderBy(g => g.GroupName)
                                .Distinct()
                                .ToList();

                foreach (var group in groups)
                {
                    var groupSchedules = schedules.Where(s => s.GroupId == group.Id).ToList();
                    var worksheet = workbook.Worksheets.Add(group.GroupName);

                    for (int i = 0; i < allDays.Count; i++)
                    {
                        worksheet.Cell(1, i + 2).Value = allDays[i].DayName;
                    }

                    for (int i = 0; i < allPairs.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = allPairs[i].Description;
                    }

                    for (int rowIndex = 0; rowIndex < allPairs.Count; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < allDays.Count; colIndex++)
                        {
                            var currentPairId = allPairs[rowIndex].Id;
                            var currentDayId = allDays[colIndex].Id;

                            var currentSchedule = groupSchedules.FirstOrDefault(s =>
                                s.DayOfWeekId == currentDayId &&
                                s.PairNumberId == currentPairId);

                            var cell = worksheet.Cell(rowIndex + 2, colIndex + 2);

                            if (currentSchedule != null)
                            {
                                var cellValue = $"{currentSchedule.Subject.Name}\n{currentSchedule.Teacher.FullName}";

                                if (currentSchedule.IsClassroomAssigned == true && currentSchedule.Classroom != null)
                                {
                                    cellValue += $"\nRoom: {currentSchedule.Classroom.RoomNumber}";
                                }

                                cell.Value = cellValue;
                            }
                            else
                            {
                                cell.Value = "";
                            }

                            cell.Style.Alignment.WrapText = true;
                        }
                    }

                    worksheet.Column(1).Style.Font.Bold = true;
                    var headerRow = worksheet.Row(1);
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Columns().AdjustToContents();
                    worksheet.Rows().AdjustToContents();

                    var dataRange = worksheet.Range(1, 1, allPairs.Count + 1, allDays.Count + 1);
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

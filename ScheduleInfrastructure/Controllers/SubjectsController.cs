using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleDomain.Model;
using ScheduleInfrastructure;

namespace ScheduleInfrastructure.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly UniversityPracticeContext _context;

        public SubjectsController(UniversityPracticeContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var universityPracticeContext = _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.Group);
            return View(await universityPracticeContext.ToListAsync());
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "Id", "GroupName");
            return View();
        }

        // POST: Subjects/Create
        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,TeacherId,GroupId,Hours,EducationProgram")] Subject subject)
        {
            ModelState.Remove("Teacher");
            ModelState.Remove("Group");

            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", subject.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "Id", "GroupName", subject.GroupId);
            return View(subject);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "FullName", subject.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "Id", "GroupName", subject.GroupId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TeacherId,GroupId,Hours,EducationProgram")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Teacher");
            ModelState.Remove("Group");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
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
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Email", subject.TeacherId);
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "Id", "GroupName", subject.GroupId);
            return View(subject);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
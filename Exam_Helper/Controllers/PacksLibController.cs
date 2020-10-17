using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exam_Helper;

namespace Exam_Helper.Controllers
{
    public class PacksLibController : Controller
    {
        private readonly CommonDbContext _context;

        public PacksLibController(CommonDbContext context)
        {
            _context = context;
        }

        // GET: PacksLib
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pack.ToListAsync());
        }

        // GET: PacksLib/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pack == null)
            {
                return NotFound();
            }

            return View(pack);
        }

        // GET: PacksLib/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PacksLib/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuestionSet,Author,CreationDate,UpdateDate,TagsId")] Pack pack)
        {
            if (ModelState.IsValid)
            {
                pack.CreationDate = DateTime.Now;
                pack.UpdateDate = DateTime.Now;
                pack.Author = "Admin";
                _context.Add(pack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pack);
        }

        // GET: PacksLib/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack.FindAsync(id);
            if (pack == null)
            {
                return NotFound();
            }
            return View(pack);
        }

        // POST: PacksLib/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionSet,Author,CreationDate,UpdateDate,TagsId")] Pack pack)
        {
            if (id != pack.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var old = await _context.Pack.AsNoTracking().FirstAsync(x => x.Id == id);
                    pack.CreationDate = old.CreationDate;
                    pack.UpdateDate = DateTime.Now;
                    _context.Update(pack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackExists(pack.Id))
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
            return View(pack);
        }

        // GET: PacksLib/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pack == null)
            {
                return NotFound();
            }

            return View(pack);
        }

        // POST: PacksLib/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pack = await _context.Pack.FindAsync(id);
            _context.Pack.Remove(pack);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackExists(int id)
        {
            return _context.Pack.Any(e => e.Id == id);
        }
    }
}

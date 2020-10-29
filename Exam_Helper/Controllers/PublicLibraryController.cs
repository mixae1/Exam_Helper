using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam_Helper.ViewsModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exam_Helper.Controllers
{
    public class PublicLibraryController : Controller
    {
        private readonly CommonDbContext _context;

        public PublicLibraryController(CommonDbContext context)
        {
            _context = context;
        }

        // GET: PublicLibraryController
        public async Task<IActionResult> Index(string SearchString)
        {
            var _ques = from _que in _context.Question
                       select _que;
            if (!string.IsNullOrEmpty(SearchString))
               _ques = _ques.Where(
                    x => x.Title.Contains(SearchString) ||
                    x.Proof.Contains(SearchString)      ||
                    x.TagIds.Contains(SearchString)     ||
                    x.Definition.Contains(SearchString));

            var _packs = from _pack in _context.Pack
                         select _pack;
            if (!string.IsNullOrEmpty(SearchString))
                _packs = _packs.Where(
                    x => x.Author.Contains(SearchString) ||
                    x.Name.Contains(SearchString));

            return View(new ClassForPublicLibrary
            {
                packs = await _packs.ToListAsync(),
                questions = await _ques.ToListAsync()
            });
        }

        public async Task<IActionResult> QDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var question = await _context.Question
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        public async Task<IActionResult> PDetails(int? id)
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

        // GET: QuestionsLib/Create
        [HttpGet]
        public async Task<IActionResult> QCreate()
        {
            var tags = await _context.Tags.Select(x => new TagForQuestionCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();

            return View(new ClassForQuestionCreatingModel() { question = new Question(), tags = tags });
        }

        // POST: QuestionsLib/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QCreate(ClassForQuestionCreatingModel obj)
        {
            if (ModelState.IsValid)
            {
                var StringTags = string.Join(";", obj.tags.Where(x => x.IsSelected).Select(x => x.Id));

                obj.question.CreationDate = DateTime.Now;
                obj.question.UpdateDate = DateTime.Now;
                obj.question.Author = "Admin";
                obj.question.TagIds = StringTags;
                _context.Add(obj.question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj.question);
        }

        // GET: PacksLib/Create
        public async Task<IActionResult> PCreate()
        {
            var ques = await _context.Question.Select(x => new QuestionForPackCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();
            var tags = await _context.Tags.Select(x => new TagForPackCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();

            return View(new ClassForPackCreatingModel() { questions = ques, pack = new Pack(), tags = tags });
        }

        // POST: PacksLib/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PCreate(ClassForPackCreatingModel obj)
        {
            Pack pack = new Pack();
            if (ModelState.IsValid)
            {

                var ques = obj.questions.Where(x => x.IsSelected).Select(x => x.Id);
                var StringIds = string.Join(';', ques);

                var tags = obj.tags.Where(x => x.IsSelected).Select(x => x.Id);
                var TagsIds = string.Join(';', tags);

                pack.Name = obj.pack.Name;
                pack.UpdateDate = DateTime.Now;
                pack.CreationDate = DateTime.Now;
                pack.Author = "Admin";
                pack.QuestionSet = StringIds;
                pack.TagsId = TagsIds;
                _context.Add(pack);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(pack);
        }

        // POST
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/

        // GET: PublicLibraryController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: PublicLibraryController/Edit/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/

        // GET: QuestionsLib/Edit/5
        public async Task<IActionResult> QEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: QuestionsLib/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QEdit(int id, [Bind("Id,Definition,Title,Proof,Author,TagIds")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var old = await _context.Question.AsNoTracking().FirstAsync(x => x.Id == id);
                    question.CreationDate = old.CreationDate;
                    question.UpdateDate = DateTime.Now;
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
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
            return View(question);
        }

        // GET: PacksLib/Edit/5
        public async Task<IActionResult> PEdit(int? id)
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
        public async Task<IActionResult> PEdit(int id, [Bind("Id,QuestionSet,Author,TagsId")] Pack pack)
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

        // GET: QuestionsLib/Delete/5
        public async Task<IActionResult> QDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Question
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: QuestionsLib/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QDeleteConfirmed(int id)
        {
            var question = await _context.Question.FindAsync(id);
            _context.Question.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: PublicLibraryController/Delete/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/

        // GET: PacksLib/Delete/5
        public async Task<IActionResult> PDelete(int? id)
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
        public async Task<IActionResult> PDeleteConfirmed(int id)
        {
            var pack = await _context.Pack.FindAsync(id);
            _context.Pack.Remove(pack);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public RedirectToActionResult QRedirectToTest(int id)
        {
            TempData["question_id"] = id;
            return RedirectToAction(nameof(Index), nameof(Tests));
        }

        private bool QuestionExists(int id)
        {
            return _context.Question.Any(e => e.Id == id);
        }

        private bool PackExists(int id)
        {
            return _context.Pack.Any(e => e.Id == id);
        }
    }
}

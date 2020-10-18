using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam_Helper.ViewsModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exam_Helper.Controllers
{
    public class QuestionsLibController : Controller
    {
        private readonly CommonDbContext _context;

        public QuestionsLibController(CommonDbContext context)
        {
            _context = context;
        }

        
        // GET: QuestionsLib
        public async Task<IActionResult> Index(string SearchString)
        {
            
            var ques = from que in _context.Question
                       select que; //await _context.Question.ToListAsync();
            if (!string.IsNullOrEmpty(SearchString))
                ques = ques.Where(x => x.Title.Contains(SearchString)
                                 || x.Proof.Contains(SearchString)
                                 || x.TagIds.Contains(SearchString)
                                 || x.Definition.Contains(SearchString));
            
            return View(await ques.ToListAsync());
        }

       

        // GET: QuestionsLib/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: QuestionsLib/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var tags = await _context.Tags.Select(x => new TagForQuestionCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();
            
            return View(new ClassForQuestionCreatingModel() { question = new Question(), tags = tags});
        }

        // POST: QuestionsLib/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassForQuestionCreatingModel obj)
        {
            if (ModelState.IsValid)
            {
                var StringTags = string.Join(";", obj.tags.Where(x=>x.IsSelected).Select(x=>x.Id));

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

        // GET: QuestionsLib/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id,  [Bind("Id,Definition,Title,Proof,Author,TagIds")] Question question)
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

        // GET: QuestionsLib/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Question.FindAsync(id);
            _context.Question.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _context.Question.Any(e => e.Id == id);
        }
    }
}

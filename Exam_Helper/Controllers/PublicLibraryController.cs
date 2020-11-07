using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam_Helper.ViewsModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Exam_Helper.Controllers
{
    [AllowAnonymous]
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
            //  ОСТАВИТЬ ЗДЕСЬ  А ТО МАЛО ЛИ ПОТОМ ЭТУ ХУЙНЮ ЕЩЕ ИСКАТЬ
            //  if (!User.Identity.IsAuthenticated)
            //    return RedirectToAction("Login","UserAccount");
           

            var _ques = from _que in _context.Question
                        where _que.IsPrivate==false
                        select _que;

            if (!string.IsNullOrEmpty(SearchString))
                _ques = _ques.Where(
                     x => x.Title.Contains(SearchString) ||
                     x.Proof.Contains(SearchString) ||
                     x.TagIds.Contains(SearchString) ||
                     x.Definition.Contains(SearchString));

            var _packs = from _pack in _context.Pack
                         where _pack.IsPrivate == false
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

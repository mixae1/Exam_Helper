using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam_Helper.ViewsModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Exam_Helper.ViewsModel.Libs;

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
        [AllowAnonymous]
        public async Task<IActionResult> Index(string SearchString)
        {

            //  if (!User.Identity.IsAuthenticated)
            //    return RedirectToAction("Login","UserAccount");

            //Приводим SearchString к нижнему регистру
            if (!string.IsNullOrEmpty(SearchString))
                SearchString = SearchString.ToLower();

            //Загружаем все публичные паки
            var _packs = from _pack in _context.Pack
                         where _pack.IsPrivate == false
                         select _pack;

            //Отбираем паки по SearchString
            if (!string.IsNullOrEmpty(SearchString))
                _packs = _packs.Where(
                    x => x.Author.ToLower().Trim().Contains(SearchString) ||
                    x.Name.ToLower().Trim().Contains(SearchString));

            //Собираем все вопросы, состоящие в паках = дельта вопросы(для простоты)
            var ques_in_packs = string.Join(";", _packs.Select(x => x.QuestionSet));

            //Загружаем все публичные ИЛИ дельта вопросы
            var _ques = from _que in _context.Question
                        where (_que.IsPrivate == false || ques_in_packs.Contains(_que.Id.ToString()))
                        select _que;

            //Создаём вспомогательную структуру для вопросов 
            var ques_help = _ques.Select(x => new QuestionInfo()
            {
                question = x,
                IsUser = x.IsPrivate,
                IsSearched = string.IsNullOrEmpty(SearchString) ? true :
                (//Отбираем вопросы по SearchString
                    x.Title.ToLower().Trim().Contains(SearchString) ||
                    (x.Proof != null && x.Proof.ToLower().Trim().Contains(SearchString)) ||
                    (x.TagIds != null && x.TagIds.ToLower().Trim().Contains(SearchString)) ||
                    x.Definition.ToLower().Trim().Contains(SearchString) ||
                    x.Author.ToLower().Trim().Contains(SearchString))
            });

            /*
            if (!string.IsNullOrEmpty(SearchString))
                ques_help = ques_help.Where(
                     x => x.IsSearched || x.question.Title.ToLower().Trim().Contains(SearchString) ||
                     (x.question.Proof != null && x.question.Proof.ToLower().Trim().Contains(SearchString)) ||
                      (x.question.Proof != null && x.question.TagIds.ToLower().Trim().Contains(SearchString)) ||
                     x.question.Definition.ToLower().Trim().Contains(SearchString));
            */

            //Загружаем тэги
            var tags = await _context.Tags.AsNoTracking().ToListAsync();


            return View(new ClassForUserLibrary
            {
                packs = await _packs.ToListAsync(),
                questions = await ques_help.ToListAsync(),
                tags=tags
            });
        }

        public async Task<IActionResult> QDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Question.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (question == null)
            {
                return NotFound();
            }

            var tagsIds = question.TagIds.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var tags = await _context.Tags.AsNoTracking().Where(x => tagsIds.Contains(x.Id.ToString())).ToListAsync();
            question.TagIds = string.Join(";", tags.Select(x => x.Title));

            return PartialView(question);
        }

        public async Task<IActionResult> PDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            var tagsIds = pack.TagsId.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var tags = await _context.Tags.AsNoTracking().Where(x => tagsIds.Contains(x.Id.ToString())).ToListAsync();
            pack.TagsId = string.Join("\n", tags.Select(x => x.Title));

            var queses = pack.QuestionSet.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var ques = await _context.Question.AsNoTracking().Where(x => queses.Contains(x.Id.ToString().Trim())).
                Select(x => x.Title).ToListAsync();

            pack.QuestionSet = string.Join("\n", ques);



            return PartialView(pack);
        }

        [Authorize]
        public RedirectToActionResult QRedirectToTest(int id)
        {
            //TempData["question_id"] = id;
            HttpContext.Session.Remove("question_id");
            HttpContext.Session.Remove("question");
            HttpContext.Session.Remove("ReturnControllerName");

            HttpContext.Session.SetInt32("question_id", id);
            HttpContext.Session.SetString("ReturnControllerName", "PublicLibrary");
            return RedirectToAction(nameof(Index), nameof(Tests));
        }

        [Authorize]
        public RedirectToActionResult PRedirectToTest(int id)
        {
            HttpContext.Session.Remove("pack_id");
            HttpContext.Session.Remove("pack");
            HttpContext.Session.Remove("ReturnControllerName");

            HttpContext.Session.SetInt32("pack_id", id);
            HttpContext.Session.SetString("ReturnControllerName", "PublicLibrary");
            return RedirectToAction(nameof(Index),"PackTest");
        }



        private bool QuestionExists(int id)
        {
            return _context.Question.Any(e => e.Id == id);
        }

        private bool PackExists(int id)
        {
            return _context.Pack.Any(e => e.Id == id);
        }

        
        public async Task<bool> AddQuestionToMyLib(string ques_id)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            ques_id = ques_id.Substring(1);

            if (string.IsNullOrEmpty(qa.QuestionSet))
                qa.QuestionSet = "";

            if (qa.QuestionSet.Contains(ques_id)) return false;
            else
            {
                qa.QuestionSet = string.IsNullOrEmpty(qa.QuestionSet) ? ques_id + ";" : qa.QuestionSet + ques_id + ";";
                _context.Update(qa);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        
        public async Task<IActionResult> QAddSelected(List<int> ids)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);
            var tuples = await _context.Question.AsNoTracking().Where(x => ids.Contains(x.Id) && !qa.QuestionSet.Contains(x.Id.ToString())).Select(x => new ClassForSelectedComfirmed(x.Title, x.Id)).ToListAsync();

            return PartialView(tuples);
        }

        public async Task<string> QAddSelectedComfirmed(List<int> ids)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            if (string.IsNullOrEmpty(qa.QuestionSet))
                qa.QuestionSet = "";
            foreach(var id in ids)
            {
                if (!qa.QuestionSet.Contains(ids.ToString()))
                {
                    qa.QuestionSet += id.ToString() + ";";
                }
            }
            _context.Update(qa);
            await _context.SaveChangesAsync();
            return "InDeveloping";
        }

        public async Task<bool> AddPackToMyLib(string pack_id)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            pack_id = pack_id.Substring(1);

            if (string.IsNullOrEmpty(qa.PackSet))
                qa.PackSet = "";

            if (qa.PackSet.Contains(pack_id)) return false;
            else
            {
                qa.PackSet = string.IsNullOrEmpty(qa.PackSet) ? pack_id + ";" : qa.PackSet + pack_id + ";";
                _context.Update(qa);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<IActionResult> PAddSelected(List<int> ids)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);
            var tuples = await _context.Pack.AsNoTracking().Where(x => ids.Contains(x.Id) && !qa.PackSet.Contains(x.Id.ToString())).Select(x => new ClassForSelectedComfirmed(x.Name, x.Id)).ToListAsync();

            return PartialView(tuples);
        }

        public async Task<string> PAddSelectedComfirmed(List<int> ids)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            if (string.IsNullOrEmpty(qa.PackSet))
                qa.PackSet = "";
            foreach (var id in ids)
            {
                if (!qa.PackSet.Contains(ids.ToString()))
                {
                    qa.PackSet += id.ToString() + ";";
                }
            }
            _context.Update(qa);
            await _context.SaveChangesAsync();
            return "InDeveloping";
        }

        [HttpGet]
        public RedirectToActionResult AddHashQP(string hash)
        {
            QPHash qPHash = new QPHash(hash);
            if (qPHash.type == QPHash.Type.Question)
            {

                if(_context.Question.AsNoTracking().FirstOrDefault(x => x.Id == qPHash.number) != null)
                {
                    return RedirectToAction(nameof(QDetails), new { id = qPHash.number });
                }
                else
                {
                    return RedirectToAction();
                }
            }
            else if (qPHash.type == QPHash.Type.Pack)
            {
                if (_context.Pack.AsNoTracking().FirstOrDefault(x => x.Id == qPHash.number) != null)
                {
                    return RedirectToAction(nameof(PDetails), new { id = qPHash.number });
                }
                else
                {
                    return RedirectToAction();
                }
            }
            else return RedirectToAction();
        }
    }
}

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
    [Authorize]
    public class UserLibraryController : Controller
    {
        private readonly CommonDbContext _context;

        public UserLibraryController(CommonDbContext context)
        {
            _context = context;
        }

        // GET: PublicLibraryController
        public async Task<IActionResult> Index(string SearchString)
        {
           
            //  if (!User.Identity.IsAuthenticated)
            //    return RedirectToAction("Login","UserAccount");
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            var temp_qa = qa.QuestionSet;

            var qs = string.IsNullOrEmpty(temp_qa) ? new HashSet<int>()
            : new HashSet<int>(temp_qa.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));

            var _ques = from _que in _context.Question
                        where (qs.Contains(_que.Id))
                        select _que;

            if (!string.IsNullOrEmpty(SearchString))
                _ques = _ques.Where(
                     x => x.Title.Contains(SearchString) ||
                     x.Proof.Contains(SearchString) ||
                     x.TagIds.Contains(SearchString) ||
                     x.Definition.Contains(SearchString));

            var temp_pa = qa.PackSet;
            var ps = string.IsNullOrEmpty(temp_pa) ? new HashSet<int>()
            : new HashSet<int>(temp_pa.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));
            var _packs = from _pack in _context.Pack
                         where (ps.Contains(_pack.Id))
                         select _pack;

            if (!string.IsNullOrEmpty(SearchString))
                _packs = _packs.Where(
                    x => x.Author.Contains(SearchString) ||
                    x.Name.Contains(SearchString));

            var tags =await _context.Tags.AsNoTracking().ToListAsync();

            return View(new ClassForPublicLibrary
            {
                packs = await _packs.ToListAsync(),
                questions = await _ques.ToListAsync(),
                tags=tags
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
            var tags = await _context.Tags.AsNoTracking().Select(x => new TagForQuestionCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();

            return View(new ClassForQuestionCreatingModel() { tags = tags });
        }

        // POST: QuestionsLib/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QCreate(ClassForQuestionCreatingModel ob)
        {
            if (ModelState.IsValid)
            {
                var tags = ob.tags.Where(x => x.IsSelected).Select(x => x.Id);

                if (tags.Count() == 0)
                    ModelState.AddModelError(string.Empty, "вы должны указать как минимум один тег");


                Question obj = new Question();
                var StringTags = string.Join(";",tags);
                var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);
                obj.CreationDate = DateTime.Now;
                obj.UpdateDate = DateTime.Now;
                obj.Author = User.Identity.Name;
                obj.TagIds = StringTags;
                obj.IsPrivate = true;
                obj.Definition = ob.Definition;
                obj.Proof = ob.Proof;
                obj.Title = ob.Title;

                _context.Add(obj);
                await _context.SaveChangesAsync();


                qa.QuestionSet = string.IsNullOrEmpty(qa.QuestionSet) ? obj.Id + ";" : qa.QuestionSet + obj.Id + ";";
                _context.Update(qa);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ob);
        }

        // GET: PacksLib/Create
        public async Task<IActionResult> PCreate()
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);

            var qs = qa.QuestionSet;
            List<QuestionForPackCreatingModel> ques = new List<QuestionForPackCreatingModel>();
            if (string.IsNullOrEmpty(qs))
            {
                ques = null;
            }
            else
            {

                var temp = new HashSet<int>(qs.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));

                ques=await _context.Question.AsNoTracking().Where(x=>temp.Contains(x.Id))
                .Select(x => new QuestionForPackCreatingModel()
                { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();
            }
                 var tags = await _context.Tags.AsNoTracking().Select(x => new TagForPackCreatingModel()
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
                var tags = obj.tags.Where(x => x.IsSelected).Select(x => x.Id);


                if (tags.Count() == 0)
                    ModelState.AddModelError(string.Empty, "вы должны указать как минимум один тег");

                if (ques.Count() == 0)
                {
                    ModelState.AddModelError(string.Empty, "вы должны выбрать как минимум 1 вопрос");
                    return View(obj);
                }
                var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

                var StringIds = string.Join(';', ques);

               
                var TagsIds = string.Join(';', tags);

                pack.Name = obj.pack_name;
                pack.UpdateDate = DateTime.Now;
                pack.CreationDate = DateTime.Now;
                pack.Author = User.Identity.Name;
                pack.QuestionSet = StringIds;
                pack.TagsId = TagsIds;
                pack.IsPrivate = true;

                _context.Add(pack);
                await _context.SaveChangesAsync();


                qa.PackSet = string.IsNullOrEmpty(qa.PackSet) ? pack.Id + ";" : qa.PackSet + pack.Id + ";";
                _context.Update(qa);


                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }


        // 1) вопрос чужой : get: отдаем вопрос на редактирование   post: создаем копию вопроса у себя ,старый индекс меняем на новый 
        // 2) вопрос свой :  аналогично п.1 

        // GET: QuestionsLib/Edit/5
        public async Task<IActionResult> QEdit(int? id)
        {
            if (id == null)                
            {
                return NotFound();
            }

            var question = await _context.Question.AsNoTracking().FirstAsync(x => x.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            // делаем выборку тегов и отмечаем те ,которые уже стоят на вопросе
            
            var tags = await _context.Tags.AsNoTracking().Select(x => new TagForQuestionCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = question.TagIds.Contains(x.Id.ToString())}).ToListAsync();

            if (tags == null)
                tags = new List<TagForQuestionCreatingModel>();

            return View(new ClassForQuestionCreatingModel() { Id=question.Id,Definition=question.Definition,Proof=question.Proof,
                                                              Title=question.Title, tags = tags });

        }

        // POST: QuestionsLib/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QEdit(int Id,[Bind("Definition,Title,Proof,tags")] ClassForQuestionCreatingModel ques)
        {
            if (Id == 0)
                throw new Exception("incorrect data in post QEdit");

            var obj = await _context.Question.AsNoTracking().FirstAsync(x => x.Id == Id);


            if (ModelState.IsValid)
            {
                var tags_check = ques.tags.Where(x => x.IsSelected).Select(x => x.Id);


                if (tags_check.Count() == 0)
                {
                    ModelState.AddModelError(string.Empty, "вы должны указать как минимум один тег");
                    return View(ques);
                }
                

                try
                {
                    var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

                    Question question = new Question()
                    {
                        Title = ques.Title,
                        TagIds = string.Join(";",tags_check),
                        Proof =ques.Proof,
                        Definition = ques.Definition,
                        CreationDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Author = User.Identity.Name,
                        IsPrivate = true
                    };

                    _context.Question.Add(question);
                    await _context.SaveChangesAsync();

                    if (question.Id == 0)
                        throw new Exception("smth went wrong with adding to BD");

                    qa.QuestionSet = qa.QuestionSet.Replace(Id.ToString(), "");
                    qa.QuestionSet = string.IsNullOrEmpty(qa.QuestionSet) ? question.Id + ";" : qa.QuestionSet + question.Id + ";";

                    _context.Update(qa);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(ques.Id))
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
            return View(ques);
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
        [HttpGet]
        public async Task<IActionResult> QDelete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            return PartialView(await _context.Question.AsNoTracking().FirstAsync(x => x.Id == id.Value));
        }


        // POST: QuestionsLib/Delete/5
        //[HttpPost, ActionName("QDelete")]
        //[ValidateAntiForgeryToken]
        
        public async Task<IActionResult> QDeleteConfirmed(int id)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            if (!string.IsNullOrEmpty(qa.QuestionSet))
            {
                if (qa.QuestionSet.Contains(id.ToString()))
                {
                    var qs = qa.QuestionSet.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    qa.QuestionSet = string.Join(';', qs.Where(s => s != id.ToString())) + ';';
                    _context.Update(qa);
                    await _context.SaveChangesAsync();
                    //return Json(new { success = true });
                    return RedirectToAction(nameof(Index));
                }
            }

            //return RedirectToAction(nameof(Index));
            return Json(new { success = false });
        }


        // GET: PacksLib/Delete/5
        [HttpGet]
        public async Task<IActionResult> PDelete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            return PartialView(await _context.Pack.AsNoTracking().FirstAsync(x => x.Id == id.Value));
        }
        /*
        // POST: PacksLib/Delete/5
        [HttpPost, ActionName("PDelete")]
        [ValidateAntiForgeryToken]
        */
        
         public async Task<IActionResult> PDeleteConfirmed(int id)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            if (!string.IsNullOrEmpty(qa.PackSet))
            {
                if (qa.PackSet.Contains(id.ToString()))
                {
                    var ps = qa.PackSet.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    qa.PackSet = string.Join(';', ps.Where(s => s != id.ToString())) + ';';
                    _context.Update(qa);
                    await _context.SaveChangesAsync();
                    //return Json(new { success = true });
                    return RedirectToAction(nameof(Index));
                }
            }

            //return RedirectToAction(nameof(Index));
            return Json(new { success = false });
        }

        public RedirectToActionResult QRedirectToTest(int id)
        {

            //TempData["question_id"] = id;
            HttpContext.Session.SetInt32("question_id", id);
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

        public async Task AddQuestionToMyLib(int Ques_id)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);
            var obj = await _context.Question.AsNoTracking().FirstAsync(x => x.Id == Ques_id);

            Question new_ques = new Question()
            {
                Title = obj.Title,
                TagIds = obj.TagIds,
                Proof = obj.Proof,
                Definition = obj.Definition,
                CreationDate = obj.CreationDate,
                UpdateDate = obj.UpdateDate,
                Author = obj.Author,
                IsPrivate = true
            };

            _context.Question.Add(new_ques);
            await _context.SaveChangesAsync();

            if (new_ques.Id == 0)
                throw new Exception("fuck");

            qa.QuestionSet = string.IsNullOrEmpty(qa.QuestionSet) ? new_ques.Id + ";" : qa.QuestionSet + new_ques.Id + ";";

            _context.Update(qa);
            await _context.SaveChangesAsync();

        }

        public async Task AddPackToMyLib(int pack_id)
        {


            if (pack_id == 0)
                throw new Exception("fuck");

            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);
            var obj = await _context.Pack.AsNoTracking().FirstAsync(x => x.Id == pack_id);

            Pack pck = new Pack()
            {
                IsPrivate = true,
                Author = qa.UserName,
                QuestionSet = obj.QuestionSet,
                CreationDate = obj.CreationDate,
                UpdateDate = obj.UpdateDate,
                TagsId = obj.TagsId,
                Name = obj.Name
            };

            _context.Pack.Add(pck);
            await _context.SaveChangesAsync();

            if (pck.Id == 0)
                throw new Exception("fuck");

            qa.PackSet = string.IsNullOrEmpty(qa.PackSet) ? pck.Id + ";" : qa.PackSet + pck.Id + ";";

            _context.Update(qa);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangeQuestionPrivate(string ques_id)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);

            ques_id = ques_id.Substring(1);

            if (!qa.QuestionSet.Contains(ques_id)) return false;
            else
            {
                var temp = await _context.Question.FindAsync(int.Parse(ques_id));
                
                //temp.IsPrivate = !temp.IsPrivate;
                if (temp.IsPrivate) temp.IsPrivate = false;
                else temp.IsPrivate = true;
                
                _context.Update(temp);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ChangePackPrivate(string pack_id)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            pack_id = pack_id.Substring(1);

            if (!qa.PackSet.Contains(pack_id)) return false;
            else
            {
                var temp = await _context.Pack.FindAsync(int.Parse(pack_id));

                //temp.IsPrivate = !temp.IsPrivate;
                if (temp.IsPrivate) temp.IsPrivate = false;
                else temp.IsPrivate = true;

                _context.Update(temp);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam_Helper.ViewsModel;
using Exam_Helper.ViewsModel.Libs;
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

            //Приводим SearchString к нижнему регистру
            if (!string.IsNullOrEmpty(SearchString))
                SearchString = SearchString.ToLower();

            //Загружаем пользователя
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            //Получаем список паков пользователя
            var temp_pa = qa.PackSet;

            //Получаем HashSet паков пользователя
            var ps = string.IsNullOrEmpty(temp_pa) ? 
                new HashSet<int>()
                : new HashSet<int>(temp_pa.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));
            
            //Подгружаем паки пользователя из бд
            var _packs = from _pack in _context.Pack
                         where (ps.Contains(_pack.Id))
                         select _pack;

            //Отбираем паки по SearchString
            if (!string.IsNullOrEmpty(SearchString))
                _packs = _packs.Where(
                    x => x.Author.ToLower().Trim().Contains(SearchString) ||
                    x.Name.ToLower().Trim().Contains(SearchString));

            //Собираем все вопросы, состоящие в паках = дельта вопросы(для простоты)
            var ques_in_packs = string.Join(";", _packs.Select(x => x.QuestionSet));
            /* получаем id вопросов ,которые не у пользователя 
             но приэтом находятся в паке ( такое возникает когда мы редактируем  вопрос,ибо в паках отсается старая версия и тк 
            у юзера старый айди исчезает то вопрос в паках появляться не будет)
            */

            //Собираем вопросы юзера а также дельта вопросы вместе
            var temp_qa = qa.QuestionSet + ques_in_packs;

            //Получаем HashSet вопросов пользователя
            var qs = string.IsNullOrEmpty(temp_qa) ? 
                new HashSet<int>()
                : new HashSet<int>(temp_qa.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));

            //Подгружаем вопросы пользователя
            var _ques = from _que in _context.Question
                        where (qs.Contains(_que.Id) )
                        select _que;

            //Преобразуем вопросы пользователя в List - WHY?
            //var temp_ques = await _ques.ToListAsync();

            //Создаём вспомогательную структуру для вопросов 
            var ques = _ques.Select(x => new QuestionInfo()
            {
                question = x,
                IsUser = qa.QuestionSet.Contains(x.Id.ToString()),
                IsSearched = string.IsNullOrEmpty(SearchString) ? true :
                (//Отбираем вопросы по SearchString
                    x.Title.ToLower().Trim().Contains(SearchString) ||
                    (x.Proof != null && x.Proof.ToLower().Trim().Contains(SearchString)) ||
                    (x.TagIds != null && x.TagIds.ToLower().Trim().Contains(SearchString)) ||
                    x.Definition.ToLower().Trim().Contains(SearchString) ||
                    x.Author.ToLower().Trim().Contains(SearchString)),
                    IsSelected = false
            });

            //Загружаем тэги
            var tags =await _context.Tags.AsNoTracking().ToListAsync();

            return View(new ClassForUserLibrary
            {
                packs = await _packs.ToListAsync(),
                questions = await ques.ToListAsync(),
                tags = tags
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

            var tags = await _context.Tags.AsNoTracking().Where(x => question.TagIds.Contains(x.Id.ToString())).ToListAsync();
            question.TagIds = string.Join(";", tags.Select(x => x.Title));
            if (question == null)
            {
                return NotFound();
            }

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

            var tags = await _context.Tags.AsNoTracking().Where(x => pack.TagsId.Contains(x.Id.ToString())).ToListAsync();
            pack.TagsId = string.Join("\n", tags.Select(x => x.Title));

            var ques = await _context.Question.AsNoTracking().Where(x => pack.QuestionSet.Contains(x.Id.ToString())).
                Select(x => x.Title).ToListAsync();

            pack.QuestionSet = string.Join("\n", ques);


            if (pack == null)
            {
                return NotFound();
            }

            return PartialView(pack);
        }

        // GET: QuestionsLib/Create
        [HttpGet]
        public async Task<IActionResult> QCreate()
        {
            var tags = await _context.Tags.AsNoTracking().Select(x => x.Title).ToListAsync();
            return View(new ClassForQuestionCreatingModel() { tags = new TagsForQuestionCreatinModel { LoadedTags = tags } });
        }

        // POST: QuestionsLib/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QCreate([Bind("Definition,Title,Proof,tags")]ClassForQuestionCreatingModel ob)
        {
            if (ModelState.IsValid)
            {
                List<string> selectedTags = (ob.tags == null ? new List<string>() : ob.tags.SelectedTags);
                List<Tags> loadedTags = await _context.Tags.AsNoTracking().ToListAsync();
                List<Tags> tagsIntersection = new List<Tags>();

                StringBuilder selectedTagsString = new StringBuilder();
                //получение номеров тэгов, которые уже существовали
                foreach (var tagName in selectedTags)
                {
                    Tags temp = loadedTags.FirstOrDefault(y => y.Title == tagName);
                    if (temp == null) 
                    {
                        tagsIntersection.Add(new Tags{ Title = tagName });
                        continue;
                    }
                    selectedTagsString.Append(temp.Id);
                    selectedTagsString.Append(';');
                }

                foreach(var tag in tagsIntersection)
                {
                    _context.Add(tag);
                    await _context.SaveChangesAsync();
                    selectedTagsString.Append(tag.Id);
                    selectedTagsString.Append(';');
                }

                Question obj = new Question();
                var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);
                obj.CreationDate = DateTime.Now;
                obj.UpdateDate = DateTime.Now;
                obj.Author = User.Identity.Name;
                obj.TagIds = selectedTagsString.ToString();
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

            var ps = qa.PackSet;
            List<PackForPackCreatingModel> packs = new List<PackForPackCreatingModel>();
            if (string.IsNullOrEmpty(ps))
            {
                packs = null;
            }
            else
            {

                var temp = new HashSet<int>(ps.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));

                packs = await _context.Pack.AsNoTracking().Where(x => temp.Contains(x.Id))
                .Select(x => new PackForPackCreatingModel()
                { Id = x.Id, Name = x.Name, IsSelected = false }).ToListAsync();
            }

            var tags = await _context.Tags.AsNoTracking().Select(x => new TagForPackCreatingModel()
                { Id = x.Id, Name = x.Title, IsSelected = false }).ToListAsync();
                
            return View(new ClassForPackCreatingModel() { questions = ques, packs = packs, tags = tags });
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
                var pqTemp = obj.packs?.Where(x => x.IsSelected).Select(x => x.Id);
                if (pqTemp == null)
                    pqTemp = new List<int>();

                HashSet<int> ques_from_packs = new HashSet<int>();

                //var temp2 = await _context.Pack.AsNoTracking().Where(x => pqTemp.Contains(x.Id)).
                //    Select(x => x.QuestionSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x))).ToListAsync();

                var temp3 = await _context.Pack.AsNoTracking().Where(x => pqTemp.Contains(x.Id)).ToListAsync();
                foreach(var _pack in temp3)
                {
                    var str_pack_ids = _pack.QuestionSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for(int i = 0; i < str_pack_ids.Length; i++)
                    {
                        ques_from_packs.Add(int.Parse(str_pack_ids[i]));
                    }
                }                

                var ques = obj.questions.Where(x => x.IsSelected).Select(x => x.Id);
                foreach (var id in ques)
                {
                    ques_from_packs.Add(id);
                }

                var tags = obj.tags.Where(x => x.IsSelected).Select(x => x.Id);

                
                 /* Пока разрешим создание без тегов
                if (tags.Count() == 0)
                    ModelState.AddModelError(string.Empty, "Вы должны указать как минимум один тег");
                */
                 //если нет вопросов , но были указаны паки из которых берем вопросы ,то все ок идем дальше
                if (ques.Count() == 0 && pqTemp.Count()==0)
                {   
                    //исправить нейминг
                    ModelState.AddModelError(string.Empty, "Вы должны выбрать как минимум 1 вопрос или пак");
                    return View(obj);
                }

                var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

                var StringIds = string.Join(';', ques_from_packs);

               
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

            //номера тэгов
            string[] qtags = question.TagIds.Split(';', StringSplitOptions.RemoveEmptyEntries);
            
            List<Tags> dbtags = await _context.Tags.AsNoTracking().ToListAsync();

            List<string> selectedTags = dbtags.Where(x => qtags.Contains(x.Id.ToString())).Select(x => x.Title).ToList();
            if (selectedTags == null) 
                selectedTags = new List<string>();

            return View(new ClassForQuestionCreatingModel() { 
                Id=question.Id,
                Definition=question.Definition,
                Proof=question.Proof,
                Title=question.Title,
                tags = new TagsForQuestionCreatinModel{ 
                    SelectedTags = selectedTags,
                    LoadedTags = dbtags.Select(x=>x.Title).ToList()
                } 
            });
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
                List<string> selectedTags = (ques.tags == null ? new List<string>() : ques.tags.SelectedTags);
                List<Tags> loadedTags = await _context.Tags.AsNoTracking().ToListAsync();
                List<Tags> tagsIntersection = new List<Tags>();

                StringBuilder selectedTagsString = new StringBuilder();
                //получение номеров тэгов, которые уже существовали
                foreach (var tagName in selectedTags)
                {
                    Tags temp = loadedTags.FirstOrDefault(y => y.Title == tagName);
                    if (temp == null)
                    {
                        tagsIntersection.Add(new Tags { Title = tagName });
                        continue;
                    }
                    selectedTagsString.Append(temp.Id);
                    selectedTagsString.Append(';');
                }

                foreach (var tag in tagsIntersection)
                {
                    _context.Add(tag);
                    await _context.SaveChangesAsync();
                    selectedTagsString.Append(tag.Id);
                    selectedTagsString.Append(';');
                }

                try
                {
                    var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

                    Question question = new Question()
                    {
                        Title = ques.Title,
                        TagIds = selectedTagsString.ToString(),
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
                        throw new Exception("smth went wrong with adding to DB");

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

            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);
            
            var pack = await _context.Pack.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);

            if (pack == null)
            {
                return NotFound();
            }

            var qtemp = new HashSet<int>(qa.QuestionSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));

            foreach(var q_id in pack.QuestionSet.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                qtemp.Add(int.Parse(q_id));
            }

            var ques = await _context.Question.AsNoTracking().Where(x => qtemp.Contains(x.Id))
                .Select(x => new QuestionForPackCreatingModel()
                { Id = x.Id, Name = x.Title, IsSelected = pack.QuestionSet.Contains(x.Id.ToString())}).ToListAsync();

            var ptemp = new HashSet<int>(qa.PackSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)));

            var packs = await _context.Pack.AsNoTracking().Where(x => ptemp.Contains(x.Id))
                .Select(x => new PackForPackCreatingModel()
                { Id = x.Id, Name = x.Name, IsSelected = false }).ToListAsync();

            var tags = await _context.Tags.AsNoTracking().Select(x => new TagForPackCreatingModel()
            { Id = x.Id, Name = x.Title, IsSelected = pack.TagsId.Contains(x.Id.ToString()) }).ToListAsync();

            if (tags == null)
                tags = new List<TagForPackCreatingModel>();

            return View(new ClassForPackCreatingModel() { questions = ques, packs = packs, tags = tags, pack_name = pack.Name});
        }

        // POST: PacksLib/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PEdit(int Id, ClassForPackCreatingModel old_pack)
        {
            if (old_pack == null)
                return View(old_pack);

            Pack new_pack = new Pack();

            if (Id == 0)
                throw new Exception("incorrect data in post PEdit");

            if (ModelState.IsValid)
            {
                var pqTemp = old_pack.packs.Where(x => x.IsSelected).Select(x => x.Id);

                HashSet<int> ques_from_packs = new HashSet<int>();

                var temp3 = await _context.Pack.AsNoTracking().Where(x => pqTemp.Contains(x.Id)).ToListAsync();
                foreach (var _pack in temp3)
                {
                    var str_pack_ids = _pack.QuestionSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < str_pack_ids.Length; i++)
                    {
                        ques_from_packs.Add(int.Parse(str_pack_ids[i]));
                    }
                }

                var ques = old_pack.questions.Where(x => x.IsSelected).Select(x => x.Id);
                foreach (var id in ques)
                {
                    ques_from_packs.Add(id);
                }

                if (ques_from_packs.Count() == 0)
                {
                    ModelState.AddModelError(string.Empty, "Вы должны выбрать как минимум 1 вопрос");
                    return View(old_pack);
                }


                var tags_check = old_pack.tags.Where(x => x.IsSelected).Select(x => x.Id);
              
                var obj = await _context.Pack.AsNoTracking().FirstAsync(x => x.Id == Id);
                var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

                try
                {
                    var StringIds = string.Join(';', ques_from_packs);


                    var TagsIds = string.Join(';', tags_check);


                    new_pack.Name = old_pack.pack_name;
                    new_pack.UpdateDate = DateTime.Now;
                    new_pack.CreationDate = obj.CreationDate;
                    new_pack.Author = User.Identity.Name;
                    new_pack.QuestionSet = StringIds;
                    new_pack.TagsId = TagsIds;
                    new_pack.IsPrivate = true;
                    

                    _context.Add(new_pack);
                    await _context.SaveChangesAsync();

                    if (new_pack.Id == 0)
                        throw new Exception("smth went wrong with adding to BD");

                    qa.PackSet = qa.PackSet.Replace(Id.ToString(),new_pack.Id.ToString());
                  //  qa.PackSet = string.IsNullOrEmpty(qa.PackSet) ? pack.Id + ";" : qa.PackSet + pack.Id + ";";
                    _context.Update(qa);


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackExists(old_pack.Id))
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
            return View(old_pack);
        }

        // GET: QuestionsLib/Delete/5
        [HttpGet]
        public async Task<IActionResult> QDelete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Question.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
            var tags = await _context.Tags.AsNoTracking().Where(x => question.TagIds.Contains(x.Id.ToString())).ToListAsync();
            question.TagIds = string.Join("\n", tags.Select(x => x.Title));

            return PartialView(question);
        }


        // POST: QuestionsLib/Delete/5
        //[HttpPost, ActionName("QDelete")]
        //[ValidateAntiForgeryToken]
        //[HttpPost, ActionName("QDelete")]
        public async Task<string> QDeleteComfirmed(int id)
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
                    return id.ToString();
                }
            }

            //return Json(new { success = false });
            return "error";
        }

        public async Task<IActionResult> QDeleteSelected(List<int> ids)
        {
            var tuples = await _context.Question.AsNoTracking().Where(x=>ids.Contains(x.Id)).Select(x=> new ClassForSelectedComfirmed(x.Title, x.Id)).ToListAsync();

            return PartialView(tuples);
        }

        public async Task<List<string>> QDeleteSelectedComfirmed(List<int> ids)
        {
            var qa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            if (!string.IsNullOrEmpty(qa.QuestionSet))
            {
                var qs = qa.QuestionSet.Split(';', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
                
                foreach(var id in ids)
                {
                    if(qs.Remove(id.ToString()))
                    {
                        //smth
                    }
                }
                qa.QuestionSet = string.Join(';', qs) + ';';
                _context.Update(qa);
                await _context.SaveChangesAsync();

                return ids.Select(x => x.ToString()).ToList();
            }

            //return Json(new { success = false });
            return null;
        }

        // GET: PacksLib/Delete/5
        [HttpGet]
        public async Task<IActionResult> PDelete(int? id)
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

            var tags = await _context.Tags.AsNoTracking().Where(x => pack.TagsId.Contains(x.Id.ToString())).ToListAsync();
            pack.TagsId = string.Join("\n", tags.Select(x => x.Title));

            var ques = await _context.Question.AsNoTracking().Where(x => pack.QuestionSet.Contains(x.Id.ToString())).
                Select(x => x.Title).ToListAsync();

            pack.QuestionSet = string.Join("\n", ques);

            return PartialView(pack);
        }
        /*
        // POST: PacksLib/Delete/5
        [HttpPost, ActionName("PDelete")]
        [ValidateAntiForgeryToken]
        */

        public async Task<string> PDeleteComfirmed(int id)
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
                    return id.ToString();
                }
            }

            //return Json(new { success = false });
            return "error";
        }

        public async Task<IActionResult> PDeleteSelected(List<int> ids)
        {
            var tuples = await _context.Pack.AsNoTracking().Where(x => ids.Contains(x.Id)).Select(x => new ClassForSelectedComfirmed(x.Name, x.Id)).ToListAsync();

            return PartialView(tuples);
        }

        public async Task<List<string>> PDeleteSelectedComfirmed(List<int> ids)
        {
            var pa = await _context.User.FirstAsync(x => x.UserName == User.Identity.Name);

            if (!string.IsNullOrEmpty(pa.QuestionSet))
            {
                var ps = pa.PackSet.Split(';', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

                foreach (var id in ids)
                {
                    if (ps.Remove(id.ToString()))
                    {
                        //smth
                    }
                }
                pa.PackSet = string.Join(';', ps) + ';';
                _context.Update(pa);
                await _context.SaveChangesAsync();

                return ids.Select(x => x.ToString()).ToList();
            }

            //return Json(new { success = false });
            return null;
        }

        public RedirectToActionResult QRedirectToTest(int id)
        {

            //TempData["question_id"] = id;
            HttpContext.Session.Remove("question_id");
            HttpContext.Session.Remove("question");
            HttpContext.Session.Remove("ReturnControllerName");

            HttpContext.Session.SetInt32("question_id", id);
            HttpContext.Session.SetString("ReturnControllerName", "UserLibrary");
            return RedirectToAction(nameof(Index), nameof(Tests));
        }


        [Authorize]
        public RedirectToActionResult PRedirectToTest(int id)
        {
            HttpContext.Session.Remove("pack_id");
            HttpContext.Session.Remove("pack");
            HttpContext.Session.Remove("ReturnControllerName");

            HttpContext.Session.SetInt32("pack_id", id);
            HttpContext.Session.SetString("ReturnControllerName", "UserLibrary");
            return RedirectToAction(nameof(Index), "PackTest");
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

        public async Task<IActionResult> ChangeQuestionPrivateSelected(List<int> ids, bool publish)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);
            var tuples = await _context.Question.AsNoTracking().Where(x => ids.Contains(x.Id) && qa.QuestionSet.Contains(x.Id.ToString()) && x.IsPrivate == publish).Select(x => new ClassForSelectedComfirmed(x.Title, x.Id)).ToListAsync();

            return PartialView(new ClassForChangePrivateSelectedConfirmed(tuples, publish));
        }

        public async Task<string> ChangeQuestionPrivateSelectedComfirmed(List<int> ids, bool publish)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);

            foreach(var id in ids)
            {
                if (qa.QuestionSet.Contains(id.ToString()))
                {
                    var temp = await _context.Question.FindAsync(id);

                    temp.IsPrivate = !publish;

                    _context.Update(temp);
                }
            }

            await _context.SaveChangesAsync();

            return "inDeveloping";
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


        public async Task<IActionResult> ChangePackPrivateSelected(List<int> ids, bool publish)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);
            var tuples = await _context.Pack.AsNoTracking().Where(x => ids.Contains(x.Id) && qa.PackSet.Contains(x.Id.ToString()) && x.IsPrivate == publish).Select(x => new ClassForSelectedComfirmed(x.Name, x.Id)).ToListAsync();

            return PartialView(new ClassForChangePrivateSelectedConfirmed(tuples, publish));
        }

       
        public async Task<string> ChangePackPrivateSelectedComfirmed(List<int> ids, bool publish)
        {
            var qa = await _context.User.AsNoTracking().FirstAsync(x => x.UserName == User.Identity.Name);

            foreach (var id in ids)
            {
                if (qa.PackSet.Contains(id.ToString()))
                {
                    var temp = await _context.Pack.FindAsync(id);

                    temp.IsPrivate = !publish;

                    _context.Update(temp);
                }
            }

            await _context.SaveChangesAsync();

            //это что за безобразие!
            return "inDeveloping";
        }


    }
}

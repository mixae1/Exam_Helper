using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Exam_Helper.ViewsModel;
using Exam_Helper.TestMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Exam_Helper.Controllers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }




    public class TestsController : Controller
    {

        private CommonDbContext _dbContext;

        public TestsController(CommonDbContext db)
        {
            _dbContext = db;
        }


        // GET: Tests 
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //  var ind=TempData["question_id"] as int?;
            Question temp = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session, "question");
            int? ind = 0;
            if (temp == null)
            {
                ind = HttpContext.Session.GetInt32("question_id");

                if (!ind.HasValue)
                    return RedirectToAction("Index", "PublicLibrary");
                temp = await _dbContext.Question.FindAsync(ind.Value);
                //TempData.Put("question", temp);
               SessionHelper.SetObjectAsJson(HttpContext.Session, "question", temp);
            } 
             

            var tests = await _dbContext.Tests.ToListAsync();
            var models = new TestChoiceViewModel()
            {
                TestMethodsNames = tests.Select(x => x.Name).ToArray(),
                TestsMethodsIds = tests.Select(x => x.Id).ToArray()
            };
            return View(models);
        }

        [HttpPost]
        public RedirectToActionResult Index([Bind("SelectedId, ServiceInfo")]TestChoiceViewModel temp)
        {   
            if (ModelState.IsValid)
            {
                switch (temp.SelectedId)
                {
                    case 1: return RedirectToAction(nameof(MissingWordsTest), new { Instruction = temp.ServiceInfo });
                    case 2: return RedirectToAction(nameof(PuzzleTest), new { Instruction = temp.ServiceInfo });
                    default: return RedirectToAction(nameof(Index));
                }
            }
            Console.WriteLine(ModelState.Values);
            return RedirectToAction(nameof(Index));
        }

        // для подключения к библиотеки question нужно сюда в параметры передать question
        [HttpGet]
        public IActionResult MissingWordsTest(string Instruction)
        {
             
            Question question = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session,"question");
            if (question==null) throw new Exception("question==null");
            
            TestMissedWords testMissed = new TestMissedWords(question.Definition, Instruction);
            TestInfoMissedWords ts = new TestInfoMissedWords()
            {
                Teorem = testMissed.GetWordsWithInputs(),
                Answers = testMissed.Answers,
                IsSuccessed = testMissed.IsSuccessed
            };

            return View(ts);
        }

        [HttpGet]
        public IActionResult PuzzleTest(string Instruction)
        {

            Question question = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session, "question");
            if (question == null) throw new Exception("question==null");

            TestPuzzle testPuzzle = new TestPuzzle(question.Definition, Instruction);
            TestInfoPuzzle ts = new TestInfoPuzzle()
            {
                TestStrings = testPuzzle.TestStrings,
                RightIndexes = testPuzzle.RightIndexes,
                IsSuccessed = testPuzzle.IsSuccessed
            };

            return View(ts);
        }

        /*
        [HttpPost]
        public string  CheckAnswerForMissingTest(string answers)
        {
            var jsdata = JsonSerializer.Deserialize<Dictionary<string,string>>(answers);
      
            List<bool> Is_Right = new List<bool>();
            foreach (var x in jsdata)
               Is_Right.Add(x.Key.Trim().GetHashCode() == int.Parse(x.Value));
           
            return JsonSerializer.Serialize(Is_Right);
            
        }
        */

        // GET: Tests/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tests/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tests/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tests/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

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
    
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonSerializer.Serialize(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object o);
            return o == null ? null : JsonSerializer.Deserialize<T>((string)o);
        }

        public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o = tempData.Peek(key);
            return o == null ? null : JsonSerializer.Deserialize<T>((string)o);
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
           var ind=TempData["question_id"] as int?;
            if (!ind.HasValue)
                return View();

            var temp = await _dbContext.Question.FindAsync(ind.Value);

            TempData.Put("question", temp);

            var tests = await _dbContext.Tests.ToListAsync();
            var models = new TestChoiceViewModel()
            {
                TestMethodsNames = tests.Select(x => x.Name).ToArray(),
                TestsMethodsIds = tests.Select(x => x.Id).ToArray()
            };
            return View(models);
        }

        [HttpPost]
        public RedirectToActionResult Index([Bind("SelectedId")]TestChoiceViewModel temp)
        {   
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(MissingWordsTest));

            }
            Console.WriteLine(ModelState.Values);
            return RedirectToAction(nameof(Index));
        }

        // для подключения к библиотеки question нужно сюда в параметры передать question
        [HttpGet]
        public IActionResult MissingWordsTest()
        {
            //передал значение так для теста , а вообще из question взять 
            Question question = TempData.Peek<Question>("question");
            if (question==null) throw new Exception("What a fuck");
            TestMissedWords testMissed = new TestMissedWords(question.Definition);
            TestInfoMissedWords ts = new TestInfoMissedWords() { Teorem=testMissed.GetTestString().ToArray()};
            ts.Check_Answers = testMissed.GetAnswersHashCodes();
            
            return View(ts);
        }

        [HttpPost]
        public string MissingWordsTest(TestInfoMissedWords tst)
        {
            string s = "";
            for (int i = 0; i < tst.Answer.Length; i++)
                if (tst.Answer[i].GetHashCode() == tst.Check_Answers[i])
                    s += (i + 1) + ") is correct";
                 else s+= (i + 1) + ") fuck you";
            return s;
        }
        [HttpPost]
        public string  CheckAnswerForMissingTest(string answers)
        {
            var jsdata = JsonSerializer.Deserialize<Dictionary<string,string>>(answers);
      
            List<bool> Is_Right = new List<bool>();
            foreach (var x in jsdata)
               Is_Right.Add(x.Key.GetHashCode() == int.Parse(x.Value));
           
            return JsonSerializer.Serialize(Is_Right);
            
        }


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

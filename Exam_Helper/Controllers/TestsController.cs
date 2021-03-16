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
            int? ind = HttpContext.Session.GetInt32("question_id");

            if (ind.HasValue)
            {
                Question temp = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session, "question");                   

                if(temp==null)
                temp = await _dbContext.Question.FindAsync(ind.Value);
              
               SessionHelper.SetObjectAsJson(HttpContext.Session, "question", temp);
            } 
            else
            return RedirectToAction("Index", "PublicLibrary");
             

            var tests = await _dbContext.Tests.ToListAsync();

            //лень было в БД заносить
            tests.Add(new Tests() { Name = "MultiTesting", Id = 3 });
            //полностью согласен
            tests.Add(new Tests() { Name = "wrong_text", Id = 4 });

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
                    case 3: return RedirectToAction(nameof(MultiTesting), new { Instruction = temp.ServiceInfo });
                    case 4: return RedirectToAction(nameof(TheWrongTextTest), new { Instruction = temp.ServiceInfo });
                    default:return RedirectToAction(nameof(Index));
                }
            }
            Console.WriteLine(ModelState.Values);
            return RedirectToAction(nameof(Index));
        }

        // для подключения к библиотеки question нужно сюда в параметры передать question
        [HttpGet]
        public IActionResult MissingWordsTest(string Instruction, bool isMulti=false)
        {
             
            Question question = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session,"question");
            if (question==null) throw new Exception("question==null");
            
            TestMissedWords testMissed = new TestMissedWords(question.Definition, Instruction);
            TestInfoMissedWords ts = new TestInfoMissedWords()
            {
                Title = question.Title,
                Teorem = testMissed.GetWordsWithInputs(),
                Answers = testMissed.Answers,
                IsSuccessed = testMissed.IsSuccessed,
                TestInstructions=Instruction,
                isMulti=isMulti
            };

            return View(ts);
        }

        [HttpGet]
        public IActionResult PuzzleTest(string Instruction, bool isMulti=false)
        {

            Question question = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session, "question");
            if (question == null) throw new Exception("question==null");

            TestPuzzle testPuzzle = new TestPuzzle(question.Definition, Instruction);
            TestInfoPuzzle ts = new TestInfoPuzzle()
            {
                Title = question.Title,
                TestStrings = testPuzzle.TestStrings,
                RightIndexes = testPuzzle.RightIndexes,
                IsSuccessed = testPuzzle.IsSuccessed,
                TestInstructions=Instruction,
                isMulti=isMulti
            };

            return View(ts);
        }

        [HttpGet]
        public IActionResult TheWrongTextTest(string Instruction, bool isMulti = false)
        {

            Question question = SessionHelper.GetObjectFromJson<Question>(HttpContext.Session, "question");
            if (question == null) throw new Exception("question==null");

            TestTheWrongText testTWT = new TestTheWrongText(question.Definition, Instruction);
            TestInfoTheWrongText ts = new TestInfoTheWrongText()
            {
                Title = question.Title,
                Text = testTWT.htmlText,
                IsSuccessed = testTWT.IsSuccessed,
                TestInstructions = Instruction,
                isMulti = isMulti
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

        [HttpGet]
        public RedirectToActionResult MultiTesting(string Instruction)
        {
            /* юзер выбирает этот метод ,указывая настройки : минимум 1 тест с каждого вида тестирования
             * instruction - имеет вид "d;d" d- число [1,5]. При первом запуске в данных сессии нет ключа "test_times"
             * поэтому настройки ( кол-во тестов ) будут распарсены из Instruction . Далее выбирем случайное число и перенаправляем
             * в нужный метод , указывая метод тестирования одиночный или из мультитестирования. При этом в данных сессии уже есть 
             * нужный ключ , поэтому когда идет очередной вызов этого метода , настройки достаются из сессии , и че там лежит в Instruction
             * - нам без разницы
             * 
             * В дальнейшем структура Instruction изментится - появится возможность указывать настройки ддя методов тестирования
             
             */
            if (string.IsNullOrEmpty(Instruction)) Instruction = "1;1";

            var times = SessionHelper.GetObjectFromJson<int[]>(HttpContext.Session, "test_times");
            if(times==null) times= Instruction.Split(';').Select(x => int.Parse(x)).ToArray();



            if (times[0] > 0 || times[1] > 0)
            {
                Random r = new Random();
                int nextTestMethod = r.Next(0, times.Length);

                while(times[nextTestMethod]<=0)
                  nextTestMethod = r.Next(0, times.Length );

                times[nextTestMethod]--;
                SessionHelper.SetObjectAsJson(HttpContext.Session, "test_times", times);

                switch (nextTestMethod) 
                {
                    case 1:return RedirectToAction(nameof(MissingWordsTest),new { isMulti=true});
                    case 0:return RedirectToAction(nameof(PuzzleTest), new { isMulti = true });
                }

            }


            HttpContext.Session.Remove("test_times");
            return RedirectToAction(nameof(UserStats));
          
        }

        public IActionResult UserStats()
        {
            return View();
        }
     
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Exam_Helper.ViewsModel;
using Exam_Helper.TestMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json.Serialization;
using System.Text.Json;



namespace Exam_Helper.Controllers
{

    /// <summary>
    /// инкапсулируем методы для работы с сессией 
    /// </summary>
    public interface ISessionWorker
    {
        /// <summary>
        /// удаляет ненужные ключи и данные из данных сессии.
        /// Необходимо чтобы не было коллизии
        /// </summary>
        /// <param name="Keys"></param>
        public void RemoveDataSession(params string[] Keys);

        /// <summary>
        /// получение пака из сессии.Если такого ключа нет , выбрасывается исключение : "problem with pack in session data"
        /// В дальнешем можно перебрасывать на индекс с уведомлением об ошибке 
        /// </summary>
        /// <param name="pack"></param>
        public void GetPack(string key, out Pack pack);

        /// <summary>
        /// получение вопроса из сессии.Если такого ключа нет , выбрасывается исключение : "problem with question in session data"
        /// В дальнешем можно перебрасывать на индекс с уведомлением об ошибке 
        /// </summary>
        /// <param name="pack"></param>

        public void GetQuestion(string key, out Question question);

    }

    public class SessionWorker : ISessionWorker
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionWorker(IHttpContextAccessor httpContext)
        {
            _httpContextAccessor = httpContext;
        }

        public void RemoveDataSession(params string[] Keys)
        {
            foreach (var key in Keys)
            {
                if (!string.IsNullOrEmpty(key))
                    _httpContextAccessor.HttpContext.Session.Remove(key);
            }
        }

        public void GetPack(string key, out Pack pack)
        {
            pack = SessionHelper.GetObjectFromJson<Pack>(_httpContextAccessor.HttpContext.Session, key);
            if (pack == null) throw new Exception("problem with pack in  session data");
        }

        public void GetQuestion(string key, out Question question)
        {
            question = SessionHelper.GetObjectFromJson<Question>(_httpContextAccessor.HttpContext.Session, "question");
            if (question == null) throw new Exception("problem with question  in  session data");

        }

    }



    public class PackTestController : Controller
    {

        private CommonDbContext _dbContext;
        private ISessionWorker _sessionWorker;

        public PackTestController(CommonDbContext db, ISessionWorker session)
        {
            _dbContext = db;
            _sessionWorker = session;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? ind = HttpContext.Session.GetInt32("pack_id");

            if (ind.HasValue)
            {
                Pack temp = SessionHelper.GetObjectFromJson<Pack>(HttpContext.Session, "pack");

                if (temp == null)
                    temp = await _dbContext.Pack.FindAsync(ind.Value);

                SessionHelper.SetObjectAsJson(HttpContext.Session, "pack", temp);
            }
            else
                return RedirectToAction("Index", "PublicLibrary");



            var models = new TestChoiceViewModel()
            {
                TestMethodsNames = new string[] { "NameAndDesc", "TestConstructor", "Dummy" },
                TestsMethodsIds = new int[] { 1, 2, 3 }
            };
            return View(models);
        }


        [HttpPost]

        public RedirectToActionResult Index([Bind("SelectedId, ServiceInfo")] TestChoiceViewModel temp)
        {
            if (ModelState.IsValid)
            {
                switch (temp.SelectedId)
                {
                    case 1: return RedirectToAction(nameof(TestNamesAndDescription));
                    case 2: return RedirectToAction(nameof(MultiTesting), new { Instruction = temp.ServiceInfo });
                    default: return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> TestNamesAndDescription(string Instruction)
        {

            _sessionWorker.GetPack("pack", out Pack pack);

            //отслеживать производительность
            var ques = await _dbContext.Question.Where(x => pack.QuestionSet.Contains(x.Id.ToString())).Select(x => new HelpStruct(x.Definition, x.Title)).ToListAsync();

            TestNamesAndDescription testNamesAndDescription = new TestNamesAndDescription(ques);
            TestInfoNamesAndDescription ts = new TestInfoNamesAndDescription()
            {
                Names = testNamesAndDescription.FinalNames,
                Description = testNamesAndDescription.Description,
                AnswerID = testNamesAndDescription.AnswerId,
                Instruction = Instruction
            };
            return View(ts);
        }

        /*
         * Первая версия с простыми настройками , без рефакторинга 
         * 
         */
        public async Task<RedirectToActionResult> MultiTesting(string Instruction)
        {
            _sessionWorker.GetPack("pack", out Pack pack);

            var PackQuestionsIds = pack.QuestionSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            Random r = new Random();
            int TestQuestionId = r.Next(0, PackQuestionsIds.Length);

            if (string.IsNullOrEmpty(Instruction))
                Instruction = "3";


            int? times;
            times = HttpContext.Session.GetInt32("PackTestingTimes");

            if (times == null)
                times = int.Parse(Instruction);

            if (times.Value > 0)
            {

                var question = await _dbContext.Question.FindAsync(int.Parse(PackQuestionsIds[TestQuestionId]));
                SessionHelper.SetObjectAsJson(HttpContext.Session, "question", question);

                times--;
                HttpContext.Session.SetInt32("PackTestingTimes", times.Value);

                int TestMethodId = r.Next(0, 1);

                switch (TestMethodId)
                {
                    case 1: return RedirectToAction(nameof(TestsController.MissingWordsTest), nameof(Tests), new { ControllerName = "PackTest", isMulti = true });
                    case 0: return RedirectToAction(nameof(TestsController.PuzzleTest), nameof(Tests), new { ControllerName = "PackTest", isMulti = true });
                }

            }
            _sessionWorker.RemoveDataSession("question", "PackTestingTimes");

            return RedirectToAction(nameof(TestsController.UserStats), nameof(Tests));

        }


    }


}

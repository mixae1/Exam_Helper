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
    public class PackTestController : Controller
    {

        private CommonDbContext _dbContext;

        public PackTestController(CommonDbContext db)
        {
            _dbContext = db;
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
                TestMethodsNames = new string[] { "сопоставить название и определение(Тест) ", "сопоставить название и док-во(Тест)", "Методы тестирования/заучивания" },
                TestsMethodsIds =new int[] { 1,2,3}
            };
            return View(models);
        }


        [HttpPost]
        public RedirectToActionResult Index([Bind("SelectedId")] TestChoiceViewModel temp)
        {
            if (ModelState.IsValid)
            {
                switch (temp.SelectedId)
                {
                    case 1: return RedirectToAction(nameof(TestNamesAndDescription));
                    default: return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> TestNamesAndDescription(string Instruction)
        {

            Pack pack = SessionHelper.GetObjectFromJson<Pack>(HttpContext.Session, "pack");
            if (pack == null) throw new Exception("problem with session data");

            var ques = await _dbContext.Question.Where(x => pack.QuestionSet.Contains(x.Id.ToString())).Select(x => new HelpStruct(x.Definition, x.Title)).ToListAsync();

            TestNamesAndDescription testNamesAndDescription = new TestNamesAndDescription(ques);
            TestInfoNamesAndDescription ts = new TestInfoNamesAndDescription()
            {
                Names = testNamesAndDescription.FinalNames,
                Description = testNamesAndDescription.Description,
                AnswerID = testNamesAndDescription.AnswerId
            };
            return View(ts);
        }

    }
}

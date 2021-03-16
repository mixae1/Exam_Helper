﻿using Microsoft.AspNetCore.Mvc;
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
                TestMethodsNames = new string[] { "сопоставить название и определение(Тест) ", "Конструктор тестов для паков", "Методы тестирования/заучивания" },
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
                    case 2: return RedirectToAction(nameof(MultiTesting),new {Instruction=temp.ServiceInfo});
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

            //отслеживать производительность
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

        /*
         * Первая версия без настроек , без рефакторинга 
         * 
         */
        public async Task<RedirectToActionResult> MultiTesting(string Instruction)
        {
            Pack pack = SessionHelper.GetObjectFromJson<Pack>(HttpContext.Session, "pack");
            if (pack == null) throw new Exception("problem with session data");

            var PackQuestionsIds = pack.QuestionSet.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            Random r = new Random();
            int TestQuestionId = r.Next(0, PackQuestionsIds.Length);

            if (string.IsNullOrEmpty(Instruction))
                Instruction = "3";

            int? times;
            times = HttpContext.Session.GetInt32("PackTestingTimes");

            if(times==null)
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
                   case 1:return RedirectToAction(nameof(TestsController.MissingWordsTest),nameof(Tests), new {ControllerName = "PackTest", isMulti = true });
                  case 0: return RedirectToAction(nameof(TestsController.PuzzleTest),nameof(Tests) ,new { ControllerName = "PackTest", isMulti = true });
                }

            }

            HttpContext.Session.Remove("question");
            HttpContext.Session.Remove("PackTestingTimes");

            return RedirectToAction(nameof(TestsController.UserStats), nameof(Tests));

        }


    }
}

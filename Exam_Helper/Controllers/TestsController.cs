using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Exam_Helper.ViewsModel;
using Exam_Helper.TestMethods;

namespace Exam_Helper.Controllers
{
    public class TestsController : Controller
    {
        // GET: Tests
        // для подключения к библиотеки question нужно сюда в параметры передать question
        [HttpGet]
        public IActionResult Index()
        {
            //передал значение так для теста , а вообще из question взять 
            TestMissedWords testMissed = new TestMissedWords("Если функция монотонна и непрерывна на некотором отрезке и на концах этого отрезка принимает значения разных знаков, то существует точка, в которой значение функции равно нулю");
            TestInfoMissedWords ts = new TestInfoMissedWords() { Teorem=testMissed.GetTestString().ToArray()};
            ts.Check_Answers = testMissed.GetAnswersHashCodes();
            
            return View(ts);
        }

        [HttpPost]
        public string Index(TestInfoMissedWords tst)
        {
            string s = "";
            for (int i = 0; i < tst.Answer.Length; i++)
                if (tst.Answer[i].GetHashCode() == tst.Check_Answers[i])
                    s += (i + 1) + ") is correct";
                 else s+= (i + 1) + ") fuck you";
            return s;
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

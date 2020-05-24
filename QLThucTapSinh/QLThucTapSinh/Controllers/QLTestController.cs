using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class QLTestController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();

        // GET: QLTest
        public ActionResult Index1(int id)
        {
            var model = database.InternShip.Find(id);
            return View(model);
        }

        public ActionResult Index()
        {
            string personID = Session["Person"].ToString();
            var find = database.Intern.Find(personID);
            // Chưa tính trường hợp Intership đã bị khóas
            var statu = database.InternShip.Find(find.InternshipID).Status;
            if (statu)
            {
                Session["Result"] = find.Result;
                var list = database.IntershipWithTask.Where(x => x.InternshipID == find.InternshipID).OrderBy(x => x.SORT).ToList();
                List<Task> model = new List<Task>();
                foreach (var item in list)
                {
                    var model1 = database.Task.Find(item.TaskID);
                    model.Add(model1);
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index1", new { id = find.InternshipID });
            }
            
        }

        public ActionResult Question(long? taskid = null)
        {
            string taikhoan = Session["TenTK"].ToString();
            var listQuestion = database.Question.Where(x => x.TaskID == taskid).ToList();
            int count = listQuestion.Count - 5;
            for(int i = 0; i < count; i++)
            {
                Random rd = new Random();
                int j = rd.Next(count);
                listQuestion.RemoveAt(j);
            }
            return PartialView(listQuestion);
        }

        public bool UpdateAnswer(string id, int answer, int task)
        {
            try
            {
                TestResults testResults = new TestResults();
                testResults.ID = database.TestResults.Count() + 1;
                testResults.PersonID = id;
                testResults.TaskID = task;
                testResults.Answer = answer;
                database.TestResults.Add(testResults);
                var model = database.Intern.Find(id);
                model.Result = model.Result + 1;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
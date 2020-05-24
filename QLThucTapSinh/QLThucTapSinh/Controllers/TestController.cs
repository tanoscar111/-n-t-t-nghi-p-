using PagedList;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class TestController : Controller
    {

        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        public ActionResult Index( string search,int page = 1, int pageSize = 5)
        {
            IQueryable<Task> model = database.Task.Where(x=>x.PersonID == "ZXCKBHML");
            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(x=>x.TaskName.Contains(search)||x.Note.Contains(search));
            }

            var model1 = model.OrderBy(x => x.TaskID).ToPagedList(page, pageSize);
            ViewBag.ChuoiTimKiem = search;
            return View(model1);
        }


        public ActionResult Testing(string search = "")
      {
            var model = database.Task.Where(x => x.TaskName.Contains(search)).OrderBy(x => x.TaskID).ToPagedList(1, 5);
            var count = model.Count();
            return PartialView(model);
        }

    }
}
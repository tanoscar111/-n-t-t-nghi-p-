using QLThucTapSinh.Common;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class StatisticController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        // GET: Statistic
        public ActionResult Index()
        {
            Statistic model = new Statistic();
            model.countOre = database.Organization.Count();
            model.countInteship = database.InternShip.Count();
            model.countIntern = database.Intern.Count();
            model.countleader = database.Person.Where(x => x.RoleID == 4).Count();
            return View(model);
        }

        [HttpPost]
        public ActionResult Jsonstatis()
        {
            List<int> list = new List<int>();
            list.Add(database.Person.Where(x => x.RoleID == 2).Count());
            list.Add(database.Person.Where(x => x.RoleID == 3).Count());
            list.Add(database.Person.Where(x => x.RoleID == 5).Count());
            list.Add(database.InternShip.Count());

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
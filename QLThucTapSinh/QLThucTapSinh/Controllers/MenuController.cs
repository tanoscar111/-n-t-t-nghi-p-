using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            var role = Convert.ToInt32(Session["Role"].ToString());
            SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
            var model = database.Menu.Where(x => x.RoleID == role).ToList();
            return PartialView(model);
        }
    }
}
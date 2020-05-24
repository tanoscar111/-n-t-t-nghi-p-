using QLThucTapSinh.Common;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class QLPersonController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TTcanhan1()
        {
            var userName = Session["TenTk"].ToString();
            var findU = database.Users.Find(userName);
            var model = database.Person.Find(findU.PersonID);
            SetViewBagG();
            return View(model);
        }

        [HttpPost]
        public ActionResult TTcanhan1(string Password, string Newpassword)
        {
            var userName = Session["TenTk"].ToString();
            var model = database.Users.Find(userName);
            if (model.PassWord != MaHoaMd5.MD5Hash(Password))
            {
                ModelState.AddModelError("", "Mật khẩu không đúng.");

            }
            else
            {
                HomeController home = new HomeController();
                home.UpdateUser(model.PersonID, Newpassword);
            }
            SetViewBagG();
            var findP = database.Person.Find(model.PersonID);
            return View("TTcanhan1",findP);
        }

        [HttpPost]
        public ActionResult UpdatePerson(Person person)
        {
            var userName = Session["TenTk"].ToString();
            var model = database.Users.Find(userName);
            new Share().UpdatePerson(person);
            SetViewBagG();
            var findP = database.Person.Find(model.PersonID);
            return View("TTcanhan1", findP);
        }
        public void SetViewBag(string selectedID = null)
        {
            var list = database.Person.Where(x => x.RoleID == 2).ToList();
            List<Organization> Organ = new List<Organization>();
            foreach(var item in list)
            {
                var model = database.Organization.Find(item.CompanyID);
                Organ.Add(model);
            }
            SelectList OrganList = new SelectList(Organ, "ID", "Name");
            ViewBag.OrganList = OrganList;
        }

        public void SetViewBagS(string selectedID = null)
        {
            var list = database.Person.Where(x => x.RoleID == 3).ToList();
            List<Organization> Organ = new List<Organization>();
            foreach (var item in list)
            {
                var model = database.Organization.Find(item.SchoolID);
                Organ.Add(model);
            }
            SelectList SchoolList = new SelectList(Organ, "ID", "Name");
            ViewBag.SchoolList = SchoolList;
        }

        public void SetViewBagG(string selectedID = null)
        {
            SelectList GenGender = new SelectList( new[] {
                new {Text = "Nam", Value = true},
                new {Text = "Nữ", Value = false},
            }, "Value", "Text");
            ViewBag.GenGender = GenGender;
        }
        public ActionResult Testing(string personID = "ZXCDBUNQ")
        {
            var model = database.Person.Find(personID);
            SetViewBag();
            SetViewBagS();
            SetViewBagG();
            return View(model);
        }


    }
}
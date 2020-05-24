using PagedList;
using QLThucTapSinh.Common;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class QLSchoolController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();

        // GET: QLSchool
        public ActionResult Index()
        {
            var modle = new Share().listOrgan(3).OrderByDescending(x => x.StartDay).ToList();
            SetViewBagM();
            return View(modle);
        }

        public void SetViewBagM(string selectedID = null)
        {
            SelectList Month = new SelectList(new[] {
                new {Text = "Một Tháng", Value = 1},
                new {Text = "Ba Tháng", Value = 3},
                new {Text = "Sáu Tháng", Value = 6},
                new {Text = "Một Năm", Value = 12},
            }, "Value", "Text");
            ViewBag.Month = Month;
        }

        [HttpPost]
        public JsonResult ChangeStatus(string id)
        {
            var res = new Share().ChangeStatus(id, 3);
            return Json(new
            {
                status = res
            });
        }

        public bool extension(string id, int val)
        {
            try
            {
                var model = database.Organization.Find(id);
                if (model.Status == true)
                {
                    model.ExpiryDate += +val;
                }
                else
                {
                    model.StartDay = DateTime.Now;
                    model.ExpiryDate = val;
                    model.SendEmail = false;
                    new Share().ChangeStatus(id, 3);
                }
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                CompanyAndSchool organ = new CompanyAndSchool();
                if (organ.Create(organization, 3))
                {

                    var modle = new Share().listOrgan(3).OrderByDescending(x => x.StartDay).ToList();
                    var comid = modle[0].ID;
                    var find = database.Person.SingleOrDefault(x => x.SchoolID == comid);
                    SendMailTK(find.PersonID);
                    return RedirectToAction("Index", modle);
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Công ty thất bại");
                }
            }
            return View("Create");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = database.Organization.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Organization organi)
        {
            if (ModelState.IsValid)
            {

                var model1 = new CompanyAndSchool().Update(organi);
                if (model1)
                {
                    var modle = new Share().listOrgan(3).OrderByDescending(x => x.StartDay).ToList();
                    return RedirectToAction("Index", modle);
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại");
                }
            }
            return View("Edit");
        }

        public ActionResult Delete(string id)
        {
            var find = database.Person.SingleOrDefault(x => x.SchoolID == id && x.RoleID == 3);
            if (database.Users.SingleOrDefault(x => x.PersonID == find.PersonID) != null)
            {
                database.Users.Remove(database.Users.SingleOrDefault(x => x.PersonID == find.PersonID));
            }
            database.Person.Remove(find);
            var model = database.Organization.Find(id);
            database.Organization.Remove(model);
            database.SaveChanges();
            var model1 = database.Organization.OrderByDescending(x => x.StartDay).ToList();
            return RedirectToAction("Index", model1);
        }

        public bool SendMailTK(string personID)
        {
            try
            {
                var res = database.Person.Find(personID);
                string cus = res.LastName + " " + res.FirstName;
                var com = database.Organization.Find(res.SchoolID);
                string compa = com.Name;
                string email = res.Email;
                string nd = personID;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/TaiKhoan.html"));
                content = content.Replace("{{CustomerName}}", cus);
                content = content.Replace("{{CompanyName}}", compa);
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Thông báo Đăng ký Tài khoản";
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient client = new SmtpClient(smtpHost, 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                client.UseDefaultCredentials = false;
                client.Credentials = nc;
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
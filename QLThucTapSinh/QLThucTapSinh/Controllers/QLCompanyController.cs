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
    public class QLCompanyController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        // GET: QLCompany
        public ActionResult Index()
        {
            var modle = new Share().listOrgan(2).OrderByDescending(x=>x.StartDay).ToList();
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
            var res = new Share().ChangeStatus(id, 2);
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
                if(model.Status == true)
                {
                    model.ExpiryDate += +val;
                }
                else
                {
                    model.StartDay = DateTime.Now;
                    model.ExpiryDate = val;
                    model.SendEmail = false;
                     new Share().ChangeStatus(id, 2);
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
                if (organ.Create(organization, 2)){
                    
                    var modle = new Share().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
                    var comid = modle[0].ID;
                    var find = database.Person.SingleOrDefault(x => x.CompanyID == comid);
                    SendMailTK(find.PersonID);
                    return RedirectToAction("Index",modle);
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
            var model = database.Organization.SingleOrDefault(x => x.ID == id);
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
                    var modle = new Share().listOrgan(2).OrderByDescending(x => x.StartDay).ToList();
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
            if (DeleteIntern(id) == true)
            {
                if (DeleteQuestion(id) == true)
                {
                    if (DeleteTask(id) == true)
                    {
                        if (DeleteInternship(id) == true)
                        {
                            if (DeleteLedder(id) == true)
                            {
                                database.Person.Remove(database.Person.SingleOrDefault(x=>x.CompanyID == id && x.RoleID == 2));
                                var model = database.Organization.Find(id);
                                database.Organization.Remove(model);
                                database.SaveChanges();
                            }
                        }
                    }
                }
            }
            var model1 = database.Organization.OrderByDescending(x => x.StartDay).ToList();
            return RedirectToAction("Index", model1);
        }

        public bool DeleteIntern(string id)
        {
            try
            {
                var listIntern = database.Person.Where(x => x.CompanyID == id && x.RoleID == 5).ToList();
                foreach (var item in listIntern)
                {
                    
                    database.Intern.Remove(database.Intern.Find(item.PersonID));
                    database.Users.Remove(database.Users.SingleOrDefault(x => x.PersonID == item.PersonID));
                    database.Person.Remove(item);
                    database.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteQuestion(string id)
        {
            try

            {
                var ListQuestion = (from a in database.Person
                                    join b in database.Task on a.PersonID equals b.PersonID
                                    join c in database.Question on b.TaskID equals c.TaskID
                                    where a.CompanyID == id
                                    select new QuestionClass()
                                    {
                                        QuestionID = c.QuestionID
                                    }).ToList();
                foreach (var item in ListQuestion)
                {
                    database.Question.Remove(database.Question.Find(item.QuestionID));
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteTask(string id)
        {
            try
            {
                var ListTask = (from a in database.Person
                                join b in database.Organization on a.CompanyID equals b.ID
                                join c in database.Task on a.PersonID equals c.PersonID
                                where a.CompanyID == id
                                select new TaskClass()
                                {
                                    TaskID = c.TaskID
                                }).ToList();
                foreach (var item in ListTask)
                {
                    database.Task.Remove(database.Task.Find(item.TaskID));
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteInternship(string id)
        {
            try
            {
                var ListInternship = database.InternShip.Where(x=>x.CompanyID == id).ToList();
                foreach (var item in ListInternship)
                {
                    database.InternShip.Remove(item);
                    var ListInternship2 = database.IntershipWithTask.Where(x => x.InternshipID == item.InternshipID).ToList();
                    foreach (var item2 in ListInternship2)
                    {
                        database.IntershipWithTask.Remove(item2);
                        database.SaveChanges();
                    }
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool DeleteLedder(string id)
        {
            try
            {
                var listLedder = database.Person.Where(x => x.CompanyID == id && x.RoleID == 4).ToList();
                foreach (var item in listLedder)
                {
                    database.Users.Remove(database.Users.SingleOrDefault(x => x.PersonID == item.PersonID));
                    database.Person.Remove(item);
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool SendMailTK(string personID)
        {
            try
            {
                var res = database.Person.Find(personID);
                string cus = res.LastName + " " + res.FirstName;
                var com = database.Organization.Find(res.CompanyID);
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
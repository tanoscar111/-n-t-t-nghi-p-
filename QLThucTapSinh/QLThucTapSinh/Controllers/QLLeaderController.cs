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
    public class QLLeaderController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();

        // GET: QLLeader
        public ActionResult Index(string id = null)
        {
            var company = Session["CompanyID"].ToString();
            var model = (from a in database.Person
                         join b in database.Users on a.PersonID equals b.PersonID
                         where a.CompanyID == company && a.RoleID == 4
                         select new LeaderClass()
                         {
                             PersonID = a.PersonID,
                             LastName = a.LastName,
                             FirstName = a.FirstName,
                             Address = a.Address,
                             Phone = a.Phone,
                             Email = a.Email,
                             Status = b.Status,
                         }).OrderByDescending(x => x.PersonID).ToList();
            if (id == null)
            {
                SetViewBagI(model[0].PersonID);
            }
            else
            {
                SetViewBagI(id);
            }

            return View(model);
        }

        public void SetViewBagI(string selectedID)
        {
            var list = database.InternShip.Where(x => x.PersonID == selectedID).ToList();
            ViewBag.IList = list;
        }

        public List<Internshipclass> listInternship(string selectedID)
        {
            var model = (from a in database.InternShip
                         join c in database.Person on a.PersonID equals c.PersonID
                         join b in database.Organization on a.CompanyID equals b.ID
                         where a.PersonID == selectedID
                         select new Internshipclass()
                         {
                             InternshipID = a.InternshipID,
                             CourseName = a.CourseName,
                             PersonID = c.PersonID,
                             FullName = c.LastName + " " + c.FirstName,
                             CompanyID = a.CompanyID,
                             Name = b.Name,
                             StartDay = a.StartDay,


                         }).OrderByDescending(x => x.StartDay).ToList();
            return model;
        }

        public bool ChangeStatusLeader(string id)
        {
            var com = database.Users.SingleOrDefault(x => x.PersonID == id);
            if (com.Status == true)
            {
                com.Status = false;
                ChangeStatusInternShip(id, false);
            }
            else
            {
                com.Status = true;
                ChangeStatusInternShip(id, true);

            }
            database.SaveChanges();
            return com.Status;

        }

        public void ChangeStatusInternShip(string id, bool status)
        {
            var list = new List<InternShip>();
            var date = DateTime.Now;
            if (status == false)
            {
                list = database.InternShip.Where(x => x.PersonID == id && x.Status == true).ToList();
                foreach (var item in list)
                {
                    item.Status = false;
                }
                database.SaveChanges();
            }
            else
            {
                list = database.InternShip.Where(x => x.PersonID == id).ToList();
                foreach (var item in list)
                {
                    if (item.StartDay > date)
                    {
                        item.Status = false;
                    }
                    else if (item.StartDay.AddMonths(item.ExpiryDate) > date)
                    {
                        item.Status = true;
                    }
                }
                database.SaveChanges();
            }
        }

        [HttpPost]
        public JsonResult ChangeStatus(string id)
        {
            var res = ChangeStatusLeader(id);
            return Json(new
            {
                status = res
            });
        }

        public bool updateTask(string id, string idnew)
        {
            try
            {
                var listQ = database.Task.Where(x => x.PersonID == id);
                foreach (var item in listQ)
                {
                    database.Task.Find(item.TaskID).PersonID = idnew;
                }
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool updateInternShip(string id, string idnew)
        {
            try
            {
                var list = database.InternShip.Where(x => x.PersonID == id).ToList();

                foreach (var item in list)
                {
                    database.InternShip.Find(item.InternshipID).PersonID = idnew;
                    database.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetViewBagL(string selectedID = null)
        {
            var model = Session["CompanyID"].ToString();
            var list = database.Person.Where(x => x.CompanyID == model && x.RoleID == 4).ToList();
            var listLeader = new List<LeaderClass>();
            foreach (var item in list)
            {
                LeaderClass leader = new LeaderClass();
                leader.FullName = item.LastName + " " + item.FirstName;
                leader.PersonID = item.PersonID;
                listLeader.Add(leader);
            }
            SelectList ListLeader = new SelectList(listLeader, "PersonID", "FullName");
            ViewBag.ListLeader = ListLeader;
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            SetViewBagL();
            var model = database.Person.Find(id);
            LeaderClass leader = new LeaderClass();
            leader.PersonID = model.PersonID;
            leader.FullName = model.LastName + " " + model.FirstName;
            return View(leader);
        }

        [HttpPost]
        public ActionResult Delete(LeaderClass leader)
        {
            if (updateTask(leader.PersonID, leader.NewPersonID))
            {
                if (updateInternShip(leader.PersonID, leader.NewPersonID))
                {
                    database.Users.Remove(database.Users.SingleOrDefault(x => x.PersonID == leader.PersonID));
                    database.Person.Remove(database.Person.Find(leader.PersonID));
                    database.SaveChanges();
                    ModelState.AddModelError("", "Chuyển giao Thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Chuyển giao Thất bại");
                }
            }
            else
            {
                ModelState.AddModelError("", "Chuyển giao Thất bại");
            }
            SetViewBagL();

            return View("Delete");
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBagG();
            return View();
        }

        public void SetViewBagG(string selectedID = null)
        {
            SelectList GenGender = new SelectList(new[] {
                new {Text = "Nam", Value = true},
                new {Text = "Nữ", Value = false},
            }, "Value", "Text");
            ViewBag.GenGender = GenGender;
        }

        [HttpPost]
        public ActionResult Create(Person ledder)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                string personID;
                do
                {
                    personID = new Share().RandomText();
                } while (new CompanyAndSchool().FindPerson(personID) == false);
                person.PersonID = personID;
                person.RoleID = 4;
                person.LastName = ledder.LastName;
                person.FirstName = ledder.FirstName;
                person.Birthday = ledder.Birthday;
                person.Gender = ledder.Gender;
                person.Address = ledder.Address;
                person.Phone = ledder.Phone;
                person.Email = ledder.Email;
                person.CompanyID = Session["CompanyID"].ToString();
                InsertPer(person);
                if (SendMailTK(personID))
                {
                    ModelState.AddModelError("", "Thêm Quản lý thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Quản lý thất bại");
                }
                SetViewBagG();
            }
            return View("Create");
        }

        public void InsertPer(Person person)
        {
            database.Person.Add(person);
            database.SaveChanges();
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = database.Person.Find(id);
            SetViewBagG();
            return View(model);
        }

        public bool Update(Person ledder)
        {
            try
            {
                var model = database.Person.Find(ledder.PersonID);
                model.LastName = ledder.LastName;
                model.FirstName = ledder.FirstName;
                model.Birthday = ledder.Birthday;
                model.Gender = ledder.Gender;
                model.Address = ledder.Address;
                model.Phone = ledder.Phone;
                model.Email = ledder.Email;
                model.Image = ledder.Image;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult Edit(Person ledder)
        {
            if (ModelState.IsValid)
            {
                var model1 = Update(ledder);
                if (model1)
                {
                    ModelState.AddModelError("", "Cập nhật thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại");
                }
            }
            SetViewBagG();
            return View("Edit");
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
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/TaiKhoan1.html"));
                //content = content.Replace("{{CustomerName}}", cus);
                content = content.Replace("{{CompanyName}}", compa);
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Thông báo đăng ký Tài khoản";
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
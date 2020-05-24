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
    public class HomeController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        public ActionResult Index()
        {
            ViewBag.ListCom = new Share().listOrgan(2).ToList();
            ViewBag.ListSchool = new Share().listOrgan(3).ToList();
            return View();
        }

        public ActionResult Login(Users user)
        {
            if (ModelState.IsValid)
            {
                var res = database.Users.SingleOrDefault(x => x.UserName == user.UserName);
                if (res == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else
                {
                    if (res.Status == false)
                    {
                        ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                    }
                    else
                    {
                        if (res.PassWord != MaHoaMd5.MD5Hash(user.PassWord))
                        {
                            ModelState.AddModelError("", "Mật khẩu không đúng.");
                            
                        }
                        else
                        {
                            Session["TenTk"] = user.UserName;
                            var role = database.Person.SingleOrDefault(x => x.PersonID == res.PersonID);
                            Session["Role"] = role.RoleID;
                            
                            // Admin = 1
                            if (role.RoleID == 1)
                            {
                                return RedirectToAction("TTcanhan1", "QLPerson");
                            }
                            else
                            {
                                Session["Person"] = role.PersonID;
                                // Interns = 5
                                if (role.RoleID == 5)
                                {
                                    return RedirectToAction("TTcanhan1", "QLPerson");
                                }
                                else
                                {
                                    
                                    if (role.RoleID == 3)
                                    {
                                        Session["SchoolID"]= role.SchoolID;
                                        return RedirectToAction("TTcanhan1", "QLPerson");
                                    }
                                    else
                                    {
                                        Session["CompanyID"] = role.CompanyID;
                                        if (role.RoleID == 2)
                                        {
                                            return RedirectToAction("TTcanhan1", "QLPerson");
                                        }
                                        else
                                        {
                                            return RedirectToAction("TTcanhan1", "QLPerson");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.ListCom = new Share().listOrgan(2).ToList();
            ViewBag.ListSchool = new Share().listOrgan(3).ToList();
            return View("Index");
            //return RedirectToAction("Index", "Home");
        }

        public ActionResult DangXuat()
        {
            Session["TenTk"] = null;
            Session["Role"] = null;
            Session["Person"] = null;
            Session["SchoolID"] = null;
            Session["CompanyID"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult PasswordRetrieval()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordRetrieval(RestartPassword restart)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra Object User có tồn tại hay không
                var model = database.Users.Find(restart.UserName);
                if (model == null)
                {
                    ModelState.AddModelError("", "UserName không tồn tại");
                }
                else
                {
                   
                    var findPer = database.Person.Find(model.PersonID);
                    if (findPer.Email == restart.Email)
                    {
                        var newPass = new Share().RandomText();
                        if (SendPass(findPer.PersonID, newPass))
                        {
                            UpdateUser(findPer.PersonID, newPass);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Lỗi gửi Email");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email không chính xác!");
                    }
                }
            }
            return View("PasswordRetrieval");
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        public ActionResult CreateUser(Users user)
        {
            var findU = database.Users.Find(user.UserName);
            if(findU != null)
            {
                ModelState.AddModelError("", "UserName đã tồn tại");
            }
            else
            {
                var findP = database.Person.Find(user.PersonID);
                if(findP == null)
                {
                    ModelState.AddModelError("", "Sai mã xác nhận");
                }
                else
                {
                    Users tk = new Users();
                    tk.UserName = user.UserName;
                    tk.PassWord = MaHoaMd5.MD5Hash(user.PassWord);
                    tk.PersonID = user.PersonID;
                    tk.Status = true;
                    database.Users.Add(tk);
                    database.SaveChanges();
                    ModelState.AddModelError("", "Đăng ký thành công");
                }
            }
            return View("CreateUser");
        }
        public void UpdateUser(string personID, string newPass)
        {
            var model = database.Users.SingleOrDefault(x=>x.PersonID == personID);
            model.PassWord = MaHoaMd5.MD5Hash(newPass);
            database.SaveChanges();

        }

        public void checkEDate()
        {
            checkOrgan();
            CheckInternShip();
        }

        public void checkOrgan()
        {
            // Lấy ngày hiện tại
            DateTime time = DateTime.Now;
            // Danh sách Cơ quan
            var model = database.Organization.ToList();
            // tạo 2 danh sách mới
            List<Organization> listTimeExpired = new List<Organization>();
            List<Organization> listSendEmail = new List<Organization>();
            foreach (var item in model)
            {
                // Xác định hạn cuối
                DateTime eDate = item.StartDay.AddMonths(item.ExpiryDate);
                if (eDate >= time)
                {
                    // Tính số ngày còn lại
                    var timespan = eDate - time;
                    // Lấy số  chẳn.
                    double view = Math.Floor(timespan.TotalDays);
                    if (view < 30 && item.SendEmail == false)
                    {
                        listSendEmail.Add(item);
                    }
                }
                else
                {
                    listTimeExpired.Add(item);
                }

            }
            foreach (var item1 in listTimeExpired)
            {
                ChangesStatusOrgan(item1.ID);
            }
            // Gửi Email cho công ty gần tời hạn
            foreach (var item2 in listSendEmail)
            {
                if (Send(item2.ID))
                {
                    database.Organization.Find(item2.ID).SendEmail = true;
                }
            }
            database.SaveChanges();
        }

        public void ChangesStatusOrgan(string id)
        {
            // Khóa Công ty
            database.Organization.Find(id).Status = false;
            // Tìm Manager của công ty
            var findP = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == 2);
            // Trường hợp  là Công ty
            if (findP != null)
            {
                var findU = database.Users.SingleOrDefault(x => x.PersonID == findP.PersonID);
                findU.Status = false;
                // Lấy danh sách Ledder của công ty
                var model = database.Person.Where(x => x.CompanyID == id && x.RoleID == 4).ToList();
                foreach (var item in model)
                {
                    if (ChangeStatusInternship(item.PersonID))
                    {
                        var findUser = database.Users.SingleOrDefault(x => x.PersonID == item.PersonID);
                        findUser.Status = false;
                    }
                }
            }
            else
            // Trường hợp nhà trường
            {
                var findPre = database.Person.SingleOrDefault(x => x.SchoolID == id && x.RoleID == 3);
                if (findPre != null)
                {
                    var findU = database.Users.SingleOrDefault(x => x.PersonID == findPre.PersonID);
                    findU.Status = false;
                }
            }
            database.SaveChanges();
        }

        public bool ChangeStatusInternship(string id)
        {
            try
            {
                // Lấy danh sách Khóa học của Ledder có id ở trên
                var model = database.InternShip.Where(x => x.PersonID == id).ToList();
                foreach (var item in model)
                {
                    item.Status = false;
                }
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckInternShip()
        {
            DateTime time = DateTime.Now;
            var model = database.InternShip.ToList();
            foreach (var item in model)
            {
                DateTime eDate = item.StartDay;
                if(eDate == time)
                {
                    item.Status = true;
                }else if (eDate.AddMonths(item.ExpiryDate) <= time)
                {
                    item.Status = false;
                }
            }
            database.SaveChanges();
        }

        public bool Send(string id)
        {
            try
            {
                string email;
                var res = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == 2);
                if (res != null)
                {
                    email = res.Email;
                }
                else
                {
                    var res1 = database.Person.SingleOrDefault(x => x.SchoolID == id && x.RoleID == 3);
                    email = res1.Email;
                }
                var findO = database.Organization.Find(id);
                DateTime eDate = findO.StartDay.AddMonths(findO.ExpiryDate);
                string emailOr = findO.Email;
                string nd = eDate.ToString("dd/MM/yyyy");
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/GopY.html"));
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Thông báo gia hạn";
                message.IsBodyHtml = true;
                message.Body = content;

                MailMessage messageOr = new MailMessage(fromEmailAddress, emailOr);
                messageOr.Subject = "Thông báo gia hạn";
                messageOr.IsBodyHtml = true;
                messageOr.Body = content;

                SmtpClient client = new SmtpClient(smtpHost, 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                // Đăng nhập gmail
                NetworkCredential nc = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                client.UseDefaultCredentials = false;
                client.Credentials = nc;
                client.Send(message);
                client.Send(messageOr);
                return true;
            }
            catch
            {
                return false;
            }


        }

        public bool SendPass(string id, string newPass)
        {
            try
            {
                var res = database.Person.Find(id);
                string cus = res.LastName + " " + res.FirstName;
                string email = res.Email;
                string nd = newPass;
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Email/EmailPassword.html"));
                content = content.Replace("{{CustomerName}}", cus);
                content = content.Replace("{{noidung}}", nd);

                var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();

                MailMessage message = new MailMessage(fromEmailAddress, email);
                message.Subject = "Cấp mật khẩu mới";
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient client = new SmtpClient(smtpHost, 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                // Đăng nhập gmail
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

        public ActionResult cvOrgan(string id)
        {
            var model = database.Organization.SingleOrDefault(x => x.ID == id);
            return View(model);
        }
    }
}
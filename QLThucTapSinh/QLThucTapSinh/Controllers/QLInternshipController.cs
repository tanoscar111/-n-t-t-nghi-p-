using PagedList;
using QLThucTapSinh.Common;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLThucTapSinh.Controllers
{
    public class QLInternshipController : Controller
    {
        // GET: QLInternship
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();

        public ActionResult Index(int id)
        {
            var model = listIShip();
            if (id == 0)
            {
                id = model[0].InternshipID;
                Session["InternshipID"] = id;
            }          
            var l = database.IntershipWithTask.Where(x => x.InternshipID == id).OrderBy(x => x.SORT).ToList();
            SelectList chose = new SelectList(l, "SORT", "SORT");
            ViewBag.listID = chose;
            ViewBag.listT = model;
            return View(ListTask(id));
        }

        public ActionResult Index1()
        {
            var comid = Session["CompanyID"].ToString();
            var model = database.InternShip.Where(x => x.CompanyID == comid && x.PersonID == null).ToList();
            return View(model);
        }

        public List<InternShip> listIShip()
        {
            var personID = Session["Person"].ToString();
            var role = Convert.ToInt32(Session["Role"]);
            List<InternShip> model = new List<InternShip>();
            if (role == 4)
            {
                var mo = database.InternShip.Where(x => x.PersonID == personID).OrderByDescending(x=>x.StartDay).ToList();
                foreach (var item in mo)
                {
                    model.Add(item);
                }
            }
            else
            {
                var find = database.Person.Find(personID);
                var list = database.InternShip.Where(x => x.CompanyID == find.CompanyID).OrderByDescending(x => x.StartDay).ToList();
                foreach (var item in list)
                {
                    model.Add(item);
                }
            }
            return model;
        }

        public List<Task> tasks(int id)
        {
            List<Task> tt = new List<Task>();
            var listinandt = database.IntershipWithTask.Where(x => x.InternshipID == id).ToList();
            // danh sách task cú cùng intership
            foreach (var item in listinandt)
            {
                var tas = database.Task.Find(item.TaskID);
                tt.Add(tas);
                // lấy ds task trong bảng task
                // đối tượng task (Find)
            }
            return tt;
        }

        public List<TaskDatabase> ListTask(int id)
        {
            var l = (from a in tasks(id)
                     join b in database.IntershipWithTask on a.TaskID equals b.TaskID
                     where b.InternshipID == id
                     select new TaskDatabase
                     {
                         ID = b.ID,
                         taskid = a.TaskID,
                         taskname = a.TaskName,
                         sort = b.SORT,
                         InternshipID = b.InternshipID
                     }).OrderBy(x => x.sort).ToList();
            return l;
        }

        public ActionResult Dele(int id)
        {
            var xoa = database.IntershipWithTask.Find(id);
            var lsort = database.IntershipWithTask.Where(x => x.InternshipID == xoa.InternshipID && x.SORT > xoa.SORT).ToList();
            foreach (var item in lsort)
            {
                item.SORT = item.SORT - 1;
                database.SaveChanges();
            }
            int count = database.IntershipWithTask.Count();
            var f = database.IntershipWithTask.Find(count);
            xoa.InternshipID = f.InternshipID;
            xoa.TaskID = f.TaskID;
            xoa.SORT = f.SORT;
            database.IntershipWithTask.Remove(f);
            database.SaveChanges();
            Session["InternshipID"] = xoa.InternshipID;
            var tk = Convert.ToInt32(Session["InternshipID"]);
            return RedirectToAction("Index",new { id = tk });
        }

        public void UpdateInter (int id)
        {
            var list = database.Intern.Where(x => x.InternshipID == id).ToList();
            foreach (var item in list)
            {
                item.InternshipID = null;
                item.Result = 0;
            }
            database.SaveChanges();
        }

        public bool UpdateIn(int id)
        {
            try
            {
                var list = database.IntershipWithTask.Where(x => x.InternshipID == id).ToList();
                foreach (var item in list)
                {
                    database.IntershipWithTask.Remove(item);
                }
                database.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ActionResult Delete(int id)
        {
            var i = database.InternShip.Find(id);
            if (UpdateIn(id))
            {
                UpdateInter(id);
                var cuoi = database.InternShip.Count();
                if (id == cuoi)
                {
                    database.InternShip.Remove(i);
                }
                else
                {
                    var ish = database.InternShip.Find(cuoi);
                    i.CourseName = ish.CourseName;
                    i.Note = ish.Note;
                    i.StartDay = ish.StartDay;
                    i.ExpiryDate = ish.ExpiryDate;
                    i.CompanyID = ish.CompanyID;
                    i.PersonID = ish.PersonID;
                    i.Status = ish.Status;
                    database.InternShip.Remove(ish);
                }
                database.SaveChanges();
            }
            return RedirectToAction("Index/0");
        }

        public ActionResult UpdateSort(int id, int sort)
        {
            var find1 = database.IntershipWithTask.Find(id);
            int sort1 = find1.SORT;
            find1.SORT = sort;
            var find2 = database.IntershipWithTask.SingleOrDefault(x => x.InternshipID == find1.InternshipID && x.SORT == sort);
            find2.SORT = sort1;
            database.SaveChanges();
            Session["InternshipID"] = find1.InternshipID;
            return Content(find1.InternshipID.ToString());
        }

        public bool ChangeStatusInternS(int id)
        {
            var com = database.InternShip.Find(id);
            if (com.Status == true)
            {
                com.Status = false;
            }
            else
            {
                var date = DateTime.Now;
                if(com.StartDay > date)
                {
                    com.Status = false;
                }
                else if(com.StartDay.AddMonths(com.ExpiryDate) > date)
                {
                    com.Status = true;
                }
            }
            database.SaveChanges();
            return com.Status;

        }

        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {
            var res = ChangeStatusInternS(id);
            return Json(new
            {
                status = res
            });
        }

        public ActionResult Create()
        {
            SetViewBagL();
            SetViewBagM();
            return View();
        }

        public void SetViewBagM(string selectedID = null)
        {
            SelectList Month = new SelectList(new[] {
                new {Text = "Một Tháng", Value = 1},
                new {Text = "Hai Tháng", Value = 2},
                new {Text = "Ba Tháng", Value = 3},
                new {Text = "Bốn Tháng", Value = 4},
                new {Text = "Năm Tháng", Value = 5},
                new {Text = "Sáu Tháng", Value = 6},
            }, "Value", "Text");
            ViewBag.Month = Month;
        }

        public void SetViewBagL(string selectedID = null)
        {
            var model = Session["CompanyID"].ToString();
            var list = database.Person.Where(x => x.CompanyID == model && x.RoleID == 4).ToList();
            var listLeader = new List<LeaderClass>();
            LeaderClass leader = new LeaderClass();
            foreach (var item in list)
            {
                leader.FullName = item.LastName + " " + item.FirstName;
                leader.PersonID = item.PersonID;
                listLeader.Add(leader);
            }
            SelectList ListLeader = new SelectList(listLeader, "PersonID", "FullName");
            ViewBag.ListLeader = ListLeader;
        }
        [HttpPost]
        public ActionResult Create(InternShip ish)
        {
            InternShip i = new InternShip();
            i.InternshipID = database.InternShip.Count() + 1;
            i.CourseName = ish.CourseName;
            i.Note = ish.Note;
            i.StartDay = ish.StartDay;
            i.ExpiryDate = ish.ExpiryDate;
            i.Status = false;
            i.CompanyID = Session["CompanyID"].ToString();
            var ro = Convert.ToInt32(Session["Role"]);
            if (ish.PersonID != null)
            {
                i.PersonID = ish.PersonID;
            }
            else if(ro == 4)
            {
                var pid = Session["Person"].ToString();
                i.PersonID = pid;
            }
            database.InternShip.Add(i);
            database.SaveChanges();
            ModelState.AddModelError("", "Thành công");
            SetViewBagL();
            SetViewBagM();
            return View("Create");
        }

        public ActionResult Edit(int id)
        {
            var model = database.InternShip.Find(id);
            SetViewBagL();
            SetViewBagM();
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(InternShip ish)
        {
            var i = database.InternShip.Find(ish.InternshipID);
            i.CourseName = ish.CourseName;
            i.Note = ish.Note;
            i.StartDay = ish.StartDay;
            i.ExpiryDate = ish.ExpiryDate;
            if(ish.PersonID != null)
            {
                i.PersonID = ish.PersonID;
            }
            database.SaveChanges();
            SetViewBagL();
            SetViewBagM();
            ModelState.AddModelError("", "Thành công");
            return View("Edit");
        }

        public ActionResult Accuracy(int id)
        {
            var pid = Session["Person"].ToString();
            var model = database.InternShip.Find(id);
            model.PersonID = pid;
            database.SaveChanges();
            return RedirectToAction("Index", new { id = 0 });
        }

        //public ActionResult listT(int tk)
        //{
        //    Session["InternshipID"] = tk;
        //    var l = database.IntershipWithTask.Where(x => x.InternshipID == tk).OrderBy(x => x.SORT).ToList();
        //    SelectList chose = new SelectList(l, "SORT", "SORT");
        //    ViewBag.listID = chose;
        //    var model = ListTask(tk);
        //    return PartialView(model);
        //}
    }
}
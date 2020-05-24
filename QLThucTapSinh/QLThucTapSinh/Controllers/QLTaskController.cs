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
    public class QLTaskController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        // GET: QLTask

        public List<TaskClass> tasks()
        {
            var ro = int.Parse(Session["Role"].ToString());
            var pid = Session["Person"].ToString();
            if (ro == 4)
            {
                var model = (from a in database.Task
                             join b in database.Person on a.PersonID equals b.PersonID
                             where b.PersonID == pid
                             select new TaskClass
                             {
                                 TaskID = a.TaskID,
                                 TaskName = a.TaskName,
                                 Note = a.Note,
                                 Video = a.Video,
                                 PersonID = a.PersonID,
                                 FullName = b.LastName + " " + b.FirstName
                             }).OrderBy(x => x.TaskID).ToList();
                return model;
            }
            else
            {
                var comid = Session["CompanyID"].ToString();
                var model = (from a in database.Task
                             join b in database.Person on a.PersonID equals b.PersonID
                             where b.CompanyID == comid && b.RoleID == 4
                             select new TaskClass
                             {
                                 TaskID = a.TaskID,
                                 TaskName = a.TaskName,
                                 Note = a.Note,
                                 Video = a.Video,
                                 PersonID = a.PersonID,
                                 FullName = b.LastName + " " + b.FirstName
                             }).OrderBy(x => x.PersonID).ToList();
                return model;
            }
        }



        public ActionResult Index()
        {
            ViewBag.listin = listin();
            return View(tasks());
        }

        public List<InternShip> listin()
        {

            var r = int.Parse(Session["Role"].ToString());
            var id = Session["Person"].ToString();
            var find = database.Person.Find(id);
            if (r == 4)
            {
                return database.InternShip.Where(x => x.PersonID == find.PersonID).ToList();
            }
            else
            {
                return database.InternShip.Where(x => x.CompanyID == find.CompanyID).ToList();
            }
        }

        public bool AddTask(List<int> listTask, int id)
        {
            try
            {
                foreach (var i in listTask)
                {
                    IntershipWithTask addTask = new IntershipWithTask();
                    var count = database.IntershipWithTask.Count();
                    addTask.ID = count + 1;
                    addTask.InternshipID = id;
                    addTask.TaskID = i;
                    var sort = database.IntershipWithTask.Where(x => x.InternshipID == id).Count();
                    addTask.SORT = sort + 1;
                    database.IntershipWithTask.Add(addTask);
                    database.SaveChanges();
                }
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
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                if (CreateTask(task))
                {
                    ModelState.AddModelError("", "Thêm Bài học thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Công ty thất bại");
                }
            }
            return View("Create");
        }

        public bool CreateTask(Task task)
        {
            try
            {
                var personID = Session["Person"].ToString();
                var count = database.Task.Count();
                Task t = new Task();
                t.TaskID = count + 1;
                t.TaskName = task.TaskName;
                t.Note = task.Note;
                t.Video = task.Video;
                t.PersonID = personID;
                database.Task.Add(t);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = database.Task.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {
                var model = Update(task);
                if (model)
                {
                    ModelState.AddModelError("", "Cập nhật thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại");
                }
            }
            return View("Edit");
        }

        public bool Update(Task task)
        {
            try
            {
                var model = database.Task.Find(task.TaskID);
                model.TaskName = task.TaskName;
                model.Note = task.Note;
                model.Video = task.Video;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            try
            {
                var listtask = database.IntershipWithTask.Where(x => x.TaskID == id).ToList();
                for (int i = 0; i < listtask.Count; i++)
                {
                    var idinship = listtask[i].InternshipID;
                    var sort = listtask[i].SORT;
                    var count = database.IntershipWithTask.Count();
                    var find = database.IntershipWithTask.Find(count);
                    var model = database.IntershipWithTask.SingleOrDefault(x => x.InternshipID == idinship && x.TaskID == id);
                    var idao = model.ID;
                    model.InternshipID = find.InternshipID;
                    model.TaskID = find.TaskID;
                    model.SORT = find.SORT;
                    database.SaveChanges();
                    database.IntershipWithTask.Remove(find);                    
                    database.SaveChanges();
                    var list = database.IntershipWithTask.Where(x => x.InternshipID == idinship && x.SORT > sort).OrderBy(x => x.SORT).ToList();
                    foreach (var item in list)
                    {
                        item.SORT = item.SORT - 1;
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

        public ActionResult Delete(int id)
        {
            if (DeleteTask(id))
            {
                var task1 = database.Task.Find(id);
                var count = database.Task.Count();
                var task2 = database.Task.Find(count);
                task1.TaskName = task2.TaskName;
                task1.Note = task2.Note;
                task1.Video = task2.Video;
                task1.PersonID = task2.PersonID;
                database.Task.Remove(task2);
                foreach (var item in database.IntershipWithTask)
                {
                    if(item.TaskID == task2.TaskID)
                    {
                        item.TaskID = id;
                    }
                }
                database.SaveChanges();
            }
            var role = Convert.ToInt32(Session["Role"].ToString());
            var personID = Session["Person"].ToString();
            if (role == 2)
            {
                var findP = database.Person.Find(personID);
                var model = (from a in database.Task
                             join b in database.Person on a.PersonID equals b.PersonID
                             where b.CompanyID == findP.CompanyID && b.RoleID == 4
                             select new TaskClass
                             {
                                 TaskID = a.TaskID,
                                 TaskName = a.TaskName,
                                 Note = a.Note,
                                 Video = a.Video,
                                 PersonID = a.PersonID,
                                 FullName = b.LastName + " " + b.FirstName
                             }).OrderBy(x => x.PersonID).ToList();
                return RedirectToAction("Index", model);
            }
            else
            {
                var model = (from a in database.Task
                             join b in database.Person on a.PersonID equals b.PersonID
                             where b.PersonID == personID
                             select new TaskClass
                             {
                                 TaskID = a.TaskID,
                                 TaskName = a.TaskName,
                                 Note = a.Note,
                                 Video = a.Video,
                                 PersonID = a.PersonID,
                                 FullName = b.LastName + " " + b.FirstName
                             }).OrderBy(x => x.PersonID).ToList();
                return RedirectToAction("Index", model);
            }
        }
    }
}
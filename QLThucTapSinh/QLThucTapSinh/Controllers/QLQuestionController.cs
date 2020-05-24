using PagedList;
using QLThucTapSinh.Common;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
namespace QLThucTapSinh.Controllers
{
    public class QLQuestionController : Controller
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        // GET: QLQuestion
        public ActionResult Index(int page = 1, int pagesize = 5)
        {
            // Lấy roleID
            var role = Convert.ToInt32(Session["Role"].ToString());
            // Đối với Ledder
            if (role == 4)
            {
                var personID = Session["Person"].ToString();
                var list = (from a in database.Person
                            join c in database.Task on a.PersonID equals c.PersonID
                            join d in database.Question on c.TaskID equals d.TaskID
                            where a.PersonID == personID
                            select new QuestionClass()
                            {
                                QuestionID = d.QuestionID,
                                Content = d.Content,
                                Answer = d.Answer,
                                A = d.A,
                                B = d.B,
                                C = d.C,
                                D = d.D,
                                TaskID = c.TaskID,
                                TaskName = c.TaskName,
                                PersonID = a.PersonID,
                                FullName = a.LastName + " " + a.FirstName
                            }).OrderBy(x => x.TaskID).ToPagedList(page, pagesize);
                var count = list.Count();
                return View(list);
            }
            //Đối với manager
            else
            {
                var companyID = Session["CompanyID"].ToString();
                var list = (from a in database.Person
                            join c in database.Task on a.PersonID equals c.PersonID
                            join d in database.Question on c.TaskID equals d.TaskID
                            where a.CompanyID == companyID && a.RoleID == 4
                            select new QuestionClass()
                            {
                                QuestionID = d.QuestionID,
                                Content = d.Content,
                                Answer = d.Answer,
                                A = d.A,
                                B = d.B,
                                C = d.C,
                                D = d.D,
                                TaskID = c.TaskID,
                                TaskName = c.TaskName,
                                PersonID = a.PersonID,
                                FullName = a.LastName + " " + a.FirstName
                            }).OrderBy(x => x.TaskID).ToPagedList(page, pagesize);
                var count = list.Count();
                return View(list);
            }

        }

        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Question question)
        {
            if (CreateQ(question))
            {
                ModelState.AddModelError("", "Thêm Câu hỏi thành công");
            }
            else
            {
                ModelState.AddModelError("", "Thêm Câu hỏi thất bại");
            }
            SetViewBag();
            return View("Create");
        }

        public bool CreateQ(Question question)
        {
            try
            {
                Question q = new Question();
                int count = database.Question.Count();
                q.QuestionID = count + 1;
                q.Content = question.Content;
                q.Answer = question.Answer;
                q.A = question.A;
                q.B = question.B;
                q.C = question.C;
                q.D = question.D;
                q.TaskID = question.TaskID;
                database.Question.Add(q);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetViewBag()
        {
            var personID = Session["Person"].ToString();
            List<Task> Task = database.Task.Where(x=>x.PersonID == personID).ToList();
            SelectList QueList = new SelectList(Task, "TaskID", "TaskName");
            ViewBag.QueList = QueList;
        }
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = database.Question.Find(id);
            SetViewBag();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Question q)
        {
            if (ModelState.IsValid)
            {
                var model1 = Update(q);
                if (model1)
                {
                    return RedirectToAction("Index", "QLQuestion");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công");
                }
                SetViewBag();
            }
            return View("Edit");
        }

        public bool Update(Question question)
        {
            try
            {
                var model = database.Question.Find(question.QuestionID);
                model.Content = question.Content;
                model.Answer = question.Answer;
                model.TaskID = question.TaskID;
                model.A = question.A;
                model.B = question.B;
                model.C = question.C;
                model.D = question.D;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ActionResult Delete(int id)
        {
            var q1 = database.Question.Find(id);
            var count = database.Question.Count();
            var q2 = database.Question.Find(count);
            q1.Content = q2.Content;
            q1.Answer = q2.Answer;
            q1.TaskID = q2.TaskID;
            q1.A = q2.A;
            q1.B = q2.B;
            q1.C = q2.C;
            q1.D = q2.D;
            database.Question.Remove(q2);
            database.SaveChanges();
            var model1 = database.Question.OrderBy(x => x.TaskID).ToList();
            return RedirectToAction("Index", model1);
        }

        public ActionResult CreateExcel()
        {
            var personID = Session["Person"].ToString();
            ViewBag.ListTask = database.Task.Where(x=>x.PersonID == personID).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateExcel(HttpPostedFileBase excelfile, int taskID)
        {
            var personID = Session["Person"].ToString();
            // Kiểm tra file đó có tồn tại hay không
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.ListTask = database.Task.Where(x => x.PersonID == personID).ToList();
                ViewBag.Error = "Thêm File mới<br /> ";
                return View("CreateExcel");
            }
            else
            {
                // kiểm tra đuôi file có phải là file Excel hay không
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    // Khai báo đường dẫn
                    string path = Path.Combine("D:/", excelfile.FileName);
                    // Tạo đối tượng COM. Tạo một đối tượng COM cho mọi thứ được tham chiếu
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    //Lặp lại qua các hàng và cột và in ra bàn điều khiển khi nó xuất hiện trong tệp
                    //excel is not zero based!!
                    for (int i = 2; i < range.Rows.Count; i++)
                    {
                        Question q = new Question();
                        q.Content = ((Excel.Range)range.Cells[i, 2]).Text;
                        q.Answer = ((Excel.Range)range.Cells[i, 3]).Text;
                        q.A = ((Excel.Range)range.Cells[i, 4]).Text;
                        q.B = ((Excel.Range)range.Cells[i, 5]).Text;
                        q.C = ((Excel.Range)range.Cells[i, 6]).Text;
                        q.D = ((Excel.Range)range.Cells[i, 7]).Text;
                        q.TaskID = taskID;
                        CreateQ(q);
                    }

                    //cleanup
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    //xuất các đối tượng com để dừng hoàn toàn quá trình excel chạy trong nền
                    Marshal.ReleaseComObject(range);
                    Marshal.ReleaseComObject(worksheet);
                    //đóng lại và xuất thông tin
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                    //thoát và xuất thông tin
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                    //ViewBag.ListProduct = listproducts;
                    //dem = listproducts.Count();
                    //return View("List");
                    ViewBag.ListTask = database.Task.Where(x => x.PersonID == personID).ToList();
                    ViewBag.Error = "Thêm thành công<br /> ";
                    return View("CreateExcel");
                }
                else
                {
                    ViewBag.ListTask = database.Task.Where(x => x.PersonID == personID).ToList();
                    ViewBag.Error = "File không hợp lệ<br /> ";
                    return View("CreateExcel");
                }
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLThucTapSinh.Common
{
    public class QuestionClass
    {
        public int? QuestionID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string TaskName { get; set; }
        public int InternshipID { get; set; }
        public string CourseName { get; set; }
        public string PersonID { get; set; }
        public string FullName { get; set; }
    }
}
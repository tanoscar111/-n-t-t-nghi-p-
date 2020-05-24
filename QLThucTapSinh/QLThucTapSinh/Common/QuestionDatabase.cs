using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyInInterns.ViewModel
{
    public class QuestionDatabase
    {
        public int QuestionID { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public Nullable<int> TaskID { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string LedderID { get; set; }
    }
}
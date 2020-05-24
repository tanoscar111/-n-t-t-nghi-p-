using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLThucTapSinh.Common
{
    public class TestResultsClass
    {
        public int ID { get; set; }
        public string PersonID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public string TaskName { get; set; }
        public Nullable<int> Answer { get; set; }
    }
}
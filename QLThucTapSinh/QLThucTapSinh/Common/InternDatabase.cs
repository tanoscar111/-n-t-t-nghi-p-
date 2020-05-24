using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLThucTapSinh.Common
{
    public class InternDatabase
    {
        public string PersonID { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> InternshipID { get; set; }
        public string SchoolID { get; set; }
        public string SchoolName { get; set; }
        public string StudentCode { get; set; }
        public Nullable<int> Result { get; set; }
        public bool Status { get; set; }
    }
}
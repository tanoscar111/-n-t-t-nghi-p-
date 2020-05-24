using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLThucTapSinh.Common
{
    public class Internshipclass
    {
        public int InternshipID { get; set; }

        [StringLength(250)]
        public string CourseName { get; set; }

        [StringLength(8)]
        public string PersonID { get; set; }

        public string FullName { get; set; }

        [StringLength(8)]
        public string CompanyID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public DateTime StartDay { get; set; }


    }
}
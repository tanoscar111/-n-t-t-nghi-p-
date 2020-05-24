using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLThucTapSinh.Common
{
    public class RestartPassword
    {
        [Required(ErrorMessage = "Yêu cầu nhập UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }
    }
}
namespace QLThucTapSinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Menu")]
    public partial class Menu
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string TextMenu { get; set; }

        [StringLength(250)]
        public string Link { get; set; }

        public int? RoleID { get; set; }

        public virtual Role Role { get; set; }
    }
}

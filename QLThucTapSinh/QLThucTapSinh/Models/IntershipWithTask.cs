namespace QLThucTapSinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IntershipWithTask")]
    public partial class IntershipWithTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int InternshipID { get; set; }

        public int TaskID { get; set; }

        public int SORT { get; set; }

        public virtual InternShip InternShip { get; set; }

        public virtual Task Task { get; set; }
    }
}

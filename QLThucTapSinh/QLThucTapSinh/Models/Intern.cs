namespace QLThucTapSinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Intern")]
    public partial class Intern
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Intern()
        {
            TestResults = new HashSet<TestResults>();
        }

        [Key]
        [StringLength(8)]
        public string PersonID { get; set; }

        [StringLength(15)]
        public string StudentCode { get; set; }

        public int? InternshipID { get; set; }

        public int? Result { get; set; }

        public virtual Person Person { get; set; }

        public virtual InternShip InternShip { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResults> TestResults { get; set; }
    }
}

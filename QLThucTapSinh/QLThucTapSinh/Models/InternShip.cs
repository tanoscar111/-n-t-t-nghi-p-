namespace QLThucTapSinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InternShip")]
    public partial class InternShip
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InternShip()
        {
            Intern = new HashSet<Intern>();
            IntershipWithTask = new HashSet<IntershipWithTask>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InternshipID { get; set; }

        [StringLength(250)]
        public string CourseName { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        [StringLength(8)]
        public string PersonID { get; set; }

        [StringLength(8)]
        public string CompanyID { get; set; }

        public DateTime StartDay { get; set; }

        public int ExpiryDate { get; set; }

        public bool Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Intern> Intern { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual Person Person { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntershipWithTask> IntershipWithTask { get; set; }
    }
}

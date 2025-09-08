using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    [Table("Doctor")]

    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Specialization { get; set; }

        public string Fee { get; set; }

        public string Picture { get; set; } // Store file name or path
        // Foreign keys
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }

        // Navigation Properties
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }
}

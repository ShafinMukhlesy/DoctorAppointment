using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    [Table("Department")]
    public class Department
    {
        
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Navigation Property (One-to-Many with Doctor)
        public virtual ICollection<Doctor> Doctors { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }
}
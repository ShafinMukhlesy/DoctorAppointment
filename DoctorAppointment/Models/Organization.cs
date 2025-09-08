using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        // Navigation Property (One-to-Many with Doctor)
        public virtual ICollection<Doctor> Doctors { get; set; }
    }
}
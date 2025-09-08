using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    [Table("Appointment")]
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public int DoctorId { get; set; }
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; } // from available slots

        [Required, StringLength(100)]
        public string PatientName { get; set; }

        [Phone]
        public string PatientPhone { get; set; }

        [EmailAddress]
        public string PatientEmail { get; set; }

        // New fields
        public DateTime? PatientDOB { get; set; }

        [StringLength(10)]
        public string PatientGender { get; set; }

        [StringLength(250)]
        public string PatientAddress { get; set; }

        // Navigation
        public virtual Doctor Doctor { get; set; }
        public virtual Department Department { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
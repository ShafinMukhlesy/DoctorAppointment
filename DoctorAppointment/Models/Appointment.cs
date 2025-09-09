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

        public int PatientId { get; set; }


        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; } // from available slots

        // Navigation
        public virtual Patient Patient { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Department Department { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
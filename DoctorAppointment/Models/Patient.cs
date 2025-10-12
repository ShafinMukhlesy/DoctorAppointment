using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    public class Patient
    {
        public int PatientId { get; set; }   // Primary Key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [StringLength(10)]
        public string Gender { get; set; }   // Male, Female, Other
        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(5)]
        public string BloodGroup { get; set; } // e.g., A+, O-, AB+

        public decimal? Weight { get; set; }   
        public string Height { get; set; }   

        [StringLength(500)]
        public string MedicalHistory { get; set; } 

        [StringLength(250)]
        public string PatientImage { get; set; } // store file path like: "/Uploads/Patients/p123.png"

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Track record creation

        [NotMapped]
        public string returnUrl { get; set; } 
    }
}
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

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 digits and contain only digits")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be 11 digits and contain only digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }   // Male, Female, Other
        [StringLength(250)]
        [Required(ErrorMessage = "Address is required")]
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

        public string NID { get; set; }
        public string Title { get; set; }

        [Required(ErrorMessage = "Age is required")]
        public decimal? Age { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string Occupation { get; set; }
        public string GuardianType { get; set; }
        public string GuardianName { get; set; }
        public string PassportNo { get; set; }
        public string MaritalStatus { get; set; }
    }
}
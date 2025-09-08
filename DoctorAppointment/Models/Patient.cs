using System;
using System.Collections.Generic;
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
    }
}
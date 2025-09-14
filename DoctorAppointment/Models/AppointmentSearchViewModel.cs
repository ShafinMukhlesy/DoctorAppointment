using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    public class AppointmentSearchViewModel
    {
        public string PatientName { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<Appointment> Appointments { get; set; }
        public List<Doctor> Doctors { get; set; }
    }
}
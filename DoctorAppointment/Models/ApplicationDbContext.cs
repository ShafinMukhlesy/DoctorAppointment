using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DoctorAppionmentDBEntities") { }

        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Organization> Organization { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        public DbSet<Appointment> Appointments { get; set; }




    }
}
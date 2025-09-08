using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace DoctorAppointment.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Organization> Organizations { get; set; }
    }
}
using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Appointment
        public ActionResult SelectDoctor()
        {
            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name");
            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentId", "Name");
            ViewBag.DoctorId = new SelectList(db.Doctor, "DoctorId", "Name");




            return View();
        }

        [HttpPost]
        public ActionResult SelectDoctor(int OrganizationId, int DepartmentId, int DoctorId, DateTime AppointmentDate, string AppointmentTime)
        {
            // Redirect to next step with selected values
            return RedirectToAction("Create", new { OrganizationId, DepartmentId, DoctorId, AppointmentDate,AppointmentTime });
        }

        // Step 2 - Fill patient info
        public ActionResult Create(int OrganizationId, int DepartmentId, int DoctorId, DateTime AppointmentDate, string AppointmentTime)
        {
            var model = new Appointment
            {
                OrganizationId = OrganizationId,
                DepartmentId = DepartmentId,
                DoctorId = DoctorId,
                AppointmentDate = AppointmentDate,
                AppointmentTime = TimeSpan.Parse(AppointmentTime)  // convert string to TimeSpan
            };

            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "FirstName");

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Confirm", new { id = appointment.AppointmentId });
            }
            return View(appointment);
        }

        // Step 3 - Confirmation
        public ActionResult Confirm(int id)
        {
            var appointment = db.Appointments
                .Include("Doctor").Include("Department").Include("Organization")
                .FirstOrDefault(a => a.AppointmentId == id);

            return View(appointment);
        }

        private List<TimeSpan> GetAvailableSlots(int doctorId, DateTime date)
        {
            // Look up doctor schedule for that day
            var schedule = db.DoctorSchedules
                .FirstOrDefault(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek);

            if (schedule == null) return new List<TimeSpan>();

            List<TimeSpan> slots = new List<TimeSpan>();
            var time = schedule.StartTime;
            while (time < schedule.EndTime)
            {
                slots.Add(time);
                time = time.Add(TimeSpan.FromMinutes(schedule.SlotDuration));
            }
            return slots;
        }

        public JsonResult GetDepartments(int organizationId)
        {
            var departments = db.Department
                                .Where(d => d.OrganizationId == organizationId)
                                .Select(d => new { d.DepartmentId, d.Name })
                                .ToList();

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDoctors(int departmentId)
        {
            var doctors = db.Doctor
                            .Where(d => d.DepartmentId == departmentId)
                            .Select(d => new { d.DoctorId, d.Name })
                            .ToList();

            return Json(doctors, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDoctorDetails(int doctorId)
        {
            var doctor = db.Doctor
                .Where(d => d.DoctorId == doctorId)
                .Select(d => new {
                    d.DoctorId,
                    d.Name,
                    d.Specialization,
                    d.Fee,
                    d.Email,
                    d.Phone,
                    d.Picture // store image URL in DB
                })
                .FirstOrDefault();

            return Json(doctor, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableSlotsforPatient(int doctorId, DateTime appointmentDate)
        {
            var allSlots = GetAvailableSlots(doctorId, appointmentDate)
                           .Select(t => new { Value = t.ToString(@"hh\:mm"), Text = DateTime.Today.Add(t).ToString("hh:mm tt") })
                           .ToList();

            var bookedSlots = db.Appointments
                         .Where(a => a.DoctorId == doctorId && a.AppointmentDate == appointmentDate)
                         .Select(a => a.AppointmentTime)  // keep as TimeSpan
                         .ToList()                        // bring into memory
                         .Select(t => t.ToString(@"hh\:mm")) // now safe to format
                         .ToList();

            return Json(new { AvailableSlots = allSlots, BookedSlots = bookedSlots }, JsonRequestBehavior.AllowGet);
        }


    }
}
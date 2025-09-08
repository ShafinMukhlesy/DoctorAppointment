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
        public ActionResult SelectDoctor(int OrganizationId, int DepartmentId, int DoctorId, DateTime AppointmentDate)
        {
            // Redirect to next step with selected values
            return RedirectToAction("Create", new { OrganizationId, DepartmentId, DoctorId, AppointmentDate });
        }

        // Step 2 - Fill patient info
        public ActionResult Create(int OrganizationId, int DepartmentId, int DoctorId, DateTime AppointmentDate)
        {
            var model = new Appointment
            {
                OrganizationId = OrganizationId,
                DepartmentId = DepartmentId,
                DoctorId = DoctorId,
                AppointmentDate = AppointmentDate
            };

            // TODO: Load available slots from DoctorSchedule
            //ViewBag.AvailableSlots = GetAvailableSlots(DoctorId, AppointmentDate);

            var allSlots = GetAvailableSlots(DoctorId, AppointmentDate)
                           .Select(t => new SelectListItem
                           {
                               Value = t.ToString(@"hh\:mm"),
                               Text = DateTime.Today.Add(t).ToString("hh:mm tt")
                           })
                           .ToList();

            // Get already booked slots for this doctor/date
            var bookedSlots = db.Appointments
                                .Where(a => a.DoctorId == DoctorId && a.AppointmentDate == AppointmentDate)
                                .Select(a => a.AppointmentTime)
                                .ToList();

            // Pass both lists to the view
            ViewBag.AvailableSlots = allSlots;
            ViewBag.BookedSlots = bookedSlots;



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

        public JsonResult GetPatientByPhone(string phone)
        {
            var patient = db.Appointments
                .Where(a => a.PatientPhone == phone)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.PatientName,
                    a.PatientEmail,
                    a.PatientDOB,
                    a.PatientGender,
                    a.PatientAddress
                })
                .FirstOrDefault();

            if (patient == null)
                return Json(new { found = false }, JsonRequestBehavior.AllowGet);

            return Json(new { found = true, patient }, JsonRequestBehavior.AllowGet);
        }


    }
}
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

        public ActionResult Index(string patientName, int? doctorId, DateTime? fromDate, DateTime? toDate)
        {
            var query = db.Appointments
                          .Include("Doctor")
                          .Include("Department")
                          .Include("Organization")
                          .Include("Patient")
                          .AsQueryable();

            if (!string.IsNullOrEmpty(patientName))
            {
                query = query.Where(a => (a.Patient.FirstName + " " + a.Patient.LastName).Contains(patientName));
            }

            if (doctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate >= fromDate.Value && a.AppointmentDate <= toDate.Value);
            }

            var appointments = query.ToList();

            var billedAppointments = db.Bill.Select(b => b.AppointmentId).ToList();
            ViewBag.BilledAppointments = billedAppointments;

            var model = new AppointmentSearchViewModel
            {
                PatientName = patientName,
                DoctorId = doctorId,
                FromDate = fromDate,
                ToDate = toDate,
                Appointments = appointments,
                Doctors = db.Doctor.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult SelectDoctorFromList(int OrganizationId, int DepartmentId, int DoctorId)
        {

            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name", OrganizationId);
            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentId", "Name", DepartmentId);
            ViewBag.DoctorId = new SelectList(db.Doctor, "DoctorId", "Name", DoctorId);

            ViewBag.SelectedOrganizationId = OrganizationId;
            ViewBag.SelectedDepartmentId = DepartmentId;
            ViewBag.SelectedDoctorId = DoctorId;

            return View("SelectDoctor");
        }


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

            ViewBag.PatientId = new SelectList(
                                                db.Patients.Select(p => new
                                                {
                                                    PatientId = p.PatientId,
                                                    FullName = p.FirstName + " " + p.LastName
                                                }),
                                                "PatientId",
                                                "FullName"
                                            );


            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
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


        public JsonResult GetDoctorScheduleDetails(int doctorId)
        {
            var doctor = db.DoctorSchedules
                .Where(d => d.DoctorId == doctorId)
                .ToList() // bring data into memory
                .Select(d => new
                {
                    d.DoctorId,
                    d.MaxPatients,
                    DayOfWeek = d.DayOfWeek.ToString(),
                    StartTime = d.StartTime.ToString(@"hh\:mm"),
                    EndTime = d.EndTime.ToString(@"hh\:mm"),
                    d.SlotDuration
                })
                .ToList();

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

        public JsonResult GetPatientsByPhone(string phone)
        {
            var patients = db.Patients
                             .Where(p => p.PhoneNumber.Contains(phone))
                             .Select(p => new
                             {
                                 PatientId = p.PatientId,
                                 FullName = p.FirstName + " " + p.LastName 
                             })
                             .ToList();

            return Json(patients, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPatientGender(int patientId)
        {
            var gender = db.Patients
                .Where(p => p.PatientId == patientId)
                .Select(p => p.Gender)
                .FirstOrDefault();

            return Json(gender, JsonRequestBehavior.AllowGet);
        }


    }
}
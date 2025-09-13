using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    public class BillController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Bill
        public ActionResult Index()
        {
            var bills = db.Bill
                     .Include("Doctor")
                     .Include("Patient")
                     .ToList();
            return View(bills);

        }

        // GET: Bill/Create
        public ActionResult Create(int? appointmentId, int patientId, int doctorId)
        {
            ViewBag.AppointmentId = new SelectList(db.Appointments, "AppointmentId", "AppointmentId", appointmentId);
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "FirstName", patientId);
            ViewBag.DoctorId = new SelectList(db.Doctor, "DoctorId", "Name", doctorId);

            var doctor = db.Doctor.Find(doctorId);
 

            var model = new Bill
            {
                ConsultationFee = doctor.Fee,
                BillDate = DateTime.Now,
                PaymentStatus = "Unpaid"
            };

            return View(model);
        }

        // POST: Bill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bill bill)
        {
            if (ModelState.IsValid)
            {
                // auto-calc total
                bill.TotalAmount = (decimal.Parse(bill.ConsultationFee ?? "0") + decimal.Parse(bill.OtherCharges ?? "0") - decimal.Parse(bill.Discount ?? "0")).ToString();

                db.Bill.Add(bill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppointmentId = new SelectList(db.Appointments, "AppointmentId", "AppointmentId", bill.AppointmentId);
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "Name", bill.PatientId);
            ViewBag.DoctorId = new SelectList(db.Doctor, "DoctorId", "Name", bill.DoctorId);

            return View(bill);
        }
    }
}
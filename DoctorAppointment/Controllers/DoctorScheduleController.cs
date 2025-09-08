using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    public class DoctorScheduleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: DoctorSchedule
        public ActionResult Index()
        {
            var doctorScheduleList = db.DoctorSchedules.Include(x=>x.Doctor).ToList();
            return View(doctorScheduleList);
        }

        public ActionResult CreateOrEdit(int? id)
        {
            ViewBag.DoctorId = new SelectList(db.Doctor, "DoctorId", "Name");

            if (id == null)
            {
                // New schedule
                return View(new DoctorSchedule());
            }

            // Edit existing schedule
            DoctorSchedule schedule = db.DoctorSchedules.Find(id);
            if (schedule == null) return HttpNotFound();

            return View(schedule);
        }

        // POST: DoctorSchedule/CreateOrEdit
        [HttpPost]
    
        public ActionResult CreateOrEdit(DoctorSchedule schedule)
        {
            if (ModelState.IsValid)
            {
                if (schedule.ScheduleId == 0)
                {
                    // Insert
                    db.DoctorSchedules.Add(schedule);
                }
                else
                {
                    // Update
                    db.Entry(schedule).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DoctorId = new SelectList(db.Doctor, "DoctorId", "Name", schedule.DoctorId);
            return View(schedule);
        }
    }
}
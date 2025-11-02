using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.IO;

namespace DoctorAppointment.Controllers
{
    public class DoctorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Doctor
        public ActionResult Index()
        {
            var doctorinfo = db.Doctor.Include(d => d.Department).Include(d => d.Organization).Include(d=>d.DoctorSchedules);
            return View(doctorinfo);

        }


        public ActionResult DoctorListView()
        {
            var doctorinfo = db.Doctor.Include(d => d.Department).Include(d => d.Organization);
            return View(doctorinfo);

        }

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentId", "Name");
            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Doctor doctor, HttpPostedFileBase PictureFile)
        {
            if (ModelState.IsValid)
            {
                if (PictureFile != null && PictureFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PictureFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/Doctors"), fileName);
                    PictureFile.SaveAs(path);

                    doctor.Picture = "/Uploads/Doctors/" + fileName; // Store relative path
                }

                db.Doctor.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentId", "Name", doctor.DepartmentId);
            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name", doctor.OrganizationId);

            return View(doctor);
        }

        // GET: Doctor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Doctor doctor = db.Doctor.Find(id);
            if (doctor == null) return HttpNotFound();

            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentId", "Name", doctor.DepartmentId);
            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name", doctor.OrganizationId);

            return View(doctor);
        }

        // POST: Doctor/Edit/5
        [HttpPost]
        public ActionResult Edit(Doctor doctor, HttpPostedFileBase PictureFile)
        {
            if (ModelState.IsValid)
            {
                var existingDoctor = db.Doctor.Find(doctor.DoctorId);
                if (existingDoctor == null)
                {
                    return HttpNotFound();
                }

                // Update basic fields
                existingDoctor.Name = doctor.Name;
                existingDoctor.Email = doctor.Email;
                existingDoctor.Phone = doctor.Phone;
                existingDoctor.Specialization = doctor.Specialization;
                existingDoctor.DepartmentId = doctor.DepartmentId;
                existingDoctor.OrganizationId = doctor.OrganizationId;
                existingDoctor.Fee = doctor.Fee;
                existingDoctor.IsActive = doctor.IsActive;
                if (PictureFile != null && PictureFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PictureFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/Doctors"), fileName);
                    PictureFile.SaveAs(path);

                    existingDoctor.Picture = "/Uploads/Doctors/" + fileName; // Store relative path
                }

                db.Entry(existingDoctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentId", "Name", doctor.DepartmentId);
            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name", doctor.OrganizationId);

            return View(doctor);
        }

        // GET: Doctor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Doctor doctor = db.Doctor.Include(d => d.Department).Include(d => d.Organization)
                                      .FirstOrDefault(d => d.DoctorId == id);

            if (doctor == null) return HttpNotFound();

            return View(doctor);

        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctor.Find(id);
            db.Doctor.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
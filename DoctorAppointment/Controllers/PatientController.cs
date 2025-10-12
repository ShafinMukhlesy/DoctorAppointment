using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    public class PatientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Patient
        public ActionResult Index()
        {
            var paitents = db.Patients.ToList();
            return View(paitents);
        }

        // GET: Patient/CreateEdit or Patient/CreateEdit/5
        public ActionResult CreateEdit(int? id , string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null) // Create mode
            {
                return View(new Patient());
            }

            // Edit mode
            var patient = db.Patients.Find(id);
            if (patient == null)
                return HttpNotFound();

            return View(patient);
        }

        // POST: Patient/CreateEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(Patient patient, HttpPostedFileBase ImageFile)
        {
           

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var folder = Server.MapPath("~/Uploads/Patients");


                    var path = Path.Combine(folder, fileName);
                    ImageFile.SaveAs(path);

                    // Save relative path
                    patient.PatientImage = "/Uploads/Patients/" + fileName;
                }

                if (patient.PatientId == 0) // New patient
                {
                    patient.CreatedAt = DateTime.Now;
                    db.Patients.Add(patient);
                }
                else // Existing patient
                {
                    Patient existingPatient = db.Patients.Find(patient.PatientId);
                    if (existingPatient == null)
                    {
                        return HttpNotFound();
                    }

                    existingPatient.FirstName = patient.FirstName;
                    existingPatient.LastName = patient.LastName;
                    existingPatient.DateOfBirth = patient.DateOfBirth;
                    existingPatient.Email = patient.Email;
                    existingPatient.PhoneNumber = patient.PhoneNumber;
                    existingPatient.Gender = patient.Gender;
                    existingPatient.Address = patient.Address;
                    existingPatient.BloodGroup = patient.BloodGroup;
                    existingPatient.Weight = patient.Weight;
                    existingPatient.Height = patient.Height;
                    existingPatient.MedicalHistory = patient.MedicalHistory;

                    if (!string.IsNullOrEmpty(patient.PatientImage))
                    {
                        existingPatient.PatientImage = patient.PatientImage;
                    }

                    db.Entry(existingPatient).State = System.Data.Entity.EntityState.Modified;

                   
                }

                 db.SaveChanges();

                // **Always redirect to returnUrl if provided**
                if (!string.IsNullOrEmpty(patient.returnUrl))
                {
                    var separator = patient.returnUrl.Contains("?") ? "&" : "?";
                    var url = patient.returnUrl + separator + "newPatientId=" + patient.PatientId;
                    return Redirect(url);
                }

                return RedirectToAction("Index");
            

            return View(patient);
        }

    }
}
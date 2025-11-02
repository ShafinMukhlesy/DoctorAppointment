using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    public class DepartmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Department
        public ActionResult Index()
        {
            var dept = db.Department.ToList();
            return View(dept);
        }

        public ActionResult Create()
        {
            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Department.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name");

            return View(department);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Department department = db.Department.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name", department.OrganizationId);

            return View(department);
        }

        [HttpPost]
        public ActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrganizationId = new SelectList(db.Organization, "OrganizationId", "Name", department.OrganizationId);

            return View(department);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    

    }
}
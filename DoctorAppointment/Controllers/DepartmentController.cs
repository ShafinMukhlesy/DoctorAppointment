using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
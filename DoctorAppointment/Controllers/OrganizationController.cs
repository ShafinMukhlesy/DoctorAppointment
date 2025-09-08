using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    public class OrganizationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Organization
        public ActionResult Index()
        {
            var org = db.Organization.ToList();
            return View(org);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                db.Organization.Add(organization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organization);
        }
    }
}
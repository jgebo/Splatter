using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Splatter.Models;


namespace Splatter.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Roles
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var roles = db.Roles.ToList(); 
            var model = new List<RolesViewModel>();

            foreach (var item in roles)
            {
                model.Add(new RolesViewModel { RoleId = item.Id, RoleName = item.Name });
            }
            return View(model);
        }

        // GET: Roles/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RoleName == null || (model.RoleName.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Role Name Required!");
                }

                if (ModelState.IsValid)
                {
                    var result = db.Roles.Add(new IdentityRole(model.RoleName));
                    if (result != null)
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            } 
            // if we got this far, something has gone wrong... return to Create View  
            return View(model);
        }

        // GET: Roles/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            var role = db.Roles.Find(id);
            var model = new RolesViewModel();
            model.RoleName = role.Name;
            model.RoleId = role.Id;
            return View(model);
        }

        // POST: Roles/Edit/5
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RoleName == null || (model.RoleName.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Role Name Required!");
                }

                if (ModelState.IsValid)
                {
                    var role = db.Roles.Find(model.RoleId);
                    // change role name to match whats in the model
                    role.Name = model.RoleName;
                    // tell the DB the role entry has been modified
                    db.Entry(role).State = EntityState.Modified;
                    // save the changes
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // GET: Roles/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            var role = db.Roles.Find(id);
            var model = new RolesViewModel();
            model.RoleName = role.Name;
            model.RoleId = role.Id;
            return View(model);
        }

        // POST: Roles/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(RolesViewModel model)
        {
            var role = db.Roles.Find(model.RoleId);
            // change role name to match whats in the model
            role.Name = model.RoleName;
            // tell the DB the role entry has been modified
            db.Roles.Remove(role);
            // save the changes
            db.SaveChanges();
            // redirect control back to the roles list view
            return RedirectToAction("Index");
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

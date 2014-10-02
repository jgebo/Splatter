using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Splatter.Models;

namespace Splatter.Controllers
{
    public class ProjectsController : Controller
    {
        static bool SortDirection;
        static string SortProperty;

        private BugTracker db = new BugTracker();

          // Sort: Projects
        [Authorize(Roles = "Administrator, Developer")]
         public ActionResult Sort(string property)
        {
            IEnumerable<Project> model;

            if (SortProperty == property)
            {
                // toggle direction
                SortDirection = !SortDirection;
            }
            else
                {
                    // initila direction (ascending)
                    SortDirection = false;
                }

            SortProperty = property;

            if (!SortDirection)
            {
                model = db.Projects.ToList().OrderBy(p => p.Name);
            }
            else
                {
                    model = db.Projects.ToList().OrderByDescending(p => p.Name);
                }
            TempData["model"] = model;
            return RedirectToAction("Index");
        }
               
        // GET: Projects
        [Authorize(Roles = "Administrator, Developer")]
        public ActionResult Index(IEnumerable<Project> model)
        {
              if (TempData["model"] != null)
            {
                model = (IEnumerable<Project>)TempData["model"];
                return View(model);
            }
           
            {
               return View(db.Projects.ToList().OrderBy( p => p.Name));
            }
        }
        //{
        //    // Need to emtpy tempdata for tickets view
        //    TempData["sort"] = null;

        //    return View(db.Projects.ToList().OrderBy( p => p.Name));
        //}

        // GET: Projects/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                if (project.Name == null || (project.Name.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Project Name Required!");
                }

                if (ModelState.IsValid)
                {
                    db.Projects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                if (project.Name == null || (project.Name.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Project Name Required!");
                }

                if (ModelState.IsValid)
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
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

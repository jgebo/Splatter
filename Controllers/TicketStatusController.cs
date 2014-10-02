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
    public class TicketStatusController : Controller
    {
        private BugTracker db = new BugTracker();

        // GET: TicketStatus
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(db.TicketStatuses.ToList());
        }

        // GET: TicketStatus/Details/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            if (ticketStatus == null)
            {
                return HttpNotFound();
            }
            return View(ticketStatus);
        }

        // GET: TicketStatus/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TicketStatus ticketStatus)
        {
            if (ModelState.IsValid)
            {
                 if (ticketStatus.Name == null || (ticketStatus.Name.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Status Name Required!");
                }

                if (ModelState.IsValid)
                {
                    db.TicketStatuses.Add(ticketStatus);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(ticketStatus);
        }

        // GET: TicketStatus/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            if (ticketStatus == null)
            {
                return HttpNotFound();
            }
            return View(ticketStatus);
        }

        // POST: TicketStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TicketStatus ticketStatus)
        {
            if (ModelState.IsValid)
            {
                if (ticketStatus.Name == null || (ticketStatus.Name.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Status Name Required!");
                }

                if (ModelState.IsValid)
                {
                    db.Entry(ticketStatus).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(ticketStatus);
        }

        // GET: TicketStatus/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            if (ticketStatus == null)
            {
                return HttpNotFound();
            }
            return View(ticketStatus);
        }

        // POST: TicketStatus/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketStatus ticketStatus = db.TicketStatuses.Find(id);
            db.TicketStatuses.Remove(ticketStatus);
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

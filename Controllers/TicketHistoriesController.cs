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
    public class TicketHistoriesController : Controller
    {
        private BugTracker db = new BugTracker();

        // GET: TicketHistories
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Index()
        {
            return View(db.TicketHistories.ToList().OrderByDescending(p => p.Changed));
        }


        // GET: TicketHistories/Details/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistory);
        }

        // GET: TicketHistories/Create
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Create()
        {
            ViewBag.UserName = new SelectList(db.BTUsers, "UserName", "FirstName");
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            return View();
        }

        // POST: TicketHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Property,OldValue,NewValue,Changed,UserName")] TicketHistory ticketHistory)
        {
            if (ModelState.IsValid)
            {
                db.TicketHistories.Add(ticketHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserName = new SelectList(db.BTUsers, "UserName", "FirstName", ticketHistory.UserName);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketHistory.TicketId);
            return View(ticketHistory);
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

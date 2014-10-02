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
    public class TicketCommentsController : Controller
    {
        private BugTracker db = new BugTracker();
                     
        // GET: TicketComments/Index 
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Index()
        {
            var ticketComments = db.TicketComments.Include(t => t.BTUser).Include(t => t.Ticket);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Create(int id)
        {

            var ticketComment = new TicketComment();
            ticketComment.TicketId = id;
            ticketComment.Ticket = db.Tickets.Find(id);
            ticketComment.Created = DateTimeOffset.Now;
            ticketComment.BTUser = db.BTUsers.Find(User.Identity.Name);
            ticketComment.UserName = User.Identity.Name;
            return View(ticketComment);
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Comment,Created,UserName")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                if (ticketComment.Comment == null || (ticketComment.Comment.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Comment Required!");
                    return View(ticketComment);
                }
                                               
                ticketComment.Created = DateTimeOffset.Now;
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Edit", "Tickets", new { id = ticketComment.TicketId });
            }

            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
          
            return View(ticketComment);
                        
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,Comment,Created,UserName")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                if (ticketComment.Comment == null || (ticketComment.Comment.Trim().Length == 0))
                {
                    ticketComment.Ticket = db.Tickets.Find(ticketComment.TicketId);
                    ModelState.AddModelError("", "Comment Required!");
                    return View(ticketComment);
                }
                
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", "Tickets", new { id = ticketComment.TicketId });
            }
            ViewBag.UserName = new SelectList(db.BTUsers, "UserName", "FirstName", ticketComment.UserName);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
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

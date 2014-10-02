using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Splatter.Models;
using System.IO;
namespace Splatter.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private BugTracker db = new BugTracker();

        // GET: TicketAttachments
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Index()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.BTUser).Include(t => t.Ticket);
            return View(ticketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ticketAttachment.FileUrl = ticketAttachment.FileUrl.Substring(14);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Create(int id)
        {
            var ticketAttachment = new TicketAttachment();
            ticketAttachment.TicketId = id;
            ticketAttachment.Ticket = db.Tickets.Find(id);
            ticketAttachment.Created = DateTimeOffset.Now;
            ticketAttachment.BTUser = db.BTUsers.Find(User.Identity.Name);
            ticketAttachment.UserName = User.Identity.Name;
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserName,FileUrl")] TicketAttachment ticketAttachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    ticketAttachment.FilePath = Path.Combine(Server.MapPath("~/Attachments/"), fileName);
                    file.SaveAs(ticketAttachment.FilePath);
                    ticketAttachment.FileUrl = "~/Attachments/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("", "Attachment Required!");
                }

                if (ticketAttachment.Description == null || (ticketAttachment.Description.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Description Required!");
                }
                if(!ModelState.IsValid)
                {
                    ticketAttachment.Ticket = db.Tickets.Find(ticketAttachment.TicketId);
                    return View(ticketAttachment);
                }
                ticketAttachment.Created = DateTimeOffset.Now;
                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();
                return RedirectToAction("Edit", "Tickets", new { id = ticketAttachment.TicketId });
            }
            
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }

            ticketAttachment.FileUrl = ticketAttachment.FileUrl.Substring(14);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserName,FileUrl")] TicketAttachment ticketAttachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (System.IO.File.Exists(ticketAttachment.FilePath))
                    {
                        System.IO.File.Delete(ticketAttachment.FilePath);
                    }
                    var fileName = Path.GetFileName(file.FileName);
                    ticketAttachment.FilePath = Path.Combine(Server.MapPath("~/Attachments/"), fileName);
                    file.SaveAs(ticketAttachment.FilePath);
                    ticketAttachment.FileUrl = "~/Attachments/" + fileName;
                }
                else
                {
                    ticketAttachment.FileUrl = "~/Attachments/" + ticketAttachment.FileUrl;
                }

                if (ticketAttachment.Description == null || (ticketAttachment.Description.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Description Required!");
                }
                if (!ModelState.IsValid)
                {
                   ticketAttachment.Ticket = db.Tickets.Find(ticketAttachment.TicketId);
                    return View(ticketAttachment);
                }

                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", "Tickets", new { id = ticketAttachment.TicketId });
            }
            ViewBag.UserName = new SelectList(db.BTUsers, "UserName", "FirstName", ticketAttachment.UserName);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
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

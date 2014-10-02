using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Splatter.Models;
using PagedList.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Splatter.Controllers
{
    public class TicketsController : Controller
    {
        private BugTracker db = new BugTracker();
        private ApplicationDbContext AppDb = new ApplicationDbContext();
        private static int ProjectId = 0; 

        static bool SortDirection;
        private static string SortProperty;
       
        private static IEnumerable<Ticket> tickets;

        // GET: Tickets
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Index(int? projectid, string param, string paramtype, int? page )
        {
            ViewBag.pid = ProjectId;
            if(projectid != null)
            {
                ProjectId = (int)projectid; 
            }

            if (User.IsInRole("Administrator"))
            {
                 tickets = db.Tickets.Include(t => t.BTUser)
                                    .Include(t => t.BTUser1)
                                    .Include(t => t.Project)
                                    .Include(t => t.TicketStatus)
                                    .Include(t => t.TicketType)
                                    .Include(t => t.TicketPriority);
            }
            else if (User.IsInRole("Developer"))
            {
                tickets = db.Tickets.Include(t => t.BTUser)
                                  .Include(t => t.BTUser1)
                                  .Include(t => t.Project)
                                  .Include(t => t.TicketStatus)
                                  .Include(t => t.TicketType)
                                  .Include(t => t.TicketPriority).Where(t => t.AssignedToUserName == User.Identity.Name);
            }
            else
            {
                tickets = db.Tickets.Include(t => t.BTUser)
                                    .Include(t => t.BTUser1)
                                    .Include(t => t.Project)
                                    .Include(t => t.TicketStatus)
                                    .Include(t => t.TicketType)
                                    .Include(t => t.TicketPriority).Where(t => t.OwnerUserName == User.Identity.Name);
            }
                                           
            if (projectid > 0)
            {
                ViewBag.pid = ProjectId;
                ViewBag.Title = "Tickets:" + db.Projects.Find(ProjectId).Name;
                tickets = tickets.Where(t => t.ProjectId == ProjectId);
            }
            else
            {
                ViewBag.Title ="Title";
            }

            switch (paramtype)
            {
                case "Owner":
                    tickets = tickets.Where(t => t.BTUser1.DisplayName == param);
                    break;
                case "Assigned":
                    tickets = tickets.Where(t => t.BTUser.DisplayName == param);
                    break;
                case "Project":
                    tickets = tickets.Where(t => t.Project.Name == param);
                    break;
                case "Priority":
                    tickets = tickets.Where(t => t.TicketPriority.Name == param);
                    break;
                case "Status":
                    tickets = tickets.Where(t => t.TicketStatus.Name == param);
                    break;
                case "Type":
                    tickets = tickets.Where(t => t.TicketType.Name == param);
                    break;
                default:
                    break;
            }

             switch (SortProperty)
            {
                case "Title":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.Title).ThenBy(t => t.Updated);
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.Title).ThenByDescending(t => t.Updated);
                    }
                    break;
                case "Last Updated":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.Updated);
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.Updated);
                    }
                    break;
                case "Owner":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.BTUser1.DisplayName).ThenBy(t => t.Updated);
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.BTUser1.DisplayName).ThenByDescending(t => t.Updated);
                    }
                    break;
                case "Assigned":
                    if (!SortDirection)
                    {
                          
                        tickets = tickets.OrderBy(t => t.BTUser.DisplayName).ThenBy(t => t.Updated);
                    }
                    else
                    {
                         tickets = tickets.OrderByDescending(t => t.BTUser.DisplayName).ThenByDescending(t => t.Updated);
                    }
                    break;
                case "Project":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.Project.Name).ThenBy(t => t.Updated);
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.Project.Name).ThenByDescending(t => t.Updated);
                    }
                    break;
                case "Priority":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.TicketPriority.Name).ThenBy(t => t.Updated); 
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.TicketPriority.Name).ThenByDescending(t => t.Updated); 
                    }
                    break;
                case "Status":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.TicketStatus.Name).ThenBy(t => t.Updated); 
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.TicketStatus.Name).ThenByDescending(t => t.Updated); 
                    }
                    break;
                case "Type":
                    if (!SortDirection)
                    {
                        tickets = tickets.OrderBy(t => t.TicketType.Name).ThenBy(t => t.Updated); 
                    }
                    else
                    {
                        tickets = tickets.OrderByDescending(t => t.TicketType.Name).ThenByDescending(t => t.Updated); 
                    }
                    break;
                default:
                    {
                        tickets = tickets.OrderBy(t => t.Title).ThenBy(t => t.Updated); 
                    }
                    break;
             }     

            int pagesize = 10;
            int pagenumber = page ?? 1;
           
            return View(tickets.ToPagedList(pagenumber, pagesize));
        }
      
        // GET: Tickets/Details/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.pid = ProjectId;
            ViewBag.tid = ticket.Id;

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            Ticket model = new Ticket();
            DateTimeOffset date = DateTimeOffset.Now;
            model.Created = date;
            model.Updated = date;
            ViewBag.pid = ProjectId;
            model.OwnerUserName = User.Identity.Name;

            //Get Developers 
            List<BTUser> devlist = new List<BTUser>();
            foreach (var user in AppDb.Users)
            {
                if (user.IsInRole("Developer"))
                {
                   devlist.Add(db.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.Id));
                }
            }

            ViewBag.AssignedToUserName = new SelectList(devlist, "UserName", "FirstName");

           
            //Get Submitters Projects//
            List<ProjectUser> projectList = new List<ProjectUser>();
            List<Project> userProjects = new List<Project>();
            projectList = db.ProjectUsers.Where(o => o.UserName == User.Identity.Name).ToList();
             
            // loop through the projects to find user projects 

            foreach (var up in projectList)
            {
                userProjects.Add(db.Projects.FirstOrDefault(u => u.Id  ==  up.ProjectId));
            }
            ViewBag.ProjectId = new SelectList(userProjects, "Id", "Name");
            //-----------------------//
                                  
            //ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");           
            //ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "FirstName");
            //ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "FirstName");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserName,AssignedToUserName")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket.Title == null || (ticket.Title.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Title Required!");
                }
                if (ticket.Description == null || (ticket.Description.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Description Required!");
                }
                           
                {
                    if(!ModelState.IsValid)
                    {
                        ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "FirstName");
                        ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "FirstName");
                        ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
                        ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
                        ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                        ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                        return View(ticket);
                    }
                }

                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            DateTimeOffset date = DateTimeOffset.Now;
            ticket.Updated = date;
            ViewBag.pid = ProjectId;

            //Get Submitters and developer lists//
            List<BTUser> devlist = new List<BTUser>();
            List<BTUser> userlist = new List<BTUser>();
            foreach (var user in AppDb.Users)
            {
                if (user.IsInRole("Developer"))
                {
                    devlist.Add(db.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.Id));
                }
                if (user.IsInRole("Submitter"))
                {
                    userlist.Add(db.BTUsers.FirstOrDefault(u => u.AspNetUserId == user.Id));
                }
            }

            //Get Submitters Projects//
            List<ProjectUser> projectList = new List<ProjectUser>();
            List<Project> userProjects = new List<Project>();
            projectList = db.ProjectUsers.Where(o => o.UserName == User.Identity.Name).ToList();
            // loop through the projects to find user projects 
            foreach (var up in projectList)
            {
                userProjects.Add(db.Projects.FirstOrDefault(u => u.Id == up.ProjectId));
            }

            ViewBag.ProjectId = new SelectList(userProjects, "Id", "Name");
            ViewBag.AssignedToUserName = new SelectList(devlist, "UserName", "FirstName", ticket.AssignedToUserName);
            ViewBag.OwnerUserName = new SelectList(userlist, "UserName", "FirstName", ticket.OwnerUserName);
            //ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "FirstName", ticket.AssignedToUserName);
            //ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "FirstName", ticket.OwnerUserName);
            //ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            TempData["OldTicket"] = ticket;    
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserName,AssignedToUserName")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket.Description == null || (ticket.Description.Trim().Length == 0))
                {
                    ModelState.AddModelError("", "Description Required!");
                    ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "FirstName");
                    ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "FirstName");
                    ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
                    ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
                    ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                    ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                    return View(ticket);
                }

                var oldTicket = (Ticket)TempData["OldTicket"];

                if (oldTicket.Description != ticket.Description)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Description",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.Description,
                        NewValue = ticket.Description
                    });
                }
                if (oldTicket.ProjectId != ticket.ProjectId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Project",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.Project.Name,
                        NewValue = db.Projects.Find(ticket.ProjectId).Name
                    });
                }
                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                   
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Status",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = db.TicketStatuses.Find(ticket.TicketStatusId).Name
                    });
                }
                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Type",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name
                    });
                }
                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Priority",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name
                    });
                }
                if (oldTicket.AssignedToUserName != ticket.AssignedToUserName)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Assigned To",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.AssignedToUserName,
                        NewValue = ticket.AssignedToUserName
                    });
                }
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }    

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [Authorize(Roles = "Submitter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Sort: 
        [Authorize(Roles = "Administrator, Developer")]
         public ActionResult Sort(string property, IEnumerable<Ticket> model)
        {
            tickets = model;

            if (SortProperty == property)
            {
                // toggle direction
                SortDirection = !SortDirection;
            }
            else
                {
                    // initial direction (ascending)
                    SortDirection = false;
                }

            SortProperty = property;
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
